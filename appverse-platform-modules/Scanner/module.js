/*
 * SCANNER INTERFACES
 */

/**
 * @class Appverse.Scanner 
 * Module class to access Appverse Scanner module interface. 
 * <br>This interface provides features to scan different types of codes. Currently the module only implements the QRCode handling.<br>
 * <br> @version 5.0.3
 * <pre>Usage: Appverse.Scanner.&lt;metodName&gt;([params]).<br>Example: Appverse.Scanner.DetectQRCode().</pre>
 * @component
 * @aside guide appverse_modules
 * @constructor Constructs a new Scanner interface.
 * @return {Appverse.Scanner} A new Scanner interface.
 */
Scanner = function() {
    /**
     * @cfg {String}
     * Scanner service name (as configured on Platform Service Locator).
     * <br> @version 5.0.3
     */
    this.serviceName = "scanner";

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
    this.QRTYPE_GEO = 5;

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
     * @event onQRCodeDetected Fired when a QR Code has been read, and its data is returned to the app in order to perform the desired javascript code on this case.
     * <br> For further information see, {@link Appverse.Scanner.MediaQRContent MediaQRContent}.
     * <br> Method to be overrided by JS applications, to handle this event.
     * @aside guide application_listeners
     * <br> @version 3.9
     * @method
     * @param {Appverse.Scanner.MediaQRContent} QRCodeContent The scanned QR Code data read
     * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
     * 
     */
    this.onQRCodeDetected = function(QRCodeContent) {
    };
};

Appverse.Scanner = new Scanner();

/**
 * Fires the camera to detected and process a QRCode image.
 * <br> @version 5.0
 * @param {Boolean} autoHandleQR True value to indicates that the detected QRCode should be handled by the platform (if possible) automatically, or False to just be get data returned.
 * QRCode data is provided via the proper event handled by the "Appverse.Scanner.onQRCodeDetected" method; please, override to handle the event.
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Scanner.prototype.DetectQRCode = function(autoHandleQR)
{
    post_to_url_async(Appverse.Scanner.serviceName, "DetectQRCode", get_params([autoHandleQR]), null, null);
};

/**
 * Handles the given QRCode data to be processed (if possible) by the system. <br/>For further information see, {@link Appverse.Scanner.MediaQRContent MediaQRContent}.
 * <br> The content types that could be processed by the platform are:
 * <br> {@link Appverse.Scanner#QRTYPE_EMAIL_ADDRESS}, {@link Appverse.Scanner#QRTYPE_URI} and {@link Appverse.Scanner#QRTYPE_TEL}.
 * <br> Other types couldn't be processed without pre-parsing, so they are returned to be handled by the application.
 * <br> @version 5.0
 * <br> It returns a {int} with the current QRCode content type.
 * @param {Appverse.Scanner.MediaQRContent} mediaQRContent The QRCode data scanned that needs to be handle.
 * @param {String} callbackFunctionName The name of the callback function to be called when the method response is handled. Arguments of this function are the invocation result object and the invocation callbackId. Defaults to "callback".
 * @param {String} callbackId The id to uniquely identify different callbacks with the same callback function. Defaults to "callbackid".
 * @method
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/> *mock data</pre>
 */
Scanner.prototype.HandleQRCode = function(mediaQRContent, callbackFunctionName, callbackId)
{
    post_to_url_async(Appverse.Scanner.serviceName, "HandleQRCode", get_params([mediaQRContent]), callbackFunctionName, callbackId);
};