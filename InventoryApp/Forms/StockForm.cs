namespace InventoryApp.Forms;

public class StockForm : Form
{
    private readonly ComboBox cmbType = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown numQty = new() { Minimum = 1, Maximum = 1000000, Value = 1 };
    private readonly TextBox txtReason = new();

    public int Change { get; private set; }
    public string Reason { get; private set; } = "";

    public StockForm(string productName)
    {
        cmbType.Items.AddRange(new object[] { "Stock In (received)", "Stock Out (sold / used)" });
        cmbType.SelectedIndex = 0;

        var fields = UI.FormLayout(
            ("Type", cmbType),
            ("Quantity", numQty),
            ("Reason", txtReason));

        UI.BuildDialog(this, $"Adjust Stock: {productName}", new Size(380, 220), fields, Save);
    }

    private void Save()
    {
        int qty = (int)numQty.Value;
        Change = cmbType.SelectedIndex == 0 ? qty : -qty;
        Reason = txtReason.Text.Trim();
        DialogResult = DialogResult.OK;
    }
}