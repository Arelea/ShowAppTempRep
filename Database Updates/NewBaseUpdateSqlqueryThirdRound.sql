USE [VestPlastDatabase]
GO

/****** Object:  Table [dbo].[ConsNumbers]    Script Date: 28.08.2022 21:47:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DocsNumbers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](35) NOT NULL,
	[AutoDate] [smalldatetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Lines](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[DisplayName] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

Alter table ManufacturingRecords
Add DocNumberId int null

ALTER TABLE [dbo].[ManufacturingRecords]  WITH CHECK ADD  CONSTRAINT [FK_ManufacturingRecords_DocsNumbers] FOREIGN KEY([DocNumberId])
REFERENCES [dbo].[DocsNumbers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ManufacturingRecords] CHECK CONSTRAINT [FK_ManufacturingRecords_DocsNumbers]
GO

Declare @date smalldatetime;
Select @date = GetDate();
Insert into DocsNumbers (Name, AutoDate)
Select DISTINCT DocumentNumber, @date
From ManufacturingRecords
Where Action = 1

Declare @countAll int
Declare @count int
Declare @countTrue int
Declare @countDelta int
Declare @manId int

Set @count = 0
Set @countTrue = 0
Set @countAll = (Select count(Id) From ManufacturingRecords) 

Begin
WHILE @countAll >= @countTrue
begin
        set @countTrue = @countTrue + 1
		Set @manId = (select TOP (1) Id from ManufacturingRecords Where Action = 1 and Id > @count Order By Id ASC)
		Set @count = @manId

		Update ManufacturingRecords
		Set DocNumberId = (select TOP (1) Id 
			from DocsNumbers
			Where 
			[Name] = (Select [DocumentNumber] From ManufacturingRecords Where Id = @manId))
		Where Id = @manId and Action = 1
		end
end

Delete ConsNumbers
Where Action = 1

ALTER Table ConsNumbers
Drop Column Action

ALter Table ConsNumbers
Add TypeId int null

ALter Table ConsNumbers
Add LineId int null

Alter table ManufacturingRecords
Add ConsNumberId int null

ALTER TABLE [dbo].[ManufacturingRecords]  WITH CHECK ADD  CONSTRAINT [FK_ManufacturingRecords_ConsNumbers] FOREIGN KEY([ConsNumberId])
REFERENCES [dbo].[ConsNumbers] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ManufacturingRecords] CHECK CONSTRAINT [FK_ManufacturingRecords_ConsNumbers]
GO

ALTER TABLE [dbo].[ConsNumbers]  WITH CHECK ADD  CONSTRAINT [FK_ConsNumbers_ConsTypes] FOREIGN KEY([TypeId])
REFERENCES [dbo].[ConsTypes] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ConsNumbers] CHECK CONSTRAINT [FK_ConsNumbers_ConsTypes]
GO

ALTER TABLE [dbo].[ConsNumbers]  WITH CHECK ADD  CONSTRAINT [FK_ConsNumbers_Lines] FOREIGN KEY([LineId])
REFERENCES [dbo].[Lines] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ConsNumbers] CHECK CONSTRAINT [FK_ConsNumbers_Lines]
GO

Declare @countAll2 int
Declare @count2 int
Declare @countTrue2 int
Declare @countDelta2 int
Declare @manId2 int

Set @count2 = 0
Set @countTrue2 = 0
Set @countAll2 = (Select count(Id) From ManufacturingRecords) 

Begin
WHILE @countAll2 >= @countTrue2
begin
        set @countTrue2 = @countTrue2 + 1
		Set @manId2 = (select TOP (1) Id from ManufacturingRecords Where Action in (2,3) and Id > @count2 Order By Id ASC)
		Set @count2 = @manId2

		Update ManufacturingRecords
		Set ConsNumberId = (select TOP (1) Id 
			from ConsNumbers
			Where 
			[Name] = (Select [DocumentNumber] From ManufacturingRecords Where Id = @manId2))
		Where Id = @manId2 and Action in (2,3)
		end
end

Declare @countAll333 int
Declare @count333 int
Declare @countTrue333 int
Declare @countDelta333 int
Declare @manId333 int

Set @count333 = 0
Set @countTrue333 = 0
Set @countAll333 = (Select count(Id) From ConsNumbers) 

Begin
WHILE @countAll333 >= @countTrue333
begin
        set @countTrue333 = @countTrue333 + 1
		Set @manId333 = (select TOP (1) Id from ConsNumbers Where Id > @count333 Order By Id ASC)
		Set @count333 = @manId333

		Update ConsNumbers
		Set TypeId = (select TOP (1) Id 
			from ConsTypes
			Where 
			[Name] = (Select TOP (1) [Document] From ManufacturingRecords Where ConsNumberId = @manId333 Group By Document Order by Count([Document]) desc ))
		Where Id = @manId333
		end
end

INSERT INTO [dbo].[Lines]
           ([Name]
           ,[DisplayName])
     VALUES
           ('Bolshevik',
           N'Bolshevik')		   
GO

INSERT INTO [dbo].[Lines]
           ([Name]
           ,[DisplayName])
     VALUES
           ('Xinda',
           'Xinda1')		   
GO

INSERT INTO [dbo].[Lines]
           ([Name]
           ,[DisplayName])
     VALUES
           ('Biersdorff',
           'Biersdorff')		   
GO

INSERT INTO [dbo].[Lines]
           ([Name]
           ,[DisplayName])
     VALUES
           ('Bandera',
           'BANDERA')		   
GO

INSERT INTO [dbo].[Lines]
           ([Name]
           ,[DisplayName])
     VALUES
           ('Maris',
           'MARIS')		   
GO

INSERT INTO [dbo].[Lines]
           ([Name]
           ,[DisplayName])
     VALUES
           ('SyntesLab',
           N'Лаборатория синтеза')		   
GO

INSERT INTO [dbo].[Lines]
           ([Name]
           ,[DisplayName])
     VALUES
           ('Xinda2',
           'Xinda2')		   
GO

Declare @countAll4444 int
Declare @count4444 int
Declare @countTrue4444 int
Declare @countDelta4444 int
Declare @manId4444 int

Set @count4444 = 0
Set @countTrue4444 = 0
Set @countAll4444 = (Select count(Id) From ConsNumbers) 

Begin
WHILE @countAll4444 >= @countTrue4444
begin
        set @countTrue4444 = @countTrue4444 + 1
		Set @manId4444 = (select TOP (1) Id from ConsNumbers Where Id > @count4444 Order By Id ASC)
		Set @count4444 = @manId4444

		Update ConsNumbers
		Set LineId = (select TOP (1) Id 
			from Lines
			Where 
			[DisplayName] = (Select TOP (1) [Line] From ManufacturingRecords Where ConsNumberId = @manId4444 Group By Line Order by Count([Line]) desc ))
		Where Id = @manId4444
		end
end

ALTER Table ManufacturingRecords
Drop Column Document

ALTER Table ManufacturingRecords
Drop Column DocumentNumber

ALTER Table ManufacturingRecords
Drop Column Line

ALter Table ConsNumbers
Add StatusId int null



CREATE TABLE [dbo].[ConsNumberStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](35) NOT NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[ConsNumberStatuses]
           ([Name])
     VALUES
           (N'Устаревшая')		   
GO

INSERT INTO [dbo].[ConsNumberStatuses]
           ([Name])
     VALUES
           (N'Производится')		   
GO

INSERT INTO [dbo].[ConsNumberStatuses]
           ([Name])
     VALUES
           (N'Отгружена')		   
GO

ALTER TABLE [dbo].[ConsNumbers]  WITH CHECK ADD  CONSTRAINT [FK_ConsNumbers_ConsNumberStatuses] FOREIGN KEY([StatusId])
REFERENCES [dbo].[ConsNumberStatuses] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ConsNumbers] CHECK CONSTRAINT [FK_ConsNumbers_ConsNumberStatuses]
GO

EXEC sp_rename 'ConsNumbers.AutoDate', 'InsertDate';

EXEC sp_rename 'ConsTypes', 'ConsNumberTypes';

EXEC sp_rename 'Lines', 'ConsNumberLines';

EXEC sp_rename 'ConsNumbers', 'Batches';

EXEC sp_rename 'ConsNumberTypes', 'BatchTypes';

EXEC sp_rename 'ConsNumberLines', 'BatchLines';

EXEC sp_rename 'ManufacturingRecords.ConsNumberId', 'BatchId';

EXEC sp_rename 'ConsNumberStatuses', 'BatchStatuses';

EXEC sp_rename 'DocsNumbers', 'ReplenishmentDocuments';

EXEC sp_rename 'ManufacturingRecords.DocNumberId', 'ReplenishmentDocumentId';

EXEC sp_rename 'BatchTypes.AutoDate', 'InsertDate';

EXEC sp_rename 'ManufacturingRecords.AutoDate', 'InsertDate';

EXEC sp_rename 'ManufacturingRecords.Action', 'ActionType';

EXEC sp_rename 'ReplenishmentDocuments.AutoDate', 'InsertDate';

EXEC sp_rename 'Comments.AutoDate', 'InsertDate';

EXEC sp_rename 'ManufacturingIndexes.Indexation', 'Index';

ALter Table [Batches]
ALTER COLUMN LineId int not null

ALter Table [Batches]
ALTER COLUMN StatusId int not null

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
		m.Quantity as 'Количество', m.Leftovers as 'Остатки', b.[Name] as 'Номер партии', bt.[Name] as 'Тип партии',
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

ALTER TABLE BatchTypes
ADD CONSTRAINT UC_BatchTypes_Name UNIQUE (Name);

ALTER TABLE BatchLines
ADD CONSTRAINT UC_BatchLines_Name UNIQUE (Name);

ALTER TABLE [Batches]
ADD CONSTRAINT UC_Batches_Name UNIQUE (Name);

INSERT INTO [dbo].[BatchStatuses]
           ([Name])
     VALUES
           (N'Готова')		   
GO


CREATE TABLE [dbo].[BatchesHistory](
	Id int IDENTITY(1,1) NOT NULL,
	BatchId int NOT NULL,
	Text nvarchar(3000) NOT NULL,
	OperationTypeId int NOT NULL,
	ActionTypeId int NOT NULL,
	InsertDate smalldatetime NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sp_rename 'BatchesHistory', 'BatchHistories';

ALTER TABLE [dbo].[BatchHistories]  WITH CHECK ADD  CONSTRAINT [FK_BatchHistories_Batches] FOREIGN KEY([BatchId])
REFERENCES [dbo].[Batches] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BatchHistories] CHECK CONSTRAINT [FK_BatchHistories_Batches]
GO


--CREATE TABLE [dbo].[AspNetUsers](
--	[Id] [nvarchar](450) NOT NULL,
--	[Login] [nvarchar](max) NULL,
--	[FirstName] [nvarchar](max) NULL,
--	[FamilyName] [nvarchar](max) NULL,
--	[UserName] [nvarchar](256) NULL,
--	[NormalizedUserName] [nvarchar](256) NULL,
--	[Email] [nvarchar](256) NULL,
--	[NormalizedEmail] [nvarchar](256) NULL,
--	[EmailConfirmed] [bit] NOT NULL,
--	[PasswordHash] [nvarchar](max) NULL,
--	[SecurityStamp] [nvarchar](max) NULL,
--	[ConcurrencyStamp] [nvarchar](max) NULL,
--	[PhoneNumber] [nvarchar](max) NULL,
--	[PhoneNumberConfirmed] [bit] NOT NULL,
--	[TwoFactorEnabled] [bit] NOT NULL,
--	[LockoutEnd] [datetimeoffset](7) NULL,
--	[LockoutEnabled] [bit] NOT NULL,
--	[AccessFailedCount] [int] NOT NULL,
-- CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
--GO

--CREATE TABLE [dbo].[AspNetUserTokens](
--	[UserId] [nvarchar](450) NOT NULL,
--	[LoginProvider] [nvarchar](450) NOT NULL,
--	[Name] [nvarchar](450) NOT NULL,
--	[Value] [nvarchar](max) NULL,
-- CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
--(
--	[UserId] ASC,
--	[LoginProvider] ASC,
--	[Name] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
--GO

--ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--ON DELETE CASCADE
--GO

--ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
--GO

--CREATE TABLE [dbo].[AspNetUserLogins](
--	[LoginProvider] [nvarchar](450) NOT NULL,
--	[ProviderKey] [nvarchar](450) NOT NULL,
--	[ProviderDisplayName] [nvarchar](max) NULL,
--	[UserId] [nvarchar](450) NOT NULL,
-- CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
--(
--	[LoginProvider] ASC,
--	[ProviderKey] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
--GO

--ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--ON DELETE CASCADE
--GO

--ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
--GO

--CREATE TABLE [dbo].[AspNetRoles](
--	[Id] [nvarchar](450) NOT NULL,
--	[Name] [nvarchar](256) NULL,
--	[NormalizedName] [nvarchar](256) NULL,
--	[ConcurrencyStamp] [nvarchar](max) NULL,
-- CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
--GO

--CREATE TABLE [dbo].[AspNetUserClaims](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[UserId] [nvarchar](450) NOT NULL,
--	[ClaimType] [nvarchar](max) NULL,
--	[ClaimValue] [nvarchar](max) NULL,
-- CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
--GO

--ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--ON DELETE CASCADE
--GO

--ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
--GO

--CREATE TABLE [dbo].[AspNetRoleClaims](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[RoleId] [nvarchar](450) NOT NULL,
--	[ClaimType] [nvarchar](max) NULL,
--	[ClaimValue] [nvarchar](max) NULL,
-- CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
--GO

--ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
--REFERENCES [dbo].[AspNetRoles] ([Id])
--ON DELETE CASCADE
--GO

--ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
--GO

--CREATE TABLE [dbo].[AspNetUserRoles](
--	[UserId] [nvarchar](450) NOT NULL,
--	[RoleId] [nvarchar](450) NOT NULL,
-- CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
--(
--	[UserId] ASC,
--	[RoleId] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
--) ON [PRIMARY]
--GO

--ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
--REFERENCES [dbo].[AspNetRoles] ([Id])
--ON DELETE CASCADE
--GO

--ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
--GO

--ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--ON DELETE CASCADE
--GO

--ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
--GO

--INSERT INTO VestPlastDatabase.dbo.AspNetRoles
--SELECT * FROM UsersAppDB.dbo.AspNetRoles

--INSERT INTO VestPlastDatabase.dbo.AspNetUsers
--SELECT * FROM UsersAppDB.dbo.AspNetUsers

--INSERT INTO VestPlastDatabase.dbo.AspNetUserTokens
--SELECT * FROM UsersAppDB.dbo.AspNetUserTokens


--INSERT INTO VestPlastDatabase.dbo.AspNetUserRoles
--SELECT * FROM UsersAppDB.dbo.AspNetUserRoles


--INSERT INTO VestPlastDatabase.dbo.AspNetUserLogins
--SELECT * FROM UsersAppDB.dbo.AspNetUserLogins

CREATE TABLE [dbo].[Customers](
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(300) Not null,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

 Alter Table BatchHistories
  Add CustomerId int NULL

  Alter Table BatchHistories
  Add Package int NULL

  Alter Table BatchHistories
  Add Quantity decimal(15, 3) NULL

  Alter Table BatchHistories
  Add Remarks nvarchar(500) NULL

ALTER TABLE [dbo].[BatchHistories]  WITH CHECK ADD  CONSTRAINT [FK_BatchHistories_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
GO

ALTER TABLE [dbo].[BatchHistories] CHECK CONSTRAINT [FK_BatchHistories_Customers]
GO

ALter Table [Batches]
Add InitialPackage int NULL

ALter Table [Batches]
Add InitialQuantity decimal(15, 3) NULL

ALter Table [Batches]
Add LastUpdateDate smalldatetime NULL

ALter Table [Batches]
Add CreateDate smalldatetime NULL

Update [Batches]
Set LastUpdateDate = GetDate()

ALter Table [Batches]
Alter column LastUpdateDate smalldatetime NOt NULL


CREATE TABLE [dbo].[BatchHistoryActionTypes](
	Id int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(100) Not null,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[BatchHistoryOperationTypes](
	Id int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(100) Not null,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BatchHistories]  WITH CHECK ADD  CONSTRAINT [FK_BatchHistories_BatchHistoryOperationTypes] FOREIGN KEY([OperationTypeId])
REFERENCES [dbo].[BatchHistoryOperationTypes] ([Id])
GO

ALTER TABLE [dbo].[BatchHistories] CHECK CONSTRAINT [FK_BatchHistories_BatchHistoryOperationTypes]
GO

ALTER TABLE [dbo].[BatchHistories]  WITH CHECK ADD  CONSTRAINT [FK_BatchHistories_BatchHistoryActionTypes] FOREIGN KEY([ActionTypeId])
REFERENCES [dbo].[BatchHistoryActionTypes] ([Id])
GO

ALTER TABLE [dbo].[BatchHistories] CHECK CONSTRAINT [FK_BatchHistories_BatchHistoryActionTypes]
GO

INSERT INTO [dbo].[BatchHistoryOperationTypes]
           ([Name])
     VALUES
           (N'Действие')		   
GO

INSERT INTO [dbo].[BatchHistoryOperationTypes]
           ([Name])
     VALUES
           (N'Редактирование')		   
GO

INSERT INTO [dbo].[BatchHistoryActionTypes]
           ([Name])
     VALUES
           (N'Создание')		   
GO

INSERT INTO [dbo].[BatchHistoryActionTypes]
           ([Name])
     VALUES
           (N'Поступление на склад товаров')		   
GO

INSERT INTO [dbo].[BatchHistoryActionTypes]
           ([Name])
     VALUES
           (N'Отгрузка')		   
GO

INSERT INTO [dbo].[BatchHistoryActionTypes]
           ([Name])
     VALUES
           (N'Возврат на склад сырья')		   
GO

INSERT INTO [dbo].[BatchHistoryActionTypes]
           ([Name])
     VALUES
           (N'Возврат от покупателя')		   
GO

INSERT INTO [dbo].[BatchHistoryActionTypes]
           ([Name])
     VALUES
           (N'Слитие с новой партией')		   
GO


ALTER TABLE [Customers]
ADD CONSTRAINT UC_Customers_Name UNIQUE (Name);

Update BatchStatuses
  Set [Name] = N'Нет данных'
  Where Id = 1

  Update BatchStatuses
  Set [Name] = N'В процессе производства'
  Where Id = 2

  Update BatchStatuses
  Set [Name] = N'На складе'
  Where Id = 3

  Update BatchStatuses
  Set [Name] = N'Израсходована'
  Where Id = 4


   Alter Table ManufacturingIndexes
  Add ManufacturingDate smalldatetime NULL

  Alter Table ManufacturingIndexes
  Add ExpirationDate smalldatetime NULL



   

