using System.IO.Compression;
using Launcher.Client.Settings;
using Launcher.LoaderAPI;

namespace Launcher.Client.Net;

public static class DownloadManager
{
    private static bool _isDownload = false;
    
    public static async Task DownloadGameData(string url, string path, string versionUrl, string? currentVersion = "-1")
    {
        _isDownload = true;
        
        var param1 = Config.GetField(url);
        var param2 = Config.GetField(path);
        var param3 = "Cache";
        
        var dirCache = new DirectoryInfo("./.cache");
        if (dirCache.Exists)
            dirCache.Delete(true);
        dirCache.Create();
        var http = new HttpRequest(param1, "./.cache/" + param3);
        // get version game
        var response = await http.NetClient.GetAsync(Config.GetField(versionUrl));

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to retrieve file, status code: {response.StatusCode}");
            _isDownload = false;
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        content = content.Replace("\n", "");
        // return if versions was equal
        if (content == Config.GetField(currentVersion) || content == "-1")
        {
            // todo: existing
            dirCache.Delete(true);
            _isDownload = false;
            return;
        }
        Console.WriteLine("Update version: " + content);
        Config.GetFieldObject(currentVersion).Data = content;
        Config.Save();
        // process
        http.ProgressChanged += (long? totalFileSize, long totalBytesDownloaded, double? progressPercentage) =>
        {
            //this.FindControl<ProgressBar>("ProcessBar").Maximum = (double)totalFileSize;
            //this.FindControl<ProgressBar>("ProcessBar").Value = (double)totalBytesDownloaded;
            //this.FindControl<TextBlock>("InfoLabel").Text = $"Download: {totalFileSize} / {totalBytesDownloaded}";
            // todo: progress

        };
        await http.StartDownload();
        Console.WriteLine("Finished!");
        Console.WriteLine("Start process...");
        Progress<ZipProgress> progress = new();
        progress.ProgressChanged += (object sender, ZipProgress zipProgress) =>
        {
            //this.FindControl<ProgressBar>("ProcessBar").Maximum = zipProgress.Total;
            //this.FindControl<ProgressBar>("ProcessBar").Value = zipProgress.Processed;
            //this.FindControl<TextBlock>("InfoLabel").Text = $"Download: {zipProgress.Total} / {zipProgress.Processed}";
            // todo: progress
        };
        using (FileStream zipToOpen = new FileStream("./.cache/"+param3, FileMode.Open))
        {
            ZipArchive zipp = new ZipArchive(zipToOpen);
            zipp.ExtractToDirectory(param2, progress);
        }
        dirCache.Delete(true);
        // todo: leave from process
        // yes
        _isDownload = false;
    }
}