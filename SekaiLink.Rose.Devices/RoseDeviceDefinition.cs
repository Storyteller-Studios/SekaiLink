using System;
using System.Collections.Generic;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Transport;
using SekaiLink.Rose.Protocol.Protocols;

namespace SekaiLink.Rose.Devices
{
    /// Values and ordering recovered from RoseLink 3.8.0's RDeviceType enum.
    public enum RoseDeviceType
    {
        Unknown = 0,
        RsCommon = 1,
        RsCommonV2 = 2,
        Ceramics = 3,
        CeramicsX = 4,
        CeramicsX7034 = 5,
        CeramicsMk2 = 6,
        CeramicsMk28912F = 7,
        CeramicsMk25736 = 8,
        CeramicsU = 9,
        CeramicsUHsc2710 = 10,
        Agate = 11,
        AgateX = 12,
        Explorer = 13,
        EarfreeI5 = 14,
        Headfree = 15,
        EarfreeI3 = 16,
        Budsfree = 17,
        Swift = 18,
        Cambrian = 19,
        Openfun = 20,
        Sportfree = 21,
        Openfree = 22,
        OpenfeelMk2 = 23,
        Speakfree = 24,
        Magician = 25,
        Furina = 26,
        Zircon = 27,
        Hyacinth = 28,
        Jasmine = 29,
        AstroX = 30,
        Opal = 31,
        Mirror = 32,
        Ky01 = 33,
        Coral = 34,
        BudsfeelMk2 = 35,
        Swift2 = 36,
        LuminousPatamon = 37,
        LuminousTailmon = 38,
        LuminousGabumon = 39,
        LuminousAgumon = 40,
        EarfeelI7 = 41,
        Speakfeel2 = 42,
        OpenfeelL = 43,
        FirstAngel = 44,
        Auramic = 45,
        Moka = 46
    }

    public enum RoseSenderKind
    {
        None,
        F8,
        Explorer,
        RsCommon,
        RsCommonV2,
        RsCommonV2Mic,
        Zt219,
        Zt229,
        Zt231
    }

    [Flags]
    public enum RoseDeviceFeatures : ulong
    {
        None = 0,
        Battery = 1UL << 0,
        DeviceInformation = 1UL << 1,
        NoiseControl = 1UL << 2,
        NoiseLevel = 1UL << 3,
        Equalizer = 1UL << 4,
        LowLatency = 1UL << 5,
        WearDetection = 1UL << 6,
        TouchControls = 1UL << 7,
        SpatialAudio = 1UL << 8,
        Multipoint = 1UL << 9,
        FindDevice = 1UL << 10,
        PromptToneLevel = 1UL << 11,
        FirmwareUpdate = 1UL << 12,
        AudioCodec = 1UL << 13,
        Language = 1UL << 14,
        HearingProtection = 1UL << 15,
        VoiceAssistant = 1UL << 16,
        AdaptiveNoiseControl = 1UL << 17,
        Recorder = 1UL << 18,
        MicrophoneControl = 1UL << 19
    }

    public sealed class RoseGattConfiguration
    {
        public RoseGattConfiguration(Guid serviceUuid, Guid writeUuid, Guid notifyUuid)
        {
            ServiceUuid = serviceUuid;
            WriteUuid = writeUuid;
            NotifyUuid = notifyUuid;
        }

        public Guid ServiceUuid { get; }
        public Guid WriteUuid { get; }
        public Guid NotifyUuid { get; }
    }

    public sealed class RoseDeviceDefinition
    {
        internal RoseDeviceDefinition(
            RoseDeviceType type,
            string id,
            string displayName,
            RoseFamily family,
            string? hardwareModel,
            IReadOnlyList<string> advertisementSignatures,
            RoseDeviceFeatures features,
            IReadOnlyList<string> aliases,
            IReadOnlyList<Guid> serviceUuids,
            RoseSenderKind senderKind,
            RoseGattConfiguration? gattConfiguration = null,
            IFeatureCommandMapper<RoseFrame>? commandMapper = null)
        {
            Type = type;
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Family = family;
            HardwareModel = hardwareModel;
            AdvertisementSignatures = advertisementSignatures ?? throw new ArgumentNullException(nameof(advertisementSignatures));
            Features = features;
            Aliases = aliases ?? throw new ArgumentNullException(nameof(aliases));
            ServiceUuids = serviceUuids ?? throw new ArgumentNullException(nameof(serviceUuids));
            SenderKind = senderKind;
            GattConfiguration = gattConfiguration;
            CommandMapper = commandMapper;
        }

        public RoseDeviceType Type { get; }
        public string Id { get; }
        public string DisplayName { get; }
        public RoseFamily Family { get; }
        public ProtocolFamily ProtocolFamily => Family switch
        {
            RoseFamily.F8 => ProtocolFamily.RoseF8,
            RoseFamily.RsCommon => ProtocolFamily.RoseRsCommon,
            RoseFamily.RsCommonV2 => ProtocolFamily.RoseRsCommonV2,
            RoseFamily.Zt => ProtocolFamily.RoseZt,
            _ => ProtocolFamily.Unknown
        };
        public string? HardwareModel { get; }
        public IReadOnlyList<string> AdvertisementSignatures { get; }
        public RoseDeviceFeatures Features { get; }
        public IReadOnlyList<string> Aliases { get; }
        public IReadOnlyList<Guid> ServiceUuids { get; }
        public RoseSenderKind SenderKind { get; }
        public RoseGattConfiguration? GattConfiguration { get; }
        public IFeatureCommandMapper<RoseFrame>? CommandMapper { get; }

        public bool Supports(RoseDeviceFeatures feature) => (Features & feature) == feature;

        public DeviceProfile CreateProfile(TransportEndpoint endpoint)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));

            var featureTypes = new List<Type>();
            var capabilities = new List<CapabilityDescriptor>();

            AddFeature(Features, RoseDeviceFeatures.Battery, typeof(IBatteryFeature), capabilities, featureTypes,
                new CapabilityDescriptor("battery", SupportStatus.Supported, true, false, true) { Unit = "percent", Minimum = 0, Maximum = 100, Step = 1 });
            AddFeature(Features, RoseDeviceFeatures.DeviceInformation, typeof(IDeviceInformationFeature), capabilities, featureTypes,
                new CapabilityDescriptor("device-information", SupportStatus.Supported, true));
            AddFeature(Features, RoseDeviceFeatures.NoiseControl, typeof(INoiseControlFeature), capabilities, featureTypes,
                new CapabilityDescriptor("noise-control", SupportStatus.Supported, true, true, true) { Options = new[] { "normal", "anc", "transparency", "wind" } });
            AddFeature(Features, RoseDeviceFeatures.NoiseLevel, typeof(INoiseLevelFeature), capabilities, featureTypes,
                new CapabilityDescriptor("noise-level", SupportStatus.Supported, true, true, true));
            AddFeature(Features, RoseDeviceFeatures.Equalizer, typeof(IEqPresetFeature), capabilities, featureTypes,
                new CapabilityDescriptor("equalizer", SupportStatus.Supported, true, true, true));
            AddFeature(Features, RoseDeviceFeatures.LowLatency, typeof(ILowLatencyFeature), capabilities, featureTypes,
                new CapabilityDescriptor("low-latency", SupportStatus.Supported, true, true, true) { Options = new[] { "off", "on" } });
            AddFeature(Features, RoseDeviceFeatures.WearDetection, typeof(IWearDetectionFeature), capabilities, featureTypes,
                new CapabilityDescriptor("wear-detection", SupportStatus.Supported, true, true, true));
            AddFeature(Features, RoseDeviceFeatures.TouchControls, typeof(ITouchControlFeature), capabilities, featureTypes,
                new CapabilityDescriptor("touch-controls", SupportStatus.Supported, true, true, true));
            if (Supports(RoseDeviceFeatures.TouchControls)) featureTypes.Add(typeof(IGestureMappingFeature));
            AddFeature(Features, RoseDeviceFeatures.SpatialAudio, typeof(ISpatialAudioFeature), capabilities, featureTypes,
                new CapabilityDescriptor("spatial-audio", SupportStatus.Supported, true, true, true));

            AddMetadataCapabilities(capabilities);

            return new DeviceProfile(Id, ProtocolFamily, endpoint)
            {
                FeatureTypes = featureTypes.ToArray(),
                Capabilities = capabilities.ToArray()
            };
        }

        private static void AddFeature(
            RoseDeviceFeatures available,
            RoseDeviceFeatures flag,
            Type featureType,
            ICollection<CapabilityDescriptor> capabilities,
            ICollection<Type> featureTypes,
            CapabilityDescriptor descriptor)
        {
            if ((available & flag) == 0) return;
            featureTypes.Add(featureType);
            capabilities.Add(descriptor);
        }

        private void AddMetadataCapabilities(ICollection<CapabilityDescriptor> capabilities)
        {
            AddMetadataCapability(capabilities, RoseDeviceFeatures.Multipoint, "multipoint");
            AddMetadataCapability(capabilities, RoseDeviceFeatures.FindDevice, "find-device");
            AddMetadataCapability(capabilities, RoseDeviceFeatures.PromptToneLevel, "prompt-tone-level");
            AddMetadataCapability(capabilities, RoseDeviceFeatures.FirmwareUpdate, "firmware-update", canRead: true);
            AddMetadataCapability(capabilities, RoseDeviceFeatures.AudioCodec, "audio-codec", canRead: true);
            AddMetadataCapability(capabilities, RoseDeviceFeatures.Language, "device-language", canRead: true);
            AddMetadataCapability(capabilities, RoseDeviceFeatures.HearingProtection, "hearing-protection");
            AddMetadataCapability(capabilities, RoseDeviceFeatures.VoiceAssistant, "voice-assistant");
            AddMetadataCapability(capabilities, RoseDeviceFeatures.AdaptiveNoiseControl, "adaptive-noise-control");
            AddMetadataCapability(capabilities, RoseDeviceFeatures.Recorder, "recorder");
            AddMetadataCapability(capabilities, RoseDeviceFeatures.MicrophoneControl, "microphone-control");
        }

        private void AddMetadataCapability(ICollection<CapabilityDescriptor> capabilities, RoseDeviceFeatures flag, string id, bool canRead = true)
        {
            if (Supports(flag)) capabilities.Add(new CapabilityDescriptor(id, SupportStatus.Supported, canRead, true, true));
        }
    }
}
