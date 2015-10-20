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
using Unity.Core.System;
using System.Xml.Serialization;
using System.IO;
using Adform;

namespace Appverse.Platform.IPhone
{
	public class IPhoneAdform : WeakDelegateManager, IAdform
	{

		private AdformInitialization _initOptions;

		public IPhoneAdform ()
		{
		}

		#region IAdform implementation

		public void SendTrackPoint (AdformTrackPoint adFormTrackPoint)
		{
			
			if (_initOptions == null) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Adform [SendTrackPoint] - Error: no init options loaded, please check if you have the 'adform-config.xml' file in your project");
				return;
			}

			try {
				nint tpID = _initOptions.TrackingID;

				TrackPoint trackPoint = new TrackPoint(tpID);
				trackPoint.SetSectionName(adFormTrackPoint.SectionName);

				if(adFormTrackPoint.CustomParameters != null) {
					foreach (AdformCustomParameter cp in adFormTrackPoint.CustomParameters) {
						trackPoint.AddParameter(cp.Name, cp.Value);
					}
				}

				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Adform [SendTrackPoint] to id: " + tpID);

				AdformTrackingSDK.SharedInstance ().SendTrackPoint(trackPoint);

			} catch (Exception ex) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "EXCEPTION Adform SendTrackPoint... ", ex);
			}
		}

		#endregion

		#region IWeakDelegateManager implementation

		public override void FinishedLaunching (UIKit.UIApplication application, Foundation.NSDictionary launchOptions)
		{
			try {

				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Adform Start tracking [" + _initOptions.TrackingID + "]...");
				AdformTrackingSDK.SharedInstance ().StartTracking (_initOptions.TrackingID);


				string sectionName = "Download";
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Adform Tracking Point with Section [" + sectionName + "]...");

				AdformTrackPoint tPoint = new AdformTrackPoint();
				tPoint.SectionName = sectionName;
				
				this.SendTrackPoint(tPoint);

			} catch (Exception ex) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "EXCEPTION Adform Start tracking... ", ex);
			}
		}

		public override string GetConfigFilePath ()
		{
			return "app/config/adform-config.xml";	
		}

		public override void ConfigFileLoadedData (byte[] configData)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "IPhoneAdform Module Initialization... loading init options");

			try
			{   // FileStream to read the XML document.
				if (configData != null)
				{
					XmlSerializer serializer = new XmlSerializer(typeof(AdformInitialization));
					_initOptions = (AdformInitialization)serializer.Deserialize(new MemoryStream(configData));
				}
			}
			catch (Exception e)
			{
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error when loading adform configuration", e);
			}
		}

		#endregion
	}
}

