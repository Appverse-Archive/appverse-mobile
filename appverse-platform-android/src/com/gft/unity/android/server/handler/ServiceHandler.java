/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://www.appverse.mobi/licenses/apl_v2.0.pdf.

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
package com.gft.unity.android.server.handler;

import java.io.ByteArrayInputStream;
import java.io.IOException;

import com.gft.unity.android.AndroidInvocationManager;
import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.activity.AndroidActivityManager;
import com.gft.unity.core.system.server.net.HttpRequest;
import com.gft.unity.core.system.server.net.HttpResponse;
import com.gft.unity.core.system.server.net.Request;
import com.gft.unity.core.system.server.net.Response;
import com.gft.unity.core.system.server.net.Server;

public class ServiceHandler extends AndroidHandler {

	private static final String SERVICE_PREFIX = "/service/";
	private static final String SERVICE_ASYNC_PREFIX = "/service-async/";
	private static final String PARAM_PREFIX = "json=";

	private static final String HEADER_CACHE_NAME = "Cache-Control";
	private static final String HEADER_CACHE_DEFAULT_VALUE = "no-cache";
	
	public ServiceHandler() {
	}

	@Override
	public boolean initialize(String handlerName, Server server) {
		super.initialize(handlerName, server);

		Log("Initialized.");

		return true;
	}

	@Override
	public boolean handle(Request aRequest, Response aResponse)
			throws IOException {

		if (aRequest instanceof HttpRequest) {
			HttpRequest request = (HttpRequest) aRequest;
			HttpResponse response = (HttpResponse) aResponse;
			Log(request.getMethod() + " " + request.getUrl());

			if (request.getUrl().startsWith(SERVICE_PREFIX) || request.getUrl().startsWith (SERVICE_ASYNC_PREFIX)) {

				boolean asyncmode = false;
				int serviceUriLength = SERVICE_PREFIX.length();
				if(request.getUrl().startsWith (SERVICE_ASYNC_PREFIX)) {
					asyncmode = true;
					serviceUriLength = SERVICE_ASYNC_PREFIX.length();
				}

				String[] commands = request.getUrl()
						.substring(serviceUriLength).split("/");
				String serviceName = commands[0];
				String methodName = commands[1];
				Log("Service: [" + serviceName + "] Method:[" + methodName
						+ "]");

				String params = "";
				byte[] paramsBytes = request.getPostData();
				if (paramsBytes != null) {
					params = new String(paramsBytes);
				}

				AndroidInvocationManager aim = (AndroidInvocationManager) AndroidInvocationManager
						.getInstance();
				AndroidServiceLocator asl = (AndroidServiceLocator) AndroidServiceLocator
						.GetInstance();
				byte[] result = null;
				try {
					if (asyncmode){
						AndroidActivityManager aam = (AndroidActivityManager) asl.GetService(
								AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
						// Process result asynchronously
						this.processAsyncPOSTResult(aim, aam, asl.GetService(serviceName), methodName, params);
					} else {
						// Process result synchronously
						result = this.processPOSTResult(aim, asl.GetService(serviceName), methodName, params);
					}
				
				} catch (Exception e) {
					Log("Exception invoking method [" + methodName + "]", e);
					// sending bad request message, instead of passing to next handler
					String badRequestResponse = "{\"result\":\"Malformed request\"}";
					result = badRequestResponse.getBytes();
				}
				
				// adding a cache header to make sure the browser never caches
				// the application service requests
				String cacheControlHeader = HEADER_CACHE_DEFAULT_VALUE;
				
				if(aim.CacheControlHeader()!=null) {
					// use cache control header if set by the invoked service, otherwise, use default
					cacheControlHeader = aim.CacheControlHeader();
				}
				
				response.addHeader(HEADER_CACHE_NAME, cacheControlHeader);
				Log ("Added Caching-Control header to response: " + cacheControlHeader);
				
				if (result != null && result.length > 0) {
					response.setMimeType("application/json");
					ByteArrayInputStream bais = new ByteArrayInputStream(result);
					response.sendResponse(bais, result.length);

					response.setStatusCode(200);
					response.commitResponse();
				}

				return true;
			}
			Log("Unknown URL format: [" + request.getUrl() + "]");
		} else {
			Log("Expecting HttpRequest, received " + aRequest);
		}

		return false;
	}
	
	private void processAsyncPOSTResult(AndroidInvocationManager aim, AndroidActivityManager aam,
				Object service, String methodName, String query) {
		
		Log("Processing result asynchronously");
		Log("query: " + query);

		String callback = null;
		String ID = null;
		String JSON = null;

		// querystring format: callbackFunction$$$ID$$$json=****** 
		if (query!= null) {
			String token0 = "&";
			String token1 = "callback=";
			String token2 = "callbackid=";

			if(query.indexOf(token1)==0) {
				query = query.substring(token1.length());
				callback = query.substring(0, query.indexOf(token0));
				query = query.substring(query.indexOf(token0)+1);
			}
			if(query.indexOf(token2)==0) {
				query = query.substring(token2.length());
				ID = query.substring(0, query.indexOf(token0));
				query = query.substring(query.indexOf(token0)+1);
			}

			JSON = query;
		}
		Log("callback function: " + callback);
		Log("callback ID: " + ID);
		Log("JSON data: " + JSON);

		this.processServiceAsynchronously(callback, ID, aim, aam, service, methodName,JSON);

	}
	
	protected void processServiceAsynchronously(String callbackFunction, String id, AndroidInvocationManager aim, AndroidActivityManager aam,
			Object service, String methodName, String query) {
		AsyncServiceThread asyncThread = new AsyncServiceThread(callbackFunction, id, aim, aam, service,methodName, query);
		asyncThread.start();
	}

	public byte[] processPOSTResult(AndroidInvocationManager invocationManager, Object service, String methodName, String params) throws Exception {

		if (params.length() > PARAM_PREFIX.length()) {
			params = params.substring(PARAM_PREFIX.length());
		} else {
			params = null;
		}
		//Log("invoke service params: " + params);
		return invocationManager.InvokeService (service, methodName, params);
	}

	private class AsyncServiceThread extends Thread {
        
		String callbackFunction;
		String id; 
		AndroidInvocationManager invocationManager;
		AndroidActivityManager activityManager;
		Object service;
		String methodName;
		String query;
        
        public AsyncServiceThread(String callbackFunction, String id,
    			AndroidInvocationManager invocationManager, 
    			AndroidActivityManager activityManager,
    			Object service, String methodName, String query) {
    		super();
    		this.callbackFunction = callbackFunction;
    		this.id = id;
    		this.invocationManager = invocationManager;
    		this.activityManager = activityManager;
    		this.service = service;
    		this.methodName = methodName;
    		this.query = query;
    	}
        
       @Override
        public void run() {
    	   	byte[] result = null;
			try {
				result = processPOSTResult(invocationManager, service, methodName, query);
			} catch (Exception e) {
				Log(" ############## Exception while invoking service: " + e.getMessage());
			}
	   		String jsonResultString = null;
	   		if(result!=null) {
	   			jsonResultString = new String(result);
	   		}
	
	   		this.sendBackResult(activityManager, callbackFunction, id, jsonResultString);
          
        }
       
       protected void sendBackResult(AndroidActivityManager aam, String callbackFunction, String id, String jsonResultString) {
	   		Log(" ############## sending back result to callback fn [" + callbackFunction+ "] and id [" + id+ "]: " + 
	   				(jsonResultString!=null?jsonResultString.length():0));
	   		aam.executeJSCallback(callbackFunction, id, jsonResultString);
	   }
    }
	
}
