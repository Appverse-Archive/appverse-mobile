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

import com.gft.unity.core.log.AbstractLog;
import com.gft.unity.core.log.LogLevel;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class AndroidLog extends AbstractLog {

	private static final String LOGGER_MODULE = "ILog";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);

	private Logger logger;

	public AndroidLog() {
		logger = Logger.getInstance(LogCategory.APPLICATION, "User");
	}

	@Override
	public boolean Log(String message) {
		Log(message, LogLevel.DEBUG);
		return true;
	}

	@Override
	public boolean Log(String message, LogLevel level) {
		boolean result = true;

		LOGGER.logOperationBegin("Log", new String[] { "msg", "level" },
				new Object[] { message, level });

		switch (level) {
		case TRACE:
		case DEBUG:
			logger.logDebug("Log", message);
			break;
		case WARN:
			logger.logWarning("Log", message);
			break;
		case ERROR:
			logger.logError("Log", message);
			break;
		case FATAL:
			logger.logFatal("Log", message);
			break;
		case INFO:
		default:
			logger.logInfo("Log", message);
			break;
		}

		LOGGER.logOperationEnd("Log", result);

		return result;
	}
}
