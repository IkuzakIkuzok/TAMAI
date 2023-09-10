
// (c) 2023 Kazuki KOHZUKI

using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using System.IO;
using System.Text;
using TAMAI.Data;
using TAMAI.Spectra;
using TAMAI.Win.Properties;
using TAMAI.Win.Utils;
using MBrushes = System.Windows.Media.Brushes;

namespace TAMAI.Win.Controls;

[InitialSize(800, 650)]
internal class MainForm : AutoResizeForm
{
    private TasData? data;

    [ResizeMode(ResizeType.AdjustSize)]
    private readonly LiveCharts.WinForms.CartesianChart chart;

    [ResizeMode(ResizeType.AdjustTop | ResizeType.AdjustWidth)]
    private readonly RangeSelectionTable<Time> TimeRangeSelectionTable;

    [ResizeMode(ResizeType.AdjustLeft)]
    private readonly TextBox tb_sampleName;

    [ResizeMode(ResizeType.AdjustLeft | ResizeType.AdjustHeight)]
    private readonly TextBox tb_comment;

    private readonly ToolStripMenuItem menu_save, menu_saveAs, menu_exportSpectra;

    private string? SampleName => this.data?.Metadata.SampleName;

    internal MainForm()
    {
        SetTitle();
        this.Size = this.MinimumSize = new(800, 650);

        var axisX = new Axis()
        {
            Title = "Wavelength / nm",
        };

        var axisY = new Axis()
        {
            Title = "ΔmOD",
            LabelFormatter = (x) => $"{x:F3}",
        };

        axisX.Separator.Stroke = axisY.Separator.Stroke = MBrushes.LightGray;

        this.chart = new()
        {
            Top = 50,
            Left = 20,
            Size = new(400, 300),
            AxisX = new AxesCollection { axisX },
            AxisY = new AxesCollection { axisY },
            Zoom = ZoomingOptions.Xy,
            DisableAnimations = true,
            Parent = this,
        };

        this.TimeRangeSelectionTable = new()
        {
            Top = 370,
            Left = 20,
            Size = new(400, 200),
            Parent = this,
        };
        this.TimeRangeSelectionTable.RangesUpdated += DrawSpectra;
        this.TimeRangeSelectionTable.Columns[1].Width =
        this.TimeRangeSelectionTable.Columns[2].Width = 148;

        this.tb_sampleName = new()
        {
            Top = 50,
            Left = 450,
            Width = 300,
            PlaceholderText = Resources.PlaceholderSampleName,
            Enabled = false,
            Parent  = this,
        };
        this.tb_sampleName.TextChanged +=
            (sender, e) => this.data!.Metadata.SampleName = this.tb_sampleName.Text;

        this.tb_comment = new()
        {
            Top = 80,
            Left = 450,
            Multiline = true,
            Size = new(300, 200),
            PlaceholderText = Resources.PlaceholderSampleComment,
            Enabled = false,
            Parent = this,
        };
        this.tb_comment.TextChanged +=
            (sender, e) => this.data!.Metadata.Comment = this.tb_comment.Text;

        #region menu

        var ms = new MenuStrip()
        {
            Parent = this,
        };

        #region menu.file

        var file = new ToolStripMenuItem()
        {
            Text = Resources.MenuFile,
        };
        ms.Items.Add(file);

        var open = new ToolStripMenuItem()
        {
            Text = Resources.MenuFileOpen,
        };
        file.DropDownItems.Add(open);

        var openUsTas = new ToolStripMenuItem()
        {
            Text = Resources.MenuFileOpenUsTas,
            ShortcutKeys = Keys.Control | Keys.Shift | Keys.U,
        };
        openUsTas.Click += OpenUsTasData;
        open.DropDownItems.Add(openUsTas);

        open.DropDownItems.Add("-");

        var openSavedTas = new ToolStripMenuItem()
        {
            Text = Resources.MenuFileOpenSavedTas,
            ShortcutKeys = Keys.Control | Keys.O,
        };
        openSavedTas.Click += OpenSavedTasData;
        open.DropDownItems.Add(openSavedTas);

        this.menu_save = new()
        {
            Text = Resources.MenuFileSave,
            ShortcutKeys = Keys.Control | Keys.S,
            Enabled = false,
        };
        this.menu_save.Click += SaveTasData;
        file.DropDownItems.Add(this.menu_save);

        this.menu_saveAs = new()
        {
            Text = Resources.MenuFileSaveAs,
            ShortcutKeys = Keys.Control | Keys.Shift | Keys.S,
            Enabled = false,
        };
        this.menu_saveAs.Click += SaveAs;
        file.DropDownItems.Add(this.menu_saveAs);

        file.DropDownItems.Add("-");

        var exit = new ToolStripMenuItem()
        {
            Text = Resources.MenuFileExit,
            ShortcutKeys = Keys.Alt | Keys.F4,
        };
        exit.Click += CloseForm;
        file.DropDownItems.Add(exit);

        #endregion menu.file

        #region menu.tool

        var tool = new ToolStripMenuItem()
        {
            Text = Resources.MenuTool,
        };
        ms.Items.Add(tool);

        this.menu_exportSpectra = new()
        {
            Text = Resources.MenuToolExportSpectra,
            Enabled = false,
        };
        this.menu_exportSpectra.Click += ExportSpectra;
        tool.DropDownItems.Add(this.menu_exportSpectra);

        #endregion menu.tool

        #endregion menu
    } // ctor ()

    internal MainForm(string? filename) : this()
    {
        if (File.Exists(filename))
            OpenSavedTasData(filename);
    } // ctor (string?)

    private void SetTitle()
    {
        this.Text = Resources.ProductName;
        if (!string.IsNullOrEmpty(this.SampleName))
            this.Text += $" - {this.SampleName}";
    } // private void SetTitle ()

    private void CloseForm(object? sender, EventArgs e)
    {
        if (this.data != null)
        {
            var dr = MessageBox.Show(
                Resources.MessageSaveBeforeClose,
                Resources.ProductName,
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            if (dr == DialogResult.Yes)
            {
                if (!Save()) return;
            }
            else if (dr != DialogResult.No) return;
        }
        Application.Exit();
    } // private void CloseForm (object?, EventArgs)

    #region open

    private void LoadTasData<T>(Func<string, Task<T>> tasLoader, Action setTimeRange, string path) where T : TasData, new()
    {
        void PostLoadTask(Task _)
        {
            if (this.data == null)
            {
                ToastNotification.Show(Resources.MessageErrorOpen);
                return;
            }

            Invoke(() =>
            {
                this.tb_sampleName.Text = this.data.Metadata.SampleName;
                this.tb_comment.Text = this.data.Metadata.Comment;

                SetDataManipulationEnabled();
                SetTitle();
                DrawSpectra();
            });
        } // void PostLoadTask (Task)

        tasLoader(path)
            .ContinueWith(data => this.data = data.Result)
            .ContinueWith((_) => Invoke(setTimeRange))
            .ContinueWith(PostLoadTask);
    } // private void LoadTasData (Func<string, Task<TasData>>, Action, string)

    private void OpenUsTasData(object? sender, EventArgs e)
    {
        if (!CheckUnsaved()) return;

        using var ofd = new CommonOpenFileDialog()
        {
            Title = Resources.TitleOpenUsTas,
            IsFolderPicker = true,
        };

        if (ofd.ShowDialog() != CommonFileDialogResult.Ok) return;

        LoadTasData(MicroSecondTasData.LoadAsync, SetUsDefaultTimeRange, ofd.FileName);
    } // private void OpenUsTasData (object?, EventArgs)

    private void OpenFsTasData(object? sender, EventArgs e)
    {
        // TODO: implement
    } // private void OpenFsTasData (object?, EventArgs)

    private void OpenSavedTasData(object? sender, EventArgs e)
    {
        if (!CheckUnsaved()) return;

        using var ofd = new OpenFileDialog()
        {
            Title = Resources.TitleOpenSavedTas,
            Filter = Resources.FilterOpenSavedTas,
        };

        if (ofd.ShowDialog() != DialogResult.OK) return;

        OpenSavedTasData(ofd.FileName);
    } // private void OpenSavedTasData (object?, EventArgs)

    private void OpenSavedTasData(string filename)
    {
        var ext = Path.GetExtension(filename).ToUpper();
        if (ext == ".USTAS")
        {
            LoadTasData(TasData.LoadAsync<MicroSecondTasData>, SetUsDefaultTimeRange, filename);
        }
        else if (ext == ".FSTAS")
        {
            //TODO: implement fsTAS
            //this.data = TasData.Load<FemtoSecondTasData>(filename);
        }
        else
        {
            MessageBox.Show(
                Resources.MessageNotSupportedFormat,
                Resources.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation
            );
            return;
        }
    } // private void OpenSavedTasData (string)

    private bool CheckUnsaved()
    {
        if (this.data == null) return true;

        var dr = MessageBox.Show(
            Resources.MessageSaveBeforeOpen,
            Resources.ProductName,
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question
        );

        if (dr == DialogResult.Yes) return Save();
        if (dr == DialogResult.No) return true;
        return false;
    } // private bool CheckUnsaved ()

    #endregion open

    #region save

    private void SaveTasData(object? sender, EventArgs e)
        => Save();

    private bool Save()
    {
        if (this.data == null) return false;

        if (string.IsNullOrEmpty(this.data.Filename)) return SaveAs();

        try
        {
            this.data.Save();
            ToastNotification.Show(Resources.MessageFileSaved);
        }
        catch (Exception e)
        {
            e.MakeCrashReport();
            MessageBox.Show(
                Resources.MessageErrorSave,
                Resources.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            return false;
        }

        SetTitle();

        return true;
    } // private bool Save ()

    private void SaveAs(object? sender, EventArgs e)
        => SaveAs();

    private bool SaveAs()
    {
        if (this.data == null) return false;
        
        var title = (TasTypes)this.data.Metadata.TasType switch
        {
            TasTypes.MicroSecond => Resources.TitleSaveUsTas,
            TasTypes.FemtoSecond => Resources.TitleSaveFsTas,
            _ => string.Empty,
        };

        var filter = (TasTypes)this.data.Metadata.TasType switch
        {
            TasTypes.MicroSecond => Resources.FilterSaveUsTas,
            TasTypes.FemtoSecond => Resources.FilterSaveFsTas,
            _ => string.Empty,
        };

        var defaultName = CreateDefaultFilename();

        using var sfd = new SaveFileDialog()
        {
            Title = title,
            Filter = filter,
            FileName = defaultName,
        };

        if (sfd.ShowDialog() != DialogResult.OK) return false;

        try
        {
            this.data.SaveAs(sfd.FileName);
            ToastNotification.Show(Resources.MessageFileSaved);
        }
        catch (Exception e)
        {
            e.MakeCrashReport();
            MessageBox.Show(
                Resources.MessageErrorSave,
                Resources.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            return false;
        }

        SetTitle();

        return true;
    } // private bool SaveAs ()

    private string CreateDefaultFilename(string extension = "")
    {
        var sampleName = this.SampleName;
        if (string.IsNullOrEmpty(sampleName)) return string.Empty;

        foreach (var c in Path.GetInvalidFileNameChars())
            sampleName = sampleName.Replace(c, '_');

        return $"{sampleName}{extension}";
    } // private string CreateDefaultFilename ()

    #endregion save

    private void ExportSpectra(object? sender, EventArgs e)
        => ExportSpectra();

    private void ExportSpectra()
    {
        if (this.data?.Spectra == null) return;

        var ranges = this.TimeRangeSelectionTable.Ranges;
        if (!ranges.Any()) return;

        using var sfd = new SaveFileDialog()
        {
            Title = Resources.TitleExportSpectra,
            Filter = Resources.FilterExportSpectra,
            FileName = CreateDefaultFilename("_spectra.csv"),
        };
        if (sfd.ShowDialog() != DialogResult.OK) return;

        try
        {
            var filename = sfd.FileName;
            using var sw = new StreamWriter(filename, false, Encoding.UTF8);
            this.data.Spectra.Export(sw, ranges);
            new ToastNotification(Resources.MessageSpectraSaved)
                .AddButton(Resources.ToastOpenFile, _ => new Process() { StartInfo = new(filename) { UseShellExecute = true, } }.Start())
                .AddButton(Resources.ToastOpenDir, _ => Process.Start("explorer.exe", $"/select,\"{filename}\""))
                .Show();
        }
        catch (Exception e)
        {
            e.MakeCrashReport();
            MessageBox.Show(
                Resources.MessageErrorExportSpectra,
                Resources.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    } // private void ExportSpectra ()

    private void SetDataManipulationEnabled()
    {
        if (this.data == null) return;

        this.tb_sampleName.Enabled = this.tb_comment.Enabled = true;
        this.menu_save.Enabled = this.menu_saveAs.Enabled = this.menu_exportSpectra.Enabled = true;
    } // private void SetDataManipulationEnabled ()

    private void SetUsDefaultTimeRange()
    {
        if (this.data == null) return;
        this.TimeRangeSelectionTable.Unit = "us";
        this.TimeRangeSelectionTable.Bias = 0.00_000_1;
        this.TimeRangeSelectionTable.Rows.Clear();
        if (!this.data.AnalysisData.SpectraRange.Any())
        {
            this.data.AnalysisData.SpectraRange.AddRange(new SerializableValueRange<Time>[]
            {
                // TODO: determine appropriate default values from measured data
                new(Time.FromDouble(1.0e-6), Time.FromDouble(1.2e-6)),
                new(Time.FromDouble(2.0e-6), Time.FromDouble(2.5e-6)),
                new(Time.FromDouble(3.0e-6), Time.FromDouble(5.0e-6)),
            });
        }

        foreach (var range in this.data.AnalysisData.SpectraRange)
            this.TimeRangeSelectionTable.Add(Math.Round(range.Start.MicroSecond, 5), Math.Round(range.End.MicroSecond, 6));
    } // private void SetUsDefaultTimeRange ()

    private void DrawSpectra(object? sender, EventArgs e)
        => DrawSpectra();

    private void DrawSpectra()
    {
        try
        {
            var spectra = this.data?.Spectra;
            if (spectra == null) return;

            this.chart.Series.Clear();
            this.data!.AnalysisData.SpectraRange.Clear();

            foreach (var range in this.TimeRangeSelectionTable.Ranges)
            {
                this.data.AnalysisData.SpectraRange.Add(new(range.Start, range.End));
                var series = spectra.GetLineSeries(range);
                this.chart.Series.Add(series);
            }
        }
        catch (Exception e)
        {
            e.MakeCrashReport();
            MessageBox.Show(
                Resources.MessageErrorDrawingSpectra,
                Resources.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    } // private void DrawSpectra ()
} // internal class MainForm : AutoResizeForm
