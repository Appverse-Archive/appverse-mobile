/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://www.appverse.mobi/licenses/apl_v2.0.pdf.

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
using System.Globalization;
using System.Text;
using System.Threading;
using System.Management;
using System.Drawing;
using System.Windows.Forms;
using Unity.Core.System;

namespace Unity.Platform.Windows
{
    public class WindowsSystem : AbstractSystem
    {

        public override UnityContext GetUnityContext()
        {
            UnityContext unityContext = new UnityContext();
            unityContext.Windows = true;
            return unityContext;
        }

        /// <summary>
        /// Returns current Operating System information.
        /// </summary>
        /// <returns>Operating System information.</returns>
       public override OSInfo GetOSInfo()
        {
            OSInfo oi = new OSInfo();
            OperatingSystem os = System.Environment.OSVersion;
            oi.Name = os.Platform.ToString(); 
            oi.Vendor = "Microsoft Corporation";
            oi.Version = os.Version.ToString(4) ;
            return oi;
        }

        /// <summary>
        /// Returns current battery information.
        /// </summary>
        /// <returns>Power information.</returns>
        public override PowerInfo GetPowerInfo()
        {
            PowerInfo pi = new PowerInfo();
            pi.Level = SystemInformation.PowerStatus.BatteryLifePercent * 100;

            if (SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online)
            {
                pi.Time = -1;
                if (pi.Level == 100.0)
                {
                    pi.Status = Unity.Core.System.PowerStatus.FullyCharged;
                }
                else
                {
                    pi.Status = Unity.Core.System.PowerStatus.Charging;
                }
            }
            else
            {
                pi.Time = (SystemInformation.PowerStatus.BatteryLifeRemaining * 1000);
                pi.Status = Unity.Core.System.PowerStatus.Discharging;
            }
            return pi;
        }

        /// <summary>
        /// Returns current CPU information.
        /// </summary>
        /// <returns>CPU inforomation.</returns>
        public override CPUInfo GetCPUInfo()
        {
            CPUInfo ci = new CPUInfo();
            ci.Name = "Unknown";
            ci.Speed = 0;
            ci.Vendor = "Unknown";
            ci.UUID = "Unknown";
            string Key = "Win32_Processor";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + Key);
            foreach (ManagementObject share in searcher.Get())
            {
                foreach (PropertyData PC in share.Properties)
                {
                    if (PC.Name == "Name")
                    {
                        ci.Name = PC.Value.ToString();
                    }
                    if (PC.Name == "CurrentClockSpeed")
                    {
                        ci.Speed = Double.Parse(PC.Value.ToString());
                    }
                    if (PC.Name == "Manufacturer")
                    {
                        ci.Vendor = PC.Value.ToString();
                    }
                    if (PC.Name == "ProcessorId")
                    {
                        ci.UUID = PC.Value.ToString();
                    }
                }
            }
            return ci;
        }

        /// <summary>
        /// Returns current Hardware information.
        /// </summary>
        /// <returns>Hardware Information.</returns>
        public override HardwareInfo GetOSHardwareInfo()
        {
            HardwareInfo hi = new HardwareInfo();
            hi.Name = "Unknown";
            hi.UUID = "Unknown";
            hi.Version = "Unknown";
            hi.Vendor = "Unknown";

            string Key = "Win32_ComputerSystemProduct";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + Key);
            foreach (ManagementObject share in searcher.Get())
            {
                foreach (PropertyData PC in share.Properties)
                {
                    if (PC.Name == "Name")
                    {
                        hi.Name = PC.Value.ToString();
                    }
                    if (PC.Name == "UUID")
                    {
                        hi.UUID = PC.Value.ToString();
                    }
                    if (PC.Name == "Vendor")
                    {
                        hi.Vendor = PC.Value.ToString();
                    }
                    if (PC.Name == "Version")
                    {
                        hi.Version = PC.Value.ToString();
                    }
                }
            }

            return hi;
        }

        /// <summary>
        /// Returns windows memory types. 
        /// In this case, only Main type is available.
        /// </summary>
        /// <returns>Available memory types.</returns>
        public override MemoryType[] GetMemoryAvailableTypes()
        {
            return new MemoryType[] {MemoryType.Main};
        }

        /// <summary>
        /// Returns TOTAL memory status information.
        /// </summary>
        /// <returns>Memory status.</returns>
        public override MemoryStatus GetMemoryStatus()
        {
            MemoryStatus msApplication = GetApplicationMemoryStatus();
            MemoryStatus msDisk = GetDiskMemoryStatus();

            #if DEBUG
            Console.WriteLine("Physical total memory:   " + msApplication.MemoryTotal);
            Console.WriteLine("Physical free memory:    " + msApplication.MemoryFree);
            Console.WriteLine("Physical used memory:    " + msApplication.MemoryUsed);
            Console.WriteLine("Disk total space:        " + msDisk.MemoryTotal);
            Console.WriteLine("Disk free space:         " + msDisk.MemoryFree);
            Console.WriteLine("Disk used space:         " + msDisk.MemoryUsed);
            #endif

            MemoryStatus totalMemoryStatus = new MemoryStatus();
            totalMemoryStatus.MemoryTotal = msApplication.MemoryTotal + msDisk.MemoryTotal;
            totalMemoryStatus.MemoryFree = msApplication.MemoryFree + msDisk.MemoryFree;

            return totalMemoryStatus;

        }

        /// <summary>
        /// Returns memory information for the physical memory.
        /// </summary>
        /// <returns>Memory status.</returns>
        private MemoryStatus GetApplicationMemoryStatus()
        {
            MemoryStatus ms = new MemoryStatus();

            string Key = "Win32_LogicalMemoryConfiguration";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + Key);
            foreach (ManagementObject item in searcher.Get())
            {
                ms.MemoryTotal = Convert.ToInt64(item["TotalPhysicalMemory"]) * 1000; // in bytes.
            }
            Key = "Win32_PerfRawData_PerfOS_Memory";
            searcher = new ManagementObjectSearcher("select * from " + Key);
            foreach (ManagementObject item in searcher.Get())
            {
                ms.MemoryFree = Convert.ToInt64(item["AvailableBytes"]); // in bytes.
                
            }

            return ms;
        }

        /// <summary>
        /// Returns space infroamtion for the local disk.
        /// </summary>
        /// <returns>Memory status.</returns>
        private MemoryStatus GetDiskMemoryStatus()
        {
            MemoryStatus ms = new MemoryStatus();

            string Key = "Win32_LogicalDisk";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + Key);
            foreach (ManagementObject item in searcher.Get())
            {
                if (item["DriveType"].ToString() == "3")
                { // Local Disk
                    ms.MemoryFree = Convert.ToInt64(item["FreeSpace"]); // in bytes.
                    ms.MemoryTotal = Convert.ToInt64(item["Size"]); // in bytes.
                }
            }
            
            return ms;
        }

        /// <summary>
        /// Returns memory info for the given type.
        /// In this case, only one type is available, so method is returning total memory status.
        /// </summary>
        /// <param name="type">Memory type.</param>
        /// <returns>Memory status.</returns>
        public override MemoryStatus GetMemoryStatus(MemoryType type)
        {
            MemoryType[] availableMemTypes = GetMemoryAvailableTypes();
			if (availableMemTypes!= null && availableMemTypes.Length>0) {
				for (int i=0;i<availableMemTypes.Length;i++) {
					if (availableMemTypes[i] == type) {
						return GetMemoryStatus();
					}
				}
			} 
			return null;
           
        }

        /// <summary>
        /// Returns memory available for the given use (application or storage).
        /// </summary>
        /// <param name="use">Memory use.</param>
        /// <returns>Available memory.</returns>
        public override long GetMemoryAvailable(MemoryUse use)
        {
            if (use == MemoryUse.Application)
            {
                return GetApplicationMemoryStatus().MemoryFree;
            }
            else if (use == MemoryUse.Storage)
            {
                return GetDiskMemoryStatus().MemoryFree;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns memory available for the given use (application or storage) and type.
        /// In this case, only one type is available, so method is returning memory status depending on use.
        /// </summary>
        /// <param name="use">Memory use.</param>
        /// <param name="type">Memory type.</param>
        /// <returns>Available memory.</returns>
        public override long GetMemoryAvailable(MemoryUse use, MemoryType type)
        {
            return GetMemoryAvailable(use);
        }

        /// <summary>
        /// All displays will accept the two available orientations: portrait and landscape.
        /// </summary>
        /// <param name="displayNumber">Display Number</param>
        /// <returns>Supported display orientations.</returns>
        public override DisplayOrientation[] GetOrientationSupported(int displayNumber)
        {
            if (displayNumber < 0 || displayNumber > GetDisplays())
            {   // displayNumber is not a valid displayNumber.
                return null;
            }

            List<DisplayOrientation> list = new List<DisplayOrientation>();

            try
            {
                Type displayOrientationType = typeof(DisplayOrientation);
                IEnumerator<DisplayOrientation> enumerator = (Enum.GetValues(displayOrientationType) as IEnumerable<DisplayOrientation>).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    list.Add(enumerator.Current);
                }
            }
            catch (Exception e)
            {
            #if DEBUG
                Console.WriteLine("Exception getting supported display orientations: " + e);
            #endif
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the number of screens connected to the device.
        /// </summary>
        /// <returns>Number of displays.</returns>
        public override int GetDisplays()
        {
            return Screen.AllScreens.Length;
        }

        /// <summary>
        /// Returns the information for the given display number.
        /// Display array starts at 1 (primary display).
        /// </summary>
        /// <param name="displayNumber"></param>
        /// <returns>Display information, or null if display number </returns>
        public override DisplayInfo GetDisplayInfo(int displayNumber)
        {
            DisplayInfo[] diArray = GetDisplayInfoList();
            DisplayInfo displayInfo = null;
            if ((displayNumber - 1) < diArray.Length)
            {
                displayInfo = diArray[displayNumber - 1];
            }

            return displayInfo;
        }

        /// <summary>
        /// Provides information of screens connected to the device. Display 1 is the primary.
        /// </summary>
        /// <returns>Display info array.</returns>
        private DisplayInfo[] GetDisplayInfoList()
        {
            List<DisplayInfo> list = new List<DisplayInfo>();
            int i = 1;
            foreach (var screen in Screen.AllScreens)
            {
                DisplayInfo di = new DisplayInfo();

                di.DisplayNumber = i;
                di.DisplayX = screen.WorkingArea.Width;
                di.DisplayY = screen.WorkingArea.Height;

                if (screen.Primary)
                {
                    di.DisplayType = DisplayType.Primary;
                }
                else
                {
                    di.DisplayType = DisplayType.External;
                }

                switch(screen.BitsPerPixel) {
                    case -1:
                        di.DisplayBitDepth = DisplayBitDepth.Unknown;
                        break; 
                    case 32:
                         di.DisplayBitDepth = DisplayBitDepth.Bpp32;
                         break;
                    case 24:
                        di.DisplayBitDepth = DisplayBitDepth.Bpp24;
                        break;
                    case 16:
                        di.DisplayBitDepth = DisplayBitDepth.Bpp16;
                        break;
                    default:
                        di.DisplayBitDepth = DisplayBitDepth.Bpp8;
                        break;
                }

                //Compare height and width of screen and act accordingly.
                if (di.DisplayY > di.DisplayX)
                {
                    di.DisplayOrientation = DisplayOrientation.Portrait;
                }
                else
                {
                    di.DisplayOrientation = DisplayOrientation.Landscape;
                }

                list.Add(di);
                i++;
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the current Culture Information (current locale).
        /// </summary>
        /// <returns>Current Locale.</returns>
        public override Locale GetLocaleCurrent()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            return new Locale(currentCulture);
        }

        /// <summary>
        /// Returns the supported locales.
        /// List ordered by the "LanguageISOCode-CountryISOCode".
        /// </summary>
        /// <returns>Supported locales.</returns>
        public override Locale[] GetLocaleSupported()
        {
            List<Locale> list = new List<Locale>();
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                   list.Add(new Locale(ci));
            }
            list.Sort();  // default sort is using ToString() method.
            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override InputCapability GetInputMethodCurrent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns input methods supported by the device.
        /// </summary>
        /// <returns>Supported input methods.</returns>
        public override InputCapability[] GetInputMethods()
        {
            List<InputCapability> list = new List<InputCapability>();

            ManagementObjectSearcher searcherKeyboard = new ManagementObjectSearcher("select * from Win32_Keyboard");
            foreach (ManagementObject itemKb in searcherKeyboard.Get())
            {
                if (itemKb["Status"] != null && itemKb["Status"].ToString() == "OK")
                {
                    list.Add(InputCapability.ExternalKeyboard);
                }
            }

            ManagementObjectSearcher searcherPointingDevice = new ManagementObjectSearcher("select * from Win32_PointingDevice");
            foreach (ManagementObject itemPD in searcherPointingDevice.Get())
            {
               if (itemPD["Status"] != null && itemPD["Status"].ToString() == "OK")
                {
                    list.Add(InputCapability.ExternalPointingDevice);
                }
            }

            return list.ToArray();
        }

        public override bool CopyToClipboard(string text)
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

        /// <summary>
        /// Shows the splash screen.
        /// </summary>
        /// <returns>
        /// The splash screen.
        /// </returns>
        public override bool ShowSplashScreen()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dismisses the splash screen.
        /// </summary>
        /// <returns>
        /// The splash screen.
        /// </returns>
        public override bool DismissSplashScreen()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dismisses the current application.
        /// </summary>
        /// <returns>
        /// The splash screen.
        /// </returns>
        public override void DismissApplication()
        {
            throw new NotImplementedException();
        }

        public override void LaunchApplication(Unity.Core.System.Launch.App application, string query)
        {
            throw new NotImplementedException();
        }
    }
}
