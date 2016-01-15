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
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Unity.Core.Messaging;
using UnityPlatformWindowsPhone.Internals;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhoneMessaging : AbstractMessaging, IAppverseService
    {
        public WindowsPhoneMessaging()
        {
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
        }

        public override async Task<bool> SendMessageSMS(string phoneNumber, string text)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(phoneNumber) || String.IsNullOrWhiteSpace(text)) return false;
                var bNumberIsValid = phoneNumber.All(character => char.IsDigit(character) || character.Equals('+') || character.Equals('+') || character.Equals('#'));
                if (!bNumberIsValid) return false;
                var sms = new ChatMessage { Body = text };
                sms.Recipients.Add(phoneNumber);
                if (AppverseBridge.Instance.RuntimeHandler.Webview.Dispatcher.HasThreadAccess) await ChatMessageManager.ShowComposeSmsMessageAsync(sms);
                else await AppverseBridge.Instance.RuntimeHandler.Webview.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () => await ChatMessageManager.ShowComposeSmsMessageAsync(sms));

                return true;
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log("Error while sending SMS: " + ex.Message);
                return false;
            }

        }

        public override async Task<bool> SendMessageMMS(string phoneNumber, string text, AttachmentData attachment)
        {

            //Cant send MMS programatically
            throw new NotImplementedException();
        }

        public override async Task<bool> SendEmail(EmailData emailData)
        {
            try
            {
                var mail = new EmailMessage
                {
                    Body = String.IsNullOrWhiteSpace(emailData.MessageBody) ? "Body Message" : emailData.MessageBody,
                    Subject = String.IsNullOrWhiteSpace(emailData.Subject) ? "Subject" : emailData.MessageBody
                };
                //var EmailTasks = 
                var emailTasks = new List<Task>
                {
                    Task.Run(() => { AddEmailRecipientsToList(emailData.ToRecipients, mail.To); }),
                    Task.Run(() => { AddEmailRecipientsToList(emailData.CcRecipients, mail.CC); }),
                    Task.Run(() => { AddEmailRecipientsToList(emailData.BccRecipients, mail.Bcc); })
                };
                await Task.WhenAll(emailTasks.ToArray());
                Parallel.ForEach(emailData.Attachment, async attachment =>
                {
                    var bReferenceUrlIsOk = false;
                    if (!String.IsNullOrWhiteSpace(attachment.ReferenceUrl))
                    {

                        try
                        {
                            mail.Attachments.Add(new EmailAttachment(attachment.FileName, RandomAccessStreamReference.CreateFromFile(await WindowsPhoneUtils.GetFileFromReferenceURL(attachment.ReferenceUrl))));
                            bReferenceUrlIsOk = true;
                        }
                        catch (Exception e)
                        {
                            WindowsPhoneUtils.Log(String.Concat("Error creating email attachment from Reference URL: ", e.Message));
                        }
                    }
                    if (bReferenceUrlIsOk) return;
                    var attachmentTempFile =
                        await
                            ApplicationData.Current.TemporaryFolder.CreateFileAsync(attachment.FileName,
                                CreationCollisionOption.GenerateUniqueName);
                    mail = await CreateAttachmentObject(attachmentTempFile, attachment, mail);
                });

                if (AppverseBridge.Instance.RuntimeHandler.Webview.Dispatcher.HasThreadAccess) await EmailManager.ShowComposeNewEmailAsync(mail);
                else await AppverseBridge.Instance.RuntimeHandler.Webview.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () => await EmailManager.ShowComposeNewEmailAsync(mail));
                return true;
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log("Error while sending Email: " + ex.Message);
                return false;
            }
        }

        #region PRIVATE METHODS

        private static void AddEmailRecipientsToList(IEnumerable<EmailAddress> sourceEmailAddresses, ICollection<EmailRecipient> emailRecipients)
        {
            var recipients = sourceEmailAddresses.Where(
                    emailAddress => emailAddress != null && !String.IsNullOrWhiteSpace(emailAddress.Address))
                    .Select(
                        emailAddress =>
                            new EmailRecipient(emailAddress.Address,
                                (!String.IsNullOrWhiteSpace(emailAddress.CommonName) ? emailAddress.CommonName : null)));
            foreach (var recipient in recipients)
            {
                emailRecipients.Add(recipient);
            }

        }

        private static async Task<EmailMessage> CreateAttachmentObject(IStorageFile attachmentTempFile, Unity.Core.AttachmentData attachment, EmailMessage mail)
        {
            await (FileIO.WriteBytesAsync(attachmentTempFile, attachment.Data).AsTask());
            var properties = await attachmentTempFile.GetBasicPropertiesAsync();
            if (properties.Size < (ulong)(attachment.DataSize))
            {
                mail.Attachments.Add(new EmailAttachment(attachment.FileName, RandomAccessStreamReference.CreateFromFile(attachmentTempFile)));
            }
            return mail;
        }

        #endregion

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }
    }
}
