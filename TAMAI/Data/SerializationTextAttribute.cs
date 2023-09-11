
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Data;

/// <summary>
/// Represents a string value for serialization.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SerializationTextAttribute"/> class,
/// </remarks>
/// <param name="text"><inheritdoc/></param>
public class SerializationTextAttribute(string text) : TextValueAttribute(text);
