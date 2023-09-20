
// (c) 2023 Kazuki KOHZUKI

using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using TAMAI.Data;
using TAMAI.Spectra;
using DecayItem = System.Collections.Generic.KeyValuePair<TAMAI.Spectra.Time, TAMAI.Spectra.Signal>;
using MBrushes = System.Windows.Media.Brushes;
using WlRange = TAMAI.Win.Controls.ValueRange<TAMAI.Spectra.Wavelength>;

namespace TAMAI.Win.Controls;

internal static class DecayUtils
{
    private static CartesianMapper<DecayItem> GetCartesianMapper(TasTypes tasTypes, Func<double, double> Xmapper, Func<double, double> Ymapper)
    {
        var bias = tasTypes switch
        {
            TasTypes.MicroSecond => 1e6,
            TasTypes.FemtoSecond => 1e9,
            _ => 1
        };
        return new CartesianMapper<DecayItem>()
            .X(item => Xmapper(item.Key.Second * bias))
            .Y(item => Ymapper((double)item.Value.MilliOD));
    }

    private static ChartValues<DecayItem> ToChartDecay(this Decay decay)
        => new(decay);

    private static Decay GetDecay(this Spectra.Spectra spectra, WlRange range)
        => spectra[range.Start, range.End];

    internal static LineSeries GetLineSeries(this Spectra.Spectra spectra, WlRange range, TasTypes tasTypes, Func<double, double> Xmapper, Func<double, double> Ymapper)
        => new()
        {
            Title = range.ToString(),
            Configuration = GetCartesianMapper(tasTypes, Xmapper, Ymapper),
            Values = spectra.GetDecay(range).ToChartDecay(),
            Fill = MBrushes.Transparent,
            PointGeometrySize = 0,
            LineSmoothness = 0,
        };

    internal static LineSeries GetLineSeries(this (IEnumerable<double> time, IEnumerable<double> signals) data, string title, TasTypes tasTypes, Func<double, double> Xmapper, Func<double, double> Ymapper)
        => new()
        {
            Title = title,
            Configuration = GetCartesianMapper(tasTypes, Xmapper, Ymapper),
            Values = new Decay(data.time.Select(t => new Time(t)), data.signals.Select(s => new Signal(s))).ToChartDecay(),
            Fill = MBrushes.Transparent,
            PointGeometrySize = 0,
        };
} // internal static class DecayUtils
