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
import java.io.BufferedInputStream;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.util.Collection;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Properties;
import java.util.StringTokenizer;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.logging.Level;

/**
 * <p> Server is the core of the system. A server glues together
 * {@link Handler}s and {@link EndPoint}s. {@link EndPoint}s are responsible for
 * reading the {@link HttpRequest} from a source and sending the HttpResponse
 * over that source. {@link EndPoint}s then sends the request to the
 * {@link Handler} by calling the post() method on the server to send the
 * request to this server's {@link Handler}s. {@link Handler}s process the
 * {@link HttpRequest} and produce an appropriate {@link HttpResponse}. </p> <p>
 * The server contains the configuration for the entire server. What are the
 * expected values in the configuration is mainly controlled by what handlers
 * and endpoints are configured. Depending on which handlers and endpoints have
 * been enabled, the configuration will vary. The only two parameters are
 * required: <i>handler</i> and <i>&lt;handler's name&gt;.class</i>. Here is an
 * example configuration: </p>
 *
 * <pre>
 * <div class="code">
 * handler=my\ handler
 * my\ handler.class=com.gft.unity.core.system.server.net.handlers.DefaultChainHandler
 * my\ handler.chain=handler1, handler2
 * my\ handler.url-prefix=/
 *
 * handler1.class=com.gft.unity.core.system.server.net.handlers.FileHandler
 * handler1.root=C:\temp
 * handler1.url-prefix=/home-directory
 *
 * handler2.class=com.gft.unity.core.system.server.net.handlers.ResourceHandler
 * handler2.url-prefix=/jar
 * handler2.resourceMount=/html
 * handler2.default=index.html
 * </pre>
 *
 * </div <p> In the above configuration, <i>handler</i> property is the name of
 * first handler. The name is used to find all the other properties for that
 * particular handler. The .class property is used to tell the Server the name
 * of the class to instantiate. The two other properties, .chain and
 * .url-prefix, are particular to the
 * {@link com.gft.unity.core.system.server.net.handlers.DefaultChainHandler} .
 * </p> <p> Server's only have <b>one</b> {@link Handler}. However, the power of
 * {@link Handler}s is the ability to have more than one. The
 * {@link com.gft.unity.core.system.server.net.handlers.DefaultChainHandler}
 * provides the ability to create a chain of multiple handlers. See
 * {@link com.gft.unity.core.system.server.net.handlers.DefaultChainHandler} for
 * information on configuring it. </p> <p> Server also contains a set of
 * {@link com.gft.unity.core.system.server.net.core.EndPoint}s. When the server
 * initializes itself it looks in the configuration for the <i>endpoints</i>
 * parameter. The <i>endpoints</i> parameter contains a space separated list of
 * the names of the endpoints this server will create. For each name in the list
 * it will look for a config parameter <i>&lt;name of endpoint&gt;.class</i> in
 * the configuration. It will instantiate the classname using the no-argument
 * constructor and add it to the set of endpoints in the server. </p> <p> If the
 * server does not find the <i>endpoints</i> parameter, then it will create a
 * default EndPoint of type
 * {@link com.gft.unity.core.system.server.net.core.ServerSocketEndPoint} named
 * http. Here is an example of using the <i>endpoints</i> parameter: </p> <div
 * class="code">
 *
 * <pre>
 * endpoints=endpoint1 endpoint2
 * handler=handler1
 *
 * endpoint1.class=my.package.MyEndPoint
 * endpoint1.param1=foo
 * endpoint1.param2=bar
 * endpoint2.class=my.package.AnotherEndPoint
 * endpoint2.param1=foo
 * endpoint2.param2=bar
 * endpoint2.param3=baz
 * ...
 * </pre>
 *
 * </div> <p> Server class looks for the following properties in the
 * configuration: </p> <table class="inner"> <tr class="header"> <td>Parameter
 * Name</td> <td>Default Value</td> <td>Required</td> </tr> <tr class="row">
 * <td>handler</td> <td>None</td> <td>Yes</td> </tr> <tr class="altrow">
 * <td>endpoints</td> <td>http</td> <td>No</td> </tr> <tr class="row">
 * <td>&lt;handler name&gt;.class</td> <td>None</td> <td>Yes</td> </tr> <tr
 * class="altrow"> <td>&lt;endpoint name&gt;.class</td> <td>None</td> <td>if
 * endpoints param is defined</td> </tr> <tr class="row">
 * <td>threadpool.size</td> <td>5</td> <td>No</td> </tr> </table>
 */
public class Server implements Runnable {

    private static final String LOGGER_MODULE = "WebServer.ConnectionRunnable";
    private static final Logger LOGGER = Logger.getInstance(LogCategory.CORE,
            LOGGER_MODULE);
    private static final String CLAZZ = ".class";
    private Properties config = new ChainableProperties();
    private HashMap<String, EndPoint> endpoints = new HashMap<String, EndPoint>();
    private Handler handler = null;
    private ResponseListener responseListener = null;
    private ExecutorService threadPool;
    private boolean threadPoolInitialized = false;

    /**
     * This creates a server using the given filename as the configuration for
     * this server. The configuration file should follow format of normal
     * {@link java.util.Properties} file.
     *
     * @param filename the path to a file to use as the configuration of this
     * server.
     * @throws IOException
     */
    public Server(String filename) throws IOException {
        InputStream is = null;
        try {
            is = new BufferedInputStream(new FileInputStream(filename));
            config.load(is);
        } finally {
            if (is != null) {
                is.close();
            }
        }
    }

    /**
     * This creates a server using the given configuration.
     *
     * @param config the configuration to use for this server.
     */
    public Server(Properties config) {
        this.config = config;
    }

    /**
     * This method adds an {@link EndPoint} to this server. It will be
     * initialized once the {@link #start} method is called.
     *
     * @param name The name of this EndPoint instance.
     * @param endpoint The instance of the endpoint to add.
     */
    public void addEndPoint(String name, EndPoint endpoint) {
        endpoints.put(name, endpoint);
    }

    /**
     * This puts a configuration property into the server's configuration.
     *
     * @param key The unique key to store the value under.
     * @param value The value of the key.
     */
    public void putProperty(Object key, Object value) {
        config.put(key, value);
    }

    /**
     * Returns the property stored under the key.
     *
     * @param key the configuration key to look up.
     * @return the value stored in the configuration at this key.
     */
    public String getProperty(String key) {
        return config.getProperty(key);
    }

    /**
     * Returns the property stored under the key. If there isn't a property
     * called key, then it returns the defaultValue.
     *
     * @param key the configuration key to look up.
     * @param defaultValue the defaultValue returned if nothing is found under
     * the key.
     * @return the value stored in the configuration at this key.
     */
    public String getProperty(String key, String defaultValue) {
        return config.getProperty(key, defaultValue);
    }

    /**
     * Returns true if the key is in the configuration
     *
     * @param key the name of the key.
     * @return true if and only if the key is contained in the configuration.
     * False otherwise.
     */
    public boolean hasProperty(String key) {
        return config.containsKey(key);
    }

    /**
     * This returns the object stored under the given key.
     *
     * @param key the key to look up the stored object.
     * @return the object stored at the given key.
     */
    public Object get(Object key) {
        return config.get(key);
    }

    /**
     * Returns the configuration for the server.
     *
     * @return the configuration for the server.
     */
    public Properties getConfig() {
        return config;
    }

    public Object getRegisteredComponent(Class<?> clazz) {
        return config.get(clazz);
    }

    /**
     * This is called when a program wants to register a shared component for
     * Handlers to have access to. The objects's class will be the defining key
     * for identifying the object. Registering more than one instance will not
     * be supported. If you're program must differ between two instances, then
     * register a manager and allow the Handler's to interact with that to
     * differentiate the individual instances.
     *
     * @param object the object the user wants to make available to Handler
     * instances.
     */
    public void registerComponent(Object object) {
        config.put(object.getClass(), object);
    }

    /**
     * This method is called to start the web server. It will initialize the
     * server's Handler and all the EndPoints then call the
     * {@link EndPoint#start} on each EndPoint. This method will return after
     * the above steps are done.
     */
    public void start() {

        if (!threadPoolInitialized) {
            initializeThreads();
            threadPoolInitialized = true;
        }
        initializeHandler();

        constructEndPoints();

        for (Iterator<EndPoint> i = endpoints.values().iterator(); i.hasNext();) {
            EndPoint currentEndPoint = i.next();
            currentEndPoint.start();
            while (!currentEndPoint.isListening()) {
                try {
                    Thread.sleep(50);
                } catch (InterruptedException ex) {
                    java.util.logging.Logger.getLogger(Server.class.getName()).log(Level.SEVERE, null, ex);
                }
            }
        }
    }

    private void initializeThreads() {

        try {
            threadPool = Executors.newFixedThreadPool(Integer.parseInt(config
                    .getProperty("threadpool.size", "5")));
        } catch (NumberFormatException e) {
            LOGGER.logWarning("initializeThreads",
                    "threadpool.size was not a number using default of 5");
            threadPool = Executors.newFixedThreadPool(5);
        }
    }

    protected void initializeHandler() {

        if (handler == null) {
            handler = (Handler) constructCoreServerObject(getProperty("handler"));
        }
        handler.initialize(getProperty("handler"), this);
    }

    /**
     * This is the method used to construct Server objects. Given the object
     * name it appends .class onto the end and looks for the classname in the
     * server's configuration. It then analyzes the class's constructor
     * parameters for objects that it depends on. Then it looks those objects up
     * by class in the registered object pool. Finally, it calls the constructor
     * using reflection passing any registered objects as arguments. It returns
     * the newly constructed object or null if there was a problem.
     *
     * @param objectName the name of the object defined in the server's
     * configuration.
     * @return the newly constructed object, or null there was a problem
     * instantiated the object.
     */
    public Object constructCoreServerObject(String objectName) {
        Object theObject = null;

        String objectClassname = getProperty(objectName + CLAZZ);
        try {
            if (objectClassname == null) {
                throw new ClassNotFoundException(objectName + CLAZZ
                        + " configuration property not found.");
            }
            Class<?> handlerClass = Class.forName(objectClassname);
            Constructor<?>[] constructors = handlerClass.getConstructors();
            Class<?>[] paramClass = constructors[0].getParameterTypes();
            Object[] params = new Object[paramClass.length];
            for (int i = 0; i < paramClass.length; i++) {
                if (paramClass[i].equals(Server.class)) {
                    params[i] = this;
                } else if (paramClass[i].equals(String.class)) {
                    params[i] = objectName;
                } else {
                    params[i] = getRegisteredComponent(paramClass[i]);
                }
            }
            theObject = constructors[0].newInstance(params);
            LOGGER.logInfo("constructCoreServerObject",
                    "Server object constructed. object=" + objectName
                    + " class=" + objectClassname);
        } catch (IllegalAccessException e) {
            LOGGER.logError(
                    "constructCoreServerObject",
                    "Could not access constructor.  Make sure it has the constructor is public.  Service not started.  class="
                    + objectClassname, e);
        } catch (InstantiationException e) {
            LOGGER.logError("constructCoreServerObject",
                    "Could not instantiate object.  Service not started.  class="
                    + objectClassname, e);
        } catch (ClassNotFoundException e) {
            LOGGER.logError("constructCoreServerObject",
                    "Could not find class.  Service not started.  class="
                    + objectClassname, e);
        } catch (InvocationTargetException e) {
            LOGGER.logError(
                    "constructCoreServerObject",
                    "Could not instantiate object because constructor threw an exception.  Service not started.  class="
                    + objectClassname, e);
            LOGGER.logError("constructCoreServerObject", "Cause:", e);
        }

        return theObject;
    }

    private void constructEndPoints() {

        String val = getProperty("endpoints");
        if (val != null) {
            StringTokenizer tokenizer = new StringTokenizer(val);
            while (tokenizer.hasMoreTokens()) {
                String endPointName = tokenizer.nextToken();
                try {
                    EndPoint endPoint = (EndPoint) constructCoreServerObject(endPointName);
                    endPoint.initialize(endPointName, this);
                    addEndPoint(endPointName, endPoint);
                } catch (IOException e) {
                    LOGGER.logError("constructEndPoints", endPointName
                            + " was not initialized properly.", e);
                }
            }
        } else {
            LOGGER.logError("constructEndPoints", "No endpoints defined.");
        }
    }

    /**
     * This is called when the server is shutdown thread is called.
     */
    @Override
    public void run() {
        shutdown();
        synchronized (this) {
            this.notify();
        }
    }

    /**
     * This method will shutdown the Handler, and call {@link EndPoint#shutdown}
     * on each EndPoint.
     */
    public void shutdown() {

        LOGGER.logInfo("shutdown", "Starting shutdown.");
        try {

            if (handler != null) {
                LOGGER.logInfo("shutdown", "Shutting down handlers.");
                boolean shutdownHandler = handler.shutdown(this);
                LOGGER.logInfo("shutdown", "Handler reports: "
                        + shutdownHandler);
            }

            Collection<EndPoint> values = endpoints.values();
            if (values != null) {
                for (Iterator<EndPoint> i = values.iterator(); i.hasNext();) {
                    EndPoint currentEndPoint = i.next();
                    LOGGER.logInfo("shutdown", "Shutting down endpoint: "
                            + currentEndPoint.getName());
                    LOGGER.logInfo("shutdown", "Shutting down endpoint type: "
                            + currentEndPoint.getClass().getName());
                    currentEndPoint.shutdown(this);
                }
            }

            // keep threadpool opened throughout lifetime of app
            //threadPool.shutdown();

        } finally {
            LOGGER.logInfo("shutdown", "Shutdown complete.");
        }
    }

    /**
     * This method is used to post a {@link HttpRequest} to the server's
     * handler. It will create a HttpResponse for the EndPoint to send to the
     * client.
     *
     * @param request
     * @return A HttpResponse that corresponds to the HttpRequest being handled.
     * @throws IOException
     */
    public boolean post(Request request, Response response) throws IOException {
        return handler.handle(request, response);
    }

    /**
     * This method posts a Runnable onto the Server's task queue. The server's
     * {@link ThreadPool} will service the runnable once a thread is freed up.
     *
     * @param runnable An instance of Runnable that the user wishes to run on
     * the server's {@link ThreadPool}.
     */
    public void post(Runnable runnable) {
        threadPool.execute(runnable);
    }

    /**
     * Returns the instance of the ResponseListener for this Server.
     *
     * @return the ResponseListener for this Server, or null if there is none.
     */
    public ResponseListener getResponseListeners() {
        return responseListener;
    }

    /**
     * This sets the ResponseListener for entire server. All replys being sent
     * to any client will be notified to this instance.
     *
     * @param listener the instance of a ResponseListener to use for this
     * Server.
     */
    public void setResponseListener(ResponseListener listener) {
        this.responseListener = listener;
    }
}
