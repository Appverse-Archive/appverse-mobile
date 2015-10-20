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
using UIKit;
using System.Collections.Generic;
using Foundation;
using Unity.Core.IO.ScriptSerialization;
using WebKit;
using System.Xml;
using Unity.Core.System;

namespace Appverse.Platform.IPhone
{
	public class IPhonePushNotifications : AbstractPushNotifications
	{
		private static IDictionary<RemoteNotificationType, UIRemoteNotificationType> rnTypes = 
			new Dictionary<RemoteNotificationType, UIRemoteNotificationType> ();

		static IPhonePushNotifications() {
			rnTypes[RemoteNotificationType.NONE] = UIRemoteNotificationType.None;
			rnTypes[RemoteNotificationType.ALERT] = UIRemoteNotificationType.Alert;
			rnTypes[RemoteNotificationType.BADGE] = UIRemoteNotificationType.Badge;
			rnTypes[RemoteNotificationType.SOUND] = UIRemoteNotificationType.Sound;
			rnTypes[RemoteNotificationType.CONTENT_AVAILABILITY] = UIRemoteNotificationType.NewsstandContentAvailability;
		}

		public IPhonePushNotifications ()
		{
		}

		public override void RegisterForRemoteNotifications (string senderId, RemoteNotificationType[] types)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM,"Registering senderId ["+ senderId +"] for receiving  push notifications");

			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.None;
				try {
					if(types != null) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM,"Remote Notifications types enabled #num : " + types.Length);
						foreach(RemoteNotificationType notificationType in types) {
							notificationTypes = notificationTypes | rnTypes[notificationType] ;
						}
					}

					SystemLogger.Log(SystemLogger.Module.PLATFORM,"Remote Notifications types enabled: " + notificationTypes);
					if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
						UIUserNotificationType uiUserNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
						var settings = UIUserNotificationSettings.GetSettingsForTypes(uiUserNotificationTypes, new NSSet (new string[] {}));
						UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
					}else{

						//This tells our app to go ahead and ask the user for permission to use Push Notifications
						// You have to specify which types you want to ask permission for
						// Most apps just ask for them all and if they don't use one type, who cares
						UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
					}
				} catch(Exception e) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM,"Exception ocurred: " + e.Message);
				}
			});
		}


		public override void UnRegisterForRemoteNotifications ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				UIApplication.SharedApplication.UnregisterForRemoteNotifications();
				PushNotificationsUtils.FireUnityJavascriptEvent ("Appverse.PushNotifications.OnUnRegisterForRemoteNotificationsSuccess", null);
			});

		}


	}
}

