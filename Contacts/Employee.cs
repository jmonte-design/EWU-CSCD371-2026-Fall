namespace Contacts;

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


