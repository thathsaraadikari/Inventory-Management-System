/* ============================================================
   Inventory Management System - Database Schema (schema.sql)
   Run this FIRST, then seed.sql (optional sample data).
   Safe to re-run: existing objects are not recreated.
   ============================================================ */

IF DB_ID(N'InventoryDB') IS NULL
    CREATE DATABASE InventoryDB;
GO

USE InventoryDB;
GO

IF OBJECT_ID(N'dbo.Suppliers', N'U') IS NULL
CREATE TABLE Suppliers (
    SupplierId    INT IDENTITY(1,1) PRIMARY KEY,
    Name          NVARCHAR(100) NOT NULL,
    ContactPerson NVARCHAR(100) NULL,
    Phone         NVARCHAR(30)  NULL,
    Email         NVARCHAR(100) NULL,
    Address       NVARCHAR(200) NULL
);
GO

IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
CREATE TABLE Products (
    ProductId    INT IDENTITY(1,1) PRIMARY KEY,
    Name         NVARCHAR(100) NOT NULL,
    SKU          NVARCHAR(50)  NOT NULL UNIQUE,
    Category     NVARCHAR(50)  NULL,
    Price        DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    Quantity     INT NOT NULL DEFAULT 0 CHECK (Quantity >= 0),
    ReorderLevel INT NOT NULL DEFAULT 5 CHECK (ReorderLevel >= 0),
    SupplierId   INT NULL,
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierId)
        REFERENCES Suppliers(SupplierId) ON DELETE SET NULL
);
GO

IF OBJECT_ID(N'dbo.StockMovements', N'U') IS NULL
CREATE TABLE StockMovements (
    MovementId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId  INT NOT NULL,
    ChangeQty  INT NOT NULL,
    Reason     NVARCHAR(200) NULL,
    MovedAt    DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Movements_Products FOREIGN KEY (ProductId)
        REFERENCES Products(ProductId) ON DELETE CASCADE
);
GO