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

using System;
using AppsFlyer;
using Unity.Core;
using UIKit;
using Foundation;
using CoreBluetooth;
using System.Xml.Serialization;
using System.IO;

namespace Appverse.Platform.IPhone
{
	public class IPhoneAppsFlyer : WeakDelegateManager, IAppsFlyer
	{

		private AppsFlyerInitialization _initOptions;

		public IPhoneAppsFlyer ()
		{
			
		}

		#region IWeakDelegateManager implementation

		public override void FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "didFinishLaunchingWithOptions from AppDelegate. Initializing AppsFlyer...");

			this.Initialize (_initOptions);
		}

		public override string GetConfigFilePath ()
		{
			return "app/config/appsflyer-config.xml";	
		}


		public override void ConfigFileLoadedData (byte[] configData)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "IPhoneAppsFlyer Module Initialization... loading init options");

			try
			{   // FileStream to read the XML document.
				if (configData != null)
				{
					XmlSerializer serializer = new XmlSerializer(typeof(AppsFlyerInitialization));
					_initOptions = (AppsFlyerInitialization)serializer.Deserialize(new MemoryStream(configData));
				}
			}
			catch (Exception e)
			{
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error when loading appsflyers configuration", e);
			}
		}

		public override void OnActivated (UIApplication application)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "applicationDidBecomeActive from AppDelegate.. Tracking launch event AppsFlyer");

			this.TrackAppLaunch ();
		}

		#endregion


		#region IAppsFlyer implementation

		/// <summary>
		/// Initialize and Start the tracking system with the App ID and a possible customer app name.
		/// It is automatically called by the platform when launching the app 
		/// (no need to call it from the app code, at least any field needs to be changed at runtime)
		/// </summary>
		/// <param name="initOptions">AppsFlyer Initialization data.</param>
		public void Initialize (AppsFlyerInitialization initOptions)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer Initialize");

			try {
				if (initOptions == null) {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer Initialize - no init options provided. Cannot initiliaze module.");
					return;
				}

				SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer Initialize - DevKey [" + initOptions.DevKey + "]");
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer Initialize - AppleAppID [" + initOptions.AppID + "]");
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer Initialize - CustomerID [" + initOptions.CustomerUserID + "]");
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer Initialize - Currency [" + initOptions.CurrencyCode + "]");
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer Initialize - IsHTTPS [" + (initOptions.CommunicationsProtocol == CommunicationsProtocol.HTTPS) + "]");

				// Your unique developer ID, which is accessible from your account
				AppsFlyerTracker.SharedTracker.AppsFlyerDevKey = initOptions.DevKey;

				// Your iTunes App 
				AppsFlyerTracker.SharedTracker.AppleAppID = initOptions.AppID;

				if(initOptions.CustomerUserID != null) 
					AppsFlyerTracker.SharedTracker.CustomerUserID = initOptions.CustomerUserID;

				AppsFlyerTracker.SharedTracker.CurrencyCode = initOptions.CurrencyCode;

				//AppsFlyerTracker.SharedTracker.IsHTTPS = (initOptions.CommunicationsProtocol == CommunicationsProtocol.HTTPS) ;


			} catch (Exception ex) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer Initialize - exception catched: " + ex.Message, ex);
			}

		}


		/// <summary>
		/// Detects installations, sessions (app openings) and updates.
		/// It is automatically called by the platform when launching the app 
		/// (no need to call it from the app code, at least any field needs to be changed at runtime)
		/// </summary>
		public void TrackAppLaunch ()
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer TrackAppLaunch");

			try {

				AppsFlyerTracker.SharedTracker.TrackAppLaunch();

			} catch (Exception ex) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer TrackAppLaunch - exception catched: " + ex.Message, ex);
			}
		}


		/// <summary>
		/// Sends in-app events to the AppsFlyer analytics server
		/// </summary>
		/// <param name="trackEvent">AppsFlyerTrackEvent data to track</param>
		public void TrackEvent (AppsFlyerTrackEvent trackEvent)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer TrackEvent");

			if (trackEvent == null) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer TrackEvent - no track event data provided. Cannot send track event.");
				return;
			}

			try {

				AppsFlyerTracker.SharedTracker.TrackEvent(trackEvent.EventName, trackEvent.EventRevenueValue);

			} catch (Exception ex) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "AppsFlyer TrackEvent - exception catched: " + ex.Message, ex);
			}

		}
		#endregion
	}
}

