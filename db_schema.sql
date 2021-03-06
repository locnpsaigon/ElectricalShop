USE [ElectricalShop]
GO
/****** Object:  Table [dbo].[Brands]    Script Date: 4/22/17 1:41:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brands](
	[BrandId] [int] IDENTITY(1,1) NOT NULL,
	[BrandName] [nvarchar](250) NULL,
	[Description] [nvarchar](1500) NULL,
 CONSTRAINT [PK_Brands] PRIMARY KEY CLUSTERED 
(
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Categories]    Script Date: 4/22/17 1:41:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](150) NULL,
	[Description] [nvarchar](1500) NULL,
	[ParrentId] [int] NULL,
	[SortIndex] [int] NULL CONSTRAINT [DF_Categories_SortIndex]  DEFAULT ((1000)),
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Exports]    Script Date: 4/22/17 1:41:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Exports](
	[ExportId] [int] IDENTITY(1,1) NOT NULL,
	[ExportDate] [datetime] NULL CONSTRAINT [DF_Exports_ExportDate]  DEFAULT (getdate()),
	[ProductId] [int] NULL,
	[Quantity] [int] NULL,
	[ExportPrice] [money] NULL,
	[Note] [nvarchar](250) NULL,
	[CreationDate] [datetime] NULL CONSTRAINT [DF_Exports_CreationDate]  DEFAULT (getdate()),
	[CreationUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_Exports] PRIMARY KEY CLUSTERED 
(
	[ExportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Imports]    Script Date: 4/22/17 1:41:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Imports](
	[ImportId] [int] IDENTITY(1,1) NOT NULL,
	[ImportDate] [datetime] NULL CONSTRAINT [DF_Imports_ImportDate]  DEFAULT (getdate()),
	[ProductId] [int] NULL,
	[Quantity] [int] NULL,
	[ImportPrice] [money] NULL,
	[Note] [nvarchar](250) NULL,
	[CreationDate] [datetime] NULL CONSTRAINT [DF_Imports_CreationDate]  DEFAULT (getdate()),
	[CreationUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_Imports] PRIMARY KEY CLUSTERED 
(
	[ImportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Logs]    Script Date: 4/22/17 1:41:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[LogDate] [datetime] NULL,
	[Action] [nvarchar](50) NULL,
	[Tags] [nvarchar](100) NULL,
	[Message] [nvarchar](150) NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Products]    Script Date: 4/22/17 1:41:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](150) NULL,
	[ProductNameUnsign] [nvarchar](150) NULL,
	[Description] [nvarchar](1000) NULL,
	[DescriptionUnsign] [nvarchar](1000) NULL,
	[BrandId] [int] NULL,
	[CategoryId] [int] NULL,
	[SKU] [nvarchar](50) NULL,
	[QuantityPerUnit] [nvarchar](100) NULL,
	[Price] [money] NULL CONSTRAINT [DF_Products_Price]  DEFAULT ((0)),
	[Discount] [money] NULL CONSTRAINT [DF_Products_Discount]  DEFAULT ((0)),
	[UnitInStock] [int] NULL CONSTRAINT [DF_Products_UnitInStock]  DEFAULT ((0)),
	[CreationDate] [datetime] NULL CONSTRAINT [DF_Products_CreationDate]  DEFAULT (getdate()),
	[CreationUser] [nvarchar](50) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUpdateUser] [nvarchar](50) NULL,
	[Status] [int] NULL CONSTRAINT [DF_Products_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Roles]    Script Date: 4/22/17 1:41:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [int] IDENTITY(10000000,1) NOT NULL,
	[RoleName] [nvarchar](50) NULL,
	[Description] [nvarchar](250) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 4/22/17 1:41:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[UserId] [int] NULL,
	[RoleId] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 4/22/17 1:41:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(10000000,1) NOT NULL,
	[UserName] [nvarchar](50) NULL,
	[Password] [varchar](512) NULL,
	[Salt] [varchar](64) NULL,
	[FullName] [nvarchar](32) NULL,
	[Phone] [nvarchar](64) NULL,
	[Email] [nvarchar](128) NULL,
	[IsActive] [bit] NULL,
	[CreateDate] [datetime] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Logs] ADD  CONSTRAINT [DF_Logs_LogDate]  DEFAULT (getdate()) FOR [LogDate]
GO
