/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
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
package com.gft.unity.core.system.log;

import java.util.HashMap;
import java.util.Map;

public class Logger {

    public enum LogCategory {

        CORE, PLATFORM, GUI, APPLICATION, GENERAL
    };

    public enum LogLevel {

        DEBUG, INFO, WARNING, ERROR, FATAL
    };
    public final static String[] EMPTY_PARAMS = new String[]{};
    public final static Object[] EMPTY_VALUES = new Object[]{};
    private final static Map<String, Logger> cache = new HashMap<String, Logger>();
    private final ILoggerDelegate delegate = LogManager.getDelegate();
    private LogCategory category;
    private String module;

    private Logger(LogCategory category, String module) {
        this.category = category;
        this.module = module;
    }

    public static Logger getInstance(LogCategory category, String module) {

        String key = category.toString() + "#" + module;
        Logger logger = cache.get(key);
        if (logger == null) {
            logger = new Logger(category, module);
            cache.put(key, logger);
        }

        return logger;
    }

    public void log(LogLevel level, String operation, String message) {
        delegate.log(level, category, module, operation, message);
    }

    public void log(LogLevel level, String operation, String message,
            Exception ex) {
        delegate.log(level, category, module, operation, message, ex);
    }

    public void logDebug(String operation, String message) {
        delegate.logDebug(category, module, operation, message);
    }

    public void logInfo(String operation, String message) {
        delegate.logInfo(category, module, operation, message);
    }

    public void logWarning(String operation, String message) {
        delegate.logWarning(category, module, operation, message);
    }

    public void logWarning(String operation, String message, Exception ex) {
        delegate.logWarning(category, module, operation, message, ex);
    }

    public void logError(String operation, String message) {
        delegate.logError(category, module, operation, message);
    }

    public void logError(String operation, String message, Exception ex) {
        delegate.logError(category, module, operation, message, ex);
    }

    public void logFatal(String operation, String message) {
        delegate.logFatal(category, module, operation, message);
    }

    public void logFatal(String operation, String message, Exception ex) {
        delegate.logFatal(category, module, operation, message, ex);
    }

    public void logOperationBegin(String operation, String[] paramNames,
            Object[] paramValues) {
        delegate.logOperationBegin(category, module, operation, paramNames,
                paramValues);
    }

    public void logOperationEnd(String operation, Object result) {
        delegate.logOperationEnd(category, module, operation, result);
    }

    public boolean isLoggable(LogLevel level) {
        return delegate.isLoggable(category, level);
    }
}