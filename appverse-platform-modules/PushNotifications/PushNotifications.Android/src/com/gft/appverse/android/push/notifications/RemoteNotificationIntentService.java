package com.gft.appverse.android.push.notifications;

import java.util.HashMap;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserFactory;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.net.Uri;
import android.os.Build;

import com.gft.appverse.android.push.AndroidPush;
import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.activity.AndroidActivityManager;
import com.gft.unity.android.notification.NotificationUtils;
import com.gft.unity.core.json.JSONSerializer;
import com.gft.unity.core.notification.NotificationData;
import com.gft.unity.core.notification.RegistrationError;
import com.gft.unity.core.notification.RegistrationToken;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.google.android.gcm.GCMBaseIntentService;


public class RemoteNotificationIntentService extends GCMBaseIntentService {
	public static enum RemoteNotificationFields{
		RN_TITLE, RN_BODY, RN_TICKER, RN_SOUND, RN_SMALL_ICON, RN_LARGE_ICON, RN_LED_COLOR_ARGB, RN_LED_ONMS, RN_LED_OFFMS
	};
	
	

	public static final String DEFAULT_NOTIFICATION_NAME = "iconnotification";
	
	private static Resources APP_RESOURCES;
	private static AndroidActivityManager activityManager;
	private static Activity appActivity;
	private static NotificationData notificationData;
	private static RegistrationError notificationError;
	private static RegistrationToken notificationToken;
	
	private static final String NOTIFICATION_CONFIG_FILE = "app/config/notification-config.xml";
	private static final String DEFAULT_ENCODING = "UTF-8";
	private static final String ANDROID_NOTIFICATION_SECTION = "ANDROID";
	private static final String FIELD_NODE_ATTRIBUTE = "FIELD";
	private static final String STYLE_NODE_ATTRIBUTE = "STYLE";
	private static final String TYPE_ATTRIBUTE = "type";
	private static final String VALUE_ATTRIBUTE = "value";
	private static final String NAME_ATTRIBUTE = "name";
	
	private static final String BG_COLOR_NOTIFICATION_ICON_DEFAULT = "#213e7f";
	
	private static String BG_COLOR_NOTIFICATION_ICON = BG_COLOR_NOTIFICATION_ICON_DEFAULT;
	

	private static HashMap<String,String> FIELD_MAP;
	private static HashMap<String,String> NOTIFICATION_STRUCTURE;
	
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, "PUSH_NOTIFICATION_SERVICE");
	
	public RemoteNotificationIntentService(String senderId) {
		super(senderId);
		// TODO Auto-generated constructor stub
	}

	public RemoteNotificationIntentService() {
		super("PUSH_NOTIFICATION_SERVICE");
		// TODO Auto-generated constructor stub
	}

	public static void loadNotificationOptions(Resources resourcePackage, AndroidActivityManager aam, Activity act){
		activityManager = aam;
		appActivity = act;
		APP_RESOURCES = resourcePackage;
		FIELD_MAP = loadNotificationConfig(null);
	}
	
	private static HashMap<String, String> loadNotificationConfig(Context context) {


		if(context == null) context = AndroidServiceLocator.getContext();
		try {
			HashMap<String, String> returnValue = new HashMap<String, String>();
			XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
			factory.setNamespaceAware(true);
			XmlPullParser xpp = factory.newPullParser();
			xpp.setInput(context.getAssets().open(NOTIFICATION_CONFIG_FILE),
					DEFAULT_ENCODING);
			int eventType = xpp.getEventType();
			String fieldName = "";
			String fieldType = "";
			Boolean bAndroidSection = false;
			while (eventType != XmlPullParser.END_DOCUMENT) {
				if (eventType == XmlPullParser.START_TAG) {
					if (xpp.getName().toUpperCase()
							.equalsIgnoreCase(ANDROID_NOTIFICATION_SECTION)) { bAndroidSection = true;}
					if (xpp.getName().toUpperCase()
							.equalsIgnoreCase(FIELD_NODE_ATTRIBUTE) && bAndroidSection) {
						fieldName = xpp.getAttributeValue(null,
								NAME_ATTRIBUTE);
						fieldType = xpp.getAttributeValue(null,
								TYPE_ATTRIBUTE);
						returnValue .put(fieldName, fieldType);
					}
					if (xpp.getName().toUpperCase()
							.equalsIgnoreCase(STYLE_NODE_ATTRIBUTE) && bAndroidSection) {
						fieldName = xpp.getAttributeValue(null,
								NAME_ATTRIBUTE);
						
						if(fieldName.equals("bgcolor")) {
							BG_COLOR_NOTIFICATION_ICON = xpp.getAttributeValue(null, VALUE_ATTRIBUTE);
						}
						
					}
				} else if (eventType == XmlPullParser.END_TAG) {
					if (xpp.getName().toUpperCase()
							.equalsIgnoreCase(ANDROID_NOTIFICATION_SECTION)) { bAndroidSection = false;}
					if (xpp.getName().toUpperCase()
							.equalsIgnoreCase(FIELD_NODE_ATTRIBUTE) && bAndroidSection) {
						if (fieldName == null) {
							fieldType = "";
						}
						if (fieldType == null) {
							fieldType = "data";
						}
						returnValue .put(fieldName, fieldType);
					}
				}
				eventType = xpp.next();
			}	
			return returnValue ;
		} catch (Exception ex) {
			LOGGER.logError("PUSH NOTIFICATION LOAD CONFIG","LoadConfig error ["
					+ NOTIFICATION_CONFIG_FILE + "]: " + ex.getMessage());
		}
		return null;
	}

	private HashMap<String, String> storeIntentExtras(Context context, Intent intent) {
		try{

			notificationData = new NotificationData();
			HashMap<String, String> returnValue = new HashMap<String, String>();
			HashMap<String, String> JSONSerializable = new HashMap<String, String>();
			
			for(String sFieldName:intent.getExtras().keySet()){
				if(FIELD_MAP.containsKey(sFieldName)){
					String sFieldType = FIELD_MAP.get(sFieldName);
					returnValue.put(sFieldType, intent.getStringExtra(sFieldName));
					JSONSerializable.put(sFieldType, intent.getStringExtra(sFieldName));
				}else{
					JSONSerializable.put(sFieldName, intent.getStringExtra(sFieldName));
				}
			}
			//fill mandatory fields
			if(!returnValue.containsKey(RemoteNotificationFields.RN_TITLE.toString())||returnValue.get(RemoteNotificationFields.RN_TITLE.toString()).trim().equals("")){returnValue.put(RemoteNotificationFields.RN_TITLE.toString(), NotificationUtils.APPLICATION_NAME);}
			if(!returnValue.containsKey(RemoteNotificationFields.RN_TICKER.toString())||returnValue.get(RemoteNotificationFields.RN_TICKER.toString()).trim().equals("")){returnValue.put(RemoteNotificationFields.RN_TICKER.toString(), returnValue.get(RemoteNotificationFields.RN_TITLE.toString()));}
			if(!returnValue.containsKey(RemoteNotificationFields.RN_SMALL_ICON.toString())||returnValue.get(RemoteNotificationFields.RN_SMALL_ICON.toString()).trim().equals("")){returnValue.put(RemoteNotificationFields.RN_SMALL_ICON.toString(), "icon");}
			if(!returnValue.containsKey(RemoteNotificationFields.RN_LED_COLOR_ARGB.toString())||returnValue.get(RemoteNotificationFields.RN_LED_COLOR_ARGB.toString()).trim().equals("")){returnValue.put(RemoteNotificationFields.RN_LED_COLOR_ARGB.toString(), String.valueOf(Color.BLUE));}
			if(!returnValue.containsKey(RemoteNotificationFields.RN_LED_ONMS.toString())||returnValue.get(RemoteNotificationFields.RN_LED_ONMS.toString()).trim().equals("")){returnValue.put(RemoteNotificationFields.RN_LED_ONMS.toString(), String.valueOf(1000));}
			if(!returnValue.containsKey(RemoteNotificationFields.RN_LED_OFFMS.toString())||returnValue.get(RemoteNotificationFields.RN_LED_OFFMS.toString()).trim().equals("")){returnValue.put(RemoteNotificationFields.RN_LED_OFFMS.toString(), String.valueOf(4000));}
			
			notificationData.setAlertMessage(returnValue.get(RemoteNotificationFields.RN_BODY.toString()));
			notificationData.setSound(returnValue.get(RemoteNotificationFields.RN_SOUND.toString()));
			
			notificationData.setCustomDataJsonString(JSONSerializer.serialize(JSONSerializable));
			notificationData.setAppWasRunning(AndroidPush.isInForeground());
			JSONSerializable = null;
			return returnValue;
		}catch(Exception ex){}
		return null;
	}
	
	private int getNotificationIcon() {

		int iIconId = 0;
	    boolean whiteIcon = (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP);
	    if( whiteIcon ){
	    	iIconId = APP_RESOURCES.getIdentifier(DEFAULT_NOTIFICATION_NAME, NotificationUtils.DRAWABLE_TYPE, NotificationUtils.PACKAGE_NAME);
			if(iIconId != 0){
				return iIconId;
			}
	    }

		//Log.v("GUI [K]", "[K] NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_SMALL_ICON.toString()) "+NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_SMALL_ICON.toString()));
		iIconId = APP_RESOURCES.getIdentifier(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_SMALL_ICON.toString()), NotificationUtils.DRAWABLE_TYPE, NotificationUtils.PACKAGE_NAME);;
			if(iIconId == 0){

			
			iIconId = APP_RESOURCES.getIdentifier(NotificationUtils.APPLICATION_NAME, NotificationUtils.DRAWABLE_TYPE, NotificationUtils.PACKAGE_NAME);
		}else if (iIconId == 0){

			iIconId = APP_RESOURCES.getIdentifier(NotificationUtils.DEFAULT_ICON_NAME, NotificationUtils.DRAWABLE_TYPE, NotificationUtils.PACKAGE_NAME);
		}
		
		return iIconId;
	}
	
	@SuppressLint("NewApi")
	@Override
	protected void onMessage(Context context, Intent intent) {
		
		try{
			//TODO REMOVE REFERENCES
			
			if(FIELD_MAP == null) FIELD_MAP = loadNotificationConfig(context);
			if(APP_RESOURCES == null) APP_RESOURCES = context.getResources();
			//Create the intent that will launch the application when the notification is clicked
			NotificationManager notifyManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
			
			NOTIFICATION_STRUCTURE = storeIntentExtras(context,intent);
			int notificationId = NotificationUtils.getNotificationId();
			PendingIntent contentIntent = NotificationUtils.getMainActivityAsPendingIntent(
					context, NotificationUtils.NOTIFICATION_TYPE_REMOTE, NotificationUtils.MAIN_ACTIVITY_CLASS_NAME, ""+notificationId, notificationData);
			
			
			
			Bitmap largeIconBMP = null;
			if(NOTIFICATION_STRUCTURE.containsKey(RemoteNotificationFields.RN_LARGE_ICON.toString())){
				int iLargeIconId = APP_RESOURCES.getIdentifier(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_LARGE_ICON.toString()), NotificationUtils.DRAWABLE_TYPE, NotificationUtils.PACKAGE_NAME);
				largeIconBMP = BitmapFactory.decodeResource(APP_RESOURCES, iLargeIconId);
			}

			Notification notif=null;
			
			
			Notification.Builder mBuilder = new Notification.Builder(context)
				.setDefaults(0)
				.setContentIntent(contentIntent)
				//.setColor(Color.parseColor(BG_COLOR_NOTIFICATION_ICON))
				.setSmallIcon(getNotificationIcon())
				.setLights(Integer.parseInt(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_LED_COLOR_ARGB.toString())), 
						Integer.valueOf(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_LED_ONMS.toString())), 
						Integer.valueOf(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_LED_OFFMS.toString())))
				.setTicker(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_TICKER.toString()))
				.setContentText(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_BODY.toString()))
				.setContentTitle(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_TITLE.toString()));
				
			if(largeIconBMP!=null){mBuilder.setLargeIcon(largeIconBMP);}
			if(NOTIFICATION_STRUCTURE.containsKey(RemoteNotificationFields.RN_SOUND.toString())){ mBuilder.setSound(Uri.parse(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_SOUND.toString())));}
			else {
				// set default sound
				mBuilder.setDefaults(Notification.DEFAULT_SOUND);
			}
							
			if(Build.VERSION.SDK_INT < Build.VERSION_CODES.LOLLIPOP){
				notif = mBuilder.getNotification();}
			else{

				mBuilder.setColor(Color.parseColor(BG_COLOR_NOTIFICATION_ICON));		
				notif = mBuilder.build();
			}
			
			notif.flags = Notification.FLAG_SHOW_LIGHTS;
			if(!AndroidPush.isInForeground()){
				notifyManager.notify(notificationId, notif);
			}
			
			if(appActivity != null && activityManager!=null){
				
				activityManager.loadUrlIntoWebView("javascript:try{Appverse.PushNotifications.OnRemoteNotificationReceived(" + JSONSerializer.serialize(notificationData) +")}catch(e){}");
			}
			
		}catch(Exception ex){/*CANNOT LOG CAUSE CALLING LOGGER WILL PROMPT NULL POINTER EXCEPTION*/} 		
	}

	@Override
	protected void onRegistered(Context context, String registrationId) {
		try{
			LOGGER.logInfo("onRegistered", "Device Successfully registered.");
			notificationToken = new RegistrationToken();
			notificationToken.setStringRepresentation(registrationId);
			notificationToken.setBinary(registrationId.getBytes());
			
			if(appActivity != null && activityManager!=null){

				LOGGER.logInfo("onRegistered", "Calling Appverse.PushNotifications.OnRegisterForRemoteNotificationsSuccess...");
				activityManager.loadUrlIntoWebView("javascript:try{Appverse.PushNotifications.OnRegisterForRemoteNotificationsSuccess(" + JSONSerializer.serialize(notificationToken) +")}catch(e){}");
				
			}
		}catch(Exception ex){
			LOGGER.logError("onRegistered",ex.getMessage());
		}
	}

	@Override
	protected void onUnregistered(Context context, String registrationId) {
		try{
			LOGGER.logInfo("onUnregistered", "Device Successfully unregistered");
			
			// remove shared preference (sender id)
			NotificationUtils.removeRemoteNotificationsSharedPreference(context, NotificationUtils.RN_PREFERENCE_SENDER_ID);
			
			// call platform listener to advise application
			if(appActivity != null && activityManager!=null){
				LOGGER.logInfo("onUnregistered", "Calling Appverse.PushNotifications.OnUnRegisterForRemoteNotificationsSuccess...");
				activityManager.loadUrlIntoWebView("javascript:try{Appverse.PushNotifications.OnUnRegisterForRemoteNotificationsSuccess()}catch(e){}");
			}
		} catch(Exception ex){
			LOGGER.logError("onUnregistered",ex.getMessage());
		}
	}
	
	@Override
	protected void onError(Context context, String error) {
		notificationError = new RegistrationError();
		notificationError.setCode(""+NotificationUtils.RN_GCM_SERVER_EXCEPTION);
		notificationError.setLocalizedDescription(error);
		if(appActivity != null && activityManager!=null){
			LOGGER.logInfo("onError", "Calling Appverse.PushNotifications.OnRegisterForRemoteNotificationsFaAppverse.PushNotifications...");
			activityManager.loadUrlIntoWebView("javascript:try{Appverse.PushNotifications.OnRegisterForRemoteNotificationsFailure(" + JSONSerializer.serialize(notificationError) +")}catch(e){}");
		}
	}
}
