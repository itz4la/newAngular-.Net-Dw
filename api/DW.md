Data Warehouse ETL Project DocumentationSource System: WideWorldImporters (WWI) Standard Database
Target Architecture: 5-Dimension Star Schema
ETL Tool: SQL Server Integration Services (SSIS)1. Project Overview & ArchitectureThis project extracts transactional data from the WideWorldImporters operational database and transforms it into a highly optimized, denormalized Star Schema for analytical reporting.We selected a 5-Dimension Star Schema over a Snowflake or Constellation schema because:Performance: It minimizes the number of joins required during querying, providing the fastest read times for BI tools like Power BI.Simplicity: It provides a clean, intuitive structure for end-users to understand.1.1 Schema DesignThe schema centers around one Fact table surrounded by five descriptive dimensions:Dim_Date: Tracks the date of the invoice.Dim_Customer: Attributes related to the buyer.Dim_StockItem: Attributes related to the product sold.Dim_Employee: Attributes related to the salesperson.Dim_City: Geographical attributes for the delivery location.Fact_Sales: The central table containing measurable metrics (Quantity, Total Amount).2. Phase 1: Database Creation (DDL Scripts)Open SQL Server Management Studio (SSMS), create a new blank database named WWI_DataWarehouse, open a New Query window, paste this code, and click "Execute".-- 1. Create Dim_Date
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
); 3. Phase 2: Visual Studio Setup & Connections (Beginner Steps)Before building any Data Flows, you must tell SSIS where your data is coming from and where it is going.Open Visual Studio and create a new Integration Services Project.Look at the bottom of your screen for the Connection Managers window.Create the Source Connection (WWI):Right-click in the Connection Managers window -> Select New OLE DB Connection.Click New.In "Server name", type localhost (or your SQL Server name).In "Select or enter a database name", choose WideWorldImporters.Click OK, then OK again. Rename this connection manager to Source_WWI.Create the Destination Connection (Data Warehouse):Right-click again -> Select New OLE DB Connection.Click New.Server name: localhost.Database name: WWI_DataWarehouse (the one you just created in SSMS).Click OK. Rename this connection manager to Dest_DW.4. Phase 3: Loading Dimensions with Upsert Logic (Load_Dimensions.dtsx)To prevent duplicate data when running the package multiple times, we use Upsert (Update existing, Insert new) logic. For the Date table, we filter out existing dates. For the other tables, we use the Slowly Changing Dimension (SCD) wizard.4.1. Loading Dim_Date (Insert Only - Filtering Duplicates)If a date already exists, we want to ignore it. If it doesn't, we insert it.Create the Task: Add a Data Flow Task named Load Dim_Date and open it.Add Source: Drag an OLE DB Source.Connection: Source_WWI. Mode: SQL command.Code:WITH DateCTE AS (
SELECT CAST('2013-01-01' AS DATE) AS FullDate
UNION ALL
SELECT DATEADD(day, 1, FullDate)
FROM DateCTE
WHERE FullDate < '2016-12-31'
)
SELECT
FullDate,
YEAR(FullDate) AS [Year],
MONTH(FullDate) AS [Month],
DATENAME(month, FullDate) AS MonthName,
DATEPART(quarter, FullDate) AS [Quarter],
DATENAME(weekday, FullDate) AS DayOfWeek
FROM DateCTE
OPTION (MAXRECURSION 0);
Convert Text: Drag a Data Conversion. Connect the Source to it.Check MonthName and DayOfWeek. Change their Data Type to string [DT_STR] and Length to 20.Filter Existing Dates (New Step): Drag a Lookup block. Connect Data Conversion to it.Double-click Lookup. General Tab: Change "Fail component" to Redirect rows to no match output.Connection Tab: Select Dest_DW and table [dbo].[Dim_Date].Columns Tab: Drag FullDate (left) to FullDate (right). Do not check any boxes. Click OK.Why? This checks the database. If the date exists, it throws it away. If it doesn't exist, it sends it to the "No Match" exit.Destination: Drag an OLE DB Destination.Connect the blue arrow from the Lookup to the Destination. A box will pop up: select Lookup No Match Output and click OK.Open the Destination. Connection: Dest_DW. Table: [dbo].[Dim_Date].Mappings: Ensure MonthName maps to Copy of MonthName, and DayOfWeek maps to Copy of DayOfWeek. Click OK.4.2. Loading Dim_Customer (Upsert using SCD Wizard)We will use the SCD Wizard to automatically update customer names if they change, or insert them if they are brand new.Create Task: Add a Data Flow Task named Load Dim_Customer and open it.Add Source: Drag an OLE DB Source.Connection: Source_WWI. Mode: SQL command.Query:SELECT
c.CustomerID,
CAST(c.CustomerName AS VARCHAR(150)) AS CustomerName,
CAST(cc.CustomerCategoryName AS VARCHAR(50)) AS CustomerCategoryName,
CAST(bg.BuyingGroupName AS VARCHAR(50)) AS BuyingGroupName
FROM Sales.Customers c
LEFT JOIN Sales.CustomerCategories cc ON c.CustomerCategoryID = cc.CustomerCategoryID
LEFT JOIN Sales.BuyingGroups bg ON c.BuyingGroupID = bg.BuyingGroupID
Clean Data: Drag a Derived Column. Connect the Source to it.Derived Column Name: CleanBuyingGroup.Derived Column action: Replace 'BuyingGroupName'.Expression: REPLACENULL(BuyingGroupName,"No Group")The SCD Wizard (Replaces OLE DB Destination): Drag a Slowly Changing Dimension block from the toolbox. Connect the Derived Column to it.Double-click the Slowly Changing Dimension block to open the Wizard. Click Next.Connection Manager: Dest_DW. Table: [dbo].[Dim_Customer].In the grid below, find CustomerID. Under "Dimension Columns", make sure it maps to CustomerID. Change the "Key Type" dropdown to Business Key. Click Next.Choose Change Types: Here, you tell SSIS what to do when data changes. For CustomerName, CustomerCategoryName, and CleanBuyingGroup (the three remaining columns), select Changing attribute from the dropdowns. (This is called a Type 1 SCD—it just overwrites old data with new data). Click Next.Uncheck "Fail the transformation if changes are detected in a fixed attribute". Click Next.Click Next on the remaining screens until you hit Finish.Watch the magic: Visual Studio will automatically generate two new branches (an OLE DB Command for Updates, and an OLE DB Destination for Inserts). Your duplicates problem is solved!4.3. Loading Dim_StockItem (Upsert using SCD Wizard)Add a Data Flow Task named Load Dim_StockItem.Source: Drag an OLE DB Source (Source_WWI, SQL command). We use ISNULL to fix empty data and CAST to fix data types directly in SQL, avoiding SSIS conversion errors:SELECT
si.StockItemID,
CAST(si.StockItemName AS VARCHAR(150)) AS StockItemName,
CAST(ISNULL(si.Brand, 'N/A') AS VARCHAR(50)) AS Brand,
CAST(ISNULL(si.Size, 'N/A') AS VARCHAR(20)) AS Size,
CAST(ISNULL(c.ColorName, 'N/A') AS VARCHAR(20)) AS ColorName,
si.UnitPrice
FROM Warehouse.StockItems si
LEFT JOIN Warehouse.Colors c ON si.ColorID = c.ColorID
SCD Wizard: Connect the Source directly to a Slowly Changing Dimension block.Target Table: [dbo].[Dim_StockItem].Business Key: map StockItemID to StockItemID.Change Types: Map StockItemName, Brand, Size, ColorName, and UnitPrice to Changing attribute. Click Finish.4.4. Loading Dim_Employee & Dim_City (Upsert using SCD Wizard)Repeat the SCD process for the remaining two dimensions.Dim_Employee:Source Query:SELECT
PersonID,
CAST(FullName AS VARCHAR(150)) AS EmployeeName,
CAST(PreferredName AS VARCHAR(50)) AS PreferredName,
IsSalesperson
FROM Application.People
WHERE IsEmployee = 1
SCD Wizard: Target [dbo].[Dim_Employee]. Business Key = PersonID. Changing Attributes = EmployeeName, PreferredName, IsSalesperson.Dim_City:Source Query:SELECT
c.CityID,
CAST(c.CityName AS VARCHAR(50)) AS CityName,
CAST(sp.StateProvinceName AS VARCHAR(50)) AS StateProvinceName,
CAST(sp.SalesTerritory AS VARCHAR(50)) AS SalesTerritory
FROM Application.Cities c
JOIN Application.StateProvinces sp ON c.StateProvinceID = sp.StateProvinceID
SCD Wizard: Target [dbo].[Dim_City]. Business Key = CityID. Changing Attributes = CityName, StateProvinceName, SalesTerritory.5. Phase 4: Loading the Fact Table (Load_Fact_Sales.dtsx)In the Solution Explorer, right-click "SSIS Packages", select New SSIS Package, and rename it to Load_Fact_Sales.dtsx.
This package must match WWI IDs (like CustomerID) to our new Data Warehouse IDs (CustomerSK). We do this using Lookups.Drag a Data Flow Task onto the screen. Rename to Load Fact Table and double-click it.The Source: Drag an OLE DB Source. Double-click it.Connection: Source_WWI. Mode: SQL command.Query:SELECT il.InvoiceLineID, i.InvoiceDate, i.CustomerID, il.StockItemID,
i.SalespersonPersonID, c.DeliveryCityID, il.Quantity, il.ExtendedPrice AS TotalAmount
FROM Sales.InvoiceLines il
JOIN Sales.Invoices i ON il.InvoiceID = i.InvoiceID
JOIN Sales.Customers c ON i.CustomerID = c.CustomerID
Click OK.5.1 Step-by-Step LookupsYou will drag five Lookup blocks onto the screen.Lookup 1 (Date):Drag a Lookup from the toolbox. Connect the blue arrow from your OLE DB Source to this Lookup.Double-click the Lookup.General Tab: Change the dropdown at the bottom from "Fail component" to "Ignore failure" (This prevents the whole process from crashing if one date is missing).Connection Tab: Change connection manager to Dest_DW. Select the table [dbo].[Dim_Date].Columns Tab: \* Click and hold InvoiceDate from the "Available Input Columns" list (left side).Drag it over and drop it onto FullDate in the "Available Lookup Columns" list (right side). A black line will connect them.On the right side, find DateSK and click the small checkbox next to it. (This means "give me this ID").Click OK.Lookup 2 (Customer):Drag another Lookup. Connect the blue arrow from Lookup 1 to Lookup 2.
(A box might pop up asking which output to use. Select Lookup Match Output and click OK).Double-click Lookup 2.General Tab: Set to "Ignore failure".Connection Tab: Select Dest_DW and table [dbo].[Dim_Customer].Columns Tab: Drag CustomerID (left) to CustomerID (right). Check the box for CustomerSK. Click OK.Lookup 3, 4, and 5:
Repeat the exact same process linking the blue arrows from one to the next.Lookup 3 (Stock): Table = Dim_StockItem. Drag StockItemID to StockItemID. Check StockItemSK.Lookup 4 (Employee): Table = Dim_Employee. Drag SalespersonPersonID to PersonID. Check EmployeeSK.Lookup 5 (City): Table = Dim_City. Drag DeliveryCityID to CityID. Check CitySK.5.2 Filtering Existing Facts (Anti-Duplication)Just like our dimensions, if we run this package twice, it will duplicate our sales data! To fix this, we add one final Lookup to check if the invoice line already exists in our Data Warehouse.Drag another Lookup from the toolbox. Let's call it Lookup 6 (Check Duplicates). Connect the blue arrow from Lookup 5 (City) to this new Lookup. (Select "Lookup Match Output" if prompted).Double-click the new Lookup.General Tab: Change the dropdown at the bottom from "Fail component" to "Redirect rows to no match output".Connection Tab: Select Dest_DW and table [dbo].[Fact_Sales].Columns Tab: Drag InvoiceLineID (left) to InvoiceLineID (right). Do not check any boxes. Click OK.Why? This checks the target Fact table. If the InvoiceLineID already exists, SSIS throws it away. If it doesn't exist, it sends the row down the "No Match" path to be inserted!5.3 The Final DestinationDrag an OLE DB Destination to the very bottom (or use the one you already have on screen).Connect the blue arrow from Lookup 6 (Check Duplicates) to the OLE DB Destination. A box will pop up: select Lookup No Match Output and click OK.Double-click the OLE DB Destination.Connection: Dest_DW. Table: [dbo].[Fact_Sales].Click Mappings. Ensure these specific columns match up correctly:Input DateSK -> Destination DateSKInput CustomerSK -> Destination CustomerSKInput StockItemSK -> Destination StockItemSKInput EmployeeSK -> Destination EmployeeSKInput CitySK -> Destination CitySKInput Quantity -> Destination QuantityInput TotalAmount -> Destination TotalAmountInput InvoiceLineID -> Destination InvoiceLineIDClick OK.To run the project: Click the green "Start" button at the top of Visual Studio to run the Dimensions package first, wait for it to finish, stop debugging, and then run the Fact Sales package!6. Phase 5: .NET Application Integration & AnalyticsNow that the Data Warehouse is built, we can integrate it into your existing .NET (C#) application to generate business statistics. Since your application uses the Repository Pattern, the best practice is to keep your operational database and your data warehouse completely isolated using two different connection strings.6.1. Steps to Connect 2 Databases in .NET (Entity Framework Core)Step 1: Update appsettings.json
Add a second connection string for your Data Warehouse next to your existing Operational database connection."ConnectionStrings": {
"OperationalDb": "Server=localhost;Database=WideWorldImporters;Trusted_Connection=True;",
"DataWarehouseDb": "Server=localhost;Database=WWI_DataWarehouse;Trusted_Connection=True;"
}
Step 2: Create a second DbContext
You already have a DbContext for your main app. Create a new one specifically for reporting.public class DWContext : DbContext
{
public DWContext(DbContextOptions<DWContext> options) : base(options) { }

    // Add your DW tables here
    public DbSet<FactSales> Fact_Sales { get; set; }
    public DbSet<DimDate> Dim_Date { get; set; }
    public DbSet<DimCustomer> Dim_Customer { get; set; }
    public DbSet<DimEmployee> Dim_Employee { get; set; }
    public DbSet<DimCity> Dim_City { get; set; }
    public DbSet<DimStockItem> Dim_StockItem { get; set; }

}
Step 3: Register Both Contexts in Program.cs (or Startup.cs)builder.Services.AddDbContext<OperationalContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("OperationalDb")));

builder.Services.AddDbContext<DWContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DataWarehouseDb")));
Step 4: Create an Analytics Repository
Create a new interface IAnalyticsRepository and its implementation. Inject the DWContext into it (not the operational context).public class AnalyticsRepository : IAnalyticsRepository
{
private readonly DWContext \_dwContext;

    public AnalyticsRepository(DWContext dwContext)
    {
        _dwContext = dwContext;
    }

    // Your statistical methods go here!

}
6.2. 10 Business Statistics to Generate from the DWBecause you used a Star Schema, generating these statistics is incredibly fast. Here are 10 KPIs you can add to your AnalyticsRepository. I have provided the SQL logic, which you can easily translate to LINQ or run via Dapper/Raw SQL in your repository.1. Total Revenue by Year (Trend Analysis)Logic: Join Fact_Sales to Dim_Date. Group by Year. Sum TotalAmount.SQL: SELECT d.Year, SUM(f.TotalAmount) AS TotalRevenue FROM Fact_Sales f JOIN Dim_Date d ON f.DateSK = d.DateSK GROUP BY d.Year ORDER BY d.Year2. Top 10 Best-Selling Products (Product Performance)Logic: Join Fact_Sales to Dim_StockItem. Group by StockItemName. Sum Quantity. Order descending.SQL: SELECT TOP 10 s.StockItemName, SUM(f.Quantity) AS TotalSold FROM Fact_Sales f JOIN Dim_StockItem s ON f.StockItemSK = s.StockItemSK GROUP BY s.StockItemName ORDER BY TotalSold DESC3. Salesperson Leaderboard (Employee Performance)Logic: Join Fact_Sales to Dim_Employee. Group by EmployeeName. Sum TotalAmount.SQL: SELECT e.EmployeeName, SUM(f.TotalAmount) AS RevenueGenerated FROM Fact_Sales f JOIN Dim_Employee e ON f.EmployeeSK = e.EmployeeSK GROUP BY e.EmployeeName ORDER BY RevenueGenerated DESC4. Revenue by Sales Territory (Geographical Analysis)Logic: Join Fact_Sales to Dim_City. Group by SalesTerritory. Sum TotalAmount.SQL: SELECT c.SalesTerritory, SUM(f.TotalAmount) AS TotalRevenue FROM Fact_Sales f JOIN Dim_City c ON f.CitySK = c.CitySK GROUP BY c.SalesTerritory ORDER BY TotalRevenue DESC5. Most Valuable Customer Categories (Market Segmentation)Logic: Join Fact_Sales to Dim_Customer. Group by CustomerCategoryName. Sum TotalAmount.6. Peak Sales Day of the Week (Operational Insights)Logic: Join Fact_Sales to Dim_Date. Group by DayOfWeek. Sum Quantity. (This tells you if customers buy more on Mondays vs. Fridays).SQL: SELECT d.DayOfWeek, SUM(f.Quantity) AS ItemsSold FROM Fact_Sales f JOIN Dim_Date d ON f.DateSK = d.DateSK GROUP BY d.DayOfWeek7. Top 5 Customers by Lifetime Value (VIP Identification)Logic: Join Fact_Sales to Dim_Customer. Group by CustomerName. Sum TotalAmount. Order descending limit to 5.8. Product Brand Performance (Inventory Planning)Logic: Join Fact_Sales to Dim_StockItem. Filter out "N/A" brands. Group by Brand. Sum TotalAmount.9. Average Order Value by City (Pricing Strategy)Logic: Join Fact_Sales to Dim_City. Calculate Average (AVG) of TotalAmount grouped by CityName.10. Monthly Revenue Growth (Year-over-Year)Logic: Join Fact_Sales to Dim_Date. Group by Year and MonthName. Sum TotalAmount. This allows your front-end application to draw a beautiful line chart comparing January 2014 vs January 2015.
