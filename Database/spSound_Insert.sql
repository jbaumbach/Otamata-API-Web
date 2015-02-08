USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spSound_Insert]    Script Date: 12/17/2011 13:02:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spSound_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spSound_Insert]
GO

USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spSound_Insert]    Script Date: 12/17/2011 13:02:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- Author:		John Baumbach
-- Create date: 12/1/2011
-- Description:	Inserts a sound into the database

-- Modified by: John Baumbach
-- Date:		6/6/2012
-- Reason:		Updated result to be the new sound's id
--				Added disabledDate as a parameter
-- =========================================================================
CREATE PROCEDURE [dbo].[spSound_Insert] 

	@name			VARCHAR(25)
	, @fileName		VARCHAR(100)
	, @description	VARCHAR(140)
	, @uploadBy		VARCHAR(50)
	, @soundData	VARBINARY(MAX)
	, @md5			VARCHAR(32)
	, @iconData		VARBINARY(MAX) = NULL
	, @icon_ext		VARCHAR(10) = NULL
	, @icon_md5		VARCHAR(32) = NULL
	, @isEnabled	BIT = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- keep off to return the rows affected accurately -- 
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	DECLARE @soundId		INT
	DECLARE @disabledDate	DATETIME
	
	IF @isEnabled = 0
		SET @disabledDate = GETDATE()
	
	INSERT 
		tblSound
	(
		sound_name
		, sound_filename
		, sound_description
		, sound_date
		, sound_upload_by
		, sound_disabled_date
		, sound_size
	)
	VALUES
	(
		@name
		, @fileName
		, @description
		, GETUTCDATE()
		, @uploadBy
		, @disabledDate
		, LEN(@soundData)
	)
	
	SET @soundId = @@IDENTITY
	
	INSERT
		tblSoundData
	(
		sound_id
		, sound_data
		, sound_md5
	)
	VALUES
	(
		@soundId
		, @soundData
		, @md5
	)
	
	IF @iconData IS NOT NULL 
	
		INSERT
			tblSoundIcon
		(
			sound_id
			, sound_icon_data
			, icon_extension
			, icon_md5
		)
		VALUES
		(
			@soundId
			, @iconData
			, @icon_ext
			, @icon_md5
		)
		
	SELECT @soundId as 'new_sound_id'
	
END

GO


grant execute on spSound_Insert to OTTAWEB
GO
