USE [<Database>]
GO

ALTER TABLE ContentItems ADD
Transferred DATETIME NULL,
UserTransferred UNIQUEIDENTIFIER NULL
GO
INSERT INTO DbChanges (Change, WhenChanged) VALUES ('LyniconTransfer 0.0', GETDATE())
GO

