using System.Globalization;

namespace SekaiLink.ConsoleApp;

internal sealed class CommandLine
{
    private readonly Dictionary<string, string?> _options = new(StringComparer.OrdinalIgnoreCase);

    private CommandLine(IReadOnlyList<string> positionals)
    {
        Positionals = positionals;
    }

    public IReadOnlyList<string> Positionals { get; }

    public bool HasOption(string name) => _options.ContainsKey(name);

    public string? GetOption(string name) => _options.TryGetValue(name, out var value) ? value : null;

    public int GetIntOption(string name, int fallback, int minimum, int maximum)
    {
        var value = GetOption(name);
        if (value is null) return fallback;
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) || parsed < minimum || parsed > maximum)
            throw new CommandLineException($"--{name} 必须是 {minimum} 到 {maximum} 之间的整数。");
        return parsed;
    }

    public static CommandLine Parse(string[] args)
    {
        var result = new CommandLine(new List<string>());
        var positionals = (List<string>)result.Positionals;
        for (var index = 0; index < args.Length; index++)
        {
            var current = args[index];
            if (!current.StartsWith("--", StringComparison.Ordinal) || current.Length == 2)
            {
                positionals.Add(current);
                continue;
            }

            var option = current[2..];
            var separator = option.IndexOf('=');
            if (separator >= 0)
            {
                result._options[option[..separator]] = option[(separator + 1)..];
                continue;
            }

            if (IsFlag(option))
                result._options[option] = null;
            else if (index + 1 < args.Length && !args[index + 1].StartsWith("--", StringComparison.Ordinal))
                result._options[option] = args[++index];
            else
                result._options[option] = null;
        }
        return result;
    }

    private static bool IsFlag(string option) => option.Equals("all", StringComparison.OrdinalIgnoreCase)
        || option.Equals("dry-run", StringComparison.OrdinalIgnoreCase)
        || option.Equals("help", StringComparison.OrdinalIgnoreCase);
}

internal sealed class CommandLineException : Exception
{
    public CommandLineException(string message) : base(message) { }
}
