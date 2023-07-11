using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Launcher.Avalonia.UserInterfaces.Controls;

public class PanelWidget : Control
{
    // Override the OnRender method to define the drawing logic
    public override void Render(DrawingContext context)
    {
        var rect = new Rect(new Size(Bounds.Width, Bounds.Height));
        var brush = new SolidColorBrush(Colors.Blue);
        context.FillRectangle(brush, rect, 10);
    }
}