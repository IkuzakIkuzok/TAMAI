
// (c) 2023 Kazuki KOHZUKI

using System.Diagnostics.CodeAnalysis;

namespace TAMAI.Spectra;

/// <summary>
/// Represents a TA signals.
/// </summary>
[PhysicalQuantity(nameof(OD), "OD")]
public readonly struct Signal : IRatioScalePhysicalQuantity<Signal>, IFormattable
{
    private static readonly Signal zero = new();

   
    /// <inheritdoc/>
    public static Signal Zero => zero;

    /// <summary>
    /// Gets the singnal intensity, in OD.
    /// </summary>
    public double OD { get; } = .0;

    /// <summary>
    /// Gets the singnal intensity, in mOD.
    /// </summary>
    public Signal MilliOD => this * 1_000;

    /// <summary>
    /// Gets the singnal intensity, in uOD.
    /// </summary>
    public Signal MicroOD => this * 1_000_000;

    /// <summary>
    /// Gets the absolute value of the signal.
    /// </summary>
    public Signal Absolute => Abs(this);

    /// <summary>
    /// Initializes a new instance of the <see cref="Signal"/> structure.
    /// </summary>
    /// <param name="od">The signal value, in OD.</param>
    public Signal(double od)
    {
        this.OD = od;
    } // ctor (double)

    /// <inheritdoc/>
    public static Signal FromDouble(double od) => new(od);

    /// <summary>
    /// Cast a <see cref="Signal"/> into <see cref="double"/>, with unit of OD.
    /// </summary>
    /// <param name="s">The <see cref="Signal"/> to be casted into <see cref="double"/>.</param>
    public static explicit operator double(Signal s)
        => s.OD;

    /// <inheritdoc/>
    override public bool Equals([NotNullWhen(true)] object? obj)
        => obj is Signal s && this == s;

    /// <inheritdoc/>
    override public int GetHashCode()
        => this.OD.GetHashCode();

    /// <summary>
    /// Converts the signal of this instance to its equivalent string representation.
    /// </summary>
    /// <param name="format">A numeric format string.</param>
    /// <param name="formatProvider">An object that supplied culture-specific formatting information.</param>
    /// <returns>The string representation of the value of this instance
    /// as specified by <paramref name="format"/> and <paramref name="formatProvider"/></returns>
    public string ToString(string? format, IFormatProvider? formatProvider)
        => this.OD.ToString(format, formatProvider);

    /// <inheritdoc/>
    public static Signal Abs(Signal s)
        => new(Math.Abs(s.OD));

    /// <inheritdoc/>
    public static Signal Ln(Signal s)
        => new(Math.Log(s.OD));

    /// <inheritdoc/>
    public static Signal Log(Signal s)
        => new(Math.Log10(s.OD));

    /// <inheritdoc/>
    public int CompareTo(Signal other)
        => Math.Abs(this.OD - other.OD) < double.Epsilon * Math.Max((double)this.Absolute, (double)other.Absolute) ? 0 : this.OD.CompareTo(other.OD);

    #region operators

    /// <inheritdoc/>
    public static bool operator ==(Signal s1, Signal s2)
        => s1.CompareTo(s2) == 0;

    /// <inheritdoc/>
    public static bool operator !=(Signal s1, Signal s2)
        => !(s1 == s2);

    /// <inheritdoc/>
    public static bool operator >(Signal s1, Signal s2)
        => s1.OD > s2.OD;

    /// <inheritdoc/>
    public static bool operator >=(Signal s1, Signal s2)
        => s1 > s2 || s1 == s2;

    /// <inheritdoc/>
    public static bool operator <(Signal s1, Signal s2)
        => !(s1 >= s2);

    /// <inheritdoc/>
    public static bool operator <=(Signal s1, Signal s2)
        => !(s1 > s2);

    /// <inheritdoc/>
    public static Signal operator +(Signal s1, Signal s2)
        => new(s1.OD + s2.OD);

    /// <inheritdoc/>
    public static Signal operator -(Signal s1, Signal s2)
        => new(s1.OD - s2.OD);

    /// <summary>
    /// Calculates the signal multiplied by a constant.
    /// </summary>
    /// <param name="s">The signal to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied signal.</returns>
    public static Signal operator *(Signal s, int a)
        => new(s.OD * a);

    /// <summary>
    /// Calculates the signal multiplied by a constant.
    /// </summary>
    /// <param name="s">The signal to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied signal.</returns>
    public static Signal operator *(int a, Signal s)
        => s * a;

    /// <summary>
    /// Calculates the signal multiplied by a constant.
    /// </summary>
    /// <param name="s">The signal to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied signal.</returns>
    public static Signal operator *(Signal s, float a)
        => new(s.OD * a);

    /// <summary>
    /// Calculates the signal multiplied by a constant.
    /// </summary>
    /// <param name="s">The signal to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied signal.</returns>
    public static Signal operator *(float a, Signal s)
        => s * a;

    /// <summary>
    /// Calculates the signal multiplied by a constant.
    /// </summary>
    /// <param name="s">The signal to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied signal.</returns>
    public static Signal operator *(Signal s, double a)
        => new(s.OD * a);

    /// <summary>
    /// Calculates the signal multiplied by a constant.
    /// </summary>
    /// <param name="s">The signal to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied signal.</returns>
    public static Signal operator *(double a, Signal s)
        => s * a;

    /// <summary>
    /// Calculates the ratio of two signals.
    /// </summary>
    /// <param name="s1">The first value to calculates ratio.</param>
    /// <param name="s2">The second value to calculates ratio.</param>
    /// <returns>The ratio of <paramref name="s1"/> and <paramref name="s2"/>.</returns>
    public static Signal operator /(Signal s1, Signal s2)
        => new(s1.OD / s2.OD);

    /// <summary>
    /// Calculates the signal divided by a constant.
    /// </summary>
    /// <param name="s">The signal to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided signal.</returns>
    public static Signal operator /(Signal s, int a)
        => new(s.OD / a);

    /// <summary>
    /// Calculates the signal divided by a constant.
    /// </summary>
    /// <param name="s">The signal to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided signal.</returns>
    public static Signal operator /(Signal s, float a)
        => new(s.OD / a);

    /// <summary>
    /// Calculates the signal divided by a constant.
    /// </summary>
    /// <param name="s">The signal to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided signal.</returns>
    public static Signal operator /(Signal s, double a)
        => new(s.OD / a);

    #endregion operators
} // public readonly struct Signal : IRatioScalePhysicalQuantity<Signal>
