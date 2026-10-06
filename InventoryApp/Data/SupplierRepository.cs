using System.Data;
using InventoryApp.Models;
using Microsoft.Data.SqlClient;

namespace InventoryApp.Data;

public static class SupplierRepository {
    public static DataTable GetAll() => Db.Query("SELECT SupplierId, Name, ContactPerson, Phone, Email, Address FROM Suppliers ORDER BY Name");

    public static List<Supplier> GetList() => GetAll().AsEnumerable().Select(r => new Supplier {
        SupplierId = r.Field<int>("SupplierId"),
        Name = r.Field<string>("Name") ?? "",
        ContactPerson = r.Field<string>("ContactPerson") ?? "",
        Phone = r.Field<string>("Phone") ?? "",
        Email = r.Field<string>("Email") ?? "",
        Address = r.Field<string>("Address") ?? ""
    }).ToList();

    public static Supplier? GetById(int id) => 
        GetList().FirstOrDefault(s => s.SupplierId == id);

    private static SqlParameter[] Params(Supplier s) => new[] {
        new SqlParameter("@name", s.Name),
        new SqlParameter("@contact", s.ContactPerson),
        new SqlParameter("@phone", s.Phone),
        new SqlParameter("@email", s.Email),
        new SqlParameter("@address", s.Address)
    };

    public static void Add(Supplier s) => Db.Execute(@"INSERT INTO Suppliers (Name, ContactPerson, Phone, Email, Address)
        VALUES (@name, @contact, @phone, @email, @address)", Params(s));

    public static void Update(Supplier s)
    {
        var parameters = Params(s).Append(new SqlParameter("@id", s.SupplierId)).ToArray();
        Db.Execute(@"UPDATE Suppliers
            SET Name = @name, ContactPerson = @contact, Phone = @phone, Email = @email, Address = @address
            WHERE SupplierId = @id", parameters);
    }

    public static void Delete(int id) => Db.Execute("DELETE FROM Suppliers WHERE SupplierId = @id", new SqlParameter("@id", id));
}