
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Data;

/// <summary>
/// Represents a directory of the raw data.
/// </summary>
public sealed class RawDataDirectory : RawData
{
    private readonly List<RawData> files = new();

    /// <summary>
    /// Gets the files and directories in this directory.
    /// </summary>
    public IReadOnlyList<RawData> Files => this.files;

    /// <summary>
    /// Initializes a new instance of the <see cref="RawDataDirectory"/> class.
    /// </summary>
    /// <param name="filename">The filename of the directory.</param>
    public RawDataDirectory(string filename) : base(filename)
    {
        foreach (var dir in Directory.GetDirectories(filename))
            this.files.Add(new RawDataDirectory(dir));

        foreach (var file in Directory.EnumerateFiles(filename))
            this.files.Add(new RawDataFile(file));
    } // ctor (string) : base(string)

    /// <inheritdoc/>
    override public void SaveTo(string path)
    {
        var fullpath = Path.Combine(path, this.Filename);
        Directory.CreateDirectory(fullpath);

        foreach (var child in this.files)
            child.SaveTo(fullpath);
    } // override public void SaveTo (string)
} // public sealed class RawDataDirectory : RawData
