using System;
using System.Collections.Generic;
using System.Linq;
using SekaiLink.Oppo.Devices;
using SekaiLink.Oppo.Protocol.Protocols;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Routing;
using SekaiLink.Protocols.Transport;
using SekaiLink.Rose.Devices;

namespace SekaiLink.Router
{
    public sealed class DeviceCatalogEntry
    {
        public DeviceCatalogEntry(string id, string displayName, string brand, string family, bool isSupported)
        { Id = id; DisplayName = displayName; Brand = brand; Family = family; IsSupported = isSupported; }
        public string Id { get; }
        public string DisplayName { get; }
        public string Brand { get; }
        public string Family { get; }
        public bool IsSupported { get; }
    }

    /// <summary>Brand catalog access for clients that do not own protocol details.</summary>
    public static class DeviceCatalog
    {
        private const string GattWrite = "0000079B-D102-11E1-9B23-00025B00A5A5";
        private const string GattNotify = "0000079C-D102-11E1-9B23-00025B00A5A5";

        private static readonly IReadOnlyList<DeviceCatalogEntry> Entries = Array.AsReadOnly(
            RoseDeviceCatalog.All.Select(d => new DeviceCatalogEntry(d.Id, d.DisplayName, "RoseElsa", d.ProtocolFamily.ToString(), true))
                .Concat(OppoDeviceCatalog.All.Select(d => new DeviceCatalogEntry(d.Id, d.DisplayName, d.Brand,
                    d.IsSupported ? "OPPO" : "unsupported", d.IsSupported))).ToArray());

        public static IReadOnlyList<DeviceCatalogEntry> All => Entries;

        public static DeviceCatalogEntry? Find(string id) =>
            Entries.FirstOrDefault(d => d.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

        public static bool IsOppo(DeviceRoute route) => route.Definition is OppoDeviceDefinition;
        public static bool PrefersClassic(string id) => OppoDeviceCatalog.Find(id) != null;
        public static bool PrefersClassic(DeviceRoute route) => route.Definition is OppoDeviceDefinition;

        public static DeviceProfile? CreateProfile(DeviceRoute route) => route.Definition switch
        {
            RoseDeviceDefinition rose => rose.CreateProfile(route.Endpoint),
            OppoDeviceDefinition oppo => oppo.CreateProfile(route.Endpoint),
            _ => null
        };

        public static DeviceRoute RefineByProductId(DeviceRoute route, string productId)
        {
            var exact = OppoDeviceCatalog.Find(productId);
            if (exact == null || route.Definition is not OppoDeviceDefinition) return route;
            if (!exact.IsSupported) throw new NotSupportedException($"Product ID {productId} does not support this OPPO protocol.");
            if (exact.Id == route.DeviceId) return route;
            return new DeviceRoute(exact.Brand, exact.Id, exact.DisplayName, route.ProtocolFamily,
                route.Endpoint, 1200, exact);
        }

        public static DeviceRoute CreateRoute(string id, string address, string? transport = null)
        {
            var rose = RoseDeviceCatalog.Find(id);
            if (rose != null)
            {
                if (transport != null && !transport.Equals("gatt", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("This model only supports a GATT endpoint.", nameof(transport));
                var gatt = rose.GattConfiguration ?? throw new NotSupportedException($"{rose.DisplayName} has no GATT configuration.");
                var endpoint = new TransportEndpoint(TransportKind.BleGatt, address, gatt.ServiceUuid.ToString("D"),
                    gatt.WriteUuid.ToString("D"), gatt.NotifyUuid.ToString("D"));
                return new DeviceRoute("RoseElsa", rose.Id, rose.DisplayName, rose.ProtocolFamily, endpoint, 1000, rose);
            }

            var oppo = OppoDeviceCatalog.Find(id);
            if (oppo == null && OppoDeviceCatalog.TryResolve(new DeviceIdentity { Name = id }, out var byName)) oppo = byName;
            if (oppo == null) throw new ArgumentException($"Unknown or ambiguous model '{id}'.", nameof(id));
            if (!oppo.IsSupported) throw new NotSupportedException($"{oppo.DisplayName} does not support this OPPO protocol.");
            if (transport != null && !transport.Equals("gatt", StringComparison.OrdinalIgnoreCase) &&
                !transport.Equals("rfcomm", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Transport must be gatt or rfcomm.", nameof(transport));
            var ble = transport?.Equals("gatt", StringComparison.OrdinalIgnoreCase) == true;
            var endpointOppo = ble
                ? new TransportEndpoint(TransportKind.BleGatt, address, OppoCommands.SppServiceUuid.ToString("D"), GattWrite, GattNotify)
                : new TransportEndpoint(TransportKind.Rfcomm, address, OppoCommands.SppServiceUuid.ToString("D"));
            return new DeviceRoute(oppo.Brand, oppo.Id, oppo.DisplayName,
                ble ? ProtocolFamily.OppoGatt : ProtocolFamily.OppoSpp, endpointOppo, 1000, oppo);
        }
    }
}
