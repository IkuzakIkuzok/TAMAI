
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Data;

/// <summary>
/// Defines text value to serialize a value.
/// </summary>
public abstract class TextValueAttribute : Attribute
{
    /// <summary>
    /// Gets the text value.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets or sets the value whether the current value is the default one.
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextValueAttribute"/> class.
    /// </summary>
    /// <param name="text">The text value.</param>
    public TextValueAttribute(string text)
    {
        this.Text = text;
    } // ctor (string)

    /// <summary>
    /// Checks the current instance is valid or not.
    /// </summary>
    /// <returns><c>true</c> if the current instance is valid; otherwise, <c>false</c>.</returns>
    virtual public bool IsValid() => true;
} // public abstract class TextValueAttribute : Attribute
