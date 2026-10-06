# 📦 Inventory Management System

A Windows desktop application for managing products, suppliers and stock levels, with a full stock movement history and built-in reports.

Built with **C#**, **WinForms**, **SQL Server** and **ADO.NET**.

![C#](https://img.shields.io/badge/C%23-512BD4?style=flat-square&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white)
![Windows](https://img.shields.io/badge/Windows-0078D6?style=flat-square&logo=windows&logoColor=white)

---

## ✨ Features

- **Product management:** add, edit and delete products, with live search by name, SKU or category
- **Stock tracking:** stock in and stock out with a reason, and automatic protection against going below zero
- **Movement history:** every quantity change is recorded, so the stock level can always be traced
- **Low-stock alerts:** products at or below their reorder level are highlighted in the grid
- **Supplier management:** add, edit and delete suppliers and link them to products
- **Reports:**
  - Low stock list with supplier contact details
  - Stock value by category
  - Last 100 stock movements
- **Data safety:** parameterized queries (no SQL injection) and database transactions for stock changes

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Language | C# |
| Framework | .NET 8, Windows Forms |
| Database | SQL Server (LocalDB or Express) |
| Data access | ADO.NET (`Microsoft.Data.SqlClient`) |

## 📸 Screenshots

| Products | Reports |
|---|---|
| ![Products](Docs/products.png) | ![Reports](Docs/reports.png) |

## 🚀 Getting Started

### Prerequisites

- Windows 10 or 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or Visual Studio 2022 with the **.NET desktop development** workload)
- SQL Server LocalDB or SQL Server Express
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms) to run the SQL scripts

### 1. Clone the repository

```bash
git clone https://github.com/thathsaraadikari/inventory-management-system.git
cd inventory-management-system
```

### 2. Set up the database

1. Open SSMS and connect to your server (for example `(localdb)\MSSQLLocalDB`). If the connection fails, tick **Trust Server Certificate**.
2. Open a new query window and run `Database/schema.sql`. This creates the `InventoryDB` database and its tables.
3. *(Optional)* Run `Database/seed.sql` to load sample suppliers, products and stock movements.

### 3. Configure the connection

Open `Data/Db.cs` and set `DataSource` to your server name:

```csharp
DataSource = @"(localdb)\MSSQLLocalDB",   // or @"localhost\SQLEXPRESS"
```

The app uses Windows Authentication, so no username or password is needed.

### 4. Run the app

```bash
dotnet run
```

Or open the solution in Visual Studio and press **F5**.

## 🗂️ Project Structure

```
InventoryApp/
├── Database/
│   ├── schema.sql              # Tables and constraints
│   └── seed.sql                # Sample data (optional)
├── Models/                     # Product and Supplier classes
├── Data/
│   ├── Db.cs                   # Connection and query helpers
│   ├── ProductRepository.cs    # Product CRUD and stock adjustment
│   ├── SupplierRepository.cs   # Supplier CRUD
│   └── ReportRepository.cs     # Report queries
├── Forms/                      # Main window and dialogs
├── UI.cs                       # Shared dialog layout helpers
└── Program.cs                  # Entry point
```

## 🗄️ Database Design

```
Suppliers 1 ──── ∞ Products 1 ──── ∞ StockMovements
```

| Table | Purpose |
|---|---|
| `Suppliers` | Supplier name and contact details |
| `Products` | Name, unique SKU, category, price, quantity, reorder level, supplier |
| `StockMovements` | One row for every stock in or out, with a reason and timestamp |

A product's quantity is never edited directly. It only changes through **Stock In/Out**, which updates the quantity and writes a movement record inside a single transaction, so the two can never get out of sync.

## 📚 What I Learned

- Building a layered desktop app (UI, data access and models kept separate)
- Writing safe database code with parameterized queries and transactions
- Designing a relational schema with foreign keys and constraints
- Handling errors such as duplicate SKUs and insufficient stock

## 🔮 Future Improvements

- [ ] Modern dashboard and sidebar navigation
- [ ] Login screen with admin and staff roles
- [ ] Export reports to CSV or Excel
- [ ] Barcode scanning support
- [ ] Unit tests for the repositories

## 👤 Author

**Thathsara Adikari**
Undergraduate at the University of Colombo School of Computing (UCSC)

[![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat-square&logo=github&logoColor=white)](https://github.com/thathsaraadikari)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0A66C2?style=flat-square&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/thathsara-adikari-211b03362/)
