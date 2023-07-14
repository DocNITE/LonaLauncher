using System.Diagnostics;
using Launcher.Client.Settings;

namespace Launcher.Client.Game;

public partial class GameProcess 
{
    private Process? _process;
    public GameProcess() {
        Run(Config.GetField("game_path") + "/Game.exe");
    }

    // That uses for running applications
    public Process? Run(string fileName) 
    {
        if (_process != null)
            return _process;

        var programPath = fileName;

        Logger.Info($"Start the game...");
        
        if (!File.Exists(programPath))
        {
            Logger.Warn($"Cannot find '{programPath}'.");
            // TODO: So, if we cannot find the game - we can trye
            // to run auto-update or something... Idk...
            return null;
        }
        
        var startInfo = new ProcessStartInfo
        {
            FileName = programPath,
            UseShellExecute = false
        };
    
        var process = new Process { StartInfo = startInfo };
        process.Start();

        _process = process;

        Logger.Info($"File has been running!");
        return process;
    }
}