using System;
using WebKit;
using Foundation;
using Unity.Core.System;
using Unity.Core.System.Service;

namespace Unity.Platform.IPhone
{
	public class IPhoneWKScriptMessageHandler : WKScriptMessageHandler
	{

		IPhoneServiceURIHandler serviceURIHandler = null;
		ServiceInvocationManager serviceInvocationManager = null;
		IServiceLocator serviceLocator = null;
	
		public IPhoneWKScriptMessageHandler ()
		{
			serviceLocator = IPhoneServiceLocator.GetInstance ();
			serviceURIHandler = new IPhoneServiceURIHandler (serviceLocator);
			serviceInvocationManager = new ServiceInvocationManager ();
		}

		public override void DidReceiveScriptMessage (WKUserContentController userContentController, WKScriptMessage message)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneWKScriptMessageHandler - DidReceiveScriptMessage");

			try {
				NSDictionary body = message.Body as NSDictionary;
				
				string requestUrl = null;
				string query = null;
				
				string serviceName = "undefined";
				string methodName = "undefined";
				Object service = null;
				
				if (body.ContainsKey (new NSString ("uri"))) {
					requestUrl = (body ["uri"]).ToString ();
				
					// get service path
					if (requestUrl != null) {
						if (requestUrl.StartsWith (IPhoneServiceLocator.APPVERSE_SERVICE_URI)) {
							string commandParams = requestUrl.Substring (IPhoneServiceLocator.APPVERSE_SERVICE_URI.Length);
							//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneWKScriptMessageHandler - commandParams: " + commandParams);
							string[] commandParamsArray = commandParams.Split (new char[] { '/' });
							serviceName = commandParamsArray [0];
							methodName = commandParamsArray [1];
							//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneWKScriptMessageHandler - serviceName: " + serviceName);
							//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneWKScriptMessageHandler - methodName: " + methodName);
							service = serviceLocator.GetService (serviceName);
						}
					}
				
				}
				
				if (body.ContainsKey (new NSString ("query"))) {
					query = (body ["query"]).ToString ();
					//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneWKScriptMessageHandler - query: " + query);
				}
				
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneWKScriptMessageHandler - sending Async POST result for service: " + serviceName + ", and method: " + methodName);
				
				serviceURIHandler.ProcessAsyncPOSTResult (serviceInvocationManager, service, methodName, query);

			} catch (Exception ex) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneWKScriptMessageHandler - exception handling WKScriptMessage. Exception message: " + ex.Message); 
			}

		}
	}
}

