
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Analysis;

/// <summary>
/// Represents a decay function with one or more exponential functions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExponentialsDecay"/> class.
/// </remarks>
/// <param name="a">An array of amplitude coefficients.</param>
/// <param name="t">An array of time constants.</param>
public sealed class ExponentialsDecay(double[] a, double[] t) : IDecayFunction
{
    private readonly double[] A = a, T = t;

    /// <inheritdoc/>
    public double TimeConstant => this.A.Zip(T, (a, t) => a * t).Sum() / this.A.Sum();

    /// <inheritdoc/>
    public Func<double, double> Func => (x) => this.A.Zip(this.T, (a, t) => a * Math.Exp(-x / t)).Sum();
} // public sealed class ExponentialsDecay : IDecayFunction
