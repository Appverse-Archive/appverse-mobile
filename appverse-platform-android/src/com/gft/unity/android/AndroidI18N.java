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

import java.io.BufferedInputStream;
import java.io.IOException;
import java.io.InputStream;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserFactory;

import android.content.Context;

import com.gft.unity.android.util.plist.PList;
import com.gft.unity.android.util.plist.PListParser;
import com.gft.unity.core.i18n.AbstractI18N;
import com.gft.unity.core.i18n.I18NConfiguration;
import com.gft.unity.core.i18n.Locale;
import com.gft.unity.core.i18n.ResourceLiteralDictionary;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class AndroidI18N extends AbstractI18N {

	private static final String LOGGER_MODULE = "II18N";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);

	public AndroidI18N() {
	}

	@Override
	protected I18NConfiguration loadConfiguration() {

		I18NConfiguration configuration = new I18NConfiguration();

		LOGGER.logInfo("LoadConfiguration", "Loading 18N configuration...");
		BufferedInputStream bis = null;
		try {

			// open configuration file
			Context context = AndroidServiceLocator.getContext();
			bis = new BufferedInputStream(AndroidUtils.getInstance().getAssetInputStream(context.getAssets(), I18N_CONFIG_FILE));

			// parse configuration file
			XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
			XmlPullParser parser = factory.newPullParser();
			parser.setInput(bis, ENCODING);
			int event = parser.getEventType();
			while (event != XmlPullParser.END_DOCUMENT) {

				if (event == XmlPullParser.START_TAG) {
					if (DEFAULT_LOCALE_TAG.equalsIgnoreCase(parser.getName())) {
						// default locale
						String defaultLanguage = parser.getAttributeValue(null,
								LANGUAGE_ATTR);
						String defaultCountry = parser.getAttributeValue(null,
								COUNTRY_ATTR);
						configuration.setDefaultLocale(new Locale(
								defaultLanguage, defaultCountry));
					} else if (SUPPORTED_LOCALE_TAG.equalsIgnoreCase(parser
							.getName())) {
						// supported locale
						String language = parser.getAttributeValue(null,
								LANGUAGE_ATTR);
						String country = parser.getAttributeValue(null,
								COUNTRY_ATTR);
						configuration.addSupportedLocale(new Locale(language,
								country));
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

	@Override
	public String GetResourceLiteral(String key, String localeDescriptor) {
		String result = null;

		LOGGER.logOperationBegin("GetResourceLiteral", new String[] { "key",
				"localeDescriptor" }, new Object[] { key, localeDescriptor });

		BufferedInputStream bis = null;
		try {
			Context context = AndroidServiceLocator.getContext();
			bis = new BufferedInputStream(AndroidUtils.getInstance().getAssetInputStream(context.getAssets(),
					getResourcesFilePath(localeDescriptor)));
			PList plist = PListParser.parse(bis);
			if (plist != null) {
				result = plist.getValue(key);
			}
		} catch (IOException ex) {
			LOGGER.logError("GetResourceLiteral", "Error", ex);
		} finally {
			closeStream(bis);
			LOGGER.logOperationEnd("GetResourceLiteral", result);
		}

		return result;
	}

	@Override
	public ResourceLiteralDictionary GetResourceLiterals(String localeDescriptor) {
		ResourceLiteralDictionary result = null;

		LOGGER.logOperationBegin("GetResourceLiterals",
				new String[] { "localeDescriptor" },
				new Object[] { localeDescriptor });

		BufferedInputStream bis = null;
		try {
			Context context = AndroidServiceLocator.getContext();
			bis = new BufferedInputStream(AndroidUtils.getInstance().getAssetInputStream(context.getAssets(),
					getResourcesFilePath(localeDescriptor)));
			PList plist = PListParser.parse(bis);
			result = new ResourceLiteralDictionary();
			result.putAll(plist.getValues());
		} catch (IOException ex) {
			LOGGER.logError("GetResourceLiterals", "Error", ex);
		} finally {
			closeStream(bis);
			LOGGER.logOperationEnd("GetResourceLiterals", result);
		}

		return result;
	}

	private String getResourcesFilePath(String localeDescriptor) {
		return APP_CONFIG_PATH + "/" + localeDescriptor + PLIST_EXTENSION;
	}

	private static void closeStream(InputStream is) {

		try {
			if (is != null) {
				is.close();
			}
		} catch (Exception ex) {
			LOGGER.logWarning("CloseStream", "Error closing stream", ex);
		}
	}
}
