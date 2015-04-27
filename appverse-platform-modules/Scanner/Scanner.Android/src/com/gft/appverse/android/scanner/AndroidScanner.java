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

import java.util.List;

import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.net.Uri;
import android.os.Bundle;
import android.telephony.PhoneNumberUtils;
import android.webkit.WebView;
import android.app.Activity;
import android.app.AlertDialog;

import com.gft.unity.core.json.JSONSerializer;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.google.zxing.BarcodeFormat;
import com.google.zxing.Result;
import com.google.zxing.client.result.ParsedResult;
import com.google.zxing.client.result.ResultParser;

public class AndroidScanner extends AbstractScanner {

	private static final String LOGGER_MODULE = "Scanner Module";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);	

	private static final String PHONE_PREFIX = "tel://";

	private Context context;
	private WebView webView;
	
	
	public AndroidScanner(Context ctx, WebView appView) {
		super();
		LOGGER.logInfo("Init", "Initializing Scanner Module Service");
		context = ctx;
		webView = appView;
	}

	@Override
	public void DetectQRCode(boolean autoHandleQR) {			
		try {

			final Activity mainActivity = (Activity) context;
			
			final Intent intent = new Intent("com.google.zxing.client.android.SCAN");
			intent.setPackage(context.getPackageName());
			intent.putExtra("SCAN_MODE", "QR_CODE_MODE");
			
			final boolean autoHandle = autoHandleQR;
			
			final PackageManager pckmanager = context.getPackageManager();
			List<ResolveInfo> resolveInfo = pckmanager.queryIntentActivities(intent, PackageManager.MATCH_DEFAULT_ONLY);
			if(resolveInfo.size()>0) {
				
				mainActivity.runOnUiThread(new Runnable() {
					
					@Override
					public void run() {
						if(autoHandle)
							mainActivity.startActivityForResult(intent, ScannerResultReceiver.QRCODE_DETECT_RC_AUTOHANDLE);
						else
							mainActivity.startActivityForResult(intent, ScannerResultReceiver.QRCODE_DETECT_RC_NOT_AUTOHANDLE);
					}
				});
				
			} else {
				LOGGER.logWarning("DetectQRCode", "Intent 'com.google.zxing.client.android.SCAN', is not found. Please verify you have configured it in your manifest.");
			}
		} catch (Exception ex) {
			LOGGER.logError("DetectQRCode", "Error", ex);
		}      	
	}
	
	@Override
	public QRType HandleQRCode(MediaQRContent mediaQRContent) {
		try {
			if(mediaQRContent != null && mediaQRContent.getQRType() != null){
				
				Intent intent;
				Activity mainActivity = (Activity) context;
				
				switch(mediaQRContent.getQRType()){				
				case TEL:
					String numberFormatted = PhoneNumberUtils.formatNumber(mediaQRContent.getText().substring(4));
					intent = new Intent(Intent.ACTION_CALL,Uri.parse(PHONE_PREFIX + numberFormatted));
					intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
					mainActivity.startActivity(intent);				
					break;				
				case URI:
					intent = new Intent(Intent.ACTION_VIEW);
					intent.setData(Uri.parse(mediaQRContent.getText()));
					mainActivity.startActivity(intent);				
					break;
				case EMAIL_ADDRESS:
					intent = new Intent(Intent.ACTION_SEND);
					if(mediaQRContent.getText().toLowerCase().startsWith("mailto:")){
						String[] emailFields = getEmailFieldsFromQR(mediaQRContent.getText());
						intent.setType("text/html");
						intent.putExtra(Intent.EXTRA_EMAIL, new String[] {emailFields[0]});
						intent.putExtra(Intent.EXTRA_SUBJECT, emailFields[1]);
						intent.putExtra(Intent.EXTRA_TEXT, emailFields[2]);
						intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
						mainActivity.startActivity(Intent.createChooser(intent, "Email"));
						break;
					}//else execute the DEFAULT block
				default:
					this.notifyAlert (mainActivity, 
							"QR Alert", 
							"The QR Code " + mediaQRContent.getQRType().toString() + " cannot be processed automatically.", 
							"OK");
				}			
				return mediaQRContent.getQRType();
			}
		} catch(Exception ex) {
			LOGGER.logError("HandleQRCode", "Error", ex);
		}
		return null;
	}
	
	public void onOk(Bundle data, boolean bAutoHandle) {
		LOGGER.logInfo("DetectQRCodeListener.onOk", ((data!=null)?"result data received": ""));
		try {			
			String contents = data.getString("SCAN_RESULT");
	        String format = data.getString("SCAN_RESULT_FORMAT");
	        
	        Result p = new Result(contents, null, null, BarcodeFormat.valueOf(format));
	        MediaQRContent mediaQRContent = new MediaQRContent(contents, ZxingToBarcode(BarcodeFormat.valueOf(format)), getQRTypeFromCode(p));
	        
	       this.executeJS((Activity)context, "Appverse.Scanner.onQRCodeDetected", mediaQRContent);
	        
	        if(bAutoHandle){//HANDLE EVERYTHING
	        	HandleQRCode(mediaQRContent);		        	
	        }
		} catch (Exception ex) {
			LOGGER.logError("DetectQRCodeListener.onOk", "Error", ex);
		}
	}
	
	private BarCodeType ZxingToBarcode (BarcodeFormat format){
		for (BarCodeType type : BarCodeType.values()) {
			if(format.toString().equals(type.toString())){
				return type;				
			}
		}
		return BarCodeType.DEFAULT;		
	}
		
	private QRType getQRTypeFromCode(Result readQRCode){
		ParsedResult parsed = ResultParser.parseResult(readQRCode);
		switch(parsed.getType()){
			case ADDRESSBOOK:
				return QRType.ADDRESSBOOK;			
			case CALENDAR:
				return QRType.CALENDAR;			
			case EMAIL_ADDRESS:
				return QRType.EMAIL_ADDRESS;
			case GEO:
				return QRType.GEO;
			case ISBN:
				return QRType.ISBN;
			case PRODUCT:
				return QRType.PRODUCT;
			case SMS:
				return QRType.SMS;
			case TEL:
				return QRType.TEL;
			case URI:
				return QRType.URI;
			case WIFI:
				return QRType.WIFI;
			case TEXT:
			default:
				return QRType.TEXT;
		}	
	}
	
	private void executeJS(Activity main, String method, Object data) {
 
		if (this.webView != null) {
			String jsonData = "null";
			if(data != null) {
				jsonData = JSONSerializer.serialize(data);
			}
			String jsCallbackFunction = "javascript:if(" + method + "){" + method + "("
					+ jsonData + ");}";

			main.runOnUiThread(new AAMExecuteJS(this.webView, jsCallbackFunction));
		}

	}
	
	private class AAMExecuteJS implements Runnable {

		private String javascript;
		private WebView view;
		

		public AAMExecuteJS(WebView view, String javascript) {
			this.javascript = javascript;
			this.view = view;
		}

		@Override
		public void run() {
			if(this.view != null) {
				this.view.loadUrl(this.javascript);
			}
		}
	}
	
	private void notifyAlert(Activity mainActivity, String title, String message,
			String buttonText) {
		
		final String alertTitle = title;
		final String alertMessage = message;
		final String alertButtonText = buttonText;
		
		Runnable action = new Runnable() {

			@Override
			public void run() {
				AlertDialog.Builder builder = new AlertDialog.Builder(context);
				if (alertTitle != null && !alertTitle.equals("")) {
					builder.setTitle(alertTitle);
				}
				if (alertMessage != null && !alertMessage.equals("")) {
					builder.setMessage(alertMessage);
				}
				if (alertButtonText != null && !alertButtonText.equals("")) {

					builder.setPositiveButton(alertButtonText,
							new DialogInterface.OnClickListener() {

								@Override
								public void onClick(DialogInterface dialog,
										int which) {
									dialog.cancel();
								}
							}).create();

				}
				AlertDialog dialog = builder.create();
				dialog.setCancelable(true);
				dialog.show();
			}
		};

		mainActivity.runOnUiThread(action);
		
	}
	
	private String [] getEmailFieldsFromQR(String text){
		String [] fields = new String[3];
		fields[0] = text.split("\\?")[0].substring(7);
		fields[1] = text.split("\\?")[1].split("\\&")[0].substring(8);
		fields[2] = text.split("\\?")[1].split("\\&")[1].substring(5);
		return fields;		
	}
}
