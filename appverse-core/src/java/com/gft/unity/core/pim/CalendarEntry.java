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

import java.util.Arrays;

public class CalendarEntry {

    private CalendarAlarm[] alarms;
    private CalendarAttendee[] attendees;
    private DateTime endDate;
    private boolean IsAllDayEvent;
    private boolean IsEditable;
    private boolean IsRecurrentEvent;
    private String location;
    private String notes;
    private CalendarRecurrence recurrence;
    private long recurrenceNumber;
    private DateTime startDate;
    private String title;
    private CalendarType type;
    private String uid;

    public CalendarEntry() {
    }

    public boolean getIsAllDayEvent() {
        return IsAllDayEvent;
    }

    public void setIsAllDayEvent(boolean IsAllDayEvent) {
        this.IsAllDayEvent = IsAllDayEvent;
    }

    public boolean getIsEditable() {
        return IsEditable;
    }

    public void setIsEditable(boolean IsEditable) {
        this.IsEditable = IsEditable;
    }

    public boolean getIsRecurrentEvent() {
        return IsRecurrentEvent;
    }

    public void setIsRecurrentEvent(boolean IsRecurrentEvent) {
        this.IsRecurrentEvent = IsRecurrentEvent;
    }

    public CalendarAlarm[] getAlarms() {
        return alarms;
    }

    public void setAlarms(CalendarAlarm[] alarms) {
        this.alarms = alarms;
    }

    public CalendarAttendee[] getAttendees() {
        return attendees;
    }

    public void setAttendees(CalendarAttendee[] attendees) {
        this.attendees = attendees;
    }

    public DateTime getEndDate() {
        return endDate;
    }

    public void setEndDate(DateTime endDate) {
        this.endDate = endDate;
    }

    public String getLocation() {
        return location;
    }

    public void setLocation(String location) {
        this.location = location;
    }

    public String getNotes() {
        return notes;
    }

    public void setNotes(String notes) {
        this.notes = notes;
    }

    public CalendarRecurrence getRecurrence() {
        return recurrence;
    }

    public void setRecurrence(CalendarRecurrence recurrence) {
        this.recurrence = recurrence;
    }

    public long getRecurrenceNumber() {
        return recurrenceNumber;
    }

    public void setRecurrenceNumber(long recurrenceNumber) {
        this.recurrenceNumber = recurrenceNumber;
    }

    public DateTime getStartDate() {
        return startDate;
    }

    public void setStartDate(DateTime startDate) {
        this.startDate = startDate;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public CalendarType getType() {
        return type;
    }

    public void setType(CalendarType type) {
        this.type = type;
    }

    public String getUid() {
        return uid;
    }

    public void setUid(String uid) {
        this.uid = uid;
    }

    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("CalendarEntry [alarms=");
        builder.append(Arrays.toString(alarms));
        builder.append(", attendees=");
        builder.append(Arrays.toString(attendees));
        builder.append(", endDate=");
        builder.append(endDate);
        builder.append(", IsAllDayEvent=");
        builder.append(IsAllDayEvent);
        builder.append(", IsEditable=");
        builder.append(IsEditable);
        builder.append(", IsRecurrentEvent=");
        builder.append(IsRecurrentEvent);
        builder.append(", location=");
        builder.append(location);
        builder.append(", notes=");
        builder.append(notes);
        builder.append(", recurrence=");
        builder.append(recurrence);
        builder.append(", recurrenceNumber=");
        builder.append(recurrenceNumber);
        builder.append(", startDate=");
        builder.append(startDate);
        builder.append(", title=");
        builder.append(title);
        builder.append(", type=");
        builder.append(type);
        builder.append(", uid=");
        builder.append(uid);
        builder.append("]");
        return builder.toString();
    }
}
