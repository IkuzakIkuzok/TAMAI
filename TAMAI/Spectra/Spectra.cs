
// (c) 2023 Kazuki KOHZUKI

using TAMAI.Utils;

namespace TAMAI.Spectra;

/// <summary>
/// Represents TA spectra.
/// </summary>
public class Spectra
{
    /// <summary>
    /// Times associated with the current instance.
    /// </summary>
    protected Time[] time;

    /// <summary>
    /// Spectra associated with the current instance.
    /// </summary>
    protected Spectrum[] spectra;

    /// <summary>
    /// Gets the number of time points in the current spectra data.
    /// </summary>
    public int TimeCount => this.time.Length;

    /// <summary>
    /// Gets the number of wavelength points in the current spectra data.
    /// </summary>
    public int WavelengthCount => this.spectra[0].Wavelengths.Length;

    /// <summary>
    /// Gets the minimum time value.
    /// </summary>
    public Time TimeMin => this.time[0];
    
    /// <summary>
    /// Gets the maximum time value.
    /// </summary>
    public Time TimeMax => this.time[this.TimeCount - 1];

    /// <summary>
    /// Gets the minimum wavelength value.
    /// </summary>
    public Wavelength WavelengthMin => this.spectra[0].Wavelengths[0];

    /// <summary>
    /// Gets the maximum wavelength value.
    /// </summary>
    public Wavelength WavelengthMax => this.spectra[0].Wavelengths[this.WavelengthCount - 1];

    /// <summary>
    /// Gets a spectrum at the specified time.
    /// </summary>
    /// <param name="time">The target time.</param>
    /// <returns>A spectrum at the <paramref name="time"/>.</returns>
    public Spectrum this[Time time]
        => this.spectra[this.time.FindNearestIndex(time)];

    /// <summary>
    /// Gets a spectrum averaged over the specified time range.
    /// </summary>
    /// <param name="timeFrom">The time from which spectra are averaged.</param>
    /// <param name="timeUntil">The time until which spectra are averaged.</param>
    /// <returns>The averaged spectrum.</returns>
    /// <exception cref="ArgumentException"><paramref name="timeUntil"/> must be greater than or equal to <paramref name="timeFrom"/>.</exception>
    public Spectrum this[Time timeFrom, Time timeUntil]
    {
        get
        {
            if (timeFrom > timeUntil) throw new ArgumentException("timeUntil must be greater than or equal to timeFrom");

            if (timeFrom == timeUntil) return this[timeFrom];

            var first = this.time.FindNearestIndex(timeFrom, ValueSearchOption.EqualOrGreater);
            var last  = this.time.FindNearestIndex(timeUntil, ValueSearchOption.EqualOrLess);

            if (last < first) last = first;
            if (first == last) return this.spectra[first];

            return this.spectra.Average(first, last);
        }
    }

    /// <summary>
    /// Gets a decay at the specified wavelength.
    /// </summary>
    /// <param name="wavelength">The target wavelength.</param>
    /// <returns>A decay at the <paramref name="wavelength"/>.</returns>
    public Decay this[Wavelength wavelength]
        => new(this.time, this.spectra.Select(s => s[wavelength]));

    /// <summary>
    /// Gets a decay averaged over the specified wavelength range.
    /// </summary>
    /// <param name="wlMin">The minimum wavelength of a range in which decay is averaged.</param>
    /// <param name="wlMax">The maximum wavelength of a range in which decay is averaged.</param>
    /// <returns>The averaged decay.</returns>
    /// <exception cref="ArgumentException"><paramref name="wlMax"/> must be greater than or equal to <paramref name="wlMin"/>.</exception>
    public Decay this[Wavelength wlMin, Wavelength wlMax]
    {
        get
        {
            if (wlMin > wlMax) throw new ArgumentException("wlMax must be greater than or equal to wlMin.");

            if (wlMin == wlMax) return this[wlMin];

            return new(this.time, this.spectra.Select(s => s[wlMin, wlMax]));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Spectra"/> class.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <param name="spectra">The spectra</param>
    /// <exception cref="ArgumentException"><paramref name="time"/> and <paramref name="spectra"/> must have same length.</exception>
    public Spectra(Time[] time, Spectrum[] spectra)
    {
        if (time.Length != spectra.Length) throw new ArgumentException("time and spectra must have same length.");

        this.time = time;
        this.spectra = spectra;
    } // ctor (Time[], Spectrum[])

    /// <summary>
    /// Saves the current spectra.
    /// </summary>
    /// <param name="directory">A directory in which the data are saved.</param>
    public void Save(string directory)
    {
        Directory.CreateDirectory(directory);
        foreach ((var time, var spectrum) in this.time.Zip(this.spectra))
        {
            var filename = $"{new ScientificValue<Time>(time)}.csv";
            var fullpath = Path.Combine(directory, filename);
            spectrum.Save(fullpath);
        }
    } // public void Save (string)

    /// <summary>
    /// Loads spectra from files.
    /// </summary>
    /// <param name="directory">The directory in which the spectra files are stored.</param>
    /// <returns></returns>
    public static Spectra Load(string directory)
    {
        var data = new SortedDictionary<Time, Spectrum>();
        foreach (var file in Directory.EnumerateFiles(directory))
        {
            var time = new ScientificValue<Time>(Path.GetFileNameWithoutExtension(file)).Value;
            data.Add(time, Spectrum.Load(file));
        }
        return new(data.Keys.ToArray(), data.Values.ToArray());
    } // public static Spectra Load (string)
} // public class Spectra
