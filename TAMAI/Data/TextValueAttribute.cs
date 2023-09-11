
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Data;

/// <summary>
/// Defines text value to serialize a value.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TextValueAttribute"/> class.
/// </remarks>
/// <param name="text">The text value.</param>
public abstract class TextValueAttribute(string text) : Attribute
{
    /// <summary>
    /// Gets the text value.
    /// </summary>
    public string Text { get; } = text;

    /// <summary>
    /// Gets or sets the value whether the current value is the default one.
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Checks the current instance is valid or not.
    /// </summary>
    /// <returns><c>true</c> if the current instance is valid; otherwise, <c>false</c>.</returns>
    virtual public bool IsValid() => true;
} // public abstract class TextValueAttribute : Attribute
