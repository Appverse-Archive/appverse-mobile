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
using Unity.Core.Geo;
using Unity.Core.IO.ScriptSerialization;
using System.Globalization;

namespace Unity.Core.Geo
{
	public class JSLocationCoordinateConverter : JavaScriptConverter
	{
		#region implemented abstract members of Unity.Core.IO.ScriptSerialization.JavaScriptConverter
		public override object Deserialize (IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			object objX = dictionary ["XCoordinate"];
			string sX = objX.ToString ();
			sX = sX.Replace (',', '.');
			double XCoordinate = Double.Parse (sX, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, FormatUtils.GetNumberFormatInfo ());
			
			object objY = dictionary ["YCoordinate"];
			string sY = objY.ToString ();
			sY = sY.Replace (',', '.');
			double YCoordinate = Double.Parse (sY, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, FormatUtils.GetNumberFormatInfo ());
			
			object objZ = dictionary ["ZCoordinate"];
			string sZ = objZ.ToString ();
			sZ = sZ.Replace (',', '.');
			double ZCoordinate = Double.Parse (sZ, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, FormatUtils.GetNumberFormatInfo ());
			
			object objXDop = dictionary ["XDoP"];
			string sXDop = objXDop.ToString ();
			sXDop = sXDop.Replace (',', '.');
			float XDop = Single.Parse (sXDop, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, FormatUtils.GetNumberFormatInfo ());
			
			object objYDop = dictionary ["YDoP"];
			string sYDop = objYDop.ToString ();
			sYDop = sYDop.Replace (',', '.');
			float YDop = Single.Parse (sYDop, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, FormatUtils.GetNumberFormatInfo ());
			
			LocationCoordinate coordinate = new LocationCoordinate ();
			coordinate.XCoordinate = XCoordinate;
			coordinate.YCoordinate = YCoordinate;
			coordinate.ZCoordinate = ZCoordinate;
			coordinate.XDoP = XDop;
			coordinate.YDoP = YDop;
			return coordinate;
		}
		
		public override IDictionary<string, object> Serialize (object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> d = new Dictionary<string, object> (StringComparer.Ordinal);
			
			LocationCoordinate coordinate = new LocationCoordinate ();
			if (obj.GetType () == Type.GetType ("Unity.Core.Geo.LocationCoordinate")) {
				coordinate = (LocationCoordinate)obj;
			}
			
			d.Add ("XCoordinate", coordinate.XCoordinate);
			d.Add ("YCoordinate", coordinate.YCoordinate);
			d.Add ("ZCoordinate", coordinate.ZCoordinate);
			d.Add ("XDoP", coordinate.XDoP);
			d.Add ("YDoP", coordinate.YDoP);
			
			return d;
		}
		
		public override IEnumerable<Type> SupportedTypes {
			get {
				List<Type> supportedTypes = new List<Type> ();
				
				supportedTypes.Add (Type.GetType ("Unity.Core.Geo.LocationCoordinate")); 
				
				return (supportedTypes as IEnumerable<Type>);
			}
		}
		
		#endregion
		public JSLocationCoordinateConverter ()
		{
		}
	}
	
	
}

