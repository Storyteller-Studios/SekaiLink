using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SekaiLink.Protocols.Transport
{
    /// Deterministic transport for offline protocol and scheduler tests.
    public sealed class InMemoryTransport : IProtocolTransport
    {
        private bool _disposed;
        public InMemoryTransport(TransportKind kind = TransportKind.Mock, int? maximumWriteLength = null) { Kind = kind; MaximumWriteLength = maximumWriteLength; }
        public TransportKind Kind { get; }
        public TransportState State { get; private set; } = TransportState.Disconnected;
        public int? MaximumWriteLength { get; }
        public List<byte[]> Sent { get; } = new List<byte[]>();
        public event EventHandler<TransportDataReceivedEventArgs>? DataReceived;
        public event EventHandler<TransportStateChangedEventArgs>? StateChanged;
        public Task ConnectAsync(TransportEndpoint endpoint, CancellationToken cancellationToken = default)
        { ThrowIfDisposed(); SetState(TransportState.Connecting); SetState(TransportState.Ready); return Task.CompletedTask; }
        public Task DisconnectAsync(CancellationToken cancellationToken = default)
        { if (!_disposed) SetState(TransportState.Disconnected); return Task.CompletedTask; }
        public Task SendAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed(); if (State != TransportState.Ready) throw new InvalidOperationException("Transport is not ready.");
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (MaximumWriteLength.HasValue && data.Length > MaximumWriteLength.Value) throw new ArgumentException("Payload exceeds the transport write limit.", nameof(data));
            Sent.Add((byte[])data.Clone()); return Task.CompletedTask;
        }
        public void InjectReceive(byte[] data, string? source = null) { ThrowIfDisposed(); DataReceived?.Invoke(this, new TransportDataReceivedEventArgs((byte[])data.Clone(), source)); }
        private void SetState(TransportState state) { State = state; StateChanged?.Invoke(this, new TransportStateChangedEventArgs(state)); }
        private void ThrowIfDisposed() { if (_disposed) throw new ObjectDisposedException(nameof(InMemoryTransport)); }
        public ValueTask DisposeAsync() { if (!_disposed) { _disposed = true; SetState(TransportState.Disconnected); } return default; }
    }
}
