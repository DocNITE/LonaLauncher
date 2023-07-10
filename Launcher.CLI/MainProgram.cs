using System.Net.Mime;
using System.Reflection;
using Launcher.Client;
using Launcher.Client.Settings;
using Launcher.LoaderAPI;

namespace Launcher.CLI;

public class MainProgram
{
    private static ProgressBar? _progress;
    
    // Entry point
    [STAThread]
    public static void Main(string[] args)
    {
        Logger.Info("Launcher.CLI (ver 0.1) by DocNight");
        Logger.Info("List commands - 'help'");

        // init settings
        Config.Load();
        
        while (true)
        {
            var commandline = Console.ReadLine();

            CommandHandler(commandline ?? string.Empty);
            
            if (commandline == "exit")
                break;
        }
    }

    private static async void CommandHandler(string param)
    {
        var cmd = param.Split(' ');

        switch (cmd[0])
        {
            case "update":
                _progress = new ProgressBar();
                
                HttpManager.OnDownloadProgress += OnDownloadProgress;
                HttpManager.OnDownloadComplete += OnDownloadComplete;
                HttpManager.OnExistingFile += OnExistingFile;
                HttpManager.OnChangeFileVersion += OnChangeFileVersion;
                HttpManager.OnZipExtractProgress += OnZipExtractProgress;

                Logger.Info("Start downloading game files...");
                
                await HttpManager.DownloadCoreFile("game",
                Config.GetField("game_url"), 
                    Config.GetField("game_path"),
                    Config.GetField("game_version_url"),
                    Config.GetField("game_version"));
                
                Logger.Info("Start downloading patch archive...");
                
                await HttpManager.DownloadCoreFile("patch",
                    Config.GetField("patch_url"), 
                    Config.GetField("game_path"),
                    Config.GetField("patch_version_url"),
                    Config.GetField("patch_version"));
                
                HttpManager.OnDownloadProgress -= OnDownloadProgress;
                HttpManager.OnDownloadComplete -= OnDownloadComplete;
                HttpManager.OnExistingFile -= OnExistingFile;
                HttpManager.OnChangeFileVersion -= OnChangeFileVersion;
                HttpManager.OnZipExtractProgress -= OnZipExtractProgress;
                
                _progress.Dispose();
                _progress = null;
                
                break;
            case "help":
                var commandsText =
                    "Command list:\n" +
                    " update - Update all game files\n" +
                    " help - Get all commands list";
                Logger.Info(commandsText);
                break;
            default:
                Logger.Error("Not found command!");
                break;
        }
    }

    private static void OnDownloadProgress(DownloadProgress downloadProgress)
    {
        _progress?.Report(downloadProgress.Progress / 100);
    }
    private static void OnExistingFile()
    {
        Logger.Warn("No need to update. You have actual version!");
    }
    private static void OnChangeFileVersion(string groupFiles, string version)
    {
        var field = Config.GetFieldObjectWithVersion(groupFiles);
        
        if (field == null) 
            return;
        
        field.Data = version;
        Config.Save();
        
        Logger.Info("Changed version game...");
    }
    private static void OnZipExtractProgress(ZipProgress zipProgress)
    {
        _progress?.Report(zipProgress.Total / zipProgress.Processed);
    }
    private static void OnDownloadComplete()
    {   
        Logger.Info("Files has been downloaded!");
    }
}