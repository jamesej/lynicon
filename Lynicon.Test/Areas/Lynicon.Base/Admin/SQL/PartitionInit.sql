ALTER TABLE ContentItems ADD
PartitionId INT NULL
GO
INSERT INTO DbChanges (Change, WhenChanged) VALUES ('LyniconPartition 0.0', GETDATE())
GO

