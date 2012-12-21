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
import java.io.OutputStream;
import java.io.PrintWriter;
import java.io.StringWriter;
import java.net.HttpURLConnection;

/**
 * This holds the response data for the HTTP response.
 */
public class HttpResponse extends Response {

    private int statusCode = HttpURLConnection.HTTP_OK;
    public static String CUSTOM_RESPONSE_HEADERS = "$custom_response_headers_replace_me$";
    private String mimeType = "text/html";
    private HttpHeaders responseHeaders;
    private InternetOutputStream stream;
    private ResponseDataList dataStreamList;
    private HttpRequest request;
    private boolean keepConnectionOpen;
    private ResponseListener responseListener;

    public HttpResponse(HttpRequest request, OutputStream aStream) {
        this(request, aStream, null);
    }

    public HttpResponse(HttpRequest request, OutputStream aStream,
            ResponseListener listener) {
        this.stream = new InternetOutputStream(aStream);
        this.request = request;
        this.dataStreamList = new ResponseDataList();
        this.responseHeaders = new HttpHeaders();
        this.keepConnectionOpen = request.isKeepAlive();
        this.responseListener = listener;
    }

    public boolean isKeepAlive() {
        return (keepConnectionOpen && request.isKeepAlive());
    }

    public void addHeader(String key, String value) {
        responseHeaders.put(key, value);
    }

    public PrintWriter getPrintWriter() {
        return dataStreamList.addPrintWriter();
    }

    public void setMimeType(String aMimeType) {
        mimeType = aMimeType;
    }

    public void sendError(int statusCode, String errorMessage) {
        sendError(statusCode, errorMessage, null);
    }

    public void sendError(int statusCode, String errorMessage, Exception e) {
        keepConnectionOpen = false;
        String body = "<html>\n<head>\n"
                + "<title>Error: "
                + statusCode
                + "</title>\n"
                + "<body>\n<h1>"
                + statusCode
                + " <b>"
                + Http.getStatusPhrase(statusCode)
                + "</b></h1><br>\nThe requested URL <b>"
                + ((request.getUrl() == null) ? "<i>unknown URL</i>" : Http
                .encodeHtml(request.getUrl())) + "</b>\n "
                + Http.encodeHtml(errorMessage) + "\n<hr>";
        if (e != null) {
            StringWriter writer = new StringWriter();
            writer.write("<pre>");
            e.printStackTrace(new PrintWriter(writer));
            writer.write("</pre>");
            body += writer.toString();
        }
        body += "</body>\n</html>";

        this.dataStreamList.reset();
        this.statusCode = statusCode;
        this.mimeType = "text/html";
        PrintWriter out = getPrintWriter();
        out.write(body);
    }

    public void sendResponse(InputStream is, int length) throws IOException {
        this.dataStreamList.addResponse(is, length);
    }

    public void sendResponse(InputStream is, long beginning, long ending)
            throws IOException {
        this.dataStreamList.addResponse(is, beginning, ending - beginning);
    }

    public void setStatusCode(int statusCode) {
        this.statusCode = statusCode;
    }

    public void commitResponse() throws IOException {
        try {
            startTransfer();
            sendHttpReply(statusCode);
            sendHeaders(mimeType, dataStreamList.getTotalLength());
            if (!isHeadMethod()) {
                sendBody();
            }
            endTransfer();
        } catch (IOException e) {
            endTransfer(e);
            throw e;
        }
    }

    private void sendBody() throws IOException {
        dataStreamList.sendData(stream,
                !request.isProtocolVersionLessThan(1, 1));
    }

    private void sendHttpReply(int code) throws IOException {
        StringBuffer buffer = new StringBuffer(request.getProtocol());
        buffer.append(" ");
        buffer.append(code);
        buffer.append(" ");
        buffer.append(Http.getStatusPhrase(code));
        buffer.append(Http.CRLF);
        stream.write(buffer.toString().getBytes());
    }

    private void sendHeaders(String mimeType, long contentLength)
            throws IOException {
        // NOT NEEDED responseHeaders.put("Date", Http.getCurrentTime());
        responseHeaders.put("Server", "UnityEmbedded/1.0 (Android)");
        String str = request.isKeepAlive() ? "Keep-Alive" : "close";
        responseHeaders.put(request.getConnectionHeader(), str);
        if (contentLength >= 0) {
            responseHeaders.put("Content-Length", Long.toString(contentLength));
        } else if (!request.isProtocolVersionLessThan(1, 1)) {
            responseHeaders.put("Transfer-Encoding", "chunked");
        }

        if (mimeType != null) {
            responseHeaders.put("Content-Type", mimeType);
        }

        responseHeaders.put("Content-Encoding", "utf-8");

        // ADDING CUSTOM HEADERS(new feature)
        // NEEDS MORE TESTING responseHeaders = parseCustomHeaders(responseHeaders);

        responseHeaders.print(stream);
    }

    private HttpHeaders parseCustomHeaders(HttpHeaders headers) {
        /* NEEDS MORE TESTING
         try {
         System.out.println("****** CUSTOM HEADERS: " +HttpResponse.CUSTOM_RESPONSE_HEADERS);
         String[] customHeaders = HttpResponse.CUSTOM_RESPONSE_HEADERS.split("\r\n");
                
         for(String customHeader : customHeaders) {
         System.out.println("****** customHeader: " +customHeader);
         String[] customHeaderValues = customHeader.split(":");
                    
         if(customHeaderValues.length == 2) {
         headers.put(customHeaderValues[0],customHeaderValues[1]);
         System.out.println("****** [ " +customHeaderValues[0]+ " , "+customHeaderValues[1]+ "] ");
         }
         }
                
                
         } catch (Exception e) {
         // TODO log this error as warning, so defined custom headers could be properly defined.
         }
         */
        return headers;
    }

    private boolean isHeadMethod() {
        return "HEAD".equalsIgnoreCase(request.getMethod());
    }

    public OutputStream getOutputStream() {
        return stream;
    }

    protected void startTransfer() {
        if (responseListener != null) {
            responseListener.startTransfer(request);
        }
    }

    protected void notifyListeners(int bytesSent, int length)
            throws IOException {
        if (responseListener != null) {
            responseListener.notify(request, bytesSent, length);
        }
    }

    protected void endTransfer() {
        endTransfer(null);
    }

    protected void endTransfer(Exception e) {
        if (responseListener != null) {
            responseListener.endTransfer(request, e);
        }
    }
}
