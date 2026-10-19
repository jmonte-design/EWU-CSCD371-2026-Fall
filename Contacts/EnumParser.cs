namespace Contacts;

public static class EnumParser
{
    /// <summary>
    /// Matches by name only, ignoring case, spaces, hyphens and underscores
    /// ("full time" -> FullTime). Unlike Enum.TryParse it rejects numbers ("99") and
    /// comma lists ("Junior,Senior"), so only declared values are ever returned.
    /// </summary>
    public static bool TryParse<TEnum>(string? text, out TEnum value) where TEnum : struct, Enum
    {
        value = default;
        if (string.IsNullOrWhiteSpace(text)) return false;

        string wanted = Normalize(text);
        foreach (TEnum candidate in Enum.GetValues<TEnum>())
        {
            if (Normalize(candidate.ToString()).Equals(wanted, StringComparison.OrdinalIgnoreCase))
            {
                value = candidate;
                return true;
            }
        }
        return false;
    }

    /// <exception cref="ArgumentException">The text is not one of the enum's names.</exception>
    public static TEnum Parse<TEnum>(string? text) where TEnum : struct, Enum =>
        TryParse(text, out TEnum value)
            ? value
            : throw new ArgumentException($"Choose one of: {Options<TEnum>()}.");

    public static string Options<TEnum>() where TEnum : struct, Enum =>
        string.Join(", ", Enum.GetNames<TEnum>());

    private static string Normalize(string text) =>
        new(text.Where(char.IsLetterOrDigit).ToArray());
}