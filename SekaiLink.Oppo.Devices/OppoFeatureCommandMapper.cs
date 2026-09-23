using System;
using System.Collections.Generic;
using System.Linq;
using SekaiLink.Oppo.Protocol.Protocols;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;

namespace SekaiLink.Oppo.Devices
{
    internal sealed class OppoFeatureCommandMapper : IFeatureCommandMapper<OppoFrame>
    {
        private readonly OppoDeviceDefinition _device;
        public OppoFeatureCommandMapper(OppoDeviceDefinition device) => _device = device;

        public IReadOnlyList<FeatureDefinition> InitializationCommands { get; } = new[]
        {
            new FeatureDefinition(FeatureIdentifiers.Battery),
            new FeatureDefinition(FeatureIdentifiers.DeviceInformation)
        };

        public OppoFrame Encode(FeatureDefinition command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            return command.Value == null ? Read(command.Identifier) : Write(command);
        }

        public string? GetResponseIdentifier(OppoFrame frame) => frame.Command switch
        {
            0x8106 => FeatureIdentifiers.Battery,
            0x8105 => FeatureIdentifiers.DeviceInformation,
            0x810C => FeatureIdentifiers.NoiseControl,
            0x810F or 0x0504 => FeatureIdentifiers.Equalizer,
            0x812A => FeatureIdentifiers.SpatialAudio,
            0x810D => null, // Multiplexed feature response; its payload must be dispatched by feature ID.
            _ => null
        };

        private OppoFrame Read(string id) => id switch
        {
            FeatureIdentifiers.Battery => OppoCommands.QueryBattery(),
            FeatureIdentifiers.DeviceInformation => new OppoFrame(0x0105, Array.Empty<byte>()),
            FeatureIdentifiers.NoiseControl when _device.Supports(OppoDeviceFeatures.NoiseControl) => OppoCommands.QueryNoiseMode(),
            FeatureIdentifiers.Equalizer when _device.Supports(OppoDeviceFeatures.Equalizer) => new OppoFrame(OppoCommands.QueryEqualizer, Array.Empty<byte>()),
            FeatureIdentifiers.SpatialAudio when _device.Supports(OppoDeviceFeatures.SpatialAudio) => new OppoFrame(0x012A, Array.Empty<byte>()),
            FeatureIdentifiers.LowLatency when _device.Supports(OppoDeviceFeatures.LowLatency) => QueryFeature(GameFeature),
            FeatureIdentifiers.WearDetection when _device.Supports(OppoDeviceFeatures.WearDetection) => QueryFeature(0x04),
            _ => throw Unsupported(id)
        };

        private OppoFrame Write(FeatureDefinition command) => command.Identifier switch
        {
            FeatureIdentifiers.NoiseControl when _device.Supports(OppoDeviceFeatures.NoiseControl) => SetNoise(command.Value!),
            FeatureIdentifiers.Equalizer when _device.Supports(OppoDeviceFeatures.Equalizer) => SetEqualizer(command.Value!),
            FeatureIdentifiers.LowLatency when _device.Supports(OppoDeviceFeatures.LowLatency) => SetFeature(GameFeature, Require<bool>(command)),
            FeatureIdentifiers.WearDetection when _device.Supports(OppoDeviceFeatures.WearDetection) => SetFeature(0x04, Require<bool>(command)),
            FeatureIdentifiers.SpatialAudio when _device.Supports(OppoDeviceFeatures.SpatialAudio) => SetSpatial(command.Value!),
            _ => throw Unsupported(command.Identifier)
        };

        private byte GameFeature => _device.Supports(OppoDeviceFeatures.GameSound) ? (byte)0x28 : (byte)0x06;
        private static OppoFrame QueryFeature(byte feature) => new OppoFrame(0x010D, new[] { (byte)1, feature });
        private static OppoFrame SetFeature(byte feature, bool enabled) => OppoCommands.SetFeatureValue(feature, enabled);

        private OppoFrame SetNoise(object value)
        {
            string key;
            if (value is NoiseModeId mode)
                key = mode switch
                {
                    NoiseModeId.Normal => "Off",
                    NoiseModeId.Transparency => "Transparency",
                    NoiseModeId.AdaptiveNoiseCancellation => "Adaptive",
                    NoiseModeId.NoiseCancellation => "NC",
                    _ => throw new ArgumentOutOfRangeException(nameof(value))
                };
            else key = value is OppoNoiseOption option ? option.Key : value as string ?? throw new ArgumentException("Expected a noise mode.");

            // Older models call their main cancellation mode Smart instead of NC.
            if (key == "NC" && !_device.NoiseOptions.Any(option => option.IsSelectable && option.Key == "NC") &&
                _device.NoiseOptions.Any(option => option.IsSelectable && option.Key == "Smart")) key = "Smart";

            foreach (var option in _device.NoiseOptions)
            {
                if (!option.IsSelectable || !string.Equals(option.Key, key, StringComparison.OrdinalIgnoreCase)) continue;
                var payload = new byte[3 + option.ProtocolIndex / 8];
                payload[0] = payload[1] = 1;
                payload[2 + option.ProtocolIndex / 8] = (byte)(1 << (option.ProtocolIndex % 8));
                return new OppoFrame(OppoCommands.SetNoise, payload);
            }
            throw new ArgumentOutOfRangeException(nameof(value), "This model does not expose that noise mode.");
        }

        private OppoFrame SetEqualizer(object value)
        {
            var id = value is EqualizerPreset preset ? preset.Id : value as string;
            if (id == null) throw new ArgumentException("Expected an EQ preset ID.", nameof(value));
            foreach (var option in _device.EqualizerOptions)
                if (string.Equals(id, option.Id, StringComparison.OrdinalIgnoreCase))
                    return new OppoFrame(OppoCommands.SetEqualizer, new[] { option.ProtocolIndex });
            throw new ArgumentOutOfRangeException(nameof(value), "This model does not expose that EQ preset.");
        }

        private OppoFrame SetSpatial(object value)
        {
            var mode = value is SpatialAudioMode spatial ? spatial.ToString() : value as string;
            var index = mode?.ToLowerInvariant() switch
            {
                "off" => 0,
                "fixed" => 1,
                "headtracking" or "track" => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
            if (!_device.SpatialTypes.Contains((byte)index)) throw new ArgumentOutOfRangeException(nameof(value));
            return new OppoFrame(OppoCommands.SpatialAudio, new[] { (byte)index });
        }

        private static T Require<T>(FeatureDefinition command) => command.Value is T value
            ? value : throw new ArgumentException($"Feature '{command.Identifier}' expects {typeof(T).Name}.");
        private static NotSupportedException Unsupported(string id) => new NotSupportedException($"No verified OPPO mapping for '{id}'.");
    }
}
