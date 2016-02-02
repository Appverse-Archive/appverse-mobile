
var testDisplayNumber = 1;
var testMemoryUse = 0; // application(0), storage(1), other(2)
var testMemoryType = 1; // unknown(0), main(1), extended(2)
var testCopyToClipboard = "This is the test you want to copy/paste";

var testLaunchMail = "appmail";
var testLaunchMailParams = "unityversal@gmail.com";
if(Appverse.is.Android) {
	testLaunchMailParams = "additionalPathIfNeeded/?android.intent.extra.EMAIL=[unityversal@gmail.com,jon@example.com]&android.intent.extra.SUBJECT=Email subject&android.intent.extra.TEXT=Email message text";
	testLaunchMailParams = unescape(testLaunchMailParams); // to avoid black spaces, etc.
	
	/* Example of extra intents that could be send to the EMAIL android app
	
	emailIntent.putExtra(Intent.EXTRA_EMAIL, new String[] {"jon@example.com"}); // recipients
	emailIntent.putExtra(Intent.EXTRA_SUBJECT, "Email subject");
	emailIntent.putExtra(Intent.EXTRA_TEXT, "Email message text");
	emailIntent.putExtra(Intent.EXTRA_STREAM, Uri.parse("content://path/to/email/attachment"));
	
	Intent.EXTRA_EMAIL = "android.intent.extra.EMAIL";
	Intent.EXTRA_SUBJECT = "android.intent.extra.SUBJECT";
	Intent.EXTRA_TEXT = "android.intent.extra.TEXT";
	*/
	
}

var testLaunchTelephone = "apptel";
var testLaunchTelephoneParams = "686998899";

var testLaunchMaps = "appmaps";
var testLaunchMapsParams = "maps.apple.com/?daddr=San+Francisco,+CA&saddr=cupertino"; //for ios
if(Appverse.is.Android) {
	testLaunchMapsParams = ""; // launching explicit component name without extras
}

var testLaunchMapsGoogle = "appmaps-google";
var testLaunchMapsGoogleParams = "0,0?q=1600+Amphitheatre+Parkway,+Mountain+View,+California";
if(Appverse.is.iOS) {
	testLaunchMapsGoogleParams = "maps.google.com/maps?q=1%20Infinite Loop,%20Cupertino,%20CA%2095014";
}

//********** UI COMPONENTS

//********** SYSTEM TESTCASES
var TestCase_System_Processor = [Appverse.System,
	[['GetCPUInfo','']]];	

var TestCase_System_Power = [Appverse.System,
	[['GetPowerInfo',''],
	['GetPowerRemainingTime','']]];
	
var TestCase_System_OS = [Appverse.System,
	[['GetApplication','{"param1":' + JSON.stringify(testLaunchMail) + '}'],
	['GetApplications',''],
	['LaunchApplication#mail','{"param1":' +  JSON.stringify(testLaunchMail) + ',"param2":' + JSON.stringify(testLaunchMailParams) + '}'],
	['LaunchApplication#tel','{"param1":' +  JSON.stringify(testLaunchTelephone) + ',"param2":' + JSON.stringify(testLaunchTelephoneParams) + '}'],
	['LaunchApplication#maps','{"param1":' +  JSON.stringify(testLaunchMaps) + ',"param2":' + JSON.stringify(testLaunchMapsParams) + '}'],
	['LaunchApplication#maps-google','{"param1":' +  JSON.stringify(testLaunchMapsGoogle) + ',"param2":' + JSON.stringify(testLaunchMapsGoogleParams) + '}'],
	['GetOSHardwareInfo',''],
	['GetOSInfo',''],
	['GetOSUserAgent',''],
	['DismissApplication','']]];
	
var TestCase_System_Memory = [Appverse.System,
	[['GetMemoryAvailable','{"param1":' +  JSON.stringify(testMemoryUse) + ',"param2":' + JSON.stringify(testMemoryType) + '}'],
	['GetMemoryAvailableTypes',''],
	['GetMemoryStatus','{"param1":' + JSON.stringify(testMemoryType) + '}'],
	['GetMemoryTypes',''],
	['GetMemoryUses','']]];		
	
var TestCase_System_HumanInteraction = [Appverse.System,
	[['CopyToClipboard','{"param1":' +  JSON.stringify(testCopyToClipboard) + '}'],
	['GetInputButtons',''],
	['GetInputGestures',''],
	['GetInputMethods',''],
	['GetInputMethodCurrent',''],
	['GetLocaleCurrent',''],
	['GetLocaleSupported','']]]; //GetInputMethodCurrent,GetInputGestures,GetInputButtons
	
var TestCase_System_Display = [Appverse.System,
	[['GetDisplays',''],
	['GetDisplayInfo','{"param1":' +  JSON.stringify(testDisplayNumber) + '}'],
	['GetOrientation','{"param1":' +  JSON.stringify(testDisplayNumber) + '}'],
	['GetOrientationCurrent',''],
	['GetOrientationSupported',''],
	['LockOrientation','{"param1":true,"param2":2}']]];	
	
//********** HANDLING CALLBACKS

/**
 * Applications should override/implement this method to be aware of being lanched by a third-party application, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.System.LaunchData LaunchData}.
 * <br> @version 4.2
 * @method
 * @param {Appverse.System.LaunchData[]} launchData The launch data received.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 * 
 */
Appverse.OnExternallyLaunched = function(launchData) {
	var stringLog = null;
        if(launchData!=null) {
		console.log("launch data array length: " + launchData.length);
		stringLog = "size:"+ launchData.length;
		for(var i=0; i<launchData.length; i++) {
			console.log(launchData[i].Name + "=" + launchData[i].Value);
			stringLog = stringLog + "<br/>" + launchData[i].Name + "=" + launchData[i].Value ;
		}
		//Showcase.app.getController('Main').toast("Externally Launched", stringLog);
                
	} else {
		
		//Showcase.app.getController('Main').toast("Externally Launched","No launch data received");
                
	}
        Showcase.app.getController('Main').console(feedObj("Appverse.OnExternallyLaunched","Appverse.OnExternallyLaunched",(stringLog?stringLog:"No launch data received")));
};

Appverse.System.onLaunchApplicationResult = function (canOpen, canOpenMessage) {
	Showcase.app.getController('Main').console(feedObj("Appverse.System.onLaunchApplicationResult","Appverse.System.onLaunchApplicationResult",""+canOpen+ " - "+ canOpenMessage));
};

