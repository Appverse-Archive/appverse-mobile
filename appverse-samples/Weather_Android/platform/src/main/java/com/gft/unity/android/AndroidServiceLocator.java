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

import java.io.BufferedInputStream;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.URI;
import java.net.URISyntaxException;
import java.net.URLDecoder;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.Semaphore;

import android.content.Context;
import android.content.res.AssetManager;
import android.os.Build;
import android.os.ResultReceiver;
import android.webkit.WebResourceResponse;

import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.android.helpers.AndroidUtils;
import com.gft.unity.android.server.handler.AssetHandler;
import com.gft.unity.core.IAppDelegate;
import com.gft.unity.core.storage.filesystem.FileData;
import com.gft.unity.core.storage.filesystem.IFileSystem;
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.server.net.ServerSocketEndPoint;
import com.gft.unity.core.system.service.AbstractServiceLocator;
import com.gft.unity.core.system.service.IServiceLocator;

public class AndroidServiceLocator extends AbstractServiceLocator {

	private static final AndroidSystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();
	
	// legacy internal server
	public static final String INTERNAL_SERVER_HOST = "127.0.0.1";
	public static final String INTERNAL_SERVER_PORT = "8080";
	public static final String INTERNAL_SERVER_URL = "http://" + INTERNAL_SERVER_HOST + ":" + INTERNAL_SERVER_PORT;
	
	// new Appverse URI
	public static final String APPVERSE_URI = "https://appverse/";
	
	public static final String SERVICE_ANDROID_ASSET_MANAGER = "android.asset";
	public static final String SERVICE_ANDROID_ACTIVITY_MANAGER = "android.activity";

	private static Context context;
	//NOT ANYMORE NEEDED private static WebView webView;
	private static IActivityManager activityManager;
	
	private static final Semaphore SEMAPHORE = new Semaphore(1);
	
	private static String IN_DEBUG_MODE = "$debuggable$";
	
	private static List<String> managedServices = new ArrayList<String>();
	
	public String inUseURI = null;
	
	private Map<Integer, ResultReceiver> resultReceivers;
			
	private AndroidServiceLocator() {
		super();
		resultReceivers = new HashMap<Integer, ResultReceiver>();
		registerServices();
		
		if(Build.VERSION.SDK_INT >= 17){
			this.inUseURI = APPVERSE_URI.substring(0, APPVERSE_URI.length() - 1); // remove trailing slash
		} else {
			this.inUseURI = INTERNAL_SERVER_URL;
		}
	}
	
	public static void setContext(Context context, IActivityManager am) {
		AndroidServiceLocator.context = context;
		AndroidServiceLocator.activityManager = am;
	}
	
	/*
	public static void setContext(Context context, WebView appView) {
		AndroidServiceLocator.context = context;
		AndroidServiceLocator.webView = appView;
	}
	*/
	
	public static Context getContext() {
		return context;
	}
	
	/*
	public static WebView getApplicationWebView() {
		return webView;
	}
	*/
	
	public static IActivityManager getActivityManager() {
		return activityManager;
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
		this.RegisterService(new AndroidSecurity(),
				AndroidServiceLocator.SERVICE_TYPE_SECURITY);
		this.RegisterService(new AndroidAppLoader(),
				AndroidServiceLocator.SERVICE_TYPE_APPLOADER);
		
		/*/ TODO :: create implementation classes for modules
		this.RegisterService(new AndroidNotification(),
				AndroidServiceLocator.SERVICE_TYPE_PUSH);*/
		
	}
	
	public void initConfigDataForServices() {
		IAppDelegate[] appDelegates = this.getAppDelegateServices();
		for (IAppDelegate iAppDelegate : appDelegates) {
			try {
				String configDataFile = iAppDelegate.getConfigFilePath();
				if(configDataFile!=null) {
					LOG.LogDebug(SystemLogger.Module.PLATFORM, "Found IAppDelegates to load config data at [" + configDataFile +"]");
					InputStream configData = AndroidUtils.getInstance().getAssetInputStream(context.getAssets(), configDataFile);
					iAppDelegate.setConfigFileLoadedData(configData);
				}
			} catch(Exception ex) {
				LOG.LogDebug(SystemLogger.Module.PLATFORM, "Error getting config data for module. Exception message: " + ex.getMessage());
			}
			
		} 
	}
	
	public void RegisterResultReceiver(int[] resultCodes, ResultReceiver resultReceiver) {
		for (int i : resultCodes) {
			resultReceivers.put(i, resultReceiver);
		}
		
	}
	
	public ResultReceiver getResultReceiver(int resultCode) {
		return resultReceivers.get(resultCode);
	}
	
	
	public void sendApplicationEvent(AndroidApplicationEvent event) {
		
		try {
			IAppDelegate[] appDelegates = this.getAppDelegateServices();
			LOG.LogDebug(SystemLogger.Module.PLATFORM, "Found IAppDelegates to send event [" + event.toString() +"]: #" + appDelegates.length); 
			for (IAppDelegate iAppDelegate : appDelegates) {
				switch (event) {
				case onCreate:
					iAppDelegate.onCreate();
					break;
				case onPause:
					iAppDelegate.onPause();
					break;
				case onResume:
					iAppDelegate.onResume();
					break;	
				case onStop:
					iAppDelegate.onStop();
					break;
				case onDestroy:
					iAppDelegate.onDestroy();
					break;
				default:
					break;	
				}
			}
		} catch (Exception e) {
			LOG.LogDebug(SystemLogger.Module.PLATFORM, 
					"*** catched exception sending application event to the modules... event type: "  + event.toString() + ", exception message: " + e.getMessage());
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
    
    /**
     * LEGACY code: used by UnityWebviewClient  (for older Androird SDK versions)
     * @param url
     */
    public static void checkResourceIsManagedService(String url) {
    	if(url!=null && url.indexOf(AndroidServiceLocator.INTERNAL_SERVER_URL)>-1 
				&& (url.indexOf("/service/")>-1 ) ) {
			//LOG.LogDebug(SystemLogger.Module.PLATFORM, "Handle managed service: " + url);
			AndroidServiceLocator.registerManagedService(url, ""+System.currentTimeMillis());
		}
    }
    
    /**
     * NEW CODE: used by the new AppverseWebviewClient. Only resources are managed
     * @param url
     * @return
     */
    public static WebResourceResponse checkManagedResource(String url) {
    	if(url!=null && url.indexOf(AndroidServiceLocator.APPVERSE_URI)>-1 
				&& url.indexOf(AssetHandler.ASSET_PATH)>-1 ) {
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "Handle managed resource: " + url);
    		
    		String assetPath = url.substring(AndroidServiceLocator.APPVERSE_URI.length());
    		try {
    			URI assetUrl = new URI(url);
    			assetPath = assetUrl.getPath();  // removing query parameters
    			assetPath = assetPath.substring(1);  // removing leading slash
			} catch (URISyntaxException e) {
				LOG.LogDebug(SystemLogger.Module.PLATFORM, "Malformat URL syntax (web resource). Exception message: " + e.getMessage());
			}
    		
    		byte[] assetData = AndroidServiceLocator.handleAssetResource(assetPath);
    		String mimeType = AndroidServiceLocator.getMimeType(assetPath);
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "Handle managed resource - mimeType: " + mimeType);
    		if(assetData!=null)
    			return new WebResourceResponse(mimeType, "utf-8", new ByteArrayInputStream(assetData));
    		else
    			LOG.LogDebug(SystemLogger.Module.PLATFORM, "Exception getting resource data bytes for url: " + url +". No resource found");
    		
    	} else  if(url!=null && url.indexOf(AndroidServiceLocator.APPVERSE_URI)>-1 
				&& url.indexOf(AssetHandler.DOCUMENT_PATH)>-1 ) {
    		
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "Handle managed document resource: " + url);
    		
    		String docPath = url.substring(AndroidServiceLocator.APPVERSE_URI.length());
    		try {
    			URI assetDocUrl = new URI(url);
    			docPath = assetDocUrl.getPath();  // removing query parameters, keeping leading slash
			} catch (URISyntaxException e) {
				LOG.LogDebug(SystemLogger.Module.PLATFORM, "Malformat URL syntax (document resource). Exception message: " + e.getMessage());
			}
    		
    		byte[] docData = AndroidServiceLocator.handleDocument(docPath);
    		String mimeType = AndroidServiceLocator.getMimeType(docPath);
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "Handle managed document resource - mimeType: " + mimeType);
    		if(docData!=null)
    			return new WebResourceResponse(mimeType, "utf-8", new ByteArrayInputStream(docData));
    		else
    			LOG.LogDebug(SystemLogger.Module.PLATFORM, "Exception getting document data bytes for url: " + url +". No document found");
    	}
    	
    	// do not handled the resource (it is not managed)
    	return null;
    }
    
    private static byte[] handleAssetResource(String assetPath) {
		InputStream is = null;
		BufferedInputStream bis = null;
		ByteArrayOutputStream baos = null;
		try {
			is = AndroidUtils.getInstance().getAssetInputStream(getAssetManager(), assetPath);
			bis = new BufferedInputStream(is, 2048);
			baos = new ByteArrayOutputStream();
			byte[] buf = new byte[512];
			int length = 0;
			while ((length = bis.read(buf)) != -1) {
				baos.write(buf, 0, length);
			}
		} catch (IOException ex) {
			LOG.Log(SystemLogger.Module.PLATFORM, "Error loading managed asset", ex);
			return null;
		} finally {
			if (bis != null) {
				try {
					bis.close();
				} catch (Exception ex) {
				}
			}
			if (is != null) {
				try {
					is.close();
				} catch (Exception ex) {
				}
			}
			if (baos != null) {
				try {
					baos.close();
				} catch (Exception ex) {
				}
			}
		}

		return baos.toByteArray();
	}
    
    private static byte[] handleDocument(String documentPath) {

		// normalize the document path
		documentPath = normalizeDocumentPath(documentPath);
		
		// read document/file
		FileData fd = new FileData();
		fd.setFullName(documentPath);
		return getFileSystemService().ReadFile(fd);
	}

    private static AssetManager getAssetManager() {
		return (AssetManager)  AndroidServiceLocator.GetInstance().GetService(AndroidServiceLocator.SERVICE_ANDROID_ASSET_MANAGER);
	}
    
    private static IFileSystem getFileSystemService() {
		return (IFileSystem)  AndroidServiceLocator.GetInstance().GetService(AndroidServiceLocator.SERVICE_TYPE_FILESYSTEM);
	}
    
    public static String getMimeType(String filename) {
        int index = filename.lastIndexOf(".");
        String mimeType = null;
        if (index > 0) {
            mimeType = com.gft.unity.core.system.server.HttpServer.SERVER_CONFIG.getProperty("mime"
                    + filename.substring(index).toLowerCase());
        }

        return mimeType;
    }
    
    private static String normalizeDocumentPath(String url) {

    	if(url == null) return url;
    	
		String documentPath = URLDecoder.decode(url);
		return documentPath.substring(AssetHandler.DOCUMENT_PATH.length());
	}

    
    /**
     * LEGACY code: used by UnityWebviewClient  (for older Androird SDK versions)
     */
    public static void registerManagedService(String service, String timestamp) {
    	long uid = System.currentTimeMillis();
    	LOG.LogDebug(SystemLogger.Module.PLATFORM, "[" +uid+ "] registerManagedService Acquiring semaphore "+ System.currentTimeMillis());	
    	SEMAPHORE.acquireUninterruptibly();
    	try {	    	
    		//LOG.LogDebug(SystemLogger.Module.PLATFORM, "Registered managedServices Size b4:" + managedServices.size());
        	managedServices.add(service + "_" + timestamp);
        	//LOG.LogDebug(SystemLogger.Module.PLATFORM, "Registered managedServices Size after:" + managedServices.size());
	    	
    	} catch (Throwable th) {
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "*************** Throwable exception #registerManagedService: " + th.getMessage());
    		//th.printStackTrace();
    	}finally{
    		LOG.LogDebug(SystemLogger.Module.PLATFORM, "[" +uid+ "] registerManagedService Releasing semaphore "+ System.currentTimeMillis());
    		SEMAPHORE.release();
    	}
    }
    
    /**
     * LEGACY code: used by UnityWebviewClient  (for older Androird SDK versions)
     */
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
