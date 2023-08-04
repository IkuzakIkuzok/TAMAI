
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI;

/// <summary>
/// Specifies the properties of a physical quantity.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public class PhysicalQuantityAttribute : Attribute
{
    /// <summary>
    /// Gets the name of field or property to get the value.
    /// </summary>
    public string ValueName { get; }

    /// <summary>
    /// Gets the most common unit.
    /// </summary>
    public string Unit { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Unit"/> accepts SI prefix.
    /// </summary>
    public bool AcceptSIPrefix { get; set; } = true;

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public double DefaultValue { get; set; } = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="PhysicalQuantityAttribute"/> class
    /// with the name of a value and its unit.
    /// </summary>
    /// <param name="value">The name of a value to handle.</param>
    /// <param name="unit">The unit for the value.</param>
    public PhysicalQuantityAttribute(string value, string unit)
    {
        this.ValueName = value;
        this.Unit = unit;
    } // ctor (string, string)
} // public class PhysicalQuantityAttribute : Attribute
