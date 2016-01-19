

/*
 * WEBTREKK INTERFACES
 */

/**
 * @class Appverse.Webtrekk
 * Module class to access Appverse Webtrekk module interface. 
 * <br>This interface provides features to track application usage and send to Webtrekk.<br>
 * <br> @version 5.0.3
 * <pre>Usage: Appverse.Webtrekk.&lt;metodName&gt;([params]).<br>Example: Appverse.Webtrekk.TrackContent('/mycontent').</pre>
 * @component
 * @aside guide appverse_modules
 * @constructor Constructs a new Webtrekk interface.
 * @return {Appverse.Webtrekk} A new Webtrekk interface.
 */
Webtrekk = function() {
    /**
     * @cfg {String}
     * Webtrekk service name (as configured on Platform Service Locator).
     * <br> @version 5.0.3
     */
    this.serviceName = "webtrekk";

    /**
     * Country code of the client's language settings (e.g 'de').
     * <br> @version 5.0.3
     * @type string
     */
    this.COUNTRY_CODE = "la";

    /**
     * Order value.
     * <br> @version 5.0.3
     * @type string
     */
    this.ORDER_VALUE = "ov";

    /**
     * Order ID.
     * <br> @version 5.0.3
     * @type string
     */
    this.ORDER_ID = "oi";

    /**
     * Products in shopping basket.
     * <br> @version 5.0.3
     * @type string
     */
    this.SHOPPING_BASKET = "ba";

    /**
     * Product costs.
     * <br> @version 5.0.3
     * @type string
     */
    this.PRODUCTS_COSTS = "co";

    /**
     * Number of products.
     * <br> @version 5.0.3
     * @type string
     */
    this.NUMBER_OF_PRODUCTS = "qn";

    /**
     * Product category. Value is 'ca1' but you can add more categories by using 'ca2', 'ca3', etc...
     * <br> @version 5.0.3
     * @type string
     */
    this.PRODUCT_CATEGORY_1 = "ca1";

    /**
     * Status of the shopping basket (add|conf|view).
     * <br> @version 5.0.3
     * @type string
     */
    this.STATUS_SHOPPING_BASKET = "st";

    /**
     * Customer ID
     * <br> @version 5.0.3
     * @type string
     */
    this.CUSTOMER_ID = "cd";

    /**
     * Search term of internal search function.
     * <br> @version 5.0.3
     * @type string
     */
    this.SEARCH_TERM = "is";

    /**
     * Campaign ID consistinf od media code parameter and value ('wt_mc=newsletter')
     * <br> @version 5.0.3
     * @type string
     */
    this.CAMPAIGN_ID = "mc";

    /**
     * Content Group. Value is 'cg1' but you can add more categories by using 'cg2', 'cg3', etc...
     * <br> @version 5.0.3
     * @type string
     */
    this.CONTENT_GROUP = "cg1";

    /**
     * Page parameter. Value is 'cp1' but you can add more categories by using 'cp2', 'cp3', etc...
     * <br> @version 5.0.3
     * @type string
     */
    this.PAGE_PARAMETER = "cp1";

    /**
     * Session parameter. Value is 'cs1' but you can add more categories by using 'cs2', 'cs3', etc...
     * <br> @version 5.0.3
     * @type string
     */
    this.SESSION_PARAMETER = "cs1";

    /**
     * Action parameter. Value is 'ck1' but you can add more categories by using 'ck2', 'ck3', etc...
     * <br> @version 5.0.3
     * @type string
     */
    this.ACTION_PARAMETER = "ck1";

    /**
     * Independent parameter. Value is 'ce1' but you can add more categories by using 'ce2', 'ce3', etc...
     * <br> @version 5.0.3
     * @type string
     */
    this.INDEPENDENT_PARAMETER = "ce1";

    /**
     * Campaign parameter. Value is 'cc1' but you can add more categories by using 'cc2', 'cc3', etc...
     * <br> @version 5.0.3
     * @type string
     */
    this.CAMPAIGN_PARAMETER = "cc1";

    /**
     * E-commerce parameter. Value is 'cb1' but you can add more categories by using 'cb2', 'cb3', etc...
     * <br> @version 5.0.3
     * @type string
     */
    this.ECOMMERCE_PARAMETER = "cb1";
};

Appverse.Webtrekk = new Webtrekk();

/**
 * Starts the tracker - for the given web server URL and Track Id - from receiving and dispatching data to the server.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of true if the tracker was started successfully
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
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Webtrekk.prototype.StartTracking = function(webServerUrl, trackId, samplingRate, callbackFunctionName, callbackId) {
	if(samplingRate!=null)
    	post_to_url_async(Appverse.Webtrekk.serviceName, "StartTracking", get_params([webServerUrl, trackId, samplingRate]), callbackFunctionName, callbackId);
	else
		post_to_url_async(Appverse.Webtrekk.serviceName, "StartTracking", get_params([webServerUrl, trackId]), callbackFunctionName, callbackId);
};

/**
 * Stops the tracker from receiving and dispatching data to the server
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of true if tracker was stopped
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Webtrekk.prototype.StopTracking = function(callbackFunctionName, callbackId) {
    post_to_url_async(Appverse.Webtrekk.serviceName, "StopTracking", null, callbackFunctionName, callbackId);
};

/**
 * Sends a button click event to be tracked by the webtrekk tracker
 * <br> You should invoke the {@link Appverse.Webtrekk#StartTracking StartTracking} method prior to invoke this method.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of true if the content/event was successfully tracked
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
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Webtrekk.prototype.TrackClick = function(clickId, contentId, additionalParameters, callbackFunctionName, callbackId) {
    post_to_url_async(Appverse.Webtrekk.serviceName, "TrackClick", get_params([clickId, contentId, additionalParameters]), callbackFunctionName, callbackId);
};

/**
 * Sends a content/event to be tracked by the webtrekk tracker
 * <br> You should invoke the {@link Appverse.Webtrekk#StartTracking StartTracking} method prior to invoke this method.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of true if the content/event was successfully tracked
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
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Webtrekk.prototype.TrackContent = function(contentId, additionalParameters, callbackFunctionName, callbackId) {
    post_to_url_async(Appverse.Webtrekk.serviceName, "TrackContent", get_params([contentId, additionalParameters]), callbackFunctionName, callbackId);
};

/**
 * Sets the time interval the request will use to transmit data to the server
 * <br> This method should be executed prior to start the session tracking using the {@link Appverse.Webtrekk#StartTracking StartTracking} method.
 * <br> @version 5.0
 * <br> It returns a {Boolean} with a value of true if the interval was successfully set
 * <pre>
 * Default value is 5 minutes (300 seconds).
 * To maximise battery life, the time interval can be increased to, for example, 10 minutes (600 seconds).
 * </pre>
 * @param {double} intervalInSeconds The interval in seconds the request will transmit data to the server
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/> </pre>
 */
Webtrekk.prototype.SetRequestInterval = function(intervalInSeconds, callbackFunctionName, callbackId) {
    post_to_url_async(Appverse.Webtrekk.serviceName, "SetRequestInterval", get_params([intervalInSeconds]), callbackFunctionName, callbackId);
};
