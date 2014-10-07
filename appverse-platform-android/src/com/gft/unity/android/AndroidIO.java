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
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.math.BigInteger;
import java.net.HttpURLConnection;
import java.net.Socket;
import java.net.URI;
import java.net.URISyntaxException;
import java.net.URL;
import java.net.UnknownHostException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.KeyManagementException;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.NoSuchProviderException;
import java.security.Security;
import java.security.SignatureException;
import java.security.UnrecoverableKeyException;
import java.security.cert.CertPathBuilder;
import java.security.cert.CertPathBuilderException;
import java.security.cert.CertStore;
import java.security.cert.CertificateException;
import java.security.cert.CertificateExpiredException;
import java.security.cert.CertificateNotYetValidException;
import java.security.cert.CollectionCertStoreParameters;
import java.security.cert.PKIXBuilderParameters;
import java.security.cert.PKIXCertPathBuilderResult;
import java.security.cert.TrustAnchor;
import java.security.cert.X509CertSelector;
import java.security.cert.X509Certificate;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.Vector;
import java.util.concurrent.TimeUnit;
import java.util.zip.GZIPInputStream;

import javax.net.ssl.HostnameVerifier;
import javax.net.ssl.HttpsURLConnection;
import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

import org.apache.http.Header;
import org.apache.http.HeaderElement;
import org.apache.http.HttpEntity;
import org.apache.http.HttpHost;
import org.apache.http.HttpResponse;
import org.apache.http.HttpResponseInterceptor;
import org.apache.http.HttpVersion;
import org.apache.http.client.CookieStore;
import org.apache.http.client.methods.HttpEntityEnclosingRequestBase;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.conn.params.ConnRoutePNames;
import org.apache.http.conn.scheme.PlainSocketFactory;
import org.apache.http.conn.scheme.Scheme;
import org.apache.http.conn.scheme.SchemeRegistry;
import org.apache.http.conn.ssl.SSLSocketFactory;
import org.apache.http.conn.ssl.X509HostnameVerifier;
import org.apache.http.cookie.Cookie;
import org.apache.http.entity.HttpEntityWrapper;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.impl.conn.tsccm.ThreadSafeClientConnManager;
import org.apache.http.params.HttpProtocolParams;
import org.apache.http.protocol.HTTP;
import org.apache.http.protocol.HttpContext;
import org.spongycastle.asn1.ASN1Primitive;
import org.spongycastle.asn1.DEROctetString;
import org.spongycastle.asn1.ocsp.OCSPObjectIdentifiers;
import org.spongycastle.asn1.x509.AccessDescription;
import org.spongycastle.asn1.x509.AuthorityInformationAccess;
import org.spongycastle.asn1.x509.X509Extension;
import org.spongycastle.asn1.x509.X509Extensions;
import org.spongycastle.jce.provider.X509CertParser;
import org.spongycastle.jce.provider.X509CertificateObject;
import org.spongycastle.ocsp.BasicOCSPResp;
import org.spongycastle.ocsp.CertificateID;
import org.spongycastle.ocsp.OCSPException;
import org.spongycastle.ocsp.OCSPReq;
import org.spongycastle.ocsp.OCSPReqGenerator;
import org.spongycastle.ocsp.OCSPResp;
import org.spongycastle.ocsp.RevokedStatus;
import org.spongycastle.ocsp.SingleResp;
import org.spongycastle.x509.util.StreamParsingException;
import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserFactory;

import android.content.Context;
import android.os.Build;

import com.gft.unity.core.io.AbstractIO;
import com.gft.unity.core.io.HTTPProtocolVersion;
import com.gft.unity.core.io.IOCookie;
import com.gft.unity.core.io.IOHeader;
import com.gft.unity.core.io.IORequest;
import com.gft.unity.core.io.IOResponse;
import com.gft.unity.core.io.IOResponseHandle;
import com.gft.unity.core.io.IOResponseHandler;
import com.gft.unity.core.io.IOService;
import com.gft.unity.core.io.IOServiceEndpoint;
import com.gft.unity.core.io.IOSessionContext;
import com.gft.unity.core.io.RequestMethod;
import com.gft.unity.core.io.ServiceType;
import com.gft.unity.core.system.IOperatingSystem;
import com.gft.unity.core.system.SystemLogger.Module;

public class AndroidIO extends AbstractIO implements Runnable{

	private static final AndroidSystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();
	private static String _VALIDATECERTIFICATES = "$ValidateCertificates$";
	private static String _VALIDROOTAUTHORITIES = "VeriSign;";
	//private static String _VALIDROOTAUTHORITIES = "$ValidRoothAuthorities$";
	
	private static final String DEFAULT_SERVICE_TYPE = "XMLRPC_JSON";
	private static final String DEFAULT_SERVICE_METHOD = "POST";
	private static final String DEFAULT_ENCODING = "UTF-8";
	private static final String SERVICE_NODE_ATTRIBUTE = "SERVICE";
	private static final String ENDPOINT_NODE_ATTRIBUTE = "END-POINT";
	private static final String TYPE_ATTRIBUTE = "type";
	private static final String REQ_METHOD_ATTRIBUTE = "req-method";
	private static final String HOST_ATTRIBUTE = "host";
	private static final String PORT_ATTRIBUTE = "port";
	private static final String PATH_ATTRIBUTE = "path";
	private static final String PROXY_ATTRIBUTE = "proxy";
	private static final String SCHEME_ATTRIBUTE = "scheme";
	private static final String SERVICE_ATTRIBUTE = "name";

	// private static final String HTTP_SCHEME = "http";
	private static final String HTTPS_SCHEME = "https";
	
	private static int DEFAULT_READWRITE_TIMEOUT = 15000; // 15 seconds timeout establishing connection
	private static int DEFAULT_RESPONSE_TIMEOUT = 100000; // 100 seconds timeout reading response
	
	// reading response parameters
	private static int DEFAULT_BUFFER_READ_SIZE = 4096;	// 4 KB
	private static int MAX_BINARY_SIZE = 8*1024*1024;  // 8 MB
	
	private static DefaultHttpClient httpClient = new DefaultHttpClient();
	private static DefaultHttpClient httpSSLClient = null;
	private static final CookieStore cookieStore = httpClient.getCookieStore();
	
	private boolean addedGzipHttpResponseInterceptor = false;

	public AndroidIO() {
		loadServicesConfig();
		(new Thread(this)).start();
	}
	
	public boolean Validatecertificates() {
		return Boolean.parseBoolean(AndroidIO._VALIDATECERTIFICATES);
	}

	private void loadServicesConfig() {
		Context context = AndroidServiceLocator.getContext();
		ArrayList<IOService> servicesList = new ArrayList<IOService>();
		try {

			XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
			factory.setNamespaceAware(true);
			XmlPullParser xpp = factory.newPullParser();
			xpp.setInput(AndroidUtils.getInstance().getAssetInputStream(context.getAssets(), SERVICES_CONFIG_FILE),
					DEFAULT_ENCODING);
			int eventType = xpp.getEventType();
			IOService service = null;
			String serviceName = "";
			String serviceType = "";
			String serviceMethod = "";
			IOServiceEndpoint serviceEndpoint = null;
			while (eventType != XmlPullParser.END_DOCUMENT) {
				if (eventType == XmlPullParser.START_TAG) {
					if (xpp.getName().toUpperCase()
							.equals(SERVICE_NODE_ATTRIBUTE)) {
						service = new IOService();
						serviceName = xpp.getAttributeValue(null,
								SERVICE_ATTRIBUTE);
						serviceType = xpp.getAttributeValue(null,
								TYPE_ATTRIBUTE);
						serviceMethod = xpp.getAttributeValue(null,
								REQ_METHOD_ATTRIBUTE);
					} else if (xpp.getName().toUpperCase()
							.equals(ENDPOINT_NODE_ATTRIBUTE)) {
						serviceEndpoint = new IOServiceEndpoint();
						serviceEndpoint.setHost(xpp.getAttributeValue(null,
								HOST_ATTRIBUTE));
						String port = xpp.getAttributeValue(null,
								PORT_ATTRIBUTE);
						if (port != null) {
							serviceEndpoint.setPort(Integer.valueOf(port));
						}
						serviceEndpoint.setPath(xpp.getAttributeValue(null,
								PATH_ATTRIBUTE));
						serviceEndpoint.setProxyUrl(xpp.getAttributeValue(null,
								PROXY_ATTRIBUTE));
						serviceEndpoint.setScheme(xpp.getAttributeValue(null,
								SCHEME_ATTRIBUTE));

					}
				} else if (eventType == XmlPullParser.END_TAG) {
					if (xpp.getName().toUpperCase()
							.equals(SERVICE_NODE_ATTRIBUTE)) {
						service.setName(serviceName);
						if (serviceType == null) {
							serviceType = DEFAULT_SERVICE_TYPE;
						}
						service.setType(ServiceType.valueOf(serviceType));
						if (serviceMethod == null) {
							serviceMethod = DEFAULT_SERVICE_METHOD;
						}
						service.setRequestMethod(RequestMethod
								.valueOf(serviceMethod));
						service.setEndpoint(serviceEndpoint);
						servicesList.add(service);
					}
				}
				eventType = xpp.next();
			}
		} catch (Exception ex) {
			LOG.LogDebug(Module.PLATFORM, "LoadConfig error ["
					+ SERVICES_CONFIG_FILE + "]: " + ex.getMessage());
		}
		servicesConfig.setServices(servicesList
				.toArray(new IOService[servicesList.size()]));
	}

	private String formatRequestUriString(IORequest request, IOServiceEndpoint endpoint, String requestMethod) {
		String requestUriString = endpoint.getHost() + ":"
				+ endpoint.getPort() + endpoint.getPath();
		if (endpoint.getPort() == 0) {
			requestUriString = endpoint.getHost() + endpoint.getPath();
		}

		if (requestMethod.equalsIgnoreCase(RequestMethod.GET.toString())) {
			// add request content to the URI string when GET method.
			if (request.getContent() != null) {
				requestUriString += request.getContent();
			}
		}
		
		// JUST FOR LOCAL TESTING, DO NOT UNCOMMENT FOR PLATFORM RELEASE
		// LOG.LogDebug(Module.PLATFORM, "Requesting service: " + requestUriString);
		LOG.LogDebug(Module.PLATFORM, "Request method: " + requestMethod);
		
		return requestUriString;
	}
	
	private boolean applySecurityValidations(String requestUriString) throws KeyStoreException, NoSuchAlgorithmException, CertificateException, IOException, KeyManagementException, UnrecoverableKeyException {
		if (requestUriString.startsWith(HTTPS_SCHEME)) {
			LOG.LogDebug(Module.PLATFORM, "Applying Custom HTTPSClient (requested URI contains HTTPS protocol)");
			if(httpSSLClient == null) {
				LOG.LogDebug(Module.PLATFORM, "Custom HTTPSClient not yet intialized on first request, forcing creating it...");
				createHttpClients();
			}
			httpClient = httpSSLClient;
		}else{
			LOG.LogDebug(Module.PLATFORM, "Applying DefaultHTTPClient");
			httpClient = new DefaultHttpClient();
		}
		return true;
	}
	
	/**
	 * 
	 * @param request
	 * @param endpoint
	 * @throws URISyntaxException
	 */
	private void addingHttpClientParms(IORequest request, IOServiceEndpoint endpoint) throws URISyntaxException {
		
		// preserving the cookies between requests
		httpClient.setCookieStore(cookieStore);
		
		if(request.getProtocolVersion() == HTTPProtocolVersion.HTTP11) {
			httpClient.getParams().setParameter("http.protocol.version", HttpVersion.HTTP_1_1);
		} else {
			httpClient.getParams().setParameter("http.protocol.version", HttpVersion.HTTP_1_0);  // not chunked requests
		}
		
		httpClient.getParams().setIntParameter("http.connection.timeout", DEFAULT_READWRITE_TIMEOUT);
		httpClient.getParams().setIntParameter("http.socket.timeout", DEFAULT_RESPONSE_TIMEOUT);
	
		if (endpoint.getProxyUrl() != null
				&& !endpoint.getProxyUrl().equals("")
				&& !endpoint.getProxyUrl().equals("null")) {
			URI proxyUrl = new URI(endpoint.getProxyUrl());
			HttpHost proxyHost = new HttpHost(proxyUrl.getHost(),
					proxyUrl.getPort(), proxyUrl.getScheme());
			httpClient.getParams().setParameter(
					ConnRoutePNames.DEFAULT_PROXY, proxyHost);
		}
		
		// [MOBPLAT-200] : allow gzip, deflate decompression modes
		if(!addedGzipHttpResponseInterceptor) {
			httpClient.addResponseInterceptor(new GzipHttpResponseInterceptor());
			addedGzipHttpResponseInterceptor = true;
		}
	}
	
	
	private HttpEntityEnclosingRequestBase buildWebRequest(IORequest request, IOService service, String requestUriString, String requestMethod ) 
			throws UnsupportedEncodingException, URISyntaxException {
		
		
		HttpEntityEnclosingRequestBase httpRequest = new HttpAppverse(new URI(requestUriString), requestMethod);
		
		/*************
		 * adding content as entity, for request methods != GET
		 *************/
		if(!requestMethod.equalsIgnoreCase(RequestMethod.GET.toString())){
			if (request.getContent() != null
					&& request.getContent().length() > 0) {
				httpRequest.setEntity(new StringEntity(
						request.getContent(), HTTP.UTF_8));
			}
		}
		
		/*************
		 * CONTENT TYPE
		 *************/
		String contentType = contentTypes.get(service.getType()).toString();
		if (request.getContentType() != null) {
			contentType = request.getContentType();
			
		} 
		httpRequest.setHeader("Content-Type", contentType);
		
		/*************
		 * CUSTOM HEADERS HANDLING
		 *************/
		if (request.getHeaders() != null
				&& request.getHeaders().length > 0) {
			for (IOHeader header : request.getHeaders()) {
				httpRequest.setHeader(header.getName(),
						header.getValue());
			}
		}
		
		/*************
		 * COOKIES HANDLING
		 *************/
		if (request.getSession() != null
				&& request.getSession().getCookies() != null
				&& request.getSession().getCookies().length > 0) {
			StringBuffer buffer = new StringBuffer();
			IOCookie[] cookies = request.getSession().getCookies();
			for (int i = 0; i < cookies.length; i++) {
				IOCookie cookie = cookies[i];
				buffer.append(cookie.getName());
				buffer.append("=");
				buffer.append(cookie.getValue());
				if (i + 1 < cookies.length) {
					buffer.append(" ");
				}
			}
			httpRequest.setHeader("Cookie", buffer.toString());
		}
		
		/*************
		 * DEFAULT HEADERS
		 *************/
		httpRequest.setHeader("Accept", contentType); // Accept header should be the same as the request content type used (could be override by the request, or use the service default)
		// httpRequest.setHeader("content-length",
		// String.valueOf(request.getContentLength()));
		httpRequest.setHeader("keep-alive", String.valueOf(false));
		
		// TODO: set conn timeout ???
		
		/*************
		 * setting user-agent
		 *************/
		IOperatingSystem system = (IOperatingSystem) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_TYPE_SYSTEM);
		HttpProtocolParams.setUserAgent(httpClient.getParams(),
				system.GetOSUserAgent());
		
		return httpRequest;
	}
	
	/**
	 * 
	 * @param httpResponse
	 * @param service
	 * @return
	 * @throws IOException 
	 * @throws IllegalStateException 
	 */
	private IOResponse readWebResponse(HttpResponse httpResponse, IOService service) throws IllegalStateException, IOException {
		
		IOResponse response = new IOResponse ();
		response.setSession(new IOSessionContext());

		byte[] resultBinary = null;
		
		String responseMimeTypeOverride = (httpResponse.getLastHeader("Content-Type")!=null? httpResponse.getLastHeader("Content-Type").getValue() : null);
		LOG.LogDebug(Module.PLATFORM, "response content type: " + responseMimeTypeOverride);
		
		// getting response input stream
		InputStream responseStream = httpResponse.getEntity().getContent();
		
		int lengthContent = -1;
		int bufferReadSize = DEFAULT_BUFFER_READ_SIZE;
		
		try {
			lengthContent = (int) httpResponse.getEntity().getContentLength();
			if (lengthContent >= 0 && lengthContent<=bufferReadSize) {
				bufferReadSize = lengthContent;
			}
		} catch (Exception e) {
			LOG.LogDebug(Module.PLATFORM, "Error while getting Content-Length header from response: " + e.getMessage());
		}
		
		if(lengthContent>MAX_BINARY_SIZE) {
			LOG.LogDebug(Module.PLATFORM, "WARNING! - file exceeds the maximum size defined in platform (" + MAX_BINARY_SIZE+ " bytes)");
		} else {
			LOG.LogDebug(Module.PLATFORM, "reading response stream content length: " + lengthContent);
			/* WE DON'T READ IN A FULL BLOCK ANYMORE
			 * 
			// Read in block, if content length provided.
			// Create the byte array to hold the data
			resultBinary = new byte[length];
	
			// Read in the bytes
			int offset = 0;
			int numRead = 0;
			while (offset < resultBinary.length
					&& (numRead = responseStream.read(resultBinary,
							offset, resultBinary.length - offset)) >= 0) {
				offset += numRead;
			}
			
			*/
			
			// Read in buffer blocks till the end of stream.
			ByteArrayOutputStream outBuffer = new ByteArrayOutputStream();

			byte[] readBuffer = new byte[bufferReadSize];
			int readLen = 0;
			int totalReadLen = 0;
			try {
				while ((readLen = responseStream.read(readBuffer, 0,
						readBuffer.length)) > 0) {
					outBuffer.write(readBuffer, 0, readLen);
					totalReadLen = totalReadLen + readLen;
				}
			} finally {
				resultBinary = outBuffer.toByteArray();
				outBuffer.close();
				outBuffer = null;
			}
			LOG.LogDebug(Module.PLATFORM, "total read length: " + totalReadLen);
		}
		
		/*************
		 * COOKIES HANDLING
		 *************/
		LOG.LogDebug(Module.PLATFORM, "reading cookies.. ");
		if (cookieStore.getCookies().size() > 0) {

			IOCookie[] cookies = new IOCookie[cookieStore.getCookies()
					.size()];
			for (int i = 0; i < cookieStore.getCookies().size(); i++) {
				Cookie cookie = cookieStore.getCookies().get(i);
				IOCookie ioCookie = new IOCookie();
				ioCookie.setName(cookie.getName());
				ioCookie.setValue(cookie.getValue());
				cookies[i] = ioCookie;
			}
			response.getSession().setCookies(cookies);
		}

		/*************
		 * CACHE
		 *************/
		LOG.LogDebug(Module.PLATFORM, "reading cache header.. ");
		// preserve cache-control header from remote server, if any
		String cacheControlHeader = (httpResponse.getLastHeader("Cache-Control")!=null? httpResponse.getLastHeader("Cache-Control").getValue() : null);
		if (cacheControlHeader != null && !cacheControlHeader.isEmpty()) {
			LOG.LogDebug(Module.PLATFORM, "Found Cache-Control header on response: " + cacheControlHeader + ", using it on internal response...");
			
			IOHeader cacheHeader = new IOHeader();
			cacheHeader.setName("Cache-Control");
			cacheHeader.setValue(cacheControlHeader);
			
			List<IOHeader> headers = new ArrayList<IOHeader>();
			if(response.getHeaders() != null) {
				headers = Arrays.asList(response.getHeaders());
			}
			headers.add(cacheHeader);
			response.setHeaders((IOHeader[])headers.toArray(new IOHeader[0]));
		}
		
		// Close the input stream and return bytes
		responseStream.close();

		LOG.LogDebug(Module.PLATFORM, "checking binary service type... ");
		if (ServiceType.OCTET_BINARY.equals(service.getType())) {
			if (responseMimeTypeOverride != null && !responseMimeTypeOverride.equals(contentTypes.get(service.getType()))) {
				response.setContentType(responseMimeTypeOverride);
			} else {
				response.setContentType(contentTypes.get(service.getType()).toString());
			}
			response.setContentBinary(resultBinary); // Assign binary
			// content here
		} else {
			response.setContentType(contentTypes.get(service.getType())
					.toString());
			response.setContent(new String(resultBinary));
		}
		LOG.LogDebug(Module.PLATFORM, "END reading response.. ");
		return response;
	}
	
	/**
	 * 
	 * @return
	 */
	private String GetDirectoryRoot () {
		// should match the same directory root used by other platform APIs
		
		Context context = AndroidServiceLocator.getContext();
		return context.getFilesDir().getAbsolutePath();
	}
	
	/**
	 * 
	 * @param httpResponse
	 * @param service
	 * @param storePath
	 * @return
	 * @throws IllegalStateException
	 * @throws IOException
	 */
	private String readWebResponseAndStore(HttpResponse httpResponse, IOService service, String storePath) throws IllegalStateException, IOException {
		
		// getting response input stream
		InputStream responseStream = httpResponse.getEntity().getContent();
		
		int lengthContent = -1;
		int bufferReadSize = DEFAULT_BUFFER_READ_SIZE;
		
		try {
			lengthContent = (int) httpResponse.getEntity().getContentLength();
			if (lengthContent >= 0 && lengthContent<=bufferReadSize) {
				bufferReadSize = lengthContent;
			}
		} catch (Exception e) {
			LOG.LogDebug(Module.PLATFORM, "Error while getting Content-Length header from response: " + e.getMessage());
		}
		LOG.LogDebug(Module.PLATFORM, "reading response stream content length: " + lengthContent);
		BufferedInputStream bis = new BufferedInputStream(responseStream);
		 
        int size;
        byte[] buffer = new byte[bufferReadSize];

        File fullStorePath = new File(this.GetDirectoryRoot (), storePath);
        LOG.LogDebug(Module.PLATFORM, "Storing file at: " + fullStorePath.getAbsolutePath());
		
        FileOutputStream fos = new FileOutputStream(fullStorePath);
        BufferedOutputStream bos = new BufferedOutputStream(fos, buffer.length);
        int totalReadLen = 0;
        while ((size = bis.read(buffer, 0, buffer.length)) != -1) {
            bos.write(buffer, 0, size);
            // just for testing // LOG.LogDebug(Module.PLATFORM, "read length: " + size);
            totalReadLen = totalReadLen + size;
        }
        
        LOG.LogDebug(Module.PLATFORM, "total read length: " + totalReadLen);

        bos.flush();
        bos.close();
        fos.close();

        bis.close();
		
		// Close the input stream and return bytes
		responseStream.close();

		return storePath;
	}
	
	@Override
	public IOResponse InvokeService(IORequest request, IOService service) {

		IOServiceEndpoint endpoint = service.getEndpoint();
		IOResponse response = new IOResponse();
		
		if (service != null) {
			// JUST FOR LOCAL TESTING, DO NOT UNCOMMENT FOR PLATFORM RELEASE
			// LOG.LogDebug(Module.PLATFORM, "Request content: " + request.getContent());

			if (endpoint == null) {
				LOG.LogDebug(Module.PLATFORM,
						"No endpoint configured for this service name: "
								+ service.getName());
				return response;
			}

			String requestMethod = service.getRequestMethod().toString();
			if(request.getMethod() != null && request.getMethod().length()>0) requestMethod = request.getMethod().toUpperCase();
			
			String requestUriString = formatRequestUriString(request, endpoint, requestMethod);
			Thread timeoutThread = null;
			
			try {

				// Security - VALIDATIONS
				if(!this.applySecurityValidations(requestUriString)) {
					return null;
				}
				
				// Adding HTTP Client Parameters
				this.addingHttpClientParms(request, endpoint);
				
				// Building Web Request to send
				HttpEntityEnclosingRequestBase httpRequest = this.buildWebRequest(request, service, requestUriString, requestMethod);
				
				LOG.LogDebug(Module.PLATFORM, "Downloading service content");

				// Throw a new Thread to check absolute timeout
				timeoutThread = new Thread(new CheckTimeoutThread(httpRequest));
				timeoutThread.start();
				
				long start = System.currentTimeMillis();
				HttpResponse httpResponse = httpClient.execute(httpRequest);
				LOG.LogDebug(Module.PLATFORM,
						"Content downloaded in "
								+ (System.currentTimeMillis() - start) + "ms");
				
				
				// Read response
				response = this.readWebResponse(httpResponse, service);
				
			} catch (Exception ex) {
				LOG.Log(Module.PLATFORM,
						"Unnandled Exception requesting service.", ex);
				response.setContentType(contentTypes.get(ServiceType.REST_JSON)
						.toString());
				response.setContent("Unhandled Exception Requesting Service. Message: " + ex.getMessage());
			} finally {
				// abort any previous timeout checking thread
				if(timeoutThread!=null && timeoutThread.isAlive()) {
					timeoutThread.interrupt();
				}
			}
		}

		LOG.LogDebug(Module.PLATFORM, "invoke service finished");
		return response;
	}
	


	@Override
	public String InvokeServiceForBinary(IORequest request, IOService service, String storePath) {
		
		IOServiceEndpoint endpoint = service.getEndpoint();
		
		if (service != null) {
			// JUST FOR LOCAL TESTING, DO NOT UNCOMMENT FOR PLATFORM RELEASE
			// LOG.LogDebug(Module.PLATFORM, "Request content (for binary): " + request.getContent());

			if (endpoint == null) {
				LOG.LogDebug(Module.PLATFORM, "No endpoint configured for this service name: "
								+ service.getName());
				return null;
			}
			
			if (!ServiceType.OCTET_BINARY.equals (service.getType())) {
				LOG.LogDebug(Module.PLATFORM, "This method only admits OCTET_BINARY service types");
				return null;
			}

			String requestMethod = service.getRequestMethod().toString();
			if(request.getMethod() != null && request.getMethod().length()>0) requestMethod = request.getMethod().toUpperCase();
			
			String requestUriString = formatRequestUriString(request, endpoint, requestMethod);
			Thread timeoutThread = null;
			
			try {

				// Security - VALIDATIONS
				if(!this.applySecurityValidations(requestUriString)) {
					return null;
				}
				
				// Adding HTTP Client Parameters
				this.addingHttpClientParms(request, endpoint);
				
				// Building Web Request to send
				HttpEntityEnclosingRequestBase httpRequest = this.buildWebRequest(request, service, requestUriString, requestMethod);
				
				LOG.LogDebug(Module.PLATFORM, "Downloading service content");

				// Throw a new Thread to check absolute timeout
				timeoutThread = new Thread(new CheckTimeoutThread(httpRequest));
				timeoutThread.start();
				
				long start = System.currentTimeMillis();
				HttpResponse httpResponse = httpClient.execute(httpRequest);
				LOG.LogDebug(Module.PLATFORM,
						"Content downloaded in "
								+ (System.currentTimeMillis() - start) + "ms");
				
				
				// Read response and store to local filestystem
				return this.readWebResponseAndStore(httpResponse, service, storePath);
				
			} catch (Exception ex) {
				LOG.Log(Module.PLATFORM,
						"Unnandled Exception requesting service.", ex);
			} finally {
				// abort any previous timeout checking thread
				if(timeoutThread!=null && timeoutThread.isAlive()) {
					timeoutThread.interrupt();
				}
			}
		}

		LOG.LogDebug(Module.PLATFORM, "invoke service (for binary) finished");
		return null;
	}

	@Override
	public IOResponseHandle InvokeService(IORequest request, IOService service,
			IOResponseHandler handler) {
		throw new UnsupportedOperationException("Not supported yet.");
	}
	
	private class CheckTimeoutThread implements Runnable {

		private final long ABSOLUTE_INVOKE_TIMEOUT = 60000; // 60 seconds
		private HttpUriRequest httpRequest = null;
		
		public CheckTimeoutThread(HttpUriRequest httpRequest) {
			super();
			this.httpRequest = httpRequest;
		}

		@Override
		public void run() {
			try {
				Thread.sleep(ABSOLUTE_INVOKE_TIMEOUT);
				
				LOG.LogDebug(Module.PLATFORM, "*** INVOKE SERVICE TIMEOUT *** Absolute timeout checking completed.");
				if(httpRequest != null) {
					LOG.LogDebug(Module.PLATFORM, "*** INVOKE SERVICE TIMEOUT *** Aborting request...");
					httpRequest.abort();
				}
				
			} catch (InterruptedException e) {
				LOG.LogDebug(Module.PLATFORM, "*** INVOKE SERVICE TIMEOUT *** Absolute timeout checking interrupted.");
			}
			
		}
	}
	
	public class HttpAppverse extends HttpEntityEnclosingRequestBase {

		private String method = null;
		
		public HttpAppverse(final URI uri, String method) {
			this.setURI(uri);
			this.method = method;
		}

		@Override
		public String getMethod() {
			return method;
		}	
	}
	
	public class MySSLSocketFactory extends SSLSocketFactory {
	    SSLContext sslContext = SSLContext.getInstance("TLS");

	    public MySSLSocketFactory(KeyStore truststore) throws NoSuchAlgorithmException, KeyManagementException, KeyStoreException, UnrecoverableKeyException {
	        super(truststore);

	        TrustManager tm = new X509TrustManager() {
	            public void checkClientTrusted(X509Certificate[] chain, String authType) throws CertificateException {
	            }

	            public void checkServerTrusted(X509Certificate[] chain, String authType) throws CertificateException {
					LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Accepting all certificates");
	            }

	            public X509Certificate[] getAcceptedIssuers() {
	                return null;
	            }
	        };

	        sslContext.init(null, new TrustManager[] { tm }, null);
	    }

	    @Override
	    public Socket createSocket(Socket socket, String host, int port, boolean autoClose) throws IOException, UnknownHostException {
	        return sslContext.getSocketFactory().createSocket(socket, host, port, autoClose);
	    }

	    @Override
	    public Socket createSocket() throws IOException {
	        return sslContext.getSocketFactory().createSocket();
	    }
	}
	
	public static class ValidatingSSLSocketFactory extends SSLSocketFactory {
	    protected static SSLContext sslContext;
	    protected static Map<Integer, Long> myCertificateList;
	    protected static ValidatingSSLSocketFactory singletonFactory;
	    
	    public static ValidatingSSLSocketFactory GetInstance(KeyStore truststore)
	    {
	    	if(singletonFactory==null){
	    		try {
					singletonFactory = new ValidatingSSLSocketFactory(truststore);
				} catch (KeyManagementException e) {
					// TODO Auto-generated catch block
					return null;
				} catch (NoSuchAlgorithmException e) {
					// TODO Auto-generated catch block
					return null;
				} catch (KeyStoreException e) {
					// TODO Auto-generated catch block
					return null;
				} catch (UnrecoverableKeyException e) {
					// TODO Auto-generated catch block
					return null;
				}
	    	}
	    	return singletonFactory;
	    }

	    private ValidatingSSLSocketFactory(final KeyStore truststore) throws NoSuchAlgorithmException, KeyManagementException, KeyStoreException, UnrecoverableKeyException {
	        super(truststore);
	        sslContext = SSLContext.getInstance("TLS");
	        myCertificateList = new HashMap<Integer, Long>();
	        
	        Security.addProvider(new org.spongycastle.jce.provider.BouncyCastleProvider());
	        
	        TrustManager tm = new X509TrustManager() {
	        	 
	        	public X509Certificate[] getAcceptedIssuers() {
	        		try{
	        		X509Certificate[] returnIssuers = new X509Certificate[truststore.size()];
	                Enumeration aliases = truststore.aliases();
	                
	                int i=0;
	                while(aliases.hasMoreElements()){
	                	returnIssuers[i] = (X509Certificate) truststore.getCertificate((String)aliases.nextElement());
	                	//LOG.LogDebug(Module.PLATFORM, "TRUSTED CERT " + i + ": NAME " + returnIssuers[i].getSubjectDN().getName() + " ;ISSUER NAME: " + returnIssuers[i].getIssuerDN().getName());
	                	i++;
	                }
	                return returnIssuers;
	        		}catch(Exception ex){
	        			LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Couldnt retrieve Accepted Issuers");
	        		}
	        		return null;
	        		
	            }
	            public void checkClientTrusted(X509Certificate[] chain, String authType) throws CertificateException {
	            }

	            public void checkServerTrusted(X509Certificate[] chain, String authType) throws CertificateException {
	            	boolean bErrorsFound = false;
	            	try {
	    				LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Starting Certificate Validation process");
	    				if(chain!= null && chain.length>0) LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate chain elements: " + chain.length ); 
	            		//check the certificate is in memory
	            		if(!certificateIsTheSame(chain[0])){
	            			if(certChainIsValid(chain[0], chain)){
		            			for(int i=0; i< chain.length; i++){
		            				X509Certificate chainCert = chain[i];
		            				LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate Name: " + chainCert.getSubjectDN().getName() + " ;SN: " + chainCert.getSerialNumber().toString() );
		            				
				            		X509CertParser parser = new X509CertParser();
				            		ByteArrayInputStream bais = new ByteArrayInputStream(chainCert.getEncoded());
				    	            parser.engineInit(bais);
				    	            bais.close();
				    	            
				            		X509CertificateObject cert = (X509CertificateObject)parser.engineRead();
				            		if(certIsValidNow(cert)){
				            			if(!certIsSelfSigned(cert)){
				            				//if(certChainIsValid(cert, chain)){
				            					//all checks went OK. Add certificate to memory with current date and time
				            					//myCertificateList.put(Integer.valueOf(chain[0].hashCode()), Long.valueOf(System.currentTimeMillis()));
				            					LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate is Valid");
				            				//}
				            			}else{ 
				            				bErrorsFound = true;
				            				LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate is Self Signed");
				            			}
					            	}else{ 
					            		bErrorsFound = true;
					            		LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate is expired");
					            	}
		            			}
		            			if(!bErrorsFound) myCertificateList.put(Integer.valueOf(chain[0].hashCode()), Long.valueOf(System.currentTimeMillis())); 
	            			}else{ 
            					bErrorsFound = true;
            					LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate Chain is not valid");
            				}
	            		}else{LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Trusted Certificate");}
	            		
					} catch (StreamParsingException e) {
						bErrorsFound = true;
						LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate chain error");
					} catch (IOException e) {
						bErrorsFound = true;
						LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Error in certificate chain");
					}
	            	if(bErrorsFound){ throw new CertificateException("Certificate Validation: Errors found in the Certificate Chain");}
	            }
	            
	            /**
	             * Check the certificate is in memory and is still valid
	             * @param cert
	             * 				Certificate to check
	             * @return
	             * 				True if certificate is valid and in memory, otherwise false
	             */
	            private boolean certificateIsTheSame(X509Certificate cert){
	            	removeTimedOutCertificates();
	                if (!myCertificateList.isEmpty() && 
	                		myCertificateList.containsKey(Integer.valueOf(cert.hashCode())) && 
	                		certIsValidNow(cert)){
	                	return true;
	                }else{
	                	return false;
	                }
	            }
	            
	            /**
	             * Remove the certificates that have been accessed and visited after 10 minutes from memory
	             */
	            private void removeTimedOutCertificates(){
	            	long currentTimeMillis = System.currentTimeMillis();
	            	Set<Integer> certsToRemove = new HashSet<Integer>();
	            	if (!myCertificateList.isEmpty()) {
	            		for(Map.Entry<Integer, Long> listEntry : myCertificateList.entrySet()){
		            		long certAccessTime = listEntry.getValue().longValue();
		            		if(TimeUnit.MILLISECONDS.toSeconds(currentTimeMillis - certAccessTime)>=600){
		            			certsToRemove.add(listEntry.getKey());
		            		}
	            		}
	            		myCertificateList.keySet().removeAll(certsToRemove);
	            	}
	            }
	            
	            /**
	             * Checks the Certificate Chain is well formed and certificates not revoked
	             * @param cert
	             * 				End certificate the request is consuming
	             * @param chain
	             * 				Certificate chain containing the rest of certificates
	             * @return
	             * 				True if the Certificate chain is valid, otherwise False
	             */
	            private boolean certChainIsValid(X509Certificate cert, X509Certificate[] chain){
	            	// DO NOT check OCSP revocation URLs. The time consuming this is expensive.
					// TODO make this configurable and asynchronously in the case of enabled
				    // !verifyCertificateOCSP(chain)  ---> ASYNC
	            	//if(!verifyCertificateOCSP(chain)){
	            	LOG.LogDebug(Module.PLATFORM, 
	            			"*************** OCSP Verification (certificate revocation check) is DISABLED for this build");
	            		try{
	            			//Selector to point out the end certificate
	            			X509CertSelector selector = new X509CertSelector();
	            			selector.setCertificate(cert);
	            			
	            			//Trust anchor to point out the root certificate
	            			Set<TrustAnchor> trust = new HashSet<TrustAnchor>();
	            			trust.add(new TrustAnchor(chain[chain.length-1],null));
	            			
	            			//Params containing the selector, trust anchors and certificate chain. We disable CRL checks
	            			PKIXBuilderParameters pParams = new PKIXBuilderParameters(trust, selector);
	            			pParams.setRevocationEnabled(false);
	            			Set<X509Certificate> setchain = new HashSet<X509Certificate>();
	            			for(X509Certificate SCCert:chain){setchain.add(SCCert);}
	            			CertStore allcerts = CertStore.getInstance("Collection", new CollectionCertStoreParameters(setchain));
	            			pParams.addCertStore(allcerts);
	            			
	            			//Create the cert path result, if no exception is thrown means all went OK
	            			CertPathBuilder builder = CertPathBuilder.getInstance("PKIX");
	            			PKIXCertPathBuilderResult result = (PKIXCertPathBuilderResult)builder.build(pParams); 
					        LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate Path is valid");
		            		return true;
	            		}catch (CertPathBuilderException e) {
							LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Errors found in the Certificate Path ");
						} catch (NoSuchAlgorithmException e) {
							LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Errors found in the Certificate Path ");
						}catch (InvalidAlgorithmParameterException e) {
							LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Errors found in the Certificate Path ");
						}
	            	//}else{return false;}
					return false;
				}
	            
	            /**
	             * Checks that all the certificates in the chain are not revoked. The checks are made via OCSP 
	             * @param chain
	             * 				Certificate chain to check
	             * @return
	             * 				True if any certificate is Revoked, otherwise False
	             */
	            private boolean verifyCertificateOCSP(X509Certificate[] chain){
	            	ArrayList<URL> certsUrls = new ArrayList<URL>();
	            	ArrayList<OCSPReq> requestList = new ArrayList<OCSPReq>();
	            	boolean bCertificateIsRevoked = false;
	            	try {
	            		for(X509Certificate cert:chain){
	            			//Read the OCSP extension from the certificates
	            			byte[] extensionBytes = cert.getExtensionValue(X509Extensions.AuthorityInfoAccess.getId());
	            			if(extensionBytes != null && extensionBytes.length>0){
	            				
		            			DEROctetString derObjAccessDescriptors = (DEROctetString)ASN1Primitive.fromByteArray(extensionBytes);
		            			ASN1Primitive primitiveObject = ASN1Primitive.fromByteArray(derObjAccessDescriptors.getOctets());
								AccessDescription[] descriptors = AuthorityInformationAccess.getInstance(primitiveObject).getAccessDescriptions();
								//If the URL is already in the list do not add it
								if(descriptors!= null && descriptors.length>0){
									String urlContent = descriptors[0].getAccessLocation().getName().toString();
									if(urlContent.startsWith("http://")){
										URL url = new URL(urlContent);
										if(!certsUrls.contains(url)) certsUrls.add(url);
									}
								}
	            			}
	            		}	            		
		                
	            		if(!certsUrls.isEmpty()){
	            			//Create the OCSP request content for each certificate
		            		OCSPReqGenerator OCSPRequestGenerator;
		            		for(int i=0; i<(chain.length -1);i++){
		            			OCSPRequestGenerator = new OCSPReqGenerator();
		            			CertificateID ID = new CertificateID(CertificateID.HASH_SHA1, chain[i+1], chain[i].getSerialNumber());
		            			OCSPRequestGenerator.addRequest(ID);
		            			BigInteger nonce = BigInteger.valueOf(System.currentTimeMillis());
			            		Vector oids = new Vector();
			            		Vector values = new Vector();
			            		oids.add(OCSPObjectIdentifiers.id_pkix_ocsp_nonce);
			            		values.add(new X509Extension(false, new DEROctetString(nonce.toByteArray())));
			            		requestList.add(OCSPRequestGenerator.generate());
		            		}
		            		
		            		//Create the OCSP request and get the response
		            		for(int i=0;i<certsUrls.size() && !bCertificateIsRevoked;i++){
			            		for(int j=0;j<requestList.size() && !bCertificateIsRevoked; j++){
			            			HttpURLConnection requestToOCSPServer = (HttpURLConnection)certsUrls.get(i).openConnection();
			            			requestToOCSPServer.setRequestProperty("Content-Type", "application/ocsp-request");
			            			requestToOCSPServer.setRequestProperty("Accept", "application/ocsp-response");
			            			requestToOCSPServer.setDoOutput(true);
			            			requestToOCSPServer.setReadTimeout(DEFAULT_RESPONSE_TIMEOUT);
			            			requestToOCSPServer.setConnectTimeout(DEFAULT_READWRITE_TIMEOUT);
			            			
			            			byte[] bRequestBytes = requestList.get(j).getEncoded();
			            			OutputStream out = requestToOCSPServer.getOutputStream();
			            			BufferedOutputStream buf = new BufferedOutputStream(out);
			            			DataOutputStream dataOut = new DataOutputStream(buf);
			            			dataOut.write(bRequestBytes);			            			
			            			dataOut.flush();
			            			dataOut.close();
			            			buf.close();
			            			out.close();
			            			
			            			InputStream in = (InputStream)requestToOCSPServer.getContent();
			            			OCSPResp OCSPResponse = new OCSPResp(in);
			            			BasicOCSPResp basicOCSPResponse = (BasicOCSPResp)OCSPResponse.getResponseObject();
			            			//Check the response for certificate status
			            			if(basicOCSPResponse != null){
			            				for(SingleResp singleResponse:basicOCSPResponse.getResponses()){
			            					Object certStatus = singleResponse.getCertStatus();
			            					if(certStatus instanceof RevokedStatus) bCertificateIsRevoked = true;
			            				}
			            			}
			            			
			            		}
		            		}
	            		}
					} catch (IOException e) {
						LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Errors found in the Certificate OCSP Validation ");
						bCertificateIsRevoked = true;
					} catch (OCSPException e) {
						LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Errors found in the Certificate OCSP Validation");
						bCertificateIsRevoked = true;
					}
	            	return bCertificateIsRevoked;
	            }
	            
	            /**
	             * Checks the certificate is valid at the current date and time
	             * @param cert
	             * 				Certificate to check
	             * @return
	             * 				True if is valid, otherwise false
	             */
	            private boolean certIsValidNow(X509Certificate cert)
	            {
	            	try {
						cert.checkValidity();
						LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate is not expired");
						return true;
					} catch (CertificateExpiredException e) {
						LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate is expired");
					} catch (CertificateNotYetValidException e) {
						LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate is not yet valid");
					}
	            	return false;
	            }
	            
	            /**
	             * Checks the certificate is self signed
	             * @param cert
	             * 				Certificate to check
	             * @return
	             * 				True if the certificate is self signed, otherwise False
	             */
	            private boolean certIsSelfSigned(X509Certificate cert)
	            {
	            	try {
						cert.verify(cert.getPublicKey());
						LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate is self-signed");
						
						//ROOT TOKEN READ
						List<String> rootAuth = null;
						boolean bLookForRoots = false;
						if(!_VALIDROOTAUTHORITIES.isEmpty() ){
							LOG.LogDebug(Module.PLATFORM, "Certificate Validation: List of Root Authorities Found");
							bLookForRoots = true;
							if(_VALIDROOTAUTHORITIES.indexOf(';')!=-1){
								rootAuth = Arrays.asList(_VALIDROOTAUTHORITIES.split("/\\;/g"));
								LOG.LogDebug(Module.PLATFORM, "Certificate Validation: List of Root Authorities Found : " + rootAuth.get(0));
							}else 
							{
								rootAuth = Arrays.asList(new String[]{_VALIDROOTAUTHORITIES});
								LOG.LogDebug(Module.PLATFORM, "Certificate Validation: List of Root Authorities Found : " + rootAuth.get(0));
							}
						}
						/////////////
						
						// LOOK FOR TRUSTED ROOT CERTS AND TRUSTED ROOT ISSUER MUST CONTAIN A VALID NAME
						boolean bRootFound = false;
						for(X509Certificate trustedCert : this.getAcceptedIssuers()){
							//If trusted and cert have same name
							if(trustedCert.getSubjectX500Principal().getName().equals(cert.getIssuerX500Principal().getName())){
								//if we have a list of valid Root Authorities
								if(bLookForRoots){
									//If list is filled
									if(rootAuth!= null){
										LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Checking Valid Root Authorities");
										for(String caRootName : rootAuth){
											caRootName = caRootName.replace(';', ' ').trim().toLowerCase();
											if(!caRootName.isEmpty() && trustedCert.getIssuerX500Principal().getName().toLowerCase().contains(caRootName)) {
												bRootFound = true;
												LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Valid Authority found");
												//Found a match, do not look for more
												break;
											}
										}
									}else{
										//SOMETHING WEIRD. CANNOT LOOK FOR AUTHORITIES AND NOT HAVING AUTHORITIES
										bRootFound = false;
										LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Inconsistency. Cannot look for Roots and not having a list");
										//END THE LOOP AND GIVE IT AS A BAD ATTEMPT
										break;
									}
								}else{
									//NOT LOOKING FOR AUTHORITIES, WE FOUND A MATCH WITH NAMES
									bRootFound = true;
									LOG.LogDebug(Module.PLATFORM, "Certificate Validation: All authorities Valid");
								}
								
								if(bRootFound){
									LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Trusted Certificate found");
									return false;
								}
							}
						}
						/////////////////
						return true;
					} catch (InvalidKeyException e) {
					} catch (CertificateException e) {
					} catch (NoSuchAlgorithmException e) {
					} catch (NoSuchProviderException e) {
					} catch (SignatureException e) {}
	            	LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate is not self signed");
	            	return false;
	            }
	            
	            /* NOT TO USE
	             * Reason: Takes too much time depending the CRL file. 1Mb Crl file download in 15 secs
	             
				private boolean verifyCertificateCRLs(X509Certificate cert){
	            	boolean bContinueValidating = true;
	            	try{
	            		List<String> crlDistPoints = getCrlDistributionPoints(cert);
	            		if(crlDistPoints.size()>0){
			                crlDistPoints.get(0);
			                for (int i =0; i< crlDistPoints.size() && bContinueValidating;i++) {
			                	String crlDP = crlDistPoints.get(i);
			                    X509CRL crl = downloadCRL(crlDP);
			                    if (crl!= null && crl.isRevoked(cert)) {
			                    	bContinueValidating = false;
			                        LOG.LogDebug(Module.PLATFORM, "Certificate " + cert.getSubjectX500Principal().getName() + " is revoked.");
			                    }
			                }
	            		}
		                return bContinueValidating;
	            	}catch(Exception e){
	            		e.printStackTrace();
	            	}
	            	return false;
	            }

	            
	            private X509CRL downloadCRL(String crlURL) throws IOException,CertificateException, CRLException, NamingException {
	                if (crlURL.startsWith("http://") || crlURL.startsWith("https://") || crlURL.startsWith("ftp://")) {
	                	LOG.LogDebug(Module.PLATFORM, "Downloading CRL from : " + crlURL);
	                    X509CRL crl = downloadCRLFromWeb(crlURL);
	                    return crl;
	                }
	                return null;
	            }
	            
	            private X509CRL downloadCRLFromWeb(String crlURL) throws MalformedURLException, IOException, CertificateException,CRLException {
	                URL url = new URL(crlURL);
	                InputStream crlStream = null;
	                try {
	                	crlStream = url.openStream();
	                    CertificateFactory cf = CertificateFactory.getInstance("X.509");
	                    X509CRL crl = (X509CRL) cf.generateCRL(crlStream);
	                    return crl;
	                } finally {
	                    if(crlStream != null) crlStream.close();
	                }
	            }
	            
	            private List<String> getCrlDistributionPoints(X509Certificate cert) throws IOException{
	                byte[] crldpExt = cert.getExtensionValue(X509Extensions.CRLDistributionPoints.getId());
	                if (crldpExt == null) {
	                    List<String> emptyList = new ArrayList<String>();
	                    return emptyList;
	                }
	                DEROctetString derObjCrlDP = (DEROctetString)ASN1Primitive.fromByteArray(crldpExt);
	                ASN1Primitive pp = ASN1Primitive.fromByteArray(derObjCrlDP.getOctets());
	                CRLDistPoint distPoint = CRLDistPoint.getInstance(pp);
	                List<String> crlUrls = new ArrayList<String>();
	                for (DistributionPoint dp : distPoint.getDistributionPoints()) {
	                    DistributionPointName dpn = dp.getDistributionPoint();
	                    // Look for URIs in fullName
	                    if (dpn != null) {
	                        if (dpn.getType() == DistributionPointName.FULL_NAME) {
	                            GeneralName[] genNames = GeneralNames.getInstance(dpn.getName()).getNames();
	                            // Look for an URI
	                            for (int j = 0; j < genNames.length; j++) {
	                                if (genNames[j].getTagNo() == GeneralName.uniformResourceIdentifier) {
	                                    String url = DERIA5String.getInstance(genNames[j].getName()).getString();
	                                    crlUrls.add(url);
	                                }
	                            }
	                        }
	                    }
	                }
	                return crlUrls;
	            }
	             */
	            
	            
	        };

	        sslContext.init(null, new TrustManager[] { tm }, null);
	    }

	    @Override
	    public Socket createSocket(Socket socket, String host, int port, boolean autoClose) throws IOException, UnknownHostException {
	        return sslContext.getSocketFactory().createSocket(socket, host, port, autoClose);
	    }

	    @Override
	    public Socket createSocket() throws IOException {
	        return sslContext.getSocketFactory().createSocket();
	    }
	}
	
	/**
	 * This response interceptor is used to inflate GZIp responses
	 * @author maps
	 * [MOBPLAT-200]: allow gzip, deflate decompression modes
	 */
	private class GzipHttpResponseInterceptor implements HttpResponseInterceptor {
        
    	@Override
    	public void process(final HttpResponse response, final HttpContext context) {
            final HttpEntity entity = response.getEntity();
            final Header encoding = entity.getContentEncoding();
            if (encoding != null) {
            	LOG.LogDebug(Module.PLATFORM, "Response has content-enconding headers #" 
            			+ ((encoding.getElements()!=null)?encoding.getElements().length: "NULL"));
                inflateGzip(response, encoding);
            }
        }
 
        private void inflateGzip(final HttpResponse response, final Header encoding) {
            for (HeaderElement element : encoding.getElements()) {
                if (element.getName().equalsIgnoreCase("gzip")) {
                	LOG.LogDebug(Module.PLATFORM, "Response GZIP Encoding found. Inflating response content..."); 
                    response.setEntity(new GzipInflatingEntity(response.getEntity()));
                    break;
                }
            }
        }
    }
	
	/**
	 * Inflated Response Entity
	 * @author maps
	 * [MOBPLAT-200]: allow gzip, deflate decompression modes
	 */
	private class GzipInflatingEntity extends HttpEntityWrapper {
        public GzipInflatingEntity(final HttpEntity wrapped) {
            super(wrapped);
        }
 
        @Override
        public InputStream getContent() throws IOException {
        	LOG.LogDebug(Module.PLATFORM, "Returning response entity as GZIPInputStream"); 
            return new GZIPInputStream(wrappedEntity.getContent());
        }
 
        @Override
        public long getContentLength() {
            return -1;
        }
    }

	@Override
	public void run() {
		if(this.Validatecertificates()){
			try {
				createHttpClients();
			} catch (KeyManagementException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (UnrecoverableKeyException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (NoSuchAlgorithmException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (CertificateException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (KeyStoreException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		
	}
	
	public void createHttpClients() throws NoSuchAlgorithmException, CertificateException, IOException, KeyStoreException, KeyManagementException, UnrecoverableKeyException{
		
		SSLSocketFactory socketFactory;
		SchemeRegistry registry = new SchemeRegistry();
		
		LOG.LogDebug(Module.PLATFORM, "Certificate Validation Enabled = " + this.Validatecertificates());
		
		if (this.Validatecertificates()) {
			HostnameVerifier hostnameVerifier = org.apache.http.conn.ssl.SSLSocketFactory.ALLOW_ALL_HOSTNAME_VERIFIER;
			// Set verifier
			HttpsURLConnection
					.setDefaultHostnameVerifier(hostnameVerifier);
			
			
			/******************************** 
			 * USING DEFAULT ANDROID DEVICE SSLSocketFactory
			 * the default factory was throwing errors verifying ssl certificates chains for some specific CA Authorities
			 * (for example, Verisign root ceritificate G5 is not available on android devices <=2.3)
			 * See more details on jira ticket [MOBPLAT-63]
			 ******************************** 
			SSLSocketFactory socketFactory = SSLSocketFactory
					.getSocketFactory();
			socketFactory
					.setHostnameVerifier((X509HostnameVerifier) hostnameVerifier);
			*/
			
			/*
	        /******************************** 
			 * USING VALIDATING SSLSocketFactory - Validating certificates per demand
			 * See more details on jira ticket [MOBPLAT-63]
			 ********************************
			 */
			KeyStore trustStore;
			if(Build.VERSION.SDK_INT>=14){
				trustStore = KeyStore.getInstance("AndroidCAStore");
				trustStore.load(null,null);					
			}else{
				try{
					trustStore = KeyStore.getInstance(KeyStore.getDefaultType());;
					String filename = "/system/etc/security/cacerts.bks".replace('/', File.separatorChar);						
					FileInputStream is = new FileInputStream(filename);
					trustStore.load(is,"changeit".toCharArray());
			        is.close();
				}catch(Exception ex)
				{
					try{
						/*
				        /******************************** 
						 * HTC 2.3.5 Access Keystore problem
						 * See more details on jira ticket [MOBPLAT-91]
						 ********************************
						 */
						trustStore = KeyStore.getInstance(KeyStore.getDefaultType());
						String filename = "/system/etc/security/cacerts.bks".replace('/', File.separatorChar);
						FileInputStream is = new FileInputStream(filename);
						trustStore.load(is,null);
				        is.close();
					}catch(Exception e)
					{
						trustStore = null;
						LOG.Log(Module.PLATFORM, "A problem has been detected while accessing the device keystore.",e);
					}
				}
			}
			socketFactory =  ValidatingSSLSocketFactory.GetInstance(trustStore);
	        socketFactory.setHostnameVerifier((X509HostnameVerifier) hostnameVerifier);
	        
	        LOG.LogDebug(Module.PLATFORM, "Using ValidatingSSLSocketFactory (custom socket Factory)");

		} else {
			/*
			 * ******************************* 
			 * USING CUSTOM SSLSocketFactory - accept all certificates
			 * See more details on jira ticket [MOBPLAT-63]
			 ********************************
			*/ 
			KeyStore trustStore = KeyStore.getInstance(KeyStore.getDefaultType());
			trustStore.load(null, null);
			socketFactory =  new MySSLSocketFactory(trustStore);
			
			LOG.LogDebug(Module.PLATFORM, "Using MySSLSocketFactory (custom socket factory - accepting all certificates)");
		}
		
		registry.register(new Scheme("https", socketFactory, 443));
		registry.register(new Scheme("http", PlainSocketFactory.getSocketFactory(), 80));
		ThreadSafeClientConnManager mgr = new ThreadSafeClientConnManager(
				new DefaultHttpClient().getParams(), registry);
		httpSSLClient = new DefaultHttpClient(mgr,
				new DefaultHttpClient().getParams());
		
		LOG.LogDebug(Module.PLATFORM, "httpSSLClient stored for next HTTPS access");
		
	}
    
}