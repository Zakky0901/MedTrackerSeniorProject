
/* Create MedTrackerDb with required tables and relationships (SSMS 2022 friendly) */
IF DB_ID(N'MedTrackerDb') IS NULL BEGIN CREATE DATABASE MedTrackerDb; END
GO
USE MedTrackerDb; GO

/* Lookup tables */
IF OBJECT_ID(N'dbo.RelationshipTypes', N'U') IS NULL
BEGIN
  CREATE TABLE dbo.RelationshipTypes(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(60) NOT NULL UNIQUE
  );
  INSERT INTO dbo.RelationshipTypes(Name) VALUES (N'Spouse'),(N'Parent'),(N'Child'),(N'Caregiver'),(N'Friend'),(N'Other');
END
GO
IF OBJECT_ID(N'dbo.BloodTypes', N'U') IS NULL
BEGIN
  CREATE TABLE dbo.BloodTypes(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(5) NOT NULL UNIQUE
  );
  INSERT INTO dbo.BloodTypes(Name) VALUES (N'O+'),(N'O-'),(N'A+'),(N'A-'),(N'B+'),(N'B-'),(N'AB+'),(N'AB-');
END
GO

/* Core tables */
IF OBJECT_ID(N'dbo.Medications', N'U') IS NULL
BEGIN
  CREATE TABLE dbo.Medications(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(120) NOT NULL,
    DosageDisplay NVARCHAR(50) NULL,
    Frequency NVARCHAR(20) NULL,
    TimeOfDay NVARCHAR(5) NULL,
    PillsRemaining INT NOT NULL CONSTRAINT DF_Med_Pills DEFAULT(0),
    RefillDate DATE NULL,
    Notes NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Med_Active DEFAULT(1)
  );
  CREATE INDEX IX_Medications_Name ON dbo.Medications(Name);
END
GO
IF OBJECT_ID(N'dbo.Doses', N'U') IS NULL
BEGIN
  CREATE TABLE dbo.Doses(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MedicationId INT NOT NULL,
    [Date] DATE NOT NULL,
    [Time] NVARCHAR(5) NOT NULL,
    Taken BIT NOT NULL CONSTRAINT DF_Dose_Taken DEFAULT(0),
    TakenAt DATETIME NULL,
    CONSTRAINT FK_Dose_Med FOREIGN KEY (MedicationId) REFERENCES dbo.Medications(Id) ON DELETE CASCADE
  );
  CREATE UNIQUE INDEX UX_Dose_Unique ON dbo.Doses(MedicationId,[Date],[Time]);
END
GO
IF OBJECT_ID(N'dbo.AuthorizedUsers', N'U') IS NULL
BEGIN
  CREATE TABLE dbo.AuthorizedUsers(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(120) NOT NULL,
    RelationshipTypeId INT NOT NULL,
    Email NVARCHAR(120) NULL,
    Phone NVARCHAR(40) NULL,
    AddedOn DATE NOT NULL CONSTRAINT DF_Auth_Added DEFAULT(getdate()),
    CONSTRAINT FK_AU_Rel FOREIGN KEY(RelationshipTypeId) REFERENCES dbo.RelationshipTypes(Id)
  );
END
GO
IF OBJECT_ID(N'dbo.EmergencyCards', N'U') IS NULL
BEGIN
  CREATE TABLE dbo.EmergencyCards(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DateOfBirth DATE NULL,
    Last4SSN CHAR(4) NULL,
    BloodTypeId INT NULL,
    InsuranceProvider NVARCHAR(120) NULL,
    InsurancePolicyNumber NVARCHAR(80) NULL,
    CONSTRAINT FK_EC_Blood FOREIGN KEY(BloodTypeId) REFERENCES dbo.BloodTypes(Id)
  );
END
GO
