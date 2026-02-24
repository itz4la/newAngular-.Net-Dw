-- 1. Create Dim_Date
CREATE TABLE Dim_Date (
    DateSK INT IDENTITY(1,1) PRIMARY KEY,
    FullDate DATE NOT NULL,
    [Year] INT,
    [Month] INT,
    MonthName VARCHAR(20),
    [Quarter] INT,
    DayOfWeek VARCHAR(20)
);

-- 2. Create Dim_Customer
CREATE TABLE Dim_Customer (
    CustomerSK INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL, -- Business Key
    CustomerName VARCHAR(150),
    CustomerCategoryName VARCHAR(50),
    BuyingGroupName VARCHAR(50)
);

-- 3. Create Dim_StockItem
CREATE TABLE Dim_StockItem (
    StockItemSK INT IDENTITY(1,1) PRIMARY KEY,
    StockItemID INT NOT NULL, -- Business Key
    StockItemName VARCHAR(150),
    Brand VARCHAR(50),
    Size VARCHAR(20),
    ColorName VARCHAR(20),
    UnitPrice DECIMAL(18,2)
);

-- 4. Create Dim_Employee
CREATE TABLE Dim_Employee (
    EmployeeSK INT IDENTITY(1,1) PRIMARY KEY,
    PersonID INT NOT NULL, -- Business Key
    EmployeeName VARCHAR(150),
    PreferredName VARCHAR(50),
    IsSalesperson BIT
);

-- 5. Create Dim_City
CREATE TABLE Dim_City (
    CitySK INT IDENTITY(1,1) PRIMARY KEY,
    CityID INT NOT NULL, -- Business Key
    CityName VARCHAR(50),
    StateProvinceName VARCHAR(50),
    SalesTerritory VARCHAR(50)
);

-- 6. Create Fact_Sales
CREATE TABLE Fact_Sales (
    SalesSK INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceLineID INT NOT NULL, -- Business Key
    DateSK INT FOREIGN KEY REFERENCES Dim_Date(DateSK),
    CustomerSK INT FOREIGN KEY REFERENCES Dim_Customer(CustomerSK),
    StockItemSK INT FOREIGN KEY REFERENCES Dim_StockItem(StockItemSK),
    EmployeeSK INT FOREIGN KEY REFERENCES Dim_Employee(EmployeeSK),
    CitySK INT FOREIGN KEY REFERENCES Dim_City(CitySK),
    Quantity INT,
    TotalAmount DECIMAL(18,2)
);