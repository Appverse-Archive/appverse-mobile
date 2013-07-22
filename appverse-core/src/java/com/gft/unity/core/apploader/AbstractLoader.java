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
package com.gft.unity.core.apploader;

import com.gft.unity.core.io.*;
import com.gft.unity.core.notification.INotification;
import com.gft.unity.core.storage.filesystem.IFileSystem;
import com.gft.unity.core.system.log.Logger;
import java.io.File;
import java.util.HashMap;
import java.util.Map;
import java.util.StringTokenizer;

public abstract class AbstractLoader implements ILoader {
    
    private static final String LOGGER_MODULE = "Appverse AbstractLoader";
    private static final Logger LOGGER = Logger.getInstance(Logger.LogCategory.CORE,
            LOGGER_MODULE);

    private static String MODULES_VERSION_SEPARATOR = ".";
    private ModuleContext _context = null;
    public static String MODULES_PATH = "apps";
    public static String DEFAULT_HOME_PAGE = "index.html";
    // LOADING MESSAGES
    public static String DEFAULT_LOADING_MESSAGE_UPDATE_MODULES = "Updating Modules";
    public static String DEFAULT_LOADING_MESSAGE_UPDATE_MODULE = "Updating Module";
    public static String DEFAULT_LOADING_MESSAGE_DELETE_MODULES = "Deleting Modules";
    // ALERT MESSAGE
    public static String DEFAULT_ALERT_MESSAGE_TITLE = "AppLoader Alert";
    public static String DEFAULT_ALERT_MESSAGE_LOAD_MODULE_ERROR = "The module could not be loaded";
    private Map<String, String> localizedMessages = new HashMap<String, String>();

    public AbstractLoader() {
    }

    /**
     * Initializes the module context.
     *
     * @param context
     */
    public void InitializeModuleContext(ModuleContext context) {
        this._context = context;

        if (this._context != null) {
            localizedMessages.put(DEFAULT_LOADING_MESSAGE_UPDATE_MODULES, this._context.getLoadingMessage_UpdateModules());
            localizedMessages.put(DEFAULT_LOADING_MESSAGE_UPDATE_MODULE, this._context.getLoadingMessage_UpdateModule());
            localizedMessages.put(DEFAULT_LOADING_MESSAGE_DELETE_MODULES, this._context.getLoadingMessage_DeleteModule());
            localizedMessages.put(DEFAULT_ALERT_MESSAGE_TITLE, this._context.getAlertMessage_Title());
            localizedMessages.put(DEFAULT_ALERT_MESSAGE_LOAD_MODULE_ERROR, this._context.getAlertMessage_LoadModuleError());
        }
    }

    public abstract Module[] ListInstalledModules();

    public abstract void UpdateModules(Module[] modules, String callbackId);

    public abstract void UpdateModule(Module module, String callbackId);

    public abstract void DeleteModules(Module[] modules);

    public abstract void LoadModule(Module module, ModuleParam[] moduleParams);

    public abstract void LoadModule(Module module, ModuleParam[] moduleParams, boolean autoUpdate);

    /**
     * Gets the service from URL.
     *
     * @param url
     * @return
     */
    protected IOService GetServiceFromUrl(String url) {
        IOService service = new IOService();
        service.setName("updateModule");
        service.setEndpoint(new IOServiceEndpoint());
        service.getEndpoint().setHost(url);
        service.getEndpoint().setPort(0);
        service.getEndpoint().setPath("");
        service.setRequestMethod(RequestMethod.GET);
        service.setType(ServiceType.OCTET_BINARY);

        return service;
    }

    
    protected IORequest GetRequestWithRequiredHeaders() {
        IORequest request = new IORequest();

        if (_context != null && _context.getCredentials() != null && _context.getCredentials().length() > 0) {
            // Basic Authentication available
            // Credentials should be a base64 encoding for the "username:password" concatenation string
            // TODO  allow other authentication types in the future
            IOHeader authHeader = new IOHeader();
            authHeader.setName("Authorization");
            authHeader.setValue("Basic " + _context.getCredentials());

            request.setHeaders(new IOHeader[]{authHeader});

        }
        
        // Accept header is needed to avoid "406 not acceptable" error, but should be set in the request final object
        // I/O Services use the request content type to set this value on the final request object
        request.setContentType("*/*");
        
        return request;
    }

    /**
     * Parse a module version from the given string version (formatted as
     * major.minor.revision).
     *
     * @param version
     * @return The formatted version string.
     */
    protected ModuleVersion ParseModuleVersion(String version) {
        ModuleVersion moduleVersion = new ModuleVersion();
        if (version != null) {
            StringTokenizer versionDetails = new StringTokenizer(version,MODULES_VERSION_SEPARATOR);
            int numTokens = versionDetails.countTokens();
            if (numTokens > 0) {
                moduleVersion.setMajor(versionDetails.nextToken());
            }
            if (numTokens > 1) {
                moduleVersion.setMinor(versionDetails.nextToken());
            }
            if (numTokens > 2) {
                moduleVersion.setRevision(versionDetails.nextToken());
            }
            
        }

        return moduleVersion;
    }

    /**
     * Gets the modules root path.
     *
     * @return
     */
    protected String GetModulesRootPath() {
        String ModulesPath = MODULES_PATH;
        if (_context != null && _context.getUser() != null && _context.getUser().length() > 0) {
            ModulesPath = new File(ModulesPath, _context.getUser()).getAbsolutePath();
        }
        return ModulesPath;
    }

    protected String GetLocalizedMessage(String key) {
        if (localizedMessages.containsKey(key)) {
            String localizedText = localizedMessages.get(key);
            return ((localizedText != null && localizedText.length() > 0 ? localizedText : key));
        } else {
            return key;
        }
    }

    /**
     * Gets the module location.
     * @param module
     * @param versioned
     * @return True to return location including version folder, False to return just the module location (for deleting tasks).
     */
    protected String GetModuleLocation(Module module, boolean versioned) {
        String modulePath = "";
        if (module != null) {
            if (versioned) {
                String moduleRelativePath = new File(module.getId(), module.getVersion().toString()).getAbsolutePath();
                modulePath = new File(this.GetModulesRootPath(), moduleRelativePath).getAbsolutePath();
            } else {
                modulePath = new File(this.GetModulesRootPath(), module.getId()).getAbsolutePath();
            }
        }
        return modulePath;
    }

    /// <summary>
    /// Updates the or install module.
    /// </summary>
    /// <param name="module">Module.</param>
    protected boolean UpdateOrInstallModule(Module module) {

        IOService service = this.GetServiceFromUrl(module.getLoadUrl());
        IORequest request = this.GetRequestWithRequiredHeaders();

        String tempFile = this.GetIOService().InvokeServiceForBinary(request, service, "tmp.zip");

        if (tempFile != null) {
            return this.StoreModuleZipFile(module, tempFile);
        } else {
            LOGGER.logWarning("UpdateOrInstallModule", "It was not possible to get module data from url: " + module.getLoadUrl());
        }

        return false;
    }

    public abstract IIo GetIOService();

    public abstract IFileSystem GetFileSystemService();

    public abstract INotification GetNotificationService();

    public abstract boolean StoreModuleZipFile(Module module, String tempFile);
}
