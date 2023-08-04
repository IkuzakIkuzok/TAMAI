
// (c) 2023 Kazuki KOHZUKI

using System.Numerics;

namespace TAMAI;

/// <summary>
/// Defines operators and methods that a physical quantity must implement.
/// </summary>
/// <typeparam name="TSelf">The type that implements this interface.</typeparam>
public interface IPhysicalQuantity<TSelf> :
    IComparable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>, IComparisonOperators<TSelf, TSelf, bool>,
    IAdditionOperators<TSelf, TSelf, TSelf>, ISubtractionOperators<TSelf, TSelf, TSelf>
    where TSelf : IPhysicalQuantity<TSelf>
{
    /// <summary>
    /// Creates an instance of the <typeparamref name="TSelf"/> from a value,
    /// in most common unit.
    /// </summary>
    /// <param name="value">The value to use for initialization.</param>
    /// <returns>A new instance representing <paramref name="value"/>.</returns>
    abstract static TSelf FromDouble(double value);
} // public interface IPhysicalQuantity<TSelf> : ... where TSelf : IPhysicalQuantity<TSelf>
