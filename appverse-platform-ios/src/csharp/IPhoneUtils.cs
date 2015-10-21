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
using System.IO;
using System.Runtime.InteropServices;
using Foundation;
using UIKit;
using Unity.Core.IO.ScriptSerialization;
using Unity.Core.System;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;

namespace Unity.Platform.IPhone
{

	public class IPhoneUtils
	{
		
		private JavaScriptSerializer Serialiser = null;

		private static IPhoneUtils singleton = null;
		private IPhoneUtils() : base() {
		}



		private void LoadZippedFile() {
			
		}


		
		public static IPhoneUtils GetInstance() {
			if (singleton==null) {
				singleton = new IPhoneUtils();

				singleton.Serialiser = new JavaScriptSerializer ();


			}
			return singleton;
		}

		public string[] GetFilesFromDirectory(string directoryPath, string filePattern) {
			
			return Directory.GetFiles (directoryPath, filePattern);

		}


		public bool ResourceExists(string resourcePath) {
			//SystemLogger.Log (SystemLogger.Module.PLATFORM, "# Checking file exists: " + resourcePath);

			return File.Exists(resourcePath);

			
		}

		private bool ResourceFromZippedExists(string resourcePath) {

			return false;
		}



		public static byte[] ConvertNonSeekableStreamToByteArray(Stream nonSeeakable) {
			if(nonSeeakable is MemoryStream) {
				return ((MemoryStream)nonSeeakable).ToArray();
			} else {
				using(MemoryStream ms = new MemoryStream()) {
					nonSeeakable.CopyTo(ms, 32*1024);
					return ms.ToArray();
				}
			}

		}


		public NSUrl GetNSUrlFromPath(string path) {
			NSUrl nsUrl = null;
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "# Getting nsurl from path: " + path);
			try {
			
				// check resource directly from local file system
				nsUrl = NSUrl.FromFilename(path);
			

			} catch (Exception e) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "# Error trying to get nsurl from file [" + path +"]", e);
			}
			
			return nsUrl;
		}
		
		public string GetFileFullPath(string filePath) {
			
			return Path.Combine(NSBundle.MainBundle.BundlePath, filePath);

		}

		private Stream GetResourceAsStreamFromFile(string resourcePath) 
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "# Loading resource from file: " + resourcePath);
			
			MemoryStream bufferStream = new MemoryStream();
			//Monitor.Enter(this);
			
			try {
				
				NSData data = NSData.FromFile(resourcePath);
				byte[] buffer = new byte[data.Length];
				Marshal.Copy(data.Bytes, buffer,0,buffer.Length);
				bufferStream.Write(buffer,0,buffer.Length);
				
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Resource Stream length:" + bufferStream.Length);
				
			} catch (Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error trying to get file stream [" + resourcePath +"]", ex);
			}
			//Monitor.Exit(this);
			return bufferStream;
		}
		
		public Stream GetResourceAsStream(string resourcePath) 
		{
			
			return this.GetResourceAsStreamFromFile(resourcePath);

		}

		public byte[] GetResourceAsBinary(string resourcePath, bool isWebResource)  {
			
			return this.GetResourceAsBinaryFromFile(resourcePath);

		}
		
		private byte[] GetResourceAsBinaryFromFile(string resourcePath) 
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "# Loading resource from file: " + resourcePath);

			try {
				//Stopwatch stopwatch = new Stopwatch();
				//stopwatch.Start();

				NSData data = NSData.FromFile(resourcePath);

				if(data == null) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "File not available at path: " + resourcePath);
				} else {
					byte[] buffer = new byte[data.Length];
					Marshal.Copy(data.Bytes, buffer,0,buffer.Length);

					return buffer;
				}

				//stopwatch.Stop();
				//SystemLogger.Log(SystemLogger.Module.PLATFORM, "CSV not-zipped," + resourcePath + ","+ stopwatch.ElapsedMilliseconds);

			} catch (Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error trying to get file binary data [" + resourcePath +"]", ex);
			}

			return new byte[0];
		}
		
		public bool IsIPad() {
			IOperatingSystem iOS = (IOperatingSystem) IPhoneServiceLocator.GetInstance ().GetService ("system");
			HardwareInfo hInfo =  iOS.GetOSHardwareInfo();
			if(hInfo != null && hInfo.Version != null && hInfo.Version.Contains("iPad")) {
				return true;
			}
			
			return false;
		}
		
		public string GetUserAgent() {
			IOperatingSystem iOS = (IOperatingSystem) IPhoneServiceLocator.GetInstance ().GetService ("system");
			return iOS.GetOSUserAgent();
		}
		
		
		public bool ShouldAutorotate() {
			try {
				IDisplay service = (IDisplay) IPhoneServiceLocator.GetInstance().GetService("system");
				return !service.IsOrientationLocked();
			} catch (Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error trying to determine if application should autorotate", ex);
			}
			return true; // by default
		}

		public Dictionary<String,Object> ConvertToDictionary(NSMutableDictionary dictionary) {
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

		private Object[] ConvertToArray(NSArray nsArray) {
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

		public String JSONSerialize(object data) {
			try {

				return Serialiser.Serialize(data);
			} catch (Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error trying to serialize object to JSON.", ex);
			}
			return null;
		}


		public String JSONSerializeObjectData(object data) {
			try {
				string dataJSONString = Serialiser.Serialize (data);
				if (data is String) {
					dataJSONString = "'"+ (data as String) +"'";
				}
				return dataJSONString;
			} catch (Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error trying to serialize object to JSON. Message; " + ex.Message );
			}
			return null;
		}

		public T JSONDeserialize<T> (string json) {
			try {
				return Serialiser.Deserialize<T>(json);
			} catch (Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error trying to serialize object to JSON.", ex);
			}
			return default(T);
		}

		public void FireUnityJavascriptEvent (string method, object data)
		{
			string dataJSONString = "null";
			if (data != null) {
				dataJSONString = Serialiser.Serialize (data);
				if (data is String) {
					dataJSONString = "'" + (data as String) + "'";
				}
			}
			string jsCallbackFunction = "if("+method+"){"+method+"("+dataJSONString+");}";
			//only for testing //SystemLogger.Log(SystemLogger.Module.PLATFORM, "NotifyJavascript (single object): " + method + ", dataJSONString: " + dataJSONString);
			IPhoneServiceLocator.CurrentDelegate.EvaluateJavascript(jsCallbackFunction);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "NotifyJavascript EVALUATED (" +  method +")");
		}

		public void FireUnityJavascriptEvent (string method, object[] dataArray)
		{
			string dataJSONString = "null";
			if (dataArray != null) {
				StringBuilder builder = new StringBuilder ();
				int numObjects = 0;
				foreach (object data in dataArray) {
					if (numObjects > 0) {
						builder.Append (",");
					}
					if (data == null) {
						builder.Append ("null");
					}
					if (data is String) {
						builder.Append ("'" + (data as String) + "'");
					} else {
						builder.Append (Serialiser.Serialize (data));
					}
					numObjects++;
				}
				dataJSONString = builder.ToString ();
			}
			string jsCallbackFunction = "if("+method+"){"+method+"("+dataJSONString+");}";
			//only for testing //SystemLogger.Log(SystemLogger.Module.PLATFORM, "NotifyJavascript (array object): " + method + ", dataJSONString: " + dataJSONString); // + ",jsCallbackFunction="+ jsCallbackFunction 
			IPhoneServiceLocator.CurrentDelegate.EvaluateJavascript(jsCallbackFunction);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "NotifyJavascript EVALUATED (" +  method +")");
		}

		public void ExecuteJavascriptCallback(string callbackFunction, string id, string jsonResultString) {
			string jsCallbackFunction = "try{if("+callbackFunction+"){"+callbackFunction+"("+jsonResultString+", '"+ id +"');}}catch(e) {console.log('error executing javascript callback: ' + e)}";
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "ExecuteJavascriptCallback: " + callbackFunction);
			IPhoneServiceLocator.CurrentDelegate.EvaluateJavascript(jsCallbackFunction);
		}

		public string GetDefaultBasePath ()
		{
			string s = Assembly.GetEntryAssembly ().Location;
			string defaultBasePath = s;
			if (s != null) {
				int n = s.Length;
				int a = s.IndexOf (".app/");  
				if(a>=0) { // applied only for special binaries
					defaultBasePath = s.Substring (0, (a + ".app/".Length));
				}
			}
			return Path.GetDirectoryName (defaultBasePath);
		}


		public static DateTime NSDateToDateTime(NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime( 
				new DateTime(2001, 1, 1, 0, 0, 0) );
			return reference.AddSeconds(date.SecondsSinceReferenceDate);
		}

		public static NSDate DateTimeToNSDate(DateTime date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0) );
			return NSDate.FromTimeIntervalSinceReferenceDate(
				(date - reference).TotalSeconds);
		}
	}
}
