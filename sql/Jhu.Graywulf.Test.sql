USE [Graywulf_Test]
GO
/****** Object:  User [graywulf]    Script Date: 11/05/2012 09:02:32 ******/
CREATE USER [graywulf] FOR LOGIN [graywulf] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[TableWithPrimaryKey]    Script Date: 11/05/2012 09:02:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TableWithPrimaryKey](
	[ID] [int] NOT NULL,
	[Data1] [nchar](10) NULL,
	[Data2] [nchar](10) NULL,
	[Flag] [bit] NULL,
 CONSTRAINT [PK_TableWithPrimaryKey] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[spTest]    Script Date: 11/05/2012 09:02:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[spTest]
	@hello nvarchar(50)
AS
	SELECT @@version
GO
/****** Object:  Table [dbo].[SampleData]    Script Date: 11/05/2012 09:02:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SampleData](
	[float] [real] NULL,
	[double] [float] NULL,
	[decimal] [money] NULL,
	[nvarchar(50)] [nvarchar](50) NULL,
	[bigint] [bigint] NULL,
	[int] [int] NOT NULL,
	[tinyint] [tinyint] NULL,
	[smallint] [smallint] NULL,
	[bit] [bit] NULL,
	[ntext] [nvarchar](max) NULL,
	[char] [char](1) NULL,
	[datetime] [datetime] NULL,
	[guid] [uniqueidentifier] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BookAuthor]    Script Date: 11/05/2012 09:02:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookAuthor](
	[BookID] [bigint] NOT NULL,
	[AuthorID] [bigint] NOT NULL,
 CONSTRAINT [PK_BookAuthor] PRIMARY KEY CLUSTERED 
(
	[BookID] ASC,
	[AuthorID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Book]    Script Date: 11/05/2012 09:02:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Book](
	[ID] [bigint] NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Year] [int] NULL,
 CONSTRAINT [PK_Book] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Author]    Script Date: 11/05/2012 09:02:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Author](
	[ID] [bigint] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Author] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[ViewWithStar]    Script Date: 11/05/2012 09:02:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ViewWithStar]
AS
	SELECT * FROM TableWithPrimaryKey
GO
/****** Object:  View [dbo].[ViewOverSameTable]    Script Date: 11/05/2012 09:02:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ViewOverSameTable]
AS
	SELECT a.ID a_id, b.ID b_id
	FROM TableWithPrimaryKey a
	INNER JOIN TableWithPrimaryKey b
		ON a.id = b.id
GO
/****** Object:  View [dbo].[ViewOverPrimaryKey]    Script Date: 11/05/2012 09:02:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ViewOverPrimaryKey] WITH SCHEMABINDING
AS
	SELECT t.ID, t.Flag, t.Data1, t.Data2 FROM dbo.TableWithPrimaryKey t
	WHERE Flag = 1
GO
SET ARITHABORT ON
GO
SET CONCAT_NULL_YIELDS_NULL ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
SET ANSI_PADDING ON
GO
SET ANSI_WARNINGS ON
GO
SET NUMERIC_ROUNDABORT OFF
GO
CREATE UNIQUE CLUSTERED INDEX [PK_ViewOverPrimaryKey] ON [dbo].[ViewOverPrimaryKey] 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[ViewCrossJoinOneTable]    Script Date: 11/05/2012 09:02:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ViewCrossJoinOneTable] WITH SCHEMABINDING
AS
	SELECT a.ID a_id, b.ID b_id
	FROM dbo.TableWithPrimaryKey a
	CROSS JOIN dbo.TableWithPrimaryKey b
GO
/****** Object:  View [dbo].[ViewComputedColumn]    Script Date: 11/05/2012 09:02:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ViewComputedColumn]
AS
	SELECT a.ID + b.ID id
	FROM TableWithPrimaryKey a
	CROSS JOIN TableWithPrimaryKey b
GO
