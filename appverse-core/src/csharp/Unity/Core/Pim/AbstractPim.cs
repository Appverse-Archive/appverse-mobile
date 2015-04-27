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
#if WP8
using System.Threading.Tasks;
#endif


namespace Unity.Core.Pim
{
    public abstract class AbstractPim : ICalendar, IContacts
    {


        #region Miembros de ICalendar
#if !WP8
		public abstract void ListCalendarEntries (DateTime date);

		public abstract void ListCalendarEntries (DateTime startDate, DateTime endDate);

		public abstract CalendarEntry CreateCalendarEntry (CalendarEntry entry);

		public abstract bool DeleteCalendarEntry (CalendarEntry entry);

		public abstract bool MoveCalendarEntry (CalendarEntry entry, DateTime newStartDate, DateTime newEndDate);
#else
        public abstract Task ListCalendarEntries(DateTime date);
        public abstract Task ListCalendarEntries(DateTime startDate, DateTime endDate);
        public abstract Task<CalendarEntry> CreateCalendarEntry(CalendarEntry entry);
        public abstract Task<bool> DeleteCalendarEntry(CalendarEntry entry);
        public abstract Task<bool> MoveCalendarEntry(CalendarEntry entry, DateTime newStartDate, DateTime newEndDate);
#endif

        #endregion

        #region Miembros de IContacts
#if !WP8
		public void ListContacts ()
		{
			ListContacts (null);
		}

		public abstract void GetContact(String id);

		public abstract void ListContacts (ContactQuery query);
		
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
#else
        public abstract Task GetContact(string id);
        public abstract Task ListContacts();
        public abstract Task ListContacts(ContactQuery query);
        public abstract Task<Contact> CreateContact(Contact contactData);
        public abstract Task<bool> UpdateContact(string ID, Contact newContactData);
        public abstract Task<bool> DeleteContact(Contact contact);
#endif
        #endregion


    }
}
