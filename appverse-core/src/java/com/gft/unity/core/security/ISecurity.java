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
package com.gft.unity.core.security;

public interface ISecurity {

    /**
     * Checks if the device has been modified.
     *
     * @return TRUE if the device is modified, otherwise FALSE
     */
    public boolean IsDeviceModified();
    
    /**
     * Adds or updates  - if already exists - a key/value pair
     *
     * @param keypair KeyPair object to store
     */
    public void StoreKeyValuePair(KeyPair keypair);

    
    /**
     *  Adds or updates  - if already exists - a given list of key/value pairs
     *
     * @param keypairs Array of KeyPair objects to store
     */
    public void StoreKeyValuePairs(KeyPair[] keypairs);

    
    /**
     * Returns a previously stored key/value pair. Null if not found
     *
     * @param keyName A string with the Key to retrieve
     */
    public void GetStoredKeyValuePair(String keyName);

    /**
     * Returns a list of previously stored key/value pair. Null if not found
     *
     * @param keyNames Array of string containing the Keys to retrieve
     */
    public void GetStoredKeyValuePairs(String[] keyNames);

    /**
     * Removes - if already exists - a given key/value pair
     *
     * @param keyName A string with the Key to remove
     */
    public void RemoveStoredKeyValuePair(String keyName);

    /**
     * Removes - if already exists - a given list of key/value pairs
     *
     * @param keyNames Array of string containing the Keys to remove
     */
    public void RemoveStoredKeyValuePairs(String[] keyNames);
}
