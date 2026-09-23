using System;
using System.Collections.Generic;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;
using SekaiLink.Rose.Protocol.Protocols;

namespace SekaiLink.Rose.Devices.Devices.Zt
{
    /// Exact standard ZtSender table with the Zt219 and Zt231 model overrides.
    internal sealed class ZtModelFeatureCommandMapper : IFeatureCommandMapper<RoseFrame>
    {
        private readonly RoseDeviceType _type;
        private readonly RoseDeviceFeatures _features;
        public ZtModelFeatureCommandMapper(RoseDeviceType type, RoseDeviceFeatures features)
        {
            _type = type; _features = features;
            var commands = new List<FeatureDefinition> { Read(FeatureIdentifiers.DeviceInformation), Read(FeatureIdentifiers.FirmwareVersion) };
            Add(commands, RoseDeviceFeatures.NoiseControl, FeatureIdentifiers.NoiseControl);
            Add(commands, RoseDeviceFeatures.Equalizer, FeatureIdentifiers.Equalizer);
            Add(commands, RoseDeviceFeatures.LowLatency, FeatureIdentifiers.LowLatency);
            Add(commands, RoseDeviceFeatures.AudioCodec, FeatureIdentifiers.AudioCodec);
            Add(commands, RoseDeviceFeatures.TouchControls, FeatureIdentifiers.GestureMappings);
            Add(commands, RoseDeviceFeatures.TouchControls, FeatureIdentifiers.TouchControls);
            Add(commands, RoseDeviceFeatures.NoiseLevel, FeatureIdentifiers.NoiseCancellationLevel);
            Add(commands, RoseDeviceFeatures.NoiseLevel, FeatureIdentifiers.TransparencyLevel);
            Add(commands, RoseDeviceFeatures.PromptToneLevel, FeatureIdentifiers.PromptToneLevel);
            InitializationCommands = commands;
        }
        public IReadOnlyList<FeatureDefinition> InitializationCommands { get; }
        public RoseFrame Encode(FeatureDefinition c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c)); if (c.Value == null) return EncodeRead(c.Identifier);
            return c.Identifier switch
            {
                FeatureIdentifiers.NoiseControl => Noise(Require<NoiseModeId>(c)),
                FeatureIdentifiers.NoiseCancellationLevel => LevelCommand(0x87, c),
                FeatureIdentifiers.TransparencyLevel => LevelCommand(0x84, c),
                FeatureIdentifiers.PromptToneLevel => Prompt(c),
                FeatureIdentifiers.Equalizer => Eq(c.Value),
                FeatureIdentifiers.LowLatency => Game(Require<bool>(c)),
                FeatureIdentifiers.AudioCodec => Audio(Require<AudioCodecMode>(c)),
                FeatureIdentifiers.Multipoint => Multipoint(Require<bool>(c)),
                FeatureIdentifiers.TouchControls => Touch(Require<bool>(c)),
                FeatureIdentifiers.GestureMappings => Gesture(Require<GestureMapping>(c)),
                FeatureIdentifiers.FindDevice => Find(Require<FindDeviceAction>(c)),
                FeatureIdentifiers.DeviceLanguage => Language(Require<DeviceLanguage>(c)),
                FeatureIdentifiers.GainLevel => Gain(c),
                _ => throw new NotSupportedException($"{_type} does not support '{c.Identifier}'.")
            };
        }
        public string? GetResponseIdentifier(RoseFrame f) => (f.Group, f.Command) switch
        {
            (1, 1) => FeatureIdentifiers.DeviceInformation,
            (1, 3) => FeatureIdentifiers.Battery,
            (13, 1) => FeatureIdentifiers.FirmwareVersion,
            (2, 1) => FeatureIdentifiers.Equalizer,
            (4, 1) => FeatureIdentifiers.GestureMappings,
            (6, 3) => FeatureIdentifiers.LowLatency,
            (8, 1) => FeatureIdentifiers.AudioCodec,
            (6, 2) => FeatureIdentifiers.NoiseControl,
            (6, 7) => FeatureIdentifiers.NoiseCancellationLevel,
            (6, 4) => FeatureIdentifiers.TransparencyLevel,
            (14, 3) => FeatureIdentifiers.PromptToneLevel,
            (10, 1) => FeatureIdentifiers.TouchControls,
            _ => null
        };
        private RoseFrame EncodeRead(string id) => id switch
        {
            FeatureIdentifiers.DeviceInformation => Frame(1, 1),
            FeatureIdentifiers.Battery => _type == RoseDeviceType.Headfree ? Frame(1, 3) : Frame(1, 1),
            FeatureIdentifiers.NoiseControl => Query(RoseDeviceFeatures.NoiseControl, 6, 2),
            FeatureIdentifiers.Equalizer => Query(RoseDeviceFeatures.Equalizer, 2, 1),
            FeatureIdentifiers.LowLatency => Query(RoseDeviceFeatures.LowLatency, 6, 3),
            FeatureIdentifiers.AudioCodec => Query(RoseDeviceFeatures.AudioCodec, 8, 1),
            FeatureIdentifiers.Multipoint => Query(RoseDeviceFeatures.Multipoint, 8, 1),
            FeatureIdentifiers.FirmwareVersion => Frame(13, 1),
            FeatureIdentifiers.GestureMappings => Query(RoseDeviceFeatures.TouchControls, 4, 1),
            FeatureIdentifiers.TouchControls => Query(RoseDeviceFeatures.TouchControls, 10, 1),
            FeatureIdentifiers.NoiseCancellationLevel => Query(RoseDeviceFeatures.NoiseLevel, 6, 7),
            FeatureIdentifiers.TransparencyLevel => Query(RoseDeviceFeatures.NoiseLevel, 6, 4),
            FeatureIdentifiers.PromptToneLevel => Query(RoseDeviceFeatures.PromptToneLevel, 14, 3),
            FeatureIdentifiers.GainLevel => Query(RoseDeviceFeatures.NoiseLevel, 2, 3),
            _ => throw new NotSupportedException($"{_type} has no verified read for '{id}'.")
        };
        private RoseFrame Noise(NoiseModeId mode)
        {
            Need(RoseDeviceFeatures.NoiseControl, FeatureIdentifiers.NoiseControl);
            if (_type == RoseDeviceType.EarfreeI3) return mode switch
            {
                NoiseModeId.NoiseCancellation => Frame(6, 0x82, 1, 0, 0),
                NoiseModeId.Normal => Frame(6, 0x82, 0, 0, 1),
                NoiseModeId.Transparency => Frame(6, 0x82, 0, 1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };
            return mode switch
            {
                NoiseModeId.NoiseCancellation => Frame(6, 0x82, 1, 0, 0, 0),
                NoiseModeId.Normal => Frame(6, 0x82, 0, 1, 0, 0),
                NoiseModeId.Transparency => Frame(6, 0x82, 0, 0, 0, 1),
                NoiseModeId.WindReduction => Frame(6, 0x82, 0, 0, 1, 0),
                NoiseModeId.AdaptiveNoiseCancellation => Frame(6, 0x82, 2, 0, 0, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };
        }
        private RoseFrame Eq(object value) { Need(RoseDeviceFeatures.Equalizer, FeatureIdentifiers.Equalizer); var id = value is EqualizerPreset p ? p.Id : value.ToString() ?? ""; var v = id.ToLowerInvariant() switch { "pop" => 0, "hifi" => 1, "rock" => 2, "light" when _type != RoseDeviceType.EarfreeI3 => 3, _ => throw new ArgumentOutOfRangeException(nameof(value)) }; return Frame(2, 0x81, (byte)v); }
        private RoseFrame Audio(AudioCodecMode mode) { Need(RoseDeviceFeatures.AudioCodec, FeatureIdentifiers.AudioCodec); return mode switch { AudioCodecMode.AacSbc => Frame(8, 0x81, 0), AudioCodecMode.Ldac => Frame(8, 0x81, 1), AudioCodecMode.Lhdc when _type == RoseDeviceType.Headfree => Frame(8, 0x81, 2), _ => throw new ArgumentOutOfRangeException(nameof(mode)) }; }
        private RoseFrame Game(bool enabled) { Need(RoseDeviceFeatures.LowLatency, FeatureIdentifiers.LowLatency); return Frame(6, 0x83, enabled ? (byte)0 : (byte)1); }
        private RoseFrame Multipoint(bool enabled) { Need(RoseDeviceFeatures.Multipoint, FeatureIdentifiers.Multipoint); return Frame(8, 0x81, enabled ? (byte)0 : (byte)1); }
        private RoseFrame Touch(bool enabled) { Need(RoseDeviceFeatures.TouchControls, FeatureIdentifiers.TouchControls); return Frame(10, 0x81, enabled ? (byte)0 : (byte)1); }
        private RoseFrame LevelCommand(int command, FeatureDefinition c) { Need(RoseDeviceFeatures.NoiseLevel, c.Identifier); return Frame(6, command, NoiseLevel(c)); }
        private RoseFrame Prompt(FeatureDefinition c) { Need(RoseDeviceFeatures.PromptToneLevel, FeatureIdentifiers.PromptToneLevel); return Frame(14, _type == RoseDeviceType.Headfree ? 3 : 0x83, Level(c)); }
        private RoseFrame Language(DeviceLanguage language) { Need(RoseDeviceFeatures.Language, FeatureIdentifiers.DeviceLanguage); return Frame(15, 0x81, language == DeviceLanguage.English ? (byte)1 : (byte)0); }
        private RoseFrame Gain(FeatureDefinition c) { Need(RoseDeviceFeatures.NoiseLevel, FeatureIdentifiers.GainLevel); return Frame(2, 0x83, Byte(c)); }
        private RoseFrame Gesture(GestureMapping v) { Need(RoseDeviceFeatures.TouchControls, FeatureIdentifiers.GestureMappings); return Frame(4, 0x81, (byte)v.Side, (byte)v.Gesture, (byte)v.Action); }
        private RoseFrame Find(FindDeviceAction v) { Need(RoseDeviceFeatures.FindDevice, FeatureIdentifiers.FindDevice); var command = _type == RoseDeviceType.Headfree ? (byte)0x02 : (byte)0x82; return v switch { FindDeviceAction.Left => Frame(14, command, 0, 1), FindDeviceAction.Right => Frame(14, command, 1, 1), FindDeviceAction.Stop => Frame(14, command, 3, 0), _ => throw new ArgumentOutOfRangeException(nameof(v)) }; }
        private RoseFrame Query(RoseDeviceFeatures feature, int group, int command) { Need(feature, feature.ToString()); return Frame(group, command); }
        private void Need(RoseDeviceFeatures f, string id) { if ((_features & f) == 0) throw new NotSupportedException($"{_type} does not advertise '{id}'."); }
        private static RoseFrame Frame(int g, int c, params byte[] p) => new((byte)g, (byte)c, p);
        private static FeatureDefinition Read(string id) => new(id);
        private void Add(ICollection<FeatureDefinition> list, RoseDeviceFeatures feature, string id) { if ((_features & feature) != 0) list.Add(Read(id)); }
        private static byte Level(FeatureDefinition c) { var v = Require<int>(c); if (v < 1 || v > 5) throw new ArgumentOutOfRangeException(); return (byte)(v - 1); }
        private static byte NoiseLevel(FeatureDefinition c) => Require<int>(c) switch { 1 => 0, 3 => 1, 5 => 2, _ => throw new ArgumentOutOfRangeException() };
        private static byte Byte(FeatureDefinition c) { var v = Require<int>(c); if (v < 0 || v > 255) throw new ArgumentOutOfRangeException(); return (byte)v; }
        private static T Require<T>(FeatureDefinition c) => c.Value is T v ? v : throw new ArgumentException($"Feature '{c.Identifier}' expects {typeof(T).Name}.");
    }
}
