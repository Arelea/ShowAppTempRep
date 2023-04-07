EXEC sp_rename 'Warehouse135.TypeOfMaterial', 'Type';

EXEC sp_rename 'Warehouse135.NameOfTypeMaterial', 'SubType';
EXEC sp_rename 'Warehouse135', 'Warehouse';
EXEC sp_rename 'Warehouse.SubsType', 'Mode';

EXEC sp_rename 'IndexDataLab.Id_Warehouse', 'WarehouseId';
EXEC sp_rename 'IndexData.Id_Warehouse', 'WarehouseId';



ALTER TABLE [dbo].[IndexData]  WITH CHECK ADD  CONSTRAINT [FK_Warehouse_IndexData] FOREIGN KEY([WarehouseId])
REFERENCES [dbo].[Warehouse] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[IndexData] CHECK CONSTRAINT [FK_Warehouse_IndexData]
GO

EXEC sp_rename 'IndexDataLab', 'LabIndexData';

ALTER TABLE [dbo].[LabIndexData]  WITH CHECK ADD  CONSTRAINT [FK_Warehouse_LabIndexData] FOREIGN KEY([WarehouseId])
REFERENCES [dbo].[Warehouse] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[LabIndexData] CHECK CONSTRAINT [FK_Warehouse_LabIndexData]
GO

EXEC sp_rename 'PartyNames', 'ConsignmentTypes';

EXEC sp_rename 'Parties', 'ConsignmentNumbers';
EXEC sp_rename 'PartiesLab', 'LabConsignmentNumbers';

EXEC sp_rename 'LabTable', 'LaboratoryTable';
EXEC sp_rename 'MainRecordTable', 'ManufacturingTable';

ALTER TABLE ManufacturingTable DROP COLUMN Cell;

ALTER TABLE ManufacturingTable 
ADD WarehouseId int NULL

Declare @countAll int
Declare @count int
Declare @countTrue int
Declare @countDelta int
Declare @manId int

Set @count = 0
Set @countTrue = 0
Set @countAll = (Select count(Id) From ManufacturingTable) 

Begin
WHILE @countAll >= @countTrue
begin
        set @countTrue = @countTrue + 1
		Set @manId = (select TOP (1) Id from ManufacturingTable Where Id > @count Order By Id ASC)
		Set @count = @manId

		Update ManufacturingTable
		Set WarehouseId = (select TOP (1) Id 
			from Warehouse 
			Where 
			[Type] = (Select [TypeOfMaterial] From ManufacturingTable Where Id = @manId)
			AND SubType = (Select [NameOfTypeMaterial] From ManufacturingTable Where Id = @manId)
			AND [Provider] = (Select [Provider] From ManufacturingTable Where Id = @manId)
			AND Manufacturer = (Select [Manufacturer] From ManufacturingTable Where Id = @manId)
			AND Mode = 1)
		Where Id = @manId
		end
end

ALTER TABLE [dbo].[ManufacturingTable]  WITH CHECK ADD  CONSTRAINT [FK_Warehouse_ManufacturingTable] FOREIGN KEY([WarehouseId])
REFERENCES [dbo].[Warehouse] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ManufacturingTable] CHECK CONSTRAINT [FK_Warehouse_ManufacturingTable]
GO

ALTER TABLE ManufacturingTable 
ALTER Column WarehouseId int NOT NULL

EXEC sp_rename 'ManufacturingTable.NumberOfDocument', 'DocumentNumber';
EXEC sp_rename 'ManufacturingTable.IpAdress', 'IpAddress';

ALTER TABLE ManufacturingTable 
ADD IndexId int NULL

Declare @IcountAll int
Declare @Icount int
Declare @IcountTrue int
Declare @IcountDelta int
Declare @ImanId int
Declare @IwarId int

Set @Icount = 0
Set @IcountTrue = 0
Set @IcountAll = (Select count(Id) From ManufacturingTable) 

Begin
WHILE @IcountAll >= @IcountTrue
begin
        set @IcountTrue = @IcountTrue + 1
		Set @ImanId = (select TOP (1) Id from ManufacturingTable Where Id > @Icount Order By Id ASC)
		Set @Icount = @ImanId
		Set @IwarId = (select top(1) WarehouseId From ManufacturingTable Where Id = @ImanId)
		Update ManufacturingTable
		Set IndexId = (select TOP (1) Id 
			from IndexData 
			Where 
			[Indexation] = (Select [Indexation] From ManufacturingTable Where Id = @ImanId) And WarehouseId = @IwarId)
		Where Id = @ImanId and IndexId is null
		end
end

Declare @date smalldatetime;
Select @date = GetDate();
Insert into IndexData (Indexation, WarehouseId, AutoDate, Leftovers)
Select DISTINCT m.Indexation, w.Id, @date, 0
From ManufacturingTable as m
Inner join Warehouse as w on w.Id = m.WarehouseId
Where m.IndexId is null

Declare @IIcountAll int
Declare @IIcount int
Declare @IIcountTrue int
Declare @IIcountDelta int
Declare @IImanId int
Declare @IIwarId int

Set @IIcount = 0
Set @IIcountTrue = 0
Set @IIcountAll = (Select count(Id) From ManufacturingTable) 

Begin
WHILE @IIcountAll >= @IIcountTrue
begin
        set @IIcountTrue = @IIcountTrue + 1
		Set @IImanId = (select TOP (1) Id from ManufacturingTable Where Id > @IIcount Order By Id ASC)
		Set @IIcount = @IImanId
		Set @IIwarId = (select top(1) WarehouseId From ManufacturingTable Where Id = @IImanId)

		Update ManufacturingTable
		Set IndexId = (select TOP (1) Id 
			from IndexData 
			Where 
			[Indexation] = (Select [Indexation] From ManufacturingTable Where Id = @IImanId) and WarehouseId = @IIwarId)
		Where Id = @IImanId and IndexId is null
		end
end

ALTER TABLE [dbo].[ManufacturingTable]  WITH CHECK ADD  CONSTRAINT [FK_IndexData_ManufacturingTable] FOREIGN KEY([IndexId])
REFERENCES [dbo].[IndexData] ([Id])
GO

ALTER TABLE [dbo].[ManufacturingTable] CHECK CONSTRAINT [FK_IndexData_ManufacturingTable]
GO

ALTER TABLE ManufacturingTable DROP COLUMN TypeOfMaterial;
ALTER TABLE ManufacturingTable DROP COLUMN NameOfTypeMaterial;
ALTER TABLE ManufacturingTable DROP COLUMN [Provider];
ALTER TABLE ManufacturingTable DROP COLUMN Manufacturer;
ALTER TABLE ManufacturingTable DROP COLUMN Indexation;
ALTER TABLE ManufacturingTable DROP COLUMN Gild;


EXEC sp_rename 'LaboratoryTable', 'LaboratoryRecords';

ALTER TABLE LaboratoryRecords
ADD WarehouseId int NULL

Declare @LabcountAll int
Declare @Labcount int
Declare @LabcountTrue int
Declare @LabcountDelta int
Declare @LabmanId int

Set @Labcount = 0
Set @LabcountTrue = 0
Set @LabcountAll = (Select count(Id) From LaboratoryRecords) 

Begin
WHILE @LabcountAll >= @LabcountTrue
begin
        set @LabcountTrue = @LabcountTrue + 1
		Set @LabmanId = (select TOP (1) Id from LaboratoryRecords Where Id > @Labcount Order By Id ASC)
		Set @Labcount = @LabmanId

		Update LaboratoryRecords
		Set WarehouseId = (select TOP (1) Id 
			from Warehouse 
			Where 
			[Type] = (Select [TypeOfMaterial] From LaboratoryRecords Where Id = @LabmanId)
			AND SubType = (Select [NameOfTypeMaterial] From LaboratoryRecords Where Id = @LabmanId)
			AND [Provider] = (Select [Provider] From LaboratoryRecords Where Id = @LabmanId)
			AND Manufacturer = (Select [Manufacturer] From LaboratoryRecords Where Id = @LabmanId)
			AND Mode = 2)
		Where Id = @LabmanId
		end
end

ALTER TABLE [dbo].[LaboratoryRecords]  WITH CHECK ADD  CONSTRAINT [FK_Warehouse_LaboratoryRecords] FOREIGN KEY([WarehouseId])
REFERENCES [dbo].[Warehouse] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[LaboratoryRecords] CHECK CONSTRAINT [FK_Warehouse_LaboratoryRecords]
GO

ALTER TABLE LaboratoryRecords 
ALTER Column WarehouseId int NOT NULL

EXEC sp_rename 'LaboratoryRecords.NumberOfDocument', 'DocumentNumber';
EXEC sp_rename 'LaboratoryRecords.IpAdress', 'IpAddress';

ALTER TABLE LaboratoryRecords
ADD IndexId int NULL

Declare @LabIcountAll int
Declare @LabIcount int
Declare @LabIcountTrue int
Declare @LabIcountDelta int
Declare @LabImanId int
Declare @LabIwarId int

Set @LabIcount = 0
Set @LabIcountTrue = 0
Set @LabIcountAll = (Select count(Id) From LaboratoryRecords) 

Begin
WHILE @LabIcountAll >= @LabIcountTrue
begin
        set @LabIcountTrue = @LabIcountTrue + 1
		Set @LabImanId = (select TOP (1) Id from LaboratoryRecords Where Id > @LabIcount Order By Id ASC)
		Set @LabIcount = @LabImanId
		Set @LabIwarId = (select top(1) WarehouseId From LaboratoryRecords Where Id = @LabImanId)
		Update LaboratoryRecords
		Set IndexId = (select TOP (1) Id 
			from LabIndexData 
			Where 
			[Indexation] = (Select [Indexation] From LaboratoryRecords Where Id = @LabImanId) and WarehouseId = @LabIwarId)
		Where Id = @LabImanId and IndexId is null
		end
end


ALTER TABLE [dbo].[LaboratoryRecords]  WITH CHECK ADD  CONSTRAINT [FK_LabIndexData_LaboratoryRecords] FOREIGN KEY([IndexId])
REFERENCES [dbo].[LabIndexData] ([Id])
GO

ALTER TABLE [dbo].[LaboratoryRecords] CHECK CONSTRAINT [FK_LabIndexData_LaboratoryRecords]
GO

ALTER TABLE LaboratoryRecords DROP COLUMN TypeOfMaterial;
ALTER TABLE LaboratoryRecords DROP COLUMN NameOfTypeMaterial;
ALTER TABLE LaboratoryRecords DROP COLUMN [Provider];
ALTER TABLE LaboratoryRecords DROP COLUMN Manufacturer;
ALTER TABLE LaboratoryRecords DROP COLUMN Indexation;


EXEC sp_rename 'ConsignmentNumbers.TypeOfAction', 'Action';
EXEC sp_rename 'ConsignmentNumbers', 'ConsNumbers';
EXEC sp_rename 'ConsignmentTypes', 'ConsTypes';
EXEC sp_rename 'LabConsignmentNumbers', 'ConsLabNumbers';

EXEC sp_rename 'ManufacturingTable', 'ManufacturingRecords';

EXEC sp_rename 'IndexData', 'ManufacturingIndexes';

EXEC sp_rename 'LabIndexData', 'LaboratoryIndexes';

EXEC sp_rename 'ManufacturingRecords.OperationType', 'Action';
EXEC sp_rename 'LaboratoryRecords.OperationType', 'Action';

exec('
ALTER PROCEDURE [dbo].[_Excel_GetData]
(
@dateStart smalldatetime,
@dateEnd smalldatetime
)

AS
BEGIN
Declare @EM nvarchar(100)
	BEGIN TRY
		SELECT m.Id as Id, w.Type as ''Тип'', w.SubType as ''Наименование типа'', w.Provider as ''Поставщик'', w.Manufacturer as ''Производитель'',
		m.Quantity as ''Количество'', m.Leftovers as ''Остатки'', m.Document as ''Наименование партии'', m.DocumentNumber as ''Номер партии/документа'',
		i.Indexation as ''Паспорт'', m.Line as ''Линия'', m.DocDate as ''Дата'', m.Employee as ''Сотрудник'', m.IpAddress as ''Ip адрес'', m.AutoDate as ''Авто дата'',
		m.Remarks as ''Примичание'', m.Action as ''Тип операции''
		FROM ManufacturingRecords m
		INNER JOIN Warehouse w on w.Id = m.WarehouseId
		INNER JOIN ManufacturingIndexes i on i.Id = m.IndexId
		WHERE m.AutoDate <= @dateEnd AND m.AutoDate >= @dateStart
		ORDER BY m.Id DESC
	END TRY
	BEGIN CATCH
		Select @EM = ERROR_MESSAGE();
		Throw 51011, @EM, 1; 
	END CATCH
END
')

exec('
ALTER PROCEDURE [dbo].[_Excel_GetDataLab]
(
@dateStart smalldatetime,
@dateEnd smalldatetime
)

AS
BEGIN
Declare @EM nvarchar(100)
	BEGIN TRY
		SELECT m.Id as Id, w.Type as ''Тип'', w.SubType as ''Наименование типа'', w.Provider as ''Поставщик'', w.Manufacturer as ''Производитель'',
		m.Quantity as ''Количество'', m.Leftovers as ''Остатки'', m.Document as ''Наименование партии'', m.DocumentNumber as ''Номер партии'',
		i.Indexation as ''Паспорт'', m.DocDate as ''Дата'', m.Employee as ''Сотрудник'', m.IpAddress as ''Ip адрес'', m.AutoDate as ''Авто дата'',
		m.Remarks as ''Примичание'', m.Action as ''Тип операции''
		FROM LaboratoryRecords m
		INNER JOIN Warehouse w on w.Id = m.WarehouseId
		INNER JOIN LaboratoryIndexes i on i.Id = m.IndexId
		WHERE m.AutoDate <= @dateEnd AND m.AutoDate >= @dateStart
		ORDER BY m.Id DESC
	END TRY
	BEGIN CATCH
		Select @EM = ERROR_MESSAGE();
		Throw 51011, @EM, 1; 
	END CATCH
END
')

CREATE TABLE Comments
(
Id int IDENTITY(1,1) Primary key  not null,
Name    nvarchar(150)  NOT NULL, 
Text     nvarchar(max)  NOT NULL,
AutoDate  smalldatetime         NOT NULL,
Type      int not  NULL,
Status int not  NULL,
)