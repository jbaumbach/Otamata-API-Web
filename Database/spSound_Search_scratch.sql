--
-- Basic pagination testing
--
exec spSound_Search @term = '', @order = 0, @includeInappropriate = 1	-- order by date - 15 results, 'start fire' to 'quake protect'
exec spSound_Search @term = '', @order = 0, @includeInappropriate = 1, @pageToReturn = 0, @rowsToReturn = 5	-- order by date - 5 results, 'start fire' to 'testing'
exec spSound_Search @term = '', @order = 0, @includeInappropriate = 1, @pageToReturn = 1, @rowsToReturn = 5	-- order by date - 5 results, 'canucks' to 'colbert'
exec spSound_Search @term = '', @order = 0, @includeInappropriate = 1, @pageToReturn = 2, @rowsToReturn = 5	-- order by date - 5 results, 'grapple' to 'quake protect'


exec spSound_Search @term = '', @order = 1, @includeInappropriate = 1	-- order by rating - 15 results, 'hey baby' to 'start fire'
exec spSound_Search @term = '', @order = 1, @includeInappropriate = 1, @pageToReturn = 0, @rowsToReturn = 5	-- order by date - 5 results, 'start fire' to 'grapple'
exec spSound_Search @term = '', @order = 1, @includeInappropriate = 1, @pageToReturn = 1, @rowsToReturn = 5	-- order by date - 5 results, 'butthead' to 'canucks'
exec spSound_Search @term = '', @order = 1, @includeInappropriate = 1, @pageToReturn = 2, @rowsToReturn = 5	-- order by date - 5 results, 'testing' to 'start fire


exec spSound_Search @term = '', @order = 2, @includeInappropriate = 1	-- order by downloads - 15 results, 'quake protect' to 'test 123'
exec spSound_Search @term = '', @order = 2, @includeInappropriate = 1, @pageToReturn = 0, @rowsToReturn = 5	-- order by date - 5 results, 'quake' to 'grapple'
exec spSound_Search @term = '', @order = 2, @includeInappropriate = 1, @pageToReturn = 1, @rowsToReturn = 5	-- order by date - 5 results, 'butthead' to 'yoda'
exec spSound_Search @term = '', @order = 2, @includeInappropriate = 1, @pageToReturn = 2, @rowsToReturn = 5	-- order by date - 5 results, 'testing' to 'test 123'



--
-- Filter tests
--
exec spSound_Search @term = '', @includeInappropriate = 1 -- all, by date

exec spSound_Search @term = 'austin powers', @order = 0, @includeInappropriate = 1	-- 2 results - ok
exec spSound_Search @term = 'lavamantis', @order = 0, @includeInappropriate = 1, @pageToReturn = 0	-- 13 results - ok
exec spSound_Search @term = 'lavamantis', @order = 0, @includeInappropriate = 1, @pageToReturn = 3, @rowsToReturn = 5	-- 2 results - ok
exec spSound_Search @term = 'quake', @order = 2, @includeInappropriate = 1	-- 2 results - ok
