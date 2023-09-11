
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI;

/// <summary>
/// Specifies the properties of a physical quantity.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public class PhysicalQuantityAttribute(string value, string unit) : Attribute
{
    /// <summary>
    /// Gets the name of field or property to get the value.
    /// </summary>
    public string ValueName { get; } = value;

    /// <summary>
    /// Gets the most common unit.
    /// </summary>
    public string Unit { get; } = unit;

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Unit"/> accepts SI prefix.
    /// </summary>
    public bool AcceptSIPrefix { get; set; } = true;

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    public double DefaultValue { get; set; } = 0;
} // public class PhysicalQuantityAttribute : Attribute
