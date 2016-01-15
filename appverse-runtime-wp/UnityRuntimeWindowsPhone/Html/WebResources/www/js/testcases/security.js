var testKeyPairName1 = {
    "Key": "mykey1"
};
var testKeyPairName2 = {
    "Key": "mykey2"
};
var testKeyPairName3 = {
    "Key": "mykey3"
};

var testKeyPairNames = [testKeyPairName1, testKeyPairName2, testKeyPairName3];
var testKeyPairNamesRemove = [testKeyPairName1.Key, testKeyPairName2.Key, testKeyPairName3.Key];

var testKeyPair1 = {};
testKeyPair1.Key = testKeyPairName1.Key;
testKeyPair1.Value = "myvalue1";

var testKeyPair2 = {};
testKeyPair2.Key = testKeyPairName2.Key;
testKeyPair2.Value = "myvalue2";

var testKeyPair3 = {};
testKeyPair3.Key = testKeyPairName3.Key;
testKeyPair3.Value = "myvalue3";

var testKeyPairs = [testKeyPair1, testKeyPair2, testKeyPair3];

var touchIDReason = "This operation requires Touch ID authentication";

//********** UI COMPONENTS

//********** SECURITY TESTCASES
var TestCase_Security = [Appverse.Security,
	[['IsDeviceModified', ''],
	 ['GetStoredKeyValuePair', '{"param1":' + JSON.stringify(testKeyPairName1) + '}'],
	 ['GetStoredKeyValuePairs', '{"param1":' + JSON.stringify(testKeyPairNames) + '}'],
	 ['StoreKeyValuePair', '{"param1":' + JSON.stringify(testKeyPair1) + '}'],
	 ['StoreKeyValuePairs', '{"param1":' + JSON.stringify(testKeyPairs) + '}'],
	 ['RemoveStoredKeyValuePair', '{"param1":' + JSON.stringify(testKeyPairName1.Key) + '}'],
	 ['RemoveStoredKeyValuePairs', '{"param1":' + JSON.stringify(testKeyPairNamesRemove) + '}']
	 ]];

if (Appverse.is.iOS) {
    TestCase_Security[1].push(['StartLocalAuthenticationWithTouchID', '{"param1":' + JSON.stringify(touchIDReason) + '}']);
}

if (Appverse.is.Android) {
    TestCase_Security[1].push(['IsROMModified', '']);
}

//********** HANDLING CALLBACKS

/**
 * Applications should override/implement this method to be aware of storing of KeyPairs object into the local secure storage, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Security.KeyPair KeyPair}.
 * <br> @version 5.1
 * @method
 * @param {Appverse.Security.KeyPair[]} storedKeyPairs An array of KeyPair objects successfully stored in the device local secure storage.
 * @param {Appverse.Security.KeyPair[]} failedKeyPairs An array of KeyPair objects that could not be successfully stored in the device local secure storage.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Appverse.Security.OnKeyValuePairsStoreCompleted = function (storedKeyPairs, failedKeyPairs) {
    console.log(arguments);
    //Showcase.app.getController('Main').toast("Stored " + (storedKeyPairs?storedKeyPairs.length:0) + " keys; failed: " + (failedKeyPairs?failedKeyPairs.length:0));
    Showcase.app.getController('Main').console(feedObj("Appverse.OnKeyValuePairsStoreCompleted", "Appverse.OnKeyValuePairsStoreCompleted", "Stored " + (storedKeyPairs ? storedKeyPairs.length : 0) + " keys; failed: " + (failedKeyPairs ? failedKeyPairs.length : 0)));
};

/**
 * Applications should override/implement this method to be aware of KeyPair objects found in the device local secure storage, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Security.KeyPair KeyPair}.
 * <br> @version 5.1
 * @method
 * @param {Appverse.Security.KeyPair[]} foundKeyPairs An array of KeyPair objects found in the device local secure storage.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Appverse.Security.OnKeyValuePairsFound = function (foundKeyPairs) {

    console.log(arguments);
    //Showcase.app.getController('Main').toast("Found " + (foundKeyPairs?foundKeyPairs.length:0) + " stored keys");
    Showcase.app.getController('Main').console(feedObj("Appverse.OnKeyValuePairsFound", "Appverse.OnKeyValuePairsFound", "Found " + (foundKeyPairs ? foundKeyPairs.length : 0) + " stored keys"));

    console.dir((foundKeyPairs != null && foundKeyPairs.length > 0) ? foundKeyPairs[0].Value : "null");
};

/**
 * Applications should override/implement this method to be aware of deletion of KeyPairs objects from the local secure storage, and should perform the desired javascript code on this case.
 * <br> For further information see, {@link Appverse.Security.KeyPair KeyPair}.
 * <br> @version 5.1
 * @method
 * @param {Appverse.Security.KeyPair[]} removedKeyPairs An array of KeyPair objects successfully removed from the device local secure storage.
 * @param {Appverse.Security.KeyPair[]} failedKeyPairs An array of KeyPair objects that could not be removed from the device local secure storage.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/check.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/check.png"/></pre>
 */
Appverse.Security.OnKeyValuePairsRemoveCompleted = function (removedKeyPairs, failedKeyPairs) {

    //Showcase.app.getController('Main').toast("Removed " + (removedKeyPairs?removedKeyPairs.length:0) + " stored keys; failed: " + (failedKeyPairs?failedKeyPairs.length:0));
    console.log(arguments);
    Showcase.app.getController('Main').console(feedObj("Appverse.OnKeyValuePairsRemoveCompleted", "Appverse.OnKeyValuePairsRemoveCompleted", "Removed " + (removedKeyPairs ? removedKeyPairs.length : 0) + " stored keys; failed: " + (failedKeyPairs ? failedKeyPairs.length : 0)));
};

/**
 * @event onTouchIDNotAvailable Fired when the app requests a local authentication using Touch ID (biometrics) but this device has not available this feature
 * <br>Method to be overrided by JS applications, to handle this event.
 * @aside guide application_listeners
 * <br> @version 5.0.7
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Appverse.Security.onTouchIDNotAvailable = function () {
    console.log("Appverse.Security.onTouchIDNotAvailable");
    Showcase.app.getController('Main').console(feedObj("Appverse.Security.onTouchIDNotAvailable", "Appverse.Security.onTouchIDNotAvailable", "Touch ID is not available on this device"));
};

/**
 * @event onLocalAuthenticationWithTouchIDReply Fired when the app requests a local authentication using Touch ID (biometrics) and the user completes the authentication action
 * <b> Result could be success or failure
 * <br>Method to be overrided by JS applications, to handle this event.
 * @aside guide application_listeners
 * <br> @version 5.0.7
 * @param {int} status The authentication status, 0 if success. Possible values: Appverse.Security.LA_STATUS_SUCCESS, Appverse.Security.LA_STATUS_RETRY_EXCEEDED, Appverse.Security.LA_STATUS_USER_CANCELED, and Appverse.Security.LA_STATUS_USER_FALLBACK
 * @param {String} errorDescription The error description if authentication failure, null otherwise.
 * <pre> Available in: <br> iOS <img src="resources/images/check.png"/> | android <img src="resources/images/error.png"/> | windows <img src="resources/images/error.png"/> | emulator <img src="resources/images/error.png"/></pre>
 */
Appverse.Security.onLocalAuthenticationWithTouchIDReply = function (status, errorDescription) {
    console.log("Appverse.Security.onLocalAuthenticationWithTouchIDReply, success?: " + status);
    Showcase.app.getController('Main').console(feedObj("Appverse.Security.onLocalAuthenticationWithTouchIDReply", "Appverse.Security.onLocalAuthenticationWithTouchIDReply", status + " - " + errorDescription));
};
