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


import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.core.net.NetworkType;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.webkit.WebView;

/**
 * @author maps
 *
 */
public class AndroidNetworkReceiver extends BroadcastReceiver {

	private static final String LOGGER_MODULE = "Appverse.AndroidNetworkReceiver";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);
	
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
		
		LOGGER.logDebug("onReceive", "*********** Network connectivity change");
	     /*if(intent.getExtras()!=null) {
	        NetworkInfo ni=(NetworkInfo) intent.getExtras().get(ConnectivityManager.EXTRA_NETWORK_INFO);
	        if(ni!=null && ni.getState()==NetworkInfo.State.CONNECTED) {
	            LOGGER.logDebug("onReceive", "*********** Network "+ni.getTypeName()+" connected: "+ni.getType());
	            LOGGER.logDebug("onReceive", "*********** Network "+ni.getTypeName()+" connected: "+ni.getDetailedState());
	            LOGGER.logDebug("onReceive", "*********** Network "+ni.getTypeName()+" connected: "+ni.getExtraInfo());
	            LOGGER.logDebug("onReceive", "*********** Network "+ni.getTypeName()+" connected: "+ni.getTypeName());
	            LOGGER.logDebug("onReceive", "*********** Network "+ni.getTypeName()+" connected: "+ni.getReason());
	        }
	     }
	     if(intent.getExtras().getBoolean(ConnectivityManager.EXTRA_NO_CONNECTIVITY,Boolean.FALSE)) {
	            LOGGER.logDebug("onReceive", "*********** Network There's no network connectivity");
	     }*/
	     
	    ConnectivityManager connectivityManager = (ConnectivityManager)ctx.getSystemService(Context.CONNECTIVITY_SERVICE);
        int networkType = intent.getExtras().getInt(ConnectivityManager.EXTRA_NETWORK_TYPE);
        boolean isWiFi = networkType == ConnectivityManager.TYPE_WIFI;
        boolean isMobile = networkType == ConnectivityManager.TYPE_MOBILE;
        
        NetworkInfo networkInfo = connectivityManager.getNetworkInfo(networkType);
        boolean isConnected = networkInfo.isConnected();
        com.gft.unity.core.net.NetworkType type = NetworkType.Unknown;
        if (isWiFi) {
            if (isConnected) {
            	LOGGER.logDebug("onReceive", "Wi-Fi - CONNECTED ("+networkInfo.getType()+")");
                type = NetworkType.Wifi;
                WifiManager wifiManager = (WifiManager) ctx
                        .getSystemService(ctx.WIFI_SERVICE);
                WifiInfo wifiInfo = wifiManager.getConnectionInfo();

                LOGGER.logDebug("onReceive", "SSID: "+wifiInfo.getSSID());
                    
            } else {
            	LOGGER.logDebug("onReceive", "Wi-Fi - DISCONNECTED ("+networkInfo.getType()+")");
                
            }
        } else if (isMobile) {
            if (isConnected) {
            	LOGGER.logDebug("onReceive", "Mobile - CONNECTED ("+networkInfo.getType()+")");
                type = NetworkType.Carrier_3G;
            } else {
            	LOGGER.logDebug("onReceive", "Mobile - DISCONNECTED ("+networkInfo.getType()+")");
            }
        } else {
            if (isConnected) {
            	LOGGER.logDebug("onReceive", networkInfo.getTypeName() + " - CONNECTED ("+networkInfo.getType()+")");
            } else {
            	LOGGER.logDebug("onReceive", networkInfo.getTypeName() + " - DISCONNECTED ("+networkInfo.getType()+")");
            }
        }
       
        
        IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
        LOGGER.logDebug("onReceive", "*********************** NETWORKSTATUS: "+type.ordinal());
        am.executeJS("try{if(Appverse&&Appverse.Net){Appverse.Net.NetworkStatus = " + type.ordinal() + "; localStorage.setItem('_NetworkStatus', Appverse.Net.NetworkStatus); Appverse.Net.onConnectivityChange(Appverse.Net.NetworkStatus);}}catch(e){console.log('Error setting network status from AndroidNetworkReceiver(please check onConnectivityChange method): '+e);}");
        //am.executeJS("Appverse.Net.onConnectivityChange",type.ordinal());
		
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
				LOGGER.logDebug("ProxySettingsAction#run", "******* NETWORK RECEIVER error setting proxy " + ex.getMessage());
				LOGGER.logDebug("ProxySettingsAction#run", "stacktrace:" + ex);
			}
		}
	}

}
