USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spInappropriate_Insert]    Script Date: 12/17/2011 13:02:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spInappropriate_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spInappropriate_Insert]
GO

USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spInappropriate_Insert]    Script Date: 12/17/2011 13:02:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Baumbach
-- Create date: 1/17/2012
-- Description:	Inserts a sound rating into the database
-- =============================================
CREATE PROCEDURE [dbo].[spInappropriate_Insert] 

	@soundId			INT
	, @InappropriateBy	VARCHAR(50) = NULL	-- NOTE: this is not in this version

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- Turn off to return rows affected to caller - SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT
		tblInappropriate
	(
		sound_id
		, inappropriate_date
	)
	VALUES
	(
		@soundId
		, GETUTCDATE()
	)
		
END

GO


grant execute on spInappropriate_Insert to OTTAWEB
GO
