
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Win.Controls;

internal sealed class RangeSelectionTableRow<T> : DataGridViewRow where T : IPhysicalQuantity<T>
{
    private double Bias => ((RangeSelectionTable<T>)this.DataGridView!).Bias;

    internal static RangeSelectionTable<T> TableTemplate { get; set; } = new();

    internal bool Checked => (bool)this.Cells[0].Value;

    internal ValueRange<T> Range
        => new(
            T.FromDouble((double)this.Cells[1].Value * this.Bias),
            T.FromDouble((double)this.Cells[2].Value * this.Bias)
           );

    private RangeSelectionTableRow() : base()
    {
        CreateCells(TableTemplate, true, null, null);
    } // private RangeSelectionTableRow() : this(0, 0)

    internal RangeSelectionTableRow(double start, double end) : base()
    {
        CreateCells(TableTemplate, true, start, end);
    } // ctor (double, double)

    override public object Clone()
        => new RangeSelectionTableRow<T>();
} // internal sealed class RangeSelectionTableRow<T> : DataGridViewRow where T : IPhysicalQuantity<T>
