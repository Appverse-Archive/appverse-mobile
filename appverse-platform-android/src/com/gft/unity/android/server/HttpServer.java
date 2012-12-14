/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://www.appverse.mobi/licenses/apl_v2.0.pdf.

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

import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

import android.app.Activity;
import android.content.Context;
import android.net.LocalServerSocket;
import android.net.LocalSocket;
import android.os.Build;
import android.webkit.WebView;

import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

public class HttpServer extends com.gft.unity.core.system.server.HttpServer {
	
	private static final SystemLogger LOG = SystemLogger.getInstance();

	private Properties serverProperties;
	private Context context;
	private WebView view;

	public HttpServer(int port, Context ctx, WebView view) {
		super(port);
		this.context = ctx;
		this.view = view;
	}

	@Override
	protected void platformSpecificSettings() {
		/*
		// Resetting Proxy Settings
		try {
			LOG.Log(Module.PLATFORM, "HttpServer setting webview proxy...");
			
			if (Build.VERSION.SDK_INT < 14) {
				ProxySettingsAction action = new ProxySettingsAction(this.context, this.view);
				if(this.context instanceof Activity) {
					((Activity)this.context).runOnUiThread(action);
				}
			} else {
				ProxySettings.setProxy(this.context, this.view, "", 0);
			}
			
			Thread.yield();
			
			LOG.Log(Module.PLATFORM, "HttpServer end setting webview proxy");
			
		} catch(Exception ex) {
			LOG.Log(Module.PLATFORM, "HttpServer Exception setting proxy : " + ex.getMessage());
			ex.printStackTrace();
		}
		*/
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
				LOG.Log(Module.PLATFORM, "HttpServer error setting proxy", ex);
			}
		}
	}
	
	@Override
	public Properties getServerProperties() {
		if (this.serverProperties == null) {
			this.serverProperties = com.gft.unity.core.system.server.HttpServer.SERVER_CONFIG;
			this.serverProperties.setProperty("chain.chain",
					"asset service");
			this.serverProperties.setProperty("asset.class",
					"com.gft.unity.android.server.handler.AssetHandler");
			this.serverProperties.setProperty("service.class",
					"com.gft.unity.android.server.handler.ServiceHandler");
		}
		return this.serverProperties;
	}
}
