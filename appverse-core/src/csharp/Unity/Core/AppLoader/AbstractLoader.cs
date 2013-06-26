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
using Unity.Core.IO;
using Unity.Core.Storage.FileSystem;
using Unity.Core.Notification;
using Unity.Core.System;
using System.Collections.Generic;

namespace Unity.Core.AppLoader
{
	public abstract class AbstractLoader : ILoader
	{
		private static char MODULES_VERSION_SEPARATOR = '.';
		private ModuleContext _context = null;
		public static string MODULES_PATH = "apps";
		public static string DEFAULT_HOME_PAGE = "index.html";

		// LOADING MESSAGES
		public static string DEFAULT_LOADING_MESSAGE_UPDATE_MODULES = "Updating Modules";
		public static string DEFAULT_LOADING_MESSAGE_UPDATE_MODULE = "Updating Module";
		public static string DEFAULT_LOADING_MESSAGE_DELETE_MODULES = "Deleting Modules";

		// ALERT MESSAGE
		public static string DEFAULT_ALERT_MESSAGE_TITLE = "AppLoader Alert";
		public static string DEFAULT_ALERT_MESSAGE_LOAD_MODULE_ERROR = "The module could not be loaded";

		private IDictionary<string, string> localizedMessages = new Dictionary<string, string> ();

		public AbstractLoader ()
		{
		}

		#region ILoader implementation

		/// <summary>
		/// Initializes the module context.
		/// </summary>
		/// <param name="context">Context.</param>
		public void InitializeModuleContext (ModuleContext context)
		{
			this._context = context;

			if(this._context != null) {
				localizedMessages[DEFAULT_LOADING_MESSAGE_UPDATE_MODULES] = this._context.LoadingMessage_UpdateModules;
				localizedMessages[DEFAULT_LOADING_MESSAGE_UPDATE_MODULE] = this._context.LoadingMessage_UpdateModule;
				localizedMessages[DEFAULT_LOADING_MESSAGE_DELETE_MODULES] = this._context.LoadingMessage_DeleteModule;
				localizedMessages[DEFAULT_ALERT_MESSAGE_TITLE] = this._context.AlertMessage_Title;
				localizedMessages[DEFAULT_ALERT_MESSAGE_LOAD_MODULE_ERROR] = this._context.AlertMessage_LoadModuleError;
			} 

		}

		public abstract Module[] ListInstalledModules ();

		public abstract void UpdateModules (Module[] modules, string callbackId);

		public abstract void UpdateModule (Module module, string callbackId);

		public abstract void DeleteModules (Module[] modules);

		public abstract void LoadModule (Module module, ModuleParam[] moduleParams);

		public abstract void LoadModule (Module module, ModuleParam[] moduleParams, bool autoUpdate);

		#endregion

		/// <summary>
		/// Gets the service from URL.
		/// </summary>
		/// <returns>The service from URL.</returns>
		/// <param name="url">URL.</param>
		public virtual IOService GetServiceFromUrl(String url) {
			IOService service = new IOService();
			service.Name = "updateModule";
			service.Endpoint = new IOServiceEndpoint();
			service.Endpoint.Host = url;
			service.Endpoint.Port = 0;
			service.Endpoint.Path = "";
			service.RequestMethod = RequestMethod.GET;
			service.Type = ServiceType.OCTET_BINARY;
			
			return service;
		}

		/// <summary>
		/// Parse a module version from the given string version (formatted as major.minor.revision).
		/// </summary>
		/// <returns>The parsed module version object.</returns>
		/// <param name="version">The formatted version string.</param>
		public virtual ModuleVersion ParseModuleVersion(string version) {
			ModuleVersion moduleVersion = new ModuleVersion();
			if (version!=null) {
				string[] versionDetails = version.Split (MODULES_VERSION_SEPARATOR);
				if(versionDetails.Length>0) {
					moduleVersion.Major = versionDetails[0];
				}
				if(versionDetails.Length>1) {
					moduleVersion.Minor = versionDetails[1];
				}
				if(versionDetails.Length>2) {
					moduleVersion.Revision = versionDetails[2];
				}
			}
			
			return moduleVersion;
		}
		
		/// <summary>
		/// Gets the modules root path.
		/// </summary>
		/// <returns>The modules root path.</returns>
		public virtual string GetModulesRootPath() {
			string ModulesPath = MODULES_PATH;
			if(_context != null && _context.User!=null && _context.User.Length>0) {
				ModulesPath = Path.Combine(ModulesPath, _context.User);
			}
			return ModulesPath;
		}

		public virtual string GetLocalizedMessage(String key) {
			if(localizedMessages.ContainsKey(key)) {
				string localizedText = localizedMessages[key];
				return ((localizedText!=null && localizedText.Length>0? localizedText : key));
			} else {
				return key;
			}
		}

		/// <summary>
		/// Gets the module location.
		/// </summary>
		/// <returns>The module location.</returns>
		/// <param name="module">Module.</param>
		/// <param name="versioned">True to return location including version folder, False to return just the module location (for deleting tasks).</param>
		public virtual string GetModuleLocation(Module module, bool versioned) {
			if(module!=null) {
				if(versioned)
    				return Path.Combine(this.GetModulesRootPath(), Path.Combine(module.Id, module.Version.ToString()));
				else
					return Path.Combine(this.GetModulesRootPath(), module.Id);
			}
			return "";
		}

		/// <summary>
		/// Updates the or install module.
		/// </summary>
		/// <param name="module">Module.</param>
		public virtual bool UpdateOrInstallModule(Module module) {
			
			IOService service = this.GetServiceFromUrl(module.LoadUrl);
			IORequest request = new IORequest(); // empty request, no additional data is required apart from the load url
			
			String tempFile = this.GetIOService().InvokeServiceForBinary(request, service, "tmp.zip");
			
			if (tempFile!=null) {
				return this.StoreModuleZipFile(module, tempFile);
			} else {
				SystemLogger.Log(SystemLogger.Module.CORE, "It was not possible to get module data from url: " + module.LoadUrl);
			}
			
			return false;
		}

		public abstract IIo GetIOService();
		
		public abstract IFileSystem GetFileSystemService();
		
		public abstract INotification GetNotificationService();

		public abstract bool StoreModuleZipFile(Module module, string tempFile);
	}
}

