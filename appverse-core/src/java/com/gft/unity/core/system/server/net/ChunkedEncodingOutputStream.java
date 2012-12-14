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

import java.io.FilterOutputStream;
import java.io.IOException;
import java.io.OutputStream;

public class ChunkedEncodingOutputStream extends FilterOutputStream {

    private static final int DEFAULT_CHUNK_SIZE = 4096;
    private byte[] buf = null;
    private int count = 0;

    public ChunkedEncodingOutputStream(OutputStream out, int maxChunkSize) {
        super(out);
        this.buf = new byte[maxChunkSize];
    }

    public ChunkedEncodingOutputStream(OutputStream out) {
        this(out, DEFAULT_CHUNK_SIZE);
    }

    @Override
    public void write(int b) throws IOException {
        if (count >= buf.length) {
            flush();
        }
        buf[count++] = (byte) b;
    }

    @Override
    public void write(byte b[]) throws IOException {
        this.write(b, 0, b.length);
    }

    @Override
    public void write(byte b[], int off, int len) throws IOException {
        for (int i = off; i < len; i++) {
            if (count >= buf.length) {
                flush();
            }
            buf[count++] = b[i];
        }
    }

    @Override
    public void flush() throws IOException {
        writeChunkSize(count);
        writeChunkData(buf, count);
        count = 0;
        out.flush();
    }

    @Override
    public void close() throws IOException {
        if (count > 0) {
            flush();
        }
        writeChunkEnding();
        out.close();
    }

    private void writeChunkSize(int count) throws IOException {
        if (count > 0) {
            out.write((Integer.toHexString(count)).getBytes());
            out.write(Http.CRLF.getBytes());
        }
    }

    private void writeChunkData(byte[] buf, int count) throws IOException {
        if (count > 0) {
            out.write(buf, 0, count);
            out.write(Http.CRLF.getBytes());
        }
    }

    private void writeChunkEnding() throws IOException {
        out.write("0".getBytes());
        out.write(Http.CRLF.getBytes());
        out.write(Http.CRLF.getBytes());
    }
}
