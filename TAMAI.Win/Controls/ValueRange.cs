
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Win.Controls;

internal class ValueRange<T> where T : IPhysicalQuantity<T>
{
    internal T Start { get; }

    internal T End { get; }

    internal ValueRange(T start, T end)
    {
        this.Start = start;
        this.End = end;
    } // ctor (T, T)

    override public string ToString()
        => $"{new ScientificValue<T>(this.Start)}–{new ScientificValue<T>(this.End)}";
} // internal class ValueRange
