/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://www.appverse.mobi/licenses/apl_v2.0.pdf.

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
package org.me.unity4jui_android;

import java.io.IOException;
import java.util.Properties;

import android.os.Build;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.res.AssetManager;
import android.content.res.Configuration;
import android.os.Bundle;
import android.view.Display;
import android.view.KeyEvent;
import android.view.Surface;
import android.view.ViewGroup.LayoutParams;
import android.view.Window;
import android.view.WindowManager;
import android.webkit.ConsoleMessage;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings.RenderPriority;
import android.webkit.WebStorage;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.webkit.WebSettings;
import android.widget.ImageView;
import android.widget.Toast;
import android.graphics.Bitmap;
import android.graphics.Canvas;

import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.AndroidSystem;
import com.gft.unity.android.activity.AndroidActivityManager;
import com.gft.unity.android.log.AndroidLoggerDelegate;
import com.gft.unity.android.server.HttpServer;
import com.gft.unity.android.server.ProxySettings;
import com.gft.unity.android.server.AndroidNetworkReceiver;
import com.gft.unity.core.system.DisplayOrientation;
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;
import com.gft.unity.core.system.log.LogManager;

public class MainActivity extends Activity {

	private static final SystemLogger LOG = SystemLogger.getInstance();

	//private static final String WEBVIEW_MAIN_URL = "file:///android_asset/WebResources/www/index.html";
	private static final String WEBVIEW_MAIN_URL = "http://127.0.0.1:8080/WebResources/www/index.html";

	private static final String SERVER_PROPERTIES = "Settings.bundle/Root.properties";
	private static final String SERVER_PORT_PROPERTY = "IPC_DefaultPort";

	private WebView appView;
	private WebChromeClient webChromeClient;
	private boolean hasSplash = false;
	//private boolean splashShownOnBackground = false;
	private AndroidActivityManager activityManager = null;
	private boolean holdSplashScreenOnStartup = false;
	private boolean disableThumbnails = false;

	private HttpServer server = null;
	private Properties serverProperties;
	private int serverPort;

	private static final int APPVIEW_ID = 10;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		LOG.Log(Module.GUI, "onCreate");
		
		// GUI initialization code
		getWindow().requestFeature(Window.FEATURE_NO_TITLE);
		getWindow().setFlags(
				WindowManager.LayoutParams.FLAG_FORCE_NOT_FULLSCREEN,
				WindowManager.LayoutParams.FLAG_FORCE_NOT_FULLSCREEN);
				
		disableThumbnails = checkUnityProperty("Unity_DisableThumbnails");
		
		// security reasons; don't allow screen shots while this window is displayed
		/* not valid for builds under level 14 */
		if(disableThumbnails && Build.VERSION.SDK_INT >= 14) {
			getWindow().setFlags(
				WindowManager.LayoutParams.FLAG_SECURE,
				WindowManager.LayoutParams.FLAG_SECURE);
		}
		
		appView = new WebView(this);
		appView.enablePlatformNotifications();
		setGlobalProxy();
		
		appView.setLayoutParams(new LayoutParams(LayoutParams.FILL_PARENT,
				LayoutParams.FILL_PARENT));
		appView.setId(APPVIEW_ID);
		appView.setWebViewClient(new UnityWebViewClient());
		appView.getSettings().setJavaScriptEnabled(true);
		appView.getSettings().setJavaScriptCanOpenWindowsAutomatically(true);
		appView.getSettings().setAllowFileAccess(true);
		appView.getSettings().setSupportZoom(false);
		appView.getSettings().setAppCacheEnabled(false);
		appView.getSettings().setCacheMode(WebSettings.LOAD_NO_CACHE);
		appView.getSettings().setAppCacheMaxSize(0);
		appView.getSettings().setSavePassword(false);
		appView.getSettings().setSaveFormData(false);
		appView.getSettings().setDefaultTextEncodingName("UTF-8");
		appView.getSettings().setGeolocationEnabled(true);
		appView.getSettings().setLightTouchEnabled(true);
		appView.getSettings().setRenderPriority(RenderPriority.HIGH);

		appView.setVerticalScrollBarEnabled(false);

		// Required settings to enable HTML5 database storage
		appView.getSettings().setDatabaseEnabled(true);
		String databasePath = this.getApplicationContext()
				.getDir("database", Context.MODE_PRIVATE).getPath();
		appView.getSettings().setDatabasePath(databasePath);

		webChromeClient = new WebChromeClient() {

			@Override
			public boolean onJsAlert(WebView view, String url, String message,
					android.webkit.JsResult result) {
				Toast.makeText(getApplicationContext(), message,
						Toast.LENGTH_SHORT);
				return super.onJsAlert(view, url, message, result);
			};

			// Required settings to enable HTML5 database storage
			@Override
			public void onExceededDatabaseQuota(String url,
					String databaseIdentifier, long currentQuota,
					long estimatedSize, long totalUsedQuota,
					WebStorage.QuotaUpdater quotaUpdater) {
				quotaUpdater.updateQuota(estimatedSize * 2);
			};

			@Override
			public void onReachedMaxAppCacheSize(long spaceNeeded,
					long totalUsedQuota,
					android.webkit.WebStorage.QuotaUpdater quotaUpdater) {
				quotaUpdater.updateQuota(0);
			};

			@Override
			public boolean onConsoleMessage(ConsoleMessage cm) {
				LOG.Log(Module.GUI,
						cm.message() + " -- From line " + cm.lineNumber()
								+ " of " + cm.sourceId());
				return true;
			}

		};

		appView.setWebChromeClient(webChromeClient);
		
		// create the application logger
		LogManager.setDelegate(new AndroidLoggerDelegate());
		
		// save the context for further access
		AndroidServiceLocator.setContext(this);
		
		// initialize the service locator
		activityManager = new AndroidActivityManager(this, appView);
		
		//killing previous background processes from the same package
		activityManager.killBackgroundProcesses();
		
		AndroidServiceLocator serviceLocator = (AndroidServiceLocator) AndroidServiceLocator
				.GetInstance();
		serviceLocator.RegisterService(this.getAssets(),
				AndroidServiceLocator.SERVICE_ANDROID_ASSET_MANAGER);
		serviceLocator.RegisterService(
				activityManager,
				AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		startServer();

		/* THIS COULD NOT BE CHECKED ON API LEVEL < 11; NO suchmethodexception
		boolean hwAccelerated = appView.isHardwareAccelerated();
		if(hwAccelerated)
			LOG.Log(Module.GUI,"Application View is HARDWARE ACCELERATED");
		else
			LOG.Log(Module.GUI,"Application View is NOT hardware accelerated");
		*/
		
		final IntentFilter actionFilter = new IntentFilter();
		actionFilter.addAction(android.net.ConnectivityManager.CONNECTIVITY_ACTION);
		//actionFilter.addAction("android.intent.action.SERVICE_STATE");
		registerReceiver(new AndroidNetworkReceiver(appView), actionFilter);
		
		final Activity currentContext = this;
		new Thread(new Runnable() {
			public void run() {
				currentContext.runOnUiThread(new Runnable() {
					public void run() {
						appView.loadUrl(WEBVIEW_MAIN_URL);

					}
				});
			}
		}).start();
		
		holdSplashScreenOnStartup =  checkUnityProperty("Unity_HoldSplashScreenOnStartup");
		hasSplash = activityManager.showSplashScreen(appView);
	}
	
	private boolean checkUnityProperty(String propertyName) {
		int resourceIdentifier = getResources().getIdentifier(propertyName, "string", getPackageName()); 
		try {
			boolean propertyValue = Boolean.parseBoolean(getResources().getString(resourceIdentifier));
			LOG.Log(Module.GUI, propertyName + "? " + propertyValue);
			return propertyValue; 
				
		} catch (Exception ex) {
			LOG.Log(Module.GUI,"Exception getting value for " + propertyName + ": " + ex.getMessage());
			return false;
		}
	}
	
	
	@Override
	public boolean onCreateThumbnail (Bitmap outBitmap, Canvas canvas) {
		LOG.Log(Module.GUI, "onCreateThumbnail");
		if(!disableThumbnails) {
			return super.onCreateThumbnail(outBitmap,canvas);
		} else {
			return true; // for security reasons, thumbnails are not allowed
		}
	}
	
	@Override
	public void onWindowFocusChanged(boolean hasFocus) {
		LOG.Log(Module.GUI, "onWindowFocusChanged");
		if(hasFocus) {
			LOG.Log(Module.GUI, "application has focus; calling foreground listener");
			appView.loadUrl("javascript:try{Unity._toForeground()}catch(e){}");
		} else {
			LOG.Log(Module.GUI, "application lost focus; calling background listener");
			appView.loadUrl("javascript:try{Unity._toBackground()}catch(e){}");
			/*
			if (server == null) {
				// security reasons; the splash screen is shown when application enters in background (hiding sensitive data)
				// it will be dismissed "onResume" method
				if(!splashShownOnBackground) {
					splashShownOnBackground = activityManager.showSplashScreen(appView);
				}
			}
			*/
		}
		
	}

	@Override
	protected void onPause() {
		LOG.Log(Module.GUI, "onPause");

		appView.loadUrl("javascript:try{Unity._toBackground()}catch(e){}");
		
		// Stop HTTP server
		stopServer();
		super.onPause();
	}

	@Override
	protected void onResume() {
		super.onResume();
		
		if(ProxySettings.checkSystemProxyProperties()) {
			ProxySettings.shouldSetProxySetting = true;
		}

		// Save the context for further access
		AndroidServiceLocator.setContext(this);

		LOG.Log(Module.GUI, "onResume");
		
		/*
		// security reasons
		if(splashShownOnBackground) {
			activityManager.dismissSplashScreen();
			splashShownOnBackground = false;
		}
		*/
		// Start HTTP server
		startServer();

		appView.loadUrl("javascript:try{Unity._toForeground()}catch(e){}");
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

		// Stop HTTP server
		stopServer();
		super.onDestroy();
		
		LOG.Log(Module.GUI, "killing process...");
		android.os.Process.killProcess(android.os.Process.myPid());
		
	}

	@Override
	protected void onStop() {
		LOG.Log(Module.GUI, "onStop");

		// Stop HTTP server
		stopServer();
		super.onStop();
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		AndroidActivityManager aam = (AndroidActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		if (aam != null) {
			aam.publishActivityResult(requestCode, resultCode, data);
		}
	}

	private void startServer() {

		if (server == null) {
			AssetManager am = (AssetManager) AndroidServiceLocator
					.GetInstance()
					.GetService(
							AndroidServiceLocator.SERVICE_ANDROID_ASSET_MANAGER);
			if (serverProperties == null) {
				serverProperties = new Properties();
				try {
					serverProperties.load(am.open(SERVER_PROPERTIES));
				} catch (IOException ex) {
					LOG.Log(Module.GUI, ex.toString());
				}
			}
			LOG.Log(Module.GUI,
					"The Port is: "
							+ serverProperties.getProperty(
									SERVER_PORT_PROPERTY, "Missing"));
			try {
				serverPort = Integer.parseInt(serverProperties
						.getProperty(SERVER_PORT_PROPERTY));
				server = new HttpServer(serverPort, this, this.appView);
				server.start();
			} catch (Exception ex) {
				LOG.Log(Module.GUI, ex.toString());
			}
			LOG.Log(Module.GUI, "Server started.");
		}
	}

	private void stopServer() {

		if (server != null) {
			server.shutdown();
			server = null;
			LOG.Log(Module.GUI, "Server stopped.");
		}
	}
	
	
	private void setGlobalProxy() {
		final WebView view = this.appView;
		ProxySettings.shouldSetProxySetting = true;
		ProxySettings.setProxy(view.getContext(), view, "", 0);
	}

	private class UnityWebViewClient extends WebViewClient {

		@Override
		public boolean shouldOverrideUrlLoading(WebView view, String url) {
			LOG.Log(Module.GUI, "should override url loading [" + url + "]");
			view.loadUrl(url);
			return true;
		}
		
		@Override
		public void onLoadResource(WebView view, String url) {
			LOG.Log(Module.GUI, "loading resource [" + url + "]");
			super.onLoadResource(view, url);
		}
		
		@Override
		public void onReceivedError (WebView view, int errorCode, String description, String failingUrl) {
			LOG.Log(Module.GUI, "UnityWebViewClient failed loading: " + failingUrl + ", error code: " + errorCode + " [" + description + "]");
		}

		@Override
		public void onPageStarted (WebView view, String url, Bitmap favicon) {
			LOG.Log(Module.GUI, "UnityWebViewClient onPageStarted [" + url + "]");
			super.onPageStarted(view, url, favicon);
		}

		@Override
		public void onPageFinished(WebView view, String url) {
			LOG.Log(Module.GUI, "UnityWebViewClient onPageFinished.");
			
			if (hasSplash && !holdSplashScreenOnStartup) {
				LOG.Log(Module.GUI, "UnityWebViewClient Dismissing SplashScreen (default)");
				activityManager.dismissSplashScreen();
			}
			
			super.onPageFinished(view, url);
		}
	}

	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {

		if (keyCode == KeyEvent.KEYCODE_BACK) {
			appView.loadUrl("javascript:try{Unity._backButtonPressed()}catch(e){}");
			return false;
		}

		return super.onKeyDown(keyCode, event);
	}

	@Override
	public void onConfigurationChanged(Configuration newConfig) {
		super.onConfigurationChanged(newConfig);
		AndroidSystem system = (AndroidSystem) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_TYPE_SYSTEM);
		boolean locked = system.IsOrientationLocked();
		if (locked) {
			int configOrientation;
			DisplayOrientation lockedOrientation = system
					.GetLockedOrientation();
			if (DisplayOrientation.Portrait.equals(lockedOrientation)) {
				configOrientation = Configuration.ORIENTATION_PORTRAIT;
			} else if (DisplayOrientation.Landscape.equals(lockedOrientation)) {
				configOrientation = Configuration.ORIENTATION_LANDSCAPE;
			} else {
				// Portrait as default orientation
				configOrientation = Configuration.ORIENTATION_PORTRAIT;
			}
			if (newConfig.orientation != configOrientation) {
				LOG.Log(Module.GUI, "Main Activity onConfigurationChanged setting requested orientation: " + configOrientation);
				
				setRequestedOrientation(configOrientation);
			}
		} else {
			activityManager.layoutSplashscreen();
			appView.requestLayout();
		}
	}
}
