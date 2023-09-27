
// (c) 2023 Kazuki KOHZUKI

using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using TAMAI.Analysis;
using TAMAI.Data;
using TAMAI.Spectra;
using TAMAI.Win.Analysis;
using TAMAI.Win.Properties;
using TAMAI.Win.Utils;
using WMBrushes = System.Windows.Media.Brushes;

namespace TAMAI.Win.Controls;

internal sealed class KineticsForm : AutoResizeForm
{
    private readonly TasData data;
    private readonly IEnumerable<KeyValuePair<Guid, IDecayFunctionModel>> functions;

    [ResizeMode(ResizeType.AdjustSize)]
    private readonly LiveCharts.WinForms.CartesianChart chart;

    private readonly CheckBox cb_semilogX, cb_semilogY, cb_absolute;

    [ResizeMode(ResizeType.AdjustLeft | ResizeType.AdjustHeight)]
    private readonly WavelengthRangeSelectionTable wavelengthRangeSelectionTable;

    private Func<double, double> XMapper => this.cb_semilogX.Checked ? Math.Log10 : x => x;

    private Func<double, double> YMapper
    {
        get
        {
            if (this.cb_absolute.Checked && this.cb_semilogY.Checked)
                return x => Math.Log10(Math.Abs(x));
            else if (this.cb_absolute.Checked)
                return x => Math.Abs(x);
            else if (this.cb_semilogY.Checked)
                return x => Math.Log10(x);
            else
                return x => x;
        }
    }

    internal KineticsForm(TasData data)
    {
        this.data = data;
        this.functions = DecayFittingFunctions.Functions;

        this.Text = Resources.TitleTimeEvolution;
        this.Size = this.MinimumSize = new(800, 400);

        this.cb_semilogX = new()
        {
            Text = Resources.TextSemilogX,
            Top = 20,
            Left = 20,
            Width = 80,
            Parent = this,
        };
        this.cb_semilogX.CheckedChanged += DrawDecays;

        this.cb_semilogY = new()
        {
            Text = Resources.TextSemilogY,
            Checked = true,
            Top = 40,
            Left = 20,
            Width = 80,
            Parent = this,
        };
        this.cb_semilogY.CheckedChanged += DrawDecays;

        this.cb_absolute = new()
        {
            Text = Resources.TextAbsSignal,
            Checked = true,
            Top = 40,
            Left = 100,
            Width = 80,
            Parent = this,
        };
        this.cb_absolute.CheckedChanged += DrawDecays;

        var timeUnit = (TasTypes)data.Metadata.TasType switch
        {
            TasTypes.MicroSecond => "us",
            TasTypes.FemtoSecond => "fs",
            _ => "s",
        };

        var axisX = new Axis()
        {
            Title = $"Time / ({timeUnit})",
        };

        var axisY = new Axis()
        {
            Title = "ΔmOD",
            LabelFormatter = (x) => $"{x:F3}",
        };

        axisX.Separator.Stroke = axisY.Separator.Stroke = WMBrushes.LightGray;

        this.chart = new()
        {
            Top = 80,
            Left = 20,
            Size = new(250, 250),
            AxisX = new AxesCollection { axisX },
            AxisY = new AxesCollection { axisY },
            Zoom = ZoomingOptions.Xy,
            DisableAnimations = true,
            Parent = this,
        };

        this.wavelengthRangeSelectionTable = new(functions)
        {
            Top = 20,
            Left = 300,
            Size = new(450, 310),
            Parent = this,
        };
        this.wavelengthRangeSelectionTable.RangesUpdated += DrawDecays;
    } // ctor (TasData)

    private void DrawDecays(object? sender, EventArgs e)
        => DrawDecays();

    private void DrawDecays()
    {
        using var _ = new ControlDrawingSuspender(this);

        try
        {
            var spectra = this.data.Spectra;
            if (spectra == null) return;

            lock (spectra)
            {
                this.chart.Series.Clear();

                this.chart.AxisX[0].LabelFormatter = (x) => $"{(this.cb_semilogX.Checked ? Math.Pow(10, x) : x):F3}";
                this.chart.AxisY[0].LabelFormatter = (y) => $"{(this.cb_semilogY.Checked ? Math.Pow(10, y) : y):F3}";

                var rows = this.wavelengthRangeSelectionTable.Rows
                    .Cast<DataGridViewRow>()
                    .Select(r => r as WavelengthRangeSelectionTableRow)
                    .Where(r => r?.Checked ?? false)
                    .SkipLast(1)
                    .ToList();

                var gradient = new ColorGradient(Program.GradientStart, Program.GradientEnd, rows.Count);
                var fittedList = new List<LineSeries>();
                foreach ((var i, var row) in rows.Enumerate())
                {
                    var color = gradient[i];
                    var range = row!.Range;
                    var decay = spectra.GetLineSeries(range, this.data.Metadata.TasType, this.XMapper, this.YMapper);
                    decay.Stroke = color;
                    this.chart.Series.Add(decay);

                    var model = row.Function;
                    if (model == null) continue;
                    var d = spectra[range.Start, range.End];
                    var n = d.Count;
                    var i0 = d.Select((x, i) => new { Index = i, Values = x })
                        .Where(x => x.Values.Key >= Time.Zero)
                        .First().Index;
                    var X = d.Skip(i0).Select(x => x.Key.Second);
                    var Y = d.Skip(i0).Select(x => x.Value.Absolute.OD);
                    var func = model.GetFunc(X, Y, row.FittingArgs);
                    var fitted = X.Select(func.Func);
                    var timeConstant = new ScientificValue<Time>(new Time(func.TimeConstant)).Text;
                    var fittedSeries = (X, fitted).GetLineSeries(timeConstant, this.data.Metadata.TasType, this.XMapper, this.YMapper);
                    fittedSeries.Stroke = color;
                    fittedSeries.StrokeDashArray = [2];
                    fittedList.Add(fittedSeries);
                    row.TimeConstant = timeConstant;
                }

                foreach (var fitted in fittedList)
                    this.chart.Series.Add(fitted);
            }
        }
        catch (Exception e)
        {
            e.MakeCrashReport();
        }
    } // private void DrawDecays ()
} // internal sealed class KineticsForm : AutoResizeForm
