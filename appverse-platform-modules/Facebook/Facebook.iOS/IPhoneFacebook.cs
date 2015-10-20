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
using Unity.Core;
using UIKit;
using Foundation;
using System.Xml.Serialization;
using System.IO;
using Unity.Core.System;
using Facebook.CoreKit;


namespace Appverse.Platform.IPhone
{
	public class IPhoneFacebook : WeakDelegateManager
	{
		public IPhoneFacebook ()
		{
		}

		private FacebookInit _init;

		#region IWeakDelegateManager implementation

		public override void FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "************** [Facebook_Module] FinishedLaunching ... Facebook Init Settings");

			Facebook_InitSettings ();

		}

		public override string GetConfigFilePath ()
		{
			return "app/config/facebook-config.xml";
		}


		public override void ConfigFileLoadedData (byte[] configData)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "IPhoneAppsFlyer Module Initialization... loading init options");

			try
			{   // FileStream to read the XML document.
				if (configData != null)
				{
					XmlSerializer serializer = new XmlSerializer(typeof(FacebookInit));
					_init = (FacebookInit)serializer.Deserialize(new MemoryStream(configData));
				}
			}
			catch (Exception e)
			{
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error when loading appsflyers configuration", e);
			}
		}

		public override void OnActivated (UIApplication application)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "************** [Facebook_Module] OnActivated... Facebook Activate App");

			Facebook_ActivateApp ();
		}

		#endregion

		private void Facebook_InitSettings() {
			try {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Facebook Settings [AppID = " + _init.FacebookAppId + ", DisplayName= " + _init.FacebookDisplayName + "]");

				Settings.AppID = _init.FacebookAppId;
				Settings.DisplayName = _init.FacebookDisplayName;
			} catch (Exception ex) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "EXCEPTION Facebook Settings", ex);
			}
		}

		private void Facebook_ActivateApp() {
			try {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Facebook Activate App [ " + Settings.AppID + " ] ...");

				AppEvents.ActivateApp();

				//FBAppCall.HandleDidBecomeActive();

			} catch (Exception ex) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "EXCEPTION Activating App for Facebook  [" + Settings.AppID + "]: ", ex);
			}
		}

	}
}

