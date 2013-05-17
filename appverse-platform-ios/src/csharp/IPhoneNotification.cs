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
using MonoTouch.AudioToolbox;
using System;
using Unity.Core.Notification;
using System.Threading;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using Unity.Core.System;
using System.Collections.Generic;

namespace Unity.Platform.IPhone
{

	public class IPhoneNotification : AbstractNotification
	{

		private bool AUDIOSESSION_INITIALIZED = false;
		private bool AUDIOSESSION_ACTIVE = false;
		
		private LoadingView loadingView = new LoadingView();
		private static IDictionary<RemoteNotificationType, UIRemoteNotificationType> rnTypes = 
								new Dictionary<RemoteNotificationType, UIRemoteNotificationType> ();

		static IPhoneNotification() {
			rnTypes[RemoteNotificationType.NONE] = UIRemoteNotificationType.None;
			rnTypes[RemoteNotificationType.ALERT] = UIRemoteNotificationType.Alert;
			rnTypes[RemoteNotificationType.BADGE] = UIRemoteNotificationType.Badge;
			rnTypes[RemoteNotificationType.SOUND] = UIRemoteNotificationType.Sound;
			rnTypes[RemoteNotificationType.CONTENT_AVAILABILITY] = UIRemoteNotificationType.NewsstandContentAvailability;
		}

		public override bool StartNotifyBeep ()
		{
			if (!AUDIOSESSION_INITIALIZED) {
				AudioSession.Initialize ();
				AudioSession.Category = AudioSessionCategory.MediaPlayback;
				AUDIOSESSION_INITIALIZED = true;
			}
			
			if (!AUDIOSESSION_ACTIVE) {
				AudioSession.SetActive (true);
				AUDIOSESSION_ACTIVE = true;
			}
			var sound = SystemSound.FromFile ("res/SystemAlert.wav");
			sound.PlayAlertSound ();
			return true;
		}

		public override bool StopNotifyBeep ()
		{
			AudioSession.SetActive (false);
			AUDIOSESSION_ACTIVE = false;
			return true;
		}


		[Export("PlayVibration")]
		private void PlayVibration ()
		{
			using (var pool = new NSAutoreleasePool ()) {
				// Run code in new thread
				while (PLAYING_VIBRATION) {
					SystemSound.Vibrate.PlaySystemSound ();
					Thread.Sleep (VIBRATION_FREQUENCY);
				}
			}
		}

		public override bool StartNotifyVibrate ()
		{
			if (!PLAYING_VIBRATION) {
				PLAYING_VIBRATION = true;
				
				// Starts vibration on background thread
				Thread vibrationThread = new Thread (new ThreadStart (PlayVibration));
				vibrationThread.Start ();
			}
			return true;
		}

		public override bool StopNotifyVibrate ()
		{
			PLAYING_VIBRATION = false;
			return true;
		}

		public override bool IsNotifyActivityRunning ()
		{
			return UIApplication.SharedApplication.NetworkActivityIndicatorVisible;
		}

		public override bool IsNotifyLoadingRunning ()
		{
			// return loadingView.Visible;
			// should be invoked on main thread, but it couldn't be done returning a result
			return false;
		}

		public override bool StartNotifyActivity ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			
			return true;
		}

		public override bool StartNotifyAlert (string message)
		{
			return StartNotifyAlert ("Alert", message, "OK");
		}

		public override bool StartNotifyAlert (string title, string message, string buttonText)
		{
			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (ShowAlert);
				string[] alertData = new string[] { title, message, buttonText };
				thread.Start (alertData);
			}
			return true;
		}

		[Export("ShowAlert")]
		private void ShowAlert (object alertData)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				string[] alertParams = (string[]) alertData;
				UIAlertView alert = new UIAlertView(alertParams[0],alertParams[1],null,alertParams[2],null);
				alert.Show();
				
				// Stops notify loading if any
				this.StopNotifyLoading();
			});
		}
		
		
		public override bool StartNotifyActionSheet(string title, string[] buttons, string[] javascriptCallBackFunctions) {
			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (ShowActionSheet);
				ActionSheet sheet = new ActionSheet();
				sheet.Title = title;
				sheet.Buttons = buttons;
				sheet.JsCallbackFunctions = javascriptCallBackFunctions;
				thread.Start (sheet);
			}
			return true;
		}
		
		[Export("ShowActionSheet")]
		private void ShowActionSheet (object sheet)
		{
			ActionSheet actionSheet = (ActionSheet) sheet;
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				
				if(actionSheet.Title == null) {
					actionSheet.Title = ""; // null value cannot be passed to UIActionSheet constructor.
				}
				
				string cancelButton = null;
				string[] buttons = new string[0];
				
				if(IPhoneUtils.GetInstance().IsIPad()) {
					// For iPad, cancelButton cannot be specified. It has to be included as an "other" button.
					buttons = actionSheet.Buttons;
				} else {
					if(actionSheet.Buttons != null && actionSheet.Buttons.Length>0) {
						buttons = new string[actionSheet.Buttons.Length-1];
						cancelButton = actionSheet.Buttons[0];
						for(int i=1; i< actionSheet.Buttons.Length; i++) {
							buttons[i-1] = actionSheet.Buttons[i];
						}
					} else {
						// default
						cancelButton = "Cancel";
					}
				}
				
				UIActionSheet uiSheet = new UIActionSheet(actionSheet.Title, new ActionSheetDelegate(actionSheet.JsCallbackFunctions), cancelButton, null, buttons);
				uiSheet.ShowInView(IPhoneServiceLocator.CurrentDelegate.MainUIViewController().View);
			});
		}
			
		private class ActionSheetDelegate : UIActionSheetDelegate {
			
			private string[] JsCallBackFunctions = null;
			
			public ActionSheetDelegate(string[] javascriptCallBackFunctions) {
				JsCallBackFunctions = javascriptCallBackFunctions;
			}
			
			public override void Clicked(UIActionSheet actionSheet, int index) {
				UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
					if(JsCallBackFunctions != null && JsCallBackFunctions.Length>0 && JsCallBackFunctions.Length>=index) {
						IPhoneServiceLocator.CurrentDelegate.MainUIWebView().EvaluateJavascript(JsCallBackFunctions[index]);
					}
				});
			}
		}

		public override bool StartNotifyBlink ()
		{
			throw new System.NotImplementedException ();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="loadingText">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public override bool StartNotifyLoading (string loadingText)
		{
			if(loadingText==null) {
				return StartNotifyLoading();
			}
			
			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (ShowLoading);
				thread.Start (loadingText);
			}
			return true;
		}
		
		[Export("ShowLoading")]
		private void ShowLoading (object text)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				loadingView.Show((string)text);
			});
		}
		
		private class LoadingView : UIAlertView
		{
			private UIActivityIndicatorView _activityView;
			
			public void Show(string title) {
				
				// Show alert
				Title = title;
				Show();
				
				// Show loadind indicator inside
				_activityView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
				_activityView.Frame = new RectangleF((Bounds.Width / 2) - 15, Bounds.Height - 50, 30, 30);
				_activityView.StartAnimating();
    			AddSubview(_activityView);
			}
			
			public void Hide() {
				DismissWithClickedButtonIndex(0,true);
			}
			
		}
		
		public override bool StopNotifyActivity ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			
			return true;
		}

		public override bool StopNotifyAlert ()
		{
			throw new System.NotImplementedException ();
		}

		public override bool StopNotifyBlink ()
		{
			throw new System.NotImplementedException ();
		}

		public override bool StopNotifyLoading ()
		{
			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (DismissLoading);
				thread.Start ();
			}

			return true;
		}

		[Export("DismissLoading")]
		private void DismissLoading ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				if(loadingView.Visible) {
					loadingView.Hide();
				}
			});
		}

		public override void UpdateNotifyLoading (float progress)
		{
			throw new System.NotImplementedException ();
		}
		


		public override void RegisterForRemoteNotifications (string senderId, RemoteNotificationType[] types)
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM,"Registering senderId ["+ senderId +"] for receiving  push notifications");

			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.None;
				try {
					if(types != null) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM,"Remote Notifications types enabled #num : " + types.Length);
						foreach(RemoteNotificationType notificationType in types) {
							notificationTypes = notificationTypes | rnTypes[notificationType] ;
						}
					}

					SystemLogger.Log(SystemLogger.Module.PLATFORM,"Remote Notifications types enabled: " + notificationTypes);

					//This tells our app to go ahead and ask the user for permission to use Push Notifications
					// You have to specify which types you want to ask permission for
					// Most apps just ask for them all and if they don't use one type, who cares
					UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
				} catch(Exception e) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM,"Exception ocurred: " + e.Message);
				}
			});
		}


		public override void UnRegisterForRemoteNotifications ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				UIApplication.SharedApplication.UnregisterForRemoteNotifications();
			});
		}

		public override void SetApplicationIconBadgeNumber (int badge) {
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				UIApplication.SharedApplication.ApplicationIconBadgeNumber = badge;
			});
		}
		
		public override void IncrementApplicationIconBadgeNumber () {
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				int currentBadge = UIApplication.SharedApplication.ApplicationIconBadgeNumber;
				UIApplication.SharedApplication.ApplicationIconBadgeNumber = currentBadge + 1 ;
			});
		}
		
		public override void DecrementApplicationIconBadgeNumber () {
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				int currentBadge = UIApplication.SharedApplication.ApplicationIconBadgeNumber;
				UIApplication.SharedApplication.ApplicationIconBadgeNumber = currentBadge - 1 ;
			});
		}

	}
}
