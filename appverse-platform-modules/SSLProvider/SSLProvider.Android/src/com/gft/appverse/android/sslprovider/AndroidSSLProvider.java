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
package com.gft.appverse.android.sslprovider;

import java.io.InputStream;

import android.content.Context;
import android.content.Intent;

import com.gft.unity.core.IAppDelegate;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.google.android.gms.security.ProviderInstaller;

public class AndroidSSLProvider implements IAppDelegate {

	private static final String LOGGER_MODULE = "SSLProvider.Module";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);
	
	private Context context;
	
	public AndroidSSLProvider(Context ctx) {
		super();
		context = ctx;
	}
	
	@Override
	public void buildMode(boolean arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public String getConfigFilePath() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void onCreate() {
		LOGGER.logDebug("onCreate", "Installing NEW OpenSSL from GMS...");
		
		try {
			ProviderInstaller.installIfNeededAsync(context, new ProviderInstaller.ProviderInstallListener() {
				
				@Override
				public void onProviderInstalled() {
					LOGGER.logDebug("onCreate", "******** onProviderInstalled");
				}
				
				@Override
				public void onProviderInstallFailed(int code, Intent intent) {
					LOGGER.logDebug("onCreate", "******** onProviderInstallFailed: " + code);
				}
			});
		} catch (Exception ex) {
			LOGGER.logDebug("onCreate", "Exception installing GMS_OpenSSL Provider. Message: " + ex.getMessage());
		}
	}

	@Override
	public void onDestroy() {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onPause() {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onResume() {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onStop() {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void setConfigFileLoadedData(InputStream arg0) {
		// TODO Auto-generated method stub
		
	}

}
