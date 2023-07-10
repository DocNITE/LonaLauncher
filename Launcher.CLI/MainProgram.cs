using System.Net.Mime;
using System.Reflection;
using Launcher.Client;

namespace Launcher.CLI;

public class MainProgram
{
    // Entry point
    [STAThread]
    public static void Main(string[] args)
    {
        Logger.Info("Launcher.CLI (ver 0.1) by DocNight");
        Logger.Info("List commands - 'help'");
        
        while (true)
        {
            var commandline = Console.ReadLine();
            
            if (commandline == "exit")
                break;
        }
    }
}