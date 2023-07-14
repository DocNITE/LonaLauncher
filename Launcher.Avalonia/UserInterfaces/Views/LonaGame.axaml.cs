using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Launcher.Client;
using Launcher.Client.Settings;
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
       
        await Static.HttpManager.DownloadCoreFile("game",
            Config.GetField("game_url"), 
            Config.GetField("game_path"),
            Config.GetField("game_version_url"),
            Config.GetField("game_version"));
        
        await Static.HttpManager.DownloadCoreFile("patch",
            Config.GetField("patch_url"), 
            Config.GetField("game_path"),
            Config.GetField("patch_version_url"),
            Config.GetField("patch_version"));
    }
}