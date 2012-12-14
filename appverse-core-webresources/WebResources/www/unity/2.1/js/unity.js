/*
 * Javascript Classes/Interfaces related here are used to communicate HTML/Javascript files with Unity Platform.
 */

/**
 * @class Unity
 * This is the global UNITY interface class. 
 * <br><br>This interface gives singleton access to all Unity Javascript Interfaces (quick access to already instantiated Unity classes).<br>
 * @singleton
 * @constructor
 * @author Marga Parets maps@gft.com
 * @version 1.0
 */ 
Unity = new function() {
	// javadoc utility to show singleton classes.
};

Unity={version:"2.1"};

// Checking Unity compatibility versions
if(typeof(UNITY_VERSION)!="undefined") {
	if(UNITY_VERSION!=Unity.version) {
		alert("UNITY WARNING\nYour application was compiled with a Unity version different from the one configured.");
	}
}

/**
 * Applications should override/implement this method to provide current device orientation javascript stored variable.
 * @method
 * @return {String} Current Device Orientation.
 * @version 1.0
 * 
 */
Unity.getCurrentOrientation = function() { };

/**
 * Applications should override this method to implement specific rotation/resizing actions for given orientation, width and height.
 * @method
 * @param {String} orientation The device current orientation.
 * @param {int} width The new width to be set.
 * @param {int} height The height width to be set.
 * @version 1.0
 */
Unity.setOrientationChange = function(orientation, width, height) { };

/**
 * @ignore
 * Updates current device orientation, width and height values.
 * @method
 * @version 1.0 - changes added on 2.1
 */
var updateOrientation = function() {
	
	if(Unity.is.iPhone) {
		////// trigger orientationchange in UIWebView for Javascript Frameworks (such as Sencha) 
		var e = document.createEvent('Events'); 
		e.initEvent('orientationchange', true, false);
		document.dispatchEvent(e);
	} else if (!Unity.is.iPad){
	
		if (Unity.getCurrentOrientation() == 'portrait') {
			var newWidth = window.innerHeight+20;
			var newHeight = window.innerWidth-20;
			Unity.setOrientationChange( 'landscape' , newWidth , newHeight );
		} else {
			var newWidth = window.innerHeight+20;
			var newHeight = window.innerWidth-20;
			Unity.setOrientationChange( 'portrait' , newWidth , newHeight );
		}
	}
}

/**
 * @ignore
 * Launches "set orientation change" method providing current device orientation, width and height values.
 * @method
 * @version 1.0
 */
var refreshOrientation = function() {
	Unity.setOrientationChange( Unity.getCurrentOrientation() , window.innerWidth , window.innerHeight );
}


// private variable to hold status for application
Unity._background = false;

/**
 * Indicates if application is currently in background or not.
 * @method
 * @return {Boolean} True if application has been set to background.
 * @version 2.0
 * 
 */
Unity.isBackground = function() {
	return Unity._background ? Unity._background : false;
}

/**
 * Applications should override/implement this method to be aware of application being send to background, and should perform the desired javascript code on this case.
 * @method
 * @version 2.0
 * 
 */
Unity.backgroundApplicationListener= function() {};

/**
 * Applications should override/implement this method to be aware of application coming back from background, and should perform the desired javascript code on this case.
 * @method
 * @version 2.0
 * 
 */
Unity.foregroundApplicationListener = function() {};

/**
 * @ignore
 * Unity Platform will call this method when application goes to background.
 * @method
 * @version 2.0
 */
Unity._toBackground = function() {
	Unity._background  = true;
	
	//call overrided function to inform application that we are about to put application on background
	if(Unity.backgroundApplicationListener && typeof Unity.backgroundApplicationListener == "function"){
		Unity.backgroundApplicationListener();
	}
}


/**
 * @ignore
 * Unity Platform will call this method when application comes from background to foreground.
 * @method
 * @version 2.0
 */
Unity._toForeground = function() {
	Unity._background  = false;
	
	//call overrided function to inform application that we are about to put application on foreground
	if(Unity.foregroundApplicationListener && typeof Unity.foregroundApplicationListener == "function"){
		Unity.foregroundApplicationListener();
	}
}

/*
 * Manually update orientation, as no proper event is thrown using Unity 'UIWebView'
 */
window.onorientationchange = updateOrientation;


/**
 * Relative URI to access Unity Local Services.
 * <pre>Default value: '/service/'.</pre>
 * @type String
 * @version 1.0
 */
Unity.SERVICE_URI = '/service/';

/**
 * Relative URI to access Unity Remote Resources.
 * <pre>Default value: '/proxy/'.</pre>
 * @type String
 * @version 1.0
 */
Unity.REMOTE_RESOURCE_URI = '/proxy/';

/**
 * Relative URI to access Unity Resources from Application Documents.
 * <pre>Default value: '/documents/'.</pre>
 * @type String
 * @version 2.1
 */
Unity.DOCUMENTS_RESOURCE_URI = '/documents/';

if(typeof(LOCAL_SERVER_PORT)!="undefined") {
	Unity.SERVICE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Unity.SERVICE_URI;
	Unity.REMOTE_RESOURCE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Unity.REMOTE_RESOURCE_URI;
	Unity.DOCUMENTS_RESOURCE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Unity.DOCUMENTS_RESOURCE_URI;
}

/**
 * @class is 
 * This class gives information about current device (what kind of device it is, etc)
 * <pre>Usage: Unity.is.&lt;property&gt;.<br><br>Example:<br> if(Unity.is.Phone) { <br> &nbsp;&nbsp;// do something here only for smartphone devices <br> }</pre>
 * @namespace Unity
 * @singleton
 */
Unity.is = {
    /**
     * Initialization function
     */
	init : function() {
		/**
         * @property iPod Returns true if current device is an iPod device. Default to false.
         * @static
         * @type Boolean
         */
		this.iPod = false;
		/**
         * @property Mac Returns true if current device is a Mac. Default to false.
         * @static
         * @type Boolean
         */
		this.Mac = false;
		/**
         * @property Windows Returns true if current device is a Windows. Default to false.
         * @static
         * @type Boolean
         */
		this.Windows = false;
		/**
         * @property Linux Returns true if current device is a Linux. Default to false.
         * @static
         * @type Boolean
         */
		this.Linux = false;
		
		/**
         * @property iPad Returns true if current device is an iPad device. Default to false.
         * @static
         * @type Boolean
         */
		this.iPad = false;
		/**
         * @property iPhone Returns true if current device is an iPhone device. Default to false.
         * @static
         * @type Boolean
         */
		this.iPhone = false;
		/**
         * @property Android Returns true if current device is an Android device. Default to false.
         * @static
         * @type Boolean
         */
		this.Android = false;
		/**
         * @property Blackberry Returns true if current device is a Blackberry device. Default to false.
         * @static
         * @type Boolean
         */
		this.Blackberry = false;
	
		var hardwareInfo = Unity.System.GetOSHardwareInfo();
		if(hardwareInfo) {
			if(hardwareInfo.Version) {
				var deviceModel = hardwareInfo.Version;
				if(deviceModel.indexOf("iPad")>=0) {
					this.iPad = true;
				} else if(deviceModel.indexOf("iPhone")>=0) {
					this.iPhone = true;
				}
			}
		}
		
		var osInfo = Unity.System.GetOSInfo();
		if(osInfo) {
			if(osInfo.Name) {
				var osName = osInfo.Name;
				if(osName.indexOf("Android")>=0) {
					this.Android = true;
				}
			}
		}
	
		/**
         * @property Desktop Returns true if current device is a desktop device (could be a Mac, a Windows, or a non-Android Linux).
         * @static
         * @type Boolean
         */
		this.Desktop = this.Mac || this.Windows || (this.Linux && !this.Android);
        /**
         * @property Tablet Returns true if current device is a tablet device (only iPad is currently supported).
         * @static
         * @type Boolean
         */
		this.Tablet = this.iPad;
        /**
         * @property Phone Returns true if current device is an smartphone device (non destkop, neither tablet device).
         * @static
         * @type Boolean
         */
		this.Phone = !this.Desktop && !this.Tablet;
        /**
         * @property iOS Returns true if current device has an iOS operating systemn (iPhone, iPad, or iPod devices).
         * @static
         * @type Boolean
         */
		this.iOS = this.iPhone || this.iPad || this.iPod;
	}
};

Unity.fireEvent = function(name, target, data, service, method) {
	//Ready: create a generic event
	var evt = document.createEvent("Events")
	//Aim: initialize it to be the event we want
	evt.initEvent(name, true, true); //true for can bubble, true for cancelable
    evt.data = data;
    evt.service = service;
    evt.method = method;
	//FIRE!
	target.dispatchEvent(evt);
};

/**
 * Handle events sent from Unity Platform, and redirects them to the specific API handling event method.
 * @method
 * @param {Event} Unity event thrown by the platform. Event fields:<pre>service: the unity service that threw the event, method: the unity service method that threw the event, data: the data object resulting of some service callback action.</pre>
 * @version 2.0
 */
Unity.handleEvent = function(unityEvent) {
    if(unityEvent) {
        var serviceAPI = null;
        for(var api in Unity) {
            if(unityEvent.service == Unity[api].serviceName) {
                serviceAPI = Unity[api];
                break;
            }
        }
        
        if(serviceAPI && serviceAPI.handleEvent) {
            serviceAPI.handleEvent(unityEvent);
        }
    }
};

/** Add listener to all unity events **/
window.addEventListener("unityEvent", function(evt) {
        Unity.handleEvent(evt);
    }
    , false); //false to get it in bubble not capture.

/*
 * NETWORK INTERFACES
 */

/**
 * @class Unity.Net 
 * Singleton class field to access Net interface. 
 * <br><br>This interface gives access to device cellular and WIFI connection information.<br>
 * <pre>Usage: Unity.Net.&lt;metodName&gt;([params]).<br>Example: Unity.Net.IsNetworkReachable('gft.com').</pre>
 * <br>Each method could be called Asynchrnously by doing:.<br>
 * <pre>Usage: Unity.Net.<b>Async</b>.&lt;metodName&gt;([params]).<br>Example: Unity.Net.<b>Async</b>.IsNetworkReachable('gft.com').</pre>
 * @singleton
 * @constructor Constructs a new Net interface.
 * @return Net A new Net interface.
 * @version 1.0
 */
Net = function() {
	/**
	 * Net service name (as configured on Platform Service Locator).
	 * @type String
	 * @version 1.0
	 */
	this.serviceName = "net";
    /**
	 * Unknown Network Type.
	 * @type int
	 * @version 1.0
	 */
	this.NETWORKTYPE_UNKNOWN = 0;
    /**
	 * Network Type for Cable.
	 * @type int
	 * @version 1.0
	 */
	this.NETWORKTYPE_CABLE = 1;
    /**
	 * Network Type for GSM Carrier.
	 * @type int
	 * @version 1.0
	 */
	this.NETWORKTYPE_GSM = 2;
    /**
	 * Network Type for 2G Carrier.
	 * @type int
	 * @version 1.0
	 */
	this.NETWORKTYPE_2G = 3;
    /**
	 * Network Type for 25G Carrier.
	 * @type int
	 * @version 1.0
	 */
	this.NETWORKTYPE_25G = 4;
	/**
	 * Network Type for 3G Carrier.
	 * @type int
	 * @version 1.0
	 */
	this.NETWORKTYPE_3G = 5;
	/**
	 * Network Type for WIFI.
	 * @type int
	 * @version 1.0
	 */
	this.NETWORKTYPE_WIFI = 6;
}


Unity.Net = new Net();

/**
 * Detects if network is reachable or not.
 * @param {String} url The host url to check for reachability.
 * @return Boolean True/false if reachable. 
 * @version 1.0
 */
Net.prototype.IsNetworkReachable = function(url)
{
	return post_to_url(Unity.Net.serviceName, "IsNetworkReachable", get_params([url]), "POST");
}

/**
 * Gets the network types currently supported by this device.
 * <br/>Possible values of the network types: 
 * {@link Unity.Net#NETWORKTYPE_UNKNOWN NETWORKTYPE_UNKNOWN}, 
 * {@link Unity.Net#NETWORKTYPE_CABLE NETWORKTYPE_CABLE},
 * {@link Unity.Net#NETWORKTYPE_GSM NETWORKTYPE_GSM},
 * {@link Unity.Net#NETWORKTYPE_2G NETWORKTYPE_2G},
 * {@link Unity.Net#NETWORKTYPE_25G NETWORKTYPE_25G},
 * {@link Unity.Net#NETWORKTYPE_3G NETWORKTYPE_3G},
 * & {@link Unity.Net#NETWORKTYPE_WIFI NETWORKTYPE_WIFI}
 * @return int[] Array of supported network types. 
 * @version 1.0
 */
Net.prototype.GetNetworkTypeSupported = function()
{
	return post_to_url(Unity.Net.serviceName, "GetNetworkTypeSupported", null, "POST");
}

/**
 * Gets the network types from which this device is able to reach the given url host. Preference ordered list.
 * <br/>Possible values of the network types: 
 * {@link Unity.Net#NETWORKTYPE_UNKNOWN NETWORKTYPE_UNKNOWN}, 
 * {@link Unity.Net#NETWORKTYPE_CABLE NETWORKTYPE_CABLE},
 * {@link Unity.Net#NETWORKTYPE_GSM NETWORKTYPE_GSM},
 * {@link Unity.Net#NETWORKTYPE_2G NETWORKTYPE_2G},
 * {@link Unity.Net#NETWORKTYPE_25G NETWORKTYPE_25G},
 * {@link Unity.Net#NETWORKTYPE_3G NETWORKTYPE_3G},
 * & {@link Unity.Net#NETWORKTYPE_WIFI NETWORKTYPE_WIFI}
* @param {String} url The host url to check for reachability.
 * @return int[] Array of network types from which given url host is reachable. 
 * @version 1.0
 */
Net.prototype.GetNetworkTypeReachableList = function(url)
{
	return post_to_url(Unity.Net.serviceName, "GetNetworkTypeReachableList", get_params([url]), "POST");
}

/**
 * Gets the prefered network type from which this device is able to reach the given url host.
 * <br/>Possible values of the network types: 
 * {@link Unity.Net#NETWORKTYPE_UNKNOWN NETWORKTYPE_UNKNOWN}, 
 * {@link Unity.Net#NETWORKTYPE_CABLE NETWORKTYPE_CABLE},
 * {@link Unity.Net#NETWORKTYPE_GSM NETWORKTYPE_GSM},
 * {@link Unity.Net#NETWORKTYPE_2G NETWORKTYPE_2G},
 * {@link Unity.Net#NETWORKTYPE_25G NETWORKTYPE_25G},
 * {@link Unity.Net#NETWORKTYPE_3G NETWORKTYPE_3G},
 * & {@link Unity.Net#NETWORKTYPE_WIFI NETWORKTYPE_WIFI}
 * @param {String} url The host url to check for reachability.
 * @return int Prefered network type from which given url host is reachable. 
 * @version 1.0
 */
Net.prototype.GetNetworkTypeReachable = function(url)
{
	return post_to_url(Unity.Net.serviceName, "GetNetworkTypeReachable", get_params([url]), "POST");
}

/**
 * Opens the given url in a different Web View with a Navigation Bar.
 * @param {String} title The title of the Navigation Bar.
 * @param {String} buttonText The Back Button text of the Navigation Bar.
 * @param {String} url The url to be opened.
 * @return Boolean True on successful 
 * @version 1.0
 */
Net.prototype.OpenBrowser = function(title, buttonText, url)
{
	return post_to_url(Unity.Net.serviceName, "OpenBrowser", get_params([title, buttonText, url]), "POST");
}

/**
 * Renders the given html in a different Web View with a Navigation Bar.
 * @param {String} title The title of the Navigation Bar.
 * @param {String} buttonText The Back Button text of the Navigation Bar.
 * @param {String} htmls The html string to be rendered.
 * @return Boolean True on successful 
 * @version 1.0
 */
Net.prototype.ShowHtml = function(title, buttonText, html)
{
	return post_to_url(Unity.Net.serviceName, "ShowHtml", get_params([title, buttonText, html]), "POST");
}

/**
 * Downloads the given url file by using the default native handler.
 * @param {String} url The url to be opened.
 * @return Boolean True on successful 
 * @version 2.0
 */
Net.prototype.DownloadFile = function(url)
{
	return post_to_url(Unity.Net.serviceName, "DownloadFile", get_params([url]), "POST");
}

/**
 * Invokes all Net API methods asynchronously.
 * Callback function is passed to the methods (last argument) to handle the result object when it is received from unity runtime.
 */
Net.prototype.Async = {


/**
 * Detects ASYNC if network is reachable or not.
 * <pre>Usage: Unity.Net.<b>Async</b>.IsNetworkReachable('gft.com', function(result){ <br>	...//code here your custom functionality to handle the result... <br>}).</pre>
 * @param {String} url The host url to check for reachability.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True/false if reachable. 
 * @version 2.0
 */
IsNetworkReachable : function(url, callbackFunction)
{
	post_to_url_async(Unity.Net.serviceName, "IsNetworkReachable", get_params([url]), "POST", callbackFunction);
},

/**
 * Gets ASYNC the network types currently supported by this device.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: int[] Array of supported network types. 
 * @version 2.0
 */
GetNetworkTypeSupported : function(callbackFunction)
{
	post_to_url_async(Unity.Net.serviceName, "GetNetworkTypeSupported", null, "POST", callbackFunction);
},

/**
 * Gets ASYNC the network types from which this device is able to reach the given url host. Preference ordered list.
 * @param {String} url The host url to check for reachability.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: int[] Array of network types from which given url host is reachable. 
 * @version 2.0
 */
GetNetworkTypeReachableList : function(url, callbackFunction)
{
    post_to_url_async(Unity.Net.serviceName, "GetNetworkTypeReachableList", get_params([url]), "POST", callbackFunction);
},

/**
 * Gets ASYNC the prefered network type from which this device is able to reach the given url host.
 * @param {String} url The host url to check for reachability.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: int Prefered network type from which given url host is reachable. 
 * @version 2.0
 */
GetNetworkTypeReachable : function(url, callbackFunction)
{
	post_to_url_async(Unity.Net.serviceName, "GetNetworkTypeReachable", get_params([url]), "POST", callbackFunction);
},

/**
 * Opens ASYNC the given url in a different Web View with a Navigation Bar.
 * @param {String} title The title of the Navigation Bar.
 * @param {String} buttonText The Back Button text of the Navigation Bar.
 * @param {String} url The url to be opened.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True on successful 
 * @version 2.0
 */
OpenBrowser : function(title, buttonText, url, callbackFunction)
{
	post_to_url_async(Unity.Net.serviceName, "OpenBrowser", get_params([title, buttonText, url]), "POST", callbackFunction);
},

/**
 * Renders ASYNC the given html in a different Web View with a Navigation Bar.
 * @param {String} title The title of the Navigation Bar.
 * @param {String} buttonText The Back Button text of the Navigation Bar.
 * @param {String} htmls The html string to be rendered.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True on successful 
 * @version 2.0
 */
ShowHtml : function(title, buttonText, html, callbackFunction)
{
	post_to_url_async(Unity.Net.serviceName, "ShowHtml", get_params([title, buttonText, html]), "POST", callbackFunction);
}


};

/*
 * SYSTEM INTERFACES
 */
 
/**
 * @class Unity.System 
 * Singleton class field to access System interface. 
 * <br><br>This interface provides device information:<br>
 * - available displays and orientations,<br>
 * - supported locales,<br>
 * - memory status,<br>
 * - operating system, processor, and hardware information,<br>
 * - battery life information.<br>
 * <pre>Usage: Unity.System.&lt;metodName&gt;([params]).<br>Example: Unity.System.GetDisplays().</pre>
 * @singleton
 * @constructor Constructs a new System interface.
 * @return System A new System interface.
 * @version 1.0
 */
System = function() {
	/**
	 * System service name (as configured on Platform Service Locator).
	 * @type String
 	 * @version 1.0
	 */
	this.serviceName = "system";
	/**
	 * Unknown Display Orientation.
	 * @type int
 	 * @version 1.0
	 */
	this.ORIENTATION_UNKNOWN = 0;
	/**
	 * Portrait Display Orientation.
	 * @type int
 	 * @version 1.0
	 */
	this.ORIENTATION_PORTRAIT = 1;
	/**
	 * Landscape Display Orientation.
	 * @type int
 	 * @version 1.0
	 */
	this.ORIENTATION_LANDSCAPE = 2;
	/**
	 * Unknown Display Type.
	 * @type int
 	 * @version 1.0
	 */
	this.DISPLAYTYPE_UNKNOWN = 0;
	/**
	 * Primary Display Type.
	 * @type int
 	 * @version 1.0
	 */
	this.DISPLAYTYPE_PRIMARY = 1;
	/**
	 * External Display Type.
	 * @type int
 	 * @version 1.0
	 */
	this.DISPLAYTYPE_EXTERNAL = 2;
	/**
	 * Unknown Display Bit Depth.
	 * @type int
 	 * @version 1.0
	 */
	this.BITDEPTH_UNKNOWN = 0;
	/**
	 * Bpp8 Display Bit Depth.
	 * @type int
 	 * @version 1.0
	 */
	this.BITDEPTH_BPP8 = 1;
	/**
	 * Bpp16 Display Bit Depth.
	 * @type int
 	 * @version 1.0
	 */
	this.BITDEPTH_BPP16 = 2;
	/**
	 * Bpp24 Display Bit Depth.
	 * @type int
 	 * @version 1.0
	 */
	this.BITDEPTH_BPP24 = 3;
	/**
	 * Bpp32 Display Bit Depth.
	 * @type int
 	 * @version 1.0
	 */
	this.BITDEPTH_BPP32 = 4;
	/**
	 * Unknown Input Method.
	 * @type int
 	 * @version 1.0
	 */
	this.INPUTCAPABILITY_UNKNOWN = 0;
	/**
	 * Internal Touch Keyboard Input Method.
	 * @type int
 	 * @version 1.0
	 */
	this.INPUTCAPABILITY_INTERNAL_TOUCH_KEYBOARD = 1;
	/**
	 * Internal Keyboard Input Method .
	 * @type int
 	 * @version 1.0
	 */
	this.INPUTCAPABILITY_INTERNAL_KEYBOARD = 2;
	/**
	 * External Keyboard Input Method .
	 * @type int
 	 * @version 1.0
	 */
	this.INPUTCAPABILITY_EXTERNAL_KEYBOARD = 3;
	/**
	 * Internal Pointing Device Input Method .
	 * @type int
 	 * @version 1.0
	 */
	this.INPUTCAPABILITY_INTERNAL_POINTING = 4;
	/**
	 * External Pointing Device Input Method .
	 * @type int
 	 * @version 1.0
	 */
	this.INPUTCAPABILITY_EXTERNAL_POINTING = 5;
	/**
	 * Voice Recognition Device Input Method .
	 * @type int
 	 * @version 1.0
	 */
	this.INPUTCAPABILITY_VOICE_RECOGNITION = 6;
	/**
	 * Multi Touch Gestures Device Input Method .
	 * @type int
 	 * @version 1.0
	 */
	this.INPUTCAPABILITY_MULTI_TOUCH_GESTURES = 7;
	/**
	 * Mono Touch Gestures Device Input Method .
	 * @type int
 	 * @version 1.0
	 */
	this.INPUTCAPABILITY_MONO_TOUCH_GESTURES = 8;
	/**
	 * Unknown Memory Type.
	 * @type int
 	 * @version 1.0
	 */
	this.MEMORYTYPE_UNKNOWN = 0;
	/**
	 * Main Memory Type.
	 * @type int
 	 * @version 1.0
	 */
	this.MEMORYTYPE_MAIN = 1;
	/**
	 * Extended Memory Type.
	 * @type int
 	 * @version 1.0
	 */
	this.MEMORYTYPE_EXTENDED = 2;
	/**
	 * Application Memory Use.
	 * @type int
 	 * @version 1.0
	 */
	this.MEMORYUSE_APPLICATION = 0;
	/**
	 * Storage Memory Use.
	 * @type int
 	 * @version 1.0
	 */
	this.MEMORYUSE_STORAGE = 1;
	/**
	 * Other Memory Use.
	 * @type int
 	 * @version 1.0
	 */
	this.MEMORYUSE_OTHER = 2;
	/**
	 * Unknown Power Status.
	 * @type int
 	 * @version 1.0
	 */
	this.POWER_UNKNOWN = 0;
	/**
	 * Fully Charged Power Status.
	 * @type int
 	 * @version 1.0
	 */
	this.POWER_FULLY_CHARGED = 1;
	/**
	 * Charging Power Status.
	 * @type int
 	 * @version 1.0
	 */
	this.POWER_CHARGING = 2;
	/**
	 * Discharging Power Status.
	 * @type int
 	 * @version 1.0
	 */
	this.POWER_DISCHARGING = 3;
	
}

Unity.System = new System();

/**
 * Provides the number of screens connected to the device. Display 1 is the primary.
 * @return int Number of available displays. 
 * @version 1.0
 */
System.prototype.GetDisplays = function()
{
	return post_to_url(Unity.System.serviceName, "GetDisplays", null, "POST");
}

/**
 * Provides information about the display given its index. <br/><br/>For further information see, {@link Unity.System.DisplayInfo DisplayInfo}. 
 * @param {int} displayNumber The display number index. If not provided, primary display information is returned.
 * @return DisplayInfo The given display information, if found. Null value is returned, if given diplay number does not corresponds a valid index.
 * @version 1.0
 */
System.prototype.GetDisplayInfo = function(displayNumber)
{
	if(displayNumber == null) {
		return post_to_url(Unity.System.serviceName, "GetDisplayInfo", null, "POST");
	} else {
		return post_to_url(Unity.System.serviceName, "GetDisplayInfo", get_params([displayNumber]), "POST");
	}
}

/**
 * Provides the current orientation of the given display index, 1 being the primary display.
 * <br/>Possible values of display orientation: 
 * {@link Unity.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, 
 * {@link Unity.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT},
 * & {@link Unity.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @param {int} displayNumber The display number index. If not provided, primary display orientation is returned.
 * @return int The given display orientation, if found. "Unknown" value is returned, if given diplay number does not corresponds a valid index.
 * @version 1.0
 */
System.prototype.GetOrientation = function(displayNumber)
{
	return post_to_url(Unity.System.serviceName, "GetOrientation", get_params([displayNumber]), "POST");
}

/**
 * Provides the current orientation of the primary display - the primary display is 1.
 * <br/>Possible values of display orientation: 
 * {@link Unity.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, 
 * {@link Unity.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT},
 * & {@link Unity.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @return int The primary display orientation, if found.
 * @version 1.0
 */
System.prototype.GetOrientationCurrent = function()
{
	return post_to_url(Unity.System.serviceName, "GetOrientationCurrent", null, "POST");
}

/**
 * Provides the list of supported orientations for the given display number.
 * <br/>Possible values of display orientation: 
 * {@link Unity.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, 
 * {@link Unity.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT},
 * & {@link Unity.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @param {int} displayNumber The display number index. If not provided, primary display supported orientations are returned.
 * @return int[] The list of supported device orientations, for the given display.
 * @version 1.0
 */
System.prototype.GetOrientationSupported = function(displayNumber)
{
	if(displayNumber == null) {
		return post_to_url(Unity.System.serviceName, "GetOrientationSupported", null, "POST");
	} else {
		return post_to_url(Unity.System.serviceName, "GetOrientationSupported", get_params([displayNumber]), "POST");
	}
}

/**
 * List of available Locales for the device. <br/><br/>For further information see, {@link Unity.System.Locale Locale}. 
 * @return Locale[] The list of supported locales.
 * @version 1.0
 */
System.prototype.GetLocaleSupported = function()
{
	return post_to_url(Unity.System.serviceName, "GetLocaleSupported", null, "POST");
}

/**
 * Gets the current Locale for the device.<br/><br/>For further information see, {@link Unity.System.Locale Locale}. 
 * @return Locale The current Locale information.
 * @version 1.0
 */
System.prototype.GetLocaleCurrent = function()
{
	return post_to_url(Unity.System.serviceName, "GetLocaleCurrent", null, "POST");
}

/**
 * Gets the supported input methods.
 * <br/>Possible values of input methods: 
 * {@link Unity.System#INPUTCAPABILITY_EXTERNAL_KEYBOARD INPUTCAPABILITY_EXTERNAL_KEYBOARD}, 
 * {@link Unity.System#INPUTCAPABILITY_INTERNAL_POINTING INPUTCAPABILITY_INTERNAL_POINTING},
 * {@link Unity.System#INPUTCAPABILITY_EXTERNAL_POINTING INPUTCAPABILITY_EXTERNAL_POINTING},
 * {@link Unity.System#INPUTCAPABILITY_INTERNAL_KEYBOARD INPUTCAPABILITY_INTERNAL_KEYBOARD},
 * {@link Unity.System#INPUTCAPABILITY_INTERNAL_TOUCH_KEYBOARD INPUTCAPABILITY_INTERNAL_TOUCH_KEYBOARD},
 * {@link Unity.System#INPUTCAPABILITY_MONO_TOUCH_GESTURES INPUTCAPABILITY_MONO_TOUCH_GESTURESv},
 * {@link Unity.System#INPUTCAPABILITY_MULTI_TOUCH_GESTURES INPUTCAPABILITY_MULTI_TOUCH_GESTURES},
 * {@link Unity.System#INPUTCAPABILITY_UNKNOWN INPUTCAPABILITY_UNKNOWN},
 * & {@link Unity.System#INPUTCAPABILITY_VOICE_RECOGNITION INPUTCAPABILITY_VOICE_RECOGNITION} 
 * @return int[] List of input methods supported by the device.
 * @version 1.0
 */
System.prototype.GetInputMethods = function()
{
	return post_to_url(Unity.System.serviceName, "GetInputMethods", null, "POST");
}

/**
 * Gets the supported input gestures.
 * @return int[] List of input gestures supported by the device.
 * @version 1.0
 */
System.prototype.GetInputGestures = function()
{
	return post_to_url(Unity.System.serviceName, "GetInputGestures", null, "POST");
}

/**
 * Gets the supported input buttons.
 * @return int[]List of input buttons supported by the device.
 * @version 1.0
 */
System.prototype.GetInputButtons = function()
{
	return post_to_url(Unity.System.serviceName, "GetInputButtons", null, "POST");
}

/**
 * Gets the currently active input method.
 * <br/>Possible values of input method: 
 * {@link Unity.System#INPUTCAPABILITY_EXTERNAL_KEYBOARD INPUTCAPABILITY_EXTERNAL_KEYBOARD}, 
 * {@link Unity.System#INPUTCAPABILITY_INTERNAL_POINTING INPUTCAPABILITY_INTERNAL_POINTING},
 * {@link Unity.System#INPUTCAPABILITY_EXTERNAL_POINTING INPUTCAPABILITY_EXTERNAL_POINTING},
 * {@link Unity.System#INPUTCAPABILITY_INTERNAL_KEYBOARD INPUTCAPABILITY_INTERNAL_KEYBOARD},
 * {@link Unity.System#INPUTCAPABILITY_INTERNAL_TOUCH_KEYBOARD INPUTCAPABILITY_INTERNAL_TOUCH_KEYBOARD},
 * {@link Unity.System#INPUTCAPABILITY_MONO_TOUCH_GESTURES INPUTCAPABILITY_MONO_TOUCH_GESTURESv},
 * {@link Unity.System#INPUTCAPABILITY_MULTI_TOUCH_GESTURES INPUTCAPABILITY_MULTI_TOUCH_GESTURES},
 * {@link Unity.System#INPUTCAPABILITY_UNKNOWN INPUTCAPABILITY_UNKNOWN},
 * & {@link Unity.System#INPUTCAPABILITY_VOICE_RECOGNITION INPUTCAPABILITY_VOICE_RECOGNITION} 
 * @return int Current input method.
 * @version 1.0
 */
System.prototype.GetInputMethodCurrent = function()
{
	return post_to_url(Unity.System.serviceName, "GetInputMethodCurrent", null, "POST");
}

/**
 * Provides memory available for the given use and type.
 * <br/>Possible values of memory types: 
 * {@link Unity.System#MEMORYTYPE_EXTENDED MEMORYTYPE_EXTENDED}, 
 * {@link Unity.System#MEMORYTYPE_MAIN MEMORYTYPE_MAIN},
 * & {@link Unity.System#MEMORYTYPE_UNKNOWN MEMORYTYPE_UNKNOWN} 
 * <br/>Possible values of memory uses: 
 * {@link Unity.System#MEMORYUSE_APPLICATION MEMORYUSE_APPLICATION}, 
 * {@link Unity.System#MEMORYUSE_STORAGE MEMORYUSE_STORAGE},
 * & {@link Unity.System#MEMORYUSE_OTHER MEMORYUSE_OTHER} 
 * @param {int} memUse The memory use. 
 * @param {int} memType The memory type. Optional parameter.
 * @return long The memory available in bytes.
 * @version 1.0
 */
System.prototype.GetMemoryAvailable = function(memUse, memType)
{
	if(memType == null) {
		return post_to_url(Unity.System.serviceName, "GetMemoryAvailable", get_params([memUse]), "POST");
	} else {
		return post_to_url(Unity.System.serviceName, "GetMemoryAvailable", get_params([memUse,memType]), "POST");
	}
}

/**
 * Gets the device installed memory types.
 * <br/>Possible values of memory types: 
 * {@link Unity.System#MEMORYTYPE_EXTENDED MEMORYTYPE_EXTENDED}, 
 * {@link Unity.System#MEMORYTYPE_MAIN MEMORYTYPE_MAIN},
 * & {@link Unity.System#MEMORYTYPE_UNKNOWN MEMORYTYPE_UNKNOWN} 
 * @return int[] The installed storage types.
 * @version 1.0
 */
System.prototype.GetMemoryAvailableTypes = function()
{
	return post_to_url(Unity.System.serviceName, "GetMemoryAvailableTypes", null, "POST");
}

/**
 * Provides a global map of the memory status for all storage types installed, if 'memType' not provided.
 * Provides a map of the memory status for the given storage type, if 'memType' provided.
 * <br/><br/>For further information see, {@link Unity.System.MemoryStatus MemoryStatus}. 
 * <br/>Possible values of memory types: 
 * {@link Unity.System#MEMORYTYPE_EXTENDED MEMORYTYPE_EXTENDED}, 
 * {@link Unity.System#MEMORYTYPE_MAIN MEMORYTYPE_MAIN},
 * & {@link Unity.System#MEMORYTYPE_UNKNOWN MEMORYTYPE_UNKNOWN} 
 * @param {int} memType The type of memory to check for status. Optional parameter.
 * @return MemoryStatus The memory status information.
 * @version 1.0
 */
System.prototype.GetMemoryStatus = function(memType)
{
	if(memType == null) {
		return post_to_url(Unity.System.serviceName, "GetMemoryStatus", null, "POST");
	} else {
		return post_to_url(Unity.System.serviceName, "GetMemoryStatus", get_params([memType]), "POST");
	}
}

/**
 * Gets the device currently available memory types.
 * <br/>Possible values of memory types: 
 * {@link Unity.System#MEMORYTYPE_EXTENDED MEMORYTYPE_EXTENDED}, 
 * {@link Unity.System#MEMORYTYPE_MAIN MEMORYTYPE_MAIN},
 * & {@link Unity.System#MEMORYTYPE_UNKNOWN MEMORYTYPE_UNKNOWN} 
 * @return int[] The available storafe types.
 * @version 1.0
 */
System.prototype.GetMemoryTypes = function()
{
	return post_to_url(Unity.System.serviceName, "GetMemoryTypes", null, "POST");
}

/**
 * Gets the device currently available memory uses.
 * <br/>Possible values of memory uses: 
 * {@link Unity.System#MEMORYUSE_APPLICATION MEMORYUSE_APPLICATION}, 
 * {@link Unity.System#MEMORYUSE_STORAGE MEMORYUSE_STORAGE},
 * & {@link Unity.System#MEMORYUSE_OTHER MEMORYUSE_OTHER} 
 * @return int[] The available memory uses.
 * @version 1.0
 */
System.prototype.GetMemoryUses = function()
{
	return post_to_url(Unity.System.serviceName, "GetMemoryUses", null, "POST");
}

/**
 * Provides information about the device hardware.<br/><br/>For further information see, {@link Unity.System.HardwareInfo HardwareInfo}.
 * @return HardwareInfo The device hardware information (name, version, UUID, etc).
 * @version 1.0
 */
System.prototype.GetOSHardwareInfo = function()
{
	return post_to_url(Unity.System.serviceName, "GetOSHardwareInfo", null, "POST");
}

/**
 * Provides information about the device operating system.<br/><br/>For further information see, {@link Unity.System.OSInfo OSInfo}.
 * @return OSInfo The device OS information (name, vendor, version).
 * @version 1.0
 */
System.prototype.GetOSInfo = function()
{
	return post_to_url(Unity.System.serviceName, "GetOSInfo", null, "POST");
}

/**
 * Provides the current user agent string.
 * @return String The user agent string. 
 * @version 1.0
 */
System.prototype.GetOSUserAgent = function()
{
	return post_to_url(Unity.System.serviceName, "GetOSUserAgent", null, "POST");
}

/**
 * Provides information about the device charge.<br/><br/>For further information see, {@link Unity.System.PowerInfo PowerInfo}.
 * @return PowerInfo The current charge information.
 * @version 1.0
 */
System.prototype.GetPowerInfo = function()
{
	return post_to_url(Unity.System.serviceName, "GetPowerInfo", null, "POST");
}

/**
 * Provides device autonomy time (in milliseconds).
 * @return long The remaining power time.
 * @version 1.0
 */
System.prototype.GetPowerRemainingTime = function()
{
	return post_to_url(Unity.System.serviceName, "GetPowerRemainingTime", null, "POST");
}

/**
 * Provides information about the device CPU.<br/><br/>For further information see, {@link Unity.System.CPUInfo CPUInfo}.
 * @return CPUInfo The processor information (name, vendor, speed, UUID, etc).
 * @version 1.0
 */
System.prototype.GetCPUInfo = function()
{
	return post_to_url(Unity.System.serviceName, "GetCPUInfo", null, "POST");
}

/**
 * Provides information about if the current application is allowed to autorotate or not. If locked, 
 * @return Boolean True if application remains with the same screen orientation (even though user rotates the device).
 * @version 2.0
 */
System.prototype.IsOrientationLocked  = function() {
	return post_to_url(Unity.System.serviceName, "IsOrientationLocked", null, "POST");
}

/**
 * Sets wheter the current application could autorotate or not (whether orientation is locked or not)
 * @param {Boolean} Set value to true if application should remain with the same screen orientation (even though user rotates the device)..
 * @param {int} Set the orientation to lock the device to (this value is ignored if "lock" argument is "false"). Possible values of display orientation: {@link Unity.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, {@link Unity.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT} or {@link Unity.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @version 2.0
 */
System.prototype.LockOrientation = function(lock, orientation) {
	return post_to_url(Unity.System.serviceName, "LockOrientation", get_params([lock,orientation]), "POST");
}

/*
 * DATABASE INTERFACES
 */

/**
 * @class Unity.Database 
 * Singleton class field to access Database interface. 
 * <br><br>This interface allows to create SQL Databases for use by your application and access them from your application's Javascript.<br>
 * <pre>Usage: Unity.Database.&lt;metodName&gt;([params]).<br>Example: Unity.Database.GetDatabaseList().</pre>
 * @singleton
 * @constructor Constructs a new Database interface.
 * @return System A new Database interface.
 * @version 1.0
 */
Database = function() {
	/**
	 * Database service name (as configured on Platform Service Locator).
	 * @type String
 	 * @version 1.0
	 */
	this.serviceName = "db";
}

Unity.Database = new Database();

/**
 * Gets stored databases.<br/><br/>For further information see, {@link Unity.Database.Database Database}.
 * @return Database[] List of application Databases.
 * @version 1.0
 */
Database.prototype.GetDatabaseList = function()
{
	return post_to_url(Unity.Database.serviceName, "GetDatabaseList", null, "POST");
}

/**
 * Creates database on default path.<br/><br/>For further information see, {@link Unity.Database.Database Database}.
 * @param {String} dbName The database file name (please include .db extension).
 * @return Database The created database reference object.
 * @version 1.0
 */
Database.prototype.CreateDatabase = function(dbName)
{
	return post_to_url(Unity.Database.serviceName, "CreateDatabase", get_params([dbName]), "POST");
}

/**
 * Gets database reference object by given name.<br/><br/>For further information see, {@link Unity.Database.Database Database}.
 * <br>Databases are located on the default database path: /<PersonalFolder>/sqlite/
 * @param {String} dbName The database file name (inlcuding .db extension).
 * @return Database The created database reference object.
 * @version 1.0
 */
Database.prototype.GetDatabase = function(dbName)
{
	return post_to_url(Unity.Database.serviceName, "GetDatabase", get_params([dbName]), "POST");
}

/**
 * Creates a table inside the given database.<br/><br/>For further information see, {@link Unity.Database.Database Database}.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase} ).
 * @param {String} tableName The table name to be inserted.
 * @param {String[]} columnsDefs The column definitions array (SQLITE syntax).
 * @return Boolean True on successful table creation.
 * @version 1.0
 */
Database.prototype.CreateTable = function(db,tableName,columnsDefs)
{
	return post_to_url(Unity.Database.serviceName, "CreateTable", get_params([db,tableName, columnsDefs]), "POST");
}

/**
 * Deletes database on default path.<br/><br/>For further information see, {@link Unity.Database.Database Database}.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}) to be deleted.
 * @return Boolean True on successful database deletion.
 * @version 1.0
 */
Database.prototype.DeleteDatabase = function(db)
{
	return post_to_url(Unity.Database.serviceName, "DeleteDatabase", get_params([db]), "POST");
}

/**
 * Deletes table from the given database.<br/><br/>For further information see, {@link Unity.Database.Database Database}.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase} ).
 * @param {String} tableName The table name to be deleted.
 * @return Boolean True on successful table deletion.
 * @version 1.0
 */
Database.prototype.DeleteTable = function(db,tableName)
{
	return post_to_url(Unity.Database.serviceName, "DeleteTable", get_params([db,tableName]), "POST");
}

/**
 * Gets table names from the given database.<br/><br/>For further information see, {@link Unity.Database.Database Database}.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}) to check for table names.
 * @return String[] List of table names.
 * @version 1.0
 */
Database.prototype.GetTableNames = function(db)
{
	return post_to_url(Unity.Database.serviceName, "GetTableNames", get_params([db]), "POST");
}

/**
 * Checks if database exists by database bean reference, if 'tableName' is not provided.
 * Checks if database table exists by database bean reference and table name, if 'tableName' is provided.
 * <br/><br/>For further information see, {@link Unity.Database.Database Database}.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} tableName The table name  to check for existence. Optional parameter.
 * @return Boolean True if database or database table exists.
 * @version 1.0
 */
Database.prototype.Exists = function(db, tableName)
{
	if(tableName == null) {
		 return post_to_url(Unity.Database.serviceName, "Exists", get_params([db]), "POST");
	} else {
		return post_to_url(Unity.Database.serviceName, "Exists", get_params([db,tableName]), "POST");
	}
}

/**
 * Checks if database exists by given database name (including .db extension).<br/><br/>For further information see, {@link Unity.Database.Database Database}.
 * @param {String} dbName The database name to check for existence.
 * @return Boolean True if database exists.
 * @version 1.0
 */
Database.prototype.ExistsDatabase = function(dbName)
{
	return post_to_url(Unity.Database.serviceName, "ExistsDatabase", get_params([dbName]), "POST");
}

/**
 * Executes SQL query against given database.<br/><br/>For further information see, {@link Unity.Database.Database Database} and {@link Unity.Database.ResultSet ResultSet}.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} query The SQL query to execute. 
 * @param {String[]} replacements The replacement arguments for a preformatted SQL query. Optional parameter.
 * @return ResultSet The result set (with zero rows count parameter if no rows satisfy query conditions).
 * @version 1.0
 */
Database.prototype.ExecuteSQLQuery = function(db, query, replacements)
{
	if(replacements == null) {
		return post_to_url(Unity.Database.serviceName, "ExecuteSQLQuery", get_params([db,query]), "POST");
	} else {
		return post_to_url(Unity.Database.serviceName, "ExecuteSQLQuery", get_params([db,query,replacements]), "POST");
	}
}

/**
 * Executes SQL statement into the given database.<br/><br/>For further information see, {@link Unity.Database.Database Database}.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} statement The SQL statement to execute. 
 * @param {String[]} replacements The replacement arguments for a preformatted SQL statement. Optional parameter.
 * @return Boolean True on successful statement execution.
 * @version 1.0
 */
Database.prototype.ExecuteSQLStatement = function(db, statement, replacements)
{
	if(replacements == null) {
		return post_to_url(Unity.Database.serviceName, "ExecuteSQLStatement", get_params([db,statement]), "POST");
	} else {
		return post_to_url(Unity.Database.serviceName, "ExecuteSQLStatement", get_params([db,statement,replacements]), "POST");
	}
}

/**
 * Executes SQL transaction (some statements chain) inside given database.<br/><br/>For further information see, {@link Unity.Database.Database Database}.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String[]} statements The statements to be executed during transaction (sqlite syntax language).. 
 * @param {Boolean} rollbackFlag Indicates if rollback should be performed when any statement execution fails.
 * @return Boolean True on successful transaction execution.
 * @version 1.0
 */
Database.prototype.ExecuteSQLTransaction = function(db, statements, rollbackFlag)
{
	return post_to_url(Unity.Database.serviceName, "ExecuteSQLTransaction", get_params([db,statements,rollbackFlag]), "POST");
}

/**
 * Invokes all Database API methods asynchronously.
 * Callback function is passed to the methods (last argument) to handle the result object when it is received from unity runtime.
 */
Database.prototype.Async = {

/**
 * Gets stored databases, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Database[] List of application Databases.
 * @version 2.0
 */
GetDatabaseList : function(callbackFunction)
{
	post_to_url_async(Unity.Database.serviceName, "GetDatabaseList", null, "POST", callbackFunction);
},

/**
 * Creates database on default path, in ASYNC mode.
 * @param {String} dbName The database file name (please include .db extension).
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Database The created database reference object.
 * @version 2.0
 */
CreateDatabase : function(dbName, callbackFunction)
{
	post_to_url_async(Unity.Database.serviceName, "CreateDatabase", get_params([dbName]), "POST", callbackFunction);
},

/**
 * Gets database reference object by given name, in ASYNC mode.
 * <br>Databases are located on the default database path: /<PersonalFolder>/sqlite/
 * @param {String} dbName The database file name (inlcuding .db extension).
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Database The created database reference object.
 * @version 2.0
 */
GetDatabase : function(dbName, callbackFunction)
{
	post_to_url_async(Unity.Database.serviceName, "GetDatabase", get_params([dbName]), "POST", callbackFunction);
},

/**
 * Creates a table inside the given database, in ASYNC mode.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase} ).
 * @param {String} tableName The table name to be inserted.
 * @param {String[]} columnsDefs The column definitions array (SQLITE syntax).
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True on successful table creation.
 * @version 2.0
 */
CreateTable : function(db,tableName,columnsDefs, callbackFunction)
{
	post_to_url_async(Unity.Database.serviceName, "CreateTable", get_params([db,tableName, columnsDefs]), "POST", callbackFunction);
},

/**
 * Deletes database on default path, in ASYNC mode.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}) to be deleted.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True on successful database deletion.
 * @version 2.0
 */
DeleteDatabase : function(db, callbackFunction)
{
	post_to_url_async(Unity.Database.serviceName, "DeleteDatabase", get_params([db]), "POST", callbackFunction);
},

/**
 * Deletes table from the given database, in ASYNC mode.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase} ).
 * @param {String} tableName The table name to be deleted.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True on successful table deletion.
 * @version 2.0
 */
DeleteTable : function(db,tableName, callbackFunction)
{
	post_to_url_async(Unity.Database.serviceName, "DeleteTable", get_params([db,tableName]), "POST", callbackFunction);
},

/**
 * Gets table names from the given database, in ASYNC mode.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}) to check for table names.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: String[] List of table names.
 * @version 1.0
 */
GetTableNames : function(db, callbackFunction)
{
	post_to_url_async(Unity.Database.serviceName, "GetTableNames", get_params([db]), "POST", callbackFunction);
},

/**
 * Checks if database exists by database bean reference, if 'tableName' is not provided, in ASYNC mode.
 * Checks if database table exists by database bean reference and table name, if 'tableName' is provided.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} tableName The table name  to check for existence. Optional parameter.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if database or database table exists.
 * @version 2.0
 */
Exists : function(db, tableName, callbackFunction)
{
	if(tableName == null) {
        post_to_url_async(Unity.Database.serviceName, "Exists", get_params([db]), "POST", callbackFunction);
	} else {
		post_to_url_async(Unity.Database.serviceName, "Exists", get_params([db,tableName]), "POST", callbackFunction);
	}
},

/**
 * Checks if database exists by given database name (including .db extension), in ASYNC mode.
 * @param {String} dbName The database name to check for existence.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if database exists.
 * @version 2.0
 */
ExistsDatabase : function(dbName, callbackFunction)
{
	post_to_url_async(Unity.Database.serviceName, "ExistsDatabase", get_params([dbName]), "POST", callbackFunction);
},

/**
 * Executes SQL query against given database, in ASYNC mode.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} query The SQL query to execute. 
 * @param {String[]} replacements The replacement arguments for a preformatted SQL query. Optional parameter.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: ResultSet The result set (with zero rows count parameter if no rows satisfy query conditions).
 * @version 2.0
 */
ExecuteSQLQuery : function(db, query, replacements, callbackFunction)
{
	if(replacements == null) {
		post_to_url_async(Unity.Database.serviceName, "ExecuteSQLQuery", get_params([db,query]), "POST", callbackFunction);
	} else {
		post_to_url_async(Unity.Database.serviceName, "ExecuteSQLQuery", get_params([db,query,replacements]), "POST", callbackFunction);
	}
},

/**
 * Executes SQL statement into the given database, in ASYNC mode.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} statement The SQL statement to execute. 
 * @param {String[]} replacements The replacement arguments for a preformatted SQL statement. Optional parameter.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True on successful statement execution.
 * @version 2.0
 */
ExecuteSQLStatement : function(db, statement, replacements, callbackFunction)
{
	if(replacements == null) {
		post_to_url_async(Unity.Database.serviceName, "ExecuteSQLStatement", get_params([db,statement]), "POST", callbackFunction);
	} else {
		post_to_url_async(Unity.Database.serviceName, "ExecuteSQLStatement", get_params([db,statement,replacements]), "POST", callbackFunction);
	}
},

/**
 * Executes SQL transaction (some statements chain) inside given database, in ASYNC mode.
 * @param {Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String[]} statements The statements to be executed during transaction (sqlite syntax language).. 
 * @param {Boolean} rollbackFlag Indicates if rollback should be performed when any statement execution fails.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True on successful transaction execution.
 * @version 2.0
 */
ExecuteSQLTransaction : function(db, statements, rollbackFlag, callbackFunction)
{
	post_to_url_async(Unity.Database.serviceName, "ExecuteSQLTransaction", get_params([db,statements,rollbackFlag]), "POST", callbackFunction);
}

};


/*
 * FILE INTERFACES
 */

/**
 * @class Unity.FileSystem 
 * Singleton class field to access FileSystem interface. 
 * <br><br>This interface provides access to the device filesystem (only personal folder is accessible), to create/access/delete directories and files.<br>
 * <pre>Usage: Unity.FileSystem.&lt;metodName&gt;([params]).<br>Example: Unity.FileSystem.GetDirectoryRoot().</pre>
 * @singleton
 * @constructor Constructs a new FileSystem interface.
 * @return System A new FileSystem interface.
 * @version 1.0
 */
FileSystem = function() {
	/**
	 * FileSystem service name (as configured on Platform Service Locator).
	 * @type String
 	 * @version 1.0
	 */
	this.serviceName = "file";
}

Unity.FileSystem = new FileSystem();

/**
 * Get configured root directory.<br/><br/>For further information see, {@link Unity.FileSystem.DirectoryData DirectoryData}.
 * @return DirectoryData The configured root directory information.
 * @version 1.0
 */
FileSystem.prototype.GetDirectoryRoot = function()
{
	return post_to_url(Unity.FileSystem.serviceName, "GetDirectoryRoot", null, "POST");
}

/**
 * Creates a directory under the given base directory, or under root directory if it is not provided.<br/><br/>For further information see, {@link Unity.FileSystem.DirectoryData DirectoryData}.
 * @param {String} directoryName The directory name to be created. 
 * @param {DirectoryData} baseDirectory The base Directory to create directory under it. Optional parameter.
 * @return DirectoryData The directory created, or null if folder cannot be created.
 * @version 1.0
 */
FileSystem.prototype.CreateDirectory = function(directoryName, baseDirectory)
{
	if(baseDirectory == null) {
		return post_to_url(Unity.FileSystem.serviceName, "CreateDirectory", get_params([directoryName]), "POST");
	} else {
		return post_to_url(Unity.FileSystem.serviceName, "CreateDirectory", get_params([directoryName,baseDirectory]), "POST");
	}
}

/**
 * Creates a file under the given base directory, or under root directory if it is not provided.<br/><br/>For further information see, {@link Unity.FileSystem.DirectoryData DirectoryData} and {@link Unity.FileSystem.FileData FileData}.
 * @param {String} fileName The file name to be created. 
 * @param {DirectoryData} baseDirectory The base Directory to create file under it. Optional parameter.
 * @return FileData The file created, or null if folder cannot be created.
 * @version 1.0
 */
FileSystem.prototype.CreateFile = function(fileName, baseDirectory)
{
	if(baseDirectory == null) {
		return post_to_url(Unity.FileSystem.serviceName, "CreateFile", get_params([fileName]), "POST");
	} else {
		return post_to_url(Unity.FileSystem.serviceName, "CreateFile", get_params([fileName,baseDirectory]), "POST");
	}
}

/**
 * List all directories under the given base directory data, or under root directory if it is not provided.<br/><br/>For further information see, {@link Unity.FileSystem.DirectoryData DirectoryData}.
 * @param {DirectoryData} dirData The base Directory to check for directories under it. Optional parameter.
 * @return DirectoryData[] The directories information array.
 * @version 1.0
 */
FileSystem.prototype.ListDirectories = function(dirData)
{
	if(dirData == null) {
		return post_to_url(Unity.FileSystem.serviceName, "ListDirectories", null, "POST");
	} else {
		return post_to_url(Unity.FileSystem.serviceName, "ListDirectories", get_params([dirData]), "POST");
	}
}

/**
 * List all files under the given base directory data, or under root directory if it is not provided.<br/><br/>For further information see, {@link Unity.FileSystem.DirectoryData DirectoryData} and {@link Unity.FileSystem.FileData FileData}.
 * @param {DirectoryData} dirData The base Directory to check for files under it. Optional parameter.
 * @return FileData[] The files information array.
 * @version 1.0
 */
FileSystem.prototype.ListFiles = function(dirData)
{
	return post_to_url(Unity.FileSystem.serviceName, "ListFiles", get_params([dirData]), "POST");
}

/**
 * Checks if the given directory exists.<br/><br/>For further information see, {@link Unity.FileSystem.DirectoryData DirectoryData}.
 * @param {DirectoryData} dirData The directory to check for existence.
 * @return Boolean True if directory exists.
 * @version 1.0
 */
FileSystem.prototype.ExistsDirectory = function(dirData)
{
	return post_to_url(Unity.FileSystem.serviceName, "ExistsDirectory", get_params([dirData]), "POST");
}

/**
 * Deletes the given directory.<br/><br/>For further information see, {@link Unity.FileSystem.DirectoryData DirectoryData}.
 * @param {DirectoryData} dirData The directory to be deleted.
 * @return Boolean True on successful directory deletion.
 * @version 1.0
 */
FileSystem.prototype.DeleteDirectory = function(dirData)
{
	return post_to_url(Unity.FileSystem.serviceName, "DeleteDirectory", get_params([dirData]), "POST");
}

/**
 * Deletes the given file.<br/><br/>For further information see, {@link Unity.FileSystem.FileData FileData}.
 * @param {FileData} fileData The file to be deleted.
 * @return Boolean True on successful file deletion.
 * @version 1.0
 */
FileSystem.prototype.DeleteFile = function(fileData)
{
	return post_to_url(Unity.FileSystem.serviceName, "DeleteFile", get_params([fileData]), "POST");
}

/**
 * Checks if the given file exists.<br/><br/>For further information see, {@link Unity.FileSystem.FileData FileData}.
 * @param {FileData} fileData The file data to check for existence.
 * @return Boolean True if file exists.
 * @version 1.0
 */
FileSystem.prototype.ExistsFile = function(fileData)
{
	return post_to_url(Unity.FileSystem.serviceName, "ExistsFile", get_params([fileData]), "POST");
}

/**
 * Reads file on given path.<br/><br/>For further information see, {@link Unity.FileSystem.FileData FileData}.
 * @param {FileData} fileData The file data to read.
 * @return byte[] Readed bytes.
 * @version 1.0
 */
FileSystem.prototype.ReadFile = function(fileData)
{
	return post_to_url(Unity.FileSystem.serviceName, "ReadFile", get_params([fileData]), "POST");
}

/**
 * Writes contents to file on given path.<br/><br/>For further information see, {@link Unity.FileSystem.FileData FileData}.
 * @param {FileData} fileData The file to add/append contents to.
 * @param {byte[]} contents The data to be written to file.
 * @param {Boolean} appendFlag True if data should be appended to previous file data.
 * @return Boolean True if file could be written.
 * @version 1.0
 */
FileSystem.prototype.WriteFile = function(fileData, contents, appendFlag)
{
	return post_to_url(Unity.FileSystem.serviceName, "WriteFile", get_params([fileData,contents,appendFlag]), "POST");
}

/**
 * Copies the given file on "fromPath" to the "toPath". 
 * @param {String} sourceFileName The file name (relative path under "resources" application directory) to be copied from. 
 * @param {String} destFileName The file name (relative path under "documents" application directory) to be copied to.
 * @return Boolean True if file could be copied.
 * @version 1.1
 */
FileSystem.prototype.CopyFromResources = function(sourceFileName, destFileName)
{
	return post_to_url(Unity.FileSystem.serviceName, "CopyFromResources", get_params([sourceFileName,destFileName]), "POST");
}

/**
 * Copies the given remote file from "url" to the "toPath" (local relative path). 
 * @param {String} url The remote url file to be copied from. 
 * @param {String} destFileName The file name (relative path under "documents" application directory) to be copied to.
 * @return Boolean True if file could be copied.
 * @version 2.1
 */
FileSystem.prototype.CopyFromRemote = function(url, destFileName)
{
	return post_to_url(Unity.FileSystem.serviceName, "CopyFromRemote", get_params([url,destFileName]), "POST");
}

/*
 * Notification INTERFACES
 */
 
/**
 * @class Unity.Notification 
 * Singleton class field to access Notification interface. 
 * <br><br>This interface handles visual, audible, and tactile device notifications.<br>
 * <pre>Usage: Unity.Notification.&lt;metodName&gt;([params]).<br>Example: Unity.Notification.StartNotifyActivity().</pre>
 * @singleton
 * @constructor Constructs a new Notification interface.
 * @return System A new Notification interface.
 * @version 1.0
 */
Notification = function() {
	/**
	 * Notification service name (as configured on Platform Service Locator).
	 * @type String
 	 * @version 1.0
	 */
	this.serviceName = "notify";
}

Unity.Notification = new Notification();

/**
 * Shows and starts the activity indicator animation.
 * @return Boolean True if activity indicator could be started.
 * @version 1.0
 */
Notification.prototype.StartNotifyActivity = function()
{
	return post_to_url(Unity.Notification.serviceName, "StartNotifyActivity", null, "POST");
}

/**
 * Stops and hides the activity indicator animation.
 * @return Boolean True if activity indicator could be stopped.
 * @version 1.0
 */
Notification.prototype.StopNotifyActivity = function()
{
	return post_to_url(Unity.Notification.serviceName, "StopNotifyActivity", null, "POST");
}

/**
 * Checks if activity indicator animation is started.
 * @return Boolean True/false wheter activity indicator is running.
 * @version 1.0
 */
Notification.prototype.IsNotifyActivityRunning = function()
{
	return post_to_url(Unity.Notification.serviceName, "IsNotifyActivityRunning", null, "POST");
}

/**
 * Starts an alert notification.
 * @param {String} message The alert message to be displayed.
 * @param {String} title The alert title to be displayed.
 * @param {String} buttonText The accept button text to be displayed.
 * @return Boolean True if alert notification could be started.
 * @version 1.0
 */
Notification.prototype.StartNotifyAlert = function(message, title, buttonText)
{
	if(title == null && buttonText == null) {
		return post_to_url(Unity.Notification.serviceName, "StartNotifyAlert", get_params([message]), "POST");
	} else {
		return post_to_url(Unity.Notification.serviceName, "StartNotifyAlert", get_params([title,message,buttonText]), "POST");
	}
}

/**
 * Stops an alert notification.
 * @return Boolean True if alert notification could be stopped.
 * @version 1.0
 */
Notification.prototype.StopNotifyAlert = function()
{
	return post_to_url(Unity.Notification.serviceName, "StopNotifyAlert", null, "POST");
}

/**
 * Shows an action sheet.
 * @param {String} title The title to be displayed on the action sheet.
 * @param {String[]} buttons Array of button texts to be displayed. First index button is the "cancel" button, default button.
 * @param {String[]} jsCallbackFunctions The callback javascript functions as string texts for each of the given buttons. Empty string if no action is required for a button.
 * @return Boolean True if action sheet could be showed.
 * @version 1.0
 */
Notification.prototype.StartNotifyActionSheet = function(title, buttons, jsCallbackFunctions)
{
	return post_to_url(Unity.Notification.serviceName, "StartNotifyActionSheet", get_params([title, buttons, jsCallbackFunctions]), "POST");
}

/**
 * Starts a beep notification.
 * @return Boolean True if beep notification could be started.
 * @version 1.0
 */
Notification.prototype.StartNotifyBeep = function()
{
	return post_to_url(Unity.Notification.serviceName, "StartNotifyBeep", null, "POST");
}

/**
 * Stops the current beep notification.
 * @return Boolean True if beep notification could be stopped.
 * @version 1.0
 */
Notification.prototype.StopNotifyBeep = function()
{
	return post_to_url(Unity.Notification.serviceName, "StopNotifyBeep", null, "POST");
}

/**
 * Starts a blink notification.
 * @return Boolean True if beep notification could be started.
 * @version 1.0
 */
Notification.prototype.StartNotifyBlink = function()
{
	return post_to_url(Unity.Notification.serviceName, "StartNotifyBlink", null, "POST");
}

/**
 * Stops the current blink notification.
 * @return Boolean True if blink notification could be stopped.
 * @version 1.0
 */
Notification.prototype.StopNotifyBlink = function()
{
	return post_to_url(Unity.Notification.serviceName, "StopNotifyBlink", null, "POST");
}

/**
 * Shows and starts the progress indicator animation.
 * @return Boolean True if progress indicator animation could be started.
 * @version 1.0
 */
Notification.prototype.StartNotifyLoading = function(loadingText)
{
	if(loadingText == null) {
		return post_to_url(Unity.Notification.serviceName, "StartNotifyLoading", null, "POST");
	} else {
		return post_to_url(Unity.Notification.serviceName, "StartNotifyLoading", get_params([loadingText]), "POST");
	}
}

/**
 * Stops the current progress indicator animation.
 * @return Boolean True if progress indicator animation could be stopped.
 * @version 1.0
 */
Notification.prototype.StopNotifyLoading = function()
{
	return post_to_url(Unity.Notification.serviceName, "StopNotifyLoading", null, "POST");
}

/**
 * Checks if progress indicator animation is started.
 * @return Boolean True/false wheter progress indicator is running.
 * @version 1.0
 */
Notification.prototype.IsNotifyLoadingRunning = function()
{
	return post_to_url(Unity.Notification.serviceName, "IsNotifyLoadingRunning", null, "POST");
}

/**
 * Updates the progress indicator animation.
 * @param {float} progress The current progress; values between 0.0 and 1.0 (completed).
 * @version 1.0
 */
Notification.prototype.UpdateNotifyLoading = function(progress)
{
	return post_to_url(Unity.Notification.serviceName, "UpdateNotifyLoading", get_params([progress]), "POST");
}

/**
 * Starts a vibration notification.
 * @return Boolean True if vibration notification could be started.
 * @version 1.0
 */
Notification.prototype.StartNotifyVibrate = function()
{
	return post_to_url(Unity.Notification.serviceName, "StartNotifyVibrate", null, "POST");
}

/**
 * Stops the current vibration notification.
 * @return Boolean True if vibration notification could be stopped.
 * @version 1.0
 */
Notification.prototype.StopNotifyVibrate = function()
{
	return post_to_url(Unity.Notification.serviceName, "StopNotifyVibrate", null, "POST");
}

/**
 * Invokes all Notification API methods asynchronously.
 * Callback function is passed to the methods (last argument) to handle the result object when it is received from unity runtime.
 */
Notification.prototype.Async = {

/**
 * Shows and starts the activity indicator animation, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: True if activity indicator could be started.
 * @version 2.0
 */
StartNotifyActivity : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "StartNotifyActivity", null, "POST", callbackFunction);
},

/**
 * Stops and hides the activity indicator animation, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if activity indicator could be stopped.
 * @version 2.0
 */
StopNotifyActivity : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "StopNotifyActivity", null, "POST",callbackFunction);
},

/**
 * Checks if activity indicator animation is started, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True/false wheter activity indicator is running.
 * @version 2.0
 */
IsNotifyActivityRunning : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "IsNotifyActivityRunning", null, "POST", callbackFunction);
},

/**
 * Starts an alert notification, in ASYNC mode.
 * @param {String} message The alert message to be displayed.
 * @param {String} title The alert title to be displayed.
 * @param {String} buttonText The accept button text to be displayed.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if alert notification could be started.
 * @version 2.0
 */
StartNotifyAlert : function(message, title, buttonText, callbackFunction)
{
	if(title == null && buttonText == null) {
		post_to_url_async(Unity.Notification.serviceName, "StartNotifyAlert", get_params([message]), "POST", callbackFunction);
	} else {
		post_to_url_async(Unity.Notification.serviceName, "StartNotifyAlert", get_params([title,message,buttonText]), "POST", callbackFunction);
	}
},

/**
 * Stops an alert notification, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if alert notification could be stopped.
 * @version 2.0
 */
StopNotifyAlert : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "StopNotifyAlert", null, "POST", callbackFunction);
},

/**
 * Shows an action sheet, in ASYNC mode.
 * @param {String} title The title to be displayed on the action sheet.
 * @param {String[]} buttons Array of button texts to be displayed. First index button is the "cancel" button, default button.
 * @param {String[]} jsCallbackFunctions The callback javascript functions as string texts for each of the given buttons. Empty string if no action is required for a button.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if action sheet could be showed.
 * @version 2.0
 */
StartNotifyActionSheet : function(title, buttons, jsCallbackFunctions, callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "StartNotifyActionSheet", get_params([title, buttons, jsCallbackFunctions]), "POST", callbackFunction);
},

/**
 * Starts a beep notification, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if beep notification could be started.
 * @version 2.0
 */
StartNotifyBeep : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "StartNotifyBeep", null, "POST", callbackFunction);
},

/**
 * Stops the current beep notification, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if beep notification could be stopped.
 * @version 2.0
 */
StopNotifyBeep : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "StopNotifyBeep", null, "POST", callbackFunction);
},

/**
 * Starts a blink notification, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if beep notification could be started.
 * @version 2.0
 */
StartNotifyBlink : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "StartNotifyBlink", null, "POST", callbackFunction);
},

/**
 * Stops the current blink notification, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if blink notification could be stopped.
 * @version 2.0
 */
StopNotifyBlink : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "StopNotifyBlink", null, "POST", callbackFunction);
},

/**
 * Shows and starts the progress indicator animation, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if progress indicator animation could be started.
 * @version 2.0
 */
StartNotifyLoading : function(loadingText, callbackFunction)
{
	if(loadingText == null) {
		post_to_url_async(Unity.Notification.serviceName, "StartNotifyLoading", null, "POST", callbackFunction);
	} else {
		post_to_url_async(Unity.Notification.serviceName, "StartNotifyLoading", get_params([loadingText]), "POST", callbackFunction);
	}
},

/**
 * Stops the current progress indicator animation, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if progress indicator animation could be stopped.
 * @version 2.0
 */
StopNotifyLoading : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "StopNotifyLoading", null, "POST", callbackFunction);
},

/**
 * Checks if progress indicator animation is started, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True/false wheter progress indicator is running.
 * @version 2.0
 */
IsNotifyLoadingRunning : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "IsNotifyLoadingRunning", null, "POST", callbackFunction);
},

/**
 * Updates the progress indicator animation, in ASYNC mode.
 * @param {float} progress The current progress; values between 0.0 and 1.0 (completed).
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: no result object for this API method.
 * @version 2.0
 */
UpdateNotifyLoading : function(progress, callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "UpdateNotifyLoading", get_params([progress]), "POST", callbackFunction);
},

/**
 * Starts a vibration notification, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if vibration notification could be started.
 * @version 2.0
 */
StartNotifyVibrate : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "StartNotifyVibrate", null, "POST", callbackFunction);
},

/**
 * Stops the current vibration notification, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if vibration notification could be stopped.
 * @version 2.0
 */
StopNotifyVibrate : function(callbackFunction)
{
	post_to_url_async(Unity.Notification.serviceName, "StopNotifyVibrate", null, "POST", callbackFunction);
}

};

/*
 * I/O INTERFACES
 */

/**
 * @class Unity.IO 
 * Singleton class field to access IO interface. 
 * <br><br>This interface provides communication with external services, such as WebServices or Servlets... in many formats: JSON, XML, etx.<br>
 * <pre>Usage: Unity.IO.&lt;metodName&gt;([params]).<br>Example: Unity.IO.GetService(serviceName).</pre>
 * @singleton
 * @constructor Constructs a new IO interface.
 * @return System A new IO interface.
 * @version 1.0
 */
IO = function() {
	/**
	 * IO service name (as configured on Platform Service Locator).
	 * @type String
 	 * @version 1.0
	 */
	this.serviceName = "io";
	/**
	 * SOAP XML Service Type.
	 * @type int
 	 * @version 1.0
	 */
	this.SERVICETYPE_SOAP_XML = 0;
	/**
	 * SOAP JSON Service Type.
	 * @type int
 	 * @version 1.0
	 */
	this.SERVICETYPE_SOAP_JSON = 1;
	/**
	 * XML RPC Service Type.
	 * @type int
 	 * @version 1.0
	 */
	this.SERVICETYPE_XMLRPC_XML = 2;
	/**
	 * REST XML Service Type.
	 * @type int
 	 * @version 1.0
	 */
	this.SERVICETYPE_REST_XML = 3;
	/**
	 * JSON RPC Service Type.
	 * @type int
 	 * @version 1.0
	 */
	this.SERVICETYPE_XMLRPC_JSON = 4;
	/**
	 * REST JSON Service Type.
	 * @type int
 	 * @version 1.0
	 */
	this.SERVICETYPE_REST_JSON = 5;
	/**
	 * AMF Serialization Service Type.
	 * @type int
 	 * @version 1.0
	 */
	this.SERVICETYPE_AMF_SERIALIZATION = 6;
	/**
	 * Remoting Serialization Service Type.
	 * @type int
 	 * @version 1.0
	 */
	this.SERVICETYPE_REMOTING_SERIALIZATION = 7;
	/**
	 * Octet Binary Service Type.
	 * @type int
 	 * @version 1.0
	 */
	this.SERVICETYPE_OCTET_BINARY = 8;
	
	/**
	 * GWT RPC Service Type.
	 * @type int
 	 * @version 2.1
	 */
	this.SERVICETYPE_GWT_RPC = 9;
}

Unity.IO = new IO();

/**
 * Gets the configured I/O services (the ones configured on the '/app/config/io-services-config.xml' file).<br/><br/>For further information see, {@link Unity.IO.IOService IOService}.
 * @return IOService[] List of external services.
 * @version 1.0
 */
IO.prototype.GetServices = function()
{
	return post_to_url(Unity.IO.serviceName, "GetServices", null, "POST");
}

/**
 * Gets the I/O Service that matches the given name, and type (if provided). It is possible to define two services with the same name, but different type.
 * <br/><br/>For further information see, {@link Unity.IO.IOService IOService}.
 * <br/>Possible values of service type: 
 * {@link Unity.IO#SERVICETYPE_AMF_SERIALIZATION SERVICETYPE_AMF_SERIALIZATION}, 
 * {@link Unity.IO#SERVICETYPE_GWT_RPC SERVICETYPE_GWT_RPC}, 
 * {@link Unity.IO#SERVICETYPE_OCTET_BINARY SERVICETYPE_OCTET_BINARY}, 
 * {@link Unity.IO#SERVICETYPE_REMOTING_SERIALIZATION SERVICETYPE_REMOTING_SERIALIZATION}, 
 * {@link Unity.IO#SERVICETYPE_REST_JSON SERVICETYPE_REST_JSON}, 
 * {@link Unity.IO#SERVICETYPE_REST_XML SERVICETYPE_REST_XML},
 * {@link Unity.IO#SERVICETYPE_SOAP_JSON SERVICETYPE_SOAP_JSON} ,
 * {@link Unity.IO#SERVICETYPE_SOAP_XML SERVICETYPE_SOAP_XML},
 * {@link Unity.IO#SERVICETYPE_XMLRPC_JSON SERVICETYPE_XMLRPC_JSON},
 * & {@link Unity.IO#SERVICETYPE_XMLRPC_XML SERVICETYPE_XMLRPC_XML}
 * @param {String} serviceName The service name to look for.
 * @param {int} serviceType The service type to look for. Optional parameter.
 * @return IOService The external service matched.
 * @version 1.0
 */
IO.prototype.GetService = function(serviceName, serviceType)
{
	if(serviceType == null) {
		return post_to_url(Unity.IO.serviceName, "GetService",get_params([serviceName]), "POST");
	} else {
		return post_to_url(Unity.IO.serviceName, "GetService",get_params([serviceName,serviceType]), "POST");
	}
}

/**
 * Invokes the I/O Service that matches the given service name (or service object reference), and type (if provided).
 * <br/><br/>For further information see, {@link Unity.IO.IOService IOService}, {@link Unity.IO.IORequest IORequest} and {@link Unity.IO.IOResponse IOResponse}.
 * <br/>Possible values of service type: 
 * {@link Unity.IO#SERVICETYPE_AMF_SERIALIZATION SERVICETYPE_AMF_SERIALIZATION}, 
 * {@link Unity.IO#SERVICETYPE_GWT_RPC SERVICETYPE_GWT_RPC}, 
 * {@link Unity.IO#SERVICETYPE_OCTET_BINARY SERVICETYPE_OCTET_BINARY}, 
 * {@link Unity.IO#SERVICETYPE_REMOTING_SERIALIZATION SERVICETYPE_REMOTING_SERIALIZATION}, 
 * {@link Unity.IO#SERVICETYPE_REST_JSON SERVICETYPE_REST_JSON}, 
 * {@link Unity.IO#SERVICETYPE_REST_XML SERVICETYPE_REST_XML},
 * {@link Unity.IO#SERVICETYPE_SOAP_JSON SERVICETYPE_SOAP_JSON} ,
 * {@link Unity.IO#SERVICETYPE_SOAP_XML SERVICETYPE_SOAP_XML},
 * {@link Unity.IO#SERVICETYPE_XMLRPC_JSON SERVICETYPE_XMLRPC_JSON},
 * & {@link Unity.IO#SERVICETYPE_XMLRPC_XML SERVICETYPE_XMLRPC_XML}
 * @param {IORequestObject} requestObjt The request object with the needed invocation parameters. Example:<pre>{"Session":null,"Content":"{method:authenticationService.login,id:1,params:['username','password']}"}</pre>
 * @param {String/IOService} service This param could be a IOService object (as provided by {@link #GetService}), or only the service name. First service match would be invoked.
 * @param {int} serviceType The service type to look for. Optional parameter.
 * @return IOResponse The response object returned from remote service. Access content doing: <pre>ioResponse.Content</pre>
 * @version 1.0
 */
IO.prototype.InvokeService = function(requestObjt, service, serviceType)
{
	if(serviceType == null) {
		return post_to_url(Unity.IO.serviceName, "InvokeService",get_params([requestObjt,service]), "POST");
	} else {
		return post_to_url(Unity.IO.serviceName, "InvokeService",get_params([requestObjt,service,serviceType]), "POST");
	}
}

/**
 * Invokes all IO API methods asynchronously.
 * Callback function is passed to the methods (last argument) to handle the result object when it is received from unity runtime.
 */
IO.prototype.Async = {

/**
 * Gets ASYNC the configured I/O services (the ones configured on the '/app/config/io-services-config.xml' file).
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: IOService[] List of external services.
 * @version 2.0
 */
GetServices : function(callbackFunction)
{
	post_to_url_async(Unity.IO.serviceName, "GetServices", null, "POST", callbackFunction);
},

/**
 * Gets ASYNC the I/O Service that matches the given name, and type (if provided). It is possible to define two services with the same name, but different type.
 * @param {String} serviceName The service name to look for.
 * @param {int} serviceType The service type to look for. Optional parameter.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: "IOService" The external service matched.
 * @version 2.0
 */
GetService : function(serviceName, serviceType, callbackFunction)
{
	if(serviceType == null) {
        post_to_url_async(Unity.IO.serviceName, "GetService",get_params([serviceName]), "POST", callbackFunction);
	} else {
        post_to_url_async(Unity.IO.serviceName, "GetService",get_params([serviceName,serviceType]), "POST", callbackFunction);
	}
},

/**
 * Invokes ASYNC the I/O Service that matches the given service name (or service object reference), and type (if provided).
 * @param {IORequestObject} requestObjt The request object with the needed invocation parameters. Example:<pre>{"Session":null,"Content":"{method:authenticationService.login,id:1,params:['username','password']}"}</pre>
 * @param {String/IOService} service This param could be a IOService object (as provided by {@link #GetService}), or only the service name. First service match would be invoked.
 * @param {int} serviceType The service type to look for. Optional parameter.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: IOResponse The response object returned from remote service. Access content doing: <pre>ioResponse.Content</pre>.
 * @version 2.0
 */
InvokeService : function(requestObjt, service, serviceType, callbackFunction)
{
	if(serviceType == null) {
        post_to_url_async(Unity.IO.serviceName, "InvokeService",get_params([requestObjt,service]), "POST", callbackFunction);
	} else {
        post_to_url_async(Unity.IO.serviceName, "InvokeService",get_params([requestObjt,service,serviceType]), "POST", callbackFunction);
	}
}

};

/*
 * GEO INTERFACES
 */
 
/**
 * @class Unity.Geo 
 * Singleton class field to access Geo interface. 
 * <br><br>This interface provides access to GPS device features (getting current location, acceleration, etc) and embedded Map views, to locate/handle Points of Interest.<br>
 * <pre>Usage: Unity.Geo.&lt;metodName&gt;([params]).<br>Example: Unity.Geo.GetAcceleration().</pre>
 * @singleton
 * @constructor Constructs a new Geo interface.
 * @return System A new Geo interface.
 * @version 1.0
 */
Geo = function() {
	/**
	 * Geo service name (as configured on Platform Service Locator).
	 * @type String
 	 * @version 1.0
	 */
	this.serviceName = "geo";
	/**
	 * Magnetic North Type.
	 * @type int
 	 * @version 1.0
	 */
	this.NORTHTYPE_MAGNETIC = 0;
	/**
	 * True North Type.
	 * @type int
 	 * @version 1.0
	 */
	this.NORTHTYPE_TRUE	= 1;
}

Unity.Geo = new Geo();

/**
 * Gets the current device acceleration (measured in meters/second/second). <br/><br/>For further information see, {@link Unity.Geo.Acceleration Acceleration}.
 * @return Acceleration Current acceleration info (coordinates and acceleration vector number).
 * @version 1.0
 */
Geo.prototype.GetAcceleration = function()
{
	return post_to_url(Unity.Geo.serviceName, "GetAcceleration", null, "POST");
}

/**
 * Gets the current device location coordinates. <br/><br/>For further information see, {@link Unity.Geo.LocationCoordinate LocationCoordinate}.
 * @return LocationCoordinate Current location info (coordinates and precision).
 * @version 1.0
 */
Geo.prototype.GetCoordinates = function()
{
	return post_to_url(Unity.Geo.serviceName, "GetCoordinates", null, "POST");
}

/**
 * Gets the heading relative to the given north type (if 'northType' is not provided, default is used: magnetic noth pole).
 * <br/>Possible values of north type: 
 * {@link Unity.Geo#NORTHTYPE_MAGNETIC NORTHTYPE_MAGNETIC}, 
 * & {@link Unity.Geo#NORTHTYPE_TRUE NORTHTYPE_TRUE}
 * @param {int} northType Type of north to measured heading relative to it. Optional parameter.
 * @return float Current heading. Measured in degrees, minutes and seconds.
 * @version 1.0
 */
Geo.prototype.GetHeading = function(northType)
{
	var headingString = "0";
	if(northType == null) {
		headingString = post_to_url(Unity.Geo.serviceName, "GetHeading", null, "POST", true);  // "true" to get value as string, and parse to float here
	} else { 
		headingString = post_to_url(Unity.Geo.serviceName, "GetHeading", get_params([northType]), "POST", true); // "true" to get value as string, and parse to float here
	}
	headingString = headingString.replace(/,/, '.');  // change comma to points, if case.
	return parseFloat(headingString);
}

/**
 * Gets the orientation relative to the magnetic north pole.
 * @return float Current orientation. Measured in degrees, minutes and seconds.
 * @version 1.0
 */
Geo.prototype.GetDeviceOrientation = function()
{
	var orientationString = post_to_url(Unity.Geo.serviceName, "GetDeviceOrientation", null, "POST", true); // "true" to get value as string, and parse to float here
	orientationString = orientationString.replace(/,/, '.');  // change comma to points, if case.
	return parseFloat(orientationString);
}

/**
 * Gets the current device velocity.
 * @return float Device speed (in meters/second).
 * @version 1.0
 */
Geo.prototype.GetVelocity = function()
{
	var velocityString = post_to_url(Unity.Geo.serviceName, "GetVelocity", null, "POST", true); // "true" to get value as string, and parse to float here
	velocityString = velocityString.replace(/,/, '.');  // change comma to points, if case.
	return parseFloat(velocityString);
}

/**
 * Shows Map on screen.
 * @version 1.0
 */
Geo.prototype.GetMap = function()
{
	return post_to_url(Unity.Geo.serviceName, "GetMap", null, "POST");
}

/**
 * Specifies current map scale and bounding box radius.
 * @param {float} scale The desired map scale.
 * @param {float} boundingBox The desired map view bounding box.
 * @version 1.0
 */
Geo.prototype.SetMapSettings = function(scale, boundingBox)
{
	return post_to_url(Unity.Geo.serviceName, "SetMapSettings", get_params([scale,boundingBox]), "POST");
}

/**
 * List of POIs for the current location, given a radius (bounding box). Optionaly, a query text and/or a category could be added to search for specific conditions.
 * <br/><br/>For further information see, {@link Unity.Geo.POI POI}.
 * @param {LocationCoordinate} location Map location point to search nearest POIs.
 * @param {float} radius The radius around location to search POIs in.
 * @param {String} queryText The query to search POIs.. Optional parameter.
 * @param {LocationCategory} category The query to search POIs.. Optional parameter.
 * @return POI[] Points of Interest for location, ordered by distance.
 * @version 1.0
 */
Geo.prototype.GetPOIList = function(location, radius, queryText, category)
{
	if(queryText == null && category == null) {
		return post_to_url(Unity.Geo.serviceName, "GetPOIList", get_params([location,radius]), "POST");
	} else if(queryText != null && category == null) {
		return post_to_url(Unity.Geo.serviceName, "GetPOIList", get_params([location,radius,queryText]), "POST");
	} else if(queryText == null && category != null) {
		return post_to_url(Unity.Geo.serviceName, "GetPOIList", get_params([location,radius,category]), "POST");
	} else {
		return post_to_url(Unity.Geo.serviceName, "GetPOIList", get_params([location,radius,queryText,category]), "POST");
	}
}

/**
 * Gets a POI by the given id. <br/><br/>For further information see, {@link Unity.Geo.POI POI}.
 * @param {String} poiId POI identifier.
 * @return POI Point of Interest found.
 * @version 1.0
 */
Geo.prototype.GetPOI = function(poiId)
{
	return post_to_url(Unity.Geo.serviceName, "GetPOI", get_params([poiId]), "POST");
}

/**
 * Removes a POI given its id. <br/><br/>For further information see, {@link Unity.Geo.POI POI}.
 * @param {String} poiId POI identifier.
 * @version 1.0
 */
Geo.prototype.RemovePOI = function(poiId)
{
	return post_to_url(Unity.Geo.serviceName, "RemovePOI", get_params([poiId]), "POST");
}

/**
 * Moves a POI - given its id - to target location. <br/><br/>For further information see, {@link Unity.Geo.POI POI}.
 * @param {String} poiId POI identifier.
 * @version 1.0
 */
Geo.prototype.UpdatePOI = function(poi)
{
	return post_to_url(Unity.Geo.serviceName, "UpdatePOI", get_params([poi]), "POST");
}

/**
 * Starts the location services in order to get the latitude, longitude, altitude, speed, etc.
 * @return Boolean True if the device can start the location services
 * @version 1.0
 */
Geo.prototype.StartUpdatingLocation = function()
{
	return post_to_url(Unity.Geo.serviceName, "StartUpdatingLocation", null, "POST");
}

/**
 * Stops the location services in order to get the latitude, longitude, altitude, speed, etc.
 * @return Boolean True if the device can stop the location services
 * @version 1.0
 */
Geo.prototype.StopUpdatingLocation = function()
{
	return post_to_url(Unity.Geo.serviceName, "StopUpdatingLocation", null, "POST");
}

/**
 * Starts the heading services in order to get the latitude, longitude, altitude, speed, etc.
 * @return Boolean True if the device can start the location services
 * @version 1.0
 */
Geo.prototype.StartUpdatingHeading = function()
{
	return post_to_url(Unity.Geo.serviceName, "StartUpdatingHeading", null, "POST");
}

/**
 * Stops the heading services in order to get the latitude, longitude, altitude, speed, etc.
 * @return Boolean True if the device can stop the location services
 * @version 1.0
 */
Geo.prototype.StopUpdatingHeading = function()
{
	return post_to_url(Unity.Geo.serviceName, "StopUpdatingHeading", null, "POST");
}

/**
 * Performs a reverse geocoding in order to get, from the present latitude and longitude,
 * attributes like "County", "Street", "County code", "Location", ... in case such attributes
 * are available for that location.
 * <br/><br/>For further information see, {@link Unity.Geo.GeoDecoderAttributes GeoDecoderAttributes}.
 * @return GeoDecoderAttributes Reverse geocoding attributes from the present location (latitude and longitude)
 * @version 1.0
 */
Geo.prototype.GetGeoDecoder = function()
{
	return post_to_url(Unity.Geo.serviceName, "GetGeoDecoder", null, "POST");
}

/**
 * The proximity sensor detects an object close to the device.
 * @return Boolean True if the proximity sensor detects an object close to the device
 * @version 1.0
 */
Geo.prototype.StartProximitySensor = function()
{
	return post_to_url(Unity.Geo.serviceName, "StartProximitySensor", null, "POST");
}

/**
 * Stops the proximity sensor service.
 * @return Boolean True if the proximity sensor service could be stopped.
 * @version 1.0
 */
Geo.prototype.StopProximitySensor = function()
{
	return post_to_url(Unity.Geo.serviceName, "StopProximitySensor", null, "POST");
}

/*
 * MEDIA INTERFACES
 */

/**
 * @class Unity.Media 
 * Singleton class field to access Media interface. 
 * <br><br>This interface provides access to device's audio/movie player and camera applications.<br>
 * <pre>Usage: Unity.Media.&lt;metodName&gt;([params]).<br>Example: Unity.Media.Play(filePath).</pre>
 * @singleton
 * @constructor Constructs a new Media interface.
 * @return System A new Media interface.
 * @version 1.0
 */
Media = function() {
	/**
	 * Media service name (as configured on Platform Service Locator).
	 * @type String
 	 * @version 1.0
	 */
	this.serviceName = "media";
	/**
	 * Not Supported Media Type.
	 * @type int
 	 * @version 1.0
	 */
	this.MEDIATYPE_NOT_SUPPORTED = 0;
	/**
	 * Audio Media Type.
	 * @type int
 	 * @version 1.0
	 */
	this.MEDIATYPE_AUDIO = 1;
	/**
	 * Video Media Type.
	 * @type int
 	 * @version 1.0
	 */
	this.MEDIATYPE_VIDEO = 2;
	/**
	 * Photo Media Type.
	 * @type int
 	 * @version 1.0
	 */
	this.MEDIATYPE_PHOTO = 3;
	/**
	 * Playing Media State.
	 * @type int
 	 * @version 1.0
	 */
	this.MEDIATSTATE_PLAYING = 0;
	/**
	 * Recording Media State.
	 * @type int
 	 * @version 1.0
	 */
	this.MEDIATSTATE_RECORDING = 1;
	/**
	 * Paused Media State.
	 * @type int
 	 * @version 1.0
	 */
	this.MEDIATSTATE_PAUSED = 2;
	/**
	 * Stopped Media State.
	 * @type int
 	 * @version 1.0
	 */
	this.MEDIATSTATE_STOPPED = 3;
	/**
	 * Error Media State.
	 * @type int
 	 * @version 1.0
	 */
	this.MEDIATSTATE_ERROR = 4;
    
    /**
     * Handling events thown by the Unity Media API.
     * Method to be overrided by JS applications, to handle these events.
     * @method 
     * @param {UnityEvent} unityEvent The Unity Event sent by the Unity Media API. Data is provided under "data" property. The APi method that threw the event is provided under the "method" property.
     * @version 2.0
     */
    this.handleEvent = function(unityEvent){};
}

Unity.Media = new Media();

/**
 * Gets Media metadata.<br/><br/>For further information see, {@link Unity.Media.MediaMetadata MediaMetadata}.
 * @param {String} filePath The media file path.
 * @return MediaMetadata Media file metadata.
 * @version 1.0
 */
Media.prototype.GetMetadata = function(filePath)
{
	return post_to_url(Unity.Media.serviceName, "GetMetadata",  get_params([filePath]), "POST");
}

/**
 * Starts playing media.
 * @param {String} filePath The media file path.
 * @return Boolean True if media file could be started.
 * @version 1.0
 */
Media.prototype.Play = function(filePath)
{
	return post_to_url(Unity.Media.serviceName, "Play",  get_params([filePath]), "POST");
}

/**
 * Starts playing media.
 * @param {String} url The media remote URL.
 * @return Boolean True if media file could be started.
 * @version 1.0
 */
Media.prototype.PlayStream = function(url)
{
	return post_to_url(Unity.Media.serviceName, "PlayStream",  get_params([url]), "POST");
}

/**
 * Moves player to the given position in the media.
 * @param {long} position Index position.
 * @return Boolean True if player position could be moved.
 * @version 1.0
 */
Media.prototype.SeekPosition = function(position)
{
	return post_to_url(Unity.Media.serviceName, "SeekPosition",  get_params([position]), "POST");
}

/**
 * Stops the current media playing.
 * @return Boolean True if media file could be stopped.
 * @version 1.0
 */
Media.prototype.Stop = function()
{
	return post_to_url(Unity.Media.serviceName, "Stop",  null, "POST");
}

/**
 * Pauses the current media playing.
 * @return Boolean True if media file could be stopped.
 * @version 1.0
 */
Media.prototype.Pause = function()
{
	return post_to_url(Unity.Media.serviceName, "Pause",  null, "POST");
}

/**
 * Gets Audio/Movie player state.
 * <br/>Possible values of media states: 
 * {@link Unity.Media#MEDIATSTATE_ERROR MEDIATSTATE_ERROR}, 
 * {@link Unity.Media#MEDIATSTATE_PAUSED MEDIATSTATE_PAUSED}, 
 * {@link Unity.Media#MEDIATSTATE_PLAYING MEDIATSTATE_PLAYING}, 
 * {@link Unity.Media#MEDIATSTATE_RECORDING MEDIATSTATE_RECORDING}, 
 * & {@link Unity.Media#MEDIATSTATE_STOPPED MEDIATSTATE_STOPPED}
 * @return int Current player state.
 * @version 1.0
 */
Media.prototype.GetState = function()
{
	return post_to_url(Unity.Media.serviceName, "GetState",  null, "POST");
}

/**
 * Gets the currently playing media file metadata.<br/><br/>For further information see, {@link Unity.Media.MediaMetadata MediaMetadata}.
 * @return MediaMetadata Current media file metadata.
 * @version 1.0
 */
Media.prototype.GetCurrentMedia = function()
{
	return post_to_url(Unity.Media.serviceName, "GetCurrentMedia",  null, "POST");
}

/**
 * Opens user interface view to select a picture from the device photos album.<br/><br/>For further information see, {@link Unity.Media.MediaMetadata MediaMetadata}.
 * Data is provided via the proper event handled by the "Unity.Media.handleEvent" method; please, override to handle the event.
 * Returned value is "null" on synchronous call.
 * @return MediaMetadata Media file metadata picked from album. 
 * @version 2.0
 */
Media.prototype.GetSnapshot = function()
{
	return post_to_url(Unity.Media.serviceName, "GetSnapshot",  null, "POST");
}

/**
 * Opens user interface view to take a picture using the device camera.<br/><br/>For further information see, {@link Unity.Media.MediaMetadata MediaMetadata}.
 * Data is provided via the proper event handled by the "Unity.Media.handleEvent" method; please, override to handle the event.
 * Returned value is "null" on synchronous call.
 * @return MediaMetadata Media file metadata taken by the camera. 
 * @version 2.0
 */
Media.prototype.TakeSnapshot = function()
{
	return post_to_url(Unity.Media.serviceName, "TakeSnapshot",  null, "POST");
}

/**
 * Invokes all Media API methods asynchronously.
 * Callback function is passed to the methods (last argument) to handle the result object when it is received from unity runtime.
 */
Media.prototype.Async = {

/**
 * Gets Media metadata, in ASYNC mode.
 * @param {String} filePath The media file path.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: MediaMetadata Media file metadata.
 * @version 2.0
 */
GetMetadata : function(filePath, callbackFunction)
{
	post_to_url_async(Unity.Media.serviceName, "GetMetadata",  get_params([filePath]), "POST", callbackFunction);
},

/**
 * Starts playing media, in ASYNC mode.
 * @param {String} filePath The media file path.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if media file could be started.
 * @version 2.0
 */
Play : function(filePath, callbackFunction)
{
	post_to_url_async(Unity.Media.serviceName, "Play",  get_params([filePath]), "POST", callbackFunction);
},

/**
 * Starts playing media, in ASYNC mode.
 * @param {String} url The media remote URL.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if media file could be started.
 * @version 2.0
 */
PlayStream : function(url, callbackFunction)
{
	post_to_url_async(Unity.Media.serviceName, "PlayStream",  get_params([url]), "POST", callbackFunction);
},

/**
 * Moves player to the given position in the media, in ASYNC mode.
 * @param {long} position Index position.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if player position could be moved.
 * @version 2.0
 */
SeekPosition : function(position, callbackFunction)
{
	post_to_url_async(Unity.Media.serviceName, "SeekPosition",  get_params([position]), "POST", callbackFunction);
},

/**
 * Stops the current media playing, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if media file could be stopped.
 * @version 2.0
 */
Stop : function(callbackFunction)
{
	post_to_url_async(Unity.Media.serviceName, "Stop",  null, "POST", callbackFunction);
},

/**
 * Pauses the current media playing, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: Boolean True if media file could be stopped.
 * @version 2.0
 */
Pause : function(callbackFunction)
{
	post_to_url_async(Unity.Media.serviceName, "Pause",  null, "POST", callbackFunction);
},

/**
 * Gets Audio/Movie player state, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: MediaState Current player state.
 * @version 2.0
 */
GetState : function(callbackFunction)
{
	post_to_url_async(Unity.Media.serviceName, "GetState",  null, "POST", callbackFunction);
},

/**
 * Gets the currently playing media file metadata, in ASYNC mode.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: MediaMetadata Current media file metadata.
 * @version 2.0
 */
GetCurrentMedia : function(callbackFunction)
{
	post_to_url_async(Unity.Media.serviceName, "GetCurrentMedia",  null, "POST", callbackFunction);
},

/**
 * Opens user interface view to select a picture from the device photos album, in ASYNC mode.
 * Data is provided via the proper event handled by the "Unity.Media.handleEvent" method; please, override to handle the event.
 * Returned value is "null" on synchronous call.
 * @param {Object} callbackFunction The function object to be called when the method response is handled. Argument to this function is the invocation result object: MediaMetadata Media file metadata picked from album. 
 * @version 2.0
 */
GetSnapshot : function(callbackFunction)
{
	post_to_url_async(Unity.Media.serviceName, "GetSnapshot",  null, "POST", callbackFunction);
},

/**
 * Opens user interface view to take a picture using the device camera.
 * Data is provided via the proper event handled by the "Unity.Media.handleEvent" method; please, override to handle the event.
 * Returned value is "null" on synchronous call.
 * @return MediaMetadata Media file metadata taken by the camera. 
 * @version 2.0
 */
TakeSnapshot : function(callbackFunction)
{
	post_to_url_async(Unity.Media.serviceName, "TakeSnapshot",  null, "POST", callbackFunction);
}

};

/*
 * MESSAGING INTERFACES
 */

/**
 * @class Unity.Messaging 
 * Singleton class field to access Messaging interface. 
 * <br><br>This interface provides access to device's messaging and telephone applications.<br>
 * <pre>Usage: Unity.Messaging.&lt;metodName&gt;([params]).<br>Example: Unity.Messaging.SendEmail(emailData).</pre>
 * @singleton
 * @constructor Constructs a new Messaging interface.
 * @return System A new Messaging interface.
 * @version 1.0
 */
Messaging = function () {
	/**
	 * Messaging service name (as configured on Platform Service Locator).
	 * @type String
 	 * @version 1.0
	 */
	this.serviceName = "message";
}

Unity.Messaging = new Messaging();

/**
 * Sends a text message (SMS).
 * @param {String} phoneNumber The phone address to send the message to.
 * @param {String} text The message body.
 * @return Boolean True if SMS could be send.
 * @version 1.0
 */
Messaging.prototype.SendMessageSMS = function(phoneNumber, text)
{
	return post_to_url(Unity.Messaging.serviceName, "SendMessageSMS",  get_params([phoneNumber,text]), "POST");
}

/**
 * Sends a multimedia message (MMS).
 * @param {String} phoneNumber The phone address to send the message to.
 * @param {String} text The message body.
 * @param {AttachmentData} attachment Attachament data.
 * @return Boolean True if MMS could be send.
 * @version 1.0
 */
Messaging.prototype.SendMessageMMS = function(phoneNumber, text, attachment)
{
	return post_to_url(Unity.Messaging.serviceName, "SendMessageMMS",  get_params([phoneNumber,text, attachment]), "POST");
}

/**
 * Sends an email message.<br/><br/>For further information see, {@link Unity.Messaging.EmailData EmailData}.
 * @param {EmailData} emailData The email message data, such as: subject, 'To','Cc','Bcc' addresses, etc.
 * @return Boolean True if email could be send.
 * @version 1.0
 */
Messaging.prototype.SendEmail = function(emailData)
{
	return post_to_url(Unity.Messaging.serviceName, "SendEmail",  get_params([emailData]), "POST");
}

/*
 * PIM INTERFACES
 */

/**
 * @class Unity.Pim 
 * Singleton class field to access Pim interface. 
 * <br><br>This interface provides access to device's Contacts and Calendar applications.<br> PIM stands for 'Personal Information Management'<br>
 * <pre>Usage: Unity.Pim.&lt;metodName&gt;([params]).<br>Example: Unity.Pim.ListContacts(queryText).</pre>
 * @singleton
 * @constructor Constructs a new Pim interface.
 * @return System A new Pim interface.
 * @version 1.0
 */
Pim = function() {
	/**
	 * Pim service name (as configured on Platform Service Locator).
	 * @type String
 	 * @version 1.0
	 */
	this.serviceName = "pim";
	/**
	 * Query parameter name to search for contacts' name matching.
	 * @type String
 	 * @version 1.0
	 */
	this.CONTACTS_QUERY_PARAM_NAME = "name";
	/**
	 * Query parameter name to search for contacts' group matching.
	 * @type String
 	 * @version 1.0
	 */
	this.CONTACTS_QUERY_PARAM_GROUP = "group";
	/**
	 * Other Number Type.
	 * @type int
 	 * @version 1.0
	 */
	this.NUMBERTYPE_OTHER = 0;
	/**
	 * Mobile Number Type.
	 * @type int
 	 * @version 1.0
	 */
	this.NUMBERTYPE_MOBILE = 1;
	/**
	 * Fixed Line Number Type.
	 * @type int
 	 * @version 1.0
	 */
	this.NUMBERTYPE_FIXED_LINE = 2; 
	/**
	 * Work Number Type.
	 * @type int
 	 * @version 1.0
	 */
	this.NUMBERTYPE_WORK = 3;
	/**
	 * Home Fax Number Type.
	 * @type int
 	 * @version 1.0
	 */
	this.NUMBERTYPE_HOME_FAX = 4;
	/**
	 * WorkFax Number Type.
	 * @type int
 	 * @version 1.0
	 */
	this.NUMBERTYPE_WORK_FAX = 5; 
	/**
	 * Pager Number Type.
	 * @type int
 	 * @version 1.0
	 */
	this.NUMBERTYPE_PAGER = 6;
	/**
	 * Other Disposition Type.
	 * @type int
 	 * @version 1.0
	 */
	this.DISPOSITIONTYPE_OTHER = 0;
	/**
	 * Personal Disposition Type.
	 * @type int
 	 * @version 1.0
	 */
	this.DISPOSITIONTYPE_PERSONAL = 1;
	/**
	 * Work Disposition Type.
	 * @type int
 	 * @version 1.0
	 */
	this.DISPOSITIONTYPE_WORK = 2;
	/**
	 * HomeOffice Disposition Type.
	 * @type int
 	 * @version 1.0
	 */
	this.DISPOSITIONTYPE_HOME_OFFICE = 3;
	/**
	 * Other Calendar Type.
	 * @type int
 	 * @version 1.0
	 */
	this.CALENDARTYPE_OTHER = 0;
	/**
	 * Birthday Calendar Type.
	 * @type int
 	 * @version 1.0
	 */
	this.CALENDARTYPE_BIRTHDAY = 1;
	/**
	 * Calendaring Extensions to WebDAV (CalDAV) Calendar Type.
	 * @type int
 	 * @version 1.0
	 */
	this.CALENDARTYPE_CALDAV = 2;
	/**
	 * Exchange Calendar Type.
	 * @type int
 	 * @version 1.0
	 */
	this.CALENDARTYPE_EXCHANGE = 3;
	/**
	 * IMAP Calendar Type.
	 * @type int
 	 * @version 1.0
	 */
	this.CALENDARTYPE_IMAP = 4;
	/**
	 * Local Calendar Type.
	 * @type int
 	 * @version 1.0
	 */
	this.CALENDARTYPE_LOCAL = 5;
	/**
	 * Subscription Calendar Type.
	 * @type int
 	 * @version 1.0
	 */
	this.CALENDARTYPE_SUBSCRIPTION = 6;
	/**
	 * 'Needs Action' Attendee Status.
	 * @type int
 	 * @version 1.0
	 */
	this.ATTENDEESTATUS_NeedsAction = 0;
	/**
	 * 'Accepted' Attendee Status.
	 * @type int
 	 * @version 1.0
	 */
	this.ATTENDEESTATUS_ACCEPTED = 1;
	/**
	 * 'Declined' Attendee Status.
	 * @type int
 	 * @version 1.0
	 */
	this.ATTENDEESTATUS_DECLINED = 2;
	/**
	 * 'Tentative' Attendee Status.
	 * @type int
 	 * @version 1.0
	 */
	this.ATTENDEESTATUS_TENTATIVE = 3;
	/**
	 * Display Alarm Type.
	 * @type int
 	 * @version 1.0
	 */
	this.ALARM_DISPAY = 0;
	/**
	 * Email Alarm Type.
	 * @type int
 	 * @version 1.0
	 */
	this.ALARM_EMAIL = 1;
	/**
	 * Sound Alarm Type.
	 * @type int
 	 * @version 1.0
	 */
	this.ALARM_SOUND = 2;
	/**
	 * Weekly Recurrence Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RECURRENCE_WEEKLY = 0;
	/**
	 * Fortnightly Recurrence Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RECURRENCE_FORTNIGHTLY = 1;
	/**
	 * FourWeekly Recurrence Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RECURRENCE_FOURWEEKLY = 2;
	/**
	 * Montly Recurrence Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RECURRENCE_MONTLY = 3;
	/**
	 * Yearly Recurrence Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RECURRENCE_YEARLY = 4;
	/**
	 * None Relationship Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RELATIONSHIP_NONE = 0;
	/**
	 * Brother Relationship Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RELATIONSHIP_BROTHER = 1;
	/**
	 * Sister Relationship Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RELATIONSHIP_SISTER = 2;
	/**
	 * Parent Relationship Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RELATIONSHIP_PARENT = 3;
	/**
	 * Child Relationship Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RELATIONSHIP_CHILD = 4;
	/**
	 * Friend Relationship Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RELATIONSHIP_FRIEND = 5;
	/**
	 * Partner Relationship Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RELATIONSHIP_PARTNER = 6;
	/**
	 * Relative Relationship Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RELATIONSHIP_RELATIVE = 7;
	/**
	 * Spouse Relationship Type.
	 * @type int
 	 * @version 1.0
	 */
	this.RELATIONSHIP_SPOUSE = 8;
}

Unity.Pim = new Pim();

/**
 * List of stored phone contacts that match given query. <br/><br/>For further information see, {@link Unity.Pim.Contact Contact}.
 * @param {String} queryText The search query text. Optional parameter.<pre>Format is: &lt;queryParam1Name&gt;=&lt;queryParam1Value&gt;&&lt;queryParam2Name&gt;=&lt;queryParam2Value&gt;&....</pre>
 * @return Contact[] List of contacts.
 * @version 1.0
 */
Pim.prototype.ListContacts = function(queryText)
{
	if(queryText == null) {
		 return post_to_url(Unity.Pim.serviceName, "ListContacts",  null, "POST");
	} else {
		return post_to_url(Unity.Pim.serviceName, "ListContacts",  get_params([queryText]), "POST");
	}
}

/**
 * Creates a Contact based on given contact data. <br/><br/>For further information see, {@link Unity.Pim.Contact Contact}.
 * @param {Contact} contact Contact data to be created.
 * @return Contact Created contact.
 * @version 1.0
 */
Pim.prototype.CreateContact = function(contact)
{
	return post_to_url(Unity.Pim.serviceName, "CreateContact",  get_params([contact]), "POST");
}

/**
 * Updates contact data (given its ID) with the given contact data. <br/><br/>For further information see, {@link Unity.Pim.Contact Contact}.
 * @param {string} contactId Contact identifier to be updated with new data.
 * @param {Contact} newContact New contact data to be added to the given contact.
 * @return Boolean True on successful updating.
 * @version 1.0
 */
Pim.prototype.UpdateContact = function(contactId, newContactData)
{
	return post_to_url(Unity.Pim.serviceName, "UpdateContact",  get_params([contactId,newContactData]), "POST");
}

/**
 * Deletes the given contact. <br/><br/>For further information see, {@link Unity.Pim.Contact Contact}.
 * @param {Contact} contact Contact data to be deleted.
 * @return Boolean True on successful deletion.
 * @version 1.0
 */
Pim.prototype.DeleteContact = function(contact)
{
	return post_to_url(Unity.Pim.serviceName, "DeleteContact",  get_params([contact]), "POST");
}

/**
 * Lists calendar entries for given date. <br/><br/>For further information see, {@link Unity.Pim.CalendarEntry CalendarEntry}.
 * @param {DateTime} date Date to match calendar entries.
 * @return CalendarEntry[] List of calendar entries.
 * @version 1.0
 */
Pim.prototype.ListCalendarEntriesByDate = function(date)
{
	return post_to_url(Unity.Pim.serviceName, "ListCalendarEntries",  get_params([date]), "POST");
}

/**
 * Lists calendar entries between given start and end dates. <br/><br/>For further information see, {@link Unity.Pim.CalendarEntry CalendarEntry}.
 * @param {DateTime} startDate Start date to match calendar entries.
 * @param {DateTime} endDate End date to match calendar entries.
 * @return CalendarEntry[] List of calendar entries.
 * @version 1.0
 */
Pim.prototype.ListCalendarEntriesByDateRange = function(startDate, endDate)
{
	return post_to_url(Unity.Pim.serviceName, "ListCalendarEntries",  get_params([startDate,endDate]), "POST");
}

/**
 * Creates a calendar entry. <br/><br/>For further information see, {@link Unity.Pim.CalendarEntry CalendarEntry}.
 * @param {CalendarEntry} entry Calendar entry to be created.
 * @return CalendarEntry Created calendar entry.
 * @version 1.0
 */
Pim.prototype.CreateCalendarEntry = function(entry)
{
	return post_to_url(Unity.Pim.serviceName, "CreateCalendarEntry",  get_params([entry]), "POST");
}

/**
 * Deletes the given calendar entry. <br/><br/>For further information see, {@link Unity.Pim.CalendarEntry CalendarEntry}.
 * @param {CalendarEntry} entry Calendar entry to be deleted.
 * @return Boolean True on successful deletion.
 * @version 1.0
 */
Pim.prototype.DeleteCalendarEntry = function(entry)
{
	return post_to_url(Unity.Pim.serviceName, "DeleteCalendarEntry",  get_params([entry]), "POST");
}

/**
 * Moves the given calendar entry to the new start and end dates. <br/><br/>For further information see, {@link Unity.Pim.CalendarEntry CalendarEntry}.
 * @param {CalendarEntry} entry Calendar entry to be moved. 
 * @param {DateTime} startDate New start date to move the calendar entry.
 * @param {DateTime} endDate New end date to move the calendar entry.
 * @return Boolean True on successful deletion.
 * @version 1.0
 */
Pim.prototype.MoveCalendarEntry = function(entry, startDate, endDate)
{
	return post_to_url(Unity.Pim.serviceName, "MoveCalendarEntry",  get_params([entry,startDate,endDate]), "POST");
}

/*
 * TELEPHONY INTERFACES
 */

/**
 * @class Unity.Telephony 
 * Singleton class field to access Telephony interface. 
 * <br><br>This interface provides access to device's Telephony application.<br>
 * <pre>Usage: Unity.Telephony.&lt;metodName&gt;([params]).<br>Example: Unity.Telephony.Call('3493xxxxxxx',1).</pre>
 * @singleton
 * @constructor Constructs a new Telephony interface.
 * @return System A new Telephony interface.
 * @version 1.0
 */
Telephony = function() {
	/**
	 * Telephony service name (as configured on Platform Service Locator).
	 * @type String
	 * @version 1.0
	 */
	this.serviceName = "phone";
	/**
	 * Dialing Call Status.
	 * @type int
	 * @version 1.0
	 */
	this.STATUS_DIALING = 0;
	/**
	 * Establishing Call Status.
	 * @type int
	 * @version 1.0
	 */
	this.STATUS_ESTABLISHING = 1; 
	/**
	 * Established Call Status.
	 * @type int
	 * @version 1.0
	 */
	this.STATUS_ESTABLISHED = 2;
	/**
	 * Dropped Call Status.
	 * @type int
	 * @version 1.0
	 */
	this.STATUS_DROPPED = 3;
	/**
	 * Failed Call Status.
	 * @type int
	 * @version 1.0
	 */
	this.STATUS_FAILED = 4;
	/**
	 * Busy Call Status.
	 * @type int
	 * @version 1.0
	 */
	this.STATUS_BUSY = 5;
	/**
	 * Finished Call Status.
	 * @type int
	 * @version 1.0
	 */
	this.STATUS_FINISHED = 6;
	/**
	 * Voice Call Type.
	 * @type int
	 * @version 1.0
	 */
	this.CALLTYPE_VOICE = 0;
	/**
	 * Fax Call Type.
	 * @type int
	 * @version 1.0
	 */
	this.CALLTYPE_FAX = 1;
	/**
	 * Dial Up Call Type.
	 * @type int
	 * @version 1.0
	 */
	this.CALLTYPE_DIALUP = 2;
}

Unity.Telephony = new Telephony();

/**
 * Shows and starts a phone call. 	
 * <br/>Possible values of the 'callType' argument: 
 * {@link Unity.Telephony#CALLTYPE_VOICE CALLTYPE_VOICE}, 
 * {@link Unity.Telephony#CALLTYPE_FAX CALLTYPE_FAX}, 
 * & {@link Unity.Telephony#CALLTYPE_DIALUP CALLTYPE_DIALUP}
 * @param {String} number Phone number to call to.
 * @param {int} callType The type of call to open.
 * @return ICallControl Call control interface to handle current call.
 * @version 1.0
 */
Telephony.prototype.Call = function(number, callType)
{
	return post_to_url(Unity.Telephony.serviceName, "Call",  get_params([number,callType]), "POST");
}

/*
 * I18N INTERFACES
 */

/**
 * @class Unity.I18N 
 * Singleton class field to access I18N interface. 
 * <br><br>This interface provides features to build your application with 'localized' (centralized on external files) and 'internationalized' (for different languages) texts.<br>
 * <pre>Usage: Unity.I18N.&lt;metodName&gt;([params]).<br>Example: Unity.I18N.GetResourceLiteral('myKey').</pre>
 * @singleton
 * @constructor Constructs a new I18N interface.
 * @return System A new I18N interface.
 * @version 1.0
 */
I18N = function() {
	/**
	 * I18N service name (as configured on Platform Service Locator).
	 * @type String
	 * @version 1.0
	 */
	this.serviceName = "i18n";
}

Unity.I18N = new I18N();


/**
 * List of supported locales for the application (the ones configured on the '/app/config/i18n-config.xml' file).
 *  <br/><br/>For further information see, {@link Unity.I18N.Locale Locale}.
 * @return Locale[] List of locales.
 * @version 1.0
 */
I18N.prototype.GetLocaleSupported = function()
{
	return post_to_url(Unity.I18N.serviceName, "GetLocaleSupported",  null, "POST");
}

/**
 * List of supported locales for the application (the ones configured on the '/app/config/i18n-config.xml' file).
 * <br/><br/>For further information see, {@link Unity.I18N.Locale Locale}. 
 * @return String[] List of locales (only locale descriptor string, such as "en-US").
 * @version 1.0
 */
I18N.prototype.GetLocaleSupportedDescriptors = function()
{
	return post_to_url(Unity.I18N.serviceName, "GetLocaleSupportedDescriptors",  null, "POST");
}

/**
 * Gets the text/message corresponding the given key and locale.
 * <br/><br/>For further information see, {@link Unity.I18N.Locale Locale}.
 * @param {String} key The key to match text.
 * @param {String/Locale} locale The full locale object to get localized message, or the locale desciptor ("language" or "language-country" two-letters ISO codes.
 * @return String Localized text.
 * @version 1.0
 */
I18N.prototype.GetResourceLiteral = function(key, locale)
{
	if(locale == null) {
		return post_to_url(Unity.I18N.serviceName, "GetResourceLiteral",  get_params([key]), "POST");
	} else {
		return post_to_url(Unity.I18N.serviceName, "GetResourceLiteral",  get_params([key,locale]), "POST");
	}
}

/*
 * LOG INTERFACES
 */

/**
 * @class Unity.Log 
 * Singleton class field to access Log interface. 
 * <br><br>This interface provides features to log message to the environment standard console.<br>
 * <pre>Usage: Unity.Log.&lt;metodName&gt;([params]).<br>Example: Unity.Log.Log('myKey').</pre>
 * @singleton
 * @constructor Constructs a new Log interface.
 * @return System A new Log interface.
 * @version 1.0
 */
Log = function() {
	/**
	 * Log service name (as configured on Platform Service Locator).
	 * @type String
	 * @version 1.0
	 */
	this.serviceName = "log";
}

Unity.Log = new Log();


/**
 * Logs the given message, with the given log level if specified, to the standard platform/environment.
 * @param {String} message The message to be logged.
 * @param {int} level The log level (optional).
 * @return Boolean True on successful logged.
 * @version 1.1
 */
Log.prototype.Log = function(message, level)
{
	if(level == null) {
		return post_to_url(Unity.Log.serviceName, "Log",  get_params([message]), "POST");
	} else {
		return post_to_url(Unity.Log.serviceName, "Log",  get_params([message,key]), "POST");
	}
}

/*
 * COMMON FUNCTIONS
 */

/**
 * @ignore
 * This method is used to build the JSON string request from the given invocation parameters array.
 * @param {Object[]} paramsArray Array of invocation parameters.
 * @return String The JSON string request to be send when invoking Unity Platform services.
 * <pre>Returned string format is: {"param1":&lt;paramsArray[0] in JSON string format&gt;,"param2":&lt;paramsArray[1] in JSON string format&gt;,...}</pre>
 * @version 1.0
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
 * This method is used to invoke Unity Service method, using synchronous XMLHttpRequest call.
 * @param {String} serviceName The service name (as configured on Platform Service Locator).
 * @param {String} methodName The method name as defined on the given service.
 * @param {String} params The JSON string request qith the params needed for method invocation. Null value if no invocation arguments are required.
 * @param {String} method The request method. POST or GET. If nor provided, default is POST.
 * @return Object Service invocation returned object (javacript object).
 * @version 1.0
 */
function post_to_url(serviceName, methodName, params, method, returnAsString) {
	method = method || "POST"; // Set method to post by default, if not specified.

	var path = Unity.SERVICE_URI + serviceName + "/" + methodName;
	
	if(Unity.isBackground()) {
		// socket is closed, do not call unity services
		console.log("Application is on background. Internal Unity Socket is closed. Call to '" + path + "' has been stopped.");
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
		reqData = "json=" + unescape(params);
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
			console.log("wrong responseText received from Unity calls: " + responseText);
			return null;
		}
	} else {
		return null;
	}
}


function post_to_url_async(serviceName, methodName, params, method, callBackFunc) {
    method = method || "POST"; // Set method to post by default, if not specified.

	var path = Unity.SERVICE_URI + serviceName + "/" + methodName;
	
	if(Unity.isBackground()) {
		// socket is closed, do not call unity services
		console.log("Application is on background. Internal Unity Socket is closed. Call to '" + path + "' has been stopped.");
		return null;
	}
	
	
	var xhr = new XMLHttpRequest(); 
	/* ASYNCHRONOUS OPTION	*/
	xhr.onreadystatechange  = function()
    {    
        if(xhr.readyState  == 4)  //The 4 state means for the response is ready and sent by the server. 
         {  
              if(xhr.status  == 200) { //This status means ok, otherwise some error code is returned, 404 for example.
                  var responseText = xhr.responseText;
                  if(responseText!=null) {
					try {
                    	var result = eval('(' + responseText + ')');
                    	if(callBackFunc) {
                        	callBackFunc(result);
                    	}
					} catch(e){
						console.log("wrong responseText received from Unity calls: " + responseText);
						return null;
					}
                  } else {
                    if(callBackFunc) {callBackFunc(null);}
                  }
              }
              else {
                 console.dir("Error code " + xhr.status);
              } 
         }
    }; 
	
    xhr.open( method, path, false); 
	xhr.setRequestHeader("Content-type", "application/json");
	var reqData = null;
	if(params!=null) {
		reqData = "json=" + unescape(params);
	}
	try {
		xhr.send(reqData);
	} catch (e) {
        console.dir("error sending data async: " + reqData);
    }
}

/**
 * @class Unity.JSON
 * This is the utility JSON class. 
 * <br><br>This class provides functions to handle JSON strings (from JS object to JSON string).<br>
 * <pre>Usage: JSON.stringify(&lt;Object&gt;).<br>Example: JSON.stringify(myObject).</pre>
 * @singleton
 * @version 1.0
 */ 
JSON = new function() {
	// javadoc utility to show singleton classes.
	return JSON || {};  
	// this allows including already specified JSON utilities (for example, the ones implemented by Sencha or PastryKit JS frameworks)
	// if JSON utilities are not already implemented, below "stringify" function is used.
};

var JSON = JSON || {};

/**
 * Method shortcut. See {@link Unity.JSON#toJSONString}.
 * @type String.
 * @version 1.0
 */
JSON.stringify = JSON.stringify || JSON.toJSONString;
/**
 * This method implements JSON.stringify serialization.
 * @param {Object} obj Object to be converted to JSON string format. See <a href="http://www.json.org/example.html">JSON (JavaScript Object Notation)</a>  for JSON format examples.
 * @return String The JSON string that represents the given object.
 * @version 1.0
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
 * @return String The formatted result string.
 * @version 1.0
 */	
String.prototype.format = function() {  
    var pattern = /\{\d+\}/g;
    var args = arguments;
    return this.replace(pattern, function(capture){ return args[capture.match(/\d+/)]; });
} 


/*
 * Initialize Unity Utility "is" object
 *
 */
Unity.is.init();