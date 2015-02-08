USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spSound_GetBinaryData]    Script Date: 12/17/2011 13:02:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spSound_GetBinaryData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spSound_GetBinaryData]
GO

USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spSound_GetBinaryData]    Script Date: 12/17/2011 13:02:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ==========================================================================
-- Author:		John Baumbach
-- Create date: December 2011
-- Description:	Grabs the sound data for the passed sound ID
-- ==========================================================================
CREATE PROCEDURE [dbo].[spSound_GetBinaryData] 

	@soundId	INT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		S.sound_id
		, sound_filename
		, sound_md5
		, sound_data
		, sound_size
	FROM
		tblSound S
	LEFT OUTER JOIN
		tblSoundData SD
	ON
		S.sound_id = SD.sound_id
	WHERE
		S.sound_id = @soundId
END

GO


grant execute on spSound_GetBinaryData to OTTAWEB
GO
