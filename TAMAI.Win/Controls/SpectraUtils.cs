
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Win.Controls;

using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System.Windows.Media;
using TAMAI.Spectra;
using SpectrumItem = KeyValuePair<Spectra.Wavelength, Spectra.Signal>;


internal static class SpectraUtils
{
    private static readonly CartesianMapper<SpectrumItem> spectrumMapper = new CartesianMapper<SpectrumItem>()
        .X(x => x.Key.Value)
        .Y(x => (double)x.Value.MilliOD);

    private static ChartValues<SpectrumItem> ToChartSpectrum(this Spectrum spectrum)
        => new (spectrum);

    internal static LineSeries GetLineSeries(this Spectra spectra, Time start, Time end)
    {
        var spectrum = spectra[start, end];
        return new()
        {
            Title = $"{new ScientificValue<Time>(start)}–{new ScientificValue<Time>(end)}",
            Configuration = spectrumMapper,
            Values = spectrum.ToChartSpectrum(),
            Fill = Brushes.Transparent,
        };
    } // internal static LineSeries GetLineSeries (this Spectra, Time, Time)
} // internal static class SpectraUtils
