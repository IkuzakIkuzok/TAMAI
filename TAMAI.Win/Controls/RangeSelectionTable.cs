
// (c) 2023 Kazuki KOHZUKI

using TAMAI.Win.Properties;

namespace TAMAI.Win.Controls;

[DesignerCategory("code")]
internal class RangeSelectionTable<T> : DataGridView where T : IPhysicalQuantity<T>
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

    private DataGridViewColumn SortColumn
    {
        get
        {
            foreach (var column in this.Columns.Cast<DataGridViewColumn>())
            {
                var direction = column.HeaderCell.SortGlyphDirection;
                if (direction != SortOrder.None)
                    return column;
            }
            return this.Columns[1];
        }
    }

    private ListSortDirection SortDirection
        => this.SortColumn.HeaderCell.SortGlyphDirection switch
        {
            SortOrder.Descending => ListSortDirection.Descending,
            _ => ListSortDirection.Ascending,
        };

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

    override protected void OnCellValueChanged(DataGridViewCellEventArgs e)
    {
        base.OnCellValueChanged(e);

        if (e.RowIndex < 0) return;
        if (e.RowIndex == this.Rows.Count - 1) return;

        if (e.ColumnIndex == 0)
        {
            OnRangesUpdated(EventArgs.Empty);
            return;
        }

        var row = (RangeSelectionTableRow<T>)this.Rows[e.RowIndex];
        if (row.Cells[1].Value is not double min) return;
        if (row.Cells[2].Value is not double max) return;

        if (min > max)
        {
            row.Cells[1].Value = max;
            row.Cells[2].Value = min;
        }

        Sort(this.SortColumn, this.SortDirection);
        OnRangesUpdated(EventArgs.Empty);
    } // override protected void OnCellValueChanged (DataGridViewCellEventArgs)

    override protected void OnSortCompare(DataGridViewSortCompareEventArgs e)
    {
        e.Handled = true;

        if (e.Column.DisplayIndex == 0)
        {
            var e1 = (bool)e.CellValue1;
            var e2 = (bool)e.CellValue2;
            if (e1 != e2)
            {
                e.SortResult = e1.CompareTo(e2);
                return;
            }
        }

        if (e.CellValue1 is not double v1 || e.CellValue2 is not double v2)
        {
            e.Handled = false;
            return;
        }

        if (v1 != v2)
        {
            e.SortResult = v1.CompareTo(v2);
            return;
        }

        if (e.Column.DisplayIndex == 1)
        {
            var row1 = this.Rows[e.RowIndex1];
            var row2 = this.Rows[e.RowIndex2];

            if (row1.Cells[2].Value is not double end1 || row2.Cells[2].Value is not double end2)
            {
                e.Handled = false;
                return;
            }

            e.SortResult = end1.CompareTo(end2);
            return;
        }

        e.Handled = false;
    } // override protected void OnSortCompare (DataGridViewSortCompareEventArgs)

    private void SetUnit()
    {
        this.Columns[1].Name = $"{Resources.TableHeaderFrom} ({this.unit})";
        this.Columns[2].Name = $"{Resources.TableHeaderTo} ({this.unit})";
    } // private void SetUnit ()

    internal void Add(double start, double end)
        => this.Rows.Add(new RangeSelectionTableRow<T>(start, end));

    virtual protected void OnRangesUpdated(EventArgs e)
        => RangesUpdated?.Invoke(this, e);
} // internal class RangeSelectionTable<T> : DataGridView where T : IPhysicalQuantity<T>
