
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Analysis;

/// <summary>
/// Represents a decay function.
/// </summary>
public interface IDecayFunction
{
    /// <summary>
    /// Gets the time constant of this decay function.
    /// </summary>
    public double TimeConstant { get; }

    /// <summary>
    /// Gets the function.
    /// </summary>
    public Func<double, double> Func { get; }
}  // public interface IDecayFunction
