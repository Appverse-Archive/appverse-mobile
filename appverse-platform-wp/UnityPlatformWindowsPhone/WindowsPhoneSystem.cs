/*
 Copyright (c) 2015 GFT Appverse, S.L., Sociedad Unipersonal.

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
using System.Globalization;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Phone.Devices.Power;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.System.UserProfile;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Unity.Core.System;
using Unity.Core.System.Launch;
using UnityPlatformWindowsPhone.Internals;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhoneSystem : AbstractSystem, IAppverseService
    {

        public DisplayOrientation CurrentDisplayOrientation { get; private set; }

        public WindowsPhoneSystem()
        {
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
        }

        #region OVERRIDE METHODS

        public override async Task<UnityContext> GetUnityContext()
        {
            //Windows Phone Platform only runs in Windows Phones, not tablets
            return new UnityContext
            {
                Windows = true
            };
        }

        public override async Task<DisplayOrientation> GetOrientationCurrent()
        {
            var currentOrientation = SimpleOrientationSensor.GetDefault().GetCurrentOrientation();

            switch (currentOrientation)
            {
                case SimpleOrientation.Faceup:
                case SimpleOrientation.Facedown:
                    break;
                case SimpleOrientation.NotRotated:
                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    CurrentDisplayOrientation = DisplayOrientation.Portrait;
                    break;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    CurrentDisplayOrientation = DisplayOrientation.Landscape;
                    break;
                default:
                    CurrentDisplayOrientation = DisplayOrientation.Unknown;
                    break;
            }
            return CurrentDisplayOrientation;
        }

        public override async Task<DisplayOrientation> GetOrientation(int displayNumber)
        {
            return await GetOrientationCurrent();
        }

        public override async Task<DisplayOrientation[]> GetOrientationSupported()
        {
            return await GetOrientationSupported(0);
        }

        public override async Task<DisplayOrientation[]> GetOrientationSupported(int displayNumber)
        {
            return new[] { DisplayOrientation.Portrait, DisplayOrientation.Landscape };
        }

        public override async Task<int> GetDisplays()
        {
            return 1;
        }

        public override async Task<DisplayInfo> GetDisplayInfo()
        {
            return null;

        }

        public override async Task<DisplayInfo> GetDisplayInfo(int displayNumber)
        {
            return await GetDisplayInfo();
        }

        public override async Task LockOrientation(bool lockOrientation, DisplayOrientation orientation)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> IsOrientationLocked()
        {
            throw new NotImplementedException();
        }

        public override async Task<DisplayOrientation> GetLockedOrientation()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> ShowSplashScreen()
        {
            await AppverseBridge.Instance.RuntimeHandler.ShowSplashScreen();
            return true;
        }

        public override async Task<bool> DismissSplashScreen()
        {
            await AppverseBridge.Instance.RuntimeHandler.DismissSplashScreen();
            return true;
        }

        public override async Task<Locale[]> GetLocaleSupported()
        {
            //NOT SUPPORTED IN STORE APPS
            throw new NotImplementedException();
        }

        public override async Task<Locale> GetLocaleCurrent()
        {
            return new Locale(new CultureInfo(GlobalizationPreferences.Languages[0]).Name);
        }

        public override async Task<InputCapability> GetInputMethodCurrent()
        {
            throw new NotImplementedException();
        }

        public override async Task<InputCapability[]> GetInputMethods()
        {
            throw new NotImplementedException();
        }

        public override async Task<InputGesture[]> GetInputGestures()
        {
            throw new NotImplementedException();
        }

        public override async Task<InputButton[]> GetInputButtons()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> CopyToClipboard(string text)
        {
            //NOT SUPPORTED IN WINDOWS STORE APPS
            throw new NotImplementedException();
        }

        public override async Task<MemoryType[]> GetMemoryTypes()
        {
            throw new NotImplementedException();
        }

        public override async Task<MemoryUse[]> GetMemoryUses()
        {
            throw new NotImplementedException();
        }

        public override async Task<MemoryType[]> GetMemoryAvailableTypes()
        {
            throw new NotImplementedException();
        }

        public override async Task<MemoryStatus> GetMemoryStatus()
        {
            throw new NotImplementedException();
        }

        public override async Task<MemoryStatus> GetMemoryStatus(MemoryType type)
        {
            throw new NotImplementedException();
        }

        public override async Task<long> GetMemoryAvailable(MemoryUse use)
        {
            throw new NotImplementedException();
        }

        public override async Task<long> GetMemoryAvailable(MemoryUse use, MemoryType type)
        {
            throw new NotImplementedException();
        }

        public override async Task<HardwareInfo> GetOSHardwareInfo()
        {
            var clientDeviceInformation = new EasClientDeviceInformation();

            var hardwareInfo = new HardwareInfo
            {
                Name = clientDeviceInformation.SystemProductName,
                Vendor = clientDeviceInformation.SystemManufacturer,
                Version = clientDeviceInformation.SystemHardwareVersion
            };

            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;
            var hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
            var hashed = hasher.HashData(hardwareId);
            hardwareInfo.UUID = CryptographicBuffer.EncodeToHexString(hashed);
            return hardwareInfo;
        }

        public override async Task<OSInfo> GetOSInfo()
        {
            var clientDeviceInformation = new EasClientDeviceInformation();

            return new OSInfo
            {
                Name = clientDeviceInformation.OperatingSystem,
                Vendor = "Microsoft",
                Version = clientDeviceInformation.SystemFirmwareVersion
            };
        }

        public override async Task<string> GetOSUserAgent()
        {
            var webview = AppverseBridge.Instance.RuntimeHandler.Webview;
            var userAgent = String.Empty;
            if (webview.Dispatcher.HasThreadAccess)
            {
                userAgent = await webview.InvokeScriptAsync("eval", new[] { "navigator.userAgent;" });
            }
            else
            {
                await webview.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                {
                    userAgent = await GetOSUserAgent();
                });
            }
            return userAgent;
        }

        public override async Task DismissApplication()
        {
            Application.Current.Exit();
        }

        public override async Task LaunchApplication(App application, string query)
        {
            throw new NotImplementedException();
        }

        public override async Task LaunchApplication(string appName, string query)
        {
            throw new NotImplementedException();
        }

        public override async Task<App> GetApplication(string appName)
        {
            throw new NotImplementedException();
        }

        public override async Task<App[]> GetApplications()
        {
            throw new NotImplementedException();
        }

        public override async Task<long> GetPowerRemainingTime()
        {
            return (long)Battery.GetDefault().RemainingDischargeTime.TotalMilliseconds;
        }

        public override async Task<PowerInfo> GetPowerInfo()
        {
            var battery = Battery.GetDefault();
            return new PowerInfo
            {
                Level = battery.RemainingChargePercent,
                Time = (long)battery.RemainingDischargeTime.TotalMilliseconds,
                Status = PowerStatus.Unknown
            };
        }

        public override async Task<CPUInfo> GetCPUInfo()
        {
            //Only possible to retrieve the number of CPUs and architecture of cpu
            //Environment.ProcessorCount
            //Package.Current.Id.Architecture
            return null;
        }

        #endregion

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }
    }
}
