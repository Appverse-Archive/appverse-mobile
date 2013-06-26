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
package com.gft.unity.core.apploader;

public class ModuleContext {
    
    private String user;
    private String credentials;


    // LOADING MESSAGES
    private String loadingMessage_UpdateModules;
    private String loadingMessage_UpdateModule;
    private String loadingMessage_DeleteModule;

    // ALERT MESSAGES
    private String alertMessage_Title;
    private String alertMessage_LoadModuleError;

    public String getAlertMessage_LoadModuleError() {
        return alertMessage_LoadModuleError;
    }

    public void setAlertMessage_LoadModuleError(String alertMessage_LoadModuleError) {
        this.alertMessage_LoadModuleError = alertMessage_LoadModuleError;
    }

    public String getAlertMessage_Title() {
        return alertMessage_Title;
    }

    public void setAlertMessage_Title(String alertMessage_Title) {
        this.alertMessage_Title = alertMessage_Title;
    }

    public String getCredentials() {
        return credentials;
    }

    public void setCredentials(String credentials) {
        this.credentials = credentials;
    }

    public String getLoadingMessage_DeleteModule() {
        return loadingMessage_DeleteModule;
    }

    public void setLoadingMessage_DeleteModule(String loadingMessage_DeleteModule) {
        this.loadingMessage_DeleteModule = loadingMessage_DeleteModule;
    }

    public String getLoadingMessage_UpdateModule() {
        return loadingMessage_UpdateModule;
    }

    public void setLoadingMessage_UpdateModule(String loadingMessage_UpdateModule) {
        this.loadingMessage_UpdateModule = loadingMessage_UpdateModule;
    }

    public String getLoadingMessage_UpdateModules() {
        return loadingMessage_UpdateModules;
    }

    public void setLoadingMessage_UpdateModules(String loadingMessage_UpdateModules) {
        this.loadingMessage_UpdateModules = loadingMessage_UpdateModules;
    }

    public String getUser() {
        return user;
    }

    public void setUser(String user) {
        this.user = user;
    }
    
    
    
}
