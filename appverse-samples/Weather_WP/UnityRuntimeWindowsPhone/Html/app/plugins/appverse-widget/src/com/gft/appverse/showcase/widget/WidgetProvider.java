package com.gft.appverse.showcase.widget;

import java.util.Calendar;

import com.gft.appverse.showcase.widget.services.ActionDispacherService;
import com.gft.appverse.showcase.widget.services.WidgetUpdateService;
import com.gft.appverse.showcase.widget.utils.WidgetUtils;

import android.app.AlarmManager;
import android.app.PendingIntent;
import android.appwidget.AppWidgetManager;
import android.appwidget.AppWidgetProvider;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

/**
 * Main Class for the widget application. This AppWidgetProvider defines only the onUpdate() method
 * for the purpose of defining a PendingIntent that launches an Activity and attaching it to the App
 * Widget's button with setOnClickPendingIntent(int, PendingIntent).
 */
@SuppressWarnings("rawtypes")
public class WidgetProvider extends AppWidgetProvider {

	private PendingIntent service = null;
	
	private static Context appCtx = null;

	// private static int count = 0;

	// @formatter:off
	private static String APPVERSE_APP_CLASS_NAME = "org.me.unity4jui_android.AppverseApplication";
	private static String APPVERSE_APP_CTX_METHOD_NAME = "getAppContext";
	// @formatter:on

	/**
	 * This method is executed before all the methods in the class. This method is responsible to
	 * populate the static variables of the class
	 */
	static {

		try {

			// Getting the MyApplication class from appverse by name from a parameter on the
			// manifest
			Class MyApplication = Class.forName(APPVERSE_APP_CLASS_NAME);

			// Execute a method from the previous class by Java Reflection
			appCtx = (Context) MyApplication.getMethod(APPVERSE_APP_CTX_METHOD_NAME, null).invoke(null, null);

			WidgetUtils.log(Log.VERBOSE, "WidgerProvider STATIC POPULATION");

		} catch (ClassNotFoundException e) {
			WidgetUtils.log(Log.ERROR, "The Application Class defined by Appverse is not founded. Please review the Class Name define in the Widget Provider: " + e.getMessage());
		} catch (NoSuchMethodException e) {
			WidgetUtils.log(Log.ERROR, "The Method for obtaining the context defined in the Appverse Application Class is not founded. Please review the Method Name define in the Widget Provider: "
					+ e.getMessage());
		} catch (Exception e) {
			WidgetUtils.log(Log.ERROR, "Unhandled exception obtaining the context of the Appverse Application");
		}

	}

	/**
	 * Method used to obtain the application context
	 * 
	 * @return Application Context
	 */
	public static Context getAppContext() {
		return appCtx;
	}

	/**
	 * This method runs every time you install the widget on the home screen of your smartphone or
	 * the phone is rebooted.
	 */
	@Override
	public void onEnabled(Context ctx) {

		ctx = appCtx;

		WidgetUtils.log(Log.VERBOSE, "The WidgetProvider ENABLED. instances[" + hasInstances(ctx) + "]");

		super.onEnabled(ctx);

		WidgetUtils.log(Log.VERBOSE, "This widget has instances? " + hasInstances(ctx));

		try {
			// Start the service in order to execute the action
			Intent service = new Intent(ctx, ActionDispacherService.class);
			service.putExtra(Constants.ACTION_ID, Constants.LOAD_START_SCREEN);
			ctx.startService(service);

			// Set a global variable in the shared preferences in order to get the state of the
			// widget
			WidgetUtils.setBooleanSharedPreferences(Constants.WIDGET_ENABLED, false);

			// Starts the update service
			// startUpdateService(ctx);
		} catch (Exception e) {
			WidgetUtils.log(Log.ERROR, "There is being a problem starting the widget action dispacher service: " + e.getMessage());
		}
	}

	/**
	 * Called in response to the ACTION_APPWIDGET_UPDATE broadcast when this AppWidget provider is
	 * being asked to provide RemoteViews for a set of AppWidgets. Override this method to implement
	 * your own AppWidget functionality.
	 */
	@Override
	public void onUpdate(Context ctx, AppWidgetManager appWidgetManager, int[] appWidgetIds) {

		ctx = appCtx;
		
		WidgetUtils.log(Log.VERBOSE, "The WidgetProvider UPDATED. instances[" + hasInstances(ctx) + "]");

		// Log.e("test", "longitud: " + appWidgetIds.length + "");
		// for (int i = 0; i < appWidgetIds.length; i++) {
		// Log.e("test", appWidgetIds[i] + "");
		// AppWidgetHost host = new AppWidgetHost(ctx, 0);
		// host.deleteAppWidgetId(appWidgetIds[0]);
		// }

		// count++;
		// if (count > 1) {
		// AndroidUtils.toast(ctx, "count: " + count, Toast.LENGTH_SHORT);
		// AppWidgetHost host = new AppWidgetHost(ctx, 0);
		// host.deleteAppWidgetId(appWidgetIds[1]);
		// return;
		// }

		// Perform this loop procedure for each App Widget that belongs to this
		// provider
		for (int i = 0; i < appWidgetIds.length; i++) {

			// AndroidUtils.log(Log.VERBOSE, "The widget with identifier[" + appWidgetIds[i] +
			// "] is updated");

			// Start the service in order to execute the action
			// if (!AndroidUtils.getBooleanSharedPreferences(Constants.WIDGET_PAYMENT_STATUS)) {

			Intent service = new Intent(ctx, ActionDispacherService.class);
			service.putExtra(Constants.ACTION_ID, Constants.LOAD_START_SCREEN);
			ctx.startService(service);
			
			// }
		}
	}

	/**
	 * Implements onReceive(Context, Intent) to dispatch calls to the various other methods on
	 * AppWidgetProvider.
	 */
	@Override
	public void onReceive(Context ctx, Intent intent) {

		ctx = appCtx;

		final String action = intent.getAction();

		WidgetUtils.log(Log.VERBOSE, "The WidgetProvider RECEIVED (" + action + "). instances[" + hasInstances(ctx) + ": "+getInstances(ctx)+"]");

		if (AppWidgetManager.ACTION_APPWIDGET_DELETED.equals(action)) {

			WidgetUtils.log(Log.VERBOSE, "The widget provider is DELETED");

			final int appWidgetId = intent.getExtras().getInt(AppWidgetManager.EXTRA_APPWIDGET_ID, AppWidgetManager.INVALID_APPWIDGET_ID);

			if (appWidgetId != AppWidgetManager.INVALID_APPWIDGET_ID) {
				this.onDeleted(ctx, new int[] { appWidgetId });
			}

		} else {

			super.onReceive(ctx, intent);
		}
	}

	private boolean hasInstances(Context context) {
		AppWidgetManager appWidgetManager = AppWidgetManager.getInstance(context);
		int[] appWidgetIds = appWidgetManager.getAppWidgetIds(new ComponentName(context, this.getClass()));
		return (appWidgetIds.length > 0);
	}

	private int getInstances(Context context) {
		AppWidgetManager appWidgetManager = AppWidgetManager.getInstance(context);
		int[] appWidgetIds = appWidgetManager.getAppWidgetIds(new ComponentName(context, this.getClass()));
		return appWidgetIds.length;
	}

	/**
	 * This method runs every time you install the widget on the home screen of your smartphone or
	 * the phone is rebooted.
	 */
	@Override
	public void onDisabled(Context ctx) {

		ctx = appCtx;

		super.onDisabled(ctx);

		WidgetUtils.log(Log.VERBOSE, "The WidgetProvider DISABLED. instances[" + hasInstances(ctx) + "]");

		// count = 0;

		try {
			// Cancel the update service
			final AlarmManager m = (AlarmManager) ctx.getSystemService(Context.ALARM_SERVICE);
			m.cancel(service);

			// Set a global variable in the shared preferences in order to get the state of the
			// widget
			WidgetUtils.setBooleanSharedPreferences(Constants.WIDGET_ENABLED, false);

			
		} catch (Exception e) {
			WidgetUtils.log(Log.ERROR, "There is being a problem cancelling the payments and the update services " + e.getMessage());
		}
	}

	/**
	 * This method runs every time you remove the widget from the main screen of your smartphone.
	 * These method is used to cancel all the background tasks and services executed.
	 */
	@Override
	public void onDeleted(Context ctx, int[] appWidgetIds) {

		ctx = appCtx;

		super.onDeleted(ctx, appWidgetIds);

		WidgetUtils.log(Log.VERBOSE, "The WidgetProvider DISABLED. instances[" + hasInstances(ctx) + "]");

		// count = 0;

		try {
			// Cancel the update service
			final AlarmManager m = (AlarmManager) ctx.getSystemService(Context.ALARM_SERVICE);
			m.cancel(service);

			// Set a global variable in the shared preferences in order to get the state of the
			// widget
			WidgetUtils.setBooleanSharedPreferences(Constants.WIDGET_ENABLED, false);

			
		} catch (Exception e) {
			WidgetUtils.log(Log.ERROR, "There is being a problem cancelling the payments and the update services " + e.getMessage());
		}
	}

	/**
	 * Method tht throws the alarm in order to update the widget with a costum update time. The
	 * miminum default time in android in order to update the wodget is 30 minuts. With this system
	 * it is posible to update in one second. Carefully with the battery consumption.
	 * 
	 * @param ctx Android Context
	 */
	@SuppressWarnings("unused")
	private void startUpdateService(Context ctx) {

		WidgetUtils.log(Log.VERBOSE, "The service for updating the widget every " + Constants.UPDATE_SECONDS_INTERVAL + " is started");

		final AlarmManager m = (AlarmManager) ctx.getSystemService(Context.ALARM_SERVICE);

		final Calendar TIME = Calendar.getInstance();
		TIME.set(Calendar.MINUTE, 0);
		TIME.set(Calendar.SECOND, 0);
		TIME.set(Calendar.MILLISECOND, 0);

		// Create the service if is not created
		if (service == null) {
			service = PendingIntent.getService(ctx, 0, new Intent(ctx, WidgetUpdateService.class), PendingIntent.FLAG_CANCEL_CURRENT);
		}

		// Start the service repiting interval
		m.setRepeating(AlarmManager.RTC, TIME.getTime().getTime(), 1000 * Constants.UPDATE_SECONDS_INTERVAL, service);

	}

}
