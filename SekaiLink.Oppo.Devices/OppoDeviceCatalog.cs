using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SekaiLink.Oppo.Protocol.Protocols;
using SekaiLink.Protocols.Models;

namespace SekaiLink.Oppo.Devices;

public static partial class OppoDeviceCatalog
{
    private static readonly IReadOnlyDictionary<string, OppoDeviceDefinition> ById =
        All.ToDictionary(device => device.Id, StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyList<OppoDeviceDefinition> All { get; } = Array.AsReadOnly(CreateDefinitions());

    public static OppoDeviceDefinition? Find(string? productId)
    {
        if (string.IsNullOrWhiteSpace(productId)) return null;
        return ById.TryGetValue(productId.Trim(), out var definition) ? definition : null;
    }

    public static OppoDeviceDefinition? FindFromProductIdResponse(OppoFrame frame)
    {
        return OppoCommands.TryParseProductId(frame, out var productId) ? Find(productId) : null;
    }

    /// <summary>Resolves only an unambiguous model. Product ID takes precedence over display name.</summary>
    public static bool TryResolve(DeviceIdentity identity, out OppoDeviceDefinition? definition)
    {
        if (identity == null) throw new ArgumentNullException(nameof(identity));
        definition = Find(identity.ProductId);
        if (definition != null) return true;
        if (!string.IsNullOrWhiteSpace(identity.ProductId)) return false;

        var model = Normalize(identity.Model);
        var name = Normalize(identity.Name);
        var bestScore = 0;
        var tied = false;
        foreach (var candidate in All)
        {
            var known = Normalize(candidate.DisplayName);
            if (known.Length < 5) continue;
            var score = model == known || name == known
                ? 100
                :
                model.StartsWith(known, StringComparison.Ordinal) || name.StartsWith(known, StringComparison.Ordinal)
                    ?
                    80
                    : 0;
            // A UUID shared by many products is insufficient to identify a model.
            if (score > bestScore)
            {
                definition = candidate;
                bestScore = score;
                tied = false;
            }
            else if (score > 0 && score == bestScore)
            {
                tied = true;
            }
        }

        if (bestScore == 0 || tied) definition = null;
        return definition != null;
    }

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var normalized = new StringBuilder(value.Length);
        foreach (var character in value)
            if (char.IsLetterOrDigit(character))
                normalized.Append(char.ToLowerInvariant(character));
        return normalized.ToString();
    }

    private static OppoDeviceDefinition[] CreateDefinitions()
    {
        return CreateGeneratedDefinitions();
    }

    private static partial OppoDeviceDefinition[] CreateGeneratedDefinitions();
}