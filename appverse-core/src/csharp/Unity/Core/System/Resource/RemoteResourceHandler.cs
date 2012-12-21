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
using Unity.Core.System.Server.Net;
using Unity.Core.System.Service;
using Unity.Core.IO;

namespace Unity.Core.System.Resource
{
	public class RemoteResourceHandler : IHttpHandler
	{
		
		private static string REMOTE_RESOURCE_URI = "/proxy/";
		private static string UNITY_IO_SERVICE_NAME = "io";
		private IServiceLocator serviceLocator = null;
		
		public RemoteResourceHandler (IServiceLocator _serviceLocator)
		{
			serviceLocator = _serviceLocator;
		}

	
		#region IHttpHandler implementation
		public bool Process (HttpServer server, HttpRequest request, HttpResponse response)
		{
			SystemLogger.Log (SystemLogger.Module .CORE, " ############## " + this.GetType () + " -> " + request.Url);
			if (request.Url.StartsWith (REMOTE_RESOURCE_URI)) {
				SystemLogger.Log (SystemLogger.Module .CORE, "Remote resource protocol.");
				try {
					string commandParams = request.Url.Substring (REMOTE_RESOURCE_URI.Length);
					string[] commandParamsArray = commandParams.Split (new char[] { '/' });
					if (commandParamsArray.Length > 0) {
						string ioServiceName = commandParamsArray [0];
	                   
						Object unityIOService = serviceLocator.GetService (UNITY_IO_SERVICE_NAME);
						if (unityIOService != null && ioServiceName != null) {
							IIo io = (IIo)unityIOService;
							string parameters = commandParams.Substring (ioServiceName.Length + 1);
							
							IORequest ioRequest = new IORequest ();
							ioRequest.Content = parameters;
							IOResponse ioResponse = io.InvokeService (ioRequest, ioServiceName);
							if (ioResponse != null) {
								response.ContentType = ioResponse.ContentType;
								response.RawContent = ioResponse.ContentBinary;
							}
						}
					}	
					
					if (response.RawContent == null) {
						response.Content = "No content available.";
						SystemLogger.Log (SystemLogger.Module .CORE, "No content available for request [" + request.Url + "," + request.Method + "]. Continue to next handler...");
						return false;
					}
                    
					return true;
				} catch (Exception e) {
					SystemLogger.Log (SystemLogger.Module .CORE, "Exception when parsing request [" + request.Url + "]", e);
					response.Content = "Malformed request.";
					return false;
				}
			} else {
				SystemLogger.Log (SystemLogger.Module .CORE, "Non remote resource protocol. Continue to next handler...");
				return false;
			}
		}
		#endregion
	}
}

