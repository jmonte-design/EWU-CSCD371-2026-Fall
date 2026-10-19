# Assignment 4 - Contacts CLI

## Overview

Build a simple command-line contacts list that stores both **student** and **employee** records. This assignment is about **records and equality**: two contacts with the same data must be treated as the same contact.

How you design the types, the menu, and the user experience is up to you - choose simplicity over complexity.

## Reading

Reference **Chapters 9 - Introducing Structs and Records & Chapter 10 - Well Formed Types**

## Tasks

### Types

- Define a full name record (first, last, optional middle), handling `null` and optional values appropriately. ❌✔
- Define student and employee records, both with contact capabilities. ❌✔
  - Consider what Student and Employee share, and refactor the common code (an abstract base is one option). ❌✔
  - Consider what members Student and Employee should be unique to each type. ❌✔
- Use the provided `ContactBook` class to store the contacts. ❌✔

### Equality

- Two contacts with the same data are equal even though they have different `Id`s. ❌✔
- Records synthesize equality over every property, so decide how to keep `Id` out of it. ❌✔
- A student and an employee with otherwise identical data are **not** equal. ❌✔

### ContactBook (must use `HashSet<IContact>`)

- `ContactBook` must keep storing contacts in a `HashSet<IContact>` (a provided test checks this). ❌✔
- Adding a contact equal to a stored one must not add a duplicate, and the caller must be able to tell. ❌✔
- `Contains` and `Remove` must work with an equal but different instance. ❌✔
- Add whatever else your CLI needs (for example, search or list). ❌✔

The `HashSet` is what makes equality matter: it calls `Equals` and `GetHashCode` through `IContact`, so a wrong implementation shows up as duplicate or "missing" contacts.

### CLI

- Let the user add students and employees, list contacts, and remove a contact. ❌✔
- Adding a duplicate contact is reported to the user, not silently ignored. ❌✔
- Bad input must not crash the program. ❌✔
- Keep your code testable. ❌✔

### Tests

- Unit test all appropriate code including `ContactBook`. ❌✔
- Include tests showing a duplicate is rejected, a student and employee with the same data are both stored, and an equal-but-different instance is found and removed. ❌✔

## Fundamentals

- Target all projects at .NET 10 (`net10.0`) with the latest `LangVersion`.
- Be sure you enable:
  - `Nullable` for all projects ❌✔
  - Code analysis for all projects (`EnableNETAnalyzers`) ❌✔
  - `EnforceCodeStyleInBuild` ❌✔
- **Ensure there are no errors or warnings (including code analysis warnings)** ❌✔
- **All of the above should be unit tested** ❌✔
- **Choose simplicity over complexity** ❌✔

## Extra Credit

- Persist the contact list to a JSON file with `System.Text.Json`, loading it at startup. ❌✔
- Implement editing an existing user. ❌✔
- Allow contacts to store an arbitrary number of email addresses. Ensure all equality rules above remain in place. ❌✔

## Relevant APIs and Docs to know about

[Equality](https://learn.microsoft.com/dotnet/csharp/fundamentals/expressions/equality#use-records-for-value-equality)

[HashSet&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.hashset-1) Uses `Equals` and `GetHashCode` to decide membership.

[System.HashCode](https://learn.microsoft.com/dotnet/api/system.hashcode) `HashCode.Combine` builds a good hash code from several values.

[Records](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/record) Synthesized equality, `with` expressions, and `EqualityContract`.

## See [Docs](https://github.com/IntelliTect-Samples/EWU-CSCD371-2026-Fall-Instructors/blob/main/Docs/README.md)
