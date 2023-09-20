
// (c) 2023 Kazuki KOHZUKI

using TAMAI.Analysis;
using TAMAI.Spectra;

namespace TAMAI.Win.Controls;

internal sealed class WavelengthRangeSelectionTableRow : DataGridViewRow
{
    internal bool Checked => (bool)this.Cells[0].Value;

    internal static WavelengthRangeSelectionTable TableTemplate { get; set; } = new();

    internal ValueRange<Wavelength> Range
        => new(
            new((double)this.Cells[1].Value),
            new((double)this.Cells[2].Value)
           );

    internal IDecayFunctionModel Function => (IDecayFunctionModel)this.Cells[3].Value;

    internal object FittingArgs => this.Cells[4].Value;

    internal string TimeConstant
    {
        get => this.Cells[5].Value?.ToString() ?? "-";
        set => this.Cells[5].Value = value;
    }

    private WavelengthRangeSelectionTableRow() : base()
    {
        CreateCells(TableTemplate, true, 0, 0, null, null, "-");
    } // private WavelengthRangeSelectionTableRow() : this(0, 0)

    internal WavelengthRangeSelectionTableRow(double start, double end) : base()
    {
        CreateCells(TableTemplate, true, start, end, null, null, "-");
    } // ctor (double, double)

    override public object Clone()
        => new WavelengthRangeSelectionTableRow();
} // internal sealed class WavelengthRangeSelectionTableRow : DataGridViewRow
