USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spPurchase_Insert]    Script Date: 12/17/2011 13:02:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spPurchase_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spPurchase_Insert]
GO

USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spPurchase_Insert]    Script Date: 12/17/2011 13:02:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Baumbach
-- Create date: 1/17/2012
-- Description:	Inserts a sound rating into the database
-- =============================================
CREATE PROCEDURE [dbo].[spPurchase_Insert] 

	@soundId		INT
	, @purchaseBy	VARCHAR(40) = NULL	-- NOTE: this is not in this version

AS
BEGIN
	-- Don't use SET NOCOUNT ON; 
	-- since we want to return the number of rows affected.

    -- Insert statements for procedure here
	INSERT
		tblPurchase
	(
		sound_id
		, download_date
		, download_by
	)
	VALUES
	(
		@soundId
		, GETUTCDATE()
		, @purchaseBy
	)
		
END

GO


grant execute on spPurchase_Insert to OTTAWEB
GO
