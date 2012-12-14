/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
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
using Unity.Core.Messaging;
using MonoTouch.MessageUI;
using System.Threading;
using MonoTouch.Foundation;
using Unity.Core.Notification;
using MonoTouch.UIKit;
using System;
using Unity.Core.System;
using System.Text;
using System.IO;
using Unity.Core.Storage.FileSystem;

namespace Unity.Platform.IPhone
{


	public class IPhoneMessaging : AbstractMessaging
	{

		public override bool SendEmail (EmailData emailData)
		{	UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				if (MFMailComposeViewController.CanSendMail) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM,"device supports email send");
					using (var pool = new NSAutoreleasePool ()) {
						var thread = new Thread (ShowMailComposer);
						thread.Start (emailData);
					}
					;
				} else {
					INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
					if (notificationService != null) {
						notificationService.StartNotifyAlert ("Mail Alert", "Sending of mail messages is not enabled or supported on this device.", "OK");
					}
				}
			});
			return true;
		}

		[Export("ShowMailComposer")]
		private void ShowMailComposer (object emailObject)
		{	
			SystemLogger.Log(SystemLogger.Module.PLATFORM,"ShowMailComposer... ");
			EmailData emailData = (EmailData)emailObject;
			
			
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				
				
				MFMailComposeViewController vcMail = new MFMailComposeViewController ();
				
				// To
				if (emailData.ToRecipientsAsString.Length > 0) {
					vcMail.SetToRecipients (emailData.ToRecipientsAsString);
				}
				// Cc
				if (emailData.CcRecipientsAsString.Length > 0) {
					vcMail.SetCcRecipients (emailData.CcRecipientsAsString);
				}
				// Bcc
				if (emailData.BccRecipientsAsString.Length > 0) {
					vcMail.SetBccRecipients (emailData.BccRecipientsAsString);
				}
				// Subject
				if (emailData.Subject != null) {
					vcMail.SetSubject (emailData.Subject);
				}
				// Body
				bool IsHtml = "text/html".Equals (emailData.MessageBodyMimeType);
				if (emailData.MessageBody != null) {
					vcMail.SetMessageBody (emailData.MessageBody, IsHtml);
				}
				// Attachement
				if (emailData.Attachment != null) {
					foreach (AttachmentData attachment in emailData.Attachment) {
						try {
							
							NSData data = null;
							if(attachment.Data == null || attachment.Data.Length == 0) {
								
								IFileSystem fileSystemService = (IFileSystem)IPhoneServiceLocator.GetInstance ().GetService ("file");
								string fullPath = Path.Combine(fileSystemService.GetDirectoryRoot().FullName, attachment.ReferenceUrl);
								data = NSData.FromFile(fullPath);
								if(attachment.FileName == null || attachment.FileName.Length == 0) {
									attachment.FileName = Path.GetFileName(attachment.ReferenceUrl);
								}
							} else {
								data = NSData.FromArray (attachment.Data);
							}
							if(data != null) {
								vcMail.AddAttachmentData (data, attachment.MimeType, attachment.FileName);
							}
						} catch (Exception e) {
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "Error adding attachment to Mail Composer.", e);
						}
					}
				}
				
				vcMail.Finished += HandleMailFinished;
				
				IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().PresentModalViewController (vcMail, true);
				
			});
		}

		void HandleMailFinished (object sender, MFComposeResultEventArgs e)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				if (e.Result == MFMailComposeResult.Sent) {
					// No action required
				} else if (e.Result == MFMailComposeResult.Cancelled) {
					// No action required
				} else if (e.Result == MFMailComposeResult.Failed) {
					INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
					if (notificationService != null) {
						notificationService.StartNotifyAlert ("Mail Error", "Failed to send mail.\n" + e.Error, "OK");
					}
				} else if (e.Result == MFMailComposeResult.Saved) {
					INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
					if (notificationService != null) {
						notificationService.StartNotifyAlert ("Mail Alert", "Mail saved to draft.", "OK");
					}
				}
				e.Controller.DismissModalViewControllerAnimated (true);
			});
		}

		public override bool SendMessageMMS (string phoneNumber, string text, AttachmentData attachment)
		{
			return SendMessageSMS (phoneNumber, text);
			// MMS not supported yet
		}

		public override bool SendMessageSMS (string phoneNumber, string text)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				StringBuilder filteredPhoneNumber = new StringBuilder ();
				if (phoneNumber != null && phoneNumber.Length > 0) {
					foreach (char c in phoneNumber) {
						if (Char.IsNumber (c) || c == '+' || c == '-' || c == '.') {
							filteredPhoneNumber.Append (c);
						}
					}
				}
				string textBody = "";
				/* NOTE: sms scheme with body is not working well with current iOS versions... we are waiting for them to fix this on further versions
				if(text!=null) {
					textBody="?body=" + Uri.EscapeUriString(text);
				}
				*/
				NSUrl urlParam = new NSUrl ("sms:" + filteredPhoneNumber.ToString() + textBody);
				if (UIApplication.SharedApplication.CanOpenUrl (urlParam)) {
					using (var pool = new NSAutoreleasePool ()) {
						var thread = new Thread (ShowTextComposer);
						thread.Start (urlParam);
					}
					;
				} else {
					INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
					if (notificationService != null) {
						notificationService.StartNotifyAlert ("Message Alert", "Sending of text messages is not enabled or supported on this device.", "OK");
					}
				}
			});
			return true;
		}

		[Export("ShowTextComposer")]
		private void ShowTextComposer (object textURI)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				NSUrl urlParam = (NSUrl)textURI;
				SystemLogger.Log(SystemLogger.Module.PLATFORM,"Send SMS using URL :"+urlParam.ToString());
				UIApplication.SharedApplication.OpenUrl (urlParam);
			});
		}
	}
}
