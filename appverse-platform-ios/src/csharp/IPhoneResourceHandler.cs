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
using System.IO;
using Unity.Core.Storage.FileSystem;
using Unity.Core.System;
using Unity.Core.System.Resource;
using Unity.Core.System.Server.Net;
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Runtime.InteropServices;
using MonoTouch.AssetsLibrary;

namespace Unity.Platform.IPhone
{
	public class IPhoneResourceHandler : ResourceHandler
	{
		private static string DOCUMENTS_URI = "/documents/";
		
		public IPhoneResourceHandler () : base()
		{
		}

		public IPhoneResourceHandler (ApplicationSource _appSource) : base(_appSource)
		{
		}

		/// <summary>
		/// Files the exists. Uses IPhoneUtils class to check if a file exists (could be zipped or not)
		/// </summary>
		/// <returns>
		/// <c>true</c>, if exists was filed, <c>false</c> otherwise.
		/// </returns>
		/// <param name='resourcePath'>
		/// Resource path.
		/// </param>
		public override bool FileExists(string resourcePath) {
			return IPhoneUtils.GetInstance().ResourceExists(resourcePath);
		}

		protected override string GetContentFromStreamReader (string filePath)
		{
			//return base.GetContentFromStreamReader(filePath);
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneResourceHandler. Getting Content From StreamReader on file path: " + filePath);
			
			Stream sr = IPhoneUtils.GetInstance ().GetResourceAsStream (filePath);
			
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding ();
			string content = enc.GetString (((MemoryStream)sr).GetBuffer ());
			sr.Close ();
			
			return content;
		}

		protected override byte[] GetRawContentFromFileStream (string filePath)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "# IPhoneResourceHandler. Getting Raw Content From FileStream on file path: " + filePath);

			// assuming that this method is just called for accessing web resources
			return IPhoneUtils.GetInstance ().GetResourceAsBinary (filePath, true); 
		}
		
		public override bool Process (HttpServer server, HttpRequest request, HttpResponse response)
		{
			if (request.Url.StartsWith (DOCUMENTS_URI)) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, " ############## " + this.GetType () + " -> " + request.Url);
				
				string resourcePath = request.Url.Substring (DOCUMENTS_URI.Length);
				
				// Getting mime type
				string ext = Path.GetExtension (resourcePath.ToLower());
				string mime = (string)MimeTypes [ext];
				if (mime == null)
					mime = "application/octet-stream";
				response.ContentType = mime;
				
				// Getting Root Directory for Files in this platform.
				IFileSystem fileSystemService = (IFileSystem)IPhoneServiceLocator.GetInstance ().GetService ("file");
				DirectoryData rootDir = fileSystemService.GetDirectoryRoot ();
				resourcePath = Path.Combine (rootDir.FullName, resourcePath);
				
				// Get Resources as Binary Data and Write them to the server response
				response.RawContent = IPhoneUtils.GetInstance ().GetResourceAsBinary (resourcePath, false);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, " ############## " + this.GetType () + " -> file extension: " + ext + ", mimetype: " + mime + ", binary data length: " + response.RawContent.Length);
				
				return true;
				
			} else {
				return base.Process (server, request, response);
			}
		}
		
	}
}
