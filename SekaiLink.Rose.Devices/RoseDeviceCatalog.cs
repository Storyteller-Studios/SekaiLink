using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SekaiLink.Protocols.Features;
using SekaiLink.Protocols.Models;
using SekaiLink.Rose.Devices.Devices.Auramic;
using SekaiLink.Rose.Devices.Devices.Common;
using SekaiLink.Rose.Devices.Devices.Zt;
using SekaiLink.Rose.Protocol.Protocols;

namespace SekaiLink.Rose.Devices;

/// Device catalogue recovered from cn.ikaile.ruoshui.client 3.8.0 (versionCode 5310).
public static class RoseDeviceCatalog
{
    private static readonly Guid CeramicsServiceUuid = new("00007034-0000-1000-8000-00805F9B34FB");

    private static readonly RoseGattConfiguration F8Gatt = new(
        RoseProtocolUuids.Fdb3ServiceUuid, RoseProtocolUuids.Ff16WriteUuid, RoseProtocolUuids.Ff17NotifyUuid);

    private static readonly RoseGattConfiguration RsCommonGatt = new(
        RoseProtocolUuids.RsCommonServiceUuid, RoseProtocolUuids.RsCommonWriteUuid,
        RoseProtocolUuids.RsCommonNotifyUuid);

    private static readonly RoseGattConfiguration ZtGatt = new(
        RoseProtocolUuids.ZtServiceUuid, RoseProtocolUuids.ZtWriteUuid, RoseProtocolUuids.ZtNotifyUuid);

    private static readonly RoseGattConfiguration CeramicsGatt = new(
        CeramicsServiceUuid, RoseProtocolUuids.Ff16WriteUuid, RoseProtocolUuids.Ff17NotifyUuid);

    private static readonly RoseGattConfiguration BudsfeelMk2Gatt = new(
        new Guid("0000AE00-0000-1000-8000-00805F9B34FB"),
        new Guid("0000AE01-0000-1000-8000-00805F9B34FB"),
        new Guid("0000AE02-0000-1000-8000-00805F9B34FB"));

    public static IReadOnlyList<RoseDeviceDefinition> All { get; } = CreateDefinitions();

    public static RoseDeviceDefinition? Find(RoseDeviceType type)
    {
        return All.FirstOrDefault(device => device.Type == type);
    }

    public static RoseDeviceDefinition? Find(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        var normalized = Normalize(id);
        return All.FirstOrDefault(device => IdentityAliases(device).Any(alias => Normalize(alias) == normalized));
    }

    /// Resolves a device only when there is one best match. Shared model codes or services deliberately return false.
    public static bool TryResolve(DeviceIdentity identity, out RoseDeviceDefinition? definition)
    {
        if (identity == null) throw new ArgumentNullException(nameof(identity));

        RoseDeviceDefinition? best = null;
        var bestScore = 0;
        var tied = false;
        foreach (var candidate in All)
        {
            var score = Score(candidate, identity);
            if (score > bestScore)
            {
                best = candidate;
                bestScore = score;
                tied = false;
            }
            else if (score > 0 && score == bestScore)
            {
                tied = true;
            }
        }

        definition = bestScore > 0 && !tied ? best : null;
        return definition != null;
    }

    private static int Score(RoseDeviceDefinition candidate, DeviceIdentity identity)
    {
        var score = 0;
        score = Math.Max(score, ScoreIdentityText(identity.Model, candidate, 1000, 620));
        score = Math.Max(score, ScoreIdentityText(identity.Name, candidate, 900, 520));

        if (!string.IsNullOrWhiteSpace(candidate.HardwareModel))
        {
            score = Math.Max(score, ScoreToken(identity.Model, candidate.HardwareModel!, 860, 800));
            score = Math.Max(score, ScoreToken(identity.Name, candidate.HardwareModel!, 840, 780));
        }

        foreach (var pair in identity.AdvertisementData)
        {
            foreach (var signature in candidate.AdvertisementSignatures)
            {
                score = Math.Max(score, ScoreToken(pair.Key, signature, 1200, 1120));
                score = Math.Max(score, ScoreToken(pair.Value, signature, 1200, 1120));
            }

            if (!string.IsNullOrWhiteSpace(candidate.HardwareModel))
                score = Math.Max(score, ScoreToken(pair.Value, candidate.HardwareModel!, 880, 820));
        }

        foreach (var service in identity.ServiceIds)
        {
            var normalizedService = Normalize(service);
            if (candidate.ServiceUuids.Any(value => Normalize(value.ToString()) == normalizedService))
                score = Math.Max(score, 160);
            if (candidate.GattConfiguration != null &&
                Normalize(candidate.GattConfiguration.ServiceUuid.ToString()) == normalizedService)
                score = Math.Max(score, 180);
        }

        return score;
    }

    private static int ScoreIdentityText(string? value, RoseDeviceDefinition candidate, int exactScore,
        int containsScore)
    {
        var best = 0;
        foreach (var alias in IdentityAliases(candidate))
            best = Math.Max(best,
                ScoreToken(value, alias, exactScore + Normalize(alias).Length,
                    containsScore + Normalize(alias).Length));
        return best;
    }

    private static int ScoreToken(string? value, string token, int exactScore, int containsScore)
    {
        if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(token)) return 0;
        var normalizedValue = Normalize(value);
        var normalizedToken = Normalize(token);
        if (normalizedToken.Length < 4) return 0;
        if (normalizedValue == normalizedToken) return exactScore;
        return normalizedValue.IndexOf(normalizedToken, StringComparison.Ordinal) >= 0 ? containsScore : 0;
    }

    private static IEnumerable<string> IdentityAliases(RoseDeviceDefinition definition)
    {
        yield return definition.Id;
        yield return definition.DisplayName;
        yield return definition.Type.ToString();
        foreach (var alias in definition.Aliases) yield return alias;
    }

    private static string Normalize(string value)
    {
        var result = new StringBuilder(value.Length);
        foreach (var character in value)
            if (char.IsLetterOrDigit(character))
                result.Append(char.ToLowerInvariant(character));
        return result.ToString();
    }

    private static IReadOnlyList<RoseDeviceDefinition> CreateDefinitions()
    {
        const RoseDeviceFeatures B = RoseDeviceFeatures.Battery | RoseDeviceFeatures.DeviceInformation;
        const RoseDeviceFeatures N = RoseDeviceFeatures.NoiseControl;
        const RoseDeviceFeatures L = RoseDeviceFeatures.NoiseLevel;
        const RoseDeviceFeatures E = RoseDeviceFeatures.Equalizer;
        const RoseDeviceFeatures G = RoseDeviceFeatures.LowLatency;
        const RoseDeviceFeatures W = RoseDeviceFeatures.WearDetection;
        const RoseDeviceFeatures T = RoseDeviceFeatures.TouchControls;
        const RoseDeviceFeatures S = RoseDeviceFeatures.SpatialAudio;
        const RoseDeviceFeatures M = RoseDeviceFeatures.Multipoint;
        const RoseDeviceFeatures F = RoseDeviceFeatures.FindDevice;
        const RoseDeviceFeatures P = RoseDeviceFeatures.PromptToneLevel;
        const RoseDeviceFeatures O = RoseDeviceFeatures.FirmwareUpdate;
        const RoseDeviceFeatures A = RoseDeviceFeatures.AudioCodec;
        const RoseDeviceFeatures D = RoseDeviceFeatures.Language;
        const RoseDeviceFeatures H = RoseDeviceFeatures.HearingProtection;
        const RoseDeviceFeatures V = RoseDeviceFeatures.VoiceAssistant;
        const RoseDeviceFeatures X = RoseDeviceFeatures.AdaptiveNoiseControl;

        return new[]
        {
            Device(RoseDeviceType.Ceramics, "ceramics", "Ceramics", RoseFamily.F8, null, Values("18840100", "18840103"),
                B | E | G | T),
            Device(RoseDeviceType.CeramicsX, "CERAMICS-X", "Ceramics X", RoseFamily.F8, "24J51R2V9918",
                Values("18840109"), B | N | E | G | T | M | O | A | D | V, gatt: CeramicsGatt),
            Device(RoseDeviceType.CeramicsX7034, "CERAMICS-X-7034", "Ceramics X 7034", RoseFamily.F8, "24J51R2V9918",
                Values("18840112"), B | N | E | G | T | M | O | A | D | V, gatt: CeramicsGatt),
            Device(RoseDeviceType.Agate, "agate", "Agate", RoseFamily.F8, null, Values("18840505"), B | E | G | T),
            Device(RoseDeviceType.AgateX, "agatex", "Agate X", RoseFamily.F8, "25J51R2V3265", Values("18840506"),
                B | E | T, Values("agate-x")),
            Device(RoseDeviceType.Explorer, "EXPLORER", "Explorer", RoseFamily.F8, "25J5151G1183", Values("18840107"),
                B | N | E | G | W | T | M | F | P | O | A | D, gatt: CeramicsGatt),

            Device(RoseDeviceType.Budsfree, "budsfree", "Budsfree", RoseFamily.RsCommon, "24J51R2VC019",
                Values("12cc3e4c"), B | N | E | G | T | A),
            Device(RoseDeviceType.Swift, "swift", "Swift", RoseFamily.RsCommon, "2023DP10823", Values("0b0b0a0b"),
                B | G | T | F),
            Device(RoseDeviceType.Cambrian, "cambrian", "Cambrian", RoseFamily.RsCommon, "25J5151GL088",
                Values("0b0c0a0c"), B | N | E | G | T | M | F | P | O | A | V),
            Device(RoseDeviceType.Openfun, "openfun", "Openfun", RoseFamily.RsCommon, "25J5151GT816",
                Values("0b0a0a0e", "0b0a0a10"), B | G | T | M | F | P, Values("open-fun")),
            Device(RoseDeviceType.Sportfree, "sportfree", "Sportfree", RoseFamily.RsCommon, "26J51U5AK389",
                Values("0b0a0a0f"), B | G | T | S | M | F | P, Values("sport-free")),
            Device(RoseDeviceType.Openfree, "OpenFree", "Openfree", RoseFamily.RsCommon, "26J51U5AR748",
                Values("0b0a0a0b", "0b0a0a0c", "0b0a0a0d"), B | E | G | T | M | A, Values("open-free")),

            Device(RoseDeviceType.CeramicsMk2, "ceramics-mk2", "Ceramics MK2", RoseFamily.RsCommonV2, null,
                Values("0b0e0a0c"), B | N | E | G | T | M | F | P | O | A | D | V),
            Device(RoseDeviceType.CeramicsMk28912F, "ceramics-mk2-8912f", "Ceramics MK2 8912F", RoseFamily.RsCommonV2,
                null, Values("0b0e208b"), B | N | E | G | T | M | F | P | O | A | D | V),
            Device(RoseDeviceType.CeramicsMk25736, "ceramics-mk2-5736", "Ceramics MK2 5736", RoseFamily.RsCommonV2,
                "25J5151GX255", Values("45d7b286"), B | N | E | G | T | M | F | P | O | V),
            Device(RoseDeviceType.CeramicsU, "CERAMICS-U", "Ceramics U", RoseFamily.RsCommonV2, "25J5151GE005",
                Values("0b0d0a0b"), B | N | L | E | G | T | M | F | P | O | A | D | V | X, gatt: CeramicsGatt),
            Device(RoseDeviceType.CeramicsUHsc2710, "CERAMICS-U-HSC-2710", "Ceramics U HSC 2710", RoseFamily.RsCommonV2,
                "26J51U5AH961", Values("0b137957"), B | N | L | E | G | T | M | F | P | O | A | D | V),
            Device(RoseDeviceType.AstroX, "astrox", "Astro X", RoseFamily.RsCommonV2, "25J51U5AX976",
                Values("0b0e0a0b"), B | L | E | G | W | T | S | M | F | P | O | A | H | V, Values("astro-x")),
            Device(RoseDeviceType.Auramic, "AURAMIC", "Auramic", RoseFamily.RsCommonV2, null, Values("0b0a0a10"),
                B | O | RoseDeviceFeatures.MicrophoneControl, commandMapper: new AuramicFeatureCommandMapper()),
            Device(RoseDeviceType.BudsfeelMk2, "budsfeel_mk2", "BudsFeel MK2", RoseFamily.RsCommonV2, "26J51U5AX128",
                Values("0b150a0b"), B | N | E | G | T | M | F | P | O | A | V, Values("budsfeel-mk2"),
                Guids(BudsfeelMk2Gatt.ServiceUuid), BudsfeelMk2Gatt),
            Device(RoseDeviceType.Coral, "coral", "Coral", RoseFamily.RsCommonV2, "26J51U5A4199", Values("0b0e0a0e"),
                B | N | E | G | T | M | F | P | O | A),
            Device(RoseDeviceType.EarfeelI7, "earfeel-i7", "EarFeel i7", RoseFamily.RsCommonV2, "26J51U5AF317",
                Values("b8e5290f"), B | N | L | E | G | W | T | M | F | P | O | A | H | V | X),
            Device(RoseDeviceType.FirstAngel, "first-angel", "First Angel", RoseFamily.RsCommonV2, "26J51U5AG904",
                Values("0b0e64be"), B | E | G | T | M | F | P | O),
            Device(RoseDeviceType.Furina, "furina", "Furina", RoseFamily.RsCommonV2, "25J5151G8377", Values("0b110a0c"),
                B | N | E | G | W | T | M | F | P | O | A | D),
            Device(RoseDeviceType.Hyacinth, "hyacinth", "Hyacinth", RoseFamily.RsCommonV2, "25J5151GM181",
                Values("0b120a0b"), B | E | G | T | M | F | P | A | D),
            Device(RoseDeviceType.Jasmine, "jasmine", "Jasmine", RoseFamily.RsCommonV2, "25J5151GM751",
                Values("0b0a0a11"), B | G | T | M | F | P | O | D),
            Device(RoseDeviceType.Ky01, "ky01", "KY01", RoseFamily.RsCommonV2, null, Values("0a0b0a0d"),
                B | E | G | T | M | F | P | O | D),
            Device(RoseDeviceType.LuminousAgumon, "luminous_agumon", "Luminous Agumon", RoseFamily.RsCommonV2,
                "26J51U5AL725", Values("0b0a34fd"), B | G | T | M | F | P, Values("luminous-agumon")),
            Device(RoseDeviceType.LuminousGabumon, "luminous_gabumon", "Luminous Gabumon", RoseFamily.RsCommonV2, null,
                Values("0b0a2aed"), B | G | T | M | F | P, Values("luminous-gabumon")),
            Device(RoseDeviceType.LuminousPatamon, "luminous_patamon", "Luminous Patamon", RoseFamily.RsCommonV2, null,
                Values("0b0ae916"), B | G | T | M | F | P, Values("luminous-patamon")),
            Device(RoseDeviceType.LuminousTailmon, "luminous_tailmon", "Luminous Tailmon", RoseFamily.RsCommonV2, null,
                Values("0b0a941f"), B | G | T | M | F | P, Values("luminous-tailmon")),
            Device(RoseDeviceType.Magician, "magician", "Magician", RoseFamily.RsCommonV2, null, Values("0b0e0a0d"),
                B | G | M | F | P | O | A | D),
            Device(RoseDeviceType.Mirror, "mirror", "Mirror", RoseFamily.RsCommonV2, "26J51U5AB492", Values("0b0a0a13"),
                B | E | G | T | M | F | P),
            Device(RoseDeviceType.Moka, "moka", "Moka", RoseFamily.RsCommonV2, "26J51U5AN659", Values("68f5b8eb"),
                B | E | G | M | P | O | A),
            Device(RoseDeviceType.Opal, "opal", "Opal", RoseFamily.RsCommonV2, "26J51U5A3126", Values("0b0a0a12"),
                B | N | E | G | T | M | F | P),
            Device(RoseDeviceType.OpenfeelL, "openfeel-l", "Openfeel L", RoseFamily.RsCommonV2, null,
                Values("45d7cbed"), B | L | E | G | T | M | F | P | O | A | H, Values("openfeel_l")),
            Device(RoseDeviceType.OpenfeelMk2, "OpenFeel_mk2", "Openfeel MK2", RoseFamily.RsCommonV2, "26J51U5A3186",
                Values("0b0eb657"), B | L | E | G | T | M | F | P | O | A | H | V, Values("openfeel-mk2")),
            Device(RoseDeviceType.Speakfeel2, "speakfeel2", "SpeakFeel 2", RoseFamily.RsCommonV2, null,
                Values("0b0e3ce6"), B | L | E | G | T | M | F | P | O | A | H | RoseDeviceFeatures.MicrophoneControl,
                Values("speakfeel-2")),
            Device(RoseDeviceType.Speakfree, "speakfree", "Speakfree", RoseFamily.RsCommonV2, "25J5151GJ111",
                Values("0b0f0a0b"), B | E | G | M | F | P | O | A | D, Values("speak-free")),
            Device(RoseDeviceType.Swift2, "swift2", "Swift 2", RoseFamily.RsCommonV2, "26J51U5A2752",
                Values("5999064c"), B | G | T | M | F | P | O, Values("swift-2")),
            Device(RoseDeviceType.Zircon, "zircon", "Zircon", RoseFamily.RsCommonV2, "25J5151GH810", Values("0b110a0b"),
                B | N | E | G | W | T | M | F | P | O | A | D | RoseDeviceFeatures.Recorder),

            Device(RoseDeviceType.EarfreeI3, "earfree-i3", "EarFree i3", RoseFamily.Zt, null,
                Values("f10203", "010203"), B | N | E | G | T | M | O | A,
                commandMapper: new ZtModelFeatureCommandMapper(RoseDeviceType.EarfreeI3,
                    B | N | E | G | T | M | O | A)),
            Device(RoseDeviceType.EarfreeI5, "EARFREE-i5", "EarFree i5", RoseFamily.Zt, "26J51U5A3756",
                Values("525300010001"), B | N | L | E | G | W | T | M | F | P | O | A | V,
                commandMapper: new Zt229FeatureCommandMapper()),
            Device(RoseDeviceType.Headfree, "headfree", "HeadFree", RoseFamily.Zt, "25J5151GT231",
                Values("525300020002"), B | N | L | E | G | T | M | F | P | O | A,
                commandMapper: new ZtModelFeatureCommandMapper(RoseDeviceType.Headfree,
                    B | N | L | E | G | T | M | F | P | O | A))
        };
    }

    private static RoseDeviceDefinition Device(
        RoseDeviceType type,
        string id,
        string displayName,
        RoseFamily family,
        string? hardwareModel,
        IReadOnlyList<string> signatures,
        RoseDeviceFeatures features,
        IReadOnlyList<string>? aliases = null,
        IReadOnlyList<Guid>? services = null,
        RoseGattConfiguration? gatt = null,
        IFeatureCommandMapper<RoseFrame>? commandMapper = null,
        bool commandsAvailable = true)
    {
        var effectiveGatt = gatt ?? GetDefaultGatt(family);
        var effectiveServices = services ?? (effectiveGatt == null
            ? Array.Empty<Guid>()
            : Guids(effectiveGatt.ServiceUuid));
        var effectiveMapper =
            commandsAvailable ? commandMapper ?? CreateFamilyCommandMapper(type, family, features) : null;
        var senderKind = GetSenderKind(type, family, commandsAvailable);
        return new RoseDeviceDefinition(type, id, displayName, family, hardwareModel, signatures, features,
            aliases ?? Array.Empty<string>(), effectiveServices, senderKind, effectiveGatt, effectiveMapper);
    }

    private static RoseSenderKind GetSenderKind(RoseDeviceType type, RoseFamily family, bool available)
    {
        if (!available) return RoseSenderKind.None;
        return type switch
        {
            RoseDeviceType.Explorer => RoseSenderKind.Explorer,
            RoseDeviceType.EarfreeI3 => RoseSenderKind.Zt219,
            RoseDeviceType.EarfreeI5 => RoseSenderKind.Zt229,
            RoseDeviceType.Headfree => RoseSenderKind.Zt231,
            RoseDeviceType.Auramic => RoseSenderKind.RsCommonV2Mic,
            _ => family switch
            {
                RoseFamily.F8 => RoseSenderKind.F8,
                RoseFamily.RsCommon => RoseSenderKind.RsCommon,
                RoseFamily.RsCommonV2 => RoseSenderKind.RsCommonV2,
                _ => RoseSenderKind.None
            }
        };
    }

    private static IFeatureCommandMapper<RoseFrame>? CreateFamilyCommandMapper(
        RoseDeviceType type,
        RoseFamily family,
        RoseDeviceFeatures features)
    {
        return family switch
        {
            RoseFamily.F8 => new RoseF8FeatureCommandMapper(type, features),
            RoseFamily.RsCommon or RoseFamily.RsCommonV2 => new RoseRsFeatureCommandMapper(family, features),
            _ => null
        };
    }

    private static RoseGattConfiguration? GetDefaultGatt(RoseFamily family)
    {
        return family switch
        {
            RoseFamily.F8 => F8Gatt,
            RoseFamily.RsCommon => RsCommonGatt,
            RoseFamily.RsCommonV2 => RsCommonGatt,
            RoseFamily.Zt => ZtGatt,
            _ => null
        };
    }

    private static string[] Values(params string[] values)
    {
        return values;
    }

    private static Guid[] Guids(params Guid[] values)
    {
        return values;
    }
}