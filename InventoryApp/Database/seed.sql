/* ============================================================
   Inventory Management System - Dummy Data (seed.sql)
   Run AFTER schema.sql. Safe to re-run: it clears the tables first.
   WARNING: this deletes all existing suppliers, products and movements.
   ============================================================ */
USE InventoryDB;
GO

-- 1. Clear existing data (children first) and reset ID counters
DELETE FROM StockMovements;
DELETE FROM Products;
DELETE FROM Suppliers;
DBCC CHECKIDENT ('StockMovements', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Suppliers', RESEED, 0);
GO

-- 2. Suppliers
INSERT INTO Suppliers (Name, ContactPerson, Phone, Email, Address) VALUES
(N'TechWorld Distributors',   N'Nuwan Perera',        N'0112345678', N'sales@techworld.lk',      N'45 Galle Road, Colombo 03'),
(N'Office Plus Lanka',        N'Dilini Fernando',     N'0112456789', N'orders@officeplus.lk',    N'12 Duplication Road, Colombo 04'),
(N'Fresh Foods Pvt Ltd',      N'Kasun Silva',         N'0771234567', N'supply@freshfoods.lk',    N'88 Kandy Road, Kiribathgoda'),
(N'Home & Living Imports',    N'Amaya Jayawardena',   N'0719876543', N'info@homeliving.lk',      N'23 High Level Road, Nugegoda'),
(N'Sports Zone Wholesale',    N'Ruwan Wickramasinghe',N'0765551234', N'wholesale@sportszone.lk', N'7 Temple Road, Kandy'),
(N'CleanCare Supplies',       N'Sanduni Rathnayake',  N'0703334455', N'hello@cleancare.lk',      N'56 Main Street, Negombo');

-- 3. Products + stock movements
--    Quantity is never typed in directly. It is calculated from the movements,
--    exactly like the app does it, so the data stays consistent.
DECLARE @Seed TABLE (
    Name         NVARCHAR(100),
    SKU          NVARCHAR(50),
    Category     NVARCHAR(50),
    Price        DECIMAL(10,2),
    ReorderLevel INT,
    SupplierName NVARCHAR(100),
    InitialQty   INT,
    SoldQty      INT
);

INSERT INTO @Seed (Name, SKU, Category, Price, ReorderLevel, SupplierName, InitialQty, SoldQty) VALUES
-- Electronics
(N'Wireless Mouse',               N'WM-001', N'Electronics', 2500.00, 10, N'TechWorld Distributors',  60, 52),
(N'Mechanical Keyboard',          N'KB-001', N'Electronics', 7500.00,  5, N'TechWorld Distributors',  30, 18),
(N'USB-C Charger 65W',            N'CH-065', N'Electronics', 4200.00,  8, N'TechWorld Distributors',  40, 15),
(N'HDMI Cable 2m',                N'HD-002', N'Electronics', 1200.00, 15, N'TechWorld Distributors',  80, 30),
(N'Bluetooth Speaker',            N'BS-010', N'Electronics', 8900.00,  5, N'TechWorld Distributors',  20, 16),
(N'USB Flash Drive 64GB',         N'FD-064', N'Electronics', 2200.00, 10, N'TechWorld Distributors', 100, 45),
-- Stationery
(N'A4 Paper Ream (500 sheets)',   N'PA-A4',  N'Stationery',  1350.00, 20, N'Office Plus Lanka',      200, 120),
(N'Ballpoint Pens (Box of 50)',   N'PN-050', N'Stationery',  1800.00, 10, N'Office Plus Lanka',       50, 38),
(N'Spiral Notebook 200 Pages',    N'NB-200', N'Stationery',   450.00, 25, N'Office Plus Lanka',      150, 110),
(N'Heavy Duty Stapler',           N'ST-010', N'Stationery',  1650.00,  5, N'Office Plus Lanka',       25, 20),
(N'Highlighter Set (6 pcs)',      N'HL-006', N'Stationery',   780.00, 15, N'Office Plus Lanka',       90, 70),
-- Grocery
(N'Basmati Rice 5kg',             N'RC-005', N'Grocery',     2450.00, 20, N'Fresh Foods Pvt Ltd',    120, 95),
(N'Coconut Oil 1L',               N'OL-001', N'Grocery',     1150.00, 15, N'Fresh Foods Pvt Ltd',     80, 68),
(N'Ceylon Tea 400g',              N'TE-400', N'Grocery',     1050.00, 20, N'Fresh Foods Pvt Ltd',    150, 90),
(N'Red Lentils 1kg',              N'LN-001', N'Grocery',      640.00, 25, N'Fresh Foods Pvt Ltd',    140, 100),
(N'Milk Powder 400g',             N'MP-400', N'Grocery',     1100.00, 20, N'Fresh Foods Pvt Ltd',    120, 85),
-- Home & Living
(N'LED Desk Lamp',                N'LP-020', N'Home',        3600.00,  6, N'Home & Living Imports',   35, 22),
(N'Ceramic Mug Set (6 pcs)',      N'MG-006', N'Home',        2900.00,  8, N'Home & Living Imports',   40, 30),
(N'Storage Box 30L',              N'SB-030', N'Home',        1950.00, 10, N'Home & Living Imports',   60, 41),
(N'Bed Sheet Set (Queen)',        N'BD-Q01', N'Home',        6800.00,  5, N'Home & Living Imports',   25, 22),
-- Sports
(N'Yoga Mat',                     N'YM-006', N'Sports',      3200.00,  8, N'Sports Zone Wholesale',   40, 28),
(N'Dumbbell Set 10kg',            N'DB-010', N'Sports',      9500.00,  4, N'Sports Zone Wholesale',   15, 9),
(N'Cricket Bat (Willow)',         N'CB-001', N'Sports',     14500.00,  3, N'Sports Zone Wholesale',   12, 10),
(N'Football (Size 5)',            N'FB-005', N'Sports',      4800.00,  6, N'Sports Zone Wholesale',   30, 19),
-- Cleaning
(N'Dishwashing Liquid 750ml',     N'DL-750', N'Cleaning',     520.00, 30, N'CleanCare Supplies',     200, 150),
(N'Laundry Detergent 2kg',        N'LD-002', N'Cleaning',    1850.00, 15, N'CleanCare Supplies',      90, 20),
(N'Floor Cleaner 1L',             N'FC-001', N'Cleaning',     690.00, 20, N'CleanCare Supplies',     110, 70),
(N'Hand Sanitizer 500ml',         N'HS-500', N'Cleaning',     850.00, 20, N'CleanCare Supplies',     130, 30);

INSERT INTO Products (Name, SKU, Category, Price, Quantity, ReorderLevel, SupplierId)
SELECT t.Name, t.SKU, t.Category, t.Price, 0, t.ReorderLevel, s.SupplierId
FROM @Seed t
JOIN Suppliers s ON s.Name = t.SupplierName;

-- Initial stock (45 days ago)
INSERT INTO StockMovements (ProductId, ChangeQty, Reason, MovedAt)
SELECT p.ProductId, t.InitialQty, N'Initial stock', DATEADD(DAY, -45, SYSDATETIME())
FROM @Seed t JOIN Products p ON p.SKU = t.SKU;

-- Sales, first half (20 days ago)
INSERT INTO StockMovements (ProductId, ChangeQty, Reason, MovedAt)
SELECT p.ProductId, -(t.SoldQty / 2), N'Sales', DATEADD(DAY, -20, SYSDATETIME())
FROM @Seed t JOIN Products p ON p.SKU = t.SKU
WHERE t.SoldQty / 2 > 0;

-- Sales, second half (5 days ago)
INSERT INTO StockMovements (ProductId, ChangeQty, Reason, MovedAt)
SELECT p.ProductId, -(t.SoldQty - t.SoldQty / 2), N'Sales', DATEADD(DAY, -5, SYSDATETIME())
FROM @Seed t JOIN Products p ON p.SKU = t.SKU
WHERE t.SoldQty - t.SoldQty / 2 > 0;

-- A few restocks (10 days ago)
INSERT INTO StockMovements (ProductId, ChangeQty, Reason, MovedAt)
SELECT p.ProductId, v.Qty, N'Restock from supplier', DATEADD(DAY, -10, SYSDATETIME())
FROM (VALUES (N'KB-001', 10), (N'DB-010', 5), (N'RC-005', 30), (N'TE-400', 40)) AS v(SKU, Qty)
JOIN Products p ON p.SKU = v.SKU;

-- 4. Set each product's Quantity from its movements
UPDATE p
SET p.Quantity = ISNULL((SELECT SUM(m.ChangeQty) FROM StockMovements m WHERE m.ProductId = p.ProductId), 0)
FROM Products p;
GO

-- 5. Quick check
SELECT COUNT(*) AS Suppliers FROM Suppliers;
SELECT COUNT(*) AS Products FROM Products;
SELECT COUNT(*) AS Movements FROM StockMovements;
SELECT Name, SKU, Quantity, ReorderLevel FROM Products WHERE Quantity <= ReorderLevel ORDER BY Quantity;
GO