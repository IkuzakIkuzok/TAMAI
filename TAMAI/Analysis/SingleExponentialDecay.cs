
// (c) 2023 Kazuki KOHZUKI

using TAMAI.Api;
using TAMAI.Spectra;

namespace TAMAI.Analysis;

/// <summary>
/// Represents a single exponential decay function.
/// </summary>
public sealed class SingleExponentialDecay : IExtension, IDecayFunctionModel
{
    /// <inheritdoc/>
    public string Name => "Single exponential decay";

    /// <inheritdoc/>
    public string DisplayName => "1exp";

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleExponentialDecay"/> class.
    /// </summary>
    public SingleExponentialDecay() { }

    /// <inheritdoc/>
    public IDecayFunction GetFunc(IEnumerable<double> X, IEnumerable<double> Y, object? args)
    {
        if (args is string timeRange)
        {
            var range = timeRange.Split('-');
            if (range.Length == 2)
            {
                if (!string.IsNullOrEmpty(range[0]))
                {
                    var start = new ScientificValue<Time>(range[0].Trim()).ValueAsDouble;
                    var i0 = 0;
                    X = X.SkipWhile((x, i) => { i0 = i; return x < start; }).ToArray();
                    Y = Y.Skip(i0);
                }

                if (!string.IsNullOrEmpty(range[1]))
                {
                    var end = new ScientificValue<Time>(range[1].Trim()).ValueAsDouble;
                    X = X.TakeWhile(x => x <= end);
                    Y = Y.TakeWhile((_, i) => i < X.Count());
                }
            }
        }

        var n = X.Count();
        if (n != Y.Count())
            throw new ArgumentException("The number of X and Y must be the same.");

        var lnY = Y.Select(y => Math.Log(y));
        var S_x = X.Sum();
        var S_y = lnY.Sum();
        var S_x2 = X.Select(x => x * x).Sum();
        var S_xy = X.Zip(lnY, (x, y) => x * y).Sum();

        var d = S_x * S_x - n * S_x2;
        var T_inv = (n * S_xy - S_x * S_y) / d;
        var ln_a = (S_x * S_xy - S_x2 * S_y) / d;

        var a = Math.Exp(ln_a);
        var t = 1 / T_inv;

        return new ExponentialsDecay([a], [t]);
    } // public IDecayFunction GetFunc(IEnumerable<double> X, IEnumerable<double> Y, object? args)
} // public sealed class SingleExponentialDecay : IExtension, IDecayFunctionModel
