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
using AudioToolbox;
using System;
using Unity.Core.Notification;
using System.Threading;
using Foundation;
using UIKit;
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
			
			public override void Clicked(UIActionSheet actionSheet, nint index) {
				UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
					if(JsCallBackFunctions != null && JsCallBackFunctions.Length>0 && JsCallBackFunctions.Length>=index) {
						IPhoneServiceLocator.CurrentDelegate.EvaluateJavascript(JsCallBackFunctions[index]);
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
				_activityView.Frame = new CoreGraphics.CGRect((((nfloat)Bounds.Width) / 2) - 15, ((nfloat)Bounds.Height) - 50, 30, 30);
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



		public override void SetApplicationIconBadgeNumber (int badge) {
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				UIApplication.SharedApplication.ApplicationIconBadgeNumber = badge;
			});
		}
		
		public override void IncrementApplicationIconBadgeNumber () {
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				nint currentBadge = UIApplication.SharedApplication.ApplicationIconBadgeNumber;
				UIApplication.SharedApplication.ApplicationIconBadgeNumber = currentBadge + 1 ;
			});
		}
		
		public override void DecrementApplicationIconBadgeNumber () {
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				nint currentBadge = UIApplication.SharedApplication.ApplicationIconBadgeNumber;
				UIApplication.SharedApplication.ApplicationIconBadgeNumber = currentBadge - 1 ;
			});
		}

		private UILocalNotification PrepareLocalNotification (NotificationData notification) {
			if(notification!=null) {
				UILocalNotification localNotification = new UILocalNotification();
				localNotification.AlertBody = notification.AlertMessage;
				localNotification.ApplicationIconBadgeNumber = notification.Badge;
				localNotification.SoundName = UILocalNotification.DefaultSoundName; // defaults
				if(notification.Sound != null && notification.Sound.Length>0 && !notification.Sound.Equals("default")) {
					// for sounds different from the default one
					localNotification.SoundName = notification.Sound;
				}
				if(notification.CustomDataJsonString != null && notification.CustomDataJsonString.Length>0) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM,"Custom Json String received: " + notification.CustomDataJsonString);

					Dictionary<String,Object> userDictionary = (Dictionary<String,Object>) IPhoneUtils.GetInstance().JSONDeserialize<Dictionary<String,Object>>(notification.CustomDataJsonString);
					localNotification.UserInfo = IPhoneUtils.GetInstance().ConvertToNSDictionary(userDictionary);
				}
				return localNotification;
				
			} else {
				return null;
			}
		}

		public override void PresentLocalNotificationNow (NotificationData notification)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				if(notification!=null) {
					UILocalNotification localNotification = this.PrepareLocalNotification(notification);
					UIApplication.SharedApplication.PresentLocalNotificationNow(localNotification);
				} else {
					SystemLogger.Log(SystemLogger.Module.PLATFORM,"No suitable data object received for presenting local notification");
				}
			});
		}

		/// <summary>
		/// Gets the current scheduled local notifications. SHOULD BE USED (INVOKED) INSIDE AN UIApplication.InvokeOnMainThread block.
		/// </summary>
		/// <returns>The current scheduled local notifications.</returns>
		private int GetCurrentScheduledLocalNotifications() {
			UILocalNotification[] localNotifications = UIApplication.SharedApplication.ScheduledLocalNotifications;
			if (localNotifications == null) {
				return 0;
			}
			return localNotifications.Length;
		}

		public override void ScheduleLocalNotification (NotificationData notification, SchedulingData schedule)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				if(notification!=null) {
					UILocalNotification localNotification = this.PrepareLocalNotification(notification);
					if(schedule != null) {
						localNotification.FireDate = IPhoneUtils.DateTimeToNSDate(DateTime.SpecifyKind(schedule.FireDate, DateTimeKind.Local));
						SystemLogger.Log(SystemLogger.Module.PLATFORM,"Scheduling local notification at " 
						                 + schedule.FireDate.ToLongTimeString() + ", with a repeat interval of: " + schedule.RepeatInterval);
						NSCalendarUnit repeatInterval = 0; // The default value is 0, which means don't repeat.
						if(schedule.RepeatInterval.Equals(RepeatInterval.HOURLY)) {
							repeatInterval = NSCalendarUnit.Hour;
						} else if(schedule.RepeatInterval.Equals(RepeatInterval.DAILY)) {
							repeatInterval = NSCalendarUnit.Day;
						} else if(schedule.RepeatInterval.Equals(RepeatInterval.WEEKLY)) {
							repeatInterval = NSCalendarUnit.Week;
						} else if(schedule.RepeatInterval.Equals(RepeatInterval.MONTHLY)) {
							repeatInterval = NSCalendarUnit.Month;
						} else if(schedule.RepeatInterval.Equals(RepeatInterval.YEARLY)) {
							repeatInterval = NSCalendarUnit.Year;
						} 
						localNotification.RepeatInterval = repeatInterval;
						UIApplication.SharedApplication.ScheduleLocalNotification(localNotification);
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Local Notification scheduled successfully [" + localNotification.FireDate.ToString() +"]");
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Current scheduled #num of local notifications: " + this.GetCurrentScheduledLocalNotifications());
					} else {
						SystemLogger.Log(SystemLogger.Module.PLATFORM,"No suitable scheduling data object received for scheduling this local notification");
					}
				} else {
					SystemLogger.Log(SystemLogger.Module.PLATFORM,"No suitable data object received for presenting local notification");
				}
			});
		}

		public override void CancelLocalNotification (DateTime fireDate)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				int numScheduledLocalNotifications = this.GetCurrentScheduledLocalNotifications();
				if(numScheduledLocalNotifications<=0) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM,"No scheduled local notifications found. It is not possible to cancel the one requested");
				} else {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "(1) Current scheduled #num of local notifications: " + numScheduledLocalNotifications);
					NSDate fireDateNS = IPhoneUtils.DateTimeToNSDate(DateTime.SpecifyKind(fireDate, DateTimeKind.Local));
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "Checking local notification to be cancelled, scheduled at " + fireDateNS.ToString());
					foreach(UILocalNotification notification in UIApplication.SharedApplication.ScheduledLocalNotifications) {
						if(notification.FireDate.SecondsSinceReferenceDate == fireDateNS.SecondsSinceReferenceDate) {
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "Cancelling notification scheduled at: " + notification.FireDate.ToString());
							UIApplication.SharedApplication.CancelLocalNotification(notification);
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "Cancelled");
						}
					}
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "(2) Current scheduled #num of local notifications: " + this.GetCurrentScheduledLocalNotifications());
				}
			});

		}

		public override void CancelAllLocalNotifications ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				UILocalNotification[] localNotifications = UIApplication.SharedApplication.ScheduledLocalNotifications;
				if(localNotifications==null || localNotifications.Length<=0) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM,"No scheduled notifications found to cancel");
				} else {
					foreach(UILocalNotification notification in localNotifications) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM,
							"Cancelling local notification scheduled at: " + notification.FireDate.ToString() 
						                 + ", with a repeat interval of: " + notification.RepeatInterval);
					}
				}

				UIApplication.SharedApplication.CancelAllLocalNotifications();
			});
		}

	}
}
