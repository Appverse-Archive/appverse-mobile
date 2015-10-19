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
package com.gft.unity.core.io;

// TODO javadoc
public interface IIo {

    /**
     *
     * @param name
     * @return
     */
    public IOService GetService(String name);

    /**
     *
     * @param name
     * @param type
     * @return
     */
    public IOService GetService(String name, ServiceType type);

    /**
     *
     * @return
     */
    public IOService[] GetServices();

    /**
     *
     * @param request
     * @param service
     * @return
     */
    public IOResponse InvokeService(IORequest request, IOService service);

    /**
     *
     * @param request
     * @param serviceName
     * @return
     */
    public IOResponse InvokeService(IORequest request, String serviceName);

    /**
     *
     * @param request
     * @param serviceName
     * @param type
     * @return
     */
    public IOResponse InvokeService(IORequest request, String serviceName, ServiceType type);

    /**
     *
     * @param request
     * @param service
     * @param handler
     * @return
     */
    public IOResponseHandle InvokeService(IORequest request, IOService service, IOResponseHandler handler);

    /**
     *
     * @param request
     * @param serviceName
     * @param handler
     * @return
     */
    public IOResponseHandle InvokeService(IORequest request, String serviceName, IOResponseHandler handler);

    /**
     *
     * @param request
     * @param serviceName
     * @param type
     * @param handler
     * @return
     */
    public IOResponseHandle InvokeService(IORequest request, String serviceName, ServiceType type, IOResponseHandler handler);
    
    
    /**
     * Invokes a service for getting a big binary, storing it into filesystem and returning the reference url.
     * Only OCTET_BINARY service types are allowed.
     * @param request The request to be send.
     * @param service The service endpoint to call.
     * @param storePath The store path (realtive path under application Documents folder).
     * @return The reference Url for the stored file (if success, null otherwise.
     */
    public String InvokeServiceForBinary (IORequest request, IOService service, String storePath);
    
    /**
     * Clears the native cookie container
     */
    public void ClearCookieContainer();
}
