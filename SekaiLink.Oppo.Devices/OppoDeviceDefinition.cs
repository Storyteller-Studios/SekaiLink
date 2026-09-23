using System;
using System.Collections.Generic;
using System.Linq;
using SekaiLink.Oppo.Protocol.Protocols;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;
using SekaiLink.Protocols.Transport;

namespace SekaiLink.Oppo.Devices;

[Flags]
public enum OppoDeviceFeatures : uint
{
    None = 0,
    Battery = 1 << 0,
    DeviceInformation = 1 << 1,
    NoiseControl = 1 << 2,
    Equalizer = 1 << 3,
    LowLatency = 1 << 4,
    WearDetection = 1 << 5,
    SpatialAudio = 1 << 6,
    Multipoint = 1 << 7,
    FindDevice = 1 << 8,
    PromptToneLevel = 1 << 9,
    FirmwareUpdate = 1 << 10,
    TouchControls = 1 << 11,
    CustomEqualizer = 1 << 12,
    GameSound = 1 << 13
}

public sealed class OppoNoiseOption
{
    public OppoNoiseOption(string key, byte protocolIndex, bool selectable = true)
    {
        Key = key;
        ProtocolIndex = protocolIndex;
        IsSelectable = selectable;
    }

    public string Key { get; }
    public byte ProtocolIndex { get; }
    public bool IsSelectable { get; }
}

public sealed class OppoEqualizerOption
{
    public OppoEqualizerOption(byte protocolIndex, int modeType)
    {
        ProtocolIndex = protocolIndex;
        ModeType = modeType;
    }

    public byte ProtocolIndex { get; }
    public int ModeType { get; }
    public string Id => "M" + ProtocolIndex;
}

/// <summary>Static, model-specific facts derived from OppoPodsManager's DeviceModels.json.</summary>
public sealed class OppoDeviceDefinition
{
    internal OppoDeviceDefinition(string id, string displayName, string brand, string? serviceUuid,
        bool supportsSpp, int protocolType, OppoDeviceFeatures features,
        OppoNoiseOption[] noiseOptions, OppoEqualizerOption[] equalizerOptions, byte[] spatialTypes)
    {
        Id = id;
        DisplayName = displayName;
        Brand = brand;
        ServiceUuid = serviceUuid;
        SupportsSpp = supportsSpp;
        ProtocolType = protocolType;
        Features = features;
        NoiseOptions = Array.AsReadOnly(noiseOptions);
        EqualizerOptions = Array.AsReadOnly(equalizerOptions);
        SpatialTypes = Array.AsReadOnly(spatialTypes);
        CommandMapper = new OppoFeatureCommandMapper(this);
    }

    public string Id { get; }
    public string DisplayName { get; }
    public string Brand { get; }
    public string? ServiceUuid { get; }
    public bool SupportsSpp { get; }
    public int ProtocolType { get; }
    public bool IsSupported => SupportsSpp && ProtocolType != 0;
    public OppoDeviceFeatures Features { get; }
    public IReadOnlyList<OppoNoiseOption> NoiseOptions { get; }
    public IReadOnlyList<OppoEqualizerOption> EqualizerOptions { get; }
    public IReadOnlyList<byte> SpatialTypes { get; }
    public IFeatureCommandMapper<OppoFrame> CommandMapper { get; }

    public bool Supports(OppoDeviceFeatures feature)
    {
        return (Features & feature) == feature;
    }

    public DeviceProfile CreateProfile(TransportEndpoint endpoint)
    {
        if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
        var types = new List<Type> { typeof(IBatteryFeature), typeof(IDeviceInformationFeature) };
        var capabilities = new List<CapabilityDescriptor>
        {
            new(FeatureIdentifiers.Battery, SupportStatus.Supported, true, false, true)
                { Unit = "percent", Minimum = 0, Maximum = 100, Step = 1 },
            new(FeatureIdentifiers.DeviceInformation, SupportStatus.Supported, true)
        };

        if (Supports(OppoDeviceFeatures.NoiseControl))
        {
            types.Add(typeof(INoiseControlFeature));
            var options = new List<string>();
            foreach (var option in NoiseOptions)
                if (option.IsSelectable && !options.Contains(option.Key))
                    options.Add(option.Key);
            capabilities.Add(new CapabilityDescriptor(FeatureIdentifiers.NoiseControl, SupportStatus.Supported, true,
                    true, true)
                { Options = options.AsReadOnly() });
        }

        if (Supports(OppoDeviceFeatures.Equalizer))
        {
            types.Add(typeof(IEqPresetFeature));
            var options = new List<string>();
            foreach (var option in EqualizerOptions) options.Add(option.Id);
            capabilities.Add(
                new CapabilityDescriptor(FeatureIdentifiers.Equalizer, SupportStatus.Supported, true, true, true)
                    { Options = options.AsReadOnly() });
        }

        if (Supports(OppoDeviceFeatures.LowLatency))
        {
            types.Add(typeof(ILowLatencyFeature));
            capabilities.Add(
                new CapabilityDescriptor(FeatureIdentifiers.LowLatency, SupportStatus.Supported, true, true, true)
                    { Options = new[] { "off", "on" } });
        }

        if (Supports(OppoDeviceFeatures.WearDetection))
        {
            types.Add(typeof(IWearDetectionFeature));
            capabilities.Add(new CapabilityDescriptor(FeatureIdentifiers.WearDetection, SupportStatus.Supported, true,
                true, true));
        }

        if (Supports(OppoDeviceFeatures.SpatialAudio))
        {
            types.Add(typeof(ISpatialAudioFeature));
            capabilities.Add(new CapabilityDescriptor(FeatureIdentifiers.SpatialAudio, SupportStatus.Supported, true,
                true, true)
            {
                Options = SpatialTypes.Contains((byte)2)
                    ? new[] { "off", "fixed", "track" }
                    : new[] { "off", "fixed" }
            });
        }

        AddMetadata(capabilities, OppoDeviceFeatures.Multipoint, FeatureIdentifiers.Multipoint);
        AddMetadata(capabilities, OppoDeviceFeatures.FindDevice, FeatureIdentifiers.FindDevice);
        AddMetadata(capabilities, OppoDeviceFeatures.PromptToneLevel, FeatureIdentifiers.PromptToneLevel);
        AddMetadata(capabilities, OppoDeviceFeatures.FirmwareUpdate, "firmware-update");
        AddMetadata(capabilities, OppoDeviceFeatures.TouchControls, FeatureIdentifiers.TouchControls);
        AddMetadata(capabilities, OppoDeviceFeatures.CustomEqualizer, "custom-equalizer");

        return new DeviceProfile(Id,
                endpoint.Kind == TransportKind.BleGatt ? ProtocolFamily.OppoGatt : ProtocolFamily.OppoSpp, endpoint)
            { FeatureTypes = types.AsReadOnly(), Capabilities = capabilities.AsReadOnly() };
    }

    private void AddMetadata(ICollection<CapabilityDescriptor> capabilities, OppoDeviceFeatures flag, string identifier)
    {
        if (Supports(flag)) capabilities.Add(new CapabilityDescriptor(identifier, SupportStatus.Supported));
    }
}