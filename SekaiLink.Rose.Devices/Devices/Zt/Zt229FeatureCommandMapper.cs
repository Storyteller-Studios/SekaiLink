using System;
using System.Collections.Generic;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;
using SekaiLink.Rose.Protocol.Protocols;

namespace SekaiLink.Rose.Devices.Devices.Zt;

/// <summary>
///     Exact ZT229 feature mapping recovered from RoseLink 3.8.0's Zt229Sender.
///     A RoseFrame payload contains the bytes after the ZT reserved byte and before the checksum.
/// </summary>
internal sealed class Zt229FeatureCommandMapper : IFeatureCommandMapper<RoseFrame>
{
    public IReadOnlyList<FeatureDefinition> InitializationCommands { get; } = new[]
    {
        Read(FeatureIdentifiers.DeviceInformation),
        Read(FeatureIdentifiers.NoiseControl),
        Read(FeatureIdentifiers.Equalizer),
        Read(FeatureIdentifiers.LowLatency),
        Read(FeatureIdentifiers.AudioCodec),
        Read(FeatureIdentifiers.FirmwareVersion),
        Read(FeatureIdentifiers.GestureMappings),
        Read(FeatureIdentifiers.TouchControls),
        Read(FeatureIdentifiers.NoiseControlCycle),
        Read(FeatureIdentifiers.WearDetection),
        Read(FeatureIdentifiers.NoiseCancellationLevel),
        Read(FeatureIdentifiers.TransparencyLevel),
        Read(FeatureIdentifiers.PromptToneLevel)
    };

    public RoseFrame Encode(FeatureDefinition command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        return command.Value == null ? EncodeRead(command.Identifier) : EncodeWrite(command);
    }

    public string? GetResponseIdentifier(RoseFrame frame)
    {
        return (frame.Group, frame.Command) switch
        {
            (0x01, 0x01) => FeatureIdentifiers.DeviceInformation,
            (0x01, 0x03) => FeatureIdentifiers.Battery,
            (0x0D, 0x01) => FeatureIdentifiers.FirmwareVersion,
            (0x02, 0x01) => FeatureIdentifiers.Equalizer,
            (0x04, 0x01) => FeatureIdentifiers.GestureMappings,
            (0x06, 0x03) => FeatureIdentifiers.LowLatency,
            (0x0C, 0x01) => FeatureIdentifiers.WearDetection,
            (0x08, 0x01) => FeatureIdentifiers.AudioCodec,
            (0x06, 0x02) => FeatureIdentifiers.NoiseControl,
            (0x06, 0x07) => FeatureIdentifiers.NoiseCancellationLevel,
            (0x06, 0x04) => FeatureIdentifiers.TransparencyLevel,
            (0x0E, 0x03) => FeatureIdentifiers.PromptToneLevel,
            (0x0A, 0x01) => FeatureIdentifiers.TouchControls,
            (0x0E, 0x02) => FeatureIdentifiers.FindDevice,
            (0x06, 0x05) => FeatureIdentifiers.NoiseControlCycle,
            (0x08, 0x02) => FeatureIdentifiers.Multipoint,
            _ => null
        };
    }

    private static RoseFrame EncodeRead(string identifier)
    {
        return identifier switch
        {
            FeatureIdentifiers.DeviceInformation or FeatureIdentifiers.Battery => Frame(0x01, 0x01),
            FeatureIdentifiers.NoiseControl => Frame(0x06, 0x02),
            FeatureIdentifiers.Equalizer => Frame(0x02, 0x01),
            FeatureIdentifiers.LowLatency => Frame(0x06, 0x03),
            FeatureIdentifiers.AudioCodec or FeatureIdentifiers.Multipoint => Frame(0x08, 0x01),
            FeatureIdentifiers.FirmwareVersion => Frame(0x0D, 0x01),
            FeatureIdentifiers.GestureMappings => Frame(0x04, 0x01),
            FeatureIdentifiers.TouchControls => Frame(0x0A, 0x01),
            FeatureIdentifiers.NoiseControlCycle => Frame(0x06, 0x05),
            FeatureIdentifiers.WearDetection => Frame(0x0C, 0x01),
            FeatureIdentifiers.NoiseCancellationLevel => Frame(0x06, 0x07),
            FeatureIdentifiers.TransparencyLevel => Frame(0x06, 0x04),
            FeatureIdentifiers.PromptToneLevel => Frame(0x0E, 0x03),
            _ => throw new NotSupportedException($"ZT229 does not support reading feature '{identifier}'.")
        };
    }

    private static RoseFrame EncodeWrite(FeatureDefinition command)
    {
        return command.Identifier switch
        {
            FeatureIdentifiers.NoiseControl => SetNoiseMode(Require<NoiseModeId>(command)),
            FeatureIdentifiers.NoiseControlCycle => SetNoiseCycle(Require<NoiseControlCycle>(command)),
            FeatureIdentifiers.NoiseCancellationLevel => Frame(0x06, 0x87, MapNoiseLevel(Require<int>(command))),
            FeatureIdentifiers.TransparencyLevel => Frame(0x06, 0x84, MapNoiseLevel(Require<int>(command))),
            FeatureIdentifiers.PromptToneLevel => SetPromptLevel(Require<int>(command)),
            FeatureIdentifiers.LowLatency => Frame(0x06, 0x83, Inverted(Require<bool>(command))),
            FeatureIdentifiers.Equalizer => SetEqualizer(command.Value!),
            FeatureIdentifiers.AudioCodec => SetAudioCodec(Require<AudioCodecMode>(command)),
            FeatureIdentifiers.Multipoint => Frame(0x08, 0x81, Require<bool>(command) ? (byte)0 : (byte)1),
            FeatureIdentifiers.WearDetection => Frame(0x0C, 0x81, Inverted(Require<bool>(command))),
            FeatureIdentifiers.TouchControls => Frame(0x0A, 0x81, Inverted(Require<bool>(command))),
            FeatureIdentifiers.GestureMappings => SetGesture(Require<GestureMapping>(command)),
            FeatureIdentifiers.FindDevice => SetFindDevice(Require<FindDeviceAction>(command)),
            _ => throw new NotSupportedException($"ZT229 does not support writing feature '{command.Identifier}'.")
        };
    }

    private static RoseFrame SetNoiseMode(NoiseModeId mode)
    {
        return mode switch
        {
            NoiseModeId.Transparency => Frame(0x06, 0x82, 0x00, 0x00, 0x00, 0x01),
            NoiseModeId.Normal => Frame(0x06, 0x82, 0x00, 0x01, 0x00, 0x00),
            NoiseModeId.WindReduction => Frame(0x06, 0x82, 0x00, 0x00, 0x01, 0x00),
            NoiseModeId.NoiseCancellation => Frame(0x06, 0x82, 0x01, 0x00, 0x00, 0x00),
            NoiseModeId.AdaptiveNoiseCancellation => Frame(0x06, 0x82, 0x02, 0x00, 0x00, 0x00),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported ZT229 noise mode.")
        };
    }

    private static RoseFrame SetNoiseCycle(NoiseControlCycle value)
    {
        return Frame(
            0x06,
            0x85,
            Bit(value.Normal),
            Bit(value.Transparency),
            Bit(value.WindReduction),
            Bit(value.NoiseCancellation));
    }

    private static RoseFrame SetPromptLevel(int level)
    {
        if (level < 1 || level > 5)
            throw new ArgumentOutOfRangeException(nameof(level), "Prompt level must be 1..5.");
        return Frame(0x0E, 0x83, (byte)(level - 1));
    }

    private static RoseFrame SetEqualizer(object value)
    {
        var id = value switch
        {
            EqualizerPreset preset => preset.Id,
            string identifier => identifier,
            _ => throw InvalidValue(FeatureIdentifiers.Equalizer, typeof(string), value)
        };
        var presetValue = id.ToLowerInvariant() switch
        {
            "pop" => (byte)0,
            "hifi" => (byte)1,
            "rock" => (byte)2,
            "light" => (byte)3,
            _ => throw new ArgumentOutOfRangeException(nameof(value), id, "Unsupported ZT229 equalizer preset.")
        };
        return Frame(0x02, 0x81, presetValue);
    }

    private static RoseFrame SetAudioCodec(AudioCodecMode value)
    {
        return value switch
        {
            AudioCodecMode.AacSbc => Frame(0x08, 0x81, 0x00),
            AudioCodecMode.Ldac => Frame(0x08, 0x81, 0x01),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported ZT229 audio codec.")
        };
    }

    private static RoseFrame SetGesture(GestureMapping value)
    {
        return Frame(
            0x04,
            0x81,
            value.Side switch
            {
                DeviceSide.Left => (byte)0,
                DeviceSide.Right => (byte)1,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            },
            value.Gesture switch
            {
                DeviceGesture.Single => (byte)0,
                DeviceGesture.Double => (byte)1,
                DeviceGesture.Triple => (byte)2,
                DeviceGesture.LongPress => (byte)3,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            },
            value.Action switch
            {
                GestureAction.VolumeUp => (byte)0,
                GestureAction.VolumeDown => (byte)1,
                GestureAction.PreviousTrack => (byte)2,
                GestureAction.NextTrack => (byte)3,
                GestureAction.VoiceAssistant => (byte)4,
                GestureAction.PlayPause => (byte)5,
                GestureAction.None => (byte)6,
                GestureAction.NoiseControl => (byte)7,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            });
    }

    private static RoseFrame SetFindDevice(FindDeviceAction value)
    {
        return value switch
        {
            FindDeviceAction.Stop => Frame(0x0E, 0x82, 0x03, 0x00),
            FindDeviceAction.Left => Frame(0x0E, 0x82, 0x00, 0x01),
            FindDeviceAction.Right => Frame(0x0E, 0x82, 0x01, 0x01),
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
    }

    private static RoseFrame Frame(byte group, byte command, params byte[] payload)
    {
        return new RoseFrame(group, command, payload);
    }

    private static byte Bit(bool value)
    {
        return value ? (byte)1 : (byte)0;
    }

    // ZT229 uses inverted Boolean values for game mode, wear detection, and touch control.
    private static byte Inverted(bool value)
    {
        return value ? (byte)0 : (byte)1;
    }

    private static byte MapNoiseLevel(int level)
    {
        return level switch
        {
            1 => 0,
            3 => 1,
            5 => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(level), "Noise level must be 1, 3, or 5.")
        };
    }

    private static T Require<T>(FeatureDefinition command)
    {
        if (command.Value is T value) return value;
        throw InvalidValue(command.Identifier, typeof(T), command.Value);
    }

    private static ArgumentException InvalidValue(string identifier, Type expected, object? actual)
    {
        return new ArgumentException(
            $"Feature '{identifier}' expects {expected.Name}, but received {actual?.GetType().Name ?? "null"}.",
            "command");
    }

    private static FeatureDefinition Read(string identifier)
    {
        return new FeatureDefinition(identifier);
    }
}