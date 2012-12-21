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
package com.gft.unity.core.system.server;

import com.gft.unity.core.system.server.net.Server;
import java.util.Properties;
import java.util.logging.Level;
import java.util.logging.Logger;

public class HttpServer {

    protected static final Properties SERVER_CONFIG;

    static {
        SERVER_CONFIG = new Properties();

        SERVER_CONFIG.setProperty("endpoints", "http");
        SERVER_CONFIG.setProperty("http.class",
                "com.gft.unity.core.system.server.net.ServerSocketEndPoint");
        SERVER_CONFIG.setProperty("http.port", "8080");
        SERVER_CONFIG.setProperty("handler", "chain");

        //SERVER_CONFIG.setProperty("chain.chain", "print");
        SERVER_CONFIG
                .setProperty("chain.class",
                "com.gft.unity.core.system.server.net.handler.DefaultChainHandler");
        //SERVER_CONFIG.setProperty("print.class",
        //        "com.gft.unity.core.system.server.net.handler.PrintHandler");

        SERVER_CONFIG.setProperty("mime.txt", "text/plain");
        SERVER_CONFIG.setProperty("mime.html", "text/html");
        SERVER_CONFIG.setProperty("mime.gif", "image/gif");
        SERVER_CONFIG.setProperty("mime.jpeg", "image/jpeg");
        SERVER_CONFIG.setProperty("mime.jpg", "image/jpeg");
        SERVER_CONFIG.setProperty("mime.png", "image/png");
        SERVER_CONFIG.setProperty("mime.zip", "application/x-zip-compressed");
        SERVER_CONFIG.setProperty("mime.js", "text/javascript");
        SERVER_CONFIG.setProperty("mime.css", "text/css");
        SERVER_CONFIG.setProperty("mime.json", "application/javascript");
    }
    private Server server;
    private Properties serverProperties;

    public HttpServer(int port) {
        serverProperties = getServerProperties();
        serverProperties.setProperty("http.port", Integer.toString(port));
    }

    public Properties getServerProperties() {
        if (serverProperties == null) {
            serverProperties = SERVER_CONFIG;
        }
        return serverProperties;
    }

    public void start() {
        
    
        if (server != null) {
            shutdown();
        }
        Thread launcherThread = new Thread(new Runnable() {
        
            public void run() {
                server = new Server(serverProperties);
                server.start();
            }
            
        });
        launcherThread.start();
        
    }

    public void shutdown() {
        
        if (server != null) {
            server.shutdown();
            server = null;
        }
        
    }
    
    
    /**
     * To be override.
     */
    protected void platformSpecificSettings() {
        // override this method to provide platform specific settings prior to running https server
    }
            
    
    
}
