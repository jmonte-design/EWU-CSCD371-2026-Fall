namespace Contacts;

// Each value record validates in its constructor and exposes get-only properties.
// There is no `init`, so `with` cannot be used to sneak in an invalid value, and the
// compiler-synthesized equality compares exactly the (normalized) values.

public sealed record FullName
{
    public FullName(string first, string last, string? middle = null)
    {
        First = Validate.PersonName(first, "First name");
        Last = Validate.PersonName(last, "Last name");
        Middle = Validate.OptionalPersonName(middle, "Middle name"); // blank -> null
    }

    public string First { get; }
    public string Last { get; }
    public string? Middle { get; }

    public bool HasMiddle => Middle is not null;

    public string Initials =>
        $"{First[0]}{(HasMiddle ? Middle![0].ToString() : "")}{Last[0]}".ToUpperInvariant();

    // "Ada Lovelace" or "Ada Augusta Lovelace"
    public override string ToString() =>
        HasMiddle ? $"{First} {Middle} {Last}" : $"{First} {Last}";
}

public sealed record EmailAddress
{
    public EmailAddress(string value) => Value = Validate.Email(value);

    public string Value { get; }

    public override string ToString() => Value;
}

public sealed record PhoneNumber
{
    public PhoneNumber(string value) => Value = Validate.Phone(value);

    public string Value { get; }

    public override string ToString() => Value;
}

public sealed record StudentId
{
    public StudentId(string value) => Value = Validate.Identifier(value, "Student ID");

    public string Value { get; }

    public override string ToString() => Value;
}

public sealed record EmployeeId
{
    public EmployeeId(string value) => Value = Validate.Identifier(value, "Employee ID");

    public string Value { get; }

    public override string ToString() => Value;
}
