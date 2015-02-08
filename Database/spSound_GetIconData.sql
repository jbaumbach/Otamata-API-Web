USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spSound_GetIconData]    Script Date: 12/17/2011 13:02:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spSound_GetIconData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spSound_GetIconData]
GO

USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spSound_GetIconData]    Script Date: 12/17/2011 13:02:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ================================================================
-- Author:		John Baumbach
-- Create date: December 2011
-- Description:	Gets icon data for the passed sound id
-- ================================================================
CREATE PROCEDURE [dbo].[spSound_GetIconData] 

	@soundId	INT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		S.sound_id
		, icon_extension
		, icon_md5
		, sound_icon_data
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


grant execute on spSound_GetIconData to OTTAWEB
GO
