
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Win.Controls;

using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.IO;
using System.Windows.Media;
using TAMAI.Spectra;
using TAMAI.Win.Utils;
using SpectrumItem = KeyValuePair<Spectra.Wavelength, Spectra.Signal>;
using TimeRange = ValueRange<Spectra.Time>;

internal static class SpectraUtils
{
    private static readonly CartesianMapper<SpectrumItem> spectrumMapper = new CartesianMapper<SpectrumItem>()
        .X(x => x.Key.Value)
        .Y(x => (double)x.Value.MilliOD);

    private static ChartValues<SpectrumItem> ToChartSpectrum(this Spectrum spectrum)
        => new (spectrum);

    private static Spectrum GetSpectrum(this Spectra spectra, TimeRange timeRange)
        => spectra[timeRange.Start, timeRange.End];

    internal static LineSeries GetLineSeries(this Spectra spectra, TimeRange range)
    {
        var spectrum = spectra.GetSpectrum(range);
        return new()
        {
            Title = range.ToString(),
            Configuration = spectrumMapper,
            Values = spectrum.ToChartSpectrum(),
            Fill = Brushes.Transparent,
        };
    } // internal static LineSeries GetLineSeries (this Spectra, TimeRange)

    internal static void Export(this Spectra spectra, TextWriter writer, IEnumerable<TimeRange> ranges)
    {
        if (!ranges.Any()) return;

        // header
        writer.Write("Wavelength [nm],");
        writer.WriteLine(string.Join(',', ranges));

        var wavelengths = spectra.GetSpectrum(ranges.First()).Wavelengths;
        var l_wl = wavelengths.Length;
        var l_time = ranges.Count();
        var signals = new double[l_wl, l_time];
        foreach ((var i, var range) in ranges.Enumerate())
        {
            var spectrum = spectra.GetSpectrum(range);
            foreach ((var j, var s) in spectrum.Enumerate())
                signals[j, i] = s.Value.OD;
        }

        foreach ((var i, var wl) in wavelengths.ToArray().Enumerate())
        {
            writer.Write(wl);
            for (var j = 0; j < l_time; ++j)
            {
                writer.Write(',');
                writer.Write(signals[i, j]);
            }
            writer.WriteLine();
        }
    } // internal static void Export(this Spectra, TextWriter, IEnumerable<TimeRange>)
} // internal static class SpectraUtils
