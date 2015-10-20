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
package com.gft.appverse.android.facebook;

import java.io.BufferedInputStream;
import java.io.InputStream;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserFactory;

import android.content.Context;

import com.facebook.AppEventsLogger;
import com.facebook.Settings;
import com.gft.unity.core.IAppDelegate;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class AndroidFacebook implements IAppDelegate {

	private static final String LOGGER_MODULE = "Android Module";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);
	
	private Context context;
	private InputStream initOptions_is = null;
	private FacebookInit _initOptions;
	
	private static final String DEFAULT_ENCODING = "UTF-8";
	
	public AndroidFacebook(Context ctx) {
		super();
		context = ctx;
	}

	@Override
	public void buildMode(boolean arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public String getConfigFilePath() {
		return "app/config/facebook-config.xml";
	}
	
	@Override
	public void setConfigFileLoadedData(InputStream is) {
		LOGGER.logDebug("SetConfigFileLoadedData", "storing input stream for facebook init options");
		initOptions_is = is;
	}

	
	private FacebookInit loadConfiguration() {

		FacebookInit configuration = new FacebookInit();

		LOGGER.logInfo("LoadConfiguration", "Loading Facebook configuration...");
		BufferedInputStream bis = null;
		try {

			// open configuration file
			bis = new BufferedInputStream(initOptions_is);

			// parse configuration file
			XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
			XmlPullParser parser = factory.newPullParser();
			parser.setInput(bis, DEFAULT_ENCODING);
			int event = parser.getEventType();
			while (event != XmlPullParser.END_DOCUMENT) {

				if (event == XmlPullParser.START_TAG) {
					if ("facebook-config".equalsIgnoreCase(parser.getName())) {
						configuration.setFacebookAppId(parser.getAttributeValue(null, "app-id"));
						configuration.setFacebookDisplayName(parser.getAttributeValue(null, "display-name"));
					} 
				}
				event = parser.next();
			}
		} catch (Exception ex) {
			LOGGER.logDebug("LoadConfiguration", "Error: " + ex.getMessage());
		} finally {
			closeStream(bis);
		}

		return configuration;
	}
	
	private void closeStream(InputStream is) {

		try {
			if (is != null) {
				is.close();
			}
		} catch (Exception ex) {
			LOGGER.logWarning("CloseStream", "Error closing stream", ex);
		}
	}
	
	@Override
	public void onCreate() {
		
		LOGGER.logDebug("onCreate", "Initializating Facebook library");
		_initOptions = loadConfiguration();
		
		try {
			LOGGER.logDebug("onCreate", "Setting facebook app id: " + _initOptions.getFacebookAppId());
			Settings.setApplicationId(_initOptions.getFacebookAppId());
			
			String packageName = context.getPackageName();
			String appVersion = context.getPackageManager().getPackageInfo(packageName, 0).versionName;
			LOGGER.logDebug("onCreate", "Setting facebook app version [" + packageName +  "]: " + appVersion);
			Settings.setAppVersion(appVersion);
			
		} catch (Exception ex) {
			LOGGER.logDebug("onCreate", "Error: " + ex.getMessage());
		}
	}

	@Override
	public void onDestroy() {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onPause() {
		LOGGER.logDebug("onPause", "Facebook Deactivate App");
		AppEventsLogger.deactivateApp(context); 
	}

	@Override
	public void onResume() {
		LOGGER.logDebug("onResume", "Facebook Activate App");
		AppEventsLogger.activateApp(context); 
	}

	@Override
	public void onStop() {
		// TODO Auto-generated method stub
		
	}
	
}
