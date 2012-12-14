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
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;

public class ThreadPool {

    private static final String LOGGER_MODULE = "WebServer.ThreadPool";
    private static final Logger LOGGER = Logger.getInstance(LogCategory.CORE,
            LOGGER_MODULE);
    private List<Thread> threads = new ArrayList<Thread>();
    private LinkedList<Runnable> queue = new LinkedList<Runnable>();

    public ThreadPool(int numberOfThreads) {

        for (int i = 0; i < numberOfThreads; i++) {

            if (LOGGER.isLoggable(LogLevel.DEBUG)) {
                LOGGER.logDebug("<init>", "Creating thread " + i);
            }

            PooledThread thread = new PooledThread("Pooled Thread " + i);
            thread.setPriority(Thread.MAX_PRIORITY);
            thread.start();
            threads.add(thread);
        }
    }

    public void execute(Runnable runnable) {

        LOGGER.logDebug("execute", "Queueing runnable in thread pool.");

        synchronized (queue) {
            queue.add(runnable);
            queue.notify();
        }
    }

    public void shutdown() {

        for (int i = 0; i < threads.size(); i++) {
            Thread thread = threads.get(i);
            thread.interrupt();
        }
    }

    protected class PooledThread extends Thread {

        public PooledThread(String name) {
            super(name);
            setDaemon(false);
        }

        @Override
        public void run() {

            try {
                while (!isInterrupted()) {
                    waitForTask();
                    Runnable runnable = retrieveTask();
                    if (runnable != null) {
                        if (LOGGER.isLoggable(LogLevel.DEBUG)) {
                            LOGGER.logDebug("run",
                                    "Starting runnable on thread "
                                    + Thread.currentThread().getName());
                        }
                        try {
                            runnable.run();
                        } catch (Exception e) {
                            LOGGER.logError("run", e.toString(), e);
                        }
                    }
                    if (LOGGER.isLoggable(LogLevel.DEBUG)) {
                        LOGGER.logDebug("run", "Returning to thread pool "
                                + Thread.currentThread().getName());
                    }
                }
            } catch (InterruptedException e) {
                LOGGER.logDebug("run", Thread.currentThread().getName()
                        + " interrupted");
            } finally {
                LOGGER.logInfo("run", Thread.currentThread().getName()
                        + " is shutting down");
            }
        }

        private void waitForTask() throws InterruptedException {

            synchronized (queue) {
                if (queue.isEmpty()) {
                    queue.wait();
                }
            }
        }

        private Runnable retrieveTask() {

            Runnable runnable = null;
            synchronized (queue) {
                if (!queue.isEmpty()) {
                    runnable = queue.removeFirst();
                }
            }
            return runnable;
        }
    }
}
