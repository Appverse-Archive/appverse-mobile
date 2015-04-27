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
namespace Unity.Core.Messaging
{
    public interface IMessaging
    {
#if !WP8
        /// <summary>
        /// Send a text message (SMS).
        /// </summary>
        /// <param name="phoneNumber">The address to send the message to.</param>
        /// <param name="text">The message body.</param>
        bool SendMessageSMS(string phoneNumber, string text);

        /// <summary>
        /// Send a media message (MMS).
        /// </summary>
        /// <param name="phoneNumber">The address to send the message to.</param>
        /// <param name="text">The message body.</param>
        /// <param name="attachment">Attachament data.</param>
        bool SendMessageMMS(string phoneNumber, string text, AttachmentData attachment);

        /// <summary>
        /// Send an email message.
        /// </summary>
        /// <param name="emailData">Email message data.</param>
        bool SendEmail(EmailData emailData);
#else
        /// <summary>
        /// Send a text message (SMS).
        /// </summary>
        /// <param name="phoneNumber">The address to send the message to.</param>
        /// <param name="text">The message body.</param>
        Task<bool> SendMessageSMS(string phoneNumber, string text);

        /// <summary>
        /// Send a media message (MMS).
        /// </summary>
        /// <param name="phoneNumber">The address to send the message to.</param>
        /// <param name="text">The message body.</param>
        /// <param name="attachment">Attachament data.</param>
        Task<bool> SendMessageMMS(string phoneNumber, string text, AttachmentData attachment);

        /// <summary>
        /// Send an email message.
        /// </summary>
        /// <param name="emailData">Email message data.</param>
        Task<bool> SendEmail(EmailData emailData);
#endif

    }//end IMessaging

}//end namespace Messaging