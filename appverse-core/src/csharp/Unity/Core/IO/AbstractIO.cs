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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Unity.Core.System;
using System.Threading;
#if !WP8
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
#else
using System.Threading.Tasks;
#endif
using System.Collections;


namespace Unity.Core.IO
{
    public abstract class AbstractIO : IIo
    {
        private static string SERVICES_CONFIG_FILE = "app/config/io-services-config.xml";
        private static int ABSOLUTE_INVOKE_TIMEOUT = 60000; // 60 seconds
        private static int DEFAULT_READWRITE_TIMEOUT = 15000; // 15 seconds
        private static int DEFAULT_RESPONSE_TIMEOUT = 100000; // 100 seconds
        private static int MAX_BINARY_SIZE = 8 * 1024 * 1024;  // 8 MB
        private static int DEFAULT_BUFFER_READ_SIZE = 4096;	// 4 KB
        private IOServicesConfig servicesConfig = new IOServicesConfig();  // empty list
        private static IDictionary<ServiceType, string> contentTypes = new Dictionary<ServiceType, string>();
        private CookieContainer cookieContainer = null;



        static AbstractIO()
        {
            contentTypes[ServiceType.XMLRPC_JSON] = "application/json";
            contentTypes[ServiceType.XMLRPC_XML] = "text/xml";
            contentTypes[ServiceType.REST_JSON] = "application/json";
            contentTypes[ServiceType.REST_XML] = "text/xml";
            contentTypes[ServiceType.SOAP_JSON] = "application/json";
            contentTypes[ServiceType.SOAP_XML] = "text/xml";
            contentTypes[ServiceType.AMF_SERIALIZATION] = "";
            contentTypes[ServiceType.REMOTING_SERIALIZATION] = "";
            contentTypes[ServiceType.OCTET_BINARY] = "application/octet-stream";
            contentTypes[ServiceType.GWT_RPC] = "text/x-gwt-rpc; charset=utf-8";
        }

        private string _servicesConfigFile = SERVICES_CONFIG_FILE;
        private string _IOUserAgent = "Unity 1.0";

        public virtual string IOUserAgent
        {
            get
            {
                return this._IOUserAgent;
            }
            set
            {
                this._IOUserAgent = value;
            }
        }

        public virtual string ServicesConfigFile
        {
            get
            {
                return this._servicesConfigFile;
            }
            set
            {
                this._servicesConfigFile = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AbstractIO()
        {
            LoadServicesConfig();
            this.cookieContainer = new CookieContainer();
        }

        protected void removeFingerprints()
        {
            IOService[] services = servicesConfig.Services.ToArray();
            if (services != null)
            {
                foreach (IOService serv in services)
                {
                    serv.Endpoint.Fingerprint = null;
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        protected void LoadServicesConfig()
        {
            try
            {   // FileStream to read the XML document.
                byte[] configFileRawData = GetConfigFileBinaryData();
                if (configFileRawData != null)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IOServicesConfig));
                    servicesConfig = (IOServicesConfig)serializer.Deserialize(new MemoryStream(configFileRawData));
                }
            }
            catch (Exception e)
            {
                SystemLogger.Log(SystemLogger.Module.CORE, "Error when loading services configuration", e);
                servicesConfig = new IOServicesConfig(); // reset services config mapping when the services could not be loaded for any reason
            }
        }

#if !WP8
        public abstract String GetDirectoryRoot();

        /// <summary>
        /// Default method, to be overrided by platform implementation. 
        /// </summary>
        /// <returns>
        /// A <see cref="Stream"/>
        /// </returns>
        public virtual byte[] GetConfigFileBinaryData()
        {
            SystemLogger.Log(SystemLogger.Module.CORE, "# Loading IO Services Configuration from file: " + ServicesConfigFile);

            Stream fs = new FileStream(ServicesConfigFile, FileMode.Open);
            if (fs != null)
            {
                return ((MemoryStream)fs).GetBuffer();
            }
            else
            {
                return null;
            }
        }
#endif

        #region Miembros de IIo
#if !WP8
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IOService[] GetServices ()
		{
			return servicesConfig.Services.ToArray ();
		}



		/// <summary>
		/// Get the IO Service that matches the given name.
		/// </summary>
		/// <param name="name">Service name to match.</param>
		/// <returns>IO Service.</returns>
		public IOService GetService (string name)
		{
			IOService service = null;
			IOService[] services = GetServices ();
			if (services != null) {
				foreach (IOService serv in services) {
					if (serv.Name == name) {
						service = serv;
						break;
					}
				}
			}
			return service;
		}

		/// <summary>
		/// Get the IO Service that matches the given name and type.
		/// </summary>
		/// <param name="name">Service name to match.</param>
		/// <param name="type">Service type to match.</param>
		/// <returns>IO Service.</returns>
		public IOService GetService (string name, ServiceType type)
		{
			IOService service = null;
			IOService[] services = GetServices ();
			if (services != null) {
				foreach (IOService serv in services) {
					if (serv.Name == name && serv.Type == type) {
						service = serv;
						break;
					}
				}
			}
			return service;
		}

		/// <summary>
		/// Callback to accept all certificates
		/// </summary>
		/// <returns>
		/// <c>true</c>, if web certificates was validated, <c>false</c> otherwise.
		/// </returns>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='certificate'>
		/// Certificate.
		/// </param>
		/// <param name='chain'>
		/// Chain.
		/// </param>
		/// <param name='sslPolicyErrors'>
		/// Ssl policy errors.
		/// </param>
		public virtual bool ValidateWebCertificates (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			SystemLogger.Log (SystemLogger.Module .CORE, "*************** On ServerCertificateValidationCallback: accept all certificates");
			return true;
		}

		/// <summary>
		/// Checks the invoke timeout.
		/// </summary>
		/// <param name='requestObject'>
		/// Request object.
		/// </param>
		private void CheckInvokeTimeout(object requestObject) {

			Thread.Sleep(ABSOLUTE_INVOKE_TIMEOUT);
			SystemLogger.Log (SystemLogger.Module .CORE, "Absolute timeout checking completed.");
			HttpWebRequest req = requestObject as HttpWebRequest;
			if(req != null) {
				SystemLogger.Log (SystemLogger.Module .CORE, "Aborting request...");
				req.Abort(); 
				// this causes a WebException with the Status property set to RequestCanceled
				// for any subsequent call to the GetResponse, BeginGetResponse, EndGetResponse, GetRequestStream, BeginGetRequestStream, or EndGetRequestStream methods.
			}
		}


		private string FormatRequestUriString(IORequest request, IOService service, string reqMethod) {

			string requestUriString = String.Format ("{0}:{1}{2}", service.Endpoint.Host, service.Endpoint.Port, service.Endpoint.Path);
			if (service.Endpoint.Port == 0) 
			{
				requestUriString = String.Format ("{0}{1}", service.Endpoint.Host, service.Endpoint.Path);
			}
			
			if (reqMethod.Equals(RequestMethod.GET.ToString()) && request.Content != null)
			{
				// add request content to the URI string when NOT POST method (GET, PUT, DELETE).
				requestUriString += request.Content;
			}
			
			SystemLogger.Log (SystemLogger.Module .CORE, "Requesting service: " + requestUriString);
			return requestUriString;
		}

		private HttpWebRequest BuildWebRequest(IORequest request, IOService service, string requestUriString, string reqMethod) {

			HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create (requestUriString);
			webReq.Method = reqMethod; // default is POST
			webReq.ContentType = contentTypes [service.Type];
			
			// check specific request ContentType defined, and override service type in that case
			if (request.ContentType != null && request.ContentType.Length > 0) 
			{
				webReq.ContentType = request.ContentType;
			}
			
			SystemLogger.Log (SystemLogger.Module.CORE, "Request content type: " + webReq.ContentType);
			SystemLogger.Log (SystemLogger.Module.CORE, "Request method: " + webReq.Method);
			
			webReq.Accept = webReq.ContentType; // setting "Accept" header with the same value as "Content Type" header, it is needed to be defined for some services.
			webReq.ContentLength = request.GetContentLength ();
			SystemLogger.Log (SystemLogger.Module.CORE, "Request content length: " + webReq.ContentLength);
			webReq.Timeout = DEFAULT_RESPONSE_TIMEOUT; // in millisecods (default is 100 seconds)
			webReq.ReadWriteTimeout = DEFAULT_READWRITE_TIMEOUT; // in milliseconds
			webReq.KeepAlive = false;
			webReq.ProtocolVersion = HttpVersion.Version10;
			if (request.ProtocolVersion == HTTPProtocolVersion.HTTP11) webReq.ProtocolVersion = HttpVersion.Version11;

            // [MOBPLAT-200] ... Allow Gzip or Deflate decompression methods
            webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			
			// user agent needs to be informed - some servers check this parameter and send 500 errors when not informed.
			webReq.UserAgent = this.IOUserAgent;
			SystemLogger.Log (SystemLogger.Module.CORE, "Request UserAgent : " + webReq.UserAgent);

			/*************
			 * HEADERS HANDLING
			 *************/

			// add specific headers to the request
			if (request.Headers != null && request.Headers.Length > 0) {
				foreach (IOHeader header in request.Headers) {
					webReq.Headers.Add (header.Name, header.Value);
					SystemLogger.Log (SystemLogger.Module.CORE, "Added request header: " + header.Name + "=" + webReq.Headers.Get (header.Name));
				}
			}

			/*************
			 * COOKIES HANDLING
			 *************/

			// Assign the cookie container on the request to hold cookie objects that are sent on the response.
			// Required even though you no cookies are send.
			webReq.CookieContainer = this.cookieContainer;
			
			// add cookies to the request cookie container
			if (request.Session != null && request.Session.Cookies != null && request.Session.Cookies.Length > 0) {
				foreach (IOCookie cookie in request.Session.Cookies) {
					if (cookie != null && cookie.Name != null) {
						webReq.CookieContainer.Add (webReq.RequestUri, new Cookie (cookie.Name, cookie.Value));
						SystemLogger.Log (SystemLogger.Module.CORE, "Added cookie [" + cookie.Name + "] to request.");
					}
				}
			}
			SystemLogger.Log (SystemLogger.Module.CORE, "HTTP Request cookies: " + webReq.CookieContainer.GetCookieHeader (webReq.RequestUri));
			
			/*************
			 * SETTING A PROXY (ENTERPRISE ENVIRONMENTS)
			 *************/

			if (service.Endpoint.ProxyUrl != null) {
				WebProxy myProxy = new WebProxy ();
				Uri proxyUri = new Uri (service.Endpoint.ProxyUrl);
				myProxy.Address = proxyUri;
				webReq.Proxy = myProxy;
			}

			return webReq;
		}

        /*
        private static Stream GetStreamForResponse(HttpWebResponse webResponse)
        {
            Stream stream;
            switch (webResponse.ContentEncoding.ToUpperInvariant())
            {
                case "GZIP":
                    stream = new GZipStream(webResponse.GetResponseStream(), CompressionMode.Decompress);
                    break;
                case "DEFLATE":
                    stream = new DeflateStream(webResponse.GetResponseStream(), CompressionMode.Decompress);
                    break;

                default:
                    stream = webResponse.GetResponseStream();
                    //stream.ReadTimeout = readTimeOut;
                    break;
            }
            return stream;
        }
        */

		private IOResponse ReadWebResponse(HttpWebRequest webRequest, HttpWebResponse webResponse, IOService service) {
			IOResponse response = new IOResponse ();

			// result types (string or byte array)
			byte[] resultBinary = null;
			string result = null;

			string responseMimeTypeOverride = webResponse.GetResponseHeader ("Content-Type");

			using (Stream stream = webResponse.GetResponseStream()) {
				SystemLogger.Log (SystemLogger.Module.CORE, "getting response stream...");
				if (ServiceType.OCTET_BINARY.Equals (service.Type)) {
					
					int lengthContent = -1;
					if (webResponse.GetResponseHeader ("Content-Length") != null && webResponse.GetResponseHeader ("Content-Length") != "") {
						lengthContent = Int32.Parse (webResponse.GetResponseHeader ("Content-Length"));
					}
					// testing log line
					// SystemLogger.Log (SystemLogger.Module.CORE, "content-length header: " + lengthContent +", max file size: " + MAX_BINARY_SIZE);
					int bufferReadSize = DEFAULT_BUFFER_READ_SIZE;
					if (lengthContent >= 0 && lengthContent<=bufferReadSize) {
						bufferReadSize = lengthContent;
					}
					
					if(lengthContent>MAX_BINARY_SIZE) {
						SystemLogger.Log (SystemLogger.Module.CORE, 
						                  "WARNING! - file exceeds the maximum size defined in platform (" + MAX_BINARY_SIZE+ " bytes)");
					} else {
						// Read to end of stream in blocks
						SystemLogger.Log (SystemLogger.Module.CORE, "buffer read: " + bufferReadSize + " bytes");
						MemoryStream memBuffer = new MemoryStream ();
						byte[] readBuffer = new byte[bufferReadSize];
						int readLen = 0;
						do {
							readLen = stream.Read (readBuffer, 0, readBuffer.Length);
							memBuffer.Write (readBuffer, 0, readLen);
						} while (readLen >0);
						
						resultBinary = memBuffer.ToArray ();
						memBuffer.Close ();
						memBuffer = null;
					}
				} else {
					SystemLogger.Log (SystemLogger.Module.CORE, "reading response content...");
					using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
						result = reader.ReadToEnd ();
					}
				}
			}

			/*************
			 * CACHE
			 *************/

			// preserve cache-control header from remote server, if any
			string cacheControlHeader = webResponse.GetResponseHeader ("Cache-Control");
			if (cacheControlHeader != null && cacheControlHeader != "") {
				SystemLogger.Log (SystemLogger.Module.CORE, "Found Cache-Control header on response: " + cacheControlHeader + ", using it on internal response...");
				if(response.Headers == null) {
					response.Headers = new IOHeader[1];
				}
				IOHeader cacheHeader = new IOHeader();
				cacheHeader.Name = "Cache-Control";
				cacheHeader.Value = cacheControlHeader;
				response.Headers[0] = cacheHeader;
			}

			/*************
			 * COOKIES HANDLING
			 *************/

			// get response cookies (stored on cookiecontainer)
			if (response.Session == null) {
				response.Session = new IOSessionContext ();
				
			}
			response.Session.Cookies = new IOCookie[this.cookieContainer.Count];
			IEnumerator enumerator = this.cookieContainer.GetCookies (webRequest.RequestUri).GetEnumerator ();
			int i = 0;
			while (enumerator.MoveNext()) {
				Cookie cookieFound = (Cookie)enumerator.Current;
				SystemLogger.Log (SystemLogger.Module.CORE, "Found cookie on response: " + cookieFound.Name + "=" + cookieFound.Value);
				IOCookie cookie = new IOCookie ();
				cookie.Name = cookieFound.Name;
				cookie.Value = cookieFound.Value;
				response.Session.Cookies [i] = cookie;
				i++;
			}

			if (ServiceType.OCTET_BINARY.Equals (service.Type)) {
				if (responseMimeTypeOverride != null && !responseMimeTypeOverride.Equals (contentTypes [service.Type])) {
					response.ContentType = responseMimeTypeOverride;
				} else {
					response.ContentType = contentTypes [service.Type];
				}
				response.ContentBinary = resultBinary; // Assign binary content here
			} else {
				response.ContentType = contentTypes [service.Type];
				response.Content = result;
			}


			return response;

		}

		private string ReadWebResponseAndStore(HttpWebRequest webRequest, HttpWebResponse webResponse, IOService service, string storePath) {

			using (Stream stream = webResponse.GetResponseStream()) {
				SystemLogger.Log (SystemLogger.Module.CORE, "getting response stream...");

				int lengthContent = -1;
				if (webResponse.GetResponseHeader ("Content-Length") != null && webResponse.GetResponseHeader ("Content-Length") != "") {
					lengthContent = Int32.Parse (webResponse.GetResponseHeader ("Content-Length"));
				}
				// testing log line
				// SystemLogger.Log (SystemLogger.Module.CORE, "content-length header: " + lengthContent +", max file size: " + MAX_BINARY_SIZE);
				int bufferReadSize = DEFAULT_BUFFER_READ_SIZE;
				if (lengthContent >= 0 && lengthContent<=bufferReadSize) {
					bufferReadSize = lengthContent;
				}
				SystemLogger.Log (SystemLogger.Module.CORE, "buffer read: " + bufferReadSize + " bytes");
				string fullStorePath = Path.Combine(this.GetDirectoryRoot (), storePath);
				SystemLogger.Log (SystemLogger.Module.CORE, "storing file at: " + fullStorePath);
				FileStream streamWriter = new FileStream (fullStorePath, FileMode.Create);

				byte[] readBuffer = new byte[bufferReadSize];
				int readLen = 0;
				int totalRead = 0;
				do {
					readLen = stream.Read (readBuffer, 0, readBuffer.Length);
					streamWriter.Write (readBuffer, 0, readLen);
					totalRead = totalRead + readLen;
				} while (readLen >0);


				SystemLogger.Log (SystemLogger.Module.CORE, "total bytes: " + totalRead);
				streamWriter.Close ();
				streamWriter = null;

				return storePath;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		public virtual IOResponse InvokeService (IORequest request, IOService service)
		{
			IOResponse response = new IOResponse ();

			if (service != null) {

				if (service.Endpoint == null) {
					SystemLogger.Log (SystemLogger.Module .CORE, "No endpoint configured for this service name: " + service.Name);
					return response;
				} 

				SystemLogger.Log (SystemLogger.Module .CORE, "Request content: " + request.Content);
				byte[] requestData = request.GetRawContent ();
                
				String reqMethod = service.RequestMethod.ToString(); // default is POST
                if (request.Method != null && request.Method != String.Empty) reqMethod = request.Method.ToUpper();

				String requestUriString = this.FormatRequestUriString(request, service, reqMethod);
				Thread timeoutThread = null;

				try {

					// Security - VALIDATIONS
					ServicePointManager.ServerCertificateValidationCallback = ValidateWebCertificates;

					// Building Web Request to send
					HttpWebRequest webReq = this.BuildWebRequest(request, service, requestUriString, reqMethod);
					
					// Throw a new Thread to check absolute timeout
					timeoutThread = new Thread(CheckInvokeTimeout);
					timeoutThread.Start(webReq);

					// POSTING DATA using timeout
                    if (!reqMethod.Equals(RequestMethod.GET.ToString()) && requestData != null)
                    {
						// send data only for POST method.
						SystemLogger.Log (SystemLogger.Module.CORE, "Sending data on the request stream... (POST)");
						SystemLogger.Log (SystemLogger.Module.CORE, "request data length: " + requestData.Length);
						using (Stream requestStream = webReq.GetRequestStream()) {
							SystemLogger.Log (SystemLogger.Module.CORE, "request stream: " + requestStream);
							requestStream.Write (requestData, 0, requestData.Length);
						}
					}
	
					using (HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse()) {

						SystemLogger.Log (SystemLogger.Module.CORE, "getting response...");
						response = this.ReadWebResponse(webReq, webResp, service);
					}

				} catch (WebException ex) {
					SystemLogger.Log (SystemLogger.Module .CORE, "WebException requesting service: " + requestUriString + ".", ex);
					response.ContentType = contentTypes [ServiceType.REST_JSON];
					response.Content = "WebException Requesting Service: " + requestUriString + ". Message: " + ex.Message;
				} catch (Exception ex) {
					SystemLogger.Log (SystemLogger.Module .CORE, "Unnandled Exception requesting service: " + requestUriString + ".", ex);
					response.ContentType = contentTypes [ServiceType.REST_JSON];
					response.Content = "Unhandled Exception Requesting Service: " + requestUriString + ". Message: " + ex.Message;
				} finally {
					// abort any previous timeout checking thread
					if(timeoutThread!=null && timeoutThread.IsAlive) {
						timeoutThread.Abort();
					}
				}
			} else {
				SystemLogger.Log (SystemLogger.Module .CORE, "Null service received for invoking.");
			}

			return response;
		}

		/// <summary>
		/// Invokes a service for getting a big binary, storing it into filesystem and returning the reference url.
		/// Only OCTET_BINARY service types are allowed.
		/// </summary>
		/// <returns>The reference Url for the stored file (if success, null otherwise.</returns>
		/// <param name="request">Request.</param>
		/// <param name="service">Service.</param>
		/// <param name="storePath">Store path.</param>
		public virtual string InvokeServiceForBinary (IORequest request, IOService service, string storePath) {

			if (service != null) {
				
				if (service.Endpoint == null) {
					SystemLogger.Log (SystemLogger.Module .CORE, "No endpoint configured for this service name: " + service.Name);
					return null;
				} 

				if (!ServiceType.OCTET_BINARY.Equals (service.Type)) {
					SystemLogger.Log (SystemLogger.Module .CORE, "This method only admits OCTET_BINARY service types");
					return null;
				}
				
				SystemLogger.Log (SystemLogger.Module .CORE, "Request content: " + request.Content);
				byte[] requestData = request.GetRawContent ();
				
				String reqMethod = service.RequestMethod.ToString(); // default is POST
				if (request.Method != null && request.Method != String.Empty) reqMethod = request.Method.ToUpper();
				
				String requestUriString = this.FormatRequestUriString(request, service, reqMethod);
				Thread timeoutThread = null;
				
				try {
					
					// Security - VALIDATIONS
					ServicePointManager.ServerCertificateValidationCallback = ValidateWebCertificates;
					
					// Building Web Request to send
					HttpWebRequest webReq = this.BuildWebRequest(request, service, requestUriString, reqMethod);
					
					// Throw a new Thread to check absolute timeout
					timeoutThread = new Thread(CheckInvokeTimeout);
					timeoutThread.Start(webReq);
					
					// POSTING DATA using timeout
					if (!reqMethod.Equals(RequestMethod.GET.ToString()) && requestData != null)
					{
						// send data only for POST method.
						SystemLogger.Log (SystemLogger.Module.CORE, "Sending data on the request stream... (POST)");
						SystemLogger.Log (SystemLogger.Module.CORE, "request data length: " + requestData.Length);
						using (Stream requestStream = webReq.GetRequestStream()) {
							SystemLogger.Log (SystemLogger.Module.CORE, "request stream: " + requestStream);
							requestStream.Write (requestData, 0, requestData.Length);
						}
					}
					
					using (HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse()) {
						
						SystemLogger.Log (SystemLogger.Module.CORE, "getting response...");
						return this.ReadWebResponseAndStore(webReq, webResp, service, storePath);
					}
					
				} catch (WebException ex) {
					SystemLogger.Log (SystemLogger.Module .CORE, "WebException requesting service: " + requestUriString + ".", ex);
				} catch (Exception ex) {
					SystemLogger.Log (SystemLogger.Module .CORE, "Unnandled Exception requesting service: " + requestUriString + ".", ex);
				} finally {
					// abort any previous timeout checking thread
					if(timeoutThread!=null && timeoutThread.IsAlive) {
						timeoutThread.Abort();
					}
				}
			} else {
				SystemLogger.Log (SystemLogger.Module .CORE, "Null service received for invoking.");
			}
			
			return null;
		}

		/// <summary>
		/// Invokes service, given its name, using the provided request.
		/// </summary>
		/// <param name="request">IO request.</param>
		/// <param name="serviceName">Service Name.</param>
		/// <returns>IO response.</returns>
		public IOResponse InvokeService (IORequest request, string serviceName)
		{
			return InvokeService (request, GetService (serviceName));
		}

		/// <summary>
		/// Invokes service, given its name and type, using the provided request.
		/// </summary>
		/// <param name="request">IO request.</param>
		/// <param name="serviceName"></param>
		/// <param name="type"></param>
		/// <returns>IO response.</returns>
		public IOResponse InvokeService (IORequest request, string serviceName, ServiceType type)
		{
			return InvokeService (request, GetService (serviceName, type));
		}

		public IOResponseHandle InvokeService (IORequest request, IOService service, IOResponseHandler handler)
		{
			throw new NotImplementedException ();
		}

		public IOResponseHandle InvokeService (IORequest request, string serviceName, IOResponseHandler handler)
		{
			throw new NotImplementedException ();
		}

		public IOResponseHandle InvokeService (IORequest request, string serviceName, ServiceType type, IOResponseHandler handler)
		{
			throw new NotImplementedException ();
		}
#else
        public abstract Task<string> GetDirectoryRoot();
        public abstract Task<IOService[]> GetServices();
        public abstract Task<IOService> GetService(string name);
        public abstract Task<IOService> GetService(string name, ServiceType type);
        public abstract Task<IOResponse> InvokeService(IORequest request, IOService service);
        public abstract Task<IOResponse> InvokeService(IORequest request, string serviceName);
        public abstract Task<IOResponse> InvokeService(IORequest request, string serviceName, ServiceType type);
        public abstract Task<IOResponseHandle> InvokeService(IORequest request, IOService service, IOResponseHandler handler);
        public abstract Task<IOResponseHandle> InvokeService(IORequest request, string serviceName, IOResponseHandler handler);
        public abstract Task<IOResponseHandle> InvokeService(IORequest request, string serviceName, ServiceType type, IOResponseHandler handler);
        public abstract Task<string> InvokeServiceForBinary(IORequest request, IOService service, string storePath);

        public virtual byte[] GetConfigFileBinaryData()
        {
            return null;
        }
#endif

        #endregion


    }
}
