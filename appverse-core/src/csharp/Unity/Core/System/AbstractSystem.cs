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
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Unity.Core.System.Launch;
using System.IO;
#if WP8
using System.Threading.Tasks;
#endif

namespace Unity.Core.System
{
    public abstract class AbstractSystem : IDisplay, IHumanInteraction, IMemory, IOperatingSystem, IPower, IProcessor
    {

#if !WP8
        private static string UNITY_VERSION = "0.1";
        protected bool locked = false;
        protected DisplayOrientation lockedOrientation = DisplayOrientation.Portrait;

        private static string LAUNCH_CONFIG_FILE = "app/config/launch-config.xml";
        private LaunchConfig launchConfig = new LaunchConfig();  // empty apps list
        private string _launchConfigFile = LAUNCH_CONFIG_FILE;

        public abstract UnityContext GetUnityContext();

        public virtual string LaunchConfigFile
        {
            get
            {
                return this._launchConfigFile;
            }
            set
            {
                this._launchConfigFile = value;
            }
        }

        #region Internal Unity Methods

        /// <summary>
        /// 
        /// </summary>
        public AbstractSystem()
        {
            LoadLaunchConfig();
        }

        


        /// <summary>
        /// Load launch config file
        /// </summary>
        protected void LoadLaunchConfig()
        {
            try
            {   // FileStream to read the XML document.
                byte[] configFileRawData = GetConfigFileBinaryData();
                if (configFileRawData != null && configFileRawData.Length > 0)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(LaunchConfig));
                    launchConfig = (LaunchConfig)serializer.Deserialize(new MemoryStream(configFileRawData));
                }
                else
                {
                    launchConfig = new LaunchConfig();  // the app has no launch config file available
                }
            }
            catch (Exception e)
            {
                SystemLogger.Log(SystemLogger.Module.CORE, "Error when loading launch configuration", e);
                launchConfig = new LaunchConfig(); // reset launch config mapping when the services could not be loaded for any reason
            }
        }

        /// <summary>
        /// Default method, to be overrided by platform implementation. 
        /// </summary>
        /// <returns>
        /// A <see cref="Stream"/>
        /// </returns>
        public virtual byte[] GetConfigFileBinaryData()
        {
            SystemLogger.Log(SystemLogger.Module.CORE, "# Loading Launch Apps Configuration from file: " + LaunchConfigFile);

            Stream fs = new FileStream(LaunchConfigFile, FileMode.Open);
            if (fs != null)
            {
                return ((MemoryStream)fs).GetBuffer();
            }
            else
            {
                return null;
            }
        }

        #endregion
#else
#endif

        #region Miembros de IDisplay
#if !WP8
		/// <summary>
		/// Returns primary display orientation.
		/// </summary>
		/// <returns>Display orientation.</returns>
		public DisplayOrientation GetOrientationCurrent ()
		{
			return GetDisplayInfo ().DisplayOrientation;
		}

		/// <summary>
		/// Returns display orientation given its display number.
		/// </summary>
		/// <param name="displayNumber">Display number.</param>
		/// <returns>Display orientation, or default (portrait) if displayNumber does not match any display.</returns>
		public DisplayOrientation GetOrientation (int displayNumber)
		{
			DisplayInfo di = GetDisplayInfo (displayNumber);
			if (di != null) {
				return di.DisplayOrientation;
			}

			return DisplayOrientation.Portrait;
		}

		/// <summary>
		/// Returns the supported orientations for the primary display.
		/// </summary>
		/// <returns>Supported orientations.</returns>
		public DisplayOrientation[] GetOrientationSupported ()
		{
			int primaryDisplayNumber = 1;
			return GetOrientationSupported (primaryDisplayNumber);
		}

		public abstract DisplayOrientation[] GetOrientationSupported (int displayNumber);

		public abstract int GetDisplays ();

		/// <summary>
		/// Returns the information for the primary display.
		/// </summary>
		/// <returns>Display information.</returns>
		public DisplayInfo GetDisplayInfo ()
		{
			int primaryDisplayNumber = 1;
			return GetDisplayInfo (primaryDisplayNumber);
		}

		public abstract DisplayInfo GetDisplayInfo (int displayNumber);
		
		/// <summary>
		/// Sets whether the current application should autorotate or not.
		/// If value is set to 'false', application's orientation will be set to the given orientation.
		/// </summary>
		/// <param name="lockOrientation">
		/// A <see cref="System.Boolean"/> value indicating whether the application view should autorotate; 'true' to remain on the specified orientation
		/// </param>
		/// <param name="orientation">
		/// A <see cref="DisplayOrientation"/> the orientation enum constant that the application should be locked, if lock is false this value is ignored
		/// </param>
		public void LockOrientation (bool lockOrientation, DisplayOrientation orientation)
		{
			this.locked = lockOrientation;
			this.lockedOrientation = orientation;
			
			// TODO Set current orientation to the given orientation.
			// Check on the future if the platform code could set the device current orientation to the given orientation. 
			// At this moment, this is not possible on some platforms that extend this Unity C# Core (such as iOS)
		}
		
		/// <summary>
		/// Indicates whether the current application if currently configured to autorotate or not.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/> If returned value is 'true', application remains on the default screen mode (portrait, in the iOS default case)
		/// </returns>
		public bool IsOrientationLocked ()
		{
			return this.locked;
		}
		
		/// <summary>
		/// Getter for the current locked orientation
		/// </summary>
		/// <returns>
		/// A <see cref="DisplayOrientation"/> the locked orientation
		/// </returns>
		public DisplayOrientation GetLockedOrientation ()
		{
			return this.lockedOrientation;
		}
		
		/// <summary>
		/// Shows the splash screen.
		/// </summary>
		/// <returns>
		/// The splash screen.
		/// </returns>
		public abstract bool ShowSplashScreen ();
		
		/// <summary>
		/// Dismisses the splash screen.
		/// </summary>
		/// <returns>
		/// The splash screen.
		/// </returns>
		public abstract bool DismissSplashScreen ();
#else
        public abstract Task<DisplayOrientation> GetOrientationCurrent();
        public abstract Task<DisplayOrientation> GetOrientation(int displayNumber);
        public abstract Task<DisplayOrientation[]> GetOrientationSupported();
        public abstract Task<DisplayOrientation[]> GetOrientationSupported(int displayNumber);
        public abstract Task<int> GetDisplays();
        public abstract Task<DisplayInfo> GetDisplayInfo();
        public abstract Task<DisplayInfo> GetDisplayInfo(int displayNumber);
        public abstract Task LockOrientation(bool lockOrientation, DisplayOrientation orientation);
        public abstract Task<bool> IsOrientationLocked();
        public abstract Task<DisplayOrientation> GetLockedOrientation();
        public abstract Task<bool> ShowSplashScreen();
        public abstract Task<bool> DismissSplashScreen();
#endif
        #endregion

        #region Miembros de IHumanInteraction
#if !WP8
		public abstract Locale[] GetLocaleSupported ();

		public abstract Locale GetLocaleCurrent ();

		public abstract InputCapability GetInputMethodCurrent ();

		public abstract InputCapability[] GetInputMethods ();

		public abstract InputGesture[] GetInputGestures ();

		public abstract InputButton[] GetInputButtons ();
		
		public abstract bool CopyToClipboard (string text);
#else
        public abstract Task<Locale[]> GetLocaleSupported();
        public abstract Task<Locale> GetLocaleCurrent();
        public abstract Task<InputCapability> GetInputMethodCurrent();
        public abstract Task<InputCapability[]> GetInputMethods();
        public abstract Task<InputGesture[]> GetInputGestures();
        public abstract Task<InputButton[]> GetInputButtons();
        public abstract Task<bool> CopyToClipboard(string text);
#endif
        #endregion

        #region Miembros de IMemory
#if !WP8
		/// <summary>
		/// Returns the available memory types from MemoryType enumeration.
		/// </summary>
		/// <returns>Array of available memory types, or empty array if error or no memory types available.</returns>
		public MemoryType[] GetMemoryTypes ()
		{
			List<MemoryType> memTypesList = new List<MemoryType> ();

			try {
				Type enumType = typeof(MemoryType);
				IEnumerator<MemoryType> enumerator = (Enum.GetValues (enumType) as IEnumerable<MemoryType>).GetEnumerator ();
				while (enumerator.MoveNext()) {
					memTypesList.Add (enumerator.Current);
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module .CORE, "Exception getting available memory types", e);
			}

			return memTypesList.ToArray ();
		}

		/// <summary>
		/// Returns the available memory uses from MemoryUse enumeration.
		/// </summary>
		/// <returns>Array of available memory uses, or empty array if error or no memory uses available.</returns>
		public MemoryUse[] GetMemoryUses ()
		{
			List<MemoryUse> memUsesList = new List<MemoryUse> ();

			try {
				Type enumType = typeof(MemoryUse);
				IEnumerator<MemoryUse> enumerator = (Enum.GetValues (enumType) as IEnumerable<MemoryUse>).GetEnumerator ();
				while (enumerator.MoveNext()) {
					memUsesList.Add (enumerator.Current);
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module .CORE, "Exception getting available memory uses", e);
			}

			return memUsesList.ToArray ();
		}

		public abstract MemoryType[] GetMemoryAvailableTypes ();

		public abstract MemoryStatus GetMemoryStatus ();

		public abstract MemoryStatus GetMemoryStatus (MemoryType type);

		public abstract long GetMemoryAvailable (MemoryUse use);

		public abstract long GetMemoryAvailable (MemoryUse use, MemoryType type);
#else
        public abstract Task<MemoryType[]> GetMemoryTypes();
        public abstract Task<MemoryUse[]> GetMemoryUses();
        public abstract Task<MemoryType[]> GetMemoryAvailableTypes();
        public abstract Task<MemoryStatus> GetMemoryStatus();
        public abstract Task<MemoryStatus> GetMemoryStatus(MemoryType type);
        public abstract Task<long> GetMemoryAvailable(MemoryUse use);
        public abstract Task<long> GetMemoryAvailable(MemoryUse use, MemoryType type);
#endif
        #endregion

        #region Miembros de IOperatingSystem
#if !WP8
		public abstract HardwareInfo GetOSHardwareInfo ();

		public abstract OSInfo GetOSInfo ();

		public abstract void DismissApplication ();

		public string GetOSUserAgent ()
		{
			return "Unity " + UNITY_VERSION + "/" + GetOSInfo ().ToString ();
		}

		/// <summary>
		/// Launches the given application with the needed launch data paramaters as a query string ().
		/// </summary>
		/// <param name="application">Application to be launched.</param>
		/// <param name="query">Query string in the format: "relative_url?param1=value1&param2=value2". Set it to null for not sending extra launch data.</param>
		public abstract void LaunchApplication (App application, string query);


		/// <summary>
		/// Launches the application given its name (matching it on the "app/config/launch-config.xml" configuration file).
		/// </summary>
		/// <param name="appName">App name for the application to be launched.</param>
		/// <param name="query">Query string in the format: "relative_url?param1=value1&param2=value2". Set it to null for not sending extra launch data.</param>
		public void LaunchApplication (string appName, string query)
		{
			App app = this.GetApplication (appName);
			if (app != null) {
				this.LaunchApplication (app, query);
			} else {
				// TODO - check if an alert notification is required
				SystemLogger.Log (SystemLogger.Module .CORE, "Application with name [" + appName + "] couldn't be found in the configuration file");
			}
		}

		/// <summary>
		/// Gets the application object given its name, matching it on the "app/config/launch-config.xml" configuration file.
		/// </summary>
		/// <returns>The application.</returns>
		/// <param name="appName">App name.</param>
		public App GetApplication (string appName)
		{
			App app = null;
			App[] apps = GetApplications ();
			if (apps != null) {
				foreach (App _app in apps) {
					if (_app.Name == appName) {
						app = _app;
						break;
					}
				}
			}
			return app;
		}

		/// <summary>
		/// Gets the application objects array configured on the "app/config/launch-config.xml" configuration file, if any.
		/// </summary>
		/// <returns>The applications.</returns>
		public virtual App[] GetApplications ()
		{
			return launchConfig.Apps.ToArray ();
		}
#else
        public abstract Task<UnityContext> GetUnityContext();
        public abstract Task<HardwareInfo> GetOSHardwareInfo();
        public abstract Task<OSInfo> GetOSInfo();
        public abstract Task<string> GetOSUserAgent();
        public abstract Task DismissApplication();
        public abstract Task LaunchApplication(App application, string query);
        public abstract Task LaunchApplication(string appName, string query);
        public abstract Task<App> GetApplication(string appName);
        public abstract Task<App[]> GetApplications();
#endif
        #endregion

        #region Miembros de IPower
#if !WP8
		/// <summary>
		/// Returns battery autonomy time if the battery is discharging or -1 if the device is connected to the main power supply.
		/// </summary>
		/// <returns>Time remaining in milliseconds or -1 if on external power.</returns>
		public long GetPowerRemainingTime ()
		{
			PowerInfo pi = GetPowerInfo ();
			if (pi.Status == PowerStatus.Discharging) {
				return pi.Time;
			} else {
				return -1;
			}
		}

		public abstract PowerInfo GetPowerInfo ();
#else
        public abstract Task<long> GetPowerRemainingTime();
        public abstract Task<PowerInfo> GetPowerInfo();
#endif
        #endregion

        #region Miembros de IProcessor
#if !WP8
		public abstract CPUInfo GetCPUInfo ();
#else
        public abstract Task<CPUInfo> GetCPUInfo();
#endif

        #endregion
    }
}
