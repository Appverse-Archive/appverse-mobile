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

// Specific Appverse Emulator settings, not applied in real device platforms
AppverseEmulator = {
    queuedListenerMessagesCount: 0,
    queuedCallbackMessagesCount: 0,
    pollEnabled: true,
    pollingInterval: 50, // milliseconds
    pollingTotalTimeout: 10000, // 10 seconds //check if it is required to be used
    eventListenersRegistered: [],
    simulateNetworkToOffline: function(){Appverse.Net.NetworkStatus = Appverse.Net.NETWORKTYPE_UNKNOWN; Appverse.Net.onConnectivityChange(Appverse.Net.NetworkStatus);},
    simulateNetworkToWifi: function(){Appverse.Net.NetworkStatus = Appverse.Net.NETWORKTYPE_WIFI; Appverse.Net.onConnectivityChange(Appverse.Net.NetworkStatus);},
    simulateNetworkToCarrier: function(){Appverse.Net.NetworkStatus = Appverse.Net.NETWORKTYPE_3G; Appverse.Net.onConnectivityChange(Appverse.Net.NetworkStatus);}
};

Appverse = {
    version: "5.0.7",
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
    init: function() {

        if (Appverse.executingInEmulator) {
            if(localStorage.getItem('_AppverseContext')) Appverse.initAppverseContext();
            console.warn('%c WARNING - This code should only be executed in Appverse MobileEmulator. ', 'background: #222; color: white');
            console.log("Appverse Emulator - queue result for Appverse Context");
            AppverseEmulator.queuedListenerMessagesCount++;
            Appverse.checkAppverseContextData();

            auxInterval = setInterval(function() {
                //only for TESTING console.log("checking interval: " + AppverseEmulator.queuedListenerMessagesCount);
                if (AppverseEmulator.queuedListenerMessagesCount == 0) {

                    Appverse.initAppverseContext();

                    clearInterval(auxInterval);
                }
            }, 100);
			
			Appverse.SERVICE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.SERVICE_URI;
			Appverse.REMOTE_RESOURCE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.REMOTE_RESOURCE_URI;
			Appverse.DOCUMENTS_RESOURCE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.DOCUMENTS_RESOURCE_URI;
			Appverse.MODULES_RESOURCE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.MODULES_RESOURCE_URI;
			
        } else {
			Appverse.APPVERSE_URI = 'https://appverse';
			Appverse.APPVERSE_SERVICE_URI = Appverse.APPVERSE_URI + Appverse.SERVICE_URI;
			Appverse.SERVICE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.SERVICE_URI;  // required for legacy android versions
			Appverse.REMOTE_RESOURCE_URI = Appverse.APPVERSE_URI + Appverse.REMOTE_RESOURCE_URI;
			
			Appverse.initAppverseContext();
			
			if(Appverse.is.Android && !window.appverseJSBridge) {   
				// use legacy serer for older android versions
				Appverse.DOCUMENTS_RESOURCE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.DOCUMENTS_RESOURCE_URI;
				Appverse.MODULES_RESOURCE_URI = 'http://127.0.0.1:' + LOCAL_SERVER_PORT + Appverse.MODULES_RESOURCE_URI;
			} else {
				Appverse.DOCUMENTS_RESOURCE_URI = Appverse.APPVERSE_URI + Appverse.DOCUMENTS_RESOURCE_URI;
				Appverse.MODULES_RESOURCE_URI = Appverse.APPVERSE_URI + Appverse.MODULES_RESOURCE_URI;
			}
        }
    },
    checkAppverseContextData: function() {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', "/!appverse_emulator_poll?" + (+new Date()), false);

        try {
            xhr.send("mn=init#AppverseContext");
        } catch (e) {
            console.log("Error polling Appverse emulator queued messages..." + e);
        }

        if (xhr.status == 200) {  //This status means ok, otherwise some error code is returned, 404 for example.
            responseText = xhr.responseText;

            try {
                if (responseText != null && responseText != '' && responseText != 'listener_result_not_yet_avaliable') {
                    console.log("processing Appverse Context queued message... ");
                    // ONLY FOR TESTING console.log(responseText);
                    eval(responseText); // evaluate JS code, not JSON (no enclosing brackets are needed here)
                    AppverseEmulator.queuedListenerMessagesCount--;
                }
            } catch (e) {
                console.log("Error parsing Appverse emulator queued message..." + e + " - Response received: " + responseText);
            }
        }
        else {
            console.log("Error polling Appverse emulator  queued messages. Error code: " + xhr.status);
        }
    },
    initAppverseContext: function() {
        console.log("Initializing Appverse Context...");
        if (typeof(_AppverseContext) != "undefined") {
            this.is = _AppverseContext;
            localStorage.setItem('_AppverseContext', JSON.stringify(_AppverseContext));
            delete _AppverseContext;
        } else if (_AppverseContext = window.localStorage.getItem("_AppverseContext")) {
            this.is = JSON.parse(_AppverseContext);  
            delete _AppverseContext;          
        } else {
            console.log("WARNING: Appverse Context cannot be properly intialized. Missing Appverse.is information. Please check the running platform.");
            this.is = {};
        }

        if (typeof(_OSInfo) != "undefined") {
            this.OSInfo = _OSInfo;
            localStorage.setItem('_OSInfo', JSON.stringify(_OSInfo));
            delete _OSInfo;
        } else if (_OSInfo = window.localStorage.getItem('_OSInfo')) {
            this.OSInfo = JSON.parse(_OSInfo);
			delete _OSInfo;
        } else {
            console.log("WARNING: Appverse Context cannot be properly intialized. Missing OS information. Please check the running platform.");
            this.OSInfo = {};
        }

        if (typeof(_HwInfo) != "undefined") {
            this.HwInfo = _HwInfo;
            localStorage.setItem('_HwInfo', JSON.stringify(_HwInfo));
            delete _HwInfo;
        } else if (_HwInfo = window.localStorage.getItem('_HwInfo')) {
            this.HwInfo = JSON.parse(_HwInfo);
			delete _HwInfo;
        } else {
            console.log("WARNING: Appverse Context cannot be properly intialized. Missing Hardware information. Please check the running platform.");
            this.HwInfo = {};
        }

        if (typeof(_i18n) != "undefined") {
            this.LocalizedLiterals = _i18n;
            localStorage.setItem('_i18n', JSON.stringify(_i18n));
            delete _i18n;
        } else if (_i18n = window.localStorage.getItem('_i18n')) {
            this.LocalizedLiterals = JSON.parse(_i18n);
			delete _i18n;
        } else {
            console.log("WARNING: Appverse Context cannot be properly intialized. Missing i18n information. Please check the running platform.");
            this.LocalizedLiterals = {};
        }

        if (typeof(_CurrentDeviceLocale) != "undefined") {
            this.CurrentDeviceLocale = _CurrentDeviceLocale;
            localStorage.setItem('_CurrentDeviceLocale', JSON.stringify(_CurrentDeviceLocale));
            delete _CurrentDeviceLocale;
        } else if (_CurrentDeviceLocale = window.localStorage.getItem('_CurrentDeviceLocale')) {
            this.CurrentDeviceLocale = JSON.parse(_CurrentDeviceLocale);
			delete _CurrentDeviceLocale;
        } else {
            console.log("WARNING: Appverse Context cannot be properly intialized. Missing Current Device Locale information. Please check the running platform.");
            this.CurrentDeviceLocale = {};
        }

        if (typeof(_IOServices) != "undefined") {
            this.IOServices = _IOServices;
            localStorage.setItem('_IOServices', JSON.stringify(_IOServices));
            delete _IOServices;
        } else if (_IOServices = window.localStorage.getItem('_IOServices')) {
            this.IOServices = JSON.parse(_IOServices);
			delete _IOServices;
        } else {
            console.log("WARNING: Appverse Context cannot be properly intialized. Missing IO Services information. Please check the running platform.");
            this.IOServices = [];
        }
		
        if (typeof(_NetworkStatus) != "undefined") {
            this.Net.NetworkStatus = _NetworkStatus;
			localStorage.setItem('_NetworkStatus', Appverse.Net.NetworkStatus);
            this.Net.onConnectivityChange(this.Net.NetworkStatus);
            delete _NetworkStatus;
        }else if (_NetworkStatus = window.localStorage.getItem('_NetworkStatus')) {
            this.Net.NetworkStatus = JSON.parse(_NetworkStatus);
            this.Net.onConnectivityChange(this.Net.NetworkStatus);
            delete _NetworkStatus;
        } else {
            console.log("WARNING: Appverse Context cannot be properly intialized. Missing Network status information. Please check the running platform.");
            this._NetworkStatus = 0;
        }
        
        /*
         * Ensure Sencha Touch Framework to take the correct device format factor 
         */
        if(Appverse.is.Tablet && !location.search) location.search = "deviceType=Tablet";

        if (typeof(this.is.Emulator) != "undefined" && this.is.Emulator == true) {
            var consoleBuffer = "platform is EMULATOR";
            if (this.is.iOS) {
                consoleBuffer = consoleBuffer + ", orientation: " + this.is.EmulatorOrientation;
                window.orientation = this.is.EmulatorOrientation;
                if (Appverse.is.Tablet) {
                    window.deviceType = 'iPad'; // used by some JS frameworks to determine device type (phone/tablet) in iOS.
                    consoleBuffer = consoleBuffer + ", window.deviceType: iPad";                    

                }
                if (typeof(this.is.EmulatorScreen) != "undefined") {
                    try {
                        window.screen = eval('(' + this.is.EmulatorScreen + ')');
                        consoleBuffer = consoleBuffer + ", window.screen: " + JSON.stringify(window.screen);
                    } catch (e) {
                        console.log("error setting window.screen", e);
                    }
                }
            }
			console.log(consoleBuffer);
            post_to_url_async = post_to_url_async_emu;

            // ********************** enable JS queued messages for emulator [MOBDEVKIT-85]

            // polling for queued listener message each polling interval
            AppverseEmulator.appverseListenerPollingTimerFunc = function(mn) {
                if (AppverseEmulator.pollEnabled) {
                    AppverseEmulator.appverseListenerPollOnce(mn);
                    console.log("should keep processing listener messages ? " + AppverseEmulator.queuedListenerMessagesCount);
                    if (AppverseEmulator.queuedListenerMessagesCount > 0) {
                        console.log("keep processing listener messages...");
                        
						(function(m) {
                            setTimeout(function() {
                                AppverseEmulator.appverseListenerPollingTimerFunc(m);
                            }, AppverseEmulator.pollingInterval);
                        })(mn);
                    }
                }
            };

            // polling for queued listener message each polling interval
            AppverseEmulator.appverseCallbackPollingTimerFunc = function(cbfn, cbid) {
                if (AppverseEmulator.pollEnabled) {
                    AppverseEmulator.appverseCallbackPollOnce(cbfn, cbid);
                    if (AppverseEmulator.queuedCallbackMessagesCount > 0) {
                        console.log("keep processing callback messages...");

                        (function(c, ci) {
                            setTimeout(function() {
                                AppverseEmulator.appverseCallbackPollingTimerFunc(c, ci);
                            }, AppverseEmulator.pollingInterval);
                        })(cbfn, cbid);
                    }
                }
            };

            AppverseEmulator.appverseListenerPollOnce = function(methodName) {
                var xhrL = new XMLHttpRequest();
                xhrL.open('POST', "/!appverse_emulator_poll?" + (+new Date()), false);

                try {
                    xhrL.send("mn=" + methodName);
                } catch (e) {
                    console.log("Error polling Appverse emulator queued messages..." + e);
                }

                if (xhrL.status == 200) {  //This status means ok, otherwise some error code is returned, 404 for example.
                    responseText = xhrL.responseText;

                    try {
                        var xhrResponse = null;
                        if (responseText != null && responseText != '' && responseText != 'listener_result_not_yet_avaliable') {
                            console.log("processing listener queued message... ");
                            xhrResponse = eval(responseText); // evaluate JS code, not JSON (no enclosing brackets are needed here)
                            AppverseEmulator.queuedListenerMessagesCount--;
                        }
                    } catch (e) {
                        console.log("Error parsing Appverse emulator listener queued message..." + e + " - Response received: " + responseText);
                    }
                }
                else {
                    console.log("Error polling Appverse emulator listener queued messages. Error code: " + xhrL.status);
                }
            };


            AppverseEmulator.appverseCallbackPollOnce = function(callBackFuncName, callbackid) {
                var xhrC = new XMLHttpRequest();
                xhrC.open('POST', "/!appverse_emulator_callback_poll?" + (+new Date()), false);

                try {
                    xhrC.send("callbackFn=" + callBackFuncName + "&callbackid=" + callbackid);
                } catch (e) {
                    console.log("Error polling Appverse emulator queued messages..." + e);
                    xhrC.send("error=Error polling Appverse emulator queued messages");
                }

                var callbackfnPolled = window[callBackFuncName];
                if (!callbackfnPolled) {
                    try {
                        callbackfnPolled = eval('(' + callBackFuncName + ')');
                    } catch (e) {
                        console.log("please define the callback function as a global variable. Error while evaluating function: " + e);
                    }
                }

                if (xhrC.status == 200) {  //This status means ok, otherwise some error code is returned, 404 for example.
                    var responseText = xhrC.responseText;



                    try {
                        if (responseText != null && responseText != '') {
                            console.log("processing callback queued message... ");
                            console.log(responseText);

                            var success = false;
                            var responseObject = null;
                            try {
                                if (responseText != "callback_result_not_yet_avaliable") {
                                    responseObject = eval('(' + responseText + ')');
                                    success = true;
                                    AppverseEmulator.queuedCallbackMessagesCount--;
                                } // otherwise, keep trying
                            } catch (e) {
                                console.log("wrong responseText received from Appverse calls: " + e);
                                success = false;
                                if (callbackfnPolled)
                                    callbackfnPolled(null, callbackid);
                            }

                            try {
                                if (callbackfnPolled && success)
                                    callbackfnPolled(responseObject, callbackid);
                            } catch (e) {
                                console.log("error calling callback function [" + callBackFuncName + "]: " + e);
                                if (callbackfnPolled)
                                    callbackfnPolled(null, callbackid);
                            }

                        } else {
                            console.log("responseText is null for callbackid : " + callbackid);
                            if (callbackfnPolled)
                                callbackfnPolled(null, callbackid);
                        }

                    } catch (e) {
                        console.log("Error parsing Appverse emulator callback queued message..." + e + " - Response received: " + responseText);
                        if (callbackfnPolled)
                            callbackfnPolled(null, callbackid);
                    }
                }
                else {
                    console.log("Error polling Appverse emulator callback queued messages. Error code: " + xhrC.status);
                    if (callbackfnPolled)
                        callbackfnPolled(null, callbackid);
                }
            };

			
			AppverseEmulator.normalizeListenerCallingName  = function(methodName) {
				
				if(methodName=="TakeSnapshot") 				return "GetSnapshot";
				if(methodName=="GetStoredKeyValuePair") 	return "GetStoredKeyValuePairs";
				if(methodName=="StoreKeyValuePair") 		return "StoreKeyValuePairs";
				if(methodName=="RemoveStoredKeyValuePair") 	return "RemoveStoredKeyValuePairs";
				
				return methodName;
			};
			
            // list of events that messages could be enqueued 
            AppverseEmulator.eventListenersRegistered = AppverseEmulator.eventListenersRegistered.concat([
				"ListContacts", "GetContact", "ListCalendarEntries", 
				"GetSnapshot", "TakeSnapshot", 
				"UpdateModule", "UpdateModules", 
				"GetStoredKeyValuePair", "GetStoredKeyValuePairs",
				"StoreKeyValuePair", "StoreKeyValuePairs",
				"RemoveStoredKeyValuePair","RemoveStoredKeyValuePairs"]);
			if(Appverse.Scanner) {  // that module could not be present (as per app configuration)
				AppverseEmulator.eventListenersRegistered.push("DetectQRCode");
			}
            if(Appverse.PushNotifications) {
            	AppverseEmulator.eventListenersRegistered.push("RegisterForRemoteNotifications");
            	AppverseEmulator.eventListenersRegistered.push("UnRegisterForRemoteNotifications");
            }
			if(Appverse.NFC) { // that module could not be present (as per app configuration)
				AppverseEmulator.eventListenersRegistered.push("StartNFCPaymentEngine"); // also success will be queued
            	AppverseEmulator.eventListenersRegistered.push("StartNFCPayment"); // also success, countDowntUpdated, countDowntFinished and failed will be queued
            	AppverseEmulator.eventListenersRegistered.push("CancelNFCPayment");
            }
			if(Appverse.Beacon) {  // that module could not be present (as per app configuration)
                AppverseEmulator.eventListenersRegistered.push("StartMonitoringRegion");
                AppverseEmulator.eventListenersRegistered.push("StartMonitoringAllRegions");
            }
			if(Appverse.is.iOS) {  // only for iOS
				AppverseEmulator.eventListenersRegistered.push("StartLocalAuthenticationWithTouchID");
			}
        }
    }
};

// Checking Appverse compatibility versions
if (typeof(APPVERSE_VERSION) != "undefined") {
    if (UAPPVERSE_VERSION != Appverse.version) {
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
Appverse.getCurrentOrientation = function() {
};

/**
 * Applications should override this method to implement specific rotation/resizing actions for given orientation, width and height.
 * <br> @version 1.0
 * @method
 * @param {String} orientation The device current orientation.
 * @param {int} width The new width to be set.
 * @param {int} height The height width to be set.
 */
Appverse.setOrientationChange = function(orientation, width, height) {
    
};

/**
 * @ignore
 * Updates current device orientation, width and height values.
 * <br> @version 1.0 - changes added on 2.1
 * @method
 */
var updateOrientation = function() {
    
    /*
     * We found this is not needed any more for project in Appverse 5.0 but android tablet.
     */
    Appverse.setOrientationChange((Ext ? Ext.Viewport.getOrientation() : "landscape") , screen.width, screen.height);
    return;
    if (Appverse.is.iPhone) {
        ////// trigger orientationchange in UIWebView for Javascript Frameworks (such as Sencha) 
        var e = document.createEvent('Events');
        e.initEvent('orientationchange', true, false);
        document.dispatchEvent(e);
    } else if (!Appverse.is.iPad) {

        if (Appverse.getCurrentOrientation() == 'portrait') {
            var newWidth = window.innerHeight + 20;
            var newHeight = window.innerWidth - 20;
            Appverse.setOrientationChange('landscape', newWidth, newHeight);
        } else {
            var newWidth = window.innerHeight + 20;
            var newHeight = window.innerWidth - 20;
            Appverse.setOrientationChange('portrait', newWidth, newHeight);
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
    Appverse.setOrientationChange(Appverse.getCurrentOrientation(), window.innerWidth, window.innerHeight);
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
 * @aside guide application_listeners
 * <br> @version 2.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.backgroundApplicationListener = function() {
};

/**
 * Applications should override/implement this method to be aware of application coming back from background, and should perform the desired javascript code on this case.
 * <br> @version 2.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * @aside guide application_listeners
 */
Appverse.foregroundApplicationListener = function() {
};

/**
 * Applications should override/implement this method to be aware of device physical back button has been pressed, and should perform the desired javascript code on this case.
 * @aside guide application_listeners
 * <br> @version 3.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> N/A | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.backButtonListener = function() {
};

/**
 * Applications should override/implement this method to be aware of local notification reception, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Notification.NotificationData NotificationData}.
 * @aside guide application_listeners
 * <br> @version 3.9
 * @method
 * @param {Appverse.Notification.NotificationData} notificationData The notification data received (visual data and custom provider data)
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> N/A | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 * 
 */
Appverse.OnLocalNotificationReceived = function(notificationData) {
};

/**
 * Applications should override/implement this method to be aware of storing of KeyPairs object into the local secure storage, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Security.KeyPair KeyPair}.
 * @aside guide application_listeners
 * <br> @version 4.2
 * @method
 * @param {Appverse.Security.KeyPair[]} storedKeyPairs An array of KeyPair objects successfully stored in the device local secure storage.
 * @param {Appverse.Security.KeyPair[]} failedKeyPairs An array of KeyPair objects that could not be successfully stored in the device local secure storage.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Appverse.OnKeyValuePairsStoreCompleted = function(storedKeyPairs, failedKeyPairs) {
};

/**
 * Applications should override/implement this method to be aware of KeyPair objects found in the device local secure storage, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Security.KeyPair KeyPair}.
 * @aside guide application_listeners
 * <br> @version 4.2
 * @method
 * @param {Appverse.Security.KeyPair[]} foundKeyPairs An array of KeyPair objects found in the device local secure storage.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Appverse.OnKeyValuePairsFound = function(foundKeyPairs) {
};

/**
 * Applications should override/implement this method to be aware of deletion of KeyPairs objects from the local secure storage, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Security.KeyPair KeyPair}.
 * @aside guide application_listeners
 * <br> @version 4.2
 * @method
 * @param {Appverse.Security.KeyPair[]} removedKeyPairs An array of KeyPair objects successfully removed from the device local secure storage.
 * @param {Appverse.Security.KeyPair[]} failedKeyPairs An array of KeyPair objects that could not be removed from the device local secure storage.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Appverse.OnKeyValuePairsRemoveCompleted = function(removedKeyPairs, failedKeyPairs) {
};

/**
 * Applications should override/implement this method to be aware of being lanched by a third-party application, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.System.LaunchData LaunchData}.
 * <br> URI scheme protocols could contain any relative path information before parameter query string; 
 * in this case, that information will be received as a LaunchData object with the Name equals to {@link Appverse.System#LAUNCH_DATA_URI_SCHEME_PATH LAUNCH_DATA_URI_SCHEME_PATH}
 * @aside guide application_listeners
 * <br> @version 4.2
 * @method
 * @param {Appverse.System.LaunchData[]} launchData The launch data received.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 * 
 */
Appverse.OnExternallyLaunched = function(launchData) {
};

/**
 * @ignore
 * Appverse Platform will call this method when application goes to background.
 * <br> @version 2.0
 * @method
 */
Appverse._toBackground = function() {
    //call overrided function to inform application that we are about to put application on background
    if (Appverse.backgroundApplicationListener && typeof Appverse.backgroundApplicationListener == "function" && !Appverse._background) {
        Appverse.backgroundApplicationListener();
    }
    // setting flag after calling backgroundApplicationListener; a appverse service call could be executed in that listener
    Appverse._background = true;
}


/**
 * @ignore
 * Appverse Platform will call this method when application comes from background to foreground.
 * <br> @version 2.0
 * @method
 */
Appverse._toForeground = function() {
    Appverse._background = false;

    //call overrided function to inform application that we are about to put application on foreground
    if (Appverse.foregroundApplicationListener && typeof Appverse.foregroundApplicationListener == "function") {
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
    if (Appverse.backButtonListener && typeof Appverse.backButtonListener == "function") {
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

if (typeof(LOCAL_SERVER_PORT) == "undefined") {
    LOCAL_SERVER_PORT = 8080; // default port
    
    if(_LOCAL_SERVER_PORT = window.localStorage.getItem("_LOCAL_SERVER_PORT")) {
       LOCAL_SERVER_PORT = _LOCAL_SERVER_PORT;
       // getting port from HTML5 storage (if app is reloaded from JS code)
    }
} else {
    localStorage.setItem('_LOCAL_SERVER_PORT', LOCAL_SERVER_PORT);
    // storing listening port to HTML5 storage for furter checking
}

/*
 * NETWORK INTERFACES
 */

/**
 * @class Appverse.Net 
 * Singleton class field to access Net interface. 
 * <br>This interface gives access to device cellular and WIFI connection information.<br>
 * <pre>Usage: Appverse.Net.&lt;metodName&gt;([params]).<br>Example: Appverse.Net.IsNetworkReachable('gft.com').</pre>
 * <br> @version 1.0
 * @singleton
 * @constructor Constructs a new Net interface.
 * @return {Appverse.Net} A new Net interface.
 */
Net = function() {
    /**
     * @cfg {String}
     * Net service name (as configured on Platform Service Locator).
     * <br> @version 1.0
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
	
	/**
     * Network Type for the actual connectivity Status.
     * <br> @version 5.3
	 * <br/>Possible values of network: 
	 * {@link Appverse.Net#NETWORKTYPE_3G NETWORKTYPE_3G}, 
	 * {@link Appverse.Net#NETWORKTYPE_WIFI NETWORKTYPE_WIFI},
	 * & {@link Appverse.Net#NETWORKTYPE_UNKNOWN NETWORKTYPE_UNKNOWN}
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
     * @type int
     */
	this.NetworkStatus = this.NETWORKTYPE_UNKNOWN;
    
    /**
     * @event onConnectivityChange Fired when the Connectivity changes and information is returned to the javascript application.
     * <br>Method to be overrided by JS applications, to handle this event.
     * <br> For further information see, {@link Appverse.Net}.
     * @aside guide application_listeners
     * <br> @version 5.0
     * @param {int} NetworkType a value with the given connectivity status.
	 * <br/>Possible values of network: 
	 * {@link Appverse.Net#NETWORKTYPE_3G NETWORKTYPE_3G}, 
	 * {@link Appverse.Net#NETWORKTYPE_WIFI NETWORKTYPE_WIFI},
	 * & {@link Appverse.Net#NETWORKTYPE_UNKNOWN NETWORKTYPE_UNKNOWN}
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
     */
   
    this.onConnectivityChange =  function(NetworkType){        
    };
}


Appverse.Net = new Net();

/**
 * Detects if network is reachable or not.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True/false if reachable. 
 * @param {String} url The host url to check for reachability.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * <br> <img src="resources/images/warning.png"/> &nbsp; <b>For iOS</b>: only the hostname is allowed as the URL to check reachability. For example: "www.google.com". Otherwise, the method will return always false.
 * <br> <img src="resources/images/warning.png"/> &nbsp; <b>For Android</b>: you could specifiy a more complete URL. For example: "http://www.google.com", "https://www.dropbox.com/", etc. If scheme is not provided, the platform will check first HTTP and then HTTPS to check reachability agaisnt the given URL.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.IsNetworkReachable = function(url, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Net.serviceName, "IsNetworkReachable", get_params([url]), callbackFunctionName, callbackId);
};

/**
 * Gets ASYNC the network information. <br/>For further information see, {@link Appverse.Net.NetworkData NetworkData}.
 * <br> @version 5.0
 * @return {Appverse.Net.NetworkData} NetworkData object. 
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.GetNetworkData = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Net.serviceName, "GetNetworkData", null, callbackFunctionName, callbackId);
};

/**
 * Gets the network types currently supported by this device.
 * <br> @version 5.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.GetNetworkTypeSupported = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Net.serviceName, "GetNetworkTypeSupported", null, callbackFunctionName, callbackId);
};

/**
 * Gets the network types from which this device is able to reach the given url host. Preference ordered list.
 * <br> @version 5.0
 * @param {String} url The host url to check for reachability.
 * <br> <img src="resources/images/warning.png"/> &nbsp; <b>For iOS</b>: only the hostname is allowed as the URL to check reachability. For example: "www.google.com". Otherwise, the method will return always false.
 * <br> <img src="resources/images/warning.png"/> &nbsp; <b>For Android</b>: you could specifiy a more complete URL. For example: "http://www.google.com", "https://www.dropbox.com/", etc. If scheme is not provided, the platform will check first HTTP and then HTTPS to check reachability agaisnt the given URL.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.GetNetworkTypeReachableList = function(url, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Net.serviceName, "GetNetworkTypeReachableList", get_params([url]), callbackFunctionName, callbackId);
};

/**
 * Gets the prefered network type from which this device is able to reach the given url host.
 * <br> @version 5.0
 * @param {String} url The host url to check for reachability.
 * <br> <img src="resources/images/warning.png"/> &nbsp; <b>For iOS</b>: only the hostname is allowed as the URL to check reachability. For example: "www.google.com". Otherwise, the method will return always false.
 * <br> <img src="resources/images/warning.png"/> &nbsp; <b>For Android</b>: you could specifiy a more complete URL. For example: "http://www.google.com", "https://www.dropbox.com/", etc. If scheme is not provided, the platform will check first HTTP and then HTTPS to check reachability agaisnt the given URL.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 */
Net.prototype.GetNetworkTypeReachable = function(url, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Net.serviceName, "GetNetworkTypeReachable", get_params([url]), callbackFunctionName, callbackId);
};

/**
 * Opens the given url in a different Web View with a Navigation Bar.
 * <br/><img src="resources/images/warning.png"/> &nbsp; <b>PDF</b> files could not be displayed on most <b>Android</b> devices (PDF viewer/reader is not included by default). A workaround could be to use the online Google DOCS viewer:<br/>
 * <pre>To see this PDF url 'http://mydomain.com/folder/mypdffile.pdf', 
 * you could use the URL, http://docs.google.com/viewer?url=http%3A%2F%2Fmydomain.com%2Ffolder%2Fmypdffile.pdf</pre>
 * More info at: [https://docs.google.com/viewer?hl=en][1]
 * [1]: https://docs.google.com/viewer?hl=en
 * <br> @version 5.0
 * @param {String} title The title of the Navigation Bar.
 * @param {String} buttonText The Back Button text of the Navigation Bar.
 * @param {String} url The url to be opened.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.OpenBrowser = function(title, buttonText, url, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Net.serviceName, "OpenBrowser", get_params([title, buttonText, url]), callbackFunctionName, callbackId);
};

/**
 * Opens the given url in a different Web View with a Navigation Bar.
 * <br/><img src="resources/images/warning.png"/> &nbsp; <b>PDF</b> files could not be displayed on most <b>Android</b> devices (PDF viewer/reader is not included by default). A workaround could be to use the online Google DOCS viewer:<br/>
 * <pre>To see this PDF url 'http://mydomain.com/folder/mypdffile.pdf', 
 * you could use the URL, http://docs.google.com/viewer?url=http%3A%2F%2Fmydomain.com%2Ffolder%2Fmypdffile.pdf</pre>
 * More info at: [https://docs.google.com/viewer?hl=en][1]
 * [1]: https://docs.google.com/viewer?hl=en
 * <br> @version 5.0
 * @param {Appverse.Net.SecondaryBrowserOptions} secondaryBrowserOptions Object containing options like title, url, close button text and a list of file extensions the browser will handle like the operating system
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.OpenBrowserWithOptions = function(secondaryBrowserOptions, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Net.serviceName, "OpenBrowserWithOptions", get_params([secondaryBrowserOptions]), callbackFunctionName, callbackId);
};

/**
 * Renders the given html in a different Web View with a Navigation Bar.
 * <br> @version 5.0
 * @param {String} title The title of the Navigation Bar.
 * @param {String} buttonText The Back Button text of the Navigation Bar.
 * @param {String} htmls The html string to be rendered.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.ShowHtml = function(title, buttonText, html, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Net.serviceName, "ShowHtml", get_params([title, buttonText, html]), callbackFunctionName, callbackId);
};

/**
 * Opens the given html in a different Web View with a Navigation Bar.
 * <br/><img src="resources/images/warning.png"/> &nbsp; <b>PDF</b> files could not be displayed on most <b>Android</b> devices (PDF viewer/reader is not included by default). A workaround could be to use the online Google DOCS viewer:<br/>
 * <pre>To see this PDF url 'http://mydomain.com/folder/mypdffile.pdf', 
 * you could use the URL, http://docs.google.com/viewer?url=http%3A%2F%2Fmydomain.com%2Ffolder%2Fmypdffile.pdf</pre>
 * More info at: [https://docs.google.com/viewer?hl=en][1]
 * [1]: https://docs.google.com/viewer?hl=en
 * <br> @version 5.0
 * @param {Appverse.Net.SecondaryBrowserOptions} secondaryBrowserOptions Object containing options like title, url, close button text and a list of file extensions the browser will handle like the operating system
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.ShowHtmlWithOptions = function(secondaryBrowserOptions, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Net.serviceName, "ShowHtmlWithOptions", get_params([secondaryBrowserOptions]), callbackFunctionName, callbackId);
};

/**
 * Downloads the given url file by using the default native handler.
 * <br> @version 2.0
 * @param {String} url The url to be opened.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Net.prototype.DownloadFile = function(url, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Net.serviceName, "DownloadFile", get_params([url]), callbackFunctionName, callbackId);
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
     * @cfg {String}
     * System service name (as configured on Platform Service Locator).
     * <br> @version 1.0
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
 * <br> It returns an {int} Number of available displays. 
 * <br> @version 5.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/information.png"/> *harcoded data (always 1) | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetDisplays = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetDisplays", null, callbackFunctionName, callbackId);
};

/*System.prototype.showSoftKeyboard = function(callbackFunctionName, callbackId)
{    
    post_to_url_async(Appverse.System.serviceName, "showSoftKeyboard", null, callbackFunctionName, callbackId);    
};*/

/**
 * Provides information about the display given its index. <br/>For further information see, {@link Appverse.System.DisplayInfo DisplayInfo}.
 * <br> The result value in the callback will be a {Appverse.System.DisplayInfo} object containing the display information, if found. Null value is returned, if given diplay number does not corresponds a valid index.
 * <br> @version 5.0
 * @param {int} displayNumber The display number index. If not provided, primary display information is returned.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/warning.png"/> *data needs to be returned by callback| android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetDisplayInfo = function(displayNumber, callbackFunctionName, callbackId)
{
    if (displayNumber == null) {
        post_to_url_async(Appverse.System.serviceName, "GetDisplayInfo", null, callbackFunctionName, callbackId);
    } else {
        post_to_url_async(Appverse.System.serviceName, "GetDisplayInfo", get_params([displayNumber]), callbackFunctionName, callbackId);
    }
};

/**
 * Provides the current orientation of the given display index, 1 being the primary display.
 * <br> It returns the given display orientation, if found. "Unknown" value is returned, if given diplay number does not corresponds a valid index.
 * <br> @version 5.0
 * <br/>Possible values of display orientation: 
 * {@link Appverse.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, 
 * {@link Appverse.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT},
 * & {@link Appverse.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @param {int} displayNumber The display number index. If not provided, primary display orientation is returned.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOrientation = function(displayNumber, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetOrientation", get_params([displayNumber]), callbackFunctionName, callbackId);
};

/**
 * Provides the current orientation of the primary display - the primary display is 1.
 * <br> It returns and {int} specifying the primary display orientation, if found.
 * <br> @version 5.0
 * <br/>Possible values of display orientation: 
 * {@link Appverse.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, 
 * {@link Appverse.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT},
 * & {@link Appverse.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOrientationCurrent = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetOrientationCurrent", null, callbackFunctionName, callbackId);
};

/**
 * Provides the list of supported orientations for the given display number.
 * <br> It returns an aray of numbers {int[]} specifiyng the list of supported device orientations, for the given display.
 * <br> @version 5.0
 * <br/>Possible values of display orientation: 
 * {@link Appverse.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, 
 * {@link Appverse.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT},
 * & {@link Appverse.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @param {int} displayNumber The display number index. If not provided, primary display supported orientations are returned.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/information.png"/> *returns portrait&landscape | android <img src="resources/images/information.png"/> *returns portrait&landscape | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOrientationSupported = function(displayNumber, callbackFunctionName, callbackId)
{
    if (displayNumber == null) {
        post_to_url_async(Appverse.System.serviceName, "GetOrientationSupported", null, callbackFunctionName, callbackId);
    } else {
        post_to_url_async(Appverse.System.serviceName, "GetOrientationSupported", get_params([displayNumber]), callbackFunctionName, callbackId);
    }
};

/**
 * List of available Locales for the device. <br/>For further information see, {@link Appverse.System.Locale Locale}. 
 * <br> @version 5.0
 * <br> It returns an array of {Appverse.System.Locale[]} containing the list of supported locales.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
System.prototype.GetLocaleSupported = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetLocaleSupported", null, callbackFunctionName, callbackId);
};

/**
 * Gets the current Locale for the device.<br/>For further information see, {@link Appverse.System.Locale Locale}. 
 * <br> @version 5.0
 * <br> It contains the {Appverse.System.Locale} with the current Locale information.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
System.prototype.GetLocaleCurrent = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetLocaleCurrent", null, callbackFunctionName, callbackId);
};

/**
 * Gets the supported input methods.
 * <br> @version 5.0
 * <br/> It returns an array of {int[]} with the list of input methods supported by the device.
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
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetInputMethods = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetInputMethods", null, callbackFunctionName, callbackId);
};

/**
 * Gets the supported input gestures.
 * <br> @version 5.0
 * <br> It returns an array of {int[]} containing the list of input gestures supported by the device.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetInputGestures = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetInputGestures", null, callbackFunctionName, callbackId);
};

/**
 * Gets the supported input buttons.
 * <br> @version 5.0
 * <br> It returns an array of {int[]} containing the list of input buttons supported by the device.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetInputButtons = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetInputButtons", null, callbackFunctionName, callbackId);
};

/**
 * Gets the currently active input method.
 * <br> @version 5.0
 * <br> It contains an {int} with the current input method.
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
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetInputMethodCurrent = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetInputMethodCurrent", null, callbackFunctionName, callbackId);
};

/**
 * Provides memory available for the given use and type.
 * <br> @version 5.0
 * <br> It returns a {long} representing the memory available in bytes.
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
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetMemoryAvailable = function(memUse, memType, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetMemoryAvailable", get_params([memUse, memType]), callbackFunctionName, callbackId);
};

/**
 * Gets the device installed memory types.
 * <br> @version 5.0
 * <br> It returns an array of {int[]} containing the installed storage types.
 * <br/>Possible values of memory types: 
 * {@link Appverse.System#MEMORYTYPE_EXTENDED MEMORYTYPE_EXTENDED}, 
 * {@link Appverse.System#MEMORYTYPE_MAIN MEMORYTYPE_MAIN},
 * & {@link Appverse.System#MEMORYTYPE_UNKNOWN MEMORYTYPE_UNKNOWN} 
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetMemoryAvailableTypes = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetMemoryAvailableTypes", null, callbackFunctionName, callbackId);
};

/**
 * Provides a global map of the memory status for all storage types installed, if 'memType' not provided.
 * Provides a map of the memory status for the given storage type, if 'memType' provided.
 * <br> It returns the {Appverse.System.MemoryStatus} with the requested memory status information.
 * <br/>For further information see, {@link Appverse.System.MemoryStatus MemoryStatus}. 
 * <br> @version 5.0
 * <br/>Possible values of memory types: 
 * {@link Appverse.System#MEMORYTYPE_EXTENDED MEMORYTYPE_EXTENDED}, 
 * {@link Appverse.System#MEMORYTYPE_MAIN MEMORYTYPE_MAIN},
 * & {@link Appverse.System#MEMORYTYPE_UNKNOWN MEMORYTYPE_UNKNOWN} 
 * @param {int} memType The type of memory to check for status. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetMemoryStatus = function(memType, callbackFunctionName, callbackId)
{
    if (memType == null) {
        post_to_url_async(Appverse.System.serviceName, "GetMemoryStatus", null, callbackFunctionName, callbackId);
    } else {
        post_to_url_async(Appverse.System.serviceName, "GetMemoryStatus", get_params([memType]), callbackFunctionName, callbackId);
    }
};

/**
 * Gets the device currently available memory types.
 * <br> It returns an array of {int[]} with the available storafe types.
 * <br> @version 5.0
 * <br/>Possible values of memory types: 
 * {@link Appverse.System#MEMORYTYPE_EXTENDED MEMORYTYPE_EXTENDED}, 
 * {@link Appverse.System#MEMORYTYPE_MAIN MEMORYTYPE_MAIN},
 * & {@link Appverse.System#MEMORYTYPE_UNKNOWN MEMORYTYPE_UNKNOWN} 
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/information.png"/> *harcoded values | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetMemoryTypes = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetMemoryTypes", null, callbackFunctionName, callbackId);
};

/**
 * Gets the device currently available memory uses.
 * <br> @version 5.0
 * It returns an array of {int[]} The available memory uses.
 * <br/>Possible values of memory uses: 
 * {@link Appverse.System#MEMORYUSE_APPLICATION MEMORYUSE_APPLICATION}, 
 * {@link Appverse.System#MEMORYUSE_STORAGE MEMORYUSE_STORAGE},
 * & {@link Appverse.System#MEMORYUSE_OTHER MEMORYUSE_OTHER} 
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/information.png"/> *harcoded values | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.GetMemoryUses = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetMemoryUses", null, callbackFunctionName, callbackId);
};

/**
 * Provides information about the device hardware.<br/>For further information see, {@link Appverse.System.HardwareInfo HardwareInfo}.
 * <br> @version 5.0
 * <br< It returns the {Appverse.System.HardwareInfo} object with the device hardware information (name, version, UUID, etc).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOSHardwareInfo = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetOSHardwareInfo", null, callbackFunctionName, callbackId);
};

/**
 * Provides information about the device operating system.<br/>For further information see, {@link Appverse.System.OSInfo OSInfo}.
 * <br> @version 5.0
 * <br> It returns the {Appverse.System.OSInfo} with the device OS information (name, vendor, version).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOSInfo = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetOSInfo", null, callbackFunctionName, callbackId);
};

/**
 * Provides the current user agent string.
 * <br> @version 5.0
 * >br> It returns the {String} with the user agent string. 
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetOSUserAgent = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetOSUserAgent", null, callbackFunctionName, callbackId);
};

/**
 * Provides information about the device charge.<br/>For further information see, {@link Appverse.System.PowerInfo PowerInfo}.
 * <br> @version 5.0
 * <br> It returns the  {Appverse.System.PowerInfo} with the current charge information.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetPowerInfo = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetPowerInfo", null, callbackFunctionName, callbackId);
};

/**
 * Provides device autonomy time (in milliseconds).
 * <br> @version 5.0
 * <br> It returns the {long} with the remaining power time.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetPowerRemainingTime = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetPowerRemainingTime", null, callbackFunctionName, callbackId);
};

/**
 * Provides information about the device CPU.<br/>For further information see, {@link Appverse.System.CPUInfo CPUInfo}.
 * <br> @version 5.0
 * <br> It returns the {Appverse.System.CPUInfo} with the processor information (name, vendor, speed, UUID, etc).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> *not available on iOS SDK | android <img src="resources/images/error.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetCPUInfo = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetCPUInfo", null, callbackFunctionName, callbackId);
};

/**
 * Provides information about if the current application is allowed to autorotate or not. If locked, 
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a True value if application remains with the same screen orientation (even though user rotates the device).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.IsOrientationLocked = function(callbackFunctionName, callbackId) {
    post_to_url_async(Appverse.System.serviceName, "IsOrientationLocked", null, callbackFunctionName, callbackId);
};

/**
 * Sets wheter the current application could autorotate or not (whether orientation is locked or not)
 * <br> @version 5.0
 * @param {Boolean} Set value to true if application should remain with the same screen orientation (even though user rotates the device)..
 * @param {int} Set the orientation to lock the device to (this value is ignored if "lock" argument is "false"). Possible values of display orientation: {@link Appverse.System#ORIENTATION_LANDSCAPE ORIENTATION_LANDSCAPE}, {@link Appverse.System#ORIENTATION_PORTRAIT ORIENTATION_PORTRAIT} or {@link Appverse.System#ORIENTATION_UNKNOWN ORIENTATION_UNKNOWN}
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.LockOrientation = function(lock, orientation) {
    post_to_url_async(Appverse.System.serviceName, "LockOrientation", get_params([lock, orientation]), null, null);
};

/**
 * Copies a specified text to the native device clipboard.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a True value if the text was successfully copied to the Clipboard, else False.
 * @param {String} textToCopy Text to copy to the Clipboard.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
System.prototype.CopyToClipboard = function(textToCopy, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "CopyToClipboard", get_params([textToCopy]), callbackFunctionName, callbackId);
};

/**
 * Shows default splashcreen (on current orientation). Only the corresponding {@link Appverse.System.DismissSplashScreen} method could dismiss this splash screen.
 * The splash screen could be shown on application start up by default, by properly configure it on the applaction build.properties (build property: app.showsplashscreen.onstartup=true)
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a True value if the splash screen is successfully shown, else False.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.ShowSplashScreen = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "ShowSplashScreen", null, callbackFunctionName, callbackId);
};

/**
 * Dismisses the splashcreen previously shown using {@link Appverse.System.ShowSplashScreen}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a True value if the splash screen is successfully dismissed, else False.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
System.prototype.DismissSplashScreen = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "DismissSplashScreen", null, callbackFunctionName, callbackId);
};

/**
 * Dismisses the current application programmatically.
 * It is up to the HTML app to manage the state and determine when to close the application using this method.
 * <br> <b>This feature is not supported on iOS platform (interface is available, but with no effect)</b>
 * <br> @version 5.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> *N/A* | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.DismissApplication = function()
{
    post_to_url_async(Appverse.System.serviceName, "DismissApplication", null, null, null);
};

/**
 * Returns all applications configured to be launched (using Appverse.System.LaunchApplication method) at configuration file: app/config/launch-data.xml.
 * <br> @version 5.0
 * <br> It retuns an array of {Appverse.System.App[]} containing the Applications to be launched.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetApplications = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetApplications", null, callbackFunctionName, callbackId);
};

/**
 * Returns an application configured to be launched (using Appverse.System.LaunchApplication method) at configuration file: app/config/launch-data.xml, given its name.
 * <br> @version 5.0
 * <br> It returns a {Appverse.System.App} object representing the Application configured to be launched that match the given name.
 * @method
 * @param {String} appName The application name to match.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.GetApplication = function(appName, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.System.serviceName, "GetApplication", get_params([appName]), callbackFunctionName, callbackId);
};

/**
 * Returns all applications configured to be launched (using Appverse.System.LaunchApplication method) at configuration file: app/config/launch-data.xml.
 * <br> @version 5.0
 * @method
 * @param {Appverse.System.App/String} app The application object (or its name) to be launched.
 * @param {String} query The query string (parameters) in the format: "relative_url?param1=value1&param2=value2". Set it to null for not sending extra launch data.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
System.prototype.LaunchApplication = function(app, query)
{
    post_to_url_async(Appverse.System.serviceName, "LaunchApplication", get_params([app, query]), null, null);
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
     * @cfg {String}
     * Database service name (as configured on Platform Service Locator).
     * <br> @version 1.0
     */
    this.serviceName = "db";
}

Appverse.Database = new Database();

/**
 * Gets stored databases.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 5.0
 * <br> It returns an array of {Appverse.Database.Database[]} with a list of application Databases.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.GetDatabaseList = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "GetDatabaseList", null, callbackFunctionName, callbackId);
};

/**
 * Creates database on default path.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.Database.Database} object with the created database reference object.
 * @param {String} dbName The database file name (please include .db extension).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.CreateDatabase = function(dbName, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "CreateDatabase", get_params([dbName]), callbackFunctionName, callbackId);
};

/**
 * Gets database reference object by given name.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br>Databases are located on the default database path: /<PersonalFolder>/sqlite/
 * <br> @version 5.0
 * <br> It returns a {Appverse.Database.Database} object with the created database reference object.
 * @param {String} dbName The database file name (including .db extension).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.GetDatabase = function(dbName, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "GetDatabase", get_params([dbName]), callbackFunctionName, callbackId);
};

/**
 * Creates a table inside the given database.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a True value on successful table creation. False, otherwise.
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase} ).
 * @param {String} tableName The table name to be inserted.
 * @param {String[]} columnsDefs The column definitions array (SQLITE syntax).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.CreateTable = function(db, tableName, columnsDefs, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "CreateTable", get_params([db, tableName, columnsDefs]), callbackFunctionName, callbackId);
};

/**
 * Deletes database on default path.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a True value on successful database deletion.
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}) to be deleted.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.DeleteDatabase = function(db, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "DeleteDatabase", get_params([db]), callbackFunctionName, callbackId);
};

/**
 * Deletes table from the given database.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True on successful table deletion.
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase} ).
 * @param {String} tableName The table name to be deleted.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.DeleteTable = function(db, tableName, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "DeleteTable", get_params([db, tableName]), callbackFunctionName, callbackId);
};

/**
 * Gets table names from the given database.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 5.0
 * <br> It returns an array of {String[]} with a list of table names.
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}) to check for table names.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.GetTableNames = function(db, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "GetTableNames", get_params([db]), callbackFunctionName, callbackId);
};

/**
 * Checks if database exists by database bean reference, if 'tableName' is not provided.
 * Checks if database table exists by database bean reference and table name, if 'tableName' is provided.
 * <br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} value of True if database or database table exists.
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} tableName The table name  to check for existence. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.Exists = function(db, tableName, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "Exists", get_params([db, tableName]), callbackFunctionName, callbackId);
};

/**
 * Checks if database exists by given database name (including .db extension).<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 1.0
 * It returns a {Boolean} value of True if database exists.
 * @param {String} dbName The database name to check for existence.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.ExistsDatabase = function(dbName, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "ExistsDatabase", get_params([dbName]), callbackFunctionName, callbackId);
};

/**
 * Executes SQL query against given database.<br/>For further information see, {@link Appverse.Database.Database Database} and {@link Appverse.Database.ResultSet ResultSet}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.Database.ResultSet} object with the result set (with zero rows count parameter if no rows satisfy query conditions).
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} query The SQL query to execute. 
 * @param {String[]} replacements The replacement arguments for a preformatted SQL query. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.ExecuteSQLQuery = function(db, query, replacements, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "ExecuteSQLQuery", get_params([db, query, replacements]), callbackFunctionName, callbackId);
};

/**
 * Executes SQL statement into the given database.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} value of True on successful statement execution.
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String} statement The SQL statement to execute. 
 * @param {String[]} replacements The replacement arguments for a preformatted SQL statement. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.ExecuteSQLStatement = function(db, statement, replacements, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "ExecuteSQLStatement", get_params([db, statement, replacements]), callbackFunctionName, callbackId);
};

/**
 * Executes SQL transaction (some statements chain) inside given database.<br/>For further information see, {@link Appverse.Database.Database Database}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True on successful transaction execution.
 * @param {Appverse.Database.Database} db The database object reference (as provided by {@link #GetDatabase}).
 * @param {String[]} statements The statements to be executed during transaction (sqlite syntax language).. 
 * @param {Boolean} rollbackFlag Indicates if rollback should be performed when any statement execution fails.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Database.prototype.ExecuteSQLTransaction = function(db, statements, rollbackFlag, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Database.serviceName, "ExecuteSQLTransaction", get_params([db, statements, rollbackFlag]), callbackFunctionName, callbackId);
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
     * @cfg {String}
     * FileSystem service name (as configured on Platform Service Locator).
     * <br> @version 1.0
     */
    this.serviceName = "file";
}

Appverse.FileSystem = new FileSystem();

/**
 * Get configured root directory.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.FileSystem.DirectoryData} object with the configured root directory information.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.GetDirectoryRoot = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "GetDirectoryRoot", null, callbackFunctionName, callbackId);
};

/**
 * Creates a directory under the given base directory, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.FileSystem.DirectoryData} object with the directory created, or null if folder cannot be created.
 * @param {String} directoryName The directory name to be created. 
 * @param {Appverse.FileSystem.DirectoryData} baseDirectory The base Directory to create directory under it. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.CreateDirectory = function(directoryName, baseDirectory, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "CreateDirectory", get_params([directoryName, baseDirectory]), callbackFunctionName, callbackId);
};

/**
 * Creates a file under the given base directory, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData} and {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.FileSystem.FileData} object with the file created, or null if folder cannot be created.
 * @param {String} fileName The file name to be created. 
 * @param {Appverse.FileSystem.DirectoryData} baseDirectory The base Directory to create file under it. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.CreateFile = function(fileName, baseDirectory, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "CreateFile", get_params([fileName, baseDirectory]), callbackFunctionName, callbackId);
};

/**
 * List all directories under the given base directory data, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}.
 * <br> @version 5.0
 * <br> It returns an array of {Appverse.FileSystem.DirectoryData[]} objects containing the directories information array.
 * @param {Appverse.FileSystem.DirectoryData} dirData The base Directory to check for directories under it. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.ListDirectories = function(dirData, callbackFunctionName, callbackId)
{
    if (dirData == null) {
        post_to_url_async(Appverse.FileSystem.serviceName, "ListDirectories", null, callbackFunctionName, callbackId);
    } else {
        post_to_url_async(Appverse.FileSystem.serviceName, "ListDirectories", get_params([dirData]), callbackFunctionName, callbackId);
    }
};

/**
 * List all files under the given base directory data, or under root directory if it is not provided.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData} and {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 5.0
 * <br> It returns an array of {Appverse.FileSystem.FileData[]} objects with the files information array.
 * @param {Appverse.FileSystem.DirectoryData} dirData The base Directory to check for files under it. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.ListFiles = function(dirData, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "ListFiles", get_params([dirData]), callbackFunctionName, callbackId);
};

/**
 * Checks if the given directory exists.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if directory exists.
 * @param {Appverse.FileSystem.DirectoryData} dirData The directory to check for existence.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 */
FileSystem.prototype.ExistsDirectory = function(dirData, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "ExistsDirectory", get_params([dirData]), callbackFunctionName, callbackId);
};

/**
 * Deletes the given directory.<br/>For further information see, {@link Appverse.FileSystem.DirectoryData DirectoryData}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True on successful directory deletion.
 * @param {Appverse.FileSystem.DirectoryData} dirData The directory to be deleted.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.DeleteDirectory = function(dirData, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "DeleteDirectory", get_params([dirData]), callbackFunctionName, callbackId);
};

/**
 * Deletes the given file.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True on successful file deletion.
 * @param {Appverse.FileSystem.FileData} fileData The file to be deleted.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.DeleteFile = function(fileData, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "DeleteFile", get_params([fileData]), callbackFunctionName, callbackId);
};

/**
 * Checks if the given file exists.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} value of True if file exists.
 * @param {Appverse.FileSystem.FileData} fileData The file data to check for existence.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 */
FileSystem.prototype.ExistsFile = function(fileData, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "ExistsFile", get_params([fileData]), callbackFunctionName, callbackId);
};

/**
 * Reads file on given path.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 5.0
 * <br> It returns an array of {byte[]} objects with the readed bytes.
 * @param {Appverse.FileSystem.FileData} fileData The file data to read.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.ReadFile = function(fileData, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "ReadFile", get_params([fileData]), callbackFunctionName, callbackId);
};

/**
 * Writes contents to file on given path.<br/>For further information see, {@link Appverse.FileSystem.FileData FileData}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if file could be written.
 * @param {Appverse.FileSystem.FileData} fileData The file to add/append contents to.
 * @param {byte[]} contents The data to be written to file.
 * @param {Boolean} appendFlag True if data should be appended to previous file data.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.WriteFile = function(fileData, contents, appendFlag, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "WriteFile", get_params([fileData, contents, appendFlag]), callbackFunctionName, callbackId);
};

/**
 * Copies the given file on "fromPath" to the "toPath". 
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if file could be copied.
 * @param {String} sourceFileName The file name (relative path under "resources" application directory) to be copied from. 
 * @param {String} destFileName The file name (relative path under "documents" application directory) to be copied to.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/warning.png"/> *"resources" path pending to be defined for this platform | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.CopyFromResources = function(sourceFileName, destFileName, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "CopyFromResources", get_params([sourceFileName, destFileName]), callbackFunctionName, callbackId);
};

/**
 * Copies the given remote file from "url" to the "toPath" (local relative path). 
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if file could be copied.
 * @param {String} url The remote url file to be copied from. 
 * @param {String} destFileName The file name (relative path under "documents" application directory) to be copied to.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
FileSystem.prototype.CopyFromRemote = function(url, destFileName, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.FileSystem.serviceName, "CopyFromRemote", get_params([url, destFileName]), callbackFunctionName, callbackId);
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
     * @cfg {String}
     * Notification service name (as configured on Platform Service Locator).
     * <br> @version 1.0
     */
    this.serviceName = "notify";
	
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
}

Appverse.Notification = new Notification();

/**
 * Shows and starts the activity indicator animation.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if activity indicator could be started.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StartNotifyActivity = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StartNotifyActivity", null, callbackFunctionName, callbackId);
};

/**
 * Stops and hides the activity indicator animation.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if activity indicator could be stopped.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StopNotifyActivity = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StopNotifyActivity", null, callbackFunctionName, callbackId);
};

/**
 * Checks if activity indicator animation is started.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True/false wheter activity indicator is running.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.IsNotifyActivityRunning = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "IsNotifyActivityRunning", null, callbackFunctionName, callbackId);
};

/**
 * Starts an alert notification.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if alert notification could be started.
 * @param {String} message The alert message to be displayed.
 * @param {String} title The alert title to be displayed.
 * @param {String} buttonText The accept button text to be displayed.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StartNotifyAlert = function(message, title, buttonText, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StartNotifyAlert", get_params([title, message, buttonText]), callbackFunctionName, callbackId);
};

/**
 * Stops an alert notification.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if alert notification could be stopped.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StopNotifyAlert = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StopNotifyAlert", null, callbackFunctionName, callbackId);
};

/**
 * Shows an action sheet.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if action sheet could be showed.
 * @param {String} title The title to be displayed on the action sheet.
 * @param {String[]} buttons Array of button texts to be displayed. First index button is the "cancel" button, default button.
 * @param {String[]} jsCallbackFunctions The callback javascript functions as string texts for each of the given buttons. Empty string if no action is required for a button.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Notification.prototype.StartNotifyActionSheet = function(title, buttons, jsCallbackFunctions, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StartNotifyActionSheet", get_params([title, buttons, jsCallbackFunctions]), callbackFunctionName, callbackId);
};

/**
 * Starts a beep notification.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if beep notification could be started.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StartNotifyBeep = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StartNotifyBeep", null, callbackFunctionName, callbackId);
};

/**
 * Stops the current beep notification.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if beep notification could be stopped.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StopNotifyBeep = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StopNotifyBeep", null, callbackFunctionName, callbackId);
};

/**
 * Starts a blink notification.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if beep notification could be started.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Notification.prototype.StartNotifyBlink = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StartNotifyBlink", null, callbackFunctionName, callbackId);
};

/**
 * Stops the current blink notification.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if blink notification could be stopped.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Notification.prototype.StopNotifyBlink = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StopNotifyBlink", null, callbackFunctionName, callbackId);
};

/**
 * Shows and starts the loading indicator animation (native loading mask).
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if loading indicator animation could be started.
 * @param {String} loadingText The loading text to be dispayed.
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
Notification.prototype.StartNotifyLoading = function(loadingText, callbackFunctionName, callbackId)
{
    if (loadingText == null) {
        post_to_url_async(Appverse.Notification.serviceName, "StartNotifyLoading", null, callbackFunctionName, callbackId);
    } else {
        post_to_url_async(Appverse.Notification.serviceName, "StartNotifyLoading", get_params([loadingText]), callbackFunctionName, callbackId);
    }
};

/**
 * Stops the current progress indicator animation.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if progress indicator animation could be stopped.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.StopNotifyLoading = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StopNotifyLoading", null, callbackFunctionName, callbackId);
};

/**
 * Checks if progress indicator animation is started.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True/false wheter progress indicator is running.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.IsNotifyLoadingRunning = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "IsNotifyLoadingRunning", null, callbackFunctionName, callbackId);
};

/**
 * Updates the progress indicator animation.
 * <br> @version 5.0
 * @param {float} progress The current progress; values between 0.0 and 1.0 (completed).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Notification.prototype.UpdateNotifyLoading = function(progress, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "UpdateNotifyLoading", get_params([progress]), callbackFunctionName, callbackId);
};

/**
 * Starts a vibration notification.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if vibration notification could be started.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Notification.prototype.StartNotifyVibrate = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StartNotifyVibrate", null, callbackFunctionName, callbackId);
};

/**
 * Stops the current vibration notification.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if vibration notification could be stopped.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Notification.prototype.StopNotifyVibrate = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Notification.serviceName, "StopNotifyVibrate", null, callbackFunctionName, callbackId);
};

/**
 * Sets the current application icon badge number (the one inside the red bubble).
 * <br> @version 5.0
 * @param {int} badge The badge number to set.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> N/A  | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.SetApplicationIconBadgeNumber = function(badge)
{
    if (Appverse.is.iOS)
        post_to_url_async(Appverse.Notification.serviceName, "SetApplicationIconBadgeNumber", get_params([badge]), null, null);
};

/**
 * Increments (adds one to) the current application icon badge number (the one inside the red bubble).
 * <br> @version 5.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> N/A  | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.IncrementApplicationIconBadgeNumber = function()
{
    if (Appverse.is.iOS)
        post_to_url_async(Appverse.Notification.serviceName, "IncrementApplicationIconBadgeNumber", null, null, null);
};

/**
 * Decrements (substracts one from) the current application icon badge number (the one inside the red bubble).
 * <br> @version 5.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> N/A  | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.DecrementApplicationIconBadgeNumber = function()
{
    if (Appverse.is.iOS)
        post_to_url_async(Appverse.Notification.serviceName, "DecrementApplicationIconBadgeNumber", null, null, null);
};

/**
 * Presents a local notification immediately for the current application.
 * <br> @version 5.0
 * @method
 * @param {Appverse.Notification.NotificationData} notification The notification data to be presented. For further information see, {@link Appverse.Notification.NotificationData NotificationData}.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.PresentLocalNotificationNow = function(notification)
{
    post_to_url_async(Appverse.Notification.serviceName, "PresentLocalNotificationNow", get_params([notification]), null, null);
};

/**
 * chedules a local notification fo delivery on a scheduled date and time.
 * <br> @version 5.0
 * @method
 * @param {Appverse.Notification.NotificationData} notification The notification data to be presented. For further information see, {@link Appverse.Notification.NotificationData NotificationData}.
 * @param {SchedulingData} schedule The scheduling data with the fire date. For further information see, {@link Appverse.Notification.SchedulingData SchedulingData}.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.ScheduleLocalNotification = function(notification, schedule)
{
    post_to_url_async(Appverse.Notification.serviceName, "ScheduleLocalNotification", get_params([notification, schedule]), null, null);
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
    post_to_url_async(Appverse.Notification.serviceName, "CancelLocalNotification", get_params([fireDate]), null, null);
};

/**
 * Cancels all local notifications already scheduled.
 * <br> @version 3.9
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Notification.prototype.CancelAllLocalNotifications = function()
{
    post_to_url_async(Appverse.Notification.serviceName, "CancelAllLocalNotifications", null, null, null);
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
     * @cfg {String}
     * IO service name (as configured on Platform Service Locator).
     * <br> @version 1.0
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
 * <br> @version 5.0
 * <br> It returns an array of {Appverse.IO.IOService[]} objects containing the list of external services.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
IO.prototype.GetServices = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.IO.serviceName, "GetServices", null, callbackFunctionName, callbackId);
};

/**
 * Gets the I/O Service that matches the given name, and type (if provided). It is possible to define two services with the same name, but different type.
 * <br/>For further information see, {@link Appverse.IO.IOService IOService}.
 * <br> @version 5.0
 * <br> It retursn a {Appverse.IO.IOService} object with the external service matched.
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
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
IO.prototype.GetService = function(serviceName, serviceType, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.IO.serviceName, "GetService", get_params([serviceName, serviceType]), callbackFunctionName, callbackId);
};

/**
 * Invokes the I/O Service that matches the given service name (or service object reference), and type (if provided).
 * <br/>For further information see, {@link Appverse.IO.IOService IOService}, {@link Appverse.IO.IORequest IORequest} and {@link Appverse.IO.IOResponse IOResponse}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.IO.IOResponse} object with the response object returned from remote service. Access content doing: <pre>ioResponse.Content</pre>
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
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
IO.prototype.InvokeService = function(requestObjt, service, serviceType, callbackFunctionName, callbackId)
{
  if(serviceType==null)  post_to_url_async(Appverse.IO.serviceName, "InvokeService", get_params([requestObjt, service]), callbackFunctionName, callbackId);
  else  post_to_url_async(Appverse.IO.serviceName, "InvokeService", get_params([requestObjt, service, serviceType]), callbackFunctionName, callbackId);
};

/**
 * Invokes the I/O Service (that matches the given service object reference) for retreiving a file (specially big ones) and stores it locally (under given store path)
 * Only {@link Appverse.IO#SERVICETYPE_OCTET_BINARY SERVICETYPE_OCTET_BINARY} types are allowed in this method.
 * <br/>For further information see, {@link Appverse.IO.IOService IOService} and {@link Appverse.IO.IORequest IORequest}.
 * <br> @version 5.0
 * <br> It returns a {String} with the reference url for the stored file, or null on error case. If store file is a temporal file, application should remove it when no more needed.
 * @param {Appverse.IO.IORequest} requestObjt The request object with the needed invocation parameters. Example:<pre>{"Session":null,"Content":"{method:authenticationService.login,id:1,params:['username','password']}"}</pre>
 * @param {Appverse.IO.IOService} service This param could be a IOService object (as provided by {@link #GetService}), or only the service name. First service match would be invoked. ATTENTION: when using the 'object', the third argument (type) shouldn't be informed.
 * @param {String} storePath The relative path (under application documents root direectory) to store the contents received from this service invocation.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
IO.prototype.InvokeServiceForBinary = function(requestObjt, service, storePath, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.IO.serviceName, "InvokeServiceForBinary", get_params([requestObjt, service, storePath]), callbackFunctionName, callbackId);
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
     * @cfg {String}
     * Geo service name (as configured on Platform Service Locator).
     * <br> @version 1.0
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
    this.NORTHTYPE_TRUE = 1;
}

Appverse.Geo = new Geo();

/**
 * Gets the current device acceleration (measured in meters/second/second). <br/>For further information see, {@link Appverse.Geo.Acceleration Acceleration}.
 * <br> @version 5.0
 * <br> It returns an {Appverse.Geo.Acceleration} object with the current acceleration info (coordinates and acceleration vector number).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetAcceleration = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "GetAcceleration", null, callbackFunctionName, callbackId);
};

/**
 * Gets the current device location coordinates. <br/>For further information see, {@link Appverse.Geo.LocationCoordinate LocationCoordinate}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.Geo.LocationCoordinate} object with the current location info (coordinates and precision).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetCoordinates = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "GetCoordinates", null, callbackFunctionName, callbackId);
};

/**
 * Gets the heading relative to the given north type (if 'northType' is not provided, default is used: magnetic noth pole).
 * <br> @version 5.0
 * <br> It returns a {float} with the current heading. Measured in degrees, minutes and seconds.
 * <br/>Possible values of north type: 
 * {@link Appverse.Geo#NORTHTYPE_MAGNETIC NORTHTYPE_MAGNETIC}, 
 * & {@link Appverse.Geo#NORTHTYPE_TRUE NORTHTYPE_TRUE}
 * @param {int} northType Type of north to measured heading relative to it. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetHeading = function(northType, callbackFunctionName, callbackId)
{
    if (northType == null) {
        post_to_url_async(Appverse.Geo.serviceName, "GetHeading", null, callbackFunctionName, callbackId);
    } else {
        post_to_url_async(Appverse.Geo.serviceName, "GetHeading", get_params([northType]), callbackFunctionName, callbackId);
    }
};

/**
 * Gets the orientation relative to the magnetic north pole.
 * <br> @version 5.0
 * <br> It returns a {float} with the current orientation. Measured in degrees, minutes and seconds.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetDeviceOrientation = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "GetDeviceOrientation", null, callbackFunctionName, callbackId);
};

/**
 * Gets the current device velocity.
 * <br> @version 5.0
 * <br> It returns a {float} with the device speed (in meters/second).
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetVelocity = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "GetVelocity", null, callbackFunctionName, callbackId);
};

/**
 * Shows Map on screen.
 * <br> @version 5.0
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Geo.prototype.GetMap = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "GetMap", null, callbackFunctionName, callbackId);
};

/**
 * Specifies current map scale and bounding box radius.
 * <br> @version 5.0
 * @param {float} scale The desired map scale.
 * @param {float} boundingBox The desired map view bounding box.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
Geo.prototype.SetMapSettings = function(scale, boundingBox, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "SetMapSettings", get_params([scale, boundingBox]), callbackFunctionName, callbackId);
};

/**
 * List of POIs for the current location, given a radius (bounding box). Optionaly, a query text and/or a category could be added to search for specific conditions.
 * <br/>For further information see, {@link Appverse.Geo.POI POI}.
 * <br> @version 5.0
 * <br> It returns an array of {Appverse.Geo.POI[]} points of Interest for location, ordered by distance.
 * @param {Appverse.Geo.LocationCoordinate} location Map location point to search nearest POIs.
 * @param {float} radius The radius around location to search POIs in.
 * @param {String} queryText The query to search POIs.. Optional parameter.
 * @param {Appverse.Geo.LocationCategory} category The query to search POIs.. Optional parameter.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Geo.prototype.GetPOIList = function(location, radius, queryText, category, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "GetPOIList", get_params([location, radius, queryText, category]), callbackFunctionName, callbackId);
};

/**
 * Gets a POI by the given id. <br/>For further information see, {@link Appverse.Geo.POI POI}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.Geo.POI} object representing the Point of Interest found.
 * @param {String} poiId POI identifier.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Geo.prototype.GetPOI = function(poiId, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "GetPOI", get_params([poiId]), callbackFunctionName, callbackId);
};

/**
 * Removes a POI given its id. <br/>For further information see, {@link Appverse.Geo.POI POI}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if the point of interest has been successfully removed.
 * @param {String} poiId POI identifier.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Geo.prototype.RemovePOI = function(poiId, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "RemovePOI", get_params([poiId]), callbackFunctionName, callbackId);
};

/**
 * Moves a POI - given its id - to target location. <br/>For further information see, {@link Appverse.Geo.POI POI}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if the point of interest has been successfully updated.
 * @param {String} poiId POI identifier.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Geo.prototype.UpdatePOI = function(poi, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "UpdatePOI", get_params([poi]), callbackFunctionName, callbackId);
};

/**
 * Starts the location services in order to get the latitude, longitude, altitude, speed, etc.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if the device can start the location services
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StartUpdatingLocation = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "StartUpdatingLocation", null, callbackFunctionName, callbackId);
};

/**
 * Stops the location services in order to get the latitude, longitude, altitude, speed, etc.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if the device can stop the location services
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StopUpdatingLocation = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "StopUpdatingLocation", null, callbackFunctionName, callbackId);
};

/**
 * Starts the heading services in order to get the latitude, longitude, altitude, speed, etc.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if the device can start the location services
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StartUpdatingHeading = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "StartUpdatingHeading", null, callbackFunctionName, callbackId);
};

/**
 * Stops the heading services in order to get the latitude, longitude, altitude, speed, etc.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if the device can stop the location services
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StopUpdatingHeading = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "StopUpdatingHeading", null, callbackFunctionName, callbackId);
};

/**
 * Performs a reverse geocoding in order to get, from the present latitude and longitude,
 * attributes like "County", "Street", "County code", "Location", ... in case such attributes
 * are available for that location.
 * <br/>For further information see, {@link Appverse.Geo.GeoDecoderAttributes GeoDecoderAttributes}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.Geo.GeoDecoderAttributes} object with the reverse geocoding attributes from the present location (latitude and longitude)
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.GetGeoDecoder = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "GetGeoDecoder", null, callbackFunctionName, callbackId);
};

/**
 * The proximity sensor detects an object close to the device.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if the proximity sensor detects an object close to the device
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StartProximitySensor = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "StartProximitySensor", null, callbackFunctionName, callbackId);
};

/**
 * Stops the proximity sensor service.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if the proximity sensor service could be stopped.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.StopProximitySensor = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "StopProximitySensor", null, callbackFunctionName, callbackId);
};

/**
 * Determines whether the Location Services (GPS) is enabled.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if the device can start the location services
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Geo.prototype.IsGPSEnabled = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Geo.serviceName, "IsGPSEnabled", null, callbackFunctionName, callbackId);
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
     * @cfg {String}
     * Media service name (as configured on Platform Service Locator).
     * <br> @version 1.0
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
     * @event onFinishedPickingImage Fired when an image have been picked, either from the Photos library (after calling the {@link Appverse.Media.GetSnapshot GetSnapshot}), 
     * or from the Camera (after calling the {@link Appverse.Media.TakeSnapshot TakeSnapshot})
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 3.1
     * @param {Appverse.Media.MediaMetadata} mediaMetadata The metadata for the image picked.
     */
    this.onFinishedPickingImage = function(mediaMetadata) {
    };

}

Appverse.Media = new Media();

/**
 * Gets Media metadata.<br/>For further information see, {@link Appverse.Media.MediaMetadata MediaMetadata}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.Media.MediaMetadata} object with the media file metadata.
 * @param {String} filePath The media file path.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.GetMetadata = function(filePath, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Media.serviceName, "GetMetadata", get_params([filePath]), callbackFunctionName, callbackId);
};

/**
 * Starts playing media.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if media file could be started.
 * @param {String} filePath The media file path.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.Play = function(filePath, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Media.serviceName, "Play", get_params([filePath]), callbackFunctionName, callbackId);
};

/**
 * Starts playing media.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if media file could be started.
 * @param {String} url The media remote URL.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/warning.png"/> * bug fixing | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.PlayStream = function(url, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Media.serviceName, "PlayStream", get_params([url]), callbackFunctionName, callbackId);
};

/**
 * Moves player to the given position in the media.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if player position could be moved.
 * @param {long} position Index position.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.SeekPosition = function(position, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Media.serviceName, "SeekPosition", get_params([position]), callbackFunctionName, callbackId);
};

/**
 * Stops the current media playing.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if media file could be stopped.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.Stop = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Media.serviceName, "Stop", null, callbackFunctionName, callbackId);
};

/**
 * Pauses the current media playing.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if media file could be stopped.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.Pause = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Media.serviceName, "Pause", null, callbackFunctionName, callbackId);
};

/**
 * Gets Audio/Movie player state.
 * <br> @version 5.0
 * >br> It returns an {int} with the current player state.
 * <br/>Possible values of media states: 
 * {@link Appverse.Media#MEDIATSTATE_ERROR MEDIATSTATE_ERROR}, 
 * {@link Appverse.Media#MEDIATSTATE_PAUSED MEDIATSTATE_PAUSED}, 
 * {@link Appverse.Media#MEDIATSTATE_PLAYING MEDIATSTATE_PLAYING}, 
 * {@link Appverse.Media#MEDIATSTATE_RECORDING MEDIATSTATE_RECORDING}, 
 * & {@link Appverse.Media#MEDIATSTATE_STOPPED MEDIATSTATE_STOPPED}
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.GetState = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Media.serviceName, "GetState", null, callbackFunctionName, callbackId);
};

/**
 * Gets the currently playing media file metadata.<br/>For further information see, {@link Appverse.Media.MediaMetadata MediaMetadata}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.Media.MediaMetadata} object with the Current media file metadata.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/information.png"/> *mock data | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Media.prototype.GetCurrentMedia = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Media.serviceName, "GetCurrentMedia", null, callbackFunctionName, callbackId);
};

/**
 * Opens user interface view to select a picture from the device photos album.<br/>For further information see, {@link Appverse.Media.MediaMetadata MediaMetadata}.
 * Data is provided via the proper event handled by the "Appverse.Media.onFinishedPickingImage" method; please, override to handle the event.
 * <br> @version 5.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/warning.png"/> * in progess | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/warning.png"/> *in progress</pre>
 */
Media.prototype.GetSnapshot = function()
{
    post_to_url_async(Appverse.Media.serviceName, "GetSnapshot", null, null, null);
};

/**
 * Opens user interface view to take a picture using the device camera.<br/>For further information see, {@link Appverse.Media.MediaMetadata MediaMetadata}.
 * Data is provided via the proper event handled by the "Appverse.Media.onFinishedPickingImage" method; please, override to handle the event.
 * <br> @version 5.0
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/warning.png"/> * in progess | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/warning.png"/> *in progress</pre>
 */
Media.prototype.TakeSnapshot = function()
{
    post_to_url_async(Appverse.Media.serviceName, "TakeSnapshot", null, null, null);
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
Messaging = function() {
    /**
     * @cfg {String}
     * Messaging service name (as configured on Platform Service Locator).
     * <br> @version 1.0
     */
    this.serviceName = "message";
}

Appverse.Messaging = new Messaging();

/**
 * Sends a text message (SMS).
 * <br> @version 1.0. Modified version 4.5: this method is now asynchronous, but it has no callback function.
 * @param {String} phoneNumber The phone address to send the message to (also, multiple addresses separated by comma or semicolon).
 * @param {String} text The message body.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data </pre>
 */
Messaging.prototype.SendMessageSMS = function(phoneNumber, text, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Messaging.serviceName, "SendMessageSMS", get_params([phoneNumber, text]), callbackFunctionName, callbackId);
};

/**
 * Sends a multimedia message (MMS).
 * <br> @version 1.0. Modified version 4.5: this method is now asynchronous, but it has no callback function.
 * @param {String} phoneNumber The phone address to send the message to (also, multiple addresses separated by comma or semicolon). 
 * @param {String} text The message body.
 * @param {Appverse.Messaging.AttachmentData} attachment Attachament data.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data </pre>
 */
Messaging.prototype.SendMessageMMS = function(phoneNumber, text, attachment)
{
    post_to_url_async(Appverse.Messaging.serviceName, "SendMessageMMS", get_params([phoneNumber, text, attachment]), null, null);
};

/**
 * Sends an email message.<br/>For further information see, {@link Appverse.Messaging.EmailData EmailData}.
 * <br> @version 1.0. Modified version 4.5: this method is now asynchronous, but it has no callback function.
 * @param {Appverse.Messaging.EmailData} emailData The email message data, such as: subject, 'To','Cc','Bcc' addresses, etc.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data </pre>
 */
Messaging.prototype.SendEmail = function(emailData)
{
    post_to_url_async(Appverse.Messaging.serviceName, "SendEmail", get_params([emailData]), null, null);
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
     * @cfg {String}
     * Pim service name (as configured on Platform Service Locator).
     * <br> @version 1.0
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
     * <br> @version 1.0
     * @type int
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
     * Defines the column used to search contacts by the "phone" column 
     * <br> ATTENTION: at this moment, only the {@link Appverse.Pim#CONTACTS_QUERY_CONDITION_AVAILABLE CONTACTS_QUERY_CONDITION_AVAILABLE} condition could be used in conjunction with this CONTACTS_QUERY_COLUMN_PHONE column in the same ContactQuery.
     * <br> This means that you could query the contacts that have at least 1 phone available (contacts without phone informed are not returned) by using this pair (column==Phone AND condition==Available)
     * <br> @version 4.5
     * @type int
     */
    this.CONTACTS_QUERY_COLUMN_PHONE = 2;

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
     * Defines the condition used to match contacts as an "is available" condition.
     * <br> At this moment, used ONLY with the column specified by the {@link Appverse.Pim#CONTACTS_QUERY_COLUMN_PHONE CONTACTS_QUERY_COLUMN_PHONE}) 
     * <br> This means that you could query the contacts that have at least 1 phone available (contacts without phone informed are not returned) by using this pair (column==Phone AND condition==Available)
     * <br> @version 4.5
     * @type int
     */
    this.CONTACTS_QUERY_CONDITION_AVAILABLE = 4;

    /**
     * @event onListContactsEnd Fired when the list of contacts (retrieved from the phone address book) is returned to the javascript application.
     * <br>Method to be overrided by JS applications, to handle this event.
     * <br> For further information see, {@link Appverse.Pim.ContactLite ContactLite}.
     * @aside guide application_listeners
     * <br> @version 4.3
     * @param {Appverse.Pim.ContactLite[]} contacts An array of ContactLite objects successfully retrieved from the device local agenda.
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
     */
    this.onListContactsEnd = function(contacts) {
    };
	
    /**
     * @event onAccessToContactsDenied Fired when the app executes any of the contacts API feature (list contacts, create contact, get contact, etc) 
	 * and the user has revoked or never granted access to the contacts information for this app (via the device Privacy Settings).
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 4.8
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
     */
    this.onAccessToContactsDenied = function() {
    };

    /**
     * @event onContactFound Fired when the contact search (retrieved from the phone address book) is returned to the javascript application.
     * <br>Method to be overrided by JS applications, to handle this event.
     * <br> For further information see, {@link Appverse.Pim.Contact Contact}.
     * @aside guide application_listeners
     * <br> @version 5.0
     * @param {Appverse.Pim.Contact} contact The Contact object successfully retrieved from the device local agenda. Or null if that contact is not found.
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
     */
    this.onContactFound = function(contact) {
    };

    /**
     * @event onListCalendarEntriesEnd Fired when the list of calendar entries (retrieved from the phone calendar) is returned to the javascript application.
     * <br>Method to be overrided by JS applications, to handle this event.
     * <br> For further information see, {@link Appverse.Pim.CalendarEntry CalendarEntry}.
     * @aside guide application_listeners
     * <br> @version 5.0
     * @param {Appverse.Pim.CalendarEntry[]} contacts An array of CalendarEntry objects successfully retrieved from the device local calendar.
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
     */
    this.onListCalendarEntriesEnd = function(calendarEntries) {
    };

}

Appverse.Pim = new Pim();

/**
 * List of stored phone contacts that match given query. <br/>For further information see, {@link Appverse.Pim.ContactLite ContactLite}.
 * <br> Data is returned via the proper event handled by the "Appverse.Pim.onListContactsEnd" method; please, override to handle the event.
 * <br> @version 5.0
 * @param {Appverse.Pim.ContactQuery} query The search query object. Optional parameter.<pre>null value for all contact returned.</pre>
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.ListContacts = function(query)
{
    if (query == null) {
        post_to_url_async(Appverse.Pim.serviceName, "ListContacts", null, null, null);
    } else {
        post_to_url_async(Appverse.Pim.serviceName, "ListContacts", get_params([query]), null, null);
    }
};

/**
 * Get full version of a contact given its Id.<br/>For further information see, {@link Appverse.Pim.Contact Contact}.
 * <br> Contact data found is returned via the proper event handled by the "Appverse.Pim.onContactFound" method; please, override to handle the event.
 * <br> @version 5.0
 * <br> It returns a {Appverse.Pim.Contact} object with the contact requested.
 * @param {String} id The contact identifier to search for.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.GetContact = function(id)
{
    post_to_url_async(Appverse.Pim.serviceName, "GetContact", get_params([id]), null, null);
};


/**
 * Creates a Contact based on given contact data. <br/>For further information see, {@link Appverse.Pim.Contact Contact}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.Pim.Contact} object representing the Created contact.
 * @param {Appverse.Pim.Contact} contact Contact data to be created.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.CreateContact = function(contact, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Pim.serviceName, "CreateContact", get_params([contact]), callbackFunctionName, callbackId);
};

/**
 * Updates contact data (given its ID) with the given contact data. <br/>For further information see, {@link Appverse.Pim.Contact Contact}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True on successful updating.
 * @param {string} contactId Contact identifier to be updated with new data.
 * @param {Appverse.Pim.Contact} newContact New contact data to be added to the given contact.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.UpdateContact = function(contactId, newContactData, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Pim.serviceName, "UpdateContact", get_params([contactId, newContactData]), callbackFunctionName, callbackId);
};

/**
 * Deletes the given contact. <br/>For further information see, {@link Appverse.Pim.Contact Contact}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True on successful deletion.
 * @param {Appverse.Pim.Contact} contact Contact data to be deleted.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.DeleteContact = function(contact, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Pim.serviceName, "DeleteContact", get_params([contact]), callbackFunctionName, callbackId);
};

/**
 * Lists calendar entries for given date. <br/>For further information see, {@link Appverse.Pim.CalendarEntry CalendarEntry}.
 * <br> @version 5.0D
 * <br> It returns an array of {Appverse.Pim.CalendarEntry[]} objects with the list of calendar entries.
 * <br> Data is returned via the proper event handled by the "Appverse.Pim.onListCalendarEntriesEnd" method; please, override to handle the event.
 * @param {Appverse.DateTime} date Date to match calendar entries.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/warning.png"/> *further testing required | android <img src="resources/images/warning.png"/> *further testing required | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.ListCalendarEntriesByDate = function(date, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Pim.serviceName, "ListCalendarEntries", get_params([date]), callbackFunctionName, callbackId);
};

/**
 * Lists calendar entries between given start and end dates. <br/>For further information see, {@link Appverse.Pim.CalendarEntry CalendarEntry}.
 * <br> @version 5.0
 * <br> It returns an array of {Appverse.Pim.CalendarEntry[]} objects with the list of calendar entries.
 * <br> Data is returned via the proper event handled by the "Appverse.Pim.onListCalendarEntriesEnd" method; please, override to handle the event.
 * @param {Appverse.DateTime} startDate Start date to match calendar entries.
 * @param {Appverse.DateTime} endDate End date to match calendar entries.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/warning.png"/> *further testing required | android <img src="resources/images/warning.png"/> *further testing required | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.ListCalendarEntriesByDateRange = function(startDate, endDate, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Pim.serviceName, "ListCalendarEntries", get_params([startDate, endDate]), callbackFunctionName, callbackId);
};

/**
 * Creates a calendar entry. <br/>For further information see, {@link Appverse.Pim.CalendarEntry CalendarEntry}.
 * <br> @version 5.0
 * <br> It returns a {Appverse.Pim.CalendarEntry} object with the created calendar entry.
 * @param {Appverse.Pim.CalendarEntry} entry Calendar entry to be created.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/warning.png"/> *issues with recurrences and alarms | android <img src="resources/images/warning.png"/> *issues with recurrences and alarms | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.CreateCalendarEntry = function(entry, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Pim.serviceName, "CreateCalendarEntry", get_params([entry]), callbackFunctionName, callbackId);
};

/**
 * Deletes the given calendar entry. <br/>For further information see, {@link Appverse.Pim.CalendarEntry CalendarEntry}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True on successful deletion.
 * @param {Appverse.Pim.CalendarEntry} entry Calendar entry to be deleted.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.DeleteCalendarEntry = function(entry, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Pim.serviceName, "DeleteCalendarEntry", get_params([entry]), callbackFunctionName, callbackId);
};

/**
 * Moves the given calendar entry to the new start and end dates. <br/>For further information see, {@link Appverse.Pim.CalendarEntry CalendarEntry}.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True on successful deletion.
 * @param {Appverse.Pim.CalendarEntry} entry Calendar entry to be moved. 
 * @param {Appverse.DateTime} startDate New start date to move the calendar entry.
 * @param {Appverse.DateTime} endDate New end date to move the calendar entry.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *xml data store</pre>
 */
Pim.prototype.MoveCalendarEntry = function(entry, startDate, endDate, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Pim.serviceName, "MoveCalendarEntry", get_params([entry, startDate, endDate]), callbackFunctionName, callbackId);
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
     * @cfg {String}
     * Telephony service name (as configured on Platform Service Locator).
     * <br> @version 1.0
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
 * <br> @version 5.0
 * <br/>Possible values of the 'callType' argument: 
 * {@link Appverse.Telephony#CALLTYPE_VOICE CALLTYPE_VOICE}, 
 * {@link Appverse.Telephony#CALLTYPE_FAX CALLTYPE_FAX}, 
 * & {@link Appverse.Telephony#CALLTYPE_DIALUP CALLTYPE_DIALUP}
 * @param {String} number Phone number to call to.
 * @param {int} callType The type of call to open.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Telephony.prototype.Call = function(number, callType)
{
    post_to_url_async(Appverse.Telephony.serviceName, "Call", get_params([number, callType]), null, null);
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
     * @cfg {String}
     * I18N service name (as configured on Platform Service Locator).
     * <br> @version 1.0
     */
    this.serviceName = "i18n";
}

Appverse.I18N = new I18N();


/**
 * List of supported locales for the application (the ones configured on the '/app/config/i18n-config.xml' file).
 * <br/>For further information see, {@link Appverse.I18N.Locale Locale}.
 * <br> @version 5.0
 * <br> It returns an array of {Appverse.I18N.Locale[]} objects with the list of supported app locales.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
I18N.prototype.GetLocaleSupported = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.I18N.serviceName, "GetLocaleSupported", null, callbackFunctionName, callbackId);
};

/**
 * List of supported locales for the application (the ones configured on the '/app/config/i18n-config.xml' file).
 * <br/>For further information see, {@link Appverse.I18N.Locale Locale}. 
 * <br> @version 5.0
 * <br> It returns an array of {String[]} objects with the list of locale descriptors (only locale descriptor string, such as "en-US").
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
I18N.prototype.GetLocaleSupportedDescriptors = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.I18N.serviceName, "GetLocaleSupportedDescriptors", null, callbackFunctionName, callbackId);
};

/**
 * Gets the text/message corresponding to the given key and locale.
 * <br/>For further information see, {@link Appverse.I18N.Locale Locale}.
 * <br> @version 51.0
 * <br> It returns a {String} with the localized text.
 * @param {String} key The key to match text.
 * @param {String/Appverse.I18N.Locale} locale The full locale object to get localized message, or the locale desciptor ("language" or "language-country" two-letters ISO codes.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/> </pre>
 */
I18N.prototype.GetResourceLiteral = function(key, locale, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.I18N.serviceName, "GetResourceLiteral", get_params([key, locale]), callbackFunctionName, callbackId);
};

/**
 * Gets the full application configured literals (key/message pairs) corresponding to the given locale.
 * <br/>For further information see, {@link Appverse.I18N.Locale Locale} and {@link Appverse.I18N.ResourceLiteralDictionary ResourceLiteralDictionary}.
 * <br> @version 5.0
 * <br> It returns the {Appverse.I18N.ResourceLiteralDictionary} object Localized texts in the form of an object (you could get the value of a keyed literal using <b>resourceLiteralDictionary.MY_KEY</b> or <b>resourceLiteralDictionary["MY_KEY"]</b>).
 * @param {String/Appverse.I18N.Locale} locale The full locale object to get localized message, or the locale desciptor ("language" or "language-country" two-letters ISO codes.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/check.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
I18N.prototype.GetResourceLiterals = function(locale, callbackFunctionName, callbackId)
{
    if (locale == null) {
        post_to_url_async(Appverse.I18N.serviceName, "GetResourceLiterals", null, callbackFunctionName, callbackId);
    } else {
        post_to_url_async(Appverse.I18N.serviceName, "GetResourceLiterals", get_params([locale]), callbackFunctionName, callbackId);
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
     * @cfg {String}
     * Log service name (as configured on Platform Service Locator).
     * <br> @version 1.0
     */
    this.serviceName = "log";
}

Appverse.Log = new Log();


/**
 * Logs the given message, with the given log level if specified, to the standard platform/environment.
 * <br> @version 5.0
 * @param {String} message The message to be logged.
 * @param {int} level The log level (optional).
 * <br> It returns a {Boolean} with a value of True on successful logged.
 * @method
 */
Log.prototype.Log = function(message, level)
{
    post_to_url_async(Appverse.Log.serviceName, "Log", get_params([message, level]), null, null);
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
     * @cfg {String}
     * Security service name (as configured on Platform Service Locator).
     * <br> @version 3.7
     */
    this.serviceName = "security";
	
    /**
     * Local Authentication Status : Success
     * <br> @version 5.0.7
     * @type int
     */
	this.LA_STATUS_SUCCESS = 0;
	
    /**
     * Local Authentication Status : Application retry limit exceeded
     * <br> @version 5.0.7
     * @type int
     */
	this.LA_STATUS_RETRY_EXCEEDED = 1;
	
    /**
     * Local Authentication Status : Canceled by User
     * <br> @version 5.0.7
     * @type int
     */
	this.LA_STATUS_USER_CANCELED = 2;
	
    /**
     * Local Authentication Status : Authentication failed because the user used a fallback (for example, a password).
     * <br> @version 5.0.7
     * @type int
     */
	this.LA_STATUS_USER_FALLBACK = 3;
	
    /**
     * @event onTouchIDNotAvailable Fired when the app requests a local authentication using Touch ID (biometrics) but this device has not available this feature
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 5.0.7
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
     */
    this.onTouchIDNotAvailable = function() {
    };
	
    /**
     * @event onLocalAuthenticationWithTouchIDReply Fired when the app requests a local authentication using Touch ID (biometrics) and the user completes the authentication action
	 * <b> Result could be success or failure
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 5.0.7
 	 * @param {int} status The authentication status, 0 if success.  Possible values: Appverse.Security.LA_STATUS_SUCCESS, Appverse.Security.LA_STATUS_RETRY_EXCEEDED, Appverse.Security.LA_STATUS_USER_CANCELED, and Appverse.Security.LA_STATUS_USER_FALLBACK
     * @param {String} errorDescription The error description if authentication failure, null otherwise.
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
     */
    this.onLocalAuthenticationWithTouchIDReply = function(success, errorDescription) {
    };
}

Appverse.Security = new Security();


/**
 * Checks if the device has been modified.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of True if the device is modified.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data </pre>
 */
Security.prototype.IsDeviceModified = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Security.serviceName, "IsDeviceModified", null, callbackFunctionName, callbackId);
};

/**
 * Adds or updates  - if already exists - a given key/value pair into the device local secure storage.
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnKeyValuePairsStoreCompleted
 * <br> @version 5.0
 * @method
 * @param {Appverse.Security.KeyPair} keyPair A key/value pair to store
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Security.prototype.StoreKeyValuePair = function(keyPair)
{
    post_to_url_async(Appverse.Security.serviceName, "StoreKeyValuePair", get_params([keyPair]), null, null);
};

/**
 * Adds or updates - if already exists - a given list of key/value pairs into/to the device local secure storage.
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnKeyValuePairsStoreCompleted
 * <br> @version 4.2
 * @method
 * @param {Appverse.Security.KeyPair[]} keyPair A list of key/value pairs to store
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Security.prototype.StoreKeyValuePairs = function(keyPairs)
{
    post_to_url_async(Appverse.Security.serviceName, "StoreKeyValuePairs", get_params([keyPairs]), null, null);
};

/**
 * Returns a previously stored key/value pair from the device local secure storage.
 * <br> Returned data should be handled by overriding the corresponding Platform Listeners Appverse.OnKeyValuePairsFound
 * <br> @version 4.2
 * @method
 * @param {String} key Name of the key to be returned
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Security.prototype.GetStoredKeyValuePair = function(key)
{
    post_to_url_async(Appverse.Security.serviceName, "GetStoredKeyValuePair", get_params([key]), null, null);
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
    post_to_url_async(Appverse.Security.serviceName, "GetStoredKeyValuePairs", get_params([keys]), null, null);
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
    post_to_url_async(Appverse.Security.serviceName, "RemoveStoredKeyValuePair", get_params([key]), null, null);
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
    post_to_url_async(Appverse.Security.serviceName, "RemoveStoredKeyValuePairs", get_params([keys]), null, null);
};

/**
 *	Starts a local authentication operation displaying Touch ID screen (biometrics).
 * <br> Result data should be handled by overriding the corresponding Platform Listeners Appverse.Security.onLocalAuthenticationWithTouchIDReply
 * <br> Touch ID feature is only available for iPhone 5S and greater iOS devices. When not available, a platform listener Appverse.Security.onTouchIDNotAvailable is called to aware the application.
 * <br> @version 5.0.7
 * @method
 * @param {String} reason A reason to explain why authentication is needed. This helps to build trust with the user.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Security.prototype.StartLocalAuthenticationWithTouchID = function(reason) {
	post_to_url_async(Appverse.Security.serviceName, "StartLocalAuthenticationWithTouchID", get_params([reason]), null, null);
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
     * @cfg {String}
     * AppLoader service name (as configured on Platform Service Locator).
     * <br> @version 4.0
     */
    this.serviceName = "loader";

    /**
     * @event onUpdateModulesFinished Fired when the applications loader has finished to download (update) modules 
     * (after calling either the {@link Appverse.AppLoader.UpdateModules UpdateModules} method or the {@link Appverse.AppLoader.UpdateModule UpdateModule} method), 
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 4.0
     * @param {Appverse.AppLoader.Module[]} successUpdates The list of successful updated modules.
     * @param {Appverse.AppLoader.Module[]} failedUpdates The list of failed updated modules.
     * @param {String} callbackId The callback id (provided by when calling UpdateModule/s method) that identifies this event listener (callback) with the calling request.
     */
    this.onUpdateModulesFinished = function(successUpdates, failedUpdates, callbackId) {
    };

    /**
     * @event onDeleteModulesFinished Fired when the applications loader has finished to delete modules 
     * (after calling the {@link Appverse.AppLoader.DeleteModules DeleteModules} method), 
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 4.0
     * @param {Appverse.AppLoader.Module[]} successDeletes The list of successful deleted modules.
     * @param {Appverse.AppLoader.Module[]} failedDeletes The list of failed deleted modules.
     */
    this.onDeleteModulesFinished = function(successDeletes, failedDeletes) {
    };
}

Appverse.AppLoader = new AppLoader();


/**
 * Initializes the context of the Application Loader for the next operations.
 * <br> @version 5.0
 * @param {Appverse.AppLoader.ModuleContext} context The current context options for handling modules.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 */
AppLoader.prototype.InitializeModuleContext = function(context, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.AppLoader.serviceName, "InitializeModuleContext", get_params([context]), callbackFunctionName, callbackId);
};

/**
 * Returns a list .
 * <br> @version 5.0
 * <br> It returns an array of {Appverse.AppLoader.Module[]} objects with the list of currently installed modules (locally)
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 */
AppLoader.prototype.ListInstalledModules = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.AppLoader.serviceName, "ListInstalledModules", null, callbackFunctionName, callbackId);
};

/**
 * Updates a given module list (or installs if it was never previously downloaded).
 * <br> @version 5.0
 * @param {Appverse.AppLoader.Module[]} modules The modules to be downloaded (Appverse.AppLoader.Module#LoadUrl is used for downloading each module).
 * @param {String} callbackId The callback identifier of this request to be returned on the corresponding event listener (callback). Null value is not needed.
 * @method
 */
AppLoader.prototype.UpdateModules = function(modules, callbackId)
{
    if (!callbackId)
        callbackId = "";
    post_to_url_async(Appverse.AppLoader.serviceName, "UpdateModules", get_params([modules, callbackId]), null, callbackId);
};

/**
 * Updates a given module (or installs if it was never previously downloaded).
 * <br> @version 5.0
 * @param {Appverse.AppLoader.Module[]} module The module to be downloaded (the field <b>Appverse.AppLoader.Module#LoadUrl</b> is used for downloading each module).
 * @param {String} callbackId The callback identifier of this request to be returned on the corresponding event listener (callback). Null value is not needed.
 * @method
 */
AppLoader.prototype.UpdateModule = function(module, callbackId)
{
    if (!callbackId)
        callbackId = "";
    post_to_url_async(Appverse.AppLoader.serviceName, "UpdateModule", get_params([module, callbackId]), null, callbackId);
};

/**
 * Deletes a given modules.
 * <br> @version 5.0
 * @param {Appverse.AppLoader.Module[]} modules The modules to be deleted.
 * @method
 */
AppLoader.prototype.DeleteModules = function(modules)
{
    post_to_url_async(Appverse.AppLoader.serviceName, "DeleteModules", get_params([modules]), null, null);
};

/**
 * Loads a Module inside the Appverse WebResources Container (WebView). All modules should include an 'index.html' file as the main HTML file (entry point).
 * <br> @version 5.0
 * @param {Appverse.AppLoader.Module} module The module to be loaded.
 * @param {Appverse.AppLoader.ModuleParam[]} parameters The parameters to be added to the module main HTML file request; as GET request parameters (optional field, null for not including any parameter).
 * @param {Boolean} autoUpdate True to upload the module (using the corresponding LoadUrl and Version) prior to load it. Optional parameter. False is the default value. The update would be "silent", no event listener will be called by the platform in this case.
 * @method
 */
AppLoader.prototype.LoadModule = function(module, parameters, autoUpdate)
{
    post_to_url_async(Appverse.AppLoader.serviceName, "LoadModule", get_params([module, parameters, autoUpdate]), null, null);
};


/*
 * API INTERFACES FOR MODULES
 */

// APPVERSE_MODULES_SERVICES

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

    if (paramsArray != null && paramsArray.length > 0) {
        params = "{";
        for (var i = 0; i < paramsArray.length; i++)
        {
            if (paramsArray[i] == null) {
                if (i > 0) {
                    params = params + ",";
                }
                params = params + '"param' + (i + 1) + '":null';
            }

            if (paramsArray[i] != null) { // [fix-01062011:MAPS:in some cases we need to upload empty objects, like on the SendEmail method)]  //&& JSON.stringify(paramsArray[i])!='{}') {
                if (i > 0) {
                    params = params + ",";
                }
                params = params + '"param' + (i + 1) + '":' + JSON.stringify(paramsArray[i]);
            }
        }
        params = params + "}";
    }
    return params;
}

function post_to_url_async(serviceName, methodName, params, callBackFuncName, callbackId) {
    method = "POST"; // Set method to post by default, if not specified.

    var legacyPath = Appverse.SERVICE_URI + serviceName + "/" + methodName;
    var newPath = Appverse.APPVERSE_SERVICE_URI + serviceName + "/" + methodName; // new path for Appverse 5.0 (applied when possible)
    
	var path = legacyPath;  // by default, use legacy path

	if(Appverse.is.iOS) {
		path = newPath;  // we use the new path for all iOS devices
	}
	
    /* background services are now enabled (new in appverse 5.0)
    if (Appverse.isBackground()) {  
        // socket is closed, do not call appverse services
        console.log("Application is on background. Internal Appverse Socket is closed. Call to '" + path + "' has been stopped.");
        return null;
    }
    */

    var reqData = "";
    if (callBackFuncName != null) {
        reqData = reqData + "callback=" + callBackFuncName;
    } else {
        reqData = reqData + "callback=NULL";
    }
    if (callbackId != null) {
        reqData = reqData + "&callbackid=" + callbackId;
    } else {
        reqData = reqData + "&callbackid=callbackid";
    }

    if (params != null) {
        if (Appverse.unescapeNextRequestData) {
            reqData = reqData + "&json=" + unescape(params);
        } else {
            reqData = reqData + "&json=" + params; // we don't unscape parameters if configured
            Appverse.unescapeNextRequestData = true; // returning to default configuration value
        }
    }
	
	if(window.webkit) {
		// using new WKWebView message handlers, if available (iOS 8)
		window.webkit.messageHandlers.service.postMessage({uri: path, query: reqData});
		return;
	} 		
	
    if(window.appverseJSBridge) {  // only available for 4.2+ Android devices 
        path = newPath;
		window.appverseJSBridge.postMessage(path, reqData);
		return;
    }
	
	if(window.external) {  // using external post notifications for Windows Phone
		var t = {uri: path, query: reqData};
		window.external.notify(JSON.stringify(t));
		return;
	}
	
    var xhr = new XMLHttpRequest();
    xhr.open(method, path, false);
    xhr.setRequestHeader("Content-type", "application/json;charset=UTF-8");
    try {
        xhr.send(reqData);
    } catch (e) {
		if(e!=null && e.code==101) {
			// do not send callback (cross-side scripting warning but request has reached the internal server normally)
			return;
		}
		console.dir("error sending data async: " + reqData);
        //Javascript Injection via Request 
        var patt = new RegExp(/^[a-z_][\w_]*$/i);
        if (!patt.test(callBackFuncName)) {
            console.log("********************** UNSAFE CALL ***************");
            return;
        }
        var callbackfn = window[callBackFuncName];
        if (callbackfn)
            callbackfn(null, callbackId);
    }
    // nothing to return, callback function will be called with result data
}

function post_to_url_async_emu(serviceName, methodName, params, callBackFuncName, callbackId) {
    method = "POST"; // Set method to post by default, if not specified.

    //var path = Appverse.SERVICE_ASYNC_URI + serviceName + "/" + methodName;
    // on emulator, async call will be simulated, not fully async mode (so we could still use developer tools on external browsers)
    var path = Appverse.SERVICE_URI + serviceName + "/" + methodName;

    if (Appverse.isBackground()) {
        // socket is closed, do not call appverse services
        console.log("Application is on background. Internal Appverse Socket is closed. Call to '" + path + "' has been stopped.");
        return null;
    }

    var xhr = new XMLHttpRequest();
    xhr.open(method, path, false);
    xhr.setRequestHeader("Content-type", "application/json");
    var reqData = "";

    if (callBackFuncName != null) {
        reqData = reqData + "callback=" + callBackFuncName;
    } else {
        reqData = reqData + "callback=NULL";
    }
    if (callbackId != null) {
        reqData = reqData + "&callbackid=" + callbackId;
    } else {
        reqData = reqData + "&callbackid=callbackid";
        callbackId = "callbackId";
    }

    if (params != null) {
        //reqData = reqData + "&json=" + unescape(params);
        if (Appverse.unescapeNextRequestData) {
            reqData = reqData + "&json=" + unescape(params);
        } else {
            reqData = reqData + "&json=" + params; // we don't unscape parameters if configured
            Appverse.unescapeNextRequestData = true; // returning to default configuration value
        }
    }

    if (AppverseEmulator.eventListenersRegistered && (AppverseEmulator.eventListenersRegistered.indexOf(methodName)>=0)) {
        AppverseEmulator.queuedListenerMessagesCount++;
        console.log("Appverse Emulator - queue listener result for methodName: " + AppverseEmulator.normalizeListenerCallingName(methodName));
		
		(function(smn) {
                setTimeout(function() {
                    AppverseEmulator.appverseListenerPollingTimerFunc(smn);
                }, AppverseEmulator.pollingInterval);
            })(serviceName+"#"+AppverseEmulator.normalizeListenerCallingName(methodName));
    }

    try {
        xhr.send(reqData);

        if (callBackFuncName != null) {
            AppverseEmulator.queuedCallbackMessagesCount++;
            console.log("Appverse Emulator - queue callback result for methodName: " + methodName);
            (function(c, ci) {
                setTimeout(function() {
                    AppverseEmulator.appverseCallbackPollingTimerFunc(c, ci);
                }, AppverseEmulator.pollingInterval);
            })(callBackFuncName, callbackId);
        }


    } catch (e) {
		if(e!=null && e.code==101) {
			// do not send callback (cross-side scripting warning but request has reached the internal server normally)
			console.warn("XSS warning... ignore");
			return;
		}
		
        console.dir("error sending data async: " + reqData);
        var callbackfn = window[callBackFuncName];
        if (!callbackfn) {
            try {
                callbackfn = eval('(' + callBackFuncName + ')');
            } catch (e) {
                console.log("please define the callback function as a global variable. Error while evaluating function: " + e);
            }
        }
        if (callbackfn)
            callbackfn(null, callbackId);
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
        if (t == "string")
            obj = '"' + obj + '"';
        return String(obj);
    }
    else {
        // recurse array or object 
        var n, v, json = [], arr = (obj && obj.constructor == Array);
        for (n in obj) {
            v = obj[n];
            t = typeof(v);
            if (t == "string")
                v = '"' + v + '"';
            else if (t == "object" && v !== null)
                v = JSON.stringify(v);
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
    return this.replace(pattern, function(capture) {
        return args[capture.match(/\d+/)];
    });
}


/*
 * Initialize Appverse Utility "is" object
 *
 */
Appverse.init();