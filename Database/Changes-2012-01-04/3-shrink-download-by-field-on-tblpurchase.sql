/*
   Saturday, February 04, 20129:35:16 PM
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
ALTER TABLE dbo.tblPurchase
	DROP CONSTRAINT FK_tblSoundDownload_tblSound
GO
ALTER TABLE dbo.tblSound SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblSound', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblSound', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblSound', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tblPurchase
	DROP CONSTRAINT DF_tblSoundDownload_download_date
GO
CREATE TABLE dbo.Tmp_tblPurchase
	(
	download_id bigint NOT NULL IDENTITY (1, 1),
	sound_id int NOT NULL,
	download_date datetime NOT NULL,
	download_by varchar(40) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tblPurchase SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_tblPurchase ADD CONSTRAINT
	DF_tblSoundDownload_download_date DEFAULT (getutcdate()) FOR download_date
GO
SET IDENTITY_INSERT dbo.Tmp_tblPurchase ON
GO
IF EXISTS(SELECT * FROM dbo.tblPurchase)
	 EXEC('INSERT INTO dbo.Tmp_tblPurchase (download_id, sound_id, download_date, download_by)
		SELECT download_id, sound_id, download_date, CONVERT(varchar(40), download_by) FROM dbo.tblPurchase WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tblPurchase OFF
GO
DROP TABLE dbo.tblPurchase
GO
EXECUTE sp_rename N'dbo.Tmp_tblPurchase', N'tblPurchase', 'OBJECT' 
GO
ALTER TABLE dbo.tblPurchase ADD CONSTRAINT
	PK_tblSoundDownload PRIMARY KEY CLUSTERED 
	(
	download_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tblPurchase ADD CONSTRAINT
	FK_tblSoundDownload_tblSound FOREIGN KEY
	(
	sound_id
	) REFERENCES dbo.tblSound
	(
	sound_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblPurchase', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblPurchase', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblPurchase', 'Object', 'CONTROL') as Contr_Per 