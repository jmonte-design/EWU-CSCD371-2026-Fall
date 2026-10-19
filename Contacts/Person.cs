using System.Diagnostics.Contracts;

namespace Contacts;

// ---------------------------------------------------------------------------
// Person: everything Student and Employee share, plus the equality rules.
// ---------------------------------------------------------------------------

public abstract record Person(FullName Name, EmailAddress Email, PhoneNumber? Phone = null) : IContact
{
    // Unique per instance. Excluded from equality (see Equals below).
    public Guid Id { get; init; } = Guid.NewGuid();

    public abstract ContactKind Kind { get; }

    // Short description of what this person is, used in GetContactInfo.
    protected abstract string Role { get; }

    public string GetContactInfo() =>
        $"{Name} ({Role}) | {Email}" + (Phone is null ? "" : $" | {Phone}");

    // -----------------------------------------------------------------------
    // Equality.
    //
    // A record's synthesized Equals compares EVERY instance field, which would include
    // Id. When a record declares Equals(T) itself, the compiler does not synthesize it,
    // so we list exactly the shared fields that count (everything except Id).
    //
    // Derived records (Student, Employee) still get a synthesized Equals that calls this
    // method for the shared part and then compares their own fields.
    //
    // EqualityContract is the record's runtime type, so a Student and an Employee with
    // otherwise identical data are never equal.
    // -----------------------------------------------------------------------
    public virtual bool Equals(Person? other) =>
        other is not null
        && EqualityContract == other.EqualityContract
        && Name == other.Name
        && Email == other.Email
        && Phone == other.Phone;

    // Must agree with Equals: equal contacts => equal hash codes. Id is left out here too.
    public override int GetHashCode() =>
        HashCode.Combine(EqualityContract, Name, Email, Phone);
}

// ---------------------------------------------------------------------------
// Student: members unique to students.
// ---------------------------------------------------------------------------

public sealed record Student(
    FullName Name,
    EmailAddress Email,
    StudentId StudentId,
    Major Major,
    AcademicLevel Level,
    PhoneNumber? Phone = null) : Person(Name, Email, Phone)
{
    public override ContactKind Kind => ContactKind.Student;

    protected override string Role => $"Student, {Level} {Major}";

    public bool IsUndergraduate => Level != AcademicLevel.Grad;
}

// ---------------------------------------------------------------------------
// Employee: members unique to employees.
// ---------------------------------------------------------------------------

public sealed record Employee(
    FullName Name,
    EmailAddress Email,
    EmployeeId EmployeeId,
    Department Department,
    EmploymentType Employment,
    DateOnly HireDate,
    PhoneNumber? Phone = null) : Person(Name, Email, Phone)
{
    public override ContactKind Kind => ContactKind.Employee;

    protected override string Role => $"{Department}, {Employment}";

    // `today` is a parameter (not DateTime.Now) so this is deterministic in tests.
    public int YearsOfService(DateOnly today)
    {
        int years = today.Year - HireDate.Year;
        if (today < HireDate.AddYears(years)) years--;
        return Math.Max(0, years);
    }
}


