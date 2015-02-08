
@echo --=> Search for sound:

curl "http://www.otamata.com/services/soundssummary/xml?term=&sort=0&incappr=1" --digest --user iosDeviceUser:0t@maTA-pe1A

@echo
@echo --=> Mark Inappropriate 12

curl http://www.otamata.com/services/markinappropriate/xml --digest --user iosDeviceUser:0t@maTA-pe1A --data-ascii "soundId=12"


@echo
@echo --=> RateSound 12

curl http://www.otamata.com/services/ratesound/xml --digest --user iosDeviceUser:0t@maTA-pe1A --data-ascii "soundId=12&rating=2&text=Curld rating"


@echo
@echo --=> Record InApp purchase of curl_test_product

curl http://www.otamata.com/services/recordpurchase/xml --digest --user iosDeviceUser:0t@maTA-pe1A --data-ascii "purchaseId=curl_test_product&deviceId=curl_test_user&appVersion=0.89"


@echo
@echo --=> Make sure player understands version 1 url

curl "http://www.otamata.com/player/cCgZbVa"



@echo
@echo
@echo --=> Websearch test should run and return % status up until it's done, then it'll return a bunch of results
REM del "C:\inetpub\otamata\cache\search\anchorman" /Q
REM rmdir "C:\inetpub\otamata\cache\search\anchorman"
curl "http://www.otamata.com/services/websearch/xml?term=anchorman&clientip=192.168.1.2" --digest --user iosDeviceUser:0t@maTA-pe1A



@echo
@echo
curl "http://localhost:82/services/webimagesearch/xml?term=anchorman&clientip=192.168.1.2" --digest --user iosDeviceUser:0t@maTA-pe1A


pause