
// (c) 2023 Kazuki KOHZUKI

using TAMAI.Win.Properties;

namespace TAMAI.Win.Controls;

[DesignerCategory("code")]
internal class RangeSelectionTable<T> : RangeSelectionTableBase where T : IPhysicalQuantity<T>
{
    private string unit = "-";

    internal string Unit
    {
        get => this.unit;
        set
        {
            this.unit = value;
            SetUnit();
        }
    }

    internal double Bias { get; set; } = 1;

    internal IEnumerable<ValueRange<T>> Ranges
        => this.Rows
            .Cast<DataGridViewRow>()
            .Where(r => r is RangeSelectionTableRow<T>)
            .Cast<RangeSelectionTableRow<T>>()
            .SkipLast(1)
            .Where(r => r.Checked)
            .Select(r => r.Range);

    internal event EventHandler? RangesUpdated;

    internal RangeSelectionTable() : base()
    {
        this.ScrollBars = ScrollBars.Both;
        this.AllowUserToAddRows = true;
        this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
        this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.RowHeadersWidth = 30;
        this.AutoGenerateColumns = false;

        DataError += (sender, e) => e.Cancel = false;
        
        var enabled = new DataGridViewCheckBoxColumn()
        {
            Name = Resources.TableHeaderShow,
            Resizable = DataGridViewTriState.False,
        };
        this.Columns.Add(enabled);  // 0

        var start = new DataGridViewTextBoxColumn()
        {
            Name = $"{Resources.TableHeaderFrom} ({this.unit})",
            ValueType = typeof(double),
        };
        this.Columns.Add(start);

        var end = new DataGridViewTextBoxColumn()
        {
            Name = $"{Resources.TableHeaderTo} ({this.unit})",
            ValueType = typeof(double),
        };
        this.Columns.Add(end);

        enabled.Width = 30;
        start.Width = end.Width = 70;

        enabled.HeaderCell.Style.Alignment =
        start.HeaderCell.Style.Alignment =
        end.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

        start.DefaultCellStyle.Alignment = 
        end.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        if (RangeSelectionTableRow<T>.TableTemplate == null)
            RangeSelectionTableRow<T>.TableTemplate = this;
        this.RowTemplate = new RangeSelectionTableRow<T>(0, 0);

        //start.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
    } // ctor ()

    private void SetUnit()
    {
        this.Columns[1].Name = $"{Resources.TableHeaderFrom} ({this.unit})";
        this.Columns[2].Name = $"{Resources.TableHeaderTo} ({this.unit})";
    } // private void SetUnit ()

    internal void Add(double start, double end)
        => this.Rows.Add(new RangeSelectionTableRow<T>(start, end));

    override protected void OnRangesUpdated(EventArgs e)
        => RangesUpdated?.Invoke(this, e);
} // internal class RangeSelectionTable<T> : RangeSelectionTableBase where T : IPhysicalQuantity<T>
