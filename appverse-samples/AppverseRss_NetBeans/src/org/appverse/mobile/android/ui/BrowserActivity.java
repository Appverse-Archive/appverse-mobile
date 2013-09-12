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
package org.appverse.mobile.android.ui;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings.ZoomDensity;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;

import com.gft.unity.android.AndroidNet;

public class BrowserActivity extends Activity {

	private static final String LAYOUT_ID = "browserlayout";
	private static final String BTN_CLOSE_ID = "btnCloseBrowser";
	private static final String WEBVIEW_ID = "browserwebview";

	private static final String ID_TYPE = "id";
	private static final String LAYOUT_TYPE = "layout";

	private static final String DEFAULT_BROWSER_TITLE = "";
	private static final String DEFAULT_BTN_CLOSE_TEXT = "Close"; // TODO i18n

	private WebView mWebView;
	private Button btnClose;

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

		mWebView.setWebViewClient(new UnityWebViewClient());

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

	@Override
	protected void onResume() {
		super.onResume();

		mWebView.getSettings().setJavaScriptEnabled(true);
		Intent intent = getIntent();
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

	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {

		if ((keyCode == KeyEvent.KEYCODE_BACK) && mWebView.canGoBack()) {
			mWebView.goBack();
			return true;
		}

		return super.onKeyDown(keyCode, event);
	}

	private class UnityWebViewClient extends WebViewClient {

		@Override
		public boolean shouldOverrideUrlLoading(WebView view, String url) {
			view.loadUrl(url);
			return true;
		}
	}
}
