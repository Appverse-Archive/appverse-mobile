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
package com.gft.unity.core.notification;

public abstract class AbstractNotification implements INotification {

    /**
     * Default loading text.
     */
    // TODO hardcoded: not i18n
    protected static final String DEFAULT_LOADING_TEXT = "Loading...";

    public AbstractNotification() {
    }

    @Override
    public abstract boolean IsNotifyActivityRunning();

    @Override
    public abstract boolean IsNotifyLoadingRunning();

    @Override
    public abstract boolean StartNotifyActivity();

    @Override
    public abstract boolean StartNotifyAlert(String message);

    @Override
    public abstract boolean StartNotifyAlert(String title, String message,
            String buttonText);

    @Override
    public abstract boolean StartNotifyBeep();

    @Override
    public abstract boolean StartNotifyBlink();

    @Override
    public boolean StartNotifyLoading() {
        return StartNotifyLoading(DEFAULT_LOADING_TEXT);
    }

    @Override
    public abstract boolean StartNotifyLoading(String loadingText);

    @Override
    public abstract boolean StartNotifyVibrate();

    @Override
    public abstract boolean StopNotifyActivity();

    @Override
    public abstract boolean StopNotifyAlert();

    @Override
    public abstract boolean StopNotifyBeep();

    @Override
    public abstract boolean StopNotifyBlink();

    @Override
    public abstract boolean StopNotifyLoading();

    @Override
    public abstract boolean StopNotifyVibrate();

    @Override
    public abstract void UpdateNotifyLoading(float progress);
}
