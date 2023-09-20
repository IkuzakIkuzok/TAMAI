
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Win.Controls;

[DesignerCategory("code")]
internal abstract class RangeSelectionTableBase : DataGridView
{
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

        var row = this.Rows[e.RowIndex];
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

    abstract protected void OnRangesUpdated(EventArgs e);
} // internal abstract class RangeSelectionTableBase : DataGridView
