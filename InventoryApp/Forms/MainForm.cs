using InventoryApp;
using InventoryApp.Data;
using InventoryApp.Utils;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace InventoryApp.Forms;

public class MainForm : Form
{
    private readonly DataGridView gridProducts = Theme.NewGrid();
    private readonly DataGridView gridSuppliers = Theme.NewGrid();
    private readonly DataGridView gridReport = Theme.NewGrid();
    private readonly TextBox txtSearch = new()
    {
        PlaceholderText = "Search product name, SKU or category...",
        Width = 320,
        Font = Theme.Base,
        BackColor = Color.White,
        BorderStyle = BorderStyle.FixedSingle,
        Margin = new Padding(0, 0, 15, 0)
    };

    private Label? lblBranch;
    private Label? lblProfile;
    private Label? lblTotalItems;
    private Label? lblLowStockAlerts;
    private Label? lblConnected;
    private Button? btnProducts;
    private Button? btnSuppliers;
    private Button? btnReports;
    private TabControl? mainTabs;

    public MainForm()
    {
        Text = "Inventory Management System";
        Size = new Size(1600, 900);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.Background;
        Font = Theme.Base;
        Padding = new Padding(0);

        var mainContainer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        mainContainer.Controls.Add(BuildSidebar(), 0, 0);
        mainContainer.Controls.Add(BuildContentArea(), 1, 0);

        Controls.Add(mainContainer);

        gridProducts.DataBindingComplete += (_, _) => HighlightLowStock();
        Load += (_, _) => Run(() =>
        {
            RefreshProducts();
            RefreshSuppliers();
            RefreshStats();
        });
    }

    private Panel BuildSidebar()
    {
        var sidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 280,
            BackColor = Color.FromArgb(245, 245, 247),
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        var sidebarLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        sidebarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        sidebarLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        sidebarLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        sidebarLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // Header
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = Color.White,
            Padding = new Padding(16, 12, 16, 12),
            Margin = new Padding(0)
        };

        var headerLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = false,
            WrapContents = false,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        var appTitle = new Label
        {
            Text = "📦 Inventory",
            Font = new Font(Theme.Bold.FontFamily, 14, FontStyle.Bold),
            ForeColor = Theme.Text,
            AutoSize = true
        };

        var appSubtitle = new Label
        {
            Text = "WinUI 3 Modern Edition",
            Font = Theme.Base,
            ForeColor = Theme.Muted,
            AutoSize = true
        };

        headerLayout.Controls.Add(appTitle);
        headerLayout.Controls.Add(appSubtitle);
        header.Controls.Add(headerLayout);

        // Navigation buttons
        var navPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 250,
            BackColor = Color.Transparent,
            Padding = new Padding(8, 16, 8, 16),
            Margin = new Padding(0)
        };

        var navLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        btnProducts = CreateNavButton("📁 Products", true);
        btnProducts.Click += (_, _) => SelectTab(0);

        btnSuppliers = CreateNavButton("🏢 Suppliers", false);
        btnSuppliers.Click += (_, _) => SelectTab(1);

        btnReports = CreateNavButton("📊 Reports", false);
        btnReports.Click += (_, _) => SelectTab(2);

        navLayout.Controls.Add(btnProducts);
        navLayout.Controls.Add(btnSuppliers);
        navLayout.Controls.Add(btnReports);

        navPanel.Controls.Add(navLayout);

        // Footer
        var footer = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 100,
            BackColor = Color.White,
            Padding = new Padding(16, 12, 16, 12),
            Margin = new Padding(0)
        };

        var footerLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = false,
            WrapContents = false,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        var dbLabel = new Label { Text = "Database", Font = Theme.Base, ForeColor = Theme.Text, AutoSize = true };
        var dbValue = new Label { Text = "SQLite (Local)", Font = Theme.Base, ForeColor = Theme.Muted, AutoSize = true };
        var statusLabel = new Label { Text = "Status", Font = Theme.Base, ForeColor = Theme.Text, AutoSize = true };

        lblConnected = new Label
        {
            Text = "● Connected",
            Font = Theme.Base,
            ForeColor = Theme.Success,
            AutoSize = true
        };

        footerLayout.Controls.Add(dbLabel);
        footerLayout.Controls.Add(dbValue);
        footerLayout.Controls.Add(statusLabel);
        footerLayout.Controls.Add(lblConnected);
        footer.Controls.Add(footerLayout);

        sidebarLayout.Controls.Add(header, 0, 0);
        sidebarLayout.Controls.Add(navPanel, 0, 1);
        sidebarLayout.Controls.Add(footer, 0, 2);

        sidebar.Controls.Add(sidebarLayout);
        return sidebar;
    }

    private Button CreateNavButton(string text, bool isSelected)
    {
        var btn = new Button
        {
            Text = text,
            Dock = DockStyle.Top,
            Height = 45,
            BackColor = isSelected ? Color.FromArgb(230, 240, 255) : Color.Transparent,
            ForeColor = isSelected ? Theme.Accent : Theme.Text,
            FlatStyle = FlatStyle.Flat,
            Font = new Font(Theme.Base.FontFamily, 11),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(12, 0, 0, 0),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 4, 0, 4)
        };

        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 247);

        return btn;
    }

    private void SelectTab(int index)
    {
        if (mainTabs == null) return;
        mainTabs.SelectedIndex = index;

        if (btnProducts != null)
        {
            btnProducts.BackColor = index == 0 ? Color.FromArgb(230, 240, 255) : Color.Transparent;
            btnProducts.ForeColor = index == 0 ? Theme.Accent : Theme.Text;
        }

        if (btnSuppliers != null)
        {
            btnSuppliers.BackColor = index == 1 ? Color.FromArgb(230, 240, 255) : Color.Transparent;
            btnSuppliers.ForeColor = index == 1 ? Theme.Accent : Theme.Text;
        }

        if (btnReports != null)
        {
            btnReports.BackColor = index == 2 ? Color.FromArgb(230, 240, 255) : Color.Transparent;
            btnReports.ForeColor = index == 2 ? Theme.Accent : Theme.Text;
        }
    }

    private Panel BuildContentArea()
    {
        var contentArea = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        var contentLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contentLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        contentLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        contentLayout.Controls.Add(BuildTopBar(), 0, 0);
        contentLayout.Controls.Add(BuildCommandBar(), 0, 1);
        contentLayout.Controls.Add(BuildTabContent(), 0, 2);

        contentArea.Controls.Add(contentLayout);
        return contentArea;
    }

    private Panel BuildTopBar()
    {
        var topBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.White,
            Padding = new Padding(20, 12, 20, 12),
            Margin = new Padding(0)
        };

        var topLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));

        var title = new Label
        {
            Text = "Inventory Management System",
            Font = new Font(Theme.Base.FontFamily, 12, FontStyle.Bold),
            ForeColor = Theme.Text,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        lblBranch = new Label
        {
            Text = "● Branch: Central Retail Store #01",
            Font = Theme.Base,
            ForeColor = Theme.Muted,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        lblProfile = new Label
        {
            Text = "Profile: Cashier (1) Local Shop",
            Font = Theme.Base,
            ForeColor = Theme.Muted,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight
        };

        topLayout.Controls.Add(title, 0, 0);
        topLayout.Controls.Add(lblBranch, 1, 0);
        topLayout.Controls.Add(lblProfile, 2, 0);

        topBar.Controls.Add(topLayout);

        var border = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Theme.Border };
        topBar.Controls.Add(border);

        return topBar;
    }

    private Panel BuildCommandBar()
    {
        var cmdBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 55,
            BackColor = Color.White,
            Padding = new Padding(20, 8, 20, 8),
            Margin = new Padding(0)
        };

        var cmdLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoScroll = false,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        cmdLayout.Controls.Add(txtSearch);
        cmdLayout.Controls.Add(Theme.MakeButton("➕ Add", ButtonKind.Primary, (_, _) => AddProduct()));
        cmdLayout.Controls.Add(Theme.MakeButton("✏️ Edit", ButtonKind.Secondary, (_, _) => EditProduct()));
        cmdLayout.Controls.Add(Theme.MakeButton("🗑️ Delete", ButtonKind.Danger, (_, _) => DeleteProduct()));
        cmdLayout.Controls.Add(Theme.MakeButton("📤 Stock In/Out", ButtonKind.Primary, (_, _) => AdjustStock()));

        var spacer = new Label { Width = 20, AutoSize = false, BackColor = Color.Transparent };
        cmdLayout.Controls.Add(spacer);

        cmdLayout.Controls.Add(Theme.MakeButton("💾 Export CSV", ButtonKind.Secondary, (_, _) => ExportCurrentView()));
        cmdLayout.Controls.Add(Theme.MakeButton("🖨️ Print", ButtonKind.Secondary, (_, _) => PrintCurrentView()));

        cmdBar.Controls.Add(cmdLayout);

        var border = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Theme.Border };
        cmdBar.Controls.Add(border);

        return cmdBar;
    }

    private Panel BuildTabContent()
    {
        var tabPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.Background,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        mainTabs = new TabControl
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.Background,
            ForeColor = Theme.Text,
            Padding = new Point(0, 0),
            Margin = new Padding(0)
        };

        mainTabs.TabPages.Add(CreateProductsTab());
        mainTabs.TabPages.Add(CreateSuppliersTab());
        mainTabs.TabPages.Add(CreateReportsTab());

        tabPanel.Controls.Add(mainTabs);

        // Status bar at bottom
        var statusBar = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 40,
            BackColor = Color.White,
            Padding = new Padding(20, 8, 20, 8),
            Margin = new Padding(0)
        };

        var statusLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoScroll = false
        };

        lblTotalItems = new Label { Text = "Total Items: 0", Font = Theme.Base, ForeColor = Theme.Text, AutoSize = true, Margin = new Padding(0, 0, 30, 0) };
        lblLowStockAlerts = new Label { Text = "● Low Stock Alerts: 0", Font = Theme.Base, ForeColor = Theme.Danger, AutoSize = true, Margin = new Padding(0, 0, 30, 0) };

        statusLayout.Controls.Add(lblTotalItems);
        statusLayout.Controls.Add(lblLowStockAlerts);
        statusLayout.Controls.Add(new Label { Text = "Database: SQLite (Local C:\\StoreData\\inventory.db)", Font = Theme.Base, ForeColor = Theme.Muted, AutoSize = true });

        statusBar.Controls.Add(statusLayout);

        var border2 = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Theme.Border };
        statusBar.Controls.Add(border2);

        tabPanel.Controls.Add(statusBar);

        return tabPanel;
    }

    private TabPage CreateProductsTab()
    {
        var page = new TabPage("Products") { BackColor = Theme.Background, ForeColor = Theme.Text, Padding = new Padding(0), UseVisualStyleBackColor = false };
        page.Controls.Add(gridProducts);
        return page;
    }

    private TabPage CreateSuppliersTab()
    {
        var page = new TabPage("Suppliers") { BackColor = Theme.Background, ForeColor = Theme.Text, Padding = new Padding(0), UseVisualStyleBackColor = false };
        page.Controls.Add(gridSuppliers);
        return page;
    }

    private TabPage CreateReportsTab()
    {
        var page = new TabPage("Reports") { BackColor = Theme.Background, ForeColor = Theme.Text, Padding = new Padding(0), UseVisualStyleBackColor = false };
        page.Controls.Add(gridReport);
        return page;
    }

    private void RefreshStats()
    {
        try
        {
            var allProducts = ProductRepository.GetAll("");
            var lowStockCount = allProducts.AsEnumerable().Count(r =>
                Convert.ToInt32(r["Quantity"]) <= Convert.ToInt32(r["ReorderLevel"]));

            if (lblTotalItems != null)
                lblTotalItems.Text = $"Total Items: {allProducts.Rows.Count}";

            if (lblLowStockAlerts != null)
                lblLowStockAlerts.Text = $"● Low Stock Alerts: {lowStockCount}";
        }
        catch { }
    }

    private void RefreshProducts()
    {
        gridProducts.DataSource = ProductRepository.GetAll(txtSearch.Text.Trim());
        RefreshStats();
    }

    private void HighlightLowStock()
    {
        foreach (DataGridViewRow row in gridProducts.Rows)
        {
            int qty = Convert.ToInt32(row.Cells["Quantity"].Value);
            int reorder = Convert.ToInt32(row.Cells["ReorderLevel"].Value);
            row.DefaultCellStyle.BackColor = qty <= reorder ? Theme.DangerSoft : Theme.Card;
        }
    }

    private void ExportCurrentView()
    {
        if (mainTabs == null) return;

        var fileName = mainTabs.SelectedIndex switch
        {
            0 => $"Products_Export_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv",
            1 => $"Suppliers_Export_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv",
            2 => $"Reports_Export_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv",
            _ => "Export.csv"
        };

        var grid = mainTabs.SelectedIndex switch
        {
            0 => gridProducts,
            1 => gridSuppliers,
            2 => gridReport,
            _ => gridProducts
        };

        ExportPrintHelper.ExportToCSV(grid, fileName);
    }

    private void PrintCurrentView()
    {
        if (mainTabs == null) return;

        var title = mainTabs.SelectedIndex switch
        {
            0 => "Products Inventory Report",
            1 => "Suppliers Directory",
            2 => "Inventory Reports",
            _ => "Inventory Report"
        };

        var grid = mainTabs.SelectedIndex switch
        {
            0 => gridProducts,
            1 => gridSuppliers,
            2 => gridReport,
            _ => gridProducts
        };

        ExportPrintHelper.PrintPreview(grid, title);
    }

    private void AddProduct()
    {
        using var form = new ProductForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        Run(() => ProductRepository.Add(form.Result));
        Run(RefreshProducts);
    }

    private void EditProduct()
    {
        if (SelectedId(gridProducts, "ProductId") is not int id) { NoSelection(); return; }
        var product = ProductRepository.GetById(id);
        if (product == null) return;

        using var form = new ProductForm(product);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        Run(() => ProductRepository.Update(form.Result));
        Run(RefreshProducts);
    }

    private void DeleteProduct()
    {
        if (SelectedId(gridProducts, "ProductId") is not int id) { NoSelection(); return; }
        if (!Confirm("Delete this product?")) return;
        Run(() => ProductRepository.Delete(id));
        Run(RefreshProducts);
    }

    private void AdjustStock()
    {
        if (SelectedId(gridProducts, "ProductId") is not int id) { NoSelection(); return; }
        string name = gridProducts.CurrentRow!.Cells["Name"].Value?.ToString() ?? "";

        using var form = new StockForm(name);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        Run(() => ProductRepository.AdjustStock(id, form.Change, form.Reason));
        Run(RefreshProducts);
    }

    private void RefreshSuppliers() => gridSuppliers.DataSource = SupplierRepository.GetAll();

    private void AddSupplier()
    {
        using var form = new SupplierForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        Run(() => SupplierRepository.Add(form.Result));
        Run(RefreshSuppliers);
    }

    private void EditSupplier()
    {
        if (SelectedId(gridSuppliers, "SupplierId") is not int id) { NoSelection(); return; }
        var supplier = SupplierRepository.GetById(id);
        if (supplier == null) return;

        using var form = new SupplierForm(supplier);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        Run(() => SupplierRepository.Update(form.Result));
        Run(RefreshSuppliers);
    }

    private void DeleteSupplier()
    {
        if (SelectedId(gridSuppliers, "SupplierId") is not int id) { NoSelection(); return; }
        if (!Confirm("Delete this supplier?")) return;
        Run(() => SupplierRepository.Delete(id));
        Run(RefreshSuppliers);
        Run(RefreshProducts);
    }

    private static void Run(Action action)
    {
        try { action(); }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            MessageBox.Show("That SKU already exists.", "Duplicate SKU", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static int? SelectedId(DataGridView grid, string column) =>
        grid.CurrentRow == null ? null : Convert.ToInt32(grid.CurrentRow.Cells[column].Value);

    private static void NoSelection() =>
        MessageBox.Show("Please select a row first.", "Nothing selected", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private static bool Confirm(string message) =>
        MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
}