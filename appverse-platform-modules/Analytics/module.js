/*
 * ANALYTICS INTERFACES
 */

/**
 * @class Appverse.Analytics 
 * Module class to access Appverse Analytics module interface. 
 * <br>This interface provides features to track application usage and send to Google Analytics.<br>
 * <br> @version 5.0.3
 * <pre>Usage: Appverse.Analytics.&lt;metodName&gt;([params]).<br>Example: Appverse.Analytics.TrackPageView('/mypage').</pre>
 * @component
 * @aside guide appverse_modules
 * @constructor Constructs a new Analytics interface.
 * @return {Appverse.Analytics} A new Analytics interface.
 */
Analytics = function() {
    /**
     * @cfg {String}
     * Analytics service name (as configured on Platform Service Locator).
     * <br> @version 5.0.3
     */
    this.serviceName = "analytics";
};

Appverse.Analytics = new Analytics();

/**
 * Starts the tracker - for the given web property id - from receiving and dispatching data to the server.
 * <br> @version 5.0.3
 * <br> It returns a {Boolean} with a value of true if the tracker was started successfully
 * @param {String} webPropertyID The web property ID with the format UA-99999999-9
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Analytics.prototype.StartTracking = function(webPropertyID, callbackFunctionName, callbackId) {
    post_to_url_async(Appverse.Analytics.serviceName, "StartTracking", get_params([webPropertyID]), callbackFunctionName, callbackId);
};

/**
 * 
 * Stops the tracker from receiving and dispatching data to the server
 * <br> @version 5.0.3
 * <br> It returns a {Boolean} with a value of true if tracker was stopped
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Analytics.prototype.StopTracking = function(callbackFunctionName, callbackId) {
    post_to_url_async(Appverse.Analytics.serviceName, "StopTracking", null, callbackFunctionName, callbackId);
};

/**
 * Sends an event to be tracked by the analytics tracker
 * <br> @version 5.0.3
 * <br> It returns a {Boolean} with a value of true if the event was successfully tracked
 * @param {String} group the event group
 * @param {String} action the event action
 * @param {String} label The event label
 * @param {Integer} value The event value
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Analytics.prototype.TrackEvent = function(group, action, label, value, callbackFunctionName, callbackId) {
    post_to_url_async(Appverse.Analytics.serviceName, "TrackEvent", get_params([group, action, label, value]), callbackFunctionName, callbackId);
};

/**
 * Sends a pageview to be tracked by the analytics tracker
 * <br> @version 5.0.3
 * <br> It returns a {Boolean} with a value of true if the pageview was successfully tracked
 * @param {String} relativeUrl The relativeUrl to the page i.e. "/home"
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Analytics.prototype.TrackPageView = function(relativeUrl, callbackFunctionName, callbackId) {
    post_to_url_async(Appverse.Analytics.serviceName, "TrackPageView", get_params([relativeUrl]), callbackFunctionName, callbackId);
};