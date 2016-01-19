package com.gft.appverse.android.push;



import java.io.InputStream;

import com.gft.appverse.android.push.notifications.RemoteNotificationIntentService;
import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.activity.AndroidActivityManager;
import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.android.notification.NotificationUtils;
import com.gft.unity.core.json.JSONSerializer;
import com.gft.unity.core.notification.RegistrationError;
import com.gft.unity.core.notification.RegistrationToken;
import com.gft.unity.core.notification.RemoteNotificationType;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.google.android.gcm.GCMRegistrar;

import android.app.Activity;
import android.content.Context;

public class AndroidPush extends AbstractNotification{
	private static final String LOGGER_MODULE = "INotification";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);

	private Context ctx;
	private IActivityManager am;
	
	private static boolean mIsInForegroundMode = false;
	
	public AndroidPush() {
		
	}
	
	public AndroidPush(Context context, IActivityManager activityManager ) {
		ctx = context;
		am = activityManager;
	}

	@Override
	public void RegisterForRemoteNotifications(String senderID,
			RemoteNotificationType[] types) {
		LOGGER.logOperationBegin("RegisterForRemoteNotifications", new String[]{"senderID","types"},
				new Object[]{senderID,types});
		
		try{
			
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
		
		/*IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		*/
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
		/*IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		*/
		
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
			//Context ctx = AndroidServiceLocator.getContext();
			GCMRegistrar.unregister(ctx);
		}catch(Exception ex){ LOGGER.logError("UnRegisterForRemoteNotifications", "Error", ex);
		}finally{LOGGER.logOperationEnd("UnRegisterForRemoteNotifications", null);}
		
	}
	
	public static boolean isInForeground() {
	    return mIsInForegroundMode;
	}


	@Override
	public void buildMode(boolean arg0) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public String getConfigFilePath() {
		// TODO Auto-generated method stub
		LOGGER.logDebug("GUI", "[K] getConfigFilePath");
		return null;
	}

	@Override
	public void onCreate() {
		RemoteNotificationIntentService.loadNotificationOptions(ctx.getResources(), (AndroidActivityManager) am, (Activity)ctx);
		mIsInForegroundMode = true;
		
	}

	@Override
	public void onDestroy() {
		mIsInForegroundMode = false;
		
	}

	@Override
	public void onPause() {
		mIsInForegroundMode = false;
		
	}

	@Override
	public void onResume() {
		mIsInForegroundMode = true;
		
	}

	@Override
	public void onStop() {
		mIsInForegroundMode = false;
		
	}

	@Override
	public void setConfigFileLoadedData(InputStream arg0) {
		// TODO Auto-generated method stub
		
	}

	
	
}
