using System;
using System.Linq;
using SekaiLink.Oppo.Devices;
using SekaiLink.Oppo.Protocol.Protocols;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Routing;
using SekaiLink.Protocols.Transport;

namespace SekaiLink.Router.Brands;

[DeviceRouteProvider]
internal sealed class OppoDeviceRouteProvider : IDeviceRouteProvider
{
    private const string GattWrite = "0000079B-D102-11E1-9B23-00025B00A5A5";
    private const string GattNotify = "0000079C-D102-11E1-9B23-00025B00A5A5";
    public string Brand => "OPPO";

    public bool TryResolve(DeviceIdentity identity, TransportEndpoint discoveredEndpoint, out DeviceRoute? route)
    {
        if (OppoDeviceCatalog.TryResolve(identity, out var definition) && definition != null)
        {
            if (!definition.IsSupported)
            {
                route = null;
                return false;
            }

            var modelEndpoint = CreateEndpoint(identity, discoveredEndpoint);
            route = new DeviceRoute(definition.Brand, definition.Id, definition.DisplayName,
                modelEndpoint.Kind == TransportKind.BleGatt ? ProtocolFamily.OppoGatt : ProtocolFamily.OppoSpp,
                modelEndpoint, identity.ProductId == null ? 1000 : 1200, definition);
            return true;
        }

        // Keep the prior generic route for an explicitly advertised Melody service.
        if (!string.IsNullOrWhiteSpace(identity.ProductId) ||
            !identity.ServiceIds.Any(value =>
                Guid.TryParse(value, out var service) && service == OppoCommands.SppServiceUuid))
        {
            route = null;
            return false;
        }

        var endpoint = CreateEndpoint(identity, discoveredEndpoint);
        route = new DeviceRoute(Brand, "oppo-spp", "OPPO-compatible device",
            endpoint.Kind == TransportKind.BleGatt ? ProtocolFamily.OppoGatt : ProtocolFamily.OppoSpp,
            endpoint, 700, ProtocolFamily.OppoSpp);
        return true;
    }

    private static TransportEndpoint CreateEndpoint(DeviceIdentity identity, TransportEndpoint discovered)
    {
        var address = identity.Address ?? discovered.Address;
        if (discovered.Kind == TransportKind.BleGatt)
            return new TransportEndpoint(TransportKind.BleGatt, address,
                OppoCommands.SppServiceUuid.ToString("D"), GattWrite, GattNotify);
        return new TransportEndpoint(discovered.Kind, address, OppoCommands.SppServiceUuid.ToString("D"));
    }
}