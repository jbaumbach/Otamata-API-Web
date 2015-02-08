

/* Reporting */

PRINT 'Device Activity'

select device_id, activity_type_desc, activity_detail, COUNT(*) as cnt 
from tblDeviceActivity DA 
join tblActivityTypeCode ATC 
on DA.activity_type_code = ATC.activity_type_code 
group by device_id, activity_type_desc, activity_detail
order by cnt DESC, device_id 

PRINT 'Inappropriate Activity'

select sound_id, COUNT(*) as cnt
from tblInappropriate
group by sound_id
order by cnt DESC

PRINT 'Purchase Activity'

select download_by, COUNT(*) as cnt
from tblPurchase
group by download_by
order by cnt DESC

select S.sound_id, sound_name, COUNT(*) as cnt
from tblPurchase P
join tblSound S
on P.sound_id = S.sound_id
group by S.sound_id, sound_name
order by cnt DESC

PRINT 'Rating Activity'

select S.sound_id, sound_name, COUNT(*) as cnt
from tblRating R
join tblSound S
on R.sound_id = S.sound_id
group by S.sound_id, sound_name
order by cnt DESC
