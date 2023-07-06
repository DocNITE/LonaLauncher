using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Launcher.Avalonia.Settings;
using Launcher.LoaderAPI;

namespace Launcher.Avalonia.UserInterfaces.Views;

public enum GameStatus
{
    None = 0,
    Update = 1,
    Ready = 2
}

public partial class LonaGame : StackPanel
{
    public Action<GameStatus>? OnUpdateStatus;

    private bool _isDownload = false;
    public LonaGame()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private async void OnClick(object? sender, RoutedEventArgs e)
    {
        //var btn = (Button)sender!;
        //OnUpdateStatus?.Invoke(GameStatus.Update);
        // create cache

        if (!_isDownload)
        {
            await DownloadProcess("game_url", "game_path", "game_version_url", "game_version");
            await DownloadProcess("patch_url", "game_path", "patch_version_url", "patch_version");
        }
    }

    public async Task DownloadProcess(string url, string path, string versionUrl, string? currentVersion = "-1")
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
            //this.FindControl<TextBlock>("InfoLabel").IsVisible = true;
            //this.FindControl<TextBlock>("InfoLabel").Text = "You already have actual version game!";
            dirCache.Delete(true);
            _isDownload = false;
            return;
        }
        Console.WriteLine("Update version: " + content);
        Config.GetFieldObject(currentVersion).Data = content;
        Config.Save();
        // enable some buttons
        this.FindControl<ProgressBar>("ProcessBar").IsEnabled = true;
        this.FindControl<TextBlock>("InfoLabel").IsVisible = true;
        this.FindControl<Button>("GetGameButton").IsEnabled = false;
        // process
        http.ProgressChanged += (long? totalFileSize, long totalBytesDownloaded, double? progressPercentage) =>
        {
            this.FindControl<ProgressBar>("ProcessBar").Maximum = (double)totalFileSize;
            this.FindControl<ProgressBar>("ProcessBar").Value = (double)totalBytesDownloaded;
            this.FindControl<TextBlock>("InfoLabel").Text = $"Download: {totalFileSize} / {totalBytesDownloaded}";
            /*
             * Console.WriteLine("Download: " + (totalFileSize.ToString() ?? string.Empty) + ", " + totalBytesDownloaded.ToString() +
                              ", " + progressPercentage.ToString());
             */

        };
        await http.StartDownload();
        Console.WriteLine("Finished!");
        Console.WriteLine("Start process...");
        Progress<ZipProgress> progress = new();
        progress.ProgressChanged += (object sender, ZipProgress zipProgress) =>
        {
            this.FindControl<ProgressBar>("ProcessBar").Maximum = zipProgress.Total;
            this.FindControl<ProgressBar>("ProcessBar").Value = zipProgress.Processed;
            this.FindControl<TextBlock>("InfoLabel").Text = $"Download: {zipProgress.Total} / {zipProgress.Processed}";
            //Console.WriteLine("Extracting: " + zipProgress.Total + ", " + zipProgress.Processed);
        };
        using (FileStream zipToOpen = new FileStream("./.cache/"+param3, FileMode.Open))
        {
            ZipArchive zipp = new ZipArchive(zipToOpen);
            zipp.ExtractToDirectory(param2, progress);
        }
        dirCache.Delete(true);
        // disable some buttons
        this.FindControl<ProgressBar>("ProcessBar").IsEnabled = !this.FindControl<ProgressBar>("ProcessBar").IsEnabled;
        this.FindControl<TextBlock>("InfoLabel").IsVisible = !this.FindControl<TextBlock>("InfoLabel").IsVisible;
        this.FindControl<Button>("GetGameButton").IsEnabled = !this.FindControl<Button>("GetGameButton").IsEnabled;
        // yes
        _isDownload = false;
    }
}