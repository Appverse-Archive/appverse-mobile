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
import java.io.IOException;
import java.net.*;
import java.util.Properties;
import javax.net.ServerSocketFactory;

/**
 * This EndPoint provides normal sockets for the HTTP protocol. It can be
 * subclasses and overridden for protocols other than HTTP.
 *
 * <table class="inner"> <tr class="header"> <td>Parameter Name</td>
 * <td>Explanation</td> <td>Default Value</td> <td>Required</td> </tr> <tr
 * class="row"> <td>port</td> <td>The port the socket should listen on.</td>
 * <td>80</td> <td>No</td> </tr> <tr class="altrow"> <td>host</td> <td>The IP or
 * DNS of the host adapter this socket should bind to.</td> <td>None</td>
 * <td>No</td> </tr> <tr class="row"> <td>resolveHostName</td> <td>If the server
 * should do a reverse DNS on the connections so the logs will show the DNS name
 * of the client.</td> <td>false</td> <td>No</td> </tr> </table>
 */
public class ServerSocketEndPoint implements EndPoint, Runnable {

    private static final String LOGGER_MODULE = "WebServer.ServerSocketEndPoint";
    private static final Logger LOGGER = Logger.getInstance(LogCategory.CORE,
            LOGGER_MODULE);
    private static final ConfigOption PORT_OPTION = new ConfigOption("port",
            "80", "HTTP server port.");
    private static final ConfigOption RESOLVE_HOSTNAME_OPTION = new ConfigOption(
            "resolveHostName", "false", "Resolve host names");
    protected ServerSocketFactory factory;
    protected ServerSocket socket;
    protected Server server;
    protected String endpointName;
    protected boolean resolveHostName;
    private static boolean socketListening = false;

    public ServerSocketEndPoint() {
        this.factory = ServerSocketFactory.getDefault();
    }

    @Override
    public void initialize(String name, Server server) throws IOException {

        this.endpointName = name;
        this.server = server;
        resolveHostName = RESOLVE_HOSTNAME_OPTION.getBoolean(server,
                endpointName).booleanValue();
    }

    @Override
    public String getName() {
        return endpointName;
    }

    @Override
    public void start() {

        try {
            this.socket = createSocket(PORT_OPTION.getInteger(server,
                    endpointName).intValue());
            LOGGER.logInfo("start",
                    "Socket listening on port " + socket.getLocalPort());
            Thread thread = new Thread(this, endpointName + "["
                    + socket.getLocalPort() + "] ServerSocketEndPoint");
            thread.setPriority(Thread.MAX_PRIORITY);
            thread.setDaemon(false);
            //socketListening = true;
            thread.start();
        } catch (IOException e) {
            LOGGER.logError("start", "IOException ignored", e);
            e.printStackTrace();
            socketListening = false;
        } catch (NumberFormatException e) {
            LOGGER.logError("start", "NumberFormatException ignored", e);
        }
    }

    protected ServerSocket createSocket(int port) throws IOException {

        InetAddress localhostAddress = InetAddress.getLocalHost();
        
        //Fixes AMOB-28 ERROR_CONNECT
        if(!localhostAddress.toString().contains("127.0.0.1"))
            localhostAddress = InetAddress.getByName("localhost/127.0.0.1");  
        
        
        SocketAddress socketAddress = new InetSocketAddress(localhostAddress, port);
        
        //ServerSocket socket = factory.createServerSocket(port, 100, localhostAddress); 
        ServerSocket socket = factory.createServerSocket();
        
        // DO NOT REUSE ADDRESS TO ALLOW THROWING IOException (adress already in use),
        // AND CHECK THAT SOCKET IS NOT REALLY LISTENING
        // socket.setReuseAddress(true);
        
        socket.bind(socketAddress,100);
        
        
        LOGGER.logInfo("createSocket", "Socket created on local host: " + localhostAddress);
        return socket;
    }

    @Override
    public void run() {

        LOGGER.logInfo("run", (this.getClass().getName() + " running."));
        try {

            Properties config = new ChainableProperties(server.getConfig());
            socketListening = true;
            while (socketListening) {

                Socket client = socket.accept();
                Runnable runnable = createRunnable(client, config);
                //if (resolveHostName) {
                    // after resolving, the host name appears Socket.toString.
                    //InetAddress clientAddress = client.getInetAddress();
                    //clientAddress.getHostName();
                //}
                if (LOGGER.isLoggable(LogLevel.INFO)) {
                    LOGGER.logInfo("run",
                            "Connection from: " + client.toString());
                }
                server.post(runnable);
            }
        } catch (IOException e) {
            LOGGER.logDebug("run", "IOException ignored (shutdown probably forced by system): " + e.getMessage());
        } catch (Exception e) {
            LOGGER.logError("run", "Unhandled exception captured", e);
        } finally {
            socketListening = false;
        }
        LOGGER.logInfo("run", (this.getClass().getName() + " shutdown."));
    }

    protected String getProtocol() {
        return "http";
    }

    protected Runnable createRunnable(Socket client, Properties config)
            throws IOException {

        ConnectionRunnable runnable = new ConnectionRunnable(server,
                getProtocol(), client, config);
        return runnable;
    }

    @Override
    public void shutdown(Server server) {

        if (socket != null) {
            try {
                socket.close();
                
                socket = null;
                LOGGER.logInfo("shutdown", "SHUTDOWN: "
                        + this.getClass().getName() + " executed correctly.");
                System.gc();
            } catch (IOException e) {
                LOGGER.logWarning("shutdown", "SHUTDOWN: "
                        + this.getClass().getName() + " -> " + e);
                e.printStackTrace();
            }
            socket = null;
            System.gc();
        }
        socketListening = false;
    }
    
    @Override
    public boolean isListening() {
        return this.socketListening;
    }
    
    public static boolean isSocketListening() {
        return socketListening;
    }
}
