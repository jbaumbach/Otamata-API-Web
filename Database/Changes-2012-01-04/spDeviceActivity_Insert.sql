USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spDeviceActivity_Insert]    Script Date: 12/17/2011 13:02:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spDeviceActivity_Insert]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spDeviceActivity_Insert]
GO

USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spDeviceActivity_Insert]    Script Date: 12/17/2011 13:02:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Baumbach
-- Create date: 1/17/2012
-- Description:	Inserts a device activity into the database
-- =============================================
CREATE PROCEDURE [dbo].[spDeviceActivity_Insert] 
	@deviceId			VARCHAR(40)
	, @deviceType		INT
	, @appVersion		VARCHAR(10)
	, @activityTypeCode	INT
	, @detail			VARCHAR(50)
	

AS
BEGIN
	-- Don't use SET NOCOUNT ON; 
	-- since we want to return the number of rows affected.

    -- Insert statements for procedure here

	INSERT
		tblDeviceActivity
	(
		device_id
		, device_type
		, app_version
		, activity_type_code
		, activity_detail
	)
	VALUES
	(
		@deviceId
		, @deviceType
		, @appVersion
		, @activityTypeCode
		, @detail
	)
		
END

GO


grant execute on spDeviceActivity_Insert to OTTAWEB
GO
