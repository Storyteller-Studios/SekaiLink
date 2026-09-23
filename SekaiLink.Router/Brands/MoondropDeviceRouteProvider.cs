using System;
using System.Linq;
using SekaiLink.Moondrop.Protocol.Protocols;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Routing;
using SekaiLink.Protocols.Transport;

namespace SekaiLink.Router.Brands
{
    [DeviceRouteProvider]
    internal sealed class MoondropDeviceRouteProvider : IDeviceRouteProvider
    {
        public string Brand => "Moondrop";

        public bool TryResolve(DeviceIdentity identity, TransportEndpoint discoveredEndpoint, out DeviceRoute? route)
        {
            if (HasService(identity, MoondropCommands.ServiceUuid))
            {
                var endpoint = BleEndpoint(identity, discoveredEndpoint,
                    MoondropCommands.ServiceUuid,
                    MoondropCommands.CommandCharacteristicUuid,
                    MoondropCommands.NotificationCharacteristicUuid);
                route = new DeviceRoute(Brand, "moondrop-a5", "Moondrop A5 device",
                    ProtocolFamily.MoondropCustomBle, endpoint, 700, ProtocolFamily.MoondropCustomBle);
                return true;
            }
            if (HasService(identity, MoondropCommands.GaiaServiceUuid))
            {
                var endpoint = BleEndpoint(identity, discoveredEndpoint,
                    MoondropCommands.GaiaServiceUuid,
                    MoondropCommands.GaiaCommandCharacteristicUuid,
                    MoondropCommands.GaiaResponseCharacteristicUuid);
                route = new DeviceRoute(Brand, "moondrop-gaia", "Moondrop GAIA device",
                    ProtocolFamily.MoondropGaia, endpoint, 700, ProtocolFamily.MoondropGaia);
                return true;
            }
            route = null;
            return false;
        }

        private static bool HasService(DeviceIdentity identity, Guid service) =>
            identity.ServiceIds.Any(value => Guid.TryParse(value, out var parsed) && parsed == service);

        private static TransportEndpoint BleEndpoint(DeviceIdentity identity, TransportEndpoint discovered, Guid service, Guid write, Guid notify) =>
            new TransportEndpoint(TransportKind.BleGatt, identity.Address ?? discovered.Address,
                service.ToString("D"), write.ToString("D"), notify.ToString("D"));
    }
}
