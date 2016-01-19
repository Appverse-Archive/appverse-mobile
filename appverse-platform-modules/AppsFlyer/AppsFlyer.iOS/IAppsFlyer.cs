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

using System;

namespace Appverse.Platform.IPhone
{
	public interface IAppsFlyer
	{
		/// <summary>
		/// Initialize and Start the tracking system with the App ID and a possible customer app name.
		/// It is automatically called by the platform when launching the app 
		/// (no need to call it from the app code, at least any field needs to be changed at runtime)
		/// </summary>
		/// <param name="initOptions">AppsFlyer Initialization data.</param>
		void Initialize (AppsFlyerInitialization initOptions);

		/// <summary>
		/// Detects installations, sessions (app openings) and updates.
		/// It is automatically called by the platform when launching the app 
		/// (no need to call it from the app code, at least any field needs to be changed at runtime)
		/// </summary>
		void TrackAppLaunch ();

		/// <summary>
		/// Sends in-app events to the AppsFlyer analytics server
		/// </summary>
		/// <param name="trackEvent">AppsFlyerTrackEvent data to track</param>
		void TrackEvent (AppsFlyerTrackEvent trackEvent);

	}
}

