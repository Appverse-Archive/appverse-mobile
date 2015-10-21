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
using System;
using Foundation;
using UIKit;
using Unity.Core.System;
using System.Collections.Generic;
using Unity.Core;

namespace Unity.Platform.IPhone
{
	public class UIApplicationWeakDelegate //: NSObject
	{


		public UIApplicationWeakDelegate ()
		{
			#if DEBUG
			log("Initializing UIApplicationWeakDelegate as application weak delegate");
			#endif
		}
		

		private Dictionary<string, IWeakDelegateManager> weakDelegates = new Dictionary<string, IWeakDelegateManager>();

		public void RegisterWeakDelegate(IWeakDelegateManager weakDelegate, string key)
		{
			if (key != null && weakDelegate != null)
			{
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "******************************************* Registering UIApplicationWeakDelegate for '" + key + "' module");
				weakDelegates[key] = weakDelegate;
			}
		}


		public void OnActivated (UIApplication application) {
			#if DEBUG
			log("OnActivated");
			#endif

			// send event to registered weak delegates
			var wDelegatesEnumerator = weakDelegates.GetEnumerator ();
			while (wDelegatesEnumerator.MoveNext())
			{
				IWeakDelegateManager weakDelegate = wDelegatesEnumerator.Current.Value;
				weakDelegate.OnActivated (application);
			}
		}

		public void WillTerminate (UIApplication application) {
			#if DEBUG
			log("WillTerminate");
			#endif

			// send event to registered weak delegates
			var wDelegatesEnumerator = weakDelegates.GetEnumerator ();
			while (wDelegatesEnumerator.MoveNext())
			{
				IWeakDelegateManager weakDelegate = wDelegatesEnumerator.Current.Value;
				weakDelegate.WillTerminate (application);
			}
		}

		/// <summary>
		/// Dids the register user notification settings.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="notificationSettings">Notification settings.</param>
		public void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings)
		{ 	
			#if DEBUG
			log("DidRegisterUserNotificationSettings");
			#endif

			// send event to registered weak delegates
			var wDelegatesEnumerator = weakDelegates.GetEnumerator ();
			while (wDelegatesEnumerator.MoveNext())
			{
				IWeakDelegateManager weakDelegate = wDelegatesEnumerator.Current.Value;
				weakDelegate.DidRegisterUserNotificationSettings (application, notificationSettings);
			}
		}

		/// <summary>
		/// Succcessful registration for remote notifications.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="deviceToken">Device token.</param>
		public void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			#if DEBUG
			log("RegisteredForRemoteNotifications");
			#endif

			// send event to registered weak delegates
			var wDelegatesEnumerator = weakDelegates.GetEnumerator ();
			while (wDelegatesEnumerator.MoveNext())
			{
				IWeakDelegateManager weakDelegate = wDelegatesEnumerator.Current.Value;
				weakDelegate.RegisteredForRemoteNotifications (application, deviceToken);
			}
		}


		/// <summary>
		/// Failure when trying to register for remote notifications.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="error">Error.</param>
		public void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			#if DEBUG
			log("FailedToRegisterForRemoteNotifications");
			#endif

			// send event to registered weak delegates
			var wDelegatesEnumerator = weakDelegates.GetEnumerator ();
			while (wDelegatesEnumerator.MoveNext())
			{
				IWeakDelegateManager weakDelegate = wDelegatesEnumerator.Current.Value;
				weakDelegate.FailedToRegisterForRemoteNotifications (application, error);
			}
		}

		/// <summary>
		/// Remote notification received.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="userInfo">User info.</param>
		public void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			#if DEBUG
			log ("ReceivedRemoteNotification");
			#endif

			// send event to registered weak delegates
			var wDelegatesEnumerator = weakDelegates.GetEnumerator ();
			while (wDelegatesEnumerator.MoveNext())
			{
				IWeakDelegateManager weakDelegate = wDelegatesEnumerator.Current.Value;
				weakDelegate.ReceivedRemoteNotification (application, userInfo);
			}

		}

		private static void log (string message)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "UIApplicationWeakDelegate#" + message);

		}


	}
}

