USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spSound_GetDetails]    Script Date: 12/17/2011 13:02:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spSound_GetDetails]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spSound_GetDetails]
GO

USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spSound_GetDetails]    Script Date: 12/17/2011 13:02:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ================================================================
-- Author:		John Baumbach
-- Create date: 4/14/2012
-- Description:	Gets sound details

-- Updated:		4/27/2012
-- By:			John Baumbach
-- Description: Added "has_icon" column
-- ================================================================
CREATE PROCEDURE [dbo].[spSound_GetDetails] 

	@soundId	INT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		S.sound_id
		, sound_name
		, sound_filename
		, sound_description
		, sound_date
		, sound_upload_by
		, sound_disabled_date
		, sound_size
		, CASE 
			WHEN SI.sound_id IS NULL THEN 0
			ELSE 1
		  END AS has_icon
	FROM
		tblSound S
	LEFT OUTER JOIN
		tblSoundIcon SI
	ON
		S.sound_id = SI.sound_id
	WHERE
		S.sound_id = @soundId
END

GO


grant execute on spSound_GetDetails to OTTAWEB
GO
