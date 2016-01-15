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
using System;
using System.Collections.Generic;
using System.Media;
using System.Text;
using System.Threading;
using Unity.Core.Notification;

namespace Unity.Platform.Windows
{
    public class WindowsNotification : AbstractNotification
    {
        

        private void PlayBeep()
        {
            while (PLAYING_BEEP)
            {
                SystemSounds.Beep.Play();
                Thread.Sleep(BEEP_FREQUENCY);
            }
        }

        public override bool StartNotifyBeep()
        {
            PLAYING_BEEP = true;

            // Starts beep on background thread
            Thread beepThread = new Thread(new ThreadStart(PlayBeep));
            beepThread.IsBackground = true;
            beepThread.Start();

            return true;
        }

        public override bool StopNotifyBeep()
        {
            PLAYING_BEEP = false;

            return true;
        }

	public override bool StartNotifyVibrate ()
	{   // function not available on windows
	    return false;
	}
	
	public override bool StopNotifyVibrate ()
	{   // function not available on windows
	    return false;
	}

	public override bool IsNotifyActivityRunning ()
		{
			throw new System.NotImplementedException();
		}
		
		public override bool IsNotifyLoadingRunning ()
		{
			throw new System.NotImplementedException();
		}
		
		public override bool StartNotifyActivity ()
		{
			throw new System.NotImplementedException();
		}
		
		public override bool StartNotifyAlert (string message)
		{
			throw new System.NotImplementedException();
		}
		
		public override bool StartNotifyBlink ()
		{
			throw new System.NotImplementedException();
		}
		
		public override bool StartNotifyLoading (string loadingText)
		{
			throw new System.NotImplementedException();
		}
		
		public override bool StopNotifyActivity ()
		{
			throw new System.NotImplementedException();
		}
		
		public override bool StopNotifyAlert ()
		{
			throw new System.NotImplementedException();
		}

        public override bool StartNotifyAlert(string title, string message, string buttonText)
        {
            throw new System.NotImplementedException();
        }
		
		public override bool StopNotifyBlink ()
		{
			throw new System.NotImplementedException();
		}
		
		public override bool StopNotifyLoading ()
		{
			throw new System.NotImplementedException();
		}
		
		public override void UpdateNotifyLoading (float progress)
		{
			throw new System.NotImplementedException();
		}

        public override bool StartNotifyActionSheet(string title, string[] buttons, string[] javascriptCallBackFunctions)
        {
            throw new NotImplementedException();
        }

        /*public override void RegisterForRemoteNotifications(string senderId, RemoteNotificationType[] types)
        {
            throw new NotImplementedException();
        }

        public override void UnRegisterForRemoteNotifications()
        {
            throw new NotImplementedException();
        }*/

        public override void DecrementApplicationIconBadgeNumber()
        {
            throw new NotImplementedException();
        }

        public override void IncrementApplicationIconBadgeNumber()
        {
            throw new NotImplementedException();
        }

        public override void SetApplicationIconBadgeNumber(int badge)
        {
            throw new NotImplementedException();
        }

        public override void CancelAllLocalNotifications()
        {
            throw new NotImplementedException();
        }

        public override void CancelLocalNotification(DateTime fireDate)
        {
            throw new NotImplementedException();
        }

        public override void PresentLocalNotificationNow(NotificationData notification)
        {
            throw new NotImplementedException();
        }

        public override void ScheduleLocalNotification(NotificationData notification, SchedulingData schedule)
        {
            throw new NotImplementedException();
        }
    }
}
