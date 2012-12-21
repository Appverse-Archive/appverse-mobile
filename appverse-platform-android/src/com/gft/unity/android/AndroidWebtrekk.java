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

import java.util.LinkedHashMap;
import java.util.Map;

import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.gft.unity.core.webtrekk.AbstractWebtrekk;
import com.gft.unity.core.webtrekk.WebtrekkParametersCollection;

import com.webtrekk.android.tracking.*;

public class AndroidWebtrekk extends AbstractWebtrekk {
	private static final String LOGGER_MODULE = "IWebtrekk";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);

	@Override
	public boolean SetRequestInterval(double intervalInSeconds) {
		boolean result = false;
		
		LOGGER.logOperationBegin("SetRequestInterval",
				new String[] { "intervalInSeconds" },
				new Object[] { intervalInSeconds });
		
		try {
			Webtrekk.setRequestTime((long) intervalInSeconds);
			result = true;
		} catch (Exception ex) {
			LOGGER.logError("SetRequestInterval", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("SetRequestInterval", result);
		}
		return result;
	}

	@Override
	public boolean StartTracking(String webServerUrl, String trackId) {
		boolean result = false;

		LOGGER.logOperationBegin("StartTracking",
				new String[] { "webServerUrl", "trackId" },
				new Object[] { webServerUrl, trackId });
				
		try {
				Webtrekk.startSession(AndroidServiceLocator.getContext(), webServerUrl, trackId);
				result = true;
		} catch (Exception ex) {
			LOGGER.logError("StartTracking", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StartTracking", result);
		}

		return result;
	}

	@Override
	public boolean StartTracking(String webServerUrl, String trackId, String samplingRate) {
		boolean result = false;

		LOGGER.logOperationBegin("StartTracking",
				new String[] { "webServerUrl", "trackId", "samplingRate" },
				new Object[] { webServerUrl, trackId, samplingRate });
				
		try {
				Webtrekk.startSession(AndroidServiceLocator.getContext(), webServerUrl, trackId, samplingRate);
				result = true;
		} catch (Exception ex) {
			LOGGER.logError("StartTracking", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StartTracking", result);
		}

		return result;
	}

	@Override
	public boolean StopTracking() {
		boolean result = false;

		LOGGER.logOperationBegin("StopTracking", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		try {
			Webtrekk.endSession();
			result = true;
		} catch (Exception ex) {
			LOGGER.logError("StopTracking", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StopTracking", result);
		}

		return result;
	}

	@Override
	public boolean TrackClick(String clickId, String contentId) {
		boolean result = false;

		LOGGER.logOperationBegin("TrackClick",
				new String[] { "clickId", "contentId" },
				new Object[] { clickId, contentId });
				
		try {
				Webtrekk.trackClick(contentId, clickId);
				result = true;
		} catch (Exception ex) {
			LOGGER.logError("TrackClick", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("TrackClick", result);
		}

		return result;
	}

	@Override
	public boolean TrackClick(String clickId, String contentId, 
			WebtrekkParametersCollection additionalParameters) {
		boolean result = false;

		LOGGER.logOperationBegin("TrackClick",
				new String[] { "clickId", "contentId", "additionalParameters" },
				new Object[] { clickId, contentId, additionalParameters });
				
		try {
			Map<String,String> paramDictionary = ParametersCollectionToDictionary(additionalParameters);
			Webtrekk.trackClick(contentId, clickId, paramDictionary);
			result = true;
		} catch (Exception ex) {
			LOGGER.logError("TrackClick", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("TrackClick", result);
		}

		return result;
	}

	@Override
	public boolean TrackContent(String contentId) {
		boolean result = false;

		LOGGER.logOperationBegin("TrackContent",
				new String[] { "contentId" },
				new Object[] { contentId });
				
		try {
				Webtrekk.trackContent(contentId);
				result = true;
		} catch (Exception ex) {
			LOGGER.logError("TrackContent", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("TrackContent", result);
		}

		return result;
	}

	@Override
	public boolean TrackContent(String contentId,
			WebtrekkParametersCollection additionalParameters) {
		boolean result = false;

		LOGGER.logOperationBegin("TrackContent",
				new String[] { "contentId", "additionalParameters" },
				new Object[] { contentId, additionalParameters });
				
		try {
			Map<String,String> paramDictionary = ParametersCollectionToDictionary(additionalParameters);
			Webtrekk.trackContent(contentId, paramDictionary);
			result = true;
		} catch (Exception ex) {
			LOGGER.logError("TrackContent", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("TrackContent", result);
		}

		return result;
	}
	
	private Map<String,String> ParametersCollectionToDictionary (WebtrekkParametersCollection paramCollection)
	{
		Map<String,String> additionalParameters = new LinkedHashMap<String,String>();
		for(int i = 0; i < paramCollection.getAdditionalParameters().length; i++){
			additionalParameters.put(paramCollection.getAdditionalParameters()[i].getName(),paramCollection.getAdditionalParameters()[i].getValue());
		}
		return additionalParameters;
	}
}
