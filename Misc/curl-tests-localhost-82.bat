
@echo --=> Search for sound:

curl "http://localhost:82/services/soundssummary/xml?term=&sort=0&incappr=1" --digest --user iosDeviceUser:0t@maTA-pe1A

@echo
@echo --=> Mark Inappropriate 12

curl http://localhost:82/services/markinappropriate/xml --digest --user iosDeviceUser:0t@maTA-pe1A --data-ascii "soundId=12"


@echo
@echo --=> RateSound 12

curl http://localhost:82/services/ratesound/xml --digest --user iosDeviceUser:0t@maTA-pe1A --data-ascii "soundId=12&rating=2&text=Curld rating"


@echo
@echo --=> Record InApp purchase of curl_test_product

curl http://localhost:82/services/recordpurchase/xml --digest --user iosDeviceUser:0t@maTA-pe1A --data-ascii "purchaseId=curl_test_product&deviceId=curl_test_user&appVersion=0.89"


@echo
@echo --=> Make sure player understands version 1 url

curl "http://localhost:82/player/cCgZbVa"


@echo
@echo --=> Player url shouldn't have any errors and should create 1 file(s) in /cache/sounds dir


del "C:\Projects\OttaMatta\website-v2\cache\sounds\*.*" /Q
curl "http://localhost:82/player/epgcbha"
dir "C:\Projects\OttaMatta\website-v2\cache\sounds\"



@echo
@echo
@echo --=> Websearch test should run and return % status up until it's done, then it'll return a bunch of results
REM del "C:\Projects\OttaMatta\website-v2\cache\search\anchorman" /Q
REM rmdir "C:\Projects\OttaMatta\website-v2\cache\search\anchorman"
curl "http://localhost:82/services/websearch/xml?term=anchorman&clientip=192.168.1.2" --digest --user iosDeviceUser:0t@maTA-pe1A


@echo
@echo
curl "http://localhost:82/services/webimagesearch/xml?term=anchorman&clientip=192.168.1.2" --digest --user iosDeviceUser:0t@maTA-pe1A

pause