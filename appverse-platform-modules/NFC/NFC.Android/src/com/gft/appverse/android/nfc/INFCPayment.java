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
package com.gft.appverse.android.nfc;

/**
 *
 * @author maps
 */
public interface INFCPayment {
    
    /**
     * Sets the application NFC parameters ad hoc 
     * This properties could be settled also in the build time through an appropriate configuration file located in /res/raw/nfcpaymentengine.properties.
     * @param properties THe NFC properties to be settled.
     */
    public void SetNFCPaymentProperties(NFCPaymentProperty[] properties);
    
    /**
     * Performs security checks (the device does not have root privileges, is not protected by lock and is not in USB debugging mode).
     * If successful, the NFCPaymentEngine starts (success or failure in this starting will be returned asynchronously via JS event listener)
     * @return NFCPaymentException An exception is returned when the security checks fail. Otherwise, null is returned.
     */
    public NFCPaymentSecurityException StartNFCPaymentEngine();
    
    /**
     * Stops the NFC payment engine.
     * This should be also called on application destroy.
     */
    public boolean StopNFCPaymentEngine();
    
    /**
     * Returns the last 4 digits of the Primary Account Number (PAN) from the SIM obfuscating the first 16.
     * @return PAN number obfuscated, null if not available
     */
    public String GetPrimaryAccountNumber();
    
    /**
     * Activates an NFC payment with the Point Of Sale (POS).
     * If the NFC is not enabled in the device, the user is redirected to the NFC settings to enable it.
     * it is required to call first the 'StartNFCPaymentEngine' method.
     * @return true of the payment has been started without any issue.
     */
    public boolean StartNFCPayment();
    
    /**
     * Checks that the device has the NFC interface active.
     * @return true if NFC enabled, false otherwise.
     */
    public boolean IsNFCEnabled();
    
    /**
    * Checks the presence/installation of the Wallet app by the given "packageName".
    * @param packageName The package name of the application that needs to be checked
    * @return true if installed on the current device, fale otherwise.
    */
    public boolean IsWalletAppInstalled(String packageName);
    
    /**
     * Launches the Settings section of the device in which the user can enable the NFC interface.
     */
    public void StartNFCSettings();
    
    /**
     * Disables any NFC payment and resets the timer.
     * If the transaction is already started at the POS side this method can not finish the payment, but only reset the timer.
     * It is required to call first the 'StartNFCPaymentEngine' method and assign the PaymentListener to receive the notifications.
     */
    public void CancelNFCPayment();
    
    
}
