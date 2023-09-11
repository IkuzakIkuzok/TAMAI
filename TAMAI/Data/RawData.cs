
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Data;

/// <summary>
/// Represents a raw data of the TAS measurement.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RawData"/> class.
/// </remarks>
/// <param name="filename">The filename of the data.</param>
public abstract class RawData(string filename)
{
    /// <summary>
    /// Gets the filename of the data.
    /// </summary>
    public string Filename { get; } = Path.GetFileName(filename);

    /// <summary>
    /// Saves the current data in the specified path.
    /// </summary>
    /// <param name="path">The path to the location in which the current object is saved.</param>
    public abstract void SaveTo(string path);
} // public abstract class RawData
