using System.Globalization;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;

namespace SekaiLink.ConsoleApp;

/// <summary>Parses protocol-neutral shell verbs. A device mapper decides how to encode them.</summary>
internal static class DeviceFeatureCommandParser
{
    public static FeatureDefinition Parse(IReadOnlyList<string> action)
    {
        if (action.Count == 0) throw new CommandLineException("Missing device operation.");
        return action[0].ToLowerInvariant() switch
        {
            "get" => ParseRead(action),
            "noise" => Write(FeatureIdentifiers.NoiseControl, ParseNoise(Argument(action, 1, "noise <mode>"))),
            "anc-level" => Write(FeatureIdentifiers.NoiseCancellationLevel, Choice(action, "anc-level", 1, 3, 5)),
            "transparency-level" => Write(FeatureIdentifiers.TransparencyLevel, Choice(action, "transparency-level", 1, 3, 5)),
            "prompt-level" => Write(FeatureIdentifiers.PromptToneLevel, Choice(action, "prompt-level", 1, 2, 3, 4, 5)),
            "anc-line" => ParseNoiseCycle(action),
            "game" => Write(FeatureIdentifiers.LowLatency, Switch(Argument(action, 1, "game <on|off>"))),
            "eq" => Write(FeatureIdentifiers.Equalizer, Argument(action, 1, "eq <preset>")),
            "multipoint" => Write(FeatureIdentifiers.Multipoint, Switch(Argument(action, 1, "multipoint <on|off>"))),
            "audio" => Write(FeatureIdentifiers.AudioCodec, ParseAudio(Argument(action, 1, "audio <ldac|aac-sbc>"))),
            "spatial" => Write(FeatureIdentifiers.SpatialAudio, ParseSpatial(Argument(action, 1, "spatial <off|head|fixed>"))),
            "language" => Write(FeatureIdentifiers.DeviceLanguage, ParseLanguage(Argument(action, 1, "language <chinese|english>"))),
            "hearing-protection" => Write(FeatureIdentifiers.HearingProtection, Number(action, "hearing-protection")),
            "gain-level" => Write(FeatureIdentifiers.GainLevel, Number(action, "gain-level")),
            "mic" => ParseMicrophone(action),
            "wear-detection" => Write(FeatureIdentifiers.WearDetection, Switch(Argument(action, 1, "wear-detection <on|off>"))),
            "touch" => Write(FeatureIdentifiers.TouchControls, Switch(Argument(action, 1, "touch <on|off>"))),
            "touch-map" => ParseTouchMapping(action),
            "find" => ParseFind(action),
            _ => throw new CommandLineException($"Unknown device operation '{action[0]}'.")
        };
    }

    public static IReadOnlyList<(string Syntax, FeatureDefinition Command)> Examples { get; } = new[]
    {
        ("get info", Read(FeatureIdentifiers.DeviceInformation)),
        ("get battery", Read(FeatureIdentifiers.Battery)),
        ("get noise", Read(FeatureIdentifiers.NoiseControl)),
        ("get eq", Read(FeatureIdentifiers.Equalizer)),
        ("get game", Read(FeatureIdentifiers.LowLatency)),
        ("get spatial", Read(FeatureIdentifiers.SpatialAudio)),
        ("get audio", Read(FeatureIdentifiers.AudioCodec)),
        ("get version", Read(FeatureIdentifiers.FirmwareVersion)),
        ("get touch-map", Read(FeatureIdentifiers.GestureMappings)),
        ("get touch", Read(FeatureIdentifiers.TouchControls)),
        ("get noise-list", Read(FeatureIdentifiers.NoiseControlCycle)),
        ("get wear", Read(FeatureIdentifiers.WearDetection)),
        ("get anc-level", Read(FeatureIdentifiers.NoiseCancellationLevel)),
        ("get transparency-level", Read(FeatureIdentifiers.TransparencyLevel)),
        ("get prompt-level", Read(FeatureIdentifiers.PromptToneLevel)),
        ("noise normal", Write(FeatureIdentifiers.NoiseControl, NoiseModeId.Normal)),
        ("noise anc", Write(FeatureIdentifiers.NoiseControl, NoiseModeId.NoiseCancellation)),
        ("noise transparency", Write(FeatureIdentifiers.NoiseControl, NoiseModeId.Transparency)),
        ("noise wind", Write(FeatureIdentifiers.NoiseControl, NoiseModeId.WindReduction)),
        ("noise adaptive", Write(FeatureIdentifiers.NoiseControl, NoiseModeId.AdaptiveNoiseCancellation)),
        ("game on", Write(FeatureIdentifiers.LowLatency, true)),
        ("game off", Write(FeatureIdentifiers.LowLatency, false)),
        ("eq pop", Write(FeatureIdentifiers.Equalizer, "pop")),
        ("eq M1", Write(FeatureIdentifiers.Equalizer, "M1")),
        ("eq hifi", Write(FeatureIdentifiers.Equalizer, "hifi")),
        ("eq rock", Write(FeatureIdentifiers.Equalizer, "rock")),
        ("eq light", Write(FeatureIdentifiers.Equalizer, "light")),
        ("eq custom", Write(FeatureIdentifiers.Equalizer, "custom")),
        ("eq eap", Write(FeatureIdentifiers.Equalizer, "eap")),
        ("audio aac-sbc", Write(FeatureIdentifiers.AudioCodec, AudioCodecMode.AacSbc)),
        ("audio ldac", Write(FeatureIdentifiers.AudioCodec, AudioCodecMode.Ldac)),
        ("audio lhdc", Write(FeatureIdentifiers.AudioCodec, AudioCodecMode.Lhdc)),
        ("audio lc3", Write(FeatureIdentifiers.AudioCodec, AudioCodecMode.Lc3)),
        ("spatial head", Write(FeatureIdentifiers.SpatialAudio, SpatialAudioMode.HeadTracking)),
        ("spatial fixed", Write(FeatureIdentifiers.SpatialAudio, SpatialAudioMode.Fixed)),
        ("language english", Write(FeatureIdentifiers.DeviceLanguage, DeviceLanguage.English)),
        ("mic 5B 00 01", Write(FeatureIdentifiers.MicrophoneControl, new MicrophoneCommand(0x5B, 0x00, 0x01))),
        ("multipoint on", Write(FeatureIdentifiers.Multipoint, true)),
        ("wear-detection on", Write(FeatureIdentifiers.WearDetection, true)),
        ("touch on", Write(FeatureIdentifiers.TouchControls, true)),
        ("find left", Write(FeatureIdentifiers.FindDevice, FindDeviceAction.Left)),
        ("find right", Write(FeatureIdentifiers.FindDevice, FindDeviceAction.Right)),
        ("find off", Write(FeatureIdentifiers.FindDevice, FindDeviceAction.Stop)),
        ("anc-level 5", Write(FeatureIdentifiers.NoiseCancellationLevel, 5)),
        ("transparency-level 5", Write(FeatureIdentifiers.TransparencyLevel, 5)),
        ("prompt-level 5", Write(FeatureIdentifiers.PromptToneLevel, 5))
    };

    public static IEnumerable<string> HelpLines()
    {
        yield return "  init                              run device-defined initialization queries";
        yield return "  get <feature>                     read a supported feature";
        yield return "  noise normal|transparency|wind|anc|adaptive";
        yield return "  game on|off";
        yield return "  eq <preset>                       verified preset name for the device sender";
        yield return "  use commands <型号 ID> to list the EQ preset IDs for a model";
        yield return "  anc-level 1|3|5";
        yield return "  transparency-level 1|3|5";
        yield return "  anc-line <normal> <trans> <wind> <anc>   four 0/1 values";
        yield return "  audio ldac|aac-sbc";
        yield return "  spatial off|head|fixed";
        yield return "  language chinese|english";
        yield return "  hearing-protection <0-255>";
        yield return "  gain-level <0-255>";
        yield return "  mic <opcode-hex> <byte> [byte...]  device-specific microphone body";
        yield return "  multipoint on|off";
        yield return "  wear-detection on|off";
        yield return "  touch on|off";
        yield return "  touch-map <left|right> <single|double|triple|long> <action>";
        yield return "  prompt-level 1|2|3|4|5";
        yield return "  find left|right|off";
        yield return "  raw <hex>                         send an already framed packet";
    }

    private static FeatureDefinition ParseRead(IReadOnlyList<string> action) => Read(
        Argument(action, 1, "get <feature>").ToLowerInvariant() switch
        {
            "info" => FeatureIdentifiers.DeviceInformation,
            "battery" => FeatureIdentifiers.Battery,
            "noise" => FeatureIdentifiers.NoiseControl,
            "eq" => FeatureIdentifiers.Equalizer,
            "game" => FeatureIdentifiers.LowLatency,
            "spatial" => FeatureIdentifiers.SpatialAudio,
            "audio" => FeatureIdentifiers.AudioCodec,
            "multipoint" => FeatureIdentifiers.Multipoint,
            "version" => FeatureIdentifiers.FirmwareVersion,
            "touch-map" or "key" => FeatureIdentifiers.GestureMappings,
            "touch" => FeatureIdentifiers.TouchControls,
            "noise-list" => FeatureIdentifiers.NoiseControlCycle,
            "wear" or "wear-detection" => FeatureIdentifiers.WearDetection,
            "anc-level" => FeatureIdentifiers.NoiseCancellationLevel,
            "transparency-level" => FeatureIdentifiers.TransparencyLevel,
            "prompt-level" => FeatureIdentifiers.PromptToneLevel,
            var value => throw new CommandLineException($"Unknown feature '{value}'.")
        });

    private static FeatureDefinition ParseNoiseCycle(IReadOnlyList<string> action)
    {
        if (action.Count != 5) throw new CommandLineException("Usage: anc-line <normal 0|1> <transparency 0|1> <wind 0|1> <anc 0|1>.");
        return Write(FeatureIdentifiers.NoiseControlCycle, new NoiseControlCycle
        {
            Normal = Bit(action[1]),
            Transparency = Bit(action[2]),
            WindReduction = Bit(action[3]),
            NoiseCancellation = Bit(action[4])
        });
    }

    private static FeatureDefinition ParseTouchMapping(IReadOnlyList<string> action)
    {
        if (action.Count != 4) throw new CommandLineException("Usage: touch-map <side> <gesture> <action>.");
        var side = action[1].ToLowerInvariant() switch
        {
            "left" => DeviceSide.Left,
            "right" => DeviceSide.Right,
            _ => throw new CommandLineException("Side must be left or right.")
        };
        var gesture = action[2].ToLowerInvariant() switch
        {
            "single" or "one" => DeviceGesture.Single,
            "double" or "two" => DeviceGesture.Double,
            "triple" or "three" => DeviceGesture.Triple,
            "long" or "long-press" => DeviceGesture.LongPress,
            _ => throw new CommandLineException("Gesture must be single, double, triple or long.")
        };
        var touchAction = action[3].ToLowerInvariant() switch
        {
            "volume-up" => GestureAction.VolumeUp,
            "volume-down" => GestureAction.VolumeDown,
            "previous" or "previous-track" => GestureAction.PreviousTrack,
            "next" or "next-track" => GestureAction.NextTrack,
            "assistant" => GestureAction.VoiceAssistant,
            "play-pause" or "play" => GestureAction.PlayPause,
            "none" => GestureAction.None,
            "noise" or "noise-control" => GestureAction.NoiseControl,
            _ => throw new CommandLineException("Unknown touch action.")
        };
        return Write(FeatureIdentifiers.GestureMappings, new GestureMapping(side, gesture, touchAction));
    }

    private static FeatureDefinition ParseFind(IReadOnlyList<string> action) =>
        Argument(action, 1, "find <left|right|off>").ToLowerInvariant() switch
        {
            "left" => Write(FeatureIdentifiers.FindDevice, FindDeviceAction.Left),
            "right" => Write(FeatureIdentifiers.FindDevice, FindDeviceAction.Right),
            "off" or "stop" => Write(FeatureIdentifiers.FindDevice, FindDeviceAction.Stop),
            _ => throw new CommandLineException("Find target must be left, right or off.")
        };

    private static object ParseNoise(string value) => value.ToLowerInvariant() switch
    {
        "normal" or "off" => NoiseModeId.Normal,
        "anc" or "on" => NoiseModeId.NoiseCancellation,
        "transparency" or "trans" or "ambient" => NoiseModeId.Transparency,
        "wind" => NoiseModeId.WindReduction,
        "adaptive" or "auto" => NoiseModeId.AdaptiveNoiseCancellation,
        // Vendor catalogues may expose additional selectable submodes.
        _ => value
    };

    private static AudioCodecMode ParseAudio(string value) => value.ToLowerInvariant() switch
    {
        "ldac" => AudioCodecMode.Ldac,
        "aac-sbc" or "aac" or "sbc" or "multipoint" => AudioCodecMode.AacSbc,
        "lhdc" => AudioCodecMode.Lhdc,
        "lc3" => AudioCodecMode.Lc3,
        "aptx" => AudioCodecMode.Aptx,
        "aptx-hd" => AudioCodecMode.AptxHd,
        "aptx-adaptive" => AudioCodecMode.AptxAdaptive,
        _ => throw new CommandLineException("Audio mode must be ldac or aac-sbc.")
    };

    private static SpatialAudioMode ParseSpatial(string value) => value.ToLowerInvariant() switch
    {
        "off" => SpatialAudioMode.Off,
        "head" or "head-tracking" => SpatialAudioMode.HeadTracking,
        "fixed" => SpatialAudioMode.Fixed,
        "music" => SpatialAudioMode.Music,
        "movie" => SpatialAudioMode.Movie,
        "game" => SpatialAudioMode.Game,
        "tv" or "television" => SpatialAudioMode.Television,
        _ => throw new CommandLineException("Unknown spatial audio mode.")
    };

    private static DeviceLanguage ParseLanguage(string value) => value.ToLowerInvariant() switch
    {
        "zh" or "cn" or "chinese" => DeviceLanguage.Chinese,
        "en" or "english" => DeviceLanguage.English,
        _ => throw new CommandLineException("Language must be chinese or english.")
    };

    private static int Number(IReadOnlyList<string> action, string name)
    {
        var text = Argument(action, 1, $"{name} <0-255>");
        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) || value < 0 || value > 255)
            throw new CommandLineException($"{name} must be between 0 and 255.");
        return value;
    }

    private static FeatureDefinition ParseMicrophone(IReadOnlyList<string> action)
    {
        if (action.Count < 3) throw new CommandLineException("Usage: mic <opcode-hex> <byte> [byte...].");
        static byte HexByte(string value)
        {
            var text = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? value[2..] : value;
            if (!byte.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
                throw new CommandLineException($"Invalid hexadecimal byte '{value}'.");
            return result;
        }
        return Write(FeatureIdentifiers.MicrophoneControl, new MicrophoneCommand(HexByte(action[1]), action.Skip(2).Select(HexByte).ToArray()));
    }

    private static int Choice(IReadOnlyList<string> action, string name, params int[] choices)
    {
        var text = Argument(action, 1, $"{name} <{string.Join('|', choices)}>");
        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) || !choices.Contains(value))
            throw new CommandLineException($"{name} must be one of: {string.Join(", ", choices)}.");
        return value;
    }

    private static bool Switch(string value) => value.ToLowerInvariant() switch
    {
        "on" or "true" or "1" => true,
        "off" or "false" or "0" => false,
        _ => throw new CommandLineException("Switch value must be on or off.")
    };

    private static bool Bit(string value) => value switch
    {
        "1" => true,
        "0" => false,
        _ => throw new CommandLineException("Cycle values must be 0 or 1.")
    };

    private static string Argument(IReadOnlyList<string> action, int index, string usage)
    {
        if (action.Count <= index) throw new CommandLineException($"Usage: {usage}.");
        return action[index];
    }

    private static FeatureDefinition Read(string identifier) => new(identifier);
    private static FeatureDefinition Write(string identifier, object value) => new(identifier, value);
}
