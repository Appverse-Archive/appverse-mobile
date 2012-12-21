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

import java.io.PrintStream;
import java.io.PrintWriter;
import java.util.Enumeration;
import java.util.Properties;
import java.util.Vector;

public class ChainableProperties extends Properties {

    private static final long serialVersionUID = 1L;
    private Properties parent;

    public ChainableProperties() {
        parent = null;
    }

    public ChainableProperties(Properties parent) {
        this.parent = parent;
    }

    public Properties getParent() {
        return parent;
    }

    @Override
    public String getProperty(String key) {
        String value = super.getProperty(key);

        if (value == null && parent != null) {
            value = parent.getProperty(key);
        }
        value = resolveVariables(value, null);

        return value;
    }

    @Override
    public String getProperty(String key, String defaultValue) {
        String value = super.getProperty(key, defaultValue);

        if (value == null && parent != null) {
            value = parent.getProperty(key, defaultValue);
        }
        value = resolveVariables(value, defaultValue);

        return value;
    }

    private String resolveVariables(String value, String defaultValue) {

        if (value != null) {
            // TODO provide a mechanism to escape sequence the ${
            int start = value.indexOf("${");
            StringBuffer buf = new StringBuffer(value.substring(0,
                    (start >= 0 ? start : value.length())));
            while (start >= 0) {
                int end = value.indexOf('}', start);
                String tmp = getProperty(value.substring(start + 2, end),
                        defaultValue);
                if (tmp != null) {
                    buf.append(tmp);
                } else {
                    buf.append(value.substring(start, end + 1));
                }
                start = value.indexOf("${", end);
                buf.append(value.substring(end + 1,
                        (start >= 0 ? start : value.length())));
            }
            return buf.toString();
        } else {
            return defaultValue;
        }
    }

    @Override
    public Enumeration<Object> propertyNames() {
        Vector<Object> set = new Vector<Object>(parent.keySet());
        set.addAll(super.keySet());
        return set.elements();
    }

    @Override
    public synchronized Object get(Object key) {
        Object value = super.get(key);

        if (value == null && parent != null) {
            value = parent.get(key);
        }

        return value;
    }

    @Override
    public synchronized boolean containsKey(Object key) {
        boolean contains = super.containsKey(key);

        if (!contains && parent != null) {
            contains = parent.containsKey(key);
        }

        return contains;
    }

    @Override
    public boolean containsValue(Object value) {
        boolean contains = super.containsValue(value);

        if (!contains && parent != null) {
            contains = parent.containsValue(value);
        }

        return contains;
    }

    @Override
    public synchronized boolean contains(Object value) {
        boolean contains = super.contains(value);

        if (!contains && parent != null) {
            contains = parent.contains(value);
        }

        return contains;
    }

    @Override
    public void list(PrintStream out) {
        super.list(out);

        parent.list(out);
    }

    @Override
    public void list(PrintWriter out) {
        super.list(out);

        parent.list(out);
    }
}