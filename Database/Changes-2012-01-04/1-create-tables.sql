/*
   Saturday, February 04, 20126:45:27 PM
   User: 
   Server: (local)
   Database: OttaMatta
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.tblActivityTypeCode
	(
	activity_type_code int NOT NULL,
	activity_type_desc varchar(100) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tblActivityTypeCode ADD CONSTRAINT
	PK_tblActivityTypeCode PRIMARY KEY CLUSTERED 
	(
	activity_type_code
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblActivityTypeCode SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblActivityTypeCode', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblActivityTypeCode', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblActivityTypeCode', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.tblDeviceActivity
	(
	activity_id bigint NOT NULL IDENTITY (1, 1),
	activity_date datetime NOT NULL,
	device_id varchar(40) NOT NULL,
	device_type int NOT NULL,
	app_version varchar(10) NOT NULL,
	activity_type_code int NOT NULL,
	activity_detail varchar(50) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tblDeviceActivity ADD CONSTRAINT
	DF_tblDeviceActivity_activity_date DEFAULT GetUTCDate() FOR activity_date
GO
ALTER TABLE dbo.tblDeviceActivity ADD CONSTRAINT
	DF_tblDeviceActivity_device_type DEFAULT 0 FOR device_type
GO
ALTER TABLE dbo.tblDeviceActivity ADD CONSTRAINT
	PK_tblDeviceActivity PRIMARY KEY CLUSTERED 
	(
	activity_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblDeviceActivity ADD CONSTRAINT
	FK_tblDeviceActivity_tblActivityTypeCode FOREIGN KEY
	(
	activity_type_code
	) REFERENCES dbo.tblActivityTypeCode
	(
	activity_type_code
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tblDeviceActivity SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblDeviceActivity', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblDeviceActivity', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblDeviceActivity', 'Object', 'CONTROL') as Contr_Per 