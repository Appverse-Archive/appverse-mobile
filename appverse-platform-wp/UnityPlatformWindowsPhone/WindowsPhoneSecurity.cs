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
using Windows.Storage;
using Newtonsoft.Json;
using Unity.Core.Security;
using UnityPlatformWindowsPhone.Internals;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhoneSecurity : AbstractSecurity, IAppverseService
    {
        private const string DefaultKeychainName = "General";
        private const string RemoveKeyValueCallback = "Appverse.Security.OnKeyValuePairsRemoveCompleted";
        private const string StoreKeyValueCallback = "Appverse.Security.OnKeyValuePairsStoreCompleted";
        private const string GetKeyValueCallback = "Appverse.Security.OnKeyValuePairsFound";

        public WindowsPhoneSecurity()
        {
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
        }

        public override async Task<bool> IsDeviceModified()
        {
            //No way to programmatically find out if the device has developer unlock applied or not 
            return false;
        }

        public override async Task StoreKeyValuePair(KeyPair keypair)
        {
            StoreKeyValuePairs(new[] { keypair });
        }

        public override async Task StoreKeyValuePairs(KeyPair[] keypairs)
        {
            var keychain = GetKeychainContainer(DefaultKeychainName, true);
            var successfullKeyPairs = new List<KeyPair>();
            var failedKeyPairs = new List<KeyPair>();
            if (keychain != null)
            {
                foreach (var entry in keypairs)
                {
                    try
                    {
                        if (keychain.Values.ContainsKey(entry.Key))
                        {
                            keychain.Values.Remove(entry.Key);
                            keychain.Values.Add(entry.Key, entry.Value);
                            successfullKeyPairs.Add(entry);
                        }
                        else
                        {
                            keychain.Values.Add(entry.Key, entry.Value);
                            successfullKeyPairs.Add(entry);
                        }
                    }
                    catch (Exception)
                    {
                        failedKeyPairs.Add(entry);
                    }
                }
            }
            var jsonResultString = JsonConvert.SerializeObject(new object[] { successfullKeyPairs, failedKeyPairs });
            WindowsPhoneUtils.Log("StoreKeyValuePair: " + jsonResultString);
            WindowsPhoneUtils.InvokeCallback(StoreKeyValueCallback, WindowsPhoneUtils.CALLBACKID, jsonResultString);
        }

        public override async Task GetStoredKeyValuePair(KeyPair keyName)
        {
            GetStoredKeyValuePairs(new[] { keyName });
        }

        public override async Task GetStoredKeyValuePairs(KeyPair[] keyNames)
        {
            var keychain = GetKeychainContainer(DefaultKeychainName, false);
            var foundKeyPairs = new List<KeyPair>();
            if (keychain != null)
            {
                foundKeyPairs.AddRange(from kp in keyNames where keychain.Values.ContainsKey(kp.Key) select new KeyPair { Key = kp.Key, Value = keychain.Values[kp.Key].ToString() });
            }
            var jsonResultString = JsonConvert.SerializeObject(foundKeyPairs.ToArray());
            WindowsPhoneUtils.Log("GetStoredKeyValuePairs: " + jsonResultString);
            await WindowsPhoneUtils.InvokeCallback(GetKeyValueCallback, WindowsPhoneUtils.CALLBACKID, jsonResultString);
        }

        public override async Task RemoveStoredKeyValuePair(string keyName)
        {
            RemoveStoredKeyValuePairs(new[] { keyName });
        }

        public override async Task RemoveStoredKeyValuePairs(string[] keyNames)
        {
            var keychain = GetKeychainContainer(DefaultKeychainName, false);
            var successfullKeyPairs = new List<string>();
            var failedKeyPairs = new List<string>();
            if (keychain != null)
            {
                foreach (var key in keyNames)
                {
                    try
                    {
                        if (keychain.Values.ContainsKey(key))
                        {
                            keychain.Values.Remove(key);
                            successfullKeyPairs.Add(key);
                        }
                        else
                            failedKeyPairs.Add(key);
                    }
                    catch (Exception)
                    {
                        failedKeyPairs.Add(key);
                    }
                }
            }
            var jsonResultString = JsonConvert.SerializeObject(new object[] { successfullKeyPairs, failedKeyPairs });
            WindowsPhoneUtils.Log("RemoveKeyValuePair: " + jsonResultString);
            await WindowsPhoneUtils.InvokeCallback(RemoveKeyValueCallback, WindowsPhoneUtils.CALLBACKID, jsonResultString);
        }

        private static ApplicationDataContainer GetKeychainContainer(string sKeychainName, bool bCreateIfNotExists)
        {
            if (bCreateIfNotExists)
                return ApplicationData.Current.LocalSettings.Containers.ContainsKey(sKeychainName) ? ApplicationData.Current.LocalSettings.Containers[sKeychainName] : ApplicationData.Current.LocalSettings.CreateContainer(sKeychainName, ApplicationDataCreateDisposition.Always);
            return ApplicationData.Current.LocalSettings.Containers.ContainsKey(sKeychainName) ? ApplicationData.Current.LocalSettings.Containers[sKeychainName] : null;
        }

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }
    }
}
