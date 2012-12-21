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
package com.gft.unity.android.server;

import java.lang.reflect.Constructor;
import java.lang.reflect.Field;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

import android.content.Context;
import android.os.Build;
import android.util.Log;
import android.webkit.WebView;

/**
 * Utility class for setting WebKit proxy used by Android WebView
 * 
 */
public class ProxySettings {

	private static final String TAG = "Appverse.ProxySettings";
	
	public static boolean shouldSetProxySetting = false;
	
	static {
		if(checkSystemProxyProperties()) {
			shouldSetProxySetting = true;
		}
	}
	
	/** API LEVEL > 14 **/
	private static Method m = null;
	private static Constructor c =  null;
	
	static final int PROXY_CHANGED = 193;

	private static Object getDeclaredField(Object obj, String name) throws SecurityException,
			NoSuchFieldException, IllegalArgumentException, IllegalAccessException {
		Field f = obj.getClass().getDeclaredField(name);
		f.setAccessible(true);
		Object out = f.get(obj);
		// System.out.println(obj.getClass().getName() + "." + name + " = "+
		// out);
		return out;
	}
	
	private static Object getFieldValueSafely(Field field, Object classInstance) throws IllegalArgumentException, IllegalAccessException {
	    boolean oldAccessibleValue = field.isAccessible();
	    field.setAccessible(true);
	    Object result = field.get(classInstance);
	    field.setAccessible(oldAccessibleValue);
	    return result;      
	}

	public static Object getRequestQueue(Context ctx) throws Exception {
		Object ret = null;
		Class networkClass = Class.forName("android.webkit.Network");
		if (networkClass != null) {
			
			// this code works for api level < 11
			Object networkObj = invokeMethod(networkClass, "getInstance", new Object[] { ctx },
					Context.class);
			if (networkObj != null) {
				ret = getDeclaredField(networkObj, "mRequestQueue");
			}
		}
		return ret;
	}

	private static Object invokeMethod(Object object, String methodName, Object[] params,
			Class... types) throws Exception {
		Object out = null;
		Class c = object instanceof Class ? (Class) object : object.getClass();
		if (types != null) {
			Method method = c.getMethod(methodName, types);
			out = method.invoke(object, params);
		} else {
			Method method = c.getMethod(methodName);
			out = method.invoke(object);
		}
		// System.out.println(object.getClass().getName() + "." + methodName +
		// "() = "+ out);
		return out;
	}

	public static void resetProxy(Context ctx) throws Exception {
		Object requestQueueObject = getRequestQueue(ctx);
		if (requestQueueObject != null) {
			setDeclaredField(requestQueueObject, "mProxyHost", null);
		}
	}

	private static void setDeclaredField(Object obj, String name, Object value)
			throws SecurityException, NoSuchFieldException, IllegalArgumentException,
			IllegalAccessException {
		Field f = obj.getClass().getDeclaredField(name);
		f.setAccessible(true);
		f.set(obj, value);
	}
	
	/**
	 * Override WebKit Proxy settings
	 * 
	 * @param ctx
	 *            Android ApplicationContext
	 * @param host
	 * @param port
	 * @return true if Proxy was successfully set
	 */
	public static boolean setProxy(Context ctx, WebView view, String host, int port) {
		boolean ret = false;
		
		//if(shouldSetProxySetting) {	
			setSystemProperties(host, port);
	
			Log.d(TAG, "Build.VERSION.SDK_INT: " + Build.VERSION.SDK_INT);
			try {
				if (Build.VERSION.SDK_INT < 11) {
					
					Log.d(TAG, "Setting GB Proxy...");
					ret = setGBProxy(ctx, host, port);
	
				} else if (Build.VERSION.SDK_INT < 16) {
					
					Log.d(TAG, "Setting HC / ICS Proxy...");
					ret = setHCICSProxy(view, host, port);
					
				} else {
					
					Log.d(TAG, "Setting JB Proxy...");
					ret = setJBProxy(view, host, port);
				}
			} catch (Exception e) {
				Log.e(TAG, "error setting up webkit proxying", e);
			}
		//} else {
			//Log.d(TAG, "Proxy settings are not applied as we are not in 3g connectivity and/or proxy setting is not already set");
		//}
		
		return ret;
	}
	
	private static boolean setGBProxy(Context ctx, String host, int port) throws Exception {
		Object requestQueueObject = getRequestQueue(ctx);
		if (requestQueueObject != null) {
			// Create Proxy config object and set it into request Q
			//HttpHost httpHost = new HttpHost(host, port, "http");

			setDeclaredField(requestQueueObject, "mProxyHost", null);
			
			return true;
		} else {
			return false;
		}
		
	}
	
	private static boolean setHCICSProxy(WebView view, String host, int port) {
		
		try
		{
		  Class jwcjb = Class.forName("android.webkit.JWebCoreJavaBridge");
		  Class params[] = new Class[1];
		  params[0] = Class.forName("android.net.ProxyProperties");
		  Method updateProxyInstance = jwcjb.getDeclaredMethod("updateProxy", params);

		  Class wv = Class.forName("android.webkit.WebView");
		  Field mWebViewCoreField = wv.getDeclaredField("mWebViewCore");
		  Object mWebViewCoreFieldIntance = getFieldValueSafely(mWebViewCoreField, view);

		  Class wvc = Class.forName("android.webkit.WebViewCore");
		  Field mBrowserFrameField = wvc.getDeclaredField("mBrowserFrame");
		  Object mBrowserFrame = getFieldValueSafely(mBrowserFrameField, mWebViewCoreFieldIntance);

		  Class bf = Class.forName("android.webkit.BrowserFrame");
		  Field sJavaBridgeField = bf.getDeclaredField("sJavaBridge");
		  Object sJavaBridge = getFieldValueSafely(sJavaBridgeField, mBrowserFrame);

		  Class ppclass = Class.forName("android.net.ProxyProperties");
		  Class pparams[] = new Class[3];
		  pparams[0] = String.class;
		  pparams[1] = int.class;
		  pparams[2] = String.class;
		  Constructor ppcont = ppclass.getConstructor(pparams);
		  
		  String proxyHost = System.getProperty("http.proxyHost", "");
		  String proxyPort = System.getProperty("http.proxyPort","0");
		  
		  updateProxyInstance.invoke(sJavaBridge, ppcont.newInstance(proxyHost, Integer.parseInt(proxyPort), "127.0.0.1"));
		  //Log.d(TAG, "Proxy host is set to: " + proxyHost + ":" + proxyPort);
		  return true;
		}
		catch (Exception ex)
		{    
			Log.d(TAG, "Exception setHCProxy", ex);
			return false;
		}
	}
	
	private static boolean setJBProxy(WebView view, String host, int port) throws ClassNotFoundException,
			NoSuchMethodException, IllegalArgumentException, InstantiationException,
			IllegalAccessException, InvocationTargetException {
		
		/*
		
		
		Class webViewCoreClass = Class.forName("android.webkit.WebViewCore");
		Class proxyPropertiesClass = Class.forName("android.net.ProxyProperties");
		if (webViewCoreClass != null && proxyPropertiesClass != null) {
			if (m==null || c==null) {
				m = webViewCoreClass.getDeclaredMethod("sendStaticMessage", Integer.TYPE, Object.class);
				m.setAccessible(true);
				c = proxyPropertiesClass.getConstructor(String.class, Integer.TYPE,String.class);
				c.setAccessible(true);
			}
			String proxyHost = System.getProperty("http.proxyHost");
			String proxyPort = System.getProperty("http.proxyPort","0");
			  
			Object properties = c.newInstance(proxyHost, Integer.parseInt(proxyPort), "127.0.0.1");
			m.invoke(null, PROXY_CHANGED, properties);
			return true;
		}
		return false;
		
		*/
		
		try
		{
		  Class jwcjb = Class.forName("android.webkit.JWebCoreJavaBridge");
		  Class params[] = new Class[1];
		  params[0] = Class.forName("android.net.ProxyProperties");
		  Method updateProxyInstance = jwcjb.getDeclaredMethod("updateProxy", params);

		  Class wv = Class.forName("android.webkit.WebView");
		  Field mProviderField = wv.getDeclaredField("mProvider");
		  Object mProviderFieldIntance = getFieldValueSafely(mProviderField, view);
		  Class provider = Class.forName("android.webkit.WebViewClassic");
		  
		  Field mWebViewCoreField = provider.getDeclaredField("mWebViewCore");
		  Object mWebViewCoreFieldIntance = getFieldValueSafely(mWebViewCoreField, mProviderFieldIntance);

		  Class wvc = Class.forName("android.webkit.WebViewCore");
		  Field mBrowserFrameField = wvc.getDeclaredField("mBrowserFrame");
		  Object mBrowserFrame = getFieldValueSafely(mBrowserFrameField, mWebViewCoreFieldIntance);

		  Class bf = Class.forName("android.webkit.BrowserFrame");
		  Field sJavaBridgeField = bf.getDeclaredField("sJavaBridge");
		  Object sJavaBridge = getFieldValueSafely(sJavaBridgeField, mBrowserFrame);

		  Class ppclass = Class.forName("android.net.ProxyProperties");
		  Class pparams[] = new Class[3];
		  pparams[0] = String.class;
		  pparams[1] = int.class;
		  pparams[2] = String.class;
		  Constructor ppcont = ppclass.getConstructor(pparams);
		  
		  String proxyHost = System.getProperty("http.proxyHost","");
		  String proxyPort = System.getProperty("http.proxyPort","0");
		  
		  updateProxyInstance.invoke(sJavaBridge, ppcont.newInstance(proxyHost, Integer.parseInt(proxyPort), "127.0.0.1"));
		  //Log.d(TAG, "Proxy host is set to: " + proxyHost + ":" + proxyPort);
		  return true;
		}
		catch (Exception ex)
		{    
			Log.d(TAG, "Exception setJBProxy", ex);
			return false;
		}

	}
	
	public static boolean checkSystemProxyProperties() {

		//System.getProperties().list(System.out);
		
		if (Build.VERSION.SDK_INT < 11) {
			// Proxy system properties are not set for device under api level 11
			return true;
		} else {
			String proxyHost = System.getProperty("http.proxyHost");
			String httpsProxyHost = System.getProperty("https.proxyHost");
			
			if((proxyHost!=null && proxyHost.length()>0) || (httpsProxyHost!=null && httpsProxyHost.length()>0)) {
				Log.d(TAG, "Proxy host is defined by the system");
				return true;
			}
			Log.d(TAG, "Proxy host is not set by the system");
			return false;
		}
	}

	private static void setSystemProperties(String host, int port) {
		/*
		System.setProperty("http.proxyHost", host);
		System.setProperty("http.proxyPort", port + "");
		
		System.setProperty("https.proxyHost", System.getProperty("http.proxyHost"));
		System.setProperty("https.proxyPort", System.getProperty("http.proxyPort"));
		
		System.setProperty("http.nonProxyHosts", "127.0.0.1");
		System.setProperty("https.nonProxyHosts", "127.0.0.1");*/
	}
}
