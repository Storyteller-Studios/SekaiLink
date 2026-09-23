using System;
using System.Collections.Generic;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;
using SekaiLink.Rose.Protocol.Protocols;

namespace SekaiLink.Rose.Devices.Devices.Common;

/// Exact raw reports produced by F8Sender; Explorer's normal-mode override is preserved.
internal sealed class RoseF8FeatureCommandMapper : IFeatureCommandMapper<RoseFrame>
{
    private readonly bool _explorer;
    private readonly RoseDeviceFeatures _features;

    public RoseF8FeatureCommandMapper(RoseDeviceType type, RoseDeviceFeatures features)
    {
        _features = features;
        _explorer = type == RoseDeviceType.Explorer;
    }

    public IReadOnlyList<FeatureDefinition> InitializationCommands { get; } =
        new[] { new FeatureDefinition(FeatureIdentifiers.Battery) };

    public RoseFrame Encode(FeatureDefinition c)
    {
        if (c == null) throw new ArgumentNullException(nameof(c));
        if (c.Value == null) return Read(c.Identifier);
        return c.Identifier switch
        {
            FeatureIdentifiers.NoiseControl => Noise(Require<NoiseModeId>(c)),
            FeatureIdentifiers.NoiseCancellationLevel => Dynamic(RoseDeviceFeatures.NoiseLevel, 0x30, Byte(c)),
            FeatureIdentifiers.TransparencyLevel => Dynamic(RoseDeviceFeatures.NoiseLevel, 0x31, Byte(c)),
            FeatureIdentifiers.PromptToneLevel => Dynamic(RoseDeviceFeatures.PromptToneLevel, 0x32, Level(c)),
            FeatureIdentifiers.Equalizer => Eq(c.Value),
            FeatureIdentifiers.LowLatency => Flag(RoseDeviceFeatures.LowLatency, 0x25, Require<bool>(c), false),
            FeatureIdentifiers.AudioCodec => Audio(Require<AudioCodecMode>(c)),
            FeatureIdentifiers.Multipoint => Multipoint(Require<bool>(c)),
            FeatureIdentifiers.WearDetection => Flag(RoseDeviceFeatures.WearDetection, 0x26, Require<bool>(c), false),
            FeatureIdentifiers.TouchControls => Flag(RoseDeviceFeatures.TouchControls, 0x33, Require<bool>(c), false),
            FeatureIdentifiers.GestureMappings => Gesture(Require<GestureMapping>(c)),
            FeatureIdentifiers.FindDevice => Find(Require<FindDeviceAction>(c)),
            FeatureIdentifiers.DeviceLanguage => Language(Require<DeviceLanguage>(c)),
            _ => throw Unsupported(c.Identifier)
        };
    }

    public string? GetResponseIdentifier(RoseFrame frame)
    {
        return null;
    }

    private RoseFrame Read(string id)
    {
        return id switch
        {
            FeatureIdentifiers.DeviceInformation => throw new NotSupportedException(
                "F8Sender.getInfo returns an empty report."),
            FeatureIdentifiers.Battery => Raw(0x27, 0x01, 0x00, 0x01, 0x01),
            FeatureIdentifiers.FirmwareVersion => Raw(0x27, 0x01, 0x00, 0x01, 0x02),
            FeatureIdentifiers.Equalizer => Query(RoseDeviceFeatures.Equalizer, 0x04),
            FeatureIdentifiers.GestureMappings => Query(RoseDeviceFeatures.TouchControls, 0x05),
            FeatureIdentifiers.LowLatency => Query(RoseDeviceFeatures.LowLatency, 0x08),
            FeatureIdentifiers.WearDetection => Query(RoseDeviceFeatures.WearDetection, 0x09),
            FeatureIdentifiers.AudioCodec => Query(RoseDeviceFeatures.AudioCodec, 0x0B),
            FeatureIdentifiers.NoiseControl => Query(RoseDeviceFeatures.NoiseControl, 0x0C),
            FeatureIdentifiers.NoiseCancellationLevel => Query(RoseDeviceFeatures.NoiseLevel, 0x11),
            FeatureIdentifiers.TransparencyLevel => Query(RoseDeviceFeatures.NoiseLevel, 0x12),
            FeatureIdentifiers.PromptToneLevel => Query(RoseDeviceFeatures.PromptToneLevel, 0x13),
            FeatureIdentifiers.TouchControls => Query(RoseDeviceFeatures.TouchControls, 0x16),
            FeatureIdentifiers.DeviceLanguage => Query(RoseDeviceFeatures.Language, 0x0A),
            _ => throw new NotSupportedException($"F8Sender has no verified read for '{id}'.")
        };
    }

    private RoseFrame Noise(NoiseModeId mode)
    {
        Need(RoseDeviceFeatures.NoiseControl, FeatureIdentifiers.NoiseControl);
        var value = mode switch
        {
            NoiseModeId.WindReduction => 0, NoiseModeId.NoiseCancellation => 1, NoiseModeId.Transparency => 2,
            NoiseModeId.Normal => _explorer ? 0 : 3, _ => throw new ArgumentOutOfRangeException(nameof(mode))
        };
        return Raw(0x2C, 1, 0, 1, (byte)value);
    }

    private RoseFrame Eq(object value)
    {
        Need(RoseDeviceFeatures.Equalizer, FeatureIdentifiers.Equalizer);
        var id = value is EqualizerPreset p ? p.Id : value.ToString() ?? "";
        var v = id.ToLowerInvariant() switch
        {
            "hifi" => 0, "pop" => 1, "rock" => 2, _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
        return Raw(0x20, 1, 0, 2, 0, (byte)v);
    }

    private RoseFrame Audio(AudioCodecMode mode)
    {
        Need(RoseDeviceFeatures.AudioCodec, FeatureIdentifiers.AudioCodec);
        var v = mode switch
        {
            AudioCodecMode.Ldac => 1, AudioCodecMode.AacSbc => 2, AudioCodecMode.Lhdc => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(mode))
        };
        return Raw(0x2B, 1, 0, 1, (byte)v);
    }

    private RoseFrame Multipoint(bool enabled)
    {
        Need(RoseDeviceFeatures.Multipoint, FeatureIdentifiers.Multipoint);
        return enabled ? Raw(0x2B, 1, 0, 1, 0) : Raw(0x2B, 1, 0, 1, 2);
    }

    private RoseFrame Gesture(GestureMapping value)
    {
        Need(RoseDeviceFeatures.TouchControls, FeatureIdentifiers.GestureMappings);
        var key = (byte)((value.Side == DeviceSide.Left ? 1 : 2) + 2 * (int)value.Gesture);
        return Raw(0x22, 1, 0, 3, key, 1, (byte)value.Action);
    }

    private RoseFrame Find(FindDeviceAction value)
    {
        Need(RoseDeviceFeatures.FindDevice, FeatureIdentifiers.FindDevice);
        return Raw(0x2A, 1, 0, 1,
            value switch
            {
                FindDeviceAction.Left => 1, FindDeviceAction.Right => 2, FindDeviceAction.Stop => 0,
                _ => throw new ArgumentOutOfRangeException()
            });
    }

    private RoseFrame Language(DeviceLanguage value)
    {
        Need(RoseDeviceFeatures.Language, FeatureIdentifiers.DeviceLanguage);
        return Raw(0x29, 0x01, 0x00, 0x01, value == DeviceLanguage.English ? (byte)1 : (byte)0);
    }

    private RoseFrame Query(RoseDeviceFeatures feature, byte value)
    {
        Need(feature, feature.ToString());
        return Raw(0x27, 0x01, 0x00, 0x01, value);
    }

    private RoseFrame Flag(RoseDeviceFeatures feature, byte command, bool enabled, bool inverted)
    {
        Need(feature, command.ToString("X2"));
        return Raw(command, 1, 0, 1, (byte)(enabled ^ inverted ? 1 : 0));
    }

    private RoseFrame Dynamic(RoseDeviceFeatures feature, byte command, byte value)
    {
        Need(feature, command.ToString("X2"));
        return Raw(command, 1, 0, 1, value);
    }

    private static RoseFrame Raw(byte command, params byte[] tail)
    {
        var data = new byte[tail.Length + 2];
        data[0] = 0;
        data[1] = command;
        tail.CopyTo(data, 2);
        return new RoseFrame(0, 0, data);
    }

    private void Need(RoseDeviceFeatures f, string id)
    {
        if ((_features & f) == 0) throw Unsupported(id);
    }

    private NotSupportedException Unsupported(string id)
    {
        return new NotSupportedException($"This device does not advertise feature '{id}'.");
    }

    private static byte Level(FeatureDefinition c)
    {
        var v = Require<int>(c);
        if (v < 1 || v > 255) throw new ArgumentOutOfRangeException();
        return (byte)(v - 1);
    }

    private static byte Byte(FeatureDefinition c)
    {
        var v = Require<int>(c);
        if (v < 0 || v > 255) throw new ArgumentOutOfRangeException();
        return (byte)v;
    }

    private static T Require<T>(FeatureDefinition c)
    {
        return c.Value is T v ? v : throw new ArgumentException($"Feature '{c.Identifier}' expects {typeof(T).Name}.");
    }
}