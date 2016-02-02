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

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.lang.reflect.Method;
import java.util.ArrayList;

import android.Manifest;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.os.Environment;
import android.telephony.PhoneNumberUtils;


import com.gft.unity.android.activity.AbstractActivityManagerListener;
import com.gft.unity.android.activity.AndroidActivityManager;
import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.core.messaging.AbstractMessaging;
import com.gft.unity.core.messaging.AttachmentData;
import com.gft.unity.core.messaging.EmailData;
import com.gft.unity.core.storage.filesystem.FileData;
import com.gft.unity.core.storage.filesystem.IFileSystem;
import com.gft.unity.core.system.SystemLogger.Module;
import com.gft.unity.core.telephony.CallType;

// TODO SendEmail and SendMessageSMS: feedback for user cancels, send errors
public class AndroidMessaging extends AbstractMessaging {

	
	@Override
	// TODO review IMessaging.SendEmail implementation, attachments not working,
	// find a better way to send e-mails
	public boolean SendEmail(EmailData emailData) {
		try {	

			AndroidActivityManager aam = (AndroidActivityManager) AndroidServiceLocator
					.GetInstance().GetService(AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
			
			aam.requestPermision(Manifest.permission.SEND_SMS, aam.SENDEMAIL, new StoragePermissionListener(emailData));
		} catch (Exception ex) {
			AndroidSystemLogger.getInstance().Log("SendEmail Error", ex);
		} 
		return true;
	}
	// TODO review IMessaging.SendEmail implementation, attachments not working,
	// find a better way to send e-mails
	public boolean SendEmailOnApproval(EmailData emailData) {
		boolean result = false;

		try {
			Intent emailIntent;
			boolean hasAttachment = emailData.getAttachment() != null
					&& emailData.getAttachment().length > 0;
			boolean isMultiple = hasAttachment
					&& emailData.getAttachment().length > 1;

			if (isMultiple) {
				emailIntent = new Intent(Intent.ACTION_SEND_MULTIPLE);
			} else {
				emailIntent = new Intent(Intent.ACTION_SEND);
			}
			emailIntent
					.setType(emailData.getMessageBodyMimeType() != null ? emailData
							.getMessageBodyMimeType() : "text/html");
			emailIntent.putExtra(Intent.EXTRA_SUBJECT, emailData.getSubject());
			emailIntent.putExtra(Intent.EXTRA_TEXT, emailData.getMessageBody());
			emailIntent.putExtra(Intent.EXTRA_EMAIL,
					emailData.getToRecipientsAsString());
			emailIntent.putExtra(Intent.EXTRA_BCC,
					emailData.getBccRecipientsAsString());
			emailIntent.putExtra(Intent.EXTRA_CC,
					emailData.getCcRecipientsAsString());

			if (hasAttachment) {
				if (isMultiple) {
					ArrayList<Uri> uris = new ArrayList<Uri>();
					for (AttachmentData att : emailData.getAttachment()) {
						File attFile = createFileFromAttachment(att);
						if (attFile != null) {
							Uri u = Uri.fromFile(attFile);
							uris.add(u);
						}
					}
					emailIntent.putParcelableArrayListExtra(
							Intent.EXTRA_STREAM, uris);
				}
				Uri u = Uri.fromFile(createFileFromAttachment(emailData
						.getAttachment()[0]));
				emailIntent.putExtra(Intent.EXTRA_STREAM, u);
			}

			Context context = AndroidServiceLocator.getContext();
			context.startActivity(Intent.createChooser(emailIntent, "Email"));
			result = true;
		} catch (Exception ex) {
			AndroidSystemLogger.getInstance().Log(Module.PLATFORM, "SendEmail error",
					ex);
		}

		return result;
	}

	private File createFileFromAttachment(AttachmentData attData)
			throws IOException {
		File tempFile = null;
		Context context = AndroidServiceLocator.getContext();
		AndroidSystemLogger.getInstance().Log(Module.PLATFORM,"createFileFromAttachment");
					
		if (Environment.MEDIA_MOUNTED.equals(Environment
				.getExternalStorageState())) {
			FileOutputStream fos = null;
			try {					
				File externalDir = context.getExternalCacheDir();
				externalDir.mkdirs();
				tempFile = new File(externalDir, attData.getFileName());
				tempFile.delete();					
				tempFile.createNewFile();

				fos = new FileOutputStream(tempFile);
				
				if(attData.getData() == null){		
					File localFile = new File(context.getFilesDir().getAbsolutePath(),  attData.getReferenceUrl()); //new File(FileUri);
					AndroidSystemLogger.getInstance().Log(Module.PLATFORM,"no data binary available, searching as referenced url: "+localFile.getAbsolutePath());
					IFileSystem fileService = (IFileSystem) AndroidServiceLocator
							.GetInstance().GetService(
									AndroidServiceLocator.SERVICE_TYPE_FILESYSTEM);
					FileData fileData = new FileData();
					fileData.setFullName(localFile.getAbsolutePath());
					byte[] buffer = fileService.ReadFile(fileData);
					AndroidSystemLogger.getInstance().Log(Module.PLATFORM,"Referenced file length: " + buffer.length);
					fos.write(buffer);
				}else{
					fos.write(attData.getData());
				}
				fos.flush();
			} catch(Exception ex) {
				AndroidSystemLogger.getInstance().Log(Module.PLATFORM,"Exception while getting external cache directory to create a temporal file (attached email data). Check app permissions.");
				
			}finally {
				if (fos != null) {
					fos.close();
				}
			}
		}
		

		return tempFile;
	}

	@Override
	public boolean SendMessageMMS(String phoneNumber, String text,
			AttachmentData attachment) {
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean SendMessageSMS(String phoneNumber, String text) {
		try {	

			AndroidActivityManager aam = (AndroidActivityManager) AndroidServiceLocator
					.GetInstance().GetService(AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
			
			aam.requestPermision(Manifest.permission.SEND_SMS, aam.REQUEST_SMS, new SMSPermissionListener(phoneNumber, text));
		} catch (Exception ex) {
			AndroidSystemLogger.getInstance().Log(
					Module.PLATFORM,
					"SendMessageSMS error [" + text + "] to phone ["
							+ phoneNumber + "]", ex);
		} 
		return true;
	}
	
	public boolean SendMessageSMSOnApproval(String phoneNumber, String text) {
		boolean result = false;

		try {
			if (PhoneNumberUtils.isWellFormedSmsAddress(phoneNumber)) {
				if(Build.VERSION.SDK_INT >= 19){
					// [MOBPLAT-196] - allow other apps (such as Hangouts to sens sms if Messages native app is not available)
					AndroidSystemLogger.getInstance().Log(Module.PLATFORM,"KitKat version, using uri scheme smsto://");
					Uri smsUri = Uri.fromParts("smsto", phoneNumber, null);
					Intent intent = new Intent(Intent.ACTION_VIEW, smsUri);
					intent.putExtra("sms_body", text);
					intent.putExtra("exit_on_sent", true);  // navigate back to the original activity after message sent [MOBPLAT-190], but it doe snot work for Hangouts app :-(
					
					AndroidServiceLocator.getContext().startActivity(intent);
					
				} else {
					// the type is not valid for other sms apps, only for Messages native app.
					Uri smsUri = Uri.fromParts("sms", phoneNumber, null);
					Intent intent = new Intent(Intent.ACTION_VIEW, smsUri);
					intent.setType("vnd.android-dir/mms-sms");
					intent.putExtra("address", phoneNumber);
					intent.putExtra("sms_body", text);
					intent.putExtra("exit_on_sent", true);  // navigate back to the original activity after message sent [MOBPLAT-190]
					AndroidServiceLocator.getContext().startActivity(
							Intent.createChooser(intent, "SMS"));
				}
				
				result = true;
			}
		} catch (Exception ex) {
			AndroidSystemLogger.getInstance().Log(
					Module.PLATFORM,
					"SendMessageSMS error [" + text + "] to phone ["
							+ phoneNumber + "]", ex);
		}
		return result;
	}
	
	private class SMSPermissionListener extends AbstractActivityManagerListener {

		private String phoneNumber;
		private String text;

		public SMSPermissionListener() {
		}
		
		

		public SMSPermissionListener(String phoneNumber, String text) {
			this.phoneNumber = phoneNumber;
			this.text = text;
		}



		@Override
		public void onOk(int requestCode, Intent data) {

			AndroidSystemLogger.getInstance().Log("SMSPermissionListener.onOk");
			try {	
				SendMessageSMSOnApproval(phoneNumber, text);
			} catch (Exception ex) {
				AndroidSystemLogger.getInstance().Log("SMSPermissionListener.onOk Error", ex);
			} 

		}


		@Override
		public void onCancel(int requestCode, Intent data) {
			AndroidSystemLogger.getInstance().Log("SMSPermissionListener.onCancel");

			try {
				IActivityManager am = (IActivityManager) AndroidServiceLocator
						.GetInstance().GetService(
								AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
				am.executeJS("Appverse.OnSMSDenied", null);
				
			} catch (Exception ex) {
				AndroidSystemLogger.getInstance().Log("SMSPermissionListener.onCancel Error", ex);
			}
		}
	}
	
private class StoragePermissionListener extends AbstractActivityManagerListener {
		private EmailData emailData;
		private StoragePermissionListener(EmailData emailData){
			this.emailData = emailData;
		}
		@Override
		public void onOk(int requestCode, Intent data) {
			
			AndroidSystemLogger.getInstance().Log("StoragePermissionListener.onOk");
			
			try {
				SendEmailOnApproval(emailData);
			} catch (Exception e) {
				AndroidSystemLogger.getInstance().Log("StoragePermissionListener.onOK Error", e);
			}
		}
		
		@Override
		public void onCancel(int requestCode, Intent data) {
			AndroidSystemLogger.getInstance().Log("StoragePermissionListener.onCancel");

			try {
				IActivityManager am = (IActivityManager) AndroidServiceLocator
						.GetInstance().GetService(
								AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
				am.executeJS("Appverse.OnExternalStorageDenied", null);
				
			} catch (Exception ex) {
				AndroidSystemLogger.getInstance().Log("StoragePermissionListener.onCancel Error", ex);
			}
		}
	}

}
