using System.Text.RegularExpressions;

namespace Contacts;

/// <summary>
/// Shared validation/normalization used by the value records. Every failure is an
/// ArgumentException with a user-readable message (no parameter-name suffix), so the
/// CLI can show ex.Message directly.
/// </summary>
internal static class Validate
{
    private static readonly Regex NamePattern =
        new(@"^\p{L}[\p{L}\p{M}'’. -]*$", RegexOptions.CultureInvariant);

    private static readonly Regex EmailPattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.CultureInvariant);

    private static readonly Regex PhoneSeparators =
        new(@"[\s().\-]", RegexOptions.CultureInvariant);

    private static readonly Regex PhonePattern =
        new(@"^\+?[0-9]{7,15}$", RegexOptions.CultureInvariant);

    private static readonly Regex IdPattern =
        new(@"^[A-Z0-9-]+$", RegexOptions.CultureInvariant);

    public static string RequiredText(string? value, string label)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{label} is required.");
        return value.Trim();
    }

    public static string PersonName(string? value, string label)
    {
        string text = RequiredText(value, label);
        if (text.Length > 50)
            throw new ArgumentException($"{label} must be 50 characters or fewer.");
        if (!NamePattern.IsMatch(text))
            throw new ArgumentException(
                $"{label} may contain only letters, spaces, hyphens, apostrophes and periods.");
        return text;
    }

    /// <summary>Blank or whitespace means "no value" (null).</summary>
    public static string? OptionalPersonName(string? value, string label) =>
        string.IsNullOrWhiteSpace(value) ? null : PersonName(value, label);

    /// <summary>Trimmed and lower-cased, so equality ignores case.</summary>
    public static string Email(string? value)
    {
        string text = RequiredText(value, "Email").ToLowerInvariant();
        if (text.Length > 254 || !EmailPattern.IsMatch(text))
            throw new ArgumentException("Email must look like name@example.com.");
        return text;
    }

    /// <summary>Separators removed, so "(509) 555-0100" equals "5095550100".</summary>
    public static string Phone(string? value)
    {
        string stripped = PhoneSeparators.Replace(RequiredText(value, "Phone"), "");
        if (!PhonePattern.IsMatch(stripped))
            throw new ArgumentException("Phone must have 7-15 digits, with an optional leading +.");
        return stripped;
    }

    /// <summary>Trimmed and upper-cased, so "s1001" equals "S1001".</summary>
    public static string Identifier(string? value, string label)
    {
        string text = RequiredText(value, label).ToUpperInvariant();
        if (text.Length > 20 || !IdPattern.IsMatch(text))
            throw new ArgumentException(
                $"{label} may contain only letters, digits and hyphens (max 20 characters).");
        return text;
    }
}