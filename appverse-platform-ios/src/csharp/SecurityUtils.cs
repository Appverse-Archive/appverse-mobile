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
using MonoTouch.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.Ocsp;

namespace Unity.Platform.IPhone
{
	public static class SecurityUtils
	{
		private static Dictionary<int, DateTime> myCertificateList = new Dictionary<int, DateTime>();

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
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation");
			try {
				X509Certificate BCCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate (endCert);
				if (!CertificateIsTheSame (BCCert)) {
					chain.Build (new System.Security.Cryptography.X509Certificates.X509Certificate2 (endCert.GetRawCertData ()));
					if(Errors.Equals(SslPolicyErrors.None))
					{
						if(chain== null || chain.ChainElements== null || chain.ChainElements.Count==0) 
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Chain is empty");
						bool bCertIsOk = false;
						if (CertIsValidNow (BCCert))
							if (chain.ChainElements.Count > 1 && !VerifyCertificateOCSP(chain)) bCertIsOk = true;
						if (bCertIsOk) {
							myCertificateList.Add (BCCert.GetHashCode(), DateTime.Now);
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Valid Certificate");
							return true;
						} else{
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation. Invalid Certificate");
							return false;
						}
					
					}else if(Errors.Equals(SslPolicyErrors.RemoteCertificateChainErrors)){
						SystemLogger.Log (SystemLogger.Module.PLATFORM, "*************** Certificate Validation: Errors found in the certificate chain.");
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

