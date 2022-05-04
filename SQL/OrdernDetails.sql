USE [TestingProducts]
GO

/****** Object:  Table [dbo].[OrdernDetails]    Script Date: 05.05.2022 00:20:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OrdernDetails](
	[OrderDetail_ID] [int] IDENTITY(1,1) NOT NULL,
	[Product_ID] [int] NOT NULL,
	[Order_ID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal](10, 2) NOT NULL,
 CONSTRAINT [PK_OrdernDetails] PRIMARY KEY CLUSTERED 
(
	[OrderDetail_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OrdernDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrdernDetails_Orders] FOREIGN KEY([Order_ID])
REFERENCES [dbo].[Orders] ([Order_ID])
GO

ALTER TABLE [dbo].[OrdernDetails] CHECK CONSTRAINT [FK_OrdernDetails_Orders]
GO

ALTER TABLE [dbo].[OrdernDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrdernDetails_Products] FOREIGN KEY([Product_ID])
REFERENCES [dbo].[Products] ([Product_ID])
GO

ALTER TABLE [dbo].[OrdernDetails] CHECK CONSTRAINT [FK_OrdernDetails_Products]
GO


