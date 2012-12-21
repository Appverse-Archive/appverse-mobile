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
package com.gft.unity.core.notification;

public interface INotification {

    /**
     * Checks if activity indicator animation is started.
     *
     * @return <CODE>true</CODE> if the activity indicator is running,
     * <CODE>false</CODE> otherwise.
     */
    public boolean IsNotifyActivityRunning();

    /**
     * Checks if progress indicator animation is started.
     *
     * @return <CODE>true</CODE> if the progress indicator is running,
     * <CODE>false</CODE> otherwise.
     */
    public boolean IsNotifyLoadingRunning();

    /**
     * Shows and starts the activity indicator animation.
     *
     * @return <CODE>true</CODE> if activity could be started,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StartNotifyActivity();

    /**
     * Starts an alert notification.
     *
     * @param message The alert message to be displayed.
     * @return <CODE>true</CODE> if alert notification could be started,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StartNotifyAlert(String message);

    /**
     * Starts an alert notification.
     *
     * @param title The alert title.
     * @param message The alert message to be displayed.
     * @param buttonText The button text.
     * @return <CODE>true</CODE> if alert notification could be started,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StartNotifyAlert(String title, String message,
            String buttonText);

    /**
     * Starts a beep notification.
     *
     * @return <CODE>true</CODE> if beep notification could be started,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StartNotifyBeep();

    /**
     * Starts a blink notification.
     *
     * @return <CODE>true</CODE> if blink notification could be started,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StartNotifyBlink();

    /**
     * Shows and starts the progress indicator animation. With a default loading
     * text.
     *
     * @return <CODE>true</CODE> if loading indicator could be started,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StartNotifyLoading();

    /**
     * Shows and starts the progress indicator animation.
     *
     * @param loadingText The message to be displayed on the loading view.
     * @return <CODE>true</CODE> if loading indicator could be started,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StartNotifyLoading(String loadingText);

    /**
     * Starts a vibrate notification.
     *
     * @return <CODE>true</CODE> if vibrate notification could be started,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StartNotifyVibrate();

    /**
     * Stops and hides the activity indicator animation.
     *
     * @return <CODE>true</CODE> if activity could be stopped,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StopNotifyActivity();

    /**
     * Stops an alert notification.
     *
     * @return <CODE>true</CODE> if alert notification could be stopped,
     * <CODE>false</CODE>otherwise.
     */
    public boolean StopNotifyAlert();

    /**
     * Stops a beep notification.
     *
     * @return <CODE>true</CODE> if beep notification could be stopped,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StopNotifyBeep();

    /**
     * Stops a blink notification.
     *
     * @return <CODE>true</CODE> if blink notification could be stopped,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StopNotifyBlink();

    /**
     * Stops and hides the progress indicator animation.
     *
     * @return <CODE>true</CODE> if loading indicator could be stopped,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StopNotifyLoading();

    /**
     * Stops a vibrate notification.
     *
     * @return <CODE>true</CODE> if vibrate notification could be stopped,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StopNotifyVibrate();

    /**
     * Updates the progress indicator animation.
     *
     * @param progress The current progress; values between 0.0 and 1.0
     * (completed).
     */
    public void UpdateNotifyLoading(float progress);
}
