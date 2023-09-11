
// (c) 2023 Kazuki KOHZUKI

using System.Collections;
using TimeSignal = System.Collections.Generic.KeyValuePair<TAMAI.Spectra.Time, TAMAI.Spectra.Signal>;

namespace TAMAI.Spectra;

/// <summary>
/// Represents a signal decay.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Decay"/> class.
/// </remarks>
/// <param name="time">The time.</param>
/// <param name="signals">The signals.</param>
public class Decay(IEnumerable<Time> time, IEnumerable<Signal> signals) : IEnumerable<TimeSignal>
{
    /// <summary>
    /// Times associated with the current instance. This field is readonly.
    /// </summary>
    protected readonly Time[] time = time.ToArray();

    /// <summary>
    /// Signals associated with the current instance. This field is readonly.
    /// </summary>
    protected readonly Signal[] signals = signals.ToArray();

    /// <summary>
    /// Gets the data count in the current decay data.
    /// </summary>
    public int Count => this.time.Length;

    /// <summary>
    /// Gets the normalized decay.
    /// </summary>
    public Decay Normalized
    {
        get
        {
            var max = this.signals.Max();
            var min = this.signals.Min();
            var norm = Math.Max((double)max.Absolute, (double)min.Absolute);
            return this / norm;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<TimeSignal> GetEnumerator()
        => this.time.Zip(this.signals, (t, s) => new TimeSignal(t, s)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    #region operators

    /// <summary>
    /// Calculates the decay multiplied by a constant.
    /// </summary>
    /// <param name="d">The decay to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied decay.</returns>
    public static Decay operator *(Decay d, int a)
        => new(d.time, d.signals.Select(d => d * a));

    /// <summary>
    /// Calculates the decay multiplied by a constant.
    /// </summary>
    /// <param name="d">The decay to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied decay.</returns>
    public static Decay operator *(int a, Decay d)
        => d * a;

    /// <summary>
    /// Calculates the decay multiplied by a constant.
    /// </summary>
    /// <param name="d">The decay to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied decay.</returns>
    public static Decay operator *(Decay d, float a)
        => new(d.time, d.signals.Select(d => d * a));

    /// <summary>
    /// Calculates the decay multiplied by a constant.
    /// </summary>
    /// <param name="d">The decay to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied decay.</returns>
    public static Decay operator *(float a, Decay d)
        => d * a;

    /// <summary>
    /// Calculates the decay multiplied by a constant.
    /// </summary>
    /// <param name="d">The decay to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied decay.</returns>
    public static Decay operator *(Decay d, double a)
        => new(d.time, d.signals.Select(d => d * a));

    /// <summary>
    /// Calculates the decay multiplied by a constant.
    /// </summary>
    /// <param name="d">The decay to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied decay.</returns>
    public static Decay operator *(double a, Decay d)
        => d * a;

    /// <summary>
    /// Calculates the decay divied by a constant.
    /// </summary>
    /// <param name="d">The decay to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided decay.</returns>
    public static Decay operator /(Decay d, int a)
        => new(d.time, d.signals.Select(d => d / a));

    /// <summary>
    /// Calculates the decay divied by a constant.
    /// </summary>
    /// <param name="d">The decay to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided decay.</returns>
    public static Decay operator /(Decay d, float a)
        => new(d.time, d.signals.Select(d => d / a));

    /// <summary>
    /// Calculates the decay divied by a constant.
    /// </summary>
    /// <param name="d">The decay to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided decay.</returns>
    public static Decay operator /(Decay d, double a)
        => new(d.time, d.signals.Select(d => d / a));

    #endregion operators
} // public class Decay : IEnumerable<TimeSignal>
