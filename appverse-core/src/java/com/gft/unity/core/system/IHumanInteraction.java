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
package com.gft.unity.core.system;

public interface IHumanInteraction {

    /**
     * List of hardware buttons provided by the device.
     *
     * @return List of buttons.
     */
    public InputButton[] GetInputButtons();

    /**
     * List of gestures supported natively by the device.
     *
     * @return List of gestures.
     */
    public InputGesture[] GetInputGestures();

    /**
     * Currently active input method.
     *
     * @return Input method.
     */
    public InputCapability GetInputMethodCurrent();

    /**
     * Supported input methods.
     *
     * @return List of input methods supported by the device.
     */
    public InputCapability[] GetInputMethods();

    /**
     * Current locale of the device.
     *
     * @return Locale info.
     */
    public Locale GetLocaleCurrent();

    /**
     * List of configured Locales for the device.
     *
     * @return List of locales.
     */
    public Locale[] GetLocaleSupported();

    /**
     * Copies a specified text to the device clipboard.
     *
     * @param text Text to copy to the Clipboard
     * @return <CODE>true</CODE> if the text was successfully copied to
     * clipboard, <CODE>false</CODE> otherwise.
     */
    public boolean CopyToClipboard(String text);
}
