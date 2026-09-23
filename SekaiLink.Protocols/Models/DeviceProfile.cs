using System;
using System.Collections.Generic;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Transport;

namespace SekaiLink.Protocols.Models
{
    public enum ProtocolFamily { Unknown, OppoGatt, OppoSpp, MoondropCustomBle, MoondropGaia, MoondropFactorySpp, RoseF8, RoseRsCommon, RoseRsCommonV2, RoseZt }
    public sealed class DeviceIdentity
    {
        public string? Name { get; init; }
        public string? Address { get; init; }
        public string? Model { get; init; }
        public string? ProductId { get; init; }
        public string? Firmware { get; init; }
        public IReadOnlyDictionary<string, string> AdvertisementData { get; init; } = new Dictionary<string, string>();
        public IReadOnlyList<string> ServiceIds { get; init; } = Array.Empty<string>();
    }
    public sealed class DeviceProfile
    {
        public DeviceProfile(string id, ProtocolFamily protocolFamily, TransportEndpoint endpoint)
        { Id = id ?? throw new ArgumentNullException(nameof(id)); ProtocolFamily = protocolFamily; Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint)); }
        public string Id { get; }
        public ProtocolFamily ProtocolFamily { get; }
        public TransportEndpoint Endpoint { get; }
        public IReadOnlyList<CapabilityDescriptor> Capabilities { get; init; } = Array.Empty<CapabilityDescriptor>();
        public IReadOnlyList<Type> FeatureTypes { get; init; } = Array.Empty<Type>();
        public bool Matches(DeviceIdentity identity)
        {
            if (identity == null) return false;
            if (!string.IsNullOrEmpty(identity.Model) && identity.Model.Equals(Id, StringComparison.OrdinalIgnoreCase)) return true;
            return !string.IsNullOrEmpty(identity.Name) && identity.Name.IndexOf(Id, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
