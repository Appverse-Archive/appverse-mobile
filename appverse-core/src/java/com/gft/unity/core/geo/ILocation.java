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
package com.gft.unity.core.geo;

public interface ILocation {

    /**
     * Gets the current device acceleration (measured in meters/second/second).
     *
     * @return Current acceleration.
     */
    public Acceleration GetAcceleration();

    /**
     * Gets the current device location coordinates.
     *
     * @return Current location.
     */
    public LocationCoordinate GetCoordinates();

    /**
     * Gets GeoDecoder values like street, country, postal code, ... from the
     * current longitude and latitude
     *
     * @return Device speed.
     */
    public GeoDecoderAttributes GetGeoDecoder();

    /**
     * The heading relative to the magnetic noth pole (default). Measured in
     * degrees, minutes and seconds.
     *
     * @return Current heading.
     */
    public float GetHeading();

    /**
     * The heading relative to the given north (magnetic or true north pole).
     * Measured in degrees, minutes and seconds.
     *
     * @param type Type of north to measured orientation relative to it.
     * @return Current heading.
     *
     */
    public float GetHeading(NorthType type);

    /**
     * Gets the current device velocity (in meters/second).
     *
     * @return Device speed.
     */
    public float GetVelocity();

    /**
     * Starts the heading services of the mobile device (heading, accuracy,
     * ...).
     *
     * @return <CODE>true</CODE> if the location services could be started,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StartUpdatingHeading();

    /**
     * Starts the location services of the mobile device (lonfitude, latitude,
     * altitude, speed, etc.).
     *
     * @return <CODE>true</CODE> if the location services could be started,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StartUpdatingLocation();

    /**
     * Stops the heading services of the mobile device (heading, accurary, ...).
     *
     * @return <CODE>true</CODE> if the location services could be stopped,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StopUpdatingHeading();

    /**
     * Stops the location services of the mobile device (lonfitude, latitude,
     * altitude, speed, etc.).
     *
     * @return <CODE>true</CODE> if the location services could be stopped,
     * <CODE>false</CODE> otherwise.
     */
    public boolean StopUpdatingLocation();

    /**
     * The orientation relative to the magnetic noth pole (default). Measured in
     * degrees, minutes and seconds.
     *
     * @return Current Device Orientation.
     */
    public DeviceOrientation GetDeviceOrientation();

    /**
     * The proximity sensor detects an object or person which is closed to the
     * device and then the display screen is disabled.
     *
     * @return <CODE>true</CODE> if the proximity sensor has been enabled
     * properly, <CODE>false</CODE> otherwise.
     */
    public boolean StartProximitySensor();

    /**
     * The proximity sensor detects an object or person which is closed to the
     * device and then the display screen is disabled. Stopping the proximity
     * sensor, the screen is not disabled when an object is closed to the
     * proximity sensor.
     *
     * @return <CODE>true</CODE> if the proximity sensor has been disabled
     * properly, <CODE>false</CODE> otherwise.
     */
    public boolean StopProximitySensor();
    
    /**
     * Determines whether the GPS service is enabled.
     *
     * @return <CODE>true</CODE> if the GPS service has been enabled
     * properly, <CODE>false</CODE> otherwise.
     */
    public boolean IsGPSEnabled();
}
