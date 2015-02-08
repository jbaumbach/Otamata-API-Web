USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spRating_Insert]    Script Date: 12/17/2011 13:02:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spRating_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spRating_Insert]
GO

USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spRating_Insert]    Script Date: 12/17/2011 13:02:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Baumbach
-- Create date: 1/17/2012
-- Description:	Inserts a sound rating into the database
-- =============================================
CREATE PROCEDURE [dbo].[spRating_Insert] 

	@soundId		INT
	, @ratingValue	INT
	, @ratingText	VARCHAR(140) = NULL
	, @ratingBy		VARCHAR(50) = NULL	-- NOTE: this is not in this version

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- keep off to return rows affected -- SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT
		tblRating
	(
		sound_id
		, rating_value
		, rating_text
	)
	VALUES
	(
		@soundId
		, @ratingValue
		, @ratingText
	)
		
END

GO


grant execute on spRating_Insert to OTTAWEB
GO
