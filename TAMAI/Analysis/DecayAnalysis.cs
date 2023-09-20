
// (c) 2023 Kazuki KOHZUKI

using TAMAI.Spectra;

namespace TAMAI.Analysis;

/// <summary>
/// Provides functions for decay analysis.
/// </summary>
public static class DecayAnalysis
{
    /// <summary>
    /// Fits the decay data with the specified function.
    /// </summary>
    /// <param name="decay">The decay to be fitted.</param>
    /// <param name="function">The model function.</param>
    /// <param name="args">Additional parameters for fitting.</param>
    /// <returns>The fitted function.</returns>
    public static IDecayFunction Fit(this Decay decay, IDecayFunctionModel function, object? args)
    {
        var n = decay.Count;
        var i0 = decay.Select((x, i) => new { Index = i, Values = x })
            .Where(x => x.Values.Key >= Time.Zero)
            .First().Index;
        var X = decay.Skip(n - i0).Select(x => x.Key.Second);
        var Y = decay.Skip(n - i0).Select(x => x.Value.Absolute.OD);

        return function.GetFunc(X, Y, args);
    } // public static IDecayFunction Fit (this Decay, IDecayFunctionModel, object?)
} // public static class DecayAnalysis
