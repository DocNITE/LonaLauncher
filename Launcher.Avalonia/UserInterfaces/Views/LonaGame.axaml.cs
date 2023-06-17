using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Launcher.Avalonia.UserInterfaces.Views;

public partial class LonaGame : StackPanel
{
    public LonaGame()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}