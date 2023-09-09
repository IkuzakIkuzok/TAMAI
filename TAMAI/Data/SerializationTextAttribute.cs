
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Data;

/// <summary>
/// Represents a string value for serialization.
/// </summary>
public class SerializationTextAttribute : TextValueAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SerializationTextAttribute"/> class,
    /// </summary>
    /// <param name="text"><inheritdoc/></param>
    public SerializationTextAttribute(string text) : base(text) { }
} // public class SerializationTextAttribute : TextValueAttribute
