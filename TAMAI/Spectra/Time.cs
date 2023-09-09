
// (c) 2023 Kazuki KOHZUKI

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TAMAI.Spectra;

/// <summary>
/// Represents a time.
/// </summary>
[DebuggerDisplay("{Second} s")]
[PhysicalQuantity(nameof(Second), "s")]
public readonly struct Time : IRatioScalePhysicalQuantity<Time>
{
    private static readonly Time zero = new();

    /// <inheritdoc/>
    public static Time Zero => zero;

    /// <summary>
    /// Gets the time value, in s.
    /// </summary>
    public double Second { get; } = .0;

    /// <summary>
    /// Gets the time value, in ms.
    /// </summary>
    public Time MilliSecond => this * 1_000;

    /// <summary>
    /// Gets the time value, in us.
    /// </summary>
    public Time MicroSecond => this * 1_000_000;

    /// <summary>
    /// Gets the time value, in ns.
    /// </summary>
    public Time NanoSecond => this * 1_000_000_000;

    /// <summary>
    /// Gets the time value, in ps.
    /// </summary>
    public Time PicoSecond => this * 1_000_000_000_000;

    /// <summary>
    /// Gets the time value, in fs.
    /// </summary>
    public Time FemtoSecond => this * 1_000_000_000_000_000;

    /// <summary>
    /// Initializes a new instance of the <see cref="Time"/> structure.
    /// </summary>
    /// <param name="second">The time, in s.</param>
    public Time(double second)
    {
        this.Second = second;
    } // ctor (double)

    /// <inheritdoc/>
    public static Time FromDouble(double second) => new(second);

    /// <inheritdoc/>
    override public bool Equals([NotNullWhen(true)] object? obj)
        => obj is Time time && time == this;

    /// <inheritdoc/>
    override public int GetHashCode()
        => this.Second.GetHashCode();

    /// <summary>
    /// Casts a <see cref="Time"/> into a <see cref="double"/>, with unit of s.
    /// </summary>
    /// <param name="time">The <see cref="Time"/> to be casted into <see cref="double"/>.</param>
    public static explicit operator double(Time time)
        => time.Second;

    /// <inheritdoc/>
    public static Time Abs(Time t)
        => new(Math.Abs(t.Second));

    /// <inheritdoc/>
    public static Time Ln(Time t)
        => new(Math.Log(t.Second));

    /// <inheritdoc/>
    public static Time Log(Time t)
        => new(Math.Log10(t.Second));

    /// <inheritdoc/>
    public int CompareTo(Time other)
        => Math.Abs(this.Second - other.Second) < double.Epsilon * Math.Max(this.Second, other.Second) ? 0 : this.Second.CompareTo(other.Second);

    #region operators

    /// <inheritdoc/>
    public static bool operator ==(Time t1, Time t2)
        => t1.CompareTo(t2) == 0;

    /// <inheritdoc/>
    public static bool operator !=(Time t1, Time t2)
        => !(t1 == t2);

    /// <inheritdoc/>
    public static bool operator >(Time t1, Time t2)
        => t1.Second > t2.Second;

    /// <inheritdoc/>
    public static bool operator >=(Time t1, Time t2)
        => t1 > t2 || t1 == t2;

    /// <inheritdoc/>
    public static bool operator <(Time t1, Time t2)
        => !(t1 >= t2);

    /// <inheritdoc/>
    public static bool operator <=(Time t1, Time t2)
        => !(t1 > t2);

    /// <inheritdoc/>
    public static Time operator +(Time t1, Time t2)
        => new(t1.Second + t2.Second);

    /// <inheritdoc/>
    public static Time operator -(Time t1, Time t2)
        => new(t1.Second - t2.Second);

    /// <summary>
    /// Calculates the time multiplied by a constant.
    /// </summary>
    /// <param name="t">The time to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied time.</returns>
    public static Time operator *(Time t, int a)
        => new(t.Second * a);

    /// <summary>
    /// Calculates the time multiplied by a constant.
    /// </summary>
    /// <param name="t">The time to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied time.</returns>
    public static Time operator *(int a, Time t)
        => t * a;

    /// <summary>
    /// Calculates the time multiplied by a constant.
    /// </summary>
    /// <param name="t">The time to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied time.</returns>
    public static Time operator *(Time t, float a)
        => new(t.Second * a);

    /// <summary>
    /// Calculates the time multiplied by a constant.
    /// </summary>
    /// <param name="t">The time to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied time.</returns>
    public static Time operator *(float a, Time t)
        => t * a;

    /// <summary>
    /// Calculates the time multiplied by a constant.
    /// </summary>
    /// <param name="t">The time to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied time.</returns>
    public static Time operator *(Time t, double a)
        => new(t.Second * a);

    /// <summary>
    /// Calculates the time multiplied by a constant.
    /// </summary>
    /// <param name="t">The time to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied time.</returns>
    public static Time operator *(double a, Time t)
        => t * a;

    /// <summary>
    /// Calculates the ratio of two times.
    /// </summary>
    /// <param name="t1">The first value to calculates ratio.</param>
    /// <param name="t2">The second value to calculates ratio.</param>
    /// <returns>The ratio of <paramref name="t1"/> and <paramref name="t2"/>.</returns>
    public static Time operator /(Time t1, Time t2)
        => new(t1.Second / t2.Second);

    /// <summary>
    /// Calculates the time divided by a constant.
    /// </summary>
    /// <param name="t">The time to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided time.</returns>
    public static Time operator /(Time t, int a)
        => new(t.Second / a);

    /// <summary>
    /// Calculates the time divided by a constant.
    /// </summary>
    /// <param name="t">The time to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided time.</returns>
    public static Time operator /(Time t, float a)
        => new(t.Second / a);

    /// <summary>
    /// Calculates the time divided by a constant.
    /// </summary>
    /// <param name="t">The time to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided time.</returns>
    public static Time operator /(Time t, double a)
        => new(t.Second / a);

    #endregion operators
} // public readonly struct Time : IRatioScalePhysicalQuantity<Time>
