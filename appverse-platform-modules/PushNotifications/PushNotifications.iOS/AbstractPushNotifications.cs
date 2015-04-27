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

namespace Appverse.Core.PushNotifications
{

	public enum RemoteNotificationType {
		NONE,
		BADGE,
		SOUND,
		ALERT,
		CONTENT_AVAILABILITY
	}

	public abstract class AbstractPushNotifications : IPushNotifications, IWeakDelegateManager
	{
		public AbstractPushNotifications ()
		{
		}

		#region IWeakDelegateManager implementation

		public void InitializeWeakDelegate ()
		{
			if (UIApplication.SharedApplication.WeakDelegate == null || !(UIApplication.SharedApplication.WeakDelegate is UIApplicationWeakDelegate)) {
				UIApplication.SharedApplication.WeakDelegate = new UIApplicationWeakDelegate ();
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "******************************************* Registering UIApplicationWeakDelegate for the current UIApplication (push notifications module)");
			}
		}

		public void WebViewLoadingFinished (UIApplicationState applicationState, NSDictionary options)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "******************************************* Detected WebView Loading Finished (processing launch options, if any)");
			UIApplicationWeakDelegate.processNotification (options, true, applicationState);
		}

		#endregion

		public abstract void RegisterForRemoteNotifications (string senderId, RemoteNotificationType[] types);

		public abstract void UnRegisterForRemoteNotifications ();
	}

	public static class SystemLogger
	{
		public enum Module
		{
			CORE,
			PLATFORM,
			GUI,
			GENERAL}
		;

		public static void Log (string message)
		{
			Log (Module.GENERAL, message, null);
		}

		public static void Log (string message, Exception ex)
		{
			Log (Module.GENERAL, message, ex);
		}

		public static void Log (Module module, string message)
		{
			Log (module, message, null);
		}

		public static void Log (Module module, string message, Exception ex)
		{
			#if DEBUG
			Console.WriteLine(module+": "+message);
			if (ex!=null) {
			Console.WriteLine(module+": Exception=["+ex.Message+"] Source=["+ex.Source+"]");
			Console.WriteLine(module+": Stacktrace ---------------------");
			Console.WriteLine(module+": "+ex.StackTrace);
			}	
			#endif
		}
	}
}

