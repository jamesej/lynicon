USE [<Database>]
GO

ALTER TABLE ContentItems ADD
IsDeleted BIT NOT NULL CONSTRAINT ContentItems_Default_IsDeleted DEFAULT 0;
GO
INSERT INTO DbChanges (Change, WhenChanged) VALUES ('LyniconSoftDelete 0.0', GETDATE())
GO

