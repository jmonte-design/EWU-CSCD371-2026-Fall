using System.Globalization;


namespace Contacts;

/// <summary>
/// Command-line interface. Processes user input for Contacts project.
/// StringReader/StringWriter. Bad input never throws out of Run(): invalid values are reported to user and discarded, and any unexpected error is reported to user and program continues.
/// </summary>
public class ProgramCli(ContactBook book, TextReader input, TextWriter output)
{
    private readonly ContactBook _book = book ?? throw new ArgumentNullException(nameof(book));
    private readonly TextReader _input = input ?? throw new ArgumentNullException(nameof(input));
    private readonly TextWriter _output = output ?? throw new ArgumentNullException(nameof(output));

    public void Run()
    
    {   
        _output.WriteLine("Starting Contact book. Type 'help' for commands.");
        WriteHelp();

        while (true)
        {
            _output.Write("> ");
            string? line = _input.ReadLine();
            if (line is null) break; // input closed (Ctrl+D / end of piped input)

            ParsedCommand parsed = CommandParser.Parse(line);
            if (parsed.Command == CliCommand.Quit) break;

            try
            {
                Execute(parsed);
            }
            catch (InputCancelledException)
            {
                _output.WriteLine("Cancelled. Nothing was changed.");
            }
            catch (InputClosedException)
            {
                break;
            }
            catch (Exception ex)
            {
                // Last-resort boundary so one failure cannot crash the session.
                _output.WriteLine($"Something went wrong: {ex.Message}");
            }
        }

        _output.WriteLine("Goodbye.");
    }

    private void Execute(ParsedCommand parsed)
    {
        switch (parsed.Command)
        {
            case CliCommand.None: break;
            case CliCommand.Add: HandleAdd(parsed.Argument); break;
            case CliCommand.List: HandleList(parsed.Argument); break;
            case CliCommand.Search: HandleSearch(parsed.Argument); break;
            case CliCommand.Remove: HandleRemove(parsed.Argument); break;
            case CliCommand.Help: WriteHelp(); break;
            default:
                _output.WriteLine($"Unknown command '{parsed.Argument}'. Type 'help' for the list of commands.");
                break;
        }
    }

    // ----- add ---------------------------------------------------------------

    private void HandleAdd(string argument)
    {
        ContactKind kind;
        if (string.IsNullOrWhiteSpace(argument))
        {
            kind = PromptEnum<ContactKind>("Kind");
        }
        else if (!EnumParser.TryParse(argument, out kind))
        {
            _output.WriteLine($"Unknown contact kind '{argument}'. Choose one of: {EnumParser.Options<ContactKind>()}.");
            return;
        }

        _output.WriteLine($"Adding a {kind.ToString().ToLowerInvariant()}. Type 'cancel' at any prompt to stop.");

        string first = Prompt("First name", s => Validate.PersonName(s, "First name"));
        string last = Prompt("Last name", s => Validate.PersonName(s, "Last name"));
        string? middle = Prompt("Middle name (optional)", s => Validate.OptionalPersonName(s, "Middle name"));
        EmailAddress email = Prompt("Email", s => new EmailAddress(s));
        PhoneNumber? phone = Prompt("Phone (optional)", s => string.IsNullOrWhiteSpace(s) ? null : new PhoneNumber(s));

        var name = new FullName(first, last, middle);

        IContact contact = kind switch
        {
            ContactKind.Student => ReadStudent(name, email, phone),
            ContactKind.Employee => ReadEmployee(name, email, phone),
            _ => throw new InvalidOperationException($"Unsupported contact kind: {kind}")
        };

        if (_book.Add(contact))
        {
            _output.WriteLine($"Added: {Describe(contact)}");
        }
        else
        {
            IContact? existing = _book.GetAll().FirstOrDefault(c => c.Equals(contact));
            _output.WriteLine("Duplicate: an identical contact already exists, so nothing was added.");
            if (existing is not null) _output.WriteLine($"Existing: {Describe(existing)}");
        }
    }

    private Student ReadStudent(FullName name, EmailAddress email, PhoneNumber? phone) =>
        new(name,
            email,
            Prompt("Student ID", s => new StudentId(s)),
            PromptEnum<Major>("Major"),
            PromptEnum<AcademicLevel>("Level"),
            phone);

    private Employee ReadEmployee(FullName name, EmailAddress email, PhoneNumber? phone) =>
        new(name,
            email,
            Prompt("Employee ID", s => new EmployeeId(s)),
            PromptEnum<Department>("Department"),
            PromptEnum<EmploymentType>("Employment type"),
            Prompt("Hire date (yyyy-MM-dd)", ParseDate),
            phone);

    // ----- list / search -----------------------------------------------------

    private void HandleList(string argument)
    {
        ContactKind? kind = null;
        if (!string.IsNullOrWhiteSpace(argument))
        {
            if (!EnumParser.TryParse(argument, out ContactKind parsed))
            {
                _output.WriteLine($"Unknown contact kind '{argument}'. Choose one of: {EnumParser.Options<ContactKind>()}.");
                return;
            }
            kind = parsed;
        }

        PrintContacts(_book.List(kind), "No contacts.");
    }

    private void HandleSearch(string argument)
    {
        string query = string.IsNullOrWhiteSpace(argument)
            ? Prompt("Search text", s => Validate.RequiredText(s, "Search text"))
            : argument;

        PrintContacts(_book.Search(query), $"No contacts match '{query}'.");
    }

    // ----- remove ------------------------------------------------------------

    private void HandleRemove(string argument)
    {
        string query = string.IsNullOrWhiteSpace(argument)
            ? Prompt("Name, email, phone or ID of the contact to remove",
                     s => Validate.RequiredText(s, "Search text"))
            : argument;

        IReadOnlyList<IContact> matches = _book.Search(query);

        switch (matches.Count)
        {
            case 0:
                _output.WriteLine($"No contact matches '{query}'. Nothing was removed.");
                break;

            case 1:
                IContact target = matches[0];
                _output.WriteLine(_book.Remove(target)
                    ? $"Removed: {Describe(target)}"
                    : $"Could not remove: {Describe(target)}");
                break;

            default:
                _output.WriteLine($"{matches.Count} contacts match '{query}'. Nothing was removed. Repeat with an ID:");
                PrintContacts(matches, string.Empty);
                break;
        }
    }

    // output helper methods

    private void PrintContacts(IReadOnlyList<IContact> contacts, string emptyMessage)
    {
        if (contacts.Count == 0)
        {
            if (emptyMessage.Length > 0) _output.WriteLine(emptyMessage);
            return;
        }

        foreach (IContact contact in contacts)
            _output.WriteLine($"  {Describe(contact)}");

        _output.WriteLine($"{contacts.Count} contact(s).");
    }

    private static string Describe(IContact contact) =>
        $"[{contact.Id.ToString("N")[..8]}] {contact.GetContactInfo()}";

    private void WriteHelp()
    {
        _output.WriteLine("---Command List---");
        _output.WriteLine("  add [student|employee]   add a contact (prompts for each field)");
        _output.WriteLine("  list [student|employee]  list contacts, optionally by kind");
        _output.WriteLine("  search <text>            find by name, email, phone, ID or kind");
        _output.WriteLine("  remove <text|id>         remove the one contact matching the text");
        _output.WriteLine("  help                     show this help");
        _output.WriteLine("  exit                    exit");
    }

    // ----- prompting ---------------------------------------------------------

    /// <summary>
    /// Prompt user until parse succeeds. bad input throws ArgumentException.
    /// </summary>
    private T Prompt<T>(string label, Func<string, T> parse)
    {
        while (true)
        {
            _output.Write($"{label}: ");
            string? raw = _input.ReadLine();
            if (raw is null) throw new InputClosedException();

            string text = raw.Trim();
            if (text.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                throw new InputCancelledException();

            try
            {
                return parse(text);
            }
            catch (ArgumentException ex)
            {
                _output.WriteLine($"  Invalid input: {ex.Message}");
            }
        }
    }

    private TEnum PromptEnum<TEnum>(string label) where TEnum : struct, Enum =>
        Prompt($"{label} ({EnumParser.Options<TEnum>()})", EnumParser.Parse<TEnum>);

    private static DateOnly ParseDate(string text) =>
        DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date)
            ? date
            : throw new ArgumentException("Use the format yyyy-MM-dd, for example 2024-09-01.");

    private sealed class InputCancelledException : Exception { }

    private sealed class InputClosedException : Exception { }
}
