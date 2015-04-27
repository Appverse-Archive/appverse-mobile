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
using System.Text;
using Unity.Core.IO;
using System.IO;
using Unity.Core.Notification;
using Unity.Core.System;

namespace Unity.Platform.IPhone
{
    public class IPhoneIO : AbstractIO
    {   
		
		private static string _VALIDATECERTIFICATES = "$ValidateCertificates$";

		public static IDictionary<string, string[]> fingerprints; 

		public IPhoneIO()
		{
			IPhoneIO.fingerprints = GetFingerprints ();
		}

		public IDictionary<string, string[]> GetFingerprints (){
			Dictionary<string, string[]> result = new Dictionary<string, string[]> ();
			foreach(IOService serv in GetServices()){
				if (serv.Endpoint.Fingerprint != null) {
					string requestUriString = String.Format ("{0}:{1}{2}", serv.Endpoint.Host, serv.Endpoint.Port, serv.Endpoint.Path);
					if (serv.Endpoint.Port == 0) 
					{
						requestUriString = String.Format ("{0}{1}", serv.Endpoint.Host, serv.Endpoint.Path);
					}
					result.Add(requestUriString, serv.Endpoint.Fingerprint.Split(new char[]{','}));
					//SystemLogger.Log (SystemLogger.Module.CORE, "*************** serv fingerprint: " + requestUriString + " : " + serv.Endpoint.Fingerprint);
				}

			}

			removeFingerprints ();

			return result;
		}

		public bool ValidateCertificates{
			get{
				bool bResult;
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Should validate certificates for remote servers? " + IPhoneIO._VALIDATECERTIFICATES);
				Boolean.TryParse(IPhoneIO._VALIDATECERTIFICATES, out bResult);
				return bResult;
			}
		}
		
		public override string ServicesConfigFile { 
			get {
				return IPhoneUtils.GetInstance().GetFileFullPath(base.ServicesConfigFile);
			} 
		}

		public override string GetDirectoryRoot()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		}
		
		/// <summary>
		/// Method overrided, to use NSData to get stream from file resource inside application. 
		/// </summary>
		/// <returns>
		/// A <see cref="Stream"/>
		/// </returns>
		public override byte[] GetConfigFileBinaryData ()
	    {
	    	return IPhoneUtils.GetInstance().GetResourceAsBinary(this.ServicesConfigFile, true);
	    }

		public override bool ValidateWebCertificates (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
		
			if (this.ValidateCertificates) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** On ServerCertificateValidationCallback: Validate web certificates");
				return SecurityUtils.ValidateWebCertificates (sender, certificate, chain, sslPolicyErrors);
			}else return base.ValidateWebCertificates(sender, certificate, chain, sslPolicyErrors);
		}
		
		/// <summary>
		/// Method overrided, to start activity notification while invoking external service. 
		/// </summary>
		/// <param name="request">
		/// A <see cref="IORequest"/>
		/// </param>
		/// <param name="service">
		/// A <see cref="IOService"/>
		/// </param>
		/// <returns>
		/// A <see cref="IOResponse"/>
		/// </returns>
		public override IOResponse InvokeService (IORequest request, IOService service)
		{

			this.IOUserAgent = IPhoneUtils.GetInstance().GetUserAgent();
			INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance().GetService("notify");
			try { 
				notificationService.StartNotifyActivity();
			} catch(Exception e) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Cannot StartNotifyActivity. Message: " + e.Message);
			}
			IOResponse response = base.InvokeService (request, service);
			
			try { 
				notificationService.StopNotifyActivity();
			} catch(Exception e) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Cannot StopNotifyActivity. Message: " + e.Message);
			}
			
			return response;
    	}
    }
}
