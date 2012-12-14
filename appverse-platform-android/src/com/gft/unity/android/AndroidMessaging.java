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
package com.gft.unity.android;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Environment;
import android.telephony.PhoneNumberUtils;

import com.gft.unity.core.messaging.AbstractMessaging;
import com.gft.unity.core.messaging.AttachmentData;
import com.gft.unity.core.messaging.EmailData;
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

// TODO SendEmail and SendMessageSMS: feedback for user cancels, send errors
public class AndroidMessaging extends AbstractMessaging {

	@Override
	// TODO review IMessaging.SendEmail implementation, attachments not working,
	// find a better way to send e-mails
	public boolean SendEmail(EmailData emailData) {
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
							.getMessageBodyMimeType() : "text/plain");
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
			SystemLogger.getInstance().Log(Module.PLATFORM, "SendEmail error",
					ex);
		}

		return result;
	}

	private File createFileFromAttachment(AttachmentData attData)
			throws IOException {
		File tempFile = null;

		if (Environment.MEDIA_MOUNTED.equals(Environment
				.getExternalStorageState())) {
			FileOutputStream fos = null;
			try {
				Context context = AndroidServiceLocator.getContext();
				File externalDir = context.getExternalCacheDir();
				externalDir.mkdirs();
				tempFile = new File(externalDir, attData.getFileName());
				tempFile.delete();
				tempFile.createNewFile();

				fos = new FileOutputStream(tempFile);
				fos.write(attData.getData());
				fos.flush();
			} finally {
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
		boolean result = false;

		try {

			if (PhoneNumberUtils.isWellFormedSmsAddress(phoneNumber)) {
				Uri smsUri = Uri.fromParts("sms", phoneNumber, null);
				Intent intent = new Intent(Intent.ACTION_VIEW, smsUri);
				intent.putExtra("sms_body", text);
				intent.putExtra("address", phoneNumber);
				intent.setType("vnd.android-dir/mms-sms");
				AndroidServiceLocator.getContext().startActivity(
						Intent.createChooser(intent, "SMS"));
				result = true;
			}
		} catch (Exception ex) {
			SystemLogger.getInstance().Log(
					Module.PLATFORM,
					"SendMessageSMS error [" + text + "] to phone ["
							+ phoneNumber + "]", ex);
		}
		return result;
	}

}
