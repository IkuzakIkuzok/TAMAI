
// (c) 2023 Kazuki KOHZUKI

using System.Xml.Serialization;

namespace TAMAI.Win.Config;

[Serializable]
public sealed class ColorGradientConfig
{
    [XmlElement("start")]
    public SerializableColor StartColor { get; set; } = new() { Color = Color.Red };

    [XmlElement("end")]
    public SerializableColor EndColor { get; set; } = new() { Color = Color.Blue };

    public ColorGradientConfig() { }
} // sealed class ColorGradientConfig
