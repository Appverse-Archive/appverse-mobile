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
using Unity.Core.System;

namespace Unity.Core.Net
{
	public abstract class AbstractNet : INet
	{
		#region Miembros de INet

		public abstract NetworkType[] GetNetworkTypeSupported ();

		private static long LASTURL_TIMEOUT = 60 * 1000; // 60.000 ticks == 6 milliseconds (10.000 tick == 1 millisecond)
		private string lastUrl;
		private bool lastUrlResponse;
		private long lastUrlExpiry;

		// 30s for cache time
		/// <summary>
		/// Detect if network is reachable or not.
		/// </summary>
		/// <param name="url">The url to check reachability.</param>
		/// <returns>True if reachable.</returns>
		public bool IsNetworkReachable (string url)
		{
			if (lastUrl == url && (lastUrlExpiry > DateTime.Now.Ticks)) {
				SystemLogger.Log (SystemLogger.Module.CORE, "Reusing network reachable status for URL: " + url);
				return lastUrlResponse;
			} else {
				SystemLogger.Log (SystemLogger.Module.CORE, "Fetching network reachable status for URL: " + url);
				bool isReachable = false;
				NetworkType[] availableNetworks = GetNetworkTypeReachableList (url);
				if (availableNetworks != null && availableNetworks.Length > 0) {
					isReachable = true;
				}
				
				lastUrl = url;
				lastUrlExpiry = DateTime.Now.AddTicks (LASTURL_TIMEOUT).Ticks;
				lastUrlResponse = isReachable;
				
				return isReachable;
			}
		}

		/// <summary>
		/// Get the prefered network type to reach the given url.
		/// </summary>
		/// <param name="url">The url to check reachability.</param>
		/// <returns>Network type (default is "Unknown")</returns>
		public NetworkType GetNetworkTypeReachable (string url)
		{
			NetworkType preferedNetworkType = NetworkType.Unknown;
			NetworkType[] availableNetworks = GetNetworkTypeReachableList (url);
			if (availableNetworks != null && availableNetworks.Length > 0) {
				// get first network type.
				preferedNetworkType = availableNetworks [0];
			}
			
			return preferedNetworkType;
		}

		public abstract NetworkType[] GetNetworkTypeReachableList (string url);

		public abstract bool OpenBrowser (string title, string buttonText, string url);

		public abstract bool OpenBrowserWithOptions (SecondaryBrowserOptions browserOptions);

		public abstract bool ShowHtml (string title, string buttonText, string html);

		public abstract bool ShowHtmlWithOptions (SecondaryBrowserOptions browserOptions);

		public abstract bool DownloadFile (string url);

		public abstract NetworkData GetNetworkData();
		
		#endregion
	}
}
