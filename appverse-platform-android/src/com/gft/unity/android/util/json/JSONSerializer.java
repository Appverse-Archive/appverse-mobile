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
package com.gft.unity.android.util.json;

import java.lang.reflect.Array;
import java.lang.reflect.Method;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Iterator;
import java.util.Map;
import java.util.Set;

import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

// TODO add mechanism to skip properties
public class JSONSerializer {

	private static final SystemLogger LOG = SystemLogger.getInstance();

	private static final String GETTER_PREFIX = "get";
	private static final int GETTER_PREFIX_LENGTH = GETTER_PREFIX.length();
	private static final String SETTER_PREFIX = "set";

	private static final Set<Class<?>> WRAPPER_TYPES = new HashSet<Class<?>>();

	static {
		WRAPPER_TYPES.add(Boolean.class);
		WRAPPER_TYPES.add(Character.class);
		WRAPPER_TYPES.add(Byte.class);
		WRAPPER_TYPES.add(Short.class);
		WRAPPER_TYPES.add(Integer.class);
		WRAPPER_TYPES.add(Long.class);
		WRAPPER_TYPES.add(Float.class);
		WRAPPER_TYPES.add(Double.class);
		WRAPPER_TYPES.add(Void.class);
	}

	public static String serialize(Object object) {
		String result;

		try {
			result = JSONObject.valueToString(serializeElement(object));
		} catch (Exception ex) {
			LOG.Log(Module.PLATFORM, "JSONSerializer serialize error: "
					+ object, ex);
			result = "\"null\"";
		}

		return result;
	}

	private static Object serializeElement(Object object) {
		Object element = null;

		Class<?> classType = object.getClass();
		if (classType.isArray()) {
			element = serializeArray(object);
		} else if (classType.isPrimitive() || WRAPPER_TYPES.contains(classType)
				|| object instanceof String) {
			element = object;
		} else if (classType.isEnum()) {
			element = ((Enum<?>) object).ordinal();
		} else if (object instanceof Map) {
			element = new JSONObject((Map<?,?>) object);
		} else {
			element = serializeBean(object);
		}

		return element;
	}

	private static Object serializeArray(Object object) {
		JSONArray jsonArray = new JSONArray();

		if (object.getClass().equals(byte[].class)) {
			for (byte objChild : (byte[]) object) {
				jsonArray.put(objChild);
			}
			// TODO optimize for other primitive types
		} else {
			for (Object objChild : (Object[]) object) {
				jsonArray.put(serializeElement(objChild));
			}
		}

		return jsonArray;
	}

	private static JSONObject serializeBean(Object object) {

		HashMap<Object, Object> jsonMap = new HashMap<Object, Object>();
		Method[] methods = object.getClass().getMethods();
		for (Method method : methods) {
			if (isPropertyGetter(object, method)) {
				String methodName = method.getName();
				String fieldName = methodName.substring(GETTER_PREFIX_LENGTH);
				Object fieldValue = null;
				try {
					Object objValue = method.invoke(object, new Object[] {});
					if (objValue == null) {
						fieldValue = JSONObject.NULL;
					} else {
						fieldValue = serializeElement(objValue);
					}
				} catch (Exception e) {
					LOG.Log(Module.PLATFORM,
							"JSONSerializer error invoking method ["
									+ methodName + "] in the object [" + object
									+ "]", e);
				}
				jsonMap.put(fieldName, fieldValue);
			}
		}

		return new JSONObject(jsonMap);
	}

	private static boolean isPropertyGetter(Object object, Method method) {

		String methodName = method.getName();
		Class<?> declaringClass = method.getDeclaringClass();

		// method must have only one parameter.
		if (method.getParameterTypes().length > 0) {
			return false;
		}

		// method must start with 'get' or 'Get' and the first character after
		// the prefix must in upper case.
		if (!methodName.toUpperCase().startsWith(GETTER_PREFIX.toUpperCase())
				|| !Character.isUpperCase(methodName
						.charAt(GETTER_PREFIX_LENGTH))) {
			return false;
		}

		// method must be declared in an Unity sub package.
		if (!declaringClass.getPackage().getName().startsWith("com.gft.unity")) {
			return false;
		}

		// method must be declared in the class or immediate superclass
		if (!declaringClass.equals(object.getClass())
				&& !declaringClass.equals(object.getClass().getSuperclass())) {
			return false;
		}

		return true;
	}

	public static Object deserialize(Class<?> classType, Object object)
			throws Exception {
		return deserializeElement(classType, object);
	}

	private static Object deserializeElement(Class<?> classType, Object object)
			throws Exception {
		Object element;

		if (classType.isArray()) {
			element = deserializeArray(classType, (JSONArray) object);
		} else if (classType.isEnum()) {
				element = deserializeEnum(classType, object);
		} else if (classType.isPrimitive() 
				|| object instanceof String
				|| object instanceof Integer
				|| object instanceof Long
				|| object instanceof Short
				|| object instanceof Double
				|| object instanceof Float) {
			element = deserializeSimple(classType, object);
		} else {
			if (JSONObject.NULL.equals(object)) {
				element = null;
			} else {
				element = deserializeBean(classType, (JSONObject) object);
			}
		}

		return element;
	}

	private static Object deserializeArray(Class<?> classType,
			JSONArray jsonArray) throws Exception {
		Object array = null;

		Class<?> elementClassType = classType.getComponentType();

		if (elementClassType.isPrimitive()) {

			if (elementClassType.equals(int.class)) {
				int[] ints = new int[jsonArray.length()];
				for (int i = 0; i < ints.length; i++) {
					ints[i] = Integer.valueOf(jsonArray.getString(i));
				}
				array = ints;
			} else if (elementClassType.equals(boolean.class)) {
				boolean[] booleans = new boolean[jsonArray.length()];
				for (int i = 0; i < booleans.length; i++) {
					booleans[i] = Boolean.valueOf(jsonArray.getString(i));
				}
				array = booleans;
			} else if (elementClassType.equals(char.class)) {
				char[] chars = new char[jsonArray.length()];
				for (int i = 0; i < chars.length; i++) {
					chars[i] = Character.valueOf(jsonArray.getString(i).charAt(
							0));
				}
				array = chars;
			} else if (elementClassType.equals(byte.class)) {
				byte[] bytes = new byte[jsonArray.length()];
				for (int i = 0; i < bytes.length; i++) {
					String jsonStringByte = jsonArray.getString(i);
					// [MOBPLAT-184] - special characters such as accented, were overfloading the Byte.valueOf() method 
					// that only accepts -128 to 127 integers.
					// To avoid this, we should first parse the string to Integer, and then cast to byte
					// UTF-8 compatible
					bytes[i] = (byte) Integer.parseInt(jsonStringByte);
				}
				array = bytes;
			} else if (elementClassType.equals(short.class)) {
				short[] shorts = new short[jsonArray.length()];
				for (int i = 0; i < shorts.length; i++) {
					shorts[i] = Short.valueOf(jsonArray.getString(i));
				}
				array = shorts;
			} else if (elementClassType.equals(long.class)) {
				long[] longs = new long[jsonArray.length()];
				for (int i = 0; i < longs.length; i++) {
					longs[i] = Long.valueOf(jsonArray.getString(i));
				}
				array = longs;
			} else if (elementClassType.equals(float.class)) {
				float[] floats = new float[jsonArray.length()];
				for (int i = 0; i < floats.length; i++) {
					floats[i] = Float.valueOf(jsonArray.getString(i));
				}
				array = floats;
			} else if (elementClassType.equals(double.class)) {
				double[] doubles = new double[jsonArray.length()];
				for (int i = 0; i < doubles.length; i++) {
					doubles[i] = Double.valueOf(jsonArray.getString(i));
				}
				array = doubles;
			}
		} else {

			Object[] objects = (Object[]) Array.newInstance(elementClassType,
					jsonArray.length());
			for (int i = 0; i < jsonArray.length(); i++) {
				objects[i] = deserializeElement(elementClassType,
						jsonArray.get(i));
			}

			array = objects;
		}

		return array;
	}

	private static Object deserializeSimple(Class<?> classType, Object object) {
		Object simple = null;

		if (classType.equals(int.class)) {
			simple = Integer.valueOf(object.toString());
		} else if (classType.equals(boolean.class)) {
			simple = Boolean.valueOf(object.toString());
		} else if (classType.equals(char.class)) {
			simple = Character.valueOf(object.toString().charAt(0));
		} else if (classType.equals(byte.class)) {
			simple = Byte.valueOf(object.toString());
		} else if (classType.equals(short.class)) {
			simple = Short.valueOf(object.toString());
		} else if (classType.equals(long.class)) {
			simple = Long.valueOf(object.toString());
		} else if (classType.equals(float.class)) {
			simple = Float.valueOf(object.toString());
		} else if (classType.equals(double.class)) {
			simple = Double.valueOf(object.toString());
		} else {
			simple = object.toString();
		}

		return simple;
	}

	private static Object deserializeEnum(Class<?> classType, Object object) {

		Object[] enums = classType.getEnumConstants();
		return enums[Integer.valueOf(object.toString())];
	}

	private static Object deserializeBean(Class<?> classType, JSONObject object)
			throws Exception {

		Object bean = classType.newInstance();

		@SuppressWarnings("unchecked")
		Iterator<String> keys = object.keys();
		Method[] methods = classType.getMethods();

		while (keys.hasNext()) {
			String key = keys.next();
			Method setter = null;
			for (Method method : methods) {
				if (method.getName().toUpperCase()
						.equals((SETTER_PREFIX + key).toUpperCase())
						&& method.getParameterTypes().length == 1) {
					setter = method;
					break;
				}
			}
			if (setter != null) {
				Object parameter = deserializeElement(
						setter.getParameterTypes()[0], object.get(key));
				setter.invoke(bean, parameter);

			}
		}

		return bean;
	}
}
