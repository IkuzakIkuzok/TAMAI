
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Data;

/// <summary>
/// Represents a string value for serialization.
/// </summary>
public class SerializatoinTextAttribute : TextValueAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SerializatoinTextAttribute"/> class,
    /// </summary>
    /// <param name="text"><inheritdoc/></param>
    public SerializatoinTextAttribute(string text) : base(text) { }
} // public class SerializatoinTextAttribute : TextValueAttribute
