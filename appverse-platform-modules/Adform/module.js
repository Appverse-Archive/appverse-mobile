/*
 * Adform INTERFACES
 */

/**
 * @class Appverse.Adform 
 * Module class to access Appverse Adform module interface. 
 * <br>This interface provides features to track application usage and send to Adform.<br>
 * <br> @version 5.0.10
 * <pre>Usage: Appverse.Adform.&lt;metodName&gt;([params]).<br>Example: Appverse.Adform.SendTrackPoint({'SectionName':'mysectionname', 'CustomParameters':[]}).</pre>
 * @component
 * @aside guide appverse_modules
 * @constructor Constructs a new Adform interface.
 * @return {Appverse.Adform} A new Adform interface.
 */
Adform = function() {
    /**
     * @cfg {String}
     * Adform service name (as configured on Platform Service Locator).
     * <br> @version 5.0.10
     */
    this.serviceName = "adform";
};

Appverse.Adform = new Adform();


/**
 * Sends an track point data to be tracked by the Adform system
 * <br> @version 5.0.10
 * @param {Appverse.Adform.AdformTrackPoint} trackPoint The track point data (section and custom parameters array) to be tracked
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Adform.prototype.SendTrackPoint = function(trackPoint) {
    post_to_url_async(Appverse.Adform.serviceName, "SendTrackPoint", get_params([trackPoint]), null, null);
};
