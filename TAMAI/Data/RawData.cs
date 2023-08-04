
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Data;

/// <summary>
/// Represents a raw data of the TAS measurement.
/// </summary>
public abstract class RawData
{
    /// <summary>
    /// Gets the filename of the data.
    /// </summary>
    public string Filename { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RawData"/> class.
    /// </summary>
    /// <param name="filename">The filename of the data.</param>
    public RawData(string filename)
    {
        this.Filename = Path.GetFileName(filename);
    } // ctor (string)

    /// <summary>
    /// Saves the current data in the specified path.
    /// </summary>
    /// <param name="path">The path to the location in which the current object is saved.</param>
    public abstract void SaveTo(string path);
} // public abstract class RawData
