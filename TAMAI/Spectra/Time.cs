
// (c) 2023 Kazuki KOHZUKI

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TAMAI.Spectra;

/// <summary>
/// Represents a time.
/// </summary>
[DebuggerDisplay("{Second} s")]
[PhysicalQuantity(nameof(Second), "s")]
public readonly struct Time(double second) : IRatioScalePhysicalQuantity<Time>
{
    private static readonly Time zero = new();

    /// <inheritdoc/>
    public static Time Zero => zero;

    /// <summary>
    /// Gets the time value, in s.
    /// </summary>
    public double Second { get; } = second;

    /// <summary>
    /// Gets the time value, in ms.
    /// </summary>
    public double MilliSecond => this.Second * 1_000;

    /// <summary>
    /// Gets the time value, in us.
    /// </summary>
    public double MicroSecond => this.Second * 1_000_000;

    /// <summary>
    /// Gets the time value, in ns.
    /// </summary>
    public double NanoSecond => this.Second * 1_000_000_000;

    /// <summary>
    /// Gets the time value, in ps.
    /// </summary>
    public double PicoSecond => this.Second * 1_000_000_000_000;

    /// <summary>
    /// Gets the time value, in fs.
    /// </summary>
    public double FemtoSecond => this.Second * 1_000_000_000_000_000;

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
    public static Time Round(Time t, int significance, Func<double, double>? round = null)
    {
        if (t.Second == 0) return new(0);

        var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(t.Second))) - significance + 1);
        var value = (round ?? Math.Round)(t.Second / scale) * scale;
        return new(value);
    } // public static Time Round (Time, int, [Func<double, double>?])

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
    public static bool operator ==(Time left, Time right)
        => left.CompareTo(right) == 0;

    /// <inheritdoc/>
    public static bool operator !=(Time left, Time right)
        => !(left == right);

    /// <inheritdoc/>
    public static bool operator >(Time left, Time right)
        => left.Second > right.Second;

    /// <inheritdoc/>
    public static bool operator >=(Time left, Time right)
        => left > right || left == right;

    /// <inheritdoc/>
    public static bool operator <(Time left, Time right)
        => !(left >= right);

    /// <inheritdoc/>
    public static bool operator <=(Time left, Time right)
        => !(left > right);

    /// <inheritdoc/>
    public static Time operator +(Time left, Time right)
        => new(left.Second + right.Second);

    /// <inheritdoc/>
    public static Time operator -(Time left, Time right)
        => new(left.Second - right.Second);

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
