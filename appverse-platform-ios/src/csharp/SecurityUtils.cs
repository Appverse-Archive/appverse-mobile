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
using System.Net;
using System.IO;
using System.Linq;
using System.Net.Security;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Pkix;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509.Store;
using System.Collections;
using Unity.Core.System;
using Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.Ocsp;

using W = System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Unity.Platform.IPhone
{
	public static class SecurityUtils
	{

		private static string _VALIDATEFINGERPRINTS = "$ValidateFingerprints$";
		private static Dictionary<int, DateTime> myCertificateList = new Dictionary<int, DateTime>();

		/// <summary>
		/// Remove spaces to match the thumbprint.
		/// </summary>
		/// <returns>The thumbprint.</returns>
		/// <param name="s">Original string</param>
		private static string ToThumbprint ( string s){
			List<char> result = s.ToList();
			result.RemoveAll(c => c == ' ');
			return new string(result.ToArray()).ToUpper();
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Unity.Platform.IPhone.SecurityUtils"/> validate fingerprints.
		/// </summary>
		/// <value><c>true</c> if validate fingerprints; otherwise, <c>false</c>.</value>
		public static bool ValidateFingerprints{
			get{
				bool bResult;
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Should validate fingerprints for remote servers? " + SecurityUtils._VALIDATEFINGERPRINTS);
				Boolean.TryParse(SecurityUtils._VALIDATEFINGERPRINTS, out bResult);
				return bResult;
			}
		}

		/// <summary>
		/// Validates the web certificates.
		/// </summary>
		/// <returns>
		/// <c>true</c>, if web certificates was validated, <c>false</c> otherwise.
		/// </returns>
		/// <param name='sender'>
		/// <c>Object</c> usually parsed as WebRequest or HttpWebRequest.
		/// </param>
		/// <param name='endCert'>
		/// Certificate consumed in the request.
		/// </param>
		/// <param name='chain'>
		/// Certificate chain total or partial.
		/// </param>
		/// <param name='Errors'>
		/// Policy errors found during the chain build process.
		/// </param>
		public static bool ValidateWebCertificates (Object sender, System.Security.Cryptography.X509Certificates.X509Certificate endCert, System.Security.Cryptography.X509Certificates.X509Chain chain, SslPolicyErrors Errors)
		{

			var request = sender as WebRequest;
			string requestUri = request.RequestUri.ToString();


			SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation");
			bool bErrorsFound = false;
			try {
				X509Certificate BCCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate (endCert);
				if (!CertificateIsTheSame (BCCert)) {
					chain.Build (new System.Security.Cryptography.X509Certificates.X509Certificate2 (endCert.GetRawCertData ()));
					if(Errors.Equals(SslPolicyErrors.None))
					{
						if(chain== null || chain.ChainElements== null || chain.ChainElements.Count == 0){
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Chain is empty");
							bErrorsFound = true;
						}else
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Chain Element count: " + chain.ChainElements.Count);

						if(CertIsSelfSigned(BCCert)){
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. End Certificate is Self Signed");
							bErrorsFound = true;
						}else
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. End Certificate NOT Self Signed");


						if(ValidateFingerprints){ 
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. VALIDATING Fingerprint");
							if(!VerifyFingerprint(endCert, requestUri)){
								SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Invalid Fingerprint");
								bErrorsFound = true;
							}else
								SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Valid Fingerprint");
						}else{
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. DO NOT validate Fingerprint");
						}

						/*foreach (System.Security.Cryptography.X509Certificates.X509ChainElement cert in chain.ChainElements) {
							X509Certificate BCCerto = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate (cert.Certificate);
							if(CertIsSelfSigned(BCCerto)){
								SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** SELF SIGNED Certificate: CERT NAME " + BCCerto.SubjectDN.ToString() + " ;ID = " + BCCerto.SerialNumber);
								if(cert.Certificate.SerialNumber.Equals(chain.ChainElements[chain.ChainElements.Count-1].Certificate.SerialNumber)){
									string[] stringSeparators = new string[] {";"};
									string[] valids = _VALIDROOTAUTHORITIES.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
									foreach(String validRoot in valids){
										SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** SELF SIGNED Certificate check ["+validRoot+"]: "+cert.Certificate.SerialNumber+":"+chain.ChainElements[chain.ChainElements.Count-1].Certificate.SerialNumber);
										if(BCCerto.SubjectDN.ToString().Contains(validRoot)){
											bErrorsFound = false;
										} else {
											bErrorsFound = true;
										}
									}
								}else {
									bErrorsFound = true;
								}

								
							}else{
								SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** CERT NAME " + BCCerto.SubjectDN.ToString() + " ;ID = " + BCCerto.SerialNumber);
							}

							if(!CertIsValidNow(BCCerto)) bErrorsFound = true;
						}*/
							
							//if (chain.ChainElements.Count > 1 && !VerifyCertificateOCSP(chain)) bCertIsOk = true;

							//if (chain.ChainElements.Count > 1) bCertIsOk = true;
							// DO NOT check OCSP revocation URLs. The time consuming this is expensive.
							// TODO make this configurable and asynchronously in the case of enabled
						    // !VerifyCertificateOCSP(chain)  ---> ASYNC
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** OCSP Verification (certificate revocation check) is DISABLED for this build");

						if (!bErrorsFound) {
							myCertificateList.Add (BCCert.GetHashCode(), DateTime.Now);
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Valid Certificate");
							return true;
						} else{
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Invalid Certificate");
							return false;
						}
					
					}else if(Errors.Equals(SslPolicyErrors.RemoteCertificateChainErrors)){
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: Errors found in the certificate chain.");

						SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: Checking chain status information for each element in the chain");
						foreach (System.Security.Cryptography.X509Certificates.X509ChainElement element in chain.ChainElements)
						{
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: Checking chain element... " + element.Information);

							if (chain.ChainStatus!=null && chain.ChainStatus.Length >= 0)
							{
								SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: Chain Status array is not empty");
								for (int index = 0; index < element.ChainElementStatus.Length; index++)
								{
									SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: chain element status: " 
										+ element.ChainElementStatus[index].Status);
									SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: chain element status information: " 
										+ element.ChainElementStatus[index].StatusInformation);
								}
							}
						}
					}
					else if(Errors.Equals(SslPolicyErrors.RemoteCertificateNameMismatch)){
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: The certificate contains errors.");
					}
					else if(Errors.Equals(SslPolicyErrors.RemoteCertificateNotAvailable)){
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: The certificate is not available");
					}

					SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Policy Errors: " + Errors);
					return false;
				} else{ //Trusted certificate
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Trusted Certificate");
					return true;
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: Unhandled exception: " + e.Message);
				return false;
			}
		}

		/// <summary>
		/// Verifies the certificate fingerprint against the expected one
		/// </summary>
		/// <returns>
		/// <c>true</c>, if certificate fingerprint is the expected, <c>false</c> otherwise.
		/// </returns>
		/// <param name='endCert'>
		/// The last certificate of the chain.
		/// </param>
		private static bool VerifyFingerprint(System.Security.Cryptography.X509Certificates.X509Certificate endCert, String requestUri){
			//SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation for requestUri "+ requestUri);


			string[] fingerprints = null;
			foreach(KeyValuePair<string, string[]> entry in IPhoneIO.fingerprints)
			{
				if(requestUri.StartsWith(entry.Key)) {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "*** found fingerprint to check for requestUri: ["+requestUri+"]");

					fingerprints = new string[entry.Value.Length];
					int i = 0;
					foreach (string fprint in entry.Value) {
						fingerprints[i] = ToThumbprint (fprint);
					}
				}
			}

			if (fingerprints != null) {

				W.X509Certificate2 certificateThumb = new W.X509Certificate2 (endCert);
				//SystemLogger.Log (SystemLogger.Module.PLATFORM, "**************** Certificate Validation Thumbprint: [" + certificateThumb.Thumbprint + "]");
				//SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation fingerprint: [" + string.Join(",", fingerprints) + "]");
				if (!fingerprints.Contains (certificateThumb.Thumbprint)) {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation fingerprint ERROR!!!");
					return false;
				} 
			} else {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** WARNING!! Certificate Validation fingerprint NOT FOUND!!! (you should provide a valid fingerprint in your io-sevices-config.xml file in order to validate HTTPS web certificates)");
				return false;
			}

			return true;
		
		}


		/// <summary>
		/// Verifies the certificate chain via OCSP
		/// </summary>
		/// <returns>
		/// <c>true</c>, if certificate is revoked, <c>false</c> otherwise.
		/// </returns>
		/// <param name='chain'>
		/// The certificate chain.
		/// </param>
		private static bool VerifyCertificateOCSP (System.Security.Cryptography.X509Certificates.X509Chain chain)
		{
			List<X509Certificate> certsList = new List<X509Certificate> ();
			List<Uri> certsUrls = new List<Uri> ();
			bool bCertificateIsRevoked = false;
			try {
				//Get the OCSP URLS to be validated for each certificate.
				foreach (System.Security.Cryptography.X509Certificates.X509ChainElement cert in chain.ChainElements) {
					X509Certificate BCCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate (cert.Certificate);
					if (BCCert.CertificateStructure.TbsCertificate.Extensions != null) {
						X509Extension ext = BCCert.CertificateStructure.TbsCertificate.Extensions.GetExtension (X509Extensions.AuthorityInfoAccess);
						if (ext != null) {
							AccessDescription[] certUrls = AuthorityInformationAccess.GetInstance (ext).GetAccessDescriptions ();
							Uri url = (certUrls != null && certUrls.Length > 0 && certUrls [0].AccessLocation.Name.ToString ().StartsWith("http://")) ? new Uri (certUrls [0].AccessLocation.Name.ToString ()) : null;
							certsList.Add (BCCert);
							if (!certsUrls.Contains (url))
								certsUrls.Add (url);
						}
					}
				}
				if(certsUrls.Count>0){
					//create requests for each cert
					List<OcspReq> RequestList = new List<OcspReq>();
					OcspReqGenerator OCSPRequestGenerator;
					for (int i =0; i< (certsList.Count -1); i++) {
						OCSPRequestGenerator = new OcspReqGenerator ();
						BigInteger nonce = BigInteger.ValueOf (DateTime.Now.Ticks);
						List<DerObjectIdentifier> oids = new List<DerObjectIdentifier> ();
						oids.Add (Org.BouncyCastle.Asn1.Ocsp.OcspObjectIdentifiers.PkixOcspNonce);
						List<X509Extension> values = new List<X509Extension> ();
						values.Add (new X509Extension (false, new DerOctetString (nonce.ToByteArray ())));
						OCSPRequestGenerator.SetRequestExtensions (new X509Extensions (oids, values));
						CertificateID ID = new CertificateID (CertificateID.HashSha1, certsList [i + 1], certsList [i].SerialNumber);
						OCSPRequestGenerator.AddRequest (ID);
						RequestList.Add(OCSPRequestGenerator.Generate());
					}

					//send requests to the OCSP server and read the response
					for (int i =0; i< certsUrls.Count && !bCertificateIsRevoked; i++) {
						for(int j = 0; j<  RequestList.Count && !bCertificateIsRevoked ; j++){
							HttpWebRequest requestToOCSPServer = (HttpWebRequest)WebRequest.Create (certsUrls [i]);
							requestToOCSPServer.Method = "POST";
							requestToOCSPServer.ContentType = "application/ocsp-request";
							requestToOCSPServer.Accept = "application/ocsp-response";
							requestToOCSPServer.ReadWriteTimeout = 15000; // 15 seconds waiting to stablish connection
							requestToOCSPServer.Timeout = 100000; // 100 seconds timeout reading response

							byte[] bRequestBytes = RequestList[j].GetEncoded();
							using (Stream requestStream = requestToOCSPServer.GetRequestStream()) {
								requestStream.Write (bRequestBytes, 0, bRequestBytes.Length);
								requestStream.Flush ();
							}
							HttpWebResponse serverResponse = (HttpWebResponse)requestToOCSPServer.GetResponse ();
							OcspResp OCSPResponse = new OcspResp (serverResponse.GetResponseStream ());
							BasicOcspResp basicOCSPResponse = (BasicOcspResp)OCSPResponse.GetResponseObject ();
							//get the status from the response
							if (basicOCSPResponse != null) {
								foreach (SingleResp singleResponse in basicOCSPResponse.Responses) {
									object certStatus = singleResponse.GetCertStatus ();
									if (certStatus is RevokedStatus)
										bCertificateIsRevoked = true;
								}
							}
						}
					}
				}else { SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. No OCSP url service found. Cannot verify revocation.");}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Unhandled exception during revocation checking: " + e.Message);
				bCertificateIsRevoked = true;
			}
			if(bCertificateIsRevoked)
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Certificate is revoked");
			return bCertificateIsRevoked;
		}

		/// <summary>
		/// Checks the certificate is already in cache. Cache is valid for 10 minutes after first certificate successful verification.
		/// </summary>
		/// <returns>
		/// <c>true</c>, if is the certificate is in memory, <c>false</c> otherwise.
		/// </returns>
		/// <param name='BCCert'>
		/// BouncyCastle cert to check.
		/// </param>
        private static bool CertificateIsTheSame(X509Certificate BCCert)
        {
			RemoveTimedOutCertificates();
			if (myCertificateList.Count>0 && 
			    myCertificateList.ContainsKey(BCCert.GetHashCode()) &&
			    CertIsValidNow(BCCert)){
                    //trusted certificate
                    return true;
            }
			else return false;
        }

		/// <summary>
		/// Removes from cache the certificates which initial validation has been performed more than 10 minutes ago.
		/// </summary>
		private static void RemoveTimedOutCertificates ()
		{
			if (myCertificateList.Count > 0) {
				List<int> TimedOutCertsHash =
					(from KeyValuePair in myCertificateList
					 where DateTime.Now.Subtract (KeyValuePair.Value).TotalMinutes >= 10
					 select KeyValuePair.Key).ToList<int>();

				if(TimedOutCertsHash != null && TimedOutCertsHash.Count>0)
					foreach (int hash in TimedOutCertsHash) {
						myCertificateList.Remove(hash);
					}
			}
		}



		/// <summary>
		/// Checks wether the certificate is self-signed.
		/// </summary>
		/// <returns>
		/// <c>true</c>, if is self-signed, <c>false</c> otherwise.
		/// </returns>
		/// <param name='BCCert'>
		/// BouncyCastle cert to check.
		/// </param>
        private static bool CertIsSelfSigned (X509Certificate BCCert)
		{
			try {
				BCCert.Verify (BCCert.GetPublicKey ());
				return true;
			} catch (SignatureException sigex) {
				// Invalid signature --> not self-signed
				return false;
			} catch (InvalidKeyException kex) {
				// Invalid key --> not self-signed
				return false;
			}

        }

		/// <summary>
		/// Checks the certificate in this same date and time.
		/// </summary>
		/// <returns>
		/// <c>true</c>, if is valid now, <c>false</c> otherwise.
		/// </returns>
		/// <param name='BCCert'>
		/// BouncyCastle cert to check.
		/// </param>
        private static bool CertIsValidNow(X509Certificate BCCert)
        {
            try
            {
                BCCert.CheckValidity();
                return BCCert.IsValidNow;
            }catch(CertificateExpiredException ce){
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: Certificate has expired");
            }catch(CertificateNotYetValidException cex){
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: Certificate is not yet valid");
            }
            return false;
        }

		#region CRL
		//NOT TO USE
		/*
		private static bool CertChainIsValid(System.Security.Cryptography.X509Certificates.X509Chain chain)
        {
            try
            {
				bool bErrorsFound = false;
				for (int i = 0; i<chain.ChainElements.Count && !bErrorsFound; i++)
                {
					X509Certificate x509Cert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(chain.ChainElements[i].Certificate);
					if(CertIsRevoked(chain.ChainElements[i].Certificate.GetRawCertData(), x509Cert)) bErrorsFound=true;
                    // Identify self certificates
					if (CertIsSelfSigned(x509Cert)) bErrorsFound = true;
                }
				if(!bErrorsFound) return true;
			}catch(Exception ex){}
			return false;
        }
        
        private static bool CertIsRevoked(byte[] certRawData, X509Certificate BCCert)
        {
            try{
                List<X509Crl> certCRLCollection = LoadCRLFromDistributionPoint(certRawData, BCCert.CertificateStructure.TbsCertificate);
                X509CrlEntry crlEntry = null;
                foreach (X509Crl certCRL in certCRLCollection)
                {
                     crlEntry = certCRL.GetRevokedCertificate(BCCert.SerialNumber);
                    if (crlEntry != null)
                    {
                        //cert revoked
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: Certificate is revoked");
                        return true;
                    }
                }
            }catch(Exception ex){}
            return false;
        }
		
        private static List<X509Crl> LoadCRLFromDistributionPoint(byte[] certRawData, TbsCertificateStructure certStructure)
        {
            List<X509Crl> returnList = new List<X509Crl>();
            Asn1Object seq = Asn1Object.FromByteArray(certRawData);
            
            if (certStructure.Extensions != null)
            {
                X509Extension extvalue = certStructure.Extensions.GetExtension(X509Extensions.CrlDistributionPoints);
                if (extvalue != null)
                {
                    Asn1Object extObj = Asn1Object.FromByteArray(extvalue.Value.GetOctets());
                    DistributionPoint[] points = CrlDistPoint.GetInstance(extObj).GetDistributionPoints();
                    
                    for (int i = 0; i != points.Length; i++)
                    {
                        foreach (GeneralName downloadName in ((GeneralNames)points[i].DistributionPointName.Name).GetNames())
                        {
                            byte[] CRLbuffer = null;
                            if (downloadName.Name.ToString().ToLower().StartsWith("http") && downloadName.TagNo.Equals(GeneralName.UniformResourceIdentifier))
                            {
                                using (WebClient downloader = new WebClient())
                                {
                                    CRLbuffer = downloader.DownloadData(new Uri(downloadName.Name.ToString()));
                                }
                                returnList.Add(new X509CrlParser().ReadCrl(CRLbuffer));
                            }
                        }
                    }
                }
            }
            return returnList;
        }
        */
		#endregion
	}
}

