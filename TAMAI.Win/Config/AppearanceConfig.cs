
// (c) 2023 Kazuki KOHZUKI

using System.Xml.Serialization;

namespace TAMAI.Win.Config;

[Serializable]
public sealed class AppearanceConfig
{
    [XmlElement("color-gradient")]
    public ColorGradientConfig ColorGradientConfig { get; set; } = new();

    public AppearanceConfig() { }
} // sealed class AppearanceConfig
