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
using Foundation;
using UIKit;
using CoreGraphics;
using Unity.Core.I18N;
using Unity.Core.IO;
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
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Runtime.CompilerServices;

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
		private bool securityChecksPerfomed = false;
		private bool securityChecksPassed = false;
		private static string DEFAULT_LOCKED_HTML = "/app/config/error_rooted.html";

		//TO BE REMOVED public static string MAIN_HTML_PATTERN = "http://127.0.0.1:{0}/WebResources/www/index.html";
		public static string MAIN_HTML_PATTERN = IPhoneServiceLocator.APPVERSE_RESOURCE_URI + "index.html";

		private int previousListeningPort = 0;
		private bool socketOpened = false;
		private bool firstTimeOpening = true;

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
		
		public override UIKit.UIViewController MainUIViewController ()
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
				//TODO place here app prefference loading
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

			// inform other weak delegates (if exist) about the application finished launching event
			IPhoneServiceLocator.FinishedLaunching(application, null);

			//MainAppWindow ().AddSubview (MainViewController ().View);
			//MainAppWindow ().MakeKeyAndVisible ();

			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = this.BindCustomViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();

			this.SetDataDetectorTypes ();

			InitializeUnity ();
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			#if DEBUG
			log ("FinishedLaunching with NSDictionary");
			#endif

			// inform other weak delegates (if exist) about the application finished launching event
			IPhoneServiceLocator.FinishedLaunching(application, launchOptions);

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

			return true;
		}

		/// <summary>
		/// Processes the launch data received when launched externally (using custom scheme url).
		/// </summary>
		public void processLaunchData() {
#if DEBUG
			log ("************** Checking launch data... should handle open url? : " + handledOpenUrl + ", launchedData?: " + launchData);
#endif
			if(handledOpenUrl && launchData != null && launchData.Count > 0) {
				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.OnExternallyLaunched", launchData);
				handledOpenUrl = false;
				launchData = null;
			}
		}

		/// <summary>
		/// Processes any received launch options.
		/// </summary>
		/// <param name="options">Options.</param>
		/// <param name="fromFinishedLaunching">True if this method comes from the 'FinishedLaunching' delegated method</param>
		/// <param name="applicationState">The application state that received the special launch options</param>
		public void processLaunchOptions(NSDictionary options, bool fromFinishedLaunching, UIApplicationState applicationState)
		{

			try {
				#if DEBUG
				log ("******* Checking launch optioins (if available) fromFinishedLaunching="+fromFinishedLaunching+". application state: "+ applicationState);
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
					this.UpdateApplicationIconBadgeNumber((int)localNotification.ApplicationIconBadgeNumber);
					this.PlayNotificationSound(localNotification.SoundName);
					this.ShowNotificationAlert("Notification", localNotification.AlertBody);
				}
				
				NotificationData notificationData = new NotificationData();
				notificationData.AlertMessage = localNotification.AlertBody;
				notificationData.Badge = (int) localNotification.ApplicationIconBadgeNumber;
				notificationData.Sound = localNotification.SoundName;
				
				if(localNotification.UserInfo != null) {
					Dictionary<String,Object> customDic = IPhoneUtils.GetInstance().ConvertToDictionary(new NSMutableDictionary(localNotification.UserInfo));
					notificationData.CustomDataJsonString = IPhoneUtils.GetInstance().JSONSerialize(customDic);
				}

				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.OnLocalNotificationReceived", notificationData);
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
				UIApplication.SharedApplication.ApplicationIconBadgeNumber = (nint) badge;
			}
		}

		private void PlayNotificationSound (String soundName) {
			if (!string.IsNullOrEmpty(soundName))
			{
				// Assuming that the sound filename received (like sound.caf)
				// has been included in the project directory as a Content Build type.
				var soundObj = AudioToolbox.SystemSound.FromFile(soundName);
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
		/// Initializes the appverse context exposing data to the WebView Javascript DOM.
		/// </summary>
		private void InitializeAppverseContext () {

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			try {
				#if DEBUG
				log ("Before loading the main HTML, platform will expose some information directly to javascript...");
				#endif

				IPhoneSystem systemService = (IPhoneSystem)IPhoneServiceLocator.GetInstance ().GetService ("system");
				AbstractI18N i18nService = (AbstractI18N)IPhoneServiceLocator.GetInstance ().GetService ("i18n");
				IIo ioService = (IIo)IPhoneServiceLocator.GetInstance ().GetService ("io");

				// 1. Appverse Context (Appverse.is)
				UnityContext unityContext = systemService.GetUnityContext();
				String unityContextJsonString = IPhoneUtils.GetInstance().JSONSerializeObjectData(unityContext);
				unityContextJsonString = "_AppverseContext = " + unityContextJsonString;
				this.EvaluateJavascript (unityContextJsonString);

				// 2. OS Info (Appverse.OSInfo)
				OSInfo osInfo = systemService.GetOSInfo();
				String osInfoJsonString = IPhoneUtils.GetInstance().JSONSerializeObjectData(osInfo);
				osInfoJsonString = "_OSInfo = " + osInfoJsonString;
				this.EvaluateJavascript (osInfoJsonString);

				// 3. Hardware Info (Appverse.HardwareInfo)
				HardwareInfo hwInfo = systemService.GetOSHardwareInfo();
				String hwInfoJsonString = IPhoneUtils.GetInstance().JSONSerializeObjectData(hwInfo);
				hwInfoJsonString = "_HwInfo = " + hwInfoJsonString;
				this.EvaluateJavascript (hwInfoJsonString);

				// 4. Get all configured localized keys (Appverse.i18n)
				Unity.Core.I18N.Locale[] supportedLocales = i18nService.GetLocaleSupported ();
				String localizedStrings = "_i18n = {};  _i18n['default'] = '" + i18nService.DefaultLocale + "'; ";
				String localeLiterals = "";
				foreach(Unity.Core.I18N.Locale supportedLocale in supportedLocales) {
					ResourceLiteralDictionary literals = i18nService.GetResourceLiterals(supportedLocale);
					String literalsJsonString = IPhoneUtils.GetInstance().JSONSerializeObjectData(literals);
					localeLiterals = localeLiterals + " _i18n['" + supportedLocale.ToString() + "'] = " + literalsJsonString + "; ";
				}
				localizedStrings = localizedStrings + localeLiterals;
				this.EvaluateJavascript (localizedStrings);

				// 5. Current device locale
				Unity.Core.System.Locale currentLocale = systemService.GetLocaleCurrent();
				String currentLocaleJsonString = IPhoneUtils.GetInstance().JSONSerializeObjectData(currentLocale);
				currentLocaleJsonString = "_CurrentDeviceLocale = " + currentLocaleJsonString;
				this.EvaluateJavascript (currentLocaleJsonString);

				// 6. Configured IO services endpoints
				IOService[] services = ioService.GetServices();
				String servicesJsonString = "_IOServices = {}; ";
				foreach(IOService service in services) {
					String serviceJson = IPhoneUtils.GetInstance().JSONSerializeObjectData(service);
					servicesJsonString = servicesJsonString + " _IOServices['" + service.Name + "-" + IPhoneUtils.GetInstance().JSONSerializeObjectData(service.Type) + "'] = " + serviceJson + "; ";
				}
				this.EvaluateJavascript (servicesJsonString);

				IPhoneNet NetService = (IPhoneNet)IPhoneServiceLocator.GetInstance ().GetService ("net");
				NetService.CheckConnectivity();
				String netJsonString = "_NetworkStatus = "+NetService.getNetStatus();
				this.EvaluateJavascript (netJsonString);

			} catch (Exception ex) {
				#if DEBUG
				log ("Unable to load Appverse Context. Exception message: " + ex.Message);
				#endif
			}

			stopwatch.Stop();
			#if DEBUG
				log ("# Time elapsed initializing Appverse Context: "+ stopwatch.Elapsed);
			#endif
		}


		[Export("InitializeUnityView")]
		private void InitializeUnityView ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				//string basePath = IPhoneUtils.GetInstance().GetDefaultBasePath();
				//string applicationUrl = "file:///" + basePath + "//WebResources/www/index.html";
			
				// loading application using internal server
				string applicationUrl = AppDelegate.MAIN_HTML_PATTERN;

				int listeningPort = this.GetListeningPort();
				//TO BE REMOVED --> string applicationUrl = String.Format (AppDelegate.MAIN_HTML_PATTERN, listeningPort);
			
				#if DEBUG
					log ("WebApp to load: " + applicationUrl);
				#endif
				try {

					// initializing Appverse Javascript Context
					this.InitializeAppverseContext();

					//setting DOM variable specifying the current port listening
					this.EvaluateJavascript("try{ LOCAL_SERVER_PORT="+ listeningPort +"; }catch(e){}");

					MainViewController().loadWebView (Uri.EscapeUriString(applicationUrl));
				} catch (Exception ex) {
					#if DEBUG
					log ("Unable to load url [" + applicationUrl + "] on Appverse main view. Exception message: " + ex.Message);
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

			//TODO BLOCKROOTED
			//@@BLOCKROOTED@@



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
					string script = "try{Appverse._toForeground()}catch(e){}";
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
					string script = "try{Appverse._toBackground()}catch(e){}";
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

			IPhoneServiceLocator.UIApplicationWeakDelegate.OnActivated (application);
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


			//TODO THUMBNAILS 1
			//@@DISABLETHUMBNAILS_1@@



			using (var pool = new NSAutoreleasePool ()) {
				Thread thread = new Thread (NotifyEnterBackground as ThreadStart);
				thread.Priority = ThreadPriority.BelowNormal;
				thread.Start ();
				
			}

			this.closeOpenedSocketListener ();
			
		}
		
		public override void WillEnterForeground (UIApplication application)
		{
			#if DEBUG
			log ("WillEnterForeground");
			#endif


			//TODO THUMBNAILS 2
			//@@DISABLETHUMBNAILS_2@@


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

			IPhoneServiceLocator.UIApplicationWeakDelegate.WillTerminate (application);

			// Close Listener.
			this.closeOpenedSocketListener ();
		}

		/* ALLOW_PUSH_NOTIFICATIONS_START 

		public override void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings) {
			#if DEBUG
			log ("DidRegisterUserNotificationSettings");
			#endif

			IPhoneServiceLocator.UIApplicationWeakDelegate.DidRegisterUserNotificationSettings (application, notificationSettings);
		}

		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			#if DEBUG
			log("RegisteredForRemoteNotifications");
			#endif

			IPhoneServiceLocator.UIApplicationWeakDelegate.RegisteredForRemoteNotifications (application, deviceToken);
		}

		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			#if DEBUG
			log("FailedToRegisterForRemoteNotifications");
			#endif

			IPhoneServiceLocator.UIApplicationWeakDelegate.FailedToRegisterForRemoteNotifications (application, error);
		}


		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			#if DEBUG
			log ("ReceivedRemoteNotification");
			#endif

			IPhoneServiceLocator.UIApplicationWeakDelegate.ReceivedRemoteNotification (application, userInfo);
		}

		ALLOW_PUSH_NOTIFICATIONS_END */

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

		public override void WillChangeStatusBarFrame (UIApplication application, CGRect newStatusBarFrame)
		{
			log ("WillChangeStatusBarFrame");
			// Async notification of event to framework
		}

		public override void ChangedStatusBarFrame (UIApplication application, CGRect oldStatusBarFrame)
		{
			log ("ChangedStatusBarFrame");
			// Async notification of event to framework
		}

		public override bool RespondsToSelector (ObjCRuntime.Selector sel)
		{
			log ("RespondsToSelector -> " + sel.Name);
			return base.RespondsToSelector (sel);
			// Internal - nothing to do here.
		}

		public override void DoesNotRecognizeSelector (ObjCRuntime.Selector sel)
		{
			log ("DoesNotRecognizeSelector -> " + sel.Name);
			// Internal - nothing to do here.
		}

		public override void PerformSelector (ObjCRuntime.Selector sel, NSObject obj, double delay)
		{
			log ("PerformSelector");
			// Internal - nothing to do here.
		}

		public override void AwakeFromNib ()
		{
			log ("AwakeFromNib");
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
				defaultIPCPort = ((NSNumber)settings.ObjectForKey (new NSString (IPC_DEFAULT_PORT_KEY))).Int16Value;
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
		[MethodImpl(MethodImplOptions.Synchronized)]
		protected void openSocketListener (NSDictionary settings)
		{
			#if DEBUG
			log ("DO NOT OPEN SOCKET LISTENER");
			#endif
			/*
			try {
				#if DEBUG
				log ("[1] Checking for opening internal socket...");
				#endif
				if(!socketOpened && firstTimeOpening) {

					int defaultIPCPort = AppDelegate.GetIPCDefaultPort (settings);
					firstTimeOpening = false;
					#if DEBUG
					log ("[1] Starting opening internal socket...");
					#endif
					this.openSocketListener(defaultIPCPort);
				} else {
					#if DEBUG
					log ("[1] Socket Opening process already perfomed or in execution");
					#endif
				}

				if(this.previousListeningPort!=0 && httpServer!=null && httpServer.Server.Port != this.previousListeningPort) {
					// we should reload the view 
					#if DEBUG
					log ("Reloading view (different port used on restart)");
					#endif
					this.InitializeUnityView();
				}

				// store port used
				this.previousListeningPort = httpServer.Server.Port;
				
			} catch (Exception ex) {
				#if DEBUG
				log ("[1] Cannot open internal server socket: " + ex.Message);
				#endif
			}
			*/
		}

		public override int GetListeningPort() {

			if (httpServer != null && httpServer.Server != null) {
				return httpServer.Server.Port;
			}

			return DEFAULT_SERVER_PORT;
		}


		/// <summary>
		/// Open Socket listener on specific port
		/// </summary>
		protected void openSocketListener (int port)
		{

			try {

				#if DEBUG
				log ("[2] Opening Listener on port " + port + "...");
				#endif
				if (httpServer == null) {
					httpServer = new HttpServer (new Server (port));
				}
				socketOpened = true;

				#if DEBUG
				log ("[2] Listener OPENED on port: " + httpServer.Server.Port);
				#endif

				// Adding Resource Handler.
				IPhoneResourceHandler resourceHandler = new IPhoneResourceHandler (ApplicationSource.FILE);
				//resourceHandler.Substitute = false;
				httpServer.Handlers.Add (resourceHandler);
				httpServer.Handlers.Add (new RemoteResourceHandler (IPhoneServiceLocator.GetInstance ()));

			} catch (Exception ex) {
				#if DEBUG
				log ("[2] Cannot open internal server socket: " + ex.Message);
				#endif
				if(ex.GetType() == typeof(System.Net.Sockets.SocketException)) {
					System.Net.Sockets.SocketException socketException = ex as System.Net.Sockets.SocketException;
					//log ("[2] Socket exception: " + socketException.SocketErrorCode);
					if (socketException.SocketErrorCode == System.Net.Sockets.SocketError.AddressAlreadyInUse) {
						#if DEBUG
						log ("[2] Address already in use; trying with next port...");
						#endif
						this.openSocketListener (port + 1);
					}
				}
			}

		}

		/// <summary>
		/// 
		/// Closing Opened SocketListener
		/// </summary>
		private void closeOpenedSocketListener() {

			if (httpServer != null) {
				httpServer.Close ();
				httpServer = null;
				firstTimeOpening = true;
				socketOpened = false;

				#if DEBUG
				log ("Internal Server Socket closed.");
				#endif
			}

		}
		
	}
}
