
// (c) 2023 Kazuki KOHZUKI

using System.Text;
using System.Xml.Serialization;

namespace TAMAI.Data;

/// <summary>
/// Serializes common metadata.
/// </summary>
[Serializable]
[XmlRoot("common-metadata")]
public sealed class CommonMetaData
{
    private const string FILENAME = "metadata.xml";

    /// <summary>
    /// Gets or sets the TAS measurement type.
    /// </summary>
    [XmlElement("type")]
    public SerializableEnum<TasTypes> TasType { get; set; } = TasTypes.NotSpecified;

    /// <summary>
    /// Gets or sets the sample name for the current data.
    /// </summary>
    [XmlElement("sample-name")]
    public string? SampleName { get; set; }

    /// <summary>
    /// Gets or sets the comment for the current data.
    /// </summary>
    [XmlElement("comment")]
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonMetaData"/> class.
    /// </summary>
    public CommonMetaData() { }

    internal void Save(string directory)
    {
        var path = Path.Combine(directory, FILENAME);
        using var writer = new StreamWriter(path, false, Encoding.UTF8);
        var serializer = new XmlSerializer(typeof(CommonMetaData));
        serializer.Serialize(writer, this);
    } // internal static void Save (string)

    internal static CommonMetaData Load(string directory)
    {
        var path = Path.Combine(directory, FILENAME);
        using var reader = new StreamReader(path, Encoding.UTF8, true);
        var serializer = new XmlSerializer(typeof(CommonMetaData));
        return serializer.Deserialize(reader) as CommonMetaData ?? throw new IOException("Unable to load metadata.");
    } // internal static CommonMetaData (string)
} // public sealed class CommonMetaData
