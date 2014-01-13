CREATE TABLE Users (Id integer IDENTITY(1,1) PRIMARY KEY, UserName	nvarchar(100)	NOT NULL,Email	nvarchar(100)	NOT NULL,Password	nvarchar(20)	NOT NULL,PasswordQuestion	nvarchar(100)	NULL,PasswordAnswer	nvarchar(100)	NULL,Comment	nvarchar(4000)	NULL,IsApproved	bit	NOT NULL,IsLockedOut		bit	NOT NULL,CreationDate	datetime	NOT NULL,LastLoginDate	datetime	NULL,LastActivityDate	datetime	NULL,LastPasswordChangedDate	datetime	NOT NULL,LastLockoutDate	datetime	NOT NULL);
CREATE TABLE Roles (Id integer IDENTITY(1,1) PRIMARY KEY,Name	nvarchar(100)	NOT NULL );
CREATE TABLE UsersRoles (Id integer IDENTITY(1,1) PRIMARY KEY,UserId integer NOT NULL,RoleId integer NOT NULL);
CREATE TABLE LogEntries (  Id integer IDENTITY(1,1) PRIMARY KEY,  Created datetime,  Severity tinyint,  Category tinyint,  Source nvarchar(4000),  Message nvarchar(4000),  SystemData nvarchar(4000),  UserName nvarchar(20));
CREATE TABLE ConfigSettings (  Id integer IDENTITY(1,1) PRIMARY KEY,  Code nvarchar(100),  Value nvarchar(4000) );
ALTER TABLE LogEntries ADD ProcessId integer,MetricName nvarchar(20),MetricValue nvarchar(4000)
ALTER TABLE ConfigSettings ADD Culture nvarchar(10)
ALTER TABLE Users ADD FailedPasswordAttemptCount Integer, FailedPasswordAttemptWindowStart Datetime, FailedPasswordAnswerAttemptCount Integer, FailedPasswordAnswerAttemptWindowStart DateTime, Identifier nvarchar(36) ;
ALTER TABLE Users ADD IdentifierExpiry DateTime ;
CREATE TABLE UserIdentifiers (Id integer IDENTITY(1,1) PRIMARY KEY,UserId integer NOT NULL,  Identifier nvarchar(36)	NOT NULL, CreationDate datetime, ExpiryDate DateTime, UsedDate datetime  );
CREATE TABLE Identifiers (Id integer IDENTITY(1,1) PRIMARY KEY,UserId integer NOT NULL,  GUID nvarchar(36)	NOT NULL, CreationDate datetime, ExpiryDate DateTime, UsedDate datetime  );
DROP TABLE UserIdentifiers;
ALTER TABLE LogEntries ALTER COLUMN UserName nvarchar(100);
ALTER TABLE LogEntries ALTER COLUMN MetricName nvarchar(100);