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
package com.gft.unity.core.io;

import java.util.Arrays;

public class IORequest extends IOHeaders {

    public IORequest() {
    }
    
    private String method;
    private HTTPProtocolVersion protocolVersion;
    private boolean stopAutoRedirect;
    private AttachmentData[] attachment;
    
     public String getMethod() {
        return method;
    }

    public void setMethod(String requestMethod) {
        this.method = requestMethod;
    }
    
     public HTTPProtocolVersion getProtocolVersion() {
        return protocolVersion;
    }

    public void setProtocolVersion(HTTPProtocolVersion protocolVersion) {
        this.protocolVersion = protocolVersion;
    }

    public boolean getStopAutoRedirect() {
        return stopAutoRedirect;
    }

    public void setStopAutoRedirect(boolean stopRedirect) {
        this.stopAutoRedirect = stopRedirect;
    }
    
    public AttachmentData[] getAttachment(){
        return this.attachment;    
    }
    
    public void setAttachment(AttachmentData[] attachment){
        this.attachment = attachment;
    }
    

    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("IORequest [getHeaders()=");
        builder.append(Arrays.toString(getHeaders()));
        builder.append(", getContentType()=");
        builder.append(getContentType());
        builder.append(", getContent()=");
        builder.append(getContent());
        builder.append(", getSession()=");
        builder.append(getSession());
        builder.append(", getMethod()=");
        builder.append(getMethod());
        builder.append(", getProtocolVersion()=");
        builder.append(getProtocolVersion().toString());
        builder.append(", getStopAutoRedirect()=");
        builder.append(getStopAutoRedirect());
        builder.append("]");
        return builder.toString();
    }
}
