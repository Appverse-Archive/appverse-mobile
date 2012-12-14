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
using Unity.Core.System.Service;
using MonoTouch.Foundation;
using Unity.Core.System;
using MonoTouch.UIKit;

namespace Unity.Platform.IPhone
{
	public class IPhoneServiceURIHandler : ServiceURIHandler
	{
		public IPhoneServiceURIHandler (IServiceLocator _serviceLocator) : base(_serviceLocator)
		{
		}

		/// <summary>
		/// Processes the service asynchronously. Using NSAutoreleasePool monotouch class.
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
		protected override void ProcessServiceAsynchronously(string callbackFunction, string id, IInvocationManager invocationManager, Object service, string methodName, string query) {
			using (var pool = new NSAutoreleasePool ()) {
				base.ProcessServiceAsynchronously(callbackFunction, id, invocationManager, service, methodName, query);
			};
		}

		/// <summary>
		/// Sends the back result, by evaluating javascript inside UIWebview component (must be invoked on main thread)
		/// </summary>
		/// <param name='callbackFunction'>
		/// Callback function.
		/// </param>
		/// <param name='id'>
		/// Identifier.
		/// </param>
		/// <param name='jsonResultString'>
		/// Json result string.
		/// </param>
		protected override void SendBackResult(string callbackFunction, string id, string jsonResultString) {
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				SystemLogger.Log (SystemLogger.Module.PLATFORM , " ############## sending back result to callback fn [" + callbackFunction+ "] and id [" + id+ "]: " +
				                  (jsonResultString!=null?jsonResultString.Length:0));
				IPhoneUtils.GetInstance().ExecuteJavascriptCallback(callbackFunction, id, jsonResultString);
			});
		}
	}
}

