using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace InventoryApp;

public enum ButtonKind
{
    Primary,
    Secondary,
    Danger
}

public static class Theme
{
    private static Color Hex(string hex)
    {
        return ColorTranslator.FromHtml(hex);
    }

    // ---------- Colors ----------
    public static readonly Color Sidebar = Hex("#0F172A");
    public static readonly Color SidebarHover = Hex("#1E293B");
    public static readonly Color SidebarText = Hex("#CBD5E1");

    public static readonly Color Accent = Hex("#2563EB");
    public static readonly Color AccentDark = Hex("#1D4ED8");

    public static readonly Color Background = Hex("#F1F5F9");
    public static readonly Color Card = Color.White;
    public static readonly Color Border = Hex("#E2E8F0");

    public static readonly Color Text = Hex("#0F172A");
    public static readonly Color Muted = Hex("#64748B");

    public static readonly Color Danger = Hex("#DC2626");
    public static readonly Color DangerSoft = Hex("#FEF2F2");

    public static readonly Color Success = Hex("#16A34A");
    public static readonly Color Warning = Hex("#F59E0B");

    // ---------- Fonts ----------
    public static readonly Font Base =
        new Font("Segoe UI", 10F);

    public static readonly Font Bold =
        new Font("Segoe UI", 10F, FontStyle.Bold);

    public static readonly Font Title =
        new Font("Segoe UI", 18F, FontStyle.Bold);

    public static readonly Font Big =
        new Font("Segoe UI", 20F, FontStyle.Bold);

    // ---------- Buttons ----------
    public static Button MakeButton(
        string text,
        ButtonKind kind,
        EventHandler? onClick)
    {
        Button button = new Button
        {
            Text = text,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(88, 36),
            Padding = new Padding(10, 0, 10, 0),
            FlatStyle = FlatStyle.Flat,
            Font = Bold,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 8, 0),
            UseVisualStyleBackColor = false
        };

        switch (kind)
        {
            case ButtonKind.Primary:
                button.BackColor = Accent;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseOverBackColor = AccentDark;
                button.FlatAppearance.MouseDownBackColor =
                    Hex("#1E40AF");
                break;

            case ButtonKind.Danger:
                button.BackColor = Danger;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseOverBackColor =
                    Hex("#B91C1C");
                button.FlatAppearance.MouseDownBackColor =
                    Hex("#991B1B");
                break;

            case ButtonKind.Secondary:
            default:
                button.BackColor = Color.White;
                button.ForeColor = Text;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.BorderColor = Border;
                button.FlatAppearance.MouseOverBackColor =
                    Hex("#F1F5F9");
                button.FlatAppearance.MouseDownBackColor =
                    Hex("#E2E8F0");
                break;
        }

        if (onClick != null)
        {
            button.Click += onClick;
        }

        return button;
    }

    // ---------- Cards ----------
    public static void AddBorder(Panel panel)
    {
        panel.Paint += (_, e) =>
        {
            using Pen pen = new Pen(Border);

            e.Graphics.DrawRectangle(
                pen,
                0,
                0,
                panel.Width - 1,
                panel.Height - 1);
        };

        panel.Resize += (_, _) =>
        {
            panel.Invalidate();
        };
    }

    // White bordered card around a control,
    // with an optional title
    public static Panel WrapCard(
        Control inner,
        string? title = null)
    {
        Panel card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Card,
            Padding = new Padding(1)
        };

        AddBorder(card);

        if (title != null)
        {
            Label titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 46,
                Font = Bold,
                ForeColor = Text,
                BackColor = Card,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0)
            };

            // Add title first because DockStyle.Top
            // should be processed before the Fill control.
            card.Controls.Add(titleLabel);
        }

        inner.Dock = DockStyle.Fill;
        card.Controls.Add(inner);

        return card;
    }

    // ---------- Grid ----------
    public static DataGridView NewGrid()
    {
        DataGridView grid = new DataGridView
        {
            Dock = DockStyle.Fill,

            ReadOnly = true,

            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,

            MultiSelect = false,
            RowHeadersVisible = false,

            SelectionMode =
                DataGridViewSelectionMode.FullRowSelect,

            AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill,

            BorderStyle = BorderStyle.None,
            BackgroundColor = Card,
            GridColor = Border,

            CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal,

            EnableHeadersVisualStyles = false,

            ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.None,

            ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing,

            ColumnHeadersHeight = 42,

            Font = Base
        };

        grid.RowTemplate.Height = 40;

        // ---------- Header Style ----------
        DataGridViewCellStyle header =
            grid.ColumnHeadersDefaultCellStyle;

        header.BackColor = Hex("#F8FAFC");
        header.ForeColor = Muted;
        header.Font = Bold;

        header.SelectionBackColor = header.BackColor;
        header.SelectionForeColor = header.ForeColor;

        header.Padding = new Padding(10, 0, 10, 0);

        // ---------- Cell Style ----------
        DataGridViewCellStyle cells =
            grid.DefaultCellStyle;

        cells.BackColor = Card;
        cells.ForeColor = Text;

        cells.SelectionBackColor = Hex("#DBEAFE");
        cells.SelectionForeColor = Text;

        cells.Padding = new Padding(10, 0, 10, 0);

        // ---------- Data Binding ----------
        grid.DataBindingComplete += (_, _) =>
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                // Convert ProductId -> Product Id
                // Convert CreatedAt -> Created At
                column.HeaderText = Regex.Replace(
                    column.Name,
                    "(?<=[a-z])(?=[A-Z])",
                    " ");

                // Hide internal IDs
                if (column.Name is "ProductId" or "SupplierId")
                {
                    column.Visible = false;
                }

                bool isNumber =
                    column.ValueType == typeof(int) ||
                    column.ValueType == typeof(long) ||
                    column.ValueType == typeof(decimal);

                if (isNumber)
                {
                    column.DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;

                    column.HeaderCell.Style.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                }

                if (column.ValueType == typeof(decimal))
                {
                    column.DefaultCellStyle.Format = "N2";
                }
            }
        };

        // ---------- Cell Formatting ----------
        grid.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            string columnName =
                grid.Columns[e.ColumnIndex].Name;

            // Format movement date
            if (columnName == "MovedAt" &&
                e.Value is DateTime dateTime)
            {
                e.Value = dateTime.ToString(
                    "dd MMM yyyy, HH:mm");

                e.FormattingApplied = true;
            }

            // Format stock changes
            else if (columnName == "ChangeQty" &&
                     e.Value is int quantity)
            {
                e.Value = quantity > 0
                    ? $"+{quantity}"
                    : quantity.ToString();

                e.CellStyle!.ForeColor =
                    quantity > 0
                        ? Success
                        : Danger;

                e.CellStyle.Font = Bold;

                e.FormattingApplied = true;
            }
        };

        return grid;
    }
}