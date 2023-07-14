namespace Launcher.Client;

public class Logger
{
    private const string DebugIdx = "[DEBUG]: ";
    private const string InfoIdx = "[INFO]: ";
    private const string WarnIdx = "[WARN]: ";
    private const string ErrorIdx = "[ERROR]: ";
    
    public static void Debug(string? message) => Print(message, DebugIdx, ConsoleColor.Blue);
    public static void Info(string? message) => Print(message, InfoIdx, ConsoleColor.Cyan);
    public static void Warn(string? message) => Print(message, WarnIdx, ConsoleColor.Yellow);
    public static void Error(string? message) => Print(message, ErrorIdx, ConsoleColor.Red);

    private static void Print(string? message, string idx, ConsoleColor color, bool nextLine = true)
    {
        if (message == null)
            return;
        
        Console.ForegroundColor = color;
        Console.Write(idx);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(message);
        
        if (nextLine)
            Console.WriteLine();
    }
}