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

namespace Unity.Core.Pim
{
	public interface ICalendar
	{

		/// <summary>
		/// Lists calendar entries for given date.
		/// </summary>
		/// <param name="date">Date to match calendar entries.</param>
		/// <returns>List of calendar entries.</returns>
		CalendarEntry[] ListCalendarEntries (DateTime date);

		/// <summary>
		/// Lists calendar entries between given start and end dates.
		/// </summary>
		/// <param name="startDate">Start date to match calendar entries.</param>
		/// <param name="endDate">End date to match calendar entries.</param>
		/// <returns>List of calendar entries.</returns>
		CalendarEntry[] ListCalendarEntries (DateTime startDate, DateTime endDate);

		/// <summary>
		/// Creates a calendar entry.
		/// </summary>
		/// <param name="entry">Calendar entry to be created.</param>
		/// <returns>Created calendar entry.</returns>
		CalendarEntry CreateCalendarEntry (CalendarEntry entry);

		/// <summary>
		/// Deletes the given calendar entry.
		/// </summary>
		/// <param name="entry">>Calendar entry to be deleted.</param>
		/// <returns>True on successful deletion.</returns>
		bool DeleteCalendarEntry (CalendarEntry entry);

		/// <summary>
		/// Moves the given calendar entry to the new start and end dates.
		/// </summary>
		/// <param name="entry">Calendar entry to be moved.</param>
		/// <param name="newStartDate">New start date for calendar entry.</param>
		/// <param name="newEndDate">New end date for calendar entry.</param>
		/// <returns>True on successful relocation.</returns>
		bool MoveCalendarEntry (CalendarEntry entry, DateTime newStartDate, DateTime newEndDate);
        

	}//end ICalendar

}//end namespace Pim