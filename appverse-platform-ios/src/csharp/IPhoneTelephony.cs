/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
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
using System;
using Unity.Core.Telephony;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Threading;
using Unity.Core.Notification;
using System.Text;
using Unity.Core.System;

namespace Unity.Platform.IPhone
{

	public class IPhoneTelephony : AbstractTelephony
	{
		public override ICallControl Call (string phoneNumber, CallType type)
		{
			if (CallType.Voice.Equals (type)) {
				using (var pool = new NSAutoreleasePool ()) {
					StringBuilder filteredPhoneNumber = new StringBuilder();
					if (phoneNumber!=null && phoneNumber.Length>0) {
						foreach (char c in phoneNumber) {
							if (Char.IsNumber(c) || c == '+' || c == '-' || c == '.') {
								filteredPhoneNumber.Append(c);
							}
						}
					}
					String textURI = "tel:" + filteredPhoneNumber.ToString();
					var thread = new Thread (InitiateCall);
					thread.Start (textURI);
				}
				;
			} else {
				INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
				if (notificationService != null) {
					notificationService.StartNotifyAlert ("Phone Alert", "The requested call type is not enabled or supported on this device.", "OK");
				}
			}
			return null;
		}

		[Export("InitiateCall")]
		private void InitiateCall (object textURI)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				NSUrl urlParam = new NSUrl ((string)textURI);
				SystemLogger.Log(SystemLogger.Module.PLATFORM,"Make CALL using URL :"+urlParam.ToString());
				if (UIApplication.SharedApplication.CanOpenUrl (urlParam)) {
					UIApplication.SharedApplication.OpenUrl (urlParam);
				} else {
					INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
					if (notificationService != null) {
						notificationService.StartNotifyAlert ("Phone Alert", "Establishing voice calls is not enabled or supported on this device.", "OK");
					}
				}
			});
		}
	}
}
