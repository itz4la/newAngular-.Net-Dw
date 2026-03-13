SELECT TOP (1000) [CitySK]
      ,[CityID]
      ,[CityName]
      ,[StateProvinceName]
      ,[SalesTerritory]
  FROM [WWI_DataWarehouse].[dbo].[Dim_City]


  
   SELECT COUNT(*)     FROM [WWI_DataWarehouse].[dbo].[Dim_City]