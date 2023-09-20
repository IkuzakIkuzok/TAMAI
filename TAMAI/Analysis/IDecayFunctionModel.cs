
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Analysis;

/// <summary>
/// Represents a model function of decay curve.
/// </summary>
public interface IDecayFunctionModel
{
    /// <summary>
    /// Gets the name of the function.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the fitted function from the decay data.
    /// </summary>
    /// <param name="X">The x values.</param>
    /// <param name="Y">The y values.</param>
    /// <param name="args">Additional parameters for fitting.</param>
    /// <returns>The fitted function.</returns>
    public abstract IDecayFunction GetFunc(IEnumerable<double> X, IEnumerable<double> Y, object? args);
} // public interface IDecayFunctionModel
