USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spSound_Search]    Script Date: 12/17/2011 13:02:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spSound_Search]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spSound_Search]
GO

USE [OttaMatta]
GO

/****** Object:  StoredProcedure [dbo].[spSound_Search]    Script Date: 12/17/2011 13:02:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===========================================================================================================
-- Author:		John Baumbach
-- Create date: 12/1/2011
-- Description:	Search for sounds

-- Modified:	2012/7/26
-- By:			John Baumbach
-- Description:	Added 'sound_upload_by' to search criteria so a search can be conducted by uploader
--				Added pagination support via @pageToReturn parameter, and added "total results" result set 

-- ===========================================================================================================
CREATE PROCEDURE [dbo].[spSound_Search] 

	@term					VARCHAR(10) = ''
	, @order				INT	= 0
	, @includeInappropriate BIT = 0
	, @rowsToReturn			INT = 30
	, @pageToReturn			INT = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @maxInappropriateCount	INT 
	DECLARE @startRowNumber			BIGINT
	
	IF @includeInappropriate = 0
		SET @maxInappropriateCount = 2
	ELSE
		SET @maxInappropriateCount = 99999
		
	SELECT
		S.sound_id
		, sound_name
		, sound_filename
		, sound_description
		, sound_date
		, sound_upload_by
		, ISNULL(sqDC.sound_download_count, 0) as sound_download_count
		, CASE 
			WHEN SI.sound_id IS NULL THEN 0
			ELSE 1
		  END AS has_icon
		, sound_size
		, sqRating.sound_rating
		, ISNULL(sqInappr.inappr_count, 0) as inappr_count
	INTO
		#tempResults
	FROM
		tblSound S
	LEFT OUTER JOIN
		tblSoundIcon SI
	ON
		S.sound_id = SI.sound_id
	LEFT OUTER JOIN
	(
		SELECT 
			sound_id
			, COUNT(*) as sound_download_count
		FROM
			tblPurchase
		GROUP BY
			sound_id
	) sqDC
	ON
		S.sound_id = sqDC.sound_id
	LEFT OUTER JOIN
	(
		SELECT 
			sound_id
			, AVG(CONVERT(float, rating_value)) as sound_rating
		FROM
			tblRating
		GROUP BY
			sound_id
	) sqRating
	ON
		S.sound_id = sqRating.sound_id
	LEFT OUTER JOIN
	(
		SELECT 
			sound_id
			, COUNT(*) as inappr_count
		FROM
			tblInappropriate
		GROUP BY
			sound_id
	) sqInappr
	ON
		S.sound_id = sqInappr.sound_id
	
	WHERE
	(
		sound_description like @term + '%'
	OR
		sound_description like '% ' + @term + '%'
	OR
		sound_name like @term + '%'
	OR
		sound_upload_by like @term
	)
	AND
		sound_disabled_date IS NULL
	AND
		ISNULL(sqInappr.inappr_count, 0) <= @maxInappropriateCount

	-- 
	-- Pagination support.  I think this could
	-- be cleaner (e.g. where clause not repeated in each if-then), but none of the methods 
	-- I tried were great.  This is the least messy.
	--
	SET @startRowNumber = @rowsToReturn * @pageToReturn + 1 -- ROW_NUMBER is 1-based, don't start at 0
	
	IF @order = 1	-- Rating
		SELECT 
			*
		FROM
		(
			SELECT 
				ROW_NUMBER() OVER ( ORDER BY sound_rating DESC ) AS row_num
				, *
			FROM
				#tempResults
		) sub
		WHERE
			row_num >= @startRowNumber
		AND
			row_num < @startRowNumber + @rowsToReturn
	ELSE
	IF @order = 2	-- Downloads
		SELECT 
			*
		FROM
		(
			SELECT
				ROW_NUMBER() OVER ( ORDER BY sound_download_count DESC ) AS row_num
				, *
			FROM
				#tempResults
		) sub
		WHERE
			row_num >= @startRowNumber
		AND
			row_num < @startRowNumber + @rowsToReturn
	ELSE
					-- Date (default, and 0)
		SELECT 
			*
		FROM
		(
			SELECT 
				ROW_NUMBER() OVER ( ORDER BY sound_date DESC ) AS row_num
				, *
			FROM
				#tempResults
		) sub
		WHERE
			row_num >= @startRowNumber
		AND
			row_num < @startRowNumber + @rowsToReturn
	
	
	
	/* Very clean with correct rows, but the results are unordered.  This one has promise perhaps
		if the ordering can be figured out cleanly.
	SELECT 
		*
	FROM
	(
		SELECT 
			CASE 
				WHEN @order = 1 THEN 
					ROW_NUMBER() OVER ( ORDER BY sound_rating DESC ) 
			  	WHEN @order = 2 THEN 
			  		ROW_NUMBER() OVER ( ORDER BY sound_download_count DESC ) 
				ELSE 
					ROW_NUMBER() OVER ( ORDER BY sound_date DESC ) 
			END AS RowNum
			, *
		FROM
			#tempResults
	) sub
	WHERE
		RowNum >= @startRowNumber
	AND
		RowNum < @startRowNumber + @rowsToReturn
	*/
	
	
	
	--
	-- Return the total number of results in the overall (non-page-sized) result set
	--
	SELECT
		COUNT(*) as 'search_total'
	FROM
		#tempResults
		
		
END

GO


grant execute on spSound_Search to OTTAWEB
GO
