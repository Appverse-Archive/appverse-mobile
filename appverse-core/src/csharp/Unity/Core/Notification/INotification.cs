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
#if WP8
using System.Threading.Tasks;
#endif
using System;

namespace Unity.Core.Notification
{
    public interface INotification
    {
#if !WP8
        /// <summary>
        /// Shows and starts the activity indicator animation.
        /// </summary>
        /// <returns>True if activity could be started.</returns>
        bool StartNotifyActivity();

        /// <summary>
        /// Stops and hides the activity indicator animation.
        /// </summary>
        /// <returns>True if activity could be stopped.</returns>
        bool StopNotifyActivity();

        /// <summary>
        /// Checks if activity indicator animation is started.
        /// </summary>
        /// <returns>True/false wheter activity indicator is running.</returns>
        bool IsNotifyActivityRunning();

        /// <summary>
        /// Starts a beep notification.
        /// </summary>
        /// <returns>True if beep notification could be started.</returns>
        bool StartNotifyBeep();

        /// <summary>
        /// Stops a beep notification.
        /// </summary>
        /// <returns>True if beep notification could be stopped.</returns>
        bool StopNotifyBeep();

        /// <summary>
        /// Starts a vibrate notification.
        /// </summary>
        /// <returns>True if vibrate notification could be started.</returns>
        bool StartNotifyVibrate();

        /// <summary>
        /// Stops a vibrate notification.
        /// </summary>
        /// <returns>True if vibrate notification could be stopped.</returns>
        bool StopNotifyVibrate();

        /// <summary>
        /// Shows and starts the progress indicator animation.
        /// </summary>
        /// <returns>True if loading indicator could be started.</returns>
        bool StartNotifyLoading();

        /// <summary>
        /// Shows and starts the progress indicator animation.
        /// </summary>
        /// <param name="loadingText">The message to be displayed on the loading view.</param>
        /// <returns>True if loading indicator could be started.</returns>
        bool StartNotifyLoading(string loadingText);

        /// <summary>
        /// Updates the progress indicator animation.
        /// </summary>
        /// <param name="progress">The current progress; values between 0.0 and 1.0 (completed).</param>
        void UpdateNotifyLoading(float progress);

        /// <summary>
        /// Stops and hides the progress indicator animation.
        /// </summary>
        /// <returns>True if loading indicator could be stopped.</returns>
        bool StopNotifyLoading();

        /// <summary>
        /// Checks if progress indicator animation is started.
        /// </summary>
        /// <returns>True/false wheter progress indicator is running.</returns>
        bool IsNotifyLoadingRunning();

        /// <summary>
        /// Starts an alert notification.
        /// </summary>
        /// <param name="message">The alert message to be displayed.</param>
        /// <returns>True if alert notification could be started.</returns>
        bool StartNotifyAlert(string message);

        bool StartNotifyAlert(string title, string message, string buttonText);
        /// TODO: button toolbar for alert types, confirm (accept/cancel) ????). Click Handlers ???

        bool StartNotifyActionSheet(string title, string[] buttons, string[] javascriptCallBackFunctions);

        /// <summary>
        /// Stops an alert notification.
        /// </summary>
        /// <returns>True if alert notification could be stopped.</returns>
        bool StopNotifyAlert();

        /// <summary>
        /// Starts a blink notification.
        /// </summary>
        /// <returns>True if blink notification could be started.</returns>
        bool StartNotifyBlink();

        /// <summary>
        /// Stops a blink notification.
        /// </summary>
        /// <returns>True if blink notification could be stooped.</returns>
        bool StopNotifyBlink();

        /// <summary>
        /// Sets the application icon badge number.
        /// </summary>
        /// <param name="badge">Badge to be set.</param>
        void SetApplicationIconBadgeNumber(int badge);

        /// <summary>
        /// Increments (add one to) the application icon badge number.
        /// </summary>
        void IncrementApplicationIconBadgeNumber();

        /// <summary>
        /// Decrements (substract one from) the application icon badge number.
        /// </summary>
        void DecrementApplicationIconBadgeNumber();

        /// <summary>
        /// Presents a local notification immediately for the current application.
        /// </summary>
        /// <param name="notification">Notification to be presented.</param>
        void PresentLocalNotificationNow(NotificationData notification);

        /// <summary>
        /// Schedules a local notification fo delivery on a scheduled date and time.
        /// </summary>
        /// <param name="notification">Notification data to be presented.</param>
        /// <param name="schedule">Scheduling data with the fireDate.</param>
        void ScheduleLocalNotification(NotificationData notification, SchedulingData schedule);

        /// <summary>
        /// Cancels a local notification given its fire date identifier.
        /// </summary>
        /// <param name="fireDate">The fire date of the local notification to be cancelled
        /// (the fire date is the unique identifier of a local notification, only 1 notification could be scheduled for the same fire date... last scheduled wins)
        void CancelLocalNotification(DateTime fireDate);

        /// <summary>
        /// Cancels any local notification scheduled.
        /// </summary>
        void CancelAllLocalNotifications();
#else
        /// <summary>
        /// Shows and starts the activity indicator animation.
        /// </summary>
        /// <returns>True if activity could be started.</returns>
        Task<bool> StartNotifyActivity();

        /// <summary>
        /// Stops and hides the activity indicator animation.
        /// </summary>
        /// <returns>True if activity could be stopped.</returns>
        Task<bool> StopNotifyActivity();

        /// <summary>
        /// Checks if activity indicator animation is started.
        /// </summary>
        /// <returns>True/false wheter activity indicator is running.</returns>
        Task<bool> IsNotifyActivityRunning();

        /// <summary>
        /// Starts a beep notification.
        /// </summary>
        /// <returns>True if beep notification could be started.</returns>
        Task<bool> StartNotifyBeep();

        /// <summary>
        /// Stops a beep notification.
        /// </summary>
        /// <returns>True if beep notification could be stopped.</returns>
        Task<bool> StopNotifyBeep();

        /// <summary>
        /// Starts a vibrate notification.
        /// </summary>
        /// <returns>True if vibrate notification could be started.</returns>
        Task<bool> StartNotifyVibrate();

        /// <summary>
        /// Stops a vibrate notification.
        /// </summary>
        /// <returns>True if vibrate notification could be stopped.</returns>
        Task<bool> StopNotifyVibrate();

        /// <summary>
        /// Shows and starts the progress indicator animation.
        /// </summary>
        /// <returns>True if loading indicator could be started.</returns>
        Task<bool> StartNotifyLoading();

        /// <summary>
        /// Shows and starts the progress indicator animation.
        /// </summary>
        /// <param name="loadingText">The message to be displayed on the loading view.</param>
        /// <returns>True if loading indicator could be started.</returns>
        Task<bool> StartNotifyLoading(string loadingText);

        /// <summary>
        /// Updates the progress indicator animation.
        /// </summary>
        /// <param name="progress">The current progress; values between 0.0 and 1.0 (completed).</param>
        Task UpdateNotifyLoading(float progress);

        /// <summary>
        /// Stops and hides the progress indicator animation.
        /// </summary>
        /// <returns>True if loading indicator could be stopped.</returns>
        Task<bool> StopNotifyLoading();

        /// <summary>
        /// Checks if progress indicator animation is started.
        /// </summary>
        /// <returns>True/false wheter progress indicator is running.</returns>
        Task<bool> IsNotifyLoadingRunning();

        /// <summary>
        /// Starts an alert notification.
        /// </summary>
        /// <param name="message">The alert message to be displayed.</param>
        /// <returns>True if alert notification could be started.</returns>
        Task<bool> StartNotifyAlert(string message);

        Task<bool> StartNotifyAlert(string title, string message, string buttonText);
        /// TODO: button toolbar for alert types, confirm (accept/cancel) ????). Click Handlers ???

        Task<bool> StartNotifyActionSheet(string title, string[] buttons, string[] javascriptCallBackFunctions);

        /// <summary>
        /// Stops an alert notification.
        /// </summary>
        /// <returns>True if alert notification could be stopped.</returns>
        Task<bool> StopNotifyAlert();

        /// <summary>
        /// Starts a blink notification.
        /// </summary>
        /// <returns>True if blink notification could be started.</returns>
        Task<bool> StartNotifyBlink();

        /// <summary>
        /// Stops a blink notification.
        /// </summary>
        /// <returns>True if blink notification could be stooped.</returns>
        Task<bool> StopNotifyBlink();

        /// <summary>
        /// Sets the application icon badge number.
        /// </summary>
        /// <param name="badge">Badge to be set.</param>
        Task SetApplicationIconBadgeNumber(int badge);

        /// <summary>
        /// Increments (add one to) the application icon badge number.
        /// </summary>
        Task IncrementApplicationIconBadgeNumber();

        /// <summary>
        /// Decrements (substract one from) the application icon badge number.
        /// </summary>
        Task DecrementApplicationIconBadgeNumber();

        /// <summary>
        /// Presents a local notification immediately for the current application.
        /// </summary>
        /// <param name="notification">Notification to be presented.</param>
        Task PresentLocalNotificationNow(NotificationData notification);

        /// <summary>
        /// Schedules a local notification fo delivery on a scheduled date and time.
        /// </summary>
        /// <param name="notification">Notification data to be presented.</param>
        /// <param name="schedule">Scheduling data with the fireDate.</param>
        Task ScheduleLocalNotification(NotificationData notification, SchedulingData schedule);

        /// <summary>
        /// Cancels a local notification given its fire date identifier.
        /// </summary>
        /// <param name="fireDate">The fire date of the local notification to be cancelled
        /// (the fire date is the unique identifier of a local notification, only 1 notification could be scheduled for the same fire date... last scheduled wins)
        Task CancelLocalNotification(DateTime fireDate);

        /// <summary>
        /// Cancels any local notification scheduled.
        /// </summary>
        Task CancelAllLocalNotifications();
#endif
    }//end INotification

}//end namespace Notification