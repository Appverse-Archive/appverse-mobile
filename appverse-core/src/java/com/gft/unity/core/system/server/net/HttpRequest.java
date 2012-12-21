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

import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.Socket;
import java.net.URLDecoder;
import java.util.Map;
import java.util.NoSuchElementException;
import java.util.Properties;
import java.util.Set;
import java.util.StringTokenizer;
import java.util.TreeMap;

/**
 * This holds the HTTP request data a given request.
 */
public class HttpRequest extends Request {

    private static int nextRequestId = 0;
    private Integer requestId = null;
    private String scheme;
    private String method;
    private String url;
    private String query;
    private String protocol;
    private int major;
    private int minor;
    private HttpHeaders headers;
    private byte[] postData;
    private Map<String, String> httpVariableMap;
    private String connectionHeader;
    private long timeStamp;

    public HttpRequest(String aScheme, Socket aConnection,
            Properties serverConfig) {
        super(aConnection, serverConfig);
        scheme = aScheme;
        connection = aConnection;
        init();
    }

    public HttpRequest(String url, Properties serverConfig, boolean isInternal) {
        super(null, serverConfig);
        init();
        method = "GET";
        scheme = "http";
        parseUrl(url);
        protocol = "HTTP/1.0";
        major = 1;
        minor = 0;
        headers = new HttpHeaders();
        setIsInternal(isInternal);
    }

    public boolean readRequest(InputStream aStream) throws IOException {
        InternetInputStream stream = new InternetInputStream(aStream);
        String startLine = null;
        try {
            startLine = readHttpCommand(stream);
            if (startLine == null) {
                return false;
            }
            if (protocol.equals("HTTP/1.0")) {
                major = 1;
                minor = 0;
            } else if (protocol.equals("HTTP/1.1")) {
                major = 1;
                minor = 1;
            } else {
                throw new HttpProtocolException(HttpURLConnection.HTTP_VERSION,
                        "Protocol " + protocol + " not supported.");
            }

            headers = new HttpHeaders(stream);
            readPostData(stream);
        } catch (NoSuchElementException e) {
            throw new HttpProtocolException(HttpURLConnection.HTTP_NOT_FOUND,
                    "Bad request " + startLine);
        } catch (NumberFormatException e) {
            throw new HttpProtocolException(
                    HttpURLConnection.HTTP_LENGTH_REQUIRED,
                    "Content Length was not a number or not supplied.");
        }
        return true;
    }

    private void init() {
        method = null;
        url = null;
        query = null;
        protocol = null;
        connectionHeader = null;
        postData = null;
        httpVariableMap = null;
        timeStamp = System.currentTimeMillis();
        connectionHeader = "Connection";
        requestId = new Integer(nextRequestId++);
    }

    public Integer getRequestId() {
        return requestId;
    }

    private String readHttpCommand(InternetInputStream stream)
            throws IOException {
        String startLine = null;
        do {
            startLine = stream.readline();
            if (startLine == null) {
                return null;
            }
        } while (startLine.trim().length() == 0);

        StringTokenizer tokenizer = new StringTokenizer(startLine);
        method = tokenizer.nextToken();
        parseUrl(tokenizer.nextToken());
        protocol = tokenizer.nextToken();

        return startLine;
    }

    private void readPostData(InternetInputStream stream) throws IOException {
        String contenLength = getRequestHeader("Content-Length");
        if (contenLength == null) {
            return;
        }

        int postLength = Integer.parseInt(contenLength);
        postData = new byte[postLength];

        int length = -1;
        int offset = stream.read(postData);
        while (offset >= 0 && offset < postData.length) {
            length = stream.read(postData, offset, postData.length - offset);
            if (length == -1) {
                break;
            }
            offset += length;
        }
    }

    private void parseUrl(String aUrl) {
        int queryIndex = aUrl.indexOf('?');
        if (queryIndex < 0) {
            url = aUrl;
        } else {
            url = aUrl.substring(0, queryIndex);
            if ((queryIndex + 1) < aUrl.length()) {
                query = aUrl.substring(queryIndex + 1);
            }
        }
    }

    public String getRequestHeader(String key) {
        return headers.get(key);
    }

    public String getRequestHeader(String key, String defaultValue) {
        String val = getRequestHeader(key);
        return (val == null) ? defaultValue : val;
    }

    public String getMethod() {
        return method;
    }

    public String getUrl() {
        return url;
    }

    public String getQuery() {
        return query;
    }

    public String getParameter(String key) {

        if (httpVariableMap == null) {
            httpVariableMap = createQueryMap(query);
            if (postData != null) {
                String contentType = headers.get("Content-Type");
                if ("application/x-www-form-urlencoded".equals(contentType)) {
                    httpVariableMap
                            .putAll(createQueryMap(new String(postData)));
                }
            }
        }

        return httpVariableMap.get(key);
    }

    public Set<String> getParameterNames() {
        return httpVariableMap.keySet();
    }

    private Map<String, String> createQueryMap(String query) {
        Map<String, String> queryMap = new TreeMap<String, String>();
        if (query == null) {
            return queryMap;
        }

        query = query.replace('+', ' ');
        StringTokenizer st = new StringTokenizer(query, "&");
        try {
            while (st.hasMoreTokens()) {
                String field = st.nextToken();
                int index = field.indexOf('=');
                if (index < 0) {
                    queryMap.put(URLDecoder.decode(field, "UTF-8"), "");
                } else {
                    queryMap.put(URLDecoder.decode(field.substring(0, index),
                            "UTF-8"), URLDecoder.decode(
                            field.substring(index + 1), "UTF-8"));
                }
            }
        } catch (UnsupportedEncodingException e) {
        }

        return queryMap;
    }

    public String getScheme() {
        return scheme;
    }

    public String getProtocol() {
        return protocol;
    }

    public byte[] getPostData() {
        return postData;
    }

    public boolean isKeepAlive() {
        if ("Keep-Alive".equalsIgnoreCase(getRequestHeader(connectionHeader))) {
            return true;
        } else if ("close".equalsIgnoreCase(getRequestHeader(connectionHeader))) {
            return false;
        } else if (major >= 1 && minor > 0) {
            return true;
        } else {
            return false;
        }
    }

    public int getMajorVersion() {
        return major;
    }

    public int getMinorVersion() {
        return minor;
    }

    public String getConnectionHeader() {
        return connectionHeader;
    }

    public long getTimestamp() {
        return timeStamp;
    }

    public String getProperty(String key) {
        return getProperty(key, null);
    }

    public HttpHeaders getHeaders() {
        return headers;
    }

    @Override
    public String toString() {
        return method + " " + url + ((query != null) ? "?" + query : "") + " "
                + protocol;
    }

    public String serverUrl() {
        return getProperty("url");
    }

    public String createUrl(String absolutePath) throws IOException {
        return absolutePath;
    }

    public boolean isProtocolVersionLessThan(int aMajor, int aMinor) {
        return (major <= aMajor && minor < aMinor);
    }
}
