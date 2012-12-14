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
using System.Collections;
using Unity.Core.Webtrekk;
/// <summary>
/// IWebtrekk Interface to gather metrics data 
/// </summary>
/// 
namespace Unity.Core.Webtrekk
{
	public interface IWebtrekk
	{
		/// <summary>
		/// Starts the tracking.
		/// </summary>
		/// <returns>
		/// TTrue if started successfully
		/// </returns>
		/// <param name='webServerUrl'>
		/// Webtrekk account URL
		/// </param>
		/// <param name='trackId'>
		/// Webtrekk Track ID
		/// </param>
		/// <summary>
		/// Starts a tracking session.
		/// </summary>
		/// <returns>
		/// TRUE always
		/// </returns>
		/// <param name='webServerUrl'>
		/// Webtrekk account URL
		/// </param>
		/// <param name='trackId'>
		/// Webtrekk Track ID
		/// </param>
		bool StartTracking (string webServerUrl, string trackId);
		
		/// <summary>
		/// Starts a tracking session.
		/// </summary>
		/// <returns>
		/// TRUE always
		/// </returns>
		/// <param name='webServerUrl'>
		/// Webtrekk account URL
		/// </param>
		/// <param name='trackId'>
		/// Webtrekk Track ID
		/// </param>
		/// <param name='samplingRate'>
		/// (optional) Sampling rate, the amount of users to skip. IE: if set to 5, it will send the statistics of each 5th user, skipping the previous 4.
		/// </param>
		bool StartTracking (string webServerUrl, string trackId, string samplingRate);
		
		/// <summary>
		/// Stops a tracking session
		/// </summary>
		/// <returns>
		/// TRUE always
		/// </returns>
		bool StopTracking ();
		
		/// <summary>
		/// Sets the Time interval to send the requests to the server.
		/// </summary>
		/// <returns>
		/// TRUE always
		/// </returns>
		/// <param name='intervalInSeconds'>
		/// Interval in seconds
		/// </param>
		bool SetRequestInterval(double intervalInSeconds);
		
		/// <summary>
		/// Tracks content triggered by a button
		/// </summary>
		/// <returns>
		/// TRUE always
		/// </returns>
		/// <param name='clickId'>
		/// The trigger button ID
		/// </param>
		/// <param name='contentId'>
		/// Content to be tracked
		/// </param>
		bool TrackClick(string clickId, string contentId);
		
		/// <summary>
		/// Tracks content triggered by a button
		/// </summary>
		/// <returns>
		/// TRUE always
		/// </returns>
		/// <param name='clickId'>
		/// The trigger button ID
		/// </param>
		/// <param name='contentId'>
		/// Content to be tracked
		/// </param>
		/// <param name='additionalParameters'>
		/// (optional) Additional parameters to be tracked
		/// </param>
		bool TrackClick(string clickId, string contentId, WebtrekkParametersCollection additionalParameters);
		
		/// <summary>
		/// Tracks content.
		/// </summary>
		/// <returns>
		/// TRUE always
		/// </returns>
		/// <param name='contentId'>
		/// Content to be tracked
		/// </param>
		 bool TrackContent(string contentId);
		
		/// <summary>
		/// Tracks content.
		/// </summary>
		/// <returns>
		/// TRUE always
		/// </returns>
		/// <param name='contentId'>
		/// Content to be tracked
		/// </param>
		/// <param name='additionalParameters'>
		/// (optional) Additional parameters to be tracked
		/// </param>
		bool TrackContent(string contentId, WebtrekkParametersCollection additionalParameters);
	}
}

