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
package com.gft.appverse.android.analytics;

import android.content.Context;

import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

import com.google.analytics.tracking.android.Fields;
import com.google.analytics.tracking.android.GAServiceManager;
import com.google.analytics.tracking.android.GoogleAnalytics;
import com.google.analytics.tracking.android.MapBuilder;
import com.google.analytics.tracking.android.Tracker;


public class AndroidAnalytics extends AbstractAnalytics {

	//Using Google Analytics SDK v3.01 Implemented 13-Nov-2013
	
	private static final String LOGGER_MODULE = "Google Analytics Module";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);
	private Tracker v3Tracker = null;

	
	private Context context;
	
	
	public AndroidAnalytics(Context ctx) {
		super();
		context = ctx;
	}

	@Override
	public boolean StartTracking(String webPropertyId) {
		boolean result = false;

		LOGGER.logOperationBegin("StartTracking",
				new String[] { "webPropertyId" },
				new Object[] { webPropertyId });

		try {
			if(v3Tracker == null){
				v3Tracker = GoogleAnalytics.getInstance(context).getTracker(webPropertyId);
			}
			
			if (v3Tracker != null) {
				v3Tracker.set(Fields.SESSION_CONTROL, "start");
				result = true;
			}
		} catch (Exception ex) {
			LOGGER.logError("StartTracking", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StartTracking", result);
		}

		return result;
	}

	@SuppressWarnings("deprecation")
	@Override
	public boolean StopTracking() {
		boolean result = false;

		LOGGER.logOperationBegin("StopTracking", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);
		
		try {
			if (v3Tracker != null) {
				
				v3Tracker.set(Fields.SESSION_CONTROL, "end");
				GAServiceManager.getInstance().dispatchLocalHits();
				v3Tracker = null;
				result = true;
			}
		} catch (Exception ex) {
			LOGGER.logError("StopTracking", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StopTracking", result);
		}

		return result;
	}

	@Override
	public boolean TrackPageView(String url) {
		boolean result = false;

		LOGGER.logOperationBegin("TrackPageView", new String[] { "url" },
				new Object[] { url });

		try {
			if (v3Tracker != null) {
				v3Tracker.set(Fields.SCREEN_NAME, url);
				v3Tracker.send(MapBuilder.createAppView().build());
				result = true;
			}
		} catch (Exception ex) {
			LOGGER.logError("TrackPageView", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("TrackPageView", result);
		}

		return result;
	}

	@Override
	public boolean TrackEvent(String group, String action, String label,
			int value) {
		boolean result = false;

		LOGGER.logOperationBegin("TrackEvent", new String[] { "group",
				"action", "label", "value" }, new Object[] { group, action,
				label, value });

		try {
			if (v3Tracker != null) {
				v3Tracker.send(MapBuilder
						.createEvent(group, action, label, (long) value)
						.build()
				);
				result = true;
			}
		} catch (Exception ex) {
			LOGGER.logError("TrackEvent", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("TrackEvent", result);
		}

		return result;
	}
}
