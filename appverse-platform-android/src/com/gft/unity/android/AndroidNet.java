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
package com.gft.unity.android;

import java.net.Inet4Address;
import java.net.Inet6Address;
import java.net.InetAddress;
import java.net.MalformedURLException;
import java.net.NetworkInterface;
import java.net.URL;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.List;

import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.Uri;

import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.core.net.AbstractNet;
import com.gft.unity.core.net.NetworkData;
import com.gft.unity.core.net.NetworkType;
import com.gft.unity.core.net.SecondaryBrowserOptions;
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

// TODO review implementation for HTTPs, proxies, ...
public class AndroidNet extends AbstractNet {

	private static final SystemLogger LOG = AndroidSystemLogger.getInstance();

	private static final String HTTP_SCHEME = "http://";
	private static final String HTTPS_SCHEME = "https://";

	private static final int REACHABLE_TIMEOUT = 60;

	public static final String EXTRA_URL = "extra_url";
	public static final String EXTRA_HTML = "extra_html";
	public static final String EXTRA_BROWSER_TITLE = "extra_title";
	public static final String EXTRA_BUTTON_TEXT = "extra_buttontext";
	public static final String EXTRA_FILE_EXTENSIONS = "extra_fileextensions";

	private static final String ACTION_SHOW_BROWSER = ".SHOW_BROWSER";

	private String lastUrl;
	private boolean lastResult;
	private long lastChecked;

	@Override
	// TODO review INet.GetNetworkTypeReachableList implementation
	public NetworkType[] GetNetworkTypeReachableList(String url) {
		return new NetworkType[] { GetNetworkTypeReachable(url) };
	}

	@Override
	public NetworkType[] GetNetworkTypeSupported() {
		NetworkType[] types = null;

		try {
			ConnectivityManager cm = (ConnectivityManager) AndroidServiceLocator
					.getContext()
					.getSystemService(Service.CONNECTIVITY_SERVICE);

			List<NetworkType> typesList = new ArrayList<NetworkType>();
			NetworkInfo[] netInfo = cm.getAllNetworkInfo();
			for (NetworkInfo ni : netInfo) {
				NetworkType ntype = getNetworkTypeByNetworkInfo(ni);
				if (ntype != NetworkType.Unknown
						|| (ntype == NetworkType.Unknown && !typesList
								.contains(NetworkType.Unknown))) {
					typesList.add(ntype);
				}
			}

			types = new NetworkType[typesList.size()];
			for (int i = 0; i < typesList.size(); i++) {
				types[i] = typesList.get(i);
			}
		} catch (Exception ex) {
			LOG.Log(Module.PLATFORM, "GetNetworkTypeSupported error", ex);
		}

		return types;
	}

	@Override
	public NetworkType GetNetworkTypeReachable(String url) {
		NetworkType type = NetworkType.Unknown;

		if (IsNetworkReachable(url)) {
			Context context = AndroidServiceLocator.getContext();
			ConnectivityManager cm = (ConnectivityManager) context
					.getSystemService(Context.CONNECTIVITY_SERVICE);
			NetworkInfo network = cm.getActiveNetworkInfo();
			type = getNetworkTypeByNetworkInfo(network);
		}

		return type;
	}

	@Override
	// TODO review INet.IsNetworkReachable implementation
	public boolean IsNetworkReachable(String url) {

		if (isNetworkConnected()) {
			if (lastUrl != null
					&& lastUrl.equals(url)
					&& REACHABLE_TIMEOUT > (System.currentTimeMillis() - lastChecked)) {
				LOG.Log(Module.PLATFORM,
						"IsNetworkReachable: Using cached result");
				return lastResult;
			} else {
				lastResult = checkHttpConnection(url);
				lastChecked = System.currentTimeMillis();
				LOG.Log(Module.PLATFORM,
						"IsNetworkReachable: result checking url [" + url + "]: " + lastResult);
			}
		} else {
			lastResult = false;
			LOG.Log(Module.PLATFORM, "IsNetworkReachable: The Network Connectivity is not available.");
			lastChecked = 0;
		}
		lastUrl = url;

		return lastResult;
	}

	private boolean isNetworkConnected() {
		boolean result = false;

		Context context = AndroidServiceLocator.getContext();
		ConnectivityManager cm = (ConnectivityManager) context
				.getSystemService(Context.CONNECTIVITY_SERVICE);
		NetworkInfo network = cm.getActiveNetworkInfo();
		if (network != null) {
			result = network.isAvailable();
		}

		return result;
	}

	private NetworkType getNetworkTypeByNetworkInfo(NetworkInfo ni) {
		NetworkType type = NetworkType.Unknown;

		if (ni!= null && ni.getType() == ConnectivityManager.TYPE_MOBILE) {
			type = NetworkType.Carrier_3G;
		} else if (ni!= null && ni.getType() == ConnectivityManager.TYPE_WIFI) {
			type = NetworkType.Wifi;
		}

		return type;
	}

	private boolean checkHttpConnection(String url) {
		boolean result = false;

		String schemeUrl = url;
		boolean hasScheme = url.contains("://");
		if (!hasScheme) {
			schemeUrl = HTTP_SCHEME + url;
		}
		try {
			URL urlObj = new URL(schemeUrl);
			result = tryConnection(urlObj);
			if (!result && !hasScheme) {
				schemeUrl = HTTPS_SCHEME + url;
				urlObj = new URL(schemeUrl);
				result = tryConnection(urlObj);
			}
		} catch (MalformedURLException ex) {
			LOG.Log(Module.PLATFORM, "checkHttpConnection MalformedURLException [" + schemeUrl +"]: " + ex.getMessage());
			// it's not a valid URL... it means it's not reachable
			result = false;
		} catch (Exception ex) {
			LOG.Log(Module.PLATFORM, "checkHttpConnection unhandled exception [" + schemeUrl +"]", ex);
			result = false;
		}

		return result;
	}

	private boolean tryConnection(URL url) {
		boolean result = false;

		try {
			LOG.Log(SystemLogger.Module.PLATFORM, "tryConnection [" + url +"]...");
			url.openConnection().connect();
			result = true;
		} catch (Exception ex) {
			LOG.Log(SystemLogger.Module.PLATFORM, "tryConnection warning [" + url +"]: " + ex.getMessage());
		}

		return result;
	}

	@Override
	public boolean OpenBrowser(String title, String buttonText, String url) {
		boolean result = false;

		try {

			IActivityManager aam = (IActivityManager) AndroidServiceLocator
					.GetInstance()
					.GetService(
							AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);

			Intent intent = new Intent(AndroidServiceLocator.getContext()
					.getPackageName() + ACTION_SHOW_BROWSER);
			intent.putExtra(EXTRA_URL, url);
			intent.putExtra(EXTRA_BROWSER_TITLE, title);
			intent.putExtra(EXTRA_BUTTON_TEXT, buttonText);

			result = aam.startActivity(intent);
		} catch (Exception ex) {
			LOG.Log(SystemLogger.Module.PLATFORM, "tryConnection error", ex);
		}
		return result;
	}	
	
	@Override
	public boolean OpenBrowserWithOptions(SecondaryBrowserOptions browserOptions) {
		boolean result = false;

		try {
			IActivityManager aam = (IActivityManager) AndroidServiceLocator
					.GetInstance()
					.GetService(
							AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
			Intent intent = new Intent(AndroidServiceLocator.getContext()
					.getPackageName() + ACTION_SHOW_BROWSER);
			intent.putExtra(EXTRA_URL, browserOptions.getUrl());
			intent.putExtra(EXTRA_BROWSER_TITLE, browserOptions.getTitle());
			intent.putExtra(EXTRA_BUTTON_TEXT, browserOptions.getCloseButtonText());
			intent.putExtra(EXTRA_HTML, browserOptions.getHtml());
			intent.putExtra(EXTRA_FILE_EXTENSIONS, browserOptions.getBrowserFileExtensionsAsString());

			result = aam.startActivity(intent);
		} catch (Exception ex) {
			LOG.Log(SystemLogger.Module.PLATFORM, "tryConnection error", ex);
		}

		return result;
	}

	@Override
	public boolean ShowHtml(String title, String buttonText, String html) {
		boolean result = false;

		try {
			
			IActivityManager aam = (IActivityManager) AndroidServiceLocator
					.GetInstance()
					.GetService(
							AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);

			Intent intent = new Intent(AndroidServiceLocator
					.getContext().getPackageName()
					+ ACTION_SHOW_BROWSER);
			intent.putExtra(EXTRA_HTML, html);
			intent.putExtra(EXTRA_BROWSER_TITLE, title);
			intent.putExtra(EXTRA_BUTTON_TEXT, buttonText);

			result = aam.startActivity(intent);

		} catch (Exception ex) {
			LOG.Log(SystemLogger.Module.PLATFORM, "tryConnection error", ex);
		}

		return result;
	}
	
	@Override
	public boolean ShowHtmlWithOptions(SecondaryBrowserOptions browserOptions) {
		boolean result = false;

		try {
			IActivityManager aam = (IActivityManager) AndroidServiceLocator
					.GetInstance()
					.GetService(
							AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
			Intent intent = new Intent(AndroidServiceLocator.getContext()
					.getPackageName() + ACTION_SHOW_BROWSER);
			intent.putExtra(EXTRA_BROWSER_TITLE, browserOptions.getTitle());
			intent.putExtra(EXTRA_BUTTON_TEXT, browserOptions.getCloseButtonText());
			intent.putExtra(EXTRA_HTML, browserOptions.getHtml());
			intent.putExtra(EXTRA_FILE_EXTENSIONS, browserOptions.getBrowserFileExtensionsAsString());

			result = aam.startActivity(intent);
		} catch (Exception ex) {
			LOG.Log(SystemLogger.Module.PLATFORM, "tryConnection error", ex);
		}
		return result;
	}

	@Override
	// TODO review INet.DownloadFile implementation
	public boolean DownloadFile(String url) {
		boolean result = false;

		try {
			Intent intent = new Intent(Intent.ACTION_VIEW);
			intent.setData(Uri.parse(url));
			AndroidServiceLocator.getContext().startActivity(intent);

			result = true;
		} catch (Exception ex) {
			LOG.Log(SystemLogger.Module.PLATFORM, "tryConnection error", ex);
		}

		return result;
	}
	
	@Override
	// TODO review INet.GetNetworkData implementation
	public NetworkData GetNetworkData() {
		//Different device manufacturers name the network interfaces differently, so cannot ask directly by name
		//we have to loop thru them, only 1 of them will have a valid IP address
		//if WIFI is active 3G IP address does not appear in the interfaces list
		try{
			NetworkData returnData = new NetworkData();
			for(Enumeration<NetworkInterface> en = NetworkInterface.getNetworkInterfaces(); en.hasMoreElements();){
				NetworkInterface intf = en.nextElement();
				for(Enumeration<InetAddress> ip = intf.getInetAddresses(); ip.hasMoreElements();){
					InetAddress ia = ip.nextElement();
					if(!ia.isLoopbackAddress()){
						//Check IPv4 or IPv6
						if(ia instanceof Inet4Address){  returnData.setIPv4(ia.getHostAddress().toString());}
						else if(ia instanceof Inet6Address){
							//remove the sufix that Android adds to v6 addresses, Suffix added is %NetworkInterface.Name (EG: 2001:db8::2:1%wlan0)
							String v6Unparsed = ia.getHostAddress().toString();
							if(v6Unparsed.contains("%")){
								v6Unparsed = v6Unparsed.split("%")[0];
							}
							returnData.setIPv6(v6Unparsed);
						}
					}
				}
			}
			return returnData;
		}catch(Exception ex){LOG.Log(SystemLogger.Module.PLATFORM, "Error retrieveing Device IP Address", ex);}
		return null;
	}
}
