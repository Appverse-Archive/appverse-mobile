/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
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
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Core.System.Server.Net;
using System.Collections;
using System.Threading;

namespace Unity.Core.System.Service
{
	public class ServiceURIHandler : IHttpHandler
	{
		protected static string SERVICE_URI = "/service/";
		protected static string SERVICE_ASYNC_URI = "/service-async/";
		private static string CACHE_CONTROL_HEADER = "Cache-Control";
		private static string DEFAULT_CACHE_CONTROL = "no-cache";
		private IServiceLocator serviceLocator = null;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="_serviceLocator"></param>
		public ServiceURIHandler (IServiceLocator _serviceLocator)
		{
			serviceLocator = _serviceLocator;
		}


		private void ProcessAsyncPOSTResult(IInvocationManager invocationManager, Object service, string methodName, string query) {
	
			SystemLogger.Log (SystemLogger.Module .CORE, "Processing result asynchronously");
			SystemLogger.Log (SystemLogger.Module .CORE, "query: " + query);

			string callback = null;
			string ID = null;
			string JSON = null;

			// querystring format: callbackFunction$$$ID$$$json=****** 
			// [MOBPLAT-185], the "json" latest query parameter could not be present (for API methods without parameters)
			if (query!= null) {
				string token0 = "&";
				string token1 = "callback=";
				string token2 = "callbackid=";
				int nextParamToken = 0;

				if(query.IndexOf(token1)==0) {
					query = query.Substring(token1.Length);
					nextParamToken = query.IndexOf(token0);
					if(nextParamToken==-1) nextParamToken = query.Length;
					if(query.Length>=nextParamToken)
						callback = query.Substring(0, nextParamToken);
					if (query != null && query.Length > (nextParamToken + 1))
						query = query.Substring (nextParamToken + 1);
					else
						query = null;
				}
				if(query!= null && query.IndexOf(token2)==0) {
					query = query.Substring(token2.Length);
					nextParamToken = query.IndexOf(token0);
					if(nextParamToken==-1) nextParamToken = query.Length;
					if(query.Length>=nextParamToken)
						ID = query.Substring(0, nextParamToken);
					if (query != null && query.Length > (nextParamToken + 1))
						query = query.Substring(nextParamToken+1);
					else
						query = null;
				}

				JSON = query;
			}
			SystemLogger.Log (SystemLogger.Module .CORE, "callback function: " + callback);
			SystemLogger.Log (SystemLogger.Module .CORE, "callback ID: " + ID);
			SystemLogger.Log (SystemLogger.Module .CORE, "JSON data: " + JSON);

			this.ProcessServiceAsynchronously(callback, ID, invocationManager, service, methodName,JSON);

		}

		/// <summary>
		/// Processes the service asynchronously.
		/// Override this function to provide platform specific behaviour (such as, calling this in an autorelease pool)
		/// </summary>
		/// <param name='callbackFunction'>
		/// Callback function.
		/// </param>
		/// <param name='id'>
		/// Identifier.
		/// </param>
		/// <param name='invocationManager'>
		/// Invocation manager.
		/// </param>
		/// <param name='service'>
		/// Service.
		/// </param>
		/// <param name='methodName'>
		/// Method name.
		/// </param>
		/// <param name='query'>
		/// Query.
		/// </param>
		protected virtual void ProcessServiceAsynchronously(string callbackFunction, string id, IInvocationManager invocationManager, Object service, string methodName, string query) {
			object[] asyncData = new object[] { callbackFunction, id, invocationManager, service, methodName, query};

			var thread = new Thread (ProcessAsyncData);
			thread.Start (asyncData);
		}

		private void ProcessAsyncData(object asyncDataObj) {
			object[] asyncData = (object[]) asyncDataObj;

			string callbackFunction 				= (string) asyncData[0];
			string id 								= (string) asyncData[1]; 
			IInvocationManager invocationManager 	= (IInvocationManager) asyncData[2];
			Object service 							= (Object) asyncData[3];
			string methodName 						= (string) asyncData[4];
			string query 							= (string) asyncData[5];

			byte[] result = this.ProcessPOSTResult(invocationManager, service, methodName, query);
			string jsonResultString = null;
			if(result!=null) {
				jsonResultString = Encoding.UTF8.GetString(result);
			}

			this.SendBackResult(callbackFunction, id, jsonResultString);
		}

		protected virtual void SendBackResult(string callbackFunction, string id, string jsonResultString) {
			// Override this function on platform to send back result to application
			SystemLogger.Log (SystemLogger.Module .CORE, " ############## sending back result to callback fn [" + callbackFunction+ "] and id [" + id+ "]");
		}
		
		private byte[] ProcessGETResult(IInvocationManager invocationManager, Object service, string methodName, string[] methodParams) {
			return invocationManager.InvokeService (service, methodName, methodParams);
		}

		public byte[] ProcessPOSTResult(IInvocationManager invocationManager, Object service, string methodName, string query) {

			string queryString = null;
			if (query != null && query.Length > "json=".Length) {
				//queryString = ServiceURIHandler.UrlDecode(request.QueryString.Substring("json=".Length));
				queryString = query.Substring ("json=".Length);
			}

			return invocationManager.InvokeService (service, methodName, queryString);
		}

        #region Miembros de IHttpHandler

		public virtual bool Process (HttpServer server, HttpRequest request, HttpResponse response)
		{
			SystemLogger.Log (SystemLogger.Module .CORE, " ############## " + this.GetType () + " -> " + request.Url);
			if (request.Url.StartsWith (SERVICE_URI) || request.Url.StartsWith (SERVICE_ASYNC_URI)) {
				SystemLogger.Log (SystemLogger.Module .CORE, "Service protocol.");

				bool asyncmode = false;
				int serviceUriLength = SERVICE_URI.Length;
				if(request.Url.StartsWith (SERVICE_ASYNC_URI)) {
					asyncmode = true;
					serviceUriLength = SERVICE_ASYNC_URI.Length;
				}

				if(response.Header == null) {
					response.Header = new Hashtable();
				}

				// Adding Header Cache-Control to "no-cache" to force no caching service requests
				string cacheControlHeader = DEFAULT_CACHE_CONTROL;

				response.ContentType = "application/json";
				IInvocationManager invocationManager = getInvocationManager (request.Method);

				try {
					string commandParams = request.Url.Substring (serviceUriLength);
					string[] commandParamsArray = commandParams.Split (new char[] { '/' });
					string serviceName = commandParamsArray [0];
                   
					Object service = serviceLocator.GetService (serviceName);
					byte[] result = null;
					string methodName = commandParamsArray [1];

					if (request.Method == "GET" && !asyncmode) {
						string[] methodParams = null;
						if (commandParamsArray.Length > 2) {
							methodParams = new string[commandParamsArray.Length - 2];
							for (int i = 2; i < commandParamsArray.Length; i++) {
								methodParams [i - 2] = commandParamsArray [i];
							}
						}

						// Process result synchronously
						result = this.ProcessGETResult(invocationManager, service, methodName, methodParams);
					} else if (request.Method == "POST") {
						if (asyncmode){
							// Process result asynchronously
							this.ProcessAsyncPOSTResult(invocationManager, service, methodName, request.QueryString);
						} else {
							// Process result synchronously
							result = this.ProcessPOSTResult(invocationManager, service, methodName, request.QueryString);
						}
					}

					if(asyncmode) {
						// Return acknowledge
						response.Content = "{\"result\":\"Processed\"}";
					} else {
						response.RawContent = result;
						if (response.RawContent == null) {
							response.Content = "null";
							/* SERVICES COULD SEND NULL OBJETS.
								SystemLogger.Log(SystemLogger.Module .CORE, "No content available for request [" + request.Url + "," + request.Method + "]. Continue to next handler...");
		                        return false;
		                        */
						}
					}

				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module .CORE, "Exception when parsing request [" + request.Url + "]", e);
					response.Content = "{\"result\":\"Malformed request\"}";
				}

				if(invocationManager.CacheControlHeader()!=null) {
					cacheControlHeader = invocationManager.CacheControlHeader();
				}
				
				if(response.Header.ContainsKey(CACHE_CONTROL_HEADER)) {
					response.Header[CACHE_CONTROL_HEADER] = cacheControlHeader;
					SystemLogger.Log (SystemLogger.Module .CORE, "Updated Caching-Control header on response: " + cacheControlHeader);
				} else {
					response.Header.Add(CACHE_CONTROL_HEADER, cacheControlHeader);
					SystemLogger.Log (SystemLogger.Module .CORE, "Added Caching-Control header to response: " + cacheControlHeader);
				}

				return true;
			} else {
				SystemLogger.Log (SystemLogger.Module .CORE, "Non service protocol. Continue to next handler...");
				return false;
			}
		}

        #endregion

		/// <summary>
		/// UrlDecodes a string without requiring System.Web
		/// </summary>
		/// <param name="text">String to decode.</param>
		/// <returns>decoded string</returns>
		public static string UrlDecode (string text)
		{
			// pre-process for + sign space formatting since System.Uri doesn't handle it
			// plus literals are encoded as %2b normally so this should be safe
			text = text.Replace ("+", " ");
			return Uri.UnescapeDataString (text);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reqMethod">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="IInvocationManager"/>
		/// </returns>
		private IInvocationManager getInvocationManager (string reqMethod)
		{
			if (reqMethod == "POST") {
				return new ServiceInvocationManager ();	
			} else {
				return new ResourceInvocationManager ();
			}
		}
	}
}
