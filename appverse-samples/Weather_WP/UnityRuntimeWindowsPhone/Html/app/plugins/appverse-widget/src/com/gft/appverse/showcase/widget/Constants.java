package com.gft.appverse.showcase.widget;

import java.util.Locale;

/**
 * Class for storing the Constants variables used in the widget application for the APPVERSE PAY project
 */
public class Constants {

	/* Debug tag for the widget */
	public static final String DEBUG_TAG = "Appverse-Widget";

	/* Locale */
	public static final Locale LOCALE = Locale.ITALY;

	/* Date & Time format */
	public static final String DATE_FORMAT = "dd/MM/yyyy";
	public static final String TIME_FORMAT = "HH:mm";

	/* Widget seconds update period */
	public static final int UPDATE_SECONDS_INTERVAL = 60;

	

	/* Security Key Chain variables */
	public static String APPVERSE_KEYPAIR_KEY = "WIDGETKEY";
	public static String APPVERSE_KEYPAIR_KEY_ENCRYPTED = "WIDGETKEYENCRYPTED";

	/* Variable stored in the shared preferences */
	public static final String WIDGET_ENABLED = "widget_payment_status";
	public static final String WIDGET_GETTING_BALANCE = "widget_getting_balance";

	/* Variable names used in the service extras */
	public static final String ACTION_ID = "action_id";
	public static final String LEFT_ACTION_BUTTON = "left_action_Button";
	public static final String RIGHT_ACTION_BUTTON = "right_action_button";
	public static final String LEFT_TEXT_BUTTON = "left_text_button";
	public static final String RIGHT_TEXT_BUTTON = "right_text_button";
	public static final String ERROR_TITLE = "error_title";
	public static final String ERROR_DETAIL = "error_detail";
	
	public static final String BUSINESS_RESPONSE_JSON_STRING = "business_response_jsonstring";
	public static final String BUSINESS_RESPONSE_REQUESTED_URI = "business_response_requested_uri";


	/* Widget phases (events or screens) identifiers */
	public static final int LOAD_START_SCREEN = 100;
	
	public static final int BUSINESS_RESPONSE = 200;
	
	
	/*Server error code for Device Unique ID expired*/
	public static final String EXPIRATION_DUI_ERROR_CODE = "-1001";

	public static final int WIDGET_NOT_FOUND = 0;


	public static final int APPVERSE_RESULT = 300;

	public static final int APPVERSE_TESTCASE = 400;

	public static final int APPVERSE_TESTCASE_SYNCHRONOUS = 401;
	public static final int APPVERSE_TESTCASE_STORE = 402;
	public static final int APPVERSE_TESTCASE_GETENC = 403;
	public static final int APPVERSE_TESTCASE_GET = 404;
	public static final int APPVERSE_TESTCASE_REMOVE = 405;
	

	

	

	
}
