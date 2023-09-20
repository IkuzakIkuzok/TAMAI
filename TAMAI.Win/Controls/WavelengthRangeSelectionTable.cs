
// (c) 2023 Kazuki KOHZUKI

using System.Data;
using TAMAI.Analysis;
using TAMAI.Spectra;
using TAMAI.Win.Properties;

namespace TAMAI.Win.Controls;

[DesignerCategory("code")]
internal class WavelengthRangeSelectionTable : RangeSelectionTableBase
{
    internal IEnumerable<ValueRange<Wavelength>> Ranges
        => this.Rows
            .Cast<DataGridViewRow>()
            .Select(r => r as WavelengthRangeSelectionTableRow)
            .Where(r => r?.Checked ?? false)
            .Select(r => r!.Range);

    internal event EventHandler? RangesUpdated;

    internal WavelengthRangeSelectionTable(IEnumerable<KeyValuePair<Guid, IDecayFunctionModel>> functions) : base()
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
            Name = $"{Resources.TableHeaderFrom}",
            ValueType = typeof(double),
        };
        this.Columns.Add(start); // 1

        var end = new DataGridViewTextBoxColumn()
        {
            Name = $"{Resources.TableHeaderTo}",
            ValueType = typeof(double),
        };
        this.Columns.Add(end); // 2

        var funcSource = new DataTable();
        funcSource.Columns.Add("Name", typeof(string));
        funcSource.Columns.Add("Func", typeof(IDecayFunctionModel));
        foreach (var f in functions)
            funcSource.Rows.Add(f.Value.DisplayName, f.Value);

        var func = new DataGridViewComboBoxColumn()
        {
            Name = Resources.TableHeaderFunc,
            DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
            ValueType = typeof(IDecayFunctionModel),
            DisplayMember = "Name",
            ValueMember = "Func",
            DataSource = funcSource,
        };
        this.Columns.Add(func); // 3

        var args = new DataGridViewTextBoxColumn()
        {
            Name = Resources.TableHeaderFittingArgs,
        };
        this.Columns.Add(args); // 4

        var decay = new DataGridViewTextBoxColumn()
        {
            Name = Resources.TableHeaderTimeConstant,
            ReadOnly = true,
        };
        this.Columns.Add(decay); // 5

        WavelengthRangeSelectionTableRow.TableTemplate = this;
        this.RowTemplate = new WavelengthRangeSelectionTableRow(0, 0);
        this.Rows.Clear();
    } // ctor (IEnumerable<KeyValuePair<Guid, IDecayFunctionModel>>)

    internal WavelengthRangeSelectionTable() : this([]) { }

    override protected void OnCellValueChanged(DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex == 5) return;
        base.OnCellValueChanged(e);
    } // override protected void OnCellValueChanged (DataGridViewCellEventArgs)

    override protected void OnRangesUpdated(EventArgs e)
        => RangesUpdated?.Invoke(this, e);

    override protected void OnDefaultValuesNeeded(DataGridViewRowEventArgs e)
    {
        e.Row.Cells[0].Value = true;
        e.Row.Cells[1].Value = 0;
        e.Row.Cells[2].Value = 0;
        e.Row.Cells[3].Value = null;
        e.Row.Cells[4].Value = null;
        e.Row.Cells[5].Value = "-";
    } // override protected void OnDefaultValuesNeeded (DataGridViewRowEventArgs)
} // internal class WavelengthRangeSelectionTable : RangeSelectionTableBase
