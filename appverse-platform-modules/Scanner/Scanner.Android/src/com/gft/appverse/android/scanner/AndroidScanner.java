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

import java.io.BufferedInputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.util.Hashtable;
import java.util.List;
import java.util.UUID;

import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.net.Uri;
import android.os.Bundle;
import android.telephony.PhoneNumberUtils;
import android.webkit.MimeTypeMap;
import android.Manifest;
import android.app.Activity;
import android.app.AlertDialog;

import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.activity.AbstractActivityManagerListener;
import com.gft.unity.android.activity.AndroidActivityManager;
import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.core.media.MediaMetadata;
import com.gft.unity.core.media.MediaType;
import com.gft.unity.core.media.camera.CameraOptions;
import com.gft.unity.core.storage.filesystem.IFileSystem;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;
import com.google.zxing.BarcodeFormat;
import com.google.zxing.EncodeHintType;
import com.google.zxing.Result;
import com.google.zxing.WriterException;
import com.google.zxing.client.result.ParsedResult;
import com.google.zxing.client.result.ResultParser;
import com.google.zxing.common.BitMatrix;
import com.google.zxing.qrcode.QRCodeWriter;
import com.google.zxing.qrcode.decoder.ErrorCorrectionLevel;
import com.google.zxing.qrcode.encoder.ByteMatrix;

public class AndroidScanner extends AbstractScanner {

	private static final String LOGGER_MODULE = "Scanner Module";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);	

	private static final String PHONE_PREFIX = "tel://";

	private Context context;
	private IActivityManager activityManager;
		
	
	public AndroidScanner(Context ctx, IActivityManager aam) {
		super();
		LOGGER.logInfo("Init", "Initializing Scanner Module Service");
		context = ctx;
		activityManager = aam;
		
	}

	@Override
	public void DetectQRCode(boolean autoHandleQR) {	
		try {	
			AndroidActivityManager aam = (AndroidActivityManager) AndroidServiceLocator
					.GetInstance().GetService(AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
			
			aam.requestPermision(Manifest.permission.CAMERA, aam.SCANNER, new CameraPermissionListener(autoHandleQR, false));
		} catch (Exception ex) {
			LOGGER.logError("TakeSnapshotWithOptions", "Error", ex);
		} 
	}
	
	public void DetectQRCodeOnApproval(boolean autoHandleQR) {			
		try {
			CameraPreferences.getInstance().setFront(false);
			
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
	public void DetectQRCodeFront(boolean autoHandleQR) {	
		try {	
			AndroidActivityManager aam = (AndroidActivityManager) AndroidServiceLocator
					.GetInstance().GetService(AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
			
			aam.requestPermision(Manifest.permission.CAMERA, aam.SCANNER, new CameraPermissionListener(autoHandleQR, true));
		} catch (Exception ex) {
			LOGGER.logError("TakeSnapshotWithOptions", "Error", ex);
		} 
	}
	
	public void DetectQRCodeFrontOnApproval(boolean autoHandleQR) {	
		try {

			CameraPreferences.getInstance().setFront(true);
			
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
	
	private class CameraPermissionListener extends AbstractActivityManagerListener {

		private boolean autoHandle;
		private boolean front;
		
		public CameraPermissionListener() {

		}
		
		public CameraPermissionListener(boolean autoHandle, boolean front) {

			this.autoHandle = autoHandle;
			this.front = front;
		}
		

		@Override
		public void onOk(int requestCode, Intent data) {

			LOGGER.logInfo("CameraPermissionListener.onOk", "requestCode: "+requestCode);


			try {
				if(this.front){
					DetectQRCodeFrontOnApproval(this.autoHandle);
				}else{
					DetectQRCodeOnApproval(this.autoHandle);
				
				}
				
			} catch (Exception ex) {
				LOGGER.logError("CameraPermissionListener.onOk", "Error", ex);
			}
		}


		@Override
		public void onCancel(int requestCode, Intent data) {
			LOGGER.logInfo("CameraPermissionListener.onCancel", ((data!=null)?data.getDataString(): ""));

			try {
				IActivityManager am = (IActivityManager) AndroidServiceLocator
						.GetInstance().GetService(
								AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
				am.executeJS("Appverse.Scanner.onAccessToCameraDenied", null);
				
			} catch (Exception ex) {
				LOGGER.logError("CameraPermissionListener.onCancel", "Error", ex);
			}
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
	
	public void onCancel(Bundle data, boolean bAutoHandle) {
		LOGGER.logInfo("DetectQRCodeListener.onCancel", ((data!=null)?"result data received": ""));
		try {			
			
	       this.executeJS((Activity)context, "Appverse.Scanner.onQRCodeDetected", null);
	        
	      
		} catch (Exception ex) {
			LOGGER.logError("DetectQRCodeListener.onCancel", "Error", ex);
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
 
		if (this.activityManager != null) {
			
			this.activityManager.executeJS(method, data);
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

	@Override
	public void GenerateQRCode(MediaQRContent content) {
		MediaMetadata meta = null;
		try {
			
		    // generate a 150x150 QR code
		    //Bitmap bm = encodeAsBitmap(content.getText(), BarcodeFormat.QR_CODE, 150, 150);
			content = encodeQRCodeContents(content);		
			Bitmap bm = generateQrCode(content);
		    if(bm != null) {
		    	try {
		    		meta = new MediaMetadata();
		    		String title = "QR_" + UUID.randomUUID();
					File qrPath = new File(context.getExternalCacheDir(),
							title + ".jpeg");
					
				    FileOutputStream fOut = new FileOutputStream(qrPath);					
					bm.compress(Bitmap.CompressFormat.JPEG, 100, fOut); // bmp is your Bitmap instance
				    fOut.flush();
				    fOut.close();
					
				    Uri uri = Uri.fromFile(qrPath);

					// retrieve image metadata
					
					meta.setType(MediaType.Photo);
					meta.setMimeType("image/jpeg");
					meta.setTitle(title);

					// copy image to internal storage
					copyImageToInternalStorage(uri, meta);

					
					

				} catch (Exception ex) {
					LOGGER.logError("GenerateQRCode", "Error", ex);
				}
		    }
		} catch (WriterException e) { 
			
		}
		this.executeJS((Activity)context, "Appverse.Scanner.onGeneratedQR", meta);
	}
	
	
	public static Bitmap generateQrCode(MediaQRContent myCode) throws WriterException {
        Hashtable<EncodeHintType, ErrorCorrectionLevel> hintMap = new Hashtable<EncodeHintType, ErrorCorrectionLevel>();
        hintMap.put(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H); // H = 30% damage
        
        QRCodeWriter qrCodeWriter = new QRCodeWriter();
        int size = myCode.getSize(); 
        if(size == 0)
        	 size = 256;         

        BitMatrix bitMatrix = qrCodeWriter.encode(myCode.getText(),BarcodeFormat.QR_CODE, size, size, hintMap);
        int width = bitMatrix.getWidth();
        Bitmap bmp = Bitmap.createBitmap(width, width, Bitmap.Config.RGB_565);
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < width; y++) {
                bmp.setPixel(x, y, bitMatrix.get(x, y) ? Color.BLACK : Color.WHITE);
            }
        }
        return bmp;
    }
	
	private static String trim(String s) {
        if (s == null) { return null; }
        String result = s.trim();
        return result.length() == 0 ? null : result;
    }
		
	private static String escapeMECARD(String input) {
        if (input == null || (input.indexOf(':') < 0 && input.indexOf(';') < 0)) { return input; }
        int length = input.length();
        StringBuilder result = new StringBuilder(length);
        for (int i = 0; i < length; i++) {
            char c = input.charAt(i);
            if (c == ':' || c == ';') {
                result.append('\\');
            }
            result.append(c);
        }
        return result.toString();
    }
	 
	private MediaQRContent encodeQRCodeContents(MediaQRContent qrCode) {
       if (qrCode.getQRType().equals(QRType.EMAIL_ADDRESS)) {
            String data = trim(qrCode.getText());
            if (data != null) {
            	qrCode.setText("mailto:" + data);
                
            }
        } else if (qrCode.getQRType().equals(QRType.TEL)) {
        	String data = trim(qrCode.getText());
            if (data != null) {
            	qrCode.setText("tel:" + data);
                
            }
        } else if (qrCode.getQRType().equals(QRType.SMS)) {
        	String data = trim(qrCode.getText());
            if (data != null) {
            	qrCode.setText("sms:" + data);
                
            }
        } else if (qrCode.getQRType().equals(QRType.ADDRESSBOOK)) {
        	
            if (qrCode.getContact() != null) {
            	
                StringBuilder newContents = new StringBuilder(100);
                
                newContents.append("MECARD:");
 
                String name = trim(qrCode.getContact().getName());
                if (name != null) {
                    newContents.append("N:").append(escapeMECARD(name)).append(';');
                    
                }
 
                String address = trim(qrCode.getContact().getAddress());
                if (address != null) {
                    newContents.append("ADR:").append(escapeMECARD(address)).append(';');
                    
                }
 
                String phone = trim(qrCode.getContact().getPhone());
                if (phone != null) {
                
                    newContents.append("TEL:").append(escapeMECARD(phone)).append(';');
                    
                }
                
                String email = trim(qrCode.getContact().getEmail());
                if (email != null) {
                    newContents.append("EMAIL:").append(escapeMECARD(email)).append(';');
                    
                }
 
                String url = trim(qrCode.getContact().getUrl());
                if (url != null) {
                    // escapeMECARD(url) -> wrong escape e.g. http\://zxing.google.com
                    newContents.append("URL:").append(url).append(';');
                    
                }
 
                String note = trim(qrCode.getContact().getNote());
                if (note != null) {
                    newContents.append("NOTE:").append(escapeMECARD(note)).append(';');
                    
                }
 
                // Make sure we've encoded at least one field.
                if (newContents.length() > 0) {
                    newContents.append(';');
                    qrCode.setText(newContents.toString());
                    
                } else {
                	qrCode.setText(null);
                }
 
            }
        } else if (qrCode.getQRType().equals(QRType.GEO)) {
        	Coordinate coord = qrCode.getCoord();
            if (coord != null) {
                // These must use Bundle.getFloat(), not getDouble(), it's part of the API.
                float latitude = coord.getLatitude();
                float longitude = coord.getLongitude();
                if (latitude != Float.MAX_VALUE && longitude != Float.MAX_VALUE) {
                	qrCode.setText("geo:" + latitude + ',' + longitude);                    
                }
            }
        }
       	
       return qrCode;
    }
	
	
	private static void copyImageToInternalStorage(Uri uri, MediaMetadata meta) {

		BufferedInputStream bis = null;
		ByteArrayOutputStream baos = null;
		try {
			if(meta == null) 
				meta = new MediaMetadata();
			// read image data
			bis = new BufferedInputStream(AndroidServiceLocator.getContext()
					.getContentResolver().openInputStream(uri));
			baos = new ByteArrayOutputStream();
			byte[] buffer = new byte[1024];
			int length;
			while ((length = bis.read(buffer)) > 0) {
				baos.write(buffer, 0, length);
			}

			// store image in the application files folder
			IFileSystem fileService = (IFileSystem) AndroidServiceLocator
					.GetInstance().GetService(
							AndroidServiceLocator.SERVICE_TYPE_FILESYSTEM);
			String extension = getExtension(meta.getMimeType());
			String name = meta.getTitle();
			if (!name.endsWith(".jpg") && !name.endsWith(".jpeg")) {
				name += "." + extension;
			}
			String path = fileService.StoreFile(fileService.GetDirectoryRoot()
					.getFullName(), name, baos.toByteArray());
			File f = new File(path);
			LOGGER.logDebug("copyImageToInternalStorage", "uri.path: "+uri.getPath() + " || path: " + path);
			long size = f.length();
			LOGGER.logDebug("File Size", "******************************* FILE SIZE: "+size);
			// TODO StoreFile should return a relative path
			path = path.substring(path.lastIndexOf('/') + 1);
			meta.setReferenceUrl(path);
			
			
						
			meta.setSize(size);
			
		} catch (Exception ex) {
			LOGGER.logError("CopyImageToInternalStorage", "Error", ex);
		} finally {
			if (bis != null) {
				try {
					bis.close();
				} catch (Exception ex) {
				}
			}
			if (baos != null) {
				try {
					baos.close();
				} catch (Exception ex) {
				}
			}
		}
	}
	
	private static String getExtension(String mimeType) {
		return MimeTypeMap.getSingleton().getExtensionFromMimeType(mimeType);
	}

	
}
