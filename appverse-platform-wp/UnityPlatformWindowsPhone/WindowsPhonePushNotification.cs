/*
 Copyright (c) 2015 GFT Appverse, S.L., Sociedad Unipersonal.

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
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;
using Appverse.Core.PushNotifications;
using Newtonsoft.Json;
using NotificationsExtensions.ToastContent;
using Unity.Core.Notification;
using Unity.Core.PushNotifications;
using UnityPlatformWindowsPhone.Internals;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhonePushNotification : AbstractPushNotification, IAppverseService
    {
        private PushNotificationChannel _pushNotificationChannel;

        public WindowsPhonePushNotification()
        {
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
            //Clear any badge and Action Center toast notification
            ToastNotificationManager.History.Clear();
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
        }

        private void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            NotificationData notificationData = null;
            try
            {
                //Decide what to do when a notification arrives while the app is running
                switch (args.NotificationType)
                {
                    case PushNotificationType.Toast:
                        var toastNotification = JsonConvert.DeserializeObject<IToastText02>(args.ToastNotification.Content.GetXml());

                        notificationData = new NotificationData
                        {
                            AlertMessage =
                                String.Concat(toastNotification.TextHeading.Text, " ", toastNotification.TextBodyWrap.Text),
                            CustomDataJsonString = toastNotification.Launch,
                            AppWasRunning = true
                        };
                        break;
                    case PushNotificationType.Tile:
                        break;
                    case PushNotificationType.Badge:
                        break;
                    case PushNotificationType.Raw:
                        break;
                }
            }
            catch (Exception e)
            {
                WindowsPhoneUtils.Log(String.Concat("Error on PushNotification received: ", e.Message));
            }

            WindowsPhoneUtils.InvokeCallback("Appverse.PushNotifications.OnRemoteNotificationReceived", WindowsPhoneUtils.CALLBACKID, JsonConvert.SerializeObject(notificationData));
            //Show Push notification even when app is in foreground
            args.Cancel = false;
        }

        public async override Task RegisterForRemoteNotifications(string senderId, RemoteNotificationType[] types)
        {
            try
            {

                _pushNotificationChannel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

                _pushNotificationChannel.PushNotificationReceived += OnPushNotificationReceived;

                var registrationToken = new RegistrationToken
                {
                    Binary = Encoding.UTF8.GetBytes(_pushNotificationChannel.Uri),
                    StringRepresentation = _pushNotificationChannel.Uri
                };
                WindowsPhoneUtils.InvokeCallback("Appverse.PushNotifications.OnRegisterForRemoteNotificationsSuccess", WindowsPhoneUtils.CALLBACKID, JsonConvert.SerializeObject(registrationToken));
            }
            catch (Exception ex)
            {

                var registrationError = new RegistrationError
                {
                    Code = ex.HResult.ToString(),
                    LocalizedDescription = ex.Message
                };
                WindowsPhoneUtils.InvokeCallback("Appverse.PushNotifications.OnRegisterForRemoteNotificationsFailure", WindowsPhoneUtils.CALLBACKID, JsonConvert.SerializeObject(registrationError));
            }
        }

        public async override Task UnRegisterForRemoteNotifications()
        {
            if (_pushNotificationChannel == null) return;
            _pushNotificationChannel.Close();
            WindowsPhoneUtils.InvokeCallback("Appverse.PushNotifications.OnUnRegisterForRemoteNotificationsSuccess", WindowsPhoneUtils.CALLBACKID, null);
        }

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }
    }
}
