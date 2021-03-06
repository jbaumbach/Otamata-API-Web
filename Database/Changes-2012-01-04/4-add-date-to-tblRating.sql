/*
   Saturday, February 04, 20129:47:13 PM
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
ALTER TABLE dbo.tblRating ADD
	rating_date datetime NOT NULL CONSTRAINT DF_tblRating_rating_date DEFAULT GetUTCDate()
GO
ALTER TABLE dbo.tblRating SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tblRating', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tblRating', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tblRating', 'Object', 'CONTROL') as Contr_Per 