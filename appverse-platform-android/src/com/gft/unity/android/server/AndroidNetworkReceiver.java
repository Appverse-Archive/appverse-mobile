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
package com.gft.unity.android.server;


import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;
import android.webkit.WebView;

/**
 * @author maps
 *
 */
public class AndroidNetworkReceiver extends BroadcastReceiver {

	private static final String TAG = "Appverse.AndroidNetworkReceiver";
	
	private WebView view;
	
	/* (non-Javadoc)
	 * @see android.content.BroadcastReceiver#onReceive(android.content.Context, android.content.Intent)
	 */
	public AndroidNetworkReceiver(WebView view) {
		super();
		this.view = view;
	}

	@Override
	public void onReceive(Context ctx, Intent intent) {
		//Log.d(TAG, "******* NETWORK RECEIVER CHANGE " + intent.getAction());
		
		ProxySettingsAction action = new ProxySettingsAction(ctx, this.view);
		if(ctx instanceof Activity) {
			((Activity)ctx).runOnUiThread(action);
		}
	}
	
	private class ProxySettingsAction implements Runnable {

		private Context context;
		private WebView view;

		public ProxySettingsAction(Context ctx, WebView view) {
			this.context = ctx;
			this.view = view;
		}

		@Override
		public void run() {
			try {
				ProxySettings.setProxy(this.context, this.view, "", 0);
			} catch (Exception ex) {
				Log.d(TAG, "******* NETWORK RECEIVER error setting proxy " + ex.getMessage());
				Log.d(TAG, "stacktrace:" + ex);
			}
		}
	}

}
