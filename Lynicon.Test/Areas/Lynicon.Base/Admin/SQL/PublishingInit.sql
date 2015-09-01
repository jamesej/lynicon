USE [<Database>]
GO

ALTER TABLE ContentItems ADD
IsPubVersion BIT NOT NULL CONSTRAINT ContentItems_Default_IsLive DEFAULT 0,
PubFrom DATETIME NULL,
PubTo DATETIME NULL,
Published DATETIME NULL
GO
INSERT INTO DbChanges (Change, WhenChanged) VALUES ('LyniconPublishing 0.0', GETDATE())
GO

