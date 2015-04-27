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
using Unity.Core.Security;
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
		private bool blockRooted = false;
		private bool securityChecksPerfomed = false;
		private bool securityChecksPassed = false;
		private static string DEFAULT_LOCKED_HTML = "/app/config/error_rooted.html";

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

		public abstract UnityUI_iOSViewController BindCustomViewController ();
		public abstract void SetDataDetectorTypes();
		public abstract void DetectWebViewLoadFinishedEvent (UIApplication application, NSDictionary launchOptions);


		public UnityUI_iOSViewController MainViewController ()
		{
			return this.viewController;
		}

		public override MonoTouch.UIKit.UIViewController MainUIViewController ()
		{
			return this.viewController;
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

		public override bool SecurityChecksPassed() {
			return this.securityChecksPassed;
		}

		#if DEBUG
		protected void log (string message)
		{	
		SystemLogger.Log (SystemLogger.Module.GUI, "AppDelegate: " + message);

		}
		#endif

		public void loadApplicationPreferences() {
			try {
				var disableThumbnailskey = NSBundle.MainBundle.ObjectForInfoDictionary("Unity_DisableThumbnails");
				disableThumbnails = Convert.ToBoolean(Convert.ToInt32(""+disableThumbnailskey));

				var blockRootedkey = NSBundle.MainBundle.ObjectForInfoDictionary("Appverse_BlockRooted");
				blockRooted = Convert.ToBoolean(Convert.ToInt32(""+blockRootedkey));

				#if DEBUG
				log ("Disable Background Snapshot? " + disableThumbnails);
				log ("Should block jailbroken device? " + blockRooted);
				#endif
			} catch(Exception ex) {
				#if DEBUG
				log ("Exception getting application preferences: " + ex.Message);
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

			viewController = this.BindCustomViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			this.SetDataDetectorTypes ();

			InitializeUnity ();

			this.AdhocCustomization_FinishedLaunching ();

		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			#if DEBUG
			log ("FinishedLaunching with NSDictionary");
			#endif
			//MainAppWindow ().AddSubview (MainViewController ().View);
			//MainAppWindow ().MakeKeyAndVisible ();

			window = new UIWindow (UIScreen.MainScreen.Bounds);

			viewController = this.BindCustomViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			this.SetDataDetectorTypes ();

			// remove all cache content (testing purposes)
			//NSUrlCache.SharedCache.RemoveAllCachedResponses();

			InitializeUnity ();

			this.DetectWebViewLoadFinishedEvent(application, launchOptions);

			return this.AdhocCustomization_FinishedLaunching ();

		}

		private bool AdhocCustomization_FinishedLaunching() {
			#if DEBUG
			log ("************** AdhocCustomization_FinishedLaunching... Adform start tracking");
			#endif

			IPhoneUtils.GetInstance ().Adform_StartTracking ();

			#if DEBUG
			log ("************** AdhocCustomization_FinishedLaunching... Facebook Init Settings");
			#endif

			IPhoneUtils.GetInstance ().Facebook_InitSettings ();

			return IPhoneUtils.GetInstance ().Facebook_FinishedLaunching ();
		}

		private void AdhocCustomization_onActivated() {

			#if DEBUG
			log ("************** AdhocCustomization_onActivated... Facebook Activate App");
			#endif

			IPhoneUtils.GetInstance ().Facebook_ActivateApp ();
		}

		/// <summary>
		/// Processes the launch data received when launched externally (using custom scheme url).
		/// </summary>
		public void processLaunchData() {
			#if DEBUG
			log ("************** processLaunchData... should handle open url? : " + handledOpenUrl + ", launchedData?: " + launchData);
			#endif
			if(handledOpenUrl && launchData != null && launchData.Count > 0) {
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
		/// <param name="applicationState">The application state that received the notification</param>
		public void processNotification(NSDictionary options, bool fromFinishedLaunching, UIApplicationState applicationState)
		{

			try {
				#if DEBUG
				log ("******* PROCESSING NOTIFICATION fromFinishedLaunching="+fromFinishedLaunching+". application state: "+ applicationState);
				#endif
				if (options != null) {

					// LOCAL NOTIFICATIONS

					UILocalNotification localNotif = (UILocalNotification)options.ObjectForKey (UIApplication.LaunchOptionsLocalNotificationKey);
					this.ProcessLocalNotification (applicationState, localNotif);

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

			if (performSecurityChecks ()) {

				#if DEBUG
				log ("Security checks passed... initializing Appverse...");
				#endif

				using (var pool = new NSAutoreleasePool ()) {
					Thread thread = new Thread (InitializeUnityServer as ThreadStart);
					thread.Priority = ThreadPriority.AboveNormal;
					thread.Start ();

				}
			}
		}

		private bool performSecurityChecks() {


			if (securityChecksPerfomed) {
				#if DEBUG
				log ("security checks already performed");
				#endif
				return securityChecksPassed; // if security checks already performed, return
			}

			#if DEBUG
			log ("performing security checks...");
			#endif

			//  initialize variable
			securityChecksPassed = false;

			if (blockRooted) {

				#if DEBUG
				log ("Checking device jailbroken (this app is not allowed to run in those devices)... ");
				#endif

				ISecurity securityService = (ISecurity)IPhoneServiceLocator.GetInstance ().GetService ("security");
				bool IsDeviceModified = securityService.IsDeviceModified ();

				if (IsDeviceModified) {

					#if DEBUG
					log ("Device is jailbroken. Application is blocked as per build configuration demand");
					#endif

					UIApplication.SharedApplication.InvokeOnMainThread (delegate { 

						#if DEBUG
						log ("Loading error page...");
						#endif
						try {
							// loading error page from file system
							string basePath = Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location);
							string htmlErrorPageFile = "file://" + basePath + DEFAULT_LOCKED_HTML;

							MainViewController ().loadWebView(htmlErrorPageFile);

						} catch (Exception ex) {
							#if DEBUG
							log ("Unable to load error page on Appverse WebView. Exception message: " + ex.Message);
							#endif
						}

					});

					this.DismissSplashScreen();

				} else {
					securityChecksPassed = true;
					#if DEBUG
					log ("Device is NOT jailbroken.");
					#endif
				}

			} else { 
				securityChecksPassed = true;
				#if DEBUG
				log ("This app could be used in jailbroken devices");
				#endif
			}

			securityChecksPerfomed = true;
			return securityChecksPassed;
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
			if (performSecurityChecks ()) { // do not execute javascript on foreground if security checks failed
				UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
					string script = "try{Unity._toForeground()}catch(e){}";
					#if DEBUG
					log ("NotifyJavascript: " + script);
					#endif
					try {
						this.EvaluateJavascript (script);
					} catch (Exception ex) {
						#if DEBUG
						log ("NotifyEnterForeground: Unable to execute javascript code: " + ex.Message);
						#endif
					}

					// Processing extra data received when launched externally (using custom scheme url)
					processLaunchData ();

				});
			}

		}

		[Export("NotifyEnterBackground")]
		private void NotifyEnterBackground ()
		{
			if (performSecurityChecks ()) { // do not execute javascript on background if security checks failed
				UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
					string script = "try{Unity._toBackground()}catch(e){}";
					#if DEBUG
					log ("NotifyJavascript: " + script);
					#endif
					try {
						this.EvaluateJavascript (script);
					} catch (Exception ex) {
						#if DEBUG
						log ("NotifyEnterBackground: Unable to execute javascript code: " + ex.Message);
						#endif
					}

				});
			}

		}

		public override void OnActivated (UIApplication application)
		{
			#if DEBUG
			log ("OnActivated");
			#endif

			this.AdhocCustomization_onActivated ();

			if (httpServer == null && performSecurityChecks()) {  // do not open socket listener on foreground if security checks failed
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
				#if DEBUG
				log ("************************ HandleOpenURL without NULL .... resetting launch data");
				#endif
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
