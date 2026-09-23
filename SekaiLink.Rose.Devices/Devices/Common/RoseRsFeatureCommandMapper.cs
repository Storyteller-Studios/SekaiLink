using System;
using System.Collections.Generic;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;
using SekaiLink.Rose.Protocol.Protocols;

namespace SekaiLink.Rose.Devices.Devices.Common
{
    /// Exact command table shared by RoseLink's RsCommonSender and RsCommonV2Sender.
    internal sealed class RoseRsFeatureCommandMapper : IFeatureCommandMapper<RoseFrame>
    {
        private readonly RoseFamily _family;
        private readonly RoseDeviceFeatures _features;

        public RoseRsFeatureCommandMapper(RoseFamily family, RoseDeviceFeatures features)
        {
            if (family != RoseFamily.RsCommon && family != RoseFamily.RsCommonV2) throw new ArgumentOutOfRangeException(nameof(family));
            _family = family; _features = features;
        }

        public IReadOnlyList<FeatureDefinition> InitializationCommands { get; } = new[] { new FeatureDefinition(FeatureIdentifiers.DeviceInformation) };

        public RoseFrame Encode(FeatureDefinition command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (command.Value == null) return Read(command.Identifier);
            return command.Identifier switch
            {
                FeatureIdentifiers.NoiseControl => Noise(Require<NoiseModeId>(command)),
                FeatureIdentifiers.NoiseCancellationLevel => Dynamic(RoseDeviceFeatures.NoiseLevel, 0x2C, Byte(command)),
                FeatureIdentifiers.TransparencyLevel => Dynamic(RoseDeviceFeatures.NoiseLevel, 0x2D, Byte(command)),
                FeatureIdentifiers.Equalizer => Eq(command.Value),
                FeatureIdentifiers.LowLatency => Flag(RoseDeviceFeatures.LowLatency, 0x0E, Require<bool>(command), false),
                FeatureIdentifiers.AudioCodec => Audio(Require<AudioCodecMode>(command)),
                FeatureIdentifiers.Multipoint => Multipoint(Require<bool>(command)),
                FeatureIdentifiers.WearDetection => Flag(RoseDeviceFeatures.WearDetection, 0x08, Require<bool>(command), true),
                FeatureIdentifiers.TouchControls => Flag(RoseDeviceFeatures.TouchControls, 0x07, Require<bool>(command), true),
                FeatureIdentifiers.GestureMappings => Gesture(Require<GestureMapping>(command)),
                FeatureIdentifiers.FindDevice => Find(Require<FindDeviceAction>(command)),
                FeatureIdentifiers.PromptToneLevel => Dynamic(RoseDeviceFeatures.PromptToneLevel, 0x2E, Level(command)),
                FeatureIdentifiers.SpatialAudio => Spatial(Require<SpatialAudioMode>(command)),
                FeatureIdentifiers.DeviceLanguage => Language(Require<DeviceLanguage>(command)),
                FeatureIdentifiers.HearingProtection => Dynamic(RoseDeviceFeatures.HearingProtection, 0x46, Byte(command)),
                FeatureIdentifiers.GainLevel => Dynamic(RoseDeviceFeatures.NoiseLevel, 0x45, Byte(command)),
                _ => throw Unsupported(command.Identifier)
            };
        }

        public string? GetResponseIdentifier(RoseFrame frame) => frame.Group switch
        {
            0x09 => FeatureIdentifiers.NoiseControl,
            0x0E => FeatureIdentifiers.LowLatency,
            0x2A or 0x3E => FeatureIdentifiers.Equalizer,
            0x2B => FeatureIdentifiers.AudioCodec,
            0x32 => FeatureIdentifiers.Multipoint,
            0x08 => FeatureIdentifiers.WearDetection,
            0x07 => FeatureIdentifiers.TouchControls,
            0x01 => FeatureIdentifiers.GestureMappings,
            0x2C => FeatureIdentifiers.NoiseCancellationLevel,
            0x2D => FeatureIdentifiers.TransparencyLevel,
            0x2E => FeatureIdentifiers.PromptToneLevel,
            0x33 or 0x38 => FeatureIdentifiers.SpatialAudio,
            _ => null
        };

        private RoseFrame Read(string id)
        {
            if (id is FeatureIdentifiers.DeviceInformation or FeatureIdentifiers.Battery)
                return _family == RoseFamily.RsCommon
                    ? new RoseFrame(0xFA, 0x01, new byte[] { 0x07, 0x08, 0x09, 0x0C, 0x0D, 0x0E, 0x12, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x33 })
                    : new RoseFrame(0xFA, 0x01, new byte[] { 0x07, 0x08, 0x09, 0x0C, 0x0D, 0x0E, 0x12, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x31, 0x32, 0x33, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3F, 0x45, 0x46, 0x49, 0xFF });
            throw new NotSupportedException($"RS reads '{id}' through getInfo.");
        }

        private RoseFrame Noise(NoiseModeId mode)
        {
            Need(RoseDeviceFeatures.NoiseControl, FeatureIdentifiers.NoiseControl);
            return Frame(0x09, mode switch { NoiseModeId.NoiseCancellation => 1, NoiseModeId.Normal => 2, NoiseModeId.Transparency => 3, NoiseModeId.WindReduction => 4, _ => throw new ArgumentOutOfRangeException(nameof(mode)) });
        }

        private RoseFrame Eq(object value)
        {
            Need(RoseDeviceFeatures.Equalizer, FeatureIdentifiers.Equalizer);
            var id = value is EqualizerPreset p ? p.Id : value.ToString() ?? "";
            return Frame(0x2A, id.ToLowerInvariant() switch
            {
                "hifi" => 0,
                "pop" => 1,
                "rock" => 2,
                "light" when _family == RoseFamily.RsCommonV2 => 3,
                "custom" when _family == RoseFamily.RsCommonV2 => 4,
                "eap" when _family == RoseFamily.RsCommonV2 => 6,
                "game1" when _family == RoseFamily.RsCommonV2 => 0xA0,
                "game2" when _family == RoseFamily.RsCommonV2 => 0xA1,
                "game3" when _family == RoseFamily.RsCommonV2 => 0xA2,
                "game4" when _family == RoseFamily.RsCommonV2 => 0xA3,
                "game5" when _family == RoseFamily.RsCommonV2 => 0xA4,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            });
        }

        private RoseFrame Audio(AudioCodecMode mode)
        {
            Need(RoseDeviceFeatures.AudioCodec, FeatureIdentifiers.AudioCodec);
            return Frame(0x2B, mode switch { AudioCodecMode.AacSbc => 0, AudioCodecMode.Ldac => 1, AudioCodecMode.Lhdc => 2, AudioCodecMode.Lc3 => 3, AudioCodecMode.Aptx => 4, AudioCodecMode.AptxHd => 5, AudioCodecMode.AptxAdaptive => 6, _ => throw new ArgumentOutOfRangeException(nameof(mode)) });
        }

        private RoseFrame Multipoint(bool enabled)
        {
            Need(RoseDeviceFeatures.Multipoint, FeatureIdentifiers.Multipoint);
            return _family == RoseFamily.RsCommonV2 ? Frame(0x32, enabled ? 1 : 0) : Frame(0x2B, enabled ? 4 : 0);
        }

        private RoseFrame Gesture(GestureMapping value)
        {
            Need(RoseDeviceFeatures.TouchControls, FeatureIdentifiers.GestureMappings);
            var key = (byte)((value.Side == DeviceSide.Left ? 0 : 0x10) + (value.Gesture switch { DeviceGesture.Single => 1, DeviceGesture.Double => 2, DeviceGesture.Triple => 3, DeviceGesture.LongPress => 4, _ => throw new ArgumentOutOfRangeException() }));
            return new RoseFrame(0x01, key, new[] { (byte)value.Action });
        }

        private RoseFrame Find(FindDeviceAction value)
        {
            Need(RoseDeviceFeatures.FindDevice, FeatureIdentifiers.FindDevice);
            return Frame(0x2F, value switch { FindDeviceAction.Left => 1, FindDeviceAction.Right => 2, FindDeviceAction.Stop => 4, _ => throw new ArgumentOutOfRangeException() });
        }

        private RoseFrame Spatial(SpatialAudioMode value)
        {
            Need(RoseDeviceFeatures.SpatialAudio, FeatureIdentifiers.SpatialAudio);
            return value switch { SpatialAudioMode.Off => Frame(0x33, 0), SpatialAudioMode.HeadTracking => Frame(0x38, 1), SpatialAudioMode.Fixed => Frame(0x38, 2), _ => throw new ArgumentOutOfRangeException(nameof(value)) };
        }

        private RoseFrame Language(DeviceLanguage value)
        {
            Need(RoseDeviceFeatures.Language, FeatureIdentifiers.DeviceLanguage);
            return Frame(_family == RoseFamily.RsCommonV2 ? 0x36 : 0x2F, value == DeviceLanguage.English ? 1 : 0);
        }

        private RoseFrame Flag(RoseDeviceFeatures feature, byte group, bool enabled, bool inverted) { Need(feature, group.ToString("X2")); return Frame(group, (enabled ^ inverted) ? 1 : 0); }
        private RoseFrame Dynamic(RoseDeviceFeatures feature, byte group, byte value) { Need(feature, group.ToString("X2")); return Frame(group, value); }
        private static RoseFrame Frame(int group, int command) => new((byte)group, (byte)command, Array.Empty<byte>());
        private void Need(RoseDeviceFeatures feature, string id) { if ((_features & feature) == 0) throw Unsupported(id); }
        private NotSupportedException Unsupported(string id) => new($"This device does not advertise feature '{id}'.");
        private static byte Level(FeatureDefinition c) { var v = Require<int>(c); if (v < 1 || v > 255) throw new ArgumentOutOfRangeException(nameof(c)); return (byte)(v - 1); }
        private static byte Byte(FeatureDefinition c) { var v = Require<int>(c); if (v < 0 || v > 255) throw new ArgumentOutOfRangeException(nameof(c)); return (byte)v; }
        private static T Require<T>(FeatureDefinition c) => c.Value is T v ? v : throw new ArgumentException($"Feature '{c.Identifier}' expects {typeof(T).Name}.", nameof(c));
    }
}
