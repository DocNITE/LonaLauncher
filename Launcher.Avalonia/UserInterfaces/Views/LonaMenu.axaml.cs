using System;
using System.Runtime.Loader;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Launcher.Avalonia.UserInterfaces.Views;

public partial class LonaMenu : StackPanel
{
    public Action<string>? OnChangeWindow;
    public LonaMenu()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnClick(object? sender, RoutedEventArgs e)
    {
        var btn = (Button)sender!;
        OnChangeWindow?.Invoke(btn.Name!);
    }
}