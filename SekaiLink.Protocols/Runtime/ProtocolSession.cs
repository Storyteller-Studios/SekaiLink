using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Transport;

namespace SekaiLink.Protocols.Runtime
{
    public enum ProtocolSessionState { Disconnected, Connecting, TransportReady, Initializing, Ready, Faulted }
    public sealed class ProtocolMessageEventArgs : EventArgs
    {
        public ProtocolMessageEventArgs(byte[] payload, bool isNotification, string? source = null)
        { Payload = payload; IsNotification = isNotification; Source = source; }
        public byte[] Payload { get; }
        public bool IsNotification { get; }
        public string? Source { get; }
    }
    public sealed class ProtocolSessionStateChangedEventArgs : EventArgs
    { public ProtocolSessionStateChangedEventArgs(ProtocolSessionState state) { State = state; } public ProtocolSessionState State { get; } }
    public interface IProtocolMessageMatcher
    {
        bool IsMatch(byte[] response, object request);
    }
    public interface IProtocolSession : IAsyncDisposable
    {
        ProtocolSessionState State { get; }
        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync(CancellationToken cancellationToken = default);
        Task<OperationResult<TResponse>> ExecuteAsync<TResponse>(object request, Func<object, byte[]> encode, Func<byte[], TResponse?> decode, TimeSpan timeout, IProtocolMessageMatcher? matcher = null, CancellationToken cancellationToken = default);
        event EventHandler<ProtocolMessageEventArgs>? NotificationReceived;
        event EventHandler<ProtocolSessionStateChangedEventArgs>? StateChanged;
    }

    /// A small, protocol-neutral session coordinator. Brand implementations supply encoding and matching.
    public sealed class ProtocolSession : IProtocolSession
    {
        private readonly IProtocolTransport _transport;
        private readonly SemaphoreSlim _requestLock = new SemaphoreSlim(1, 1);
        private sealed class PendingRequest
        {
            public PendingRequest(TaskCompletionSource<byte[]> completion, object request, IProtocolMessageMatcher? matcher)
            { Completion = completion; Request = request; Matcher = matcher; }
            public TaskCompletionSource<byte[]> Completion { get; }
            public object Request { get; }
            public IProtocolMessageMatcher? Matcher { get; }
        }
        private readonly ConcurrentDictionary<int, PendingRequest> _pending = new ConcurrentDictionary<int, PendingRequest>();
        private int _generation;
        private bool _disposed;
        public ProtocolSession(IProtocolTransport transport, TransportEndpoint? endpoint = null) { _transport = transport ?? throw new ArgumentNullException(nameof(transport)); Endpoint = endpoint; _transport.DataReceived += OnDataReceived; _transport.StateChanged += OnTransportStateChanged; }
        public TransportEndpoint? Endpoint { get; }
        public ProtocolSessionState State { get; private set; } = ProtocolSessionState.Disconnected;
        public event EventHandler<ProtocolMessageEventArgs>? NotificationReceived;
        public event EventHandler<ProtocolSessionStateChangedEventArgs>? StateChanged;
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (State == ProtocolSessionState.Ready || State == ProtocolSessionState.Initializing) return;
            SetState(ProtocolSessionState.Connecting);
            try
            {
                if (Endpoint != null) await _transport.ConnectAsync(Endpoint, cancellationToken).ConfigureAwait(false);
                SetState(ProtocolSessionState.TransportReady); SetState(ProtocolSessionState.Initializing); SetState(ProtocolSessionState.Ready);
            }
            catch { SetState(ProtocolSessionState.Faulted); throw; }
        }
        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed) return;
            Interlocked.Increment(ref _generation);
            foreach (var p in _pending.Values) p.Completion.TrySetCanceled();
            _pending.Clear();
            await _transport.DisconnectAsync(cancellationToken).ConfigureAwait(false);
            SetState(ProtocolSessionState.Disconnected);
        }
        public async Task<OperationResult<TResponse>> ExecuteAsync<TResponse>(object request, Func<object, byte[]> encode, Func<byte[], TResponse?> decode, TimeSpan timeout, IProtocolMessageMatcher? matcher = null, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (State != ProtocolSessionState.Ready) return OperationResult<TResponse>.Failure(OperationErrorKind.Disconnected, "The session is not ready.");
            if (request == null || encode == null || decode == null) return OperationResult<TResponse>.Failure(OperationErrorKind.InvalidParameter);
            await _requestLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            var generation = _generation;
            var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
            var key = RuntimeHelpers.GetHashCode(tcs);
            _pending[key] = new PendingRequest(tcs, request, matcher);
            try
            {
                await _transport.SendAsync(encode(request), cancellationToken).ConfigureAwait(false);
                using (var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    timeoutCts.CancelAfter(timeout <= TimeSpan.Zero ? TimeSpan.FromSeconds(5) : timeout);
                    timeoutCts.Token.Register(() => tcs.TrySetCanceled());
                    try
                    {
                        var response = await tcs.Task.ConfigureAwait(false);
                        if (generation != _generation) return OperationResult<TResponse>.Failure(OperationErrorKind.Disconnected);
                        try { return OperationResult<TResponse>.Success(decode(response), ConfirmationLevel.Applied, response); }
                        catch { return OperationResult<TResponse>.Failure(OperationErrorKind.InvalidResponse, "Response decoding failed.", rawResponse: response); }
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { return OperationResult<TResponse>.Cancelled(); }
                    catch (OperationCanceledException) { return OperationResult<TResponse>.Failure(OperationErrorKind.Timeout); }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { return OperationResult<TResponse>.Cancelled(); }
            catch (Exception ex) { return OperationResult<TResponse>.Failure(OperationErrorKind.TransportError, ex.Message); }
            finally { _pending.TryRemove(key, out _); _requestLock.Release(); }
        }
        private void OnDataReceived(object? sender, TransportDataReceivedEventArgs e)
        {
            // A transport notification is deliberately not assumed to be a response.
            foreach (var pending in _pending.Values)
            {
                if (pending.Matcher == null || pending.Matcher.IsMatch(e.Data, pending.Request))
                {
                    if (pending.Completion.TrySetResult(e.Data)) return;
                }
            }
            NotificationReceived?.Invoke(this, new ProtocolMessageEventArgs(e.Data, true, e.Source));
        }
        private void OnTransportStateChanged(object? sender, TransportStateChangedEventArgs e) { if (e.State == TransportState.Disconnected) SetState(ProtocolSessionState.Disconnected); }
        private void SetState(ProtocolSessionState value) { State = value; StateChanged?.Invoke(this, new ProtocolSessionStateChangedEventArgs(value)); }
        private void ThrowIfDisposed() { if (_disposed) throw new ObjectDisposedException(nameof(ProtocolSession)); }
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            await DisconnectAsync().ConfigureAwait(false);
            _disposed = true;
            _transport.DataReceived -= OnDataReceived; _transport.StateChanged -= OnTransportStateChanged;
            _requestLock.Dispose(); await _transport.DisposeAsync().ConfigureAwait(false);
        }
    }
}
