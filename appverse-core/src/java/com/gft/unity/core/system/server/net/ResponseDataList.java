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
import java.io.PrintWriter;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;

public class ResponseDataList {

    List<ResponseData> dataStreamList = new LinkedList<ResponseData>();

    public void addResponse(ResponseData data) {
        dataStreamList.add(data);
    }

    public void addResponse(InputStream stream, long length) {
        addResponse(stream, 0, length);
    }

    public void addResponse(InputStream stream, long offset, long length) {
        addResponse(new InputStreamResponseData(stream, offset, length));
    }

    public PrintWriter addPrintWriter() {

        PrintWriterResponseData data = new PrintWriterResponseData();
        addResponse(data);

        return data.getPrintWriter();
    }

    public long getTotalLength() {
        long total = 0;

        for (Iterator<ResponseData> it = dataStreamList.iterator(); it
                .hasNext();) {
            ResponseData responseData = it.next();
            long len = responseData.getLength();
            total = (total >= 0 && len > 0) ? total + len : -1;
        }

        return total;
    }

    public void sendData(OutputStream os, boolean isChunkedOk)
            throws IOException {

        try {
            if (getTotalLength() < 0 && isChunkedOk) {
                os = new ChunkedEncodingOutputStream(os);
            }
            for (Iterator<ResponseData> it = dataStreamList.iterator(); it
                    .hasNext();) {
                ResponseData responseData = (ResponseData) it.next();
                responseData.send(os);
            }
        } finally {
            dataStreamList.clear();
            os.flush();
            os.close();
        }
    }

    public void reset() {
        dataStreamList.clear();
    }
}
