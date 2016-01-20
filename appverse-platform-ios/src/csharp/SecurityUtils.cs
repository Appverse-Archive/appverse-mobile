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
using System.Net.Security;
using Org.BouncyCastle.X509;

using W = System.Security.Cryptography.X509Certificates;

namespace Unity.Platform.IPhone
{
	public static class SecurityUtils
	{

		private static string _VALIDATEFINGERPRINTS = "$ValidateFingerprints$";

		private static string _VALIDATEPUBLICKEY = "$ValidatePublicKey$";

		private static Dictionary<int, DateTime> myCertificateList = new Dictionary<int, DateTime>();

		/// <summary>
		/// Remove spaces to match the thumbprint.
		/// </summary>
		/// <returns>The thumbprint.</returns>
		/// <param name="s">Original string</param>
		private static string ToThumbprint ( string s){
			return "";
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Unity.Platform.IPhone.SecurityUtils"/> validate fingerprints.
		/// </summary>
		/// <value><c>true</c> if validate fingerprints; otherwise, <c>false</c>.</value>
		public static bool ValidateFingerprints{
			get{
				
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Unity.Platform.IPhone.SecurityUtils"/> validate PK fingerprints.
		/// </summary>
		/// <value><c>true</c> if validate PK fingerprints; otherwise, <c>false</c>.</value>
		public static bool ValidatePublicKey{
			get{
				
				return false;
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

			return true;
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
			

			return true;
		
		}

		public static string ByteArrayToString(byte[] array)
		{
			
			return "";
		}


		/// <summary>
		/// Verifies the PK fingerprint.
		/// </summary>
		/// <returns><c>true</c>, if PK fingerprint was verifyed, <c>false</c> otherwise.</returns>
		/// <param name="endCert">End cert.</param>
		/// <param name="requestUri">Request URI.</param>
		private static bool VerifyPublicKey(System.Security.Cryptography.X509Certificates.X509Certificate endCert, String requestUri){
			

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
			
			return false;
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
			return false;
        }

		/// <summary>
		/// Removes from cache the certificates which initial validation has been performed more than 10 minutes ago.
		/// </summary>
		private static void RemoveTimedOutCertificates ()
		{
			
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
			return false;

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
            
            return true;
        }

		#region CRL

	
		#endregion
	}
}
