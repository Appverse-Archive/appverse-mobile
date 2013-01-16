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

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.ActivityInfo;
import android.content.res.Configuration;
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
import com.gft.unity.core.system.UnityContext;
import com.gft.unity.core.system.server.net.UUID;

// TODO difference between GetMemoryAvailableTypes and GetMemoryTypes
// TODO hardcoded: 1 display
// TODO hardcoded: available orientations
public class AndroidSystem extends AbstractSystem {

	private static final int AVAILABLE_DISPLAYS = 1;

	private static final String HARDWARE_NAME = "Android";
	private static final String OS_NAME = "Android";

	private static final long MIN_MEMORY_AVAILABLE = 1 * 1024 * 1024; // 1MiB

	public AndroidSystem() {
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
}
