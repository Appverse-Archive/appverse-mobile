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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Devices;

namespace UnityPlatformWindowsPhone.Internals
{
    public class ScannerAutoFocus : IDisposable
    {
        private readonly Stopwatch _mTimeSinceLastBarcode = Stopwatch.StartNew();
        FocusControl _mFocusControl;
        bool _mBarcodeFound;

        public bool BarcodeFound
        {
            get
            {
                return _mBarcodeFound;
            }

            set
            {
                lock (this)
                {
                    _mBarcodeFound = value;
                    if (value)
                    {
                        _mTimeSinceLastBarcode.Restart();
                    }
                }
            }
        }

        public static async Task<ScannerAutoFocus> StartAsync(FocusControl control)
        {
            var autoFocus = new ScannerAutoFocus(control);

            AutoFocusRange range;
            if (control.SupportedFocusRanges.Contains(AutoFocusRange.FullRange))
            {
                range = AutoFocusRange.FullRange;
            }
            else if (control.SupportedFocusRanges.Contains(AutoFocusRange.Normal))
            {
                range = AutoFocusRange.Normal;
            }
            else
            {
                // Auto-focus disabled
                return autoFocus;
            }

            FocusMode mode;
            if (control.SupportedFocusModes.Contains(FocusMode.Continuous))
            {
                mode = FocusMode.Continuous;
            }
            else if (control.SupportedFocusModes.Contains(FocusMode.Single))
            {
                mode = FocusMode.Single;
            }
            else
            {
                // Auto-focus disabled
                return autoFocus;
            }

            if (mode == FocusMode.Continuous)
            {
                // True continuous auto-focus
                var settings = new FocusSettings()
                {
                    AutoFocusRange = range,
                    Mode = mode,
                    WaitForFocus = false,
                    DisableDriverFallback = false
                };
                control.Configure(settings);
                await control.FocusAsync();
            }
            else
            {
                // Simulated continuous auto-focus
                var settings = new FocusSettings()
                {
                    AutoFocusRange = range,
                    Mode = mode,
                    WaitForFocus = true,
                    DisableDriverFallback = false
                };
                control.Configure(settings);

                var ignore = Task.Run(async () => { await autoFocus.DriveAutoFocusAsync(); });
            }

            return autoFocus;
        }

        public ScannerAutoFocus(FocusControl focusControl)
        {
            _mFocusControl = focusControl;
        }

        public void Dispose()
        {
            lock (this)
            {
                _mFocusControl = null;
            }
        }

        async Task DriveAutoFocusAsync()
        {
            while (true)
            {
                FocusControl control;
                bool runFocusSweep;
                lock (this)
                {
                    if (_mFocusControl == null)
                    {
                        return;
                    }
                    control = _mFocusControl;
                    runFocusSweep = _mTimeSinceLastBarcode.ElapsedMilliseconds > 1000;
                }

                if (runFocusSweep)
                {
                    try
                    {
                        await control.FocusAsync();
                    }
                    catch
                    {
                        // Failing to focus is ok (happens when preview lacks texture)
                    }
                }

                await Task.Delay(1000);
            }
        }
    }
}
