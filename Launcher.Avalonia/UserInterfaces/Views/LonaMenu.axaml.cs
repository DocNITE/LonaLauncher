using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Launcher.Avalonia.UserInterfaces.Views;

public partial class LonaMenu : StackPanel
{
    public LonaMenu()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}