
// (c) 2023 Kazuki KOHZUKI

using System.Reflection;

namespace TAMAI.Win.Controls;

[DesignerCategory("code")]
internal abstract class AutoResizeForm : Form
{
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        RegisterControls();
    } // protected override void OnShown (EventArgs)

    private void RegisterControls()
    {
        var type = GetType();

        var size = type.GetCustomAttribute<InitialSizeAttribute>();
        var width = size?.Width ?? this.Width;
        var height = size?.Height ?? this.Height;

        var fields = type.GetFields(
            BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic
        );

        foreach (var field in fields)
        {
            var attr = field.GetCustomAttribute<ResizeModeAttribute>();
            if (attr == null) continue;
            if (attr.ResizeType == ResizeType.Fixed) continue;

            if (field.GetValue(this) is not Control control) continue;

            var horizontal = attr.HorizontalResizeType;
            if (horizontal != ResizeType.Fixed)
            {
                SizeChanged += ((Func<Control, int, EventHandler>?)(horizontal switch
                {
                    ResizeType.AdjustWidth => GetAdjustWidthEventHandler,
                    ResizeType.AdjustLeft => GetAdjustLeftEventHandler,
                    _ => null,
                }))?.Invoke(control, width);
            }

            var vertical = attr.VerticalResizeType;
            if (vertical != ResizeType.Fixed)
            {
                SizeChanged += ((Func<Control, int, EventHandler>?)(vertical switch
                {
                    ResizeType.AdjustHeight => GetAdjustHeightEventHandler,
                    ResizeType.AdjustTop => GetAdjustTopEventHandler,
                    _ => null,
                }))?.Invoke(control, height);
            }
        }
    } // private void RegisterControls ()

    private EventHandler GetAdjustWidthEventHandler(Control target, int width)
    {
        var l = width - target.Width;
        return (sender, e) => target.Width = this.Width - l;
    } // private EventHandler GetAdjustWidthEventHandler (Control, int)

    internal EventHandler GetAdjustLeftEventHandler(Control target, int width)
    {
        var l = width - target.Left;
        return (sender, e) => target.Left = this.Width - l;
    } // internal EventHandler GetAdjustLeftEventHandler (Control, int)

    internal EventHandler GetAdjustHeightEventHandler(Control target, int height)
    {
        var l = height - target.Height;
        return (sender, e) => target.Height = this.Height - l;
    } // internal EventHandler GetAdjustHeightEventHandler (Control, int)

    internal EventHandler GetAdjustTopEventHandler(Control target, int height)
    {
        var l = height - target.Top;
        return (sender, e) => target.Top = this.Height - l;
    } // internal EventHandler GetAdjustTopEventHandler (Control, int)
} // internal abstract class AutoResizeForm : Form
