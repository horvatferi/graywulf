--USE [Graywulf_Log]
GO
/****** Object:  Schema [dev]    Script Date: 09/25/2013 17:23:22 ******/
CREATE SCHEMA [dev] AUTHORIZATION [dbo]
GO
/****** Object:  Table [dbo].[EventData]    Script Date: 09/25/2013 17:23:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventData](
	[EventId] [bigint] NOT NULL,
	[Key] [nvarchar](50) NOT NULL,
	[Data] [sql_variant] NOT NULL,
 CONSTRAINT [PK_EventLogData] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC,
	[Key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Event]    Script Date: 09/25/2013 17:23:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Event](
	[EventId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
	[JobGuid] [uniqueidentifier] NOT NULL,
	[ContextGuid] [uniqueidentifier] NOT NULL,
	[EventSource] [int] NOT NULL,
	[EventSeverity] [int] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[EventOrder] [bigint] NOT NULL,
	[ExecutionStatus] [int] NOT NULL,
	[Operation] [nvarchar](255) NOT NULL,
	[EntityGuid] [uniqueidentifier] NOT NULL,
	[EntityGuidFrom] [uniqueidentifier] NOT NULL,
	[EntityGuidTo] [uniqueidentifier] NOT NULL,
	[ExceptionType] [nvarchar](255) NULL,
	[Site] [nvarchar](255) NULL,
	[Message] [nvarchar](1024) NULL,
	[StackTrace] [nvarchar](max) NULL,
 CONSTRAINT [PK_EventLog] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[spGetEventData]    Script Date: 09/25/2013 17:23:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[spGetEventData]
	@EventID bigint
AS
	SELECT EventData.*
	FROM EventData
	WHERE EventData.EventId = @EventID
GO
/****** Object:  StoredProcedure [dbo].[spGetEvent]    Script Date: 09/25/2013 17:23:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[spGetEvent]
	@EventID bigint
AS
	SELECT Event.*
	FROM Event
	WHERE Event.EventId = @EventID
GO
/****** Object:  StoredProcedure [dbo].[spCreateEventData]    Script Date: 09/25/2013 17:23:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[spCreateEventData]
	@EventId bigint,
	@Key nvarchar(50),
	@Data sql_variant
AS
	INSERT [EventData]
		(EventId, [Key], Data)
	VALUES
		(@EventId, @Key, @Data)
GO
/****** Object:  StoredProcedure [dbo].[spCreateEvent]    Script Date: 09/25/2013 17:23:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[spCreateEvent]
	@EventId bigint OUTPUT,
	@UserGuid uniqueidentifier,
	@JobGuid uniqueidentifier,
	@ContextGuid uniqueidentifier,
	@EventSource int,
	@EventSeverity int,
	@EventDateTime datetime,
	@EventOrder bigint,
	@ExecutionStatus int,
	@Operation nvarchar(255),
	@EntityGuid uniqueidentifier,
	@EntityGuidFrom uniqueidentifier,
	@EntityGuidTo uniqueidentifier,
	@ExceptionType nvarchar(255),
	@Site nvarchar(255),
	@Message nvarchar(1024),
	@StackTrace nvarchar(max)
AS
	INSERT Event
		(UserGuid, JobGuid, ContextGuid, EventSource, EventSeverity,
		 EventDateTime, EventOrder, ExecutionStatus, Operation, EntityGuid, EntityGuidFrom, EntityGuidTo,
		 ExceptionType, Site, Message, StackTrace)
	VALUES
		(@UserGuid, @JobGuid, @ContextGuid, @EventSource, @EventSeverity,
		 @EventDateTime, @EventOrder, @ExecutionStatus, @Operation, @EntityGuid, @EntityGuidFrom, @EntityGuidTo,
		 @ExceptionType, @Site, @Message, @StackTrace)

	SET @EventId = @@IDENTITY
GO
/****** Object:  StoredProcedure [dev].[spCleanup]    Script Date: 09/25/2013 17:23:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dev].[spCleanup]
AS
	TRUNCATE TABLE EventData
	TRUNCATE TABLE Event
GO
