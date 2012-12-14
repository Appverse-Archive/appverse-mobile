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
package com.gft.unity.core.system.server.net.handler;

import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.gft.unity.core.system.server.net.AbstractHandler;
import com.gft.unity.core.system.server.net.ConfigOption;
import com.gft.unity.core.system.server.net.Handler;
import com.gft.unity.core.system.server.net.Request;
import com.gft.unity.core.system.server.net.Response;
import com.gft.unity.core.system.server.net.Server;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.StringTokenizer;

/**
 * This is the default implementation of a chain of handlers. The .chain
 * parameter defines the names of the handlers in the chain, and it defines the
 * order in which those handlers will be called. Each handler name is separated
 * by either a ' ' (space) or a ',' (comma). This handler will then try to
 * create a handler for each of the handler names by looking at configuration
 * property {handler-name}.class. This handler also has a .url-prefix parameter
 * it uses to know when this handler should pass the request to the chain.
 *
 * <table class="inner"> <tr class="header"> <td>Parameter Name</td>
 * <td>Explanation</td> <td>Default Value</td> <td>Required</td> </tr> <tr
 * class="row"> <td>url-prefix</td> <td>The prefix to filter request urls.</td>
 * <td>None</td> <td>Yes</td> </tr> <tr class="altrow"> <td>chain</td> <td>A
 * space or comma separated list of the names of the handlers within the
 * chain.</td> <td>None</td> <td>Yes</td> </tr> <tr class="row"> <td>class</td>
 * <td>For each of the names in the chain property, this is appended the name to
 * find the classname to instantiate.</td> <td>None</td> <td>Yes</td> </tr>
 * </table>
 */
public class DefaultChainHandler extends AbstractHandler implements Handler {

    private static final String LOGGER_MODULE = "WebServer.DefaultChainHandler";
    private static final Logger LOGGER = Logger.getInstance(LogCategory.CORE,
            LOGGER_MODULE);
    public static String CHAIN = ".chain";
    public static final ConfigOption CHAIN_OPTION = new ConfigOption("chain",
            true, "A comma separated list of handler names to chain together.");
    private List<Handler> chain;

    @Override
    public boolean initialize(String handlerName, Server server) {
        super.initialize(handlerName, server);

        chain = new ArrayList<Handler>();
        initializeChain(server);

        return true;
    }

    private void initializeChain(Server server) {

        StringTokenizer tokenizer = new StringTokenizer(
                CHAIN_OPTION.getProperty(server, handlerName), " ,");
        while (tokenizer.hasMoreTokens()) {
            String chainChildName = tokenizer.nextToken();
            try {
                Handler handler = (Handler) server
                        .constructCoreServerObject(chainChildName);
                if (handler.initialize(chainChildName, server)) {
                    chain.add(handler);
                } else {
                    LOGGER.logError("initializeChain", chainChildName
                            + " was not initialized");
                }
            } catch (ClassCastException e) {
                LOGGER.logError("initializeChain", chainChildName
                        + " class does not implement the Handler interface.", e);
            }
        }
    }

    @Override
    public boolean handle(Request request, Response response)
            throws IOException {
        boolean hasBeenHandled = false;

        for (Iterator<Handler> i = chain.iterator(); i.hasNext()
                && !hasBeenHandled;) {
            Handler handler = i.next();
            hasBeenHandled = handler.handle(request, response);
        }

        return hasBeenHandled;
    }

    @Override
    public boolean shutdown(Server server) {
        boolean success = true;

        if (chain != null) {
            for (Iterator<Handler> i = chain.iterator(); i.hasNext();) {
                Handler current = i.next();
                boolean currentSuccess = current.shutdown(server);
                success = success && currentSuccess;
            }
        }

        return success;
    }
}
