using System.Windows.Forms;
using System.Drawing;

namespace InventoryApp;

public static class UI
{
    public static TableLayoutPanel FormLayout(params (string label, Control control)[] rows)
    {
        var table = new TableLayoutPanel
        {
            ColumnCount = 2,
            Padding = new Padding(15),
            BackColor = Theme.Card
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        foreach (var (label, control) in rows)
        {
            var labelControl = new Label
            {
                Text = label,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 8, 12, 8),
                Font = Theme.Bold,
                ForeColor = Theme.Text
            };
            table.Controls.Add(labelControl);

            control.Dock = DockStyle.Fill;
            control.Margin = new Padding(0, 4, 0, 4);
            control.Font = Theme.Base;
            control.ForeColor = Theme.Text;
            control.BackColor = Color.White;

            // Set border style for TextBox
            if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.Fixed3D;
            }
            // Set border style for NumericUpDown
            else if (control is NumericUpDown numericUpDown)
            {
                numericUpDown.BorderStyle = BorderStyle.Fixed3D;
            }
            // ComboBox doesn't have direct BorderStyle, it handles its own border
            else if (control is ComboBox comboBox)
            {
                comboBox.BackColor = Color.White;
            }

            table.Controls.Add(control);
        }
        return table;
    }

    public static void BuildDialog(Form form, string title, Size size, Control fields, Action onSave)
    {
        form.Text = title;
        form.ClientSize = size;
        form.StartPosition = FormStartPosition.CenterParent;
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        form.BackColor = Theme.Background;
        form.Font = Theme.Base;

        var save = Theme.MakeButton("Save", ButtonKind.Primary, (_, _) => onSave());
        var cancel = Theme.MakeButton("Cancel", ButtonKind.Secondary, (_, _) => form.DialogResult = DialogResult.Cancel);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            Padding = new Padding(10),
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = Theme.Background,
            AutoSize = false
        };
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);

        fields.Dock = DockStyle.Fill;
        form.Controls.Add(fields);
        form.Controls.Add(buttons);
        form.AcceptButton = save;
        form.CancelButton = cancel;
    }
}