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
package com.gft.unity.core.webtrekk;

public interface IWebtrekk {

    /**
     * Starts a tracking session.
     *
     * @param webServerUrl Webtrekk URL.
     * @param trackId Webtrekk tracking Id.
     * @return <CODE>true</CODE> if started successfully, <CODE>false</CODE>
     * otherwise.
     */
    public boolean StartTracking (String webServerUrl, String trackId);

    /**
     * Starts a tracking session.
     *
     * @param webServerUrl Webtrekk URL.
     * @param trackId Webtrekk tracking Id.
     * @param samplingRate Webtrekk sampling rate.
     * @return <CODE>true</CODE> if started successfully, <CODE>false</CODE>
     * otherwise.
     */
    public boolean StartTracking (String webServerUrl, String trackId, String samplingRate);
    
    /**
     * Stops a tracking session.
     *
     * @return <CODE>true</CODE> if stopped successfully, <CODE>false</CODE>
     * otherwise.
     */
    public boolean StopTracking ();

    /**
     * Sets the request time interval to contact the server.
     *
     * @param intervalInSeconds Time interval in seconds.
     * @return <CODE>true</CODE> if time interval was set correctly, <CODE>false</CODE>
     * otherwise.
     */
    public boolean SetRequestInterval (double intervalInSeconds);

     /**
     * Tracks content triggered by button event.
     *
     * @param clickId The button that triggered the event
     * @param contentId The content Id to be tracked
     * @return <CODE>true</CODE> if the content was added successfully,
     * <CODE>false</CODE> otherwise.
     */
    public boolean TrackClick(String clickId, String contentId);
    
    /**
     * Tracks content triggered by button event.
     *
     * @param clickId The button that triggered the event
     * @param contentId The content Id to be tracked
     * @param additionalParameters Additional parameters to track
     * @return <CODE>true</CODE> if the content was added successfully,
     * <CODE>false</CODE> otherwise.
     */
    public boolean TrackClick(String clickId, String contentId, WebtrekkParametersCollection additionalParameters);

    /**
     * Tracks simple content.
     *
     * @param contentId The content Id to be tracked
     * @return <CODE>true</CODE> if the content was added successfully,
     * <CODE>false</CODE> otherwise.
     */
    public boolean TrackContent(String contentId);
	
    /**
     * Tracks simple content.
     *
     * @param contentId The content Id to be tracked
     * @param additionalParameters Additional parameters to track
     * @return <CODE>true</CODE> if the content was added successfully,
     * <CODE>false</CODE> otherwise.
     */
    public boolean TrackContent(String contentId, WebtrekkParametersCollection additionalParameters);
}
