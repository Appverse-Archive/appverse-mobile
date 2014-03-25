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
package com.gft.unity.android;

import java.io.File;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserFactory;

import android.app.Activity;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.ActivityInfo;
import android.content.res.Configuration;
import android.net.Uri;
import android.os.BatteryManager;
import android.os.Environment;
import android.os.StatFs;
import android.provider.Settings.Secure;
import android.telephony.TelephonyManager;
import android.text.ClipboardManager;
import android.view.Display;
import android.view.Surface;
import android.view.WindowManager;

import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.core.system.AbstractSystem;
import com.gft.unity.core.system.CPUInfo;
import com.gft.unity.core.system.DisplayBitDepth;
import com.gft.unity.core.system.DisplayInfo;
import com.gft.unity.core.system.DisplayOrientation;
import com.gft.unity.core.system.DisplayType;
import com.gft.unity.core.system.HardwareInfo;
import com.gft.unity.core.system.InputButton;
import com.gft.unity.core.system.InputCapability;
import com.gft.unity.core.system.InputGesture;
import com.gft.unity.core.system.Locale;
import com.gft.unity.core.system.MemoryStatus;
import com.gft.unity.core.system.MemoryType;
import com.gft.unity.core.system.MemoryUse;
import com.gft.unity.core.system.OSInfo;
import com.gft.unity.core.system.PowerInfo;
import com.gft.unity.core.system.PowerStatus;
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;
import com.gft.unity.core.system.UnityContext;
import com.gft.unity.core.system.launch.AndroidApp;
import com.gft.unity.core.system.launch.App;
import com.gft.unity.core.system.server.net.UUID;

// TODO difference between GetMemoryAvailableTypes and GetMemoryTypes
// TODO hardcoded: 1 display
// TODO hardcoded: available orientations
public class AndroidSystem extends AbstractSystem {

	private static final int AVAILABLE_DISPLAYS = 1;

	private static final String HARDWARE_NAME = "Android";
	private static final String OS_NAME = "Android";

	private static final long MIN_MEMORY_AVAILABLE = 1 * 1024 * 1024; // 1MiB

	private static final SystemLogger LOG = SystemLogger.getInstance();
	
	/* parsing configuration file requirements */
	private static final String DEFAULT_ENCODING = "UTF-8";
	
	private static final String APP_NODE_ATTRIBUTE = "APP";
	private static final String APP_NAME_ATTRIBUTE = "name";
	
	private static final String APP_EXPLICIT_INTENT = "android-explicit-intent";
	private static final String APP_IMPLICIT_INTENT = "android-implicit-intent";
	
	private static final String APP_ACTION_ATTRIBUTE = "action";
	private static final String APP_CATEGORY_ATTRIBUTE = "category";
	private static final String APP_TYPE_ATTRIBUTE = "mime-type";
	private static final String APP_SCHEME_ATTRIBUTE = "uri-scheme";
	private static final String APP_SLASHES_ATTRIBUTE = "uri-remove-double-slash";
	private static final String APP_COMPONENTNAME_ATTRIBUTE = "component-name";
	private static final String APP_PARSE_QUERY_INTENT_EXTRAS = "parse-query-as-intent-extras";

	public AndroidSystem() {
		loadLaunchConfig();
	}
	
	private void loadLaunchConfig() {
		Context context = AndroidServiceLocator.getContext();
		ArrayList<App> appsList = new ArrayList<App>();
		try {

			XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
			factory.setNamespaceAware(true);
			XmlPullParser xpp = factory.newPullParser();
			xpp.setInput(AndroidUtils.getInstance().getAssetInputStream(context.getAssets(), LAUNCH_CONFIG_FILE),
					DEFAULT_ENCODING);
			int eventType = xpp.getEventType();
			App app = null;
			String appName = "";
			String appUriScheme = null;
			String appAction = null;
			String appCategory = null;
			String appMimeType = null;
			boolean appRemoveUriDoubleSlash = false;
			boolean parseQueryAsIntentExtras = false;
			String appComponentName = null;
			while (eventType != XmlPullParser.END_DOCUMENT) {
				if (eventType == XmlPullParser.START_TAG) {
					if (xpp.getName().toUpperCase()
							.equals(APP_NODE_ATTRIBUTE)) {
						app = new App();
						
						// set default values
						appUriScheme = null;
						appAction = null;
						appCategory = null;
						appMimeType = null;
						appRemoveUriDoubleSlash = false;
						parseQueryAsIntentExtras = false;
						appComponentName = null;
						
						appName = xpp.getAttributeValue(null,
								APP_NAME_ATTRIBUTE);
						
					} else if(xpp.getName().equals(APP_EXPLICIT_INTENT)){
						appAction = xpp.getAttributeValue(null,
								APP_ACTION_ATTRIBUTE);
						
						appComponentName = xpp.getAttributeValue(null,
								APP_COMPONENTNAME_ATTRIBUTE);
						
						String booleanString = xpp.getAttributeValue(null,
								APP_PARSE_QUERY_INTENT_EXTRAS);
						
						if(booleanString!=null && !booleanString.isEmpty() && !booleanString.equalsIgnoreCase("null")) {
							try {
								parseQueryAsIntentExtras = Boolean.parseBoolean(booleanString);
							} catch (Exception e) {
								LOG.Log(Module.PLATFORM, 
									"Wrong value configured for '"+APP_PARSE_QUERY_INTENT_EXTRAS+"' attribute in the app with name[" + appName +"] : " 
									+ booleanString + ". Possible values are 'true' or 'false'");
							}
						} else {
							parseQueryAsIntentExtras = false;
						}
						
					} else if (xpp.getName().equals(APP_IMPLICIT_INTENT)){
						
						appAction = xpp.getAttributeValue(null,
								APP_ACTION_ATTRIBUTE);
						
						appCategory = xpp.getAttributeValue(null,
								APP_CATEGORY_ATTRIBUTE);
						
						appMimeType = xpp.getAttributeValue(null,
								APP_TYPE_ATTRIBUTE);
						
						appUriScheme = xpp.getAttributeValue(null,
								APP_SCHEME_ATTRIBUTE);
						
						String booleanString = xpp.getAttributeValue(null,
								APP_SLASHES_ATTRIBUTE);
						
						if(booleanString!=null && !booleanString.isEmpty() && !booleanString.equalsIgnoreCase("null")) {
							try {
								appRemoveUriDoubleSlash = Boolean.parseBoolean(booleanString);
							} catch (Exception e) {
								LOG.Log(Module.PLATFORM, 
									"Wrong value configured for '"+ APP_SLASHES_ATTRIBUTE+"' attribute in the app with name[" + appName +"] : " 
									+ booleanString + ". Possible values are 'true' or 'false'");
							}
						} else {
							appRemoveUriDoubleSlash = false;
						}
						
						booleanString = xpp.getAttributeValue(null,
								APP_PARSE_QUERY_INTENT_EXTRAS);
						
						if(booleanString!=null && !booleanString.isEmpty() && !booleanString.equalsIgnoreCase("null")) {
							try {
								parseQueryAsIntentExtras = Boolean.parseBoolean(booleanString);
							} catch (Exception e) {
								LOG.Log(Module.PLATFORM, 
									"Wrong value configured for '"+APP_PARSE_QUERY_INTENT_EXTRAS+"' attribute in the app with name[" + appName +"] : " 
									+ booleanString + ". Possible values are 'true' or 'false'");
							}
						} else {
							parseQueryAsIntentExtras = false;
						}
						
					}
				} else if (eventType == XmlPullParser.END_TAG) {
					if (xpp.getName().toUpperCase()
							.equals(APP_NODE_ATTRIBUTE)) {
						app.setName(appName);
						
						// specific data for Android implementation
						AndroidApp aapp = new AndroidApp();
						aapp.setUriScheme(appUriScheme);
						aapp.setRemoveUriDoubleSlash(appRemoveUriDoubleSlash);
						aapp.setParseQueryAsIntentExtras(parseQueryAsIntentExtras);
						aapp.setComponentName(appComponentName);
						aapp.setAction(appAction);
						aapp.setMimeType(appMimeType);
						aapp.setCategory(appCategory);
						
						app.setAndroidApp(aapp);
						LOG.Log(Module.PLATFORM, "*************** Loaded app to launch: " + app.toString());
						appsList.add(app);
					}
				}
				eventType = xpp.next();
			}
		} catch (Exception ex) {
			LOG.Log(Module.PLATFORM, "LoadConfig error ["
					+ LAUNCH_CONFIG_FILE + "]: " + ex.getMessage());
		}
		launchConfig.setApps(appsList
				.toArray(new App[appsList.size()]));
	}


	@Override
	public UnityContext GetUnityContext() {
		UnityContext unityContext = new UnityContext();
		
		// we don't need to make any checking, it is for sure an android if we are int he AndroidSystem class ;-)
		unityContext.setAndroid(true);
        
		Context context = AndroidServiceLocator.getContext();
		//System.out.println("****** screenLayout checking: " + context.getResources().getConfiguration().screenLayout) 
		//	+" & " +  Configuration.SCREENLAYOUT_SIZE_MASK + " > " + Configuration.SCREENLAYOUT_SIZE_LARGE);
		
		unityContext.setTabletDevice((context.getResources().getConfiguration().screenLayout   
                & Configuration.SCREENLAYOUT_SIZE_MASK)    
                > Configuration.SCREENLAYOUT_SIZE_LARGE);
	
		//System.out.println("****** TabletDevice?: " + unityContext.getTabletDevice());
		
		return unityContext;
	}

	@Override
	public DisplayInfo GetDisplayInfo(int displayNumber) {
		DisplayInfo displayInfo = null;
		Context context = AndroidServiceLocator.getContext();

		if (displayNumber == PRIMARY_DISPLAY_NUMBER) {

			displayInfo = new DisplayInfo();
			displayInfo.setDisplayNumber(displayNumber);
			displayInfo.setDisplayBitDepth(DisplayBitDepth.Unknown);
			displayInfo.setDisplayType(DisplayType.Primary);

			Display defaultDisplay = ((WindowManager) context
					.getSystemService(Context.WINDOW_SERVICE))
					.getDefaultDisplay();

			int rotation = defaultDisplay.getRotation();
			switch (rotation) {
			case Surface.ROTATION_0:
			case Surface.ROTATION_180:
				displayInfo.setDisplayOrientation(DisplayOrientation.Portrait);
				break;
			case Surface.ROTATION_90:
			case Surface.ROTATION_270:
				displayInfo.setDisplayOrientation(DisplayOrientation.Landscape);
				break;
			default:
				displayInfo.setDisplayOrientation(DisplayOrientation.Unknown);
				break;
			}

			displayInfo.setDisplayX(defaultDisplay.getWidth());
			displayInfo.setDisplayY(defaultDisplay.getHeight());
		}

		return displayInfo;
	}

	@Override
	public int GetDisplays() {
		return AVAILABLE_DISPLAYS;
	}

	@Override
	public DisplayOrientation[] GetOrientationSupported(int displayNumber) {
		DisplayOrientation[] orientations = new DisplayOrientation[] {
				DisplayOrientation.Portrait, DisplayOrientation.Landscape };
		return orientations;
	}

	@Override
	public synchronized void LockOrientation(boolean lock,
			DisplayOrientation orientation) {

		this.locked = lock;
		this.lockedOrientation = orientation == null ? DisplayOrientation.Unknown
				: orientation;
		int configOrientation = ActivityInfo.SCREEN_ORIENTATION_UNSPECIFIED;
		if (locked) {
			switch (lockedOrientation) {
			case Portrait:
				configOrientation = ActivityInfo.SCREEN_ORIENTATION_PORTRAIT;
				break;
			case Landscape:
				configOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE;
				break;
			default:
				configOrientation = ActivityInfo.SCREEN_ORIENTATION_PORTRAIT;
				break;
			}
		}

		Activity activity = (Activity) AndroidServiceLocator.getContext();
		activity.setRequestedOrientation(configOrientation);
	}

	@Override
	public InputButton[] GetInputButtons() {
		// TODO implement IHumanInteraction.GetInputButtons
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public InputGesture[] GetInputGestures() {
		// TODO implement IHumanInteraction.GetInputGestures
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public InputCapability GetInputMethodCurrent() {
		// TODO implement IHumanInteraction.GetInputMethodCurrent
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public InputCapability[] GetInputMethods() {
		// TODO implement IHumanInteraction.GetInputMethods
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public Locale GetLocaleCurrent() {

		java.util.Locale currentLocale = java.util.Locale.getDefault();
		return new Locale(currentLocale);
	}

	@Override
	public Locale[] GetLocaleSupported() {
		List<Locale> result = new ArrayList<Locale>();

		java.util.Locale[] availableLocales = java.util.Locale
				.getAvailableLocales();
		for (java.util.Locale availableLocale : availableLocales) {
			Locale locale = new Locale(availableLocale);
			result.add(locale);
		}

		return result.toArray(new Locale[result.size()]);
	}

	@Override
	public boolean CopyToClipboard(String text) {

		final String content = text;
		Runnable action = new Runnable() {

			@Override
			public void run() {
				Context context = AndroidServiceLocator.getContext();
				ClipboardManager cm = (ClipboardManager) context
						.getSystemService(Context.CLIPBOARD_SERVICE);
				cm.setText(content);
			}
		};

		Activity activity = (Activity) AndroidServiceLocator.getContext();
		activity.runOnUiThread(action);

		return true;
	}

	@Override
	public long GetMemoryAvailable(MemoryUse use) {
		// TODO implement IMemory.GetMemoryAvailable
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public long GetMemoryAvailable(MemoryUse use, MemoryType type) {
		// TODO implement IMemory.GetMemoryAvailable
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public MemoryType[] GetMemoryAvailableTypes() {
		List<MemoryType> types = new ArrayList<MemoryType>();

		if (GetMemoryStatus(MemoryType.Extended).getMemoryFree() > MIN_MEMORY_AVAILABLE) {
			types.add(MemoryType.Extended);
		}
		if (GetMemoryStatus(MemoryType.Main).getMemoryFree() > MIN_MEMORY_AVAILABLE) {
			types.add(MemoryType.Main);
		}

		return types.toArray(new MemoryType[types.size()]);
	}

	@Override
	public MemoryStatus GetMemoryStatus() {
		return GetMemoryStatus(MemoryType.Main);
	}

	@Override
	public MemoryStatus GetMemoryStatus(MemoryType type) {
		MemoryStatus memstatus = new MemoryStatus();

		if (type.equals(MemoryType.Extended)) {
			memstatus.setMemoryFree(getAvailableExternalMemorySize());
			memstatus.setMemoryTotal(getTotalExternalMemorySize());
		} else if (type.equals(MemoryType.Main)) {
			memstatus.setMemoryFree(getAvailableInternalMemorySize());
			memstatus.setMemoryTotal(getTotalInternalMemorySize());
		}

		return memstatus;
	}

	@Override
	public MemoryType[] GetMemoryTypes() {
		List<MemoryType> types = new ArrayList<MemoryType>();

		types.add(MemoryType.Main);
		if (isExternalMemoryAvailable()) {
			types.add(MemoryType.Extended);
		}

		return types.toArray(new MemoryType[types.size()]);
	}

	@Override
	public MemoryUse[] GetMemoryUses() {
		// TODO implement IMemory.GetMemoryUses
		throw new UnsupportedOperationException("Not supported yet.");
	}

	private boolean isExternalMemoryAvailable() {
		return android.os.Environment.getExternalStorageState().equals(
				android.os.Environment.MEDIA_MOUNTED);
	}

	private long getAvailableInternalMemorySize() {

		File path = Environment.getDataDirectory();
		StatFs stat = new StatFs(path.getPath());
		long blockSize = stat.getBlockSize();
		long availableBlocks = stat.getAvailableBlocks();

		return availableBlocks * blockSize;
	}

	private long getTotalInternalMemorySize() {

		File path = Environment.getDataDirectory();
		StatFs stat = new StatFs(path.getPath());
		long blockSize = stat.getBlockSize();
		long totalBlocks = stat.getBlockCount();

		return totalBlocks * blockSize;
	}

	private long getAvailableExternalMemorySize() {
		long memory = 0;

		if (isExternalMemoryAvailable()) {
			File path = Environment.getExternalStorageDirectory();
			StatFs stat = new StatFs(path.getPath());
			long blockSize = stat.getBlockSize();
			long availableBlocks = stat.getAvailableBlocks();
			memory = availableBlocks * blockSize;
		}

		return memory;
	}

	private long getTotalExternalMemorySize() {
		long memory = 0;

		if (isExternalMemoryAvailable()) {
			File path = Environment.getExternalStorageDirectory();
			StatFs stat = new StatFs(path.getPath());
			long blockSize = stat.getBlockSize();
			long totalBlocks = stat.getBlockCount();
			memory = totalBlocks * blockSize;
		}

		return memory;
	}

	@Override
	public HardwareInfo GetOSHardwareInfo() {
		HardwareInfo hwInfo = new HardwareInfo();

		String deviceId;
		Context context = AndroidServiceLocator.getContext();
		try {
			TelephonyManager tm = (TelephonyManager) context
					.getSystemService(Context.TELEPHONY_SERVICE);

			String tmDevice = "" + tm.getDeviceId();
			String tmSerial = "" + tm.getSimSerialNumber();
			String androidId = ""
					+ android.provider.Settings.Secure.getString(
							context.getContentResolver(),
							android.provider.Settings.Secure.ANDROID_ID);

			UUID deviceUuid = new UUID(androidId.hashCode(),
					((long) tmDevice.hashCode() << 32) | tmSerial.hashCode());
			deviceId = deviceUuid.toString();
		} catch (Exception e) {
			deviceId = Secure.getString(context.getContentResolver(),
					Secure.ANDROID_ID);
		}

		hwInfo.setUUID(deviceId);
		hwInfo.setName(HARDWARE_NAME);
		hwInfo.setVendor(android.os.Build.BRAND);
		hwInfo.setVersion(android.os.Build.MODEL);

		return hwInfo;
	}

	@Override
	public OSInfo GetOSInfo() {
		OSInfo oi = new OSInfo();

		oi.setName(OS_NAME);
		oi.setVendor(android.os.Build.BRAND);
		oi.setVersion(android.os.Build.VERSION.RELEASE);

		return oi;
	}

	@Override
	public PowerInfo GetPowerInfo() {
		PowerInfo pi = new PowerInfo();

		Intent intent = AndroidServiceLocator.getContext().registerReceiver(
				null, new IntentFilter(Intent.ACTION_BATTERY_CHANGED));

		int level = intent.getIntExtra(BatteryManager.EXTRA_LEVEL, -1);
		int scale = intent.getIntExtra(BatteryManager.EXTRA_SCALE, -1);
		int status = intent.getIntExtra(BatteryManager.EXTRA_STATUS, -1);
		pi.setLevel((Float.valueOf("" + level) / scale) * 100);
		pi.setTime(-1); // TODO calculate, not available
		PowerStatus ps;
		switch (status) {
		case BatteryManager.BATTERY_STATUS_CHARGING:
			ps = PowerStatus.Charging;
			break;
		case BatteryManager.BATTERY_STATUS_DISCHARGING:
			ps = PowerStatus.Discharging;
			break;
		case BatteryManager.BATTERY_STATUS_FULL:
			ps = PowerStatus.FullyCharged;
			break;
		case BatteryManager.BATTERY_STATUS_NOT_CHARGING:
		case BatteryManager.BATTERY_STATUS_UNKNOWN:
		default:
			ps = PowerStatus.Unknown;
			break;
		}
		pi.setStatus(ps);

		return pi;
	}

	@Override
	public CPUInfo GetCPUInfo() {
		// TODO implement IProcessor.GetCPUInfo
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean DismissSplashScreen() {
		IActivityManager aam = (IActivityManager) AndroidServiceLocator
				.GetInstance()
				.GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		aam.startDismissSplashScreen();
		return true;
	}

	@Override
	public boolean ShowSplashScreen() {
		IActivityManager aam = (IActivityManager) AndroidServiceLocator
				.GetInstance()
				.GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		aam.startShowSplashScreen();
		return true;
	}
	
	@Override
	public void DismissApplication() {
		IActivityManager aam = (IActivityManager) AndroidServiceLocator
				.GetInstance()
				.GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		aam.dismissApplication();
	}
	
	
	@Override
	public void LaunchApplication(App app, String query) {
		try {
			if(app!=null && app.getAndroidApp()!=null) {
				
				LOG.Log(Module.PLATFORM, "Launching " + app.toString());
				
				AndroidApp androidApp = app.getAndroidApp();
				Intent intent = new Intent(); // default intent value
				
				// setting a specific Intent Action
				if(androidApp.getAction()!=null) {
					intent = new Intent(androidApp.getAction());
					
					// defining URI to be launched
					if(androidApp.getUriScheme()!=null) {
						String dataUriQuery = (query!=null) ? query : "";
						String doubleSlash = ":";
						if(!androidApp.getRemoveUriDoubleSlash()) {
							doubleSlash = "://";
						}
						Uri dataUri = Uri.parse(androidApp.getUriScheme()+ doubleSlash + dataUriQuery);
						LOG.Log(Module.PLATFORM, "Provided URI: " + dataUri.toString());
						intent = new Intent(androidApp.getAction(), dataUri);
					}
				} 
				
				// Adding component for launching an Explicit Intent
				if(androidApp.getComponentName() !=null) {
					intent.setComponent(ComponentName.unflattenFromString(androidApp.getComponentName()));
				}
				
				// Adding category for this intent
				if(androidApp.getCategory() !=null) {
					intent.addCategory(androidApp.getCategory());
				}
				
				// Adding mime type
				if(androidApp.getMimeType() !=null) {
					intent.setType(androidApp.getMimeType());
				}
				
				// Adding intent extras parsing query when no scheme URI is used
				if(androidApp.getParseQueryAsIntentExtras()) {
					LOG.Log(Module.PLATFORM, "Adding extras to the Intent...");
					Map<String, String> urlParams = AndroidUtils.getUrlParameters(query, true, "context_path");
					int numIntentExtras = 0;
					for (Entry<String, String> e1 : urlParams.entrySet()) {
					    String sKey = e1.getKey();
					    String sValue = e1.getValue();
					    LOG.Log(Module.PLATFORM, "adding entry key: " + sKey + ", value: " + sValue);
					    if(sValue.startsWith("[") && sValue.endsWith("]")) {
					    	// passed object is an array of strings
					    	int sValueLength = sValue.length();
					    	String[] sValues = sValue.substring(1,sValueLength-1).split(",");
					    	intent.putExtra(sKey, sValues);
							numIntentExtras = numIntentExtras+1;
					    } else {
							numIntentExtras = numIntentExtras+1;
					    	intent.putExtra(sKey, sValue);
					    }
					    
					}
					LOG.Log(Module.PLATFORM, "Added #"+numIntentExtras+" extras to the Intent.");
					
				}
				
				IActivityManager aam = (IActivityManager) AndroidServiceLocator
						.GetInstance()
						.GetService(
								AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
			
				boolean result = aam.startActivity(intent);
				
				if(!result) {
					LOG.Log(Module.PLATFORM, "The system could not open the given url. Please check syntax.");
				}
				
			} else {
				LOG.Log(Module.PLATFORM, "No application provided to launch, please check your first argument on API method invocation");
			}
		} catch(Exception e) {
			LOG.Log(Module.PLATFORM, "An exception has been raised while launching the application.", e);			
		}
	}

	
}
