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

import java.lang.reflect.Method;
import java.util.HashMap;
import java.util.Map;

import android.Manifest;
import android.annotation.TargetApi;
import android.app.Activity;
import android.app.ActivityManager;
import android.app.Dialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.content.res.Configuration;
import android.os.Build;
import android.view.Display;
import android.view.Surface;
import android.view.ViewGroup.LayoutParams;
import android.view.Window;
import android.view.WindowManager;
import android.webkit.WebView;
import android.widget.ImageView;
import android.widget.ImageView.ScaleType;

import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.core.json.JSONSerializer;
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

public class AndroidActivityManager implements IActivityManager {

	protected static final SystemLogger LOG = AndroidSystemLogger.getInstance();

	
	protected static final String SPLASH_PORTRAIT_ID = "launch_portrait";
	protected static final String SPLASH_LANDSCAPE_ID = "launch_landscape";
	protected static final String SPLASH_TABLET_SUFIX_ID = "_tablet";
	protected static final String SPLASH_STYLE_ID = "SplashScreen";
	
	protected static final String DRAWABLE_TYPE = "drawable";
	protected static final String STYLE_TYPE = "style";
	
	public static final int GET_SNAPSHOT_RC = 5000;
	public static final int TAKE_SNAPSHOT_RC = 5001;

	public static final int REQUEST_CAMERA = 1000;
	public static final int REQUEST_READ_CONTACT = 1001;
	public static final int REQUEST_WRITE_CONTACT = 1012;
	public static final int REQUEST_STORE = 1002;
	public static final int REQUEST_CALENDAR = 1003;
	public static final int REQUEST_SMS = 1004;
	public static final int REQUEST_PHONE = 1005;
	public static final int REQUEST_GPS = 1006;
	public static final int REQUEST_NFC = 1007;
	public static final int REQUEST_BLUETOOTH = 1008;
	
	public static final int TAKESNAPSHOT = 1009;
	public static final int CALL = 1010;
	public static final int TAKESNAPSHOTWITHOPTIONS = 1011;
	public static final int LISTCONTACTS = 1012;
	public static final int GETCONTACT = 1013;
	public static final int CREATECONTACT = 1014;
	public static final int CREATECALENDAR = 1015;
	public static final int REQUEST_COMPASS = 1016;


	public static final int SENDEMAIL = 1017;
	public static final int SCANNER = 1018;
	
	public static final int REQUEST_PHONE_STATE = 1019;



	protected Activity main;
	private WebView view;
	private Map<Integer, IActivityManagerListener> listeners;
	private Map<Integer, IActivityManagerListener> permission_listeners;
	protected Dialog splashDialog = null;
	protected ImageView splashImage = null;
	
	private boolean _notifyLoadingVisible = false;

	public AndroidActivityManager(Activity main, WebView view) {
		this.main = main;
		this.view = view;
		listeners = new HashMap<Integer, IActivityManagerListener>();
		permission_listeners = new HashMap<Integer, IActivityManagerListener>();
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
	
	@Override
	public void launchApp(Intent intent) {

		AAMStartActivityAction action = new AAMStartActivityAction(intent, this);
		main.runOnUiThread(action);
	}

	private class AAMStartActivityAction implements Runnable {

		private Intent intent;
		IActivityManager aam = null;

		public AAMStartActivityAction(Intent intent) {
			this.intent = intent;
		}
		
		public AAMStartActivityAction(Intent _intent, IActivityManager _aam) {
			this.intent = _intent;
			this.aam = _aam;
		}

		@Override
		public void run() {
			boolean success = true;
			String message = "SUCCESS";
			
			try {
				main.startActivity(intent);
				
			} catch (android.content.ActivityNotFoundException notFoundException) {
				LOG.Log(Module.PLATFORM, "The system cannot open the given url (probably app is not installed, or Activity referenced not found). Please check syntax.", notFoundException);		
				success = false;
				message = "NO_APP_CAN_OPEN_URI_SCHEME";	
				
			} catch (Exception ex) {
				LOG.Log(Module.PLATFORM, "The system failed Starting Activity for intent: " + intent.getAction() +". Please check syntax.");
				success = false;
				message = "RESOURCE_CANNOT_BE_OPENED";	
			} 
			
			if(this.aam!= null) {
				aam.executeJS("Appverse.System.onLaunchApplicationResult", new Object[] {success, message});
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
	public boolean requestPermision(String[] permissions, int requestCode, IActivityManagerListener listener) {

		permission_listeners.put(requestCode, listener);

		AAMRequestPermision action = new AAMRequestPermision(
				permissions, requestCode);
		main.runOnUiThread(action);

		return true;
		
	}

	@Override
	public boolean requestPermision(String permission, int requestCode,
			IActivityManagerListener listener) {

		permission_listeners.put(requestCode, listener);

		AAMRequestPermision action = new AAMRequestPermision(
				permission, requestCode);
		main.runOnUiThread(action);

		return true;
	}

	@TargetApi(23)
	private class AAMRequestPermision implements Runnable {

		private String permission;
		private int requestCode;
		private String[] permissions;

		public AAMRequestPermision(String permission, int requestCode) {
			this.permission = permission;
			this.requestCode = requestCode;
		}
		
		public AAMRequestPermision(String[] permissions, int requestCode) {
			this.permissions = permissions;
			this.requestCode = requestCode;
		}

		@Override
		public void run() {
			if( android.os.Build.VERSION.SDK_INT >= 23){
				if(permission != null){
					if(main.checkSelfPermission(permission) == PackageManager.PERMISSION_DENIED) {
				     // only for gingerbread and newer versions
				
					//main.shouldShowRequestPermissionRationale(permission)
					
						main.requestPermissions(new String[]{permission}, requestCode);
					}else permission_listeners.get(requestCode).onOk(requestCode, null);
				}else if (permissions != null){
					boolean denied = false;
					for(String permission: permissions){
						if(main.checkSelfPermission(permission) == PackageManager.PERMISSION_DENIED) 
							denied = true;
					}
					if(denied)
						main.requestPermissions(permissions, requestCode);
				
				}else permission_listeners.get(requestCode).onOk(requestCode, null);
				
			}else permission_listeners.get(requestCode).onOk(requestCode, null);
			
			
		}
	}
	
	

	@Override
	public boolean publishPermissionResult(int requestCode, String[] permissions, int[] grantResults) {
		LOG.Log(Module.PLATFORM, "Checking for listener for requestCode: " + requestCode + ", with resultCode:" + grantResults);
		IActivityManagerListener listener = permission_listeners.get(requestCode);
		try {

			if (listener != null || grantResults.length<1) {
				switch (grantResults[0]) {
				case PackageManager.PERMISSION_GRANTED:
					listener.onOk(requestCode, null);
					break;
				default:
					listener.onCancel(requestCode, null);
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
	
	protected int getSplashId() {
		
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
		if(!splashExist()){
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
		}else {
			return splashVisibility(true);
		}
	}
	
	public boolean dismissSplashScreen() {
		
		LOG.Log(Module.PLATFORM, "AndroidActivityManager DISMISSING SPLASHCREEN");
		if(!splashExist()){
			if(this.splashImage!=null && this.splashDialog!=null) {
				
				
				this.splashDialog.dismiss();
				
				this.splashImage = null;
				this.splashDialog = null;
				return true;
			}
			LOG.Log(Module.PLATFORM, "AndroidActivityManager Splashscreen not visible, it couldn't be dismissed");
			return false;
		}else{
			return splashVisibility(false);
		}
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
	
	private boolean splashExist() {
		int splash = main.getResources().getIdentifier("splash", "drawable", main.getPackageName());
		LOG.Log(Module.PLATFORM, "splash: "+splash);
		return splash != 0;
	}
	
	private boolean splashVisibility(boolean visible) {
		
		try {
			if(visible){
				int layoutId = main.getResources().getIdentifier("splash_screen", "layout", main.getPackageName());
			    main.setContentView(layoutId);
			}else{
				main.setContentView(view);
			}
			return true;
		} catch (Exception e) {
			// TODO Auto-generated catch block
			LOG.Log("Exception with splashscreen: "+e.getMessage());
			return false;
		}
		
	}
	
}