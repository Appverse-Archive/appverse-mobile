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
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Unity.Core.Net;
using MonoTouch.SystemConfiguration;
using System.Net;
using MonoTouch.Foundation;
using System.Threading;
using MonoTouch.UIKit;
using Unity.Core.Notification;
using Unity.Core.System;
using System.Net.NetworkInformation;

namespace Unity.Platform.IPhone
{
    public class IPhoneNet : AbstractNet
    {
		private const string NETWORKINTERFACE_3G = "pdp_ip0";
		private const string NETWORKINTERFACE_WIFI = "en0";
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
				NSUrl urlParam = new NSUrl (Uri.EscapeUriString(url));
				var thread = new Thread (OpenUrlOnThread);
				thread.Start (urlParam);
			}
			return true;
		}

		public override NetworkData GetNetworkData ()
		{
			try{
				NetworkInterface intf = NetworkInterface.GetAllNetworkInterfaces ().First (x => x.Name.ToLower ().Equals (NETWORKINTERFACE_WIFI)) as NetworkInterface;
				intf = (intf == null || (intf!=null && intf.GetIPProperties().UnicastAddresses.Count == 0) ? NetworkInterface.GetAllNetworkInterfaces ().First (x => x.Name.ToLower ().Equals (NETWORKINTERFACE_3G)) as NetworkInterface : intf);
				if (intf != null && intf.GetIPProperties().UnicastAddresses.Count > 0) {
					NetworkData returnData = new NetworkData ();
					foreach (var addrInfo in intf.GetIPProperties().UnicastAddresses) {
						switch (addrInfo.Address.AddressFamily) {
						case System.Net.Sockets.AddressFamily.InterNetwork:
							//IPv4
							returnData.IPv4 = addrInfo.Address.ToString ();
							break;
						case System.Net.Sockets.AddressFamily.InterNetworkV6:
							//IPv6
							returnData.IPv6 = addrInfo.Address.ToString ();
							if(returnData.IPv6.Contains("%")) returnData.IPv6 = returnData.IPv6.Split('%')[0];
							break;
						default:
							break;
						}
					}
					return returnData;
				}
			}catch(Exception ex){ SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error found while retrieving NetworkData", ex);}
			return null;
		}
		
		public override bool OpenBrowser (string title, string buttonText, string url)
		{
			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (OpenBrowserOnThread);
				BrowserCommand browserCommand = new BrowserCommand();
				browserCommand.Title = title;
				browserCommand.ButtonText = buttonText;
				browserCommand.Url = Uri.EscapeUriString(url);
				browserCommand.CheckNullsAndSetDefaults();
				thread.Start (browserCommand);
			}
			return true;
		}

		public override bool OpenBrowserWithOptions (SecondaryBrowserOptions browserOptions)
		{ 
			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (OpenBrowserWithOptionsOnThread);
				if(browserOptions==null)
					browserOptions = new SecondaryBrowserOptions();
				else
					browserOptions.CheckNullsAndSetDefaults();
				thread.Start (browserOptions);
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

		public override bool ShowHtmlWithOptions (SecondaryBrowserOptions browserOptions)
		{
			if(browserOptions==null)
				browserOptions = new SecondaryBrowserOptions();
			else
				browserOptions.CheckNullsAndSetDefaults();
			browserOptions.Url = String.Empty;
			return OpenBrowserWithOptions (browserOptions);
		}
		
		[Export("OpenBrowserOnThread")]
		private void OpenBrowserOnThread (object browserCommandObject)
		{
			BrowserCommand browserCommand = (BrowserCommand)browserCommandObject;
			
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				
				IPhoneUIViewController contentController = new IPhoneUIViewController(browserCommand.Title, browserCommand.ButtonText);
				UIWebView webView = IPhoneNet.generateWebView();
				contentController.AddInnerView(webView);     
				
				IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().PresentModalViewController(contentController, true);				
				IPhoneServiceLocator.CurrentDelegate.SetMainUIViewControllerAsTopController(false);
				if(!String.IsNullOrWhiteSpace(browserCommand.Url)) {
					NSUrl nsUrl = new NSUrl (browserCommand.Url);				
					NSUrlRequest  nsUrlRequest = new NSUrlRequest(nsUrl,NSUrlRequestCachePolicy.ReloadRevalidatingCacheData, 120.0);
					webView.LoadRequest(nsUrlRequest);
				} else if(!String.IsNullOrWhiteSpace(browserCommand.Html)) {
					webView.LoadHtmlString(browserCommand.Html, new NSUrl("/"));
				}
				
				
			});
		}

		public static UIWebView generateWebView() {
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


			return webView;
		}

		[Export("OpenBrowserWithOptionsOnThread")]
		private void OpenBrowserWithOptionsOnThread (object browserOptionsObject)
		{
			SecondaryBrowserOptions browserOptions = (SecondaryBrowserOptions)browserOptionsObject;
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {

				IPhoneUIViewController contentController = new IPhoneUIViewController(browserOptions.Title, browserOptions.CloseButtonText);
				UIWebView webView = IPhoneNet.generateWebView();

				//IF NO EXTENSIONS ARE USED THEN PARSE THE URL FILE EXTENSION
				if(browserOptions.BrowserFileExtensions!=null && browserOptions.BrowserFileExtensions.Length>0){
					webView.ShouldStartLoad = delegate (UIWebView view, NSUrlRequest req, UIWebViewNavigationType nav){
						if(req!=null && req.Url!=null && req.Url.Path.LastIndexOf(".") != -1){
							string sFileExtension = req.Url.Path.Substring(req.Url.Path.LastIndexOf("."));
							if(browserOptions.BrowserFileExtensions.Contains(sFileExtension)){
								//HANDLE URL LIKE SYSTEM DOES
								DownloadFile(req.Url.ToString());
								//RETURN FALSE TO NOT LOAD THE URL ON OUR WEBVIEW
								return false;
							}else{
								//LOAD URL
								return true;
							}
						}
						return true;
					};
				}
				contentController.AddInnerView(webView);     
				IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().PresentModalViewController(contentController, true);				
				IPhoneServiceLocator.CurrentDelegate.SetMainUIViewControllerAsTopController(false);
				if(!String.IsNullOrWhiteSpace(browserOptions.Url)) {
					NSUrl nsUrl = new NSUrl (browserOptions.Url);				
					NSUrlRequest  nsUrlRequest = new NSUrlRequest(nsUrl,NSUrlRequestCachePolicy.ReloadRevalidatingCacheData, 120.0);
					webView.LoadRequest(nsUrlRequest);
				} else if(!String.IsNullOrWhiteSpace(browserOptions.Html)) {
					webView.LoadHtmlString(browserOptions.Html, new NSUrl("/"));
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
