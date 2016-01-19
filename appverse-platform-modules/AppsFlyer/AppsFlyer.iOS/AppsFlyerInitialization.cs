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
using System.Xml.Serialization;

namespace Appverse.Platform.IPhone
{
	[XmlRootAttribute("appsflyer-config", Namespace = "", IsNullable = false)]
	public class AppsFlyerInitialization
	{
		/// <summary>
		/// Gets or sets the dev key.
		/// </summary>
		/// <value>Unique developer ID accessible from AppsFlyer account</value>
		[XmlAttributeAttribute(AttributeName = "dev-key", DataType = "string")]
		public string DevKey { get; set; }

		/// <summary>
		/// Gets or sets the app ID.
		/// </summary>
		/// <value>Application ID</value>
		[XmlAttributeAttribute(AttributeName = "app-id", DataType = "string")]
		public string AppID { get; set; }

		/// <summary>
		/// Gets or sets the currency code.
		/// </summary>
		/// <value>Acceptable ISO currency code. Default is USD.</value>
		[XmlAttributeAttribute(AttributeName = "currency-code", DataType = "string")]
		[System.ComponentModel.DefaultValue("USD")]
		public string CurrencyCode { get; set; } //= "USD"; this does not compile from script

		/// <summary>
		/// Gets or sets the customer user ID.
		/// </summary>
		/// <value>Customer ID that will be added to the reports.</value>
		[XmlAttributeAttribute(AttributeName = "customer-id", DataType = "string")]
		public string CustomerUserID  { get; set; }

		/// <summary>
		/// Gets or sets the communications protocol.
		/// </summary>
		/// <value>HTTP or HTTPs (default is HTTP)</value>
		[XmlAttributeAttribute(AttributeName = "communications-protocol")]
		public CommunicationsProtocol CommunicationsProtocol  { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Appverse.Platform.IPhone.AppsFlyerTrackEvent"/> use http fallback.
		/// </summary>
		/// <value>True to allow the HTTP fallback in case HTTPs is configured but not enabled. Default is false.</value>
		[XmlAttributeAttribute(AttributeName = "use-http-fallback", DataType = "boolean")]
		public bool UseHttpFallback  { get; set; }


		// constructor is needed for JSON parsing
		public AppsFlyerInitialization() { }
	}


}

