using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using Launcher.LoaderAPI;

var bootHttpManager = new HttpManager();

var launcherVersion = Environment.GetEnvironmentVariable("LONA_LAUNCHER_VERSION", EnvironmentVariableTarget.User) ?? "0";
var launcherVersionUrl = "https://raw.githubusercontent.com/DocNITE/LonaLauncher/main/Launcher.Version.txt";
var launcherContentUrl = "https://raw.githubusercontent.com/DocNITE/LonaLauncher/main/Launcher.Content.zip";
await bootHttpManager.DownloadCoreFile("launcher", launcherContentUrl, "./", launcherVersionUrl, launcherVersion);

try
{
    // Check main app arguments. That's need, if we wanna load custom DLL
    var dllName = "Launcher.Avalonia";
    if (args.Length >= 1)
        dllName = args[0];

    var assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + $"{dllName}.dll");
    var myType = assembly.GetType($"{dllName}.MainProgram");

    // Check if the type was found
    if (myType == null)
        return;

    // Create an instance of the type using Activator.CreateInstance
    var instance = Activator.CreateInstance(myType);

    // Run application
    var method = myType.GetMethod("Main");
    method?.Invoke(instance, new object[] { args });
}
catch
{
    // ignored
}