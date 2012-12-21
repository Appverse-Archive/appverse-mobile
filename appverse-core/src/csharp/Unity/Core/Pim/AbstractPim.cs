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
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Unity.Core.Pim
{
	public abstract class AbstractPim : ICalendar, IContacts
	{
		public static string QUERY_PARAM_GROUP = "group";
		public static string QUERY_PARAM_NAME = "name";
		
        #region Miembros de ICalendar

		public abstract CalendarEntry[] ListCalendarEntries (DateTime date);

		public abstract CalendarEntry[] ListCalendarEntries (DateTime startDate, DateTime endDate);

		public abstract CalendarEntry CreateCalendarEntry (CalendarEntry entry);

		public abstract bool DeleteCalendarEntry (CalendarEntry entry);

		public abstract bool MoveCalendarEntry (CalendarEntry entry, DateTime newStartDate, DateTime newEndDate);

        #endregion

        #region Miembros de IContacts

		public Contact[] ListContacts ()
		{
			return ListContacts (null);
		}

		public abstract Contact[] ListContacts (string queryText);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="qstring">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Hashtable"/>
		/// </returns>
		protected Hashtable ParseQueryString (string qstring)
		{
			Hashtable outc = new Hashtable ();
			
			if (qstring != null) {
				qstring = qstring + "&";
				Regex r = new Regex (@"(?<name>[^=&]+)=(?<value>[^&]+)&", RegexOptions.IgnoreCase);
		
				IEnumerator _enum = r.Matches (qstring).GetEnumerator ();
				while (_enum.MoveNext() && _enum.Current != null) {
					outc.Add (((Match)_enum.Current).Result ("${name}"),
		                        ((Match)_enum.Current).Result ("${value}"));
				}
			}
	
			return outc;
		}

		public abstract Contact CreateContact (Contact contactData);

		public abstract bool UpdateContact (string ID, Contact newContactData);

		public abstract bool DeleteContact (Contact contact);

        #endregion
	}
}
