/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://www.appverse.mobi/licenses/apl_v2.0.pdf.

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
package com.gft.unity.core.system.server.net;

import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.gft.unity.core.system.log.Logger.LogLevel;
import java.io.IOException;

public abstract class AbstractHandler implements Handler {

    private static final String LOGGER_MODULE = "WebServer.AbstractHandler";
    private static final Logger LOGGER = Logger.getInstance(LogCategory.CORE, LOGGER_MODULE);
    protected Server server;
    protected String handlerName;
    protected String urlPrefix;
    public static final ConfigOption URL_PREFIX_OPTION = new ConfigOption(
            "url-prefix",
            "/",
            "URL prefix path for this handler.  Anything that matches starts with this prefix will be handled by this handler.");

    public AbstractHandler() {
    }

    @Override
    public boolean initialize(String handlerName, Server server) {

        this.server = server;
        this.handlerName = handlerName;
        this.urlPrefix = URL_PREFIX_OPTION.getProperty(server, handlerName);

        return true;
    }

    @Override
    public String getName() {
        return handlerName;
    }

    @Override
    public boolean handle(Request aRequest, Response aResponse)
            throws IOException {

        if (aRequest instanceof HttpRequest) {

            HttpRequest request = (HttpRequest) aRequest;
            HttpResponse response = (HttpResponse) aResponse;

            if (isRequestdForHandler(request)) {
                return handleBody(request, response);
            }

            if (LOGGER.isLoggable(LogLevel.INFO)) {
                LOGGER.logInfo("handle", "'" + request.getUrl()
                        + "' does not start with prefix '" + getUrlPrefix()
                        + "'");
            }
        }

        return false;
    }

    protected boolean isRequestdForHandler(HttpRequest request) {
        return request.getUrl().startsWith(getUrlPrefix());
    }

    protected boolean handleBody(HttpRequest request, HttpResponse response)
            throws IOException {
        return false;
    }

    @Override
    public boolean shutdown(Server server) {
        return true;
    }

    public String getUrlPrefix() {
        return urlPrefix;
    }

    protected String getMimeType(String filename) {
        int index = filename.lastIndexOf(".");
        String mimeType = null;
        if (index > 0) {
            mimeType = server.getProperty("mime"
                    + filename.substring(index).toLowerCase());
        }

        return mimeType;
    }
}
