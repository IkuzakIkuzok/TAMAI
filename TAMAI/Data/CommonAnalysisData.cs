
// (c) 2023 Kazuki KOHZUKi

using System.Text;
using System.Xml.Serialization;
using TAMAI.Spectra;

namespace TAMAI.Data;

/// <summary>
/// Serializes common analysis data.
/// </summary>
[Serializable]
public sealed class CommonAnalysisData
{
    private const string FILENAME = "analysis.xml";

    /// <summary>
    /// Gets or sets the spectra range.
    /// </summary>
    [XmlArray("spectra-range")]
    [XmlArrayItem("range")]
    public List<SerializableValueRange<Time>> SpectraRange { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonAnalysisData"/> class.
    /// </summary>
    public CommonAnalysisData() { }

    internal void Save(string directory)
    {
        var path = Path.Combine(directory, FILENAME);
        using var writer = new StreamWriter(path, false, Encoding.UTF8);
        var serializer = new XmlSerializer(typeof(CommonAnalysisData));
        serializer.Serialize(writer, this);
    } // internal void Save (string)

    internal static CommonAnalysisData Load(string directory)
    {
        var path = Path.Combine(directory, FILENAME);
        using var reader = new StreamReader(path, Encoding.UTF8, true);
        var serializer = new XmlSerializer(typeof(CommonAnalysisData));
        return serializer.Deserialize(reader) as CommonAnalysisData ?? throw new IOException("Unable to load analysis data.");
    } // internal static CommonAnalysisData Load (string)
} // public sealed class CommonAnalysisData
