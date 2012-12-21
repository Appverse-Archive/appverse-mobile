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
using System;

namespace Unity.Core.Geo
{
	public class GeoUtils
	{
		private static GeoUtils singleton = null;
		
		private GeoUtils () : base()
		{
		}
		
		public static GeoUtils GetInstance ()
		{
			if (singleton == null) {
				singleton = new GeoUtils ();
			}
			return singleton;
		}
		
		/// <summary>
		/// This method evaluates the distance using the "Havesine" method.
		/// This method remains particularly well-conditioned for numerical computation even at small distances.
		/// </summary>
		public double evaluateDistanceHaversine (double lat1, double lon1, double lat2, double lon2)
		{
			double R = 6371000.0f;
			// Average radius of the Earth in meters
			double latitude1 = convertDegreesToRadians (lat1);
			double latitude2 = convertDegreesToRadians (lat2);
			double longitude1 = convertDegreesToRadians (lon1);
			double longitude2 = convertDegreesToRadians (lon2);
			
			double dLat = (latitude2 - latitude1);
			double dLon = (longitude2 - longitude1);
			// "a" is the square of half the chord length between the points
			double a = Math.Sin (dLat / 2) * Math.Sin (dLat / 2) + Math.Cos (latitude1) * Math.Cos (latitude2) * Math.Sin (dLon / 2) * Math.Sin (dLon / 2);
			// "c" is the angular distance in radians
			double c = 2 * Math.Atan2 (Math.Sqrt (a), Math.Sqrt (1 - a));
			double d = R * c;
			return d;
		}

		/// <summary>
		/// This method evaluates the distance using the "Spherical Law of Cosines" method.
		/// When Haversine formula was published, computational precision was limited.
		/// Nowadays, most modern computers and languages, use IEEE 754 64-bit floating-point numbers,
		/// which provide 15 significant figures of precision. 
		/// With this precision, the simple spherical law of cosines formula gives well-conditioned results
		/// down to distances as small as around 1 metre.
		/// </summary>
		public double evaluateDistanceSphericalLawOfCosines (double lat1, double lon1, double lat2, double lon2)
		{
			const double R = 6371000.0f;
			// Average radius of the Earth in meters
			double latitude1 = convertDegreesToRadians (lat1);
			double latitude2 = convertDegreesToRadians (lat2);
			double longitude1 = convertDegreesToRadians (lon1);
			double longitude2 = convertDegreesToRadians (lon2);
			double d = Math.Acos (Math.Sin (latitude1) * Math.Sin (latitude2) + Math.Cos (latitude1) * Math.Cos (latitude2) * Math.Cos (longitude2 - longitude1)) * R;
			return d;
		}

		private double convertDegreesToRadians (double degrees)
		{
			double radians = (degrees * Math.PI) / 180;
			return radians;
		}
		
		
	}
}

