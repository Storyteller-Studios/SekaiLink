using System;
using System.Threading;
using System.Threading.Tasks;

namespace SekaiLink.Protocols.Transport
{
    public enum TransportKind { Unknown, BleGatt, Rfcomm, Spp, Mock }
    public enum TransportState { Disconnected, Connecting, Ready, Disconnecting, Faulted }
    public sealed class TransportEndpoint
    {
        public TransportEndpoint(TransportKind kind, string address, string? serviceId = null, string? writeCharacteristicId = null, string? notifyCharacteristicId = null)
        { Kind = kind; Address = address ?? throw new ArgumentNullException(nameof(address)); ServiceId = serviceId; WriteCharacteristicId = writeCharacteristicId; NotifyCharacteristicId = notifyCharacteristicId; }
        public TransportKind Kind { get; }
        public string Address { get; }
        public string? ServiceId { get; }
        public string? WriteCharacteristicId { get; }
        public string? NotifyCharacteristicId { get; }
    }
    public sealed class TransportDataReceivedEventArgs : EventArgs
    {
        public TransportDataReceivedEventArgs(byte[] data, string? source = null, DateTimeOffset? timestamp = null)
        { Data = data ?? throw new ArgumentNullException(nameof(data)); Source = source; Timestamp = timestamp ?? DateTimeOffset.UtcNow; }
        public byte[] Data { get; }
        public string? Source { get; }
        public DateTimeOffset Timestamp { get; }
    }
    public sealed class TransportStateChangedEventArgs : EventArgs
    { public TransportStateChangedEventArgs(TransportState state, Exception? error = null) { State = state; Error = error; } public TransportState State { get; } public Exception? Error { get; } }
    public interface IProtocolTransport : IAsyncDisposable
    {
        TransportKind Kind { get; }
        TransportState State { get; }
        int? MaximumWriteLength { get; }
        event EventHandler<TransportDataReceivedEventArgs>? DataReceived;
        event EventHandler<TransportStateChangedEventArgs>? StateChanged;
        Task ConnectAsync(TransportEndpoint endpoint, CancellationToken cancellationToken = default);
        Task DisconnectAsync(CancellationToken cancellationToken = default);
        Task SendAsync(byte[] data, CancellationToken cancellationToken = default);
    }
    public interface ITransportFactory { IProtocolTransport Create(TransportEndpoint endpoint); }
}
