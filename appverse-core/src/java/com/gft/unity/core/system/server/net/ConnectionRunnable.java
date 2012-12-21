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
package com.gft.unity.core.system.server.net;

import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.gft.unity.core.system.log.Logger.LogLevel;
import java.io.EOFException;
import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.Socket;
import java.net.SocketException;
import java.util.Properties;

public class ConnectionRunnable implements Runnable {

    private static final String LOGGER_MODULE = "WebServer.ConnectionRunnable";
    private static final Logger LOGGER = Logger.getInstance(LogCategory.CORE, LOGGER_MODULE);
    protected Server server;
    protected Socket connection;
    protected Properties config;
    protected String scheme;

    public ConnectionRunnable(Server aServer, String aScheme,
            Socket aConnection, Properties aConnectionConfig) {
        this.scheme = aScheme;
        this.server = aServer;
        this.connection = aConnection;
        this.config = aConnectionConfig;
    }

    @Override
    public void run() {

        try {
            boolean next = false;
            do {
                HttpRequest request = createRequest();
                if (request.readRequest(connection.getInputStream())) {
                    HttpResponse response = new HttpResponse(request,
                            connection.getOutputStream(),
                            server.getResponseListeners());
                    if (LOGGER.isLoggable(LogLevel.INFO)) {
                        LOGGER.logInfo("run", connection.getInetAddress()
                                .getHostAddress()
                                + ":"
                                + connection.getPort()
                                + " - " + request.getUrl());
                    }
                    if (!server.post(request, response)) {
                        response.sendError(HttpURLConnection.HTTP_NOT_FOUND,
                                " was not found on this server.");
                    }
                    next = response.isKeepAlive();
                    if (!next) {
                        LOGGER.logInfo("run", "Closing connection.");
                        response.addHeader("Connection", "close");
                    }
                    response.commitResponse();
                } else {
                    LOGGER.logInfo("run",
                            "No request sent.  Closing connection.");
                    next = false;
                }
            } while (next);
        } catch (EOFException eof) {
            LOGGER.logDebug("run", "Closing connection");
            // do nothing
        } catch (SocketException e) {
            LOGGER.logDebug("run", "Closing connection");
            // do nothing
        } catch (IOException e) {
            LOGGER.logWarning("run", "IOException: " + e.getMessage());
        } catch (Exception e) {
            LOGGER.logWarning("run", "Handler threw an exception.", e);
        } finally {
            try {
                connection.close();
            } catch (IOException e) {
            }
        }
    }

    protected HttpRequest createRequest() throws IOException {
        return new HttpRequest(scheme, connection, config);
    }
}
