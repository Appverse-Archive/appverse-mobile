/*
 * NFC INTERFACES
 */

/**
 * @class Appverse.NFC 
 * Module class to access Appverse NFC module interface. 
 * <br>This interface provides features to make NFC payments by configuring, starting, cancelling and handling the payments against a PointOfSell device (using custom library). 
 * <br>JUST AVAILABLE FOR ANDROID DEVICES.<br>
 * <br> @version 5.0.7
 * <pre>Usage: Appverse.NFC.&lt;metodName&gt;([params]).<br>Example: Appverse.NFC.StartNFCPaymentEngine().</pre>
 * @component
 * @aside guide appverse_modules
 * @constructor Constructs a new NFC interface.
 * @return {Appverse.NFC} A new NFC interface.
 */
NFC = function() {
    /**
     * @cfg {String}
     * NFC service name (as configured on Platform Service Locator).
     * <br> @version 4.5
     */
    this.serviceName = "nfc";

    /**
     * Sets the value that identifies the CMPA to use for payments.
     * <br> @version 4.5
     * @type String
     */
    this.PROPERTY_APPLICATION_ID = "application_id";

    /**
     * Sets the text label that identifies the application.
     * <br> @version 4.5
     * @type String
     */
    this.PROPERTY_APPLICATION_LABEL = "application_label";

    /**
     * Sets the number of bytes in the AID field to be published on PPSE.
     * <br> @version 4.5
     * @type String
     */
    this.PROPERTY_AID_NAME_LENGTH = "aid_name_length";

    /**
     * Sets the startup mode of Engine. (if value is true, USB debugging is allowed for NFC payment).
     * <br> @version 4.5
     * @type String
     */
    this.PROPERTY_DEBUG_MODE = "debug_mode";

    /**
     * Sets the value of the duration in milliseconds of vibration performed when there is a transaction.
     * <br> @version 4.5
     * @type String
     */
    this.PROPERTY_VIBRATION_DURATION = "vibration_duration_in_msec";

    /**
     * Sets the value of the duration in seconds of the timer, starts when the payment is triggered by the POS.
     * <br> @version 4.5
     * @type String
     */
    this.PROPERTY_TIMER_PERIOD = "timer_period_in_sec";

    /**
     * Unknown Security Error.
     * <br> @version 4.5
     * @type int
     */
    this.SECURITY_ERROR_UNKNOWN = 0;

    /**
     * Device Rooted Security Error.
     * <br> @version 4.5
     * @type int
     */
    this.SECURITY_ERROR_DEVICE_ROOTED = 1;

    /**
     * Lock Disabled Security Error.
     * <br> @version 4.5
     * @type int
     */
    this.SECURITY_ERROR_LOCK_DISABLED = 2;

    /**
     * USB Debugging Enabled Security Error.
     * <br> @version 4.5
     * @type int
     */
    this.SECURITY_ERROR_USB_DEBUG_ENABLED = 3;

    /**
     * Unknown Engine Start Error.
     * <br> @version 4.5
     * @type int
     */
    this.ENGINE_START_ERROR_UNKNOWN = 0;

    /**
     * Operation Failed Engine Start Error (registration to the Wallet failed).
     * <br> @version 4.5
     * @type int
     */
    this.ENGINE_START_ERROR_OPERATION_FAILED = 1;

    /**
     * Canceled by User Engine Start Error (registration to the Wallet was canceled by the user).
     * <br> @version 4.5
     * @type int
     */
    this.ENGINE_START_ERROR_OPERATION_CANCELED = 2;

    /**
     * Already Started Engine Start Error (the payment engine has been already started).
     * <br> @version 4.5
     * @type int
     */
    this.ENGINE_START_ERROR_ALREADY_STARTED = 3;

    /**
     * Error Card Engine Start Error (communication problem with the SIM).
     * <br> @version 4.5
     * @type int
     */
    this.ENGINE_START_ERROR_OPERATION_ERROR_CARD = 4;

    /**
     * Cardlet Not Found Engine Start Error 
     * (the engine did not detect the SIM CPMA identified by 'application_id' property at the configuration file located in /res/raw/nfcpaymentengine.properties).
     * <br> @version 4.5
     * @type int
     */
    this.ENGINE_START_ERROR_CARDLET_NOT_FOUND = 5;

    /**
     * Cardlet Security Engine Start Error (the engine has detected a problem of security regarding the CPMA within the Secure Element).
     * <br> @version 4.5
     * @type int
     */
    this.ENGINE_START_ERROR_CARDLET_SECURITY = 6;

    /**
     * Illegal Argument Engine Start Error (the requested operation failed because the parameters passed to the Wallet are not valid).
     * <br> @version 4.5
     * @type int
     */
    this.ENGINE_START_ERROR_ILLEGAL_ARGUMENT = 7;

    /**
     * Wallet Not Found Engine Start Error (the requested operation failed because the Wallet is not installed in the device)
     * <br> @version 4.5
     * @type int
     */
    this.ENGINE_START_ERROR_WALLET_NOT_FOUND = 8;

    /**
     * Unregistered Fetaure Engine Start Error (the engine has not been previously recorded in the Wallet)
     * <br> @version 4.5
     * @type int
     */
    this.ENGINE_START_ERROR_UNREGISTERED_FEATURE = 9;

    /**
     * Unknown Payment Error.
     * <br> @version 4.5
     * @type int
     */
    this.PAYMENT_ERROR_UNKNOWN = 0;

    /**
     * Canceled by User Payment Error (payment was canceled by the user).
     * <br> @version 4.5
     * @type int
     */
    this.PAYMENT_ERROR_OPERATION_CANCELED = 1;

    /**
     * Dual Tap Payment Error (error regarding the POS that require a double tap to start a transaction)
     * <br> @version 4.5
     * @type int
     */
    this.PAYMENT_ERROR_DUAL_TAP = 2;

    /**
     * Remove POS Device Payment Error (error regarding the POS that does not disable the NFC antenna once a transaction has been initiated)
     * <br> @version 4.5
     * @type int
     */
    this.PAYMENT_ERROR_REMOVE_DEVICE_FROM_POS = 3;

    /**
     * Illegal Argument Payment Error (the requested operation failed because the parameters passed to the Wallet are not valid.)
     * <br> @version 4.5
     * @type int
     */
    this.PAYMENT_ERROR_ILLEGAL_ARGUMENT = 4;

    /**
     * Wallet Not found Payment Error (the requested operation failed because the Wallet is not installed in the device)
     * <br> @version 4.5
     * @type int
     */
    this.PAYMENT_ERROR_WALLET_NOT_FOUND = 5;

    /**
     * Operation Failed Payment Error (the operation requested on this engine has failed; generic error)
     * <br> @version 4.6
     * @type int
     */
    this.PAYMENT_ERROR_OPERATION_FAILED = 6;

    /**
     * Unregistered Payment Error (error code returned when the Wallet has been uninstalled from the device and has not been invoked again the start engine method)
     * <br> @version 4.6
     * @type int
     */
    this.PAYMENT_ERROR_UNREGISTERED = 7;

    /**
     * Applet Not Found Payment Error (error code returned when the Wallet is not located on the CRS identified the applet from AID)
     * <br> @version 4.6
     * @type int
     */
    this.PAYMENT_ERROR_APPLET_NOT_FOUND = 8;

    /**
     * @event onEngineStartError Fired when there is an error while starting the NFC Payment engine.
     * (after calling the Appverse.NFC.StartNFCPaymentEngine method), 
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 4.5
     * @param {int} error The type of error produced (possible values: {@link Appverse.NFC#ENGINE_START_ERROR_UNKNOWN ENGINE_START_ERROR_UNKNOWN}, {@link Appverse.NFC#ENGINE_START_ERROR_OPERATION_FAILED ENGINE_START_ERROR_OPERATION_FAILED}, {@link Appverse.NFC#ENGINE_START_ERROR_OPERATION_CANCELED ENGINE_START_ERROR_OPERATION_CANCELED}, {@link Appverse.NFC#ENGINE_START_ERROR_ALREADY_STARTED ENGINE_START_ERROR_ALREADY_STARTED}, {@link Appverse.NFC#ENGINE_START_ERROR_OPERATION_ERROR_CARD ENGINE_START_ERROR_OPERATION_ERROR_CARD}, {@link Appverse.NFC#ENGINE_START_ERROR_CARDLET_NOT_FOUND ENGINE_START_ERROR_CARDLET_NOT_FOUND}, {@link Appverse.NFC#ENGINE_START_ERROR_CARDLET_SECURITY ENGINE_START_ERROR_CARDLET_SECURITY}, {@link Appverse.NFC#ENGINE_START_ERROR_ILLEGAL_ARGUMENT ENGINE_START_ERROR_ILLEGAL_ARGUMENT}, {@link Appverse.NFC#ENGINE_START_ERROR_WALLET_NOT_FOUND ENGINE_START_ERROR_WALLET_NOT_FOUND} and {@link Appverse.NFC#ENGINE_START_ERROR_UNREGISTERED_FEATURE ENGINE_START_ERROR_UNREGISTERED_FEATURE}).
     */
    this.onEngineStartError = function(error) {
    };

    /**
     * @event onEngineStartSuccess Fired when the NFC Payment engine has started successfully.
     * (after calling the Appverse.NFC.StartNFCPaymentEngine method), 
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 4.5
     */
    this.onEngineStartSuccess = function() {
    };

    /**
     * @event onPaymentStarted Fired when the NFC Payment has been started successfully.
     * (after calling the Appverse.NFC.StartNFCPayment method), 
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 4.5
     */
    this.onPaymentStarted = function() {
    };

    /**
     * @event onPaymentCanceled Fired when the NFC Payment has been canceled successfully.
     * (after calling the Appverse.NFC.CancelNFCPayment method), 
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 4.5
     */
    this.onPaymentCanceled = function() {
    };

    /**
     * @event onUpdateCountDown Fired during an NFC Payment indicating the remaining seconds.
     * (after calling the Appverse.NFC.StartNFCPayment method).
     * <br> The total timer period is configured in the "timer_period_in_sec" property at the configuration file.
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 4.5
     * @param {int} countDown The value of the remaining seconds before it is turned off with the POS payment
     */
    this.onUpdateCountDown = function(countDown) {
    };

    /**
     * @event onCountDownFinished Fired during an NFC Payment indicating that the payment timer has expired.
     * (after calling the Appverse.NFC.StartNFCPayment method).
     * <br> The total timer period is configured in the "timer_period_in_sec" property at the configuration file.
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 4.5
     */
    this.onCountDownFinished = function() {
    };

    /**
     * @event onPaymentSuccess Fired when the NFC Payment has been completed successfully.
     * (after calling the Appverse.NFC.StartNFCPayment method).
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * @param {Appverse.NFC.NFCPaymentSuccess} paymentSuccess The payment success summary (with the amount, date and time of the just succeeded payment)
     * <br> @version 4.5
     */
    this.onPaymentSuccess = function(paymentSuccess) {
    };

    /**
     * @event onPaymentFailed Fired when the NFC Payment has failed.
     * (after calling the Appverse.NFC.StartNFCPayment method).
     * <br>Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 4.5
     * @param {int} error The type of error produced. 
     * Possible values: 
     * {@link Appverse.NFC#PAYMENT_ERROR_UNKNOWN PAYMENT_ERROR_UNKNOWN}, 
     * {@link Appverse.NFC#PAYMENT_ERROR_OPERATION_CANCELED PAYMENT_ERROR_OPERATION_CANCELED}, 
     * {@link Appverse.NFC#PAYMENT_ERROR_DUAL_TAP PAYMENT_ERROR_DUAL_TAP}, 
     * {@link Appverse.NFC#PAYMENT_ERROR_REMOVE_DEVICE_FROM_POS PAYMENT_ERROR_REMOVE_DEVICE_FROM_POS}, 
     * {@link Appverse.NFC#PAYMENT_ERROR_ILLEGAL_ARGUMENT PAYMENT_ERROR_ILLEGAL_ARGUMENT},
     * and {@link Appverse.NFC#PAYMENT_ERROR_WALLET_NOT_FOUND PAYMENT_ERROR_WALLET_NOT_FOUND}).
     */
    this.onPaymentFailed = function(error) {
    };

}

Appverse.NFC = new NFC();

/**
 * Sets the application NFC parameters ad hoc by using the given properties object.
 * <br> This properties could be settled also through an appropriate configuration file located in /res/raw/nfcpaymentengine.properties at build time.
 * <br> @version 4.5
 * @param {Appverse.NFC.NFCPaymentProperty[]} properties The NFC properties to be settled.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *simulated,only TIMEOUT value is settled and used</pre>
 */
NFC.prototype.SetNFCPaymentProperties = function(properties)
{
    post_to_url_async(Appverse.NFC.serviceName, "SetNFCPaymentProperties", get_params([properties]), null, null);
};

/**
 * Performs security checks (the device does not have root privileges, is not protected by lock and is not in USB debugging mode).
 * <br> If successful, the NFCPaymentEngine starts (success or failure in this starting will be returned asynchronously via JS event listener).
 * <br> @version 4.5
 * @return {Appverse.NFC.NFCPaymentSecurityException} null if everything is ok, otherwise an exception object is received with the error data.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *simulated</pre>
 */
NFC.prototype.StartNFCPaymentEngine = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.NFC.serviceName, "StartNFCPaymentEngine", null, callbackFunctionName, callbackId);
};

/**
 * Stops the NFC payment engine.
 * <br> This should be also called on application destroy (the platform will do it automatically).
 * <br> @version 4.5
 * @return {boolean} true if everything is ok, false otherwise to indicate there were issues stopping the engine.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *simulated</pre>
 */
NFC.prototype.StopNFCPaymentEngine = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.NFC.serviceName, "StopNFCPaymentEngine", null, callbackFunctionName, callbackId);
};

/**
 * Returns the last 4 digits of the Primary Account Number (PAN) from the SIM obfuscating the first 16.
 * <br> It is required to call first the 'Appverse.NFC.StartNFCPaymentEngine' method to get the value, otherwise a null value is returned.
 * <br> @version 4.5
 * @return {String} PAN number obfuscated, null if not available.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *simulated</pre>
 */
NFC.prototype.GetPrimaryAccountNumber = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.NFC.serviceName, "GetPrimaryAccountNumber", null, callbackFunctionName, callbackId);
};

/**
 * Activates an NFC payment with the Point Of Sale (POS).
 * <br> If the NFC is not enabled in the device, the user is redirected to the NFC settings to enable it.
 * <br> It is required to call first the 'Appverse.NFC.StartNFCPaymentEngine' method.
 * <br> @version 4.5
 * @return {boolean} true if everything is ok, false otherwise to indicate there were issues starting the NFC payment.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *simulated</pre>
 */
NFC.prototype.StartNFCPayment = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.NFC.serviceName, "StartNFCPayment", null, callbackFunctionName, callbackId);
};

/**
 * Checks that the device has the NFC interface active.
 * <br> @version 4.5
 * @return {boolean} true if NFC interface is enabled, false otherwise.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *simulated</pre>
 */
NFC.prototype.IsNFCEnabled = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.NFC.serviceName, "IsNFCEnabled", null, callbackFunctionName, callbackId);
};

/**
 * Checks the presence/installation of the Wallet app by the given "packageName".
 * <br> @version 4.5
 * @param {String} packageName The package name of the application that needs to be checked
 * @return {boolean} true if the app is installed on the current device, false otherwise.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *simulated</pre>
 */
NFC.prototype.IsWalletAppInstalled = function(packageName, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.NFC.serviceName, "IsWalletAppInstalled", get_params([packageName]), callbackFunctionName, callbackId);
};

/**
 * Launches the Settings section of the device in which the user can enable the NFC interface.
 * <br> @version 4.5
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *popup</pre>
 */
NFC.prototype.StartNFCSettings = function( )
{
    post_to_url_async(Appverse.NFC.serviceName, "StartNFCSettings", null, null, null);
};

/**
 * Disables any NFC payment and resets the timer.
 * <br> If the transaction is already started at the POS side this method can not finish the payment, but only reset the timer.
 * <br> It is required to call first the 'Appverse.NFC.StartNFCPaymentEngine' method that assigns the appropiate PaymentListener to receive the notifications.
 * <br> @version 4.5
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/error.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *simulated</pre>
 */
NFC.prototype.CancelNFCPayment = function(callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.NFC.serviceName, "CancelNFCPayment", null, callbackFunctionName, callbackId);
};