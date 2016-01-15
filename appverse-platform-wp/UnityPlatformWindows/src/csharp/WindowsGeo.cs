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
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Core.Geo;

namespace Unity.Platform.Windows
{
    public class WindowsGeo : AbstractGeo
    {
        public override Acceleration GetAcceleration()
        {
            throw new NotImplementedException();
        }

        public override LocationCoordinate GetCoordinates()
        {
            throw new NotImplementedException();
        }

        public override float GetHeading()
        {
            throw new NotImplementedException();
        }

        public override float GetHeading(NorthType type)
        {
            throw new NotImplementedException();
        }

        public override void GetMap()
        {
            throw new NotImplementedException();
        }

        public override DeviceOrientation GetDeviceOrientation()
        {
            throw new NotImplementedException();
        }

        public override POI GetPOI(string id)
        {
            throw new NotImplementedException();
        }

        public override POI[] GetPOIList(LocationCoordinate location, float radius)
        {
            throw new NotImplementedException();
        }

        public override POI[] GetPOIList(LocationCoordinate location, float radius, string queryText)
        {
            throw new NotImplementedException();
        }

        public override POI[] GetPOIList(LocationCoordinate location, float radius, LocationCategory category)
        {
            throw new NotImplementedException();
        }

        public override POI[] GetPOIList(LocationCoordinate location, float radius, string queryText, LocationCategory category)
        {
            throw new NotImplementedException();
        }

        public override float GetVelocity()
        {
            throw new NotImplementedException();
        }

        public override void SetMapSettings(float scale, float boundingBox)
        {
            throw new NotImplementedException();
        }

        public override bool RemovePOI(string id)
        {
            throw new NotImplementedException();
        }

        public override bool UpdatePOI(POI poi)
        {
            throw new NotImplementedException();
        }

        public override bool StartUpdatingLocation()
        {
            throw new NotImplementedException();
        }

        public override bool StopUpdatingLocation()
        {
            throw new NotImplementedException();
        }

        public override bool StartUpdatingHeading()
        {
            throw new NotImplementedException();
        }

        public override bool StopUpdatingHeading()
        {
            throw new NotImplementedException();
        }

        public override GeoDecoderAttributes GetGeoDecoder()
        {
            throw new NotImplementedException();
        }

        public override bool StartProximitySensor()
        {
            throw new NotImplementedException();
        }

        public override bool StopProximitySensor()
        {
            throw new NotImplementedException();
        }

        public override bool IsGPSEnabled()
        {
            throw new NotImplementedException();
        }
    }
}
