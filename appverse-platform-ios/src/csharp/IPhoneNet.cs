/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://www.appverse.mobi/licenses/apl_v2.0.pdf.

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
using System.Collections.Generic;
using System.Text;
using Unity.Core.Net;
using MonoTouch.SystemConfiguration;
using System.Net;
using MonoTouch.Foundation;
using System.Threading;
using MonoTouch.UIKit;
using Unity.Core.Notification;
using Unity.Core.System;

namespace Unity.Platform.IPhone
{
    public class IPhoneNet : AbstractNet
    {
		/// <summary>
		/// Returns supported NetworkTypes (for iphone/ipad, always Wifi and 3G) 
		/// </summary>
		/// <returns>
		/// A <see cref="NetworkType[]"/>
		/// </returns>
        public override NetworkType[] GetNetworkTypeSupported()
        {
            return new NetworkType[]{ NetworkType.Carrier_3G, NetworkType.Wifi};
        }

		/// <summary>
		/// Returns an ordered list of netwrok types through which url is reachable by.
		/// </summary>
		/// <param name="url">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="NetworkType[]"/>
		/// </returns>
        public override NetworkType[] GetNetworkTypeReachableList(string url)
        {
            NetworkType networkTypeReachable = RemoteHostStatus(url);
			if(networkTypeReachable != NetworkType.Unknown)
			{
				if(networkTypeReachable == NetworkType.Wifi) {
					if(InternetConnectionStatus() == NetworkType.Carrier_3G) {
						return new NetworkType[]{ NetworkType.Wifi, NetworkType.Carrier_3G}; // reachable by wifi and 3G
					} else {
						return new NetworkType[]{ NetworkType.Wifi}; // reachable only by wifi
					}
				} 
				else 
				{
					if(LocalWifiConnectionStatus() == NetworkType.Wifi) {
						return new NetworkType[]{ NetworkType.Carrier_3G, NetworkType.Wifi}; // reachable by 3G and wifi
					} else {
						return new NetworkType[]{ NetworkType.Carrier_3G}; // only reachable by 3G
					}
				}
				
			} else 
			{
				// url is not reachable, return empty list;
				return new NetworkType[]{};
			}
        }
		
		
		/// <summary>
		/// Is the host reachable with the current network configuration
		/// </summary>
		/// <param name="host">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private static bool IsHostReachable (string host)
		{
			if (host == null || host.Length == 0)
				return false;
	
			using (var r = new NetworkReachability (host)){
				NetworkReachabilityFlags flags;
				
				if (r.TryGetFlags (out flags)){
					return IsReachable (flags) && IsNoConnectionRequired(flags);  // is reachable without requiring connection.
				}
			}
			return false;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="flags">
		/// A <see cref="NetworkReachabilityFlags"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private static bool IsReachable (NetworkReachabilityFlags flags)
		{
			// Is it reachable with the current network configuration?
			bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;
	
			return isReachable;
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="flags">
		/// A <see cref="NetworkReachabilityFlags"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private static bool IsNoConnectionRequired (NetworkReachabilityFlags flags)
		{
			// Do we need a connection to reach it?
			bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0;
	
			// Since the network stack will automatically try to get the WAN up,
			// probe that
			if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
				noConnectionRequired = true;
	
			return  noConnectionRequired;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="flags">
		/// A <see cref="NetworkReachabilityFlags"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private static bool IsAdHocWiFiNetworkAvailable (out NetworkReachabilityFlags flags)
		{
			NetworkReachability adHocWiFiNetworkReachability = new NetworkReachability (new IPAddress (new byte [] {169,254,0,0}));
			
			if (!adHocWiFiNetworkReachability.TryGetFlags (out flags))
				return false;
	
			return IsReachable (flags) && IsNoConnectionRequired(flags);  // is reachable without requiring connection.
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="flags">
		/// A <see cref="NetworkReachabilityFlags"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private static bool IsNetworkAvaialable (out NetworkReachabilityFlags flags)
		{
			NetworkReachability defaultRouteReachability = new NetworkReachability (new IPAddress (0));

			if (defaultRouteReachability.TryGetFlags (out flags))
			    return false;
			return IsReachable (flags) && IsNoConnectionRequired(flags);  // is reachable without requiring connection.
		}	
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="NetworkType"/>
		/// </returns>
		private static NetworkType LocalWifiConnectionStatus ()
		{
			NetworkReachabilityFlags flags;
			if (IsAdHocWiFiNetworkAvailable (out flags)){
				if ((flags & NetworkReachabilityFlags.IsDirect) != 0)
					return NetworkType.Wifi;
			}
			return NetworkType.Unknown;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="NetworkType"/>
		/// </returns>
		private static NetworkType InternetConnectionStatus ()
		{
			NetworkReachabilityFlags flags;
			bool defaultNetworkAvailable = IsNetworkAvaialable (out flags);
			if (defaultNetworkAvailable){
				if ((flags & NetworkReachabilityFlags.IsDirect) != 0){
					return NetworkType.Unknown;
				}
			} else if ((flags & NetworkReachabilityFlags.IsWWAN) != 0) {
				//return NetworkType.Carrier_GSM; // TODO get which type of carrier is being used.
				return NetworkType.Carrier_3G; // HARDCODED.
			}	
			return NetworkType.Wifi;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="host">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="NetworkType"/>
		/// </returns>
		private static NetworkType RemoteHostStatus (string host)
		{
			bool reachable = IsHostReachable(host);;			
	
			if (!reachable)
				return NetworkType.Unknown;
	
			using (var r = new NetworkReachability (host)){
				NetworkReachabilityFlags flags;
				
				if (r.TryGetFlags (out flags)){
					if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
						//return NetworkType.Carrier_GSM;  // TODO get which type of carrier is being used.
						return NetworkType.Carrier_3G; // HARDCODED.
				}
			}
	
			return NetworkType.Wifi;
		}
		
		public override bool DownloadFile (string url)
		{
			using (var pool = new NSAutoreleasePool ()) {
				NSUrl urlParam = new NSUrl (url);
				var thread = new Thread (OpenUrlOnThread);
				thread.Start (urlParam);
			}
			return true;
		}
		
		public override bool OpenBrowser (string title, string buttonText, string url)
		{
			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (OpenBrowserOnThread);
				BrowserCommand browserCommand = new BrowserCommand();
				browserCommand.Title = title;
				browserCommand.ButtonText = buttonText;
				browserCommand.Url = url;
				browserCommand.CheckNullsAndSetDefaults();
				thread.Start (browserCommand);
			}
			return true;
		}
		
		public override bool ShowHtml (string title, string buttonText, string html)
		{
			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (OpenBrowserOnThread);
				BrowserCommand browserCommand = new BrowserCommand();
				browserCommand.Title = title;
				browserCommand.ButtonText = buttonText;
				browserCommand.Html = html;
				browserCommand.CheckNullsAndSetDefaults();
				thread.Start (browserCommand);
			}
			return true;
		}
		
		[Export("OpenBrowserOnThread")]
		private void OpenBrowserOnThread (object browserCommandObject)
		{
			BrowserCommand browserCommand = (BrowserCommand)browserCommandObject;
			
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				
				IPhoneUIViewController contentController = new IPhoneUIViewController(browserCommand.Title, browserCommand.ButtonText);
				
				UIWebView webView = new UIWebView();
				webView.ScalesPageToFit = true;
				webView.LoadStarted+= delegate (object sender, EventArgs e) {
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				};
				webView.LoadFinished+=delegate (object sender, EventArgs e) {
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
					
					// stop notify loading masks if any
					INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
					notificationService.StopNotifyLoading();
				};	
				contentController.AddInnerView(webView);     
				
				IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().PresentModalViewController(contentController, true);				
				if(browserCommand.Url!=null && browserCommand.Url.Length>0) {
					NSUrl nsUrl = new NSUrl (browserCommand.Url);				
					NSUrlRequest  nsUrlRequest = new NSUrlRequest(nsUrl,NSUrlRequestCachePolicy.ReloadRevalidatingCacheData, 120.0);
					webView.LoadRequest(nsUrlRequest);
				} else if(browserCommand.Html!=null && browserCommand.Html.Length>0) {
					webView.LoadHtmlString(browserCommand.Html, new NSUrl("/"));
				}
				
				
			});
		}
		
		[Export("OpenUrlOnThread")]
		private void OpenUrlOnThread (object url)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				NSUrl urlParam = (NSUrl)url;
				SystemLogger.Log(SystemLogger.Module.PLATFORM,"Opening URL :"+urlParam.ToString());
				UIApplication.SharedApplication.OpenUrl (urlParam);
			});
		}
    }
}
