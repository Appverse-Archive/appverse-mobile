/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
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
package com.gft.unity.android;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.FilenameFilter;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.List;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;

import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.android.util.FilenameUtils;
import com.gft.unity.core.apploader.AbstractLoader;
import com.gft.unity.core.apploader.Module;
import com.gft.unity.core.apploader.ModuleParam;
import com.gft.unity.core.io.IIo;
import com.gft.unity.core.notification.INotification;
import com.gft.unity.core.storage.filesystem.DirectoryData;
import com.gft.unity.core.storage.filesystem.IFileSystem;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class AndroidAppLoader extends AbstractLoader {
	
	private static String DOCUMENTS_URI = "http://127.0.0.1:8080/documents";

	private static final String LOGGER_MODULE = "Appverse App Loader";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);

	/** SERVICES DEPENDENCIES **/

	@Override
	public IFileSystem GetFileSystemService() {
		return (IFileSystem) AndroidServiceLocator.GetInstance().GetService(
				AndroidServiceLocator.SERVICE_TYPE_FILESYSTEM);
	}

	@Override
	public IIo GetIOService() {
		return (IIo) AndroidServiceLocator.GetInstance().GetService(
				AndroidServiceLocator.SERVICE_TYPE_IO);
	}

	@Override
	public INotification GetNotificationService() {
		return (INotification) AndroidServiceLocator.GetInstance().GetService(
				AndroidServiceLocator.SERVICE_TYPE_NOTIFICATION);
	}
	/** END DEPENDENCIES **/
	
	@Override
	public void DeleteModules(Module[] modules) {
		List<Module> successlist = new ArrayList<Module>();
		List<Module> failedlist = new ArrayList<Module>();

		this.GetNotificationService().StartNotifyLoading(this.GetLocalizedMessage(DEFAULT_LOADING_MESSAGE_DELETE_MODULES));

		try {
			for(Module module : modules) {
				boolean moduleDeleted = false;
				try {
					String location = this.GetModuleLocation(module, false);
					String directoryName = new File(
							this.GetFileSystemService().GetDirectoryRoot().getFullName(),
							location).getAbsolutePath();
					LOGGER.logInfo("DeleteModules", "Deleting module under: " + location);
					
					File df = new File(directoryName);
					if (df.exists() && df.isDirectory()) {
						moduleDeleted = this.deleteR(df);
					} else {
						LOGGER.logWarning("DeleteModules", "Module does not exists on filesystem. It couldn't be deleted.");
					}
				} catch (Exception ex) {
					LOGGER.logWarning("DeleteModules",  
						"Exception when deleting module [" + (module!=null?module.getId():"undefined")+ "]: " + ex.getMessage());
				}

				if(moduleDeleted) {
					successlist.add(module);
				} else {
					failedlist.add(module);
				}
			}
		} catch (Exception ex) {
			LOGGER.logWarning("DeleteModules", "Exception when deleting modules: " + ex.getMessage());
		}
		
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		am.executeJS("Unity.AppLoader.onDeleteModulesFinished", 
				new Object []{successlist.toArray(new Module[successlist.size()]), failedlist.toArray(new Module[failedlist.size()])});
		
		this.GetNotificationService().StopNotifyLoading();
	}

	@Override
	public Module[] ListInstalledModules() {
		List<Module> list = new ArrayList<Module>();

		String ModulesPath = this.GetModulesRootPath();
		LOGGER.logInfo("ListInstalledModules", "Listing installed modules under path: " + ModulesPath);

		DirectoryData modulesDirectory = new DirectoryData(ModulesPath);

		if(this.GetFileSystemService().ExistsDirectory(modulesDirectory)) {
			DirectoryData[] apps = this.GetFileSystemService().ListDirectories(modulesDirectory);
			for(DirectoryData app : apps) {
				LOGGER.logInfo("ListInstalledModules", "directory: " + app.getFullName());
				DirectoryData[] versions = this.GetFileSystemService().ListDirectories(app);

				for(DirectoryData version : versions) {
					LOGGER.logInfo("ListInstalledModules", "version: " + version.getFullName());
					Module module = new Module();
					String fileName = new File(app.getFullName()).getName();
					module.setId(fileName);
					fileName = new File(version.getFullName()).getName();
					module.setVersion(this.ParseModuleVersion(fileName));
					list.add(module);
				}
			}
		} else {
			LOGGER.logWarning("ListInstalledModules", "Modules directory does not exists: " + ModulesPath);
		}

		return list.toArray(new Module[list.size()]);
	}

	@Override
	public void LoadModule(Module module, ModuleParam[] moduleParams) {
		try {
			if(module != null) {

				String location = this.GetModuleLocation(module, true);
				LOGGER.logInfo("LoadModule", "Loading module at location: " + location);
				File directory = new File (this.GetFileSystemService().GetDirectoryRoot().getFullName(), location);
				
				String directoryName = directory.getAbsolutePath();
				LOGGER.logInfo("LoadModule", "Loading module at directoryName: " + directoryName);
				
				String path = DOCUMENTS_URI + FilenameUtils.UNIX_SEPARATOR + location + FilenameUtils.UNIX_SEPARATOR + DEFAULT_HOME_PAGE;
				LOGGER.logInfo("LoadModule", "Loading module at path: " + path);

				if(directory.exists()) {
					// pass parameters to the request URL
					if(moduleParams != null) {
						StringBuilder builder = new StringBuilder();
						int numParams = 0;
						for(ModuleParam p : moduleParams) {
							if(p.getName()!=null && p.getName().length()>0 && p.getValue()!=null && p.getValue().length()>0) {
								if(numParams==0) {
									builder.append("?");
								} else {
									builder.append("&");
								}
								builder.append(p.getName()+"="+p.getValue());
								numParams++;
							}
						}
						path = path + builder.toString();
					}

					LOGGER.logInfo("LoadModule", "Loading module at path: " + path);
					
					// Uri.EscapeUriString(path)  // TODO check is escaping url is needed
					IActivityManager am = (IActivityManager) AndroidServiceLocator
							.GetInstance().GetService(
									AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
					am.loadUrlIntoWebView(path);
					
				} else {
					this.GetNotificationService().StartNotifyAlert(this.GetLocalizedMessage(DEFAULT_ALERT_MESSAGE_TITLE),  
					                                               this.GetLocalizedMessage(DEFAULT_ALERT_MESSAGE_LOAD_MODULE_ERROR), "OK");
				}
			} else {
						this.GetNotificationService().StartNotifyAlert(this.GetLocalizedMessage(DEFAULT_ALERT_MESSAGE_TITLE),  
				                                               this.GetLocalizedMessage(DEFAULT_ALERT_MESSAGE_LOAD_MODULE_ERROR), "OK");
			}
		} catch (Exception ex) {
			LOGGER.logWarning("LoadModule", "Exception when loading module: " + ex.getMessage());
		}
	}

	@Override
	public void LoadModule(Module module, ModuleParam[] moduleParams, boolean autoUpdate) {
		if(autoUpdate) {
			// show activity indicator
			// not yet supported // this.GetNotificationService().StartNotifyActivity();
			this.GetNotificationService().StartNotifyLoading(this.GetLocalizedMessage(DEFAULT_LOADING_MESSAGE_UPDATE_MODULE));
			
			if(module != null) {
				boolean success = this.UpdateOrInstallModule(module);
				if(success) {
					LOGGER.logInfo("LoadModule#autoUpdate", "The module [ " +  module.getId() + "] was successfully updated");
				} else {
					LOGGER.logInfo("LoadModule#autoUpdate]", "The module [ " +  module.getId() + "] was NOT successfully updated");
				}
			}
			
			// hide activity indicator
			// not yet supported // this.GetNotificationService().StopNotifyActivity();
			this.GetNotificationService().StopNotifyLoading();
		}

		// Load the just updated (or not) module.
		this.LoadModule(module, moduleParams);
	}

	@Override
	public boolean StoreModuleZipFile(Module module, String tempFile) {
		boolean result = false;
		File fullTempFile = null;

		try {
			fullTempFile = new File(this.GetFileSystemService().GetDirectoryRoot ().getFullName(), tempFile);

			String location = this.GetModuleLocation(module, true);
			String moduleRootDir = module.getId() + "-" + module.getVersion().toString() + FilenameUtils.UNIX_SEPARATOR;
			File versionDirectory = new File (this.GetFileSystemService().GetDirectoryRoot().getFullName(), location);
			File appDirectory = new File (this.GetFileSystemService().GetDirectoryRoot().getFullName(), this.GetModuleLocation(module, false));
			LOGGER.logInfo("StoreModuleZipFile", "Storing module to: " +location);

			if(!appDirectory.exists()) {
				versionDirectory.mkdirs();
			} else {
				this.deleteR(appDirectory);
				versionDirectory.mkdirs();
			}

			ZipFile zipFile = new ZipFile(fullTempFile);
			
			for (Enumeration<? extends ZipEntry> e = zipFile.entries(); e.hasMoreElements();) {
				
				ZipEntry theEntry = (ZipEntry) e.nextElement();
				
				String fileName = FilenameUtils.getName(theEntry.getName());
				if (!fileName.isEmpty()) {
					String fullPath = versionDirectory.getAbsolutePath() + FilenameUtils.UNIX_SEPARATOR + theEntry.getName();
					if(theEntry.getName().indexOf(moduleRootDir)==0) {
						fullPath = versionDirectory + "/" + theEntry.getName().substring(moduleRootDir.length());
					}
					fullPath = fullPath.replace (FilenameUtils.WINDOWS_SEPARATOR, FilenameUtils.UNIX_SEPARATOR);
					File fullFile = new File(fullPath);
					File fullDir = fullFile.getParentFile();
					// just for testing //LOGGER.logInfo("StoreModuleZipFile", "Storing module fullPath: " + fullPath);
					if (fullDir!=null  && !fullDir.exists()) {
						// just for testing //LOGGER.logInfo("StoreModuleZipFile", "creating directories for path: " + fullDir.getAbsolutePath());
						fullDir.mkdirs();  // FileOutputStream will create the file for us
					}
					InputStream entryStream = zipFile.getInputStream(theEntry);
					BufferedInputStream bis = new BufferedInputStream(entryStream);
					 
	                int size;
	                byte[] buffer = new byte[2048];
	 
	                FileOutputStream fos = new FileOutputStream(fullPath);
	                BufferedOutputStream bos = new BufferedOutputStream(fos, buffer.length);
	 
	                while ((size = bis.read(buffer, 0, buffer.length)) != -1) {
	                    bos.write(buffer, 0, size);
	                }
	 
	                bos.flush();
	                bos.close();
	                fos.close();
	 
	                bis.close();
					
				}
				
				result = true;
			}
		} catch (Exception ex) {
			LOGGER.logWarning("StoreModuleZipFile", "Exception when storing module: " + ex.getMessage(), ex);
		} finally {
			
			if(fullTempFile!=null && fullTempFile.exists()) {
				// deleting tempFile
				fullTempFile.delete();
				LOGGER.logInfo("StoreModuleZipFile", "tmp file deleted");
			}

		}
		return result;
	}

	@Override
	public void UpdateModule(Module module, String callbackId) {
		List<Module> successlist = new ArrayList<Module> ();
		List<Module> failedlist = new ArrayList<Module> ();

		// show activity indicator
		// not yet supported // this.GetNotificationService().StartNotifyActivity();
		this.GetNotificationService().StartNotifyLoading(this.GetLocalizedMessage(DEFAULT_LOADING_MESSAGE_UPDATE_MODULE));

		if(module != null) {
			boolean success = this.UpdateOrInstallModule(module);
			if(success) {
				LOGGER.logInfo("UpdateModule", "The module [ " +  module.getId() + "] was successfully updated");
				successlist.add(module);
			} else {
				LOGGER.logInfo("UpdateModule", "The module [ " +  module.getId() + "] was NOT successfully updated");
				failedlist.add (module);
			}
		}

		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		am.executeJS("Unity.AppLoader.onUpdateModulesFinished", 
				new Object []{successlist.toArray(new Module[successlist.size()]), failedlist.toArray(new Module[failedlist.size()]), callbackId});
		

		// hide activity indicator
		// not yet supported // this.GetNotificationService().StopNotifyActivity();
		this.GetNotificationService().StopNotifyLoading();

	}

	@Override
	public void UpdateModules(Module[] modules, String callbackId) {
		List<Module> successlist = new ArrayList<Module> ();
		List<Module> failedlist = new ArrayList<Module> ();
		
		// show activity indicator
		// not yet supported // this.GetNotificationService().StartNotifyActivity();
		this.GetNotificationService().StartNotifyLoading(this.GetLocalizedMessage(DEFAULT_LOADING_MESSAGE_UPDATE_MODULES));

		for(Module module : modules) {
			boolean success = this.UpdateOrInstallModule(module);
			if(success) {
				LOGGER.logInfo("UpdateModules", "The module [ " +  module.getId() + "] was successfully updated");
				successlist.add(module);
			} else {
				LOGGER.logInfo("UpdateModules", "The module [ " +  module.getId() + "] was NOT successfully updated");
				failedlist.add (module);
			}
		}

		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		am.executeJS("Unity.AppLoader.onUpdateModulesFinished", 
				new Object []{successlist.toArray(new Module[successlist.size()]), failedlist.toArray(new Module[failedlist.size()]), callbackId});
		

		// hide activity indicator
		// not yet supported // this.GetNotificationService().StopNotifyActivity();
		this.GetNotificationService().StopNotifyLoading();

	}
	
	/** UTILITY FUNCTIONS **/
	
	private boolean deleteR(File f) {

		if (f.isDirectory()) {
			File[] children = f.listFiles();
			for (File child : children) {
				deleteR(child);
			}
		}

		return f.delete();
	}

}
