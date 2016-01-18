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

public interface INet {

    /**
     * Get the preferred network type to reach the given url.
     *
     * @param url The url to check reachability.
     * @return Network type.
     */
    public NetworkType GetNetworkTypeReachable(String url);

    /**
     * List of network types available to reach the given url.
     *
     * @param url The url to check reachability.
     * @return List of network types, ordered by preference.
     */
    public NetworkType[] GetNetworkTypeReachableList(String url);

    /**
     * List of supported network types (cable, wifi, etc.) on the current
     * device.
     *
     * @return Network types.
     */
    public NetworkType[] GetNetworkTypeSupported();

    /**
     * Detect if network is reachable or not.
     *
     * @param url The url to check reachability.
     * @return <CODE>true</CODE> if reachable, <CODE>false</CODE> otherwise.
     */
    public boolean IsNetworkReachable(String url);

    /**
     * Opens the native Browser as a modal application view, and loads inside it
     * the given url path. This native view should have a top header with a
     * customizable title, and a close button (with a customizable label inside)
     * to close the view and return to the application that fired it. No native
     * browser toolbars should be added.
     *
     * @param title The title text inside the top view header. Defaults to
     * empty.
     * @param buttonText The text inside the close button. Defaults to "Close"
     * if not provided.
     * @param url The url to load inside the native browser.
     * @return <CODE>true</CODE> if browser could be opened, <CODE>false</CODE>
     * otherwise.
     */
    public boolean OpenBrowser(String title, String buttonText, String url);
    
    /**
     * Opens the native Browser as a modal application view, and loads inside it
     * the given url path. This native view should have a top header with a
     * customizable title, and a close button (with a customizable label inside)
     * to close the view and return to the application that fired it. No native
     * browser toolbars should be added. File Extensions in the passed object 
     * will be parsed and urls that expect a file with those extensions will be
     * handled by the Operating System
     *
     * @param browserOptions Title, button text, url and file extensions 
     * contained in a single object.
     * @return <CODE>true</CODE> if browser could be opened, <CODE>false</CODE>
     * otherwise.
     */
    public boolean OpenBrowserWithOptions(SecondaryBrowserOptions browserOptions);

    /**
     * Opens the native Browser as a modal application view, and loads inside it
     * the given HTML code. This native view should have a top header with a
     * customizable title, and a close button (with a customizable label inside)
     * to close the view and return to the application that fired it. No native
     * browser toolbars should be added.
     *
     * @param title The title text inside the top view header. Defaults to
     * empty.
     * @param buttonText The text inside the close button. Defaults to "Close"
     * if not provided.
     * @param html The HTML code (as an string) to load inside the native
     * browser.
     * @return <CODE>true</CODE> if browser could be opened, <CODE>false</CODE>
     * otherwise.
     */
    public boolean ShowHtml(String title, String buttonText, String html);
    
    /**
     * Opens the native Browser as a modal application view, and loads inside it
     * the given HTML code. This native view should have a top header with a
     * customizable title, and a close button (with a customizable label inside)
     * to close the view and return to the application that fired it. No native
     * browser toolbars should be added. Any URLS that request a file which
     * extension is passed in the object with be handled natively by the 
     * operating system
     *
     * @param browserOptions Title, button text, html code and file extensions 
     * @return <CODE>true</CODE> if browser could be opened, <CODE>false</CODE>
     * otherwise.
     */
    public boolean ShowHtmlWithOptions(SecondaryBrowserOptions browserOptions);

    /**
     * Downloads the given url file by using the default native handler.
     *
     * @param url The file url path to be opened.
     * @return <CODE>true</CODE> if file could be opened/downloaded,
     * <CODE>false</CODE> otherwise.
     */
    public boolean DownloadFile(String url);
    
    /**
     * Returns the IP address of the active connection (WIFI or 3G).
     *
     * @return NetworkData
     */
    public NetworkData GetNetworkData();
}
