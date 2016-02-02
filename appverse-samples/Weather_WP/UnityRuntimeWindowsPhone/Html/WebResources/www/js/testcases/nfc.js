
var nfcProperties = [];
if(Appverse.NFC){
nfcProperties[0] = {};
nfcProperties[0].Key = Appverse.NFC.PROPERTY_TIMER_PERIOD;
nfcProperties[0].Value = "20";  // in seconds
nfcProperties[1] = {};
nfcProperties[1].Key = Appverse.NFC.PROPERTY_APPLICATION_ID;
nfcProperties[1].Value = "a0000000041010bb43495042b1000901";  // should be in lowercase
nfcProperties[2] = {};
nfcProperties[2].Key = Appverse.NFC.PROPERTY_APPLICATION_LABEL;
nfcProperties[2].Value = "MASTERCARD";
}
var timWalletPackageName = "com.telecomitalia.paymentintentconsole";

//********** UI COMPONENTS

//********** MEDIA TESTCASES
var TestCase_NFC = [Appverse.NFC,
	[['StartNFCPaymentEngine',''],
	 ['GetPrimaryAccountNumber',''],
	 ['StartNFCPayment',''],
	 ['CancelNFCPayment',''],
	 ['StopNFCPaymentEngine',''],
	 ['SetNFCPaymentProperties','{"param1":' + JSON.stringify(nfcProperties) +'}'],
	 ['IsNFCEnabled',''],
	 ['IsWalletAppInstalled','{"param1":' + JSON.stringify(timWalletPackageName) +'}'],
	 ['StartNFCSettings', '']
	]];	

	
//********** HANDLING CALLBACKS

if(Appverse.NFC){
	
	var engineStartErrorCodes = {};
	engineStartErrorCodes[Appverse.NFC.ENGINE_START_ERROR_UNKNOWN] = "Unknown Error";
	engineStartErrorCodes[Appverse.NFC.ENGINE_START_ERROR_OPERATION_FAILED] = "Operation Failed";
	engineStartErrorCodes[Appverse.NFC.ENGINE_START_ERROR_OPERATION_CANCELED] = "Operation Canceled by user";
	engineStartErrorCodes[Appverse.NFC.ENGINE_START_ERROR_ALREADY_STARTED] = "Payment Engine already started";
	engineStartErrorCodes[Appverse.NFC.ENGINE_START_ERROR_OPERATION_ERROR_CARD] = "Communication problem with the SIM";
	engineStartErrorCodes[Appverse.NFC.ENGINE_START_ERROR_CARDLET_NOT_FOUND] = "Cardlet not found";
	engineStartErrorCodes[Appverse.NFC.ENGINE_START_ERROR_CARDLET_SECURITY] = "Cardlet security issue";
	engineStartErrorCodes[Appverse.NFC.ENGINE_START_ERROR_ILLEGAL_ARGUMENT] = "Invalid parameters passed to the Wallet";
	engineStartErrorCodes[Appverse.NFC.ENGINE_START_ERROR_WALLET_NOT_FOUND] = "Wallet not found";
	engineStartErrorCodes[Appverse.NFC.ENGINE_START_ERROR_UNREGISTERED_FEATURE] = "Not previously recorded in the Wallet";

	var paymentErrorCodes = {};
	paymentErrorCodes[Appverse.NFC.PAYMENT_ERROR_UNKNOWN] = "Unknown Error";
	paymentErrorCodes[Appverse.NFC.PAYMENT_ERROR_OPERATION_CANCELED] = "Operation Canceled by the user";
	paymentErrorCodes[Appverse.NFC.PAYMENT_ERROR_DUAL_TAP] = "POS dual tap error";
	paymentErrorCodes[Appverse.NFC.PAYMENT_ERROR_REMOVE_DEVICE_FROM_POS] = "POS NFC antenna disabled during transaction";
	paymentErrorCodes[Appverse.NFC.PAYMENT_ERROR_ILLEGAL_ARGUMENT] = "Invalid parameters passed to the Wallet";
	paymentErrorCodes[Appverse.NFC.PAYMENT_ERROR_WALLET_NOT_FOUND] = "Wallet not installed";

	/**
	 * Fired when there is an error while starting the NFC Payment engine.
	 */
	Appverse.NFC.onEngineStartError = function(error){
		//Showcase.app.getController('Main').toast("NFC Engine Start", false);
			Showcase.app.getController('Main').console(feedObj("Appverse.NFC.onEngineStartError","Appverse.NFC.onEngineStartError",engineStartErrorCodes[error]));
			submitCallback(engineStartErrorCodes[error], "Appverse.NFC.onEngineStartError");
	};

	/**
	 * Fired when the NFC Payment engine has started successfully.
	 */
	Appverse.NFC.onEngineStartSuccess = function(){
		//Showcase.app.getController('Main').toast("NFC Engine Start", false);
			Showcase.app.getController('Main').console(feedObj("Appverse.NFC.onEngineStartSuccess","Appverse.NFC.onEngineStartSuccess","Success"));
			submitCallback("Success", "Appverse.NFC.onEngineStartSuccess");
	};

	/**
	 * Fired when the NFC Payment has been started successfully.
	 */
	Appverse.NFC.onPaymentStarted = function(){
		//Showcase.app.getController('Main').toast("NFC Payment",  false);
			Showcase.app.getController('Main').console(feedObj("Appverse.NFC.onPaymentStarted","Appverse.NFC.onPaymentStarted","Started"));
			submitCallback("Started", "Appverse.NFC.onPaymentStarted");
	};

	/**
	 * Fired when the NFC Payment has been canceled successfully.
	 */
	Appverse.NFC.onPaymentCanceled = function(){
		//Showcase.app.getController('Main').toast("NFC Payment", "Canceled");
			Showcase.app.getController('Main').console(feedObj("Appverse.NFC.onPaymentCanceled","Appverse.NFC.onPaymentCanceled","Canceled"));
	};

	/**
	 * Fired when the NFC Payment has failed due to an error.
	 */
	Appverse.NFC.onPaymentFailed = function(error){
		//Showcase.app.getController('Main').toast("NFC Payment Failed", false);
			Showcase.app.getController('Main').console(feedObj("Appverse.NFC.onPaymentFailed","Appverse.NFC.onPaymentFailed",paymentErrorCodes[error]));
			submitCallback(paymentErrorCodes[error], "Appverse.NFC.onPaymentFailed");
	};

	/**
	 * Fired during an NFC Payment indicating the remaining seconds.
	 */
	Appverse.NFC.onUpdateCountDown = function(countDown){
		//Showcase.app.getController('Main').toast("NFC Payment", "Count down (seconds): " + countDown, true);
			Showcase.app.getController('Main').console(feedObj("Appverse.NFC.onUpdateCountDown","Appverse.NFC.onUpdateCountDown","Count down (remaining seconds): " + countDown));
		console.log("NFC Payment : Count down (remaining seconds): " + countDown);
	};

	/**
	 * Fired during an NFC Payment indicating the finish of the count down for the payment.
	 */
	Appverse.NFC.onCountDownFinished = function(){
		//Showcase.app.getController('Main').toast("NFC Payment - Count down finished!", false);
			Showcase.app.getController('Main').console(feedObj("Appverse.NFC.onCountDownFinished","Appverse.NFC.onCountDownFinished","Count down finished!"));
		console.log("NFC Payment : Count down finished!");
	};

	/**
	 * Fired when the NFC Payment has been completed successfully.
	 */
	Appverse.NFC.onPaymentSuccess = function(paymentSuccess){
		//Showcase.app.getController('Main').toast("NFC Payment Success", false);
			Showcase.app.getController('Main').console(feedObj("Appverse.NFC.onPaymentSuccess","Appverse.NFC.onPaymentSuccess",paymentSuccess));
			submitCallback(paymentSuccess, "Appverse.NFC.onPaymentSuccess");
	};
}