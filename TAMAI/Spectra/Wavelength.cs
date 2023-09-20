
// (c) 2023 Kazuki KOHZUKI

using System.Diagnostics.CodeAnalysis;

namespace TAMAI.Spectra;

/// <summary>
/// Represents a wavelength, in nm.
/// </summary>
[PhysicalQuantity(nameof(Value), "nm", AcceptSIPrefix = false, DefaultValue = 532)]
public readonly struct Wavelength(double value) : IPhysicalQuantity<Wavelength>
{
    private const double WLtoEnergy = 1239.841984;

    /// <summary>
    /// Gets the wavelengths value, in nm.
    /// </summary>
    public double Value { get; } = value;

    /// <summary>
    /// Gets the photon energy corresponding to the wavelengths.
    /// </summary>
    public double Energy => WLtoEnergy / this.Value;

    /// <inheritdoc/>
    public static Wavelength FromDouble(double wl) => new(wl);

    /// <summary>
    /// Creates a new instance of the <see cref="Wavelength"/> structure from energy in eV.
    /// </summary>
    /// <param name="energy">The energy value, in eV.</param>
    /// <returns>A new instance of the <paramref name="energy"/> whose corresponding energy is equal to <paramref name="energy"/>.</returns>
    public static Wavelength FromEnergy(double energy)
        => new(WLtoEnergy / energy);

    /// <summary>
    /// Casts an instance of <see cref="double"/> into its corresponding <see cref="Wavelength"/>.
    /// </summary>
    /// <param name="wavelength">The value.</param>
    public static explicit operator double(Wavelength wavelength)
        => wavelength.Value;

    /// <summary>
    /// Casts an instance of <see cref="Wavelength"/> into its corresponding <see cref="double"/>.
    /// </summary>
    /// <param name="value">The value, in nm.</param>
    public static explicit operator Wavelength(double value)
        => new(value);

    /// <inheritdoc/>
    public static Wavelength Round(Wavelength wl, int significance, Func<double, double>? round = null)
    {
        if (wl.Value == 0) return new(0);

        var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(wl.Value))) - significance + 1);
        var value = (round ?? Math.Round)(wl.Value / scale) * scale;
        return new(value);
    } // Round (Wavelength, int, Func<double, double>?

    /// <inheritdoc/>
    public int CompareTo(Wavelength other)
        => Math.Abs(this.Value - other.Value) < double.Epsilon * Math.Max(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);

    /// <summary>
    /// Converts the wavelength of this instance to its equivalent string representation.
    /// </summary>
    /// <returns>The string representation of the value of this instance.</returns>
    public override string ToString()
        => $"{this.Value} nm";

    /// <inheritdoc/>
    override public bool Equals([NotNullWhen(true)] object? obj)
        => obj is Wavelength wl && this == wl;

    /// <inheritdoc/>
    override public int GetHashCode()
        => this.Value.GetHashCode();

    #region operators

    /// <inheritdoc/>
    public static bool operator ==(Wavelength wl1, Wavelength wl2)
        => wl1.CompareTo(wl2) == 0;

    /// <inheritdoc/>
    public static bool operator !=(Wavelength wl1, Wavelength wl2)
        => !(wl1 == wl2);

    /// <inheritdoc/>
    public static bool operator >(Wavelength wl1, Wavelength wl2)
        => wl1.Value > wl2.Value;

    /// <inheritdoc/>
    public static bool operator >=(Wavelength wl1, Wavelength wl2)
        => wl1 > wl2 || wl1 == wl2;

    /// <inheritdoc/>
    public static bool operator <(Wavelength wl1, Wavelength wl2)
        => !(wl1 >= wl2);

    /// <inheritdoc/>
    public static bool operator <=(Wavelength wl1, Wavelength wl2)
        => !(wl1 > wl2);

    /// <inheritdoc/>
    public static Wavelength operator +(Wavelength wl1, Wavelength wl2)
        => new(wl1.Value + wl2.Value);

    /// <inheritdoc/>
    public static Wavelength operator -(Wavelength wl1, Wavelength wl2)
        => new(wl1.Value - wl2.Value);

    #endregion operators
} // public readonly struct Wavelength : IPhysicalQuantity<Wavelength>
