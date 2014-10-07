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
package com.gft.unity.android.log;

import java.util.HashMap;

import android.util.Log;

import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.core.system.log.AbstractLoggerDelegate;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.gft.unity.core.system.log.Logger.LogLevel;

public class AndroidLoggerDelegate extends AbstractLoggerDelegate {

	private static final HashMap<LogLevel, Integer> LEVEL_MAP = new HashMap<LogLevel, Integer>();

	static {
		LEVEL_MAP.put(LogLevel.DEBUG, Log.DEBUG);
		LEVEL_MAP.put(LogLevel.INFO, Log.INFO);
		LEVEL_MAP.put(LogLevel.WARNING, Log.WARN);
		LEVEL_MAP.put(LogLevel.ERROR, Log.ERROR);
		LEVEL_MAP.put(LogLevel.FATAL, Log.ERROR);
	}

	@Override
	public void log(LogLevel level, LogCategory category, String module,
			String operation, String message) {
		log(level, category, module, operation, message, null);
	}

	@Override
	public void log(LogLevel level, LogCategory category, String module,
			String operation, String message, Exception ex) {

		String tag = category.toString();
		String text = module + "." + operation + ": " + message;
		switch (level) {
		case DEBUG:
			if(AndroidServiceLocator.isDebuggable())
				Log.d(tag, text);
			break;
		case INFO:
			if (ex != null) {
				Log.i(tag, text, ex);
			} else {
				Log.i(tag, text);
			}
			break;
		case WARNING:
			if (ex != null) {
				Log.w(tag, text, ex);
			} else {
				Log.w(tag, text);
			}
			break;
		case ERROR:
			if (ex != null) {
				Log.e(tag, text, ex);
			} else {
				Log.e(tag, text);
			}
			break;
		case FATAL:
			if (ex != null) {
				Log.wtf(tag, text, ex);
			} else {
				Log.wtf(tag, text);
			}
			break;
		}
	}

	@Override
	public boolean isLoggable(LogCategory category, LogLevel level) {
		return Log.isLoggable(category.toString(), LEVEL_MAP.get(level));
	}

	@Override
	public void logOperationBegin(LogCategory category, String module,
			String operation, String[] names, Object[] values) {

		String tag = category.toString();
		if (Log.isLoggable(tag, Log.INFO)) {
			Log.i(tag, module + "." + operation + " BEGIN");
			if (Log.isLoggable(tag, Log.DEBUG) && AndroidServiceLocator.isDebuggable()) {
				int index = 0;
				for (String name : names) {
					Log.d(tag, module + "." + operation + "  " + name + " ["
							+ objectToString(values[index]) + "]");
					index++;
				}
			}
		}
	}

	@Override
	public void logOperationEnd(LogCategory category, String module, String operation,
			Object result) {

		String tag = category.toString();
		if (Log.isLoggable(tag, Log.INFO)) {
			Log.i(tag, module + "." + operation + " END");
			if (Log.isLoggable(tag, Log.DEBUG) && AndroidServiceLocator.isDebuggable()) {

				if ((result != null) && (result instanceof Object[])) {
					Object[] array = (Object[]) result;
					StringBuilder sb = new StringBuilder("{");
					for (Object item : array) {
						if (sb.length() > 1) {
							sb.append(",");
						}
						sb.append(item);
					}
					sb.append("}");
					result = sb.toString();
				}

				Log.d(tag, module + "." + operation + "  result ["
						+ objectToString(result) + "]");
			}
		}
	}

	private String objectToString(Object object) {
		String result;

		if (object == null) {
			result = "null";
		} else if (object instanceof Object[]) {
			Object[] array = (Object[]) object;
			StringBuilder sb = new StringBuilder("{");
			for (Object item : array) {
				if (sb.length() > 1) {
					sb.append(", ");
				}
				sb.append(item);
			}
			sb.append("}");
			result = sb.toString();
		} else {
			result = object.toString();
		}

		return result;
	}
}