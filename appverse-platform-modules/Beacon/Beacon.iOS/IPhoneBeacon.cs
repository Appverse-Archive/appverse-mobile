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

using Appverse.Core.iBeacon;

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using Foundation;
using UIKit;
using CoreLocation;

using Robotics.Mobile.Core.Bluetooth.LE;
using Robotics.Mobile.Core.iOS;
using Robotics.Mobile.Core.Utils;
using System.Timers;

namespace Appverse.Platform.IPhone
{
	/// <summary>
	/// Manager class for Bluetooth Low Energy connectivity. Adds functionality to the 
	/// CoreBluetooth Manager to track discovered devices, scanning state, and automatically
	/// stops scanning after a timeout period.
	/// </summary>
	public class IPhoneBeacon : IBeacon
	{

		IDictionary<String,Beacon> rangingBeacons = new Dictionary<String,Beacon>();
		IDictionary<String,Beacon> rangedBeacons = new Dictionary<String,Beacon>();
		IAdapter adapter;

		List<CLBeacon> [] beaconsLocation;
		CLLocationManager locationManager;
		List<CLBeaconRegion> regionRanged;

		CLBeaconRegion regionLocation = null;

		Timer timeStop;

		/// <summary>
		/// Initializes a new instance of the <see cref="Unity.Platform.IPhone.IPhoneBeacon"/> class.
		/// </summary>
		public IPhoneBeacon()
		{

			locationManager = new CLLocationManager ();
			locationManager.DidRangeBeacons += HandleDidRangeBeacons;
			locationManager.DidStartMonitoringForRegion += HandleStartMonitoringForRegion;
			locationManager.RangingBeaconsDidFailForRegion += HandleFail;

			adapter = Adapter.Current;
			List<Beacon> beacons = new List<Beacon>();

			adapter.DeviceDiscovered += (object sender, DeviceDiscoveredEventArgs e) => {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "DeviceDiscovered: "+ e.Device.ID);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "DeviceDiscovered: "+ e.Device.NativeDevice);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "DeviceDiscovered: "+ e.Device.Name);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "DeviceDiscovered: "+ e.Device.Rssi);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "DeviceDiscovered: "+ e.Device.Services);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "DeviceDiscovered: "+ e.Device.State);


				Beacon AppverseBeacon = FromDevice(e.Device);
				String id = uniqueIDForBeacon(AppverseBeacon);
				if (!rangingBeacons.ContainsKey(id)){
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** ADDED ADAPTER: "+AppverseBeacon.Distance.ToString());
					rangingBeacons.Add(id,AppverseBeacon);
				}else{
					rangingBeacons[id] = AppverseBeacon;

				}
				//rangingBeacons.Add(e.Device.ID.ToString(),FromDevice(e.Device));
			};

			adapter.ScanTimeoutElapsed += (sender, e) => {

				this.StopMonitoringBeacons();
				//adapter.StopScanningForDevices(); // not sure why it doesn't stop already, if the timeout elapses... or is this a fake timeout we made?
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Bluetooth scan timeout elapsed");

			};
		}

		void HandleDidRangeBeacons (object sender, CLRegionBeaconsRangedEventArgs e)
		{
			Unknowns.Clear ();
			Immediates.Clear ();
			Nears.Clear ();
			Fars.Clear ();

			SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** HandleDidRangeBeacons Identifier: "+e.Region.Identifier);
			int i = 0;
			foreach (CLBeacon beacon in e.Beacons) {

				switch (beacon.Proximity) {
				case CLProximity.Immediate:
					Immediates.Add (beacon);
					break;
				case CLProximity.Near:
					Nears.Add (beacon);
					break;
				case CLProximity.Far:
					Fars.Add (beacon);
					break;
				case CLProximity.Unknown:
					Unknowns.Add (beacon);
					break;
				}
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** HandleDidRangeBeacons Accuracy: "+beacon.Accuracy);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** HandleDidRangeBeacons Proximity: "+beacon.Proximity);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** HandleDidRangeBeacons Description: "+beacon.Description);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** HandleDidRangeBeacons Major: "+beacon.Major);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** HandleDidRangeBeacons Minor: "+beacon.Minor);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** HandleDidRangeBeacons Rssi: "+beacon.Rssi);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** HandleDidRangeBeacons ProximityUuid: "+beacon.ProximityUuid);


				Beacon AppverseBeacon = FromCLBeacon (beacon);
				String id = uniqueIDForBeacon (AppverseBeacon);
				if (!rangingBeacons.ContainsKey (id)) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** ADDED LOCATION: "+AppverseBeacon.Distance.ToString());
					rangingBeacons.Add (id, AppverseBeacon);
				}else{
					rangingBeacons[id] = AppverseBeacon;

				}
			}

		}

		void HandleStartMonitoringForRegion (object sender, CLRegionEventArgs e)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** HandleStartMonitoringForRegion");
		}

		void HandleFail (object sender, CLRegionBeaconsFailedEventArgs e)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "************************** HandleFail");
		}

		List<CLBeacon> Unknowns { get; set; }
		List<CLBeacon> Immediates { get; set; }
		List<CLBeacon> Nears { get; set; }
		List<CLBeacon> Fars { get; set; }

		/// <summary>
		/// Uniques the identifier for beacon.
		/// </summary>
		/// <returns>The identifier for beacon.</returns>
		/// <param name="beacon">Beacon.</param>
		private static String uniqueIDForBeacon(Beacon beacon) {
			return (beacon.UUID + "#" + beacon.Major + "#" + beacon
				.Minor).ToUpper();
		}

		/// <summary>
		/// return a Beacon from the device.
		/// </summary>
		/// <returns>Beacon</returns>
		/// <param name="device">Device</param>
		private Beacon FromDevice(IDevice device){

			Beacon beacon = new Beacon();

			beacon.UUID = device.ID.ToString();
			beacon.Name = device.Name;

			return beacon;

		}

		private Beacon FromCLBeacon(CLBeacon device){

			Beacon beacon = new Beacon();

			beacon.Minor = device.Minor.Int16Value;
			beacon.Major = device.Major.Int16Value;
			DistanceType d = DistanceType.UNKNOWN;
			switch (device.Proximity) {
			case CLProximity.Immediate:
				d = DistanceType.INMEDIATE;
				break;
			case CLProximity.Near:
				d = DistanceType.NEAR;
				break;
			case CLProximity.Far:
				d = DistanceType.FAR;
				break;
			case CLProximity.Unknown:

				break;
			}

			beacon.Distance = d;

			return beacon;

		}

		/// <summary>
		/// Starts the monitoring all regions.
		/// </summary>
		public void StartMonitoringAllRegions ()
		{
			rangingBeacons.Clear ();

			if (adapter != null) {
				if (adapter.IsScanning) {
					this.StopMonitoringBeacons ();
				}
				adapter.StartScanningForDevices (Guid.Empty);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "adapter.StartScanningForDevices(all)");
			}
		}

		/// <summary>
		/// Starts the monitoring region.
		/// </summary>
		/// <param name="UUID">UUI.</param>
		public void StartMonitoringRegion (string UUID)
		{

			rangingBeacons.Clear ();

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				#if DEBUG
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Using new iOS 8 Location Services Authorization");
				#endif
				locationManager.RequestWhenInUseAuthorization();  //only requests for authorization in app running (foreground)
			}
			/*if (adapter != null) {
				if (adapter.IsScanning) {
					this.StopMonitoringBeacons ();
				}
				Guid region = new Guid (UUID);
				adapter.StartScanningForDevices (region); 
				SystemLogger.Log("adapter.StartScanningForDevices(" + UUID +")");
			}*/

			Unknowns = new List<CLBeacon> ();
			Immediates = new List<CLBeacon> ();
			Nears = new List<CLBeacon> ();
			Fars = new List<CLBeacon> ();
			beaconsLocation = new List<CLBeacon> [4] { Unknowns, Immediates, Nears, Fars };
			NSUuid uuid;
			if (UUID != null && UUID.Any ()) {
				uuid = new NSUuid (UUID);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "uuid: " + UUID );
				regionLocation = new CLBeaconRegion (uuid, uuid.AsString ());

				locationManager.StartRangingBeacons (regionLocation);
			}
			timeStop = new Timer ();
			timeStop.Interval = 5000;
			timeStop.Start ();
			timeStop.Elapsed += (x, y) => {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "STOPPED!" );
				if (timeStop != null) timeStop.Stop();
				this.StopMonitoringBeacons();
			};
			//timeStop.Elapsed += this.StopMonitoringBeacons ();
			//NSTimer.CreateScheduledTimer(new TimeSpan(5000), new Action


		}



		/// <summary>
		/// Stops the monitoring beacons.
		/// </summary>
		public void StopMonitoringBeacons ()
		{

			if (adapter != null && adapter.IsScanning) {
				adapter.StopScanningForDevices ();
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "adapter.StopScanningForDevices()");
			}
			if(regionLocation != null)
				locationManager.StopRangingBeacons(regionLocation);
			regionLocation = null;

			List<Beacon> onEnterBeaconsArray = new List<Beacon>();
			List<Beacon> onDiscoverBeaconsArray = new List<Beacon>();
			List<Beacon> onExitBeaconsArray = new List<Beacon>();
			List<string> allBeaconsUIDs = new List<string>();

			foreach (Beacon beacon in rangingBeacons.Values) {
				string beaconUID = uniqueIDForBeacon(beacon);
				allBeaconsUIDs.Add(beaconUID);
				if (!rangedBeacons.Keys.Contains(beaconUID)) {
					onDiscoverBeaconsArray.Add(beacon);
					SystemLogger.Log(SystemLogger.Module.PLATFORM, 
						"Beacon onDiscoverBeaconsArray:" + onDiscoverBeaconsArray.Count);				
				} else {
					onEnterBeaconsArray.Add(beacon);
					SystemLogger.Log(SystemLogger.Module.PLATFORM, 
						"Beacon onEnterBeaconsArray:" + onEnterBeaconsArray.Count);				
				}
			}
			foreach (string key in rangedBeacons.Keys) {
				if (!allBeaconsUIDs.Contains(key)) {
					Beacon beacon;
					bool get = rangedBeacons.TryGetValue(key, out beacon);
					if(get)
						onExitBeaconsArray.Add(beacon);
					SystemLogger.Log(SystemLogger.Module.PLATFORM, 
						"Beacon onExitBeaconsArray:" + onExitBeaconsArray.Count);
				}
			}
			if (!onEnterBeaconsArray.Any()) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, 
					"Beacon OnEntered");
				//executeJS("Appverse.Beacon.OnEntered", new Object[]{ onEnterBeaconsArray.ToArray()});
			}

			SystemLogger.Log(SystemLogger.Module.PLATFORM, 
				"Beacon OnDiscover");
			//executeJS("Appverse.Beacon.OnDiscover", new Object[]{ onDiscoverBeaconsArray.ToArray()});


			if (!onExitBeaconsArray.Any()) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, 
					"Beacon OnExited");
				//executeJS("Appverse.Beacon.OnExited", new Object[]{ onExitBeaconsArray.ToArray()});
			}

			foreach (String UUID in rangingBeacons.Keys) {
				if (rangedBeacons.ContainsKey(UUID)) {
					Beacon beacon; 
					bool get1 = rangingBeacons.TryGetValue(UUID, out beacon);
					Beacon beaconranged;
					bool get2 = rangedBeacons.TryGetValue(UUID, out beaconranged);
					if (!get1 || !get2) {
						continue;
					}
					DistanceType oldProximity = beaconranged.Distance;
					DistanceType newProximity = beacon.Distance;
					if (oldProximity != newProximity) {
						UIApplication.SharedApplication.InvokeOnMainThread (delegate {
							//executeJS("Appverse.Beacon.OnUpdateProximity",new Object[] { beacon, oldProximity, newProximity });
							BeaconUtils.FireUnityJavascriptEvent("Appverse.Beacon.OnUpdateProximity", new Object[] { beacon, oldProximity, newProximity });
						});
					}
				}

			}		

			rangedBeacons.Clear();
			rangedBeacons = new Dictionary<string, Beacon>(rangingBeacons);
			rangingBeacons.Clear();

			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				BeaconUtils.FireUnityJavascriptEvent("Appverse.Beacon.OnEntered", new Object[]{ onEnterBeaconsArray.ToArray()});
				BeaconUtils.FireUnityJavascriptEvent("Appverse.Beacon.OnExited", new Object[]{ onExitBeaconsArray.ToArray()});
				BeaconUtils.FireUnityJavascriptEvent("Appverse.Beacon.OnDiscover", new Object[]{ onDiscoverBeaconsArray.ToArray()});
			});
		}


	}

}
