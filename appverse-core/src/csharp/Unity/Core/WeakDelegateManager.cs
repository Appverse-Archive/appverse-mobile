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
using UIKit;
using Foundation;
using Unity.Core.System;

namespace Unity.Core
{
	public abstract class WeakDelegateManager : IWeakDelegateManager {

		public virtual void WebViewLoadingFinished (UIApplicationState applicationState, NSDictionary options)
		{
			SystemLogger.Log ("WebViewLoadingFinished event received. Override this method if you need to handle this event in the Appverse module");
		}

		public virtual void FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			SystemLogger.Log ("FinishedLaunching event received. Override this method if you need to handle this event in the Appverse module");
		}

		public virtual string GetConfigFilePath ()
		{
			SystemLogger.Log ("GetConfigFilePath method invoked. Override this method if you need to handle configuration files in the Appverse module");
			return null;
		}

		public virtual void ConfigFileLoadedData (byte[] configData)
		{
			SystemLogger.Log ("ConfigFileLoadedData method invoked. Override this method if you need to handle configuration files bytes data in the Appverse module");
		}

		public virtual void OnActivated (UIApplication application)
		{
			SystemLogger.Log ("OnActivated event received. Override this method if you need to handle this event in the Appverse module");
		}

		public virtual void WillTerminate (UIApplication application)
		{
			SystemLogger.Log ("WillTerminate event received. Override this method if you need to handle this event in the Appverse module");
		}

		public virtual void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings)
		{
			SystemLogger.Log ("DidRegisterUserNotificationSettings event received. Override this method if you need to handle this event in the Appverse module");
		}

		public virtual void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			SystemLogger.Log ("RegisteredForRemoteNotifications event received. Override this method if you need to handle this event in the Appverse module");
		}

		public virtual void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			SystemLogger.Log ("FailedToRegisterForRemoteNotifications event received. Override this method if you need to handle this event in the Appverse module");
		}

		public virtual void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			SystemLogger.Log ("ReceivedRemoteNotification event received. Override this method if you need to handle this event in the Appverse module");
		}
	}
}

