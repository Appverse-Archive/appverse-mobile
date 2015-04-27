/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
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
using Foundation;

namespace Appverse.Core.iBeacon
{
	public class Beacon
	{

		public static Double DISTANCE_THRESHOLD_WTF = 0.0;
		public static Double DISTANCE_THRESHOLD_IMMEDIATE = 0.5;
		public static Double DISTANCE_THRESHOLD_NEAR = 3.0;
		public String Address { get; set; }
		public String Name { get; set; }
		public String UUID { get; set; }
		public DistanceType Distance;
		public double Meters { get; set; }
		public int Major { get; set; }
		public int Minor { get; set; }
		public long Timestamp { get; set; }

		public Beacon() {
		}

		public void setDistance(DistanceType b)
		{
			Distance = b;

		}

		public DistanceType getDistance() {
			if (Meters < DISTANCE_THRESHOLD_WTF) {
				return DistanceType.UNKNOWN;
			}
			if (Meters < DISTANCE_THRESHOLD_IMMEDIATE) {
				return DistanceType.INMEDIATE;
			}
			if (Meters < DISTANCE_THRESHOLD_NEAR) {
				return DistanceType.NEAR;
			}
			return DistanceType.FAR;
		}

		public override string ToString() {

			return "Beacon [address="+
				Address + " " + ", name=" + " " +Name + " " +
				", uuid=" + " " +UUID + " " +", distance=" + " " +
				Distance + " " +",meters=" + " " +Meters + " " +
				", major=" + " " +Major + " " +", minor=" + " " +
				Minor + " " +", timestamp=" + " " +Timestamp+"]";
		}


	}
}

