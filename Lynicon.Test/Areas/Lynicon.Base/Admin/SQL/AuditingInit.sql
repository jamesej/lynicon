USE [<Database>]
GO

CREATE TABLE [dbo].[Audits](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[ItemId] [varchar](50) NOT NULL,
	[DataType] [varchar](250) NOT NULL,
	[Version] [varchar](500) NULL,
	[Change] [nvarchar](2000) NULL,
	[ChangeType] [char](1) NOT NULL,
 CONSTRAINT [PK_Audits] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT INTO DbChanges (Change, WhenChanged) VALUES ('LyniconAudit 0.0', GETDATE())
GO

