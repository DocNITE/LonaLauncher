using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Launcher.Avalonia;
using Launcher.Client.Settings;

namespace Launcher.Avalonia;

public partial class MainWindow : Window
{
    private StackPanel? _currentWindow;
    public MainWindow()
    {
        InitializeComponent();

        LonaMenu.OnChangeWindow += s =>
        {
            switch (s)
            {
                case "GameButton":
                    OnChangeWindow(LonaGame);
                    break;
            }
        };
        
        // init settings
        Config.Load();
        
        // TODO: Auto update files/chekin'
    }

    private void OnChangeWindow(StackPanel panel)
    {
        if (_currentWindow != null)
            _currentWindow.IsVisible = false;

        _currentWindow = panel;
        _currentWindow.IsVisible = true;
    }
}