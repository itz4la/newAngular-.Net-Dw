SELECT TOP (1000) [EmployeeSK]
      ,[PersonID]
      ,[EmployeeName]
      ,[PreferredName]
      ,[IsSalesperson]
  FROM [WWI_DataWarehouse].[dbo].[Dim_Employee]


     SELECT COUNT(*)     FROM [WWI_DataWarehouse].[dbo].[Dim_Employee]