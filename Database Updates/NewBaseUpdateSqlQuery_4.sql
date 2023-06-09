USE [VestPlastDatabase]
GO
/****** Object:  StoredProcedure [dbo].[_Excel_GetData]    Script Date: 16.11.2022 19:16:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Excel_GetManufacturingWarehouseData]
(
@type nvarchar(200) = null,
@subType nvarchar(200) = null,
@provider nvarchar(200) = null,
@manufacturer nvarchar(200) = null,
@indexName nvarchar(200) = null,
@id int = null,
@dateStart smalldatetime = null,
@dateFinish smalldatetime = null,
@showEmpty bit,
@expiredMode int = null
)

AS
BEGIN
Declare @EM nvarchar(100)
	BEGIN TRY
		SELECT i.WarehouseId as Id, w.Type as 'Тип', w.SubType as 'Наименование типа', w.[Provider] as 'Поставщик', w.Manufacturer as 'Производитель', i.[Id] as 'Id паспорта', i.[Index] as 'Паспорт',
		i.Leftovers as 'Остатки', i.ManufacturingDate as 'Дата изготовления', i.ExpirationDate as 'Срок годности', i.AutoDate as 'Автоматическая дата'
		FROM ManufacturingIndexes i
		INNER JOIN Warehouse w on w.Id = i.WarehouseId
		Where ((@type is null) or (w.[Type] = @type)) 
			and ((@subType is null) or (w.[SubType] = @subType))
			and ((@provider is null) or (w.[Provider] = @provider))
			and ((@manufacturer is null) or (w.[Manufacturer] = @manufacturer))
			and ((@indexName is null) or (i.[Index] LIKE '%'+@indexName+'%'))
			and ((@id is null) or (w.[Id] = @id))
			and ((@dateStart is null) or (i.[AutoDate] >= @dateStart))
			and ((@dateFinish is null) or (i.[AutoDate] <= @dateFinish))
			and (@showEmpty = 1 AND i.Leftovers >= 0) OR (@showEmpty = 0 AND i.Leftovers > 0)
			and ((@expiredMode is null) OR (@expiredMode = 1 AND i.ExpirationDate >= GETDATE()) OR (@expiredMode = 0 AND i.ExpirationDate >= GETDATE()))
		ORDER BY w.Type ASC
	END TRY
	BEGIN CATCH
		Select @EM = ERROR_MESSAGE();
		Throw 51011, @EM, 1; 
	END CATCH
END


ALTER PROCEDURE [dbo].[_Excel_GetData]
(
@dateStart smalldatetime,
@dateEnd smalldatetime
)

AS
BEGIN
Declare @EM nvarchar(100)
	BEGIN TRY
		SELECT m.Id as Id, w.Type as 'Тип', w.SubType as 'Наименование типа', w.Provider as 'Поставщик', w.Manufacturer as 'Производитель', i.[Index] as 'Паспорт',
		m.Quantity as 'Количество', m.Leftovers as 'Остатки', b.[Name] as 'Номер партии',
		CASE
			WHEN bt.[Name] is null and m.ActionType = 1 THEN 'ТТН'
			ELSE bt.[Name]
		END AS 'Тип партии',
		bl.DisplayName as 'Линия', rd.[Name] as 'Номер документа', m.DocDate as 'Дата', m.Employee as 'Сотрудник', m.IpAddress as 'Ip адрес', m.InsertDate as 'Дата создания',
		m.Remarks as 'Примичание', m.ActionType as 'Тип операции'
		FROM ManufacturingRecords m
		INNER JOIN Warehouse w on w.Id = m.WarehouseId
		FULL JOIN ManufacturingIndexes i on i.Id = m.IndexId
		FULL JOIN ReplenishmentDocuments rd on rd.Id = m.ReplenishmentDocumentId
		FULL JOIN [Batches] b on b.Id = m.BatchId
		FULL JOIN BatchLines bl on bl.Id = b.LineId
		FULL JOIN BatchTypes bt on bt.Id = b.TypeId
		WHERE m.InsertDate <= @dateEnd AND m.InsertDate >= @dateStart and m.Id is not null
		ORDER BY m.Id DESC
	END TRY
	BEGIN CATCH
		Select @EM = ERROR_MESSAGE();
		Throw 51011, @EM, 1; 
	END CATCH
END

ALter Table [Batches]
Add CurrentPackage int NULL

ALter Table [Batches]
Add CurrentQuantity decimal(15, 3) NULL

ALter Table [Batches]
Add CompletionDate smalldatetime NULL


EXEC sp_rename 'BatchHistories.Remarks', 'Remark';

ALter Table [BatchHistories]
Add SoldDate smalldatetime NULL

ALter Table [BatchHistories]
Add LinkedBatches int NULL

ALTER TABLE [dbo].[BatchHistories]  WITH CHECK ADD  CONSTRAINT [FK_BatchHistories_Batches_Linked] FOREIGN KEY([LinkedBatchId])
REFERENCES [dbo].[Batches] ([Id])
GO

ALTER TABLE [dbo].[BatchHistories] CHECK CONSTRAINT [FK_BatchHistories_Batches_Linked]
GO

ALter Table [BatchHistoryActionTypes]
Add CssClass nvarchar(100) null

Update BatchHistoryActionTypes
Set CssClass= 'history-stats-success'
Where Id = 1

Update BatchHistoryActionTypes
Set CssClass= 'history-stats-success'
Where Id = 2

Update BatchHistoryActionTypes
Set [Name] = N'Производство завершено, отправлена на склад'
Where Id = 2

Update BatchHistoryActionTypes
Set CssClass= 'history-stats-success'
Where Id = 3

Update BatchHistoryActionTypes
Set [Name] = N'Возврат от покупателя'
Where Id = 4

Update BatchHistoryActionTypes
Set CssClass= 'history-stats-danger'
Where Id = 4


Update BatchHistoryActionTypes
Set [Name] = N'Добавление в другую партию'
Where Id = 5

Update BatchHistoryActionTypes
Set CssClass= 'history-stats-info'
Where Id = 5

Update BatchHistoryActionTypes
Set [Name] = N'Принятие другой партии'
Where Id = 6

Update BatchHistoryActionTypes
Set CssClass= 'history-stats-info'
Where Id = 6

INSERT INTO [dbo].[BatchHistoryActionTypes]
           ([Name], CssClass
		   )
     VALUES
           (N'Утилизация', 'history-stats-danger')		   
GO

ALter Table [BatchHistoryActionTypes]
Alter column CssClass nvarchar(100) not null

ALter Table [BatchHistoryOperationTypes]
Add CssClass nvarchar(100) null

Update BatchHistoryOperationTypes
Set CssClass= 'history-stats-success'
Where Id = 1

Update BatchHistoryOperationTypes
Set CssClass= 'history-stats-danger'
Where Id = 2

ALter Table [BatchHistoryOperationTypes]
Alter column CssClass nvarchar(100) not null

ALter Table [BatchStatuses]
Add CssClass nvarchar(100) null

Update BatchStatuses
Set CssClass= 'history-stats-init'
Where Id = 1

Update BatchStatuses
Set CssClass= 'history-stats-info'
Where Id = 2

Update BatchStatuses
Set CssClass= 'history-stats-success'
Where Id = 3

Update BatchStatuses
Set CssClass= 'history-stats-danger'
Where Id = 4

ALter Table [BatchStatuses]
Alter column CssClass nvarchar(100) not null

INSERT INTO [dbo].[BatchHistoryOperationTypes]
           ([Name], CssClass
		   )
     VALUES
           (N'Информация', 'history-stats-info')		   
GO

INSERT INTO [dbo].[BatchHistoryActionTypes]
           ([Name], CssClass
		   )
     VALUES
           (N'Нет данных', 'history-stats-init')		   
GO

ALter Table [BatchHistories]
Add LeftPackage int NULL

ALter Table [BatchHistories]
Add LeftQuantity decimal(15, 3) NULL

ALter Table [BatchHistories]
Add ReturnDate smalldatetime NULL