package com.gft.unity.android.notification;

import java.util.HashMap;
import java.util.Random;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserFactory;

import android.app.Activity;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.net.Uri;
import android.os.Build;
import android.webkit.WebView;

import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.AndroidUtils;
import com.gft.unity.android.util.json.JSONSerializer;
import com.gft.unity.core.notification.NotificationData;
import com.gft.unity.core.notification.RegistrationError;
import com.gft.unity.core.notification.RegistrationToken;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.google.android.gcm.GCMBaseIntentService;;

public class RemoteNotificationIntentService extends GCMBaseIntentService {
	public static enum RemoteNotificationFields{
		RN_TITLE, RN_BODY, RN_TICKER, RN_SOUND, RN_SMALL_ICON, RN_LARGE_ICON, RN_LED_COLOR_ARGB
	};
	private static HashMap<String,String> FIELD_MAP;
	private static HashMap<String,String> NOTIFICATION_STRUCTURE;
	private static final String MAIN_ACTIVITY_CLASS_NAME = "$REPLACE_MAINACTIVITY_NAME_TOKEN$";
	private static final String PACKAGE_NAME = "$REPLACE_PACKAGE_TOKEN$";
	private static final String APPLICATION_NAME = "$REPLACE_APPNAME_TOKEN$";
	private static Resources APP_RESOURCES;
	private static WebView appView;
	private static Activity appActivity;
	private static NotificationData notificationData;
	private static RegistrationError notificationError;
	private static RegistrationToken notificationToken;
	
	private static final String NOTIFICATION_CONFIG_FILE = "app/config/notification-config.xml";
	private static final String DEFAULT_ENCODING = "UTF-8";
	private static final String ANDROID_NOTIFICATION_SECTION = "ANDROID";
	private static final String FIELD_NODE_ATTRIBUTE = "FIELD";
	private static final String TYPE_ATTRIBUTE = "type";
	private static final String NAME_ATTRIBUTE = "name";
	private static final String DRAWABLE_TYPE = "drawable";
	private static final String DEFAULT_ICON_NAME = "icon";
	
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

	public static void loadNotificationOptions(Resources resourcePackage, WebView view, Activity act){
		appView = view;
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
			xpp.setInput(AndroidUtils.getInstance().getAssetInputStream(context.getAssets(), NOTIFICATION_CONFIG_FILE),
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
			if(!returnValue.containsKey(RemoteNotificationFields.RN_TITLE.toString())||returnValue.get(RemoteNotificationFields.RN_TITLE.toString()).trim().equals("")){returnValue.put(RemoteNotificationFields.RN_TITLE.toString(), APPLICATION_NAME);}
			if(!returnValue.containsKey(RemoteNotificationFields.RN_TICKER.toString())||returnValue.get(RemoteNotificationFields.RN_TICKER.toString()).trim().equals("")){returnValue.put(RemoteNotificationFields.RN_TICKER.toString(), returnValue.get(RemoteNotificationFields.RN_TITLE.toString()));}
			if(!returnValue.containsKey(RemoteNotificationFields.RN_SMALL_ICON.toString())||returnValue.get(RemoteNotificationFields.RN_SMALL_ICON.toString()).trim().equals("")){returnValue.put(RemoteNotificationFields.RN_SMALL_ICON.toString(), "icon");}
			if(!returnValue.containsKey(RemoteNotificationFields.RN_LED_COLOR_ARGB.toString())||returnValue.get(RemoteNotificationFields.RN_LED_COLOR_ARGB.toString()).trim().equals("")){returnValue.put(RemoteNotificationFields.RN_LED_COLOR_ARGB.toString(), String.valueOf(Color.BLUE));}
			notificationData.setAlertMessage(returnValue.get(RemoteNotificationFields.RN_BODY.toString()));
			notificationData.setSound(returnValue.get(RemoteNotificationFields.RN_SOUND.toString()));
			
			notificationData.setCustomDataJsonString(JSONSerializer.serialize(JSONSerializable));
			JSONSerializable = null;
			return returnValue;
		}catch(Exception ex){}
		return null;
	}
	
	@Override
	protected void onMessage(Context context, Intent intent) {
		try{
			if(FIELD_MAP == null) FIELD_MAP = loadNotificationConfig(context);
			if(APP_RESOURCES == null) APP_RESOURCES = context.getResources();
			//Create the intent that will launch the application when the notification is clicked
			NotificationManager notifyManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
			Intent notificationIntent = new Intent("android.intent.action.MAIN");
			notificationIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK|Intent.FLAG_ACTIVITY_SINGLE_TOP|Intent.FLAG_ACTIVITY_CLEAR_TOP);
			notificationIntent.setComponent(ComponentName.unflattenFromString(MAIN_ACTIVITY_CLASS_NAME));
			notificationIntent.addCategory("android.intent.category.LAUNCHER");
			PendingIntent contentIntent = PendingIntent.getActivity(context, 0, notificationIntent, 0);
			
			NOTIFICATION_STRUCTURE = storeIntentExtras(context,intent);
			
			int iIconId = APP_RESOURCES.getIdentifier(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_SMALL_ICON.toString()), DRAWABLE_TYPE, PACKAGE_NAME);;
			if(iIconId == 0){
				iIconId = APP_RESOURCES.getIdentifier(DEFAULT_ICON_NAME, DRAWABLE_TYPE, PACKAGE_NAME);
			}
			
			Bitmap largeIconBMP = null;
			if(NOTIFICATION_STRUCTURE.containsKey(RemoteNotificationFields.RN_LARGE_ICON.toString())){
				int iLargeIconId = APP_RESOURCES.getIdentifier(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_LARGE_ICON.toString()), DRAWABLE_TYPE, PACKAGE_NAME);
				largeIconBMP = BitmapFactory.decodeResource(APP_RESOURCES, iLargeIconId);
			}

			Notification notif=null;
			//Different ways to create the notification depending the API Level restrictions
			if(Build.VERSION.SDK_INT < 11){
				notif = new Notification(iIconId, NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_TICKER.toString()), System.currentTimeMillis());//icon, text, time in millis
				notif.defaults=0;
				if(NOTIFICATION_STRUCTURE.containsKey(RemoteNotificationFields.RN_SOUND.toString())){ notif.sound = Uri.parse(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_SOUND.toString()));}
				else {
					notif.defaults |= Notification.DEFAULT_SOUND;
				}
				notif.setLatestEventInfo(context, NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_TITLE.toString()),NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_BODY.toString()), contentIntent);
				notif.ledARGB = Integer.parseInt(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_LED_COLOR_ARGB.toString()));
				notif.ledOffMS = 100;
				notif.ledOnMS = 100;
			}else{
				Notification.Builder mBuilder = new Notification.Builder(context)
				.setDefaults(0)
				.setContentIntent(contentIntent)
				.setSmallIcon(iIconId)
				.setLights(Integer.parseInt(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_LED_COLOR_ARGB.toString())), 100, 100)
				.setTicker(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_TICKER.toString()))
				.setContentText(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_BODY.toString()))
				.setContentTitle(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_TITLE.toString()));
				
				if(largeIconBMP!=null){mBuilder.setLargeIcon(largeIconBMP);}
				if(NOTIFICATION_STRUCTURE.containsKey(RemoteNotificationFields.RN_SOUND.toString())){ mBuilder.setSound(Uri.parse(NOTIFICATION_STRUCTURE.get(RemoteNotificationFields.RN_SOUND.toString())));}
				else {
					// set default sound
					mBuilder.setDefaults(Notification.DEFAULT_SOUND);
				}
				notif = mBuilder.getNotification();
				//if(Build.VERSION.SDK_INT < 16){notif = mBuilder.getNotification();}
				//else{notif = mBuilder.build();}
			}
			notif.flags = Notification.FLAG_SHOW_LIGHTS;
			Random rd = new Random();
			int notificationId =rd.nextInt(10000)+1;
			notifyManager.notify(notificationId, notif);

			if(appActivity != null && appView!=null){
				Runnable rNotification = new Runnable() {
					@Override
					public void run() {
						appView.loadUrl("javascript:try{Unity.OnRemoteNotificationReceived(" + JSONSerializer.serialize(notificationData) +")}catch(e){}");
					}
				};
				appActivity.runOnUiThread(rNotification);
			}
		}catch(Exception ex){/*CANNOT LOG CAUSE CALLING LOGGER WILL PROMPT NULL POINTER EXCEPTION*/} 		
	}

	@Override
	protected void onRegistered(Context context, String registrationId) {
		try{
		notificationToken = new RegistrationToken();
		notificationToken.setStringRepresentation(registrationId);
		notificationToken.setBinary(registrationId.getBytes());
		if(appActivity != null && appView!=null){
			Runnable rNotification = new Runnable() {
				@Override
				public void run() {
					appView.loadUrl("javascript:try{Unity.OnRegisterForRemoteNotificationsSuccess(" + JSONSerializer.serialize(notificationToken) +")}catch(e){}");
				}
			};
			appActivity.runOnUiThread(rNotification);
		}
		}catch(Exception ex){}
	}

	@Override
	protected void onUnregistered(Context context, String registrationId) {
		//nothing to do
	}
	
	@Override
	protected void onError(Context context, String error) {
		notificationError = new RegistrationError();
		notificationError.setLocalizedDescription(error);
		if(appActivity != null && appView!=null){
			Runnable rNotification = new Runnable() {
				@Override
				public void run() {
					appView.loadUrl("javascript:try{Unity.OnRegisterForRemoteNotificationsFailure(" + JSONSerializer.serialize(notificationError) +")}catch(e){}");
				}
			};
			appActivity.runOnUiThread(rNotification);
		}
	}
}
