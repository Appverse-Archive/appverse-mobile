

/*
 * Beacon INTERFACES
 */

/**
 * @class Appverse.Beacon
 * Module class to listen for Beacon events.
 * <br>This interface provides features for beacons handling.<br>
 * <br> @version 4.7
 * <pre>Usage: Appverse.Beacon.&lt;metodName&gt;([params]).<br>Example: Appverse.Beacon.StartMonitoringRegion('xxx-xxxx-xxxx').</pre>
 * @component
 * @aside guide appverse_modules
 * @constructor Constructs a new Beacon interface.
 * @return {Appverse.Beacon} A new Beacon interface.
 */
Beacon = function() {
    /**
     * @cfg {String}
     * Beacon service name (as configured on Platform Service Locator).
     * <br>Method to be overrided by JS applications, to handle this event.
     * <br> @version 4.7
     */
    this.serviceName = "beacon";

	/**
     * Distance Type as Immediate.
     * <br> @version 4.7
     * @type int
     */
    this.IMMEDIATE = 0;

	/**
     * Distance Type as Near.
     * <br> @version 4.7
     * @type int
     */
    this.NEAR = 1;

	/**
     * Distance Type as Far.
     * <br> @version 4.7
     * @type int
     */
    this.FAR = 2;

	/**
     * Distance Type as Unknown.
     * <br> @version 4.7
     * @type int
     */
    this.UNKNOWN = 3;

	/**
     * @event OnEntered Event when called when a previously detected beacon is found again.
     * <br>Method to be overrided by JS applications, to handle this event.
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
	 * @param {Appverse.Beacon.Beacon[]} beacons
     * @aside guide application_listeners
     * <br> @version 4.7
	 */
    this.OnEntered = function(beacons) {
        console.log(arguments);
        console.log('%c Override OnEntered method! ', 'background: #222; color: #bada55');
    };

    /**
     * @event OnExited Event when called when a previously detected beacon is not found.
     * <br>Method to be overrided by JS applications, to handle this event.
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
	 * @param {Appverse.Beacon.Beacon[]} beacons
     * @aside guide application_listeners
     * <br> @version 4.7
     */
    this.OnExited = function(beacons) {
        console.log(arguments);
        console.log('%c Override OnExited method! ', 'background: #222; color: #bada55');
    };

    /**
     * @event OnDiscover Event called when a new beacon is found.
     * <br>Method to be overrided by JS applications, to handle this event.
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
     * @param {Appverse.Beacon.Beacon[]} beacons
     * @aside guide application_listeners
     * <br> @version 4.7
     */
    this.OnDiscover = function(beacons) {
        console.log(arguments);
        console.log('%c Override OnDiscover method! ', 'background: #222; color: #bada55');
    };

    /**
     * @event OnUpdateProximity Event called when the distance to the known beacon changed.
     * <br>Method to be overrided by JS applications, to handle this event.
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
	 * @param {Appverse.Beacon.Beacon} beacon
     * @param {double} from
     * @param {double} to
     * @aside guide application_listeners
     * <br> @version 4.7
     */
    this.OnUpdateProximity = function(beacon, from, to) {
        console.log(arguments);
        console.log('%c Override OnUpdateProximity method! ', 'background: #222; color: #bada55');
    };

    /**
     * Applications should override/implement this method to be aware of location services petition denial.
     * @aside guide application_listeners
     * <br> @version 5.1.2
     * @event
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> N/A | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
     */
    this.onAccessToLocationDenied = function () {
        console.log(arguments);
        console.log('%c Override onAccessToLocationDenied method! ', 'background: #222; color: #bada55');
    };

}

Appverse.Beacon = new Beacon();



/**
 * Start monitoring a region looking for beacons with an UUID
 * <br> @version 4.7
 * @param {Appverse.Beacon.Region} region to monitor
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Beacon.prototype.StartMonitoringRegion = function(region)
{
    post_to_url_async(Appverse.Beacon.serviceName, "StartMonitoringRegion", get_params([region]));
};

/**
 * Start monitoring looking for all the beacons
 * <br> @version 4.7
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Beacon.prototype.StartMonitoringAllRegions = function()
{
    post_to_url_async(Appverse.Beacon.serviceName, "StartMonitoringAllRegions", null);
};

/**
 * Stop monitoring for beacons
 * <br> @version 4.7
 * @param {Appverse.Beacon.Region} region to stop monitor
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Beacon.prototype.StopMonitoring = function(region) {
    post_to_url_async(Appverse.Beacon.serviceName, "StopMonitoringBeacons", get_params([region]));
};
