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
package org.me.unity4jui_android;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;

import android.content.Context;
import android.content.pm.ActivityInfo;
import android.content.res.AssetManager;
import android.content.res.Configuration;
import android.graphics.Bitmap;
import android.os.Build;
import android.view.KeyEvent;
import android.view.ViewGroup.LayoutParams;
import android.webkit.ConsoleMessage;
import android.webkit.WebChromeClient;
import android.webkit.WebResourceResponse;
import android.webkit.WebSettings;
import android.webkit.WebSettings.RenderPriority;
import android.webkit.WebStorage;
import android.webkit.WebView;
import android.webkit.WebViewClient;

import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.AndroidSystem;
import com.gft.unity.android.activity.AndroidActivityManager;
import com.gft.unity.android.activity.AppverseMainActivity;
import com.gft.unity.android.server.AndroidNetworkReceiver;
import com.gft.unity.android.server.HttpServer;
import com.gft.unity.android.server.ProxySettings;
import com.gft.unity.android.server.handler.AndroidJavaScriptServiceInterface;
import com.gft.unity.core.system.DisplayOrientation;
import com.gft.unity.core.system.SystemLogger.Module;

// being used by the modules injection, do not removed this import (ignore warnings)
import com.gft.unity.core.IAppDelegate; 
import com.gft.unity.core.system.SystemLogger; 

import android.os.ResultReceiver;
import android.util.Log;

public class MainActivity extends AppverseMainActivity {

	
	private WebView appView;
	
	private WebChromeClient webChromeClient;
	private static final int APPVIEW_ID = 10;

	private UnityWebViewClient webViewClient = null;  				// legacy web view client
	private AppverseWebViewClient appverseWebviewClient = null;   	// new web view client

	@Override
	protected void initialiazeWebViewSettings() {
		
		LOG.Log(Module.GUI, "Initializing native Android Webview component..." );
		
		if (Build.VERSION.SDK_INT >= 19) { // >= android sdk 4.4
		    if(true){// (AndroidServiceLocator.isDebuggable()) {  HARDCODED TO ALLOW DEBUG
		    	LOG.Log(Module.GUI, "debug version; activating webview for remote debugging");
				try {
					Class webviewClass = Class
							.forName("android.webkit.WebView");
					Method setWebContentsDebuggingEnabledMethod = webviewClass
							.getDeclaredMethod(
									"setWebContentsDebuggingEnabled",
									boolean.class);
					setWebContentsDebuggingEnabledMethod.invoke(null, true);
				} catch (Exception ex) {
					LOG.Log(Module.GUI, "debug version; EXCEPTION activating webview for remote debugging: " + ex.getMessage());
				}
		    		
		    }
		  }
		LOG.Log(Module.GUI, "[K] init Webview1." );
		appView = new WebView(this);
		
		setGlobalProxy();
		
		appView.setLayoutParams(new LayoutParams(LayoutParams.FILL_PARENT,
				LayoutParams.FILL_PARENT));
		appView.setId(APPVIEW_ID);

		LOG.Log(Module.GUI, "[K] init Webview2." );
		if(Build.VERSION.SDK_INT >= 17){
			LOG.Log(Module.GUI, "Using new AppverseWebviewClient..." );
			appverseWebviewClient = new AppverseWebViewClient();
			appView.setWebViewClient(appverseWebviewClient);
		} else {
			LOG.Log(Module.GUI, "Using legacy UnityWebViewClient..." );
			webViewClient = new UnityWebViewClient();
			appView.setWebViewClient(webViewClient);
		}

		LOG.Log(Module.GUI, "[K] init Webview3." );
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
		appView.getSettings().setDomStorageEnabled(true); // [MOBPLAT-129]
															// enable HTML5
															// local storage
		appView.setVerticalScrollBarEnabled(false);

		LOG.Log(Module.GUI, "[K] init Webview4." );
		// Required settings to enable HTML5 database storage
		appView.getSettings().setDatabaseEnabled(true);
		String databasePath = this.getApplicationContext()
				.getDir("database", Context.MODE_PRIVATE).getPath();
		appView.getSettings().setDatabasePath(databasePath);
		
		webChromeClient = new WebChromeClient() {

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
				LOG.LogDebug(Module.GUI,
						cm.message() + " -- From line " + cm.lineNumber()
								+ " of " + cm.sourceId());
				return true;
			}

		};

		appView.setWebChromeClient(webChromeClient);

		LOG.Log(Module.GUI, "[K] init Webview5."+appView.getId() );
	}
	
	@Override
	protected AndroidActivityManager initialiazeActivityManager() {
		return new AndroidActivityManager(this, appView);
	}

	@Override
	protected void addJavascriptIntefaceToWebView(AndroidServiceLocator serviceLocator, String jsBridgeName){
		LOG.LogDebug(Module.GUI,"******** Adding Javascript bridge to WebView");
		appView.addJavascriptInterface(new AndroidJavaScriptServiceInterface(serviceLocator, activityManager), jsBridgeName);
	}
	
	@Override
	protected void startServer() {
		Log.d("[K]", "[K] startServer");
		if(appverseWebviewClient != null) {
			LOG.LogDebug(Module.GUI,"******** Using Appverse Webview Client - No Internal Server used");
			return;
		}
		
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
			LOG.LogDebug(Module.GUI,
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
	
	@Override
	protected void loadMainURLIntoWebview() {
		LOG.Log(Module.GUI, "[K] loadMainURLIntoWebview");
		if(webViewClient != null) {
			LOG.Log(Module.GUI, "[K] webViewClient");
			webViewClient.loadMainURLIntoWebview(appView);
		} else if(appverseWebviewClient != null) {
			LOG.Log(Module.GUI, "[K] appverseWebviewClient");
			appverseWebviewClient.loadMainURLIntoWebview(appView);
		}
	}
	
	@Override
	protected void loadRootBlockedHtmlIntoWebview() {
		appView.loadUrl(DEFAULT_LOCKED_ROOTED_HTML);
	}
	
	@Override
	protected void loadROMBlockedHtmlIntoWebview() {
		appView.loadUrl(DEFAULT_LOCKED_ROM_MODIFIED_HTML);
	}
	
	@Override
	protected AndroidNetworkReceiver initialiazeNetworkReceiver() {
		return new AndroidNetworkReceiver(appView);
	}
	
	@Override
	protected void queueJSStatementsForWebviewClient(String jsStatement) {
		if(webViewClient != null) {
			webViewClient.executeJSStatements.add(jsStatement);
		} else if(appverseWebviewClient != null) {
			appverseWebviewClient.executeJSStatements.add(jsStatement);
		}
	}
	
	@Override
	protected void showSplashScreen() {
		hasSplash = activityManager.showSplashScreen(appView);
	}
	
	@Override
	protected boolean isWebviewReady() {
		if(webViewClient!=null)
			return webViewClient.webViewReady;
		
		if(appverseWebviewClient!=null)
			return appverseWebviewClient.webViewReady;	
			
		return false;
	}
	
	@Override
	protected boolean isWebviewLoadingPage() {
		return webViewClient!=null && webViewClient.webViewLoadingPage;
	}
	
	@Override
	protected void registerModulesServices() {

		// include services from modules here
		// START_APPVERSE_MODULES_SERVICES

		// START_HERE_APPVERSE_MODULE_SERVICE
		// END_HERE_APPVERSE_MODULE_SERVICE

		// END_APPVERSE_MODULES_SERVICES

		// example:
		//  in servicelocator: this.RegisterService(new module.ios.main.class(this.getContext()), "module.api.service.name");
		// in mainactivity: this.RegisterService(new module.ios.main.class(this), "module.api.service.name");
		// in mainactivity: this.RegisterService(new module.ios.main.class(this, this.getActivityManager()), "module.api.service.name");
		
	}

	private void setGlobalProxy() {
		final WebView view = this.appView;
		ProxySettings.shouldSetProxySetting = true;
		ProxySettings.setProxy(view.getContext(), view, "", 0);
	}

	private class UnityWebViewClient extends WebViewClient {

		public boolean webViewLoadingPage = false;
		public boolean webViewReady = false;
		public List<String> executeJSStatements = new ArrayList<String>();

		@Override
		public boolean shouldOverrideUrlLoading(WebView view, String url) {
			LOG.LogDebug(Module.GUI, "** UnityWebViewClient - should override url loading [" + url + "]");
			view.loadUrl(url);
			return true;
		}
		
		@Override
		public WebResourceResponse shouldInterceptRequest (WebView view, String url) {
		
			LOG.LogDebug(Module.GUI, "** UnityWebViewClient - shouldInterceptRequest [" + url + "]");
			
			boolean isSocketListening = AndroidServiceLocator.isSocketListening();
			LOG.LogDebug(Module.GUI, "** UnityWebViewClient - isSocketListening ?: " + isSocketListening);
			if(!isSocketListening) {
				LOG.LogDebug(Module.GUI, "*** WARNING - call to service STOPPED. Appverse is not listening right now!!");
				return new WebResourceResponse("text/plain", "utf-8", 
				new ByteArrayInputStream("SECURITY ISSUE".getBytes()));
			} else {
				// Do not handle this request
				AndroidServiceLocator.checkResourceIsManagedService(url);
				return null;
			}
		}
		
		@Override
		public void onLoadResource(WebView view, String url) {
			LOG.LogDebug(Module.GUI, "** UnityWebViewClient - loading resource [" + url + "]");
			/* DEPRECATED CODE - Appverse is only supporting >=14 SDK levels */
			if(Build.VERSION.SDK_INT < 11){ 
				boolean isSocketListening = AndroidServiceLocator.isSocketListening();
				LOG.LogDebug(Module.GUI, "** UnityWebViewClient - isSocketListening ?: " + isSocketListening);
				if(isSocketListening) {
					AndroidServiceLocator.checkResourceIsManagedService(url);
				}
			}
			
			super.onLoadResource(view, url);
		}

		@Override
		public void onReceivedError(WebView view, int errorCode,
				String description, String failingUrl) {
			LOG.Log(Module.GUI, "UnityWebViewClient failed loading: "
					+ failingUrl + ", error code: " + errorCode + " ["
					+ description + "]");
		}

		@Override
		public void onPageStarted(WebView view, String url, Bitmap favicon) {
			LOG.Log(Module.GUI, "UnityWebViewClient onPageStarted [" + url
					+ "]");
			this.webViewLoadingPage = true;
			super.onPageStarted(view, url, favicon);
		}

		@Override
		public void onPageFinished(WebView view, String url) {
			LOG.Log(Module.GUI, "UnityWebViewClient onPageFinished.");

			if (hasSplash && !holdSplashScreenOnStartup) {
				LOG.Log(Module.GUI,
						"UnityWebViewClient Dismissing SplashScreen (default)");
				activityManager.dismissSplashScreen();
			}
			this.webViewLoadingPage = false;
			this.webViewReady = true;
			super.onPageFinished(view, url);

			// removing all cached files after main page has finished (in addition to setting to false the 'setAppCacheEnabled') 
			view.clearCache(true);
			
			// Execute any queued JS statements
			if (executeJSStatements != null && executeJSStatements.size() > 0) {
				for (String executeJSStatement : executeJSStatements) {
					LOG.Log(Module.GUI, "Executing JS statement... : "); 
					view.loadUrl(executeJSStatement);
				}
				executeJSStatements = new ArrayList<String>(); // reset
			}
		}
		
		public void loadMainURLIntoWebview (WebView webView) {
			webView.loadUrl(LEGACY_WEBVIEW_MAIN_URL);
		}
	}

	
	/**
	 * Upgraded WebViewClient for Appverse 5.0
	 * Webview requests are intercepted and handled
	 * @author maps
	 *
	 */
	private class AppverseWebViewClient extends WebViewClient {
		
		public boolean webViewLoadingPage = false;
		public boolean webViewReady = false;
		public List<String> executeJSStatements = new ArrayList<String>();
		
		@Override
		public boolean shouldOverrideUrlLoading(WebView view, String url) {
			LOG.LogDebug(Module.GUI, "** AppverseWebViewClient - should override url loading [" + url + "]");
			view.loadUrl(url);
			return true;
		}
		
		@Override
		public WebResourceResponse shouldInterceptRequest (WebView view, String url) {
			LOG.LogDebug(Module.GUI, "** AppverseWebViewClient - shouldInterceptRequest [" + url + "]");
			return AndroidServiceLocator.checkManagedResource(url);
		}
		
		@Override
		public void onLoadResource(WebView view, String url) {
			super.onLoadResource(view, url);
		}
		
		@Override
		public void onReceivedError(WebView view, int errorCode,
				String description, String failingUrl) {
			LOG.Log(Module.GUI, "** AppverseWebViewClient - failed loading: "
					+ failingUrl + ", error code: " + errorCode + " ["
					+ description + "]");
		}

		@Override
		public void onPageStarted(WebView view, String url, Bitmap favicon) {
			LOG.Log(Module.GUI, "** AppverseWebViewClient - onPageStarted [" + url
					+ "]");
			this.webViewLoadingPage = true;
			super.onPageStarted(view, url, favicon);
		}

		@Override
		public void onPageFinished(WebView view, String url) {
			LOG.Log(Module.GUI, "** AppverseWebViewClient - onPageFinished.");

			if (hasSplash && !holdSplashScreenOnStartup) {
				LOG.Log(Module.GUI,
						"** AppverseWebViewClient - Dismissing SplashScreen (default)");
				activityManager.dismissSplashScreen();
			}
			this.webViewLoadingPage = false;
			this.webViewReady = true;
			super.onPageFinished(view, url);

			// removing all cached files after main page has finished (in addition to setting to false the 'setAppCacheEnabled') 
			view.clearCache(true);
			
			// Execute any queued JS statements
			if (executeJSStatements != null && executeJSStatements.size() > 0) {
				for (String executeJSStatement : executeJSStatements) {
					LOG.Log(Module.GUI, "Executing JS statement... : "); 
					view.loadUrl(executeJSStatement);
				}
				executeJSStatements = new ArrayList<String>(); // reset
			}
		}
		
		public void loadMainURLIntoWebview (WebView webView) {
			webView.loadUrl(WEBVIEW_MAIN_URL);
		}
	}

	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {

		if (keyCode == KeyEvent.KEYCODE_BACK) {
			appView.loadUrl("javascript:try{Appverse._backButtonPressed()}catch(e){}");
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
				configOrientation = ActivityInfo.SCREEN_ORIENTATION_PORTRAIT;
			} else if (DisplayOrientation.Landscape.equals(lockedOrientation)) {
				configOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE;
			} else {
				// Portrait as default orientation
				configOrientation = ActivityInfo.SCREEN_ORIENTATION_PORTRAIT;
			}
			if (newConfig.orientation != configOrientation) {
				LOG.Log(Module.GUI,
						"Main Activity onConfigurationChanged setting requested orientation: "
								+ configOrientation);

				setRequestedOrientation(configOrientation);
			}
		} else {
			activityManager.layoutSplashscreen();
			appView.requestLayout();
		}
	}
	
	

	
	/**** MODULES HANDLER METHODS ****/
	
	// include handlers from modules here
	// START_APPVERSE_MODULES_HANDLERS

	// START_HERE_APPVERSE_MODULE_HANDLER
	// END_HERE_APPVERSE_MODULE_HANDLER

	// END_APPVERSE_MODULES_HANDLERS
	
	
	
}
