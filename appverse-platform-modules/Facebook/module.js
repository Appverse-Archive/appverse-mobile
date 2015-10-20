/*
 * Facebook INTERFACES
 */

/**
 * @class Appverse.Facebook 
 * Module class to access Appverse Facebook module interface. 
 * <br>This interface provides features to track application installations and analytics tracking to Facebook.<br>
 * <br> ****** ATTENTION ********  this SDK integration do not cover all Facebook SDK functionalities, it just integrates the app download/installs/launches tracking events <br>
 * <br> ****** ATTENTION ********  this SDK integration is only available for iOS and Android platforms<br>
 * <br> @version 5.0.10
 * <pre>Usage: <no interfaces provided to be used by Javascript code>.</pre>
 * @component
 * @aside guide appverse_modules
 * @constructor Constructs a new Facebook interface.
 * @return {Appverse.Facebook} A new Facebook interface.
 */
Facebook = function() {
    /**
     * @cfg {String}
     * Facebook service name (as configured on Platform Service Locator).
     * <br> @version 5.0.10
     */
    this.serviceName = "facebook";
};

Appverse.Facebook = new Facebook();
