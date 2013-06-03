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
package com.gft.unity.android.notification;

import java.util.Map;
import java.util.Set;

import com.gft.unity.core.notification.NotificationData;
import com.gft.unity.core.notification.SchedulingData;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

/**
 * Local Notification Receiver trigger on Android's BOOT_COMPLETED broadcast event
 * @author maps
 *
 */
public class LocalNotificationOnBootReceiver extends BroadcastReceiver {

	private static final String TAG = "Appverse.LocalNotificationOnBootReceiver";
	
	@Override
	public void onReceive(Context context, Intent intent) {
		
		Log.d(TAG, "Boot completed... checking local notifications to restore");
		
		// Obtain all local notifications stored on Shared Preferences
		Map<String, ?> allLocalNotifications = NotificationUtils.getLocalNotificationsOnSharedPreferences(context);
		final Set<String> allLocalNotificationsIds = allLocalNotifications.keySet();

		/*
		 * Parse each local notification details and add again to the android AlarmManager
		 */
		for (String notificationId: allLocalNotificationsIds) {
		    try {
		    	String notificationDataSerialized = (String) allLocalNotifications.get(notificationId);
				final NotificationData notificationData = NotificationUtils.deserializeNotificationData(notificationDataSerialized);
				final SchedulingData schedule = NotificationUtils.deserializeSchedulingData(notificationDataSerialized);
				
				NotificationUtils.processLocalNotification(context, notificationData, schedule);

		    } catch (Exception e) {
		    	Log.d(TAG, "Error while restoring local notifications after reboot: " + e.toString());
		    }

		    Log.d(TAG, "Successfully restored local notifications upon reboot");
		}
		
	}
	

}
