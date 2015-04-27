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
package com.gft.appverse.android.beacon;

import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

import android.content.Context;
import android.webkit.WebView;

/**
 * Class to be used in runtime, instead of AndroidBeacon, as main module class for Android API level < 18
 * @author maps
 *
 */
public class BeaconNotSupportedAPI implements IBeacon{
	
	private static final String LOGGER_MODULE = "Beacons Module (NOT SUPPORTED API)";
	protected static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);
	
	public BeaconNotSupportedAPI(Context ctx, WebView view) {
		super(); // same constructor as main module class (AndroidBeacon)
	}

	@Override
	public void StartMonitoringAllRegions() {
		LOGGER.logDebug("StartMonitoringAllRegions", "Beacons not available under API lvl 18");		
	}

	@Override
	public void StartMonitoringRegion(String region) {
		LOGGER.logDebug("StartMonitoringRegion", "Beacons not available under API lvl 18");		
	}

	@Override
	public void StopMonitoringBeacons() {
		LOGGER.logDebug("StartMonitoringRegion", "Beacons not available under API lvl 18");		
	}

	
	

}
