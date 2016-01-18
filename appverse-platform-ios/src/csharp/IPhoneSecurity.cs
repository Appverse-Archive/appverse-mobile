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
using System;
using System.IO;
using System.Collections.Specialized;
using Unity.Core.Security;
using Security;
using Foundation;
using Unity.Core.System;
using System.Collections.Generic;
using UIKit;
using LocalAuthentication;
using System.Xml.Serialization;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Security.Cryptography;


namespace Unity.Platform.IPhone
{
	public class IPhoneSecurity:AbstractSecurity
	{
		/// <summary>
		/// Paths that usually are found in modified devices
		/// </summary>
		private StringCollection scPathsToCheck = null;
		private StringCollection scPermissionPaths = null;

		private const string KEYCHAIN_ACCESS_GROUP = "$KEYCHAIN_ACCESS_GROUP$";


		private string KeyChainAccessGroup {
			get{ 
				if(KEYCHAIN_ACCESS_GROUP == null || 
				   KEYCHAIN_ACCESS_GROUP.Trim().Equals("") || 
				   KEYCHAIN_ACCESS_GROUP.Trim().Length==0 ||
				   KEYCHAIN_ACCESS_GROUP.Trim().ToLower().Equals("undefined"))
				{
					return null;
				}
				else return KEYCHAIN_ACCESS_GROUP;
			}
		}

		public IPhoneSecurity () : base()
		{
			scPathsToCheck = new StringCollection();
			scPathsToCheck.Add("/private/var/stash");
			scPathsToCheck.Add("/Applications/Cydia.app");
			scPathsToCheck.Add("/Applications/Snoop-it Config.app");
			scPermissionPaths = new StringCollection();
			scPermissionPaths.Add("/System/");
			scPermissionPaths.Add("/private/");
		}

		/// <summary>
		/// Method overrided, to use NSData to get stream from file resource inside application. 
		/// </summary>
		/// <returns>
		/// A <see cref="Stream"/>
		/// </returns>
		public override byte[] GetConfigFileBinaryData ()
		{
			return IPhoneUtils.GetInstance().GetResourceAsBinary(this.SecurityConfigFile, true);
		}


		public override string SecurityConfigFile { 
			get {
				return IPhoneUtils.GetInstance().GetFileFullPath(base.SecurityConfigFile);
			} 
		}

		/// <summary>
		/// Checks if the device has been modified
		/// </summary>
		/// <returns>
		/// TRUE if the device is modified, otherwise FALSE
		/// </returns>
		public override bool IsDeviceModified ()
		{

			bool bDeviceIsModified = false;
			try{
			foreach (String sPath in scPathsToCheck) {
				bDeviceIsModified = CheckPath (sPath);
				if (bDeviceIsModified) return true;
			}

			foreach (string sPath in scPermissionPaths) {
				bDeviceIsModified = CheckPathAccess(sPath);
				if(bDeviceIsModified) return true;
			}
			}catch(Exception ex){
				//Nothing to show in order to avoid showing information about the security measures
			}
			return bDeviceIsModified;
		}

		/// <summary>
		/// Checks the existence of the specified paths.
		/// </summary>
		/// <returns>
		/// TRUE if any specified path exists, otherwise FALSE
		/// </returns>
		/// <param name='scPaths'>
		/// String with the path to be checked
		/// </param>
		private bool CheckPath (string sPath)
		{
			return Directory.Exists(sPath);
		}


		/// <summary>
		/// Checks if we have full access over a directory
		/// </summary>
		/// <returns>
		/// TRUE if we have full access over any of the specified directories
		/// </returns>
		/// <param name='scPaths'>
		/// Directory paths to be checked
		/// </param>
		private bool CheckPathAccess (string sPath)
		{
			//If the folder is READONLY an exception will be thrown, otherwise we have access over the folder.
			try{
				if(CheckPath(sPath)){
					DirectoryInfo dirInfo = new DirectoryInfo(sPath);
					dirInfo.GetAccessControl(System.Security.AccessControl.AccessControlSections.All);
				}else{return false;}
			}catch(UnauthorizedAccessException ex){
				return false;
			}
			return true;
		}

		#region implemented abstract members of AbstractSecurity

		public override void StoreKeyValuePair (KeyPair keypair)
		{
			StoreKeyValuePairs (new KeyPair[] { keypair });
		}

		public override void StoreKeyValuePairs (KeyPair[] keypairs)
		{
			string sAccessGroup = KeyChainAccessGroup;
			List<KeyPair> successfullKeyPairs = new List<KeyPair>();
			List<KeyPair> failedKeyPairs = new List<KeyPair>();
			foreach (KeyPair kp in keypairs) {
				//SystemLogger.Log (SystemLogger.Module.PLATFORM, "Storing: " + kp.Value);
				if (kp.Encryption) {
					kp.Value = Encrypt (kp.Value);
					//SystemLogger.Log (SystemLogger.Module.PLATFORM, "Encrypted: " + kp.Value);
				}
				SecRecord srNewEntry = new SecRecord (SecKind.GenericPassword) {
					Account = kp.Key,
					Generic = NSData.FromString (kp.Key),
					ValueData = NSData.FromString (kp.Value)
				};

				if (sAccessGroup != null)
					srNewEntry.AccessGroup = sAccessGroup;
			
				if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
					if (this.GetPasscodeProtectedKeys ().Contains (kp.Key)) {
						SystemLogger.Log (SystemLogger.Module.PLATFORM, 
							"StoreKeyValuePairs - Passcode protection applied to this keychain item (as configured in security-config.xml)");
						srNewEntry.AccessControl = new SecAccessControl (SecAccessible.WhenPasscodeSetThisDeviceOnly, SecAccessControlCreateFlags.UserPresence);
					} else {
						srNewEntry.Accessible = SecAccessible.WhenUnlockedThisDeviceOnly;
						SystemLogger.Log (SystemLogger.Module.PLATFORM, 
							"StoreKeyValuePairs - Applied Accessible WhenUnlockedThisDeviceOnly  (ios 8)");
					}
				} else {
					srNewEntry.Accessible = SecAccessible.WhenUnlockedThisDeviceOnly;
					SystemLogger.Log (SystemLogger.Module.PLATFORM, 
						"StoreKeyValuePairs - Applied Accessible WhenUnlockedThisDeviceOnly  (ios 7)");
					if (this.GetPasscodeProtectedKeys ().Contains (kp.Key)) {
						SystemLogger.Log (SystemLogger.Module.PLATFORM, 
							"StoreKeyValuePairs - Passcode protection is requested for this keychain item, but protection couldn't be applied due to device is iOS<8");
					}
				}

				SecStatusCode code = SecKeyChain.Add (srNewEntry);
				if (code == SecStatusCode.DuplicateItem) {
					SecRecord srDeleteExistingEntry = new SecRecord (SecKind.GenericPassword){
						Account = kp.Key
					};
						if (sAccessGroup != null)
							srDeleteExistingEntry.AccessGroup = sAccessGroup;
						code = SecKeyChain.Remove (srDeleteExistingEntry);
					if (code == SecStatusCode.Success)
						SecKeyChain.Add (srNewEntry);
						
				}
				if (code == SecStatusCode.Success){
					successfullKeyPairs.Add (kp);
				} else {
					failedKeyPairs.Add(kp);
				}
			}

			SystemLogger.Log(SystemLogger.Module.PLATFORM,"StoreKeyValuePairs - Success: " + successfullKeyPairs.Count + ", Failed: " + failedKeyPairs.Count);
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {		
				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.Security.OnKeyValuePairsStoreCompleted", new object[]{successfullKeyPairs, failedKeyPairs});
			});
		}


		public override void GetStoredKeyValuePair (KeyPair keyname)
		{
			GetStoredKeyValuePairs (new KeyPair[] { keyname });
		}

		public override void GetStoredKeyValuePairs (KeyPair[] keynames)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				string sAccessGroup = KeyChainAccessGroup;
				List<KeyPair> foundKeyPairs = new List<KeyPair>();
				foreach(KeyPair key in keynames){
					SecRecord srSearchCriteria = new SecRecord (SecKind.GenericPassword){
						Account = key.Key
					};

					if (sAccessGroup != null)
						srSearchCriteria.AccessGroup = sAccessGroup;

					SecStatusCode keyResult;
					SecRecord srFoundKey = SecKeyChain.QueryAsRecord (srSearchCriteria, out keyResult);
					if (keyResult == SecStatusCode.Success) {
						if(srFoundKey!=null){
							KeyPair found = SecRecordToKeyPair(srFoundKey);
							//SystemLogger.Log(SystemLogger.Module.PLATFORM,"GetStoredKeyValuePairs - Found: "+found.Value);
							if(key.Encryption){
								found.Value = Decrypt(found.Value);

								//SystemLogger.Log(SystemLogger.Module.PLATFORM,"GetStoredKeyValuePairs - Decrypt: "+found.Value);
							}
							foundKeyPairs.Add(found);
						}
					}
				}

				SystemLogger.Log(SystemLogger.Module.PLATFORM,"GetStoredKeyValuePairs - Found: " + foundKeyPairs.Count);

				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.Security.OnKeyValuePairsFound", foundKeyPairs);
			});
		}

		public override void RemoveStoredKeyValuePair (string keyname)
		{
			RemoveStoredKeyValuePairs (new string[] { keyname });
		}

		public override void RemoveStoredKeyValuePairs (string[] keynames)
		{
			string sAccessGroup = KeyChainAccessGroup;
			List<string> successfullKeyPairs = new List<string>();
			List<string> failedKeyPairs = new List<string>();
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				foreach (string keyname in keynames) {
					SecRecord srDeleteEntry = new SecRecord (SecKind.GenericPassword) {
						Account = keyname
					};

					if (sAccessGroup != null)
						srDeleteEntry.AccessGroup = sAccessGroup;

					SecStatusCode code = SecKeyChain.Remove(srDeleteEntry);
					if (code == SecStatusCode.Success) {
						successfullKeyPairs.Add(keyname);
					}else{
						failedKeyPairs.Add(keyname);
					}
				}
				SystemLogger.Log(SystemLogger.Module.PLATFORM,"RemoveStoredKeyValuePair - Success: " + successfullKeyPairs.Count + ", Failed: " + failedKeyPairs.Count);
					
				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.Security.OnKeyValuePairsRemoveCompleted", new object[]{successfullKeyPairs, failedKeyPairs});
			});	
		}

		/// <summary>
		/// Starts local authentication operation displaying Touch ID screen.
		/// </summary>
		/// <param name="reason">A reason to explain why authentication is needed. This helps to build trust with the user.</param>
		public override void StartLocalAuthenticationWithTouchID (string reason) {
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "StartLocalAuthenticationWithTouchID - starting authentication using Touch ID..."); 
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {

				SystemLogger.Log (SystemLogger.Module.PLATFORM, "StartLocalAuthenticationWithTouchID - checking Touch ID available in this device"); 
				if(reason==null) {
					reason = "";
					SystemLogger.Log (SystemLogger.Module.PLATFORM, 
						"StartLocalAuthenticationWithTouchID - *** WARNING ***: no reason text provided. App should provide a text to explain why Touch ID authentication is needed");
				}
				bool available = this.CanEvaluatePolicy();
				if(available) {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "StartLocalAuthenticationWithTouchID - start device owner authenticaiton using biometrics");
					this.EvaluatePolicy(reason);
				} else {
					IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.Security.onTouchIDNotAvailable", null);
				}

			});
		}

		#endregion

		#region KEYCHAIN PRIVATE METHODS

		private KeyPair SecRecordToKeyPair (SecRecord entry)
		{
			if (entry != null) {
				KeyPair returnKeyPair = new KeyPair ();
				returnKeyPair.Key = entry.Account;
				returnKeyPair.Value = (string)NSString.FromData(entry.ValueData, NSStringEncoding.UTF8);
				return returnKeyPair;
			}
			return null;
		}

		#endregion

		#region LOCAL AUTHENTICATION WITH TOUCH ID private methods

		private bool CanEvaluatePolicy ()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var context = new LAContext ();
				string message = string.Empty;
				NSError error;
				bool success = context.CanEvaluatePolicy (LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out error);

				SystemLogger.Log (SystemLogger.Module.PLATFORM, "CanEvaluatePolicy - " + (success ? "Touch ID is available" : "Touch ID is not available"));
				return success;
			} else {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "CanEvaluatePolicy - device OS is under iOS 8 - touch ID not available");
				return false;
			}

		}

		private void EvaluatePolicy (string reason)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var context = new LAContext ();
				context.EvaluatePolicy (LAPolicy.DeviceOwnerAuthenticationWithBiometrics, reason, HandleLAContextReplyHandler);
			} else {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "EvaluatePolicy - device OS is under iOS 8 - touch ID not available");
			}
		}

		private void HandleLAContextReplyHandler (bool success, NSError error)
		{
			string message = success ? "EvaluatePolicy success" : string.Format ("EvaluatePolicy: failed with error: {0}", error.LocalizedDescription);
			string errorDescription = "Authentication Success";
			LocalAuthenticationStatus status = LocalAuthenticationStatus.Success;

			if (!success) {
				errorDescription = error.LocalizedDescription;
				switch (error.Code) {
				case 0:	
					status = LocalAuthenticationStatus.Success;
					break;
				case -1:
					status = LocalAuthenticationStatus.RetryExceeded;
					break;
				case -2:
					status = LocalAuthenticationStatus.UserCancel;
					break;
				case -3:
					status = LocalAuthenticationStatus.UserFallback;
					break;
				default:
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "HandleLAContextReplyHandler - found not handled error code: " + error.Code);
					break;
				} 
			}

			SystemLogger.Log (SystemLogger.Module.PLATFORM, "HandleLAContextReplyHandler - " + message);

			UIApplication.SharedApplication.InvokeOnMainThread (delegate {		
				IPhoneUtils.GetInstance ().FireUnityJavascriptEvent ("Appverse.Security.onLocalAuthenticationWithTouchIDReply", new object[]{ status, errorDescription});
			});
		}

		#endregion

	
		private static string Encrypt(string plainText)
		{
			var value = "";
			try{
				var key = NSBundle.MainBundle.InfoDictionary ["CFBundleIdentifier"] + "_" + typeof(IPhoneSecurity).Namespace;	
				//SystemLogger.Log (SystemLogger.Module.PLATFORM, "Decrypt: " + key);
				byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

				//byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);

				byte[] keyBytes = new Rfc2898DeriveBytes(key, new byte[8]).GetBytes(256 / 8);
				var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
				//var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
				var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(typeof(IPhoneSecurity).Namespace));

				byte[] cipherTextBytes;

				using (var memoryStream = new MemoryStream())
				{
					using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
					{
						cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
						cryptoStream.FlushFinalBlock();
						cipherTextBytes = memoryStream.ToArray();
						cryptoStream.Close();
					}
					memoryStream.Close();
				}
				value = Convert.ToBase64String(cipherTextBytes);
			}catch(Exception e){

				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Encrypt error - " + e.Message);
				value = plainText;
			}
			return value;
		}

		private static string Decrypt(string encryptedText)
		{
			var value = "";
			try{
				var key = NSBundle.MainBundle.InfoDictionary ["CFBundleIdentifier"] + "_" + typeof(IPhoneSecurity).Namespace;	
				//SystemLogger.Log (SystemLogger.Module.PLATFORM, "Decrypt: " + key);
				byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
				//byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
				byte[] keyBytes = new Rfc2898DeriveBytes(key, new byte[8]).GetBytes(256 / 8);
				var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

				//var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
				var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(typeof(IPhoneSecurity).Namespace));

				var memoryStream = new MemoryStream(cipherTextBytes);
				var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
				byte[] plainTextBytes = new byte[cipherTextBytes.Length];

				int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
				memoryStream.Close();
				cryptoStream.Close();
				value = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
			}catch(Exception e){

				SystemLogger.Log (SystemLogger.Module.PLATFORM, "Decrypt error - " + e.Message);
				value = encryptedText;
			}
			return value;
		}
	}
}

