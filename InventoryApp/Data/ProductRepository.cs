using System.Data;
using InventoryApp.Models;
using Microsoft.Data.SqlClient;

namespace InventoryApp.Data;

public static class ProductRepository
{
    public static DataTable GetAll(string search) => Db.Query(@"
        SELECT p.ProductId, p.Name, p.SKU, p.Category, p.Price,
               p.Quantity, p.ReorderLevel, s.Name AS Supplier
        FROM Products p
        LEFT JOIN Suppliers s ON s.SupplierId = p.SupplierId
        WHERE p.Name LIKE @s OR p.SKU LIKE @s OR p.Category LIKE @s
        ORDER BY p.Name",
        new SqlParameter("@s", $"%{search}%"));

    public static Product? GetById(int id)
    {
        using var conn = Db.GetConnection();
        using var cmd = new SqlCommand("SELECT * FROM Products WHERE ProductId = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();
        using var reader = cmd.ExecuteReader();
        if(!reader.Read()) return null;

        return new Product
        {
            ProductId = (int)reader["ProductId"],
            Name = (string)reader["Name"],
            Sku = (string)reader["SKU"],
            Category = (string)reader["Category"] as string ?? "",
            Price = (decimal)reader["Price"],
            Quantity = (int)reader["Quantity"],
            ReorderLevel = (int)reader["ReorderLevel"],
            SupplierID = reader["SupplierId"] as int?
        };
    }
    private static SqlParameter[] Params(Product p) => new[]
    {
        new SqlParameter("@name", p.Name),
        new SqlParameter("@sku", p.Sku),
        new SqlParameter("@cat", p.Category),
        new SqlParameter("@price", p.Price),
        new SqlParameter("@reorder", p.ReorderLevel),
        new SqlParameter("@sup", (object?)p.SupplierID ?? DBNull.Value)
    };

    public static void Add(Product p) => Db.Execute(@" INSERT INTO Products (Name, SKU, Category, Price, Quantity, ReorderLevel, SupplierId)
        VALUES (@name, @sku, @cat, @price, 0, @reorder, @sup)", Params(p));

    public static void Update(Product p)
    {
        var parameters = Params(p).Append(new SqlParameter("@id", p.ProductId)).ToArray();
        Db.Execute(@" UPDATE Products
            SET Name = @name, SKU = @sku, Category = @cat, Price = @price,
                ReorderLevel = @reorder, SupplierId = @sup
            WHERE ProductId = @id", parameters);
    }

    public static void Delete(int id) => Db.Execute("DELETE FROM Products WHERE ProductId = @id", new SqlParameter("@id", id));

    public static void AdjustStock(int productId, int change, string reason) { 
        using var conn = Db.GetConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            using var update = new SqlCommand("UPDATE Products SET Quantity = Quantity + @c WHERE ProductId = @id AND Quantity + @c >= 0", conn, tx);
            update.Parameters.AddWithValue("@c", change);
            update.Parameters.AddWithValue("@id", productId);
            if (update.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("Not Enough Stock for this Operation.");
            using var log = new SqlCommand("INSERT INTO StockAdjustments (ProductId, ChangeQty, Reason) VALUES (@id, @c, @r)", conn, tx);
            log.Parameters.AddWithValue("@id", productId);
            log.Parameters.AddWithValue("@c", change);
            log.Parameters.AddWithValue("@r", reason);
            log.ExecuteNonQuery();
            tx.Commit();
        }
        catch { 
            tx.Rollback();
            throw;
        }
    }
}