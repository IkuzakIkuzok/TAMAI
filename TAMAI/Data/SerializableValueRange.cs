
// (c) 2023 Kazuki KOHZUKI

using System.Xml.Serialization;

namespace TAMAI.Data;

/// <summary>
/// Represents a range of values.
/// </summary>
/// <typeparam name="T">The type of the range.</typeparam>
[Serializable]
public sealed class SerializableValueRange<T> where T : IPhysicalQuantity<T>
{
    /// <summary>
    /// Gets or sets the start value of the range.
    /// </summary>
    [XmlIgnore]
    public T Start { get; set; }

    /// <summary>
    /// Gets or sets the end value of the range.
    /// </summary>
    [XmlIgnore]
    public T End { get; set; }

    /// <summary>
    /// Gets or sets the string representation of the start value of the range.
    /// </summary>
    [XmlAttribute("start")]
    public string StartString
    {
        get => new ScientificValue<T>(this.Start).Text;
        set => this.Start = new ScientificValue<T>(value).Value!;
    }

    /// <summary>
    /// Gets or sets the string representation of the end value of the range.
    /// </summary>
    [XmlAttribute("end")]
    public string EndString
    {
        get => new ScientificValue<T>(this.End).Text;
        set => this.End = new ScientificValue<T>(value).Value!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableValueRange{T}"/> class.
    /// </summary>
    public SerializableValueRange()
    {
        this.Start = default!;
        this.End = default!;
    } // ctor ()

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableValueRange{T}"/> class with default values.
    /// </summary>
    /// <param name="start">The start value of the range.</param>
    /// <param name="end">The end value of the range.</param>
    public SerializableValueRange(T start, T end)
    {
        this.Start = start;
        this.End = end;
    } // ctor (T, T)
} // public sealed class SerializableValueRange
