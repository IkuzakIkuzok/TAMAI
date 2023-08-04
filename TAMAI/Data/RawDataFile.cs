
// (c) 2023 Kazuki KOHZUKI

using TAMAI.Utils;

namespace TAMAI.Data;

/// <summary>
/// Represents a file of the raw data.
/// </summary>
public sealed class RawDataFile : RawData
{
    private readonly byte[] data;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="RawDataFile"/> class.
    /// </summary>
    /// <param name="filename">The filename of the file.</param>
    public RawDataFile(string filename) : base(filename)
    {
        this.data = File.ReadAllBytes(filename);
    } // ctor (string) : base(string)

    /// <inheritdoc/>
    override public void SaveTo(string path)
    {
        var fullpath = Path.Combine(path, this.Filename);
        File.WriteAllBytes(fullpath, this.data);
    } // override public void SaveTo (string)
} // public sealed class RawDataFile : RawData
