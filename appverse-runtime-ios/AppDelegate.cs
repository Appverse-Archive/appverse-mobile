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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Unity.Core.Notification;
using Unity.Core.System;
using Unity.Core.System.Launch;
using Unity.Core.System.Resource;
using Unity.Core.System.Server.Net;
using Unity.Core.System.Service;
using Unity.Platform.IPhone;
using System.Net;
using System.Runtime.InteropServices;

namespace UnityUI.iOS
{
	[Register ("AppDelegate")]
	public abstract partial class AppDelegate : IPhoneUIApplicationDelegate
	{
		private static string ROOT_PLIST_PATH = "/Settings.bundle/Root.plist";
		private static int DEFAULT_SERVER_PORT = 8080;
		private static string IPC_DEFAULT_PORT_KEY = "IPC_DefaultPort";
		private static NSDictionary httpServerSettings = null;
		private static HttpServer httpServer = null;
                
        private static string NOT_IMPORTANT_VARIABLE = "$replace_me$";
		private bool disableThumbnails = false;

		private List<LaunchData> launchData = null;
		private bool handledOpenUrl = false;

		// class-level declarations
		UIWindow window;
		UnityUI_iOSViewController viewController;

		public AppDelegate () : base()
		{
			#if DEBUG
			log ("AppDelegate constructor default");
			#endif
			
			loadApplicationPreferences();
		}

		public AppDelegate (IntPtr ptr) : base(ptr)
		{
			#if DEBUG
			log ("AppDelegate constructor IntPtr");
			#endif
		}

		public AppDelegate (NSCoder coder) : base(coder)
		{
			#if DEBUG
			log ("AppDelegate constructor NSCoder");
			#endif
		}

		public AppDelegate (NSObjectFlag flag) : base(flag)
		{
			#if DEBUG
			log ("AppDelegate constructor NSObjectFlag");
			#endif
		}


		//public abstract UnityViewController MainViewController ();
		
		public UnityUI_iOSViewController MainViewController ()
		{
			return this.viewController;
		}
		
		public override MonoTouch.UIKit.UIViewController MainUIViewController ()
		{
			return this.viewController;
		}
		
		public override MonoTouch.UIKit.UIWebView MainUIWebView ()
		{
			return MainViewController ().webView;
		}
		
		public override void SetMainUIViewControllerAsTopController(bool topController) {
			this.MainViewController ().SetAsTopController(topController);
		}
		
		public override bool ShowSplashScreen (UIInterfaceOrientation orientation) {
			return MainViewController().ShowSplashScreen(orientation);
		}
		
		public override bool DismissSplashScreen () {
			return MainViewController().DismissSplashScreen();
		}

		#if DEBUG
		protected void log (string message)
		{	
			SystemLogger.Log (SystemLogger.Module.GUI, "AppDelegate: " + message);
			
		}
		#endif

		private void loadApplicationPreferences() {
			try {
				var disableThumbnailskey = NSBundle.MainBundle.ObjectForInfoDictionary("Unity_DisableThumbnails");
				disableThumbnails = Convert.ToBoolean(Convert.ToInt32(""+disableThumbnailskey));
#if DEBUG
				log ("Disable Background Snapshot? " + disableThumbnails);
#endif
			} catch(Exception ex) {
#if DEBUG
				log ("Exception getting 'Unity_DisableThumbnails' from application preferences: " + ex.Message);
#endif
			}
		
		}

		public override void FinishedLaunching (UIApplication application)
		{
			#if DEBUG
			log ("FinishedLaunching");
			#endif
			//MainAppWindow ().AddSubview (MainViewController ().View);
			//MainAppWindow ().MakeKeyAndVisible ();

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new UnityUI_iOSViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			// do not detect data types automatically (phone links, etc)
			MainViewController().webView.DataDetectorTypes = UIDataDetectorType.None;

			InitializeUnity ();
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launcOptions)
		{
			#if DEBUG
			log ("FinishedLaunching with NSDictionary");
			#endif
			//MainAppWindow ().AddSubview (MainViewController ().View);
			//MainAppWindow ().MakeKeyAndVisible ();

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new UnityUI_iOSViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();
			
			// do not detect data types automatically (phone links, etc)
			MainViewController().webView.DataDetectorTypes = UIDataDetectorType.None;
			
			// remove all cache content (testing purposes)
			//NSUrlCache.SharedCache.RemoveAllCachedResponses();

			InitializeUnity ();
			
			UIApplicationState applicationState = application.ApplicationState;

			MainUIWebView().LoadFinished += delegate {
#if DEBUG
				log ("************** WEBVIEW LOAD FINISHED");
#endif
			// The NSDictionary options variable would contain any notification data if the user clicked the 'view' button on the notification
			// to launch the application. 
			// This method processes these options from the FinishedLaunching, as well as the ReceivedRemoteNotification methods.
				processNotification(launcOptions, true, applicationState);
			
				// Processing extra data received when launched externally (using custom scheme url)
				processLaunchData();

			};
			
			return true;
		}

		/// <summary>
		/// Processes the launch data received when launched externally (using custom scheme url).
		/// </summary>
		void processLaunchData() {
			if(handledOpenUrl) {
				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Unity.OnExternallyLaunched", launchData);
				handledOpenUrl = false;
				launchData = null;
			}
		}

		/// <summary>
		/// Processes the notification.
		/// </summary>
		/// <param name="options">Options.</param>
		/// <param name="fromFinishedLaunching">True if this method comes from the 'FinishedLaunching' delegated method</param>
		/// <param name="applicationState">The application state that received the remote notification</param>
		void processNotification(NSDictionary options, bool fromFinishedLaunching, UIApplicationState applicationState)
		{

			try {
				#if DEBUG
			log ("******* PROCESSING NOTIFICATION fromFinishedLaunching="+fromFinishedLaunching+". application state: "+ applicationState);
				#endif
				if (options != null) {

				// LOCAL NOTIFICATIONS

					UILocalNotification localNotif = (UILocalNotification)options.ObjectForKey (UIApplication.LaunchOptionsLocalNotificationKey);
					this.ProcessLocalNotification (applicationState, localNotif);

				// REMOTE NOTIFICATIONS
					if (fromFinishedLaunching) {
						NSDictionary remoteNotif = (NSDictionary)options.ObjectForKey (UIApplication.LaunchOptionsRemoteNotificationKey);
						this.ProcessRemoteNotification (remoteNotif, fromFinishedLaunching, applicationState);
				} else {
						this.ProcessRemoteNotification (options, fromFinishedLaunching, applicationState);
				}

			} else {
					#if DEBUG
				log ("******* NO launch options");
					#endif
				}
			} catch (System.Exception ex) {
				#if DEBUG
				log ("******* Unhandled exception when trying to process notification. fromFinishedLaunching[" + fromFinishedLaunching + "]. Exception message: " + ex.Message);
				#endif
			}

		}

		private void ProcessRemoteNotification(NSDictionary options, bool fromFinishedLaunching, UIApplicationState applicationState) {
   			
			//Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (options!=null && options.ContainsKey(new NSString("aps")))
		    {
#if DEBUG
				log (" ******* PROCESSING REMOTE NOTIFICATION Notification Payload received");
#endif
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
					
						#if DEBUG
						log ("******* PROCESSING NOTIFICATION Notification Payload contains an alert message. Type [" + alertType + "]");
						#endif
				}

		        //Extract the sound string
					if (aps.ContainsKey (new NSString ("sound")) && (aps [new NSString ("sound")].GetType() == typeof(NSString))) {
						sound = (aps [new NSString ("sound")] as NSString).ToString ();
						#if DEBUG
					log ("******* PROCESSING NOTIFICATION Notification Payload contains sound");
						#endif
				}

		        //Extract the badge
					if (aps.ContainsKey (new NSString ("badge")) && (aps [new NSString ("badge")].GetType() == typeof(NSObject))) {
						string badgeStr = (aps [new NSString ("badge")] as NSObject).ToString ();
						int.TryParse (badgeStr, out badge);
						#if DEBUG
					log ("******* PROCESSING NOTIFICATION Notification Payload contains a badge number: " + badge);
						#endif
		        }

		        //If this came from the ReceivedRemoteNotification while the app was running,
		        // we of course need to manually process things like the sound, badge, and alert.
					if (!fromFinishedLaunching && applicationState == UIApplicationState.Active) {

						#if DEBUG
					log ("******* PROCESSING NOTIFICATION app was running, so manually showing notification");
						#endif

					UIRemoteNotificationType enabledRemoteNotificationTypes = UIApplication.SharedApplication.EnabledRemoteNotificationTypes;

					bool alertEnabled = ((enabledRemoteNotificationTypes & UIRemoteNotificationType.Alert) == UIRemoteNotificationType.Alert);
					bool soundEnabled = ((enabledRemoteNotificationTypes & UIRemoteNotificationType.Sound) == UIRemoteNotificationType.Sound);
					bool badgeEnabled = ((enabledRemoteNotificationTypes & UIRemoteNotificationType.Badge) == UIRemoteNotificationType.Badge);

						#if DEBUG
					log ("******* PROCESSING NOTIFICATION types enabled: alert[" + alertEnabled+"], sound[" + soundEnabled + "], badge[" + badgeEnabled+ "]");
						#endif
		            //Manually set the badge in case this came from a remote notification sent while the app was open
					if (badgeEnabled) {
							this.UpdateApplicationIconBadgeNumber (badge);
					}

		            //Manually play the sound
						if (soundEnabled) {
							this.PlayNotificationSound (sound);
					}

		            //Manually show an alert
						if (alertEnabled) {
							this.ShowNotificationAlert ("Notification", alert);
		            }
		        }


					Dictionary<String,Object> customDic = IPhoneUtils.GetInstance ().ConvertToDictionary (new NSMutableDictionary (options));
					customDic.Remove ("aps"); // it is not needed to pass the "aps" (notification iOS data) inside the "custom data json string"
					notificationData.CustomDataJsonString = IPhoneUtils.GetInstance ().JSONSerialize (customDic);
					

				} catch (System.Exception ex) {
					#if DEBUG
					log (" ******* Unhanlded exception processing notification payload received. Exception message: " + ex.Message);
					#endif
				} finally {

				notificationData.AlertMessage = alert;
				notificationData.Badge = badge;
				notificationData.Sound = sound;

					IPhoneUtils.GetInstance ().FireUnityJavascriptEvent ("Unity.OnRemoteNotificationReceived", notificationData);
				}

			} else {
#if DEBUG
				log (" ******* NO Notification Payload received");
#endif
			}
		}

		/// <summary>
		/// Processes the local notification.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="localNotification">Local notification.</param>
		private void ProcessLocalNotification(UIApplicationState applicationState, UILocalNotification localNotification) {
			if(localNotification != null) {
#if DEBUG
				log ("******* Local NOTIFICATION received");
#endif

				if (applicationState == UIApplicationState.Active)
				{
					// we need to manually process the notification while application is running.
#if DEBUG
					log ("******* Application is running, manually showing notification");
#endif
					this.UpdateApplicationIconBadgeNumber(localNotification.ApplicationIconBadgeNumber);
					this.PlayNotificationSound(localNotification.SoundName);
					this.ShowNotificationAlert("Notification", localNotification.AlertBody);
				}
				
				NotificationData notificationData = new NotificationData();
				notificationData.AlertMessage = localNotification.AlertBody;
				notificationData.Badge = localNotification.ApplicationIconBadgeNumber;
				notificationData.Sound = localNotification.SoundName;
				
				if(localNotification.UserInfo != null) {
					Dictionary<String,Object> customDic = IPhoneUtils.GetInstance().ConvertToDictionary(new NSMutableDictionary(localNotification.UserInfo));
					notificationData.CustomDataJsonString = IPhoneUtils.GetInstance().JSONSerialize(customDic);
				}

				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Unity.OnLocalNotificationReceived", notificationData);
			}
		}

		/// <summary>
		/// Manually shows a notification alert.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		private void ShowNotificationAlert(string title, string message) {
			if (!string.IsNullOrEmpty(message))
			{
				UIAlertView avAlert = new UIAlertView(title, message, null, "OK", null);
				avAlert.Show();
			}
		}

		private void UpdateApplicationIconBadgeNumber(int badge) {
			if(badge >= 0) {
				UIApplication.SharedApplication.ApplicationIconBadgeNumber = badge;
			}
		}

		private void PlayNotificationSound (String soundName) {
			if (!string.IsNullOrEmpty(soundName))
			{
				// Assuming that the sound filename received (like sound.caf)
				// has been included in the project directory as a Content Build type.
				var soundObj = MonoTouch.AudioToolbox.SystemSound.FromFile(soundName);
				if(soundObj != null) {
					soundObj.PlaySystemSound();
				} else {
#if DEBUG
					log ("it was not able to play the specified sound: " + soundName);
#endif
				}
			}
		    }

		/// <summary>
		/// Sent to the delegate when a running application receives a local notification.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="localNotification">Local notification.</param>
		public override void ReceivedLocalNotification (UIApplication application, UILocalNotification localNotification) {
			this.ProcessLocalNotification(application.ApplicationState, localNotification);
		}

		/// <summary>
		/// Remote notification received.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="userInfo">User info.</param>
		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
		    // This method gets called whenever the app is already running and receives a push notification
		    // WE MUST HANDLE the notifications in this case.  Apple assumes if the app is running, it takes care of everything
		    // this includes setting the badge, playing a sound, etc.
		    processNotification(userInfo, false, application.ApplicationState);
		}

		/// <summary>
		/// Succcessful registration for remote notifications.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="deviceToken">Device token.</param>
		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			// The deviceToken is what the push notification server needs to send out a notification
			// to the device. Most times application needs to send the device Token to its servers when it has changed

#if DEBUG
			log ("Success registering for Remote Notifications");
#endif
			// ****** REMOVED "lastDeviceToken storage" feature. Marga 06/08/2013 . Platform will always call the JS listener; same behavior in all platforms ******

			// First, get the last device token we know of
			// string lastDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("deviceToken");
			
			//There's probably a better way to do this
			NSString strFormat = new NSString("%@");
			NSString newToken = new NSString(MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(new MonoTouch.ObjCRuntime.Class("NSString").Handle, new MonoTouch.ObjCRuntime.Selector("stringWithFormat:").Handle, strFormat.Handle, deviceToken.Handle));
			
			var newDeviceToken = newToken.ToString().Replace("<", "").Replace(">", "").Replace(" ", "");
#if DEBUG
			log ("Device token: " + newDeviceToken);
#endif
			// We only want to send the device token to the server if it hasn't changed since last time
			// no need to incur extra bandwidth by sending the device token every time
			// if (!newDeviceToken.Equals(lastDeviceToken))
			//{
				// Send the new device token to your application server
				// ****** REMOVED "lastDeviceToken storage" feature. Marga 06/08/2013 . Platform will always call the JS listener; same behavior in all platforms ******

				RegitrationToken registrationToken = new RegitrationToken();
				registrationToken.StringRepresentation = newDeviceToken;
				byte[] buffer = new byte[deviceToken.Length];
				Marshal.Copy(deviceToken.Bytes, buffer,0,buffer.Length);
				registrationToken.Binary = buffer;
				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Unity.OnRegisterForRemoteNotificationsSuccess", registrationToken);

				//Save the new device token for next application launch
				// NSUserDefaults.StandardUserDefaults.SetString(newDeviceToken, "deviceToken");
			//}
		}

		/// <summary>
		/// Failure when trying to register for remote notifications.
		/// </summary>
		/// <param name="application">Application.</param>
		/// <param name="error">Error.</param>
		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			//Registering for remote notifications failed for some reason
			//This is usually due to your provisioning profiles not being properly setup in your project options
			// or not having the right mobileprovision included on your device
			// or you may not have setup your app's product id to match the mobileprovision you made
			
#if DEBUG
			log ("Failed to Register for Remote Notifications: " + error.LocalizedDescription);
#endif
			RegistrationError registrationError = new RegistrationError();
			registrationError.Code = ""+ error.Code;
			registrationError.LocalizedDescription = error.LocalizedDescription;

			IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Unity.OnRegisterForRemoteNotificationsFailure", registrationError);
		}
		

		[Export("InitializeUnityView")]
		private void InitializeUnityView ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				//string basePath = Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location);
				//string applicationUrl = "file:///" + basePath + "//WebResources/www/index.html";
			
				// loading application using internal server
				string applicationUrl = "http://127.0.0.1:8080/WebResources/www/index.html";
			
				#if DEBUG
					log ("WebApp to load: " + applicationUrl);
				#endif
				try {
					MainViewController().loadWebView (Uri.EscapeUriString(applicationUrl));
				} catch (Exception ex) {
					#if DEBUG
					log ("Unable to load url [" + applicationUrl + "] on Unity View. Exception message: " + ex.Message);
					#endif
				}

			});
		}

		//[Export("InitializeUnity")]
		private void InitializeUnity ()
		{
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (InitializeUnityServer as ThreadStart);
				thread.Priority = ThreadPriority.AboveNormal;
				thread.Start ();
				
			}
		}

		[Export("InitializeUnityServer")]
		private void InitializeUnityServer ()
		{
			
				NSDictionary settings = loadSettings ();
				openSocketListener (settings);
				//InitializeUnityView ();
			using (var pool = new NSAutoreleasePool ()) {	
				Thread thread = new Thread (InitializeUnityView as ThreadStart);
				thread.Priority = ThreadPriority.BelowNormal;
				Thread.Sleep (100); // testing race condition when starting server and view load threads
				thread.Start ();
			}
		}
		
		[Export("NotifyEnterForeground")]
		private void NotifyEnterForeground ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				string script = "try{Unity._toForeground()}catch(e){}";
				#if DEBUG
				log ("NotifyJavascript: " + script);
				#endif
				try {
					MainViewController ().webView.EvaluateJavascript(script);
				} catch (Exception ex) {
					#if DEBUG
					log ("NotifyEnterForeground: Unable to execute javascript code: " + ex.Message);
					#endif
				}
				
				// Processing extra data received when launched externally (using custom scheme url)
				processLaunchData();
				
			});
			
		}
		
		[Export("NotifyEnterBackground")]
		private void NotifyEnterBackground ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				string script = "try{Unity._toBackground()}catch(e){}";
				#if DEBUG
				log ("NotifyJavascript: " + script);
				#endif
				try {
					MainViewController ().webView.EvaluateJavascript(script);
				} catch (Exception ex) {
					#if DEBUG
					log ("NotifyEnterBackground: Unable to execute javascript code: " + ex.Message);
					#endif
				}
				
			});
			
		}
		
		public override void OnActivated (UIApplication application)
		{
			#if DEBUG
			log ("OnActivated");
			#endif
			if (httpServer == null) {
				NSDictionary settings = loadSettings ();
				openSocketListener (settings);
			}
			
			/* do it better on "WillEnterForeground"
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (NotifyEnterForeground as ThreadStart);
				thread.Priority = ThreadPriority.BelowNormal;
				thread.Start ();
				
			}
			*/
		}
		
		public override void OnResignActivation (UIApplication application)
		{
			#if DEBUG
			log ("OnResignActivation");
			#endif
		}
		
		public override void DidEnterBackground (UIApplication application)
		{
			#if DEBUG
			log ("DidEnterBackground");
			#endif
			
			if(disableThumbnails) {
				// security reasons; the splash screen is shown when application enters in background (hiding sensitive data)
				// it will be dismissed on "WillEnterForeground" method
				UIInterfaceOrientation orientation =  UIApplication.SharedApplication.StatusBarOrientation;
				this.ShowSplashScreen(orientation);
			}
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (NotifyEnterBackground as ThreadStart);
				thread.Priority = ThreadPriority.BelowNormal;
				thread.Start ();
				
			}
			
			if (httpServer != null) {
				httpServer.Close ();
				httpServer = null;
			}
			
		}
		
		public override void WillEnterForeground (UIApplication application)
		{
			#if DEBUG
			log ("WillEnterForeground");
			#endif
			
			if(disableThumbnails) {
				// security reasons
				this.DismissSplashScreen();
			}
			
			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (NotifyEnterForeground as ThreadStart);
				thread.Priority = ThreadPriority.BelowNormal;
				thread.Start ();
				
			}
		}
		
		public override bool HandleOpenURL (UIApplication application, NSUrl url)
		{
			if (url == null) {
				handledOpenUrl = false;
				launchData = null;
				return false;
			}

			#if DEBUG
			log ("************************ HandleOpenURL -> " + url.AbsoluteString);
			#endif

			handledOpenUrl = true;

			launchData = new List<LaunchData>();

			#if DEBUG
			log ("host: " + url.Host);
			log ("query: " + url.Query);

			// other possible parameters
			//log ("path: " + url.Path);
			//log ("parameter string: " + url.ParameterString);
			//log ("fragment: " + url.Fragment);

			#endif

			if (url.Host != null) {
				launchData.Add (new LaunchData (LaunchConstants.LAUNCH_DATA_URI_SCHEME_PATH, url.Host));
		}

			if (url.Query != null) {
				string[] parts = url.Query.Split (new char[] { '&' });
				//log ("parts: " + parts.Length);
				for (var i=0; i<parts.Length; i++) {
					string[] parameters = parts[i].Split (new char[] { '=' });
		/*
					log ("parameters: " + parameters.Length);
					log ("parameter.Name: " + parameters[0]);
					log ("parameter.Value: " + parameters[1]);
             */
					launchData.Add (new LaunchData (parameters[0], parameters[1]));
				}
			}

			if (launchData.Count <= 0) {
				launchData = null;
			}

			return true;
		}

		public override void ReceiveMemoryWarning (UIApplication application)
		{
			#if DEBUG
			log ("ReceiveMemoryWarning");
			#endif
		}

		public override void WillTerminate (UIApplication application)
		{
			#if DEBUG
			log ("WillTerminate");
			#endif
			// Close Listener.
			if (httpServer != null) {
				httpServer.Close ();
				httpServer = null;
			}
			#if DEBUG
			log ("Internal Server Socket closed.");
			#endif
		}
		#if DEBUG
		public override void ApplicationSignificantTimeChange (UIApplication application)
		{
			log ("ApplicationSignificantTimeChange");
			// Async notification of event to framework
		}

		public override void WillChangeStatusBarOrientation (UIApplication application, UIInterfaceOrientation newStatusBarOrientation, double duration)
		{
			log ("WillChangeStatusBarOrientation");
			// Async notification of event to framework
		}

		public override void DidChangeStatusBarOrientation (UIApplication application, UIInterfaceOrientation oldStatusBarOrientation)
		{
			log ("DidChangeStatusBarOrientation");
			// Async notification of event to framework
		}

		public override void WillChangeStatusBarFrame (UIApplication application, RectangleF newStatusBarFrame)
		{
			log ("WillChangeStatusBarFrame");
			// Async notification of event to framework
		}

		public override void ChangedStatusBarFrame (UIApplication application, RectangleF oldStatusBarFrame)
		{
			log ("ChangedStatusBarFrame");
			// Async notification of event to framework
		}

		public override bool RespondsToSelector (MonoTouch.ObjCRuntime.Selector sel)
		{
			log ("RespondsToSelector -> " + sel.Name);
			return base.RespondsToSelector (sel);
			// Internal - nothing to do here.
		}

		public override void DoesNotRecognizeSelector (MonoTouch.ObjCRuntime.Selector sel)
		{
			log ("DoesNotRecognizeSelector -> " + sel.Name);
			// Internal - nothing to do here.
		}

		public override void PerformSelector (MonoTouch.ObjCRuntime.Selector sel, NSObject obj, float delay)
		{
			log ("PerformSelector");
			// Internal - nothing to do here.
		}

		public override void AwakeFromNib ()
		{
			log ("AwakeFromNib");
			// Internal - nothing to do here.
		}

		public override void EncodeTo (NSCoder coder)
		{
			log ("EncodeTo");
			// Internal - nothing to do here.
		}
		#endif

		/// <summary>
		/// Loads the default settings from custom PLIST file.
		/// </summary>
		protected NSDictionary loadSettings ()
		{
			if (httpServerSettings == null) {
				
				string rootSettingsFilePath = NSBundle.MainBundle.BundlePath + ROOT_PLIST_PATH;
				if (File.Exists (rootSettingsFilePath)) {
					httpServerSettings = NSDictionary.FromFile (rootSettingsFilePath);
				} else {
					httpServerSettings = new NSDictionary ();
				}
			}
			return httpServerSettings;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		protected static int GetIPCDefaultPort (NSDictionary settings)
		{
			int defaultIPCPort = DEFAULT_SERVER_PORT;
			try {
				defaultIPCPort = ((NSNumber)settings.ObjectForKey (new NSString (IPC_DEFAULT_PORT_KEY))).IntValue;
			} catch (Exception e) {
				#if DEBUG
				//log ("Default port for IPC not correctly configured on Root.plist. Please enter a valid '" + IPC_DEFAULT_PORT_KEY + "' key. Exception message: " + e.Message);
				#endif
			}
			return defaultIPCPort;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		protected bool openSocketListener (NSDictionary settings)
		{
			
			bool socketOpened = false;
			
			try {
				
				int defaultIPCPort = AppDelegate.GetIPCDefaultPort (settings);
				#if DEBUG
				log ("Opening Listener on port " + defaultIPCPort + "...");
				#endif
				if (httpServer == null) {
					httpServer = new HttpServer (new Server (defaultIPCPort));
				}
				socketOpened = true;
				
				#if DEBUG
				log ("Listener OPENED on port: " + httpServer.Server.Port);
				#endif
				// Adding Resource Handler.
				IPhoneResourceHandler resourceHandler = new IPhoneResourceHandler (ApplicationSource.FILE);
				//resourceHandler.Substitute = false;
				httpServer.Handlers.Add (resourceHandler);
				
				// Adding Service URI Handler.
				httpServer.Handlers.Add (new IPhoneServiceURIHandler (IPhoneServiceLocator.GetInstance ()));
				
				// Adding Remote Resource Handler.
				httpServer.Handlers.Add (new RemoteResourceHandler (IPhoneServiceLocator.GetInstance ()));
				
			} catch (Exception ex) {
				#if DEBUG
				log ("Cannot open internal server socket: " + ex.Message);
				#endif
			}
			
			return socketOpened;
			
		}
		
	}
}
