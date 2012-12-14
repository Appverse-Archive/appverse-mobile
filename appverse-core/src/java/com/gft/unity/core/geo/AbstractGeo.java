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
package com.gft.unity.core.geo;

public abstract class AbstractGeo implements ILocation, IMap {

    public AbstractGeo() {
    }

    @Override
    public abstract void GetMap();

    @Override
    public abstract POI GetPOI(String id);

    @Override
    public abstract POI[] GetPOIList(LocationCoordinate location, float radius);

    @Override
    public abstract POI[] GetPOIList(LocationCoordinate location, float radius,
            String queryText);

    @Override
    public abstract POI[] GetPOIList(LocationCoordinate location, float radius,
            String queryText, LocationCategory category);

    @Override
    public abstract POI[] GetPOIList(LocationCoordinate location, float radius,
            LocationCategory category);

    @Override
    public abstract boolean RemovePOI(String id);

    @Override
    public abstract void SetMapSettings(float scale, float boundingBox);

    @Override
    public abstract boolean UpdatePOI(POI poi);

    @Override
    public abstract Acceleration GetAcceleration();

    @Override
    public abstract LocationCoordinate GetCoordinates();

    @Override
    public abstract GeoDecoderAttributes GetGeoDecoder();

    @Override
    public abstract float GetHeading();

    @Override
    public abstract float GetHeading(NorthType type);

    @Override
    public abstract float GetVelocity();

    @Override
    public abstract boolean StartUpdatingHeading();

    @Override
    public abstract boolean StartUpdatingLocation();

    @Override
    public abstract boolean StopUpdatingHeading();

    @Override
    public abstract boolean StopUpdatingLocation();

    @Override
    public abstract DeviceOrientation GetDeviceOrientation();

    @Override
    public abstract boolean StartProximitySensor();

    @Override
    public abstract boolean StopProximitySensor();
    
    @Override
    public abstract boolean IsGPSEnabled();
}
