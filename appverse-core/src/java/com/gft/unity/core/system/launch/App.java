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

public class App {
 
    private String name;
    private AndroidApp androidApp;
    
    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public AndroidApp getAndroidApp() {
        return androidApp;
    }

    public void setAndroidApp(AndroidApp androidApp) {
        this.androidApp = androidApp;
    }
    
    
    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("App [name=");
        builder.append(name);
        builder.append(", androidApp=");
        builder.append(androidApp.toString());
        /*
        builder.append(", uriScheme=");
        builder.append(uriScheme);
        builder.append(", removeUriDoubleSlash=");
        builder.append(removeUriDoubleSlash);
        builder.append(", componentName=");
        builder.append(componentName);
        * 
        */
        builder.append("]");
        return builder.toString();
    }
    
}
