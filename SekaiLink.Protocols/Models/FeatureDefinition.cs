using System;

namespace SekaiLink.Protocols.Models
{
    /// Describes a feature using a stable semantic identifier. Wire bytes stay in the vendor protocol assembly.
    public sealed class FeatureDefinition
    {
        public FeatureDefinition(string identifier, object? value = null, CapabilityDescriptor? constraints = null)
        {
            if (string.IsNullOrWhiteSpace(identifier)) throw new ArgumentException("Feature identifier is required.", nameof(identifier));
            Identifier = identifier; Value = value; Constraints = constraints;
        }
        public string Identifier { get; }
        public object? Value { get; }
        public CapabilityDescriptor? Constraints { get; }
    }
}
