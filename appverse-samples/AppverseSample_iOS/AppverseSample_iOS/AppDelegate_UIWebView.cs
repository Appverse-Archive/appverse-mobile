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
using Unity.Platform.IPhone;

namespace UnityUI.iOS
{
	[Register ("AppDelegate_UIWebView")]
	public class AppDelegate_UIWebView : AppDelegate
	{
		public AppDelegate_UIWebView () : base()
		{
			#if DEBUG
			log ("AppDelegate_UIWebView constructor default");
			#endif
			
			loadApplicationPreferences();
		}

		public override bool ShouldActivateManagedServices () {
			// Managed services not used in UIWebView (using NSUrlProcotol to handle managed services
			return false;
		}

		public override UnityUI_iOSViewController BindCustomViewController () {
			#if DEBUG
			log ("AppDelegate_UIWebView Binding custom ViewController using UIWebview");
			#endif
			return new UnityUI_iOSViewController_UIWebView ();
		}

		public override void EvaluateJavascript (string jsStringToEvaluate) {
			#if DEBUG
			log ("AppDelegate_UIWebView EvaluateJavascript");
			#endif
			((UnityUI_iOSViewController_UIWebView) MainViewController ()).webView.EvaluateJavascript (jsStringToEvaluate);
		}
		public override void LoadRequest (NSUrlRequest request) {
			#if DEBUG
			log ("AppDelegate_UIWebView LoadRequest");
			#endif
			((UnityUI_iOSViewController_UIWebView) MainViewController ()).webView.LoadRequest (request);
		}

		public override void SetDataDetectorTypes() {

			// do not detect data types automatically (phone links, etc)
			((UnityUI_iOSViewController_UIWebView) MainViewController ()).webView.DataDetectorTypes = UIDataDetectorType.None;
		}

		public override void DetectWebViewLoadFinishedEvent (UIApplication application, NSDictionary launchOptions) {
		
			UIApplicationState applicationState = application.ApplicationState;
			((UnityUI_iOSViewController_UIWebView) MainViewController ()).webView.LoadFinished += delegate {
				#if DEBUG
			log ("************** WEBVIEW LOAD FINISHED");
				#endif

				if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
					UIView.AnimationsEnabled = true;  //enable again animation in all view  (see UnityUI_iOSViewController_UIWebView#loadWebView for details)
				}

				// inform other weak delegates (if exist) about the web view finished event
				IPhoneServiceLocator.WebViewLoadingFinished(applicationState, launchOptions);

				// The NSDictionary options variable would contain any notification data if the user clicked the 'view' button on the notification
				// to launch the application. 
				// This method processes these options from the FinishedLaunching.
				processLaunchOptions (launchOptions, true, applicationState);

				// Processing extra data received when launched externally (using custom scheme url)
				processLaunchData ();

			};
		}

	}
}
