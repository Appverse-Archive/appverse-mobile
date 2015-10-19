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
package com.gft.unity.android;

import java.util.Map;
import java.util.Set;

import android.app.Activity;
import android.app.AlarmManager;
import android.app.AlertDialog;
import android.app.Dialog;
import android.app.PendingIntent;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Vibrator;

import com.gft.unity.android.activity.AndroidActivityManager;
import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.android.notification.LocalNotificationReceiver;
import com.gft.unity.android.notification.NotificationUtils;
import com.gft.unity.core.json.JSONSerializer;
import com.gft.unity.core.notification.AbstractNotification;
import com.gft.unity.core.notification.DateTime;
import com.gft.unity.core.notification.NotificationData;
import com.gft.unity.core.notification.RegistrationError;
import com.gft.unity.core.notification.RegistrationToken;
import com.gft.unity.core.notification.RemoteNotificationType;
import com.gft.unity.core.notification.SchedulingData;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.google.android.gcm.GCMRegistrar;

public class AndroidNotification extends AbstractNotification {
	private static final String LOGGER_MODULE = "INotification";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);

	// TODO i18n
	private static final String DEFAULT_BUTTON_TEXT = "Ok";

	/** Vibration frequency (in ms). */
	private static final int VIBRATION_FREQUENCY = 2000;

	private static ProgressDialog dialogLoading;
	private static Dialog dialogAlert;

	protected boolean playingBeep;
	protected boolean playingVibration;
	protected boolean runningLoading;

	public AndroidNotification() {

		dialogLoading = null;
		dialogAlert = null;

		playingBeep = false;
		playingVibration = false;
		runningLoading = false;
	}
	
	

	@Override
	public boolean IsNotifyActivityRunning() {
		// TODO implement INotification.IsNotifyActivityRunning
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean IsNotifyLoadingRunning() {

		LOGGER.logOperationBegin("IsNotifyLoadingRunning", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		LOGGER.logOperationEnd("IsNotifyLoadingRunning", runningLoading);

		return runningLoading;
	}

	@Override
	public boolean StartNotifyActivity() {
		// TODO implement INotification.StartNotifyActivity
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	// TODO review INotification.StartNotifyAlert implementation
	public boolean StartNotifyAlert(String message) {
		boolean result = false;

		LOGGER.logOperationBegin("StartNotifyAlert",
				new String[] { "message" }, new Object[] { message });

		try {
			result = StartNotifyAlert(null, message, DEFAULT_BUTTON_TEXT);
		} catch (Exception ex) {
			LOGGER.logError("StartNotifyAlert", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StartNotifyAlert", result);
		}

		return result;
	}

	@Override
	public boolean StartNotifyAlert(String title, String message,
			String buttonText) {
		boolean result = false;

		LOGGER.logOperationBegin("StartNotifyAlert", new String[] { "title",
				"message", "buttonText" }, new Object[] { title, message,
				buttonText });

		try {
			final String alertTitle = title;
			final String alertMessage = message;
			final String alertButtonText = buttonText;

			Runnable action = new Runnable() {

				@Override
				public void run() {
					AlertDialog.Builder builder = new AlertDialog.Builder(
							AndroidServiceLocator.getContext());
					if (alertTitle != null && !alertTitle.equals("")) {
						builder.setTitle(alertTitle);
					}
					if (alertMessage != null && !alertMessage.equals("")) {
						builder.setMessage(alertMessage);
					}
					if (alertButtonText != null && !alertButtonText.equals("")) {

						builder.setPositiveButton(alertButtonText,
								new DialogInterface.OnClickListener() {

									@Override
									public void onClick(DialogInterface dialog,
											int which) {
										dialog.cancel();
									}
								}).create();

					}
					AlertDialog dialog = builder.create();
					dialog.setCancelable(true);
					dialog.show();
				}
			};

			Activity activity = (Activity) AndroidServiceLocator.getContext();
			activity.runOnUiThread(action);
			result = true;
		} catch (Exception ex) {
			LOGGER.logError("StartNotifyAlert", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StartNotifyAlert", result);
		}

		return result;
	}

	@Override
	public boolean StartNotifyBeep() {
		// TODO implement INotification.StartNotifyBeep
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean StartNotifyBlink() {
		// TODO implement INotification.StartNotifyBlink
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean StartNotifyLoading(String loadingText) {
		boolean result = false;

		LOGGER.logOperationBegin("StartNotifyLoading",
				new String[] { "loadingText" }, new Object[] { loadingText });
		
		try {
			if(!runningLoading) {
				final String textLoading = loadingText;
				Runnable action = new Runnable() {
	
					@Override
					public void run() {
						Context context = AndroidServiceLocator.getContext();
						dialogLoading = ProgressDialog.show(context, null,
								textLoading, false);
						
						AndroidActivityManager aam = (AndroidActivityManager) AndroidServiceLocator
								.GetInstance()
								.GetService(
										AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
						if(aam!=null) aam.setNotifyLoadingVisible(true);
					}
				};
	
				Activity activity = (Activity) AndroidServiceLocator.getContext();
				activity.runOnUiThread(action);
				result = true;
				runningLoading = true;
			} else {
				LOGGER.logWarning("StartNotifyLoading", "Notify Loading already started. Call 'StopNotifiyLoading' method before send a new 'StartNotifyLoading'");
			}
		} catch (Exception ex) {
			LOGGER.logError("StartNotifyLoading", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StartNotifyLoading", result);
		}

		return result;
	}

	@Override
	public boolean StartNotifyVibrate() {
		boolean result = false;

		LOGGER.logOperationBegin("StartNotifyVibrate", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		try {
			Context context = AndroidServiceLocator.getContext();
			Vibrator vibrator = (Vibrator) context
					.getSystemService(Context.VIBRATOR_SERVICE);
			long[] pattern = { VIBRATION_FREQUENCY, 500 };
			vibrator.vibrate(pattern, 0);
			result = true;
			playingVibration = true;
		} catch (Exception ex) {
			LOGGER.logError("StartNotifyVibrate", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StartNotifyVibrate", result);
		}

		return result;
	}

	@Override
	public boolean StopNotifyActivity() {
		// TODO implement INotification.StopNotifyActivity
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean StopNotifyAlert() {
		boolean result = false;

		LOGGER.logOperationBegin("StopNotifyAlert", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		try {
			Runnable action = new Runnable() {

				@Override
				public void run() {
					if (dialogAlert != null && dialogAlert.isShowing()) {
						dialogAlert.dismiss();
						dialogAlert = null;
					}
				}
			};

			Activity activity = (Activity) AndroidServiceLocator.getContext();
			activity.runOnUiThread(action);
			result = true;
		} catch (Exception ex) {
			LOGGER.logError("StopNotifyAlert", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StopNotifyAlert", result);
		}

		return result;
	}

	@Override
	public boolean StopNotifyBeep() {
		// TODO implement INotification.StopNotifyBeep
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean StopNotifyBlink() {
		// TODO implement INotification.StopNotifyBlink
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean StopNotifyLoading() {
		boolean result = false;

		LOGGER.logOperationBegin("StopNotifyLoading", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		try {
			Runnable action = new Runnable() {

				@Override
				public void run() {
					if (dialogLoading != null && dialogLoading.isShowing()) {
						dialogLoading.dismiss();
						dialogLoading = null;
						
						AndroidActivityManager aam = (AndroidActivityManager) AndroidServiceLocator
								.GetInstance()
								.GetService(
										AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
						if(aam!=null) aam.setNotifyLoadingVisible(false);
					}
				}
			};

			Activity activity = (Activity) AndroidServiceLocator.getContext();
			activity.runOnUiThread(action);
			result = true;
			runningLoading = false;
		} catch (Exception ex) {
			LOGGER.logError("StopNotifyLoading", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StopNotifyLoading", result);
		}

		return result;
	}

	@Override
	public boolean StopNotifyVibrate() {
		boolean result = false;

		LOGGER.logOperationBegin("StopNotifyVibrate", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		try {
			Context context = AndroidServiceLocator.getContext();
			Vibrator vibrator = (Vibrator) context
					.getSystemService(Context.VIBRATOR_SERVICE);
			vibrator.cancel();
			result = true;
			playingVibration = false;
		} catch (Exception ex) {
			LOGGER.logError("StopNotifyVibrate", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("StopNotifyVibrate", result);
		}

		return result;
	}

	@Override
	public void UpdateNotifyLoading(float progress) {
		// TODO implement INotification.UpdateNotifyLoading
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public void RegisterForRemoteNotifications(String senderID,
			RemoteNotificationType[] types) {
		LOGGER.logOperationBegin("RegisterForRemoteNotifications", new String[]{"senderID","types"},
				new Object[]{senderID,types});
		
		try{
			Context ctx = AndroidServiceLocator.getContext();
			GCMRegistrar.checkDevice(ctx);
			GCMRegistrar.checkManifest(ctx);
			if(GCMRegistrar.isRegistered(ctx)) LOGGER.logInfo("RegisterForRemoteNotifications", "REGISTERED");
			String regId = GCMRegistrar.getRegistrationId(ctx);
			if(regId.equals("")){ 
				LOGGER.logInfo("RegisterForRemoteNotifications", "Registering device for sender id: " + senderID);
				GCMRegistrar.register(ctx, senderID);
				if(NotificationUtils.storeRemoteNotificationsSharedPreference(ctx, NotificationUtils.RN_PREFERENCE_SENDER_ID, senderID))
					LOGGER.logInfo("RegisterForRemoteNotifications", "Stored sender id on shared preferences");
			
			}else {
				String storedSenderId = NotificationUtils.getRemoteNotificationsSharedPreference(ctx, NotificationUtils.RN_PREFERENCE_SENDER_ID);
				LOGGER.logInfo("RegisterForRemoteNotifications", "Device already registered with ID: " + regId + ", and sender id: " + storedSenderId);
				
				if(storedSenderId!=null && senderID!=null && !storedSenderId.equalsIgnoreCase(senderID)) {
					LOGGER.logInfo("RegisterForRemoteNotifications", "Requested sender id does not match with the previous registered.");
					
					this.SendRegistrationFailureMessage(""+NotificationUtils.RN_ALREADY_REGISTERED_WITH_ANOTHER_SENDER_ID_EXCEPTION, 
							"Device already registered with a different sender Id. Please, unregister before register with another sender id");
				} else {
					// call the onSuccess listener if registration token is reused or a new token is obtained for the same sender id registration .
					this.SendRegistrationSuccessMessage(regId);
				}
			}
		}catch(Exception ex){ 
			LOGGER.logError("RegisterForRemoteNotifications", "Error", ex);
			this.SendRegistrationFailureMessage(""+NotificationUtils.RN_REGISTRATION_DEFAULT_EXCEPTION, ex.getMessage());
			
		}finally{LOGGER.logOperationEnd("RegisterForRemoteNotifications", null);}
	}
	
	/**
	 * Executes the Success listener to advise the application about a successful registration.
	 * @param registrationId
	 */
	private void SendRegistrationSuccessMessage(String registrationId) {
		
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		
		LOGGER.logInfo("RegisterForRemoteNotifications", "Calling Appverse.PushNotifications.OnRegisterForRemoteNotificationsSuccess...");
		
		RegistrationToken notificationToken = new RegistrationToken();
		notificationToken.setStringRepresentation(registrationId);
		notificationToken.setBinary(registrationId.getBytes());
			
		am.loadUrlIntoWebView("javascript:try{Appverse.PushNotifications.OnRegisterForRemoteNotificationsSuccess(" + JSONSerializer.serialize(notificationToken) +")}catch(e){}");
	}
	
	
	/**
	 * Executes the Failure listener to advise the application about some registration failure.
	 * @param exceptionCode The exception code
	 * @param exceptionMessage The exception message
	 */
	private void SendRegistrationFailureMessage(String exceptionCode, String exceptionMessage) {
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		
		LOGGER.logInfo("RegisterForRemoteNotifications", "Calling Appverse.PushNotifications.OnRegisterForRemoteNotificationsFailure...");
		
		RegistrationError notificationError = new RegistrationError();
		notificationError.setCode(exceptionCode);
		notificationError.setLocalizedDescription(exceptionMessage);
		am.loadUrlIntoWebView("javascript:try{Appverse.PushNotifications.OnRegisterForRemoteNotificationsFailure(" + JSONSerializer.serialize(notificationError) +")}catch(e){}");
	}

	@Override
	public void UnRegisterForRemoteNotifications() {
		LOGGER.logOperationBegin("UnRegisterForRemoteNotifications", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);
		try{
			Context ctx = AndroidServiceLocator.getContext();
			GCMRegistrar.unregister(ctx);
		}catch(Exception ex){ LOGGER.logError("UnRegisterForRemoteNotifications", "Error", ex);
		}finally{LOGGER.logOperationEnd("UnRegisterForRemoteNotifications", null);}
		
	}

	@Override
	public void CancelAllLocalNotifications() {
		LOGGER.logOperationBegin("CancelAllLocalNotifications", Logger.EMPTY_PARAMS, Logger.EMPTY_VALUES);
		
		try {
			Context context = AndroidServiceLocator.getContext();
			
			Map<String, ?> allLocalNotifications = NotificationUtils.getLocalNotificationsOnSharedPreferences(context);
			final Set<String> allLocalNotificationsIds = allLocalNotifications.keySet();
			LOGGER.logInfo("CancelAllLocalNotifications", "#num of scheduled local notifications to be cancelled: " + allLocalNotificationsIds.size());
			
			final AlarmManager am = NotificationUtils.getAlarmManager(context);
			
			for (String notificationId: allLocalNotificationsIds) {
				
				final Intent intent = new Intent(context, LocalNotificationReceiver.class);
				intent.setAction(notificationId);
				final PendingIntent pi = PendingIntent.getBroadcast(context, 0, intent, PendingIntent.FLAG_CANCEL_CURRENT);
				LOGGER.logInfo("CancelAllLocalNotifications", "Canceling local notification by id: " + notificationId);
				
				am.cancel(pi);
				
			}
			// removing notification from Shared Preferences
			NotificationUtils.removeAllLocalNotificationsFromSharedPrefereces(context);
				
		} catch (Exception e) {
			LOGGER.logError("CancelAllLocalNotifications", "Exception canceling local notification", e);
		}
		
		LOGGER.logOperationEnd("CancelAllLocalNotifications", null);
	}

	@Override
	public void CancelLocalNotification(DateTime fireDate) {
		LOGGER.logOperationBegin("CancelLocalNotification", new String[]{"fireDate"}, new Object[]{fireDate});
		
		/*
		 * Notification id is the identifier to cancel the notification 
		 * and cancel it.
		 */
		Context context = AndroidServiceLocator.getContext();
		final Intent intent = new Intent(context, LocalNotificationReceiver.class);
		String notificationId = NotificationUtils.getLocalNotificationId(context, fireDate);
		if(notificationId!=null) {
			LOGGER.logInfo("CancelLocalNotification", "Canceling local notification by id: " + notificationId);
			intent.setAction(notificationId);
	
			final PendingIntent pi = PendingIntent.getBroadcast(context, 0, intent, PendingIntent.FLAG_CANCEL_CURRENT);
			final AlarmManager am = NotificationUtils.getAlarmManager(context);
	
			try {
			    am.cancel(pi);
			} catch (Exception e) {
				LOGGER.logError("CancelLocalNotification", "Exception canceling local notification", e);
			}
			// removing notification from Shared Preferences
			NotificationUtils.removeLocalNotificationFromSharedPrefereces(context, notificationId);
		} else {
			LOGGER.logError("CancelLocalNotification", "Could not find any scheduled local notification by given fire date: " + fireDate);
		}
		
		LOGGER.logOperationEnd("CancelLocalNotification", null);
		
	}

	@Override
	public void PresentLocalNotificationNow(NotificationData notificationData) {
		LOGGER.logOperationBegin("PresentLocalNotificationNow", new String[]{"notificationData"}, new Object[]{notificationData});
		
		Context context = AndroidServiceLocator.getContext();
		NotificationUtils.processLocalNotification(context, notificationData, null);
		
		LOGGER.logOperationEnd("PresentLocalNotificationNow", null);
	}


	@Override
	public void ScheduleLocalNotification(NotificationData notificationData, SchedulingData schedule) {
		LOGGER.logOperationBegin("ScheduleLocalNotification", new String[]{"notificationData", "schedule"}, new Object[]{notificationData,schedule});
		
		Context context = AndroidServiceLocator.getContext();
		if(schedule != null) {
			int notificationId = NotificationUtils.processLocalNotification(context, notificationData, schedule);
			if(notificationId!=-1) {
				NotificationUtils.storeLocalNotificationOnSharedPreferences(context, ""+notificationId, notificationData, schedule);
			}
		} else {
			LOGGER.logError("ScheduleLocalNotification", "No suitable scheduling data object received for scheduling this local notification");
		}
		
		LOGGER.logOperationEnd("ScheduleLocalNotification", null);
	}
	
}
