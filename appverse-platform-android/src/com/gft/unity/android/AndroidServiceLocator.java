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

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Semaphore;

import android.content.Context;
import android.os.Build;

import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.server.net.ServerSocketEndPoint;
import com.gft.unity.core.system.service.AbstractServiceLocator;
import com.gft.unity.core.system.service.IServiceLocator;

public class AndroidServiceLocator extends AbstractServiceLocator {

	private static final AndroidSystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();
	
	public static final String INTERNAL_SERVER_HOST = "127.0.0.1";
	public static final String INTERNAL_SERVER_PORT = "8080";
	public static final String INTERNAL_SERVER_URL = "http://" + INTERNAL_SERVER_HOST + ":" + INTERNAL_SERVER_PORT;
	
	public static final String SERVICE_ANDROID_ASSET_MANAGER = "android.asset";
	public static final String SERVICE_ANDROID_ACTIVITY_MANAGER = "android.activity";

	private static Context context;
	private static final Semaphore SEMAPHORE = new Semaphore(1);
	
	private static String IN_DEBUG_MODE = "$debuggable$";
	
	private static List<String> managedServices = new ArrayList<String>();
			
	private AndroidServiceLocator() {
		super();
		registerServices();
	}
	
	public static void setContext(Context context) {
		AndroidServiceLocator.context = context;
	}

	public static Context getContext() {
		return context;
	}
	
	public static IServiceLocator GetInstance() {

		if (singletonServiceLocator == null) {
			singletonServiceLocator = new AndroidServiceLocator();
		}
		
		return singletonServiceLocator;
	}

	private void registerServices() {
		this.RegisterService(new AndroidDatabase(),
				AndroidServiceLocator.SERVICE_TYPE_DATABASE);
		this.RegisterService(new AndroidFileSystem(),
				AndroidServiceLocator.SERVICE_TYPE_FILESYSTEM);
		this.RegisterService(new AndroidGeo(),
				AndroidServiceLocator.SERVICE_TYPE_GEO);
		this.RegisterService(new AndroidI18N(),
				AndroidServiceLocator.SERVICE_TYPE_I18N);
		this.RegisterService(new AndroidIO(),
				AndroidServiceLocator.SERVICE_TYPE_IO);
		this.RegisterService(new AndroidMedia(),
				AndroidServiceLocator.SERVICE_TYPE_MEDIA);
		this.RegisterService(new AndroidMessaging(),
				AndroidServiceLocator.SERVICE_TYPE_MESSAGING);
		this.RegisterService(new AndroidNet(),
				AndroidServiceLocator.SERVICE_TYPE_NET);
		this.RegisterService(new AndroidNotification(),
				AndroidServiceLocator.SERVICE_TYPE_NOTIFICATION);
		this.RegisterService(new AndroidShare(),
				AndroidServiceLocator.SERVICE_TYPE_SHARE);
		this.RegisterService(new AndroidSystem(),
				AndroidServiceLocator.SERVICE_TYPE_SYSTEM);
		this.RegisterService(new AndroidTelephony(),
				AndroidServiceLocator.SERVICE_TYPE_TELEPHONY);
		this.RegisterService(new AndroidLog(),
				AndroidServiceLocator.SERVICE_TYPE_LOG);
		this.RegisterService(new AndroidPim(),
				AndroidServiceLocator.SERVICE_TYPE_PIM);
		this.RegisterService(new AndroidAnalytics(),
				AndroidServiceLocator.SERVICE_TYPE_ANALYTICS);
		this.RegisterService(new AndroidSecurity(),
				AndroidServiceLocator.SERVICE_TYPE_SECURITY);
		this.RegisterService(new AndroidWebtrekk(),
				AndroidServiceLocator.SERVICE_TYPE_WEBTREKK);
		this.RegisterService(new AndroidAppLoader(),
				AndroidServiceLocator.SERVICE_TYPE_APPLOADER);		
		if(Build.VERSION.SDK_INT>=18){
			this.RegisterService(new AndroidBeacon(),
				AndroidServiceLocator.SERVICE_TYPE_BEACON);
		}else {
			this.RegisterService(new AndroidNotSupportedAPI(),
					AndroidServiceLocator.SERVICE_TYPE_BEACON);
			
		}
	}
	
    public static boolean isDebuggable() {
    	
    	boolean isDebuggable = false;
    	try {
    		isDebuggable = Boolean.parseBoolean(IN_DEBUG_MODE);
    	} catch (Exception ex) {
    		LOG.Log(SystemLogger.Module.PLATFORM, "Exception while checking is app is in debug mode. Message: " + ex.getMessage());	
    	}
    	
    	return isDebuggable;
    }
    
    public static void checkResourceIsManagedService(String url) {
    	if(url!=null && url.indexOf(AndroidServiceLocator.INTERNAL_SERVER_URL)>-1 
				&& (url.indexOf("/service/")>-1 || url.indexOf("/service-async/")>-1) ) {
			//LOG.LogDebug(SystemLogger.Module.PLATFORM, "Handle managed service: " + url);
			AndroidServiceLocator.registerManagedService(url, ""+System.currentTimeMillis());
		}
    }
    
    public static void registerManagedService(String service, String timestamp) {
    	long uid = System.currentTimeMillis();
    	LOG.LogDebug(SystemLogger.Module.PLATFORM, "[" +uid+ "] registerManagedService Acquiring semaphore "+ System.currentTimeMillis());	
    	SEMAPHORE.acquireUninterruptibly();
    	try {	    	
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "Registered managedServices Size b4:" + managedServices.size());
        	managedServices.add(service + "_" + timestamp);
        	LOG.LogDebug(SystemLogger.Module.PLATFORM, "Registered managedServices Size after:" + managedServices.size());
	    	
    	} catch (Throwable th) {
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "*************** Throwable exception #registerManagedService: " + th.getMessage());
    		//th.printStackTrace();
    	}finally{
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "[" +uid+ "] registerManagedService Releasing semaphore "+ System.currentTimeMillis());
    		SEMAPHORE.release();
    	}
    }
    
    
    public static boolean consumeManagedService(String service) {
    	
    	if(Build.VERSION.SDK_INT < 11){ 
    		// "onLoadResource" in UnityWebViewClient is not 100% guaranteed to be called prior to serving the service
    		// so, the service sometimes is called before registered (from the webviewclient method).
    		// "shouldInterceptRequest" is a better method to be used, but unfortunately is not available for 2.3.6 versions.
    		try {
    			// In order to prevent this, we will wait for those devices some milliseconds to "give the WebViewClient time" to process
    			// It is not an elegant solution, but it is the only we can do to provide security in those devices
				Thread.sleep(200);
			} catch (InterruptedException e) {
				LOG.LogDebug("*** Warning checking consumed managed services");
			}  
    	}
    	
    	long uid = System.currentTimeMillis();
    	LOG.LogDebug(SystemLogger.Module.PLATFORM, "[" +uid+ "] consumeManagedService Acquiring semaphore "+ System.currentTimeMillis());
    	SEMAPHORE.acquireUninterruptibly();
    	boolean result = false;
    	try {	    	
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "consumeManagedService managedServices Size b4:" + managedServices.size());
    		int index = -1;
			for(String  managedService: managedServices) {
				if(managedService.startsWith(AndroidServiceLocator.INTERNAL_SERVER_URL + service)) {
					//LOG.LogDebug(SystemLogger.Module.PLATFORM, "Consuming managed service...");
					index = managedServices.indexOf(managedService);
					break;
				}
			}
			if(index>-1) {				
				String removedService = managedServices.remove(index);
	        
				removedService = null;
				System.gc();
				
				result = true;
			}
			
    	} catch (Throwable th) {
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "*************** Throwable exception #consumeManagedService: " + th.getMessage());
    		//th.printStackTrace();
    	}finally{
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "[" +uid+ "] consumeManagedService Releasing semaphore "+ System.currentTimeMillis());
    		SEMAPHORE.release();
    	}
    	LOG.LogDebug(SystemLogger.Module.PLATFORM, "consumeManagedService managedServices Size after:" + managedServices.size());		
    	return result;
    }
    
    public static boolean isSocketListening() {
    	return ServerSocketEndPoint.isSocketListening();
    }

}
