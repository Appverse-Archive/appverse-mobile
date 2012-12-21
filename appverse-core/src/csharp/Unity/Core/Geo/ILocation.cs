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
namespace Unity.Core.Geo
{
	public interface ILocation
	{

		/// <summary>
		/// Gets the current device acceleration (measured in meters/second/second).
		/// </summary>
		/// <returns>Current acceleration.</returns>
		Acceleration GetAcceleration ();

		/// <summary>
		/// The heading relative to the magnetic noth pole (default).
		/// Measured in degrees, minutes and seconds.
		/// </summary>
		/// <returns>Current heading.</returns>
		float GetHeading ();

		/// <summary>
		/// The heading relative to the given north (magnetic or true north pole).
		/// Measured in degrees, minutes and seconds.
		/// </summary>
		/// <param name="type">Type of north to measured orientation relative to it.</param>
		/// <returns>Current heading.</returns>
		float GetHeading (NorthType type);

		/// <summary>
		/// The orientation relative to the magnetic noth pole (default).
		/// Measured in degrees, minutes and seconds.
		/// </summary>
		/// <returns>Current orientation.</returns>
		DeviceOrientation GetDeviceOrientation ();

		/// <summary>
		/// Gets the current device location coordinates.
		/// </summary>
		/// <returns>Current location.</returns>
		LocationCoordinate GetCoordinates ();

		/// <summary>
		/// Gets the current device velocity (in meters/second).
		/// </summary>
		/// <returns>Device speed.</returns>
		float GetVelocity ();

		/// <summary>
		/// Starts the location services of the mobile device (lonfitude, latitude, altitude, speed, etc.).
		/// </summary>
		/// <returns>True if the location services could be started</returns>
		bool StartUpdatingLocation ();

		/// <summary>
		/// Stops the location services of the mobile device (lonfitude, latitude, altitude, speed, etc.).
		/// </summary>
		/// <returns>True if the location services could be stopped</returns>
		bool StopUpdatingLocation ();

		/// <summary>
		/// Starts the heading services of the mobile device (heading, accuracy, ...).
		/// </summary>
		/// <returns>True if the location services could be started</returns>
		bool StartUpdatingHeading ();

		/// <summary>
		/// Stops the heading services of the mobile device (heading, accurary, ...).
		/// </summary>
		/// <returns>True if the location services could be stopped</returns>
		bool StopUpdatingHeading ();
		
		/// <summary>
		/// Gets GeoDecoder values like street, country, postal code, ... from the current longitude and latitude
		/// </summary>
		/// <returns>Reverse geocoder attributes</returns>
		GeoDecoderAttributes GetGeoDecoder ();
		
		/// <summary>
		/// The proximity sensor detects an object or person which is closed to the device
		/// and then the display screen is disabled.
		/// </summary>
		/// <returns>True if the proximity sensor has been enabled properly</returns>		
		bool StartProximitySensor ();
		
		/// <summary>
		/// The proximity sensor detects an object or person which is closed to the device
		/// and then the display screen is disabled. Stopping the proximity sensor, the
		/// screen is not disabled when an object is closed to the proximity sensor.
		/// </summary>
		/// <returns>True if the proximity sensor has been disabled properly</returns>				
		bool StopProximitySensor ();

		/// <summary>
		/// Determines whether the Location Services (GPS) is enabled.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the GPS service is enabled; otherwise, <c>false</c>.
		/// </returns>
		bool IsGPSEnabled ();

	}//end ILocation

}//end namespace Geo