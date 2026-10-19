namespace Contacts;

public class ContactBook
{
    // Must stay a HashSet<IContact>. It uses Equals/GetHashCode through IContact, which is
    // why the equality rules on Person matter.
    private HashSet<IContact> Entities { get; } = [];

    public int Count => Entities.Count;

    /// <returns>
    /// true if the contact was added; false if an equal contact was already stored
    /// (nothing changes in that case).
    /// </returns>
    public bool Add(IContact item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return Entities.Add(item);
    }

    /// <returns>true if an equal contact was found and removed. Works with an equal but different instance.</returns>
    public bool Remove(IContact item) => item is not null && Entities.Remove(item);

    /// <returns>true if an equal contact is stored. Works with an equal but different instance.</returns>
    public bool Contains(IContact item) => item is not null && Entities.Contains(item);

    /// <summary>Unordered snapshot of every contact.</summary>
    public IContact[] GetAll() => Entities.ToArray();

    /// <summary>Snapshot sorted by name (then kind, then Id), optionally filtered by kind.</summary>
    public IReadOnlyList<IContact> List(ContactKind? kind = null) =>
        Entities
            .Where(c => kind is null || c.Kind == kind)
            .OrderBy(c => c.Name.Last, StringComparer.OrdinalIgnoreCase)
            .ThenBy(c => c.Name.First, StringComparer.OrdinalIgnoreCase)
            .ThenBy(c => c.Name.Middle ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ThenBy(c => c.Kind)
            .ThenBy(c => c.Id)
            .ToList();

    /// <summary>
    /// Case-insensitive search, sorted like <see cref="List"/>. A contact matches when the text
    /// is contained in its name, email or phone, starts its Id (with or without hyphens), or
    /// equals its kind ("student" / "employee"). Blank text matches nothing.
    /// </summary>
    public IReadOnlyList<IContact> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return [];
        string text = query.Trim();
        return List().Where(c => Matches(c, text)).ToList();
    }

    private static bool Matches(IContact c, string text) =>
        c.Name.ToString().Contains(text, StringComparison.OrdinalIgnoreCase)
        || c.Email.Value.Contains(text, StringComparison.OrdinalIgnoreCase)
        || (c.Phone?.Value.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false)
        || c.Id.ToString("N").StartsWith(text, StringComparison.OrdinalIgnoreCase)
        || c.Id.ToString("D").StartsWith(text, StringComparison.OrdinalIgnoreCase)
        || c.Kind.ToString().Equals(text, StringComparison.OrdinalIgnoreCase);
}