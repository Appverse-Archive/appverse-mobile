/*
 * AppsFlyer INTERFACES
 */

/**
 * @class Appverse.AppsFlyer 
 * Module class to access Appverse AppsFlyer module interface. 
 * <br>This interface provides features to track application usage and send to AppsFlyer.<br>
 * <br> @version 5.0.9
 * <pre>Usage: Appverse.AppsFlyer.&lt;metodName&gt;([params]).<br>Example: Appverse.AppsFlyer.TrackEvent('registration', 'myregistrationid').</pre>
 * @component
 * @aside guide appverse_modules
 * @constructor Constructs a new AppsFlyer interface.
 * @return {Appverse.AppsFlyer} A new AppsFlyer interface.
 */
AppsFlyer = function() {
    /**
     * @cfg {String}
     * AppsFlyer service name (as configured on Platform Service Locator).
     * <br> @version 5.0.9
     */
    this.serviceName = "appsflyer";
};

Appverse.AppsFlyer = new AppsFlyer();

/**
 * Initialize the AppsFlyer SDK data. Only call this method if you want to change the data at runtime 
 * By default the Platform will initialize the SDK with the data provided in the "appsflyer-config.xml"
 * <br> @version 5.0.9
 * @param {Appverse.AppsFlyer.AppsFlyerInitialization} initOptions Apps Flyer Initialization data
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
AppsFlyer.prototype.Initialize = function(initOptions) {
    post_to_url_async(Appverse.AppsFlyer.serviceName, "Initialize", get_params([initOptions]), null, null);
};


/**
 * Sends an event to be tracked by the AppsFlyer system
 * <br> @version 5.0.9
 * @param {Appverse.AppsFlyer.AppsFlyerTrackEvent} event The event name and value to be tracked
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
AppsFlyer.prototype.TrackEvent = function(event) {
    post_to_url_async(Appverse.AppsFlyer.serviceName, "TrackEvent", get_params([event]), null, null);
};
