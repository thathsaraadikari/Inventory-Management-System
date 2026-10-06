namespace InventoryApp.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public string Sku { get; set; } = "";
        public string Category { get; set; } = "";

        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public int? SupplierID {get; set;}
    }
}