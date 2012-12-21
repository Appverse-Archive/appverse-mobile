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

import java.util.HashMap;
import java.util.Map;

public abstract class AbstractIO implements IIo {

    protected static final String SERVICES_CONFIG_FILE = "app/config/io-services-config.xml";
    protected static final int DEFAULT_RESPONSE_TIMEOUT = 10000;
    protected static Map<ServiceType, String> contentTypes = new HashMap<ServiceType, String>();
    protected IOServicesConfig servicesConfig = new IOServicesConfig();

    static {
        contentTypes.put(ServiceType.XMLRPC_JSON, "application/json");
        contentTypes.put(ServiceType.XMLRPC_XML, "text/xml");
        contentTypes.put(ServiceType.REST_JSON, "application/json");
        contentTypes.put(ServiceType.REST_XML, "text/xml");
        contentTypes.put(ServiceType.SOAP_JSON, "application/json");
        contentTypes.put(ServiceType.SOAP_XML, "text/xml");
        contentTypes.put(ServiceType.AMF_SERIALIZATION, "");
        contentTypes.put(ServiceType.REMOTING_SERIALIZATION, "");
        contentTypes.put(ServiceType.OCTET_BINARY, "application/octet-stream");
        contentTypes.put(ServiceType.GWT_RPC, "text/x-gwt-rpc; charset=utf-8");
    }

    public AbstractIO() {
    }

    @Override
    public IOService GetService(String name) {
        IOService service = null;

        IOService[] services = GetServices();
        if (services != null) {
            for (IOService currentService : services) {
                if (currentService.getName() != null
                        && currentService.getName().equals(name)) {
                    service = currentService;
                    break;
                }
            }
        }

        return service;
    }

    @Override
    public IOService GetService(String name, ServiceType type) {
        IOService service = null;

        IOService[] services = GetServices();
        if (services != null) {
            for (IOService currentService : services) {
                if (currentService.getName() != null
                        && currentService.getName().equals(name)
                        && currentService.getType() != null
                        && currentService.getType().equals(type)) {
                    service = currentService;
                    break;
                }
            }
        }

        return service;
    }

    @Override
    public IOService[] GetServices() {
        return servicesConfig.getServices();
    }

    @Override
    public abstract IOResponse InvokeService(IORequest request,
            IOService service);

    @Override
    public IOResponse InvokeService(IORequest request, String serviceName) {
        return InvokeService(request, GetService(serviceName));
    }

    @Override
    public IOResponse InvokeService(IORequest request, String serviceName,
            ServiceType type) {
        return InvokeService(request, GetService(serviceName, type));
    }

    @Override
    public abstract IOResponseHandle InvokeService(IORequest request,
            IOService service, IOResponseHandler handler);

    @Override
    public IOResponseHandle InvokeService(IORequest request,
            String serviceName, IOResponseHandler handler) {
        return InvokeService(request, GetService(serviceName), handler);
    }

    @Override
    public IOResponseHandle InvokeService(IORequest request,
            String serviceName, ServiceType type, IOResponseHandler handler) {
        return InvokeService(request, GetService(serviceName, type), handler);
    }
}
