USE [<Database>]
GO

ALTER TABLE ContentItems ADD
Layer INT NOT NULL CONSTRAINT ContentItems_Default_Layer DEFAULT 0,
IsLive BIT NOT NULL CONSTRAINT ContentItems_Default_IsLive DEFAULT 0
GO
UPDATE ContentItems SET IsLive = 1
GO
CREATE TABLE [dbo].[Layers](
	[Level] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IsLive] [bit] NOT NULL,
 CONSTRAINT [PK_Layers] PRIMARY KEY CLUSTERED 
(
	[Layer] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT INTO Layers ([Level], Name, IsLive) VALUES (0, 'Baseline', 1)
GO
CREATE TABLE [dbo].[LayerTransactions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Level] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[Type] [varchar](20) NOT NULL,
	[Comment] [varchar](1000) NULL,
 CONSTRAINT [PK_LayerTransactions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT INTO LayerTransactions ([Level], [Date], [Type]) VALUES (0, getdate(), 'CREATE')
GO
ALTER TABLE Users ADD
CurrentLevel INT NULL,
NewLayerMinOffset INT NULL,
NewLayerMaxOffset INT NULL
GO
INSERT INTO DbChanges (Change, WhenChanged) VALUES ('LyniconWorkflow 0.0', GETDATE())
GO

