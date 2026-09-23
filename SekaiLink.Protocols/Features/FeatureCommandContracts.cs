using System;
using System.Collections.Generic;
using SekaiLink.Protocols.Models;

namespace SekaiLink.Protocols.Features;

/// <summary>
///     Stable feature identifiers used by the protocol-neutral command path.
///     Vendor assemblies translate these identifiers and values into wire frames.
/// </summary>
public static class FeatureIdentifiers
{
    public const string DeviceInformation = "device-information";
    public const string Battery = "battery";
    public const string FirmwareVersion = "firmware-version";
    public const string NoiseControl = "noise-control";
    public const string NoiseCancellationLevel = "noise-cancellation-level";
    public const string TransparencyLevel = "transparency-level";
    public const string NoiseControlCycle = "noise-control-cycle";
    public const string Equalizer = "equalizer";
    public const string LowLatency = "low-latency";
    public const string AudioCodec = "audio-codec";
    public const string Multipoint = "multipoint";
    public const string WearDetection = "wear-detection";
    public const string TouchControls = "touch-controls";
    public const string GestureMappings = "gesture-mappings";
    public const string FindDevice = "find-device";
    public const string PromptToneLevel = "prompt-tone-level";
    public const string SpatialAudio = "spatial-audio";
    public const string DeviceLanguage = "device-language";
    public const string HearingProtection = "hearing-protection";
    public const string GainLevel = "gain-level";
    public const string MicrophoneControl = "microphone-control";
}

/// <summary>
///     Maps protocol-neutral feature commands to vendor frames. A null
///     <see cref="FeatureDefinition.Value" /> represents a read operation.
/// </summary>
public interface IFeatureCommandMapper<TFrame>
{
    IReadOnlyList<FeatureDefinition> InitializationCommands { get; }
    TFrame Encode(FeatureDefinition command);
    string? GetResponseIdentifier(TFrame frame);
}

public enum AudioCodecMode
{
    AacSbc,
    Ldac,
    Lhdc,
    Lc3,
    Aptx,
    AptxHd,
    AptxAdaptive
}

public enum DeviceLanguage
{
    Chinese,
    English
}

public enum SpatialAudioMode
{
    Off,
    HeadTracking,
    Fixed,
    Music,
    Movie,
    Game,
    Television
}

/// A verified RsCommonV2 microphone command body in on-wire argument order.
public sealed class MicrophoneCommand
{
    public MicrophoneCommand(byte opcode, params byte[] arguments)
    {
        Opcode = opcode;
        Arguments = arguments ?? Array.Empty<byte>();
    }

    public byte Opcode { get; }
    public IReadOnlyList<byte> Arguments { get; }
}

public enum DeviceSide
{
    Left,
    Right
}

public enum DeviceGesture
{
    Single,
    Double,
    Triple,
    LongPress
}

public enum GestureAction
{
    VolumeUp,
    VolumeDown,
    PreviousTrack,
    NextTrack,
    VoiceAssistant,
    PlayPause,
    None,
    NoiseControl
}

public enum FindDeviceAction
{
    Stop,
    Left,
    Right
}

public sealed class NoiseControlCycle
{
    public bool Normal { get; init; }
    public bool Transparency { get; init; }
    public bool WindReduction { get; init; }
    public bool NoiseCancellation { get; init; }
}

public sealed class GestureMapping
{
    public GestureMapping(DeviceSide side, DeviceGesture gesture, GestureAction action)
    {
        Side = side;
        Gesture = gesture;
        Action = action;
    }

    public DeviceSide Side { get; }
    public DeviceGesture Gesture { get; }
    public GestureAction Action { get; }
}