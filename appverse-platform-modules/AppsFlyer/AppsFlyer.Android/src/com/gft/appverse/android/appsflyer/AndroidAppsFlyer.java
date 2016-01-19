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
package com.gft.appverse.android.appsflyer;

import java.io.BufferedInputStream;
import java.io.InputStream;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserFactory;

import android.content.Context;

import com.appsflyer.AppsFlyerLib;
import com.gft.unity.core.IAppDelegate;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class AndroidAppsFlyer implements IAppsFlyer, IAppDelegate {

	private static final String LOGGER_MODULE = "AppsFlyer Module";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);
	
	private static final String DEFAULT_ENCODING = "UTF-8";
	
	private Context context;
	private AppsFlyerInitialization _initOptions;
	private InputStream initOptions_is = null;
	
	public AndroidAppsFlyer(Context ctx) {
		super();
		context = ctx;
	}
	
	private AppsFlyerInitialization loadConfiguration() {

		AppsFlyerInitialization configuration = new AppsFlyerInitialization();

		LOGGER.logInfo("LoadConfiguration", "Loading AppsFlyer configuration...");
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
					if ("appsflyer-config".equalsIgnoreCase(parser.getName())) {
					
						/**
						 * <appsflyer-config 
	
							dev-key="123456" 
							app-id="66666666"	
							currency-code="EUR"
							customer-id="1234"
							communications-protocol="HTTP"
							use-http-fallback="false"
							
							/>
						 */
						
						configuration.setDevKey(parser.getAttributeValue(null, "dev-key"));
						
						configuration.setAppID(parser.getAttributeValue(null, "app-id"));
						
						if(parser.getAttributeValue(null, "currency-code")!=null)
							configuration.setCurrencyCode(parser.getAttributeValue(null, "currency-code"));
						
						configuration.setCustomerUserID(parser.getAttributeValue(null, "customer-id"));
						
						String communicationsProtocolValue = parser.getAttributeValue(null, "communications-protocol");
						if(communicationsProtocolValue!=null && communicationsProtocolValue.equals("HTTPS"))
								configuration.setCommunicationsProtocol(CommunicationsProtocol.HTTPS); //defautl value is HTTP
						
						String useHttpFallbackValue = parser.getAttributeValue(null, "use-http-fallback");
						if(useHttpFallbackValue!=null && useHttpFallbackValue.equals("true"))
								configuration.setUseHttpFallback(true); //defautl value is false
												
					} 
				}
				event = parser.next();
			}
		} catch (Exception ex) {
			LOGGER.logFatal("LoadConfiguration", "Error: " + ex.getMessage());
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
	public String getConfigFilePath() {
		return "app/config/appsflyer-config.xml";
	}

	@Override
	public void setConfigFileLoadedData(InputStream is) {
		LOGGER.logDebug("SetConfigFileLoadedData", "storing input stream for init options");
		initOptions_is = is;
	}

	
	
	@Override
	public void Initialize(AppsFlyerInitialization initOptions) {
		
		if(initOptions==null) {
			LOGGER.logDebug("Initialize", "No initOptions provided, cannot intialize AppsFlyer...");
			return;
		}
		
		LOGGER.logOperationBegin("Initialize", 
				new String[] { "appID", "devKey", "customerUserID", "currencyCode", "useHTTPFallback" },
				new Object[] { initOptions.getAppID(), initOptions.getDevKey(), initOptions.getCustomerUserID(), initOptions.getCurrencyCode(), initOptions.getUseHttpFallback() });
		_initOptions = initOptions;
		
		LOGGER.logDebug("Initialize","Intializing AppsFlyer with dev-key: " + initOptions.getDevKey());
		
		try {
			
			//not used in Android (see documentation) // AppsFlyerLib.setAppId(initOptions.getAppID());
			AppsFlyerLib.setAppsFlyerKey(initOptions.getDevKey());
			
			if(initOptions.getCustomerUserID() != null)
				AppsFlyerLib.setAppUserId(initOptions.getCustomerUserID());  // optional
			
			
			AppsFlyerLib.setCurrencyCode(initOptions.getCurrencyCode());
			
			// AF Android SDK uses HTTPS at all times, without HTTP fallback.
			// If you wish to allow the SDK to use HTTP fallback set the flag to true
			AppsFlyerLib.setUseHTTPFalback(initOptions.getUseHttpFallback()); // optional
			
		} catch (Exception ex) {
			LOGGER.logError("Initialize", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("Initialize", true);
		}
	}

	@Override
	public void TrackAppLaunch() {
		LOGGER.logOperationBegin("TrackAppLaunch", null, null);

		try {
			// minimum requirement to start tracking your app installs
			// this API enables AppsFlyer to detect installations, sessions, and updates.
			AppsFlyerLib.sendTracking(context);
			
		} catch (Exception ex) {
			LOGGER.logError("TrackAppLaunch", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("TrackAppLaunch", true);
		}
	}

	@Override
	public void TrackEvent(AppsFlyerTrackEvent event) {
		if(event==null) {
			LOGGER.logDebug("TrackEvent", "No event provided, cannot track AppsFlyer event...");
			return;
		}
		LOGGER.logOperationBegin("TrackEvent", 
				new String[] { "eventName"},
				new Object[] { event.getEventName()});

		try {
			// in-app event tracking
			AppsFlyerLib.sendTrackingWithEvent(context, event.getEventName(), event.getEventRevenueValue());
			
		} catch (Exception ex) {
			LOGGER.logError("TrackEvent", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("TrackEvent", true);
		}
	}

	@Override
	public void onCreate() {
		LOGGER.logDebug("onCreate", "Initializating AppsFlyer / Tracking Launch App");
		_initOptions = loadConfiguration();
		
		this.Initialize(_initOptions);
		this.TrackAppLaunch();
	}

	@Override
	public void buildMode(boolean arg0) {
		// TODO Auto-generated method stub
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

}
