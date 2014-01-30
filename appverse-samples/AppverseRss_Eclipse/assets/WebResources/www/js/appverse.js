/*
 * Javascript Classes/Interfaces related here are used to communicate HTML/Javascript files with Appverse Platform.
 */

/**
 * @class Appverse
 * This is the global APPVERSE interface class. 
 * <br>This interface gives singleton access to all Appverse Javascript Interfaces (quick access to already instantiated Appverse classes).
 * <br> @version 1.0
 * @author Marga Parets maps@gft.com
 * @singleton
 * @constructor
 * @return {Appverse} A new Appverse interface.
 */ 
Appverse = new function() {
	// javadoc utility to show singleton classes.
};

Appverse={
	version:"4.3",
	
	/**
	 * Boolean to indicate if the next request send to the platform (using any Appverse.<API_serviceName>.<API_serviceMethod>() call) should unscape or not the data send.
	 * <br>By default platform will unscape any data send. In some cases, scaped characters (for example, the %20 encoded characters in a URL) need to arrive to the service without being unscaped.
	 * <br>Setting this value to false, will send the next platform without replacing escaped characters; you should change value to false if time needed, because the parameter will change to true after making the call.
	 * <pre>Default value: true.</pre>
	 * <br> @version 4.0
	 * @type Boolean
	 */
	unescapeNextRequestData: true, 
	
	/**
     * Initialization function
     */
	init : function() {
		this.is = post_to_url(Appverse.System.serviceName, "GetUnityContext", null, "POST");
		if(typeof(this.is.Emulator)!="undefined" && this.is.Emulator==true) {
			console.log("platform is EMULATOR");
			if(this.is.iOS) {
				console.log("setting orientation to: " + this.is.EmulatorOrientation);
				window.orientation = this.is.EmulatorOrientation;
				if(Appverse.is.Tablet){
					window.deviceType = 'iPad'; // used by some JS frameworks to determine device type (phone/tablet) in iOS.
					console.log("setting window.deviceType to: iPad");
				}
				if(typeof(this.is.EmulatorScreen)!="undefined") {
					try{window.screen = eval('(' + this.is.EmulatorScreen + ')');
					console.log("setting window.screen", window.screen);
					} catch(e) {
						console.log("error setting window.screen",e );
					}
				}
			}
			post_to_url_async = post_to_url_async_emu;
		}
	}
};

// Checking Appverse compatibility versions
if(typeof(APPVERSE_VERSION)!="undefined") {
	if(UAPPVERSE_VERSION!=Appverse.version) {
		alert("APPVERSE WARNING\nYour application was compiled with a Appverse version different from the one configured.");
	}
}

/**
 * Applications should override/implement this method to provide current device orientation javascript stored variable.
 * <br> @version 1.0
 * @method
 * @return {String} Current Device Orientation.
 * 
 */
Appverse.getCurrentOrientation = function() { };

/**
 * Applications should override this method to implement specific rotation/resizing actions for given orientation, width and height.
 * <br> @version 1.0
 * @method
 * @param {String} orientation The device current orientation.
 * @param {int} width The new width to be set.
 * @param {int} height The height width to be set.
 */
Appverse.setOrientationChange = function(orientation, width, height) { };

/**
 * @ignore
 * Updates current device orientation, width and height values.
 * <br> @version 1.0 - changes added on 2.1
 * @method
 */
var updateOrientation = function() {
	
	if(Appverse.is.iPhone) {
		////// trigger orientationchange in UIWebView for Javascript Frameworks (such as Sencha) 
		var e = document.createEvent('Events'); 
		e.initEvent('orientationchange', true, false);
		document.dispatchEvent(e);
	} else if (!Appverse.is.iPad){
	
		if (Appverse.getCurrentOrientation() == 'portrait') {
			var newWidth = window.innerHeight+20;
			var newHeight = window.innerWidth-20;
			Appverse.setOrientationChange( 'landscape' , newWidth , newHeight );
		} else {
			var newWidth = window.innerHeight+20;
			var newHeight = window.innerWidth-20;
			Appverse.setOrientationChange( 'portrait' , newWidth , newHeight );
		}
	}
}

/**
 * @ignore
 * Launches "set orientation change" method providing current device orientation, width and height values.
 * <br> @version 1.0
 * @method
 */
var refreshOrientation = function() {
	Appverse.setOrientationChange( Appverse.getCurrentOrientation() , window.innerWidth , window.innerHeight );
}


// private variable to hold status for application
Appverse._background = false;

/**
 * Indicates if application is currently in background or not.
 * <br> @version 2.0
 * @method
 * @return {Boolean} True if application has been set to background.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.isBackground = function() {
	return Appverse._background ? Appverse._background : false;
};

/**
 * Applications should override/implement this method to be aware of application being send to background, and should perform the desired javascript code on this case.
 * <br> @version 2.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.backgroundApplicationListener= function() {};

/**
 * Applications should override/implement this method to be aware of application coming back from background, and should perform the desired javascript code on this case.
 * <br> @version 2.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.foregroundApplicationListener = function() {};

/**
 * Applications should override/implement this method to be aware of device physical back button has been pressed, and should perform the desired javascript code on this case.
 * <br> @version 3.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> N/A | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.backButtonListener = function() {};

/**
 * Applications should override/implement this method to be aware of remote notification arrival, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Notification.NotificationData NotificationData}.
 * <br> @version 3.9
 * @method
 * @param {Appverse.Notiticaton.NotificationData} notificationData The notification data received (visual data and custom provider data)
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.OnRemoteNotificationReceived = function(notificationData) {};

/**
 * Applications should override/implement this method to be aware of local notification reception, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Notification.NotificationData NotificationData}.
 * <br> @version 3.9
 * @method
 * @param {Appverse.Notification.NotificationData} notificationData The notification data received (visual data and custom provider data)
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> N/A | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.OnLocalNotificationReceived = function(notificationData) {};

/**
 * Applications should override/implement this method to be aware of a successfully registration for remote notifications, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Notification.RegistrationToken RegistrationToken}.
 * <br> @version 3.9
 * @method
 * @param {Appverse.Notification.RegistrationToken} registrationToken The registration token ("device token" for iOS or "registration ID" for Android) data received from the Notifications Service (APNs for iOS or GMC for Android).
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.OnRegisterForRemoteNotificationsSuccess = function(registrationToken) {};

/**
 * Applications should override/implement this method to be aware of a successfully registration for remote notifications, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Notification.RegistrationError RegistrationError}.
 * <br> @version 3.9
 * @method
 * @param {Appverse.Notification.RegistrationError} registrationError The registration error data received from the Notifications Service (APNs for iOS or GMC for Android).
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.OnRegisterForRemoteNotificationsFailure = function(registrationError) {};

/**
 * Applications should override/implement this method to be aware of a successfully unregistration for remote notifications, and should perform the desired javascript code on this case.
 * <br> @version 4.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.OnUnRegisterForRemoteNotificationsSuccess = function() {};

/**
 * Applications should override/implement this method to be aware of storing of KeyPairs object into the local secure storage, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Security.KeyPair KeyPair}.
 * <br> @version 4.2
 * @method
 * @param {Appverse.Security.KeyPair[]} storedKeyPairs An array of KeyPair objects successfully stored in the device local secure storage.
 * @param {Appverse.Security.KeyPair[]} failedKeyPairs An array of KeyPair objects that could not be successfully stored in the device local secure storage.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Appverse.OnKeyValuePairsStoreCompleted = function(storedKeyPairs, failedKeyPairs){};

/**
 * Applications should override/implement this method to be aware of KeyPair objects found in the device local secure storage, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Security.KeyPair KeyPair}.
 * <br> @version 4.2
 * @method
 * @param {Appverse.Security.KeyPair[]} foundKeyPairs An array of KeyPair objects found in the device local secure storage.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Appverse.OnKeyValuePairsFound = function(foundKeyPairs){};

/**
 * Applications should override/implement this method to be aware of deletion of KeyPairs objects from the local secure storage, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Security.KeyPair KeyPair}.
 * <br> @version 4.2
 * @method
 * @param {Appverse.Security.KeyPair[]} removedKeyPairs An array of KeyPair objects successfully removed from the device local secure storage.
 * @param {Appverse.Security.KeyPair[]} failedKeyPairs An array of KeyPair objects that could not be removed from the device local secure storage.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Appverse.OnKeyValuePairsRemoveCompleted = function (removedKeyPairs, failedKeyPairs){};

/**
 * Applications should override/implement this method to be aware of being lanched by a third-party application, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.System.LaunchData LaunchData}.
 * <br> URI scheme protocols could contain any relative path information before parameter query string; 
 * in this case, that information will be received as a LaunchData object with the Name equals to {@link Appverse.System#LAUNCH_DATA_URI_SCHEME_PATH LAUNCH_DATA_URI_SCHEME_PATH}
 * <br> @version 4.2
 * @method
 * @param {Appverse.System.LaunchData[]} launchData The launch data received.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 * 
 */
Appverse.OnExternallyLaunched = function(launchData) {};

/**
 * @ignore
 * Appverse Platform will call this method when application goes to background.
 * <br> @version 2.0
 * @method
 */
Appverse._toBackground = function() {
	//call overrided function to inform application that we are about to put application on background
	if(Appverse.backgroundApplicationListener && typeof Appverse.backgroundApplicationListener == "function" && !Appverse._background){
		Appverse.backgroundApplicationListener();
	}
	// setting flag after calling backgroundApplicationListener; a appverse service call could be executed in that listener
	Appverse._background  = true;
}


/**
 * @ignore
 * Appverse Platform will call this method when application comes from background to foreground.
 * <br> @version 2.0
 * @method
 */
Appverse._toForeground = function() {
	Appverse._background  = false;
	
	//call overrided function to inform application that we are about to put application on foreground
	if(Appverse.foregroundApplicationListener && typeof Appverse.foregroundApplicationListener == "function"){
		Appverse.foregroundApplicationListener();
	}
}

/**
 * @ignore
 * Appverse Platform will call this method anytime the back button is pressed. Only available for devices with the "physical" back button (i.e. android devices)
 * <br> @version 3.0
 * @method
 */
Appverse._backButtonPressed = function() {
	
	//call overrided function to inform application that the back button has been pressed
	if(Appverse.backButtonListener && typeof Appverse.backButtonListener == "function"){
		Appverse.backButtonListener();
	}
}

/*
 * Manually update orientation, as no proper event is thrown using Appverse 'UIWebView'
 */
window.onorientationchange = updateOrientation;


/**
 * URI to access Appverse Local Services.
 * <pre>Platform value: 'http://<internal_server_host>:<internal_server_port>/service/'.</pre>
 * <br> @version 1.0
 * @type String
 */
Appverse.SERVICE_URI = '/service/';

/**
 * URI to access Appverse Local Services in Asynchronous mode.
 * <pre>Platform value: 'http://<internal_server_host>:<internal_server_port>/service-async/'.</pre>
 * <br> @version 3.8
 * @type String
 */
Appverse.SERVICE_ASYNC_URI = '/service-async/';

/**
 * URI to access Appverse Remote Resources.
 * <pre>Platform value: 'http://<internal_server_host>:<internal_server_port>/proxy/'.</pre>
 * <br> @version 1.0
 * @type String
 */
Appverse.REMOTE_RESOURCE_URI = '/proxy/';

/**
 * URI to access Appverse Resources from Application Documents.
 * <pre>Platform value: 'http://<internal_server_host>:<internal_server_port>/documents/'.</pre>
 * <br> @version 2.1
 * @type String
 */
Appverse.DOCUMENTS_RESOURCE_URI = '/documents/';

/**
 * URI to access Resources from loaded App Modules.
 * <pre>DPlatform value: 'http://<internal_server_host>:<internal_server_port>/documents/apps'.</pre>
 * <br> @version 4.1
 * @type String
 */
Appverse.MODULES_RESOURCE_URI = Appverse.DOCUMENTS_RESOURCE_URI + 'apps/';

if(typeof(LOCAL_SERVER_PORT)=="undefined") {
    LOCAL_SERVER_PORT = 8080; // current used port
}

Appverse.SERVICE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.SERVICE_URI;
Appverse.SERVICE_ASYNC_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.SERVICE_ASYNC_URI;
Appverse.REMOTE_RESOURCE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.REMOTE_RESOURCE_URI;
Appverse.DOCUMENTS_RESOURCE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.DOCUMENTS_RESOURCE_URI;
Appverse.MODULES_RESOURCE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.MODULES_RESOURCE_URI;

/*
 * NETWORK INTERFACES
 */

/**
 * @class Appverse.Net 
 * Singleton class field to access Net interface. 
 * <br>This interface gives access to device cellular and WIFI connection information.<br>
 * <pre>Usage: Appverse.Net.&lt;metodName&gt;([params]).<br>Example: Appverse.Net.IsNetworkReachable('gft.com').</pre>
 * <br>Each method could be called Asynchrnously by doing:.<br>
 * <pre>Usage: Appverse.Net.<b>Async</b>.&lt;metodName&gt;([params]).<br>Example: Appverse.Net.<b>Async</b>.IsNetworkReachable('gft.com').</pre>
 * <br> @version 1.0
 * @singleton
 * @constructor Constructs a new Net interface.
 * @return {Appverse.Net} A new Net interface.
 */
Net = function() {
	/**
	 * Net service name (as configured on Platform Service Locator).
	 * <br> @version 1.0
	 * @type String
	 */
	this.serviceName = "net";
        /**
	 * Unknown Network Type.
	 * <br> @version 1.0
	 * @type int
	 */
	this.NETWORKTYPE_UNKNOWN = 0;
        /**
	 * Network Type for Cable.
	 * <br> @version 1.0
	 * @type int
	 */
	this.NETWORKTYPE_CABLE = 1;
        /**
	 * Network Type for GSM Carrier.
	 * <br> @version 1.0
	 * @type int
	 */
	this.NETWORKTYPE_GSM = 2;
        /**
	 * Network Type for 2G Carrier.
	 * <br> @version 1.0
	 * @type int
	 */
	this.NETWORKTYPE_2G = 3;
        /**
	 * Network Type for 25G Carrier.
	 * <br> @version 1.0
	 * @type int
	 */
	this.NETWORKTYPE_25G = 4;
	/**
	 * Network Type for 3G Carrier.
	 * <br> @version 1.0
	 * @type int
	 */
	this.NETWORKTYPE_3G = 5;
	/**
	 * Network Type for WIFI.
	 * <br> @version 1.0
	 * @type int
	 */
	this.NETWORKTYPE_WIFI = 6;
}


Appverse.Net = new Net();

/**
 * Detects if network is reachable or not.
 * <br> @version 1.0
 * @param {String} url The host url to check for reachability.
 * @return {Boolean} True/false if reachable. 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.IsNetworkReachable = function(url)
{
	return post_to_url(Appverse.Net.serviceName, "IsNetworkReachable", get_params([url]), "POST");
};

/**
 * Gets the network information. <br/>For further information see, {@link Appverse.Net.NetworkData NetworkData}.
 * <br> @version 3.8.5
 * @return {Appverse.Net.NetworkData} NetworkData object. 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.GetNetworkData = function()
{
	return post_to_url(Appverse.Net.serviceName, "GetNetworkData", null, "POST");
};

/**
 * Gets the network types currently supported by this device.
 * <br> @version 1.0
 * <br/>Possible values of the network types: 
 * {@link Appverse.Net#NETWORKTYPE_UNKNOWN NETWORKTYPE_UNKNOWN}, 
 * {@link Appverse.Net#NETWORKTYPE_CABLE NETWORKTYPE_CABLE},
 * {@link Appverse.Net#NETWORKTYPE_GSM NETWORKTYPE_GSM},
 * {@link Appverse.Net#NETWORKTYPE_2G NETWORKTYPE_2G},
 * {@link Appverse.Net#NETWORKTYPE_25G NETWORKTYPE_25G},
 * {@link Appverse.Net#NETWORKTYPE_3G NETWORKTYPE_3G},
 * & {@link Appverse.Net#NETWORKTYPE_WIFI NETWORKTYPE_WIFI}
 * @return {int[]} Array of supported network types. 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.GetNetworkTypeSupported = function()
{
	return post_to_url(Appverse.Net.serviceName, "GetNetworkTypeSupported", null, "POST");
};

/**
 * Gets the network types from which this device is able to reach the given url host. Preference ordered list.
 * <br> @version 1.0
 * <br/>Possible values of the network types: 
 * {@link Appverse.Net#NETWORKTYPE_UNKNOWN NETWORKTYPE_UNKNOWN}, 
 * {@link Appverse.Net#NETWORKTYPE_CABLE NETWORKTYPE_CABLE},
 * {@link Appverse.Net#NETWORKTYPE_GSM NETWORKTYPE_GSM},
 * {@link Appverse.Net#NETWORKTYPE_2G NETWORKTYPE_2G},
 * {@link Appverse.Net#NETWORKTYPE_25G NETWORKTYPE_25G},
 * {@link Appverse.Net#NETWORKTYPE_3G NETWORKTYPE_3G},
 * & {@link Appverse.Net#NETWORKTYPE_WIFI NETWORKTYPE_WIFI}
 * @param {String} url The host url to check for reachability.
 * @return {int[]} Array of network types from which given url host is reachable. 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.GetNetworkTypeReachableList = function(url)
{
	return post_to_url(Appverse.Net.serviceName, "GetNetworkTypeReachableList", get_params([url]), "POST");
};

/**
 * Gets the prefered network type from which this device is able to reach the given url host.
 * <br> @version 1.0
 * <br/>Possible values of the network types: 
 * {@link Appverse.Net#NETWORKTYPE_UNKNOWN NETWORKTYPE_UNKNOWN}, 
 * {@link Appverse.Net#NETWORKTYPE_CABLE NETWORKTYPE_CABLE},
 * {@link Appverse.Net#NETWORKTYPE_GSM NETWORKTYPE_GSM},
 * {@link Appverse.Net#NETWORKTYPE_2G NETWORKTYPE_2G},
 * {@link Appverse.Net#NETWORKTYPE_25G NETWORKTYPE_25G},
 * {@link Appverse.Net#NETWORKTYPE_3G NETWORKTYPE_3G},
 * & {@link Appverse.Net#NETWORKTYPE_WIFI NETWORKTYPE_WIFI}
 * @param {String} url The host url to check for reachability.
 * @return {int} Prefered network type from which given url host is reachable. 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.GetNetworkTypeReachable = function(url)
{
	return post_to_url(Appverse.Net.serviceName, "GetNetworkTypeReachable", get_params([url]), "POST");
};

/**
 * Opens the given url in a different Web View with a Navigation Bar.
 * <br/><img src="resources/images/warning.png"/> &nbsp; <b>PDF</b> files could not be displayed on most <b>Android</b> devices (PDF viewer/reader is not included by default). A workaround could be to use the online Google DOCS viewer:<br/>
 * <pre>To see this PDF url 'http://mydomain.com/folder/mypdffile.pdf', 
 * you could use the URL, http://docs.google.com/viewer?url=http%3A%2F%2Fmydomain.com%2Ffolder%2Fmypdffile.pdf</pre>
 * More info at: [https://docs.google.com/viewer?hl=en][1]
 * [1]: https://docs.google.com/viewer?hl=en
 * <br> @version 1.0
 * @param {String} title The title of the Navigation Bar.
 * @param {String} buttonText The Back Button text of the Navigation Bar.
 * @param {String} url The url to be opened.
 * @return {Boolean} True on successful 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.OpenBrowser = function(title, buttonText, url)
{
	return post_to_url(Appverse.Net.serviceName, "OpenBrowser", get_params([title, buttonText, url]), "POST");
};

/**
 * Renders the given html in a different Web View with a Navigation Bar.
 * <br> @version 1.0
 * @param {String} title The title of the Navigation Bar.
 * @param {String} buttonText The Back Button text of the Navigation Bar.
 * @param {String} htmls The html string to be rendered.
 * @return {Boolean} True on successful 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.ShowHtml = function(title, buttonText, html)
{
	return post_to_url(Appverse.Net.serviceName, "ShowHtml", get_params([title, buttonText, html]), "POST");
};

/**
 * Downloads the given url file by using the default native handler.
 * <br> @version 2.0
 * @param {String} url The url to be opened.
 * @return {Boolean} True on successful 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.DownloadFile = function(url)
{
	return post_to_url(Appverse.Net.serviceName, "DownloadFile", get_params([url]), "POST");
};

/** 
 * @class Appverse.Net.Async 
 * Invokes all Net API methods asynchronously.
 * <br>
 * Callback function name and callback identifier are passed to the methods (last arguments) to handle the result object when it is received from appverse runtime.
 * <pre>Usage:
 * <br> var myCallbackFn = function(result, id){ <br>	...//code here your custom functionality to handle the result... <br>}
 * <br>Appverse.Net.<b>Async</b>.IsNetworkReachable('gft.com', 'myCallbackFn', 'myId').
 * </pre>
 */
Net.prototype.Async = {


/**
 * Detects ASYNC if network is reachable or not. 
 * <br> @version 2.0
 * @param {String} url The host url to check for reachability.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
IsNetworkReachable : function(url, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Net.serviceName, "IsNetworkReachable", get_params([url]), callbackFunctionName, callbackId);
},

/**
 * Gets ASYNC the network information. <br/>For further information see, {@link Appverse.Net.NetworkData NetworkData}.
 * <br> @version 3.8.5
 * @return {Appverse.Net.NetworkData} NetworkData object. 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
GetNetworkData : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Net.serviceName, "GetNetworkData", null, callbackFunctionName, callbackId);
},

/**
 * Gets ASYNC the network types currently supported by this device.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
GetNetworkTypeSupported : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Net.serviceName, "GetNetworkTypeSupported", null, callbackFunctionName, callbackId);
},

/**
 * Gets ASYNC the network types from which this device is able to reach the given url host. Preference ordered list.
 * <br> @version 2.0
 * @param {String} url The host url to check for reachability.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
GetNetworkTypeReachableList : function(url, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Net.serviceName, "GetNetworkTypeReachableList", get_params([url]), callbackFunctionName, callbackId);
},

/**
 * Gets ASYNC the prefered network type from which this device is able to reach the given url host.
 * <br> @version 2.0
 * @param {String} url The host url to check for reachability.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 */
GetNetworkTypeReachable : function(url, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Net.serviceName, "GetNetworkTypeReachable", get_params([url]), callbackFunctionName, callbackId);
},

/**
 * Opens ASYNC the given url in a different Web View with a Navigation Bar.
 * <br/><img src="resources/images/warning.png"/> &nbsp; <b>PDF</b> files could not be displayed on most <b>Android</b> devices (PDF viewer/reader is not included by default). A workaround could be to use the online Google DOCS viewer:<br/>
 * <pre>To see this PDF url 'http://mydomain.com/folder/mypdffile.pdf', 
 * you could use the URL, http://docs.google.com/viewer?url=http%3A%2F%2Fmydomain.com%2Ffolder%2Fmypdffile.pdf</pre>
 * More info at: [https://docs.google.com/viewer?hl=en][1]
 * [1]: https://docs.google.com/viewer?hl=en
 * <br> @version 2.0
 * @param {String} title The title of the Navigation Bar.
 * @param {String} buttonText The Back Button text of the Navigation Bar.
 * @param {String} url The url to be opened.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
OpenBrowser : function(title, buttonText, url, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Net.serviceName, "OpenBrowser", get_params([title, buttonText, url]), callbackFunctionName, callbackId);
},

/**
 * Renders ASYNC the given html in a different Web View with a Navigation Bar.
 * <br> @version 2.0
 * @param {String} title The title of the Navigation Bar.
 * @param {String} buttonText The Back Button text of the Navigation Bar.
 * @param {String} htmls The html string to be rendered.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
ShowHtml : function(title, buttonText, html, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Net.serviceName, "ShowHtml", get_params([title, buttonText, html]), callbackFunctionName, callbackId);
}


};

/*
 * SYSTEM INTERFACES
 */
 
/**
 * @class Appverse.System 
 * Singleton class field to access System interface. 
 * <br>This interface provides device information:<br>
 * - available displays and orientations,<br>
 * - supported locales,<br>
 * - memory status,<br>
 * - operating system, processor, and hardware information,<br>
 * - battery life information.<br>
 * <pre>Usage: Appverse.System.&lt;metodName&gt;([params]).<br>Example: Appverse.System.GetDisplays().</pre>
 * <br> @version 1.0
 * @singleton
 * @constructor Constructs a new System interface.
 * @return {Appverse.System} A new System interface.
 */
System = function() {
	/**
	 * System service name (as configured on Platform Service Locator).
 	 * <br> @version 1.0
	 * @type String
	 */
	this.serviceName = "system";
	/**
	 * Unknown Display Orientation.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.ORIENTATION_UNKNOWN = 0;
	/**
	 * Portrait Display Orientation.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.ORIENTATION_PORTRAIT = 1;
	/**
	 * Landscape Display Orientation.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.ORIENTATION_LANDSCAPE = 2;
	/**
	 * Unknown Display Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.DISPLAYTYPE_UNKNOWN = 0;
	/**
	 * Primary Display Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.DISPLAYTYPE_PRIMARY = 1;
	/**
	 * External Display Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.DISPLAYTYPE_EXTERNAL = 2;
	/**
	 * Unknown Display Bit Depth.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.BITDEPTH_UNKNOWN = 0;
	/**
	 * Bpp8 Display Bit Depth.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.BITDEPTH_BPP8 = 1;
	/**
	 * Bpp16 Display Bit Depth.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.BITDEPTH_BPP16 = 2;
	/**
	 * Bpp24 Display Bit Depth.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.BITDEPTH_BPP24 = 3;
	/**
	 * Bpp32 Display Bit Depth.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.BITDEPTH_BPP32 = 4;
	/**
	 * Unknown Input Method.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.INPUTCAPABILITY_UNKNOWN = 0;
	/**
	 * Internal Touch Keyboard Input Method.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.INPUTCAPABILITY_INTERNAL_TOUCH_KEYBOARD = 1;
	/**
	 * Internal Keyboard Input Method.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.INPUTCAPABILITY_INTERNAL_KEYBOARD = 2;
	/**
	 * External Keyboard Input Method.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.INPUTCAPABILITY_EXTERNAL_KEYBOARD = 3;
	/**
	 * Internal Pointing Device Input Method.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.INPUTCAPABILITY_INTERNAL_POINTING = 4;
	/**
	 * External Pointing Device Input Method.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.INPUTCAPABILITY_EXTERNAL_POINTING = 5;
	/**
	 * Voice Recognition Device Input Method.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.INPUTCAPABILITY_VOICE_RECOGNITION = 6;
	/**
	 * Multi Touch Gestures Device Input Method.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.INPUTCAPABILITY_MULTI_TOUCH_GESTURES = 7;
	/**
	 * Mono Touch Gestures Device Input Method.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.INPUTCAPABILITY_MONO_TOUCH_GESTURES = 8;
	/**
	 * Unknown Memory Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEMORYTYPE_UNKNOWN = 0;
	/**
	 * Main Memory Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEMORYTYPE_MAIN = 1;
	/**
	 * Extended Memory Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEMORYTYPE_EXTENDED = 2;
	/**
	 * Application Memory Use.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEMORYUSE_APPLICATION = 0;
	/**
	 * Storage Memory Use.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEMORYUSE_STORAGE = 1;
	/**
	 * Other Memory Use.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEMORYUSE_OTHER = 2;
	/**
	 * Unknown Power Status.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.POWER_UNKNOWN = 0;
	/**
	 * Fully Charged Power Status.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.POWER_FULLY_CHARGED = 1;
	/**
	 * Charging Power Status.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.POWER_CHARGING = 2;
	/**
	 * Discharging Power Status.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.POWER_DISCHARGING = 3;
	
	/**
	 * The name of the launch data parameter that contains the "URI scheme host" or relative scheme part when application is launched with extra data.
	 * This parameter will be just provided by the platforms that launch apps using an URI scheme.
 	 * <br> @version 4.2
	 * @type String
	 */
	this.LAUNCH_DATA_URI_SCHEME_PATH = "URI_SCHEME_PATH";
	
}

Appverse.System = new System();

/**
 * Provides the number of screens connected to the device. Display 1 is the primary.
 * <br> @version 1.0
 * @return {int} Number of available displays. 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/information.png"/> *harcoded data (always 1) | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetDisplays = function()
{
	return post_to_url(Appverse.System.serviceName, "GetDisplays", null, "POST");
};

/**
 * Provides information about the display given its index. <br/>For further information see, {@link Appverse.System.DisplayInfo DisplayInfo}. 
 * <br> @version 1.0
 * @param {int} displayNumber The display number index. If not provided, primary display information is returned.
 * @return {Appverse.System.DisplayInfo} The given display information, if found. Null value is returned, if given diplay number does not corresponds a valid index.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/warning.png"/> *data needs to be returned by callback| android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetDisplayInfo = function(displayNumber)
{
	if(displayNumber == null) {
		return post_to_url(Appverse.System.serviceName, "GetDisplayInfo", null, "POST");
	} else {
		return post_to_url(Appverse.System.serviceName, "GetDisplayInfo", get_params([displayNumber]), "POST");
	}
};

/**
 * Provides the current orientation of the given display index, 1 being the primary display.
 * <br> @version 1.0
 * <br/>Possible values of display orientation: 
 * {@link Appverse.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, 
 * {@link Appverse.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT},
 * & {@link Appverse.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @param {int} displayNumber The display number index. If not provided, primary display orientation is returned.
 * @return {int} The given display orientation, if found. "Unknown" value is returned, if given diplay number does not corresponds a valid index.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOrientation = function(displayNumber)
{
	return post_to_url(Appverse.System.serviceName, "GetOrientation", get_params([displayNumber]), "POST");
};

/**
 * Provides the current orientation of the primary display - the primary display is 1.
 * <br> @version 1.0
 * <br/>Possible values of display orientation: 
 * {@link Appverse.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, 
 * {@link Appverse.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT},
 * & {@link Appverse.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @return {int} The primary display orientation, if found.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOrientationCurrent = function()
{
	return post_to_url(Appverse.System.serviceName, "GetOrientationCurrent", null, "POST");
};

/**
 * Provides the list of supported orientations for the given display number.
 * <br> @version 1.0
 * <br/>Possible values of display orientation: 
 * {@link Appverse.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, 
 * {@link Appverse.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT},
 * & {@link Appverse.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @param {int} displayNumber The display number index. If not provided, primary display supported orientations are returned.
 * @return {int[]} The list of supported device orientations, for the given display.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/information.png"/> *returns portrait&landscape | android <img src="resources/images/information.png"/> *returns portrait&landscape | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOrientationSupported = function(displayNumber)
{
	if(displayNumber == null) {
		return post_to_url(Appverse.System.serviceName, "GetOrientationSupported", null, "POST");
	} else {
		return post_to_url(Appverse.System.serviceName, "GetOrientationSupported", get_params([displayNumber]), "POST");
	}
};

/**
 * List of available Locales for the device. <br/>For further information see, {@link Appverse.System.Locale Locale}. 
 * <br> @version 1.0
 * @return {Appverse.System.Locale[]} The list of supported locales.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
System.prototype.GetLocaleSupported = function()
{
	return post_to_url(Appverse.System.serviceName, "GetLocaleSupported", null, "POST");
};

/**
 * Gets the current Locale for the device.<br/>For further information see, {@link Appverse.System.Locale Locale}. 
 * <br> @version 1.0
 * @return {Appverse.System.Locale} The current Locale information.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
System.prototype.GetLocaleCurrent = function()
{
	return post_to_url(Appverse.System.serviceName, "GetLocaleCurrent", null, "POST");
};

/**
 * Gets the supported input methods.
 * <br> @version 1.0
 * <br/>Possible values of input methods: 
 * {@link Appverse.System#INPUTCAPABILITY_EXTERNAL_KEYBOARD INPUTCAPABILITY_EXTERNAL_KEYBOARD}, 
 * {@link Appverse.System#INPUTCAPABILITY_INTERNAL_POINTING INPUTCAPABILITY_INTERNAL_POINTING},
 * {@link Appverse.System#INPUTCAPABILITY_EXTERNAL_POINTING INPUTCAPABILITY_EXTERNAL_POINTING},
 * {@link Appverse.System#INPUTCAPABILITY_INTERNAL_KEYBOARD INPUTCAPABILITY_INTERNAL_KEYBOARD},
 * {@link Appverse.System#INPUTCAPABILITY_INTERNAL_TOUCH_KEYBOARD INPUTCAPABILITY_INTERNAL_TOUCH_KEYBOARD},
 * {@link Appverse.System#INPUTCAPABILITY_MONO_TOUCH_GESTURES INPUTCAPABILITY_MONO_TOUCH_GESTURESv},
 * {@link Appverse.System#INPUTCAPABILITY_MULTI_TOUCH_GESTURES INPUTCAPABILITY_MULTI_TOUCH_GESTURES},
 * {@link Appverse.System#INPUTCAPABILITY_UNKNOWN INPUTCAPABILITY_UNKNOWN},
 * & {@link Appverse.System#INPUTCAPABILITY_VOICE_RECOGNITION INPUTCAPABILITY_VOICE_RECOGNITION} 
 * @return {int[]} List of input methods supported by the device.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetInputMethods = function()
{
	return post_to_url(Appverse.System.serviceName, "GetInputMethods", null, "POST");
};

/**
 * Gets the supported input gestures.
 * <br> @version 1.0
 * @return {int[]} List of input gestures supported by the device.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetInputGestures = function()
{
	return post_to_url(Appverse.System.serviceName, "GetInputGestures", null, "POST");
};

/**
 * Gets the supported input buttons.
 * <br> @version 1.0
 * @return {int[]} List of input buttons supported by the device.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetInputButtons = function()
{
	return post_to_url(Appverse.System.serviceName, "GetInputButtons", null, "POST");
};

/**
 * Gets the currently active input method.
 * <br> @version 1.0
 * <br/>Possible values of input method: 
 * {@link Appverse.System#INPUTCAPABILITY_EXTERNAL_KEYBOARD INPUTCAPABILITY_EXTERNAL_KEYBOARD}, 
 * {@link Appverse.System#INPUTCAPABILITY_INTERNAL_POINTING INPUTCAPABILITY_INTERNAL_POINTING},
 * {@link Appverse.System#INPUTCAPABILITY_EXTERNAL_POINTING INPUTCAPABILITY_EXTERNAL_POINTING},
 * {@link Appverse.System#INPUTCAPABILITY_INTERNAL_KEYBOARD INPUTCAPABILITY_INTERNAL_KEYBOARD},
 * {@link Appverse.System#INPUTCAPABILITY_INTERNAL_TOUCH_KEYBOARD INPUTCAPABILITY_INTERNAL_TOUCH_KEYBOARD},
 * {@link Appverse.System#INPUTCAPABILITY_MONO_TOUCH_GESTURES INPUTCAPABILITY_MONO_TOUCH_GESTURESv},
 * {@link Appverse.System#INPUTCAPABILITY_MULTI_TOUCH_GESTURES INPUTCAPABILITY_MULTI_TOUCH_GESTURES},
 * {@link Appverse.System#INPUTCAPABILITY_UNKNOWN INPUTCAPABILITY_UNKNOWN},
 * & {@link Appverse.System#INPUTCAPABILITY_VOICE_RECOGNITION INPUTCAPABILITY_VOICE_RECOGNITION} 
 * @return {int} Current input method.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetInputMethodCurrent = function()
{
	return post_to_url(Appverse.System.serviceName, "GetInputMethodCurrent", null, "POST");
};

/**
 * Provides memory available for the given use and type.
 * <br> @version 1.0
 * <br/>Possible values of memory types: 
 * {@link Appverse.System#MEMORYTYPE_EXTENDED MEMORYTYPE_EXTENDED}, 
 * {@link Appverse.System#MEMORYTYPE_MAIN MEMORYTYPE_MAIN},
 * & {@link Appverse.System#MEMORYTYPE_UNKNOWN MEMORYTYPE_UNKNOWN} 
 * <br/>Possible values of memory uses: 
 * {@link Appverse.System#MEMORYUSE_APPLICATION MEMORYUSE_APPLICATION}, 
 * {@link Appverse.System#MEMORYUSE_STORAGE MEMORYUSE_STORAGE},
 * & {@link Appverse.System#MEMORYUSE_OTHER MEMORYUSE_OTHER} 
 * @param {int} memUse The memory use. 
 * @param {int} memType The memory type. Optional parameter.
 * @return {long} The memory available in bytes.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetMemoryAvailable = function(memUse, memType)
{
	if(memType == null) {
		return post_to_url(Appverse.System.serviceName, "GetMemoryAvailable", get_params([memUse]), "POST");
	} else {
		return post_to_url(Appverse.System.serviceName, "GetMemoryAvailable", get_params([memUse,memType]), "POST");
	}
};

/**
 * Gets the device installed memory types.
 * <br> @version 1.0
 * <br/>Possible values of memory types: 
 * {@link Appverse.System#MEMORYTYPE_EXTENDED MEMORYTYPE_EXTENDED}, 
 * {@link Appverse.System#MEMORYTYPE_MAIN MEMORYTYPE_MAIN},
 * & {@link Appverse.System#MEMORYTYPE_UNKNOWN MEMORYTYPE_UNKNOWN} 
 * @return {int[]} The installed storage types.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetMemoryAvailableTypes = function()
{
	return post_to_url(Appverse.System.serviceName, "GetMemoryAvailableTypes", null, "POST");
};

/**
 * Provides a global map of the memory status for all storage types installed, if 'memType' not provided.
 * Provides a map of the memory status for the given storage type, if 'memType' provided.
 * <br/>For further information see, {@link Appverse.System.MemoryStatus MemoryStatus}. 
 * <br> @version 1.0
 * <br/>Possible values of memory types: 
 * {@link Appverse.System#MEMORYTYPE_EXTENDED MEMORYTYPE_EXTENDED}, 
 * {@link Appverse.System#MEMORYTYPE_MAIN MEMORYTYPE_MAIN},
 * & {@link Appverse.System#MEMORYTYPE_UNKNOWN MEMORYTYPE_UNKNOWN} 
 * @param {int} memType The type of memory to check for status. Optional parameter.
 * @return {Appverse.System.MemoryStatus} The memory status information.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetMemoryStatus = function(memType)
{
	if(memType == null) {
		return post_to_url(Appverse.System.serviceName, "GetMemoryStatus", null, "POST");
	} else {
		return post_to_url(Appverse.System.serviceName, "GetMemoryStatus", get_params([memType]), "POST");
	}
};

/**
 * Gets the device currently available memory types.
 * <br> @version 1.0
 * <br/>Possible values of memory types: 
 * {@link Appverse.System#MEMORYTYPE_EXTENDED MEMORYTYPE_EXTENDED}, 
 * {@link Appverse.System#MEMORYTYPE_MAIN MEMORYTYPE_MAIN},
 * & {@link Appverse.System#MEMORYTYPE_UNKNOWN MEMORYTYPE_UNKNOWN} 
 * @return {int[]} The available storafe types.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/information.png"/> *harcoded values | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetMemoryTypes = function()
{
	return post_to_url(Appverse.System.serviceName, "GetMemoryTypes", null, "POST");
};

/**
 * Gets the device currently available memory uses.
 * <br> @version 1.0
 * <br/>Possible values of memory uses: 
 * {@link Appverse.System#MEMORYUSE_APPLICATION MEMORYUSE_APPLICATION}, 
 * {@link Appverse.System#MEMORYUSE_STORAGE MEMORYUSE_STORAGE},
 * & {@link Appverse.System#MEMORYUSE_OTHER MEMORYUSE_OTHER} 
 * @return {int[]} The available memory uses.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/information.png"/> *harcoded values | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetMemoryUses = function()
{
	return post_to_url(Appverse.System.serviceName, "GetMemoryUses", null, "POST");
};

/**
 * Provides information about the device hardware.<br/>For further information see, {@link Appverse.System.HardwareInfo HardwareInfo}.
 * <br> @version 1.0
 * @return {Appverse.System.HardwareInfo} The device hardware information (name, version, UUID, etc).
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOSHardwareInfo = function()
{
	return post_to_url(Appverse.System.serviceName, "GetOSHardwareInfo", null, "POST");
};

/**
 * Provides information about the device operating system.<br/>For further information see, {@link Appverse.System.OSInfo OSInfo}.
 * <br> @version 1.0
 * @return {Appverse.System.OSInfo} The device OS information (name, vendor, version).
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOSInfo = function()
{
	return post_to_url(Appverse.System.serviceName, "GetOSInfo", null, "POST");
};

/**
 * Provides the current user agent string.
 * <br> @version 1.0
 * @return {String} The user agent string. 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOSUserAgent = function()
{
	return post_to_url(Appverse.System.serviceName, "GetOSUserAgent", null, "POST");
};

/**
 * Provides information about the device charge.<br/>For further information see, {@link Appverse.System.PowerInfo PowerInfo}.
 * <br> @version 1.0
 * @return {Appverse.System.PowerInfo} The current charge information.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetPowerInfo = function()
{
	return post_to_url(Appverse.System.serviceName, "GetPowerInfo", null, "POST");
};

/**
 * Provides device autonomy time (in milliseconds).
 * <br> @version 1.0
 * @return {long} The remaining power time.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetPowerRemainingTime = function()
{
	return post_to_url(Appverse.System.serviceName, "GetPowerRemainingTime", null, "POST");
};

/**
 * Provides information about the device CPU.<br/>For further information see, {@link Appverse.System.CPUInfo CPUInfo}.
 * <br> @version 1.0
 * @return {Appverse.System.CPUInfo} The processor information (name, vendor, speed, UUID, etc).
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> *not available on iOS SDK | android <img src="resources/images/error.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetCPUInfo = function()
{
	return post_to_url(Appverse.System.serviceName, "GetCPUInfo", null, "POST");
};

/**
 * Provides information about if the current application is allowed to autorotate or not. If locked, 
 * <br> @version 2.0
 * @return {Boolean} True if application remains with the same screen orientation (even though user rotates the device).
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.IsOrientationLocked  = function() {
	return post_to_url(Appverse.System.serviceName, "IsOrientationLocked", null, "POST");
};

/**
 * Sets wheter the current application could autorotate or not (whether orientation is locked or not)
 * <br> @version 2.0
 * @param {Boolean} Set value to true if application should remain with the same screen orientation (even though user rotates the device)..
 * @param {int} Set the orientation to lock the device to (this value is ignored if "lock" argument is "false"). Possible values of display orientation: {@link Appverse.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, {@link Appverse.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT} or {@link Appverse.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.LockOrientation = function(lock, orientation) {
	return post_to_url(Appverse.System.serviceName, "LockOrientation", get_params([lock,orientation]), "POST");
};

/**
 * Copies a specified text to the native device clipboard.
 * <br> @version 3.2
 * @param {String} textToCopy Text to copy to the Clipboard.
 * @return {Boolean} True if the text was successfully copied to the Clipboard, else False.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
System.prototype.CopyToClipboard = function(textToCopy)
{
	return post_to_url(Appverse.System.serviceName, "CopyToClipboard", get_params([textToCopy]), "POST");
};

/**
 * Shows default splashcreen (on current orientation). Only the corresponding {@link Appverse.System.DismissSplashScreen} method could dismiss this splash screen.
 * The splash screen could be shown on application start up by default, by properly configure it on the applaction build.properties (build property: app.showsplashscreen.onstartup=true)
 * <br> @version 3.2
 * @return {Boolean} True if the splash screen is successfully shown, else False.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.ShowSplashScreen = function()
{
	return post_to_url(Appverse.System.serviceName, "ShowSplashScreen", null, "POST");
};

/**
 * Dismisses the splashcreen previously shown using {@link Appverse.System.ShowSplashScreen}.
 * <br> @version 3.2
 * @return {Boolean} True if the splash screen is successfully dismissed, else False.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.DismissSplashScreen = function()
{
	return post_to_url(Appverse.System.serviceName, "DismissSplashScreen", null, "POST");
};

/**
 * Dismisses the current application programmatically.
 * It is up to the HTML app to manage the state and determine when to close the application using this method.
 * <br> <b>This feature is not supported on iOS platform (interface is available, but with no effect)</b>
 * <br> @version 3.8
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> *N/A* | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.DismissApplication = function()
{
	post_to_url(Appverse.System.serviceName, "DismissApplication", null, "POST");
};

/**
 * Returns all applications configured to be launched (using Appverse.System.LaunchApplication method) at configuration file: app/config/launch-data.xml.
 * <br> @version 4.2
 * @method
 * @return {Appverse.System.App[]} Applications to be launched.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetApplications = function()
{
	return post_to_url(Appverse.System.serviceName, "GetApplications", null, "POST");
};

/**
 * Returns an application configured to be launched (using Appverse.System.LaunchApplication method) at configuration file: app/config/launch-data.xml, given its name.
 * <br> @version 4.2
 * @method
 * @param {String} appName The application name to match.
 * @return {Appverse.System.App} Application to be launched that match the given name.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetApplication = function(appName)
{
	return post_to_url(Appverse.System.serviceName, "GetApplication", get_params([appName]), "POST");
};

/**
 * Returns all applications configured to be launched (using Appverse.System.LaunchApplication method) at configuration file: app/config/launch-data.xml.
 * <br> @version 4.2
 * @method
 * @param {Appverse.System.App/String} app The application object (or its name) to be launched.
 * @param {String} query The query string (parameters) in the format: "relative_url?param1=value1&param2=value2". Set it to null for not sending extra launch data.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.LaunchApplication = function(app, query)
{
	post_to_url(Appverse.System.serviceName, "LaunchApplication", get_params([app, query]), "POST");
};

/*
 * DATABASE INTERFACES
 */

/**
 * @class Appverse.Database 
 * Singleton class field to access Database interface. 
 * <br>This interface allows to create SQL Databases for use by your application and access them from your application's Javascript.<br>
 * <pre>Usage: Appverse.Database.&lt;metodName&gt;([params]).<br>Example: Appverse.Database.GetDatabaseList().</pre>
 * <br> @version 1.0
 * @singleton
 * @constructor Constructs a new Database interface.
 * @return {Appverse.Database} A new Database interface.
 */
Database = function() {
	/**
	 * Database service name (as configured on Platform Service Locator).
 	 * <br> @version 1.0
	 * @type String
	 */
	this.serviceName = "db";
}

Appverse.Database = new Database();

/**
 * Gets stored databases.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 1.0
 * @return {Appverse.Database.Database[]} List of application Databases.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.GetDatabaseList = function()
{
	return post_to_url(Appverse.Database.serviceName, "GetDatabaseList", null, "POST");
};

/**
 * Creates database on default path.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 1.0
 * @param {String} dbName The database file name (please include .db extension).
 * @return {Appverse.Database.Database} The created database reference object.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.CreateDatabase = function(dbName)
{
	return post_to_url(Appverse.Database.serviceName, "CreateDatabase", get_params([dbName]), "POST");
};

/**
 * Gets database reference object by given name.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br>Databases are located on the default database path: /<PersonalFolder>/sqlite/
 * <br> @version 1.0
 * @param {String} dbName The database file name (inlcuding .db extension).
 * @return {Appverse.Database.Database} The created database reference object.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.GetDatabase = function(dbName)
{
	return post_to_url(Appverse.Database.serviceName, "GetDatabase", get_params([dbName]), "POST");
};

/**
 * Creates a table inside the given database.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 1.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase} ).
 * @param {String} tableName The table name to be inserted.
 * @param {String[]} columnsDefs The column definitions array (SQLITE syntax).
 * @return {Boolean} True on successful table creation.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.CreateTable = function(db,tableName,columnsDefs)
{
	return post_to_url(Appverse.Database.serviceName, "CreateTable", get_params([db,tableName, columnsDefs]), "POST");
};

/**
 * Deletes database on default path.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 1.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}) to be deleted.
 * @return {Boolean} True on successful database deletion.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.DeleteDatabase = function(db)
{
	return post_to_url(Appverse.Database.serviceName, "DeleteDatabase", get_params([db]), "POST");
};

/**
 * Deletes table from the given database.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 1.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase} ).
 * @param {String} tableName The table name to be deleted.
 * @return {Boolean} True on successful table deletion.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.DeleteTable = function(db,tableName)
{
	return post_to_url(Appverse.Database.serviceName, "DeleteTable", get_params([db,tableName]), "POST");
};

/**
 * Gets table names from the given database.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 1.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}) to check for table names.
 * @return {String[]} List of table names.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.GetTableNames = function(db)
{
	return post_to_url(Appverse.Database.serviceName, "GetTableNames", get_params([db]), "POST");
};

/**
 * Checks if database exists by database bean reference, if 'tableName' is not provided.
 * Checks if database table exists by database bean reference and table name, if 'tableName' is provided.
 * <br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 1.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} tableName The table name  to check for existence. Optional parameter.
 * @return {Boolean} True if database or database table exists.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.Exists = function(db, tableName)
{
	if(tableName == null) {
		 return post_to_url(Appverse.Database.serviceName, "Exists", get_params([db]), "POST");
	} else {
		return post_to_url(Appverse.Database.serviceName, "Exists", get_params([db,tableName]), "POST");
	}
};

/**
 * Checks if database exists by given database name (including .db extension).<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 1.0
 * @param {String} dbName The database name to check for existence.
 * @return {Boolean} True if database exists.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.ExistsDatabase = function(dbName)
{
	return post_to_url(Appverse.Database.serviceName, "ExistsDatabase", get_params([dbName]), "POST");
};

/**
 * Executes SQL query against given database.<br/>For further information see, {@link Appverse.Database.Database Database} and {@link Appverse.Database.ResultSet ResultSet}.
 * <br> @version 1.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} query The SQL query to execute. 
 * @param {String[]} replacements The replacement arguments for a preformatted SQL query. Optional parameter.
 * @return {Appverse.Database.ResultSet} The result set (with zero rows count parameter if no rows satisfy query conditions).
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.ExecuteSQLQuery = function(db, query, replacements)
{
	if(replacements == null) {
		return post_to_url(Appverse.Database.serviceName, "ExecuteSQLQuery", get_params([db,query]), "POST");
	} else {
		return post_to_url(Appverse.Database.serviceName, "ExecuteSQLQuery", get_params([db,query,replacements]), "POST");
	}
};

/**
 * Executes SQL statement into the given database.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 1.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} statement The SQL statement to execute. 
 * @param {String[]} replacements The replacement arguments for a preformatted SQL statement. Optional parameter.
 * @return {Boolean} True on successful statement execution.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.ExecuteSQLStatement = function(db, statement, replacements)
{
	if(replacements == null) {
		return post_to_url(Appverse.Database.serviceName, "ExecuteSQLStatement", get_params([db,statement]), "POST");
	} else {
		return post_to_url(Appverse.Database.serviceName, "ExecuteSQLStatement", get_params([db,statement,replacements]), "POST");
	}
};

/**
 * Executes SQL transaction (some statements chain) inside given database.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 1.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String[]} statements The statements to be executed during transaction (sqlite syntax language).. 
 * @param {Boolean} rollbackFlag Indicates if rollback should be performed when any statement execution fails.
 * @return {Boolean} True on successful transaction execution.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.ExecuteSQLTransaction = function(db, statements, rollbackFlag)
{
	return post_to_url(Appverse.Database.serviceName, "ExecuteSQLTransaction", get_params([db,statements,rollbackFlag]), "POST");
};

/**
 * @class Appverse.Database.Async 
 * Invokes all Database API methods asynchronously.
 * <br>
 * Callback function name and callback identifier are passed to the methods (last arguments) to handle the result object when it is received from appverse runtime.
 * <pre>Usage:
 * <br> var myCallbackFn = function(result, id){ <br>	...//code here your custom functionality to handle the result... <br>}
 * <br>Appverse.Database.<b>Async</b>.GetDatabaseList('myCallbackFn', 'myId').
 * <br>or
 * <br>Appverse.Database.<b>Async</b>.GetDatabase('databaseName', 'myCallbackFn', 'myId').
 * </pre>
 */
Database.prototype.Async = {

/**
 * Gets stored databases, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
GetDatabaseList : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Database.serviceName, "GetDatabaseList", null, callbackFunctionName, callbackId);
},

/**
 * Creates database on default path, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} dbName The database file name (please include .db extension).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
CreateDatabase : function(dbName, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Database.serviceName, "CreateDatabase", get_params([dbName]), callbackFunctionName, callbackId);
},

/**
 * Gets database reference object by given name, in ASYNC mode.
 * <br>Databases are located on the default database path: /<PersonalFolder>/sqlite/
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
GetDatabase : function(dbName, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Database.serviceName, "GetDatabase", get_params([dbName]), callbackFunctionName, callbackId);
},

/**
 * Creates a table inside the given database, in ASYNC mode.
 * <br> @version 2.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase} ).
 * @param {String} tableName The table name to be inserted.
 * @param {String[]} columnsDefs The column definitions array (SQLITE syntax).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
CreateTable : function(db,tableName,columnsDefs, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Database.serviceName, "CreateTable", get_params([db,tableName, columnsDefs]), callbackFunctionName, callbackId);
},

/**
 * Deletes database on default path, in ASYNC mode.
 * <br> @version 2.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}) to be deleted.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
DeleteDatabase : function(db, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Database.serviceName, "DeleteDatabase", get_params([db]), callbackFunctionName, callbackId);
},

/**
 * Deletes table from the given database, in ASYNC mode.
 * <br> @version 2.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase} ).
 * @param {String} tableName The table name to be deleted.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
DeleteTable : function(db,tableName, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Database.serviceName, "DeleteTable", get_params([db,tableName]), callbackFunctionName, callbackId);
},

/**
 * Gets table names from the given database, in ASYNC mode.
 * <br> @version 1.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}) to check for table names.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
GetTableNames : function(db, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Database.serviceName, "GetTableNames", get_params([db]), callbackFunctionName, callbackId);
},

/**
 * Checks if database exists by database bean reference, if 'tableName' is not provided, in ASYNC mode.
 * Checks if database table exists by database bean reference and table name, if 'tableName' is provided.
 * <br> @version 2.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} tableName The table name  to check for existence. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Exists : function(db, tableName, callbackFunctionName, callbackId)
{
	if(tableName == null) {
        post_to_url_async(Appverse.Database.serviceName, "Exists", get_params([db]), callbackFunctionName, callbackId);
	} else {
		post_to_url_async(Appverse.Database.serviceName, "Exists", get_params([db,tableName]), callbackFunctionName, callbackId);
	}
},

/**
 * Checks if database exists by given database name (including .db extension), in ASYNC mode.
 * <br> @version 2.0
 * @param {String} dbName The database name to check for existence.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
ExistsDatabase : function(dbName, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Database.serviceName, "ExistsDatabase", get_params([dbName]), callbackFunctionName, callbackId);
},

/**
 * Executes SQL query against given database, in ASYNC mode.
 * <br> @version 2.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} query The SQL query to execute. 
 * @param {String[]} replacements The replacement arguments for a preformatted SQL query. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
ExecuteSQLQuery : function(db, query, replacements, callbackFunctionName, callbackId)
{
	if(replacements == null) {
		post_to_url_async(Appverse.Database.serviceName, "ExecuteSQLQuery", get_params([db,query]), callbackFunctionName, callbackId);
	} else {
		post_to_url_async(Appverse.Database.serviceName, "ExecuteSQLQuery", get_params([db,query,replacements]), callbackFunctionName, callbackId);
	}
},

/**
 * Executes SQL statement into the given database, in ASYNC mode.
 * <br> @version 2.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} statement The SQL statement to execute. 
 * @param {String[]} replacements The replacement arguments for a preformatted SQL statement. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
ExecuteSQLStatement : function(db, statement, replacements, callbackFunctionName, callbackId)
{
	if(replacements == null) {
		post_to_url_async(Appverse.Database.serviceName, "ExecuteSQLStatement", get_params([db,statement]), callbackFunctionName, callbackId);
	} else {
		post_to_url_async(Appverse.Database.serviceName, "ExecuteSQLStatement", get_params([db,statement,replacements]), callbackFunctionName, callbackId);
	}
},

/**
 * Executes SQL transaction (some statements chain) inside given database, in ASYNC mode.
 * <br> @version 2.0
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String[]} statements The statements to be executed during transaction (sqlite syntax language).. 
 * @param {Boolean} rollbackFlag Indicates if rollback should be performed when any statement execution fails.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
ExecuteSQLTransaction : function(db, statements, rollbackFlag, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Database.serviceName, "ExecuteSQLTransaction", get_params([db,statements,rollbackFlag]), callbackFunctionName, callbackId);
}

};

/*
 * FILE INTERFACES
 */

/**
 * @class Appverse.FileSystem 
 * Singleton class field to access FileSystem interface. 
 * <br>This interface provides access to the device filesystem (only personal folder is accessible), to create/access/delete directories and files.<br>
 * <br> @version 1.0
 * <pre>Usage: Appverse.FileSystem.&lt;metodName&gt;([params]).<br>Example: Appverse.FileSystem.GetDirectoryRoot().</pre>
 * @singleton
 * @constructor Constructs a new FileSystem interface.
 * @return {Appverse.FileSystem} A new FileSystem interface.
 */
FileSystem = function() {
	/**
	 * FileSystem service name (as configured on Platform Service Locator).
	 * @type String
 	 * <br> @version 1.0
	 */
	this.serviceName = "file";
}

Appverse.FileSystem = new FileSystem();

/**
 * Get configured root directory.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}.
 * <br> @version 1.0
 * @return {Appverse.FileSystem.DirectoryData} The configured root directory information.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.GetDirectoryRoot = function()
{
	return post_to_url(Appverse.FileSystem.serviceName, "GetDirectoryRoot", null, "POST");
};

/**
 * Creates a directory under the given base directory, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}.
 * <br> @version 1.0
 * @param {String} directoryName The directory name to be created. 
 * @param {Appverse.FileSystem.DirectoryData} baseDirectory The base Directory to create directory under it. Optional parameter.
 * @return {Appverse.FileSystem.DirectoryData} The directory created, or null if folder cannot be created.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.CreateDirectory = function(directoryName, baseDirectory)
{
	if(baseDirectory == null) {
		return post_to_url(Appverse.FileSystem.serviceName, "CreateDirectory", get_params([directoryName]), "POST");
	} else {
		return post_to_url(Appverse.FileSystem.serviceName, "CreateDirectory", get_params([directoryName,baseDirectory]), "POST");
	}
};

/**
 * Creates a file under the given base directory, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData} and {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 1.0
 * @param {String} fileName The file name to be created. 
 * @param {Appverse.FileSystem.DirectoryData} baseDirectory The base Directory to create file under it. Optional parameter.
 * @return {Appverse.FileSystem.FileData} The file created, or null if folder cannot be created.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.CreateFile = function(fileName, baseDirectory)
{
	if(baseDirectory == null) {
		return post_to_url(Appverse.FileSystem.serviceName, "CreateFile", get_params([fileName]), "POST");
	} else {
		return post_to_url(Appverse.FileSystem.serviceName, "CreateFile", get_params([fileName,baseDirectory]), "POST");
	}
};

/**
 * List all directories under the given base directory data, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}.
 * <br> @version 1.0
 * @param {Appverse.FileSystem.DirectoryData} dirData The base Directory to check for directories under it. Optional parameter.
 * @return {Appverse.FileSystem.DirectoryData[]} The directories information array.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.ListDirectories = function(dirData)
{
	if(dirData == null) {
		return post_to_url(Appverse.FileSystem.serviceName, "ListDirectories", null, "POST");
	} else {
		return post_to_url(Appverse.FileSystem.serviceName, "ListDirectories", get_params([dirData]), "POST");
	}
};

/**
 * List all files under the given base directory data, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData} and {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 1.0
 * @param {Appverse.FileSystem.DirectoryData} dirData The base Directory to check for files under it. Optional parameter.
 * @return {Appverse.FileSystem.FileData[]} The files information array.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.ListFiles = function(dirData)
{
	return post_to_url(Appverse.FileSystem.serviceName, "ListFiles", get_params([dirData]), "POST");
};

/**
 * Checks if the given directory exists.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}.
 * <br> @version 1.0
 * @param {Appverse.FileSystem.DirectoryData} dirData The directory to check for existence.
 * @return {Boolean} True if directory exists.
 * @method
 */
FileSystem.prototype.ExistsDirectory = function(dirData)
{
	return post_to_url(Appverse.FileSystem.serviceName, "ExistsDirectory", get_params([dirData]), "POST");
};

/**
 * Deletes the given directory.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}.
 * <br> @version 1.0
 * @param {Appverse.FileSystem.DirectoryData} dirData The directory to be deleted.
 * @return {Boolean} True on successful directory deletion.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.DeleteDirectory = function(dirData)
{
	return post_to_url(Appverse.FileSystem.serviceName, "DeleteDirectory", get_params([dirData]), "POST");
};

/**
 * Deletes the given file.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 1.0
 * @param {Appverse.FileSystem.FileData} fileData The file to be deleted.
 * @return {Boolean} True on successful file deletion.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.DeleteFile = function(fileData)
{
	return post_to_url(Appverse.FileSystem.serviceName, "DeleteFile", get_params([fileData]), "POST");
};

/**
 * Checks if the given file exists.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 1.0
 * @param {Appverse.FileSystem.FileData} fileData The file data to check for existence.
 * @return {Boolean} True if file exists.
 * @method
 */
FileSystem.prototype.ExistsFile = function(fileData)
{
	return post_to_url(Appverse.FileSystem.serviceName, "ExistsFile", get_params([fileData]), "POST");
};

/**
 * Reads file on given path.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 1.0
 * @param {Appverse.FileSystem.FileData} fileData The file data to read.
 * @return {byte[]} Readed bytes.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.ReadFile = function(fileData)
{
	return post_to_url(Appverse.FileSystem.serviceName, "ReadFile", get_params([fileData]), "POST");
};

/**
 * Writes contents to file on given path.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 1.0
 * @param {Appverse.FileSystem.FileData} fileData The file to add/append contents to.
 * @param {byte[]} contents The data to be written to file.
 * @param {Boolean} appendFlag True if data should be appended to previous file data.
 * @return {Boolean} True if file could be written.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.WriteFile = function(fileData, contents, appendFlag)
{
	return post_to_url(Appverse.FileSystem.serviceName, "WriteFile", get_params([fileData,contents,appendFlag]), "POST");
};

/**
 * Copies the given file on "fromPath" to the "toPath". 
 * <br> @version 1.1
 * @param {String} sourceFileName The file name (relative path under "resources" application directory) to be copied from. 
 * @param {String} destFileName The file name (relative path under "documents" application directory) to be copied to.
 * @return {Boolean} True if file could be copied.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/warning.png"/> *"resources" path pending to be defined for this platform | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.CopyFromResources = function(sourceFileName, destFileName)
{
	return post_to_url(Appverse.FileSystem.serviceName, "CopyFromResources", get_params([sourceFileName,destFileName]), "POST");
};

/**
 * Copies the given remote file from "url" to the "toPath" (local relative path). 
 * <br> @version 2.1
 * @param {String} url The remote url file to be copied from. 
 * @param {String} destFileName The file name (relative path under "documents" application directory) to be copied to.
 * @return {Boolean} True if file could be copied.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.CopyFromRemote = function(url, destFileName)
{
	return post_to_url(Appverse.FileSystem.serviceName, "CopyFromRemote", get_params([url,destFileName]), "POST");
};

/**
 * @class Appverse.FileSystem.Async 
 * Invokes all FileSystem API methods asynchronously.
 * <br>
 * Callback function name and callback identifier are passed to the methods (last arguments) to handle the result object when it is received from appverse runtime.
 * <pre>Usage:
 * <br> var myCallbackFn = function(result, id){ <br>	...//code here your custom functionality to handle the result... <br>}
 * <br>Appverse.FileSystem.<b>Async</b>.GetDirectoryRoot('myCallbackFn', 'myId').
 * <br>or
 * <br>Appverse.FileSystem.<b>Async</b>.ReadFile(fileDataObj, 'myCallbackFn', 'myId').
 * </pre>
 */
FileSystem.prototype.Async = {

/**
 * Get configured root directory.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}, in ASYNC mode.
 * <br> @version 3.8.5
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
GetDirectoryRoot : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.FileSystem.serviceName, "GetDirectoryRoot", null, callbackFunctionName, callbackId);
},

/**
 * Creates a directory under the given base directory, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}, in ASYNC mode.
 * <br> @version 3.8.5
 * @param {String} directoryName The directory name to be created. 
 * @param {Appverse.FileSystem.DirectoryData} baseDirectory The base Directory to create directory under it. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
CreateDirectory : function(directoryName, baseDirectory, callbackFunctionName, callbackId)
{
	if(baseDirectory == null) {
		post_to_url_async(Appverse.FileSystem.serviceName, "CreateDirectory", get_params([directoryName]), callbackFunctionName, callbackId);
	} else {
		post_to_url_async(Appverse.FileSystem.serviceName, "CreateDirectory", get_params([directoryName,baseDirectory]), callbackFunctionName, callbackId);
	}
},

/**
 * Creates a file under the given base directory, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData} and {@link Appverse.FileSystem.FileData FileData}, in ASYNC mode.
 * <br> @version 3.8.5
 * @param {String} fileName The file name to be created. 
 * @param {Appverse.FileSystem.DirectoryData} baseDirectory The base Directory to create file under it. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
CreateFile : function(fileName, baseDirectory, callbackFunctionName, callbackId)
{
	if(baseDirectory == null) {
		post_to_url_async(Appverse.FileSystem.serviceName, "CreateFile", get_params([fileName]), callbackFunctionName, callbackId);
	} else {
		post_to_url_async(Appverse.FileSystem.serviceName, "CreateFile", get_params([fileName,baseDirectory]), callbackFunctionName, callbackId);
	}
},

/**
 * List all directories under the given base directory data, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}, in ASYNC mode.
 * <br> @version 3.8.5
 * @param {Appverse.FileSystem.DirectoryData} dirData The base Directory to check for directories under it. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
ListDirectories : function(dirData, callbackFunctionName, callbackId)
{
	if(dirData == null) {
		post_to_url_async(Appverse.FileSystem.serviceName, "ListDirectories", null, callbackFunctionName, callbackId);
	} else {
		post_to_url_async(Appverse.FileSystem.serviceName, "ListDirectories", get_params([dirData]), callbackFunctionName, callbackId);
	}
},

/**
 * List all files under the given base directory data, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData} and {@link Appverse.FileSystem.FileData FileData}, in ASYNC mode.
 * <br> @version 3.8.5
 * @param {Appverse.FileSystem.DirectoryData} dirData The base Directory to check for files under it. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
ListFiles : function(dirData, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.FileSystem.serviceName, "ListFiles", get_params([dirData]), callbackFunctionName, callbackId);
},

/**
 * Checks if the given directory exists.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}, in ASYNC mode.
 * <br> @version 3.8.5
 * @param {Appverse.FileSystem.DirectoryData} dirData The directory to check for existence.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 */
ExistsDirectory : function(dirData, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.FileSystem.serviceName, "ExistsDirectory", get_params([dirData]), callbackFunctionName, callbackId);
},

/**
 * Deletes the given directory.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}, in ASYNC mode.
 * <br> @version 3.8.5
 * @param {Appverse.FileSystem.DirectoryData} dirData The directory to be deleted.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
DeleteDirectory : function(dirData, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.FileSystem.serviceName, "DeleteDirectory", get_params([dirData]), callbackFunctionName, callbackId);
},

/**
 * Deletes the given file.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}, in ASYNC mode.
 * <br> @version 3.8.5
 * @param {Appverse.FileSystem.FileData} fileData The file to be deleted.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
DeleteFile : function(fileData, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.FileSystem.serviceName, "DeleteFile", get_params([fileData]), callbackFunctionName, callbackId);
},

/**
 * Checks if the given file exists.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}, in ASYNC mode.
 * <br> @version 3.8.5
 * @param {Appverse.FileSystem.FileData} fileData The file data to check for existence.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 */
ExistsFile : function(fileData, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.FileSystem.serviceName, "ExistsFile", get_params([fileData]), callbackFunctionName, callbackId);
},

/**
 * Reads file on given path.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}, in ASYNC mode.
 * <br> @version 3.8.5
 * @param {Appverse.FileSystem.FileData} fileData The file data to read.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
ReadFile : function(fileData, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.FileSystem.serviceName, "ReadFile", get_params([fileData]), callbackFunctionName, callbackId);
},

/**
 * Writes contents to file on given path.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}, in ASYNC mode.
 * <br> @version 3.8.5
 * @param {Appverse.FileSystem.FileData} fileData The file to add/append contents to.
 * @param {byte[]} contents The data to be written to file.
 * @param {Boolean} appendFlag True if data should be appended to previous file data.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
WriteFile : function(fileData, contents, appendFlag, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.FileSystem.serviceName, "WriteFile", get_params([fileData,contents,appendFlag]), callbackFunctionName, callbackId);
},

/**
 * Copies the given file on "fromPath" to the "toPath", in ASYNC mode.
 * <br> @version 3.8.5
 * @param {String} sourceFileName The file name (relative path under "resources" application directory) to be copied from. 
 * @param {String} destFileName The file name (relative path under "documents" application directory) to be copied to.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/warning.png"/> *"resources" path pending to be defined for this platform | emulator <img src="resources/images/check.png"/> </pre>
 */
CopyFromResources : function(sourceFileName, destFileName, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.FileSystem.serviceName, "CopyFromResources", get_params([sourceFileName,destFileName]), callbackFunctionName, callbackId);
},

/**
 * Copies the given remote file from "url" to the "toPath" (local relative path), in ASYNC mode.
 * <br> @version 3.8.5
 * @param {String} url The remote url file to be copied from. 
 * @param {String} destFileName The file name (relative path under "documents" application directory) to be copied to.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
CopyFromRemote : function(url, destFileName, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.FileSystem.serviceName, "CopyFromRemote", get_params([url,destFileName]), callbackFunctionName, callbackId);
}

};

/*
 * Notification INTERFACES
 */
 
/**
 * @class Appverse.Notification 
 * Singleton class field to access Notification interface. 
 * <br>This interface handles visual, audible, and tactile device notifications.<br>
 * <br> @version 1.0
 * <pre>Usage: Appverse.Notification.&lt;metodName&gt;([params]).<br>Example: Appverse.Notification.StartNotifyActivity().</pre>
 * @singleton
 * @constructor Constructs a new Notification interface.
 * @return {Appverse.Notification} A new Notification interface.
 */
Notification = function() {
	/**
	 * Notification service name (as configured on Platform Service Locator).
	 * @type String
 	 * <br> @version 1.0
	 */
	this.serviceName = "notify";
	/**
	 * None Remote Notification Type.
	 * <br> @version 3.9
	 * @type int
	 */
	this.REMOTE_NOTIFICATION_TYPE_NONE = 0;
    /**
	 * Badge Remote Notification Type.
	 * <br> @version 3.9
	 * @type int
	 */
	this.REMOTE_NOTIFICATION_TYPE_BADGE = 1;
    /**
	 * Sound Remote Notification Type.
	 * <br> @version 3.9
	 * @type int
	 */
	this.REMOTE_NOTIFICATION_TYPE_SOUND = 2;
    /**
	 * Alert Remote Notification Type.
	 * <br> @version 3.9
	 * @type int
	 */
	this.REMOTE_NOTIFICATION_TYPE_ALERT = 3;
    /**
	 * Content Availability Remote Notification Type.
	 * <br> @version 3.9
	 * @type int
	 */
	this.REMOTE_NOTIFICATION_TYPE_CONTENT_AVAILABILITY = 4;
	
	/**
	 * No-Repeat Interval for Local Notification.
	 * <br> @version 3.9
	 * @type int
	 */
	this.LOCAL_NOTIFICATION_REPEAT_INTERVAL_NO_REPEAT = 0;
	
	/**
	 * Hourly Repeat Interval for Local Notification.
	 * <br> @version 3.9
	 * @type int
	 */
	this.LOCAL_NOTIFICATION_REPEAT_INTERVAL_HOURLY = 1;
	
	/**
	 * Daily Repeat Interval for Local Notification.
	 * <br> @version 3.9
	 * @type int
	 */
	this.LOCAL_NOTIFICATION_REPEAT_INTERVAL_DAILY = 2;
	
	/**
	 * Weekly Repeat Interval for Local Notification.
	 * <br> @version 3.9
	 * @type int
	 */
	this.LOCAL_NOTIFICATION_REPEAT_INTERVAL_WEEKLY = 3;

	/**
	 * Monthly Repeat Interval for Local Notification.
	 * <br> @version 3.9
	 * @type int
	 */
	this.LOCAL_NOTIFICATION_REPEAT_INTERVAL_MONTHLY = 4;
	
	/**
	 * Yearly Repeat Interval for Local Notification.
	 * <br> @version 3.9
	 * @type int
	 */
	this.LOCAL_NOTIFICATION_REPEAT_INTERVAL_YEARLY = 5;
	
	/**
	 * Default registration exception code for remote notifications.
	 * <br> @version 4.0
	 * @type String
	 */
	this.REMOTE_NOTIFICATION_REGISTRATION_FAILURE_DEFAULT = "99";
	
	/**
	 * Registration exception code for remote notifications indicating unsuccessful registration due to a different sender id previous registration.
	 * <br> @version 4.0
	 * @type String
	 */
	this.REMOTE_NOTIFICATION_REGISTRATION_FAILURE_MISMATCH_SENDERID = "10";
	
	/**
	 * Registration exception code send by the GCM Server for remote notifications (both registration and unregistration processes)
	 * <br> @version 4.0
	 * @type String
	 */
	this.REMOTE_NOTIFICATION_REGISTRATION_FAILURE_GCM_SERVER = "11";
}

Appverse.Notification = new Notification();

/**
 * Shows and starts the activity indicator animation.
 * <br> @version 1.0
 * @return {Boolean} True if activity indicator could be started.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StartNotifyActivity = function()
{
	return post_to_url(Appverse.Notification.serviceName, "StartNotifyActivity", null, "POST");
};

/**
 * Stops and hides the activity indicator animation.
 * <br> @version 1.0
 * @return {Boolean} True if activity indicator could be stopped.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StopNotifyActivity = function()
{
	return post_to_url(Appverse.Notification.serviceName, "StopNotifyActivity", null, "POST");
};

/**
 * Checks if activity indicator animation is started.
 * <br> @version 1.0
 * @return {Boolean} True/false wheter activity indicator is running.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.IsNotifyActivityRunning = function()
{
	return post_to_url(Appverse.Notification.serviceName, "IsNotifyActivityRunning", null, "POST");
};

/**
 * Starts an alert notification.
 * <br> @version 1.0
 * @param {String} message The alert message to be displayed.
 * @param {String} title The alert title to be displayed.
 * @param {String} buttonText The accept button text to be displayed.
 * @return {Boolean} True if alert notification could be started.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StartNotifyAlert = function(message, title, buttonText)
{
	if(title == null && buttonText == null) {
		return post_to_url(Appverse.Notification.serviceName, "StartNotifyAlert", get_params([message]), "POST");
	} else {
		return post_to_url(Appverse.Notification.serviceName, "StartNotifyAlert", get_params([title,message,buttonText]), "POST");
	}
};

/**
 * Stops an alert notification.
 * <br> @version 1.0
 * @return {Boolean} True if alert notification could be stopped.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StopNotifyAlert = function()
{
	return post_to_url(Appverse.Notification.serviceName, "StopNotifyAlert", null, "POST");
};

/**
 * Shows an action sheet.
 * <br> @version 1.0
 * @param {String} title The title to be displayed on the action sheet.
 * @param {String[]} buttons Array of button texts to be displayed. First index button is the "cancel" button, default button.
 * @param {String[]} jsCallbackFunctions The callback javascript functions as string texts for each of the given buttons. Empty string if no action is required for a button.
 * @return {Boolean} True if action sheet could be showed.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Notification.prototype.StartNotifyActionSheet = function(title, buttons, jsCallbackFunctions)
{
	return post_to_url(Appverse.Notification.serviceName, "StartNotifyActionSheet", get_params([title, buttons, jsCallbackFunctions]), "POST");
};

/**
 * Starts a beep notification.
 * <br> @version 1.0
 * @return {Boolean} True if beep notification could be started.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StartNotifyBeep = function()
{
	return post_to_url(Appverse.Notification.serviceName, "StartNotifyBeep", null, "POST");
};

/**
 * Stops the current beep notification.
 * <br> @version 1.0
 * @return {Boolean} True if beep notification could be stopped.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StopNotifyBeep = function()
{
	return post_to_url(Appverse.Notification.serviceName, "StopNotifyBeep", null, "POST");
};

/**
 * Starts a blink notification.
 * <br> @version 1.0
 * @return {Boolean} True if beep notification could be started.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Notification.prototype.StartNotifyBlink = function()
{
	return post_to_url(Appverse.Notification.serviceName, "StartNotifyBlink", null, "POST");
};

/**
 * Stops the current blink notification.
 * <br> @version 1.0
 * @return {Boolean} True if blink notification could be stopped.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Notification.prototype.StopNotifyBlink = function()
{
	return post_to_url(Appverse.Notification.serviceName, "StopNotifyBlink", null, "POST");
};

/**
 * Shows and starts the progress indicator animation.
 * <br> @version 1.0
 * @return {Boolean} True if progress indicator animation could be started.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/warning.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> 
 * <br><br><img src="resources/images/warning.png"/> Showing the native loading window in <b>Android</b> is currently sending the application to background.
 * <br>This means that the platform server (Appverse) is no available till application comes to foreground again.
 * <br>But application could not wake up itself to foreground from javascript code.
 * <br>So, we strongly recommend you to do not use this function till we solve this problem from platform side.
 * <br>Use HTML5/JS/CSS3 loading overlays instead.
 * </pre>
 */
Notification.prototype.StartNotifyLoading = function(loadingText)
{
	if(loadingText == null) {
		return post_to_url(Appverse.Notification.serviceName, "StartNotifyLoading", null, "POST");
	} else {
		return post_to_url(Appverse.Notification.serviceName, "StartNotifyLoading", get_params([loadingText]), "POST");
	}
};

/**
 * Stops the current progress indicator animation.
 * <br> @version 1.0
 * @return {Boolean} True if progress indicator animation could be stopped.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StopNotifyLoading = function()
{
	return post_to_url(Appverse.Notification.serviceName, "StopNotifyLoading", null, "POST");
};

/**
 * Checks if progress indicator animation is started.
 * <br> @version 1.0
 * @return {Boolean} True/false wheter progress indicator is running.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.IsNotifyLoadingRunning = function()
{
	return post_to_url(Appverse.Notification.serviceName, "IsNotifyLoadingRunning", null, "POST");
};

/**
 * Updates the progress indicator animation.
 * <br> @version 1.0
 * @param {float} progress The current progress; values between 0.0 and 1.0 (completed).
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.UpdateNotifyLoading = function(progress)
{
	return post_to_url(Appverse.Notification.serviceName, "UpdateNotifyLoading", get_params([progress]), "POST");
};

/**
 * Starts a vibration notification.
 * <br> @version 1.0
 * @return {Boolean} True if vibration notification could be started.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Notification.prototype.StartNotifyVibrate = function()
{
	return post_to_url(Appverse.Notification.serviceName, "StartNotifyVibrate", null, "POST");
};

/**
 * Stops the current vibration notification.
 * <br> @version 1.0
 * @return {Boolean} True if vibration notification could be stopped.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Notification.prototype.StopNotifyVibrate = function()
{
	return post_to_url(Appverse.Notification.serviceName, "StopNotifyVibrate", null, "POST");
};

/**
 * Registers this application and device for receiving remote notifications.
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnRegisterForRemoteNotificationsSuccess and Appverse.OnRegisterForRemoteNotificationsFailure
 * <br> @version 3.9
 * @method
 * @param {String} senderId The sender identifier. This parameter is required for some platforms (such as the Android platform), in iOS will be just ignored.
 * @param {Appverse.Notification.RemoteNotificationType[]} types The remote notifications types accepted by this application. For further information see, {@link Appverse.Notification#REMOTE_NOTIFICATION_TYPE_NONE REMOTE_NOTIFICATION_TYPE_NONE}, {@link Appverse.Notification#REMOTE_NOTIFICATION_TYPE_BADGE REMOTE_NOTIFICATION_TYPE_BADGE}, {@link Appverse.Notification#REMOTE_NOTIFICATION_TYPE_SOUND REMOTE_NOTIFICATION_TYPE_SOUND}, {@link Appverse.Notification#REMOTE_NOTIFICATION_TYPE_ALERT REMOTE_NOTIFICATION_TYPE_ALERT} and {@link Appverse.Notification#REMOTE_NOTIFICATION_TYPE_CONTENT_AVAILABILITY REMOTE_NOTIFICATION_TYPE_CONTENT_AVAILABILITY}
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.RegisterForRemoteNotifications = function(senderId, types) 
{
	post_to_url(Appverse.Notification.serviceName, "RegisterForRemoteNotifications", get_params([senderId, types]), "POST");
};

/**
 * Un-registers this application and device from receiving remote notifications.
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnUnRegisterForRemoteNotificationsSuccess
 * <br> @version 3.9 (listener callback only available on 4.0)
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.UnRegisterForRemoteNotifications = function() 
{
	post_to_url(Appverse.Notification.serviceName, "UnRegisterForRemoteNotifications", null, "POST");
};

/**
 * Sets the current application icon badge number (the one inside the red bubble).
 * <br> @version 3.9
 * @method
 * @param {int} badge The badge number to set.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> N/A  | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.SetApplicationIconBadgeNumber = function(badge) 
{
	post_to_url(Appverse.Notification.serviceName, "SetApplicationIconBadgeNumber", get_params([badge]), "POST");
};

/**
 * Increments (adds one to) the current application icon badge number (the one inside the red bubble).
 * <br> @version 3.9
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> N/A  | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.IncrementApplicationIconBadgeNumber = function() 
{
	post_to_url(Appverse.Notification.serviceName, "IncrementApplicationIconBadgeNumber", null, "POST");
};

/**
 * Decrements (substracts one from) the current application icon badge number (the one inside the red bubble).
 * <br> @version 3.9
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> N/A  | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.DecrementApplicationIconBadgeNumber = function() 
{
	post_to_url(Appverse.Notification.serviceName, "DecrementApplicationIconBadgeNumber", null, "POST");
};

/**
 * Presents a local notification immediately for the current application.
 * <br> @version 3.9
 * @method
 * @param {Appverse.Notification.NotificationData} notification The notification data to be presented. For further information see, {@link Appverse.Notification.NotificationData NotificationData}.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.PresentLocalNotificationNow = function(notification) 
{
	post_to_url(Appverse.Notification.serviceName, "PresentLocalNotificationNow", get_params([notification]), "POST");
};

/**
 * chedules a local notification fo delivery on a scheduled date and time.
 * <br> @version 3.9
 * @method
 * @param {Appverse.Notification.NotificationData} notification The notification data to be presented. For further information see, {@link Appverse.Notification.NotificationData NotificationData}.
 * @param {SchedulingData} schedule The scheduling data with the fire date. For further information see, {@link Appverse.Notification.SchedulingData SchedulingData}.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.ScheduleLocalNotification = function(notification, schedule) 
{
	post_to_url(Appverse.Notification.serviceName, "ScheduleLocalNotification", get_params([notification, schedule]), "POST");
};

/**
 * Cancels a local notification given its fire date.
 * The fire date is the notification unique identifier, only 1 notification could be scheduled for the same fire date... last scheduled wins!
 * <br> @version 3.9
 * @method
 * @param {Appverse.DateTime} fireDate The local notification fire date identifier to be cancelled.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.CancelLocalNotification = function(fireDate) 
{
	post_to_url(Appverse.Notification.serviceName, "CancelLocalNotification", get_params([fireDate]), "POST");
};

/**
 * Cancels all local notifications already scheduled.
 * <br> @version 3.9
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.CancelAllLocalNotifications = function() 
{
	post_to_url(Appverse.Notification.serviceName, "CancelAllLocalNotifications", null, "POST");
};

/**
 * @class Appverse.Notification.Async
 * Invokes all Notification API methods asynchronously.
 * <br>
 * Callback function name and callback identifier are passed to the methods (last arguments) to handle the result object when it is received from appverse runtime.
 * <pre>Usage:
 * <br> var myCallbackFn = function(result, id){ <br>	...//code here your custom functionality to handle the result... <br>}
 * <br>Appverse.Notification.<b>Async</b>.StartNotifyActivity('myCallbackFn', 'myId').
 * <br>or
 * <br>Appverse.Notification.<b>Async</b>.StartNotifyLoading('loading text', 'myCallbackFn', 'myId').
 * </pre>
 */
Notification.prototype.Async = {

/**
 * Shows and starts the activity indicator animation, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
StartNotifyActivity : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "StartNotifyActivity", null, callbackFunctionName, callbackId);
},

/**
 * Stops and hides the activity indicator animation, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
StopNotifyActivity : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "StopNotifyActivity", null, "POST",callbackFunctionName, callbackId);
},

/**
 * Checks if activity indicator animation is started, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
IsNotifyActivityRunning : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "IsNotifyActivityRunning", null, callbackFunctionName, callbackId);
},

/**
 * Starts an alert notification, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} message The alert message to be displayed.
 * @param {String} title The alert title to be displayed.
 * @param {String} buttonText The accept button text to be displayed.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
StartNotifyAlert : function(message, title, buttonText, callbackFunctionName, callbackId)
{
	if(title == null && buttonText == null) {
		post_to_url_async(Appverse.Notification.serviceName, "StartNotifyAlert", get_params([message]), callbackFunctionName, callbackId);
	} else {
		post_to_url_async(Appverse.Notification.serviceName, "StartNotifyAlert", get_params([title,message,buttonText]), callbackFunctionName, callbackId);
	}
},

/**
 * Stops an alert notification, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
StopNotifyAlert : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "StopNotifyAlert", null, callbackFunctionName, callbackId);
},

/**
 * Shows an action sheet, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} title The title to be displayed on the action sheet.
 * @param {String[]} buttons Array of button texts to be displayed. First index button is the "cancel" button, default button.
 * @param {String[]} jsCallbackFunctions The callback javascript functions as string texts for each of the given buttons. Empty string if no action is required for a button.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
StartNotifyActionSheet : function(title, buttons, jsCallbackFunctions, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "StartNotifyActionSheet", get_params([title, buttons, jsCallbackFunctions]), callbackFunctionName, callbackId);
},

/**
 * Starts a beep notification, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
StartNotifyBeep : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "StartNotifyBeep", null, callbackFunctionName, callbackId);
},

/**
 * Stops the current beep notification, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
StopNotifyBeep : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "StopNotifyBeep", null, callbackFunctionName, callbackId);
},

/**
 * Starts a blink notification, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
StartNotifyBlink : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "StartNotifyBlink", null, callbackFunctionName, callbackId);
},

/**
 * Stops the current blink notification, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
StopNotifyBlink : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "StopNotifyBlink", null, callbackFunctionName, callbackId);
},

/**
 * Shows and starts the progress indicator animation, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/warning.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> 
 * <br><br><img src="resources/images/warning.png"/> Showing the native loading window in <b>Android</b> is currently sending the application to background.
 * <br>This means that the platform server (Appverse) is no available till application comes to foreground again.
 * <br>But application could not wake up itself to foreground from javascript code.
 * <br>So, we strongly recommend you to do not use this function till we solve this problem from platform side.
 * <br>Use HTML5/JS/CSS3 loading overlays instead.
 * </pre>
 */
StartNotifyLoading : function(loadingText, callbackFunctionName, callbackId)
{
	if(loadingText == null) {
		post_to_url_async(Appverse.Notification.serviceName, "StartNotifyLoading", null, callbackFunctionName, callbackId);
	} else {
		post_to_url_async(Appverse.Notification.serviceName, "StartNotifyLoading", get_params([loadingText]), callbackFunctionName, callbackId);
	}
},

/**
 * Stops the current progress indicator animation, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
StopNotifyLoading : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "StopNotifyLoading", null, callbackFunctionName, callbackId);
},

/**
 * Checks if progress indicator animation is started, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
IsNotifyLoadingRunning : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "IsNotifyLoadingRunning", null, callbackFunctionName, callbackId);
},

/**
 * Updates the progress indicator animation, in ASYNC mode.
 * <br> @version 2.0
 * @param {float} progress The current progress; values between 0.0 and 1.0 (completed).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
UpdateNotifyLoading : function(progress, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "UpdateNotifyLoading", get_params([progress]), callbackFunctionName, callbackId);
},

/**
 * Starts a vibration notification, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
StartNotifyVibrate : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "StartNotifyVibrate", null, callbackFunctionName, callbackId);
},

/**
 * Stops the current vibration notification, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
StopNotifyVibrate : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Notification.serviceName, "StopNotifyVibrate", null, callbackFunctionName, callbackId);
}

};

/*
 * I/O INTERFACES
 */

/**
 * @class Appverse.IO 
 * Singleton class field to access IO interface. 
 * <br>This interface provides communication with external services, such as WebServices or Servlets... in many formats: JSON, XML, etx.<br>
 * <br> @version 1.0
 * <pre>Usage: Appverse.IO.&lt;metodName&gt;([params]).<br>Example: Appverse.IO.GetService(serviceName).</pre>
 * @singleton
 * @constructor Constructs a new IO interface.
 * @return {Appverse.IO} A new IO interface.
 */
IO = function() {
	/**
	 * IO service name (as configured on Platform Service Locator).
 	 * <br> @version 1.0
	 * @type String
	 */
	this.serviceName = "io";
	/**
	 * SOAP XML Service Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.SERVICETYPE_SOAP_XML = 0;
	/**
	 * SOAP JSON Service Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.SERVICETYPE_SOAP_JSON = 1;
	/**
	 * XML RPC Service Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.SERVICETYPE_XMLRPC_XML = 2;
	/**
	 * REST XML Service Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.SERVICETYPE_REST_XML = 3;
	/**
	 * JSON RPC Service Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.SERVICETYPE_XMLRPC_JSON = 4;
	/**
	 * REST JSON Service Type.
	 * @type int
 	 * <br> @version 1.0
	 */
	this.SERVICETYPE_REST_JSON = 5;
	/**
	 * AMF Serialization Service Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.SERVICETYPE_AMF_SERIALIZATION = 6;
	/**
	 * Remoting Serialization Service Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.SERVICETYPE_REMOTING_SERIALIZATION = 7;
	/**
	 * Octet Binary Service Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.SERVICETYPE_OCTET_BINARY = 8;
	
	/**
	 * GWT RPC Service Type.
 	 * <br> @version 2.1
	 * @type int
	 */
	this.SERVICETYPE_GWT_RPC = 9;
	
	/**
	 * HTTP Protocol Version "HTTP/1.0".
 	 * <br> @version 3.8
	 * @type int
	 */
	this.HTTP_PROTOCOL_VERSION_1_0 = 0;
	/**
	 * HTTP Protocol Version "HTTP/1.1".
 	 * <br> @version 3.8
	 * @type int
	 */
	this.HTTP_PROTOCOL_VERSION_1_1 = 1;
}

Appverse.IO = new IO();

/**
 * Gets the configured I/O services (the ones configured on the '/app/config/io-services-config.xml' file).<br/>For further information see, {@link Appverse.IO.IOService IOService}.
 * <br> @version 1.0
 * @return {Appverse.IO.IOService[]} List of external services.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
IO.prototype.GetServices = function()
{
	return post_to_url(Appverse.IO.serviceName, "GetServices", null, "POST");
};

/**
 * Gets the I/O Service that matches the given name, and type (if provided). It is possible to define two services with the same name, but different type.
 * <br/>For further information see, {@link Appverse.IO.IOService IOService}.
 * <br> @version 1.0
 * <br/>Possible values of service type: 
 * {@link Appverse.IO#SERVICETYPE_AMF_SERIALIZATION SERVICETYPE_AMF_SERIALIZATION}, 
 * {@link Appverse.IO#SERVICETYPE_GWT_RPC SERVICETYPE_GWT_RPC}, 
 * {@link Appverse.IO#SERVICETYPE_OCTET_BINARY SERVICETYPE_OCTET_BINARY}, 
 * {@link Appverse.IO#SERVICETYPE_REMOTING_SERIALIZATION SERVICETYPE_REMOTING_SERIALIZATION}, 
 * {@link Appverse.IO#SERVICETYPE_REST_JSON SERVICETYPE_REST_JSON}, 
 * {@link Appverse.IO#SERVICETYPE_REST_XML SERVICETYPE_REST_XML},
 * {@link Appverse.IO#SERVICETYPE_SOAP_JSON SERVICETYPE_SOAP_JSON} ,
 * {@link Appverse.IO#SERVICETYPE_SOAP_XML SERVICETYPE_SOAP_XML},
 * {@link Appverse.IO#SERVICETYPE_XMLRPC_JSON SERVICETYPE_XMLRPC_JSON},
 * & {@link Appverse.IO#SERVICETYPE_XMLRPC_XML SERVICETYPE_XMLRPC_XML}
 * @param {String} serviceName The service name to look for.
 * @param {int} serviceType The service type to look for. Optional parameter.
 * @return {Appverse.IO.IOService} The external service matched.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
IO.prototype.GetService = function(serviceName, serviceType)
{
	if(serviceType == null) {
		return post_to_url(Appverse.IO.serviceName, "GetService",get_params([serviceName]), "POST");
	} else {
		return post_to_url(Appverse.IO.serviceName, "GetService",get_params([serviceName,serviceType]), "POST");
	}
};

/**
 * Invokes the I/O Service that matches the given service name (or service object reference), and type (if provided).
 * <br/>For further information see, {@link Appverse.IO.IOService IOService}, {@link Appverse.IO.IORequest IORequest} and {@link Appverse.IO.IOResponse IOResponse}.
 * <br> @version 1.0
 * <br/>Possible values of service type: 
 * {@link Appverse.IO#SERVICETYPE_AMF_SERIALIZATION SERVICETYPE_AMF_SERIALIZATION}, 
 * {@link Appverse.IO#SERVICETYPE_GWT_RPC SERVICETYPE_GWT_RPC}, 
 * {@link Appverse.IO#SERVICETYPE_OCTET_BINARY SERVICETYPE_OCTET_BINARY}, 
 * {@link Appverse.IO#SERVICETYPE_REMOTING_SERIALIZATION SERVICETYPE_REMOTING_SERIALIZATION}, 
 * {@link Appverse.IO#SERVICETYPE_REST_JSON SERVICETYPE_REST_JSON}, 
 * {@link Appverse.IO#SERVICETYPE_REST_XML SERVICETYPE_REST_XML},
 * {@link Appverse.IO#SERVICETYPE_SOAP_JSON SERVICETYPE_SOAP_JSON} ,
 * {@link Appverse.IO#SERVICETYPE_SOAP_XML SERVICETYPE_SOAP_XML},
 * {@link Appverse.IO#SERVICETYPE_XMLRPC_JSON SERVICETYPE_XMLRPC_JSON},
 * & {@link Appverse.IO#SERVICETYPE_XMLRPC_XML SERVICETYPE_XMLRPC_XML}
 * @param {Appverse.IO.IORequest} requestObjt The request object with the needed invocation parameters. Example:<pre>{"Session":null,"Content":"{method:authenticationService.login,id:1,params:['username','password']}"}</pre>
 * @param {String/Appverse.IO.IOService} service This param could be a IOService object (as provided by {@link #GetService}), or only the service name. First service match would be invoked. ATTENTION: when using the 'object', the third argument (type) shouldn't be informed.
 * @param {int} serviceType The service type to look for. Optional parameter. Just inform this when you pass the service name in the second argument.
 * @return {Appverse.IO.IOResponse} The response object returned from remote service. Access content doing: <pre>ioResponse.Content</pre>
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
IO.prototype.InvokeService = function(requestObjt, service, serviceType)
{
	if(serviceType == null) {
		return post_to_url(Appverse.IO.serviceName, "InvokeService",get_params([requestObjt,service]), "POST");
	} else {
		return post_to_url(Appverse.IO.serviceName, "InvokeService",get_params([requestObjt,service,serviceType]), "POST");
	}
};

/**
 * Invokes the I/O Service (that matches the given service object reference) for retreiving a file (specially big ones) and stores it locally (under given store path)
 * Only {@link Appverse.IO#SERVICETYPE_OCTET_BINARY SERVICETYPE_OCTET_BINARY} types are allowed in this method.
 * <br/>For further information see, {@link Appverse.IO.IOService IOService} and {@link Appverse.IO.IORequest IORequest}.
 * <br> @version 4.0
 * @param {Appverse.IO.IORequest} requestObjt The request object with the needed invocation parameters. Example:<pre>{"Session":null,"Content":"{method:authenticationService.login,id:1,params:['username','password']}"}</pre>
 * @param {Appverse.IO.IOService} service This param could be a IOService object (as provided by {@link #GetService}), or only the service name. First service match would be invoked. ATTENTION: when using the 'object', the third argument (type) shouldn't be informed.
 * @param {String} storePath The relative path (under application documents root direectory) to store the contents received from this service invocation.
 * @return {String} The reference url for the stored file, or null on error case. If store file is a temporal file, application should remove it when no more needed.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
IO.prototype.InvokeServiceForBinary = function(requestObjt, service, storePath)
{
	return post_to_url(Appverse.IO.serviceName, "InvokeServiceForBinary",get_params([requestObjt,service,storePath]), "POST");
};

/**
 * @class Appverse.IO.Async
 * Invokes all IO API methods asynchronously.
 * <br>
 * Callback function name and callback identifier are passed to the methods (last arguments) to handle the result object when it is received from appverse runtime.
 * <pre>Usage:
 * <br> var myCallbackFn = function(result, id){ <br>	...//code here your custom functionality to handle the result... <br>}
 * <br>Appverse.IO.<b>Async</b>.GetServices('myCallbackFn', 'myId').
 * <br>or
 * <br>Appverse.IO.<b>Async</b>.InvokeService(requestObjt, 'serviceName', 4, 'myCallbackFn', 'myId')
 * <br>or
 * <br>Appverse.IO.<b>Async</b>.InvokeService(requestObjt, serviceObjt, 'myCallbackFn', 'myId')
 * </pre>
 */
IO.prototype.Async = {

/**
 * Gets ASYNC the configured I/O services (the ones configured on the '/app/config/io-services-config.xml' file).
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
GetServices : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.IO.serviceName, "GetServices", null, callbackFunctionName, callbackId);
},

/**
 * Gets ASYNC the I/O Service that matches the given name, and type (if provided). It is possible to define two services with the same name, but different type.
 * <br> @version 2.0
 * @param {String} serviceName The service name to look for.
 * @param {int} serviceType The service type to look for. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
GetService : function(serviceName, serviceType, callbackFunctionName, callbackId)
{
	if(serviceType == null) {
        post_to_url_async(Appverse.IO.serviceName, "GetService",get_params([serviceName]), callbackFunctionName, callbackId);
	} else {
        post_to_url_async(Appverse.IO.serviceName, "GetService",get_params([serviceName,serviceType]), callbackFunctionName, callbackId);
	}
},

/**
 * Invokes ASYNC the I/O Service that matches the given service name (or service object reference), and type (if provided).
 * <br> @version 2.0
 * @param {Appverse.IO.IORequest} requestObjt The request object with the needed invocation parameters. Example:<pre>{"Session":null,"Content":"{method:authenticationService.login,id:1,params:['username','password']}"}</pre>
 * @param {String/Appverse.IO.IOService} service This param could be a IOService object (as provided by {@link #GetService}), or only the service name. First service match would be invoked. ATTENTION: when using the 'object', the third argument (type) shouldn't be informed.
 * @param {int} serviceType The service type to look for. Optional parameter. Just inform this when you pass the service name in the second argument.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
InvokeService : function(requestObjt, service, serviceType, callbackFunctionName, callbackId)
{
	if(serviceType == null) {
        post_to_url_async(Appverse.IO.serviceName, "InvokeService",get_params([requestObjt,service]), callbackFunctionName, callbackId);
	} else {
		post_to_url_async(Appverse.IO.serviceName, "InvokeService",get_params([requestObjt,service,serviceType]), callbackFunctionName, callbackId);
	}
},

/**
 * Invokes ASYNC the I/O Service (that matches the given service object reference) for retreiving a file (specially big ones) and stores it locally (under given store path)
 * Only {@link Appverse.IO#SERVICETYPE_OCTET_BINARY SERVICETYPE_OCTET_BINARY} types are allowed in this method.
 * <br/>For further information see, {@link Appverse.IO.IOService IOService} and {@link Appverse.IO.IORequest IORequest}.
 * <br> @version 4.0
 * @param {Appverse.IO.IORequest} requestObjt The request object with the needed invocation parameters. Example:<pre>{"Session":null,"Content":"{method:authenticationService.login,id:1,params:['username','password']}"}</pre>
 * @param {Appverse.IO.IOService} service This param could be a IOService object (as provided by {@link #GetService}), or only the service name. First service match would be invoked. ATTENTION: when using the 'object', the third argument (type) shouldn't be informed.
 * @param {String} storePath The relative path (under application documents root direectory) to store the contents received from this service invocation.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
InvokeServiceForBinary : function(requestObjt, service, storePath, callbackFunctionName, callbackId)
{
	return post_to_url_async(Appverse.IO.serviceName, "InvokeServiceForBinary",get_params([requestObjt,service,storePath]), callbackFunctionName, callbackId);
}

};

/*
 * GEO INTERFACES
 */
 
/**
 * @class Appverse.Geo 
 * Singleton class field to access Geo interface. 
 * <br>This interface provides access to GPS device features (getting current location, acceleration, etc) and embedded Map views, to locate/handle Points of Interest.<br>
 * <br> @version 1.0
 * <pre>Usage: Appverse.Geo.&lt;metodName&gt;([params]).<br>Example: Appverse.Geo.GetAcceleration().</pre>
 * @singleton
 * @constructor Constructs a new Geo interface.
 * @return {Appverse.Geo} A new Geo interface.
 */
Geo = function() {
	/**
	 * Geo service name (as configured on Platform Service Locator).
 	 * <br> @version 1.0
	 * @type String
	 */
	this.serviceName = "geo";
	/**
	 * Magnetic North Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.NORTHTYPE_MAGNETIC = 0;
	/**
	 * True North Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.NORTHTYPE_TRUE	= 1;
}

Appverse.Geo = new Geo();

/**
 * Gets the current device acceleration (measured in meters/second/second). <br/>For further information see, {@link Appverse.Geo.Acceleration Acceleration}.
 * <br> @version 1.0
 * @return {Appverse.Geo.Acceleration} Current acceleration info (coordinates and acceleration vector number).
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetAcceleration = function()
{
	return post_to_url(Appverse.Geo.serviceName, "GetAcceleration", null, "POST");
};

/**
 * Gets the current device location coordinates. <br/>For further information see, {@link Appverse.Geo.LocationCoordinate LocationCoordinate}.
 * <br> @version 1.0
 * @return {Appverse.Geo.LocationCoordinate} Current location info (coordinates and precision).
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetCoordinates = function()
{
	return post_to_url(Appverse.Geo.serviceName, "GetCoordinates", null, "POST");
};

/**
 * Gets the heading relative to the given north type (if 'northType' is not provided, default is used: magnetic noth pole).
 * <br> @version 1.0
 * <br/>Possible values of north type: 
 * {@link Appverse.Geo#NORTHTYPE_MAGNETIC NORTHTYPE_MAGNETIC}, 
 * & {@link Appverse.Geo#NORTHTYPE_TRUE NORTHTYPE_TRUE}
 * @param {int} northType Type of north to measured heading relative to it. Optional parameter.
 * @return {float} Current heading. Measured in degrees, minutes and seconds.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetHeading = function(northType)
{
	var headingString = "0";
	if(northType == null) {
		headingString = post_to_url(Appverse.Geo.serviceName, "GetHeading", null, "POST", true);  // "true" to get value as string, and parse to float here
	} else { 
		headingString = post_to_url(Appverse.Geo.serviceName, "GetHeading", get_params([northType]), "POST", true); // "true" to get value as string, and parse to float here
	}
	headingString = headingString.replace(/,/, '.');  // change comma to points, if case.
	return parseFloat(headingString);
};

/**
 * Gets the orientation relative to the magnetic north pole.
 * <br> @version 1.0
 * @return {float} Current orientation. Measured in degrees, minutes and seconds.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetDeviceOrientation = function()
{
	var orientationString = post_to_url(Appverse.Geo.serviceName, "GetDeviceOrientation", null, "POST", true); // "true" to get value as string, and parse to float here
	orientationString = orientationString.replace(/,/, '.');  // change comma to points, if case.
	return parseFloat(orientationString);
};

/**
 * Gets the current device velocity.
 * <br> @version 1.0
 * @return {float} Device speed (in meters/second).
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetVelocity = function()
{
	var velocityString = post_to_url(Appverse.Geo.serviceName, "GetVelocity", null, "POST", true); // "true" to get value as string, and parse to float here
	velocityString = velocityString.replace(/,/, '.');  // change comma to points, if case.
	return parseFloat(velocityString);
};

/**
 * Shows Map on screen.
 * <br> @version 1.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Geo.prototype.GetMap = function()
{
	return post_to_url(Appverse.Geo.serviceName, "GetMap", null, "POST");
};

/**
 * Specifies current map scale and bounding box radius.
 * <br> @version 1.0
 * @param {float} scale The desired map scale.
 * @param {float} boundingBox The desired map view bounding box.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Geo.prototype.SetMapSettings = function(scale, boundingBox)
{
	return post_to_url(Appverse.Geo.serviceName, "SetMapSettings", get_params([scale,boundingBox]), "POST");
};

/**
 * List of POIs for the current location, given a radius (bounding box). Optionaly, a query text and/or a category could be added to search for specific conditions.
 * <br/>For further information see, {@link Appverse.Geo.POI POI}.
 * <br> @version 1.0
 * @param {Appverse.Geo.LocationCoordinate} location Map location point to search nearest POIs.
 * @param {float} radius The radius around location to search POIs in.
 * @param {String} queryText The query to search POIs.. Optional parameter.
 * @param {Appverse.Geo.LocationCategory} category The query to search POIs.. Optional parameter.
 * @return {Appverse.Geo.POI[]} Points of Interest for location, ordered by distance.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Geo.prototype.GetPOIList = function(location, radius, queryText, category)
{
	if(queryText == null && category == null) {
		return post_to_url(Appverse.Geo.serviceName, "GetPOIList", get_params([location,radius]), "POST");
	} else if(queryText != null && category == null) {
		return post_to_url(Appverse.Geo.serviceName, "GetPOIList", get_params([location,radius,queryText]), "POST");
	} else if(queryText == null && category != null) {
		return post_to_url(Appverse.Geo.serviceName, "GetPOIList", get_params([location,radius,category]), "POST");
	} else {
		return post_to_url(Appverse.Geo.serviceName, "GetPOIList", get_params([location,radius,queryText,category]), "POST");
	}
};

/**
 * Gets a POI by the given id. <br/>For further information see, {@link Appverse.Geo.POI POI}.
 * <br> @version 1.0
 * @param {String} poiId POI identifier.
 * @return {Appverse.Geo.POI} Point of Interest found.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Geo.prototype.GetPOI = function(poiId)
{
	return post_to_url(Appverse.Geo.serviceName, "GetPOI", get_params([poiId]), "POST");
};

/**
 * Removes a POI given its id. <br/>For further information see, {@link Appverse.Geo.POI POI}.
 * <br> @version 1.0
 * @param {String} poiId POI identifier.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Geo.prototype.RemovePOI = function(poiId)
{
	return post_to_url(Appverse.Geo.serviceName, "RemovePOI", get_params([poiId]), "POST");
};

/**
 * Moves a POI - given its id - to target location. <br/>For further information see, {@link Appverse.Geo.POI POI}.
 * <br> @version 1.0
 * @param {String} poiId POI identifier.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Geo.prototype.UpdatePOI = function(poi)
{
	return post_to_url(Appverse.Geo.serviceName, "UpdatePOI", get_params([poi]), "POST");
};

/**
 * Starts the location services in order to get the latitude, longitude, altitude, speed, etc.
 * <br> @version 1.0
 * @return {Boolean} True if the device can start the location services
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StartUpdatingLocation = function()
{
	return post_to_url(Appverse.Geo.serviceName, "StartUpdatingLocation", null, "POST");
};

/**
 * Stops the location services in order to get the latitude, longitude, altitude, speed, etc.
 * <br> @version 1.0
 * @return {Boolean} True if the device can stop the location services
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StopUpdatingLocation = function()
{
	return post_to_url(Appverse.Geo.serviceName, "StopUpdatingLocation", null, "POST");
};

/**
 * Starts the heading services in order to get the latitude, longitude, altitude, speed, etc.
 * <br> @version 1.0
 * @return {Boolean} True if the device can start the location services
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StartUpdatingHeading = function()
{
	return post_to_url(Appverse.Geo.serviceName, "StartUpdatingHeading", null, "POST");
};

/**
 * Stops the heading services in order to get the latitude, longitude, altitude, speed, etc.
 * <br> @version 1.0
 * @return {Boolean} True if the device can stop the location services
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StopUpdatingHeading = function()
{
	return post_to_url(Appverse.Geo.serviceName, "StopUpdatingHeading", null, "POST");
};

/**
 * Performs a reverse geocoding in order to get, from the present latitude and longitude,
 * attributes like "County", "Street", "County code", "Location", ... in case such attributes
 * are available for that location.
 * <br/>For further information see, {@link Appverse.Geo.GeoDecoderAttributes GeoDecoderAttributes}.
 * <br> @version 1.0
 * @return {Appverse.Geo.GeoDecoderAttributes} Reverse geocoding attributes from the present location (latitude and longitude)
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetGeoDecoder = function()
{
	return post_to_url(Appverse.Geo.serviceName, "GetGeoDecoder", null, "POST");
};

/**
 * The proximity sensor detects an object close to the device.
 * <br> @version 1.0
 * @return {Boolean} True if the proximity sensor detects an object close to the device
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StartProximitySensor = function()
{
	return post_to_url(Appverse.Geo.serviceName, "StartProximitySensor", null, "POST");
};

/**
 * Stops the proximity sensor service.
 * <br> @version 1.0
 * @return {Boolean} True if the proximity sensor service could be stopped.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StopProximitySensor = function()
{
	return post_to_url(Appverse.Geo.serviceName, "StopProximitySensor", null, "POST");
};

/**
 * Determines whether the Location Services (GPS) is enabled.
 * <br> @version 3.8
 * @return {Boolean} True if the device can start the location services
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.IsGPSEnabled = function()
{
	return post_to_url(Appverse.Geo.serviceName, "IsGPSEnabled", null, "POST");
};

/*
 * MEDIA INTERFACES
 */

/**
 * @class Appverse.Media 
 * Singleton class field to access Media interface. 
 * <br>This interface provides access to device's audio/movie player and camera applications.<br>
 * <br> @version 1.0
 * <pre>Usage: Appverse.Media.&lt;metodName&gt;([params]).<br>Example: Appverse.Media.Play(filePath).</pre>
 * @singleton
 * @constructor Constructs a new Media interface.
 * @return {Appverse.Media} A new Media interface.
 */
Media = function() {
	/**
	 * Media service name (as configured on Platform Service Locator).
 	 * <br> @version 1.0
	 * @type String
	 */
	this.serviceName = "media";
	/**
	 * Not Supported Media Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEDIATYPE_NOT_SUPPORTED = 0;
	/**
	 * Audio Media Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEDIATYPE_AUDIO = 1;
	/**
	 * Video Media Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEDIATYPE_VIDEO = 2;
	/**
	 * Photo Media Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEDIATYPE_PHOTO = 3;
	/**
	 * Playing Media State.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEDIATSTATE_PLAYING = 0;
	/**
	 * Recording Media State.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEDIATSTATE_RECORDING = 1;
	/**
	 * Paused Media State.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEDIATSTATE_PAUSED = 2;
	/**
	 * Stopped Media State.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEDIATSTATE_STOPPED = 3;
	/**
	 * Error Media State.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.MEDIATSTATE_ERROR = 4;

	/**
	 * QR Type AddressBook.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.QRTYPE_ADDRESSBOOK = 0;

	/**
	 * QR Type Email.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.QRTYPE_EMAIL_ADDRESS = 1;

	/**
	 * QR Type Product.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.QRTYPE_PRODUCT = 2;

	/**
	 * QR Type URI.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.QRTYPE_URI = 3;

	/**
	 * QR Type Text.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.QRTYPE_TEXT = 4;

	/**
	 * QR Type Geolocation.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.QRTYPE_GEO =5;

	/**
	 * QR Type Telephone.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.QRTYPE_TEL = 6;

	/**
	 * QR Type SMS.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.QRTYPE_SMS = 7;

	/**
	 * QR Type Calendar.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.QRTYPE_CALENDAR = 8;

	/**
	 * QR Type Wifi.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.QRTYPE_WIFI = 9;

	/**
	 * QR Type ISBN.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.QRTYPE_ISBN = 10;

	/**
	 * Barcode Type QR.
 	 * <br> @version 3.9
	 * @type int
	 */
	this.BARCODETYPE_QR = 11;
    
    /**
     * @event onFinishedPickingImage Fired when an image have been picked, either from the Photos library (after calling the {@link Appverse.Media.GetSnapshot GetSnapshot}), 
	 * or from the Camera (after calling the {@link Appverse.Media.TakeSnapshot TakeSnapshot})
	 * <br>Method to be overrided by JS applications, to handle this event.
     * <br> @version 3.1
	 * @param {Appverse.Media.MediaMetadata} mediaMetadata The metadata for the image picked.
     */
    this.onFinishedPickingImage = function(mediaMetadata){};
	
	
	/**
	 * @event onQRCodeDetected Fired when a QR Code has been read, and its data is returned to the app in order to perform the desired javascript code on this case.
	 * <br> For further information see, {@link Appverse.Media.MediaQRContent MediaQRContent}.
	 * <br> Method to be overrided by JS applications, to handle this event.
	 * <br> @version 3.9
	 * @method
	 * @param {Appverse.Media.MediaQRContent} QRCodeContent The scanned QR Code data read
	 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
	 * 
	 */
	this.onQRCodeDetected = function(QRCodeContent) {};

}

Appverse.Media = new Media();

/**
 * Gets Media metadata.<br/>For further information see, {@link Appverse.Media.MediaMetadata MediaMetadata}.
 * <br> @version 1.0
 * @param {String} filePath The media file path.
 * @return {Appverse.Media.MediaMetadata} Media file metadata.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.GetMetadata = function(filePath)
{
	return post_to_url(Appverse.Media.serviceName, "GetMetadata",  get_params([filePath]), "POST");
};

/**
 * Starts playing media.
 * <br> @version 1.0
 * @param {String} filePath The media file path.
 * @return {Boolean} True if media file could be started.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.Play = function(filePath)
{
	return post_to_url(Appverse.Media.serviceName, "Play",  get_params([filePath]), "POST");
};

/**
 * Starts playing media.
 * <br> @version 1.0
 * @param {String} url The media remote URL.
 * @return {Boolean} True if media file could be started.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/warning.png"/> * bug fixing | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.PlayStream = function(url)
{
	return post_to_url(Appverse.Media.serviceName, "PlayStream",  get_params([url]), "POST");
};

/**
 * Moves player to the given position in the media.
 * <br> @version 1.0
 * @param {long} position Index position.
 * @return {Boolean} True if player position could be moved.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.SeekPosition = function(position)
{
	return post_to_url(Appverse.Media.serviceName, "SeekPosition",  get_params([position]), "POST");
};

/**
 * Stops the current media playing.
 * <br> @version 1.0
 * @return {Boolean} True if media file could be stopped.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.Stop = function()
{
	return post_to_url(Appverse.Media.serviceName, "Stop",  null, "POST");
};

/**
 * Pauses the current media playing.
 * <br> @version 1.0
 * @return {Boolean} True if media file could be stopped.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.Pause = function()
{
	return post_to_url(Appverse.Media.serviceName, "Pause",  null, "POST");
};

/**
 * Gets Audio/Movie player state.
 * <br> @version 1.0
 * <br/>Possible values of media states: 
 * {@link Appverse.Media#MEDIATSTATE_ERROR MEDIATSTATE_ERROR}, 
 * {@link Appverse.Media#MEDIATSTATE_PAUSED MEDIATSTATE_PAUSED}, 
 * {@link Appverse.Media#MEDIATSTATE_PLAYING MEDIATSTATE_PLAYING}, 
 * {@link Appverse.Media#MEDIATSTATE_RECORDING MEDIATSTATE_RECORDING}, 
 * & {@link Appverse.Media#MEDIATSTATE_STOPPED MEDIATSTATE_STOPPED}
 * @return {int} Current player state.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.GetState = function()
{
	return post_to_url(Appverse.Media.serviceName, "GetState",  null, "POST");
};

/**
 * Gets the currently playing media file metadata.<br/>For further information see, {@link Appverse.Media.MediaMetadata MediaMetadata}.
 * <br> @version 1.0
 * @return {Appverse.Media.MediaMetadata} Current media file metadata.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/information.png"/> *mock data | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.GetCurrentMedia = function()
{
	return post_to_url(Appverse.Media.serviceName, "GetCurrentMedia",  null, "POST");
};

/**
 * Opens user interface view to select a picture from the device photos album.<br/>For further information see, {@link Appverse.Media.MediaMetadata MediaMetadata}.
 * Data is provided via the proper event handled by the "Appverse.Media.onFinishedPickingImage" method; please, override to handle the event.
 * Returned value is "null" on synchronous call.
 * <br> @version 2.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/warning.png"/> * in progess | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/warning.png"/> *in progress</pre>
 */
Media.prototype.GetSnapshot = function()
{
	return post_to_url(Appverse.Media.serviceName, "GetSnapshot",  null, "POST");
};

/**
 * Opens user interface view to take a picture using the device camera.<br/>For further information see, {@link Appverse.Media.MediaMetadata MediaMetadata}.
 * Data is provided via the proper event handled by the "Appverse.Media.onFinishedPickingImage" method; please, override to handle the event.
 * Returned value is "null" on synchronous call.
 * <br> @version 2.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/warning.png"/> * in progess | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/warning.png"/> *in progress</pre>
 */
Media.prototype.TakeSnapshot = function()
{
	return post_to_url(Appverse.Media.serviceName, "TakeSnapshot",  null, "POST");
};

/**
 * Fires the camera to detected and process a QRCode image.
 * <br> @version 3.9
 * @param {Boolean} autoHandleQR True value to indicates that the detected QRCode should be handled by the platform (if possible) automatically, or False to just be get data returned.
 * QRCode data is provided via the proper event handled by the "Appverse.Media.onQRCodeDetected" method; please, override to handle the event.
 * Returned value is "null" on synchronous call.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.DetectQRCode = function(autoHandleQR)
{
	post_to_url(Appverse.Media.serviceName, "DetectQRCode",  get_params([autoHandleQR]), "POST");
};

/**
 * Handles the given QRCode data to be processed (if possible) by the system. <br/>For further information see, {@link Appverse.Media.MediaQRContent MediaQRContent}.
 * <br> The content types that could be processed by the platform are:
 * <br> {@link Appverse.Media#QRTYPE_EMAIL_ADDRESS}, {@link Appverse.Media#QRTYPE_URI} and {@link Appverse.Media#QRTYPE_TEL}.
 * <br> Other types couldn't be processed without pre-parsing, so they are returned to be handled by the application.
 * <br> @version 3.9
 * @param {Appverse.Media.MediaQRContent} mediaQRContent The QRCode data scanned that needs to be handle.
 * @return {int} The current QRCode content type.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.HandleQRCode = function(mediaQRContent)
{
	return post_to_url(Appverse.Media.serviceName, "HandleQRCode",   get_params([mediaQRContent]), "POST");
};

/**
 * @class Appverse.Media.Async 
 * Invokes all Media API methods asynchronously.
 * <br>
 * Callback function name and callback identifier are passed to the methods (last arguments) to handle the result object when it is received from appverse runtime.
 * <pre>Usage:
 * <br> var myCallbackFn = function(result, id){ <br>	...//code here your custom functionality to handle the result... <br>}
 * <br>Appverse.Media.<b>Async</b>.Stop('myCallbackFn', 'myId').
 * <br>or
 * <br>Appverse.Media.<b>Async</b>.Play('filePath', 'myCallbackFn', 'myId').
 * </pre>
 */
Media.prototype.Async = {

/**
 * Gets Media metadata, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} filePath The media file path.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
GetMetadata : function(filePath, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Media.serviceName, "GetMetadata",  get_params([filePath]), callbackFunctionName, callbackId);
},

/**
 * Starts playing media, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} filePath The media file path.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Play : function(filePath, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Media.serviceName, "Play",  get_params([filePath]), callbackFunctionName, callbackId);
},

/**
 * Starts playing media, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} url The media remote URL.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/warning.png"/> * bug fixing | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
PlayStream : function(url, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Media.serviceName, "PlayStream",  get_params([url]), callbackFunctionName, callbackId);
},

/**
 * Moves player to the given position in the media, in ASYNC mode.
 * <br> @version 2.0
 * @param {long} position Index position.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
SeekPosition : function(position, callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Media.serviceName, "SeekPosition",  get_params([position]), callbackFunctionName, callbackId);
},

/**
 * Stops the current media playing, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Stop : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Media.serviceName, "Stop",  null, callbackFunctionName, callbackId);
},

/**
 * Pauses the current media playing, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Pause : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Media.serviceName, "Pause",  null, callbackFunctionName, callbackId);
},

/**
 * Gets Audio/Movie player state, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
GetState : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Media.serviceName, "GetState",  null, callbackFunctionName, callbackId);
},

/**
 * Gets the currently playing media file metadata, in ASYNC mode.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/information.png"/> *mock data | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
GetCurrentMedia : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Media.serviceName, "GetCurrentMedia",  null, callbackFunctionName, callbackId);
},

/**
 * Opens user interface view to select a picture from the device photos album, in ASYNC mode.
 * Data is provided via the proper event handled by the "Appverse.Media.onFinishedPickingImage" method; please, override to handle the event.
 * Returned value is "null" on synchronous call.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/warning.png"/> * in progess | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/warning.png"/> *in progress</pre>
 */
GetSnapshot : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Media.serviceName, "GetSnapshot",  null, callbackFunctionName, callbackId);
},

/**
 * Opens user interface view to take a picture using the device camera.
 * Data is provided via the proper event handled by the "Appverse.Media.onFinishedPickingImage" method; please, override to handle the event.
 * Returned value is "null" on synchronous call.
 * <br> @version 2.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @return {Appverse.Media.MediaMetadata} Media file metadata taken by the camera. 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/warning.png"/> * in progess | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/warning.png"/> *in progress</pre>
 */
TakeSnapshot : function(callbackFunctionName, callbackId)
{
	post_to_url_async(Appverse.Media.serviceName, "TakeSnapshot",  null, callbackFunctionName, callbackId);
}

};

/*
 * MESSAGING INTERFACES
 */

/**
 * @class Appverse.Messaging 
 * Singleton class field to access Messaging interface. 
 * <br>This interface provides access to device's messaging and telephone applications.<br>
 * <br> @version 1.0
 * <pre>Usage: Appverse.Messaging.&lt;metodName&gt;([params]).<br>Example: Appverse.Messaging.SendEmail(emailData).</pre>
 * @singleton
 * @constructor Constructs a new Messaging interface.
 * @return {Appverse.Messaging} A new Messaging interface.
 */
Messaging = function () {
	/**
	 * Messaging service name (as configured on Platform Service Locator).
 	 * <br> @version 1.0
	 * @type String
	 */
	this.serviceName = "message";
}

Appverse.Messaging = new Messaging();

/**
 * Sends a text message (SMS).
 * <br> @version 1.0
 * @param {String} phoneNumber The phone address to send the message to.
 * @param {String} text The message body.
 * @return {Boolean} True if SMS could be send.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data </pre>
 */
Messaging.prototype.SendMessageSMS = function(phoneNumber, text)
{
	return post_to_url(Appverse.Messaging.serviceName, "SendMessageSMS",  get_params([phoneNumber,text]), "POST");
};

/**
 * Sends a multimedia message (MMS).
 * <br> @version 1.0
 * @param {String} phoneNumber The phone address to send the message to.
 * @param {String} text The message body.
 * @param {Appverse.Messaging.AttachmentData} attachment Attachament data.
 * @return {Boolean} True if MMS could be send.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data </pre>
 */
Messaging.prototype.SendMessageMMS = function(phoneNumber, text, attachment)
{
	return post_to_url(Appverse.Messaging.serviceName, "SendMessageMMS",  get_params([phoneNumber,text, attachment]), "POST");
};

/**
 * Sends an email message.<br/>For further information see, {@link Appverse.Messaging.EmailData EmailData}.
 * <br> @version 1.0
 * @param {Appverse.Messaging.EmailData} emailData The email message data, such as: subject, 'To','Cc','Bcc' addresses, etc.
 * @return {Boolean} True if email could be send.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data </pre>
 */
Messaging.prototype.SendEmail = function(emailData)
{
	return post_to_url(Appverse.Messaging.serviceName, "SendEmail",  get_params([emailData]), "POST");
};

/*
 * PIM INTERFACES
 */

/**
 * @class Appverse.Pim 
 * Singleton class field to access Pim interface. 
 * <br>This interface provides access to device's Contacts and Calendar applications.<br> PIM stands for 'Personal Information Management'<br>
 * <br> @version 1.0
 * <pre>Usage: Appverse.Pim.&lt;metodName&gt;([params]).<br>Example: Appverse.Pim.ListContacts(queryText).</pre>
 * @singleton
 * @constructor Constructs a new Pim interface.
 * @return {Appverse.Pim} A new Pim interface.
 */
Pim = function() {
	/**
	 * Pim service name (as configured on Platform Service Locator).
 	 * <br> @version 1.0
	 * @type String
	 */
	this.serviceName = "pim";
	/**
	 * Other Number Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.NUMBERTYPE_OTHER = 0;
	/**
	 * Mobile Number Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.NUMBERTYPE_MOBILE = 1;
	/**
	 * Fixed Line Number Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.NUMBERTYPE_FIXED_LINE = 2; 
	/**
	 * Work Number Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.NUMBERTYPE_WORK = 3;
	/**
	 * Home Fax Number Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.NUMBERTYPE_HOME_FAX = 4;
	/**
	 * WorkFax Number Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.NUMBERTYPE_WORK_FAX = 5; 
	/**
	 * Pager Number Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.NUMBERTYPE_PAGER = 6;
	/**
	 * Other Disposition Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.DISPOSITIONTYPE_OTHER = 0;
	/**
	 * Personal Disposition Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.DISPOSITIONTYPE_PERSONAL = 1;
	/**
	 * Work Disposition Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.DISPOSITIONTYPE_WORK = 2;
	/**
	 * HomeOffice Disposition Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.DISPOSITIONTYPE_HOME_OFFICE = 3;
	/**
	 * Other Calendar Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.CALENDARTYPE_OTHER = 0;
	/**
	 * Birthday Calendar Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.CALENDARTYPE_BIRTHDAY = 1;
	/**
	 * Calendaring Extensions to WebDAV (CalDAV) Calendar Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.CALENDARTYPE_CALDAV = 2;
	/**
	 * Exchange Calendar Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.CALENDARTYPE_EXCHANGE = 3;
	/**
	 * IMAP Calendar Type.
	 * @type int
 	 * <br> @version 1.0
	 */
	this.CALENDARTYPE_IMAP = 4;
	/**
	 * Local Calendar Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.CALENDARTYPE_LOCAL = 5;
	/**
	 * Subscription Calendar Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.CALENDARTYPE_SUBSCRIPTION = 6;
	/**
	 * 'Needs Action' Attendee Status.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.ATTENDEESTATUS_NeedsAction = 0;
	/**
	 * 'Accepted' Attendee Status.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.ATTENDEESTATUS_ACCEPTED = 1;
	/**
	 * 'Declined' Attendee Status.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.ATTENDEESTATUS_DECLINED = 2;
	/**
	 * 'Tentative' Attendee Status.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.ATTENDEESTATUS_TENTATIVE = 3;
	/**
	 * Display Alarm Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.ALARM_DISPAY = 0;
	/**
	 * Email Alarm Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.ALARM_EMAIL = 1;
	/**
	 * Sound Alarm Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.ALARM_SOUND = 2;
	/**
	 * Weekly Recurrence Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RECURRENCE_WEEKLY = 0;
	/**
	 * Fortnightly Recurrence Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RECURRENCE_FORTNIGHTLY = 1;
	/**
	 * FourWeekly Recurrence Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RECURRENCE_FOURWEEKLY = 2;
	/**
	 * Montly Recurrence Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RECURRENCE_MONTLY = 3;
	/**
	 * Yearly Recurrence Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RECURRENCE_YEARLY = 4;
	/**
	 * None Relationship Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RELATIONSHIP_NONE = 0;
	/**
	 * Brother Relationship Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RELATIONSHIP_BROTHER = 1;
	/**
	 * Sister Relationship Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RELATIONSHIP_SISTER = 2;
	/**
	 * Parent Relationship Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RELATIONSHIP_PARENT = 3;
	/**
	 * Child Relationship Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RELATIONSHIP_CHILD = 4;
	/**
	 * Friend Relationship Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RELATIONSHIP_FRIEND = 5;
	/**
	 * Partner Relationship Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RELATIONSHIP_PARTNER = 6;
	/**
	 * Relative Relationship Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RELATIONSHIP_RELATIVE = 7;
	/**
	 * Spouse Relationship Type.
 	 * <br> @version 1.0
	 * @type int
	 */
	this.RELATIONSHIP_SPOUSE = 8;
	
	/*
	this.CONTACTS_QUERY_PARAM_NAME = "name";
	this.CONTACTS_QUERY_PARAM_GROUP = "group";
	*/
	
	/**
	 * Defines the column used to search contacts as the "ID" column (contact identifier: string or number, depending on the platform).
 	 * <br> @version 4.3
	 * @type int
	 */
	this.CONTACTS_QUERY_COLUMN_ID = 0;
	
	/**
	 * Defines the column used to search contacts as the "name" column (display name in most platforms).
 	 * <br> @version 4.3
	 * @type int
	 */
	this.CONTACTS_QUERY_COLUMN_NAME = 1;
	
	/**
	 * Defines the condition used to match contacts (using the column specified by the "query_column") as an "Equals" condition.
 	 * <br> @version 4.3
	 * @type int
	 */
	this.CONTACTS_QUERY_CONDITION_EQUALS = 0;
	
	/**
	 * Defines the condition used to match contacts (using the column specified by the "query_column") as a "Starts With" condition.
 	 * <br> @version 4.3
	 * @type int
	 */
	this.CONTACTS_QUERY_CONDITION_STARTSWITH = 1;
	
	/**
	 * Defines the condition used to match contacts (using the column specified by the "query_column") as an "Ends With" condition.
 	 * <br> @version 4.3
	 * @type int
	 */
	this.CONTACTS_QUERY_CONDITION_ENDSWITH = 2;
	
	/**
	 * Defines the condition used to match contacts (using the column specified by the "query_column") as a "Contains" condition.
 	 * <br> @version 4.3
	 * @type int
	 */
	this.CONTACTS_QUERY_CONDITION_CONTAINS = 3;
	
	/**
	 * Applications should override/implement this method to get the contacts from the phone agenda and should perform the desired javascript code on this case.
	 * <br> For further information see, {@link Appverse.Pim.ContactLite ContactLite}.
	 * <br> @version 4.3
	 * @method
	 * @param {Appverse.Pim.ContactLite[]} contacts An array of ContactLite objects successfully retrieved from the device local agenda.
	 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
	 */
	this.onListContactsEnd = function(contacts){};
	
}

Appverse.Pim = new Pim();

/**
 * List of stored phone contacts that match given query. <br/>For further information see, {@link Appverse.Pim.ContactLite ContactLite}.
 * <br> @version 4.3
 * @param {Appverse.Pim.ContactQuery} query The search query object. Optional parameter.<pre>null value for all contact returned.</pre>
 * @return {Appverse.Pim.ContactLite[]} List of contacts.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.ListContacts = function(query)
{
	if(query == null) {
		 post_to_url(Appverse.Pim.serviceName, "ListContacts",  null, "POST");
	} else {
		post_to_url(Appverse.Pim.serviceName, "ListContacts",  get_params([query]), "POST");
	}
};

/**
 * Get full version of a contact given its Id.<br/>For further information see, {@link Appverse.Pim.Contact Contact}.
 * <br> @version 4.3
 * @param {String} id The contact identifier to search for.
 * @return {Appverse.Pim.Contact} contact.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.GetContact = function(id)
{
        return post_to_url(Appverse.Pim.serviceName, "GetContact",  get_params([id]), "POST");	
};


/**
 * Creates a Contact based on given contact data. <br/>For further information see, {@link Appverse.Pim.Contact Contact}.
 * <br> @version 1.0
 * @param {Appverse.Pim.Contact} contact Contact data to be created.
 * @return {Appverse.Pim.Contact} Created contact.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.CreateContact = function(contact)
{
	return post_to_url(Appverse.Pim.serviceName, "CreateContact",  get_params([contact]), "POST");
};

/**
 * Updates contact data (given its ID) with the given contact data. <br/>For further information see, {@link Appverse.Pim.Contact Contact}.
 * <br> @version 1.0
 * @param {string} contactId Contact identifier to be updated with new data.
 * @param {Appverse.Pim.Contact} newContact New contact data to be added to the given contact.
 * @return {Boolean} True on successful updating.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.UpdateContact = function(contactId, newContactData)
{
	return post_to_url(Appverse.Pim.serviceName, "UpdateContact",  get_params([contactId,newContactData]), "POST");
};

/**
 * Deletes the given contact. <br/>For further information see, {@link Appverse.Pim.Contact Contact}.
 * <br> @version 1.0
 * @param {Appverse.Pim.Contact} contact Contact data to be deleted.
 * @return {Boolean} True on successful deletion.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.DeleteContact = function(contact)
{
	return post_to_url(Appverse.Pim.serviceName, "DeleteContact",  get_params([contact]), "POST");
};

/**
 * Lists calendar entries for given date. <br/>For further information see, {@link Appverse.Pim.CalendarEntry CalendarEntry}.
 * <br> @version 1.0
 * @param {Appverse.DateTime} date Date to match calendar entries.
 * @return {Appverse.Pim.CalendarEntry[]} List of calendar entries.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/warning.png"/> *further testing required | android <img src="resources/images/warning.png"/> *further testing required | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.ListCalendarEntriesByDate = function(date)
{
	return post_to_url(Appverse.Pim.serviceName, "ListCalendarEntries",  get_params([date]), "POST");
};

/**
 * Lists calendar entries between given start and end dates. <br/>For further information see, {@link Appverse.Pim.CalendarEntry CalendarEntry}.
 * <br> @version 1.0
 * @param {Appverse.DateTime} startDate Start date to match calendar entries.
 * @param {Appverse.DateTime} endDate End date to match calendar entries.
 * @return {Appverse.Pim.CalendarEntry[]} List of calendar entries.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/warning.png"/> *further testing required | android <img src="resources/images/warning.png"/> *further testing required | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.ListCalendarEntriesByDateRange = function(startDate, endDate)
{
	return post_to_url(Appverse.Pim.serviceName, "ListCalendarEntries",  get_params([startDate,endDate]), "POST");
};

/**
 * Creates a calendar entry. <br/>For further information see, {@link Appverse.Pim.CalendarEntry CalendarEntry}.
 * <br> @version 1.0
 * @param {Appverse.Pim.CalendarEntry} entry Calendar entry to be created.
 * @return {Appverse.Pim.CalendarEntry} Created calendar entry.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/warning.png"/> *issues with recurrences and alarms | android <img src="resources/images/warning.png"/> *issues with recurrences and alarms | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.CreateCalendarEntry = function(entry)
{
	return post_to_url(Appverse.Pim.serviceName, "CreateCalendarEntry",  get_params([entry]), "POST");
};

/**
 * Deletes the given calendar entry. <br/>For further information see, {@link Appverse.Pim.CalendarEntry CalendarEntry}.
 * <br> @version 1.0
 * @param {Appverse.Pim.CalendarEntry} entry Calendar entry to be deleted.
 * @return {Boolean} True on successful deletion.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.DeleteCalendarEntry = function(entry)
{
	return post_to_url(Appverse.Pim.serviceName, "DeleteCalendarEntry",  get_params([entry]), "POST");
};

/**
 * Moves the given calendar entry to the new start and end dates. <br/>For further information see, {@link Appverse.Pim.CalendarEntry CalendarEntry}.
 * <br> @version 1.0
 * @param {Appverse.Pim.CalendarEntry} entry Calendar entry to be moved. 
 * @param {Appverse.DateTime} startDate New start date to move the calendar entry.
 * @param {Appverse.DateTime} endDate New end date to move the calendar entry.
 * @return {Boolean} True on successful deletion.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.MoveCalendarEntry = function(entry, startDate, endDate)
{
	return post_to_url(Appverse.Pim.serviceName, "MoveCalendarEntry",  get_params([entry,startDate,endDate]), "POST");
};

/*
 * TELEPHONY INTERFACES
 */

/**
 * @class Appverse.Telephony 
 * Singleton class field to access Telephony interface. 
 * <br>This interface provides access to device's Telephony application.<br>
 * <br> @version 1.0
 * <pre>Usage: Appverse.Telephony.&lt;metodName&gt;([params]).<br>Example: Appverse.Telephony.Call('3493xxxxxxx',1).</pre>
 * @singleton
 * @constructor Constructs a new Telephony interface.
 * @return {Appverse.Telephony} A new Telephony interface.
 */
Telephony = function() {
	/**
	 * Telephony service name (as configured on Platform Service Locator).
	 * <br> @version 1.0
	 * @type String
	 */
	this.serviceName = "phone";
	/**
	 * Dialing Call Status.
	 * <br> @version 1.0
	 * @type int
	 */
	this.STATUS_DIALING = 0;
	/**
	 * Establishing Call Status.
	 * <br> @version 1.0
	 * @type int
	 */
	this.STATUS_ESTABLISHING = 1; 
	/**
	 * Established Call Status.
	 * <br> @version 1.0
	 * @type int
	 */
	this.STATUS_ESTABLISHED = 2;
	/**
	 * Dropped Call Status.
	 * <br> @version 1.0
	 * @type int
	 */
	this.STATUS_DROPPED = 3;
	/**
	 * Failed Call Status.
	 * <br> @version 1.0
	 * @type int
	 */
	this.STATUS_FAILED = 4;
	/**
	 * Busy Call Status.
	 * <br> @version 1.0
	 * @type int
	 */
	this.STATUS_BUSY = 5;
	/**
	 * Finished Call Status.
	 * <br> @version 1.0
	 * @type int
	 */
	this.STATUS_FINISHED = 6;
	/**
	 * Voice Call Type.
	 * <br> @version 1.0
	 * @type int
	 */
	this.CALLTYPE_VOICE = 0;
	/**
	 * Fax Call Type.
	 * <br> @version 1.0
	 * @type int
	 */
	this.CALLTYPE_FAX = 1;
	/**
	 * Dial Up Call Type.
	 * <br> @version 1.0
	 * @type int
	 */
	this.CALLTYPE_DIALUP = 2;
}

Appverse.Telephony = new Telephony();

/**
 * Shows and starts a phone call. 	
 * <br> @version 1.0
 * <br/>Possible values of the 'callType' argument: 
 * {@link Appverse.Telephony#CALLTYPE_VOICE CALLTYPE_VOICE}, 
 * {@link Appverse.Telephony#CALLTYPE_FAX CALLTYPE_FAX}, 
 * & {@link Appverse.Telephony#CALLTYPE_DIALUP CALLTYPE_DIALUP}
 * @param {String} number Phone number to call to.
 * @param {int} callType The type of call to open.
 * @return {ICallControl} Call control interface to handle current call.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Telephony.prototype.Call = function(number, callType)
{
	return post_to_url(Appverse.Telephony.serviceName, "Call",  get_params([number,callType]), "POST");
};

/*
 * I18N INTERFACES
 */

/**
 * @class Appverse.I18N 
 * Singleton class field to access I18N interface. 
 * <br>This interface provides features to build your application with 'localized' (centralized on external files) and 'internationalized' (for different languages) texts.<br>
 * <br> @version 1.0
 * <pre>Usage: Appverse.I18N.&lt;metodName&gt;([params]).<br>Example: Appverse.I18N.GetResourceLiteral('myKey').</pre>
 * @singleton
 * @constructor Constructs a new I18N interface.
 * @return {Appverse.I18N} A new I18N interface.
 */
I18N = function() {
	/**
	 * I18N service name (as configured on Platform Service Locator).
	 * @type String
	 * <br> @version 1.0
	 */
	this.serviceName = "i18n";
}

Appverse.I18N = new I18N();


/**
 * List of supported locales for the application (the ones configured on the '/app/config/i18n-config.xml' file).
 * <br/>For further information see, {@link Appverse.I18N.Locale Locale}.
 * <br> @version 1.0
 * @return {Appverse.I18N.Locale[]} List of locales.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
I18N.prototype.GetLocaleSupported = function()
{
	return post_to_url(Appverse.I18N.serviceName, "GetLocaleSupported",  null, "POST");
};

/**
 * List of supported locales for the application (the ones configured on the '/app/config/i18n-config.xml' file).
 * <br/>For further information see, {@link Appverse.I18N.Locale Locale}. 
 * <br> @version 1.0
 * @return {String[]} List of locales (only locale descriptor string, such as "en-US").
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
I18N.prototype.GetLocaleSupportedDescriptors = function()
{
	return post_to_url(Appverse.I18N.serviceName, "GetLocaleSupportedDescriptors",  null, "POST");
};

/**
 * Gets the text/message corresponding to the given key and locale.
 * <br/>For further information see, {@link Appverse.I18N.Locale Locale}.
 * <br> @version 1.0
 * @param {String} key The key to match text.
 * @param {String/Appverse.I18N.Locale} locale The full locale object to get localized message, or the locale desciptor ("language" or "language-country" two-letters ISO codes.
 * @return {String} Localized text.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
I18N.prototype.GetResourceLiteral = function(key, locale)
{
	if(locale == null) {
		return post_to_url(Appverse.I18N.serviceName, "GetResourceLiteral",  get_params([key]), "POST");
	} else {
		return post_to_url(Appverse.I18N.serviceName, "GetResourceLiteral",  get_params([key,locale]), "POST");
	}
};

/**
 * Gets the full application configured literals (key/message pairs) corresponding to the given locale.
 * <br/>For further information see, {@link Appverse.I18N.Locale Locale} and {@link Appverse.I18N.ResourceLiteralDictionary ResourceLiteralDictionary}.
 * <br> @version 3.2
 * @param {String/Appverse.I18N.Locale} locale The full locale object to get localized message, or the locale desciptor ("language" or "language-country" two-letters ISO codes.
 * @return {ResourceLiteralDictionary} Localized texts in the form of an object (you could get the value of a keyed literal using <b>resourceLiteralDictionary.MY_KEY</b> or <b>resourceLiteralDictionary["MY_KEY"]</b>).
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
I18N.prototype.GetResourceLiterals = function(locale)
{
	if(locale == null) {
		return post_to_url(Appverse.I18N.serviceName, "GetResourceLiterals",  null, "POST");
	} else {
		return post_to_url(Appverse.I18N.serviceName, "GetResourceLiterals",  get_params([locale]), "POST");
	}
};

/*
 * LOG INTERFACES
 */

/**
 * @class Appverse.Log 
 * Singleton class field to access Log interface. 
 * <br>This interface provides features to log message to the environment standard console.<br>
 * <br> @version 1.0
 * <pre>Usage: Appverse.Log.&lt;metodName&gt;([params]).<br>Example: Appverse.Log.Log('myKey').</pre>
 * @singleton
 * @constructor Constructs a new Log interface.
 * @return {Appverse.Log} A new Log interface.
 */
Log = function() {
	/**
	 * Log service name (as configured on Platform Service Locator).
	 * @type String
	 * <br> @version 1.0
	 */
	this.serviceName = "log";
}

Appverse.Log = new Log();


/**
 * Logs the given message, with the given log level if specified, to the standard platform/environment.
 * <br> @version 1.1
 * @param {String} message The message to be logged.
 * @param {int} level The log level (optional).
 * @return {Boolean} True on successful logged.
 * @method
 */
Log.prototype.Log = function(message, level)
{
	if(level == null) {
		return post_to_url(Appverse.Log.serviceName, "Log",  get_params([message]), "POST");
	} else {
		return post_to_url(Appverse.Log.serviceName, "Log",  get_params([message,key]), "POST");
	}
};

/*
 * ANALYTICS INTERFACES
 */

/**
 * @class Appverse.Analytics 
 * Singleton class field to access Analytics interface. 
 * <br>This interface provides features to track application usage and send to Google Analytics.<br>
 * <br> @version 3.0
 * <pre>Usage: Appverse.Analytics.&lt;metodName&gt;([params]).<br>Example: Appverse.Analytics.TrackPageView('/mypage').</pre>
 * @singleton
 * @constructor Constructs a new Analytics interface.
 * @return {Appverse.Analytics} A new Analytics interface.
 */
Analytics = function(){
        /**
	 * Analytics service name (as configured on Platform Service Locator).
	 * @type String
	 * <br> @version 3.0
	 */
    this.serviceName = "analytics";
};

Appverse.Analytics = new Analytics();

/**
 * Starts the tracker - for the given web property id - from receiving and dispatching data to the server.
 * <br> @version 3.0
 * @param {String} webPropertyID The web property ID with the format UA-99999999-9
 * @return {Boolean} true if the tracker was started successfully
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Analytics.prototype.StartTracking = function (webPropertyID){
    return post_to_url(Appverse.Analytics.serviceName, "StartTracking", get_params([webPropertyID]),"POST");
};

/**
 * 
 * Stops the tracker from receiving and dispatching data to the server
 * <br> @version 3.0
 * @return {Boolean} true if tracker was stopped
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Analytics.prototype.StopTracking = function (){
    return post_to_url(Appverse.Analytics.serviceName, "StopTracking", null,"POST");
};

/**
 * Sends an event to be tracked by the analytics tracker
 * <br> @version 3.0
 * @param {String} group the event group
 * @param {String} action the event action
 * @param {String} label The event label
 * @param {Integer} value The event value
 * @return {Boolean} true if the event was successfully tracked
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Analytics.prototype.TrackEvent = function (group, action, label, value){
    return post_to_url(Appverse.Analytics.serviceName, "TrackEvent", get_params([group,action,label, value]),"POST");
};

/**
 * Sends a pageview to be tracked by the analytics tracker
 * <br> @version 3.0
 * @param {String} relativeUrl The relativeUrl to the page i.e. "/home"
 * @return {Boolean} true if the pageview was successfully tracked
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Analytics.prototype.TrackPageView = function (relativeUrl){
    return post_to_url(Appverse.Analytics.serviceName, "TrackPageView", get_params([relativeUrl]),"POST");
};

/*
 * SECURITY INTERFACES
 */

/**
 * @class Appverse.Security 
 * Singleton class field to access Security interface. 
 * <br>This interface provides features to check the device security integrity.<br>
 * <br> @version 3.7
 * <pre>Usage: Appverse.Security.&lt;metodName&gt;([params]).<br>Example: Appverse.Security.IsDeviceModified().</pre>
 * @singleton
 * @constructor Constructs a new Security interface.
 * @return {Appverse.Security} A new Security interface.
 */
Security = function() {
	/**
	 * Security service name (as configured on Platform Service Locator).
	 * @type String
	 * <br> @version 3.7
	 */
	this.serviceName = "security";
}

Appverse.Security = new Security();


/**
 * Checks if the device has been modified.
 * <br> @version 3.7
 * @return {Boolean} True if the device is modified.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data </pre>
 */
Security.prototype.IsDeviceModified = function()
{
	return post_to_url(Appverse.Security.serviceName, "IsDeviceModified", null,"POST");
};

/**
 * Adds or updates  - if already exists - a given key/value pair into the device local secure storage.
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnKeyValuePairsStoreCompleted
 * <br> @version 4.2
 * @method
 * @param {Appverse.Security.KeyPair} keyPair A key/value pair to store
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Security.prototype.StoreKeyValuePair = function(keyPair)
{
	post_to_url(Appverse.Security.serviceName, "StoreKeyValuePair", get_params([keyPair]), "POST");
};

/**
 * 	Adds or updates - if already exists - a given list of key/value pairs into/to the device local secure storage.
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnKeyValuePairsStoreCompleted
 * <br> @version 4.2
 * @method
 * @param {Appverse.Security.KeyPair[]} keyPair A list of key/value pairs to store
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Security.prototype.StoreKeyValuePairs = function(keyPairs)
{
	post_to_url(Appverse.Security.serviceName, "StoreKeyValuePairs", get_params([keyPairs]), "POST");
};

/**
 * 	Returns a previously stored key/value pair from the device local secure storage.
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnKeyValuePairsFound
 * <br> @version 4.2
 * @method
 * @param {String} key Name of the key to be returned
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Security.prototype.GetStoredKeyValuePair = function(key)
{
	post_to_url(Appverse.Security.serviceName, "GetStoredKeyValuePair", get_params([key]), "POST");
};

/**
 * 	Returns a list of previously stored key/value pairs from the device local secure storage.
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnKeyValuePairsFound
 * <br> @version 4.2
 * @method
 * @param {String[]} keys Array of Strings containing the keys to be returned
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Security.prototype.GetStoredKeyValuePairs = function(keys)
{
	post_to_url(Appverse.Security.serviceName, "GetStoredKeyValuePairs", get_params([keys]), "POST");
};

/**
 *	Removes - if already exists - a given key/value pair from the device local secure storage
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnKeyValuePairsRemoveCompleted
 * <br> @version 4.2
 * @method
 * @param {String} key Name of the key to be removed
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Security.prototype.RemoveStoredKeyValuePair = function(key)
{
	post_to_url(Appverse.Security.serviceName, "RemoveStoredKeyValuePair", get_params([key]), "POST");
};

/**
 *	Removes - if already exists - a given list of key/value pairs from the device local secure storage
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnKeyValuePairsRemoveCompleted
 * <br> @version 4.2
 * @method
 * @param {String[]} keys Array of Strings containing the keys to be removed
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Security.prototype.RemoveStoredKeyValuePairs = function(keys)
{
	post_to_url(Appverse.Security.serviceName, "RemoveStoredKeyValuePairs", get_params([keys]), "POST");
};

/*
 * WEBTREKK INTERFACES
 */

/**
 * @class Appverse.Webtrekk
 * Singleton class field to access Webtrekk interface. 
 * <br>This interface provides features to track application usage and send to Webtrekk.<br>
 * <br> @version 3.8
 * <pre>Usage: Appverse.Webtrekk.&lt;metodName&gt;([params]).<br>Example: Appverse.Webtrekk.TrackContent('/mycontent').</pre>
 * @singleton
 * @constructor Constructs a new Webtrekk interface.
 * @return {Appverse.Webtrekk} A new Webtrekk interface.
 */
Webtrekk = function(){
        /**
	 * Webtrekk service name (as configured on Platform Service Locator).
	 * @type String
	 * <br> @version 3.8
	 */
    this.serviceName = "webtrekk";

    /**
     * Country code of the client's language settings (e.g 'de').
	 * <br> @version 3.8
	 * @type string
	 */
	this.COUNTRY_CODE = "la";

	/**
     * Order value.
	 * <br> @version 3.8
	 * @type string
	 */
	this.ORDER_VALUE = "ov";

	/**
     * Order ID.
	 * <br> @version 3.8
	 * @type string
	 */
	this.ORDER_ID = "oi";

	/**
     * Products in shopping basket.
	 * <br> @version 3.8
	 * @type string
	 */
	this.SHOPPING_BASKET = "ba";

	/**
     * Product costs.
	 * <br> @version 3.8
	 * @type string
	 */
	this.PRODUCTS_COSTS = "co";

	/**
     * Number of products.
	 * <br> @version 3.8
	 * @type string
	 */
	this.NUMBER_OF_PRODUCTS = "qn";

	/**
     * Product category. Value is 'ca1' but you can add more categories by using 'ca2', 'ca3', etc...
	 * <br> @version 3.8
	 * @type string
	 */
	this.PRODUCT_CATEGORY_1 = "ca1";

	/**
     * Status of the shopping basket (add|conf|view).
	 * <br> @version 3.8
	 * @type string
	 */
	this.STATUS_SHOPPING_BASKET = "st";

	/**
     * Customer ID
	 * <br> @version 3.8
	 * @type string
	 */
	this.CUSTOMER_ID = "cd";

	/**
     * Search term of internal search function.
	 * <br> @version 3.8
	 * @type string
	 */
	this.SEARCH_TERM = "is";

	/**
     * Campaign ID consistinf od media code parameter and value ('wt_mc=newsletter')
	 * <br> @version 3.8
	 * @type string
	 */
	this.CAMPAIGN_ID = "mc";

	/**
     * Content Group. Value is 'cg1' but you can add more categories by using 'cg2', 'cg3', etc...
	 * <br> @version 3.8
	 * @type string
	 */
	this.CONTENT_GROUP = "cg1";

	/**
     * Page parameter. Value is 'cp1' but you can add more categories by using 'cp2', 'cp3', etc...
	 * <br> @version 3.8
	 * @type string
	 */
	this.PAGE_PARAMETER = "cp1";

	/**
     * Session parameter. Value is 'cs1' but you can add more categories by using 'cs2', 'cs3', etc...
	 * <br> @version 3.8
	 * @type string
	 */
	this.SESSION_PARAMETER = "cs1";

	/**
     * Action parameter. Value is 'ck1' but you can add more categories by using 'ck2', 'ck3', etc...
	 * <br> @version 3.8
	 * @type string
	 */
	this.ACTION_PARAMETER = "ck1";

	/**
     * Independent parameter. Value is 'ce1' but you can add more categories by using 'ce2', 'ce3', etc...
	 * <br> @version 3.8
	 * @type string
	 */
	this.INDEPENDENT_PARAMETER = "ce1";

	/**
     * Campaign parameter. Value is 'cc1' but you can add more categories by using 'cc2', 'cc3', etc...
	 * <br> @version 3.8
	 * @type string
	 */
	this.CAMPAIGN_PARAMETER = "cc1";

	/**
     * E-commerce parameter. Value is 'cb1' but you can add more categories by using 'cb2', 'cb3', etc...
	 * <br> @version 3.8
	 * @type string
	 */
	this.ECOMMERCE_PARAMETER = "cb1";
};

Appverse.Webtrekk = new Webtrekk();

/**
 * Starts the tracker - for the given web server URL and Track Id - from receiving and dispatching data to the server.
 * <br> @version 3.8
 * <pre>
 * Usage samples: 
 * Appverse.Webtrekk.StartTracking("http://q3.webtrekk.net","111111111111");
 * or
 * Appverse.Webtrekk.StartTracking("http://q3.webtrekk.net","111111111111", "10");  // not recommended
 * </pre>
 * @param {String} webServerURL The web server URL with the format http://ap.Appverse.cat
 * @param {String} trackId The track Id with the format 123456789012345
 * @param {String} samplingRate [optional] The sampling rate in consultation with Webtrekk. 
 * The sampling supresses requests to Webtrekk depending on the specified value - that is,
 * only every nth user is tracked.
 * @return {Boolean} true if the tracker was started successfully
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Webtrekk.prototype.StartTracking = function (webServerUrl, trackId, samplingRate){
	if(samplingRate == null){
    	return post_to_url(Appverse.Webtrekk.serviceName, "StartTracking", get_params([webServerUrl,trackId]),"POST");
    }else{
    	return post_to_url(Appverse.Webtrekk.serviceName, "StartTracking", get_params([webServerUrl,trackId,samplingRate]),"POST");
    }
};

/**
 * Stops the tracker from receiving and dispatching data to the server
 * <br> @version 3.8
 * @return {Boolean} true if tracker was stopped
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Webtrekk.prototype.StopTracking = function (){
    return post_to_url(Appverse.Webtrekk.serviceName, "StopTracking", null,"POST");
};

/**
 * Sends a button click event to be tracked by the webtrekk tracker
 * <br> You should invoke the {@link Appverse.Webtrekk#StartTracking StartTracking} method prior to invoke this method.
 * <br> @version 3.8
 * <pre>
 * Action tracking measures clicks on internal or external links and buttons.
 * In mobile environments, the page name should be provided (unlike in websites).
 *
 * You could add further parameters to each request.
 * For further information see, {@link Appverse.Webtrekk.WebtrekkParametersCollection WebtrekkParametersCollection}.
 * 
 * Usage samples: 
 * Appverse.Webtrekk.TrackClick("myButton","home page");
 * or
 * Appverse.Webtrekk.TrackClick("myButton","home page",[{"Name":"la", "Value": "es"}, {"Name":"cd", "Value": "4515661"}]);
 *
 * For checking the possible names of the additional parameters, see the properties in the Appverse.Webtrekk object
 * such as the {@link Appverse.Webtrekk#COUNTRY_CODE COUNTRY_CODE} parameter.
 *</pre>
 * @param {String} clickId The button identification
 * @param {String} contentId The content identification (page name).
 * @param {Appverse.Webtrekk.WebtrekkParametersCollection} additionalParameters [optional] Array containing additional parameters
 * @return {Boolean} true if the content/event was successfully tracked
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Webtrekk.prototype.TrackClick = function (clickId, contentId, additionalParameters){
	if(additionalParameters == null){
    	return post_to_url(Appverse.Webtrekk.serviceName, "TrackClick", get_params([clickId,contentId]),"POST");
    }else{
    	return post_to_url(Appverse.Webtrekk.serviceName, "TrackClick", get_params([clickId,contentId,additionalParameters]),"POST");
    }
};

/**
 * Sends a content/event to be tracked by the webtrekk tracker
 * <br> You should invoke the {@link Appverse.Webtrekk#StartTracking StartTracking} method prior to invoke this method.
 * <br> @version 3.8
 * <pre>
 * Content tracking allows you to transmit particular app contents, such as pages or e-commerce values.
 * The contents are evaluated as page impressions and displayed in the page analysis on the Webtrekk user
 * interface.
 * 
 * A part from the content, you could also transmit additional parameters when tracking content.
 * For further information see, {@link Appverse.Analytics.AdditionalParameter AdditionalParameter}.
 * 
 * Usage samples: 
 * Appverse.Webtrekk.TrackContent("home page");
 * or
 * Appverse.Webtrekk.TrackContent("home page",[{"Name":"la", "Value": "es"}, {"Name":"cd", "Value": "4515661"}]);
 *
 * For checking the possible names of the additional parameters, see the properties in the Appverse.Webtrekk object
 * such as the {@link Appverse.Webtrekk#COUNTRY_CODE COUNTRY_CODE} parameter.
 *</pre>
 * @param {String} contentId The content identification
 * @param {Appverse.Webtrekk.WebtrekkParametersCollection} additionalParameters [optional] Array containing additional parameters
 * @return {Boolean} true if the content/event was successfully tracked
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Webtrekk.prototype.TrackContent = function (contentId, additionalParameters){
	if(additionalParameters == null){
    	return post_to_url(Appverse.Webtrekk.serviceName, "TrackContent", get_params([contentId]),"POST");
    }else{
    	return post_to_url(Appverse.Webtrekk.serviceName, "TrackContent", get_params([contentId,additionalParameters]),"POST");
    }
};

/**
 * Sets the time interval the request will use to transmit data to the server
 * <br> This method should be executed prior to start the session tracking using the {@link Appverse.Webtrekk#StartTracking StartTracking} method.
 * <br> @version 3.8
 * <pre>
 * Default value is 5 minutes (300 seconds).
 * To maximise battery life, the time interval can be increased to, for example, 10 minutes (600 seconds).
 * </pre>
 * @param {double} intervalInSeconds The interval in seconds the request will transmit data to the server
 * @return {Boolean} true if the interval was successfully set
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Webtrekk.prototype.SetRequestInterval = function (intervalInSeconds){
    return post_to_url(Appverse.Webtrekk.serviceName, "SetRequestInterval", get_params([intervalInSeconds]),"POST");
};

/*
 * APP LOADER INTERFACES
 */

/**
 * @class Appverse.AppLoader 
 * Singleton class field to Load, List, Update and Remove remote application content. 
 * <br>This interface provides features to load, list, update and remove remote application modules, also called sub applications. This content could be loaded into a webview to present it to the user.<br>
 * <br> @version 4.0
 * <pre>Usage: Appverse.AppLoader.&lt;metodName&gt;([params]).<br>Example: Appverse.AppLoader.ListInstalledModules().</pre>
 * @singleton
 * @constructor Constructs a new AppLoader interface.
 * @return {Appverse.AppLoader} A new AppLoader interface.
 */
AppLoader = function() {
	/**
	 * AppLoader service name (as configured on Platform Service Locator).
	 * @type String
	 * <br> @version 4.0
	 */
	this.serviceName = "loader";
	
	/**
     * @event onUpdateModulesFinished Fired when the applications loader has finished to download (update) modules 
	 * (after calling either the {@link Appverse.AppLoader.UpdateModules UpdateModules} method or the {@link Appverse.AppLoader.UpdateModule UpdateModule} method), 
	 * <br>Method to be overrided by JS applications, to handle this event.
     * <br> @version 4.0
	 * @param {Appverse.AppLoader.Module[]} successUpdates The list of successful updated modules.
     * @param {Appverse.AppLoader.Module[]} failedUpdates The list of failed updated modules.
	 * @param {String} callbackId The callback id (provided by when calling UpdateModule/s method) that identifies this event listener (callback) with the calling request.
	 */
    this.onUpdateModulesFinished = function(successUpdates, failedUpdates, callbackId){};

	/**
     * @event onDeleteModulesFinished Fired when the applications loader has finished to delete modules 
	 * (after calling the {@link Appverse.AppLoader.DeleteModules DeleteModules} method), 
	 * <br>Method to be overrided by JS applications, to handle this event.
     * <br> @version 4.0
	 * @param {Appverse.AppLoader.Module[]} successDeletes The list of successful deleted modules.
     * @param {Appverse.AppLoader.Module[]} failedDeletes The list of failed deleted modules.
	 */
    this.onDeleteModulesFinished = function(successDeletes, failedDeletes){};
}

Appverse.AppLoader = new AppLoader();


/**
 * Initializes the context of the Application Loader for the next operations.
 * <br> @version 4.0
 * @param {Appverse.AppLoader.ModuleContext} context The current context options for handling modules.
 * @method
 */
AppLoader.prototype.InitializeModuleContext = function(context)
{
	post_to_url(Appverse.AppLoader.serviceName, "InitializeModuleContext",  get_params([context]), "POST");
};

/**
 * Returns a list .
 * <br> @version 4.0
 * @return {Appverse.AppLoader.Module[]} List of currently installed modules (locally)
 * @method
 */
AppLoader.prototype.ListInstalledModules = function()
{
	return post_to_url(Appverse.AppLoader.serviceName, "ListInstalledModules",  null, "POST");
};

/**
 * Updates a given module list (or installs if it was never previously downloaded).
 * <br> @version 4.0
 * @param {Appverse.AppLoader.Module[]} modules The modules to be downloaded (Appverse.AppLoader.Module#LoadUrl is used for downloading each module).
 * @param {String} callbackId The callback identifier of this request to be returned on the corresponding event listener (callback). Null value is not needed.
 * @method
 */
AppLoader.prototype.UpdateModules = function(modules, callbackId)
{
	if(!callbackId) callbackId = "";
	post_to_url(Appverse.AppLoader.serviceName, "UpdateModules",  get_params([modules,callbackId]), "POST");
};

/**
 * Updates a given module (or installs if it was never previously downloaded).
 * <br> @version 4.0
 * @param {Appverse.AppLoader.Module[]} module The module to be downloaded (the field <b>Appverse.AppLoader.Module#LoadUrl</b> is used for downloading each module).
 * @param {String} callbackId The callback identifier of this request to be returned on the corresponding event listener (callback). Null value is not needed.
 * @method
 */
AppLoader.prototype.UpdateModule = function(module, callbackId)
{
	if(!callbackId) callbackId = "";
	post_to_url(Appverse.AppLoader.serviceName, "UpdateModule",  get_params([module,callbackId]), "POST");
};

/**
 * Deletes a given modules.
 * <br> @version 4.0
 * @param {Appverse.AppLoader.Module[]} modules The modules to be deleted.
 * @method
 */
AppLoader.prototype.DeleteModules = function(modules)
{
	post_to_url(Appverse.AppLoader.serviceName, "DeleteModules",  get_params([modules]), "POST");
};

/**
 * Loads a Module inside the Appverse WebResources Container (WebView). All modules should include an 'index.html' file as the main HTML file (entry point).
 * <br> @version 4.0
 * @param {Appverse.AppLoader.Module} module The module to be loaded.
 * @param {Appverse.AppLoader.ModuleParam[]} parameters The parameters to be added to the module main HTML file request; as GET request parameters (optional field, null for not including any parameter).
 * @param {Boolean} autoUpdate True to upload the module (using the corresponding LoadUrl and Version) prior to load it. Optional parameter. False is the default value. The update would be "silent", no event listener will be called by the platform in this case.
 * @method
 */
AppLoader.prototype.LoadModule = function(module, parameters, autoUpdate)
{
	if(autoUpdate == null || autoUpdate == false) {
		post_to_url(Appverse.AppLoader.serviceName, "LoadModule",  get_params([module,parameters]), "POST");
	} else {
		post_to_url(Appverse.AppLoader.serviceName, "LoadModule",  get_params([module,parameters,autoUpdate]), "POST");
	}
};

/*
 * COMMON FUNCTIONS
 */

/**
 * @ignore
 * This method is used to build the JSON string request from the given invocation parameters array.
 * <br> @version 1.0
 * @param {Object[]} paramsArray Array of invocation parameters.
 * @return {String} The JSON string request to be send when invoking Appverse Platform services.
 * <pre>Returned string format is: {"param1":&lt;paramsArray[0] in JSON string format&gt;,"param2":&lt;paramsArray[1] in JSON string format&gt;,...}</pre>
 */
function get_params(paramsArray)
{
	var params = null;
	
	if(paramsArray != null &&  paramsArray.length>0) {
		params = "{";
		for(var i=0; i<paramsArray.length; i++)
		{
			if(paramsArray[i]==null) {
				if(i>0) {
					params = params + ",";
				}
				params = params + '"param' + (i+1) + '":null';
			}
			
			if(paramsArray[i]!=null) { // [fix-01062011:MAPS:in some cases we need to upload empty objects, like on the SendEmail method)]  //&& JSON.stringify(paramsArray[i])!='{}') {
				if(i>0) {
					params = params + ",";
				}
				params = params + '"param' + (i+1) + '":' + JSON.stringify(paramsArray[i]);
			}
		}
		params = params + "}";
	}
	return params;
}

/**
 * @ignore
 * This method is used to invoke Appverse Service method, using synchronous XMLHttpRequest call.
 * <br> @version 1.0
 * @param {String} serviceName The service name (as configured on Platform Service Locator).
 * @param {String} methodName The method name as defined on the given service.
 * @param {String} params The JSON string request qith the params needed for method invocation. Null value if no invocation arguments are required.
 * @param {String} method The request method. POST or GET. If nor provided, default is POST.
 * @return {Object} Service invocation returned object (javacript object).
 */
function post_to_url(serviceName, methodName, params, method, returnAsString) {
	method = method || "POST"; // Set method to post by default, if not specified.

	var path = Appverse.SERVICE_URI + serviceName + "/" + methodName;
	
	if(Appverse.isBackground()) {
		// socket is closed, do not call appverse services
		console.log("Application is on background. Internal Appverse Socket is closed. Call to '" + path + "' has been stopped.");
		return null;
	}
	
	var xhr = new XMLHttpRequest(); 
	/* ASYNCHRONOUS OPTION	
	xhr.onreadystatechange  = function()
    { 
         if(xhr.readyState  == 4)  //The 4 state means for the response is ready and sent by the server. 
         {
              if(xhr.status  == 200)  //This status means ok, otherwise some error code is returned, 404 for example.
                  alert("Received:"  + xhr.responseText); 
              else 
                 alert("Error code " + xhr.status);
         }
    }; 
	xhr.open( method, path, true); 
	*/

	/* SYNCHRONOUS OPTION*/
	xhr.open( method, path, false); 
	xhr.setRequestHeader("Content-type", "application/json");
	var reqData = null;
	if(params!=null) {
		if(Appverse.unescapeNextRequestData) {
			reqData = "json=" + unescape(params);
		} else {
			reqData = "json=" + params; // we don't unscape parameters if configured
			Appverse.unescapeNextRequestData = true; // returning to default configuration value
		} 
	}
	try {
		xhr.send(reqData);
	} catch (e) {
		return null;
	}

	var responseText = null;
	if(xhr.status  == 200)  { //This status means ok, otherwise some error code is returned, 404 for example.
	  responseText = xhr.responseText; 
	}
    else {
		//alert("Error code " + xhr.status);
		// TODO: handle error
	}
	
	if(responseText!=null && responseText != '') { // [fix-01062011:MAPS:added checking for empty string, in Android platform void methods return empty strings, and the eval raise an exception]
		if(returnAsString) 
		{
			responseText = '"' + responseText + '"';	// eval response text as string, do conversions on required methods
		}
		try {
			return  eval('(' + responseText + ')');
		} catch(e){
			console.log("wrong responseText received from Appverse calls: " + responseText);
			return null;
		}
	} else {
		return null;
	}
}


function post_to_url_async(serviceName, methodName, params, callBackFuncName, callbackId) {
    method = "POST"; // Set method to post by default, if not specified.

	var path = Appverse.SERVICE_ASYNC_URI + serviceName + "/" + methodName;
	
	if(Appverse.isBackground()) {
		// socket is closed, do not call appverse services
		console.log("Application is on background. Internal Appverse Socket is closed. Call to '" + path + "' has been stopped.");
		return null;
	}
	
	var xhr = new XMLHttpRequest(); 
	xhr.open( method, path, false); 
	xhr.setRequestHeader("Content-type", "application/json");
	var reqData = "";
	if(callBackFuncName != null) {
		reqData = reqData + "callback=" + callBackFuncName;
	} else {
		reqData = reqData + "callback=callback";
	}
	if(callbackId != null) {
		reqData = reqData + "&callbackid=" + callbackId;
	} else {
	    reqData = reqData + "&callbackid=callbackid";
	}
	
	if(params!=null) {
		if(Appverse.unescapeNextRequestData) {
			reqData = reqData + "&json=" + unescape(params);
		} else {
			reqData = reqData + "&json=" + params; // we don't unscape parameters if configured
			Appverse.unescapeNextRequestData = true; // returning to default configuration value
		}
	}
	try {
		xhr.send(reqData);
	} catch (e) {
        console.dir("error sending data async: " + reqData);
		var callbackfn = window[callBackFuncName];
		if(callbackfn) callbackfn(null, callbackId);
    }
	// nothing to return, callback function will be called with result data
}

function post_to_url_async_emu(serviceName, methodName, params, callBackFuncName, callbackId) {
    method = "POST"; // Set method to post by default, if not specified.

	//var path = Appverse.SERVICE_ASYNC_URI + serviceName + "/" + methodName;
	// on emulator, async call will be simulated, not fully async mode (so we could still use developer tools on external browsers)
	var path = Appverse.SERVICE_URI + serviceName + "/" + methodName;
	
	if(Appverse.isBackground()) {
		// socket is closed, do not call appverse services
		console.log("Application is on background. Internal Appverse Socket is closed. Call to '" + path + "' has been stopped.");
		return null;
	}
	
	var xhr = new XMLHttpRequest(); 
	xhr.open( method, path, false); 
	xhr.setRequestHeader("Content-type", "application/json");
	var reqData = "";
	/*
	if(callBackFuncName != null) {
		reqData = reqData + "callback=" + callBackFuncName;
	} else {
		reqData = reqData + "callback=callback";
	}
	if(callbackId != null) {
		reqData = reqData + "&callbackid=" + callbackId;
	}else {
	    reqData = reqData + "&callbackid=callbackid";
	}
	*/
	if(params!=null) {
		//reqData = reqData + "&json=" + unescape(params);
		if(Appverse.unescapeNextRequestData) {
			reqData = reqData + "json=" + unescape(params);
		} else {
			reqData = reqData + "json=" + params; // we don't unscape parameters if configured
			Appverse.unescapeNextRequestData = true; // returning to default configuration value
		}
	}
	
	var callbackfn = window[callBackFuncName];
	if(!callbackfn) {
		try {
			callbackfn = eval('('+callBackFuncName+ ')');
		} catch(e){
			console.log("please define the callback function as a global variable. Error while evaluating function: " + e);
		}
	}
	try {
		xhr.send(reqData);
	} catch (e) {
        console.dir("error sending data async: " + reqData);
		if(callbackfn) callbackfn(null, callbackId);
    }
	
	var responseText = null;
	if(xhr.status  == 200)  { //This status means ok, otherwise some error code is returned, 404 for example.
	  responseText = xhr.responseText; 
	}
    else {
		console.dir("Error code " + xhr.status);
		// TODO: handle error
	}
	
	if(responseText!=null && responseText != '') { // [fix-01062011:MAPS:added checking for empty string, in Android platform void methods return empty strings, and the eval raise an exception]
		//console.log(responseText);
		var success = false;
		try {
			var responseObject = eval('(' + responseText + ')');
			success = true;
		} catch(e){
			console.log("wrong responseText received from Appverse calls: " + e);
			success = false;
			if(callbackfn) callbackfn(null, callbackId);
		}
		try {
			if(callbackfn && success) callbackfn(responseObject, callbackId);
		} catch(e){
			console.log("error calling callback function [" + callBackFuncName + "]: " + e);
		}
	} else {
		console.log("responseText is null for callbackid : " + callbackId);
		if(callbackfn) callbackfn(null, callbackId);
	}
	
	
	// nothing to return, callback function will be called with result data
}

/**
 * @class Appverse.JSON
 * This is the utility JSON class. 
 * <br>This class provides functions to handle JSON strings (from JS object to JSON string).<br>
 * <br> @version 1.0
 * <pre>Usage: JSON.stringify(&lt;Object&gt;).<br>Example: JSON.stringify(myObject).</pre>
 * @singleton
 */ 
JSON = new function() {
	// javadoc utility to show singleton classes.
	return JSON || {};  
	// this allows including already specified JSON utilities (for example, the ones implemented by Sencha or PastryKit JS frameworks)
	// if JSON utilities are not already implemented, below "stringify" function is used.
};

var JSON = JSON || {};

/**
 * Method shortcut. See {@link Appverse.JSON#toJSONString}.
 * <br> @version 1.0
 * @type String.
 * @method
 */
JSON.stringify = JSON.stringify || JSON.toJSONString;
/**
 * This method implements JSON.stringify serialization.
 * <br> @version 1.0
 * @param {Object} obj Object to be converted to JSON string format. See <a href="http://www.json.org/example.html">JSON (JavaScript Object Notation)</a>  for JSON format examples.
 * @return {String} The JSON string that represents the given object.
 * @method
 */	
JSON.toJSONString = function(obj) { 
	var t = typeof (obj);
    if (t != "object" || obj === null) { 
        // simple data type 
        if (t == "string") obj = '"'+obj+'"'; 
        return String(obj); 
    } 
    else { 
        // recurse array or object 
        var n, v, json = [], arr = (obj && obj.constructor == Array); 
        for (n in obj) { 
            v = obj[n]; t = typeof(v); 
            if (t == "string") v = '"'+v+'"'; 
            else if (t == "object" && v !== null) v = JSON.stringify(v); 
            json.push((arr ? "" : '"' + n + '":') + String(v)); 
        } 
        return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}"); 
    } 
}; 

/**
 * @ignore
 * Replaces {0},{1},{n} with the given arguments.
 * <br> @version 1.0
 * @return {String} The formatted result string.
 */	
String.prototype.format = function() {  
    var pattern = /\{\d+\}/g;
    var args = arguments;
    return this.replace(pattern, function(capture){ return args[capture.match(/\d+/)]; });
} 


/*
 * Initialize Appverse Utility "is" object
 *
 */
Appverse.init();

// to keep compatibility (just for 4.3 version). Should be DEPRECATED in next releases.
Unity = new function() {
	console.warn('%c The Unity namespace has been DEPRECATED, and it won\'t be valid on future releases. Please, start using the new Appverse namespace. ', 'background: #222; color: orange');
	return Appverse;
};