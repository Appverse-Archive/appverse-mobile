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

public interface IDisplay {

    /**
     * Provides information about the primary display.
     *
     * @return Display information.
     */
    public DisplayInfo GetDisplayInfo();

    /**
     * Provides information about display number "displayNumber".
     *
     * @param displayNumber The display number.
     * @return Display information.
     */
    public DisplayInfo GetDisplayInfo(int displayNumber);

    /**
     * Provides the number of screens connected to the device. Display 1 is the
     * primary.
     *
     * @return The number of displays connected to the device.
     */
    public int GetDisplays();

    /**
     * Provides the current orientation of the given display number, 1 being the
     * primary display.
     *
     * @param displayNumber Screen identifier.
     * @return Display orientation.
     */
    public DisplayOrientation GetOrientation(int displayNumber);

    /**
     * Provides the current orientation of the primary display
     *
     * @return Display orientation.
     */
    public DisplayOrientation GetOrientationCurrent();

    /**
     * Provides the list of supported orientations of the primary display.
     *
     * @return List of supported orientations.
     */
    public DisplayOrientation[] GetOrientationSupported();

    /**
     * Provides the list of supported orientations for display number
     * "displayNumber".
     *
     * @param displayNumber Screen identifier.
     * @return List of supported orientations.
     */
    public DisplayOrientation[] GetOrientationSupported(int displayNumber);

    /**
     * Sets whether the current application should auto-rotate or not. If value
     * is set to 'true', application's orientation will be set to the given
     * orientation.
     *
     * @param lock <CODE>boolean</CODE> value indicating whether the application
     * view should auto-rotate; 'true' to remain on the specified orientation.
     * @param orientation DisplayOrientation the orientation enum constant that
     * the application should be locked, if lock is false this value is ignored.
     */
    public void LockOrientation(boolean lock, DisplayOrientation orientation);

    /**
     * Indicates whether the current application if currently configured to
     * auto-rotate or not.
     *
     * @return <CODE>true</CODE> if the application remains on the default
     * screen mode.
     */
    public boolean IsOrientationLocked();

    /**
     * Returns the current locked orientation.
     *
     * @return The locked orientation.
     */
    public DisplayOrientation GetLockedOrientation();

    /**
     * Shows the splash screen.
     *
     * @return true on success
     */
    public boolean ShowSplashScreen();

    /**
     * Dismisses the splash screen.
     *
     * @return true on success
     */
    public boolean DismissSplashScreen();
}
