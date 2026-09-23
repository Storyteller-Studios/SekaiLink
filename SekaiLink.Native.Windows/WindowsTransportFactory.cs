using System;
using SekaiLink.Protocols.Transport;

namespace SekaiLink.Native.Windows;

/// <summary>Creates Windows Bluetooth transports for protocol endpoints.</summary>
public sealed class WindowsTransportFactory : ITransportFactory
{
    public IProtocolTransport Create(TransportEndpoint endpoint)
    {
        ArgumentNullException.ThrowIfNull(endpoint);

        return endpoint.Kind switch
        {
            TransportKind.BleGatt => new BleGattTransport(),
            TransportKind.Rfcomm or TransportKind.Spp => new RfcommTransport(endpoint.Kind),
            _ => throw new NotSupportedException($"Windows does not support transport kind '{endpoint.Kind}'.")
        };
    }
}
