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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using Unity.Core.IO;
using UnityPlatformWindowsPhone.Internals;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhoneIO : AbstractIO, IAppverseService
    {
        private const string SERVICES_CONFIG_FILE = @"ms-appx:///Html/app/config/io-services-config.xml";
        private static readonly IDictionary<ServiceType, string> ContentTypes;
        private IOServicesConfig _servicesConfigObject;
        private const int ABSOLUTE_INVOKE_TIMEOUT = 60000;
        private const int DEFAULT_BUFFER_READ_SIZE = 4096;
        private const int DEFAULT_RESPONSE_TIMEOUT = 15000;
        private const int MAX_BINARY_SIZE = 8 * 1024 * 1024;


        static WindowsPhoneIO()
        {

            ContentTypes = new Dictionary<ServiceType, string>
            {
                {ServiceType.XMLRPC_JSON, "application/json"},
                {ServiceType.XMLRPC_XML, "text/xml"},
                {ServiceType.REST_JSON, "application/json"},
                {ServiceType.REST_XML, "text/xml"},
                {ServiceType.SOAP_JSON, "application/json"},
                {ServiceType.SOAP_XML, "text/xml"},
                {ServiceType.AMF_SERIALIZATION, String.Empty},
                {ServiceType.REMOTING_SERIALIZATION, String.Empty},
                {ServiceType.OCTET_BINARY, "application/octet-stream"},
                {ServiceType.GWT_RPC, "text/x-gwt-rpc; charset=utf-8"},
                {ServiceType.MULTIPART_FORM_DATA, "multipart/form-data; boundary="}
            };
        }

        public WindowsPhoneIO()
        {
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
            IOUserAgent = "Unity 1.0";
            var pendingTask = Task.Run(async () =>
            {
                _servicesConfigObject = await GetServicesConfig();
            });

            pendingTask.Wait();
        }

        public override async Task<string> GetDirectoryRoot()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }

        public override async Task<IOService[]> GetServices()
        {
            return _servicesConfigObject.Services.ToArray();
        }

        public override async Task<IOService> GetService(string name)
        {
            if (_servicesConfigObject == null) _servicesConfigObject = await GetServicesConfig();
            return _servicesConfigObject != null ? _servicesConfigObject.Services.First(serv => serv.Name.Equals(name)) : null;
        }

        public override async Task<IOService> GetService(string name, ServiceType type)
        {

            if (_servicesConfigObject == null) _servicesConfigObject = await GetServicesConfig();
            return _servicesConfigObject != null ? _servicesConfigObject.Services.First(serv => serv.Name.Equals(name) && serv.Type == type) : null;
        }

        public override async Task<IOResponse> InvokeService(IORequest request, IOService service)
        {
            var response = new IOResponse();
            var clientBaseConfig = WindowsPhoneUtils.CreateHttpClientOptions(!request.StopAutoRedirect);
            if (service != null)
            {
                if (service.Endpoint == null)
                {
                    WindowsPhoneUtils.Log("No endpoint configured for this service name: " + service.Name);
                    return null;
                }
                WindowsPhoneUtils.Log("Request content: " + request.Content);

                var reqMethod = service.RequestMethod.ToString(); // default is POST
                if (!string.IsNullOrEmpty(request.Method)) reqMethod = request.Method.ToUpper();

                var requestUriString = FormatRequestUriString(request, service, reqMethod);
                HttpRequestMessage webReq = null;
                try
                {
                    webReq = await BuildWebRequest(request, service, requestUriString, reqMethod);
                    using (var client = new HttpClient(clientBaseConfig))
                    {
                        //ONLY DEFINE PROPERTIES ONCE, IF A REQUEST HAS BEEN SENT, HTTPCLIENT CANNOT MODIFY PROPERTIES
                        client.DefaultRequestHeaders.TryAppendWithoutValidation("User-Agent", IOUserAgent);
                        var cts = new CancellationTokenSource(new TimeSpan(0, 0, DEFAULT_RESPONSE_TIMEOUT));
                        var webResp = await client.SendRequestAsync(webReq).AsTask(cts.Token);
                        WindowsPhoneUtils.Log("getting response... ");
                        response = await ReadWebResponse(webReq, webResp, service);
                    }
                }
                catch (Exception ex)
                {
                    //Certificate errors
                    if ((ex.HResult & 65535) == 12045)
                    {
                        if (webReq != null)
                        {
                            var certErrors = webReq.TransportInformation.ServerCertificateErrors;
                        }
                    }
                    WindowsPhoneUtils.Log("Exception requesting service: " + requestUriString + "." + ex.Message);
                    response.ContentType = ContentTypes[ServiceType.REST_JSON];
                    response.Content = "Exception Requesting Service: " + requestUriString + ". Message: " + ex.Message;
                }
            }
            else
            {
                WindowsPhoneUtils.Log("Null service received for invoking.");
            }
            return response;
        }

        public override async Task<IOResponse> InvokeService(IORequest request, string serviceName)
        {
            return await InvokeService(request, await GetService(serviceName));
        }

        public override async Task<IOResponse> InvokeService(IORequest request, string serviceName, ServiceType type)
        {
            return await InvokeService(request, await GetService(serviceName, type));
        }

        public override async Task<IOResponseHandle> InvokeService(IORequest request, IOService service, IOResponseHandler handler)
        {
            throw new NotImplementedException();
        }

        public override async Task<IOResponseHandle> InvokeService(IORequest request, string serviceName, IOResponseHandler handler)
        {
            throw new NotImplementedException();
        }

        public override async Task<IOResponseHandle> InvokeService(IORequest request, string serviceName, ServiceType type, IOResponseHandler handler)
        {
            throw new NotImplementedException();
        }

        public override async Task<string> InvokeServiceForBinary(IORequest request, IOService service, string storePath)
        {
            throw new NotImplementedException();
        }

        private async Task<IOServicesConfig> GetServicesConfig()
        {
            if (_servicesConfigObject != null) return _servicesConfigObject;
            var IOConfigStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(SERVICES_CONFIG_FILE));
            var serializer = new XmlSerializer(typeof(IOServicesConfig));
            var reader = XmlReader.Create(await IOConfigStorageFile.OpenStreamForReadAsync());
            _servicesConfigObject = (IOServicesConfig)serializer.Deserialize(reader);
            return _servicesConfigObject;
        }

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }

        private async Task<IOResponse> ReadWebResponse(HttpRequestMessage webRequest, HttpResponseMessage webResponse, IOService service)
        {
            var response = new IOResponse();

            // result types (string or byte array)
            byte[] resultBinary = null;
            string result = null;

            var responseMimeTypeOverride = webResponse.Content.Headers.ContentType.MediaType;

            using (var stream = (await webResponse.Content.ReadAsInputStreamAsync()).AsStreamForRead())
            {
                WindowsPhoneUtils.Log("getting response stream...");
                if (ServiceType.OCTET_BINARY.Equals(service.Type))
                {
                    var lengthContent = (int)webResponse.Content.Headers.ContentLength;
                    // testing log line
                    // SystemLogger.Log (SystemLogger.Module.CORE, "content-length header: " + lengthContent +", max file size: " + MAX_BINARY_SIZE);
                    var bufferReadSize = DEFAULT_BUFFER_READ_SIZE;
                    if (lengthContent >= 0 && lengthContent <= bufferReadSize)
                    {
                        bufferReadSize = lengthContent;
                    }

                    if (lengthContent > MAX_BINARY_SIZE)
                    {
                        WindowsPhoneUtils.Log("WARNING! - file exceeds the maximum size defined in platform (" + MAX_BINARY_SIZE + " bytes)");
                    }
                    else
                    {
                        // Read to end of stream in blocks
                        WindowsPhoneUtils.Log("buffer read: " + bufferReadSize + " bytes");
                        var memBuffer = new MemoryStream();
                        var readBuffer = new byte[bufferReadSize];
                        int readLen;
                        do
                        {
                            //readLen = stream.Read(readBuffer, 0, readBuffer.Length);
                            readLen = await stream.ReadAsync(readBuffer, 0, readBuffer.Length);
                            memBuffer.Write(readBuffer, 0, readLen);
                        } while (readLen > 0);

                        resultBinary = memBuffer.ToArray();
                        memBuffer.Flush();
                    }
                }
                else
                {
                    WindowsPhoneUtils.Log("reading response content...");
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                        WindowsPhoneUtils.Log("Response Content: " + result);
                    }
                }
            }

            /*************
             * CACHE
             ************

            // preserve cache-control header from remote server, if any
            var cacheControlHeader = (webResponse.Headers.CacheControl != null) ? webResponse.Headers.CacheControl.ToString() : String.Empty;
            if (!String.IsNullOrWhiteSpace(cacheControlHeader))
            {
                WindowsPhoneUtils.Log("Found Cache-Control header on response: " + cacheControlHeader + ", using it on internal response...");
                if (response.Headers == null)
                {
                    response.Headers = new IOHeader[1];
                }
                var cacheHeader = new IOHeader { Name = "Cache-Control", Value = cacheControlHeader };
                response.Headers[0] = cacheHeader;
            }
            */
            /*************
             * HEADERS HANDLING
             *************/
            if (webResponse.Headers != null)
            {
                response.Headers = new IOHeader[webResponse.Headers.Count];

                var size = 0;
                foreach (var headerKey in webResponse.Headers.Keys)
                {
                    string headerValue;
                    webResponse.Headers.TryGetValue(headerKey, out headerValue);
                    var objHeader = new IOHeader { Name = headerKey, Value = headerValue };
                    response.Headers[size++] = objHeader;
                }
            }


            /*************
             * COOKIES HANDLING
             *************/

            // get response cookies (stored on cookiecontainer)
            if (response.Session == null)
            {
                response.Session = new IOSessionContext();
            }

            var filter = new HttpBaseProtocolFilter();
            var cookieCollection = filter.CookieManager.GetCookies(webRequest.RequestUri);

            var myCookies = new List<IOCookie>();

            foreach (var cookieFound in cookieCollection)
            {
                var value = cookieFound.ToString().Replace(String.Concat(cookieFound.Name, "="), String.Empty);
                WindowsPhoneUtils.Log("Found cookie on response: " + cookieFound.Name + "=" + value);
                if (cookieFound.ToString().Split(new string[] { "Path=" }, StringSplitOptions.None).Length > 2 || cookieFound.ToString().Split(new string[] { "Domain=" }, StringSplitOptions.None).Length > 2) continue;
                WindowsPhoneUtils.Log("Added cookie to response: " + cookieFound.Name + "=" + value);
                var cookie = new IOCookie { Name = cookieFound.Name, Value = value };
                myCookies.Add(cookie);
            }
            response.Session.Cookies = myCookies.ToArray();

            if (ServiceType.OCTET_BINARY.Equals(service.Type))
            {
                if (responseMimeTypeOverride != null && !responseMimeTypeOverride.Equals(ContentTypes[service.Type]))
                {
                    response.ContentType = responseMimeTypeOverride;
                }
                else
                {
                    response.ContentType = ContentTypes[service.Type];
                }
                response.ContentBinary = resultBinary; // Assign binary content here
            }
            else
            {
                response.ContentType = ContentTypes[service.Type];
                response.Content = result;
            }

            return response;
        }

        private async Task<HttpRequestMessage> BuildWebRequest(IORequest request, IOService service, string requestUriString, string reqMethod)
        {
            var _clientBaseConfig = WindowsPhoneUtils.CreateHttpClientOptions(true);
            var requestHttpMethod = HttpMethod.Post;
            HttpRequestMessage webRequest = null;

            try
            {
                if (service.Type == ServiceType.MULTIPART_FORM_DATA)
                {
                    webRequest = await BuildMultipartWebRequest(request, service, requestUriString);
                }
                else
                {
                    switch (service.RequestMethod)
                    {
                        case RequestMethod.GET:
                            requestHttpMethod = HttpMethod.Get;
                            break;
                    }

                    if (!String.IsNullOrWhiteSpace(request.Method))
                        switch (request.Method.ToLower())
                        {
                            case "get":
                                requestHttpMethod = HttpMethod.Get;
                                break;
                            case "delete":
                                requestHttpMethod = HttpMethod.Delete;
                                break;
                            case "head":
                                requestHttpMethod = HttpMethod.Head;
                                break;
                            case "options":
                                requestHttpMethod = HttpMethod.Options;
                                break;
                            case "patch":
                                requestHttpMethod = HttpMethod.Patch;
                                break;
                            case "put":
                                requestHttpMethod = HttpMethod.Put;
                                break;
                            default:
                                requestHttpMethod = HttpMethod.Post;
                                break;

                        }

                    webRequest = new HttpRequestMessage(requestHttpMethod, new Uri(requestUriString));

                    // check specific request ContentType defined, and override service type in that case
                    // check specific request ContentType defined, and override service type in that case
                    if (requestHttpMethod == HttpMethod.Get)
                    {
                        webRequest.RequestUri =
                            new Uri(String.Concat(service.Endpoint.Host, service.Endpoint.Path, request.Content),
                                UriKind.RelativeOrAbsolute);
                    }
                    else
                    {
                        if (webRequest.Content == null && request.Content != null)
                            webRequest.Content = new HttpBufferContent(request.GetRawContent().AsBuffer());
                    }

                    var sContentType = request.ContentType;

                    if (String.IsNullOrWhiteSpace(sContentType))
                    {
                        sContentType = ContentTypes[service.Type];
                    }

                    WindowsPhoneUtils.Log("Request method: " + webRequest.Method);

                    webRequest.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue(sContentType));

                    if (webRequest.Method == HttpMethod.Post && request.Content != null)
                    {
                        webRequest.Content.Headers.ContentLength = (ulong?)request.GetContentLength();
                        WindowsPhoneUtils.Log("Request content length: " + request.GetContentLength());
                    }
                    else
                    {
                        webRequest.Content = new HttpStringContent("");
                    }
                    WindowsPhoneUtils.Log("webRequest content length: " + webRequest.Content.Headers.ContentLength);
                    webRequest.Content.Headers.ContentType = new HttpMediaTypeHeaderValue(sContentType);
                    webRequest.Headers.Add("Keep-Alive", "true");
                }
                if (webRequest == null) return null;
                WindowsPhoneUtils.Log("Request UserAgent : " + IOUserAgent);

                /*************
            * HEADERS HANDLING
            *************/

                // add specific headers to the request
                if (request.Headers != null && request.Headers.Length > 0)
                {
                    foreach (var header in request.Headers)
                    {
                        webRequest.Headers.Add(header.Name, header.Value);
                        WindowsPhoneUtils.Log("Added request header: " + header.Name + "=" +
                                              webRequest.Headers[header.Name]);
                    }
                }

                /*************
            * COOKIES HANDLING
            *************/

                // Assign the cookie container on the request to hold cookie objects that are sent on the response.
                // Required even though you no cookies are send.
                //webReq.CookieContainer = this.cookieContainer;

                // add cookies to the request cookie container
                if (request.Session != null && request.Session.Cookies != null && request.Session.Cookies.Length > 0)
                {
                    foreach (
                        var cookie in request.Session.Cookies.Where(cookie => !String.IsNullOrWhiteSpace(cookie.Name)))
                    {
                        _clientBaseConfig.CookieManager.SetCookie(new HttpCookie(cookie.Name, new Uri(requestUriString).Host, "/") { Value = cookie.Value });
                        WindowsPhoneUtils.Log("Added cookie [" + cookie.Name + "] to request.");
                    }
                }
                WindowsPhoneUtils.Log("HTTP Request cookies: " +
                                      _clientBaseConfig.CookieManager.GetCookies(new Uri(requestUriString)).Count);
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log("INVOKE SERVICE ERROR:" + ex.Message);
            }


            /*************
             * SETTING A PROXY (ENTERPRISE ENVIRONMENTS)
             *************/
            /*
            if (service.Endpoint.ProxyUrl != null)
            {
            WebProxy myProxy = new WebProxy();
            Uri proxyUri = new Uri(service.Endpoint.ProxyUrl);
            myProxy.Address = proxyUri;
            webReq.Proxy = myProxy;
            }
            */
            return webRequest;
        }

        private async Task<HttpRequestMessage> BuildMultipartWebRequest(IORequest request, IOService service, string requestUriString)
        {

            var webRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(requestUriString));
            webRequest.Headers.Add("Keep-Alive", "true");
            var sBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
            var multipartFormDataContent = new HttpMultipartFormDataContent(sBoundary);

            if (request.Content != null)
            {
                var items = request.Content.TrimEnd('&').Split('&');
                foreach (var keyValue in items.Select(item => item.Split('=')))
                {
                    multipartFormDataContent.Add(new HttpStringContent(keyValue[1]), keyValue[0]);
                }
            }
            webRequest.Content = multipartFormDataContent;
            if (request.Attachment == null) return webRequest;
            foreach (var attachData in request.Attachment)
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(attachData.ReferenceUrl))
                    {
                        var attachmentFile = await WindowsPhoneUtils.GetFileFromReferenceURL(attachData.ReferenceUrl);
                        if (attachmentFile != null)
                        {
                            multipartFormDataContent.Add(new HttpStreamContent(await attachmentFile.OpenReadAsync()), attachData.FormFieldKey, attachData.FileName ?? attachmentFile.Name);
                        }
                        else
                        {
                            WindowsPhoneUtils.Log("**** [MULTIPART_FORM_DATA]. # File from referenced URL: " + attachData.ReferenceUrl + ", does not exists in filesystem, please check provided ReferenceUrl!");
                        }
                    }
                    else
                    {
                        WindowsPhoneUtils.Log("**** [MULTIPART_FORM_DATA]. # Attached file does not contain a valid ReferenceUrl field. IGNORED");
                    }
                }
                catch (Exception ex)
                {
                    WindowsPhoneUtils.Log(ex.Message);
                }
            }
            return webRequest;
        }

        private string FormatRequestUriString(IORequest request, IOService service, string reqMethod)
        {
            var requestUriString = String.Format("{0}:{1}{2}", service.Endpoint.Host, service.Endpoint.Port, service.Endpoint.Path);
            if (service.Endpoint.Port == 0)
            {
                requestUriString = String.Format("{0}{1}", service.Endpoint.Host, service.Endpoint.Path);
            }

            if (reqMethod.Equals(RequestMethod.GET.ToString()) && request.Content != null)
            {
                // add request content to the URI string when NOT POST method (GET, PUT, DELETE).
                requestUriString += request.Content;
            }

            WindowsPhoneUtils.Log("Requesting service: " + requestUriString);
            return requestUriString;
        }

        public new string IOUserAgent { get; private set; }
    }
}
