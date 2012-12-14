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
import java.io.InputStream;
import java.io.OutputStream;

public class InputStreamResponseData implements ResponseData {

    private static final int SEND_BUFFER_SIZE = 4096;
    private InputStream theData;
    private long offset = 0;
    private long length = -1;

    public InputStreamResponseData(InputStream theData, long length) {
        this.theData = theData;
        this.length = length;
    }

    public InputStreamResponseData(InputStream theData, long offset, long length) {
        this.theData = theData;
        this.offset = offset;
        this.length = length;
    }

    @Override
    public long getLength() {
        if (offset < length) {
            return length - offset;
        } else {
            return -1;
        }
    }

    @Override
    public void send(OutputStream os) throws IOException {

        theData.skip(offset);
        byte[] buffer = new byte[Math.min(SEND_BUFFER_SIZE,
                (int) (length > 0L ? length : Integer.MAX_VALUE))];
        while (true) {
            int bufLen = theData.read(buffer);
            if (bufLen < 0) {
                break;
            }
            os.write(buffer, 0, bufLen);
        }
    }
}
