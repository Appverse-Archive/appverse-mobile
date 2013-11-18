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
package com.gft.unity.core.system.launch;

public class AndroidApp {
    
    
    private String action;
    private String mimeType;
    private String category;
    private String uriScheme;
    private boolean removeUriDoubleSlash;
    private String componentName;
    private boolean parseQueryAsIntentExtras;
    
    
    public String getUriScheme() {
        return uriScheme;
    }

    public void setUriScheme(String uriScheme) {
        this.uriScheme = uriScheme;
    }

    public boolean getRemoveUriDoubleSlash() {
        return removeUriDoubleSlash;
    }

    public void setRemoveUriDoubleSlash(boolean removeUriDoubleSlash) {
        this.removeUriDoubleSlash = removeUriDoubleSlash;
    }

    public String getComponentName() {
        return componentName;
    }

    public void setComponentName(String componentName) {
        this.componentName = componentName;
    }

    public String getAction() {
        return action;
    }

    public void setAction(String action) {
        this.action = action;
    }

    public String getCategory() {
        return category;
    }

    public void setCategory(String category) {
        this.category = category;
    }

    public String getMimeType() {
        return mimeType;
    }

    public void setMimeType(String mimeType) {
        this.mimeType = mimeType;
    }

    public boolean getParseQueryAsIntentExtras() {
        return parseQueryAsIntentExtras;
    }

    public void setParseQueryAsIntentExtras(boolean parseQueryAsIntentExtras) {
        this.parseQueryAsIntentExtras = parseQueryAsIntentExtras;
    }
    
    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("[action=");
        builder.append(action);
        builder.append(", mimeType=");
        builder.append(mimeType);
        builder.append(", category=");
        builder.append(category);
        builder.append(", uriScheme=");
        builder.append(uriScheme);
        builder.append(", removeUriDoubleSlash=");
        builder.append(removeUriDoubleSlash);
        builder.append(", parseQueryAsIntentExtras=");
        builder.append(parseQueryAsIntentExtras);
        builder.append(", componentName=");
        builder.append(componentName);
        builder.append("]");
        return builder.toString();
    }
}
