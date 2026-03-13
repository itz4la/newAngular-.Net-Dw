SELECT TOP (1000) [SalesSK]
      ,[InvoiceLineID]
      ,[DateSK]
      ,[CustomerSK]
      ,[StockItemSK]
      ,[EmployeeSK]
      ,[CitySK]
      ,[Quantity]
      ,[TotalAmount]
  FROM [WWI_DataWarehouse].[dbo].[Fact_Sales]


     SELECT COUNT(*)     FROM [WWI_DataWarehouse].[dbo].[Fact_Sales]

     DELETE FROM  [WWI_DataWarehouse].[dbo].[Fact_Sales]