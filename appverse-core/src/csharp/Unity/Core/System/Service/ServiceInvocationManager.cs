/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;
using Unity.Core.IO;

namespace Unity.Core.System.Service
{
	public class ServiceInvocationManager : AbstractInvocationManager
	{
		private string _latestCacheControlHeader = null;

		public override string CacheControlHeader ()
		{
			return _latestCacheControlHeader;
		}
		
		/// <summary>
		/// Invokes the given service method, by using an array of invocation params.
		/// </summary>
		/// <param name="service">Service bean.</param>
		/// <param name="methodName">Method to be invoeked.</param>
		/// <param name="invokeParams">Invocation params (array)</param>
		/// <returns>Result in byte array.</returns>
		public override byte[] InvokeService (Object service, string methodName, object[] invokeParams)
		{
			byte[] result = null;
			
			object retObj = InvokeServiceMethod (service, methodName, invokeParams);

			_latestCacheControlHeader = null;

			if(typeof(IIo).IsAssignableFrom(service.GetType())){
				SystemLogger.Log (SystemLogger.Module .CORE, "For I/O Services, check cache control header from remote server");
				_latestCacheControlHeader = GetCacheControlHeaderFromObject(retObj);
			}
			
			if (retObj != null) {
				result = Encoding.UTF8.GetBytes (ToJSONString (retObj));
			}
			
			return result;
		}

		private string GetCacheControlHeaderFromObject(object retObj) {

			if(retObj!= null && typeof(IOResponse).IsAssignableFrom(retObj.GetType())) {
				IOResponse ioResponse = (IOResponse) retObj;
				if(ioResponse.Headers != null && ioResponse.Headers.Length > 0) {
					foreach (IOHeader header in ioResponse.Headers) {
						if(header.Name.Equals ("Cache-Control")) {
							SystemLogger.Log (SystemLogger.Module .CORE, "Found cache control header on IOResponse object: " + header.Value);
							return header.Value;
						}
					}
				}
			}

			return null;
		}
			
		/// <summary>
		/// Invokes the given service method, by using a json object as the invocation params.
		/// </summary>
		/// <param name="service">Service bean.</param>
		/// <param name="methodName">Method to be invoeked.</param>
		/// <param name="jsonString">Invocation params (json string: {"param1":"xxx","param2":"xxx", ...})</param>
		/// <returns>Result in byte array.</returns>
		public override byte[] InvokeService (Object service, string methodName, string jsonString)
		{
			object[] invokeParams = null;
			List<object> paramList = new List<object> ();
			try {
				if (jsonString != null) {
					object jsonData = serialiser.DeserializeObject (jsonString);
					
					int index = 1;
					bool readingParams = true;
					
					while (readingParams) {
						try {
							object jsonObject = ((IDictionary)jsonData) ["param" + index];
							if (jsonObject != null) {
								paramList.Add (jsonObject);
								index++;
							} else {
								if (index <= ((IDictionary)jsonData).Count) {
									// param exists but is null value.
									paramList.Add (null);
									index++;
								} else {
									// no more params expected.
									readingParams = false;
								}
							}
							
						} catch (Exception) {
							readingParams = false;
						}
					}
					if (paramList.Count > 0) {
						invokeParams = paramList.ToArray ();
					}
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module.CORE, "Exception invoking method [" + methodName + "] when parsing invocation parameters.", e);
			}	
			return InvokeService (service, methodName, invokeParams);
		}

		

		/// <summary>
		/// Formats the given object into a JSON string.
		/// </summary>
		/// <param name="obj">Object to be formatted.</param>
		/// <returns>JSON String.</returns>
		private string ToJSONString (object obj)
		{
			string json_string = null;
			if (obj != null) {
				if (obj.GetType ().IsPrimitive || obj.GetType ().FullName == "System.String") {
					json_string = obj.ToString ();
					if (obj.GetType ().FullName == "System.Boolean") {
						json_string = json_string.ToLower ();
					}
					
					if (obj.GetType ().FullName == "System.String") {
						json_string = "\"" + json_string + "\"";
					}
				} else if (obj.GetType ().IsEnum) {
					// Enumeration member values are treated as numbers in JSON.
					json_string = ((int)obj).ToString ();
				} else {
					json_string = serialiser.Serialize (obj);
				}
			}
			return json_string;
		}

		/// <summary>
		/// Gets the object represented by the 
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public override object GetObject (Type type, object json)
		{
			object obj = null;
			
			if (json != null) {
				if (type.FullName == "System.Int32") {
					obj = Int32.Parse (json.ToString ());
					// int
				} else if (type.FullName == "System.Int64") {
					obj = Int64.Parse (json.ToString ());
					// long
				} else if (type.FullName == "System.Single") {
					obj = Single.Parse (json.ToString (), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, FormatUtils.GetNumberFormatInfo ());
					// float
				} else if (type.FullName == "System.Double") {
					obj = Double.Parse (json.ToString (), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, FormatUtils.GetNumberFormatInfo ());
					// float
				} else if (type.IsEnum) {
					obj = Enum.Parse (type, json.ToString ());
				} else if (type.FullName == "System.Boolean") {
					obj = Boolean.Parse (json.ToString ());
					// bool
				} else if (type.FullName == "System.Byte[]") {
					obj = this.toByteArray ((object[])json);
				} else if (json is IDictionary) {
					obj = serialiser.ConvertToType (type, ((IDictionary<string, object>)json));
				} else if (json is Array) {
					obj = serialiser.ConvertToType (type, json);
				} else {
					obj = json;
				}
			}
			
			return obj;
		}
		
		private byte[] toByteArray (object[] objectArray)
		{
			if (objectArray != null) {
				byte[] byteArray = new byte[objectArray.Length];
				for (int i=0; i<objectArray.Length; i++) {
					byteArray [i] = Byte.Parse (objectArray [i].ToString ());
				}
				return byteArray;
			}
			
			return null;
		}
	}
}
