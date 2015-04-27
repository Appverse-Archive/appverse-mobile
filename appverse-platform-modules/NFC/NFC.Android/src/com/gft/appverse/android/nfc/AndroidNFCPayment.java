/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://appverse.org/legal/appverse-license/.

 Redistribution and use in  source and binary forms, with or without modification, 
 are permitted provided that the  conditions  of the  AppVerse Public License v2.0 
 are met.

 THIS SOFTWARE IS PROVIDED BY THE  COPYRIGHT HOLDERS  AND CONTRIBUTORS "AS IS" AND
 ANY EXPRESS  OR IMPLIED WARRANTIES, INCLUDING, BUT  NOT LIMITED TO,   THE IMPLIED
 WARRANTIES   OF  MERCHANTABILITY   AND   FITNESS   FOR A PARTICULAR  PURPOSE  ARE
 DISCLAIMED. EXCEPT IN CASE OF WILLFUL MISCONDUCT OR GROSS NEGLIGENCE, IN NO EVENT
 SHALL THE  COPYRIGHT OWNER  OR  CONTRIBUTORS  BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL,  SPECIAL,   EXEMPLARY,  OR CONSEQUENTIAL DAMAGES  (INCLUDING, BUT NOT
 LIMITED TO,  PROCUREMENT OF SUBSTITUTE  GOODS OR SERVICES;  LOSS OF USE, DATA, OR
 PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE) 
 ARISING  IN  ANY WAY OUT  OF THE USE  OF THIS  SOFTWARE,  EVEN  IF ADVISED OF THE 
 POSSIBILITY OF SUCH DAMAGE.
 */
package com.gft.appverse.android.nfc;

import nfc.payment.engine.NFCPaymentEngine;
import nfc.payment.engine.exception.CardException;
import nfc.payment.engine.exception.CardletNotFoundException;
import nfc.payment.engine.exception.CardletSecurityException;
import nfc.payment.engine.listener.OnEngineStartListener;
import nfc.payment.engine.listener.OnPaymentListener;
import nfc.payment.engine.security.SecurityManager;
import nfc.payment.engine.security.exception.ADBEnabledException;
import nfc.payment.engine.security.exception.DeviceRootedException;
import nfc.payment.engine.security.exception.LockDisabledException;
import nfc.payment.engine.settings.NFCManager;
import nfc.payment.engine.settings.PropertiesManager;
import nfc.payment.engine.settings.Utils;
import android.app.Activity;
import android.content.Context;
import android.webkit.WebView;

import com.gft.unity.core.IAppDelegate;
import com.gft.unity.core.json.JSONSerializer;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class AndroidNFCPayment implements INFCPayment, IAppDelegate {

	private static String NFC_PROPERTIES_LOCATION = "/res/raw/nfcpaymentengine.properties";
	private static String NFC_PROPERTIES_NAME = "nfcpaymentengine";
	private static String NFC_PROPERTIES_RAW_TYPE = "raw";
	
	private static final String LOGGER_MODULE = "NFC Payment Module";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);	
	
	private final AndroidNFCPaymentListener nfcPaymentListener;
	private NFCPaymentSecurityException exceptionRaised = null;
	
	private Context context;
	private WebView webView;
	private boolean debuggable;
	
	private String nfcFeaturedUsed;
	
	public AndroidNFCPayment(Context ctx, WebView appView) {
		
		// store context and web view
		context = ctx;
		webView = appView;
		
		nfcPaymentListener = new AndroidNFCPaymentListener(context, webView);
		
		nfcFeaturedUsed = this.checkStringsProperty("Appverse_NFC_featured_used");
		
		if(nfcFeaturedUsed!=null && nfcFeaturedUsed.equalsIgnoreCase("true")) {
			
			LOGGER.logDebug("Initialization", "Loading NFC Payment Engine properties (via config file)");
			
			// Android build process can include custom properties file in the appropriate "raw" folder to override the initial NFC properties
			try {
				int resourceId = ctx.getResources().getIdentifier(
						NFC_PROPERTIES_NAME, NFC_PROPERTIES_RAW_TYPE, context.getPackageName());
				if(resourceId !=0) {
					LOGGER.logDebug("Initialization", "Loading config file from: " + NFC_PROPERTIES_LOCATION);
					PropertiesManager.getInstance().setProperties(context, resourceId);
				} else {
					LOGGER.logDebug("Initialization", "Not found config file at: " + NFC_PROPERTIES_LOCATION);
				}
				
				performSecurityChecks();
				
			} catch (Exception e) {
				LOGGER.logDebug("Initialization",
						"Unhandled exception while loading nfc properties file. Exception message: " + e.getMessage());
			}
		} else {
			LOGGER.logDebug("Initialization", "AndroidNFCPayment # The NFC feature is not used in this app");
		}
	}
	
	private String checkStringsProperty(String propertyName) {
		try {
			int resourceIdentifier = context.getResources().getIdentifier(propertyName, "string",context.getPackageName()); 
			String propertyValue = context.getResources().getString(resourceIdentifier);
			LOGGER.logDebug("checkStringsProperty", "Checking Strings property: " + propertyName + "? " + propertyValue);
			return propertyValue; 
				
		} catch (Exception ex) {
			LOGGER.logDebug("checkStringsProperty", "Checking Strings property. Exception getting value for " + propertyName + ": " + ex.getMessage());
			return null;
		}
	}
	
	/**
	 * Checks the presence/installation of the Wallet app by the given "packageName".
	 * @param packageName The package name of the application that needs to be checked
	 * @return true if installed on the current device, fale otherwise.
	 */
	@Override
	public boolean IsWalletAppInstalled(String packageName) {
		try {
			LOGGER.logDebug("IsWalletAppInstalled", "Checking if wallet app is installed by package name: "+ packageName);
			
			/* Own implementation
			 *
			List<ApplicationInfo> packages;
			Context ctx = AndroidServiceLocator.getContext();
			PackageManager pm = ctx.getPackageManager();
			packages = pm.getInstalledApplications(0);
			for (ApplicationInfo packageInfo : packages) {
				LOG.Log(Module.PLATFORM,"*** TO BE REMOVED TESTING *** installed: " + packageInfo.packageName);
				if (packageInfo.packageName.equals(packageName)) {
					LOG.Log(Module.PLATFORM,"Found. Wallet app installed");
					return true;
				}
					
			}
			*/
			
			/* Using NFC library implementation */
			return Utils.isAppInstalled(context, packageName);
			
		} catch (Exception e) {
			LOGGER.logDebug("IsWalletAppInstalled", "Unhandled exception while checking wallet app installed. Exception message: "
							+ e.getMessage());
		}
		LOGGER.logDebug("IsWalletAppInstalled", "Not Found. Wallet app is not installed by package name: " + packageName);
		return false;
	}


	@Override
	public void SetNFCPaymentProperties(NFCPaymentProperty[] properties) {
		
		LOGGER.logDebug("SetNFCPaymentProperties", "Loading NFC Payment Engine properties (per app demand)");
		
		if(properties!=null) {
			try {
				
				for(NFCPaymentProperty property : properties) {
					
					String key = property.getKey();
					String value = property.getValue();
					NFCPaymentPropertyKey keyEnum =  NFCPaymentPropertyKey.valueOf(key);
					
					switch (keyEnum) {
						case application_id:
							LOGGER.logDebug("SetNFCPaymentProperties", "Setting NFC property - application_id");
							PropertiesManager.getInstance().setApplicationID(value);
							break;
						case application_label:
							LOGGER.logDebug("SetNFCPaymentProperties", "Setting NFC property - application_label");
							PropertiesManager.getInstance().setApplicationLabel(value);
							break;
						case aid_name_length:
							LOGGER.logDebug("SetNFCPaymentProperties", "Setting NFC property - aid_name_length");
							PropertiesManager.getInstance().setAIDNameLength(Integer.parseInt(value));
							break;
						case debug_mode:
							LOGGER.logDebug("SetNFCPaymentProperties", "Setting NFC property - debug_mode");
							PropertiesManager.getInstance().setDebugMode(Boolean.parseBoolean(value));
							break;	
						case vibration_duration_in_msec:
							LOGGER.logDebug("SetNFCPaymentProperties", "Setting NFC property - vibration_duration_in_msec");
							PropertiesManager.getInstance().setVibrationDurationInMSec(Integer.parseInt(value));
							break;	
						case timer_period_in_sec:
							LOGGER.logDebug("SetNFCPaymentProperties", "Setting NFC property - timer_period_in_sec");
							PropertiesManager.getInstance().setTimerDurationInSec(Integer.parseInt(value));
							break;	
						default:
							LOGGER.logDebug("SetNFCPaymentProperties", "Error setting NFC property - not valid key name: " + key);
							break;
					}
					
				}
				
			} catch (Exception e) {
				LOGGER.logDebug("SetNFCPaymentProperties", "Unhandled exception while loading nfc properties on demand. Exception message: " + e.getMessage());
			}
			
		} else {
			LOGGER.logDebug("SetNFCPaymentProperties", "No properties bean object available");
		}
		
	}
	
	/**
	 * 
	 */
	private void performSecurityChecks() {
		exceptionRaised = new NFCPaymentSecurityException();
		String messageError = null;
		try {
			
			if(!debuggable) {
				LOGGER.logDebug("performSecurityChecks", "Perfoming NFC Security Checkings...");
				LOGGER.logDebug("performSecurityChecks", "NFC properties debug_mode = " + PropertiesManager.getInstance().isDebugMode());
				// Checks that the device does not have root privileges, 
				// is not protected by lock 
				// and is not in USB debugging mode
				SecurityManager.performSecurityChecks(context);
				
			} else {
				// 
				LOGGER.logDebug("performSecurityChecks", "Application is in debug mode, we have removed the security checkings for TESTING.");
			}
			
			exceptionRaised = null;
			
		} catch (DeviceRootedException e) {
			messageError = "NFC Security Checkings failed: device is rooted";
			LOGGER.logDebug("performSecurityChecks", messageError);
			exceptionRaised.setType(NFCPaymentSecurityExceptionType.DeviceRooted);
		} catch (LockDisabledException e) {
			messageError = "NFC Security Checkings failed: device is not protected by lock";
			LOGGER.logDebug("performSecurityChecks", messageError);
			exceptionRaised.setType(NFCPaymentSecurityExceptionType.DeviceLockDisabled);
		} catch (ADBEnabledException e) {
			messageError = "NFC Security Checkings failed: device is in USB debugging mode";
			LOGGER.logDebug("performSecurityChecks", messageError);
			exceptionRaised.setType(NFCPaymentSecurityExceptionType.USBDebuggingEnabled);
		} catch (Exception e) {
			messageError = "Unhandled exception while starting payment engine. Exception message: " + e.getMessage();
			LOGGER.logDebug("performSecurityChecks", messageError);
			exceptionRaised.setType(NFCPaymentSecurityExceptionType.Unhandled);
		}
		
		if(exceptionRaised!=null) 
			exceptionRaised.setMessage(messageError);
		else
			LOGGER.logDebug("performSecurityChecks", "NFC Security Checkings PASSED");
	}
	
	/**
	 * 
	 * @param ctx
	 * @return
	 */
	private boolean isUSBDebuggingEnabled(Context ctx) {
		
		LOGGER.logDebug("isUSBDebuggingEnabled", "Checking USB Debugging enabled...");
		if (android.provider.Settings.System.getInt(ctx.getContentResolver(), "adb_enabled", 0) == 1) {
			LOGGER.logDebug("isUSBDebuggingEnabled", "USB Debugging ENABLED");
			return true;
		}
		
		LOGGER.logDebug("isUSBDebuggingEnabled", "USB Debugging DISABLED");
		return false;
	}

	@Override
	public NFCPaymentSecurityException StartNFCPaymentEngine() {
		
		String messageError = "Unhandled exception while starting payment engine";
		NFCPaymentSecurityException securityException = new NFCPaymentSecurityException();
		
		try {
			
			// double check USB debugging enabled (user could enable it during the app lifecycle)
			if(!debuggable && !PropertiesManager.getInstance().isDebugMode() && isUSBDebuggingEnabled(context))  {
				
				LOGGER.logDebug("StartNFCPaymentEngine", "The ADB settings are started. User should disable the USB Debugging");
				//Launches the Settings section of the device in which the user can disable the USB debugging mode
				SecurityManager.startADBSettings(context);
				
				NFCPaymentSecurityException usbDebuggingEnableException = new NFCPaymentSecurityException();
				String messageErrorUSB = "NFC Security Checkings failed: device is in USB debugging mode";
				LOGGER.logDebug("StartNFCPaymentEngine", messageErrorUSB);
				usbDebuggingEnableException.setType(NFCPaymentSecurityExceptionType.USBDebuggingEnabled);
				usbDebuggingEnableException.setMessage(messageErrorUSB);
				return usbDebuggingEnableException;
				
			} else {
				if(exceptionRaised!=null && exceptionRaised.getType() == NFCPaymentSecurityExceptionType.USBDebuggingEnabled)
					exceptionRaised = null; // ADB enabled already checked
			}
			
			// check stored result for initial perform security checks
			if(exceptionRaised==null) {
				
				LOGGER.logDebug("StartNFCPaymentEngine", "The device is secure. The NFC Payment Engine is starting...");
				
				// It is required to runOnUIThread this code
				final Activity activity = (Activity) context;
				Runnable action = new Runnable() {
	
					@Override
					public void run() {
						try {
							// Selects the CPMA and enables the engine to be able to make payments.
							NFCPaymentEngine.getInstance().start(activity, nfcPaymentListener);
						} catch (Exception e) {
							LOGGER.logDebug("StartNFCPaymentEngine", "Unhandled exception while starting payment engine (runOnUiThread). Exception message: " + e.getMessage());
						}
					}
				};
				activity.runOnUiThread(action);
				securityException = null;
			} else {
				LOGGER.logDebug("StartNFCPaymentEngine", "Exception raised on the intial cached performSecurityChecks. Exception type: " + exceptionRaised.getType());
				securityException = exceptionRaised;
			}
			
		} catch (Exception e) {
			messageError = "Unhandled exception while starting payment engine. Exception message: " + e.getMessage();
			LOGGER.logDebug("StartNFCPaymentEngine", messageError);
			securityException.setType(NFCPaymentSecurityExceptionType.Unhandled);
		}
		
		if(securityException!=null) 
			securityException.setMessage(messageError);
		
		return securityException;
	} 
	
	@Override
	public boolean StopNFCPaymentEngine() {
		try {
			LOGGER.logDebug("StopNFCPaymentEngine", "Unregistering Payment Listener...");
			NFCPaymentEngine.getInstance().unregisterPaymentListener(nfcPaymentListener);
			
			LOGGER.logDebug("StopNFCPaymentEngine", "The NFC Payment Engine is being stopped...");
			// Closes the channel of communication with the NFC SIM and ends on the engine. 
			// It 'must be called after the engine start.
			NFCPaymentEngine.getInstance().stop();
			return true;
		} catch (Exception e) {
			LOGGER.logDebug("StopNFCPaymentEngine", "Unhandled exception while stopping payment engine. Exception message: " + e.getMessage());
		}
		return false;
	}
	
	@Override
	public String GetPrimaryAccountNumber() {
		try {
			
			LOGGER.logDebug("GetPrimaryAccountNumber", "Getting the Primary Account Number (PAN)...");
			// Returns the last 4 digits of the Primary Account Number (PAN) from the SIM obfuscating the first 16
			return NFCPaymentEngine.getInstance().getPAN();
			
		} catch (CardException ce) {
			// The exception is raised when no SIM is not detected within the player or there are communication problems with the SIM
			LOGGER.logDebug("GetPrimaryAccountNumber", "CardException while getting PAN (Sim not detected or communication problems). " +
					"Exception message: " + ce.getMessage());
		} catch (CardletNotFoundException cnfe) {
			// The exception is raised when the CPMA is not found within the Secure Element.
			LOGGER.logDebug("GetPrimaryAccountNumber", "CardletNotFoundException while getting PAN (CPMA is not found within the Secure Element). " +
					"Exception message: " + cnfe.getMessage());
		} catch (CardletSecurityException cse) {
			// The exception is raised when errors are detected on security regarding the CPMA within the Secure Element..
			LOGGER.logDebug("GetPrimaryAccountNumber", "CardletSecurityException while getting PAN (security errors regarding CPMA within the Secure Element). " +
					"Exception message: " + cse.getMessage());
		} catch (Exception e) {
			LOGGER.logDebug("GetPrimaryAccountNumber", "Unhandled exception while getting PAN. Exception message: " + e.getMessage());
		}
		return null;
	}
	
	@Override
	public boolean IsNFCEnabled() {
		try {
			// Checks that the device has the NFC interface active
			return NFCManager.isNFCEnabled(context);
		} catch (Exception e) {
			LOGGER.logDebug("IsNFCEnabled", "Unhandled exception while checking NFC enabled. Exception message: " + e.getMessage());
		}
		return false;
	}
	
	@Override
	public void StartNFCSettings() {
		try {
			// Launches the Settings section of the device in which the user can enable the NFC interface
			NFCManager.startNFCSettings(context);
		} catch (Exception e) {
			LOGGER.logDebug("StartNFCSettings", "Unhandled exception while starting NFC settings. Exception message: " + e.getMessage());
		}
	}
	
	@Override
	public boolean StartNFCPayment() {
		boolean result = false;
		try {
			// Checks that the device has the NFC interface active
			if (NFCManager.isNFCEnabled(context)) {
				LOGGER.logDebug("StartNFCPayment", "NFC is enabled. Starting NFC Payment...");
				
				Runnable action = new Runnable() {

					@Override
					public void run() {
						try {
							// Activates an NFC payment with the Point Of Sale (POS)
							NFCPaymentEngine.getInstance().startPayment();
							
						} catch (CardException ce) {
							// The exception is raised when no SIM is not detected within the player or there are communication problems with the SIM
							LOGGER.logDebug("StartNFCPayment", "CardException while starting the NFC payment (Sim not detected or communication problems). " +
									"Exception message: " + ce.getMessage());
						} catch (CardletNotFoundException cnfe) {
							// The exception is raised when the CPMA is not found within the Secure Element.
							LOGGER.logDebug("StartNFCPayment", "CardletNotFoundException while starting the NFC payment (CPMA is not found within the Secure Element). " +
									"Exception message: " + cnfe.getMessage());
						} catch (CardletSecurityException cse) {
							// The exception is raised when errors are detected on security regarding the CPMA within the Secure Element..
							LOGGER.logDebug("StartNFCPayment", "CardletSecurityException while starting the NFC payment (security errors regarding CPMA within the Secure Element). " +
									"Exception message: " + cse.getMessage());
						} catch (Exception e) {
							LOGGER.logDebug("StartNFCPayment", "Unhandled exception while starting payment (runOnUiThread). Exception message: " + e.getMessage());
						}
					}
				};
				((Activity)context).runOnUiThread(action);
				
				result = true;
				
			} else {
				LOGGER.logDebug("StartNFCPayment", "NFC is NOT enabled. Starting NFC Settings...");
				// Launches the Settings section of the device in which the user can enable the NFC interface
				NFCManager.startNFCSettings(context);
			}
		} catch (Exception e) {
			LOGGER.logDebug("StartNFCPayment", "Unhandled exception while starting the NFC payment. Exception message: " + e.getMessage());
		}
		
		return result;
	}
	
	@Override
	public void CancelNFCPayment() {
		Runnable action = new Runnable() {
			@Override
			public void run() {
				try {
					// Disables any NFC payment and resets the timer.
					// If the transaction is already started at the POS side this method can not finish the payment, but only reset the timer.
					// 
					NFCPaymentEngine.getInstance().cancelPayment();
				} catch (CardException ce) {
					// The exception is raised when no SIM is not detected within the player or there are communication problems with the SIM
					LOGGER.logDebug("CancelNFCPayment", "CardException while canceling the NFC payment (Sim not detected or communication problems). " +
							"Exception message: " + ce.getMessage());
					
				} catch (CardletNotFoundException cnfe) {
					// The exception is raised when the CPMA is not found within the Secure Element.
					LOGGER.logDebug("CancelNFCPayment", "CardletNotFoundException while canceling the NFC payment (CPMA is not found within the Secure Element). " +
							"Exception message: " + cnfe.getMessage());
					
				} catch (CardletSecurityException cse) {
					// The exception is raised when errors are detected on security regarding the CPMA within the Secure Element..
					LOGGER.logDebug("CancelNFCPayment", "CardletSecurityException while canceling the NFC payment (security errors regarding CPMA within the Secure Element). " +
							"Exception message: " + cse.getMessage());
					
				} catch (Exception e) {
					LOGGER.logDebug("CancelNFCPayment", "Unhandled exception while canceling the NFC payment. Exception message: " + e.getMessage());
				}
			}	
		};
		((Activity)context).runOnUiThread(action);
	}
	
	
	/**
	 * Listener that implements NFC listeners to catch events related to the start of the engine and the payment process.
	 * @author maps
	 */
	private class AndroidNFCPaymentListener implements OnEngineStartListener, OnPaymentListener {
		
		private Context context;
		private WebView webView;
		
		public AndroidNFCPaymentListener(Context ctx, WebView view) {
			super();
			this.context = ctx;
			this.webView = view;
		}
		
		private void executeJS(Activity main, String method, Object data) {
			 
			if (this.webView != null) {
				String jsonData = "null";
				if(data != null) {
					jsonData = JSONSerializer.serialize(data);
				}
				String jsCallbackFunction = "javascript:if(" + method + "){" + method + "("
						+ jsonData + ");}";

				main.runOnUiThread(new AAMExecuteJS(this.webView, jsCallbackFunction));
			}

		}
		
		private class AAMExecuteJS implements Runnable {

			private String javascript;
			private WebView view;
			

			public AAMExecuteJS(WebView view, String javascript) {
				this.javascript = javascript;
				this.view = view;
			}

			@Override
			public void run() {
				if(this.view != null) {
					this.view.loadUrl(this.javascript);
				}
			}
		}

		@Override
		public void onEngineStartError(int errorCode) {
			try {
				LOGGER.logDebug("onEngineStartError", "The NFC Payment Engine could not be started due to an error. Code: " + errorCode);
				
				NFCPaymentEngineStartError error = NFCPaymentEngineStartError.Unknown;
				boolean operationOK = false;
				switch(errorCode) {
					case OnEngineStartListener.OPERATION_FAILED:
						error = NFCPaymentEngineStartError.OperationFailed;
						break;
					case OnEngineStartListener.OPERATION_CANCELED_BY_USER:
						error = NFCPaymentEngineStartError.OperationCancelledByUser;
						break;
					case OnEngineStartListener.ENGINE_ALREADY_STARTED:
						error = NFCPaymentEngineStartError.EngineAlreadyStarted;
						break;
					case OnEngineStartListener.OPERATION_ERROR_CARD:
						error = NFCPaymentEngineStartError.OperationErrorCard;
						break;
					case OnEngineStartListener.OPERATION_ERROR_CARDLET_NOT_FOUND:
						error = NFCPaymentEngineStartError.CardletNotFound;
						break;
					case OnEngineStartListener.OPERATION_ERROR_CARDLET_SECURITY:
						error = NFCPaymentEngineStartError.CardletSecurity;
						break;
					case OnEngineStartListener.OPERATION_ERROR_ILLEGAL_ARGUMENT:
						error = NFCPaymentEngineStartError.IllegalArgument;
						break;
					case OnEngineStartListener.WALLET_NOT_FOUND:
						error = NFCPaymentEngineStartError.WalletNotFound;
						break;
					case OnEngineStartListener.OPERATION_ERROR_UNREGISTERED_FEATURE:
						error = NFCPaymentEngineStartError.UnregisteredFeature;
						break;
					case OnEngineStartListener.OPERATION_OK:
						LOGGER.logDebug("onEngineStartError", "**** WARNING: called onEngineStartError() method but received error code OPERATION_OK");
						operationOK = true;
						break;
				}
				
				if(!operationOK) {
					this.executeJS((Activity) context, "Appverse.NFC.onEngineStartError", error);
				} else {
					LOGGER.logDebug("onEngineStartError", "**** WARNING: abnormal situation, not calling Appverse.NFC.onEngineStartError listener as we received an OPERATION_OK. This case should not happen");
				}
			} catch (Exception e) {
				LOGGER.logDebug("onEngineStartError", "Unhandled exception while processing the engine starting failure. Exception message: " + e.getMessage());
			}
		}

		@Override
		public void onEngineStartSuccess() {
			
			try {
				LOGGER.logDebug("onEngineStartSuccess", "The NFC Payment Engine has been successfully started.");
				
				this.executeJS((Activity) context, "Appverse.NFC.onEngineStartSuccess", null);
				
				LOGGER.logDebug("onEngineStartSuccess", "Registering Payment Listener...");
				NFCPaymentEngine.getInstance().registerPaymentListener(this);
			} catch (Exception e) {
				LOGGER.logDebug("onEngineStartSuccess", "Unhandled exception while processing the engine starting success. Exception message: " + e.getMessage());
			}
			
		}

		@Override
		public void onPaymentCanceled() {
			try {
				LOGGER.logDebug("onPaymentCanceled", "The NFC payment has been canceled.");
				
				this.executeJS((Activity) context, "Appverse.NFC.onPaymentCanceled", null);
			} catch (Exception e) {
				LOGGER.logDebug("onPaymentCanceled", "Unhandled exception while processing the payment canceled event. Exception message: " + e.getMessage());
			}
		}

		@Override
		public void onPaymentStarted() {
			try {
				LOGGER.logDebug("onPaymentStarted", "The NFC payment has been successfully started.");
				
				this.executeJS((Activity) context, "Appverse.NFC.onPaymentStarted", null);
			} catch (Exception e) {
				LOGGER.logDebug("onPaymentStarted", "Unhandled exception while processing the payment started event. Exception message: " + e.getMessage());
			}
		}

		@Override
		public void onPaymentSuccess(String amount, String date, String time) {
			
			try {
				LOGGER.logDebug("onPaymentSuccess", "The NFC payment has been completed with success.");
				
				NFCPaymentSuccess paymentSuccess = new NFCPaymentSuccess();
				paymentSuccess.setAmount(amount);
				paymentSuccess.setDate(date);
				paymentSuccess.setTime(time);
				
				this.executeJS((Activity) context, "Appverse.NFC.onPaymentSuccess", paymentSuccess);
		        
			} catch (Exception e) {
				LOGGER.logDebug("onPaymentSuccess", "Unhandled exception while processing the payment success event. Exception message: " + e.getMessage());
			}
			
		}

		@Override
		public void onUpdateCountDown(int countDown) {
			try {
				LOGGER.logDebug("onUpdateCountDown", "The NFC payment countdown remains " + countDown +" seconds.");
				
				// returns the value of the remaining seconds before it is turned off with the POS payment
				this.executeJS((Activity) context, "Appverse.NFC.onUpdateCountDown", countDown);
			} catch (Exception e) {
				LOGGER.logDebug("onUpdateCountDown", "Unhandled exception while processing the count down updated event. Exception message: " + e.getMessage());
			}
			
		}

		@Override
		public void onCountDownFinished() {
			try {
				LOGGER.logDebug("onCountDownFinished", "The NFC payment countdown has been finished.");
				
				this.executeJS((Activity) context, "Appverse.NFC.onCountDownFinished", null);
			} catch (Exception e) {
				LOGGER.logDebug("onCountDownFinished", "Unhandled exception while processing the count down finished event. Exception message: " + e.getMessage());
			}
			
		}
		
		@Override
		public void onTapToPay() {
			// TODO
		}

		@Override
		public void onPaymentFailed(int errorCode) {
			try {
				LOGGER.logDebug("onPaymentFailed", "The NFC Payment Engine could not be started due to an error. Code: " + errorCode);
				
				NFCPaymentError error = NFCPaymentError.Unknown;
				boolean operationOK = false;
				switch(errorCode) {
					case OnPaymentListener.OPERATION_FAILED:  // 0
						error = NFCPaymentError.OperationFailed;
						break;
					case OnPaymentListener.OPERATION_CANCELED_BY_USER: // -1
						error = NFCPaymentError.OperationCancelledByUser;
						break;
					case OnPaymentListener.ERROR_DUAL_TAP: // -7
						error = NFCPaymentError.DualTap;
						break;
					case OnPaymentListener.ERROR_REMOVE_DEVICE_FROM_POS: // -6
						error = NFCPaymentError.RemoveDeviceFromPOS;
						break;
					case OnPaymentListener.OPERATION_ERROR_ILLEGAL_ARGUMENT: // -2
						error = NFCPaymentError.IllegalArgument;
						break;
					case OnPaymentListener.WALLET_NOT_FOUND: // -3
						error = NFCPaymentError.WalletNotFound;
						break;
					case OnPaymentListener.OPERATION_ERROR_UNREGISTERED:  // -4
						error = NFCPaymentError.Unregistered;
						break;
					case OnPaymentListener.OPERATION_ERROR_APPLET_NOT_FOUND:  // -5
						error = NFCPaymentError.AppletNotFound;
						break;
					case OnPaymentListener.OPERATION_OK:  // 1
						LOGGER.logDebug("onPaymentFailed", "**** WARNING: called onPaymentFailed() method but received error code OPERATION_OK");
						operationOK = true;
						break;
				}
				if(!operationOK) {
					LOGGER.logDebug("onPaymentFailed", "Calling Appverse.NFC.onPaymentFailed listener...");
					this.executeJS((Activity) context, "Appverse.NFC.onPaymentFailed", error);
				} else {
					LOGGER.logDebug("onPaymentFailed", "**** WARNING: abnormal situation, not calling Appverse.NFC.onPaymentFailed listener as we received an OPERATION_OK. This case should not happen");
				}
			} catch (Exception e) {
				LOGGER.logDebug("onPaymentFailed", "Unhandled exception while processing the payment failure. Exception message: " + e.getMessage());
			}
			
		}
		
		
	}


	/* (non-Javadoc)
	 * @see com.gft.unity.core.IAppDelegate#buildMode(boolean)
	 */
	@Override
	public void buildMode(boolean isDebugBuild) {
		this.debuggable = isDebugBuild;
	}

	/* (non-Javadoc)
	 * @see com.gft.unity.core.IAppDelegate#onDestroy()
	 */
	@Override
	public void onDestroy() {
		try {
			LOGGER.logDebug("onDestroy", "event received");
			if(nfcFeaturedUsed!=null && nfcFeaturedUsed.equalsIgnoreCase("true")) {
				LOGGER.logDebug("onDestroy", "Application is destroyed. NFC feature is used in this app, stopping NFC engine...");
				// Stop any NFC payment engine if started
				boolean serverStopped = this.StopNFCPaymentEngine();
				LOGGER.logDebug("onDestroy", "NFC Payment engine stopped? " + serverStopped);
				
			} else {
				LOGGER.logDebug("onDestroy", "The NFC feature is not used in this app");
			}
		} catch (Exception e) {
			LOGGER.logDebug("onDestroy", "Exception checking NFC feature enabled. Exception: " + e.getMessage());
		}
		
	}

	/* (non-Javadoc)
	 * @see com.gft.unity.core.IAppDelegate#onPause()
	 */
	@Override
	public void onPause() {
		// TODO Auto-generated method stub
		
	}

	/* (non-Javadoc)
	 * @see com.gft.unity.core.IAppDelegate#onResume()
	 */
	@Override
	public void onResume() {
		// TODO Auto-generated method stub
		
	}

	/* (non-Javadoc)
	 * @see com.gft.unity.core.IAppDelegate#onStop()
	 */
	@Override
	public void onStop() {
		// TODO Auto-generated method stub
		
	}
	
	
	
	
}
