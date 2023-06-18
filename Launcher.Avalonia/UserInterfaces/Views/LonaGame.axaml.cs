using System;
using System.IO;
using System.IO.Compression;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Launcher.Avalonia.Settings;

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
    
    private void OnClick(object? sender, RoutedEventArgs e)
    {
        //var btn = (Button)sender!;
        //OnUpdateStatus?.Invoke(GameStatus.Update);
        // create cache

        if(!_isDownload)
            DownloadProcess();
    }

    private async void DownloadProcess()
    {
        _isDownload = true;
        
        var param1 = Config.GetField("game_url");
        var param2 = Config.GetField("game_path");
        var param3 = "GameCache";
        
        var dirCache = new DirectoryInfo("./.cache");
        if (dirCache.Exists)
            dirCache.Delete(true);
        dirCache.Create();
        var http = new HttpDownload(param1, "./.cache/" + param3);
        // get version game
        var response = await http.NetClient.GetAsync(Config.GetField("game_version_url"));

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to retrieve file, status code: {response.StatusCode}");
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        // return if versions was equal
        if (content == Config.GetField("game_version"))
        {
            this.FindControl<TextBlock>("InfoLabel").IsVisible = true;
            this.FindControl<TextBlock>("InfoLabel").Text = "You already have actual version game!";
            dirCache.Delete(true);
            return;
        }
        else
        {
            Config.GetFieldObject("game_version").Data = content;
            Config.Save();
        }
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
        Progress<ZipProgress> _dprogress;
        _dprogress = new Progress<ZipProgress>();
        _dprogress.ProgressChanged += (object sender, ZipProgress zipProgress) =>
        {
            this.FindControl<ProgressBar>("ProcessBar").Maximum = zipProgress.Total;
            this.FindControl<ProgressBar>("ProcessBar").Value = zipProgress.Processed;
            this.FindControl<TextBlock>("InfoLabel").Text = $"Download: {zipProgress.Total} / {zipProgress.Processed}";
            //Console.WriteLine("Extracting: " + zipProgress.Total + ", " + zipProgress.Processed);
        };
        using (FileStream zipToOpen = new FileStream("./.cache/"+param3, FileMode.Open))
        {
            ZipArchive zipp = new ZipArchive(zipToOpen);
            zipp.ExtractToDirectory(param2, _dprogress);
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