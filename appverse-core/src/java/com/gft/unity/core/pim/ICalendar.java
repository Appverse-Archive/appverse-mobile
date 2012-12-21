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
package com.gft.unity.core.pim;

public interface ICalendar {

    /**
     * Creates a calendar entry.
     *
     * @param entry Calendar entry to be created.
     * @return Created calendar entry.
     */
    public CalendarEntry CreateCalendarEntry(CalendarEntry entry);

    /**
     * Deletes the given calendar entry.
     *
     * @param entry Calendar entry to be deleted.
     * @return <CODE>true</CODE> on successful deletion, <CODE>false</CODE>
     * otherwise.
     */
    public boolean DeleteCalendarEntry(CalendarEntry entry);

    /**
     * Lists calendar entries for given date.
     *
     * @param date Date to match calendar entries.
     * @return List of calendar entries.
     */
    public CalendarEntry[] ListCalendarEntries(DateTime date);

    /**
     * Lists calendar entries between given start and end dates.
     *
     * @param startDate Start date to match calendar entries.
     * @param endDate End date to match calendar entries.
     * @return List of calendar entries.
     */
    public CalendarEntry[] ListCalendarEntries(DateTime startDate,
            DateTime endDate);

    /**
     * Moves the given calendar entry to the new start and end dates.
     *
     * @param entry Calendar entry to be moved.
     * @param newStartDate New start date for calendar entry.
     * @param newEndDate New end date for calendar entry.
     * @return <CODE>true</CODE> on successful relocation, <CODE>false</CODE>
     * otherwise.
     */
    public boolean MoveCalendarEntry(CalendarEntry entry,
            DateTime newStartDate, DateTime newEndDate);
}
