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
using Unity.Core.IO;

namespace Unity.Core.System.Service
{
	public class ResourceInvocationManager : AbstractInvocationManager
	{
		public ResourceInvocationManager ()
		{
		}

		public override string CacheControlHeader ()
		{
			return null;
		}
		
		public override byte[] InvokeService (Object service, string methodName, string queryString)
		{
			string[] paramsArray = null;
			
			if (queryString != null) {
				queryString.Split (new char[] { '/' });
			}
			
			return InvokeService (service, methodName, paramsArray);
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
			
			if (retObj != null) {
				if (retObj.GetType () == typeof(IOResponse)) {
					result = ((IOResponse)retObj).ContentBinary;
				}
			}
			
			return result;
		}
		
		public override object GetObject (Type type, object rawObject)
		{
			// return original object, no conversions apply here.
			return rawObject;
		}
	}
}

