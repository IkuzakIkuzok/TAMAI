
// (c) 2023 Kazuki KOHZUKI

using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace TAMAI.Win.Config;

[Serializable]
[XmlRoot("appSettings")]
public sealed class AppConfig
{
    private const string FILENAME = "TAMAIforWin.config";

    private static readonly string FullPath = Path.Combine(Program.AppDataPath, FILENAME);

    [XmlElement("appearance")]
    public AppearanceConfig AppearanceConfig { get; set; } = new();

    public AppConfig() { }

    internal static AppConfig Load()
    {
        if (!File.Exists(FullPath)) return new AppConfig();
        using var reader = new StreamReader(FullPath, Encoding.UTF8);
        return (AppConfig)new XmlSerializer(typeof(AppConfig)).Deserialize(reader)!;
    } // internal static AppConfig Load ()

    internal void Save()
    {
        if (!Directory.Exists(Program.AppDataPath)) Directory.CreateDirectory(Program.AppDataPath);
        using var writer = new StreamWriter(FullPath, false, Encoding.UTF8);
        new XmlSerializer(typeof(AppConfig)).Serialize(writer, this);
    } // internal void Save ()
} // sealed class AppConfig
