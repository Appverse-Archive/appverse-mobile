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

public class UnityContext {

    private boolean Windows = false;
    private boolean iPod = false;
    private boolean iPad = false;
    private boolean iPhone = false;
    private boolean Android = false;
    private boolean Blackberry = false;
    private boolean TabletDevice = false;

    public UnityContext() {
    }

    /**
     * @param Windows the Windows to set
     */
    public void setWindows(boolean Windows) {
        this.Windows = Windows;
    }

    /**
     * @param iPod the iPod to set
     */
    public void setiPod(boolean iPod) {
        this.iPod = iPod;
    }

    /**
     * @param iPad the iPad to set
     */
    public void setiPad(boolean iPad) {
        this.iPad = iPad;
    }

    /**
     * @param iPhone the iPhone to set
     */
    public void setiPhone(boolean iPhone) {
        this.iPhone = iPhone;
    }

    /**
     * @param Android the Android to set
     */
    public void setAndroid(boolean Android) {
        this.Android = Android;
    }

    /**
     * @param Blackberry the Blackberry to set
     */
    public void setBlackberry(boolean Blackberry) {
        this.Blackberry = Blackberry;
    }

    /**
     * @param TabletDevice the TabletDevice to set
     */
    public void setTabletDevice(boolean TabletDevice) {
        this.TabletDevice = TabletDevice;
    }

    /**
     * @return the Windows
     */
    public boolean getWindows() {
        return Windows;
    }

    /**
     * @return the iPod
     */
    public boolean getiPod() {
        return iPod;
    }

    /**
     * @return the iPad
     */
    public boolean getiPad() {
        return iPad;
    }

    /**
     * @return the iPhone
     */
    public boolean getiPhone() {
        return iPhone;
    }

    /**
     * @return the Android
     */
    public boolean getAndroid() {
        return Android;
    }

    /**
     * @return the Blackberry
     */
    public boolean getBlackberry() {
        return Blackberry;
    }

    /**
     * @return the TabletDevice
     */
    public boolean getTabletDevice() {
        return TabletDevice;
    }

    public boolean getTablet() {
        return (this.iPad || this.TabletDevice);
    }

    public boolean getPhone() {
        //return (!this.Desktop && !this.Tablet);
        return (!this.getTablet());
    }

    public boolean getiOS() {
        return (this.iPhone || this.iPad || this.iPod);
    }
}
