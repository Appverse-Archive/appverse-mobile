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

import java.io.IOException;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.Map;

public class HttpHeaders {

    private Map<String, String> map;

    public HttpHeaders() {
        this.map = new LinkedHashMap<String, String>();
    }

    public HttpHeaders(InternetInputStream stream) throws IOException {
        this();

        String currentKey = null;
        while (true) {
            String line = stream.readline();
            if ((line == null) || (line.length() == 0)) {
                break;
            }

            if (!Character.isSpaceChar(line.charAt(0))) {
                int index = line.indexOf(':');
                if (index >= 0) {
                    currentKey = line.substring(0, index).trim();
                    String value = line.substring(index + 1).trim();
                    put(currentKey, value);
                }
            } else if (currentKey != null) {
                String value = get(currentKey);
                put(currentKey, value + "\r\n\t" + line.trim());
            }
        }
    }

    public String get(String key) {
        return map.get(key);
    }

    public String get(String key, String defaultValue) {
        String value = get(key);
        return (value == null) ? defaultValue : value;
    }

    public void put(String key, String value) {
        map.put(key, value);
    }

    public boolean contains(String headerKey) {
        return map.containsKey(headerKey);
    }

    public void clear() {
        map.clear();
    }

    public Iterator<String> iterator() {
        return map.keySet().iterator();
    }

    public void print(InternetOutputStream stream) throws IOException {
        for (Iterator<String> i = iterator(); i.hasNext();) {
            String key = i.next();
            stream.println(key + ": " + get(key));
        }

        stream.println();
        stream.flush();
    }
}
