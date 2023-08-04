
// (c) 2023 Kazuki KOHZUKI

using System.Text;
using System.Xml.Serialization;
using TAMAI.Spectra;

namespace TAMAI.Data;

/// <summary>
/// Serializes analysis data for usTAS.
/// </summary>
[Serializable]
public sealed class MicroSecondTasAnalysisData
{
    private const string FILENAME = "ustas.xml";

    /// <summary>
    /// Gets or sets the time zero.
    /// </summary>
    [XmlElement("t0")]
    public ScientificValue<Time> T0 { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MicroSecondTasAnalysisData"/> class.
    /// </summary>
    public MicroSecondTasAnalysisData() { }

    internal void Save(string directory)
    {
        var path = Path.Combine(directory, FILENAME);
        using var writer = new StreamWriter(path, false, Encoding.UTF8);
        var serializer = new XmlSerializer(typeof(MicroSecondTasAnalysisData));
        serializer.Serialize(writer, this);
    } // internal void Save (string)

    internal static MicroSecondTasAnalysisData Load(string directory)
    {
        var path = Path.Combine(directory, FILENAME);
        using var reader = new StreamReader(path, Encoding.UTF8, true);
        var serializer = new XmlSerializer(typeof(MicroSecondTasAnalysisData));
        return serializer.Deserialize(reader) as MicroSecondTasAnalysisData ?? throw new IOException("Unable to analysis data.");
    } // internal static MicroSecondTasAnalysisData Load (string)
} // public sealed class MicroSecondTasAnalysisData
