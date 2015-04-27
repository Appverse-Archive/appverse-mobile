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
using System;
using Unity.Core.IO.ScriptSerialization;
using UIKit;
using WebKit;
using Foundation;
using System.Collections.Generic;

namespace Appverse.Core.PushNotifications
{
	public class PushNotificationsUtils
	{
		public PushNotificationsUtils ()
		{
		}

		public static void FireUnityJavascriptEvent (string method, object data)
		{

			UIViewController viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;

			JavaScriptSerializer Serialiser = new JavaScriptSerializer (); 
			string dataJSONString = "null";
			if (data != null) {
				dataJSONString = Serialiser.Serialize (data);
				if (data is String) {
					dataJSONString = "'" + (data as String) + "'";
				}
			}
			string jsCallbackFunction = "if("+method+"){"+method+"("+dataJSONString+");}";
			//only for testing 
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "NotifyJavascript (single object): " + method + ", dataJSONString: " + dataJSONString);

			bool webViewFound = false;
			if (viewController != null && viewController.View != null) {

				UIView[] subViews = viewController.View.Subviews;

				foreach(UIView subView in subViews) {
					if (subView is UIWebView) {
						webViewFound = true;

						// evaluate javascript as a UIWebView
						(subView as UIWebView).EvaluateJavascript (jsCallbackFunction);

					} else if (subView is WKWebView) {
						webViewFound = true;

						// evaluate javascript as a WKWebView
						(subView as WKWebView).EvaluateJavaScript (new NSString(jsCallbackFunction), delegate (NSObject result, NSError error) {
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "NotifyJavascript COMPLETED (" + method + ")");
						});
					}
				}
			} 

			if (webViewFound) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "NotifyJavascript EVALUATED (" + method + ")");
			} else {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "It was not possible to find a WebView to evaluate the javascript method");
			}

		}

		public static String JSONSerialize(object data) {
			try {
				JavaScriptSerializer Serialiser = new JavaScriptSerializer (); 
				return Serialiser.Serialize(data);
			} catch (Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error trying to serialize object to JSON.", ex);
			}
			return null;
		}

		public static Dictionary<String,Object> ConvertToDictionary(NSMutableDictionary dictionary) {
			Dictionary<String,Object> prunedDictionary = new Dictionary<String,Object>();
			foreach (NSString key in dictionary.Keys) { 
				NSObject dicValue = dictionary.ObjectForKey(key);  

				if (dicValue is NSDictionary) {
					prunedDictionary.Add (key.ToString(), ConvertToDictionary(new NSMutableDictionary(dicValue as NSDictionary)));
				} else {
					//SystemLogger.Log(SystemLogger.Module.PLATFORM, "***** key["+key.ToString ()+"] is instance of: " + dicValue.GetType().FullName);
					if ( ! (dicValue is NSNull)) {
						if(dicValue is NSString) {
							prunedDictionary.Add (key.ToString(), ((NSString)dicValue).Description);
						} else if(dicValue is NSNumber) {
							prunedDictionary.Add (key.ToString(), ((NSNumber)dicValue).Int16Value);
						} else if(dicValue is NSArray) {
							prunedDictionary.Add (key.ToString(), ConvertToArray((NSArray)dicValue));
						} else {
							prunedDictionary.Add (key.ToString(), dicValue);
						}
					} 
				}
			}
			return prunedDictionary;
		}

		public NSDictionary ConvertToNSDictionary(Dictionary<String,Object> dictionary) {
			NSMutableDictionary prunedDictionary = new NSMutableDictionary();
			foreach (String key in dictionary.Keys) { 
				Object dicValue = dictionary[key];  

				if (dicValue is Dictionary<String,Object>) {
					prunedDictionary.Add (new NSString(key), ConvertToNSDictionary((dicValue as Dictionary<String,Object>)));
				} else {
					//SystemLogger.Log(SystemLogger.Module.PLATFORM, "***** key["+key+"] is instance of: " + dicValue.GetType().FullName);
					if (dicValue != null) {
						if(dicValue is String) {
							prunedDictionary.Add (new NSString(key), new NSString((dicValue as String)));
						} else if(dicValue is int) {
							prunedDictionary.Add (new NSString(key), new NSNumber((int) dicValue));
						} else if(dicValue is Object[]) {
							prunedDictionary.Add (new NSString(key), ConvertToNSArray((Object[])dicValue));
						} else if(dicValue is System.Collections.ArrayList) {
							prunedDictionary.Add (new NSString(key), ConvertToNSArray((dicValue as System.Collections.ArrayList).ToArray()));
						}else {
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "*** exception parsing key["+key+"] instance of: " + dicValue.GetType().FullName +". No complex object are valid inside this dictionary");
						}
					} 
				}
			}
			return prunedDictionary;
		}

		private static Object[] ConvertToArray(NSArray nsArray) {
			Object[] arr = new Object[nsArray.Count];
			for (uint i = 0; i < nsArray.Count; i++) {
				var o = ObjCRuntime.Runtime.GetNSObject(nsArray.ValueAt(i));
				if(o is NSArray) {
					arr[i] = ConvertToArray((o as NSArray));
				} else if (o is NSDictionary) {

				} else if (o is NSMutableDictionary) {
					arr[i] = ConvertToDictionary((o as NSMutableDictionary));
				} if(o is NSString) {
					arr[i] = (o as NSString).Description;
				} else if(o is NSNumber) {
					arr[i] = (o as NSNumber).Int16Value;
				}
			}

			return arr;
		}

		private NSArray ConvertToNSArray(Object[] nsArray) {
			return NSArray.FromObjects(nsArray);
		}

	}
}

