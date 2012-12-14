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
package com.gft.unity.core.messaging;

import java.util.Arrays;

public class EmailData {

    private AttachmentData[] attachment;
    private EmailAddress[] bccRecipients;
    private EmailAddress[] ccRecipients;
    private EmailAddress fromAddress;
    private String messageBody;
    private String messageBodyMimeType;
    private String subject;
    private EmailAddress[] toRecipients;

    public EmailData() {
    }

    public AttachmentData[] getAttachment() {
        return attachment;
    }

    public void setAttachment(AttachmentData[] attachment) {
        this.attachment = attachment;
    }

    public EmailAddress[] getBccRecipients() {
        return bccRecipients;
    }

    public void setBccRecipients(EmailAddress[] bccRecipients) {
        this.bccRecipients = bccRecipients;
    }

    public String[] getBccRecipientsAsString() {
        return emailAddressToStringArray(this.bccRecipients);
    }

    public EmailAddress[] getCcRecipients() {
        return ccRecipients;
    }

    public void setCcRecipients(EmailAddress[] ccRecipients) {
        this.ccRecipients = ccRecipients;
    }

    public String[] getCcRecipientsAsString() {
        return emailAddressToStringArray(this.ccRecipients);
    }

    public EmailAddress getFromAddress() {
        return fromAddress;
    }

    public void setFromAddress(EmailAddress fromAddress) {
        this.fromAddress = fromAddress;
    }

    public String getMessageBody() {
        return messageBody;
    }

    public void setMessageBody(String messageBody) {
        this.messageBody = messageBody;
    }

    public String getMessageBodyMimeType() {
        return messageBodyMimeType;
    }

    public void setMessageBodyMimeType(String messageBodyMimeType) {
        this.messageBodyMimeType = messageBodyMimeType;
    }

    public String getSubject() {
        return subject;
    }

    public void setSubject(String subject) {
        this.subject = subject;
    }

    public EmailAddress[] getToRecipients() {
        return toRecipients;
    }

    public void setToRecipients(EmailAddress[] toRecipients) {
        this.toRecipients = toRecipients;
    }

    public String[] getToRecipientsAsString() {
        return emailAddressToStringArray(this.toRecipients);
    }

    private static String[] emailAddressToStringArray(EmailAddress[] addressees) {
        String[] addresses = null;
        EmailAddress[] emailAddresses = addressees;

        if (emailAddresses != null && emailAddresses.length > 0) {
            addresses = new String[emailAddresses.length];
            for (int i = 0; i < emailAddresses.length; i++) {
                if (emailAddresses[i].getCommonName() != null) {
                    // "standard" address format
                    addresses[i] = "\"" + emailAddresses[i].getCommonName()
                            + "\" <" + emailAddresses[i].getAddress() + ">";
                } else {
                    addresses[i] = emailAddresses[i].getAddress();
                }
            }
        } else {
            addresses = new String[0];
        }

        return addresses;
    }

    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("EmailData [attachment=");
        builder.append(Arrays.toString(attachment));
        builder.append(", bccRecipients=");
        builder.append(Arrays.toString(bccRecipients));
        builder.append(", ccRecipients=");
        builder.append(Arrays.toString(ccRecipients));
        builder.append(", fromAddress=");
        builder.append(fromAddress);
        builder.append(", messageBody=");
        builder.append(messageBody);
        builder.append(", messageBodyMimeType=");
        builder.append(messageBodyMimeType);
        builder.append(", subject=");
        builder.append(subject);
        builder.append(", toRecipients=");
        builder.append(Arrays.toString(toRecipients));
        builder.append("]");
        return builder.toString();
    }
}
