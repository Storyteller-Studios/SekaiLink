using System;
using System.Globalization;

namespace SekaiLink.Native.Windows.Internal;

internal static class BluetoothEndpoint
{
    public static bool TryParseAddress(string value, out ulong address)
    {
        address = 0;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var normalized = value.Trim()
            .Replace(":", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal);
        if (normalized.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) normalized = normalized[2..];

        return normalized.Length is > 0 and <= 12
               && ulong.TryParse(normalized, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out address);
    }

    public static string FormatAddress(ulong address)
    {
        var hex = (address & 0x0000FFFFFFFFFFFFUL).ToString("X12", CultureInfo.InvariantCulture);
        return string.Create(17, hex, static (result, value) =>
        {
            var source = 0;
            var target = 0;
            while (source < value.Length)
            {
                if (target > 0) result[target++] = ':';
                result[target++] = value[source++];
                result[target++] = value[source++];
            }
        });
    }

    public static Guid RequireGuid(string? value, string propertyName)
    {
        if (!Guid.TryParse(value, out var result))
            throw new ArgumentException($"Endpoint {propertyName} must be a UUID.", propertyName);
        return result;
    }
}