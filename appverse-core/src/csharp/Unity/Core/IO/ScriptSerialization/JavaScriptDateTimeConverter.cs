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
using System.Collections;
using System.Collections.Generic;

namespace Unity.Core.IO.ScriptSerialization
{
	public class JavaScriptDateTimeConverter : JavaScriptConverter
	{
		
		private static string YEAR_KEY = "Year";
		private static string MONTH_KEY = "Month";
		private static string DAY_KEY = "Day";
		private static string HOUR_KEY = "Hour";
		private static string MINUTE_KEY = "Minute";
		private static string SECOND_KEY = "Second";
		private static string MSECOND_KEY = "Millisecond";
		
		public JavaScriptDateTimeConverter ()
		{
		}
		
		#region implemented abstract members of Unity.Core.IO.ScriptSerialization.JavaScriptConverter
		public override IEnumerable<Type> SupportedTypes {
			get {
				List<Type> supportedTypes = new List<Type> ();
				
				supportedTypes.Add (Type.GetType ("System.DateTime")); 
				
				return (supportedTypes as IEnumerable<Type>);
			}
		}
		
		public override object Deserialize (IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			try {
				object objYear = dictionary [YEAR_KEY];
				string sYear = objYear.ToString ();
				int year = Int32.Parse (sYear);
		
				object objMonth = dictionary [MONTH_KEY];
				string sMonth = objMonth.ToString ();
				int month = Int32.Parse (sMonth);
		
				object objDay = dictionary [DAY_KEY];
				string sday = objDay.ToString ();
				int day = Int32.Parse (sday);
				
				int hour = 0;
				int minute = 0;
				int second = 0;
				int millisecond = 0;
				bool timeFilled = false;
				
				try { 
					if (dictionary.ContainsKey (HOUR_KEY)) {
						object objHour = dictionary [HOUR_KEY];
						string sHour = objHour.ToString ();
						hour = Int32.Parse (sHour);
						timeFilled = true;
					}
					
					if (dictionary.ContainsKey (MINUTE_KEY)) {
						object objMinute = dictionary [MINUTE_KEY];
						string sMinute = objMinute.ToString ();
						minute = Int32.Parse (sMinute);
					}
					
					if (dictionary.ContainsKey (SECOND_KEY)) {
						object objSecond = dictionary [SECOND_KEY];
						string sSecond = objSecond.ToString ();
						second = Int32.Parse (sSecond);
					}
					
					if (dictionary.ContainsKey (MSECOND_KEY)) {
						object objMillisecond = dictionary [MSECOND_KEY];
						string sMillisecond = objMillisecond.ToString ();
						millisecond = Int32.Parse (sMillisecond);
					}
				} catch (Exception) {
					// exception not handle
				}

				if(timeFilled) {
					return new DateTime (year, month, day, hour, minute, second, millisecond);
				} else {
					return new DateTime (year, month, day);
				}
				
			} catch (Exception) {
				// on exception, return default datetime
				return new DateTime ();
			} 
			
		}
		
		public override IDictionary<string, object> Serialize (object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> d = new Dictionary<string, object> (StringComparer.Ordinal);
			
			DateTime dateTime = new DateTime (); // default datetime
			if (obj.GetType () == Type.GetType ("System.DateTime")) {
				dateTime = (DateTime)obj;
			}
			
			// date
			d.Add (YEAR_KEY, dateTime.Year);
			d.Add (MONTH_KEY, dateTime.Month);
			d.Add (DAY_KEY, dateTime.Day);
			
			// time
			d.Add (HOUR_KEY, dateTime.Hour);
			d.Add (MINUTE_KEY, dateTime.Minute);
			d.Add (SECOND_KEY, dateTime.Second);
			d.Add (MSECOND_KEY, dateTime.Millisecond);
			
			return d;
		}
		
		#endregion
		
	}
}

