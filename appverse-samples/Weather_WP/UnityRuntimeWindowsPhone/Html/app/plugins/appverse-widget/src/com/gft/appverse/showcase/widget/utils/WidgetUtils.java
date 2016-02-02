package com.gft.appverse.showcase.widget.utils;


import com.gft.appverse.showcase.R;
import com.gft.appverse.showcase.widget.Constants;
import com.gft.appverse.showcase.widget.WidgetProvider;

import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;
import android.util.Log;
import android.widget.Toast;

/**
 * Utils class for widget utilities
 */
public class WidgetUtils {

	/**
	 * Common function for logging using the android log on the project
	 * 
	 * @param level Log level
	 * @param message Message to be displayed
	 */
	public static void log(int level, String message) {

		switch (level) {
		case Log.VERBOSE:
			Log.v(Constants.DEBUG_TAG, "[Widget] "+message);
			break;
		case Log.DEBUG:
			Log.d(Constants.DEBUG_TAG, "[Widget] "+message);
			break;
		case Log.INFO:
			Log.i(Constants.DEBUG_TAG, "[Widget] "+message);
			break;
		case Log.WARN:
			Log.w(Constants.DEBUG_TAG, "[Widget] "+message);
			break;
		case Log.ERROR:
			Log.e(Constants.DEBUG_TAG, "[Widget] "+message);
			break;
		default:
			Log.e(Constants.DEBUG_TAG, "[Widget] Log level not supported");
			break;
		}
	}

	/**
	 * Method that throws a toast on the smartphone screen
	 * 
	 * @param ctx Android context
	 * @param message Message show (strings.xml)
	 * @param lenght Toast Lenght (short|long)
	 */
	public static void toast(Context ctx, int message, int lenght) {

		Toast toast = Toast.makeText(ctx, ctx.getString(message), lenght);
		toast.show();
	}

	public static void toast(Context ctx, String message, int lenght) {

		Toast toast = Toast.makeText(ctx, message, lenght);
		toast.show();
	}

	/**
	 * 
	 * @param ctx Android Context
	 * @param variable Variable name
	 * @param value Value for the variable
	 */
	public static void setBooleanSharedPreferences(String variable, Boolean value) {

		try {
			SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(WidgetProvider.getAppContext());
			SharedPreferences.Editor editor = sharedPreferences.edit();
			editor.putBoolean(variable, value);
			editor.commit();
		} catch (Exception e) {
			WidgetUtils.log(Log.ERROR, "Unhandled exception setting data to the shared preferences: " + e.getMessage());
		}
	}

	/**
	 * Method for retrieving a variable from the shared preferences of Android.
	 * 
	 * @param ctx Android Context
	 * @param variable Variable Name
	 * @return Variable value
	 */
	public static Boolean getBooleanSharedPreferences(String variable) {

		Boolean ret = null;
		try {
			SharedPreferences sharedPreferences = PreferenceManager.getDefaultSharedPreferences(WidgetProvider.getAppContext());
			ret = sharedPreferences.getBoolean(variable, false);
		} catch (Exception e) {
			WidgetUtils.log(Log.ERROR, "Unhandled exception retrieving data from the shared preferences: " + e.getMessage());
		}

		return ret;
	}
}
