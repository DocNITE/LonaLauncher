using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Launcher.Avalonia.UserInterfaces.Windows;

public partial class UpdateChecker : Window
{
    public UpdateChecker()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}