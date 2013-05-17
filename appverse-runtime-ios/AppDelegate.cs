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
			
			// The NSDictionary options variable would contain any notification data if the user clicked the 'view' button on the notification
			// to launch the application. 
			// This method processes these options from the FinishedLaunching, as well as the ReceivedRemoteNotification methods.
			processNotification(launcOptions, true, application);
			
			return true;
		}

		/// <summary>
		/// Processes the notification.
		/// </summary>
		/// <param name="options">Options.</param>
		/// <param name="fromFinishedLaunching">True if this method comes from the 'FinishedLaunching' delegated method</param>
		/// <param name="application">The application that received the remote notification</param>
		void processNotification(NSDictionary options, bool fromFinishedLaunching, UIApplication application)
		{
			UIApplicationState applicationState = application.ApplicationState;
#if DEBUG
			log ("******* PROCESSING NOTIFICATION fromFinishedLaunching="+fromFinishedLaunching+". application state: "+ applicationState);
#endif
   			
			//Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if ( options!=null  && options.ContainsKey(new NSString("aps")))
		    {
				#if DEBUG
				log (" ******* PROCESSING NOTIFICATION Notification Payload received");
				#endif
				
		        //Get the aps dictionary
		        NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

		        string alert = string.Empty;
		        string sound = string.Empty;
		        int badge = -1;

		        //Extract the alert text
		        //NOTE: Just for the simple alert specified by "  aps:{alert:"alert msg here"}  "
		        //      For complex alert with Localization keys, etc., the "alert" object from the aps dictionary
		        //      will be another NSDictionary... Basically the json gets dumped right into a NSDictionary, so keep that in mind
		        if (aps.ContainsKey(new NSString("alert"))) {
		            alert = (aps[new NSString("alert")] as NSString).ToString();
					#if DEBUG
					log ("******* PROCESSING NOTIFICATION Notification Payload contains an alert message");
					#endif
				}

		        //Extract the sound string
		        if (aps.ContainsKey(new NSString("sound"))) {
		            sound = (aps[new NSString("sound")] as NSString).ToString();
					#if DEBUG
					log ("******* PROCESSING NOTIFICATION Notification Payload contains sound");
					#endif
				}

		        //Extract the badge
		        if (aps.ContainsKey(new NSString("badge")))
		        {
		            string badgeStr = (aps[new NSString("badge")] as NSObject).ToString();
		            int.TryParse(badgeStr, out badge);
					#if DEBUG
					log ("******* PROCESSING NOTIFICATION Notification Payload contains a badge number: " + badge);
					#endif
		        }

		        //If this came from the ReceivedRemoteNotification while the app was running,
		        // we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching && applicationState == UIApplicationState.Active)
		        {

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
					if (badgeEnabled && badge >= 0)
		                UIApplication.SharedApplication.ApplicationIconBadgeNumber = badge;

		            //Manually play the sound
					if (soundEnabled && !string.IsNullOrEmpty(sound))
		            {
						//This assumes that in your json payload you sent the sound filename (like sound.caf)
		                // and that you've included it in your project directory as a Content Build type.
		                var soundObj = MonoTouch.AudioToolbox.SystemSound.FromFile(sound);
						if(soundObj != null) {
		                	soundObj.PlaySystemSound();
						} else {
#if DEBUG
							log ("it was not able to play the specified sound: " + sound);
#endif
						}
					}

		            //Manually show an alert
					if (alertEnabled && !string.IsNullOrEmpty(alert))
		            {
		                UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
		                avAlert.Show();
		            }
		        }

				NotificationData notificationData = new NotificationData();
				notificationData.AlertMessage = alert;
				notificationData.Badge = badge;
				notificationData.Sound = sound;

				Dictionary<String,Object> customDic = IPhoneUtils.GetInstance().ConvertToDictionary(new NSMutableDictionary(options));
				customDic.Remove ("aps"); // it is not needed to pass the "aps" (notification iOS data) inside the "custom data json string"
				notificationData.CustomDataJsonString = IPhoneUtils.GetInstance().JSONSerialize(customDic);
				
				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Unity.OnRemoteNotificationReceived", notificationData);

		    }

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
		    processNotification(userInfo, false, application);
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

			// First, get the last device token we know of
			string lastDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("deviceToken");
			
			//There's probably a better way to do this
			NSString strFormat = new NSString("%@");
			NSString newDeviceToken = new NSString(MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(new MonoTouch.ObjCRuntime.Class("NSString").Handle, new MonoTouch.ObjCRuntime.Selector("stringWithFormat:").Handle, strFormat.Handle, deviceToken.Handle));
#if DEBUG
			log ("New device token: " + newDeviceToken);
#endif
			// We only want to send the device token to the server if it hasn't changed since last time
			// no need to incur extra bandwidth by sending the device token every time
			if (!newDeviceToken.Equals(lastDeviceToken))
			{
				// Send the new device token to your application server

				RegitrationToken registrationToken = new RegitrationToken();
				registrationToken.StringRepresentation = newDeviceToken;
				byte[] buffer = new byte[deviceToken.Length];
				Marshal.Copy(deviceToken.Bytes, buffer,0,buffer.Length);
				registrationToken.Binary = buffer;
				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Unity.OnRegisterForRemoteNotificationsSuccess", registrationToken);

				//Save the new device token for next application launch
				NSUserDefaults.StandardUserDefaults.SetString(newDeviceToken, "deviceToken");
			}
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
				thread.Priority = ThreadPriority.BelowNormal;
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
				thread.Priority = ThreadPriority.AboveNormal;
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
		
		/* TODO :: latest monotouch version does not have the same method signature, please review
		public override void HandleOpenURL (UIApplication application, NSUrl url)
		{
			#if DEBUG
			log ("HandleOpenURL -> " + url.AbsoluteString);
			#endif
		}
                */
		/*
             * Here we will check what URL scheme is being requested.
             * if mailto: -> forward to operating system
             * if sms: -> forward to operating system
             * ...
             * if unity: -> keep the URL and load from the listener.
             */

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
