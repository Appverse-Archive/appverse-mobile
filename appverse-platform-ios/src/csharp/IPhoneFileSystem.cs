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
using System.IO;
using System.Reflection;
using System.Text;
using Unity.Core.Storage.FileSystem;
using Unity.Core.System;
using MonoTouch.Foundation;
using System.Runtime.InteropServices;
using MonoTouch.UIKit;

namespace Unity.Platform.IPhone
{
    public class IPhoneFileSystem : AbstractFileSystem
    {
		// Root path is the "Documents" folder on the application root path on device.
		//public static string DEFAULT_ROOT_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		
		// Path to the "Resources" folder on the application root path on device.
		public static string DEFAULT_RESOURCES_PATH = "WebResources/";
		
		/// <summary>
		/// Gets the Directory Root path.
		/// </summary>
		/// <returns>
		/// A <see cref="DirectoryData"/>
		/// </returns>
        public override DirectoryData GetDirectoryRoot()
        {

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var documents = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0];
				Console.WriteLine ("JAVIIIIII 8 : "+documents.ToString());
				return new DirectoryData(documents.ToString());
			} else {
				var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				Console.WriteLine ("JAVIIIIII 7: "+documents);
				return new DirectoryData(documents);
			};
            //return new DirectoryData(DEFAULT_ROOT_PATH);
        }
		
		/// <summary>
		/// Gets the Directory Resources path.
		/// </summary>
		/// <returns>
		/// A <see cref="DirectoryData"/>
		/// </returns>
		public override DirectoryData GetDirectoryResources ()
		{
			return new DirectoryData(IPhoneUtils.GetInstance().GetFileFullPath(DEFAULT_RESOURCES_PATH));
		}
		
		public override bool CopyFromRemote (string url, string toPath)
		{
			try {
				// delete file if already exists
				string path = Path.Combine(this.GetDirectoryRoot().FullName,toPath);
				//TODO Review
				if (!CheckSecurePath (path))
					return false;
				NSUrl nsurl = new NSUrl(url);
				NSData data = NSData.FromUrl(nsurl);
				if(data != null) {
					// remove previous existing file
					if(File.Exists(path)) {
						this.DeleteFile(new FileData(path));
					}
					// create an empty file
					FileData toFile = this.CreateFile(toPath);

					byte[] buffer = new byte[data.Length];
					Marshal.Copy(data.Bytes, buffer,0,buffer.Length);
					// write contents and return result
					return this.WriteFile(toFile, buffer, false);
				} 
				// don't create/replace any file if NSData couldn't be obtained from remote path
			} catch (Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error copying from [" + url + "] to file [" + toPath + "]", ex);

			}
			return false;
		}
    }
}
