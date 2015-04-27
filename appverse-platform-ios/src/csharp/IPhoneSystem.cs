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
using Unity.Core.System;
using UIKit;
using Foundation;
using System.Runtime.InteropServices;
using Unity.Core.System.Launch;
using ObjCRuntime;

namespace Unity.Platform.IPhone
{
    public class IPhoneSystem : AbstractSystem
    {

		public override string LaunchConfigFile { 
			get {
				return IPhoneUtils.GetInstance().GetFileFullPath(base.LaunchConfigFile);
			} 
		}

		/// <summary>
		/// Method overrided, to use NSData to get stream from file resource inside application. 
		/// </summary>
		/// <returns>
		/// A <see cref="Stream"/>
		/// </returns>
		public override byte[] GetConfigFileBinaryData ()
		{
			return IPhoneUtils.GetInstance().GetResourceAsBinary(this.LaunchConfigFile, true);
		}


		/// <summary>
		/// Launches the given application with the needed launch data paramaters as a query string ().
		/// </summary>
		/// <param name="application">Application to be launched.</param>
		/// <param name="query">Query string in the format: "relative_url?param1=value1&param2=value2". Set it to null for not sending extra launch data.</param>
		public override void LaunchApplication (App application, string query)
		{
			if (application != null && application.IOSApp != null) {
				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					string doubleSlash = "";
					if(!application.IOSApp.RemoveUriDoubleSlash) {
						doubleSlash = "//";
					}
					string urlString = application.IOSApp.UriScheme + ":" + doubleSlash + query;
					urlString = Uri.EscapeUriString(urlString); // MOBPLAT-201: avoid crash when using blanck spaces in the URL query
					NSUrl urlParam = new NSUrl (urlString);
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "Launching application by URL: " + urlParam.AbsoluteString);
					bool result = UIApplication.SharedApplication.OpenUrl (urlParam);
					if(!result) {
						SystemLogger.Log (SystemLogger.Module.PLATFORM, 
						                  "The system could not open the given url. Please check syntax.");
					}
				});
			} else {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, 
				                  "No application provided to launch, please check your first argument on API method invocation");
			}
		}

		public override UnityContext GetUnityContext()
        {
            UnityContext unityContext = new UnityContext();

            HardwareInfo hardwareInfo = this.GetOSHardwareInfo();
            if (hardwareInfo != null)
            {
                if (hardwareInfo.Version != null)
                {
                    String deviceModel = hardwareInfo.Version;
                    if (deviceModel.IndexOf("iPad") >= 0)
                    {
                        unityContext.iPad = true;
                    }
                    else if (deviceModel.IndexOf("iPhone") >= 0)
                    {
                        unityContext.iPhone = true;
                    }
                    else if (deviceModel.IndexOf("iPod") >= 0)
                    {
                        unityContext.iPod = true;
                    }
                }
            }

            return unityContext;

        }
		
		/// <summary>
		/// Returns supported orientations for given display number. 
		/// </summary>
		/// <param name="displayNumber">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="DisplayOrientation[]"/>
		/// </returns>
        public override DisplayOrientation[] GetOrientationSupported(int displayNumber)
        {
            if(displayNumber<=GetDisplays()) {
				// Display number is a valid number.
				
				// TODO how to get orientation supported for an external display, if case.
				
				return new DisplayOrientation[] { DisplayOrientation.Portrait, DisplayOrientation.Landscape};
			}
			
			return null;
        }

		/// <summary>
		/// For IPhone, only 1 display is possible.
		/// For IPad, Screens[].lenght is requested (exception thrown when iphone).
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
        public override int GetDisplays()
        {
			int numDisplays = 1;
			if(GetOSHardwareInfo().Version.IndexOf("iPad")>=0) {
				numDisplays = UIScreen.Screens.Length;
			}
			return numDisplays;
        }

		/// <summary>
		///  Get Display Information for the given display number.
		/// </summary>
		/// <param name="displayNumber">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="DisplayInfo"/>
		/// </returns>
        public override DisplayInfo GetDisplayInfo(int displayNumber)
        {
            DisplayInfo displayInfo = null;
			
			if(displayNumber<=GetDisplays()) {
				// Display number is a valid number.
				displayInfo = new DisplayInfo();
				displayInfo.DisplayNumber = displayNumber;
				displayInfo.DisplayBitDepth = DisplayBitDepth.Bpp24; // default bit depth for iphone/ipad.
				displayInfo.DisplayType = DisplayType.Primary;
				
				// Default is MainScreen
				UIScreen screen = UIScreen.MainScreen;
				
				// display orientation is by default the device orientation.
				displayInfo.DisplayOrientation = GetDeviceOrientation();
				
				if(GetOSHardwareInfo().Version.IndexOf("iPad")>=0) {
					UIScreen[] screens = UIScreen.Screens;
					screen = screens[displayNumber-1];
					
					// TODO how to get orientation from external display, if case.
					// DisplayOrientation ??
					// DisplayType.External ??
				}
				
				displayInfo.DisplayY = (int) screen.Bounds.Height;
				displayInfo.DisplayX = (int) screen.Bounds.Width;
				
				if(displayInfo.DisplayOrientation==DisplayOrientation.Landscape) {
					displayInfo.DisplayY = (int) screen.Bounds.Width;
					displayInfo.DisplayX = (int) screen.Bounds.Height;
				}
			}
						
			
			return displayInfo;
        }
		
		/// <summary>
		/// Get Current Device Orientation. 
		/// </summary>
		/// <returns>
		/// A <see cref="DisplayOrientation"/>
		/// </returns>
		private DisplayOrientation GetDeviceOrientation() 
		{
			DisplayOrientation orientation = DisplayOrientation.Unknown;
			
			// start generating orientation notifications.
			bool beginGeneratingNotifications = false;
			if(!UIDevice.CurrentDevice.GeneratesDeviceOrientationNotifications)
			{ 	
				UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
				beginGeneratingNotifications = true;
			}
			
			if(UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.Portrait ||
			   UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.PortraitUpsideDown) {
				orientation = DisplayOrientation.Portrait;
			} else if(UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft ||
			   UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight) {
				orientation = DisplayOrientation.Landscape;
			} 
			
			// stop generating notifications
			if(beginGeneratingNotifications)
			{
				UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
			}
			
			return orientation;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="Locale[]"/>
		/// </returns>
        public override Locale[] GetLocaleSupported()
        {
            string[] availableLocaleIdentifiers = NSLocale.AvailableLocaleIdentifiers;
			List<Locale> list = new List<Locale>();
			foreach(String localeIdentifier in availableLocaleIdentifiers)
			{
				list.Add(GetLocaleFromLocaleIdentifier(localeIdentifier));
			}
			list.Sort();  // default sort is using ToString() method.
			return list.ToArray();
        }

		/// <summary>
		/// Get Current Device Locale. 
		/// </summary>
		/// <returns>
		/// A <see cref="Locale"/>
		/// </returns>
        public override Locale GetLocaleCurrent()
        {
            string localeIdentifier = NSLocale.CurrentLocale.LocaleIdentifier;
			string localePreferredLanguage = null;
			string[] preferredLanguages = NSLocale.PreferredLanguages;
			if(preferredLanguages != null && preferredLanguages.Length > 0) {
				localePreferredLanguage = preferredLanguages[0];
			}
			
			Locale locale = GetLocaleFromLocaleIdentifier(localeIdentifier);
			if(localePreferredLanguage != null) {
				locale.Language = localePreferredLanguage; // setting language as by defined on the preferred locale language, not using "region format" setting.
			}	
			return locale;
        }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="localeIdentifier">
		/// A <see cref="System.String"/>
		/// </param>
		/// 
		/// <returns>
		/// A <see cref="Locale"/>
		/// </returns>
		private Locale GetLocaleFromLocaleIdentifier(string localeIdentifier)
		{
			Locale locale = null;
			if(localeIdentifier!=null) {
				locale = new Locale();
				locale.Language = localeIdentifier;
				string[] identifiers = localeIdentifier.Split('_');
				if(identifiers.Length>0) {
					locale.Language = identifiers[0];
				}
				if(identifiers.Length>1){
					locale.Country = identifiers[1];
				}
			}	
			return locale;
		}

        public override InputCapability GetInputMethodCurrent()
        {
            throw new NotImplementedException();
        }

        public override InputCapability[] GetInputMethods()
        {
            throw new NotImplementedException();
        }

        public override InputGesture[] GetInputGestures()
        {
            throw new NotImplementedException();
        }

        public override InputButton[] GetInputButtons()
        {
            throw new NotImplementedException();
        }
		
		public override bool CopyToClipboard(string text)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				try
				{
					UIPasteboard.General.SetValue((NSString)text,"public.utf8-plain-text");	
				}catch(Exception ex)
				{
					SystemLogger.Log(SystemLogger.Module.PLATFORM, ex.Message);
				}
			});
			return true;
		}

        public override MemoryType[] GetMemoryAvailableTypes()
        {
            throw new NotImplementedException();
        }

        public override MemoryStatus GetMemoryStatus()
        {
            throw new NotImplementedException();
        }

        public override MemoryStatus GetMemoryStatus(MemoryType type)
        {
            throw new NotImplementedException();
        }

        public override long GetMemoryAvailable(MemoryUse use)
        {
            throw new NotImplementedException();
        }

        public override long GetMemoryAvailable(MemoryUse use, MemoryType type)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		///  
		/// </summary>
		/// <returns>
		/// A <see cref="HardwareInfo"/>
		/// </returns>
        public override HardwareInfo GetOSHardwareInfo()
        {
            HardwareInfo hi = new HardwareInfo();
			
			UIDevice uiDevice = UIDevice.CurrentDevice; 
			hi.Name = uiDevice.Name;
			hi.UUID = uiDevice.IdentifierForVendor.AsString();
			hi.Vendor = "Apple Inc.";
			hi.Version = DeviceHardware.Version.ToString();  // uiDevice.Model  (not enough info)
			
			return hi;
        }

		/// <summary>
		/// Get Device Operating System Information. 
		/// </summary>
		/// <returns>
		/// A <see cref="OSInfo"/>
		/// </returns>
        public override OSInfo GetOSInfo()
        {
            OSInfo oi = new OSInfo();
			
			UIDevice uiDevice = UIDevice.CurrentDevice; 
			oi.Name = uiDevice.SystemName;
			oi.Version = uiDevice.SystemVersion;
			oi.Vendor = "Apple Inc.";

            return oi;
        }

		/// <summary>
		/// Gets Power Information (level & status) 
		/// </summary>
		/// <returns>
		/// A <see cref="PowerInfo"/>
		/// </returns>
        public override PowerInfo GetPowerInfo()
        {
            PowerInfo pi = new PowerInfo();
			UIDevice.CurrentDevice.BatteryMonitoringEnabled = true;  
			pi.Level = UIDevice.CurrentDevice.BatteryLevel * 100;
			
			// TODO calculate power remaining time.
			pi.Time = -1;
			
			if(UIDevice.CurrentDevice.BatteryState == UIDeviceBatteryState.Unplugged) {
				pi.Status = Unity.Core.System.PowerStatus.Discharging;
			} else if (UIDevice.CurrentDevice.BatteryState == UIDeviceBatteryState.Charging) {
				pi.Status = Unity.Core.System.PowerStatus.Charging;
			} else if (UIDevice.CurrentDevice.BatteryState == UIDeviceBatteryState.Full) {
				pi.Status = Unity.Core.System.PowerStatus.FullyCharged;
			} else {
				pi.Status = Unity.Core.System.PowerStatus.Unknown;
			}
			
			UIDevice.CurrentDevice.BatteryMonitoringEnabled = false;  	
				
			return pi;
        }
		
		/// <summary>
		/// Gets Processor Information 
		/// </summary>
		/// <returns>
		/// A <see cref="CPUInfo"/>
		/// </returns>
        public override CPUInfo GetCPUInfo()
        {
            CPUInfo ci = new CPUInfo();
            ci.Name = "Unknown";
            ci.Speed = 0;
            ci.Vendor = "Unknown";
            ci.UUID = "Unknown";
            
			// TODO get CPU information
			
            return ci;
        }
		
		/// <summary>
		/// Shows the splash screen.
		/// </summary>
		/// <returns>
		/// The splash screen.
		/// </returns>
		public override bool ShowSplashScreen () {
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				UIInterfaceOrientation orientation =  UIApplication.SharedApplication.StatusBarOrientation;
				IPhoneServiceLocator.CurrentDelegate.ShowSplashScreen(orientation);
			});
			return true;
		}
		
		/// <summary>
		/// Dismisses the splash screen.
		/// </summary>
		/// <returns>
		/// The splash screen.
		/// </returns>
		public override bool DismissSplashScreen () {
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				IPhoneServiceLocator.CurrentDelegate.DismissSplashScreen();
			});
			return true;
		}

		/// <summary>
		/// Dismisses or finishes the application programmatically.
		/// </summary>
		public override void DismissApplication() {
			// This feature is not available on the iOS platform.
		}
		
    }

	public class DeviceHardware
	{
		public const string HardwareProperty = "hw.machine";
		
		public enum HardwareVersion
		{
			iPhone,
			iPhone1G,
			iPhone3G,
			iPhone3GS,
			iPhone4,
			iPhone4_Verizon,
			iPhone4S,
			iPhone5,
			iPhone5C,
			iPhone5S,
			iPhone6Plus,
			iPhone6,
			iPod,
			iPod1G,
			iPod2G,
			iPod3G,
			iPod4G,
			iPod5G,
			iPad,
			iPad1,
			iPad1Wifi,
			iPad1GSM,
			iPad2,
			iPad2Wifi,
			iPad2GSM,
			iPad2CDMA,
			iPad3Wifi,
			iPad3GSM,
			iPad3Global,
			iPad4Wifi,
			iPad4GSM,
			iPad4Global,
			iPadAirWifi,
			iPadAirCellular,
			iPadAir2Wifi,
			iPadAir2Cellular,
			iPadMini1GWifi,
			iPadMini1GGSM,
			iPadMini1GGlobal,
			iPadMini2Wifi,
			iPadMini2Cellular,
			iPadMini3Wifi,
			iPadMini3Cellular,
			iPadMini3A1601,
			Simulator,
			Unknown
		}
		
		[DllImport(Constants.SystemLibrary)]
		internal static extern int sysctlbyname( [MarshalAs(UnmanagedType.LPStr)] string property, // name of the property
		                                        IntPtr output, // output
		                                        IntPtr oldLen, // IntPtr.Zero
		                                        IntPtr newp, // IntPtr.Zero
		                                        uint newlen // 0
		                                        );
		
		public static HardwareVersion Version
		{
			get
			{
				// get the length of the string that will be returned
				var pLen = Marshal.AllocHGlobal(sizeof(int));
				sysctlbyname(DeviceHardware.HardwareProperty, IntPtr.Zero, pLen, IntPtr.Zero, 0);
				
				var length = Marshal.ReadInt32(pLen);
				
				// check to see if we got a length
				if (length == 0)
				{
					Marshal.FreeHGlobal(pLen);
					return HardwareVersion.Unknown;
				}
				
				// get the hardware string
				var pStr = Marshal.AllocHGlobal(length);
				sysctlbyname(DeviceHardware.HardwareProperty, pStr, pLen, IntPtr.Zero, 0);
				
				// convert the native string into a C# string
				var hardwareStr = Marshal.PtrToStringAnsi(pStr);
				var ret = HardwareVersion.Unknown;
				//SystemLogger.Log(SystemLogger.Module.PLATFORM, hardwareStr);
				// determine which hardware we are running
				// matching source: https://github.com/monospacecollective/UIDevice-Hardware/blob/master/UIDevice-Hardware.m
				if (hardwareStr == "iPhone1,1")
					ret = HardwareVersion.iPhone1G;
				else if (hardwareStr == "iPhone1,2")
					ret = HardwareVersion.iPhone3G;
				else if (hardwareStr == "iPhone2,1")
					ret = HardwareVersion.iPhone3GS;
				else if (hardwareStr == "iPhone3,1")
					ret = HardwareVersion.iPhone4;
				else if (hardwareStr == "iPhone3,3")
					ret = HardwareVersion.iPhone4_Verizon;
				else if (hardwareStr == "iPhone4,1")
					ret = HardwareVersion.iPhone4S;
				else if (hardwareStr == "iPhone5,1")
					ret = HardwareVersion.iPhone5;
				else if (hardwareStr == "iPhone5,2")
					ret = HardwareVersion.iPhone5;
				else if (hardwareStr == "iPhone5,3")
					ret = HardwareVersion.iPhone5C;
				else if (hardwareStr == "iPhone5,4")
					ret = HardwareVersion.iPhone5C;
				else if (hardwareStr == "iPhone6,1")
					ret = HardwareVersion.iPhone5S;
				else if (hardwareStr == "iPhone6,2")
					ret = HardwareVersion.iPhone5S;
				else if (hardwareStr == "iPhone7,1")
					ret = HardwareVersion.iPhone6Plus;
				else if (hardwareStr == "iPhone7,2")
					ret = HardwareVersion.iPhone6;
				else if (hardwareStr.IndexOf("iPhone")==0)
					ret = HardwareVersion.iPhone;  // if details could not be obtained, at least check for model (iPhone, iPad, iPod)
				else if (hardwareStr == "iPod1,1")
					ret = HardwareVersion.iPod1G;
				else if (hardwareStr == "iPod2,1")
					ret = HardwareVersion.iPod2G;
				else if (hardwareStr == "iPod3,1")
					ret = HardwareVersion.iPod3G;
				else if (hardwareStr == "iPod4,1")
					ret = HardwareVersion.iPod4G;
				else if (hardwareStr == "iPod5,1")
					ret = HardwareVersion.iPod5G;
				else if (hardwareStr.IndexOf("iPod")==0)
					ret = HardwareVersion.iPod;  // if details could not be obtained, at least check for model (iPhone, iPad, iPod)
				else if (hardwareStr == "iPad1,1")      
					ret = HardwareVersion.iPad1Wifi;
				else if (hardwareStr == "iPad1,2")      
					ret = HardwareVersion.iPad1GSM;
				else if (hardwareStr.IndexOf("iPad1")==0)
					ret = HardwareVersion.iPad1;
				else if (hardwareStr == "iPad2,1")      
					ret = HardwareVersion.iPad2Wifi;
				else if (hardwareStr == "iPad2,2")      
					ret = HardwareVersion.iPad2GSM;
				else if (hardwareStr == "iPad2,3")      
					ret = HardwareVersion.iPad2CDMA;
				else if (hardwareStr == "iPad2,5")      
					ret = HardwareVersion.iPadMini1GWifi;
				else if (hardwareStr == "iPad2,6")      
					ret = HardwareVersion.iPadMini1GGSM;
				else if (hardwareStr == "iPad2,7")      
					ret = HardwareVersion.iPadMini1GGlobal;
				else if (hardwareStr.IndexOf("iPad2")==0)
					ret = HardwareVersion.iPad2;
				else if (hardwareStr == "iPad3,1")      
					ret = HardwareVersion.iPad3Wifi;
				else if (hardwareStr == "iPad3,2")      
					ret = HardwareVersion.iPad3GSM;
				else if (hardwareStr == "iPad3,3")      
					ret = HardwareVersion.iPad3Global;
				else if (hardwareStr == "iPad3,4")      
					ret = HardwareVersion.iPad4Wifi;
				else if (hardwareStr == "iPad3,5")      
					ret = HardwareVersion.iPad4GSM;
				else if (hardwareStr == "iPad3,6")      
					ret = HardwareVersion.iPad4Global;
				else if (hardwareStr == "iPad4,1")      
					ret = HardwareVersion.iPadAirWifi;
				else if (hardwareStr == "iPad4,2")      
					ret = HardwareVersion.iPadAirCellular;
				else if (hardwareStr == "iPad5,3")      
					ret = HardwareVersion.iPadAir2Wifi;
				else if (hardwareStr == "iPad4,4")      
					ret = HardwareVersion.iPadAir2Cellular;
				else if (hardwareStr == "iPad4,4")      
					ret = HardwareVersion.iPadMini2Wifi;
				else if (hardwareStr == "iPad4,5")      
					ret = HardwareVersion.iPadMini2Cellular;
				else if (hardwareStr == "iPad4,7")      
					ret = HardwareVersion.iPadMini3Wifi;
				else if (hardwareStr == "iPad4,8")      
					ret = HardwareVersion.iPadMini3Cellular;
				else if (hardwareStr == "iPad4,9")      
					ret = HardwareVersion.iPadMini3A1601;
				else if (hardwareStr.IndexOf("iPad")==0)
					ret = HardwareVersion.iPad;  // if details could not be obtained, at least check for model (iPhone, iPad, iPod)
				else if (hardwareStr == "x86")       
					ret = HardwareVersion.Simulator;
				else if (hardwareStr == "x86_64")       
					ret = HardwareVersion.Simulator;
				else if (hardwareStr == "i386")
					ret = HardwareVersion.Simulator;
				
				// cleanup
				Marshal.FreeHGlobal(pLen);
				Marshal.FreeHGlobal(pStr);
				
				return ret;
			}
		}
	}
}
