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
namespace Unity.Core.Security
{
    public interface ISecurity
    {
#if !WP8
        /// <summary>
        /// Checks if the device has been modified.
        /// </summary>
        /// <returns>
        /// True in case of modified device, otherwise false.
        /// </returns>
        bool IsDeviceModified();

        /// <summary>
        /// Adds or updates  - if already exists - a given key/value pair
        /// </summary>
        /// <param name="keypair">KeyPair object to store</param>
        void StoreKeyValuePair(KeyPair keypair);

        /// <summary>
        /// Adds or updates  - if already exists - a given list of key/value pairs
        /// </summary>
        /// <param name="keypairs">Array of KeyPair objects to store</param>
        void StoreKeyValuePairs(KeyPair[] keypairs);

        /// <summary>
        /// Returns a previously stored key/value pair. Null if not found
        /// </summary>
        /// <param name="KeyName">A string with the Key to retrieve</param>
        void GetStoredKeyValuePair(string KeyName);

        /// <summary>
        /// Returns a list of previously stored key/value pair. Null if not found
        /// </summary>
        /// <param name="KeyNames">Array of string containing the Keys to retrieve</param>
        void GetStoredKeyValuePairs(string[] KeyNames);

        /// <summary>
        /// Removes - if already exists - a given key/value pair
        /// </summary>
        /// <param name="KeyName">A string with the Key to remove</param>
        void RemoveStoredKeyValuePair(string KeyName);

        /// <summary>
        /// Removes - if already exist - a given list of key/value pairs
        /// </summary>
        /// <param name="KeyNames">Array of string containing the Keys to remove</param>
        void RemoveStoredKeyValuePairs(string[] KeyNames);

		/// <summary>
		/// Starts local authentication operation displaying Touch ID screen.
		/// </summary>
		/// <param name="reason">A reason to explain why authentication is needed. This helps to build trust with the user.</param>
		void StartLocalAuthenticationWithTouchID (string reason);

#else
        /// <summary>
        /// Checks if the device has been modified.
        /// </summary>
        /// <returns>
        /// True in case of modified device, otherwise false.
        /// </returns>
        Task<bool> IsDeviceModified();

        /// <summary>
        /// Adds or updates  - if already exists - a given key/value pair
        /// </summary>
        /// <param name="keypair">KeyPair object to store</param>
        Task StoreKeyValuePair(KeyPair keypair);

        /// <summary>
        /// Adds or updates  - if already exists - a given list of key/value pairs
        /// </summary>
        /// <param name="keypairs">Array of KeyPair objects to store</param>
        Task StoreKeyValuePairs(KeyPair[] keypairs);

        /// <summary>
        /// Returns a previously stored key/value pair. Null if not found
        /// </summary>
        /// <param name="keyName">A string with the Key to retrieve</param>
        Task GetStoredKeyValuePair(string keyName);

        /// <summary>
        /// Returns a list of previously stored key/value pair. Null if not found
        /// </summary>
        /// <param name="keyNames">Array of string containing the Keys to retrieve</param>
        Task GetStoredKeyValuePairs(string[] keyNames);

        /// <summary>
        /// Removes - if already exists - a given key/value pair
        /// </summary>
        /// <param name="keyName">A string with the Key to remove</param>
        Task RemoveStoredKeyValuePair(string keyName);

        /// <summary>
        /// Removes - if already exist - a given list of key/value pairs
        /// </summary>
        /// <param name="keyNames">Array of string containing the Keys to remove</param>
        Task RemoveStoredKeyValuePairs(string[] keyNames);
#endif
    }//end ISecurity

}//end namespace Security