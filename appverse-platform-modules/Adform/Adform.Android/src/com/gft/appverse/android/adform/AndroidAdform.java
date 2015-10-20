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
package com.gft.appverse.android.adform;

import java.io.BufferedInputStream;
import java.io.InputStream;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserFactory;

import android.content.Context;

import com.adform.adformtrackingsdk.AdformTrackingSdk;
import com.adform.adformtrackingsdk.TrackPoint;
import com.gft.unity.core.IAppDelegate;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class AndroidAdform implements IAdform, IAppDelegate {

	private static final String LOGGER_MODULE = "Adform Module";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);
	
	private static final String DEFAULT_ENCODING = "UTF-8";
	

	private Context context;
	private InputStream initOptions_is = null;
	private AdformInitialization _initOptions;
	
	public AndroidAdform(Context ctx) {
		super();
		context = ctx;
	}
	
	
	private AdformInitialization loadConfiguration() {

		AdformInitialization configuration = new AdformInitialization();

		LOGGER.logInfo("LoadConfiguration", "Loading Adform configuration...");
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
					if ("adform-config".equalsIgnoreCase(parser.getName())) {
					
						/**
						 * <adform-config  tracking-id="123456"  />
						 */
						
						configuration.setTrackingID(Integer.parseInt(parser.getAttributeValue(null, "tracking-id")));
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
	public String getConfigFilePath() {
		return "app/config/adform-config.xml";
	}
	
	@Override
	public void setConfigFileLoadedData(InputStream is) {

		LOGGER.logDebug("SetConfigFileLoadedData", "storing input stream for init options");
		initOptions_is = is;
		
	}
	
	@Override
	public void buildMode(boolean arg0) {
		// TODO Auto-generated method stub
		
	}

	

	@Override
	public void onCreate() {
		LOGGER.logDebug("onCreate", "Initializating Adform library");
		_initOptions = loadConfiguration();
		
		LOGGER.logDebug("onCreate", "Start Tracking Adform [" + _initOptions.getTrackingID() + "]... ");
		
		// start tracking
		AdformTrackingSdk.startTracking(context,  _initOptions.getTrackingID());
		
		// Tracking "download" tack point section name
		String sectionName = "Download";
		AdformTrackPoint adFormTrackPoint = new AdformTrackPoint();
		adFormTrackPoint.setSectionName(sectionName);
		this.SendTrackPoint(adFormTrackPoint);
		
	}

	@Override
	public void onDestroy() {
		// TODO Auto-generated method stub
	}

	@Override
	public void onPause() {
		LOGGER.logDebug("onPause", "Tracking onPause() event to AdForm ... ");
		AdformTrackingSdk.onPause();
	}

	@Override
	public void onResume() {
		LOGGER.logDebug("onResume", "Tracking onResume() event to AdForm ... ");
		AdformTrackingSdk.onResume(context);
	}

	@Override
	public void onStop() {
		/* METHOD DEPRECATED in Adform
		LOGGER.logDebug("onStop", "Tracking onStop() event to AdForm ... ");
		AdformTrackingSdk.onStop();
		*/
	}


	@Override
	public void SendTrackPoint(AdformTrackPoint adformTrackPoint) {
		try {
			if(adformTrackPoint!=null) {
				String sectionName = adformTrackPoint.getSectionName();
				
				LOGGER.logDebug("SendTrackPoint", "Adform Tracking Point with section name [" + sectionName + "]...");
				
				TrackPoint trackpoint = new TrackPoint(_initOptions.getTrackingID());
				trackpoint.setSectionName(sectionName);
				
				if(adformTrackPoint.getCustomParameters() != null) {
					for (int i = 0; i < adformTrackPoint.getCustomParameters().length; i++) {
						trackpoint.addParameter(adformTrackPoint.getCustomParameters()[i].getName(), adformTrackPoint.getCustomParameters()[i].getValue());
					}
				}
				
				AdformTrackingSdk.sendTrackPoint(trackpoint);
				
			} else {
				LOGGER.logDebug("SendTrackPoint", "No track point object provided");
			}
		} catch (Exception e) {
			LOGGER.logDebug("SendTrackPoint", "Exception thrown... " + e.getMessage());
		}
	}

	
	
	
}
