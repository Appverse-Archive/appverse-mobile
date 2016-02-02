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
package com.gft.unity.android.util.plist;

import java.io.IOException;
import java.io.InputStream;
import java.util.HashMap;
import java.util.Map;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;
import org.xmlpull.v1.XmlPullParserFactory;

import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.gft.unity.core.system.log.Logger.LogLevel;

public class PListParser {

	private static final String LOGGER_MODULE = "PListParser";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);

	private final static String PLIST_TAG = "plist";

	private final static String DICT_TAG = "dict";

	private final static String KEY_TAG = "key";

	private final static String VALUE_TAG = "string";

	public static PList parse(InputStream is) {
		PList plist = new PList();

		try {

			XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
			factory.setNamespaceAware(false);
			factory.setValidating(false);
			XmlPullParser parser = factory.newPullParser();
			parser.setInput(is, null);

			int event = parser.next();
			while (event != XmlPullParser.END_DOCUMENT) {

				String name;
				switch (event) {
				case XmlPullParser.START_TAG:
					name = parser.getName();
					if (PLIST_TAG.equals(name)) {
						// root node, do nothing
					} else if (DICT_TAG.equals(name)) {
						// dictionary node
						plist.setValues(parseDictionary(parser));
					} else {
						throw new XmlPullParserException(
								"Unexpected element found [" + event + ","
										+ name + "]");
					}
					break;
				case XmlPullParser.END_TAG:
					name = parser.getName();
					if (PLIST_TAG.equals(name)) {
						// root node, do nothing
					} else {
						throw new XmlPullParserException(
								"Unexpected element found [" + event + ","
										+ name + "]");

					}
					break;
				}

				event = parser.next();
			}
		} catch (Exception ex) {
			plist = null;
			LOGGER.logError("Parse", "Error", ex);
		}

		if (LOGGER.isLoggable(LogLevel.DEBUG)) {
			if (plist != null) {
				LOGGER.logDebug("Parse", "Result is: " + plist.toString());
			} else {
				LOGGER.logDebug("Parse", "Result is null");
			}
		}

		return plist;
	}

	private static Map<String, String> parseDictionary(XmlPullParser parser)
			throws IOException, XmlPullParserException {
		Map<String, String> map = new HashMap<String, String>();

		String key = null;
		String value = null;
		int event = parser.next();
		while ((event != XmlPullParser.END_TAG)
				|| !DICT_TAG.equals(parser.getName())) {
			switch (event) {
			case XmlPullParser.START_TAG:
				String name = parser.getName();
				if (KEY_TAG.equals(name)) {
					key = getText(parser, KEY_TAG);
				} else if (VALUE_TAG.equals(name)) {
					value = getText(parser, VALUE_TAG);
					map.put(key, value);
				}
				break;
			}
			event = parser.next();
		}

		return map;
	}

	private static String getText(XmlPullParser parser, String tag)
			throws IOException, XmlPullParserException {

		int event = parser.getEventType();
		if ((event != XmlPullParser.START_TAG) || !tag.equals(parser.getName())) {
			throw new XmlPullParserException("Unexpected element found ["
					+ event + "," + parser.getName() + "]");
		}

		StringBuilder sb = new StringBuilder();
		event = parser.next();
		while (event == XmlPullParser.TEXT) {
			sb.append(parser.getText());
			event = parser.next();
		}

		String text = sb.toString();

		if ((event != XmlPullParser.END_TAG) || !tag.equals(parser.getName())) {
			throw new XmlPullParserException("Unexpected element found ["
					+ event + "," + parser.getName() + "]");
		}

		return text;
	}
}