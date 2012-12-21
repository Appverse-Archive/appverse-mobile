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

public abstract class AbstractSystem implements IDisplay, IHumanInteraction,
        IMemory, IOperatingSystem, IPower, IProcessor {

    // TODO hardcoded: OS_AGENT property
    protected static final String OS_AGENT = "Appverse 1.0";
    protected static final int PRIMARY_DISPLAY_NUMBER = 1;
    protected DisplayOrientation lockedOrientation;
    protected boolean locked;

    public AbstractSystem() {
        lockedOrientation = DisplayOrientation.Portrait;
        locked = false;
    }

    /**
     * Just for Unity internal use.
     *
     * @return
     */
    public abstract UnityContext GetUnityContext();

    @Override
    public abstract CPUInfo GetCPUInfo();

    @Override
    public abstract PowerInfo GetPowerInfo();

    @Override
    public long GetPowerRemainingTime() {
        long time = -1;

        PowerInfo pi = GetPowerInfo();
        if (pi != null && pi.getStatus() == PowerStatus.Discharging) {
            time = pi.getTime();
        }

        return time;
    }

    @Override
    public abstract HardwareInfo GetOSHardwareInfo();

    @Override
    public abstract OSInfo GetOSInfo();

    @Override
    public String GetOSUserAgent() {
        return OS_AGENT + "/" + GetOSInfo().toString();
    }

    @Override
    public abstract long GetMemoryAvailable(MemoryUse use);

    @Override
    public abstract long GetMemoryAvailable(MemoryUse use, MemoryType type);

    @Override
    public abstract MemoryType[] GetMemoryAvailableTypes();

    @Override
    public abstract MemoryStatus GetMemoryStatus();

    @Override
    public abstract MemoryStatus GetMemoryStatus(MemoryType type);

    @Override
    public abstract MemoryType[] GetMemoryTypes();

    @Override
    public abstract MemoryUse[] GetMemoryUses();

    @Override
    public abstract InputButton[] GetInputButtons();

    @Override
    public abstract InputGesture[] GetInputGestures();

    @Override
    public abstract InputCapability GetInputMethodCurrent();

    @Override
    public abstract InputCapability[] GetInputMethods();

    @Override
    public abstract Locale GetLocaleCurrent();

    @Override
    public abstract Locale[] GetLocaleSupported();

    @Override
    public abstract boolean CopyToClipboard(String text);

    @Override
    public DisplayInfo GetDisplayInfo() {
        return GetDisplayInfo(PRIMARY_DISPLAY_NUMBER);
    }

    @Override
    public abstract DisplayInfo GetDisplayInfo(int displayNumber);

    @Override
    public abstract int GetDisplays();

    @Override
    public DisplayOrientation GetLockedOrientation() {
        return lockedOrientation;
    }

    @Override
    public DisplayOrientation GetOrientation(int displayNumber) {
        DisplayOrientation orientation = DisplayOrientation.Unknown;

        DisplayInfo di = GetDisplayInfo(displayNumber);
        if (di != null) {
            orientation = di.getDisplayOrientation();
        }

        return orientation;
    }

    @Override
    public DisplayOrientation GetOrientationCurrent() {
        return GetDisplayInfo().getDisplayOrientation();
    }

    @Override
    public DisplayOrientation[] GetOrientationSupported() {
        return GetOrientationSupported(PRIMARY_DISPLAY_NUMBER);
    }

    @Override
    public abstract DisplayOrientation[] GetOrientationSupported(
            int displayNumber);

    @Override
    public boolean IsOrientationLocked() {
        return this.locked;
    }

    @Override
    public abstract void LockOrientation(boolean lock,
            DisplayOrientation orientation);

    /**
     * Shows the splash screen.
     *
     * @return true on success
     */
    @Override
    public abstract boolean ShowSplashScreen();

    /**
     * Dismisses the splash screen.
     *
     * @return true on success
     */
    @Override
    public abstract boolean DismissSplashScreen();
    
    /**
     * Dismisses the current running application.
     */
    @Override
    public abstract void DismissApplication();
}
