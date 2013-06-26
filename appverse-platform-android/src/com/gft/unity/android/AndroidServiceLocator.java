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
package com.gft.unity.android;

import android.content.Context;

import com.gft.unity.core.system.service.AbstractServiceLocator;
import com.gft.unity.core.system.service.IServiceLocator;

public class AndroidServiceLocator extends AbstractServiceLocator {

	public static final String SERVICE_ANDROID_ASSET_MANAGER = "android.asset";
	public static final String SERVICE_ANDROID_ACTIVITY_MANAGER = "android.activity";

	private static Context context;

	private AndroidServiceLocator() {
		super();
		registerServices();
	}
	
	public static void setContext(Context context) {
		AndroidServiceLocator.context = context;
	}

	public static Context getContext() {
		return context;
	}
	
	public static IServiceLocator GetInstance() {

		if (singletonServiceLocator == null) {
			singletonServiceLocator = new AndroidServiceLocator();
		}
		
		return singletonServiceLocator;
	}

	private void registerServices() {
		this.RegisterService(new AndroidDatabase(),
				AndroidServiceLocator.SERVICE_TYPE_DATABASE);
		this.RegisterService(new AndroidFileSystem(),
				AndroidServiceLocator.SERVICE_TYPE_FILESYSTEM);
		this.RegisterService(new AndroidGeo(),
				AndroidServiceLocator.SERVICE_TYPE_GEO);
		this.RegisterService(new AndroidI18N(),
				AndroidServiceLocator.SERVICE_TYPE_I18N);
		this.RegisterService(new AndroidIO(),
				AndroidServiceLocator.SERVICE_TYPE_IO);
		this.RegisterService(new AndroidMedia(),
				AndroidServiceLocator.SERVICE_TYPE_MEDIA);
		this.RegisterService(new AndroidMessaging(),
				AndroidServiceLocator.SERVICE_TYPE_MESSAGING);
		this.RegisterService(new AndroidNet(),
				AndroidServiceLocator.SERVICE_TYPE_NET);
		this.RegisterService(new AndroidNotification(),
				AndroidServiceLocator.SERVICE_TYPE_NOTIFICATION);
		this.RegisterService(new AndroidShare(),
				AndroidServiceLocator.SERVICE_TYPE_SHARE);
		this.RegisterService(new AndroidSystem(),
				AndroidServiceLocator.SERVICE_TYPE_SYSTEM);
		this.RegisterService(new AndroidTelephony(),
				AndroidServiceLocator.SERVICE_TYPE_TELEPHONY);
		this.RegisterService(new AndroidLog(),
				AndroidServiceLocator.SERVICE_TYPE_LOG);
		this.RegisterService(new AndroidPim(),
				AndroidServiceLocator.SERVICE_TYPE_PIM);
		this.RegisterService(new AndroidAnalytics(),
				AndroidServiceLocator.SERVICE_TYPE_ANALYTICS);
		this.RegisterService(new AndroidSecurity(),
				AndroidServiceLocator.SERVICE_TYPE_SECURITY);
		this.RegisterService(new AndroidWebtrekk(),
				AndroidServiceLocator.SERVICE_TYPE_WEBTREKK);
		this.RegisterService(new AndroidAppLoader(),
				AndroidServiceLocator.SERVICE_TYPE_APPLOADER);
	}
}
