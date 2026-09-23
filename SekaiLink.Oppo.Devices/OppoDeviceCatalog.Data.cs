using System;

namespace SekaiLink.Oppo.Devices;

// Snapshot derived from OppoPodsManager Data/DeviceModels.json (137 whitelist entries).
// Review the upstream GPL-3.0-or-later license before redistributing this derived catalog.
public static partial class OppoDeviceCatalog
{
    private static partial OppoDeviceDefinition[] CreateGeneratedDefinitions()
    {
        return new[]
        {
            new OppoDeviceDefinition("06F010", "OPPO Enco Air4s", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06B450", "OPPO Enco R5", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel |
                OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 22), new OppoEqualizerOption(1, 32), new OppoEqualizerOption(2, 31) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("066812", "realme Buds Air8", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.WearDetection |
                OppoDeviceFeatures.FindDevice, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("067414", "OnePlus Flow Buds", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.Multipoint |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Light", 6),
                    new OppoNoiseOption("Off", 0), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06E810", "OPPO Enco Air5s（星光版）", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.Multipoint |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Light", 6),
                    new OppoNoiseOption("Off", 0), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 26), new OppoEqualizerOption(1, 29), new OppoEqualizerOption(2, 28) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06C810", "OPPO Enco Air5s", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.Multipoint |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Light", 6),
                    new OppoNoiseOption("Off", 0), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 26), new OppoEqualizerOption(1, 29), new OppoEqualizerOption(2, 28) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("067014", "OnePlus Nord Buds 4", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.Multipoint |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 14), new OppoEqualizerOption(2, 12) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06E010", "OPPO Enco Clip2", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.Multipoint |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate,
                Array.Empty<OppoNoiseOption>(),
                new[]
                {
                    new OppoEqualizerOption(0, 26), new OppoEqualizerOption(2, 28), new OppoEqualizerOption(3, 34),
                    new OppoEqualizerOption(4, 30)
                }, new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06D810", "OPPO Enco Air5", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.Multipoint |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 26), new OppoEqualizerOption(1, 28), new OppoEqualizerOption(2, 29) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06C410", "OPPO Enco Air5 Pro", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.Multipoint |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 26), new OppoEqualizerOption(1, 29), new OppoEqualizerOption(2, 28) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("066414", "OnePlus Buds Ace 3", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.Multipoint |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("066814", "OnePlus Nord Buds 4 Pro", "OnePlus",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.Multipoint |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("066054", "OnePlus Buds 3V", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("065854", "OnePlus Nord Buds 3r", "OnePlus",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel |
                OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06B810", "OPPO Enco Clip", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.Multipoint |
                OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(),
                new[]
                {
                    new OppoEqualizerOption(0, 35), new OppoEqualizerOption(1, 34), new OppoEqualizerOption(2, 28),
                    new OppoEqualizerOption(3, 12)
                }, new byte[] { 0, 1 }),
            new OppoDeviceDefinition("065C14", "OnePlus Open Buds", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.Multipoint |
                OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(),
                new[]
                {
                    new OppoEqualizerOption(0, 35), new OppoEqualizerOption(1, 34), new OppoEqualizerOption(2, 28),
                    new OppoEqualizerOption(3, 12)
                }, new byte[] { 0, 1 }),
            new OppoDeviceDefinition("061410", "OPPO Enco X", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("Deep", 3), new OppoNoiseOption("Light", 2), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 1)
                },
                new[]
                    { new OppoEqualizerOption(0, 5), new OppoEqualizerOption(1, 6), new OppoEqualizerOption(2, 7) },
                Array.Empty<byte>()),
            new OppoDeviceDefinition("060410", "OPPO Enco Free", "oppo", "00001107-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("062010", "OPPO Enco Free2", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2),
                    new OppoNoiseOption("Adaptive", 3)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3),
                    new OppoEqualizerOption(3, 4)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("061C10", "OPPO Enco Play", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("061810", "OPPO Enco Air", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("062410", "OPPO Enco Buds", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FirmwareUpdate,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("062810", "OPPO Enco Air Lite", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FirmwareUpdate,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("060C10", "OPPO Enco W51", "oppo", "00001107-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.WearDetection |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("060810", "OPPO Enco W31", "oppo", "00001107-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FirmwareUpdate,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("061010", "OPPO Enco W11", "oppo", "00001107-D102-11E1-9B23-00025B00A5A5", false,
                1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("060412", "OPPO O-Free", "oppo", "00001107-D102-11E1-9B23-00025B00A5A5", false, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("062C10", "OPPO Enco W31 Lite", "oppo", "00001107-D102-11E1-9B23-00025B00A5A5",
                false, 1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("050410", "OPPO Enco M31", "oppo", "00001107-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FirmwareUpdate,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("050010", "OPPO Enco Quiet", "oppo", "00001107-D102-11E1-9B23-00025B00A5A5", false,
                1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("060414", "OnePlus Buds", "OnePlus", "00001107-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("060814", "OnePlus Buds Z", "OnePlus", "00001107-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("3D59CF", "realme Buds Air", "realme", null, true, 2,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("F85450", "realme Buds Air Neo", "realme", null, true, 2,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("E3009F", "realme Buds Wireless Pro", "realme", null, true, 2,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("8CD10F", "realme Buds Air Pro", "realme", "00001101-0000-1000-8000-00805F9B34FB",
                true, 2, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("BA5D56", "realme Buds Air 2", "realme", "00001101-0000-1000-8000-00805F9B34FB",
                true, 2, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("1B5374", "realme Buds Air 2 Neo", "realme",
                "00001101-0000-1000-8000-00805F9B34FB", true, 2,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("C1E2B1", "realme Buds Wireless 2", "realme",
                "00001101-0000-1000-8000-00805F9B34FB", true, 2,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("C70A6D", "realme Buds Wireless 2 Neo", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 2,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("693878", "realme Buds Q2", "realme", null, true, 2,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("49074B", "realme Cobble Speaker", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 2,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("C70A75", "realme Pocket Speaker", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 2,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("A5FD66", "realme Brick Speaker", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 2, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("060812", "realme Buds Air 3", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("060C12", "realme Buds Q2s", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("050412", "realme Buds Wireless 2S", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("061812", "realme Buds Air 3 Neo", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("061412", "realme Buds T100", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("061012", "realme Buds Air 3S", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("061C12", "realme Buds Air 5 Pro", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.WearDetection |
                OppoDeviceFeatures.FindDevice, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("063012", "realme Buds Air6 Pro", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.WearDetection |
                OppoDeviceFeatures.FindDevice, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("050812", "realme Buds Wireless 3", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("062012", "realme Buds Air 5", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.WearDetection |
                OppoDeviceFeatures.FindDevice, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("063412", "realme Buds Air6", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.WearDetection |
                OppoDeviceFeatures.FindDevice, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("063812", "realme Buds N1 Pro", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("063C12", "realme Buds T310", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("064012", "realme Buds N1", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("062412", "realme Buds T300", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("064812", "realme Buds Air7", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.WearDetection |
                OppoDeviceFeatures.FindDevice, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("064C12", "realme Buds Air7 Pro", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.WearDetection |
                OppoDeviceFeatures.FindDevice, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("066C12", "realme Buds Air8 Pro", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.WearDetection |
                OppoDeviceFeatures.FindDevice, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("066452", "realme Buds T500 Pro", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.WearDetection |
                OppoDeviceFeatures.FindDevice, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("067452", "realme Buds T500 Pro Harry Potter Edition", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.WearDetection |
                OppoDeviceFeatures.FindDevice, Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(),
                Array.Empty<byte>()),
            new OppoDeviceDefinition("067052", "realme Buds T500", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("065C12", "realme Buds Clip", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("065452", "realme Buds T200", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("062812", "realme Buds T110", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("064412", "realme Buds T01", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("065052", "realme Buds T200 Lite", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("065852", "realme Buds T200x", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("066012", "realme TechLife Buds", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("050C12", "realme Buds Wireless 3 Neo", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("051412", "realme Buds Wireless 5 ANC", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("052052", "realme Buds Wireless 6 ANC", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("051812", "realme Buds Wireless 6 Neo", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("051C52", "realme Buds Wireless 6", "realme",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FindDevice,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("C70A6E", "DIZO Wireless", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                2, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("C70A6F", "DIZO GoPods D", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                2, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("C70A70", "DIZO GoPods", "realme", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 2,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("060C14", "OnePlus Buds Pro", "OnePlus", "00001107-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 4), new OppoNoiseOption("Deep", 3), new OppoNoiseOption("Light", 2),
                    new OppoNoiseOption("Off", 0), new OppoNoiseOption("Transparency", 1)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14),
                    new OppoEqualizerOption(3, 13)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("062014", "OnePlus Buds Pro 2", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14),
                    new OppoEqualizerOption(3, 13), new OppoEqualizerOption(7, 19)
                }, new byte[] { 0, 1, 2 }),
            new OppoDeviceDefinition("063014", "OnePlus Buds Pro 2（轻享版）", "OnePlus",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14),
                    new OppoEqualizerOption(3, 13), new OppoEqualizerOption(7, 19)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("063414", "OnePlus Buds Pro 2R", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14),
                    new OppoEqualizerOption(3, 13), new OppoEqualizerOption(7, 19)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("064C10", "OPPO Enco Air3", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate,
                Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("065C10", "OPPO Enco Air3 Pro", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 20) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("065810", "OPPO Enco R2", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate,
                Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 21), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("066010", "OPPO Enco Free3", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 20) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("061014", "OnePlus Buds Z2", "OnePlus", "00001107-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Deep", 3), new OppoNoiseOption("Light", 2), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 1)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14),
                    new OppoEqualizerOption(3, 13)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("050810", "OPPO Enco M32", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", false,
                1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("063010", "OPPO Enco R", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(),
                new[]
                {
                    new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3),
                    new OppoEqualizerOption(3, 4)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("063410", "OPPO Enco Air2", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                Array.Empty<byte>()),
            new OppoDeviceDefinition("061C14", "OnePlus Nord Buds CE", "OnePlus",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(),
                new[]
                {
                    new OppoEqualizerOption(0, 17), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14),
                    new OppoEqualizerOption(3, 15)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("064010", "OPPO Enco Free2i", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2),
                    new OppoNoiseOption("Adaptive", 3)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3),
                    new OppoEqualizerOption(3, 4)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("068414", "OnePlus Buds", "OnePlus", "00001107-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FirmwareUpdate,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("063C10", "OPPO Enco X2", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 8), new OppoNoiseOption("Adaptive", 9)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 9), new OppoEqualizerOption(1, 6), new OppoEqualizerOption(2, 8),
                    new OppoEqualizerOption(3, 10), new OppoEqualizerOption(4, 16)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("068814", "OnePlus Buds Z", "OnePlus", "00001107-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.FirmwareUpdate,
                Array.Empty<OppoNoiseOption>(), Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("063810", "OPPO Enco Air2 Pro", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2),
                    new OppoNoiseOption("Adaptive", 3)
                },
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                Array.Empty<byte>()),
            new OppoDeviceDefinition("050414", "OnePlus Bullets Wireless Z2", "OnePlus",
                "00001107-D102-11E1-9B23-00025B00A5A5", false, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation, Array.Empty<OppoNoiseOption>(),
                Array.Empty<OppoEqualizerOption>(), Array.Empty<byte>()),
            new OppoDeviceDefinition("061414", "OnePlus Nord Buds", "OnePlus", "00001107-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                Array.Empty<OppoNoiseOption>(),
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14),
                    new OppoEqualizerOption(3, 13)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("061814", "OnePlus Buds N", "OnePlus", "00001107-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                Array.Empty<OppoNoiseOption>(),
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14),
                    new OppoEqualizerOption(3, 13)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("064410", "OPPO Enco Air2i", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer,
                Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                Array.Empty<byte>()),
            new OppoDeviceDefinition("064810", "OPPO Enco Buds2", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer,
                Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                Array.Empty<byte>()),
            new OppoDeviceDefinition("065010", "OPPO Enco R Pro", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2),
                    new OppoNoiseOption("Adaptive", 3)
                },
                new[]
                    { new OppoEqualizerOption(0, 18), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                Array.Empty<byte>()),
            new OppoDeviceDefinition("062414", "OnePlus Nord Buds 2", "OnePlus", "00001107-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 13),
                    new OppoEqualizerOption(3, 14)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("062C14", "OnePlus Nord Buds 2r", "OnePlus",
                "00001107-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.CustomEqualizer, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 13), new OppoEqualizerOption(2, 12) },
                Array.Empty<byte>()),
            new OppoDeviceDefinition("062814", "OnePlus Buds Ace", "OnePlus", "00001107-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 13) },
                Array.Empty<byte>()),
            new OppoDeviceDefinition("066C10", "OPPO Enco Air2（新声版）", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.SpatialAudio, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("067010", "OPPO Enco Air3i", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.SpatialAudio, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("050C10", "OPPO Enco M33", "oppo", "00001107-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 13),
                    new OppoEqualizerOption(3, 3)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("050C14", "OnePlus BulletsWireless Z2 ANC", "OnePlus",
                "00001107-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 13),
                    new OppoEqualizerOption(3, 14)
                }, Array.Empty<byte>()),
            new OppoDeviceDefinition("051014", "OnePlus Bullets Wireless Z3", "OnePlus",
                "00001107-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(),
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 13),
                    new OppoEqualizerOption(3, 14)
                }, new byte[] { 0, 1 }),
            new OppoDeviceDefinition("063C14", "OnePlus Buds 3", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 12), new OppoEqualizerOption(2, 14),
                    new OppoEqualizerOption(3, 13)
                }, new byte[] { 0, 1 }),
            new OppoDeviceDefinition("066810", "OPPO Enco X3i", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3),
                    new OppoEqualizerOption(3, 13)
                }, new byte[] { 0, 1 }),
            new OppoDeviceDefinition("067410", "OPPO Enco X3", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 8), new OppoNoiseOption("Adaptive", 9)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 26), new OppoEqualizerOption(1, 27), new OppoEqualizerOption(2, 28),
                    new OppoEqualizerOption(3, 29), new OppoEqualizerOption(7, 30)
                }, new byte[] { 0, 1, 2 }),
            new OppoDeviceDefinition("064014", "OnePlus Buds Pro 3", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound |
                OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 13), new OppoEqualizerOption(2, 14),
                    new OppoEqualizerOption(3, 12), new OppoEqualizerOption(7, 30)
                }, new byte[] { 0, 1, 2 }),
            new OppoDeviceDefinition("067810", "OPPO Enco Air3s", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate,
                Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("068010", "OPPO Enco Buds2 Pro", "oppo", "00001107-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.FindDevice, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 13), new OppoEqualizerOption(2, 12) },
                Array.Empty<byte>()),
            new OppoDeviceDefinition("064814", "OnePlus Buds V", "OnePlus", "00001107-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.CustomEqualizer, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 13), new OppoEqualizerOption(2, 12) },
                Array.Empty<byte>()),
            new OppoDeviceDefinition("068810", "OPPO Enco Air 3i", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1, OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer,
                Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                Array.Empty<byte>()),
            new OppoDeviceDefinition("068410", "OPPO Enco R3", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.SpatialAudio, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 25), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("067C10", "OPPO Enco Air4 Pro", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("064414", "OnePlus Nord Buds 3 Pro", "OnePlus",
                "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(3, 11), new OppoEqualizerOption(4, 14), new OppoEqualizerOption(5, 12) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("069010", "OPPO Enco Air4", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("065014", "OnePlus Nord Buds 3", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(3, 11), new OppoEqualizerOption(4, 14), new OppoEqualizerOption(5, 12) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("069C10", "OPPO Enco R3 Pro", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5),
                    new OppoNoiseOption("Light", 6), new OppoNoiseOption("Off", 0),
                    new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 33), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06A010", "OPPO Enco Buds3 Pro+", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice |
                OppoDeviceFeatures.FirmwareUpdate,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(0, 1), new OppoEqualizerOption(1, 2), new OppoEqualizerOption(2, 3) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("064C14", "OnePlus Buds Ace 2", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.FirmwareUpdate | OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("NC", 0), new OppoNoiseOption("Off", 1), new OppoNoiseOption("Transparency", 2)
                },
                new[]
                    { new OppoEqualizerOption(3, 11), new OppoEqualizerOption(4, 14), new OppoEqualizerOption(5, 12) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("068C10", "OPPO Enco Free4", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5), new OppoNoiseOption("Light", 6),
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Adaptive", 11),
                    new OppoNoiseOption("Transparency", 2), new OppoNoiseOption("Off", 0)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 26), new OppoEqualizerOption(1, 28), new OppoEqualizerOption(2, 29),
                    new OppoEqualizerOption(7, 34)
                }, new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06C010", "OPPO Enco Free4（丹拿版）", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5), new OppoNoiseOption("Light", 6),
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Adaptive", 11),
                    new OppoNoiseOption("Transparency", 2), new OppoNoiseOption("Off", 0)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 26), new OppoEqualizerOption(1, 28), new OppoEqualizerOption(2, 29),
                    new OppoEqualizerOption(3, 30), new OppoEqualizerOption(7, 34), new OppoEqualizerOption(8, 40)
                }, new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06BC10", "OPPO Enco X3s", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5), new OppoNoiseOption("Light", 6),
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Adaptive", 11),
                    new OppoNoiseOption("Transparency", 2), new OppoNoiseOption("Off", 0)
                },
                new[]
                {
                    new OppoEqualizerOption(0, 26), new OppoEqualizerOption(1, 28), new OppoEqualizerOption(2, 29),
                    new OppoEqualizerOption(3, 30), new OppoEqualizerOption(8, 40)
                }, new byte[] { 0, 1 }),
            new OppoDeviceDefinition("065414", "OnePlus Buds 4", "OnePlus", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.NoiseControl |
                OppoDeviceFeatures.Equalizer | OppoDeviceFeatures.WearDetection | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer,
                new[]
                {
                    new OppoNoiseOption("Deep", 4), new OppoNoiseOption("Medium", 5), new OppoNoiseOption("Light", 6),
                    new OppoNoiseOption("Smart", 7), new OppoNoiseOption("Adaptive", 11),
                    new OppoNoiseOption("Transparency", 2), new OppoNoiseOption("Off", 0)
                },
                new[]
                    { new OppoEqualizerOption(0, 11), new OppoEqualizerOption(1, 14), new OppoEqualizerOption(2, 12) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06A850", "OPPO Enco Buds3 Pro", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 22), new OppoEqualizerOption(1, 32), new OppoEqualizerOption(2, 31) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("069850", "OPPO Enco Air4i", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 22), new OppoEqualizerOption(1, 32), new OppoEqualizerOption(2, 31) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06AC50", "OPPO Enco R4", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.LowLatency | OppoDeviceFeatures.GameSound | OppoDeviceFeatures.SpatialAudio |
                OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel | OppoDeviceFeatures.FirmwareUpdate |
                OppoDeviceFeatures.CustomEqualizer, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 22), new OppoEqualizerOption(1, 32), new OppoEqualizerOption(2, 31) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06A450", "OPPO Enco Buds3", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5", true,
                1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel |
                OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 22), new OppoEqualizerOption(1, 32), new OppoEqualizerOption(2, 31) },
                new byte[] { 0, 1 }),
            new OppoDeviceDefinition("06B050", "OPPO Enco Air4（新声版）", "oppo", "0000079A-D102-11E1-9B23-00025B00A5A5",
                true, 1,
                OppoDeviceFeatures.Battery | OppoDeviceFeatures.DeviceInformation | OppoDeviceFeatures.Equalizer |
                OppoDeviceFeatures.SpatialAudio | OppoDeviceFeatures.FindDevice | OppoDeviceFeatures.PromptToneLevel |
                OppoDeviceFeatures.FirmwareUpdate, Array.Empty<OppoNoiseOption>(),
                new[]
                    { new OppoEqualizerOption(0, 22), new OppoEqualizerOption(1, 32), new OppoEqualizerOption(2, 31) },
                new byte[] { 0, 1 })
        };
    }
}