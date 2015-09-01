USE [<Database>]
GO

CREATE TABLE [dbo].[UserActions] (
    [Id]         BIGINT           IDENTITY(1,1)    NOT NULL,
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
    [ActionCode] VARCHAR (50)     NULL,
    [ItemId]     VARCHAR (100)    NULL,
    [Date]       DATETIME         NOT NULL,
    [Data] NVARCHAR(MAX) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO
INSERT INTO DbChanges (Change, WhenChanged) VALUES ('LyniconUserTracking 0.0', GETDATE())
GO

