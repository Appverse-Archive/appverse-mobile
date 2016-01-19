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
package org.me.unity4jui_android;

import android.app.Application;
import android.content.Context;
//import android.support.multidex.MultiDex;
//import android.support.multidex.MultiDexApplication;

import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

/**
 * Application used to share the static context of the application by other components, such as widgets, services, or providers.
 *
 */
public class AppverseApplication extends Application {

	/* Application context shared by all the application */
	private static Context applicationContext;
	
	private static final SystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();
	
	@Override
	public void onCreate() {

		super.onCreate();

		AppverseApplication.applicationContext = getApplicationContext();
		
		LOG.Log(Module.GUI, "AppverseApplication#onCreate. package name: " + this.getPackageName());
		
	}
	
	/**
	 * Method to obtain the context in a static way.
	 * @return Application context
	 */
	public static Context getAppContext() {
		return applicationContext;
	}
/*
	@Override
	protected void attachBaseContext(Context base) {
		
		super.attachBaseContext(base);
		
		// enable multidex
		MultiDex.install(this) ;
				
	}*/
	
	
	
	
}
