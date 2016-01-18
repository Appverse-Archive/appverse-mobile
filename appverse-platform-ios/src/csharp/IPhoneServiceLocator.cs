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
using UIKit;
using Foundation;
using Unity.Core.System;
using System.Runtime.CompilerServices;
using Unity.Core.System.Server.Net;
using Unity.Core;
using Unity.Core.System.Resource;
using AntiDebuggingBinding;

namespace Unity.Platform.IPhone
{
    public class IPhoneServiceLocator : AbstractServiceLocator
    {
        static IPhoneUIApplicationDelegate uiApplicationDelegate;

		/* TO BE REMOVED - 5.0.6 [AMOB-30]
		public static string INTERNAL_SERVER_HOST = "127.0.0.1";
		public static string INTERNAL_SERVER_URL = "http://" + INTERNAL_SERVER_HOST + ":{0}";
	    */
		private static string APPVERSE_HOST = "https://appverse";
		public static string APPVERSE_SERVICE_PATH = "/service/";
		public static string APPVERSE_RESOURCE_PATH = "/WebResources/www/";
		public static string APPVERSE_DOCUMENT_PATH = "/documents/";
		public static string APPVERSE_SERVICE_URI = APPVERSE_HOST + APPVERSE_SERVICE_PATH;
		public static string APPVERSE_RESOURCE_URI = APPVERSE_HOST + APPVERSE_RESOURCE_PATH;
		public static string APPVERSE_DOCUMENT_URI = APPVERSE_HOST + APPVERSE_DOCUMENT_PATH;

		private static List<string> managedServices = new List<string> ();

		private static UIApplicationWeakDelegate uiApplicationWeakDelegate;

		public static IPhoneUIApplicationDelegate CurrentDelegate {
			get {
				return uiApplicationDelegate;
			}
			set {
				uiApplicationDelegate = value;
			}
		}

		public static UIApplicationWeakDelegate UIApplicationWeakDelegate {
			get {
				return uiApplicationWeakDelegate;
			}
			set {
				uiApplicationWeakDelegate = value;
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


			// include services from modules here
			// START_APPVERSE_MODULES_SERVICES

			// START_HERE_APPVERSE_MODULE_SERVICE
			// END_HERE_APPVERSE_MODULE_SERVICE

			// END_APPVERSE_MODULES_SERVICES 

			// example:
			// typedServices["module.api.service.name"]  = new module.ios.main.class();
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

				// initialize UIApplication weak delegates
				var tsEnumerator = typedServices.GetEnumerator ();
				while (tsEnumerator.MoveNext())
				{
					var typedService = tsEnumerator.Current;
					if (typedService.Value is IWeakDelegateManager) {
						IPhoneServiceLocator.UIApplicationWeakDelegate.RegisterWeakDelegate((typedService.Value as IWeakDelegateManager), typedService.Key);
					}
				}

				NSUrlProtocol.RegisterClass (new ObjCRuntime.Class (typeof (IPhoneNSUrlProtocol)));

            }
            return singletonServiceLocator;
        }

		public static void FinishedLaunching (UIApplication application, NSDictionary launchOptions) {

			#if DEBUG
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "# Application is in DEBUG mode");
			# else
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "# Application is in RELEASE mode (activating antidebugging techniques");
				try { 
					AntiDebugging.Disable_gdb();
				} catch (Exception ex) { }
			#endif

			// Add notification observer for "userdidtakescreenshot" event
			UIApplication.Notifications.ObserveUserDidTakeScreenshot (delegate(object sender, NSNotificationEventArgs e) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "NOTIFICATIONS ObserveUserDidTakeScreenshot - " + e.Notification.Name);

				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.UserDidTakeScreenshot", null);
			});


			// informs the UIApplication Weak delegates (if any) about the application loading finished event
			var tsEnumerator = typedServices.GetEnumerator ();
			while (tsEnumerator.MoveNext())
			{
				var typedService = tsEnumerator.Current;
				if (typedService.Value is IWeakDelegateManager) {

					string loadConfigFilePath = null;
					try {
						loadConfigFilePath = (typedService.Value as IWeakDelegateManager).GetConfigFilePath ();
						if (loadConfigFilePath != null) {
							loadConfigFilePath = IPhoneUtils.GetInstance().GetFileFullPath(loadConfigFilePath);
							(typedService.Value as IWeakDelegateManager).ConfigFileLoadedData(IPhoneUtils.GetInstance().GetResourceAsBinary(loadConfigFilePath, true));
						}
					} catch (Exception ex) {
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "# Cannot load config file for module: " + loadConfigFilePath);
					}

					(typedService.Value as IWeakDelegateManager).FinishedLaunching (application, launchOptions);
				}
			}
		}

		public static void WebViewLoadingFinished(UIApplicationState applicationState, NSDictionary options) {
			// informs the UIApplication Weak delegates (if any) about the web view loading finished  event
			var tsEnumerator = typedServices.GetEnumerator ();
			while (tsEnumerator.MoveNext())
			{
				var typedService = tsEnumerator.Current;
				if (typedService.Value is IWeakDelegateManager) {
					(typedService.Value as IWeakDelegateManager).WebViewLoadingFinished (applicationState, options);
				}
			}
		}

		/* TO BE REMOVED - 5.0.6 [AMOB-30]

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

			if (!IPhoneServiceLocator.CurrentDelegate.ShouldActivateManagedServices ())
				return true;  //check if current application delegate has activated the managed services feature

			try {

				int index = -1;
				foreach(string managedService in managedServices) {
					if(managedService.StartsWith(String.Format(IPhoneServiceLocator.INTERNAL_SERVER_URL, IPhoneServiceLocator.CurrentDelegate.GetListeningPort())  + service)) {
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
		*/

    }

	public class IPhoneNSUrlProtocol : NSUrlProtocol {

		IPhoneServiceURIHandler serviceURIHandler = null;
		IPhoneResourceHandler resourceURIHandler = null;
		ServiceInvocationManager serviceInvocationManager = null;
		IServiceLocator serviceLocator = null;


		[Export ("initWithRequest:cachedResponse:client:")]
		public IPhoneNSUrlProtocol (NSUrlRequest request, NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client) 
			: base (request, cachedResponse, client)
		{
			//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol - Init with Request");

			serviceLocator = IPhoneServiceLocator.GetInstance ();
			serviceURIHandler = new IPhoneServiceURIHandler (serviceLocator);
			resourceURIHandler = new IPhoneResourceHandler (ApplicationSource.FILE);
			serviceInvocationManager = new ServiceInvocationManager ();

		}

		/*
		 * PREVIOUS implementation : TO BE REMOVED

		[Foundation.Export("canInitWithRequest:")]
		public static bool CanInitWithRequest (NSUrlRequest request) {
			// SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol canInitWithRequest: " + request.Url.Host + ", path: " + request.Url.ToString());

			bool shouldHandle = true;  //custom iPhoneNSUrlProtocol will handle all requests by default
			if (request != null && request.Url != null) {
				String url = request.Url.ToString ();

				//checking internal server status
				if (HttpServer.SingletonInstance !=null) {
					shouldHandle = !HttpServer.SingletonInstance.IsListening;
				}

				if (url.StartsWith (IPhoneServiceLocator.INTERNAL_SERVER_URL) && (url.Contains ("/service/"))) {
					if (!shouldHandle) {
						// SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol MANAGED SERVICE");
						// add to managed service mapping
						IPhoneServiceLocator.registerManagedService (url, "" + DateTime.Now.Ticks);
					}
				}

				if ((url.StartsWith ("file://") || url.StartsWith ("data:")) && !IPhoneServiceLocator.CurrentDelegate.SecurityChecksPassed()) {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol Loading error page resources");
					shouldHandle = false;  // let the webview to handle the error pages from local filesystem
				}

			}

			return shouldHandle;
		}
		*/
		 
		/* NEW implementation
		 * 
		 * exception found invoking "Client.DataLoaded" causing an app crash :  solved with Xamarin iOS 8.6.x
		 * 
		 */
		[Foundation.Export("canInitWithRequest:")]
		public static bool CanInitWithRequest (NSUrlRequest request) {
			//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol canInitWithRequest: " + request.Url.Host + ", path: " + request.Url.ToString());

			bool shouldHandle = false;  //custom iPhoneNSUrlProtocol will handle ONLY the services requests

			if (request != null && request.Url != null) {
				String url = request.Url.ToString ();

				/* TO BE REMOVED - 5.0.6 [AMOB-30]
				//checking internal server status for locahost requests (only resources)
				if (url.StartsWith (String.Format(IPhoneServiceLocator.INTERNAL_SERVER_URL, IPhoneServiceLocator.CurrentDelegate.GetListeningPort()) ) && HttpServer.SingletonInstance !=null) {
					shouldHandle = !HttpServer.SingletonInstance.IsListening;
				}
				*/

				if (url.StartsWith (IPhoneServiceLocator.APPVERSE_SERVICE_URI) 
					|| url.StartsWith (IPhoneServiceLocator.APPVERSE_RESOURCE_URI) 
					|| url.StartsWith (IPhoneServiceLocator.APPVERSE_DOCUMENT_URI)) {
					//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol - canInitWithRequest: MANAGED SERVICE: " + request.Url.ToString());
					shouldHandle = true;
					// Appverse 5.0 will serve directly the service response (see StartLoading() method)
				}

				if ((url.StartsWith ("file://") || url.StartsWith ("data:")) && !IPhoneServiceLocator.CurrentDelegate.SecurityChecksPassed()) {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol Loading error page resources");
					shouldHandle = false;  // let the webview to handle the error pages from local filesystem
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
			//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol MANAGED SERVICE - Start Loading Content - HttpMethod: " + Request.HttpMethod);

			NSMutableUrlRequest newRequest = (NSMutableUrlRequest)Request.MutableCopy ();  // change to mutable in case we need to change something
			string requestUrl = null;
			if (newRequest != null && newRequest.Url != null)
				requestUrl = newRequest.Url.ToString ();
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol - Start Loading Content - Request URl: " + requestUrl);

			/* TO BE REMOVED - 5.0.6 [AMOB-30]
			if (Request.HttpMethod == "OPTIONS")
			{
				//In case of an OPTIONS, we allow the access to the origin of the petition
				var vlsOrigin = Request.Headers["ORIGIN"];
				//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol - StartLoading: vlsOrigin: " + vlsOrigin);

				NSMutableDictionary headers = new NSMutableDictionary ();
				NSHttpUrlResponse Response = new NSHttpUrlResponse (Request.Url, 403, "1.1", headers);

				// if (vlsOrigin!=null && vlsOrigin.ToString() == String.Format(IPhoneServiceLocator.INTERNAL_SERVER_URL, IPhoneServiceLocator.CurrentDelegate.GetListeningPort()) ) {
				if (vlsOrigin!=null && vlsOrigin.ToString() == IPhoneServiceLocator.APPVERSE_HOST ) {
					headers.Add (new NSString ("Access-Control-Allow-Origin"), vlsOrigin);
					headers.Add (new NSString ("Access-Control-Allow-Methods"), new NSString ("POST"));
					headers.Add (new NSString ("Access-Control-Allow-Headers"), new NSString ("accept, content-type"));
					headers.Add (new NSString ("Access-Control-Max-Age"), new NSString ("1728000"));

					Response = new NSHttpUrlResponse (Request.Url, 200, "1.1", headers);
				} else {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol - StartLoading: WARNING - origin not allowed: " + vlsOrigin);
				}

				Client.ReceivedResponse(this, Response, NSUrlCacheStoragePolicy.NotAllowed);

			} else 
			*/

			if (requestUrl != null) {

				if (Request.HttpMethod == "POST") {


					string serviceName = "undefined";
					string methodName = "undefined";
					Object service = null;

					if (requestUrl.StartsWith (IPhoneServiceLocator.APPVERSE_SERVICE_URI)) {
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol MANAGED SERVICE");
						// get service path
						string commandParams = requestUrl.Substring (IPhoneServiceLocator.APPVERSE_SERVICE_URI.Length);
						//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol - commandParams: " + commandParams);
						string[] commandParamsArray = commandParams.Split (new char[] { '/' });
						serviceName = commandParamsArray [0];
						methodName = commandParamsArray [1];
						//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol - serviceName: " + serviceName + ", methodName: " + methodName);
						service = serviceLocator.GetService (serviceName);

						String query = null;
						if (newRequest.Body != null) {
							SystemLogger.Log (SystemLogger.Module.PLATFORM, 
								"# IPhoneNSUrlProtocol - query data [ONLY FOR TESTING PURPOSES, PLEASE REMOVE THIS TRACE FROM LOG] request length: " + newRequest.Body.Length);

							query = NSString.FromData (newRequest.Body, NSStringEncoding.UTF8);
						}

						SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol - sending Async POST result for service: " + serviceName + ", and method: " + methodName);
						serviceURIHandler.ProcessAsyncPOSTResult (serviceInvocationManager, service, methodName, query);

						NSData nsDataResponse = NSData.FromString ("MANAGED SERVICE (data returned using callback or global listener)");
						NSUrlResponse response = new NSUrlResponse (newRequest.Url, "text/html", (nint)nsDataResponse.Length, "utf-8");
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol MANAGED SERVICE - Service executed (data will be returned using callback or global listener)");
						Client.ReceivedResponse (this, response, NSUrlCacheStoragePolicy.NotAllowed);
						Client.DataLoaded (this, nsDataResponse);

					}

				} else if (Request.HttpMethod == "GET") {
					if (requestUrl.StartsWith (IPhoneServiceLocator.APPVERSE_RESOURCE_URI)) {
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol MANAGED WEB RESOURCE");

						String resourcePath = newRequest.Url.Path;
						String mimeType = resourceURIHandler.GetWebResourceMimeType (resourcePath, true);
						NSData nsDataResponse = resourceURIHandler.ProcessWebResource (resourcePath);
						if (nsDataResponse == null) {
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol MANAGED WEB RESOURCE no data bytes - " + requestUrl);
							nsDataResponse = NSData.FromString ("MANAGED WEB RESOURCE : no data bytes");
						}
						NSUrlResponse response = new NSUrlResponse (newRequest.Url, mimeType, (nint)nsDataResponse.Length, "utf-8");
						Client.ReceivedResponse (this, response, NSUrlCacheStoragePolicy.NotAllowed);
						Client.DataLoaded (this, nsDataResponse);

					} else if (requestUrl.StartsWith (IPhoneServiceLocator.APPVERSE_DOCUMENT_URI)) {

						SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol MANAGED DOCUMENT RESOURCE");

						String resourcePath = newRequest.Url.Path.Substring (IPhoneServiceLocator.APPVERSE_DOCUMENT_PATH.Length);
						String mimeType = resourceURIHandler.GetWebResourceMimeType (resourcePath.ToLower (), false);
						NSData nsDataResponse = resourceURIHandler.ProcessDocumentResource (resourcePath);
						if (nsDataResponse == null) {
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneNSUrlProtocol MANAGED DOCUMENT RESOURCE no data bytes - " + requestUrl);
							nsDataResponse = NSData.FromString ("MANAGED DOCUMENT RESOURCE : no data bytes");
						}
						NSUrlResponse response = new NSUrlResponse (newRequest.Url, mimeType, (nint)nsDataResponse.Length, "utf-8");
						Client.ReceivedResponse (this, response, NSUrlCacheStoragePolicy.NotAllowed);
						Client.DataLoaded (this, nsDataResponse);
					}
				}
			}

			Client.FinishedLoading (this);
		}

		public override void StopLoading ()
		{
		}

	}

}
