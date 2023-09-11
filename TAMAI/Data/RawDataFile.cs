
// (c) 2023 Kazuki KOHZUKI

using TAMAI.Utils;

namespace TAMAI.Data;

/// <summary>
/// Represents a file of the raw data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RawDataFile"/> class.
/// </remarks>
/// <param name="filename">The filename of the file.</param>
public sealed class RawDataFile(string filename) : RawData(filename)
{
    private readonly byte[] data = File.ReadAllBytes(filename);

    /// <summary>
    /// Gets the file content.
    /// </summary>
    public ReadOnlySpan<byte> Data => this.data;

    /// <summary>
    /// Gets the text content of the current data.
    /// </summary>
    public string TextData
    {
        get
        {
            var encoding = this.data.GetEncoding();
            return encoding?.GetString(this.data) ?? throw new IOException("Cannot load raw data as a text file.");
        }
    }

    /// <inheritdoc/>
    override public void SaveTo(string path)
    {
        var fullpath = Path.Combine(path, this.Filename);
        File.WriteAllBytes(fullpath, this.data);
    } // override public void SaveTo (string)
} // public sealed class RawDataFile : RawData
