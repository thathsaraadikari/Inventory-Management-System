using System.Data;

namespace InventoryApp.Data;

public static class ReportRepository
{
    public static DataTable LowStock() => Db.Query(@"
        SELECT p.Name, p.SKU, p.Quantity, p.ReorderLevel,
               s.Name AS Supplier, s.Phone AS SupplierPhone
        FROM Products p
        LEFT JOIN Suppliers s ON s.SupplierId = p.SupplierId
        WHERE p.Quantity <= p.ReorderLevel
        ORDER BY p.Quantity");

    public static DataTable StockValueByCategory() => Db.Query(@"
        SELECT COALESCE(NULLIF(Category, ''), 'Uncategorized') AS Category,
               COUNT(*) AS Products,
               SUM(Quantity) AS Units,
               SUM(Price * Quantity) AS TotalValue
        FROM Products
        GROUP BY COALESCE(NULLIF(Category, ''), 'Uncategorized')
        ORDER BY TotalValue DESC");

    public static DataTable RecentMovements() => Db.Query(@"
        SELECT TOP 100 m.MovedAt, p.Name AS Product, m.ChangeQty, m.Reason
        FROM StockMovements m
        JOIN Products p ON p.ProductId = m.ProductId
        ORDER BY m.MovedAt DESC");
}