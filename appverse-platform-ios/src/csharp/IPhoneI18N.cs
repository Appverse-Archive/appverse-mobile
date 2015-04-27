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
using Unity.Core.I18N;
using System.Collections.Generic;
using Unity.Core.System;
using Foundation;
using System.Xml;

namespace Unity.Platform.IPhone
{

	public class IPhoneI18N : AbstractI18N
	{
		// All methods should be implemented on Abstract class, as they are not platform dependent.
		
		public IPhoneI18N() : base()
		{
		}
		
		public override string I18NConfigFile
		{
			get
			{
				return IPhoneUtils.GetInstance().GetFileFullPath(base.I18NConfigFile);
			}
		}

		protected override bool FileExists(string filePath) {
			return IPhoneUtils.GetInstance().ResourceExists(filePath);
		}

		protected override string[] GetDirectoryFiles(string directoryPath, string filePattern) {
			return IPhoneUtils.GetInstance().GetFilesFromDirectory(directoryPath, filePattern);
		}

		protected override XmlTextReader getXmlTextReader(string textFilePath) {
			if(IPhoneUtils.GetInstance().ResourcesZipped) {
				Stream stream = IPhoneUtils.GetInstance().GetResourceAsStream(textFilePath);
				return new XmlTextReader(stream);
				
			} else {
				return base.getXmlTextReader(textFilePath);
			}
		}

		protected override string readFromPlistFile (string file, string key)
		{

			if(IPhoneUtils.GetInstance().ResourcesZipped) {
				// if resource is zipped, we will need to parse the file as an xml file (done in abstract class)
				return base.readFromPlistFile(file, key);
			} else {
				string result = string.Empty;
				if (this.FileExists (file)) {
					NSDictionary resourcesLiteral = loadResourcesLiteral (file);
					if (resourcesLiteral != null) {
						result = getResourceLiteralValue(key, file, resourcesLiteral);
					}
					result = (result ==  null || result == string.Empty) ? String.Format ("&lt;{0}&gt;", key) : result;
				} else {
					// if file does not exists, means that requested locale is not supported by application
					// try then to get default locale string
					return this.GetResourceLiteral(key);
				}
				return result;
			}
		}


		protected override ResourceLiteralDictionary readAllFromPlistFile (string file)
		{
			if(IPhoneUtils.GetInstance().ResourcesZipped) {
				// if resource is zipped, we will need to parse the file as an xml file (done in abstract class)
				return base.readAllFromPlistFile(file);
			} else {
				ResourceLiteralDictionary result = null;
				if (this.FileExists (file)) {
					result = new ResourceLiteralDictionary();
					NSDictionary literalDictionary = loadResourcesLiteral(file);
					foreach(NSObject key in literalDictionary.Keys)
					{
						result.Add(key.ToString(),literalDictionary[key].ToString());
					}
					return result;
				} else {
					// if file does not exists, means that requested locale is not supported by application
					// try then to get default locale string
					return this.GetResourceLiterals();
				}		
			}
		}
		
		private NSDictionary loadResourcesLiteral (string fullPathFilePlist)
		{
			NSDictionary resourcesLiteral = null;
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "# loading plist: " + fullPathFilePlist);
			if (this.FileExists (fullPathFilePlist)) {
				resourcesLiteral = NSDictionary.FromFile (fullPathFilePlist);
			}
			return resourcesLiteral;
		}
		
		private string getResourceLiteralValue (string key, string fullPathFilePlist, NSDictionary resourcesLiteral)
		{
			string resourceLiteralValue = string.Empty;
			try {
				resourceLiteralValue = ((NSString)resourcesLiteral.ObjectForKey (new NSString (key)));
			}
			catch (Exception e) {
				#if DEBUG
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Key:" + key + " not correctly configured on " + fullPathFilePlist + "Exception message:" + e.Message);
				#endif
			}
			return resourceLiteralValue;
		}

		
	}
}
