using InventoryApp.Models;

namespace InventoryApp.Forms;

public class SupplierForm : Form
{
    private readonly TextBox txtName = new();
    private readonly TextBox txtContact = new();
    private readonly TextBox txtPhone = new();
    private readonly TextBox txtEmail = new();
    private readonly TextBox txtAddress = new();
    private readonly int _supplierId;

    public Supplier Result { get; private set; } = new();

    public SupplierForm(Supplier? existing = null)
    {
        if (existing != null)
        {
            _supplierId = existing.SupplierId;
            txtName.Text = existing.Name;
            txtContact.Text = existing.ContactPerson;
            txtPhone.Text = existing.Phone;
            txtEmail.Text = existing.Email;
            txtAddress.Text = existing.Address;
        }

        var fields = UI.FormLayout(
            ("Name", txtName),
            ("Contact person", txtContact),
            ("Phone", txtPhone),
            ("Email", txtEmail),
            ("Address", txtAddress));

        UI.BuildDialog(this, existing == null ? "Add Supplier" : "Edit Supplier",
            new Size(400, 340), fields, Save);
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Supplier name is required.", "Missing data",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Result = new Supplier
        {
            SupplierId = _supplierId,
            Name = txtName.Text.Trim(),
            ContactPerson = txtContact.Text.Trim(),
            Phone = txtPhone.Text.Trim(),
            Email = txtEmail.Text.Trim(),
            Address = txtAddress.Text.Trim()
        };
        DialogResult = DialogResult.OK;
    }
}