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

#if !WP8
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Unity.Core.System;
#else
using System.Threading.Tasks;
#endif

namespace Unity.Core.Security
{
    public abstract class AbstractSecurity : ISecurity
    {

        #region Configuration Security
#if !WP8
        private static readonly string SECURITY_CONFIG_FILE = "app/config/security-config.xml";
        private string _securityConfigFile = SECURITY_CONFIG_FILE;
		private SecurityConfig securityConfig = new SecurityConfig();  // empty configuration

        public virtual string SecurityConfigFile
		{
			get
			{
				return this._securityConfigFile;
			}
			set
			{
				this._securityConfigFile = value;
			}
		}


		public AbstractSecurity() {
			this.LoadConfiguration();
		}

		public List<String> GetPasscodeProtectedKeys() {
			return this.securityConfig.ProtectedKeys;
		}

		/// <summary>
		/// Load security-config.xml
		/// </summary>
		protected void LoadConfiguration()
		{
            securityConfig = new SecurityConfig(); // reset services config mapping when the services could not be loaded for any reason

			try
			{   // FileStream to read the XML document.
				byte[] configFileRawData = GetConfigFileBinaryData();
				if (configFileRawData != null)
				{
					XmlSerializer serializer = new XmlSerializer(typeof(SecurityConfig));
					securityConfig = (SecurityConfig)serializer.Deserialize(new MemoryStream(configFileRawData));
				}
			}
			catch (Exception e)
			{
				//if(!(e is FileNotFoundException))
					SystemLogger.Log(SystemLogger.Module.CORE, "Error when loading security configuration", e);
			}

			SystemLogger.Log (SystemLogger.Module.CORE, "# Security config passcode protected keys #" + securityConfig.ProtectedKeys.Count);
		}

		/// <summary>
		/// Default method, to be overrided by platform implementation. 
		/// </summary>
		/// <returns>
		/// A <see cref="Stream"/>
		/// </returns>
		public virtual byte[] GetConfigFileBinaryData()
		{
			SystemLogger.Log(SystemLogger.Module.CORE, "# Loading Security Configuration from file: " + SecurityConfigFile);

			Stream fs = new FileStream(SecurityConfigFile, FileMode.Open);
			if (fs != null)
			{
				return ((MemoryStream)fs).GetBuffer();
			}
			else
			{
				return null;
			}
		}
#else
#endif
        #endregion

        #region ISecurity implementation
#if !WP8
		public abstract bool IsDeviceModified ();

        public abstract void StoreKeyValuePair(KeyPair keypair);

        public abstract void StoreKeyValuePairs(KeyPair[] keypairs);

		public abstract void GetStoredKeyValuePair(KeyPair KeyName);

		public abstract void GetStoredKeyValuePairs(KeyPair[] KeyNames);

        public abstract void RemoveStoredKeyValuePair(string KeyName);

        public abstract void RemoveStoredKeyValuePairs(string[] KeyNames);

		public abstract void StartLocalAuthenticationWithTouchID (string reason);

#else
        public abstract Task<bool> IsDeviceModified();
        public abstract Task StoreKeyValuePair(KeyPair keypair);
        public abstract Task StoreKeyValuePairs(KeyPair[] keypairs);
		public abstract Task GetStoredKeyValuePair(KeyPair keyName);
		public abstract Task GetStoredKeyValuePairs(KeyPair[] keyNames);
        public abstract Task RemoveStoredKeyValuePair(string keyName);
        public abstract Task RemoveStoredKeyValuePairs(string[] keyNames);
#endif
        #endregion


    }
}
