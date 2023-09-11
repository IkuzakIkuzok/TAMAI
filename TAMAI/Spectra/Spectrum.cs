
// (c) 2023 Kazuki KOHZUKI

using System.Collections;
using System.Numerics;
using System.Text;
using TAMAI.Utils;
using WlSignal = System.Collections.Generic.KeyValuePair<TAMAI.Spectra.Wavelength, TAMAI.Spectra.Signal>;

namespace TAMAI.Spectra;

/// <summary>
/// Represents a spectrum at a specific time or in a specific time range.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Spectrum"/> class.
/// </remarks>
/// <param name="wavelengths">The wavelength.</param>
/// <param name="signals">The signal.</param>
public class Spectrum(IEnumerable<Wavelength> wavelengths, IEnumerable<Signal> signals) :
    IEnumerable<WlSignal>, IAdditionOperators<Spectrum, Spectrum, Spectrum>,
    IMultiplyOperators<Spectrum, int, Spectrum>, IMultiplyOperators<Spectrum, float, Spectrum>, IMultiplyOperators<Spectrum, double, Spectrum>,
    IDivisionOperators<Spectrum, int, Spectrum>, IDivisionOperators<Spectrum, float, Spectrum>, IDivisionOperators<Spectrum, double, Spectrum>
{
    /// <summary>
    /// Wavelengths associated with the current instance. This field is readonly.
    /// </summary>
    protected readonly Wavelength[] wavelengths = wavelengths.ToArray();

    /// <summary>
    /// Signals associated with the current instance. This field is readonly.
    /// </summary>
    protected readonly Signal[] signals = signals.ToArray();

    /// <summary>
    /// Gets the list of wavelength associated with the current spectrum.
    /// </summary>
    public ReadOnlySpan<Wavelength> Wavelengths => this.wavelengths;

    /// <summary>
    /// Gets the list of signals correspongind to <see cref="Wavelengths"/>.
    /// </summary>
    public ReadOnlySpan<Signal> Signals => this.signals;

    /// <summary>
    /// Gets a signal at the specified wavelength.
    /// </summary>
    /// <param name="wavelength">The target wavelength.</param>
    /// <returns>A signal at the <paramref name="wavelength"/>.</returns>
    public Signal this[Wavelength wavelength]
        => this.signals[this.wavelengths.FindNearestIndex(wavelength)];

    /// <summary>
    /// Gets a signal averaged over the specified wavelength range.
    /// </summary>
    /// <param name="wlMin">The wavelength from which signal are averaged.</param>
    /// <param name="wlMax">The wavelength to which signal are averaged.</param>
    /// <returns>The averaged signal.</returns>
    /// <exception cref="ArgumentException"><paramref name="wlMax"/> must be greater than or equal to <paramref name="wlMin"/>.</exception>
    public Signal this[Wavelength wlMin, Wavelength wlMax]
    {
        get
        {
            if (wlMin > wlMax) throw new ArgumentException("wlMax must be greater than or equal to wlMin.");

            if (wlMin == wlMax) return this[wlMin];

            var first = this.wavelengths.FindNearestIndex(wlMin, ValueSearchOption.EqualOrGreater);
            var last = this.wavelengths.FindNearestIndex(wlMax, ValueSearchOption.EqualOrLess);

            if (first == last) return this[wlMin];

            return this.signals.Average(first, last);
        }
    }

    /// <summary>
    /// Gets a normalized spectrum.
    /// </summary>
    public Spectrum Normalized
    {
        get
        {
            var max = this.signals.Max();
            var min = this.signals.Min();
            var norm = Math.Max((double)max.Absolute, (double)min.Absolute);
            return this / norm;
        }
    }

    /// <summary>
    /// Gets a absolute spectrum.
    /// </summary>
    public Spectrum Absolute
        => new(this.wavelengths, this.signals.Select(s => s.Absolute));

    /// <summary>
    /// Saves the spectrum to a file.
    /// </summary>
    /// <param name="filename">The filename.</param>
    public void Save(string filename)
    {
        using var sw = new StreamWriter(filename, false, Encoding.UTF8);
        foreach ((var wl, var s) in this.wavelengths.Zip(this.signals))
        {
            var l = $"{(double)wl},{s:e}";
            sw.WriteLine(l);
        }
    } // public void Save (string)

    /// <summary>
    /// Loads a spectrum from a file.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <returns>A loaded <see cref="Spectrum"/> instance.</returns>
    public static Spectrum Load(string filename)
    {
        using var sr = new StreamReader(filename, Encoding.UTF8);
        var data = new SortedList<Wavelength, Signal>();
        string? l;
        while ((l = sr.ReadLine()) != null)
        {
            var v = l.Split(',').Select(double.Parse).ToArray();
            data.Add(new(v[0]), new(v[1]));
        }
        return new(data.Keys, data.Values);
    } // public static Spectrum Load (string)

    /// <inheritdoc/>
    public IEnumerator<WlSignal> GetEnumerator()
        => this.wavelengths.Zip(this.signals, (wl, s) => new WlSignal(wl, s)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    #region operators

    /// <inheritdoc/>
    public static Spectrum operator +(Spectrum s1, Spectrum s2)
    {
        if (!CheckWavelengthRange(s1, s2)) throw new InvalidOperationException("Cannon add spectra with different wavelength range.");

        return new(s1.wavelengths, s1.signals.Zip(s2.signals, (s1, s2) => s1 + s2));
    } // public static Spectrum operator + (Spectrum, Spectrum)

    /// <inheritdoc/>
    public static Spectrum operator -(Spectrum s1, Spectrum s2)
        => s1 + (-s2);

    /// <inheritdoc/>
    public static Spectrum operator -(Spectrum s)
        => -1 * s;

    /// <summary>
    /// Calculates the spectrum multiplied by a constant.
    /// </summary>
    /// <param name="s">The spectrum to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied spectrum.</returns>
    public static Spectrum operator *(Spectrum s, int a)
        => new(s.wavelengths, s.signals.Select(s => s * a));

    /// <summary>
    /// Calculates the spectrum multiplied by a constant.
    /// </summary>
    /// <param name="s">The spectrum to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied spectrum.</returns>
    public static Spectrum operator *(int a, Spectrum s)
        => s * a;

    /// <summary>
    /// Calculates the spectrum multiplied by a constant.
    /// </summary>
    /// <param name="s">The spectrum to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied spectrum.</returns>
    public static Spectrum operator *(Spectrum s, float a)
        => new(s.wavelengths, s.signals.Select(s => s * a));

    /// <summary>
    /// Calculates the spectrum multiplied by a constant.
    /// </summary>
    /// <param name="s">The spectrum to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied spectrum.</returns>
    public static Spectrum operator *(float a, Spectrum s)
        => s * a;

    /// <summary>
    /// Calculates the spectrum multiplied by a constant.
    /// </summary>
    /// <param name="s">The spectrum to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied spectrum.</returns>
    public static Spectrum operator *(Spectrum s, double a)
        => new(s.wavelengths, s.signals.Select(s => s * a));

    /// <summary>
    /// Calculates the spectrum multiplied by a constant.
    /// </summary>
    /// <param name="s">The spectrum to be multiplied.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant multiplied spectrum.</returns>
    public static Spectrum operator *(double a, Spectrum s)
        => s * a;

    /// <summary>
    /// Calculates the spectrum divided by a constant.
    /// </summary>
    /// <param name="s">The spectrum to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided spectrum.</returns>
    public static Spectrum operator /(Spectrum s, int a)
        => new(s.wavelengths, s.signals.Select(s => s / a));

    /// <summary>
    /// Calculates the spectrum divided by a constant.
    /// </summary>
    /// <param name="s">The spectrum to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided spectrum.</returns>
    public static Spectrum operator /(Spectrum s, float a)
        => new(s.wavelengths, s.signals.Select(s => s / a));

    /// <summary>
    /// Calculates the spectrum divided by a constant.
    /// </summary>
    /// <param name="s">The spectrum to be divided.</param>
    /// <param name="a">A constant value.</param>
    /// <returns>The constant divided spectrum.</returns>
    public static Spectrum operator /(Spectrum s, double a)
        => new(s.wavelengths, s.signals.Select(s => s / a));

    #endregion operators

    private static bool CheckWavelengthRange(Spectrum s1, Spectrum s2)
    {
        var w1 = s1.wavelengths;
        var w2 = s2.wavelengths;

        if (w1.Length != w2.Length) return false;

        for (var i = 0; i < w1.Length; i++)
        {
            if (w1[i] != w2[i]) return false;
        }

        return true;
    } // private static bool CheckWavelengthRange (Spectrum, Spectrum)
} // public class Spectrum : ...
