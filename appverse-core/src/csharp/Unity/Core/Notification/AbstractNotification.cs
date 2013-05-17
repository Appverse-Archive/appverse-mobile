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
using System.Collections.Generic;
using System.Text;

namespace Unity.Core.Notification
{
	public abstract class AbstractNotification : INotification
	{
		protected bool PLAYING_BEEP = false;
		protected int BEEP_FREQUENCY = 2000;  // in millisecons (a beep every 2 seconds)
		
		protected bool PLAYING_VIBRATION = false;
		protected int VIBRATION_FREQUENCY = 2000;  // in millisecons (a beep every 2 seconds)
		
		private static string DEFAULT_LOADING_TEXT = "Loading...";
        
        #region Miembros de INotification

		public abstract bool StartNotifyActivity ();

		public abstract bool StopNotifyActivity ();

		public abstract bool IsNotifyActivityRunning ();

		public abstract bool StartNotifyBeep ();

		public abstract bool StopNotifyBeep ();
        
		public abstract bool StartNotifyVibrate ();

		public abstract bool StopNotifyVibrate ();

		public bool StartNotifyLoading ()
		{
			return StartNotifyLoading (DEFAULT_LOADING_TEXT);
		}
		
		public abstract bool StartNotifyLoading (string loadingText);

		public abstract void UpdateNotifyLoading (float progress);

		public abstract bool StopNotifyLoading ();

		public abstract bool IsNotifyLoadingRunning ();

		public abstract bool StartNotifyAlert (string message);
		
		public abstract bool StartNotifyAlert (string title, string message, string buttonText);
		
		public abstract bool StartNotifyActionSheet (string title, string[] buttons, string[] javascriptCallBackFunctions);
		
		public abstract bool StopNotifyAlert ();

		public abstract bool StartNotifyBlink ();

		public abstract bool StopNotifyBlink ();
		
		public abstract void RegisterForRemoteNotifications (string senderId, RemoteNotificationType[] types);

		public abstract void UnRegisterForRemoteNotifications ();

		public abstract void SetApplicationIconBadgeNumber (int badge);

		public abstract void IncrementApplicationIconBadgeNumber ();

		public abstract void DecrementApplicationIconBadgeNumber ();

        #endregion
	}
}
