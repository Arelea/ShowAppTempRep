ALTER PROCEDURE [dbo].[Excel_GetManufacturingWarehouseData]

AS
BEGIN
Declare @EM nvarchar(100)
	BEGIN TRY
		SELECT i.WarehouseId as Id, w.Type as '���', w.SubType as '������������ ����', w.[Provider] as '���������', w.Manufacturer as '�������������', i.[Id] as 'Id ��������', i.[Index] as '�������',
		i.Leftovers as '�������', i.ManufacturingDate as '���� ������������', i.ExpirationDate as '���� ��������', i.AutoDate as '�������������� ����'
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
		SELECT b.Id as Id, b.[Name] as '����� ������', bt.[Name] as '��� ������', bl.[Name] as '�����', bs.[Name] as '������', b.CurrentQuantity as '������� ���', b.CurrentPackage as '������� ���������� ������',
		b.CreateDate as '���� ��������', b.InsertDate as '�������������� ����'
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
		SELECT bh.Id as HId, b.[Name] as '����� ������', bt.[Name] as '���', c.[Name] as '����������', bh.Quantity as '���', bh.Package as '���������� ������', bh.LeftQuantity as '��� �� ������� ����',
		bh.LeftPackage as '���������� ������ �� ������� ����', bh.SoldDate as '����'
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