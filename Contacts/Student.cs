namespace Contacts;

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


