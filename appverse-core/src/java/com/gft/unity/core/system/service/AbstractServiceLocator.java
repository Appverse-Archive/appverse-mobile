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
package com.gft.unity.core.system.service;

import java.util.HashMap;
import java.util.Map;

public class AbstractServiceLocator implements IServiceLocator {

    public static final String SERVICES_CONFIG_FILE = "app/config/services-config.xml";
    public static final String IO_SERVICES_CONFIG_FILE = "app/config/io-services-config.xml";
    public static final String SERVICE_TYPE_ANALYTICS = "analytics";
    public static final String SERVICE_TYPE_DATABASE = "db";
    public static final String SERVICE_TYPE_FILESYSTEM = "file";
    public static final String SERVICE_TYPE_GEO = "geo";
    public static final String SERVICE_TYPE_I18N = "i18n";
    public static final String SERVICE_TYPE_IO = "io";
    public static final String SERVICE_TYPE_LOG = "log";
    public static final String SERVICE_TYPE_MEDIA = "media";
    public static final String SERVICE_TYPE_MESSAGING = "message";
    public static final String SERVICE_TYPE_NET = "net";
    public static final String SERVICE_TYPE_NOTIFICATION = "notify";
    public static final String SERVICE_TYPE_PIM = "pim";
    public static final String SERVICE_TYPE_SECURITY = "security";
    public static final String SERVICE_TYPE_SHARE = "share";
    public static final String SERVICE_TYPE_SYSTEM = "system";
    public static final String SERVICE_TYPE_TELEPHONY = "phone";
    public static final String SERVICE_TYPE_WEBTREKK = "webtrekk";
    public static final String SERVICE_TYPE_APPLOADER = "loader";
    protected static IServiceLocator singletonServiceLocator = null;
    private Map<String, Object> services = new HashMap<String, Object>();

    protected AbstractServiceLocator() {
    }

    @Override
    public final Object GetService(String name) {
        return services.get(name);
    }

    public final void RegisterService(Object service, String key) {
        services.put(key, service);
    }
}
