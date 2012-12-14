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
package com.gft.unity.core.io;

import java.io.UnsupportedEncodingException;
import java.util.Arrays;

public class IOHeaders {

    private String content;
    private IOSessionContext session;
    private IOHeader[] headers;
    private String contentType;

    public IOHeaders() {
    }

    public IOHeader[] getHeaders() {
        return headers;
    }

    public void setHeaders(IOHeader[] headers) {
        this.headers = headers;
    }

    public String getContentType() {
        return contentType;
    }

    public void setContentType(String contentType) {
        this.contentType = contentType;
    }

    public String getContent() {
        return content;
    }

    public void setContent(String content) {
        this.content = content;
    }

    public IOSessionContext getSession() {
        return session;
    }

    public void setSession(IOSessionContext session) {
        this.session = session;
    }

    public int getContentLength() {
        int length = 0;

        byte[] rawContent = getRawContent();
        if (rawContent != null) {
            length = rawContent.length;
        }

        return length;
    }

    private byte[] getRawContent() {
        byte[] rawContent = null;

        if (content != null) {
            try {
                rawContent = content.getBytes("UTF8");
            } catch (UnsupportedEncodingException ex) {
            }
        }

        return rawContent;
    }

    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("IOHeaders [content=");
        builder.append(content);
        builder.append(", session=");
        builder.append(session);
        builder.append(", headers=");
        builder.append(Arrays.toString(headers));
        builder.append(", contentType=");
        builder.append(contentType);
        builder.append("]");
        return builder.toString();
    }
}
