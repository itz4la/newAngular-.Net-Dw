SELECT TOP (1000) [DateSK]
      ,[FullDate]
      ,[Year]
      ,[Month]
      ,[MonthName]
      ,[Quarter]
      ,[DayOfWeek]
  FROM [WWI_DataWarehouse].[dbo].[Dim_Date]


  SELECT COUNT(*) FROM [WWI_DataWarehouse].[dbo].[Dim_Date];

  DELETE FROM [WWI_DataWarehouse].[dbo].[Dim_Date];