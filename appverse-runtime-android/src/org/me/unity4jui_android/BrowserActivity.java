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
package org.me.unity4jui_android;

import java.io.ByteArrayInputStream;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.Arrays;
import java.util.List;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings.ZoomDensity;
import android.webkit.WebResourceResponse;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;

import com.gft.unity.android.AndroidNet;
import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

public class BrowserActivity extends Activity {
	
	private static final AndroidSystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();


	private static final String LAYOUT_ID = "browserlayout";
	private static final String BTN_CLOSE_ID = "btnCloseBrowser";
	private static final String WEBVIEW_ID = "browserwebview";

	private static final String ID_TYPE = "id";
	private static final String LAYOUT_TYPE = "layout";

	private static final String DEFAULT_BROWSER_TITLE = "";
	private static final String DEFAULT_BTN_CLOSE_TEXT = "Close"; // TODO i18n

	private WebView mWebView;
	private Button btnClose;
	private List<String> browserfileExtensions = null;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		getWindow().requestFeature(Window.FEATURE_PROGRESS);

		int layoutId = getResources().getIdentifier(LAYOUT_ID, LAYOUT_TYPE,
				getPackageName());
		setContentView(layoutId);
		int buttonId = getResources().getIdentifier(BTN_CLOSE_ID, ID_TYPE,
				getPackageName());
		int webviewId = getResources().getIdentifier(WEBVIEW_ID, ID_TYPE,
				getPackageName());

		mWebView = (WebView) findViewById(webviewId);
		mWebView.getSettings().setDefaultZoom(ZoomDensity.FAR);
		mWebView.getSettings().setJavaScriptEnabled(true);
		mWebView.getSettings().setBuiltInZoomControls(true);
		mWebView.getSettings().setSupportZoom(true);
		mWebView.getSettings().setDomStorageEnabled(true);
		getIntentExtras_FileExtensions();
		if(browserfileExtensions!= null && !browserfileExtensions.isEmpty()){
			mWebView.setWebViewClient(new UnityWebViewClient(browserfileExtensions));
		}else{
			mWebView.setWebViewClient(new UnityWebViewClient());
		}

		mWebView.setWebChromeClient(new WebChromeClient() {
			@Override
			public void onProgressChanged(WebView view, int progress) {
				setProgress(progress * 100);
			}
		});

		btnClose = (Button) findViewById(buttonId);
		btnClose.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View v) {
				finish();
			}
		});

	}
	
	private void getIntentExtras_FileExtensions(){
		Intent intent = getIntent();
		if(intent!=null) {
			String fileExtensions = intent.hasExtra(AndroidNet.EXTRA_FILE_EXTENSIONS) ? intent
					.getExtras().getString(AndroidNet.EXTRA_FILE_EXTENSIONS)
					: "";
			if(fileExtensions != null && !fileExtensions.trim().isEmpty()){
				browserfileExtensions = Arrays.asList(fileExtensions.split("/\\;/g"));
			}
		}
	}

	@Override
	protected void onResume() {
		super.onResume();

		mWebView.getSettings().setJavaScriptEnabled(true);
		Intent intent = getIntent();
		if(intent!=null) {
			getIntentExtras_FileExtensions();
			if (intent.hasExtra(AndroidNet.EXTRA_URL)) {
				String url = intent.getExtras().getString(AndroidNet.EXTRA_URL);
				mWebView.loadUrl(url);
			} else if (intent.hasExtra(AndroidNet.EXTRA_HTML)) {
				String html = intent.getExtras().getString(AndroidNet.EXTRA_HTML);
				mWebView.loadData(html, "text/html", "UTF-8");
			}

			String title = intent.hasExtra(AndroidNet.EXTRA_BROWSER_TITLE) ? intent
					.getExtras().getString(AndroidNet.EXTRA_BROWSER_TITLE)
					: DEFAULT_BROWSER_TITLE;
			setTitle(title);
			String buttonText = intent.hasExtra(AndroidNet.EXTRA_BUTTON_TEXT) ? intent
					.getExtras().getString(AndroidNet.EXTRA_BUTTON_TEXT)
					: DEFAULT_BTN_CLOSE_TEXT;
			btnClose.setText(buttonText);
		}
	}

	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {

		if ((keyCode == KeyEvent.KEYCODE_BACK) && mWebView.canGoBack()) {
			if(mWebView.canGoBack())
				mWebView.goBack();
			else
				this.finish();
			return true;
		}

		return super.onKeyDown(keyCode, event);
	}

	private class UnityWebViewClient extends WebViewClient {
		private List<String> fileExtensions = null;
		
		public UnityWebViewClient(){
			super();
		}
		
		public UnityWebViewClient(List<String> browserFileExtensions) {
			super();
			this.fileExtensions = browserFileExtensions;
		}

		@Override
		public boolean shouldOverrideUrlLoading(WebView view, String url) {
			try {
				//LOG.Log(Module.GUI, "shouldOverrideUrlLoading: " + url);
				
				String action = Intent.ACTION_VIEW;  // default action
				
				// tel and mailto schemes should be treated by system too
				if (url!=null && (url.startsWith("mailto:") || url.startsWith("tel:"))) { 
					LOG.Log(Module.GUI, "shouldOverrideUrlLoading: tel or mailto schemes need to be handled specifically when called inside webview");
					
					Intent intent = null;
					
					if (url.startsWith("mailto:"))  {
						/*
						* action=android.intent.action.SEND, 
						* mimeType=text/plain, 
						* category=null, uriScheme=null
						* android.intent.extra.TEXT, value: Email message text
						* android.intent.extra.SUBJECT, value: Email subject
	 					* context_path, value: additionalPathIfNeeded/
						* android.intent.extra.EMAIL, value: [unityversal@gmail.com,jon@example.com]
						*/
						
						action = Intent.ACTION_SEND;
						url = url.replaceFirst("mailto:", ""); 
			            url = url.trim(); 
						intent = new Intent(action);
						intent.setType("text/plain");
						String[] extraMailData = new String[] {url.split("\\?")[0] };
						//LOG.Log(Module.GUI, "shouldOverrideUrlLoading: mailto: extra email data: " + extraMailData);
						intent.putExtra(Intent.EXTRA_EMAIL, extraMailData);
						
					} else if (url.startsWith("tel:")) {
						intent = new Intent(action, Uri.parse(url));
						//LOG.Log(Module.GUI, "shouldOverrideUrlLoading: tel uri data: " + Uri.parse(url).toString());
						intent.addCategory(Intent.CATEGORY_BROWSABLE);
					}
					
					view.getContext().startActivity(intent);
					return true;
				}
				
				
				boolean delegatesToSystem = false;
				//LOG.Log(Module.GUI, "shouldOverrideUrlLoading: checking path");
				String path = new URL(url).getPath();
				if(path!=null && path.lastIndexOf('.')!=-1){
					String urlExtension = path.substring(path.lastIndexOf('.'));
					if(!urlExtension.isEmpty()&& fileExtensions!=null && fileExtensions.size()>0 && fileExtensions.contains(urlExtension))
					{
						delegatesToSystem = true;
					}
				}
				//LOG.Log(Module.GUI, "shouldOverrideUrlLoading: checking query");
				// extensions could also be received as Query parameters
				String query = new URL(url).getQuery();
				if(query!=null && query.lastIndexOf('.')!=-1){
					String urlExtensionQuery = query.substring(query.lastIndexOf('.'));
					if(!urlExtensionQuery.isEmpty() && fileExtensions!=null && fileExtensions.size()>0 && fileExtensions.contains(urlExtensionQuery))
					{
						delegatesToSystem = true;
					}
				}
				
				if(delegatesToSystem) {
					// Delegate to the system the open/download of that file (operating system handles the url)
					LOG.Log(Module.GUI, "shouldOverrideUrlLoading: delegating handle of this url to the system");
					
					view.getContext().startActivity(new Intent(action, Uri.parse(url)));
					return true;
				}
				
			} catch (MalformedURLException e) {
				LOG.Log(Module.GUI, "shouldOverrideUrlLoading: MalformedURLException: "+ e.getMessage());
			} catch (Exception e) {
				LOG.Log(Module.GUI, "shouldOverrideUrlLoading: General exception: "+ e.getMessage());
			}
			view.loadUrl(url);
			return true;
		}
		
		@Override
		public WebResourceResponse shouldInterceptRequest (WebView view, String url) {
			// API level 11, won't be called for lower versions
			//LOG.Log(Module.GUI, "shouldInterceptRequest [" + url + "]");			
			if(!(url!=null && url.indexOf(AndroidServiceLocator.INTERNAL_SERVER_URL)>-1 
					&& (url.indexOf("/service/")>-1 || url.indexOf("/service-async/")>-1) )){
				return null;
			}
			
			boolean isSocketListening = AndroidServiceLocator.isSocketListening();
			//LOG.Log(Module.GUI, "*** isSocketListening ?: " + isSocketListening);
			if(!isSocketListening) {
				//LOG.Log(Module.GUI, "*** WARNING - call to service STOPPED. Appverse is not listening right now!!");
				return new WebResourceResponse("text/plain", "utf-8", 
				new ByteArrayInputStream("SECURITY ISSUE".getBytes()));
			} else {
				AndroidServiceLocator.checkResourceIsManagedService(url);
				// Do not handle this request
				return null;
			}
		}
		
		@Override
		public void onLoadResource(WebView view, String url) {
			//LOG.Log(Module.GUI, "loading resource [" + url + "]");
			if(Build.VERSION.SDK_INT < 11){ 
				AndroidServiceLocator.checkResourceIsManagedService(url);
			}
			
			super.onLoadResource(view, url);
		}
	}
}
