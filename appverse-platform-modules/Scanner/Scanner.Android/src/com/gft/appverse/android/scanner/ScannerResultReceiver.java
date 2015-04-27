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
package com.gft.appverse.android.scanner;

import android.app.Activity;
import android.os.Bundle;
import android.os.Handler;
import android.os.ResultReceiver;

import com.gft.unity.core.IAppDelegate;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class ScannerResultReceiver extends ResultReceiver {
	
	private static final String LOGGER_MODULE = "Scanner Module - ScannerResultReceiver";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);	

	public static final int QRCODE_DETECT_RC_AUTOHANDLE = 5002;
	public static final int QRCODE_DETECT_RC_NOT_AUTOHANDLE = 5003;
	
	AndroidScanner scanner;

	public ScannerResultReceiver(Handler handler, AndroidScanner _scanner) {
		super(handler);
		LOGGER.logInfo("Init", "Initializing Scanner Module ResultReceiver");
		scanner = _scanner;
	}

	@Override
	protected void onReceiveResult(int resultCode, Bundle resultData) {
		LOGGER.logInfo("onReceiveResult", "Received result with code: " + resultCode);
		
		int activityResultCode = resultData.getInt(IAppDelegate.ACTIVITY_RESULT_CODE_BUNDLE_KEY);
		
		if (activityResultCode == Activity.RESULT_CANCELED){
			LOGGER.logInfo("onReceiveResult", "Cancel button pressed. No action.");
		}
		else if (resultCode == QRCODE_DETECT_RC_AUTOHANDLE && activityResultCode == Activity.RESULT_OK){
			LOGGER.logInfo("onReceiveResult", "handling result with AUTO");
			this.scanner.onOk(resultData, true);
		} else if (resultCode == QRCODE_DETECT_RC_NOT_AUTOHANDLE && activityResultCode == Activity.RESULT_OK){
			LOGGER.logInfo("onReceiveResult", "handling result with NO AUTO");
			this.scanner.onOk(resultData, false);
		}
		else {
			super.onReceiveResult(resultCode, resultData);
		}
	}

	
	
	
}
