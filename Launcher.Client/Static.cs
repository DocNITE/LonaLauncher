using Launcher.LoaderAPI;

namespace Launcher.Client;

public class Static 
{
    // Game folder name
    public const string GameFolderName = "LonaRPG";
    
    // Main http manager for core files
    public static readonly HttpManager HttpManager = new ();
}