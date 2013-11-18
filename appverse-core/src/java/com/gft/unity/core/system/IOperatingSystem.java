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
package com.gft.unity.core.system;

import com.gft.unity.core.system.launch.App;

public interface IOperatingSystem {

    /**
     * Provides information about the device hardware.
     *
     * @return Hardware information.
     */
    public HardwareInfo GetOSHardwareInfo();

    /**
     * Provides information about the operating system.
     *
     * @return Operating System information.
     */
    public OSInfo GetOSInfo();

    /**
     * Provides the current user agent String.
     *
     * @return User agent String.
     */
    public String GetOSUserAgent();
    
    /**
     * Dismisses or finishes the application programmatically.
     */
    public void DismissApplication();
    
    /**
     * Launches the given application with the needed launch data paramaters as a query string ().
     * @param application Application to be launched
     * @param query Query string in the format: "relative_url?param1=value1&param2=value2". Set it to null for not sending extra launch data.
     */
    public void LaunchApplication (App application, String query);

    /**
     * Launches the application given its name (matching it on the "app/config/launch-config.xml" configuration file).
     * @param appName Name of the application to be launched.
     * @param query Query string in the format: "relative_url?param1=value1&param2=value2". Set it to null for not sending extra launch data.
     */
    void LaunchApplication (String appName, String query);

    /**
     * Gets the application object given its name, matching it on the "app/config/launch-config.xml" configuration file.
     * @param appName The application name to be found
     * @return App object that matches the given application name.
     */
    public App GetApplication (String appName);

    /**
     * Gets the application objects array configured on the "app/config/launch-config.xml" configuration file, if any.
     * @return The App objects array configured.
     */
    public App[] GetApplications ();
}
