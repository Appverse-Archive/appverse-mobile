/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
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

using Estimote;
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

		BeaconManager beaconManager = null;
		BeaconRegion region;
		Timer tmr;
		Dictionary<string, Appverse.Core.iBeacon.Beacon> beaconDict = new Dictionary<string, Appverse.Core.iBeacon.Beacon>();
		//List<Appverse.Core.iBeacon.Beacon> beaconArray;

		#region IBeacon implementation
		/// <summary>
		/// Starts the monitoring all regions.
		/// </summary>
		public void StartMonitoringAllRegions ()
		{
			//StartMonitoringRegion (null);

			SystemLogger.Log (SystemLogger.Module.PLATFORM, "All Region not supported in iOS ");

		}

		/// <summary>
		/// Starts the monitoring region.
		/// </summary>
		/// <param name="UUID">UUI.</param>
		public void StartMonitoringRegion (string UUID)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				// REQUIRED TO BE INVOKED ONN UI MAIN THREAD

				if (beaconManager == null) {

					beaconManager = new BeaconManager ();
					beaconManager.ReturnAllRangedBeaconsAtOnce = true;
					beaconManager.AuthorizationStatusChanged += (sender, e) => StartRangingBeacons (UUID);
					beaconManager.RangedBeacons += (sender, e) => 
					{
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "Ranged Beacon [found]");
						Estimote.Beacon[] beacons = e.Beacons;
						//new UIAlertView("Beacons Found", "Just found: " + e.Beacons.Length + " beacons.", null, "OK").Show();

						foreach (Estimote.Beacon beacon in beacons) {
							var b = new Appverse.Core.iBeacon.Beacon ();
							b.Address = beacon.MacAddress;
							b.Major = beacon.Major;
							b.Minor = beacon.Minor;
							b.Name = beacon.Name;
							b.UUID = beacon.ProximityUUID.AsString();
							CLProximity proximity = beacon.Proximity;
							DistanceType proximityB;
							switch (proximity) {
							case CLProximity.Far:
								proximityB = DistanceType.FAR;
								break;
							case CLProximity.Immediate:
								proximityB = DistanceType.INMEDIATE;
								break;
							case CLProximity.Near:
								proximityB = DistanceType.FAR;
								break;
							default:
								proximityB = DistanceType.UNKNOWN;
								break;

							}
							b.setDistance (proximityB);
							b.Meters = (double)beacon.Distance;

							/*
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "K: DeviceDiscovered UUID: " + b.UUID);
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "K: DeviceDiscovered Name: " + b.Name);
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "K: DeviceDiscovered Distance: " + b.Distance);			
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "K: DeviceDiscovered Meters: " + b.Meters);
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "K: DeviceDiscovered Major: " + b.Major);
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "K: DeviceDiscovered Minor: " + b.Minor);
					*/
							var key = UniqueKey(b);
							if (beaconDict.ContainsKey(key))
							{
								beaconDict[key] = b;
							}else{
								beaconDict.Add(key, b);
							}
							//beaconArray.Add (b);


						}

					
					};
				}

				StartRangingBeacons (UUID);
			});
		}

		private string UniqueKey(Appverse.Core.iBeacon.Beacon beacon)
		{
			return beacon.UUID + "-" + beacon.Major + "-" + beacon.Minor;
		}


		/// <summary>
		/// Starts the ranging beacons.
		/// </summary>
		private void StartRangingBeacons(string UUID)
		{


			var status = BeaconManager.AuthorizationStatus ();



			if (status == CLAuthorizationStatus.NotDetermined)
			{
				if (!UIDevice.CurrentDevice.CheckSystemVersion(8, 0)) {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "CLAuthorizationStatus.NotDetermined (under 8)");
					RangedBeacons(UUID);
				} else {
					beaconManager.RequestWhenInUseAuthorization ();
				}
			}
			else if(status == CLAuthorizationStatus.AuthorizedWhenInUse)
			{
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "CLAuthorizationStatus.AuthorizedWhenInUse");
				RangedBeacons(UUID);
			}
			else if(status == CLAuthorizationStatus.AuthorizedAlways)
			{
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "CLAuthorizationStatus.AuthorizedAlways");
				RangedBeacons(UUID);
			}/*
			else if(status == CLAuthorizationStatus.AuthorizedAlways)
			{
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "K: AuthorizedAlways " + UUID);
				RangedBeacons(UUID);
			}*/
			else if(status == CLAuthorizationStatus.Denied)
			{
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Denied " + UUID);


				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					BeaconUtils.FireUnityJavascriptEvent ("Appverse.Beacon.onAccessToLocationDenied", null);
				});
			}
			else if(status == CLAuthorizationStatus.Restricted)
			{
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Restricted " + UUID);

				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					BeaconUtils.FireUnityJavascriptEvent ("Appverse.Beacon.onAccessToLocationDenied", null);
				});
			}

		}


		/// <summary>
		/// Rangeds the beacons.
		/// </summary>
		/// <param name="UUID">UUI.</param>
		void RangedBeacons (string UUID)
		{
			
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "RangedBeacons UUID " + UUID);
			if (UUID != null) {
				var uuid = new NSUuid (UUID);
				region = new BeaconRegion (uuid, "BeaconSample");
			} else {
				region = new BeaconRegion (null, "BeaconSample");
			}

			beaconManager.StartRangingBeacons(region);
			//TODO should call other appverse listeners? 
			beaconDict.Clear();
			//beaconArray = new List<Appverse.Core.iBeacon.Beacon> ();


			try{
				// Declare a timer: same steps in C# and VB
				tmr = new Timer();
				tmr.AutoReset = false;
				tmr.Interval = 5000; // 0.1 second
				tmr.Elapsed += timerHandler; // We'll write it in a bit
				tmr.Start(); // The countdown is launched!
			}catch(Exception e){
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Could not create the timer, STOP mannually. Exception: " + e.Message);
			}


		}




		/// <summary>
		/// Stops the monitoring beacons.
		/// </summary>
		public void StopMonitoringBeacons ()
		{

			beaconManager.StopRangingBeacons (region);
			beaconManager.StopEstimoteBeaconDiscovery ();
		}
		#endregion

		void timerHandler (object sender, ElapsedEventArgs e)
		{
			try{
				SystemLogger.Log (SystemLogger.Module.PLATFORM,"ELAPSED!!!! beaconDict Size: "+beaconDict.Count);

				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					BeaconUtils.FireUnityJavascriptEvent ("Appverse.Beacon.OnEntered", new Object[]{ beaconDict.Values.ToArray()});
					beaconDict.Clear();
				});

				StopMonitoringBeacons ();
				tmr.Stop ();
				tmr.Close ();
			}catch(Exception ex){
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Could not create the timer, STOP mannually. Exception: " + ex.Message);
			}
		}






	}

}
