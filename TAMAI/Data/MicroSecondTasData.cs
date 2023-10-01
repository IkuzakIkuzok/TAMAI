
// (c) 2023 Kazuki KOHZUKI

using System.Text.RegularExpressions;
using TAMAI.Spectra;
using TAMAI.Stats;
using TAMAI.Utils;

namespace TAMAI.Data;

/// <summary>
/// Represents usTAS data.
/// </summary>
public sealed class MicroSecondTasData : TasData
{
    private const string BACKGROUND = "background.csv";

    /// <summary>
    /// Gets or sets a suffix of the name of a directory in which raw data are saved.
    /// </summary>
    public static string WavelengthSuffix { get; set; } = "nm";

    /// <summary>
    /// Gets or sets a suffix of the filename for data with both pump and probe light.
    /// </summary>
    public static string ASignalSuffix { get; set; } = "-a.csv";

    /// <summary>
    /// Gets or sets a suffix of the filename for data only with pump light.
    /// </summary>
    public static string BSignalSuffix { get; set; } = "-b.csv";

    /// <summary>
    /// Gets or sets a suffix of the filename for difference data.
    /// </summary>
    public static string DiffSignalSuffix { get; set; } = "-a-b.csv";

    /// <summary>
    /// Gets or sets a suffix of the filename for smoothed difference data.
    /// </summary>
    public static string SmoothedSignalSuffix { get; set; } = "-a-b-tdm.csv";

    /// <summary>
    /// Gets the analysis data for usTAS measurement.
    /// </summary>
    public MicroSecondTasAnalysisData MicrosecondAnalysisData { get; private set; } = new();

    /// <summary>
    /// Gets the background spectrum.
    /// </summary>
    /// <remarks>
    /// The background spectrum is an averaged spectrum in the time range between the first data point and the half of t0.
    /// </remarks>
    public Spectrum? BackgroundSpectrum { get; private set; }

    /// <summary>
    /// Gets or sets the function to decide t0.
    /// </summary>
    /// <remarks>
    /// It is possible to customize how t0 is determined by setting this property.
    /// The list of raw data directories is passed to the function.
    /// By default, t0 is determined as the peak position of the B signal, with statistical outlier exclusion.
    /// </remarks>
    public Func<IEnumerable<RawDataDirectory>, Time>? DetermineT0 { get; set; } = null;

    private IEnumerable<RawDataDirectory> WlDirs
        => this.rawData
        .Where(f => f is RawDataDirectory)
        .Cast<RawDataDirectory>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MicroSecondTasData"/> class.
    /// </summary>
    public MicroSecondTasData() : base(Array.Empty<string>())
    {
        this.Metadata.TasType = TasTypes.MicroSecond;
    } // ctor ()

    /// <summary>
    /// Initializes a new instance of the <see cref="MicroSecondTasData"/> class
    /// with the raw data specified by <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The path at which the raw data is located.</param>
    public MicroSecondTasData(string path) : base(Directory.EnumerateDirectories(path))
    {
        this.Metadata.TasType = TasTypes.MicroSecond;
        this.Metadata.SampleName = Path.GetFileName(path);
        MakeSpectra();
    } // ctor (string)

    /// <summary>
    /// Asynchronously loads usTAS data.
    /// </summary>
    /// <param name="path">The path at which the raw data is loaded.</param>
    /// <returns>A task that represents the asynchronous load operation,
    /// which wraps the usTAS data.</returns>
    public static async Task<MicroSecondTasData> LoadAsync(string path)
        => await Task.Run(() => new MicroSecondTasData(path));

    /// <inheritdoc/>
    override protected void SaveTypeSpecificData(string dataDir)
    {
        this.MicrosecondAnalysisData.Save(dataDir);

        var bg = Path.Combine(dataDir, BACKGROUND);
        this.BackgroundSpectrum?.Save(bg);
    } // override protected void SaveTypeSpecificData ()

    /// <inheritdoc/>
    override protected void LoadTypeSpecificData(string dataDir)
    {
        this.MicrosecondAnalysisData = MicroSecondTasAnalysisData.Load(dataDir);

        var bg = Path.Combine(dataDir, BACKGROUND);
        if (File.Exists(bg))
            this.BackgroundSpectrum = Spectrum.Load(bg);
    } // override protected void LoadTypeSpecificData ()

    private void MakeSpectra()
    {
        var re_wl = new Regex(@$"^(\d+){WavelengthSuffix}$");

        var wavelengths = new SortedDictionary<Wavelength, RawDataDirectory>();
        var l_time = -1;
        var time = Array.Empty<Time>();
        foreach (var dir in this.WlDirs)
        {
            var name = dir.Filename;
            var m = re_wl.Match(name);
            if (!m.Success) continue;
            if (!double.TryParse(m.Groups[1].Value, out var wl)) continue;
            wavelengths.Add(new(wl), dir);

            if (l_time >= 0) continue;
            if (dir.Files[0] is not RawDataFile file) continue;
            var data = file.TextData;
            if (data == null) continue;
            time = data.Split('\n')
                .Select(row => row.Split(',')[0])
                .Select(val => val.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(double.Parse)
                .Select(t => new Time(t))
                .ToArray();
            l_time = time.Length;
        }

        static IEnumerable<double> GetSignal(RawDataFile file)
            => file.TextData.Split('\n')
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(row => row.Split(',')[1])
                .Select(val => val.Trim())
                .Select(double.Parse);

        if (this.DetermineT0 != null)
        {
            this.MicrosecondAnalysisData.T0 = this.DetermineT0(this.WlDirs);
        }
        else
        {
            var indices = new int[wavelengths.Count];
            foreach ((var i, var dir) in wavelengths.Values.Enumerate())
            {
                var file = (RawDataFile)dir.Files.First(f => f.Filename.EndsWith(BSignalSuffix));
                var data = GetSignal(file).Select(double.Abs).Take(l_time >> 1).ToArray();
                var maximum = data.Max();
                var index = Array.IndexOf(data, maximum);
                indices[i] = index;
            }
            this.MicrosecondAnalysisData.T0 = indices.SmirnovGrubbs().Select(i => time[i]).Average();
        }

        var spectra = new double[l_time, wavelengths.Count];
        foreach ((var idx_wl, var dir) in wavelengths.Values.Enumerate())
        {
            var file = (RawDataFile)dir.Files.First(f => f.Filename.EndsWith(SmoothedSignalSuffix));
            var data = GetSignal(file);

            foreach ((var idx_time, var v) in data.Enumerate())
                spectra[idx_time, idx_wl] = v;
        }

        var rawSpectra = this.Spectra = new(
            time,
            Enumerable.Range(0, l_time)
                .Select(t => Enumerable.Range(0, wavelengths.Count)
                    .Select(wl => spectra[t, wl])
                    .Select(s => new Signal(s))
                )
                .Select(s => new Spectrum(wavelengths.Keys, s))
                .ToArray()
        );

        var t_BG = (Time)this.MicrosecondAnalysisData.T0 / 2;
        this.BackgroundSpectrum = rawSpectra[Time.Zero, t_BG];

        this.Spectra = new(
            time.Select(t => t - this.MicrosecondAnalysisData.T0)
            .Select(t => new ScientificValue<Time>(t).Text)
            .Select(s => new ScientificValue<Time>(s).Value)
            .ToArray(),
            Enumerable.Range(0, l_time)
            .Select(t => rawSpectra[time[t]])
            .Select(s => s - this.BackgroundSpectrum)
            .ToArray()
        );
    } // private void MakeSpectra ()
} // public sealed class MicroSecondTasData : TasData
