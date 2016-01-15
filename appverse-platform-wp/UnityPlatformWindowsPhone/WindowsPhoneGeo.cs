/*
 Copyright (c) 2015 GFT Appverse, S.L., Sociedad Unipersonal.

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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Services.Maps;
using Unity.Core.Geo;
using UnityPlatformWindowsPhone.Internals;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhoneGeo : AbstractGeo, IAppverseService
    {
        private Geolocator _geoLocator;
        private Accelerometer _accelerometer;
        private Compass _compass;
        private CompassReading _lastCompassReading;
        private Geocoordinate _lastKnownCoordinate;
        private AccelerometerReading _lastAccelerometerReading;

        public WindowsPhoneGeo()
        {
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
        }

        public override async Task<Acceleration> GetAcceleration()
        {
            return new Acceleration { X = (float)_lastAccelerometerReading.AccelerationX, Y = (float)_lastAccelerometerReading.AccelerationY, Z = (float)_lastAccelerometerReading.AccelerationZ };
        }

        public override async Task<float> GetHeading()
        {
            return await GetHeading(NorthType.MagneticNorth);
        }

        public override async Task<float> GetHeading(NorthType type)
        {
            if (_lastCompassReading == null) return 0;
            float returnHeading = 0;
            switch (type)
            {
                case NorthType.MagneticNorth:
                    returnHeading = (float)_lastCompassReading.HeadingMagneticNorth;
                    break;
                case NorthType.TrueNorth:
                    returnHeading = (float)(_lastCompassReading.HeadingTrueNorth ?? 0);
                    break;
            }
            return returnHeading;
        }

        public override async Task<DeviceOrientation> GetDeviceOrientation()
        {
            try
            {
                var orientationSensor = SimpleOrientationSensor.GetDefault();
                if (orientationSensor == null) return DeviceOrientation.Unknown;
                var currentOrientation = orientationSensor.GetCurrentOrientation();
                var returnOrientation = DeviceOrientation.Unknown;
                switch (currentOrientation)
                {
                    case SimpleOrientation.NotRotated:
                        returnOrientation = DeviceOrientation.Portrait;
                        break;
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:
                        returnOrientation = DeviceOrientation.LandscapeLeft;
                        break;
                    case SimpleOrientation.Rotated180DegreesCounterclockwise:
                        returnOrientation = DeviceOrientation.PortraitUpsideDown;
                        break;
                    case SimpleOrientation.Rotated270DegreesCounterclockwise:
                        returnOrientation = DeviceOrientation.LandscapeRight;
                        break;
                    case SimpleOrientation.Faceup:
                        returnOrientation = DeviceOrientation.FaceUp;
                        break;
                    case SimpleOrientation.Facedown:
                        returnOrientation = DeviceOrientation.FaceDown;
                        break;
                }
                return returnOrientation;
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
                return DeviceOrientation.Unknown;
            }
        }

        public override async Task<LocationCoordinate> GetCoordinates()
        {

            try
            {
                var currentPosition = _lastKnownCoordinate ?? (await _geoLocator.GetGeopositionAsync(new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 6))).Coordinate;
                var returnCoordinate = new LocationCoordinate
                {
                    XCoordinate = currentPosition.Point.Position.Latitude,
                    YCoordinate = currentPosition.Point.Position.Longitude,
                    ZCoordinate = currentPosition.Point.Position.Altitude
                };

                if (_lastKnownCoordinate == null || _lastKnownCoordinate.SatelliteData == null) return returnCoordinate;
                returnCoordinate.XDoP = (float)(_lastKnownCoordinate.SatelliteData.HorizontalDilutionOfPrecision ?? 0);
                returnCoordinate.YDoP = (float)(_lastKnownCoordinate.SatelliteData.VerticalDilutionOfPrecision ?? 0);
                return returnCoordinate;
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
            }
            return null;
        }


        public override async Task<float> GetVelocity()
        {
            return _lastKnownCoordinate != null && _lastKnownCoordinate.Speed != null
                ? (float)_lastKnownCoordinate.Speed
                : 0;
        }

        public override async Task<bool> StartUpdatingLocation()
        {
            try
            {
                if (_geoLocator == null)
                {
                    _geoLocator = new Geolocator { DesiredAccuracy = PositionAccuracy.High, MovementThreshold = 20 };
                    _geoLocator.PositionChanged += GeoOnPositionChanged;
                    _geoLocator.StatusChanged += GeoOnStatusChanged;
                    _accelerometer = Accelerometer.GetDefault();
                    var minReportInterval = _accelerometer.MinimumReportInterval;
                    _accelerometer.ReportInterval = minReportInterval > 16 ? minReportInterval : 16;
                    _accelerometer.ReadingChanged += AccelerometerOnReadingChanged;
                }
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
                return false;
            }
            return true;
        }

        public override async Task<bool> StopUpdatingLocation()
        {
            try
            {
                if (_geoLocator != null)
                {
                    _accelerometer.ReadingChanged -= AccelerometerOnReadingChanged;
                    _accelerometer = null;
                    _geoLocator.PositionChanged -= GeoOnPositionChanged;
                    _geoLocator.StatusChanged -= GeoOnStatusChanged;
                    _geoLocator = null;
                }
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
                return false;
            }
            return true;
        }

        public override async Task<bool> StartUpdatingHeading()
        {
            _compass = _compass ?? Compass.GetDefault();
            if (_compass == null) return false;
            var minReportInterval = _compass.MinimumReportInterval;
            _compass.ReportInterval = minReportInterval > 16 ? minReportInterval : 16;
            _compass.ReadingChanged += CompassOnReadingChanged;
            return true;
        }



        public override async Task<bool> StopUpdatingHeading()
        {
            try
            {
                _compass.ReadingChanged -= CompassOnReadingChanged;
                _compass = null;
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
                return false;
            }
            return true;
        }

        public override async Task<GeoDecoderAttributes> GetGeoDecoder()
        {
            var result = await MapLocationFinder.FindLocationsAtAsync(_lastKnownCoordinate.Point);

            if (result.Status != MapLocationFinderStatus.Success) return null;

            return new GeoDecoderAttributes
             {
                 AdditionalCityLevelInfo = result.Locations[0].Address.Town,
                 AdditionalStreetLevelInfo = result.Locations[0].Address.StreetNumber,
                 Country = result.Locations[0].Address.Country,
                 CountryCode = result.Locations[0].Address.CountryCode,
                 PostalCode = result.Locations[0].Address.PostCode,
                 Locality = result.Locations[0].Address.District,
                 StreetAddress = result.Locations[0].Address.Street
             };
        }

        public override async Task<bool> StartProximitySensor()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> StopProximitySensor()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> IsGPSEnabled()
        {
            _geoLocator = _geoLocator ??
                          new Geolocator();

            var bGPSisEnabled = _geoLocator.LocationStatus != PositionStatus.Disabled;
            _geoLocator = null;
            return bGPSisEnabled;
        }

        public override async Task<POI[]> GetPOIList(LocationCoordinate location, float radius)
        {
            throw new NotImplementedException();
        }

        public override async Task<POI[]> GetPOIList(LocationCoordinate location, float radius, string queryText)
        {
            throw new NotImplementedException();
        }

        public override async Task<POI[]> GetPOIList(LocationCoordinate location, float radius, string queryText, LocationCategory category)
        {
            throw new NotImplementedException();
        }

        public override async Task<POI[]> GetPOIList(LocationCoordinate location, float radius, LocationCategory category)
        {
            throw new NotImplementedException();
        }

        public override async Task<POI> GetPOI(string id)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> UpdatePOI(POI poi)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> RemovePOI(string id)
        {
            throw new NotImplementedException();
        }

        public override async Task GetMap()
        {
            throw new NotImplementedException();
        }

        public override async Task SetMapSettings(float scale, float boundingBox)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }

        #region PRIVATE_METHODS
        private void GeoOnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            switch (args.Status)
            {
                case PositionStatus.Ready:
                    break;
                case PositionStatus.Initializing:
                    break;
                case PositionStatus.NoData:
                    break;
                case PositionStatus.Disabled:
                    WindowsPhoneUtils.InvokeCallback("Appverse.Geo.onAccessToLocationDenied", WindowsPhoneUtils.CALLBACKID, null);
                    break;
                case PositionStatus.NotInitialized:
                    break;
                case PositionStatus.NotAvailable:
                    WindowsPhoneUtils.InvokeCallback("Appverse.Geo.onAccessToLocationDenied", WindowsPhoneUtils.CALLBACKID, null);
                    break;
            }
        }

        private void GeoOnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                string source;
                var newGeoCoordinate = args.Position.Coordinate;
                /*switch (newGeoCoordinate.PositionSource)
                {
                    case PositionSource.Cellular:
                        break;
                    case PositionSource.Satellite:
                        break;
                    case PositionSource.WiFi:
                        break;
                    case PositionSource.IPAddress:
                        break;
                    case PositionSource.Unknown:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }*/
                _lastKnownCoordinate = newGeoCoordinate;
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
            }
        }

        private void CompassOnReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            _lastCompassReading = args.Reading;
        }
        private void AccelerometerOnReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            _lastAccelerometerReading = args.Reading;
        }
        #endregion
    }
}
