using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Contacts;

public interface IContact
{
    Guid Id { get; }
    ContactKind Kind { get; }
    FullName Name { get; }
    EmailAddress Email { get; }
    PhoneNumber? Phone { get; }
    // Place members here.
    string GetContactInfo();
}
