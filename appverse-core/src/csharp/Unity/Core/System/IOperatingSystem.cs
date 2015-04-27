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
#if WP8
using System.Threading.Tasks;
#endif
using Unity.Core.System.Launch;


namespace Unity.Core.System
{
    public interface IOperatingSystem
    {
#if !WP8
        /// <summary>
        /// Provides information about the device hardware.
        /// </summary>
        /// <returns>Hardware information.</returns>
        HardwareInfo GetOSHardwareInfo();

        /// <summary>
        /// Provides information about the operating system.
        /// </summary>
        /// <returns>Operating System information.</returns>
        OSInfo GetOSInfo();

        /// <summary>
        /// Provides the current user agent string.
        /// </summary>
        /// <returns>User agent string.</returns>
        string GetOSUserAgent();

        /// <summary>
        /// Dismisses or finishes the application programmatically.
        /// </summary>
        void DismissApplication();

        /// <summary>
        /// Launches the given application with the needed launch data paramaters as a query string ().
        /// </summary>
        /// <param name="application">Application to be launched.</param>
        /// <param name="query">Query string in the format: "relative_url?param1=value1&param2=value2". Set it to null for not sending extra launch data.</param>
        void LaunchApplication(App application, string query);

        /// <summary>
        /// Launches the application given its name (matching it on the "app/config/launch-config.xml" configuration file).
        /// </summary>
        /// <param name="appName">App name for the application to be launched.</param>
        /// <param name="query">Query string in the format: "relative_url?param1=value1&param2=value2". Set it to null for not sending extra launch data.</param>
        void LaunchApplication(string appName, string query);

        /// <summary>
        /// Gets the application object given its name, matching it on the "app/config/launch-config.xml" configuration file.
        /// </summary>
        /// <returns>The application.</returns>
        /// <param name="appName">App name.</param>
        App GetApplication(string appName);

        /// <summary>
        /// Gets the application objects array configured on the "app/config/launch-config.xml" configuration file, if any.
        /// </summary>
        /// <returns>The applications.</returns>
        App[] GetApplications();
#else
        /// <summary>
        /// Provides information about the device hardware.
        /// </summary>
        /// <returns>Hardware information.</returns>
        Task<HardwareInfo> GetOSHardwareInfo();

        /// <summary>
        /// Provides information about the operating system.
        /// </summary>
        /// <returns>Operating System information.</returns>
        Task<OSInfo> GetOSInfo();

        /// <summary>
        /// Provides the current user agent string.
        /// </summary>
        /// <returns>User agent string.</returns>
        Task<string> GetOSUserAgent();

        /// <summary>
        /// Dismisses or finishes the application programmatically.
        /// </summary>
        Task DismissApplication();

        /// <summary>
        /// Launches the given application with the needed launch data paramaters as a query string ().
        /// </summary>
        /// <param name="application">Application to be launched.</param>
        /// <param name="query">Query string in the format: "relative_url?param1=value1&param2=value2". Set it to null for not sending extra launch data.</param>
        Task LaunchApplication(App application, string query);

        /// <summary>
        /// Launches the application given its name (matching it on the "app/config/launch-config.xml" configuration file).
        /// </summary>
        /// <param name="appName">App name for the application to be launched.</param>
        /// <param name="query">Query string in the format: "relative_url?param1=value1&param2=value2". Set it to null for not sending extra launch data.</param>
        Task LaunchApplication(string appName, string query);

        /// <summary>
        /// Gets the application object given its name, matching it on the "app/config/launch-config.xml" configuration file.
        /// </summary>
        /// <returns>The application.</returns>
        /// <param name="appName">App name.</param>
        Task<App> GetApplication(string appName);

        /// <summary>
        /// Gets the application objects array configured on the "app/config/launch-config.xml" configuration file, if any.
        /// </summary>
        /// <returns>The applications.</returns>
        Task<App[]> GetApplications();
#endif

    }//end IOperatingSystem

}//end namespace System