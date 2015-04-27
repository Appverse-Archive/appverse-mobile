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
using WebKit;
using Unity.Core.System;
using Unity.Platform.IPhone;

namespace UnityUI.iOS
{
	[Register ("AppDelegate_WKWebView")]
	public class AppDelegate_WKWebView : AppDelegate
	{
		public AppDelegate_WKWebView () : base()
		{
			#if DEBUG
			log ("AppDelegate_WKWebView constructor default");
			#endif
			
			loadApplicationPreferences();
		}



		public override bool ShouldActivateManagedServices () {
			// Managed services not needed using new WKWebView
			return false;
		}

		public override UnityUI_iOSViewController BindCustomViewController () {
			#if DEBUG
			log ("AppDelegate_WKWebView Binding custom ViewController using WKWebview (iOS 8)");
			#endif
			return new UnityUI_iOSViewController_WKWebView ();
		}

		public override void EvaluateJavascript (string jsStringToEvaluate) {
			#if DEBUG
			log ("AppDelegate_WKWebView EvaluateJavascript");
			#endif
			((UnityUI_iOSViewController_WKWebView) MainViewController ()).webView.EvaluateJavaScript (new NSString(jsStringToEvaluate), delegate (NSObject result, NSError error) {
				#if DEBUG
				log ("AppDelegate_WKWebView EvaluateJavascript completed");
				#endif
			});
		}
		public override void LoadRequest (NSUrlRequest request) {
			#if DEBUG
			log ("AppDelegate_WKWebView LoadRequest");
			#endif
			((UnityUI_iOSViewController_WKWebView) MainViewController ()).webView.LoadRequest (request);
		}

		public override void SetDataDetectorTypes() {
			// it is not possible to set detector types in iOS 8 WkWebView
		}

		public override void DetectWebViewLoadFinishedEvent (UIApplication application, NSDictionary launchOptions) {
			// detecting load finished event using the Navigation Delegate
			((UnityUI_iOSViewController_WKWebView) MainViewController ()).webView.NavigationDelegate = new AppverseWKNavigationDelegate (this, application, launchOptions);
		}
	}

	public class AppverseWKNavigationDelegate : WKNavigationDelegate
	{
		private AppDelegate _appDelegate;
		private UIApplication _application;
		private NSDictionary _launchOptions;

		public AppverseWKNavigationDelegate (AppDelegate appDelegate, UIApplication application, NSDictionary launchOptions) : base()
		{
			_application = application;
			_launchOptions = launchOptions;
			_appDelegate = appDelegate;
		}

		/*
		public override void DecidePolicy (WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
		{
			Console.WriteLine ("**** DecidePolicy****: " + navigationAction.Request);

			webView.LoadRequest (navigationAction.Request);  // testing, it blocks the request load

		}
		*/

		public override void DidFinishNavigation (WKWebView webView, WKNavigation navigation)
		{
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.GUI, "AppverseWKNavigationDelegate ************** WEBVIEW LOAD FINISHED");
			#endif
			if (_appDelegate != null && _application != null) {

				UIApplicationState applicationState = _application.ApplicationState;

				// inform other weak delegates (if exist) about the web view finished event
				IPhoneServiceLocator.WebViewLoadingFinished(applicationState, _launchOptions);

				// The NSDictionary options variable would contain any notification data if the user clicked the 'view' button on the notification
				// to launch the application. 
				// This method processes these options from the FinishedLaunching.
				_appDelegate.processLaunchOptions (_launchOptions, true, applicationState);

				// Processing extra data received when launched externally (using custom scheme url)
				_appDelegate.processLaunchData ();

			} else {
				#if DEBUG
				SystemLogger.Log (SystemLogger.Module.GUI, "AppverseWKNavigationDelegate  ************** Application Delegate is not accessible. Stop processing notifications and/or launch data");
				#endif
			}
		}

	}
}
