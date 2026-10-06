using InventoryApp.Data;
using InventoryApp.Models;

namespace InventoryApp.Forms;

public class ProductForm : Form
{
    private readonly TextBox txtName = new();
    private readonly TextBox txtSku = new();
    private readonly TextBox txtCategory = new();
    private readonly NumericUpDown numPrice = new() { DecimalPlaces = 2, Maximum = 10000000, ThousandsSeparator = true };
    private readonly NumericUpDown numReorder = new() { Maximum = 100000, Value = 5 };
    private readonly ComboBox cmbSupplier = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly int _productId;

    public Product Result { get; private set; } = new();

    public ProductForm(Product? existing = null)
    {
        var suppliers = SupplierRepository.GetList();
        suppliers.Insert(0, new Supplier { SupplierId = 0, Name = "(None)" });
        cmbSupplier.DataSource = suppliers;
        cmbSupplier.DisplayMember = "Name";
        cmbSupplier.ValueMember = "SupplierId";

        if (existing != null)
        {
            _productId = existing.ProductId;
            txtName.Text = existing.Name;
            txtSku.Text = existing.Sku;
            txtCategory.Text = existing.Category;
            numPrice.Value = existing.Price;
            numReorder.Value = existing.ReorderLevel;
            cmbSupplier.SelectedValue = existing.SupplierID ?? 0;
        }

        var fields = UI.FormLayout(
            ("Name", txtName),
            ("SKU", txtSku),
            ("Category", txtCategory),
            ("Price", numPrice),
            ("Reorder level", numReorder),
            ("Supplier", cmbSupplier));

        UI.BuildDialog(this, existing == null ? "Add Product" : "Edit Product",
            new Size(400, 340), fields, Save);
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtSku.Text))
        {
            MessageBox.Show("Name and SKU are required.", "Missing data",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int supplierId = (int)cmbSupplier.SelectedValue!;
        Result = new Product
        {
            ProductId = _productId,
            Name = txtName.Text.Trim(),
            Sku = txtSku.Text.Trim(),
            Category = txtCategory.Text.Trim(),
            Price = numPrice.Value,
            ReorderLevel = (int)numReorder.Value,
            SupplierID = supplierId == 0 ? null : supplierId
        };
        DialogResult = DialogResult.OK;
    }
}