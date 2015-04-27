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
using Unity.Core.System.Service;
using System.IO;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Unity.Core.System;
using System.Runtime.CompilerServices;
using Unity.Core.System.Server.Net;

namespace Unity.Platform.IPhone
{
    public class IPhoneServiceLocator : AbstractServiceLocator
    {
        static IPhoneUIApplicationDelegate uiApplicationDelegate;

		public static string INTERNAL_SERVER_HOST = "127.0.0.1";
		public static string INTERNAL_SERVER_PORT = "8080";
		public static string INTERNAL_SERVER_URL = "http://" + INTERNAL_SERVER_HOST + ":" + INTERNAL_SERVER_PORT;

		private static List<string> managedServices = new List<string> ();
		
		public static IPhoneUIApplicationDelegate CurrentDelegate {
			get {
				return uiApplicationDelegate;
			}
			set {
				uiApplicationDelegate = value;
			}
		}

		static IPhoneServiceLocator()
        {
			typedServices["net"]        = new IPhoneNet();
			typedServices["system"]     = new IPhoneSystem();
			typedServices["file"]       = new IPhoneFileSystem();
			typedServices["db"]         = new IPhoneDatabase();
			typedServices["io"]         = new IPhoneIO();
			typedServices["notify"]     = new IPhoneNotification();
			typedServices["geo"]     	= new IPhoneGeo();
			typedServices["media"]     	= new IPhoneMedia();
			typedServices["message"]    = new IPhoneMessaging();
			typedServices["pim"]    	= new IPhonePIM();
			typedServices["phone"]  	= new IPhoneTelephony();
			typedServices["i18n"]  		= new IPhoneI18N();
			typedServices["log"]  		= new IPhoneLog();
			typedServices["security"]   = new IPhoneSecurity();
			typedServices["loader"]  	= new IPhoneAppLoader();
        }

        /// <summary>
        /// Private Constructor.
        /// Used to force usage of static GetInstance() method.
        /// </summary>
        private IPhoneServiceLocator() : base() {}

		/// <summary>
        /// Hides the AbstractServiceLocator class static method by using the keyword new.
        /// </summary>
        /// <returns>Singleton IServiceLocator.</returns>
        public new static IServiceLocator GetInstance() 
        {
            if (singletonServiceLocator == null)
            {
                singletonServiceLocator = new IPhoneServiceLocator();

				NSUrlProtocol.RegisterClass (new MonoTouch.ObjCRuntime.Class (typeof (IPhoneNSUrlProtocol)));

            }
            return singletonServiceLocator;
        }

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void registerManagedService(string service, string timestamp) {
			try {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneResourceHandler. Registered managed service: " + service + "_" +timestamp);
				managedServices.Add(service + "_" +timestamp);
			} catch (Exception ex) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneResourceHandler Exception in registerManagedService: " + ex.Message);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static bool consumeManagedService(String service) {
			try {

				int index = -1;
				foreach(string managedService in managedServices) {
					if(managedService.StartsWith(IPhoneServiceLocator.INTERNAL_SERVER_URL + service)) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Consuming managed service...");
						index = managedServices.IndexOf(managedService);
						break;
					}
				}
				if(index>-1) {
					managedServices.RemoveAt(index);
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "Managed service consumed.");
					return true;
				}

			} catch (Exception ex) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneResourceHandler Exception #consumeManagedService: " + ex.Message);
			}

			return false;
		}

    }

	public class IPhoneNSUrlProtocol : NSUrlProtocol {

		[Export ("initWithRequest:cachedResponse:client:")]
		public IPhoneNSUrlProtocol (NSUrlRequest request, NSCachedUrlResponse cachedResponse, NSUrlProtocolClient client) 
			: base (request, cachedResponse, client)
		{
		}

		[MonoTouch.Foundation.Export("canInitWithRequest:")]
		public static bool CanInitWithRequest (NSUrlRequest request) {
			// SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol canInitWithRequest: " + request.Url.Host + ", path: " + request.Url.ToString());

			bool shouldHandle = true;  //custom iPhoneNSUrlProtocol will handle all requests by default
			if (request != null && request.Url != null) {
				String url = request.Url.ToString ();

				//checking internal server status
				if (HttpServer.SingletonInstance !=null) {
					shouldHandle = !HttpServer.SingletonInstance.IsListening;
				}

				if (url.StartsWith ("http://127.0.0.1:8080") && (url.Contains ("/service/") || url.Contains ("/service-async/"))) {
					if (!shouldHandle) {
						// SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol MANAGED SERVICE");
						// add to managed service mapping
						IPhoneServiceLocator.registerManagedService (url, "" + DateTime.Now.Ticks);
					}
				}
			}

			return shouldHandle;
		}

		[Export ("canonicalRequestForRequest:")]
		public static new NSUrlRequest GetCanonicalRequest (NSUrlRequest forRequest)
		{
			return forRequest;
		}

		public override void StartLoading ()
		{
			Client.DataLoaded (this, NSData.FromString ("SECURITY ISSUE"));
			Client.FinishedLoading (this);
		}

		public override void StopLoading ()
		{
		}

	}

}
