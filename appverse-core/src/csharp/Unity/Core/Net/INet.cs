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
namespace Unity.Core.Net
{
	public interface INet
	{

		/// <summary>
		/// List of supported network types (cable, wifi, etc.) on the current device.
		/// </summary>
		/// <returns>Network types.</returns>
		NetworkType[] GetNetworkTypeSupported ();

		/// <summary>
		/// Detect if network is reachable or not.
		/// </summary>
		/// <param name="url">The url to check reachability.</param>
		/// <returns>True if reachable.</returns>
		bool IsNetworkReachable (string url);

		/// <summary>
		/// Get the prefered network type to reach the given url.
		/// </summary>
		/// <param name="url">The url to check reachability.</param>
		/// <returns>Network type.</returns>
		NetworkType GetNetworkTypeReachable (string url);
        
		/// <summary>
		/// List of network types available to reach the given url.
		/// </summary>
		/// <param name="url">The url to check reachability.</param>
		/// <returns>List of network types, ordered by preference.</returns>
		NetworkType[] GetNetworkTypeReachableList (string url);
		
		bool OpenBrowser (string title, string buttonText, string url);
		
		bool ShowHtml (string title, string buttonText, string html);
		
		/// <summary>
		/// Downloads the given url file by using the default native handler. 
		/// </summary>
		/// <param name="url">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		bool DownloadFile (string url);
		
		
	}//end INet

}//end namespace Net