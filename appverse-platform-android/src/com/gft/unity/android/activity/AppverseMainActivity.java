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
package com.gft.unity.android.activity;


import java.util.ArrayList;
import java.util.Collections;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Properties;
import java.util.Set;

import android.app.Activity;
import android.app.NotificationManager;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.ResultReceiver;
import android.view.Window;
import android.view.WindowManager;
import com.gft.unity.android.log.AndroidLoggerDelegate;
import com.gft.unity.android.notification.LocalNotificationReceiver;
import com.gft.unity.android.notification.NotificationUtils;
import com.gft.unity.android.notification.RemoteNotificationIntentService;
import com.gft.unity.android.server.AndroidNetworkReceiver;
import com.gft.unity.android.server.HttpServer;
import com.gft.unity.android.server.ProxySettings;
import com.gft.unity.android.AndroidApplicationEvent;
import com.gft.unity.android.AndroidI18N;
import com.gft.unity.android.AndroidIO;
import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.AndroidSystem;
import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.core.IAppDelegate;
import com.gft.unity.core.i18n.Locale;
import com.gft.unity.core.i18n.ResourceLiteralDictionary;
import com.gft.unity.core.io.IOService;
import com.gft.unity.core.json.JSONSerializer;
import com.gft.unity.core.net.NetworkType;
import com.gft.unity.core.notification.NotificationData;
import com.gft.unity.core.security.ISecurity;
import com.gft.unity.core.system.HardwareInfo;
import com.gft.unity.core.system.OSInfo;
import com.gft.unity.core.system.SystemLogger.Module;
import com.gft.unity.core.system.UnityContext;
import com.gft.unity.core.system.launch.LaunchData;
import com.gft.unity.core.system.log.LogManager;

public abstract class AppverseMainActivity extends Activity {

	protected static final AndroidSystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();

	protected static final String SERVER_PROPERTIES = "Settings.bundle/Root.properties";
	protected static final String SERVER_PORT_PROPERTY = "IPC_DefaultPort";
	
	
	private boolean securityChecksPerfomed = false;
	private boolean securityChecksPassed = false;
	protected static final String DEFAULT_LOCKED_ROOTED_HTML = "file:///android_asset/app/config/error_rooted.html";
	protected static final String DEFAULT_LOCKED_ROM_MODIFIED_HTML = "file:///android_asset/app/config/error_rom_modified.html";
	
	protected static final String WEBVIEW_MAIN_URL = AndroidServiceLocator.APPVERSE_URI + "WebResources/www/index.html";
	protected static final String LEGACY_WEBVIEW_MAIN_URL = AndroidServiceLocator.INTERNAL_SERVER_URL + "/WebResources/www/index.html";
	
	protected boolean hasSplash = false;
	// private boolean splashShownOnBackground = false;
	protected AndroidActivityManager activityManager = null;
	protected boolean holdSplashScreenOnStartup = false;
	
	
	/*private boolean disableThumbnails = false;
	private boolean blockRooted = false;
	private boolean blockROMModified = false;*/
	
	protected HttpServer server = null;
	protected Properties serverProperties;
	protected int serverPort;

	private Bundle lastIntentExtras = null;
	private Uri lastIntentData = null;

	public static final String PREFS_NAME = "IntentState";
	SharedPreferences settings;
	
	
	/** METHODS TO NEEDED TO INCLUDE MODULES **/
	
	protected IActivityManager getActivityManager() {
		return this.activityManager;
	}
	
	protected Context getContext() {
		return this;
	}
	
	protected void RegisterService(Object service, String key) {
		try {
			LOG.LogDebug(Module.GUI, "************* REGISTERING SERVICE: " + key);
			((AndroidServiceLocator) AndroidServiceLocator.GetInstance()).RegisterService(service, key);
		} catch (Exception ex) {
			LOG.LogDebug(Module.GUI, "************* Exception registering service [" + key + "], exception message: " +ex.getMessage());
		}
	}
	
	protected void RegisterResultReceiver(int[] resultCodes, ResultReceiver resultReceiver) {
		try {
			((AndroidServiceLocator) AndroidServiceLocator.GetInstance()).RegisterResultReceiver(resultCodes, resultReceiver);
		} catch (Exception ex) {
			LOG.LogDebug(Module.GUI, "************* Exception registering result receiver, exception message: " +ex.getMessage());
		}
	}
	
	/** ACTIVITY OVERRIDEN METHODS **/
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		LOG.Log(Module.GUI, "onCreate");

		// GUI initialization code
		getWindow().requestFeature(Window.FEATURE_NO_TITLE);
		getWindow().setFlags(
				WindowManager.LayoutParams.FLAG_FORCE_NOT_FULLSCREEN,
				WindowManager.LayoutParams.FLAG_FORCE_NOT_FULLSCREEN);

		/*disableThumbnails = checkUnityProperty("Unity_DisableThumbnails");
		blockRooted = checkUnityProperty("Appverse_BlockRooted");
		blockROMModified = checkUnityProperty("Appverse_BlockROMModified");*/

		// security reasons; don't allow screen shots while this window is displayed
		/* not valid for builds under level 14 */
		//TODO DISABLETHUMBNAILS_1
		//@@DISABLETHUMBNAILS_1@@











		
		
		// Initialize Webview component and provide specific settings
		this.initialiazeWebViewSettings();
		
		// create the application logger
		LogManager.setDelegate(new AndroidLoggerDelegate());

		// initialize the service locator
		activityManager = initialiazeActivityManager(); 

		// save the context for further access
		AndroidServiceLocator.setContext(this, activityManager);

		// killing previous background processes from the same package
		activityManager.killBackgroundProcesses();

		AndroidServiceLocator serviceLocator = (AndroidServiceLocator) AndroidServiceLocator
				.GetInstance();
		serviceLocator.RegisterService(this.getAssets(),
				AndroidServiceLocator.SERVICE_ANDROID_ASSET_MANAGER);
		serviceLocator.RegisterService(activityManager,
				AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		
		// registering Appverse modules (if any)
		this.registerModulesServices();

		// initialize config data files for app delegates
		serviceLocator.initConfigDataForServices();
		
		if(Build.VERSION.SDK_INT >= 17){   // Only used for JELLY_BEAN_MR1 or higher
			/*** INJECT SCRIPT MESSAGE HANDLER (new in Appverse 5) ***/
			
			/** From Android documentation:
			 * 
			 * This method can be used to allow JavaScript to control the host application. 
			 * This is a powerful feature, but also presents a security risk for apps targeting JELLY_BEAN or earlier. 
			 * Apps that target a version later than JELLY_BEAN are still vulnerable if the app runs on a device running Android earlier than 4.2. 
			 * The most secure way to use this method is to target JELLY_BEAN_MR1 and to ensure the method is called only when running on Android 4.2 or later. 
			 * With these older versions, JavaScript could use reflection to access an injected object's public fields. 
			 * Use of this method in a WebView containing untrusted content could allow an attacker to manipulate the host application in unintended ways, 
			 * executing Java code with the permissions of the host application. Use extreme care when using this method in a WebView which could contain untrusted content.
			 
			 * JavaScript interacts with Java object on a private, background thread of this WebView. Care is therefore required to maintain thread safety.
			 * The Java object's fields are not accessible.
			 * For applications targeted to API level LOLLIPOP and above, methods of injected Java objects are enumerable from JavaScript.
			 * 
			 */
			
			this.addJavascriptIntefaceToWebView(serviceLocator, "appverseJSBridge");
		}
		
		if(performSecurityChecks(serviceLocator))  {
			
			LOG.Log(Module.GUI, "Security checks passed... initializing Appverse...");
			
	
			startServer();
	
			
			 /* THIS COULD NOT BE CHECKED ON API LEVEL < 11; NO suchmethodexception
			 * boolean hwAccelerated = appView.isHardwareAccelerated();
			 * if(hwAccelerated)
			 * LOG.Log(Module.GUI,"Application View is HARDWARE ACCELERATED"); else
			 * LOG.Log(Module.GUI,"Application View is NOT hardware accelerated");
			 */
	
			final IntentFilter actionFilter = new IntentFilter();
			actionFilter
					.addAction(android.net.ConnectivityManager.CONNECTIVITY_ACTION);
			// actionFilter.addAction("android.intent.action.SERVICE_STATE");
			registerReceiver(this.initialiazeNetworkReceiver(), actionFilter);
	
			
			ConnectivityManager conMan = (ConnectivityManager) this
	                .getSystemService(Context.CONNECTIVITY_SERVICE);
	        NetworkInfo networkInfo = conMan.getActiveNetworkInfo();
	        com.gft.unity.core.net.NetworkType type = NetworkType.Unknown;
	        if(networkInfo != null){
	        	boolean isWiFi = networkInfo.getType() == ConnectivityManager.TYPE_WIFI;
	            boolean isMobile = networkInfo.getType() == ConnectivityManager.TYPE_MOBILE;
	    		boolean isConnected = networkInfo.isConnected();
	    		if (isWiFi) {
	                if (isConnected) {
	                	LOG.Log(Module.GUI, "Wi-Fi - CONNECTED ("+networkInfo.getType()+")");
	                    type = NetworkType.Wifi;
	                } else {
	                	LOG.Log(Module.GUI, "Wi-Fi - DISCONNECTED ("+networkInfo.getType()+")");
	                    
	                }
	            } else if (isMobile) {
	                if (isConnected) {
	                	LOG.Log(Module.GUI, "Mobile - CONNECTED ("+networkInfo.getType()+")");
	                    type = NetworkType.Carrier_3G;
	                } else {
	                	LOG.Log(Module.GUI, "Mobile - DISCONNECTED ("+networkInfo.getType()+")");
	                }
	            } else {
	                if (isConnected) {
	                	LOG.Log(Module.GUI, networkInfo.getTypeName() + " - CONNECTED ("+networkInfo.getType()+")");
	                } else {
	                	LOG.Log(Module.GUI, networkInfo.getTypeName() + " - DISCONNECTED ("+networkInfo.getType()+")");
	                }
	            }
	        } else {
	        	LOG.Log(Module.GUI, "DISCONNECTED");
	        }
	        	        
			final int typeOrdinal = type.ordinal();
			final Activity currentContext = this;
			new Thread(new Runnable() {
				public void run() {
					currentContext.runOnUiThread(new Runnable() {
						public void run() {

							InitializeAppverseContext(typeOrdinal);
							String networkStatusListener = "javascript:try{if(Appverse&&Appverse.Net){Appverse.Net.NetworkStatus = " + typeOrdinal + ";Appverse.Net.onConnectivityChange(Appverse.Net.NetworkStatus);}else{console.log('Appverse is not defined');}}catch(e){console.log('Error setting network status (please check onConnectivityChange method): '+e);}";
							queueJSStatementsForWebviewClient(networkStatusListener);
							loadMainURLIntoWebview();
						}
					});
				}
			}).start();
		}
		
		holdSplashScreenOnStartup = checkUnityProperty("Unity_HoldSplashScreenOnStartup");
		this.showSplashScreen();
		
		RemoteNotificationIntentService.loadNotificationOptions(getResources(), this.activityManager, this);
		LocalNotificationReceiver.initialize(this.activityManager, this);
		
		// notify app delegates about the onCreate event
		((AndroidServiceLocator)AndroidServiceLocator.GetInstance()).sendApplicationEvent(AndroidApplicationEvent.onCreate);
	}
	
	
	@Override
	public boolean onCreateThumbnail(Bitmap outBitmap, Canvas canvas) {
		LOG.Log(Module.GUI, "onCreateThumbnail");
		//TODO DISABLETHUMBNAILS_2
		//@@DISABLETHUMBNAILS_2@@










		
		
		return super.onCreateThumbnail(outBitmap, canvas);
	}

	@Override
	public void onWindowFocusChanged(boolean hasFocus) {
		LOG.Log(Module.GUI, "onWindowFocusChanged");
		if (hasFocus) {
			LOG.Log(Module.GUI,
					"application has focus; calling foreground listener");
			this.activityManager.loadUrlIntoWebView("javascript:try{Appverse._toForeground()}catch(e){}");

			// check for notification details or other extra data
			this.checkLaunchedFromNotificationOrExternaly();

		} else {
			if (!activityManager.isNotifyLoadingVisible()) {
				LOG.Log(Module.GUI,
						"application lost focus; calling background listener");
				this.activityManager.loadUrlIntoWebView("javascript:try{Appverse._toBackground()}catch(e){}");
				
			} else {
				LOG.Log(Module.GUI,
						"application lost focus due to a showing dialog (StartNotifyLoading feature); application is NOT calling background listener to allow platform calls on the meantime.");
			}
			/*
			 * if (server == null) { // security reasons; the splash screen is
			 * shown when application enters in background (hiding sensitive
			 * data) // it will be dismissed "onResume" method
			 * if(!splashShownOnBackground) { splashShownOnBackground =
			 * activityManager.showSplashScreen(appView); } }
			 */
		}

	}

	@Override
	protected void onPause() {
		LOG.Log(Module.GUI, "onPause");
		
		((AndroidServiceLocator)AndroidServiceLocator.GetInstance()).sendApplicationEvent(AndroidApplicationEvent.onPause);

		// Stop HTTP server, and send to background later
		stopServer(true);
		super.onPause();
	}

	@Override
	protected void onResume() {
		super.onResume();

		if (ProxySettings.checkSystemProxyProperties()) {
			ProxySettings.shouldSetProxySetting = true;
		}

		// Save the context for further access
		AndroidServiceLocator.setContext(this, activityManager);
		NotificationManager nMngr = (NotificationManager) this
				.getSystemService(Context.NOTIFICATION_SERVICE);
		nMngr.cancelAll();
		LOG.Log(Module.GUI, "onResume");

		/*
		 * // security reasons if(splashShownOnBackground) {
		 * activityManager.dismissSplashScreen(); splashShownOnBackground =
		 * false; }
		 */
		
		if(!performSecurityChecks((AndroidServiceLocator) AndroidServiceLocator.GetInstance())) return;

		LOG.Log(Module.GUI, "Security checks passed... beaking up Appverse...");
		
		((AndroidServiceLocator)AndroidServiceLocator.GetInstance()).sendApplicationEvent(AndroidApplicationEvent.onResume);

		// Start HTTP server
		startServer();

		this.activityManager.loadUrlIntoWebView("javascript:try{Appverse._toForeground()}catch(e){}");
			
		// TESTING getExtras();

		if (this.getIntent() != null) {

			LOG.Log(Module.GUI, "Processing intent data and extras... ");

			this.lastIntentExtras = this.getIntent().getExtras();
			Bundle nullExtras = null;
			this.getIntent().replaceExtras(nullExtras);

			this.lastIntentData = this.getIntent().getData();
			Uri nullData = null;
			this.getIntent().setData(nullData);
		}
	}

	@Override
	protected void finalize() throws Throwable {
		LOG.Log(Module.GUI, "on finalize method. Stopping server...");
		stopServer();
		super.finalize();
	}

	@Override
	protected void onDestroy() {
		LOG.Log(Module.GUI, "onDestroy");
		
		((AndroidServiceLocator)AndroidServiceLocator.GetInstance()).sendApplicationEvent(AndroidApplicationEvent.onDestroy);
		
		// Stop HTTP server
		stopServer();
		super.onDestroy();

		LOG.Log(Module.GUI, "killing process...");
		android.os.Process.killProcess(android.os.Process.myPid());

	}

	@Override
	protected void onStop() {
		LOG.Log(Module.GUI, "onStop");
		
		((AndroidServiceLocator)AndroidServiceLocator.GetInstance()).sendApplicationEvent(AndroidApplicationEvent.onStop);
		
		// Stop HTTP server
		stopServer();
		super.onStop();
	}

	@Override
	protected void onNewIntent(Intent intent) {
		LOG.Log(Module.GUI, "onNewIntent");
		super.onNewIntent(intent);
		setIntent(intent);
	}

	@Override
	public void onRequestPermissionsResult (int requestCode, String[] permissions, int[] grantResults){
		// this method is invoked with an CANCELED result code if the activity is a "singleInstance" (launchMode)
		// that is the reason that we removed that launch mode for the AndroidManifest (see SVN logs)
		LOG.LogDebug(Module.GUI, "******** onActivityResult # requestCode " +requestCode + ", grantResults: " + grantResults);
		
		AndroidActivityManager aam = (AndroidActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		boolean handleResult = false;
		if (aam != null) {
			handleResult = aam.publishPermissionResult(requestCode, permissions, grantResults);
		}
		
	}
	
	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		// this method is invoked with an CANCELED result code if the activity is a "singleInstance" (launchMode)
		// that is the reason that we removed that launch mode for the AndroidManifest (see SVN logs)
		LOG.LogDebug(Module.GUI, "******** onActivityResult # requestCode " +requestCode + ", resultCode: " + resultCode);
		
		AndroidActivityManager aam = (AndroidActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		boolean handleResult = false;
		if (aam != null) {
			handleResult = aam.publishActivityResult(requestCode, resultCode, data);
		}
		
		if(!handleResult) {
			
			ResultReceiver resultReceiver = ((AndroidServiceLocator)AndroidServiceLocator.GetInstance()).getResultReceiver(requestCode);
			
			if(resultReceiver!=null) {
				LOG.LogDebug(Module.GUI, "******** Calling ResultReceiver send  (probably from a module)...");
				Bundle bundle = (data==null?new Bundle():data.getExtras());
				bundle.putInt(IAppDelegate.ACTIVITY_RESULT_CODE_BUNDLE_KEY, resultCode);
				resultReceiver.send(requestCode, bundle);
				
			} else {
			
				LOG.LogDebug(Module.GUI, "******** Calling super.onActivityResult()");
				super.onActivityResult(requestCode, resultCode, data);
			}
		}
		
	}

	

	/**
	 * Check if this activity was launched from a local notification, and send
	 * details to application
	 */
	private void checkLaunchedFromNotificationOrExternaly() {
		List<LaunchData> launchDataList = null;
		LOG.Log(Module.GUI, "checkLaunchedFromNotificationOrExternaly ");
		if (this.lastIntentExtras != null) {
			LOG.Log(Module.GUI,
					"checkLaunchedFromNotificationOrExternaly has intent extras");
			final String notificationId = lastIntentExtras
					.getString(NotificationUtils.EXTRA_NOTIFICATION_ID);
			if (notificationId != null && notificationId.length() > 0) {

				LOG.Log(Module.GUI,
						"Activity was launched from Notification Manager... ");
				final String message = lastIntentExtras
						.getString(NotificationUtils.EXTRA_MESSAGE);
				final String notificationSound = this.lastIntentExtras
						.getString(NotificationUtils.EXTRA_SOUND);
				final String customJSONString = this.lastIntentExtras
						.getString(NotificationUtils.EXTRA_CUSTOM_JSON);
				final String notificationType = lastIntentExtras
						.getString(NotificationUtils.EXTRA_TYPE);
				LOG.LogDebug(Module.GUI, notificationType + " Notification ID = "
						+ notificationId);

				NotificationData notif = new NotificationData();
				notif.setAlertMessage(message);
				notif.setSound(notificationSound);
				notif.setCustomDataJsonString(customJSONString);

				if (notificationType != null
						&& notificationType
								.equals(NotificationUtils.NOTIFICATION_TYPE_LOCAL)) {
					this.activityManager.loadUrlIntoWebView("javascript:try{Appverse.OnLocalNotificationReceived("
							+ JSONSerializer.serialize(notif) + ")}catch(e){}");
				} else if (notificationType != null
						&& notificationType
								.equals(NotificationUtils.NOTIFICATION_TYPE_REMOTE)) {
					this.activityManager.loadUrlIntoWebView("javascript:try{Appverse.PushNotifications.OnRemoteNotificationReceived("
							+ JSONSerializer.serialize(notif) + ")}catch(e){}");
				}
			} else {
				LOG.Log(Module.GUI,
						"Activity was launched from an external app with extras... ");

				for (String key : this.lastIntentExtras.keySet()) {
					Object value = this.lastIntentExtras.get(key);
					/*
					 * debugging LOG.Log(Module.GUI, String.format("%s %s (%s)",
					 * key, value.toString(), value.getClass().getName()));
					 */
					if (launchDataList == null)
						launchDataList = new ArrayList<LaunchData>();
					LaunchData launchData = new LaunchData();
					launchData.setName(key);
					launchData.setValue(value.toString());

					launchDataList.add(launchData);
				}
				LOG.Log(Module.GUI, "#num extras: " + launchDataList.size());

			}

			this.lastIntentExtras = null;
		}
		if (this.lastIntentData != null) {
			LOG.Log(Module.GUI,
					"Activity was launched from an external app with uri scheme... ");
			
			Set<String> lastIntentDataSet = this.getQueryParameterNames(this.lastIntentData);
			for (String key : lastIntentDataSet) {
			//for (String key : this.lastIntentData.getQueryParameterNames()) {
				String value = this.lastIntentData.getQueryParameter(key);
				/*
				 * debugging LOG.Log(Module.GUI, String.format("%s %s (%s)",
				 * key, value.toString(), value.getClass().getName()));
				 */
				if (launchDataList == null)
					launchDataList = new ArrayList<LaunchData>();
				LaunchData launchData = new LaunchData();
				launchData.setName(key);
				launchData.setValue(value);

				launchDataList.add(launchData);
			}
			LOG.LogDebug(Module.GUI,
					"#num Data: "
							+ (lastIntentDataSet == null ? 0
									: lastIntentDataSet.size()));

			this.lastIntentData = null;

		}

		if (launchDataList != null) {
			String executeExternallyLaunchedListener = "javascript:try{Appverse.OnExternallyLaunched ("
					+ JSONSerializer.serialize(launchDataList
							.toArray(new LaunchData[launchDataList.size()]))
					+ ")}catch(e){console.log('TESTING OnExternallyLaunched: ' + e);}";
			
			if(this.isWebviewReady()) {
				LOG.Log(Module.GUI,
							"Calling OnExternallyLaunched JS listener...");
				this.activityManager.loadUrlIntoWebView(executeExternallyLaunchedListener);
			} else {
				this.queueJSStatementsForWebviewClient(executeExternallyLaunchedListener);
			}
		}
	}
	
	/**
	 * Returns a set of the unique names of all query parameters. Iterating
	 * over the set will return the names in order of their first occurrence.
	 *
	 * @throws UnsupportedOperationException if this isn't a hierarchical URI
	 *
	 * @return a set of decoded names
	 */
	private Set<String> getQueryParameterNames(Uri uri) {
		LOG.Log(Module.GUI,
				"Universal getQueryParameterNames");
	    if (uri.isOpaque()) {
	        throw new UnsupportedOperationException("This isn't a hierarchical URI.");
	    }

	    String query = uri.getEncodedQuery();
	    if (query == null) {
	        return Collections.emptySet();
	    }

	    Set<String> names = new LinkedHashSet<String>();
	    int start = 0;
	    do {
	        int next = query.indexOf('&', start);
	        int end = (next == -1) ? query.length() : next;

	        int separator = query.indexOf('=', start);
	        if (separator > end || separator == -1) {
	            separator = end;
	        }

	        String name = query.substring(start, separator);
	        names.add(Uri.decode(name));

	        // Move start to end of name.
	        start = end + 1;
	    } while (start < query.length());

	    return Collections.unmodifiableSet(names);
	}

	/*
	 * Stopping server, if running, and inform the app that the application is
	 * send to background
	 */
	private void stopServer(boolean sendToBackground) {
		_stopServer(sendToBackground);
	}

	/*
	 * Stopping server, if running, but do not inform the app that the
	 * applicaation is send to background
	 */
	private void stopServer() {
		_stopServer(false);
	}

	private void _stopServer(boolean sendToBackground) {

		// ******* TO BE REVIEW, this while is not well programmed, needs to be changed and assure server is stopped after all
		while (server != null && !this.isWebviewLoadingPage()) {
			// [MOBPLAT-179] wait to stop server while page is still loading
			LOG.Log(Module.GUI, "App finished loading, server could be stopped");

			server.shutdown();
			server = null;
			LOG.Log(Module.GUI, "Server stopped.");

			if (sendToBackground) {
				this.activityManager.loadUrlIntoWebView("javascript:try{Appverse._toBackground()}catch(e){}");
			}

		}

	}
	
	
	/** ABSTRACT METHODS **/
	
	protected abstract void initialiazeWebViewSettings() ;
	
	protected abstract AndroidActivityManager initialiazeActivityManager() ;
	
	protected abstract void addJavascriptIntefaceToWebView(AndroidServiceLocator serviceLocator, String jsBridgeName);
	
	protected abstract void startServer();
	
	protected abstract void loadRootBlockedHtmlIntoWebview() ;
	
	protected abstract void loadROMBlockedHtmlIntoWebview();
	
	protected abstract void loadMainURLIntoWebview();
	
	protected abstract AndroidNetworkReceiver initialiazeNetworkReceiver();
	
	protected abstract void queueJSStatementsForWebviewClient(String jsStatement);
	
	protected abstract void showSplashScreen();
	
	protected abstract boolean isWebviewReady();
	
	protected abstract boolean isWebviewLoadingPage();
	
	protected abstract void registerModulesServices();
	
	
	/** PRIVATE METHODS **/
	
	private boolean checkUnityProperty(String propertyName) {
		int resourceIdentifier = getResources().getIdentifier(propertyName,
				"string", getPackageName());
		try {
			boolean propertyValue = Boolean.parseBoolean(getResources()
					.getString(resourceIdentifier));
			LOG.LogDebug(Module.GUI, propertyName + "? " + propertyValue);
			return propertyValue;

		} catch (Exception ex) {
			LOG.LogDebug(Module.GUI, "Exception getting value for " + propertyName
					+ ": " + ex.getMessage());
			return false;
		}
	}

	private boolean performSecurityChecks(AndroidServiceLocator serviceLocator) {
		
		if (securityChecksPerfomed) {
			return securityChecksPassed; // if security checks already performed, return
		}

		//  initialize variable
		securityChecksPassed = false;
		
		
		ISecurity securityService = (ISecurity)serviceLocator.GetService(
				AndroidServiceLocator.SERVICE_TYPE_SECURITY);
		
		//TODO BLOCKROMMODIFIED
		//@@BLOCKROMMODIFIED@@


















		
		if(!securityChecksPassed) {
			securityChecksPerfomed = true; // show first error page
			return securityChecksPassed;
		}
		
		securityChecksPassed = false;  // initialize variable again (previous test passed)
		
		//TODO BLOCKROOTED
		//@@BLOCKROOTED@@

















		
		

		securityChecksPerfomed = true;
		return securityChecksPassed;
	}
	
	/**
	 * Initializes the appverse context exposing data to the WebView Javascript DOM.
	 */
	private void InitializeAppverseContext (int networkType) {

		long startTime = System.currentTimeMillis();

		try {
		
			LOG.Log(Module.GUI,"Before loading the main HTML, platform will expose some information directly to javascript...");
			

			AndroidSystem systemService = (AndroidSystem)  AndroidServiceLocator
					.GetInstance().GetService(
							AndroidServiceLocator.SERVICE_TYPE_SYSTEM);
			AndroidI18N i18nService = (AndroidI18N)  AndroidServiceLocator
					.GetInstance().GetService(
							AndroidServiceLocator.SERVICE_TYPE_I18N);
			AndroidIO ioService = (AndroidIO)  AndroidServiceLocator
					.GetInstance().GetService(
							AndroidServiceLocator.SERVICE_TYPE_IO);
			IActivityManager am = (IActivityManager)  AndroidServiceLocator
					.GetInstance().GetService(
					AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);	
			
			
			// 1. Appverse Context (Appverse.is)
			UnityContext unityContext = systemService.GetUnityContext();	
		
			String unityContextJsonString = JSONSerializer.serialize (unityContext);
			unityContextJsonString = "_AppverseContext = " + unityContextJsonString;
			LOG.LogDebug(Module.GUI, "InitializeAppverseContext: "+unityContextJsonString);
			am.executeJS( unityContextJsonString );		

			// 2. OS Info (Appverse.OSInfo)
			OSInfo osInfo = systemService.GetOSInfo();
			
			String osInfoJsonString = JSONSerializer.serialize (osInfo);
			osInfoJsonString = "_OSInfo = " + osInfoJsonString;
			LOG.LogDebug(Module.GUI, "InitializeAppverseContext: "+osInfoJsonString);
			am.executeJS( osInfoJsonString );

			// 3. Hardware Info (Appverse.HardwareInfo)
			HardwareInfo hwInfo = systemService.GetOSHardwareInfo();			
			String hwInfoJsonString = JSONSerializer.serialize (hwInfo);
			hwInfoJsonString = "_HwInfo = " + hwInfoJsonString;
			LOG.LogDebug(Module.GUI, "InitializeAppverseContext: "+hwInfoJsonString);
			am.executeJS( hwInfoJsonString );

			// 4. Get all configured localized keys (Appverse.i18n)
			Locale[] supportedLocales = i18nService.GetLocaleSupported ();
			String localizedStrings = "_i18n = {}; _i18n['default'] = '" + i18nService.getDefaultLocale() +"'; ";
			String localeLiterals = "";
			for(Locale supportedLocale : supportedLocales) {
				ResourceLiteralDictionary literals = i18nService.GetResourceLiterals(supportedLocale);				
				String literalsJsonString = JSONSerializer.serialize (literals);
				localeLiterals = localeLiterals + " _i18n['" + supportedLocale.toString() + "'] = " + literalsJsonString + "; ";
			}
			localizedStrings = localizedStrings + localeLiterals;
			LOG.LogDebug(Module.GUI, "InitializeAppverseContext: "+localizedStrings);
			am.executeJS( localizedStrings );

			// 5. Current device locale
			com.gft.unity.core.system.Locale currentLocale = systemService.GetLocaleCurrent();
			String currentLocaleJsonString = JSONSerializer.serialize (currentLocale);
			currentLocaleJsonString = "_CurrentDeviceLocale = " + currentLocaleJsonString;
			LOG.LogDebug(Module.GUI, "InitializeAppverseContext: "+currentLocaleJsonString);
			am.executeJS( currentLocaleJsonString );

			// 6. Configured IO services endpoints
			IOService[] services = ioService.GetServices();
			String servicesJsonString = "_IOServices = {}; ";
			for(IOService service : services) {
				String serviceJson = JSONSerializer.serialize (service);
				servicesJsonString = servicesJsonString + " _IOServices['" + service.getName() + "-" + JSONSerializer.serialize (service.getType()) + "'] = " + serviceJson + "; ";
			}
			LOG.LogDebug(Module.GUI, "InitializeAppverseContext: "+servicesJsonString);
			am.executeJS( servicesJsonString );
			
			String networkStatusString = "_NetworkStatus = " + networkType + ";";
			LOG.LogDebug(Module.GUI, "InitializeAppverseContext: networkType: "+networkType);
			am.executeJS( networkStatusString );

		} catch (Exception ex) {			
			LOG.LogDebug(Module.GUI,"Unable to load Appverse Context. Exception message: " + ex.getMessage());			
		}

		long timetaken = System.currentTimeMillis() - startTime;
		LOG.Log(Module.GUI,"# Time elapsed initializing Appverse Context: "+ timetaken);
		
	}
	
}
