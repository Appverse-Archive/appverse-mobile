package com.gft.unity.android.server.handler;

import com.gft.unity.android.AndroidInvocationManager;
import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.android.activity.AndroidActivityManager;
import com.gft.unity.core.system.SystemLogger.Module;

import android.webkit.JavascriptInterface;

public class AndroidJavaScriptServiceInterface {

	private final static AndroidSystemLogger LOG = AndroidSystemLogger
			.getSuperClassInstance();
	
	private static String APPVERSE_SERVICE_URI = AndroidServiceLocator.APPVERSE_URI + "service/";
	
	private AndroidServiceLocator serviceLocator;
	private ServiceHandler serviceHandler;
	private AndroidActivityManager activityManager;
	
	
	public AndroidJavaScriptServiceInterface(AndroidServiceLocator _serviceLocator, AndroidActivityManager _activityManager) {
		super();
		this.serviceLocator = _serviceLocator;
		this.serviceHandler = new ServiceHandler();
		this.activityManager = _activityManager;
	}


	@JavascriptInterface
	public void postMessage (String uri, String query) {
		try {
			//LOG.LogDebug(Module.PLATFORM, "# AndroidJavaScriptServiceInterface - Received post Message: " + uri);
			
			String serviceName = "undefined";
			String methodName = "undefined";
			Object service = null;
			
			// get service path
			if (uri != null && uri.startsWith(AndroidJavaScriptServiceInterface.APPVERSE_SERVICE_URI)) {
				String commandParams = uri.substring (AndroidJavaScriptServiceInterface.APPVERSE_SERVICE_URI.length());
				//LOG.LogDebug(Module.PLATFORM, "# AndroidJavaScriptServiceInterface - commandParams: " + commandParams);
				
				String[] commandParamsArray = commandParams.split ("/");
				serviceName = commandParamsArray [0];
				methodName = commandParamsArray [1];
				LOG.LogDebug(Module.PLATFORM, "# AndroidJavaScriptServiceInterface - Received post Message: serviceName: " + serviceName + ", methodName: " + methodName);
				
				service = serviceLocator.GetService (serviceName);
			
			} else {
				LOG.LogDebug(Module.PLATFORM, "# AndroidJavaScriptServiceInterface - no valid URI received: " + uri);
			}
			
			if(query == null) {
				
			}
			
			LOG.LogDebug(Module.PLATFORM, "# AndroidJavaScriptServiceInterface - sending Async POST result for service: " + serviceName + ", and method: " + methodName);
			
			AndroidInvocationManager aim = (AndroidInvocationManager) AndroidInvocationManager.getInstance();
			serviceHandler.processAsyncPOSTResult(aim, activityManager, service, methodName, query);
			
		} catch (Exception e) {
			LOG.LogDebug(Module.PLATFORM, 
					"# AndroidJavaScriptServiceInterface - Unhandled exception processing API service message. Exception: " + e.getMessage());
		}
	}
	
}
