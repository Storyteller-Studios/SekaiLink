using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Routing;
using SekaiLink.Protocols.Transport;
using SekaiLink.Rose.Devices;

namespace SekaiLink.Router.Brands;

[DeviceRouteProvider]
internal sealed class RoseDeviceRouteProvider : IDeviceRouteProvider
{
    public string Brand => "RoseElsa";

    public bool TryResolve(DeviceIdentity identity, TransportEndpoint discoveredEndpoint, out DeviceRoute? route)
    {
        if (!RoseDeviceCatalog.TryResolve(identity, out var definition) || definition == null)
        {
            route = null;
            return false;
        }

        var endpoint = CreateEndpoint(identity, discoveredEndpoint, definition);
        route = new DeviceRoute(
            Brand,
            definition.Id,
            definition.DisplayName,
            definition.ProtocolFamily,
            endpoint,
            1000,
            definition);
        return true;
    }

    private static TransportEndpoint CreateEndpoint(
        DeviceIdentity identity,
        TransportEndpoint discoveredEndpoint,
        RoseDeviceDefinition definition)
    {
        var address = identity.Address ?? discoveredEndpoint.Address;
        if (definition.GattConfiguration is not { } gatt)
            return new TransportEndpoint(discoveredEndpoint.Kind, address, discoveredEndpoint.ServiceId,
                discoveredEndpoint.WriteCharacteristicId, discoveredEndpoint.NotifyCharacteristicId);
        return new TransportEndpoint(
            TransportKind.BleGatt,
            address,
            gatt.ServiceUuid.ToString("D"),
            gatt.WriteUuid.ToString("D"),
            gatt.NotifyUuid.ToString("D"));
    }
}