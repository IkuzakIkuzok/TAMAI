
// (c) 2023 Kazuki KOHZUKI

using System.Numerics;

namespace TAMAI;

/// <summary>
/// Defines operators and methods that a ratio-scale physical quantity must implement.
/// </summary>
/// <typeparam name="TSelf">The type that implements this interface.</typeparam>
public interface IRatioScalePhysicalQuantity<TSelf> :
    IPhysicalQuantity<TSelf>,
    IMultiplyOperators<TSelf, int, TSelf>, IMultiplyOperators<TSelf, float, TSelf>, IMultiplyOperators<TSelf, double, TSelf>,
    IDivisionOperators<TSelf, int, TSelf>, IDivisionOperators<TSelf, float, TSelf>, IDivisionOperators<TSelf, double, TSelf>
    where TSelf : IRatioScalePhysicalQuantity<TSelf>
{
    /// <summary>
    /// Gets the zero <typeparamref name="TSelf"/> value.
    /// </summary>
    public static abstract TSelf Zero { get; }

    /// <summary>
    /// Computes the absolute of a value.
    /// </summary>
    /// <param name="value">The value for which to get its absolute.</param>
    /// <returns>The absolute of <paramref name="value"/>.</returns>
    public static abstract TSelf Abs(TSelf value);

    /// <summary>
    /// Computes the natural logarithm of a value.
    /// </summary>
    /// <param name="value">The value whose natural logarithm is to be computed.</param>
    /// <returns>The natural logarithm of <paramref name="value"/>.</returns>
    public static abstract TSelf Ln(TSelf value);

    /// <summary>
    /// Computes the base-10 logarithm of a value.
    /// </summary>
    /// <param name="value">The value whose base-10 logarithm is to be computed.</param>
    /// <returns>The base-10 logarithm of <paramref name="value"/>.</returns>
    public static abstract TSelf Log(TSelf value);
} // public interface IRatioScalePhysicalQuantity<TSelf> : IPhysicalQuantity<TSelf> where TSelf : IRatioScalePhysicalQuantity<TSelf>
