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
using Appverse.Core.PushNotifications;
using Unity.Core;
using UIKit;
using Foundation;
using System.Runtime.InteropServices;
using Unity.Core.Notification;
using System.Collections.Generic;
using Unity.Core.System;

namespace Appverse.Core.PushNotifications
{

	public enum RemoteNotificationType {
		NONE,
		BADGE,
		SOUND,
		ALERT,
		CONTENT_AVAILABILITY
	}

	public abstract class AbstractPushNotifications : WeakDelegateManager, IPushNotifications
	{
		public AbstractPushNotifications ()
		{
		}

		#region IWeakDelegateManager implementation

		public override void WebViewLoadingFinished (UIApplicationState applicationState, NSDictionary options)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "******************************************* Detected WebView Loading Finished (processing launch options, if any)");
			AbstractPushNotifications.processNotification (options, true, applicationState);
		}

		public override void FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			//throw new NotImplementedException ();
		}

		/**
		 * WEAK DELEGATE METHODS 
		 */

		public override void OnActivated (UIApplication application) {
			// bnothing to do in this module for applicationDidBecomeActive event
			//SystemLogger.Log (SystemLogger.Module.PLATFORM, "applicationDidBecomeActive from AppDelegate...");
		}

		public override void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings)
		{ 
			//application.RegisteredForRemoteNotifications (application);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "DidRegisterUserNotificationSettings from AppDelegate..."); 
			application.RegisterForRemoteNotifications (); 
		}

		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			// The deviceToken is what the push notification server needs to send out a notification
			// to the device. Most times application needs to send the device Token to its servers when it has changed

			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Success registering for Remote Notifications");

			// ****** REMOVED "lastDeviceToken storage" feature. Marga 06/08/2013 . Platform will always call the JS listener; same behavior in all platforms ******

			// First, get the last device token we know of
			// string lastDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("deviceToken");

			//There's probably a better way to do this
			//NSString strFormat = new NSString("%@");
			//NSString newToken = new NSString(ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(new ObjCRuntime.Class("NSString").Handle, new ObjCRuntime.Selector("stringWithFormat:").Handle, strFormat.Handle, deviceToken.Handle));

			NSString newToken = new NSString (deviceToken.ToString ());

			var newDeviceToken = newToken.ToString().Replace("<", "").Replace(">", "").Replace(" ", "");
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Device token: " + newDeviceToken);

			// We only want to send the device token to the server if it hasn't changed since last time
			// no need to incur extra bandwidth by sending the device token every time
			// if (!newDeviceToken.Equals(lastDeviceToken))
			//{
			// Send the new device token to your application server
			// ****** REMOVED "lastDeviceToken storage" feature. Marga 06/08/2013 . Platform will always call the JS listener; same behavior in all platforms ******

			RegistrationToken registrationToken = new RegistrationToken();
			registrationToken.StringRepresentation = newDeviceToken;
			byte[] buffer = new byte[deviceToken.Length];
			Marshal.Copy(deviceToken.Bytes, buffer,0,buffer.Length);
			registrationToken.Binary = buffer;
			PushNotificationsUtils.FireUnityJavascriptEvent("Appverse.PushNotifications.OnRegisterForRemoteNotificationsSuccess", registrationToken);

			//Save the new device token for next application launch
			// NSUserDefaults.StandardUserDefaults.SetString(newDeviceToken, "deviceToken");
		}


		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			//Registering for remote notifications failed for some reason
			//This is usually due to your provisioning profiles not being properly setup in your project options
			// or not having the right mobileprovision included on your device
			// or you may not have setup your app's product id to match the mobileprovision you made

			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Failed to Register for Remote Notifications: " + error.LocalizedDescription);

			RegistrationError registrationError = new RegistrationError();
			registrationError.Code = ""+ error.Code;
			registrationError.LocalizedDescription = error.LocalizedDescription;

			PushNotificationsUtils.FireUnityJavascriptEvent("Appverse.PushNotifications.OnRegisterForRemoteNotificationsFailure", registrationError);
		}

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "Received Remote Notification (application is in background): processing data...");

			// This method gets called whenever the app is already running and receives a push notification
			// WE MUST HANDLE the notifications in this case.  Apple assumes if the app is running, it takes care of everything
			// this includes setting the badge, playing a sound, etc.
			processNotification(userInfo, false, application.ApplicationState);

		}

		/*
		public override void DidReceiveRemoteNotification (UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler) {
			// This method is part of iOS 7.0 new remote notification support. 
			// This method is invoked if your Entitlements list the "remote-notification" background operation is set, and you receive a remote notification.
			// Upon completion, you must notify the operating system of the result of the method by invoking the provided callback.
			// Important: failure to call the provided callback method with the result code before this method completes will cause your application to be terminated.
			log ("DidReceiveRemoteNotification: processing data..." );
			processNotification(userInfo, false, application.ApplicationState);
		}
		*/

		#endregion

		public abstract void RegisterForRemoteNotifications (string senderId, RemoteNotificationType[] types);

		public abstract void UnRegisterForRemoteNotifications ();





		/// <summary>
		/// Processes the notification.
		/// </summary>
		/// <param name="options">Options.</param>
		/// <param name="fromFinishedLaunching">True if this method comes from the 'FinishedLaunching' delegated method</param>
		/// <param name="applicationState">The application state that received the remote notification</param>
		public static void processNotification(NSDictionary options, bool fromFinishedLaunching, UIApplicationState applicationState)
		{

			try {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "******* Checking for PUSH NOTIFICATION data in launch options - fromFinishedLaunching="+fromFinishedLaunching+". application state: "+ applicationState);

				if (options != null) {

					if (fromFinishedLaunching) {
						NSDictionary remoteNotif = (NSDictionary)options.ObjectForKey (UIApplication.LaunchOptionsRemoteNotificationKey);
						ProcessRemoteNotification (remoteNotif, fromFinishedLaunching, applicationState);
					} else {
						ProcessRemoteNotification (options, fromFinishedLaunching, applicationState);
					}

				} else {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "******* NO launch options");

				}
			} catch (System.Exception ex) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "******* Unhandled exception when trying to process notification. fromFinishedLaunching[" + fromFinishedLaunching + "]. Exception message: " + ex.Message);
			}

		}

		private static void ProcessRemoteNotification(NSDictionary options, bool fromFinishedLaunching, UIApplicationState applicationState) {

			//Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (options!=null && options.ContainsKey(new NSString("aps")))
			{
				SystemLogger.Log (SystemLogger.Module.PLATFORM, " ******* PROCESSING REMOTE NOTIFICATION Notification Payload received");

				NotificationData notificationData = new NotificationData ();
				string alert = string.Empty;
				string sound = string.Empty;
				int badge = -1;

				try {
					//Get the aps dictionary
					NSDictionary aps = options.ObjectForKey (new NSString ("aps")) as NSDictionary;


					//Extract the alert text
					//NOTE: Just for the simple alert specified by "  aps:{alert:"alert msg here"}  "
					//      For complex alert with Localization keys, etc., the "alert" object from the aps dictionary
					//      will be another NSDictionary... Basically the json gets dumped right into a NSDictionary, so keep that in mind
					if (aps.ContainsKey (new NSString ("alert"))) {
						string alertType = "undefined";
						if (aps[new NSString ("alert")].GetType () == typeof(NSString)) {
							alert = (aps [new NSString ("alert")] as NSString).ToString ();
							alertType = "NSString";
						} else if (aps [new NSString ("alert")].GetType () == typeof(NSDictionary)) {
							NSDictionary alertNSDictionary = aps.ObjectForKey (new NSString ("alert")) as NSDictionary;
							alertType = "NSDictionary";
							// We only get "body" key from that dictionary
							if (alertNSDictionary.ContainsKey (new NSString ("body")) 
								&& (alertNSDictionary[new NSString ("body")].GetType () == typeof(NSString))) {
								alert = (alertNSDictionary [new NSString ("body")] as NSString).ToString ();
							}
						}

						SystemLogger.Log (SystemLogger.Module.PLATFORM, "******* PROCESSING NOTIFICATION Notification Payload contains an alert message. Type [" + alertType + "]");

					}

					//Extract the sound string
					if (aps.ContainsKey (new NSString ("sound")) && (aps [new NSString ("sound")].GetType() == typeof(NSString))) {
						sound = (aps [new NSString ("sound")] as NSString).ToString ();
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "******* PROCESSING NOTIFICATION Notification Payload contains sound");
					
					}

					//Extract the badge
					if (aps.ContainsKey (new NSString ("badge")) && (aps [new NSString ("badge")].GetType() == typeof(NSObject))) {
						string badgeStr = (aps [new NSString ("badge")] as NSObject).ToString ();
						int.TryParse (badgeStr, out badge);
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "******* PROCESSING NOTIFICATION Notification Payload contains a badge number: " + badge);

					}

					//If this came from the ReceivedRemoteNotification while the app was running,
					// we of course need to manually process things like the sound, badge, and alert.
					if (!fromFinishedLaunching && applicationState == UIApplicationState.Active) {

						SystemLogger.Log (SystemLogger.Module.PLATFORM, "******* PROCESSING NOTIFICATION app was running, so manually showing notification");

						UIRemoteNotificationType enabledRemoteNotificationTypes = UIApplication.SharedApplication.EnabledRemoteNotificationTypes;

						bool alertEnabled = ((enabledRemoteNotificationTypes & UIRemoteNotificationType.Alert) == UIRemoteNotificationType.Alert);
						bool soundEnabled = ((enabledRemoteNotificationTypes & UIRemoteNotificationType.Sound) == UIRemoteNotificationType.Sound);
						bool badgeEnabled = ((enabledRemoteNotificationTypes & UIRemoteNotificationType.Badge) == UIRemoteNotificationType.Badge);

						SystemLogger.Log (SystemLogger.Module.PLATFORM, "******* PROCESSING NOTIFICATION types enabled: alert[" + alertEnabled+"], sound[" + soundEnabled + "], badge[" + badgeEnabled+ "]");

						//Manually set the badge in case this came from a remote notification sent while the app was open
						if (badgeEnabled) {
							UpdateApplicationIconBadgeNumber (badge);
						}

						//Manually play the sound
						if (soundEnabled) {
							PlayNotificationSound (sound);
						}

						//Manually show an alert
						if (alertEnabled) {
							ShowNotificationAlert ("Notification", alert);
						}
					}


					Dictionary<String,Object> customDic = PushNotificationsUtils.ConvertToDictionary (new NSMutableDictionary (options));
					customDic.Remove ("aps"); // it is not needed to pass the "aps" (notification iOS data) inside the "custom data json string"
					notificationData.CustomDataJsonString = PushNotificationsUtils.JSONSerialize (customDic);


				} catch (System.Exception ex) {
					SystemLogger.Log (SystemLogger.Module.PLATFORM,  "******* Unhanlded exception processing notification payload received. Exception message: " + ex.Message);

				} finally {

					notificationData.AlertMessage = alert;
					notificationData.Badge = badge;
					notificationData.Sound = sound;

					PushNotificationsUtils.FireUnityJavascriptEvent ("Appverse.PushNotifications.OnRemoteNotificationReceived", notificationData);
				}

			} else {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, " ******* NO Notification Payload received");
			}
		}

		private static void UpdateApplicationIconBadgeNumber(int badge) {
			if(badge >= 0) {
				UIApplication.SharedApplication.ApplicationIconBadgeNumber = (nint) badge;
			}
		}

		private static void PlayNotificationSound (String soundName) {
			if (!string.IsNullOrEmpty(soundName))
			{
				// Assuming that the sound filename received (like sound.caf)
				// has been included in the project directory as a Content Build type.
				var soundObj = AudioToolbox.SystemSound.FromFile(soundName);
				if(soundObj != null) {
					soundObj.PlaySystemSound();
				} else {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "it was not able to play the specified sound: " + soundName);
				}
			}
		}

		/// <summary>
		/// Manually shows a notification alert.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		private static void ShowNotificationAlert(string title, string message) {
			if (!string.IsNullOrEmpty(message))
			{
				UIAlertView avAlert = new UIAlertView(title, message, null, "OK", null);
				avAlert.Show();
			}
		}
	}
}

