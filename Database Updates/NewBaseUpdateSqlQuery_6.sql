ALTER PROCEDURE [dbo].[Excel_GetManufacturingWarehouseData]

AS
BEGIN
Declare @EM nvarchar(100)
	BEGIN TRY
		SELECT i.WarehouseId as Id, w.Type as 'Тип', w.SubType as 'Наименование типа', w.[Provider] as 'Поставщик', w.Manufacturer as 'Производитель', i.[Id] as 'Id паспорта', i.[Index] as 'Паспорт',
		i.Leftovers as 'Остатки', i.ManufacturingDate as 'Дата изготовления', i.ExpirationDate as 'Срок годности', i.AutoDate as 'Автоматическая дата'
		FROM ManufacturingIndexes i
		INNER JOIN Warehouse w on w.Id = i.WarehouseId	
		ORDER BY w.Type ASC
	END TRY
	BEGIN CATCH
		Select @EM = ERROR_MESSAGE();
		Throw 51011, @EM, 1; 
	END CATCH
END

----

CREATE PROCEDURE [dbo].[Excel_GetBatchWarehouseExcel]

AS
BEGIN
Declare @EM nvarchar(100)
	BEGIN TRY
		SELECT b.Id as Id, b.[Name] as 'Номер партии', bt.[Name] as 'Тип партии', bl.[Name] as 'Линия', bs.[Name] as 'Статус', b.CurrentQuantity as 'Текущий вес', b.CurrentPackage as 'Текущее количество баулов',
		b.CreateDate as 'Дата создания', b.InsertDate as 'Автоматическая дата'
		FROM [Batches] b
		FULL JOIN BatchLines bl on bl.Id = b.LineId
		FULL JOIN BatchTypes bt on bt.Id = b.TypeId	
		FULL JOIN BatchStatuses bs on bs.Id = b.StatusId
		WHERE b.Id is not null
		ORDER BY b.Id DESC
	END TRY
	BEGIN CATCH
		Select @EM = ERROR_MESSAGE();
		Throw 51011, @EM, 1; 
	END CATCH
END

---

CREATE PROCEDURE [dbo].[Excel_GetDownloadSellingsExcel]

AS
BEGIN
Declare @EM nvarchar(100)
	BEGIN TRY
		SELECT bh.Id as HId, b.[Name] as 'Номер партии', bt.[Name] as 'Тип', c.[Name] as 'Покупатель', bh.Quantity as 'Вес', bh.Package as 'Количетсво баулов', bh.LeftQuantity as 'Вес на текущую дату',
		bh.LeftPackage as 'Количество баулов на текущую дату', bh.SoldDate as 'Дата'
		FROM [BatchHistories] bh
		INNER JOIN [Batches] b on b.Id = bh.BatchId
		INNER JOIN BatchTypes bt on bt.Id = b.TypeId	
		INNER JOIN Customers c on c.Id = bh.CustomerId	
		WHERE b.Id is not null and bh.ActionTypeId = 3 and bh.LeftPackage is not null and bh.LeftQuantity is not null
		ORDER BY bh.Id DESC
	END TRY
	BEGIN CATCH
		Select @EM = ERROR_MESSAGE();
		Throw 51011, @EM, 1; 
	END CATCH
END