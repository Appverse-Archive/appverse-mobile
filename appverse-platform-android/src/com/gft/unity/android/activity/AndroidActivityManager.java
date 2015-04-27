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

import java.util.HashMap;
import java.util.Map;

import android.app.Activity;
import android.app.ActivityManager;
import android.app.Dialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.content.res.Configuration;
import android.view.Display;
import android.view.Surface;
import android.view.ViewGroup.LayoutParams;
import android.view.Window;
import android.view.WindowManager;
import android.webkit.WebView;
import android.widget.ImageView;
import android.widget.ImageView.ScaleType;

import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.core.json.JSONSerializer;
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

public class AndroidActivityManager implements IActivityManager {

	private static final SystemLogger LOG = AndroidSystemLogger.getInstance();

	public static final int GET_SNAPSHOT_RC = 5000;
	public static final int TAKE_SNAPSHOT_RC = 5001;
	
	private static final String SPLASH_PORTRAIT_ID = "launch_portrait";
	private static final String SPLASH_LANDSCAPE_ID = "launch_landscape";
	private static final String SPLASH_TABLET_SUFIX_ID = "_tablet";
	private static final String SPLASH_STYLE_ID = "SplashScreen";
	
	private static final String DRAWABLE_TYPE = "drawable";
	private static final String STYLE_TYPE = "style";

	private Activity main;
	private WebView view;
	private Map<Integer, IActivityManagerListener> listeners;
	private Dialog splashDialog = null;
	private ImageView splashImage = null;
	
	private boolean _notifyLoadingVisible = false;

	public AndroidActivityManager(Activity main, WebView view) {
		this.main = main;
		this.view = view;
		listeners = new HashMap<Integer, IActivityManagerListener>();
	}
	
	
	public boolean isNotifyLoadingVisible() {
		return _notifyLoadingVisible;
	}

	public void setNotifyLoadingVisible(boolean notifyLoadingVisible) {
		this._notifyLoadingVisible = notifyLoadingVisible;
	}

	@Override
	public boolean resolveActivity(Intent intent) {

		ResolveInfo ri = main.getPackageManager().resolveActivity(intent, PackageManager.MATCH_DEFAULT_ONLY);
		if(ri == null) {
			LOG.Log(Module.PLATFORM, "AndroidActivityManager no resolved intent. Activity could not start for intent: " + intent.getAction());
			return false;
		} else {
			LOG.Log(Module.PLATFORM, "AndroidActivityManager resolved intent. Activity could start for intent: " + intent.getAction());
			return true;
		}
	}
	
	@Override
	public boolean startActivity(Intent intent) {

		AAMStartActivityAction action = new AAMStartActivityAction(intent);
		main.runOnUiThread(action);

		return true;
	}

	private class AAMStartActivityAction implements Runnable {

		private Intent intent;

		public AAMStartActivityAction(Intent intent) {
			this.intent = intent;
		}

		@Override
		public void run() {
			try {
				main.startActivity(intent);
			} catch (Exception ex) {
				LOG.Log(Module.PLATFORM, "AndroidActivityManager error Starting Activity for intent: " + intent.getAction(),
						ex);
			}
		}
	}

	@Override
	public void loadUrlIntoWebView(String url) {
		this.main.runOnUiThread(new AAMLoadUrlAction(this.view, url));
	}
	
	private class AAMLoadUrlAction implements Runnable {

		private String url;
		private WebView view;
		

		public AAMLoadUrlAction(WebView view, String url) {
			this.url = url;
			this.view = view;
		}

		@Override
		public void run() {
			this.view.loadUrl(this.url);
		}
	}
	
	
	@Override
	public boolean startActivityForResult(Intent intent, int requestCode,
			IActivityManagerListener listener) {

		listeners.put(requestCode, listener);

		AAMStartActivityForResultAction action = new AAMStartActivityForResultAction(
				intent, requestCode);
		main.runOnUiThread(action);

		return true;
	}

	private class AAMStartActivityForResultAction implements Runnable {

		private Intent intent;
		private int requestCode;

		public AAMStartActivityForResultAction(Intent intent, int requestCode) {
			this.intent = intent;
			this.requestCode = requestCode;
		}

		@Override
		public void run() {
			// WARNING :: this "startActivityForResult" not working well with MainActivity as singleInstance
			// (cancel resultCode always received on the onActivityResult() method)
			// MOBPLAT-191 :: singleInstance launchMode is removed for this reason
			main.startActivityForResult(intent, requestCode);
		}
	}

	@Override
	public boolean publishActivityResult(int requestCode, int resultCode,
			Intent data) {

		LOG.Log(Module.PLATFORM, "Checking for listener for requestCode: " + requestCode + ", with resultCode:" + resultCode);
		IActivityManagerListener listener = listeners.get(requestCode);

		try {

			if (listener != null) {
				switch (resultCode) {
				case Activity.RESULT_OK:
					listener.onOk(requestCode, data);
					break;
				case Activity.RESULT_CANCELED:
					listener.onCancel(requestCode, data);
					break;
				default:
					listener.onCustom(requestCode, resultCode, data);
					break;
				}
			} else {
				LOG.Log(Module.PLATFORM, "No listener found for requestCode: " + requestCode);
				return false;
			}
		} catch (Exception ex) {
			LOG.Log(Module.PLATFORM, "AndroidActivityManager listener error",
					ex);
			return false;
		}

		listeners.remove(requestCode);

		return true;
	}

	@Override
	public void executeJS(String method, Object data) {

		if (view != null) {
			String jsonData = "null";
			if(data != null) {
				jsonData = JSONSerializer.serialize(data);
			}
			String jsCallbackFunction = "javascript:if(" + method + "){" + method + "("
					+ jsonData + ");}";

			this.main.runOnUiThread(new AAMExecuteJS(this.view, jsCallbackFunction));
		}
	}
	
	@Override
	public void executeJS(String method, Object[] dataArray) {
		if (view != null) {
			String dataJSONString = "null";
			if(dataArray!=null) {
				StringBuilder builder = new StringBuilder();
				int numObjects = 0;
				for(Object data : dataArray) {
					if(numObjects>0) {
						builder.append(",");
					}
					if (data == null) {
						builder.append("null");
					}
					if (data instanceof String) {
						builder.append("'"+ (String)data +"'");
					} else {
						builder.append(JSONSerializer.serialize (data));
					}
					numObjects++;
				}
				dataJSONString = builder.toString();
			}
			
			
			String jsCallbackFunction = "javascript:if(" + method + "){" + method + "("
					+ dataJSONString + ");}";

			this.main.runOnUiThread(new AAMExecuteJS(this.view, jsCallbackFunction));
		}
	}
	
	@Override
	public void executeJS(String json) {
		if (view != null) {			
			String jsCallbackFunction = "javascript:" + json + ";";
			this.main.runOnUiThread(new AAMExecuteJS(this.view, jsCallbackFunction));
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
	
	private class AAMExecuteJSCallback implements Runnable {

		private WebView view;
		String callbackFunction;
		String id;
		String jsonResultString;

		public AAMExecuteJSCallback(WebView view, String callbackFunction, String id, String jsonResultString) {
			this.view = view;
			this.callbackFunction = callbackFunction;
			this.id = id;
			this.jsonResultString = jsonResultString;
		}

		@Override
		public void run() {
			if (this.view != null) {
				String jsCallbackFunction = "javascript:try{if("+callbackFunction+"){"+callbackFunction+"("+jsonResultString+", '"+ id +"');}}catch(e) {console.log('error executing javascript callback:[" + callbackFunction + "] ' + e)}";
				this.view.loadUrl(jsCallbackFunction);
			}
		}
	}
	
	public void executeJSCallback(String callbackFunction, String id, String jsonResultString) {
		this.main.runOnUiThread(new AAMExecuteJSCallback(this.view, callbackFunction, id, jsonResultString));
	}
	
	private boolean isTabletDevice() {
		
		if (this.main == null) return false;
		
		return ((this.main.getResources().getConfiguration().screenLayout   
                & Configuration.SCREENLAYOUT_SIZE_MASK)    
                > Configuration.SCREENLAYOUT_SIZE_LARGE);
	}
	
	private int getSplashId() {
		
		int splashId = 0;
		boolean isTabletDevice = this.isTabletDevice();
		String splashSufix = "";
		LOG.Log(Module.PLATFORM, "AndroidActivityManager isTabletDevice? " + isTabletDevice);
		if(isTabletDevice) splashSufix = SPLASH_TABLET_SUFIX_ID;

		boolean portraitMode = false;
		boolean landscapeMode = false;
		
		Display display = ((WindowManager) this.main.getSystemService(this.main.WINDOW_SERVICE)).getDefaultDisplay();
		switch (display.getRotation()) {
			case Surface.ROTATION_0:
			case Surface.ROTATION_180:
				if(isTabletDevice) {
					landscapeMode = true;
				} else {
					portraitMode = true;
				}
				break;
			case Surface.ROTATION_90:
			case Surface.ROTATION_270:
				if(isTabletDevice) {
					portraitMode = true;
				} else {
					landscapeMode = true;
				}
				break;
		}
		
		if(portraitMode) {
			splashId = this.main.getResources().getIdentifier(SPLASH_PORTRAIT_ID + splashSufix,
				DRAWABLE_TYPE, this.main.getPackageName());
			LOG.Log(Module.PLATFORM, "AndroidActivityManager display mode: portrait");
		}
		if(landscapeMode) {
			splashId = this.main.getResources().getIdentifier(SPLASH_LANDSCAPE_ID + splashSufix,
				DRAWABLE_TYPE, this.main.getPackageName());
			LOG.Log(Module.PLATFORM, "AndroidActivityManager display mode: landscape");
		}
		
		//LOG.Log(Module.PLATFORM, "AndroidActivityManager display rotation: " + display.getRotation());
		return splashId;
	}

	public void layoutSplashscreen() {
		if(this.splashImage!=null && this.splashDialog!=null && this.splashDialog.isShowing()) {
			LOG.Log(Module.PLATFORM, "AndroidActivityManager LAYOUT SPLASHCREEN");
			int splashId = this.getSplashId();
			if(splashId != 0) {
				this.splashImage.setImageResource(splashId);
				//this.splashDialog.setContentView(this.splashImage);
			}
		}
	}
	
	public boolean showSplashScreen(WebView appView) {
		LOG.Log(Module.PLATFORM, "AndroidActivityManager CREATING SPLASHCREEN");
		
		if(appView != null) {
			this.view = appView;
		}
		
		int splashId = this.getSplashId();
		if(splashId != 0) {
			LOG.Log(Module.PLATFORM, "AndroidActivityManager SHOWING SPLASHCREEN");
			
			int themeIdentifier = this.main.getResources().getIdentifier(SPLASH_STYLE_ID, STYLE_TYPE, this.main.getPackageName());
			this.splashDialog = new Dialog(this.main, themeIdentifier);
			
			this.splashImage = new ImageView(this.main);
			this.splashImage.setLayoutParams(new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT));
			
			//LOG.Log(Module.PLATFORM, "scale type: " + this.splashImage.getScaleType().name());
			
			this.splashImage.setImageResource(splashId);
			this.splashImage.setScaleType(ScaleType.FIT_XY);
			this.splashDialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
			this.splashDialog.setContentView(this.splashImage);
			this.splashDialog.setCancelable(false);
			this.splashDialog.show();
			
			this.main.setContentView(this.view);
			
			return true;
			
		} else {
			LOG.Log(Module.PLATFORM, "AndroidActivityManager WARNING splashcreen not found, please provide the proper splashscreens on the AndroidResources folder");
			this.main.setContentView(this.view);
		}
		
		return false;
	}
	
	public boolean dismissSplashScreen() {
		
		
		if(this.splashImage!=null && this.splashDialog!=null) {
			LOG.Log(Module.PLATFORM, "AndroidActivityManager DISMISSING SPLASHCREEN");
			
			this.splashDialog.dismiss();
			
			this.splashImage = null;
			this.splashDialog = null;
			return true;
		}
		LOG.Log(Module.PLATFORM, "AndroidActivityManager Splashscreen not visible, it couldn't be dismissed");
		return false;
	}

	@Override
	public boolean startShowSplashScreen() {
		
		this.main.runOnUiThread(new Runnable() {
			
			@Override
			public void run() {
				showSplashScreen(null);
				
			}
		});
		
		return true;
	}

	@Override
	public boolean startDismissSplashScreen() {
		
		this.main.runOnUiThread(new Runnable() {
			
			@Override
			public void run() {
				dismissSplashScreen();
				
			}
		});
		
		return true;
	}
	
	@Override
	public void killBackgroundProcesses () {
		String packageName = this.main.getPackageName();
		LOG.Log(Module.PLATFORM, "killing background processes associated with the calling pacakge: " + packageName);
		ActivityManager am = (ActivityManager) this.main.getSystemService(Activity.ACTIVITY_SERVICE);
		if(am != null) {
			am.killBackgroundProcesses(packageName);
			LOG.Log(Module.PLATFORM, "background processes killed");
		}
	}
	
	@Override
	public void dismissApplication() {
		LOG.Log(Module.PLATFORM, "dismissing application programmaticallly");
		
		// reverted to first behaviour, as we removed the "singleInstance" mode in the Manifest 
		this.main.finish();
		
		/*
		 * MainActivity is now singleInstant and the app can be loaded through intents so when you finalize the app and reopen from history
		 * it remember the calling intent and try to process again the same parameters leading to errors, now the app is just sent to background.
		Intent i = new Intent();
		i.setAction(Intent.ACTION_MAIN);
		i.addCategory(Intent.CATEGORY_HOME);
		this.startActivity(i);
		*/
	}
	
}