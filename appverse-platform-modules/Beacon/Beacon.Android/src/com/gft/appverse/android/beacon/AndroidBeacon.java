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
package com.gft.appverse.android.beacon;

import java.util.ArrayList;
import java.util.HashMap;

import uk.co.alt236.bluetoothlelib.device.BluetoothLeDevice;
import uk.co.alt236.bluetoothlelib.device.IBeaconDevice;
import uk.co.alt236.bluetoothlelib.util.IBeaconUtils;
import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothManager;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.ResultReceiver;
import android.webkit.WebView;

import com.gft.unity.core.IAppDelegate;
import com.gft.unity.core.json.JSONSerializer;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class AndroidBeacon extends AbstractBeacon {

	private static final String LOGGER_MODULE = "Beacons Module";
	protected static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);
	
	private Context context;
	private WebView webView;
	
	private BluetoothAdapter mBluetoothAdapter;
	// TODO REMOVE ON RELEASE
	HashMap<String, Beacon> iBeacon = new HashMap<String, Beacon>();
	// private boolean mScanning;
	private String UUID;
	private Handler mHandler;

	private HashMap<String, Beacon> rangingBeacons = new HashMap<String, Beacon>();
	private HashMap<String, Beacon> rangedBeacons = new HashMap<String, Beacon>();

	public AndroidBeacon() {}
	
	public AndroidBeacon(Context ctx, WebView view) {
		
		super();
		this.context = ctx;
		this.webView = view;

		mHandler = new Handler(Looper.getMainLooper());
		
		final BluetoothManager bluetoothManager = (BluetoothManager) context
			.getSystemService(Context.BLUETOOTH_SERVICE);
		mBluetoothAdapter = bluetoothManager.getAdapter();
	
	}
	
	public void onOk(Bundle data) {
		LOGGER.logDebug("onOk","Bluetooth enable onOK");
		scanLeDevice(true);
	}
	
	
	/* (non-Javadoc)
	 * @see com.gft.unity.core.IAppDelegate#buildMode(boolean)
	 */
	@Override
	public void buildMode(boolean arg0) {
		// TODO Auto-generated method stub
		
	}

	/* (non-Javadoc)
	 * @see com.gft.unity.core.IAppDelegate#onDestroy()
	 */
	@Override
	public void onDestroy() {
		try{
			this.StopMonitoringBeacons();
		} catch (Exception e) {
			LOGGER.logDebug("onDestroy", "Exception checking BLE feature enabled. Exception: " + e.getMessage());
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

	@Override
	/**
	 * 
	 */
	public void StartMonitoringRegion(String Region) {
		if (Region != null) {
			UUID = Region;
		}else{
			UUID = null;
		}
		// Ensures Bluetooth is available on the device and it is enabled.
		// If not, displays a dialog requesting user permission to enable
		// Bluetooth.
		if (mBluetoothAdapter == null || !mBluetoothAdapter.isEnabled()) {

			Intent enableBtIntent = new Intent(
					BluetoothAdapter.ACTION_REQUEST_ENABLE);
			
			Activity mainActivity = (Activity) context;
			mainActivity.startActivityForResult(enableBtIntent, BeaconResultReceiver.ENBABLE_BT_RC);

		} else {
			scanLeDevice(true);
		}

	}

	@Override
	public void StopMonitoringBeacons() {
		Runnable action = new Runnable() {

			@Override
			public void run() {
				try {
					mBluetoothAdapter.stopLeScan(mLeScanCallback);
					onStopLeScan();

				} catch (Exception e) {
					LOGGER.logDebug("",
							"Unhandled exception while [StopListening] (runOnUiThread). Exception message: "
									+ e.getMessage());

				}
			}
		};
		((Activity) context).runOnUiThread(action);
	}

	// Stops scanning after 5 seconds.
	final long SCAN_PERIOD = 5000;

	private void scanLeDevice(final boolean enable) {

		if (enable) {

			// Stops scanning after a pre-defined scan period.
			mHandler.postDelayed(new Runnable() {
				@Override
				public void run() {
					LOGGER.logDebug("",
							"Beacon postDelayed onStopLeScan");
					// mScanning = false;
					mBluetoothAdapter.stopLeScan(mLeScanCallback);
					onStopLeScan();
				}

			}, SCAN_PERIOD);

			// mScanning = true;
			mBluetoothAdapter.startLeScan(mLeScanCallback);

		} else {
			LOGGER.logDebug("",
					"Beacon postDelayed onStopLeScan enable false");
			// mScanning = false;
			mBluetoothAdapter.stopLeScan(mLeScanCallback);
			onStopLeScan();

		}
	}

	public static String uniqueIDForBeacon(Beacon beacon) {
		return (beacon.getUUID() + "#" + beacon.getMajor() + "#" + beacon
				.getMinor()).toUpperCase();
	}

	// Device scan callback.
	private BluetoothAdapter.LeScanCallback mLeScanCallback = new BluetoothAdapter.LeScanCallback() {
		@Override
		public void onLeScan(final BluetoothDevice device, final int rssi,
				final byte[] scanRecord) {

			final BluetoothLeDevice deviceLe = new BluetoothLeDevice(device,
					rssi, scanRecord, System.currentTimeMillis());
			LOGGER.logDebug("onLeScan",
					"Beacon BluetoothLeDevice: "+deviceLe.getName());	
			((Activity) context).runOnUiThread(new Runnable() {
				@Override
				public void run() {
					LOGGER.logDebug("onLeScan",
							"Beacon rangingBeacons START");					
					// mLeDeviceListAdapter.addDevice(device);
					// mLeDeviceListAdapter.notifyDataSetChanged();
					if (device != null) {

						LOGGER.logDebug("onLeScan",
								"Device:" + device.getName());
						if (IBeaconUtils.isThisAnIBeacon(deviceLe)) {
							IBeaconDevice ib = new IBeaconDevice(deviceLe);
							
							
							
							Beacon beacon = new Beacon(ib.getAddress(), ib
									.getName(), ib.getUUID(), ib.getAccuracy(),
									ib.getMajor(), ib.getMinor(), ib
											.getTimestamp());
							String uniqueID = uniqueIDForBeacon(beacon);
							LOGGER.logDebug("onLeScan",
									"TEST beacon uniqueID: " +uniqueID);
							if(rangingBeacons.containsKey(uniqueID)) {
								LOGGER.logDebug("mLeScanCallback",
										"TEST beacon already added: " +uniqueID);
								return;
							}
							
							
							LOGGER.logDebug("onLeScan",
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getAddress());
							LOGGER.logDebug("onLeScan",
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getName());
							LOGGER.logDebug("onLeScan",
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getUUID());
							LOGGER.logDebug("onLeScan",
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getDistance());
							LOGGER.logDebug("onLeScan",
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getMeters());
							LOGGER.logDebug("onLeScan",
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getMajor());
							LOGGER.logDebug("onLeScan",
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getMinor());
							LOGGER.logDebug("onLeScan",
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getTimestamp());
							
							if (UUID != null && UUID != beacon.getUUID()){
								LOGGER.logDebug("onLeScan",
										"Beacon UUID:" + UUID);	
								return;
							}

							rangingBeacons.put(uniqueID,
									beacon);
							LOGGER.logDebug("onLeScan",
									"Beacon rangingBeacons:" + rangingBeacons.size());

						}else{
							LOGGER.logDebug("onLeScan",
									"Beacon mLeScanCallback deviceLe NOTBEACON Name: "+deviceLe.getName());
							LOGGER.logDebug("onLeScan",
									"Beacon mLeScanCallback deviceLe NOTBEACON Address: "+deviceLe.getAddress());
							LOGGER.logDebug("onLeScan",
									"Beacon mLeScanCallback deviceLe NOTBEACON Rssi: "+deviceLe.getRssi());
							
						}
					}else{
						LOGGER.logDebug("onLeScan",
								"Beacon mLeScanCallback Not a beacon device");
					}
				}
			});
		}
	};

	private synchronized void onStopLeScan() {
		LOGGER.logDebug("onStopLeScan",
				"Beacon onStopLeScan");
		if(this.rangingBeacons == null || this.rangingBeacons.size() == 0){
			LOGGER.logDebug("onStopLeScan",
					"Beacon rangingBeacons not populated yet");
			return;
		}
		ArrayList<Beacon> onEnterBeaconsArray = new ArrayList<Beacon>();
		ArrayList<Beacon> onDiscoverBeaconsArray = new ArrayList<Beacon>();
		ArrayList<Beacon> onExitBeaconsArray = new ArrayList<Beacon>();
		ArrayList<String> allBeaconsUIDs = new ArrayList<String>();
		
		for (Beacon beacon : this.rangingBeacons.values()) {
			String beaconUID = uniqueIDForBeacon(beacon);
			allBeaconsUIDs.add(beaconUID);
			if (!this.rangedBeacons.keySet().contains(beaconUID)) {
				onDiscoverBeaconsArray.add(beacon);
				LOGGER.logDebug("onStopLeScan",
						"Beacon onDiscoverBeaconsArray:" + onDiscoverBeaconsArray.size());				
			} else {
				onEnterBeaconsArray.add(beacon);
				LOGGER.logDebug("onStopLeScan",
						"Beacon onEnterBeaconsArray:" + onEnterBeaconsArray.size());				
			}
		}
		for (String key : this.rangedBeacons.keySet()) {
			if (!allBeaconsUIDs.contains(key)) {
				
				onExitBeaconsArray.add((Beacon) this.rangedBeacons.get(key));
				LOGGER.logDebug("onStopLeScan",
						"Beacon onExitBeaconsArray:" + onExitBeaconsArray.size());
			}
		}
		if (!onEnterBeaconsArray.isEmpty()) {
			LOGGER.logDebug("onStopLeScan",
					"Beacon OnEntered");
			executeJS("Appverse.Beacon.OnEntered", new Object[]{ onEnterBeaconsArray.toArray()});
		}
		//if (!onDiscoverBeaconsArray.isEmpty()) {
			LOGGER.logDebug("onStopLeScan",
					"Beacon OnDiscover");
			executeJS("Appverse.Beacon.OnDiscover", new Object[]{ onDiscoverBeaconsArray.toArray()});
		//}
		if (!onExitBeaconsArray.isEmpty()) {
			LOGGER.logDebug("onStopLeScan",
					"Beacon OnExited");
			executeJS("Appverse.Beacon.OnExited", new Object[]{ onExitBeaconsArray.toArray()});
		}

		
		for (String UUID : this.rangingBeacons.keySet()) {

			if (rangedBeacons.containsKey(UUID)) {
				Beacon beacon = rangingBeacons.get(UUID);
				double oldProximity = rangedBeacons.get(UUID).getMeters();
				double newProximity = beacon.getMeters();

				if (oldProximity != newProximity) {
					LOGGER.logDebug("onStopLeScan",
							"Beacon OnUpdateProximity");
					executeJS("Appverse.Beacon.OnUpdateProximity",
							new Object[] { beacon, oldProximity, newProximity });
				}
			}

		}		
		this.rangedBeacons.clear();
		this.rangedBeacons.putAll(this.rangingBeacons);
		this.rangingBeacons.clear();
	}

	private void executeJS(String cmd, Object[] obj) {
		try {
			LOGGER.logDebug("executeJS","BeaconEvent []" + cmd);

			this.executeJS((Activity)context, cmd, obj);
			
		} catch (Exception e) {
			LOGGER.logDebug("executeJS",
					"Unhandled exception while processing the BeaconEvent event ("
							+ cmd + "). Exception message: " + e.getMessage());
		}
	}

	/* not used
	private void executeJS(String cmd, Object obj) {
		try {
			LOGGER.logDebug("executeJS","BeaconEvent " + cmd);

			this.executeJS((Activity)context, cmd, obj);
			
		} catch (Exception e) {
			LOGGER.logDebug("executeJS",
					"Unhandled exception while processing the BeaconEvent event ("
							+ cmd + "). Exception message: " + e.getMessage());
		}

	}
	*/
	
	public void executeJS(Activity main, String method, Object[] dataArray) {
		if (this.webView != null) {
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

			main.runOnUiThread(new AAMExecuteJS(this.webView, jsCallbackFunction));
		}
	}
	
	/*
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
	*/
	
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

}
