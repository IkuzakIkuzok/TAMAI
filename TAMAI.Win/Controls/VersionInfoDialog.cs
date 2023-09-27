
// (c) 2023 Kazuki KOHZUKI

using System.Reflection;
using TAMAI.Win.Properties;
using SpectraClass = TAMAI.Spectra.Spectra;

namespace TAMAI.Win.Controls;

[DesignerCategory("code")]
internal sealed class VersionInfoDialog : Form
{
    private const int ICON_SIZE = 64;

    internal VersionInfoDialog()
    {
        this.Text = Resources.TitleVersionInfo;
        this.Size = this.MinimumSize = this.MaximumSize = new Size(300, 180);
        this.MaximizeBox = false;

        _ = new PictureBox()
        {
            Top = 20,
            Left = 20,
            Size = new(ICON_SIZE, ICON_SIZE),
            Image = new Icon(Resources.Icon, new(ICON_SIZE, ICON_SIZE)).ToBitmap(),
            Parent = this,
        };

        _ = new Label()
        {
            Text = "TAMAI:",
            Top = 20,
            Left = 100,
            Width = 80,
            Parent = this,
        };

        _ = new Label()
        {
            Text = "TAMAI.Win:",
            Top = 50,
            Left = 100,
            Width = 80,
            Parent = this,
        };

        var TAMAI = typeof(SpectraClass).Assembly;
        var TAMAIWin = Assembly.GetExecutingAssembly();

        _ = new Label()
        {
            Text = GetVersionString(TAMAI),
            Top = 20,
            Left = 180,
            Width = 100,
            Parent = this,
        };

        _ = new Label()
        {
            Text = GetVersionString(TAMAIWin),
            Top = 50,
            Left = 180,
            Width = 100,
            Parent = this,
        };

        var ok = new Button()
        {
            Text = Resources.OK,
            Top = 90,
            Left = 100,
            Size = new(80, 30),
            Parent = this,
        };
        ok.Click += (sender, e) => Close();
        this.CancelButton = ok;
    } // internal VersionInfoDialog()

    private static string GetVersionString(Assembly assembly)
    {
        var version = assembly.GetName().Version;
        if (version == null) return "N/A";
        return $"{version.Major}.{version.Minor}.{version.Build}";
    } // private static string GetVersionString (Assembly)
} // internal sealed class VersionInfoDialog
