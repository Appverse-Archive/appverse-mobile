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
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace Unity.Core.System.Service
{
	public class AbstractServiceLocator : IServiceLocator
	{

		protected static IServiceLocator singletonServiceLocator = null;
		protected static Dictionary<string, Object> typedServices = new Dictionary<string, Object> ();
		private Dictionary<string, Object> services = new Dictionary<string, Object> ();
		
		/// <summary>
		/// Constructor. Only Accessible via its subclasses.
		/// </summary>
		protected AbstractServiceLocator ()
		{
			foreach (string serviceType in typedServices.Keys) {
				SystemLogger.Log (SystemLogger.Module.CORE, "# Registered Service: name[" + serviceType + "] type[" + serviceType + "]");

				Object instantiatedService = typedServices [serviceType];
				RegisterService (instantiatedService, serviceType);
			}
		}

		/// <summary>
		/// Get IServiceLocator instace.
		/// </summary>
		/// <returns></returns>
		public static IServiceLocator GetInstance ()
		{
			if (singletonServiceLocator == null) {
				singletonServiceLocator = new AbstractServiceLocator ();
			}
			return singletonServiceLocator;
		}
		
		/// <summary>
		/// Adds given service to the inner services dictionary mappwd with the given key.
		/// </summary>
		/// <param name="service">Service bean to be addded.</param>
		/// <param name="key">Key to </param>
		public void RegisterService (Object service, string key)
		{
			if (key != null && service != null) {
				services [key] = service;
			}
		}

        #region Miembros de IServiceLocator

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Object GetService (string name)
		{
			// Get mapped service.
			Object service = null;

			try {
				service = services [name];
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module .CORE, "No service found. Service name requested:" + name + ".", e);
			}

			return service;
		}

        #endregion
	}
}
