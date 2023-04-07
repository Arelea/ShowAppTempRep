
CREATE TABLE [dbo].[ChildBatches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchId] [int] NOT NULL,
	[ChildBatchId] [int] NOT NULL,
	[Date] [smalldatetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ChildBatches]  WITH CHECK ADD  CONSTRAINT [FK_ChildBatches_Batches_Id] FOREIGN KEY([BatchId])
REFERENCES [dbo].[Batches] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ChildBatches] CHECK CONSTRAINT [FK_ChildBatches_Batches_Id]
GO

ALTER TABLE [dbo].[ChildBatches]  WITH CHECK ADD  CONSTRAINT [FK_ChildBatches_Batches_ChildId] FOREIGN KEY([ChildBatchId])
REFERENCES [dbo].[Batches] ([Id])
GO

ALTER TABLE [dbo].[ChildBatches] CHECK CONSTRAINT [FK_ChildBatches_Batches_ChildId]
GO
