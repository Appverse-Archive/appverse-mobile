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

import java.util.Calendar;
import java.util.Map;
import java.util.Random;
import java.util.Set;

import com.gft.unity.android.util.json.JSONObject;
import com.gft.unity.android.util.json.JSONSerializer;
import com.gft.unity.core.notification.DateTime;
import com.gft.unity.core.notification.NotificationData;
import com.gft.unity.core.notification.RepeatInterval;
import com.gft.unity.core.notification.SchedulingData;

import android.app.AlarmManager;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.util.Log;

public class NotificationUtils {

	public static final String APPLICATION_NAME = "$REPLACE_APPNAME_TOKEN$";
	public static final String PACKAGE_NAME = "$REPLACE_PACKAGE_TOKEN$";
	private static final String MAIN_ACTIVITY_CLASS_NAME = "$REPLACE_MAINACTIVITY_NAME_TOKEN$";
	private static final String NOTIFICATIONS_SHARED_PREFERENCES = "NOTIFICATIONS_SHARED_PREFERENCES";
	
	public static final String DRAWABLE_TYPE = "drawable";
	public static final String DEFAULT_ICON_NAME = "icon";
	
	private static final String TAG = "Appverse.NotificationUtils";
	
	private static final String NOTIFICATION_DATA_TOKEN = "@NOTIFICATION_DATA_TOKEN@";
	public static final String NOTIFICATION_TYPE_LOCAL = "LOCAL";
	public static final String NOTIFICATION_TYPE_REMOTE = "REMOTE";
	
	public static final String EXTRA_NOTIFICATION_ID = "EXTRA_NOTIFICATION_ID";
	public static final String EXTRA_MESSAGE = "EXTRA_MESSAGE";
	public static final String EXTRA_SOUND = "EXTRA_SOUND";
	public static final String EXTRA_CUSTOM_JSON = "EXTRA_CUSTOM_JSON";
	public static final String EXTRA_TYPE = "EXTRA_TYPE";
	
	public static int getNotificationId() {
		Random rd = new Random();
		return rd.nextInt(10000)+1;
	}
	
	public static PendingIntent getMainActivityAsPendingIntent(Context context, String notificationType, 
			String mainActivityClassName, String notificationID, NotificationData notificationData) {
		
		Intent notificationIntent = new Intent("android.intent.action.MAIN");
		
		// store notification details inside the intent, so they could get returned when activity starts
		if(notificationData != null) {
			notificationIntent.putExtra(EXTRA_MESSAGE, notificationData.getAlertMessage());
			notificationIntent.putExtra(EXTRA_SOUND, notificationData.getSound());
			notificationIntent.putExtra(EXTRA_CUSTOM_JSON, notificationData.getCustomDataJsonString());
			notificationIntent.putExtra(EXTRA_TYPE, notificationType);
		}
		if(notificationID != null) {
			notificationIntent.putExtra(EXTRA_NOTIFICATION_ID, notificationID);
		}
		
		notificationIntent.setAction(notificationID);
		notificationIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK|Intent.FLAG_ACTIVITY_SINGLE_TOP|Intent.FLAG_ACTIVITY_CLEAR_TOP);
		notificationIntent.setComponent(ComponentName.unflattenFromString(mainActivityClassName));
		notificationIntent.addCategory("android.intent.category.LAUNCHER");
		return PendingIntent.getActivity(context, 0, notificationIntent, 0);
	}
	public static PendingIntent getMainActivityAsPendingIntent(Context context, String notificationType, String notificationID, NotificationData notificationData) {
		return getMainActivityAsPendingIntent(context, notificationType, MAIN_ACTIVITY_CLASS_NAME, notificationID, notificationData);
	}
	
	public static AlarmManager getAlarmManager(Context context) {
		return (AlarmManager) context.getSystemService(Context.ALARM_SERVICE);
	}
	
	public static NotificationManager getNotificationManager(Context context) {
		return (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
	}
	
	/**
	 * Process a Local Notification by adding it to the android native AlarmManager.
	 * @param context Current context.
	 * @param notificationData The notification data details
	 * @param schedule 
	 * @return int notification identifier
	 */
	public static int processLocalNotification(Context context, NotificationData notificationData, SchedulingData schedule) {
		
		if (notificationData != null) {
			// 
			// type == RTC_WAKEUP : Alarm time in System.currentTimeMillis() (wall clock time in UTC), which will wake up the device when it goes off.
			// 
			int type = AlarmManager.RTC_WAKEUP;
			
			final Calendar calFireDate = Calendar.getInstance();
					
			// triggerAtTime == time in milliseconds that the alarm should first go off, using the appropriate clock (depending on the alarm type).
			if(schedule != null && schedule.getFireDate()!=null) {
				calFireDate.set(schedule.getFireDate().getYear(), schedule.getFireDate().getMonth() - 1,
						schedule.getFireDate().getDay(), schedule.getFireDate().getHour(),
						schedule.getFireDate().getMinute(), schedule.getFireDate().getSecond());
			}
			final long triggerAtTime = calFireDate.getTimeInMillis();
			Log.d(TAG, "Processing local notification for fire date: " + calFireDate.toString());
			
			// The intent (broadcast receiver) that will handle the local notification when received.
			final Intent intent = new Intent(context, LocalNotificationReceiver.class);
			
			//remove if not needed   //final int hour = calFireDate.get(Calendar.HOUR_OF_DAY);
			//remove if not needed   //final int min = calFireDate.get(Calendar.MINUTE);
	
			int notificationId = NotificationUtils.getNotificationId();
			intent.setAction("" + notificationId);
			intent.putExtra(LocalNotificationReceiver.TITLE, NotificationUtils.APPLICATION_NAME);
			intent.putExtra(LocalNotificationReceiver.BODY, notificationData.getAlertMessage());
			intent.putExtra(LocalNotificationReceiver.SOUND, notificationData.getSound());
			intent.putExtra(LocalNotificationReceiver.CUSTOM_JSON_DATA, notificationData.getCustomDataJsonString());
			intent.putExtra(LocalNotificationReceiver.TICKER, "");  // TODO add a ticker
			intent.putExtra(LocalNotificationReceiver.NOTIFICATION_ID, "" + notificationId);
			//remove if not needed   //intent.putExtra(LocalNotificationReceiver.HOUR_OF_DAY, hour);
			//remove if not needed   //intent.putExtra(LocalNotificationReceiver.MINUTE, min);
			
			// operation == action to perform when the alarm goes off; 
			final PendingIntent operation = PendingIntent.getBroadcast(context, 0, intent, PendingIntent.FLAG_CANCEL_CURRENT);
			
			/* Get the AlarmManager service */
			final AlarmManager am = NotificationUtils.getAlarmManager(context);
			
			// interval ==  interval in milliseconds between subsequent repeats of the alarm.
			// in this case (we are presenting the local notification NOW), no repeats intervals
			long interval = 0;
			if(schedule != null && schedule.getRepeatInterval()!=RepeatInterval.NO_REPEAT) {
				if (schedule.getRepeatInterval() == RepeatInterval.HOURLY) {
					interval = AlarmManager.INTERVAL_HOUR;
				} else if (schedule.getRepeatInterval() == RepeatInterval.DAILY) {
					interval = AlarmManager.INTERVAL_DAY;
				} else if (schedule.getRepeatInterval() == RepeatInterval.WEEKLY) {
					interval = AlarmManager.INTERVAL_DAY * 7;  // 7 days == week
				} else if (schedule.getRepeatInterval() == RepeatInterval.MONTHLY) {
					interval = AlarmManager.INTERVAL_DAY * 30;  // 30 days == month
				} else if (schedule.getRepeatInterval() == RepeatInterval.YEARLY) {
					interval = AlarmManager.INTERVAL_DAY * 365;  // 365 days == year
				}
				am.setRepeating(type, triggerAtTime, interval, operation);
			} else {
				am.set(type, triggerAtTime, operation);
			}
			return notificationId;
		} else {
			Log.d(TAG, "Error processing local notification. No notification data object received");
			return -1;
		}
		
	}
	
	/**
	 * 
	 * @param context
	 * @param fireDate
	 * @return
	 */
	public static String getLocalNotificationId(Context context, DateTime fireDate) {
		
		if(fireDate!=null) {
			
			Map<String, ?> allLocalNotifications = NotificationUtils.getLocalNotificationsOnSharedPreferences(context);
			final Set<String> allLocalNotificationsIds = allLocalNotifications.keySet();
			
			for (String notificationId: allLocalNotificationsIds) {
			    try {
			    	String notificationDataSerialized = (String) allLocalNotifications.get(notificationId);
					final SchedulingData schedule = NotificationUtils.deserializeSchedulingData(notificationDataSerialized);
					if(schedule!=null && schedule.getFireDate().toString().equals(fireDate.toString())) {
						return notificationId;
					}

			    } catch (Exception e) {
			    	Log.d(TAG, "Error while getting local notifications from given fire date [" + fireDate +"]: " + e.toString());
			    	System.out.println(e.getStackTrace());
			    }
			}
		}
			
		return null;
	}
	

	/**
	 * Returns all local notifications stored on Shared Preferences.
	 * @param context The current application context.
	 * @return A hash map with all the local notifications stored on the Shared Preferences.
	 */
	public static Map<String, ?> getLocalNotificationsOnSharedPreferences(Context context) {
		
		final SharedPreferences preferences = context.getSharedPreferences(NOTIFICATIONS_SHARED_PREFERENCES, Context.MODE_PRIVATE);
		
		return preferences.getAll();
	}
	
	public static String serializeNotificationData (NotificationData notificationData, SchedulingData schedule) {
		StringBuilder builder = new StringBuilder();
		builder.append(NOTIFICATION_DATA_TOKEN);
		
		try {
			if(notificationData!=null)
				builder.append(JSONSerializer.serialize(notificationData));
			
			builder.append(NOTIFICATION_DATA_TOKEN);
			
			if(schedule!=null)
				builder.append(JSONSerializer.serialize(schedule));
		} catch (Exception e) {
			Log.d(TAG, "Exception while serializing notification data."+ e.getMessage());
		}
		
		builder.append(NOTIFICATION_DATA_TOKEN);
		
		return builder.toString();
	}
	
	public static NotificationData deserializeNotificationData(String serialized) {
		if(serialized != null) {
			try {
				String delims = NOTIFICATION_DATA_TOKEN+"+";
				String[] tokens = serialized.split(delims);
				if(tokens!=null && tokens.length>0)
					return (NotificationData) JSONSerializer.deserialize(NotificationData.class, new JSONObject(tokens[1]));
			} catch (Exception e) {
				Log.d(TAG, "Exception while deserializing notification data."+ e.getMessage());
			}
		}
		
		return null;
	}
	
	public static SchedulingData deserializeSchedulingData(String serialized) {
		if(serialized != null) {
			try {
				String delims = NOTIFICATION_DATA_TOKEN+"+";
				String[] tokens = serialized.split(delims);
				if(tokens!=null && tokens.length>1) {
					return (SchedulingData) JSONSerializer.deserialize(SchedulingData.class, new JSONObject(tokens[2]));
				}
			} catch (Exception e) {
				Log.d(TAG, "Exception while deserializing notification data."+ e.getMessage());
			}
		}
		
		return null;
	}
	
	/**
	 * Stores the local notification information to the Android Shared Preferences.
     * This is needed to restore back all scheduled notifications upon device reboot.
	 * @param context The current application context.
	 * @param notificationId
	 * @param notificationData
	 * @param schedule
	 * @return
	 */
    public static boolean storeLocalNotificationOnSharedPreferences(Context context, String notificationId, 
    		NotificationData notificationData, SchedulingData schedule) {
	
    	final Editor settingsEditor = context.getSharedPreferences(NOTIFICATIONS_SHARED_PREFERENCES, Context.MODE_PRIVATE).edit();

    	String notificationDataSerialized = serializeNotificationData(notificationData, schedule);
    	
    	settingsEditor.putString(notificationId, notificationDataSerialized);

    	return settingsEditor.commit();
    }
    
    /**
     * Remove a specific local notification from the Android shared Preferences
     * 
     * @param context The current application context.
     * @param notificationId The Id of the notification that must be removed.
     * @return true On success, otherwise false
     */
    public static boolean removeLocalNotificationFromSharedPrefereces(Context context, String notificationId) {
    	final Editor settingsEditor = context.getSharedPreferences(NOTIFICATIONS_SHARED_PREFERENCES, Context.MODE_PRIVATE).edit();

    	settingsEditor.remove(notificationId);

    	return settingsEditor.commit();
    }

    /**
     * Clears all local notifications from the Android shared Preferences
     * @param context The current application context.
     * @return true On success, otherwise false
     */
    public static boolean removeAllLocalNotificationsFromSharedPrefereces(Context context) {
    	final Editor settingsEditor = context.getSharedPreferences(NOTIFICATIONS_SHARED_PREFERENCES, Context.MODE_PRIVATE).edit();

    	settingsEditor.clear();

    	return settingsEditor.commit();
    }
	
}
