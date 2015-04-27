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
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Unity.Platform.IPhone;

namespace UnityUI.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			try  {
				#if DEBUG
					Console.WriteLine("Starting main application...");
				#endif

				// if you want to use a different Application Delegate class from "AppDelegate"
				// you can specify it here.


				DeviceHardware.HardwareVersion hwVersion = DeviceHardware.Version;

				#if DEBUG
				Console.WriteLine("Current Device Version: " + hwVersion);
				#endif

				if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {

					if(	   hwVersion == DeviceHardware.HardwareVersion.iPhone1G
						|| hwVersion == DeviceHardware.HardwareVersion.iPhone3G
						|| hwVersion == DeviceHardware.HardwareVersion.iPhone3GS
						|| hwVersion == DeviceHardware.HardwareVersion.iPhone4
						|| hwVersion == DeviceHardware.HardwareVersion.iPhone4_Verizon
						|| hwVersion == DeviceHardware.HardwareVersion.iPhone4S
						|| hwVersion == DeviceHardware.HardwareVersion.iPhone5
						|| hwVersion == DeviceHardware.HardwareVersion.iPhone5C
						|| hwVersion == DeviceHardware.HardwareVersion.iPod1G
						|| hwVersion == DeviceHardware.HardwareVersion.iPod2G
						|| hwVersion == DeviceHardware.HardwareVersion.iPod3G
						|| hwVersion == DeviceHardware.HardwareVersion.iPod4G
						|| hwVersion == DeviceHardware.HardwareVersion.iPod5G
						|| hwVersion == DeviceHardware.HardwareVersion.iPad1Wifi
						|| hwVersion == DeviceHardware.HardwareVersion.iPad1GSM
						|| hwVersion == DeviceHardware.HardwareVersion.iPad2Wifi
						|| hwVersion == DeviceHardware.HardwareVersion.iPad2GSM
						|| hwVersion == DeviceHardware.HardwareVersion.iPad2CDMA
						|| hwVersion == DeviceHardware.HardwareVersion.iPadMini1GWifi
						|| hwVersion == DeviceHardware.HardwareVersion.iPadMini1GGSM
						|| hwVersion == DeviceHardware.HardwareVersion.iPadMini1GGlobal
						|| hwVersion == DeviceHardware.HardwareVersion.iPad3Wifi
						|| hwVersion == DeviceHardware.HardwareVersion.iPad3GSM
						|| hwVersion == DeviceHardware.HardwareVersion.iPad3Global
						|| hwVersion == DeviceHardware.HardwareVersion.iPad4Wifi
						|| hwVersion == DeviceHardware.HardwareVersion.iPad4GSM
						|| hwVersion == DeviceHardware.HardwareVersion.iPad4Global ) {
						
						#if DEBUG
						Console.WriteLine("Loading Application Delegate (support for iOS8)");
						#endif
						UIApplication.Main (args, null, "AppDelegate_WKWebView");
						
					} else {
						#if DEBUG
						Console.WriteLine("Loading Application Delegate (support for iOS8)... using UIWebview to avoid runtime issues (64 bits devices) ");
						#endif
						UIApplication.Main (args, null, "AppDelegate_UIWebView");
						
					}

				} else {
					#if DEBUG
					Console.WriteLine("Loading Application Delegate");
					#endif
					UIApplication.Main (args, null, "AppDelegate_UIWebView");
				}
				
				#if DEBUG
					Console.WriteLine("END main application");
				#endif
				
			} catch (Exception ex) {
				#if DEBUG
					Console.WriteLine("Exception on MAIN Application method: " + ex.Message);
					Console.WriteLine("Stacktrace ---------------------");
					Console.WriteLine(ex.StackTrace);
				#endif
			}
		}
	}
}
