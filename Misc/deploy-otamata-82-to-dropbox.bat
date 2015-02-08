
@echo Note: copying "*.pdb" as well
robocopy "C:\Projects\OttaMatta\website-v2" "C:\Users\john\Dropbox\Otamata\WebsiteBuilds82" /S /XF web.config *.wav *.mp3 *.svclog *.DS_Store *.php *.pdb /XD ".svn" "search" "webobjects"
pause
