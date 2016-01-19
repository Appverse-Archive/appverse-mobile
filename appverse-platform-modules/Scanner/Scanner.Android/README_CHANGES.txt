September 19th 2013 by APPVERSE TEAM
Comented code from the showHelpOnFirstLaunch to avoid an error showing a URL not found error the first time the application is launched and uses the DetectQR functionality.

Support Ticket 0078982
External link: http://stackoverflow.com/questions/17816600/webpage-not-found-file-android-asset-html-nl-index-html-not-found-error-o

October 2015 by APPVERSE TEAM
- Commented any reference to the BeepManager from CaptureActvitiy (due to crash issues; resource not found "beep.org" or compressed) 
- Change in GingerbreadOpenCameraInterface.java class, method open(9, in order to access to both rear and frontal cameras
- Android 6 support:  android.provider.Browser does not inlcude any more some fields commented at: BookmarkPickerActivity.java, AppPickerActivity.java and ShareActivity.java 
(all in the "com.google.zxing.client.android.share" pacakge)


