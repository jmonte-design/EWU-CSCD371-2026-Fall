namespace Contacts;

public readonly record struct ParsedCommand(CliCommand Command, string Argument);

public static class CommandParser
{
    private static readonly Dictionary<string, CliCommand> Words =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["add"] = CliCommand.Add,
            ["new"] = CliCommand.Add,
            ["list"] = CliCommand.List,
            ["ls"] = CliCommand.List,
            ["search"] = CliCommand.Search,
            ["find"] = CliCommand.Search,
            ["remove"] = CliCommand.Remove,
            ["rm"] = CliCommand.Remove,
            ["delete"] = CliCommand.Remove,
            ["help"] = CliCommand.Help,
            ["?"] = CliCommand.Help,
            ["quit"] = CliCommand.Quit,
            ["exit"] = CliCommand.Quit,
            ["q"] = CliCommand.Quit
        };

    /// <summary>Never throws. Unknown words come back as CliCommand.Unknown with the whole line as the argument.</summary>
    public static ParsedCommand Parse(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new ParsedCommand(CliCommand.None, string.Empty);

        string trimmed = line.Trim();
        int split = trimmed.IndexOfAny(new[] { ' ', '\t' });
        string word = split < 0 ? trimmed : trimmed[..split];
        string argument = split < 0 ? string.Empty : trimmed[(split + 1)..].Trim();

        return Words.TryGetValue(word, out CliCommand command)
            ? new ParsedCommand(command, argument)
            : new ParsedCommand(CliCommand.Unknown, trimmed);
    }
}