
@echo Search for sound:

curl "http://www.otamata.com:82/services/soundssummary/xml?term=&sort=0&incappr=1" --digest --user iosDeviceUser:0t@maTA-pe1A

@echo
@echo Mark Inappropriate 12

curl http://www.otamata.com:82/services/markinappropriate/xml --digest --user iosDeviceUser:0t@maTA-pe1A --data-ascii "soundId=12"


@echo
@echo RateSound 12

curl http://www.otamata.com:82/services/ratesound/xml --digest --user iosDeviceUser:0t@maTA-pe1A --data-ascii "soundId=12&rating=2&text=Curld rating"


@echo
@echo Record InApp purchase of curl_test_product

curl http://www.otamata.com:82/services/recordpurchase/xml --digest --user iosDeviceUser:0t@maTA-pe1A --data-ascii "purchaseId=curl_test_product&deviceId=curl_test_user&appVersion=0.89"


@echo
@echo Make sure player understands version 1 url

curl "http://www.otamata.com:82/player/cCgZbVa"

@echo
@echo should NOT be a 404 error

curl "http://www.otamata.com:82/player/eNgDbDa"
pause