
@echo Note: copying "*.pdb" as well
robocopy "C:\Projects\OttaMatta\Website" "C:\Users\john\Dropbox\Otamata\WebsiteBuilds" /S /XF web.config *.wav /XD ".svn" 
pause
