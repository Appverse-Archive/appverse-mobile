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
namespace Unity.Core.Nfc
{
    public interface INFCPayment
    {

        /// <summary>
        /// Sets the application NFC parameters ad hoc
        /// This properties could be settled also through an appropriate configuration file located in /res/raw/nfcpaymentengine.properties.
        /// </summary>
        /// <param name="properties">The NFC properties to be settled</param>
        void SetNFCPaymentProperties(NFCPaymentProperty[] properties);

        /// <summary>
        /// Performs security checks (the device does not have root privileges, is not protected by lock and is not in USB debugging mode).
        /// If successful, the NFCPaymentEngine starts (success or failure in this starting will be returned asynchronously via JS event listener)
        /// </summary>
        /// <returns>NFCPaymentException An exception is returned when the security checks fail. Otherwise, null is returned</returns>
        NFCPaymentSecurityException StartNFCPaymentEngine();

        /// <summary>
        /// Stops the NFC payment engine.
        /// This should be also called on application destroy.
        /// </summary>
        /// <returns>true on successful stop</returns>
        bool StopNFCPaymentEngine();

        /// <summary>
        /// Returns the last 4 digits of the Primary Account Number (PAN) from the SIM obfuscating the first 16.
        /// </summary>
        /// <returns>PAN number obfuscated, null if not available</returns>
        string GetPrimaryAccountNumber();

        /// <summary>
        /// Activates an NFC payment with the Point Of Sale (POS).
        /// If the NFC is not enabled in the device, the user is redirected to the NFC settings to enable it.
        /// It is required to call first the 'StartNFCPaymentEngine' method.
        /// </summary>
        /// <returns>true of the payment has been started without any issue</returns>
        bool StartNFCPayment();

        /// <summary>
        /// Checks that the device has the NFC interface active
        /// </summary>
        /// <returns>true if NFC enabled, false otherwise.</returns>
        bool IsNFCEnabled();

        /// <summary>
        /// Checks the presence/installation of the Wallet app by the given "packageName".
        /// </summary>
        /// <param name="packageName">The package name of the application that needs to be checked</param>
        /// <returns>true if installed on the current device, fale otherwise</returns>
        bool IsWalletAppInstalled(string packageName);
        
        /// <summary>
        /// Launches the Settings section of the device in which the user can enable the NFC interface.
        /// </summary>
        void StartNFCSettings();

        /// <summary>
        /// Disables any NFC payment and resets the timer.
        /// If the transaction is already started at the POS side this method can not finish the payment, but only reset the timer.
        /// It is required to call first the 'StartNFCPaymentEngine' method and assign the PaymentListener to receive the notifications.
        /// </summary>
        /// <returns>true if the cancel process has been started without any issue</returns>
        bool CancelNFCPayment();

        
    }//end INFC

}//end namespace Nfc