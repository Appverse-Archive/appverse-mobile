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

package com.gft.unity.core.net;

import com.gft.unity.core.system.log.Logger;

/**
 *
 * @author DDBC
 * This class is used to set the Secondary Browser (opened with OpenURL functionality) with options
 */
public class SecondaryBrowserOptions {
    private String _title, _url, _html, _closeButtonText = "";
    private String[] _browserFileExtensions;
    
    private static final String LOGGER_MODULE = "Appverse SECONDARYBROWSER";
    private static final Logger LOGGER = Logger.getInstance(Logger.LogCategory.CORE,
            LOGGER_MODULE);
    
    public SecondaryBrowserOptions(){
        this.checkNullsAndSetDefaults();
    }
    
    public SecondaryBrowserOptions(String title, String url, String html, String closeButtonText, String[] fileExtensions){
        this._title = title;
        this._url = url;
        this._closeButtonText = closeButtonText;
        this._html = html;
        this._browserFileExtensions = fileExtensions;
    }

    /**
     * @return the Secondary Browser title
     */
    public String getTitle() {
        return _title;
    }

    /**
     * @param title the Secondary Browser title to set
     */
    public void setTitle(String title) {
        this._title = title;
    }

    /**
     * @return the Secondary Browser url
     */
    public String getUrl() {
        return _url;
    }

    /**
     * @param url the Secondary Browser url to set
     */
    public void setUrl(String url) {
        this._url = url;
    }

    /**
     * @return the Secondary Browser close Button Text
     */
    public String getCloseButtonText() {
        return _closeButtonText;
    }

    /**
     * @param closeButtonText the Secondary Browser close Button Text to set
     */
    public void setCloseButtonText(String closeButtonText) {
        this._closeButtonText = closeButtonText;
    }
    
    /**
     * @return the Secondary Browser Html text to show
     */
    public String getHtml() {
        return _html;
    }

    /**
     * @param closeButtonText the Secondary Browser Html text to show
     */
    public void setHtml(String html) {
        this._html = html;
    }

    /**
     * @return the Secondary Browser File Extensions to handle like the Operating System
     */
    public String[] getBrowserFileExtensions() {
        return _browserFileExtensions;
    }

    /**
     * @param browserFileExtensions the Secondary Browser File Extensions to set to behave like the Operating System
     */
    public void setBrowserFileExtensions(String[] browserFileExtensions) {
        this._browserFileExtensions = browserFileExtensions;
    }
    
    /**
     * @return the Secondary Browser File Extensions in a single String separated by semicolon ';' 
     */
    public String getBrowserFileExtensionsAsString() {
        StringBuilder sb = new StringBuilder();
        for(String sExtension: _browserFileExtensions){
            sb.append(sExtension);
            sb.append(';');
        }
        if(sb.length()>0)
            sb.deleteCharAt(sb.lastIndexOf(";"));
        return sb.toString();
    }
    
    public void checkNullsAndSetDefaults(){
        LOGGER.logInfo("TEST", "1");
        if(_title== null || _title.isEmpty()) _title = "Title";
        LOGGER.logInfo("TEST", "2");
        if(_html == null || _html.isEmpty()) _html = "";
        LOGGER.logInfo("TEST", "3");
        if((_url == null || _url.isEmpty()) && _html.isEmpty()) _url= "http://www.google.com";
        LOGGER.logInfo("TEST", "4");
        if(_closeButtonText == null || _closeButtonText.isEmpty())_closeButtonText = "Close";
        LOGGER.logInfo("TEST", "5");
        if(_browserFileExtensions==null || _browserFileExtensions.length==0)_browserFileExtensions = null;
        LOGGER.logInfo("TEST", "6");
    }
    
}
