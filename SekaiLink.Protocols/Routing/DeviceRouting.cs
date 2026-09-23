using System;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Transport;

namespace SekaiLink.Protocols.Routing
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DeviceRouteProviderAttribute : Attribute
    {
    }

    public interface IDeviceRouteProvider
    {
        string Brand { get; }

        bool TryResolve(
            DeviceIdentity identity,
            TransportEndpoint discoveredEndpoint,
            out DeviceRoute? route);
    }

    public sealed class DeviceRoute
    {
        public DeviceRoute(
            string brand,
            string deviceId,
            string displayName,
            ProtocolFamily protocolFamily,
            TransportEndpoint endpoint,
            int score,
            object definition)
        {
            if (string.IsNullOrWhiteSpace(brand)) throw new ArgumentException("Brand is required.", nameof(brand));
            if (string.IsNullOrWhiteSpace(deviceId)) throw new ArgumentException("Device ID is required.", nameof(deviceId));
            if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("Display name is required.", nameof(displayName));
            if (score <= 0) throw new ArgumentOutOfRangeException(nameof(score));

            Brand = brand;
            DeviceId = deviceId;
            DisplayName = displayName;
            ProtocolFamily = protocolFamily;
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            Score = score;
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public string Brand { get; }
        public string DeviceId { get; }
        public string DisplayName { get; }
        public ProtocolFamily ProtocolFamily { get; }
        public TransportEndpoint Endpoint { get; }
        public int Score { get; }

        /// <summary>The immutable brand-owned device definition selected by the provider.</summary>
        public object Definition { get; }

        public bool TryGetDefinition<TDefinition>(out TDefinition? definition)
            where TDefinition : class
        {
            definition = Definition as TDefinition;
            return definition != null;
        }
    }
}
