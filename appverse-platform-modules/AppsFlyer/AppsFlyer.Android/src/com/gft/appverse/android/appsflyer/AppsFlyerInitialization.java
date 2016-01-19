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
package com.gft.appverse.android.appsflyer;

public class AppsFlyerInitialization {
	
	/**
	 * Unique developer ID accessible from AppsFlyer account
	 */
	private String DevKey;
	
	/**
	 * Application ID
	 */
	private String AppID;
	
	/**
	 * Acceptable ISO currency code. Default is USD.
	 */
	private String CurrencyCode = "USD";
	
	/**
	 * Customer ID that will be added to the reports
	 */
	private String CustomerUserID;
	
	/**
	 * HTTP or HTTPs (default is HTTP)
	 */
	private CommunicationsProtocol CommunicationsProtocol;
	
	/**
	 * True to allow the HTTP fallback in case HTTPs is configured but not enabled. Default is false.
	 */
	private boolean UseHttpFallback;
	
	
	// constructor is needed for JSON parsing
	public AppsFlyerInitialization() {
		super();
	}

	public String getDevKey() {
		return DevKey;
	}

	public void setDevKey(String devKey) {
		DevKey = devKey;
	}

	public String getCurrencyCode() {
		return CurrencyCode;
	}

	public void setCurrencyCode(String currencyCode) {
		CurrencyCode = currencyCode;
	}

	public String getCustomerUserID() {
		return CustomerUserID;
	}

	public void setCustomerUserID(String customerUserID) {
		CustomerUserID = customerUserID;
	}

	public CommunicationsProtocol getCommunicationsProtocol() {
		return CommunicationsProtocol;
	}

	public void setCommunicationsProtocol(
			CommunicationsProtocol communicationsProtocol) {
		CommunicationsProtocol = communicationsProtocol;
	}

	public boolean getUseHttpFallback() {
		return UseHttpFallback;
	}

	public void setUseHttpFallback(boolean useHttpFallback) {
		UseHttpFallback = useHttpFallback;
	}

	public String getAppID() {
		return AppID;
	}

	public void setAppID(String appID) {
		AppID = appID;
	}
	
	
	
}
