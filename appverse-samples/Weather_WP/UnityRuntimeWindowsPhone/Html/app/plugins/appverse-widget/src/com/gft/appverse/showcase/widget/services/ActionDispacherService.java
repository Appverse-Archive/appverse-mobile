package com.gft.appverse.showcase.widget.services;


import java.util.Date;

import org.json.JSONStringer;

import com.gft.appverse.showcase.R;
import com.gft.appverse.showcase.widget.Constants;
import com.gft.appverse.showcase.widget.WidgetProvider;
import com.gft.appverse.showcase.widget.utils.WidgetUtils;
import com.gft.appverse.widget.json.JSONException;
import com.gft.appverse.widget.json.JSONObject;
import com.gft.appverse.widget.json.JSONSerializer;
import com.gft.appverse.widget.json.JSONTokener;
import com.gft.appverse.widget.security.KeyPair;

import android.app.PendingIntent;
import android.app.Service;
import android.appwidget.AppWidgetManager;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.res.Resources.NotFoundException;
import android.os.Bundle;
import android.os.IBinder;
import android.util.Log;
import android.widget.RemoteViews;
import android.widget.TextView;

public class ActionDispacherService extends Service {
	
	private static final String PARAM_PREFIX = "param";
	
	static String SERVICE_KEY = "service";
	static String APPVERSE_SERVICE_KEY = "appverse_service";
	static String APPVERSE_METHOD_KEY = "appverse_method";
	static String APPVERSE_PARAMS_KEY = "appverse_params";
	static String APPVERSE_RESULT_KEY = "appverse_result";
	
	private Context ctx;
	
	private TextView result;

	/**
	 * The system calls this method when the service is first created, to perform one-time setup
	 * procedures (before it calls either onStartCommand() or onBind()). If the service is already
	 * running, this method is not called.
	 */
	@Override
	public void onCreate() {

		super.onCreate();

		// Creates the instance for interaction with the library
		
		

		WidgetUtils.log(Log.VERBOSE, "The ActionDispacherService for the widget is created");
	}

	@Override
	protected void finalize() throws Throwable {

		super.finalize();

		WidgetUtils.log(Log.VERBOSE, "The ActionDispacherService for the widget is finalized.");
	}

	@Override
	public void onLowMemory() {

		super.onLowMemory();

		WidgetUtils.log(Log.VERBOSE, "The ActionDispacherService for the widget is on Low Memory.");
	}

	@Override
	public void onStart(Intent intent, int startId) {

		super.onStart(intent, startId);

		WidgetUtils.log(Log.VERBOSE, "The ActionDispacherService for the widget is Started.");
	}

	/**
	 * The system calls this method when another component, such as an activity, requests that the
	 * service be started, by calling startService(). Once this method executes, the service is
	 * started and can run in the background indefinitely. If you implement this, it is your
	 * responsibility to stop the service when its work is done, by calling stopSelf() or
	 * stopService().
	 */
	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {

		if (intent != null && intent.getExtras()!=null) {
			WidgetUtils.log(Log.VERBOSE, "The ActionDispacherService executes the start command with an action: " + intent.getExtras().getInt(Constants.ACTION_ID));

			
			try {
				// Executes the method that handles the screen or action management
				this.actionDispacher(intent.getExtras().getInt(Constants.ACTION_ID), intent.getExtras());
			} catch (Exception e) {
				WidgetUtils.log(Log.ERROR, "Unhandled exception retrieving the data from the extras: " + e.getMessage());
			}
		}

		return super.onStartCommand(intent, flags, startId);
	}

	/**
	 * Method that handles the actions to do in the widget apperance in order to perfom the system
	 * and the user actions.
	 * 
	 * @param actionId Action identifier
	 */
	private void actionDispacher(int actionId, Bundle extras) {

		ctx = WidgetProvider.getAppContext();

		RemoteViews views = new RemoteViews(ctx.getPackageName(), R.layout.appverse_main);
		Intent intent = null;
		PendingIntent pendingIntent = null;

		try {
			// Get the current widget by class name
			ComponentName thisWidget = new ComponentName(ctx, WidgetProvider.class);
			AppWidgetManager manager = AppWidgetManager.getInstance(ctx);
			int labelId = getResources().getIdentifier("result_label", "id", getPackageName());
			
			switch (actionId) {
			case Constants.LOAD_START_SCREEN:
				WidgetUtils.log(Log.DEBUG, "LOAD_START_SCREEN");
				
				
				WidgetUtils.log(Log.DEBUG, "Set main View");
				Intent intentSynchronous = new Intent(ctx, ActionDispacherService.class);
				//intent.removeExtra(Constants.ACTION_ID);
				//intentSynchronous.putExtra(Constants.ACTION_ID, Constants.APPVERSE_TESTCASE);
				//PendingIntent pendingIntentSynchornous = PendingIntent.getService(ctx, Constants.APPVERSE_TESTCASE, intentSynchronous, 0);
				
				intentSynchronous.putExtra(Constants.ACTION_ID, Constants.APPVERSE_TESTCASE_SYNCHRONOUS);
				WidgetUtils.log(Log.DEBUG, "Set intent for synchronous");
				PendingIntent pendingIntentSynchornous = PendingIntent.getService(ctx, Constants.APPVERSE_TESTCASE_SYNCHRONOUS, intentSynchronous, 0);
				views.setOnClickPendingIntent(R.id.synchronous, pendingIntentSynchornous);
				
				Intent intentStore = new Intent(ctx, ActionDispacherService.class);
				intentStore.putExtra(Constants.ACTION_ID, Constants.APPVERSE_TESTCASE_STORE);
				PendingIntent pendingIntentStore = PendingIntent.getService(ctx, Constants.APPVERSE_TESTCASE_STORE, intentStore,  0);
				views.setOnClickPendingIntent(R.id.store, pendingIntentStore);
				
				Intent intentGetEnc = new Intent(ctx, ActionDispacherService.class);
				intentGetEnc.putExtra(Constants.ACTION_ID, Constants.APPVERSE_TESTCASE_GETENC);
				PendingIntent pendingIntentGetEnc = PendingIntent.getService(ctx, Constants.APPVERSE_TESTCASE_GETENC, intentGetEnc,  0);
				views.setOnClickPendingIntent(R.id.getEnc, pendingIntentGetEnc);
				
				Intent intentGet = new Intent(ctx, ActionDispacherService.class);
				intentGet.putExtra(Constants.ACTION_ID, Constants.APPVERSE_TESTCASE_GET);
				PendingIntent pendingIntentGet = PendingIntent.getService(ctx, Constants.APPVERSE_TESTCASE_GET, intentGet,  0);
				views.setOnClickPendingIntent(R.id.get, pendingIntentGet);

				Intent intentRemove = new Intent(ctx, ActionDispacherService.class);
				intentRemove.putExtra(Constants.ACTION_ID, Constants.APPVERSE_TESTCASE_REMOVE);
				PendingIntent pendingIntentRemove = PendingIntent.getService(ctx, Constants.APPVERSE_TESTCASE_REMOVE, intentRemove,  0);
				views.setOnClickPendingIntent(R.id.remove, pendingIntentRemove);
				
				WidgetUtils.log(Log.DEBUG, "Pending intent created");
				
				// Create the view and update the widget
				manager.updateAppWidget(thisWidget, views);
				WidgetUtils.log(Log.DEBUG, "updateAppWidget");
				break;
				
			case Constants.APPVERSE_TESTCASE:

				WidgetUtils.log(Log.DEBUG, "APPVERSE_TESTCASE");
				// Creates a Intent in order to go to the home screen

				//intent = createTestCase( Constants.APPVERSE_RESULT, "security", "GetStoredKeyValuePair", "{'param1':{'Key':'mykey1'}}");
				//intent = createTestCase( Constants.APPVERSE_RESULT, "security", "StoreKeyValuePair", "{'param1':{'Key':'mykey1','Encryption':true,'Value':'appverse'}}");
				//intent = createTestCase( Constants.APPVERSE_RESULT, "security", "IsDeviceModified", null);
				
				
				//WidgetProvider.getAppContext().startActivity(intent);
				views.setTextViewText(labelId, "TEST! "+(new Date().getTime()%10));
				// Create the view and update the widget
				manager.updateAppWidget(thisWidget, views);
				break;
			case Constants.APPVERSE_TESTCASE_SYNCHRONOUS:

				WidgetUtils.log(Log.DEBUG, "APPVERSE_TESTCASE_SYNCHRONOUS");
				// Creates a Intent in order to go to the home screen

				intent = createTestCase( Constants.APPVERSE_RESULT, "security", "IsDeviceModified", null);
				
				
				WidgetProvider.getAppContext().startActivity(intent);
				break;
			case Constants.APPVERSE_TESTCASE_STORE:

				WidgetUtils.log(Log.DEBUG, "APPVERSE_TESTCASE_STORE");
				// Creates a Intent in order to go to the home screen

				intent = createTestCase( Constants.APPVERSE_RESULT, "security", "StoreKeyValuePair",  "{'param1':{'Key':'mykey1','Encryption':true,'Value':'appverse'}}");
				
				
				WidgetProvider.getAppContext().startActivity(intent);
				break;
			case Constants.APPVERSE_TESTCASE_GETENC:

				WidgetUtils.log(Log.DEBUG, "APPVERSE_TESTCASE_GETENC");
				// Creates a Intent in order to go to the home screen

				intent = createTestCase( Constants.APPVERSE_RESULT, "security", "GetStoredKeyValuePair", "{'param1':{'Key':'mykey1','Encryption':true}}");
				
				
				WidgetProvider.getAppContext().startActivity(intent);
				break;
			case Constants.APPVERSE_TESTCASE_GET:

				WidgetUtils.log(Log.DEBUG, "APPVERSE_TESTCASE_GET");
				// Creates a Intent in order to go to the home screen

				intent = createTestCase( Constants.APPVERSE_RESULT, "security", "GetStoredKeyValuePair", "{'param1':{'Key':'mykey1'}}");
				
				
				WidgetProvider.getAppContext().startActivity(intent);
				break;
			case Constants.APPVERSE_TESTCASE_REMOVE:

				WidgetUtils.log(Log.DEBUG, "APPVERSE_TESTCASE_REMOVE");
				// Creates a Intent in order to go to the home screen

				intent = createTestCase( Constants.APPVERSE_RESULT, "security", "RemoveStoredKeyValuePair", "{'param1':'mykey1'}");	
				
				WidgetProvider.getAppContext().startActivity(intent);
				break;
			case Constants.APPVERSE_RESULT:

				WidgetUtils.log(Log.DEBUG, "APPVERSE_RESULT");
				
				if(extras.containsKey(APPVERSE_RESULT_KEY)){
					processResult(extras);
				}
				
				
				break;
			
			/*
			 * *****************************************************************************************
			 * DEFAULT CASE: Error...
			 * ***************************************************************
			 * **************************
			 */
			default:

				WidgetUtils.log(Log.ERROR, "Action NOT FOUND");

				break;
			}
			
			
		} catch (NotFoundException e) {
			WidgetUtils.log(Log.ERROR, "Unhandled exception handling the widget action(" + actionId + ") with this message: " + e.getMessage());
		} catch (Exception e) {
			WidgetUtils.log(Log.ERROR, "Unhandled exception handling the widget action(" + actionId + ") with this message: " + e.getMessage());
			
		}
		WidgetUtils.log(Log.DEBUG, "actionDispacher END");
	}
	
	
	private void processResult(Bundle extras) {
		WidgetUtils.log(Log.DEBUG, "processResult" );
		// Get the current widget by class name
		ComponentName thisWidget = new ComponentName(ctx, WidgetProvider.class);
		AppWidgetManager manager = AppWidgetManager.getInstance(ctx);
		RemoteViews views = new RemoteViews(ctx.getPackageName(), R.layout.appverse_main);
		
		int labelId = getResources().getIdentifier("result_label", "id", getPackageName());
		WidgetUtils.log(Log.DEBUG, "labelId: " + labelId );
		
		
		
		String result = extras.getString(APPVERSE_RESULT_KEY);
		WidgetUtils.log(Log.DEBUG, "service RESULT: " + result );
		String method = extras.getString(APPVERSE_METHOD_KEY);
		if(method.equals("GetStoredKeyValuePair")){
			

			WidgetUtils.log(Log.DEBUG, "Method GetStoredKeyValuePair");
			Object[] found = (Object[]) fromJSONtoKeyPair(result);
			if(found != null && found[0] != null){
				KeyPair[] success = (KeyPair[]) found[0];
				for(KeyPair kp : success){
					result = "KeyPair "+kp.getValue();
					WidgetUtils.log(Log.DEBUG, result);
				}
			}
			views.setTextViewText(labelId, result);
			
		}else if(method.equals("GetStoredKeyValuePairs")){
			
			WidgetUtils.log(Log.DEBUG, "Method GetStoredKeyValuePairs");
		
		}else if(method.equals("IsDeviceModified")){
			
			WidgetUtils.log(Log.DEBUG, "Method IsDeviceModified");
			result = "IsDeviceModified: "+extras.get(APPVERSE_RESULT_KEY);
			WidgetUtils.log(Log.DEBUG, result);
			views.setTextViewText(labelId, result);
			
		}else if(method.equals("IsROMModified")){
		
			WidgetUtils.log(Log.DEBUG, "Method IsROMModified");
			
		}else if(method.equals("RemoveStoredKeyValuePair")){
			
			WidgetUtils.log(Log.DEBUG, "Method RemoveStoredKeyValuePair");
			result = "RemoveStoredKeyValuePair: "+extras.get(APPVERSE_RESULT_KEY);
			WidgetUtils.log(Log.DEBUG, result);
			
			
			views.setTextViewText(labelId, result);

		
		}else if(method.equals("RemoveStoredKeyValuePairs")){
		
			WidgetUtils.log(Log.DEBUG, "Method RemoveStoredKeyValuePairs");
			
		}else if(method.equals("StoreKeyValuePair")){
			
			WidgetUtils.log(Log.DEBUG, "Method StoreKeyValuePair");
			Object[] found = (Object[]) fromJSONtoKeyPair(result);
			if(found != null && found[0] != null){
				KeyPair[] success = (KeyPair[]) found[0];
				for(KeyPair kp : success){
					result = "KeyPair "+kp.getValue();
					WidgetUtils.log(Log.DEBUG, result);
				}
			}

			
			views.setTextViewText(labelId, result);
		
		}else  if(method.equals("StoreKeyValuePairs")){
		
			WidgetUtils.log(Log.DEBUG, "Method StoreKeyValuePairs");
			

		}else{
			WidgetUtils.log(Log.DEBUG, "Method not supported YET");
		}
		
		// Create the view and update the widget
		manager.updateAppWidget(thisWidget, views);
		WidgetUtils.log(Log.DEBUG, "updateAppWidget");
	}

	private Object fromJSONtoKeyPair(String params){
		try {
			return JSONSerializer.deserialize(KeyPair.class, toJSONObject(params));
		} catch (Exception e) {
			WidgetUtils.log(Log.DEBUG, "Cast Exception: "+e.getMessage());
		}
		return null;
	}
	
	
	private Object toJSONObject(String params){
		//List<Object> jsonParameters = new ArrayList<Object>();
		try {
			if (params != null) {
				JSONObject query = (JSONObject) new JSONTokener(params)
						.nextValue();
				Object parameter = null;
				for (int index = 1; index <= query.length(); index++) {
					parameter = query.get(PARAM_PREFIX + index);
					//jsonParameters.add(parameter);
				}
				return parameter;
			}
		} catch (JSONException ex) {
			WidgetUtils.log(Log.DEBUG, "Parse query error"+ ex.getMessage());
		}
		
		return null;
		
	}
	
	
	private KeyPair[] fromJSONtoKeyPairArray(String params){
		try {
			return (KeyPair[]) JSONSerializer.deserialize(KeyPair[].class, toJSONObject(params));
		} catch (Exception e) {
			WidgetUtils.log(Log.DEBUG, "Cast Exception: "+e.getMessage());
		}
		return null;
	}
	
	/*
	 * Crerate the intent to execute the testcase
	 */
	private Intent createTestCase(int action, String service, String method, String params){
		Intent intent = new Intent(ctx.getPackageName() + ".APPVERSE_SERVICE");
		intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		intent.putExtra(SERVICE_KEY, this.getClass().getName());

		intent.putExtra(Constants.ACTION_ID, action);
		intent.putExtra(APPVERSE_SERVICE_KEY, service);
		intent.putExtra(APPVERSE_METHOD_KEY, method);
		intent.putExtra(APPVERSE_PARAMS_KEY, params);

		
		return intent;
		
	}
	
	/**
	 * The system calls this method when another component wants to bind with the service (such as
	 * to perform RPC), by calling bindService(). In your implementation of this method, you must
	 * provide an interface that clients use to communicate with the service, by returning an
	 * IBinder. You must always implement this method, but if you don't want to allow binding, then
	 * you should return null.
	 */
	@Override
	public IBinder onBind(Intent intent) {

		return null;
	}

	/**
	 * The system calls this method when the service is no longer used and is being destroyed. Your
	 * service should implement this to clean up any resources such as threads, registered
	 * listeners, receivers, etc. This is the last call the service receives.
	 */
	@Override
	public void onDestroy() {

		super.onDestroy();

		WidgetUtils.log(Log.VERBOSE, "The ActionDispacherService for the widget is destroyed.");
	}
	
	
	

}
