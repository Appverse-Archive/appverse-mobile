package com.gft.unity.android;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map.Entry;

import uk.co.alt236.bluetoothlelib.device.BluetoothLeDevice;
import uk.co.alt236.bluetoothlelib.device.IBeaconDevice;
import uk.co.alt236.bluetoothlelib.util.IBeaconUtils;

import com.gft.unity.android.activity.AndroidActivityManager;
import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.android.activity.IActivityManagerListener;
import com.gft.unity.core.beacon.AbstractBeacon;
import com.gft.unity.core.beacon.Beacon;
import com.gft.unity.core.system.SystemLogger.Module;

import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothManager;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.os.Handler;
import android.os.Looper;

public class AndroidBeacon extends AbstractBeacon {

	private static final AndroidSystemLogger LOG = AndroidSystemLogger
			.getSuperClassInstance();

	private Context context;
	private BluetoothAdapter mBluetoothAdapter;
	// TODO REMOVE ON RELEASE
	HashMap<String, Beacon> iBeacon = new HashMap<String, Beacon>();
	// private boolean mScanning;
	private String UUID;
	private Handler mHandler;

	private HashMap<String, Beacon> rangingBeacons = new HashMap<String, Beacon>();
	private HashMap<String, Beacon> rangedBeacons = new HashMap<String, Beacon>();

	private int REQUEST_ENABLE_BT;

	public AndroidBeacon() {
		super();
		context = AndroidServiceLocator.getContext();

		mHandler = new Handler(Looper.getMainLooper());
		
		final BluetoothManager bluetoothManager = (BluetoothManager) context
			.getSystemService(Context.BLUETOOTH_SERVICE);
		mBluetoothAdapter = bluetoothManager.getAdapter();
	
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
			IActivityManager am = (IActivityManager) AndroidServiceLocator
					.GetInstance()
					.GetService(
							AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
			am.startActivityForResult(enableBtIntent, REQUEST_ENABLE_BT,
					new IActivityManagerListener() {

						@Override
						public void onOk(int requestCode, Intent data) {
							LOG.Log(Module.PLATFORM, "Bluetooth enable onOK");
							scanLeDevice(true);
						}

						@Override
						public void onCustom(int requestCode, int resultCode,
								Intent data) {
							// TODO Auto-generated method stub

						}

						@Override
						public void onCancel(int requestCode, Intent data) {
							LOG.Log(Module.PLATFORM,
									"Bluetooth enable onCancel");

						}
					});

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
					LOG.Log(Module.PLATFORM,
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
					LOG.Log(Module.PLATFORM,
							"Beacon postDelayed onStopLeScan");
					// mScanning = false;
					mBluetoothAdapter.stopLeScan(mLeScanCallback);
					onStopLeScan();
				}

			}, SCAN_PERIOD);

			// mScanning = true;
			mBluetoothAdapter.startLeScan(mLeScanCallback);

		} else {
			LOG.Log(Module.PLATFORM,
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

			((Activity) context).runOnUiThread(new Runnable() {
				@Override
				public void run() {
					LOG.Log(Module.PLATFORM,
							"Beacon rangingBeacons START");					
					// mLeDeviceListAdapter.addDevice(device);
					// mLeDeviceListAdapter.notifyDataSetChanged();
					if (device != null) {

						if (IBeaconUtils.isThisAnIBeacon(deviceLe)) {
							IBeaconDevice ib = new IBeaconDevice(deviceLe);
							Beacon beacon = new Beacon(ib.getAddress(), ib
									.getName(), ib.getUUID(), ib.getAccuracy(),
									ib.getMajor(), ib.getMinor(), ib
											.getTimestamp());
							LOG.Log(Module.PLATFORM,
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getAddress());
							LOG.Log(Module.PLATFORM,
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getName());
							LOG.Log(Module.PLATFORM,
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getUUID());
							LOG.Log(Module.PLATFORM,
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getDistance());
							LOG.Log(Module.PLATFORM,
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getMeters());
							LOG.Log(Module.PLATFORM,
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getMajor());
							LOG.Log(Module.PLATFORM,
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getMinor());
							LOG.Log(Module.PLATFORM,
									"TEST beacon Result IBeaconDevice: "
											+ beacon.getTimestamp());


							if (UUID != null && UUID != beacon.getUUID()){
								LOG.Log(Module.PLATFORM,
										"Beacon UUID:" + UUID);	
								return;
							}
								

							rangingBeacons.put(uniqueIDForBeacon(beacon),
									beacon);
							LOG.Log(Module.PLATFORM,
									"Beacon rangingBeacons:" + rangingBeacons.size());

						}
					}
				}
			});
		}
	};

	private synchronized void onStopLeScan() {
		LOG.Log(Module.PLATFORM,
				"Beacon onStopLeScan");
		if(this.rangingBeacons == null || this.rangingBeacons.size() == 0){
			LOG.Log(Module.PLATFORM,
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
				LOG.Log(Module.PLATFORM,
						"Beacon onDiscoverBeaconsArray:" + onDiscoverBeaconsArray.size());				
			} else {
				onEnterBeaconsArray.add(beacon);
				LOG.Log(Module.PLATFORM,
						"Beacon onEnterBeaconsArray:" + onEnterBeaconsArray.size());				
			}
		}
		for (String key : this.rangedBeacons.keySet()) {
			if (!allBeaconsUIDs.contains(key)) {
				
				onExitBeaconsArray.add((Beacon) this.rangedBeacons.get(key));
				LOG.Log(Module.PLATFORM,
						"Beacon onExitBeaconsArray:" + onExitBeaconsArray.size());
			}
		}
		if (!onEnterBeaconsArray.isEmpty()) {
			LOG.Log(Module.PLATFORM,
					"Beacon OnEntered");
			executeJS("Appverse.Beacon.OnEntered", new Object[]{ onEnterBeaconsArray.toArray()});
		}
		//if (!onDiscoverBeaconsArray.isEmpty()) {
			LOG.Log(Module.PLATFORM,
					"Beacon OnDiscover");
			executeJS("Appverse.Beacon.OnDiscover", new Object[]{ onDiscoverBeaconsArray.toArray()});
		//}
		if (!onExitBeaconsArray.isEmpty()) {
			LOG.Log(Module.PLATFORM,
					"Beacon OnExited");
			executeJS("Appverse.Beacon.OnExited", new Object[]{ onExitBeaconsArray.toArray()});
		}

		
		for (String UUID : this.rangingBeacons.keySet()) {

			if (rangedBeacons.containsKey(UUID)) {
				Beacon beacon = rangingBeacons.get(UUID);
				double oldProximity = rangedBeacons.get(UUID).getMeters();
				double newProximity = beacon.getMeters();

				if (oldProximity != newProximity) {
					LOG.Log(Module.PLATFORM,
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
			LOG.Log(Module.PLATFORM, "BeaconEvent []" + cmd);

			IActivityManager am = (IActivityManager) AndroidServiceLocator
					.GetInstance()
					.GetService(
							AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
			am.executeJS(cmd, obj);
		} catch (Exception e) {
			LOG.Log(Module.PLATFORM,
					"Unhandled exception while processing the BeaconEvent event ("
							+ cmd + "). Exception message: " + e.getMessage());
		}
	}

	private void executeJS(String cmd, Object obj) {
		try {
			LOG.Log(Module.PLATFORM, "BeaconEvent " + cmd);

			IActivityManager am = (IActivityManager) AndroidServiceLocator
					.GetInstance()
					.GetService(
							AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
			am.executeJS(cmd, obj);
		} catch (Exception e) {
			LOG.Log(Module.PLATFORM,
					"Unhandled exception while processing the BeaconEvent event ("
							+ cmd + "). Exception message: " + e.getMessage());
		}

	}

}
