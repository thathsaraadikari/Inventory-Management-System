using System.Data;
using Microsoft.Data.SqlClient;

namespace InventoryApp.Data;

public static class Db
{
    private static readonly SqlConnectionStringBuilder ConnectionBuilder = new()
    {
        DataSource = @"(localdb)\MSSQLLocalDB",
        InitialCatalog = "InventoryDB",
        IntegratedSecurity = true,
        TrustServerCertificate = true,
        Encrypt = true
    };

    private static string ConnectionString => ConnectionBuilder.ConnectionString;

    public static SqlConnection GetConnection() => new SqlConnection(ConnectionString);

    public static DataTable Query(string sql, params SqlParameter[] parameters)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddRange(parameters);
        using var adapter = new SqlDataAdapter(cmd);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public static int Execute(string sql, params SqlParameter[] parameters)
    {
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddRange(parameters);
        conn.Open();
        return cmd.ExecuteNonQuery();
    }
}