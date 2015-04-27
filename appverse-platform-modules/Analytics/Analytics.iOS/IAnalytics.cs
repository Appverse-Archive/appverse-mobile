/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
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
/// <summary>
/// IAnalytics Interface to gather metrics data 
/// </summary>
namespace Appverse.Core.Analytics
{
	public interface IAnalytics
	{
		/// <summary>
		/// Starts the tracking.
		/// </summary>
		/// <returns>
		/// True if started successfully
		/// </returns>
		/// <param name='webPropertyID'>
		/// Google Analytics UID
		/// </param>
		bool StartTracking (string webPropertyID);
    
		/// <summary>
		/// Stops a tracking session
		/// </summary>
		/// <returns>
		/// True if stopped successfully
		/// </returns>
		bool StopTracking ();
    
		/// <summary>
		/// Tracks an event
		/// </summary>
		/// <returns>
		/// The event.
		/// </returns>
		/// <param name='group'>
		/// 
		/// </param>
		/// <param name='action'>
		/// 
		/// </param>
		/// <param name='label'>
		/// 
		/// </param>
		/// <param name='value'>
		/// 
		/// </param>
		bool TrackEvent (string group, string action, string label, int value);
    
		/// <summary>
		/// Tracks a page view.
		/// </summary>
		/// <returns>
		/// True if a pageview is tracked successfully
		/// </returns>
		/// <param name='relativeUrl'>
		///  Relative URL to the domain to be tracked
		/// </param>
		bool TrackPageView (string relativeUrl);
	}
}

