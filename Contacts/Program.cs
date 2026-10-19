namespace Contacts;

public static class Program
{
    public static void Main()
    {
        new ProgramCli(new ContactBook(), Console.In, Console.Out).Run();
    }
}
