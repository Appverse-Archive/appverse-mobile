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
using Unity.Platform.IPhone;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.WebKit;
using Unity.Core.System;


namespace UnityUI.iOS
{
	public partial class UnityUI_iOSViewController_WKWebView : UnityUI_iOSViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		//: base (UserInterfaceIdiomIsPhone ? "MainStoryboard_iPhone" : "MainStoryboard_iPad", null)
		public UnityUI_iOSViewController_WKWebView ()	
		{
			log ("UnityUI_iOSViewController_WKWebView Constructor default.");

			Initialize ();
		}

		public override void loadWebView (string urlPath)
		{
			log ("UnityUI_iOSViewController_WKWebView loading web view contents");

			NSUrl url = new NSUrl (urlPath);
			//NSUrlRequest request = new NSUrlRequest(url, NSUrlRequestCachePolicy.ReturnCacheDataElseLoad, 3600.0);
			// FIX (16-09-2013) - testing iOS 7 apps; resources are not refreshed when installing a new app version on the top of a previous one
			// We need to remove the cache data loaded
			NSUrlRequest request = new NSUrlRequest(url		, NSUrlRequestCachePolicy.ReloadIgnoringLocalAndRemoteCacheData, 3600.0);


			if (this.webView != null) {
				if (urlPath.StartsWith ("file://")) {
					log ("UnityUI_iOSViewController_WKWebView loading local error pages as HTML string");

					string basePath = Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location);
					 
					urlPath = urlPath.Substring (("file://" + basePath + "/").Length);
					//log ("UnityUI_iOSViewController_WKWebView local url path: " + urlPath);

					string htmlErrorPageFile = IPhoneUtils.GetInstance().GetFileFullPath(urlPath);
					byte[] htmlErrorPageBytes = IPhoneUtils.GetInstance().GetResourceAsBinary(htmlErrorPageFile, true);

					NSData htmlErrorPageData = NSData.FromArray(htmlErrorPageBytes);
					NSString htmlErrorPageString = new NSString (htmlErrorPageData, NSStringEncoding.UTF8);

					this.webView.LoadHtmlString (htmlErrorPageString, new NSUrl ("/"));

				} else {
					this.webView.LoadRequest (request);
				}
			} else {
				log ("WebView is null.");
			}

		}

		public override void EvaluateJavascript (string jsToEvaluate) {
			if (this.webView != null) {
				this.webView.EvaluateJavaScript (new NSString(jsToEvaluate), delegate (NSObject result, NSError error) {
					log ("UnityUI_iOSViewController_WKWebView EvaluateJavascript completed");
				});
			}
		}

		public override void CustomizeWebView() {
			WKWebViewConfiguration config = new WKWebViewConfiguration ();
			// UIScreen screen = UIScreen.MainScreen;
			// RectangleF viewFrame = new RectangleF (new PointF (0, 0), new SizeF (320, 480));
			this.webView = new WKWebView (this.View.Frame, config);
			//log ("************************* UnityUI_iOSViewController_WKWebView this.View.Frame.Height: " + this.View.Frame.Height);
			this.View.AddSubview (this.webView);
		}

		public override void ApplyAutoresizingMask () {
			//log ("************************* UnityUI_iOSViewController_WKWebView Appying autoresizing mask to ALL");
			this.webView.Frame = this.View.Frame;  // it is required to properly resize the frame in the WKWebview
			this.webView.AutoresizingMask = UIViewAutoresizing.All;
		}
	}
}

