namespace Calculator.App;

/// <summary>
/// Methods to cleanly write to the console in a specific colour without clogging up the rest of the code with colour resets
/// </summary>
public static class ConsoleWriter
{
    public static void WriteColoured(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.Write(text);
        Console.ResetColor();
    }

    public static void WriteColouredLine(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
