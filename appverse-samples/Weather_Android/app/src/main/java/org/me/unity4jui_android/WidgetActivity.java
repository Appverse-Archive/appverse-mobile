package org.me.unity4jui_android;


import java.util.ArrayList;
import java.util.List;

import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.android.activity.IActivityManagerListener;
import com.gft.unity.android.log.AndroidLoggerDelegate;
import com.gft.unity.core.json.JSONException;
import com.gft.unity.core.json.JSONObject;
import com.gft.unity.core.json.JSONSerializer;
import com.gft.unity.core.json.JSONTokener;
import com.gft.unity.core.security.ISecurity;
import com.gft.unity.core.security.KeyPair;
import com.gft.unity.core.system.SystemLogger.Module;
import com.gft.unity.core.system.log.LogManager;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;

public class WidgetActivity extends Activity {
	
	static String SERVICE_KEY = "service";
	static String APPVERSE_SERVICE_KEY = "appverse_service";
	static String APPVERSE_METHOD_KEY = "appverse_method";
	static String APPVERSE_PARAMS_KEY = "appverse_params";
	static String APPVERSE_RESULT_KEY = "appverse_result";
	

	private static final String PARAM_PREFIX = "param";

	protected static final AndroidSystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();
	
	private Logger log = Logger.getInstance(LogCategory.GUI, "WIDGET");

	private AndroidServiceLocator androidServiceLocator;
	
	@Override
	protected void onDestroy() {
		LOG.LogDebug(Module.GUI, "WidgetActivity onDestroy");
		
		super.onDestroy();
	}
	
	

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		// create the application logger
		LogManager.setDelegate(new AndroidLoggerDelegate());
				
		LOG.LogDebug(Module.GUI, "WidgetActivity Logger: "+log.toString());
		LOG.LogDebug(Module.GUI, "WidgetActivity onCreate");
		
		
		androidServiceLocator.setContext(AppverseApplication.getAppContext(), null);
		LOG.LogDebug(Module.GUI, "[Widget] context setted to AppverseApplication.getAppContext()");
		
		androidServiceLocator = (AndroidServiceLocator) AndroidServiceLocator.GetInstance();

		

		LOG.LogDebug(Module.GUI, "[Widget] Set Context1");
		if(getIntent() != null){
			Bundle extras = getIntent().getExtras();
			LOG.LogDebug(Module.GUI, "[Widget] register widgetactivitymanager");
			androidServiceLocator.RegisterService(new WidgetActivityManager(extras),
					AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
			
			String appverseService = extras.getString(APPVERSE_SERVICE_KEY);
			String method = extras.getString(APPVERSE_METHOD_KEY);
			String params = extras.getString(APPVERSE_PARAMS_KEY);
			LOG.LogDebug(Module.GUI, "[Widget] "+APPVERSE_SERVICE_KEY+": "+appverseService);
			LOG.LogDebug(Module.GUI, "[Widget] "+APPVERSE_METHOD_KEY+": "+method);
			LOG.LogDebug(Module.GUI, "[Widget] "+APPVERSE_PARAMS_KEY+": "+params);
			

			if(appverseService != null && appverseService.equals(AndroidServiceLocator.SERVICE_TYPE_SECURITY)){


				
				
				if(method != null){
					ISecurity security = (ISecurity) androidServiceLocator.GetService(AndroidServiceLocator.SERVICE_TYPE_SECURITY);

					
					if(method.equals("GetStoredKeyValuePair")){
						LOG.LogDebug(Module.GUI, "Method GetStoredKeyValuePair");
						security.GetStoredKeyValuePair(fromJSONtoKeyPair(params));
					}else if(method.equals("GetStoredKeyValuePairs")){
					
						LOG.LogDebug(Module.GUI, "Method GetStoredKeyValuePairs");
						security.GetStoredKeyValuePairs(fromJSONtoKeyPairArray(params));
					
					}else if(method.equals("IsDeviceModified")){
						
						LOG.LogDebug(Module.GUI, "Method IsDeviceModified");
						boolean modified = security.IsDeviceModified();
						extras.putString(APPVERSE_RESULT_KEY, String.valueOf(modified));
						startService(extras);
						finishAffinity();
					}else if(method.equals("IsROMModified")){
					
						LOG.LogDebug(Module.GUI, "Method IsROMModified");
						boolean modified = security.IsROMModified();
						extras.putString(APPVERSE_RESULT_KEY, String.valueOf(modified));
						startService(extras);
						
					}else if(method.equals("RemoveStoredKeyValuePair")){
						
						LOG.LogDebug(Module.GUI, "Method RemoveStoredKeyValuePair");
						security.RemoveStoredKeyValuePair(fromJSONtoString(params));
					
					}else if(method.equals("RemoveStoredKeyValuePairs")){
					
						LOG.LogDebug(Module.GUI, "Method RemoveStoredKeyValuePairs");
						security.RemoveStoredKeyValuePairs(fromJSONtoStringArray(params));
						
					}else if(method.equals("StoreKeyValuePair")){
						
						LOG.LogDebug(Module.GUI, "Method StoreKeyValuePair");
						security.StoreKeyValuePair(fromJSONtoKeyPair(params));
					
					}else  if(method.equals("StoreKeyValuePairs")){
					
						LOG.LogDebug(Module.GUI, "Method StoreKeyValuePairs");
						security.StoreKeyValuePairs(fromJSONtoKeyPairArray(params));
					}
				}else{

					LOG.LogDebug(Module.GUI, "Method not supported YET");
				}
				
				
			}else{
				LOG.LogDebug(Module.GUI, "Service not supported YET");
			}
			
			
		}
		
		
		
		
	}
	
	private String fromJSONtoString(String params) {
		try {
			return (String) JSONSerializer.deserialize(String.class, (Object)params);
		} catch (Exception e) {
			LOG.LogDebug(Module.GUI, "Cast Exception: "+e.getMessage());
		}
		return null;
	}

	private String[] fromJSONtoStringArray(String params) {
		try {
			return (String[]) JSONSerializer.deserialize(String.class, (Object)params);
		} catch (Exception e) {
			LOG.LogDebug(Module.GUI, "Cast Exception: "+e.getMessage());
		}
		return null;
	}

	private KeyPair fromJSONtoKeyPair(String params){
		try {
			return (KeyPair) JSONSerializer.deserialize(KeyPair.class, toJSONObject(params));
		} catch (Exception e) {
			LOG.LogDebug(Module.GUI, "Cast Exception: "+e.getMessage());
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
			LOG.LogDebug(Module.GUI, "Parse query error"+ ex.getMessage());
		}
		
		return null;
		
	}
	
	
	private KeyPair[] fromJSONtoKeyPairArray(String params){
		try {
			return (KeyPair[]) JSONSerializer.deserialize(KeyPair[].class, toJSONObject(params));
		} catch (Exception e) {
			LOG.LogDebug(Module.GUI, "Cast Exception: "+e.getMessage());
		}
		return null;
	}
	
	private void startService(Bundle extras){
		try {
			Context ctx = AppverseApplication.getAppContext();
			Intent service = new Intent(ctx, Class.forName(extras.getString(SERVICE_KEY)));
			service.putExtras(extras);
			ctx.startService(service);
		} catch (ClassNotFoundException e) {
			LOG.LogDebug(Module.GUI, "Service ["+extras.getString(SERVICE_KEY)+"] not found");
		} catch(Exception e){
			LOG.LogDebug(Module.GUI, "Something went wrong... Check intent Extras");	
		}
	}
	
	protected void finishActivity(){
		this.finish();
	}
	
	public class WidgetActivityManager implements IActivityManager{
		
		private Bundle extras;
		
		public WidgetActivityManager(Bundle extra) {
			this.extras = extra;
		}

		@Override
		public boolean resolveActivity(Intent intent) {
			// TODO Auto-generated method stub
			return false;
		}

		@Override
		public boolean startActivity(Intent intent) {
			// TODO Auto-generated method stub
			return false;
		}

		@Override
		public void launchApp(Intent intent) {
			// TODO Auto-generated method stub
			
		}

		@Override
		public boolean startActivityForResult(Intent intent, int requestCode, IActivityManagerListener listener) {
			// TODO Auto-generated method stub
			return false;
		}

		@Override
		public boolean publishActivityResult(int requestCode, int resultCode, Intent data) {
			// TODO Auto-generated method stub
			return false;
		}

		@Override
		public void executeJS(String method, Object data) {
			extras.putString(APPVERSE_RESULT_KEY, JSONSerializer.serialize(data));
			startService(extras);
			
		}

		@Override
		public void executeJS(String method, Object[] dataArray) {
			extras.putString(APPVERSE_RESULT_KEY, JSONSerializer.serialize(dataArray));
			startService(extras);
			
			finishActivity();
			
			
		}

		@Override
		public void executeJS(String json) {
			// TODO Auto-generated method stub
			
		}

		@Override
		public boolean startShowSplashScreen() {
			// TODO Auto-generated method stub
			return false;
		}

		@Override
		public boolean startDismissSplashScreen() {
			// TODO Auto-generated method stub
			return false;
		}

		@Override
		public void killBackgroundProcesses() {
			// TODO Auto-generated method stub
			
		}

		@Override
		public void dismissApplication() {
			// TODO Auto-generated method stub
			
		}

		@Override
		public void loadUrlIntoWebView(String url) {
			// TODO Auto-generated method stub
			
		}

		@Override
		public boolean publishPermissionResult(int requestCode, String[] permissions, int[] grantResults) {
			// TODO Auto-generated method stub
			return false;
		}

		@Override
		public boolean requestPermision(String permission, int requestCode, IActivityManagerListener listener) {
			// TODO Auto-generated method stub
			return false;
		}

		@Override
		public boolean requestPermision(String[] permissions, int requestCode, IActivityManagerListener listener) {
			// TODO Auto-generated method stub
			return false;
		}

	}
		
}
