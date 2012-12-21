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
import java.io.PushbackInputStream;

public class InternetInputStream extends PushbackInputStream {

    public InternetInputStream(InputStream in, int size) {
        super(in, size);
    }

    public InternetInputStream(InputStream in) {
        super(in, 4096);
    }

    public String readline() throws IOException {
        StringBuffer buf = readBuffer();
        if (buf == null) {
            return null;
        }
        return buf.toString();
    }

    public StringBuffer readBuffer() throws IOException {
        StringBuffer buffer = null;

        int ch = -1;
        while ((ch = read()) >= 0) {
            if (buffer == null) {
                buffer = new StringBuffer();
            }
            if (ch == '\r') {
                ch = read();
                if (ch > 0 && ch != '\n') {
                    unread(ch);
                }
                break;
            } else if (ch == '\n') {
                break;
            }
            buffer.append((char) ch);
        }
        return buffer;
    }
}
