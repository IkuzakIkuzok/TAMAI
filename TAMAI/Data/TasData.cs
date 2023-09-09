
// (c) 2023 Kazuki KOHZUKI

using System.IO.Compression;
using System.Text;

namespace TAMAI.Data;

/// <summary>
///  Represents a TAS data set.
/// </summary>
public abstract class TasData
{
    internal const string TEMP_DIR = "TAMAI";
    internal const string RAW_DIR = "raw";
    internal const string SPECTRA_DIR = "spectra";

    private static readonly string TempDir = Path.Combine(Path.GetTempPath(), TEMP_DIR);

    /// <summary>
    /// Gets or set the compression level of TAS data files.
    /// </summary>
    public static CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    /// <summary>
    /// A list of raw data. This field is readonly.
    /// </summary>
    protected readonly List<RawData> rawData = new();

    /// <summary>
    /// Gets a filename of the data file.
    /// </summary>
    public string? Filename { get; protected set; }

    /// <summary>
    /// Metadata of the current TAS data.
    /// </summary>
    public CommonMetaData Metadata { get; private set; } = new();

    /// <summary>
    /// Gets the analysis data.
    /// </summary>
    public CommonAnalysisData AnalysisData { get; private set; } = new();

    /// <summary>
    /// Gets raw data.
    /// </summary>
    public IReadOnlyList<RawData> RawData => this.rawData;

    /// <summary>
    /// Gets the TA spectra.
    /// </summary>
    public Spectra.Spectra? Spectra { get; protected set; }

    internal TasData(IEnumerable<string> rawFiles)
    {
        LoadRawData(rawFiles);
    } // ctor (IEnumerable<string>)

    #region load/save

    private void LoadRawData(IEnumerable<string> rawFiles)
    {
        foreach (var file in rawFiles)
        {
            if (File.Exists(file))
                this.rawData.Add(new RawDataFile(file));
            else
                this.rawData.Add(new RawDataDirectory(file));
        }
    } // private void LoadRawData (IEnumerable<string>)

    /// <summary>
    ///  Saves the current data to a file.
    /// </summary>
    /// <exception cref="InvalidOperationException">The filename is not specified.</exception>
    public void Save()
    {
        if (this.Filename == null)
            throw new InvalidOperationException("The location is not specified. Use `SaveAs` instead.");

        var dataDir = Path.Combine(TempDir, Path.GetFileNameWithoutExtension(this.Filename));

        try
        {
            Directory.CreateDirectory(dataDir);

            this.Metadata.Save(dataDir);
            this.AnalysisData.Save(dataDir);

            var rawDir = Path.Combine(dataDir, RAW_DIR);
            foreach (var raw in this.rawData)
                raw.SaveTo(rawDir);

            var spectraDir = Path.Combine(dataDir, SPECTRA_DIR);
            this.Spectra?.Save(spectraDir);

            SaveTypeSpecificData(dataDir);

            File.Delete(this.Filename);
            ZipFile.CreateFromDirectory(dataDir, this.Filename, CompressionLevel, false, Encoding.UTF8);
        }
        finally
        {
            Directory.Delete(dataDir, true);
        }       
    } // public void Save ()

    /// <summary>
    /// Saves the current data to the specified file.
    /// </summary>
    /// <param name="filename">The filename.</param>
    public void SaveAs(string filename)
    {
        this.Filename = filename;
        Save();
    } // public void SaveAs(string)

    /// <summary>
    /// Loads saved TAS data.
    /// </summary>
    /// <typeparam name="T">The type of TAS data.</typeparam>
    /// <param name="filename">The filename of the TAS data.</param>
    /// <returns>The loaded TAS data.</returns>
    public static T Load<T>(string filename) where T : TasData, new()
    {
        var dataDir = Path.Combine(TempDir, Path.GetFileNameWithoutExtension(filename));
        try
        {
            ZipFile.ExtractToDirectory(filename, dataDir, Encoding.UTF8, true);
            var rawDir = Path.Combine(dataDir, RAW_DIR);
            var spectraDir = Path.Combine(dataDir, SPECTRA_DIR);

            var data = new T
            {
                Filename = filename,
                Metadata = CommonMetaData.Load(dataDir),
                AnalysisData = CommonAnalysisData.Load(dataDir),
                Spectra = TAMAI.Spectra.Spectra.Load(spectraDir),
            };

            data.LoadRawData(new[] { rawDir });

            data.LoadTypeSpecificData(dataDir);

            return data;
        }
        finally
        {
            Directory.Delete(dataDir, true);
        }
    } // public static T Load<T>(string filename) where T : TasData, new()

    /// <summary>
    /// Asynchronously loads saved TAS data.
    /// </summary>
    /// <typeparam name="T">The type of TAS data.</typeparam>
    /// <param name="filename">The filename of the TAS data.</param>
    /// <returns>A task that represents the asynchronous load operation,
    /// which wraps the TAS data.</returns>
    public static async Task<T> LoadAsync<T>(string filename) where T : TasData, new()
        => await Task.Run(() => Load<T>(filename));

    /// <summary>
    /// Saves the type-specific data.
    /// </summary>
    /// <param name="dataDir">The directory to which the data are saved.</param>
    protected virtual void SaveTypeSpecificData(string dataDir) { }

    /// <summary>
    /// Loads the type-specific data.
    /// </summary>
    /// <param name="dataDir">The directory from which the data are loaded.</param>
    protected virtual void LoadTypeSpecificData(string dataDir) { }

    #endregion load/save
} // public class TasData
