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

import com.gft.unity.android.util.json.JSONSerializer;
import com.gft.unity.core.notification.NotificationData;

import android.app.Activity;
import android.app.Notification;
import android.app.NotificationManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
//import android.graphics.Bitmap;
//import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;
import android.webkit.WebView;

/**
 * @author maps
 *
 */
public class LocalNotificationReceiver extends BroadcastReceiver {
	
	public static final String TITLE = "TITLE";
	public static final String BODY = "BODY";
	public static final String TICKER = "TICKER";
	public static final String NOTIFICATION_ID = "NOTIFICATION_ID";
	public static final String SOUND = "SOUND";
	public static final String CUSTOM_JSON_DATA = "CUSTOM_JSON_DATA";
	//TODO public static final String LARGE_ICON = "LARGE_ICON";
	
	//remove if not needed   //public static final String HOUR_OF_DAY = "HOUR_OF_DAY"; /* 24hour format: HH */
	//remove if not needed   //public static final String MINUTE = "MINUTES";
	
	// Keeping main activity state
	private static WebView appView;
	private static Activity appActivity;
	
	private static final String TAG = "Appverse.LocalNotificationReceiver";
	
	
	public static void initialize(WebView view, Activity context){
		appView = view;
		appActivity = context;
	}

	/**
	 * @see android.content.BroadcastReceiver#onReceive(android.content.Context, android.content.Intent)
	 */
	@Override
	public void onReceive(Context context, Intent intent) {
		
		Log.d(TAG, "******* Local Notification Received " + intent.getAction());
		
		// Getting notification manager
		final NotificationManager notificationMgr = NotificationUtils.getNotificationManager(context);
		
		// Getting notification details from the Intent EXTRAS 
		final Bundle bundle = intent.getExtras();
		final String ticker = bundle.getString(TICKER);
		final String title = bundle.getString(TITLE);
		final String body = bundle.getString(BODY);
		final String notificationSound = bundle.getString(SOUND);
		final String customJsonData = bundle.getString(CUSTOM_JSON_DATA);
		int notificationId = 0;

		try {
			notificationId = Integer.parseInt(bundle.getString(NOTIFICATION_ID));
		} catch (Exception e) {
			Log.d(TAG, "Unable to process local notification with id: " + bundle.getString(NOTIFICATION_ID));
		}
		
		int iIconId = context.getResources().getIdentifier(
				NotificationUtils.DEFAULT_ICON_NAME, NotificationUtils.DRAWABLE_TYPE, NotificationUtils.PACKAGE_NAME);
		
		// Creates the notification to display
		Notification notif = null;
		NotificationData notificationData = new NotificationData();
		notificationData.setAlertMessage(body);
		notificationData.setSound(notificationSound);
		notificationData.setCustomDataJsonString(customJsonData);
		
		//Different ways to create the notification depending the API Level restrictions
		if(Build.VERSION.SDK_INT < 11){
			notif = new Notification(iIconId, ticker, System.currentTimeMillis());//icon, text, time in millis
			notif.defaults=0;
			if(notificationSound!=null && !notificationSound.isEmpty() && !notificationSound.equals("default")){ 
				notif.sound = Uri.parse(notificationSound);
			} else {
				notif.defaults |= Notification.DEFAULT_SOUND;
			}
			notif.setLatestEventInfo(context, title, body, 
					NotificationUtils.getMainActivityAsPendingIntent(context, NotificationUtils.NOTIFICATION_TYPE_LOCAL, ""+notificationId, notificationData));
			// TODO notif.ledARGB = Integer.parseInt(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_LED_COLOR_ARGB.toString()));
			notif.ledOffMS = 100;
			notif.ledOnMS = 100;
		}else{
			Notification.Builder mBuilder = new Notification.Builder(context)
			.setDefaults(0)
			.setContentIntent(NotificationUtils.getMainActivityAsPendingIntent(context, NotificationUtils.NOTIFICATION_TYPE_LOCAL, ""+notificationId, notificationData))
			.setSmallIcon(iIconId)
			//TODO .setLights(Integer.parseInt(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_LED_COLOR_ARGB.toString())), 100, 100)
			.setTicker(ticker)
			.setContentText(body)
			.setContentTitle(title);
			
			/** TODO 
			Bitmap largeIconBMP = null;
			
			if(NOTIFICATION_STRUCTURE.containsKey(RemoteNotificationFields.RN_LARGE_ICON.toString())){
				int iLargeIconId = APP_RESOURCES.getIdentifier(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_LARGE_ICON.toString()), NotificationUtils.DRAWABLE_TYPE, PACKAGE_NAME);
				largeIconBMP = BitmapFactory.decodeResource(APP_RESOURCES, iLargeIconId);
			}
			if(largeIconBMP!=null){
				mBuilder.setLargeIcon(largeIconBMP);
			}
			**/
			
			if(notificationSound!=null && !notificationSound.isEmpty() && !notificationSound.equals("default")){ 
				mBuilder.setSound(Uri.parse(notificationSound));
			} else {
				// set default sound
				mBuilder.setDefaults(Notification.DEFAULT_SOUND);
			}
			notif = mBuilder.getNotification();
		}
				
		//check if vibration should be enabled 
		//notification.vibrate = new long[] { 0, 100, 200, 300 };

		/*
		 * In order to stack all reminders in the notification bar, a random ID should be generated. 
		 * To replace an existing notification, ID should match in the notification intent.
		 */
		notificationMgr.notify(notificationId, notif);
		
		// remove notification from stored shared preferences, as it has been already processed by the notification manager
		NotificationUtils.removeLocalNotificationFromSharedPrefereces(context, ""+notificationId);
		
		final NotificationData notifData = notificationData;
		
		// TESTING MARGA
		if(appActivity != null && appView!=null){
			Runnable rNotification = new Runnable() {
				@Override
				public void run() {
					appView.loadUrl("javascript:try{Unity.OnLocalNotificationReceived(" + JSONSerializer.serialize(notifData) +")}catch(e){}");
				}
			};
			Log.d(TAG, "Invoking rNotification on UI thread... ");
			appActivity.runOnUiThread(rNotification);
		}
		
	}

}
