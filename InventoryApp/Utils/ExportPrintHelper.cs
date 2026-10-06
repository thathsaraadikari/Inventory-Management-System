using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace InventoryApp.Utils;

public static class ExportPrintHelper
{
    /// <summary>
    /// Export DataGridView to CSV file
    /// </summary>
    public static void ExportToCSV(DataGridView grid, string fileName)
    {
        try
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                FileName = fileName,
                DefaultExt = "csv"
            };

            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;

            using var sw = new StreamWriter(saveDialog.FileName, false, Encoding.UTF8);

            // Write headers
            var headers = new StringBuilder();
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                if (i > 0) headers.Append(",");
                headers.Append($"\"{grid.Columns[i].HeaderText}\"");
            }
            sw.WriteLine(headers.ToString());

            // Write data rows
            foreach (DataGridViewRow row in grid.Rows)
            {
                var values = new StringBuilder();
                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    if (i > 0) values.Append(",");
                    var cellValue = row.Cells[i].Value?.ToString() ?? "";
                    values.Append($"\"{cellValue.Replace("\"", "\"\"")}\"");
                }
                sw.WriteLine(values.ToString());
            }

            sw.Close();

            MessageBox.Show($"Data exported successfully to:\n{saveDialog.FileName}", 
                "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting to CSV:\n{ex.Message}", 
                "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Export DataTable to CSV file
    /// </summary>
    public static void ExportDataTableToCSV(DataTable table, string fileName)
    {
        try
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                FileName = fileName,
                DefaultExt = "csv"
            };

            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;

            using var sw = new StreamWriter(saveDialog.FileName, false, Encoding.UTF8);

            // Write headers
            var headers = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (i > 0) headers.Append(",");
                headers.Append($"\"{table.Columns[i].ColumnName}\"");
            }
            sw.WriteLine(headers.ToString());

            // Write data rows
            foreach (DataRow row in table.Rows)
            {
                var values = new StringBuilder();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (i > 0) values.Append(",");
                    var cellValue = row[i]?.ToString() ?? "";
                    values.Append($"\"{cellValue.Replace("\"", "\"\"")}\"");
                }
                sw.WriteLine(values.ToString());
            }

            sw.Close();

            MessageBox.Show($"Data exported successfully to:\n{saveDialog.FileName}",
                "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting to CSV:\n{ex.Message}",
                "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Print DataGridView using default printer
    /// </summary>
    public static void PrintGrid(DataGridView grid, string title)
    {
        try
        {
            var htmlContent = GenerateHtmlFromGrid(grid, title);
            var tempFile = Path.Combine(Path.GetTempPath(), $"print_{Guid.NewGuid()}.html");

            File.WriteAllText(tempFile, htmlContent, Encoding.UTF8);

            using var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                var psi = new ProcessStartInfo
                {
                    FileName = tempFile,
                    UseShellExecute = true,
                    Verb = "print"
                };

                using var process = Process.Start(psi);
                process?.WaitForExit(5000);
            }

            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);

            MessageBox.Show("Print job sent to printer.", "Print Complete", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error printing:\n{ex.Message}",
                "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Preview grid content in browser before printing
    /// </summary>
    public static void PrintPreview(DataGridView grid, string title)
    {
        try
        {
            var htmlContent = GenerateHtmlFromGrid(grid, title);
            var tempFile = Path.Combine(Path.GetTempPath(), $"preview_{Guid.NewGuid()}.html");

            File.WriteAllText(tempFile, htmlContent, Encoding.UTF8);

            Process.Start(new ProcessStartInfo
            {
                FileName = tempFile,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error opening print preview:\n{ex.Message}",
                "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Generate HTML from DataGridView for printing
    /// </summary>
    private static string GenerateHtmlFromGrid(DataGridView grid, string title)
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset='UTF-8'>");
        html.AppendLine($"<title>{title}</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
        html.AppendLine("h1 { color: #333; text-align: center; }");
        html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
        html.AppendLine("th { background-color: #2563EB; color: white; padding: 12px; text-align: left; border: 1px solid #ddd; }");
        html.AppendLine("td { padding: 10px; border: 1px solid #ddd; }");
        html.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
        html.AppendLine("tr:hover { background-color: #f0f0f0; }");
        html.AppendLine(".low-stock { color: #DC2626; font-weight: bold; }");
        html.AppendLine(".in-stock { color: #16A34A; }");
        html.AppendLine("@media print { body { margin: 0; } }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine($"<h1>{title}</h1>");
        html.AppendLine($"<p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
        html.AppendLine("<table>");

        // Headers
        html.Append("<tr>");
        foreach (DataGridViewColumn col in grid.Columns)
        {
            html.Append($"<th>{col.HeaderText}</th>");
        }
        html.AppendLine("</tr>");

        // Data rows
        foreach (DataGridViewRow row in grid.Rows)
        {
            html.Append("<tr>");
            foreach (DataGridViewCell cell in row.Cells)
            {
                var value = cell.Value?.ToString() ?? "";
                html.Append($"<td>{value}</td>");
            }
            html.AppendLine("</tr>");
        }

        html.AppendLine("</table>");
        html.AppendLine("<p style='margin-top: 30px; text-align: center; color: #666; font-size: 12px;'>");
        html.AppendLine("Inventory Management System - Confidential");
        html.AppendLine("</p>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }
}