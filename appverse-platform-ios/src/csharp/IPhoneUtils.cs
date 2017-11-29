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
		private string _RZ = "$ResourcesZipped$";
		private ZipFile _zipFile = null;
		private string[] _zipEntriesNames = null;
		private Dictionary<string, ZipEntry> _zipEntries = null; 
		private JavaScriptSerializer Serialiser = null;

		private static IPhoneUtils singleton = null;
		private IPhoneUtils() : base() {
		}

		public bool ResourcesZipped { 
			//Proprietary code
			return false;
		}

		
		public static IPhoneUtils GetInstance() {
			if (singleton==null) {
				singleton = new IPhoneUtils();

				singleton.Serialiser = new JavaScriptSerializer ();

				if(singleton.ResourcesZipped) {
					// testing removing disk and memory cache capacity
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "# Removing cache disk and memory capacity");
					NSUrlCache.SharedCache.DiskCapacity = 0;
					NSUrlCache.SharedCache.MemoryCapacity = 0;
				}
			}
			return singleton;
		}

		public string[] GetFilesFromDirectory(string directoryPath, string filePattern) {
			//Proprietary code
			return null;
		}


		public bool ResourceExists(string resourcePath) {
			//Proprietary code
			return false;
			
		}

		private bool ResourceFromZippedExists(string resourcePath) {

			//Proprietary code
			return false;
		}


		public static byte[] ConvertNonSeekableStreamToByteArray(Stream nonSeeakable) {
			//Proprietary code
			return null;

		}


		public NSUrl GetNSUrlFromPath(string path) {
			//Proprietary code
			return null;
		}
		
		public string GetFileFullPath(string filePath) {
			//Proprietary code
			return "";
		}

		public Stream GetResourceAsStream(string resourcePath) 
		{
			//Proprietary code
			return null;
		}

		public byte[] GetResourceAsBinary(string resourcePath, bool isWebResource)  {
			//Proprietary code
			return null;
		}
		
		
		public bool IsIPad() {
			//Proprietary code
			return false;
		}
		
		public string GetUserAgent() {
			//Proprietary code
			return "";
		}
		
		
		public bool ShouldAutorotate() {
			//Proprietary code
			return false;
		}

		public Dictionary<String,Object> ConvertToDictionary(NSMutableDictionary dictionary) {
			//Proprietary code
			return null;
		}

		public NSDictionary ConvertToNSDictionary(Dictionary<String,Object> dictionary) {
			//Proprietary code
			return null;
		}

		public String JSONSerialize(object data) {
			//Proprietary code
			return "";
		}


		public String JSONSerializeObjectData(object data) {
			//Proprietary code
			return "";
		}

		public T JSONDeserialize<T> (string json) {
			//Proprietary code
			return null;
		}

		public void FireUnityJavascriptEvent (string method, object data)
		{
			//Proprietary code
		}

		public void FireUnityJavascriptEvent (string method, object[] dataArray)
		{
			//Proprietary code
		}

		public void ExecuteJavascriptCallback(string callbackFunction, string id, string jsonResultString) {
			//Proprietary code
			
		}

		public string GetDefaultBasePath ()
		{
			//Proprietary code
			return "";
		}


		public static DateTime NSDateToDateTime(NSDate date)
		{
			//Proprietary code
			return null;
		}

		public static NSDate DateTimeToNSDate(DateTime date)
		{
			//Proprietary code
			return null;
		}
	}
}
