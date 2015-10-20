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
using Unity.Core.AppLoader;
using System.IO;
using Unity.Core.Storage.FileSystem;
using Unity.Core.System;
using System.Collections.Generic;
using Unity.Core.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Core;
using Foundation;
using System.Runtime.InteropServices;
using Unity.Core.Notification;
using UIKit;
using System.Text;

namespace Unity.Platform.IPhone
{
	public class IPhoneAppLoader : AbstractLoader
	{
		/* TO BE REMOVED - 5.0.6 [AMOB-30]
		private static string DOCUMENTS_URI = "http://127.0.0.1:{0}/documents";
		*/
		private static string DOCUMENTS_URI = "https://appverse/documents";

		// PLATFORM SERVICES DEPENDENCIES
		private IFileSystem _fileSystemService = null;
		private IIo _ioService = null;
		private INotification _notificationService = null;

		public IPhoneAppLoader ()
		{
		}

		#region implemented abstract members of AbstractLoader

		/// <summary>
		/// Lists the installed modules.
		/// </summary>
		/// <returns>The installed modules.</returns>
		public override Module[] ListInstalledModules ()
		{
			List<Module> list = new List<Module> ();

			string ModulesPath = this.GetModulesRootPath();
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Listing installed modules under path: " + ModulesPath);

			DirectoryData modulesDirectory = new DirectoryData(ModulesPath);

			if(this.GetFileSystemService().ExistsDirectory(modulesDirectory)) {
				DirectoryData[] apps = this.GetFileSystemService().ListDirectories(modulesDirectory);
				foreach(DirectoryData app in apps) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "directory: " + app.FullName);
					DirectoryData[] versions = this.GetFileSystemService().ListDirectories(app);

					foreach(DirectoryData version in versions) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "version: " + version.FullName);
						Module module = new Module();
						module.Id = Path.GetFileName(app.FullName);
						module.Version = this.ParseModuleVersion(Path.GetFileName(version.FullName));
						list.Add(module);
					}
				}
			} else {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Modules directory does not exists: " + ModulesPath);
			}

			return list.ToArray();
		}

		/// <summary>
		/// Updates the modules.
		/// </summary>
		/// <param name="modules">Modules.</param>
		/// <param name="callbackId">An identifier to be returned on the event listener in order to identify this request.</param>
		public override void UpdateModules (Module[] modules, string callbackId)
		{
			List<Module> successlist = new List<Module> ();
			List<Module> failedlist = new List<Module> ();
			
			// show activity indicator
			this.GetNotificationService().StartNotifyActivity();
			this.GetNotificationService().StartNotifyLoading(this.GetLocalizedMessage(DEFAULT_LOADING_MESSAGE_UPDATE_MODULES));

			foreach(Module module in modules) {
				bool success = this.UpdateOrInstallModule(module);
				if(success) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "The module [ " +  module.Id + "] was successfully updated");
					successlist.Add(module);
				} else {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "The module [ " +  module.Id + "] was NOT successfully updated");
					failedlist.Add (module);
				}
			}

			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.AppLoader.onUpdateModulesFinished", 
				                                                   new object []{successlist.ToArray(), failedlist.ToArray(), callbackId});
			});

			// hide activity indicator
			this.GetNotificationService().StopNotifyActivity();
			this.GetNotificationService().StopNotifyLoading();
		}

		/// <summary>
		/// Updates the module.
		/// </summary>
		/// <param name="module">Module.</param>
		/// <param name="callbackId">An identifier to be returned on the event listener in order to identify this request.</param>
		public override void UpdateModule (Module module, string callbackId)
		{
			List<Module> successlist = new List<Module> ();
			List<Module> failedlist = new List<Module> ();

			// show activity indicator
			this.GetNotificationService().StartNotifyActivity();
			this.GetNotificationService().StartNotifyLoading(this.GetLocalizedMessage(DEFAULT_LOADING_MESSAGE_UPDATE_MODULE));

			if(module != null) {
				bool success = this.UpdateOrInstallModule(module);
				if(success) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "The module [ " +  module.Id + "] was successfully updated");
					successlist.Add(module);
				} else {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "The module [ " +  module.Id + "] was NOT successfully updated");
					failedlist.Add (module);
				}
			}

			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.AppLoader.onUpdateModulesFinished", 
				                                                   new object []{successlist.ToArray(), failedlist.ToArray(), callbackId});
			});

			// hide activity indicator
			this.GetNotificationService().StopNotifyActivity();
			this.GetNotificationService().StopNotifyLoading();

		}

		/// <summary>
		/// Deletes the modules.
		/// </summary>
		/// <param name="modules">Modules.</param>
		public override void DeleteModules (Module[] modules)
		{
			List<Module> successlist = new List<Module> ();
			List<Module> failedlist = new List<Module> ();

			this.GetNotificationService().StartNotifyLoading(this.GetLocalizedMessage(DEFAULT_LOADING_MESSAGE_DELETE_MODULES));

			try {
				foreach(Module module in modules) {
					bool moduleDeleted = false;
					try {
						string location = this.GetModuleLocation(module, false);
						string directoryName = Path.Combine (this.GetFileSystemService().GetDirectoryRoot().FullName, location);
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Deleting module under: " + location);
						
						if(Directory.Exists(directoryName)) {
							Directory.Delete(directoryName, true);
							moduleDeleted = true;
						} else {
							SystemLogger.Log(SystemLogger.Module.PLATFORM, "Module does not exists on filesystem. It couldn't be deleted.");
						}
					} catch (Exception ex) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, 
							"Exception when deleting module [" + (module!=null?module.Id:"undefined")+ "]: " + ex.Message);
					}

					if(moduleDeleted) {
						successlist.Add(module);
					} else {
						failedlist.Add(module);
					}
				}
			} catch (Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Exception when deleting modules: " + ex.Message);
			}
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.AppLoader.onDeleteModulesFinished", 
				                                                   new object []{successlist.ToArray(), failedlist.ToArray()});
			});

			this.GetNotificationService().StopNotifyLoading();
		}

		/// <summary>
		/// Loads the module (try to update it first if 'autUpdate' argument is set to true). Update is "silent", no event listener is called.
		/// </summary>
		/// <param name="module">Module to be loaded.</param>
		/// <param name="moduleParams">Module parameters.</param>
		/// <param name="autoUpdate">True to first update the module, prior to be loaded. False is the default value.</param>
		public override void LoadModule (Module module, ModuleParam[] moduleParams, bool autoUpdate)
		{
			if(autoUpdate) {
				// show activity indicator
				this.GetNotificationService().StartNotifyActivity();
				this.GetNotificationService().StartNotifyLoading(this.GetLocalizedMessage(DEFAULT_LOADING_MESSAGE_UPDATE_MODULE));
				
				if(module != null) {
					bool success = this.UpdateOrInstallModule(module);
					if(success) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "[LoadModule#autoUpdate] The module [ " +  module.Id + "] was successfully updated");
					} else {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "[LoadModule#autoUpdate] The module [ " +  module.Id + "] was NOT successfully updated");
					}
				}
				
				// hide activity indicator
				this.GetNotificationService().StopNotifyActivity();
				this.GetNotificationService().StopNotifyLoading();
			}

			// Load the just updated (or not) module.
			this.LoadModule(module, moduleParams);
		}

		/// <summary>
		/// Loads the module.
		/// </summary>
		/// <param name="module">Module to be loaded.</param>
		/// <param name="moduleParams">Module parameters.</param>
		public override void LoadModule (Module module, ModuleParam[] moduleParams)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 

				try {
					if(module != null) {

						string location = this.GetModuleLocation(module, true);
						string directoryName = Path.Combine (this.GetFileSystemService().GetDirectoryRoot().FullName, location);

						/* TO BE REMOVED - 5.0.6 [AMOB-30]
						string path = Path.Combine (String.Format(DOCUMENTS_URI,IPhoneServiceLocator.CurrentDelegate.GetListeningPort()), location, DEFAULT_HOME_PAGE);
						*/
						string path = Path.Combine (DOCUMENTS_URI, location, DEFAULT_HOME_PAGE);

						if(Directory.Exists(directoryName)) {
							// pass parameters to the request URL
							if(moduleParams != null) {
								StringBuilder builder = new StringBuilder();
								int numParams = 0;
								foreach(ModuleParam p in moduleParams) {
									if(p.Name!=null && p.Name.Length>0 && p.Value!=null && p.Value.Length>0) {
										if(numParams==0) {
											builder.Append("?");
										} else {
											builder.Append("&");
										}
										builder.Append(p.Name+"="+p.Value);
										numParams++;
									}
								}
								path = path + builder.ToString();
							}

							SystemLogger.Log(SystemLogger.Module.PLATFORM, "Loading module at path: " + path);
							NSUrl nsUrl = new NSUrl(Uri.EscapeUriString(path));
							NSUrlRequest request = new NSUrlRequest (nsUrl, NSUrlRequestCachePolicy.ReloadIgnoringLocalAndRemoteCacheData, 3600.0);

							IPhoneServiceLocator.CurrentDelegate.LoadRequest(request);
						} else {
							this.GetNotificationService().StartNotifyAlert(this.GetLocalizedMessage(DEFAULT_ALERT_MESSAGE_TITLE),  
							                                               this.GetLocalizedMessage(DEFAULT_ALERT_MESSAGE_LOAD_MODULE_ERROR), "OK");
						}
					} else {
								this.GetNotificationService().StartNotifyAlert(this.GetLocalizedMessage(DEFAULT_ALERT_MESSAGE_TITLE),  
						                                               this.GetLocalizedMessage(DEFAULT_ALERT_MESSAGE_LOAD_MODULE_ERROR), "OK");
					}
				} catch (Exception ex) {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "Exception when loading module: " + ex.Message);
				}
			});
		}
		#endregion


		/// <summary>
		/// Stores the module zip file.
		/// </summary>
		/// <returns><c>true</c>, if module zip file was stored, <c>false</c> otherwise.</returns>
		/// <param name="module">Module.</param>
		/// <param name="tempFile">Temp file.</param>
		public override bool StoreModuleZipFile(Module module, string tempFile) {
			bool result = false;
			FileStream streamWriter = null;
			String fullTempFilePath = null;

			try {
				fullTempFilePath = Path.Combine(this.GetFileSystemService().GetDirectoryRoot ().FullName, tempFile);

				string location = this.GetModuleLocation(module, true);
				string moduleRootDir = module.Id + "-" + module.Version.ToString() + Path.DirectorySeparatorChar;
				string versionDirectory = Path.Combine (this.GetFileSystemService().GetDirectoryRoot().FullName, location);
				string appDirectory = Path.Combine (this.GetFileSystemService().GetDirectoryRoot().FullName, this.GetModuleLocation(module, false));
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Storing module to: " +location);

				if(!Directory.Exists(appDirectory)) {
					Directory.CreateDirectory(versionDirectory);
				} else {
					Directory.Delete(appDirectory, true);
					Directory.CreateDirectory(versionDirectory);
				}

				ZipFile zipFile = new ZipFile(fullTempFilePath);
				foreach(ZipEntry theEntry in zipFile) {
					// just for testing... 
					// SystemLogger.Log(SystemLogger.Module.PLATFORM, "Storing module entry name: " + theEntry.Name);
					string fileName = Path.GetFileName (theEntry.Name);
					if (fileName != String.Empty) {
						string fullPath = versionDirectory + "/" + theEntry.Name;
						
						if(theEntry.Name.IndexOf(moduleRootDir)==0) {
							fullPath = versionDirectory + "/" + theEntry.Name.Substring(moduleRootDir.Length);
						}
						
						fullPath = fullPath.Replace ("\\", "/");
						string fullDirPath = Path.GetDirectoryName (fullPath);
						if (!Directory.Exists (fullDirPath))
							Directory.CreateDirectory (fullDirPath);
						streamWriter = File.Create (fullPath);
						
						Stream entryStream = zipFile.GetInputStream(theEntry);
						byte[] data = IPhoneUtils.ConvertNonSeekableStreamToByteArray(entryStream);
						streamWriter.Write (data, 0, data.Length);
						
						streamWriter.Close ();
						streamWriter = null;
					}
				}
				result = true;

			} catch (Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Exception when storing module: " + ex.Message, ex);
			} finally {
				if(streamWriter!=null)
					streamWriter.Close();

				if(fullTempFilePath!=null && File.Exists(fullTempFilePath)) {
					// deleting tempFile
					File.Delete(fullTempFilePath);
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "tmp file deleted");
				}

			}
			return result;
		}

		public override IIo GetIOService() {
			if(_ioService == null) {
				_ioService = (IIo)IPhoneServiceLocator.GetInstance ().GetService ("io");
			}
			return _ioService;
		}

		public override IFileSystem GetFileSystemService() {
			if(_fileSystemService == null) {
				_fileSystemService = (IFileSystem)IPhoneServiceLocator.GetInstance ().GetService ("file");
			}
			return _fileSystemService;
		}

		public override INotification GetNotificationService() {
			if(_notificationService == null) {
				_notificationService = (INotification)IPhoneServiceLocator.GetInstance().GetService("notify");
			}
			return _notificationService;
		}
	}
}

