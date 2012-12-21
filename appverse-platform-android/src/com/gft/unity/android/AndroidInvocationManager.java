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

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.List;

import android.util.TimingLogger;

import com.gft.unity.android.util.json.JSONArray;
import com.gft.unity.android.util.json.JSONException;
import com.gft.unity.android.util.json.JSONObject;
import com.gft.unity.android.util.json.JSONSerializer;
import com.gft.unity.android.util.json.JSONTokener;
import com.gft.unity.core.io.IIo;
import com.gft.unity.core.io.IOHeader;
import com.gft.unity.core.io.IOResponse;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.gft.unity.core.system.log.Logger.LogLevel;
import com.gft.unity.core.system.service.AbstractInvocationManager;
import com.gft.unity.core.system.service.IInvocationManager;

public class AndroidInvocationManager extends AbstractInvocationManager {

	private static final String LOGGER_MODULE = "AndroidInvocationManager";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);

	// private static final SystemLogger LOG = SystemLogger.getInstance();

	private static final String PARAM_PREFIX = "param";
	private static final String PACKAGE_PREFIX = "com.gft.unity.";

	private static IInvocationManager singletonInvocationManager;
	
	private String _latestCacheControlHeader = null;


	private AndroidInvocationManager() {
	}

	public static IInvocationManager getInstance() {

		if (singletonInvocationManager == null) {
			singletonInvocationManager = new AndroidInvocationManager();
		}

		return singletonInvocationManager;
	}

	@Override
	public String CacheControlHeader ()
	{
		return _latestCacheControlHeader;
	}

	
	@Override
	public byte[] InvokeService(Object service, String methodName,
			String queryString) throws Exception {
		byte[] result = null;

		TimingLogger timingLogger = new TimingLogger(
				LogCategory.PLATFORM.toString(), LOGGER_MODULE
						+ ".InvokeService");

		LOGGER.logOperationBegin("InvokeService", new String[] { "service",
				"methodName", "queryString" }, new Object[] { service,
				methodName, queryString });

		// parse query parameters
		List<Object> jsonParameters = new ArrayList<Object>();
		try {
			if (queryString != null) {
				JSONObject query = (JSONObject) new JSONTokener(queryString)
						.nextValue();
				for (int index = 1; index <= query.length(); index++) {
					Object parameter = query.get(PARAM_PREFIX + index);
					jsonParameters.add(parameter);
				}
			}
		} catch (JSONException ex) {
			LOGGER.logError("InvokeService", "Parse query error", ex);
			throw ex;
		}

		timingLogger.addSplit("Parse request");

		List<Method> methods = findMethods(service, methodName, jsonParameters);
		int count = methods.size();

		// no method found
		if (count == 0) {
			LOGGER.logError("InvokeService",
					"no method found that matches method name [" + methodName
							+ "] and parameters [" + jsonParameters + "]");
			throw new UnsupportedOperationException(
					"Matching method not found -> " + methodName);
		}

		// more than 1 method found
		if (count > 1) {
			LOGGER.logError("InvokeService",
					"more than 1 method found that matches method name ["
							+ methodName + "] and parameters ["
							+ jsonParameters + "]");
			throw new UnsupportedOperationException(
					"More than 1 matching method found -> " + methodName);
		}

		timingLogger.addSplit("Find target service and method");

		// 1 method found
		Method method = methods.get(0);
		Object[] parameters = new Object[jsonParameters.size()];
		Class<?>[] parameterTypes = method.getParameterTypes();

		// deserialize (JSON to object) method parameters
		int index = 0;
		for (Object jsonParameter : jsonParameters) {

			if (LOGGER.isLoggable(LogLevel.DEBUG)) {
				LOGGER.logDebug("InvokeService", "  Deserialize values -> "
						+ parameterTypes[index].getCanonicalName() + " - "
						+ jsonParameter);
			}

			parameters[index] = JSONSerializer.deserialize(
					parameterTypes[index], jsonParameter);

			if (LOGGER.isLoggable(LogLevel.DEBUG)) {
				LOGGER.logDebug("InvokeService", "  Deserialize result -> "
						+ parameters[index]);
			}
			index++;
		}

		timingLogger.addSplit("Parse parameters");

		// invoke method
		if (LOGGER.isLoggable(LogLevel.DEBUG)) {
			LOGGER.logDebug("InvokeService", "invoking method [" + methodName
					+ "] on service [" + service.getClass().getSimpleName()
					+ "]");
			if (parameters != null) {
				for (Object parameter : parameters) {
					if (parameter != null) {
						LOGGER.logDebug("InvokeService", "  parameter type ["
								+ parameter.getClass().getName() + "], value ["
								+ parameter.toString() + "]");
					} else {
						LOGGER.logDebug("InvokeService",
								"  parameter type [<n/a>], value [null]");
					}
				}
			}
		}

		Object response = null;
		if (method.getReturnType().equals(void.class)) {
			method.invoke(service, parameters);
		} else {
			response = method.invoke(service, parameters);
		}
		
		_latestCacheControlHeader = null;

		if( service instanceof IIo){
			LOGGER.logDebug("InvokeService",  "For I/O Services, check cache control header from remote server");
			_latestCacheControlHeader = this.getCacheControlHeaderFromObject(response);
		}
		
		timingLogger.addSplit("Invoke service");

		// serialize (object to JSON) method response
		if (response != null) {
			String responseJson = JSONSerializer.serialize(response);
			if (LOGGER.isLoggable(LogLevel.DEBUG)) {
				LOGGER.logDebug("InvokeService", "  result JSON size ["
						+ responseJson.length() + "] bytes");
				LOGGER.logDebug("InvokeService", "  result JSON ["
						+ responseJson + "]");
			}
			result = responseJson.getBytes();
		}

		timingLogger.addSplit("Serialize response");
		timingLogger.dumpToLog();

		LOGGER.logOperationEnd("InvokeService", result);

		return result;
	}
	
	private String getCacheControlHeaderFromObject(Object retObj) {

		if(retObj!= null && retObj instanceof IOResponse) {
			IOResponse ioResponse = (IOResponse) retObj;
			if(ioResponse.getHeaders() != null && ioResponse.getHeaders().length > 0) {
				for (IOHeader header : ioResponse.getHeaders()) {
					if(header.getName().equalsIgnoreCase("Cache-Control")) {
						LOGGER.logDebug("getCacheControlHeaderFromObject",  "Found cache control header on IOResponse object: " + header.getValue());
						return header.getValue();
					}
				}
			}
		}

		return null;
	}


	// TODO additional checks on method compatibility (ie: enum must be numeric,
	// array elements, ...)
	/**
	 * Returns a list of methods from <CODE>service</CODE> named
	 * <CODE>methodName</CODE> and with compatible <CODE>parameters</CODE>.
	 * 
	 * @param service
	 *            Object instance.
	 * @param name
	 *            Method name.
	 * @param parameters
	 *            Parameters.
	 * @return List of methods.
	 */
	private List<Method> findMethods(Object service, String name,
			List<Object> parameters) {
		List<Method> methods = new ArrayList<Method>();

		Method[] allMethods = service.getClass().getMethods();
		for (Method method : allMethods) {
			Class<?>[] parameterTypes = method.getParameterTypes();
			if (method.getName().equals(name)
					&& parameterTypes.length == parameters.size()) {

				LOGGER.logDebug("FindMethods",
						"Checking method -> " + method.getName());

				int index = 0;
				boolean match = true;
				for (Object parameter : parameters) {

					Class<?> parameterType = parameterTypes[index];

					if (LOGGER.isLoggable(LogLevel.DEBUG)) {
						LOGGER.logDebug("FindMethods", "  Types -> "
								+ parameter.getClass().getName() + " <-> "
								+ parameterType.getCanonicalName());
					}

					if (parameter instanceof JSONArray) { // array
						if (!parameterType.isArray()) {

							LOGGER.logDebug("FindMethods",
									"Matching error -> JSONArray but target is not array");
							match = false;
							break;
						}
					} else if ((parameter instanceof JSONObject)
							|| (JSONObject.NULL.equals(parameter))) { // bean
						if (!parameterType.getPackage().getName()
								.startsWith(PACKAGE_PREFIX)) {
							LOGGER.logDebug("FindMethods",
									"Matching error -> JSONObject but target is not a bean");
							match = false;
							break;
						}
					} else { // primitive
						if (!parameterType.isEnum()
								&& !parameterType.isPrimitive()
								&& !parameterType
										.isAssignableFrom(String.class)) {
							LOGGER.logDebug("FindMethods",
									"Matching error -> JSONObject but target is not a bean");
							match = false;
							break;
						}
					}

					index++;
				}

				if (match) {
					methods.add(method);
				}
			}
		}

		return methods;
	}
}
