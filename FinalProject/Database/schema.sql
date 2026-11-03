-- =====================================================================
-- Restaurant Order & Kitchen Coordination System - SQLite schema
-- SODV2101 Rapid Application Development
-- =====================================================================
-- Design notes
--  * IsActive columns = soft delete. Deleting a menu item / user / table
--    that old orders reference would break sales history, so the manager's
--    "Delete" button sets IsActive = 0 instead.
--  * OrderItems.UnitPrice and Orders.TaxRate are copied at the moment the
--    order is placed, so changing a price or the tax rate later never
--    rewrites past sales.
--  * "Table" is a reserved word in SQL, so the table is called DiningTables.
--  * Dates are stored as ISO-8601 text ('YYYY-MM-DD HH:MM:SS', local time).
-- =====================================================================

PRAGMA foreign_keys = ON;

DROP TABLE IF EXISTS OrderItems;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS MenuItems;
DROP TABLE IF EXISTS Categories;
DROP TABLE IF EXISTS DiningTables;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Settings;

-- ---------------------------------------------------------------------
-- Settings: key/value pairs the manager can edit (tax rate, branding)
-- ---------------------------------------------------------------------
CREATE TABLE Settings (
    Key         TEXT PRIMARY KEY,
    Value       TEXT NOT NULL
);

-- ---------------------------------------------------------------------
-- Users: managers, servers and cooks
-- PasswordHash = SHA-256 hex of the password (see README / LoginService)
-- ---------------------------------------------------------------------
CREATE TABLE Users (
    UserID          INTEGER PRIMARY KEY AUTOINCREMENT,
    Username        TEXT    NOT NULL UNIQUE COLLATE NOCASE,
    PasswordHash    TEXT    NOT NULL,
    FullName        TEXT    NOT NULL,
    Role            TEXT    NOT NULL CHECK (Role IN ('Manager', 'Server', 'Cook')),
    IsActive        INTEGER NOT NULL DEFAULT 1 CHECK (IsActive IN (0, 1)),
    CreatedAt       TEXT    NOT NULL DEFAULT (datetime('now', 'localtime'))
);

-- ---------------------------------------------------------------------
-- Categories: drive the tabs on the server's menu screen
-- ---------------------------------------------------------------------
CREATE TABLE Categories (
    CategoryID      INTEGER PRIMARY KEY AUTOINCREMENT,
    Name            TEXT    NOT NULL UNIQUE,
    SortOrder       INTEGER NOT NULL DEFAULT 0,
    IsActive        INTEGER NOT NULL DEFAULT 1 CHECK (IsActive IN (0, 1))
);

-- ---------------------------------------------------------------------
-- MenuItems
--  IsAvailable = temporarily out of stock (86'd) but still on the menu
--  IsActive    = removed from the menu (soft delete)
-- ---------------------------------------------------------------------
CREATE TABLE MenuItems (
    ItemID          INTEGER PRIMARY KEY AUTOINCREMENT,
    Name            TEXT    NOT NULL,
    CategoryID      INTEGER NOT NULL REFERENCES Categories(CategoryID),
    Description     TEXT,
    Price           REAL    NOT NULL CHECK (Price >= 0),
    ImagePath       TEXT,
    IsAvailable     INTEGER NOT NULL DEFAULT 1 CHECK (IsAvailable IN (0, 1)),
    IsActive        INTEGER NOT NULL DEFAULT 1 CHECK (IsActive IN (0, 1))
);

-- ---------------------------------------------------------------------
-- DiningTables
-- ---------------------------------------------------------------------
CREATE TABLE DiningTables (
    TableID         INTEGER PRIMARY KEY AUTOINCREMENT,
    TableNumber     INTEGER NOT NULL UNIQUE,
    Seats           INTEGER NOT NULL DEFAULT 4 CHECK (Seats > 0),
    Status          TEXT    NOT NULL DEFAULT 'Available'
                    CHECK (Status IN ('Available', 'Occupied', 'Reserved')),
    IsActive        INTEGER NOT NULL DEFAULT 1 CHECK (IsActive IN (0, 1))
);

-- ---------------------------------------------------------------------
-- Orders
--  Status flow: NotStarted -> Preparing -> Ready -> Completed
--  (Cancelled is available for voided orders)
-- ---------------------------------------------------------------------
CREATE TABLE Orders (
    OrderID         INTEGER PRIMARY KEY AUTOINCREMENT,
    TableID         INTEGER NOT NULL REFERENCES DiningTables(TableID),
    UserID          INTEGER NOT NULL REFERENCES Users(UserID),
    OrderDate       TEXT    NOT NULL DEFAULT (datetime('now', 'localtime')),
    Status          TEXT    NOT NULL DEFAULT 'NotStarted'
                    CHECK (Status IN ('NotStarted', 'Preparing', 'Ready', 'Completed', 'Cancelled')),
    Subtotal        REAL    NOT NULL DEFAULT 0,
    TaxRate         REAL    NOT NULL DEFAULT 0,
    TaxAmount       REAL    NOT NULL DEFAULT 0,
    Total           REAL    NOT NULL DEFAULT 0,
    Notes           TEXT,
    StartedAt       TEXT,   -- cook clicked Start Preparing
    ReadyAt         TEXT,   -- cook clicked Mark Ready
    CompletedAt     TEXT    -- server served / closed the order
);

-- ---------------------------------------------------------------------
-- OrderItems
-- ---------------------------------------------------------------------
CREATE TABLE OrderItems (
    OrderItemID     INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderID         INTEGER NOT NULL REFERENCES Orders(OrderID) ON DELETE CASCADE,
    ItemID          INTEGER NOT NULL REFERENCES MenuItems(ItemID),
    Quantity        INTEGER NOT NULL CHECK (Quantity > 0),
    UnitPrice       REAL    NOT NULL CHECK (UnitPrice >= 0),
    Notes           TEXT    -- e.g. "no onions"
);

-- ---------------------------------------------------------------------
-- Indexes for the kitchen screen and dashboard queries
-- ---------------------------------------------------------------------
CREATE INDEX IX_Orders_Status    ON Orders(Status);
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate);
CREATE INDEX IX_Orders_TableID   ON Orders(TableID);
CREATE INDEX IX_OrderItems_Order ON OrderItems(OrderID);
CREATE INDEX IX_OrderItems_Item  ON OrderItems(ItemID);
CREATE INDEX IX_MenuItems_Cat    ON MenuItems(CategoryID);
