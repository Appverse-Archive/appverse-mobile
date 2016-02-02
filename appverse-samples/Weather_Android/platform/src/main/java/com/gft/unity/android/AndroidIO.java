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
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.net.MalformedURLException;
import java.net.Socket;
import java.net.URI;
import java.net.URISyntaxException;
import java.net.URL;
import java.net.UnknownHostException;
import java.security.KeyManagementException;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.UnrecoverableKeyException;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Random;
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
import org.apache.http.ProtocolException;
import org.apache.http.client.CookieStore;
import org.apache.http.client.RedirectHandler;
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
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntityBuilder;
import org.apache.http.entity.mime.content.FileBody;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.impl.conn.tsccm.ThreadSafeClientConnManager;
import org.apache.http.params.HttpProtocolParams;
import org.apache.http.protocol.HTTP;
import org.apache.http.protocol.HttpContext;
import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserFactory;

import android.content.Context;
import android.os.Build;

import com.gft.unity.android.helpers.AndroidUtils;
import com.gft.unity.android.helpers.ValidatingSSLSocketFactory;
import com.gft.unity.core.io.AbstractIO;
import com.gft.unity.core.io.AttachmentData;
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
	
	private static String _VALIDATEFINGERPRINTS = "$ValidateFingerprints$";
	
	private static String _VALIDATEPUBLICKEY = "$ValidatePublicKey$";

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
	private static final String FINGERPRINT_ATTRIBUTE = "fingerprint";
	private static final String PUBLICKEY_ATTRIBUTE = "public-key";
	
	private final static char[] MULTIPART_CHARS =
            "-_1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
                 .toCharArray();

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
	
	private final HashMap<String, String[]> FINGERPRINTS;
	
	private final HashMap<String, String[]> PUBLICKEYS;

	public AndroidIO() {
		loadServicesConfig();
		FINGERPRINTS = getFingerprints();
		PUBLICKEYS = getPublicKeys();
		(new Thread(this)).start();
	}
	

	private HashMap<String, String[]> getPublicKeys(){
		HashMap<String, String[]> result = new HashMap<String, String[]>();
		for(IOService serv : servicesConfig.getServices()){	
			String publicKey = serv.getEndpoint().getPublicKey();
			if(publicKey != null){
				try {
					URL aURL = new URL(serv.getEndpoint().getHost());
					result.put(aURL.getHost(),publicKey.split(","));
					//LOG.LogDebug(Module.PLATFORM,"*************** getPublicKey: " +aURL.getHost()+":"+publicKey);
				} catch (MalformedURLException e) {					
					LOG.LogDebug(Module.PLATFORM,"*************** Malformed URL Exception getting public keys");
				}
				
			}
		}
		IOService[] services = servicesConfig.getServices();
		for(IOService serv : services){
			serv.getEndpoint().setFingerprint(null);
		}
		
		return result;
	}
	
	private HashMap<String, String[]> getFingerprints(){
		HashMap<String, String[]> result = new HashMap<String, String[]>();
		for(IOService serv : servicesConfig.getServices()){	
			String fingerprint = serv.getEndpoint().getFingerprint();
			if(fingerprint != null){
				try {
					URL aURL = new URL(serv.getEndpoint().getHost());
					result.put(aURL.getHost(),fingerprint.split(","));
					//LOG.LogDebug(Module.PLATFORM,"*************** getFingerprints: " +aURL.getHost()+":"+serv.getEndpoint().getFingerprint());
				} catch (MalformedURLException e) {					
					LOG.LogDebug(Module.PLATFORM,"*************** Malformed URL Exception getting fingerprints");
				}
				
			}
		}
		IOService[] services = servicesConfig.getServices();
		for(IOService serv : services){
			serv.getEndpoint().setFingerprint(null);
		}
		
		return result;
	}
	
	public boolean Validatecertificates() {
		return Boolean.parseBoolean(AndroidIO._VALIDATECERTIFICATES);
	}
	
	public static boolean ValidateFingerprints() {
		return Boolean.parseBoolean(AndroidIO._VALIDATEFINGERPRINTS);
	}
	
	public static boolean ValidatePublicKey() {
		return Boolean.parseBoolean(AndroidIO._VALIDATEPUBLICKEY);
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
			String fingerprint = "";
			String publicKey = "";
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
						fingerprint = xpp.getAttributeValue(null,
								FINGERPRINT_ATTRIBUTE);		
						publicKey = xpp.getAttributeValue(null,
								PUBLICKEY_ATTRIBUTE);		
						if(fingerprint != null)
							fingerprint = fingerprint.toUpperCase();
						if(publicKey != null)
							publicKey = publicKey.toUpperCase().replaceAll(" ", "");
						serviceEndpoint.setFingerprint(fingerprint);
						serviceEndpoint.setPublicKey(publicKey);
						//LOG.LogDebug(Module.PLATFORM, "LoadConfig publickey [" + publicKey + "]");
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
			httpClient = httpSSLClient; // reusing HTTP SSL client
		}else{
			LOG.LogDebug(Module.PLATFORM, "Applying DefaultHTTPClient");
			httpClient = new DefaultHttpClient();
			// [MOBPLAT-200] : allow gzip, deflate decompression modes
			httpClient.addResponseInterceptor(new GzipHttpResponseInterceptor());
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
		if(request.getStopAutoRedirect()){
			LOG.LogDebug(Module.PLATFORM, "Stopping auto following redirections for this request...");
			
			/* seems deprecated, check */
			 httpClient.setRedirectHandler(new RedirectHandler() {
			 
		        public URI getLocationURI(HttpResponse response,
		                HttpContext context) throws ProtocolException {
					LOG.LogDebug(Module.PLATFORM, "RedirectHandler#getLocationURI");
		            return null;
		        }

		        public boolean isRedirectRequested(HttpResponse response,
		                HttpContext context) {
					LOG.LogDebug(Module.PLATFORM, "RedirectHandler#isRedirectRequested");
		            return false;
		        }
		    });
		    
		} else {
			LOG.LogDebug(Module.PLATFORM, "Allowing auto following redirections for this request...");
			 httpClient.setRedirectHandler(null);
		}
		
	}
	
	
	private HttpEntityEnclosingRequestBase buildWebRequest(IORequest request, IOService service, String requestUriString, String requestMethod ) 
			throws UnsupportedEncodingException, URISyntaxException {
		
		HttpEntityEnclosingRequestBase httpRequest = new HttpAppverse(new URI(requestUriString), requestMethod);
		if(service.getType().equals(ServiceType.MULTIPART_FORM_DATA))
		{
			httpRequest = buildMultipartWebRequest(httpRequest, request, service, requestUriString);
		}
		else{
			/*************
			 * adding content as entity, for request methods != GET
			 *************/
			if(!requestMethod.equalsIgnoreCase(RequestMethod.GET.toString())){
				if (request.getContent() != null
						&& request.getContent().length() > 0) {
					httpRequest.setEntity(new StringEntity(
							request.getContent(), HTTP.UTF_8));
				}
			}else if(!requestMethod.equalsIgnoreCase(RequestMethod.GET.toString())){}
			
			/*************
			 * CONTENT TYPE
			 *************/
			String contentType = contentTypes.get(service.getType()).toString();
			if (request.getContentType() != null) {
				contentType = request.getContentType();
				
			} 
			httpRequest.setHeader("Content-Type", contentType);
			/*************
			 * DEFAULT HEADERS
			 *************/
			httpRequest.setHeader("keep-alive", String.valueOf(false));
			httpRequest.setHeader("Accept", contentType); // Accept header should be the same as the request content type used (could be override by the request, or use the service default)
			
		}
		
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
		// httpRequest.setHeader("content-length",
		// String.valueOf(request.getContentLength()));
		
		
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
	
	private HttpEntityEnclosingRequestBase buildMultipartWebRequest(
			HttpEntityEnclosingRequestBase httpRequest, IORequest request,
			IOService service, String requestUriString) {
		
		try {
			LOG.LogDebug(Module.PLATFORM, "**** [MULTIPART_FORM_DATA]. Building Multipart Web Request...");
			String boundary = createFormDataBoundary();
			MultipartEntityBuilder.create();
			
			MultipartEntityBuilder entityBuilder = MultipartEntityBuilder.create();
			entityBuilder.setBoundary(boundary);
			entityBuilder.setMode(HttpMultipartMode.BROWSER_COMPATIBLE);

			httpRequest = new HttpAppverse(new URI(requestUriString), "POST");
			httpRequest.setHeader("Content-Type", contentTypes.get(ServiceType.MULTIPART_FORM_DATA) + boundary);
			// KO in some servers if Accept header is filled 
            //httpRequest.setHeader("Accept", contentTypes.get(ServiceType.MULTIPART_FORM_DATA));
			httpRequest.setHeader("keep-alive", String.valueOf(true));
			
			//Map<String,String> postData = new HashMap<String,String>();
			if(request.getContent() != null){
				String[] items = request.getContent().replaceAll("[&]+$", "").split("&");
				for(String item :items){
					String[] keyValue = item.split("=");
					entityBuilder.addTextBody(keyValue[0], keyValue[1]);
					//postData.put(keyValue[0], keyValue[1]);
				}
			}
			
			if(request.getAttachment() !=null){
				
				String filesAndroidDirectory = GetDirectoryRoot();
				//LOG.LogDebug(Module.PLATFORM, "**** [MULTIPART_FORM_DATA]. Check files under directory: " + filesAndroidDirectory);
				LOG.LogDebug(Module.PLATFORM, "**** [MULTIPART_FORM_DATA]. Processing attachment files, count: " + request.getAttachment().length);
				int attchCount = 0;
				for(AttachmentData attachData : request.getAttachment()){
					try {
						if(attachData!=null && attachData.getReferenceUrl()!=null) {
							String documentPath = attachData.getReferenceUrl(); // check if need to get a full path
							LOG.LogDebug(Module.PLATFORM, "**** [MULTIPART_FORM_DATA]. #" + attchCount+ " Attaching file from path: " + documentPath);
							entityBuilder.addPart(attachData.getFormFieldKey(), new FileBody(new File(filesAndroidDirectory,documentPath)));
						} else {
							LOG.LogDebug(Module.PLATFORM, "**** [MULTIPART_FORM_DATA]. #" + attchCount+ " Attached file does not provided a valid ReferenceUrl field, or does not contain any data. IGNORED");
						}
						attchCount++;
					} catch (Throwable th) {
						LOG.LogDebug(Module.PLATFORM, "**** [MULTIPART_FORM_DATA]. #" + attchCount+ " Exception attaching file, exception message: "+ th.getMessage());
					}
				}
			}			
			httpRequest.setEntity(entityBuilder.build());
		} catch (Exception ex) {
			LOG.LogDebug(Module.PLATFORM, "Exception generating MultipartEntity Request: " + ex.getMessage());
		}
		return httpRequest;
	}

	private String createFormDataBoundary() {
		StringBuilder buffer = new StringBuilder();
        Random rand = new Random();
        int count = rand.nextInt(11) + 30; // a random size from 30 to 40
        for (int i = 0; i < count; i++) {
        	buffer.append(MULTIPART_CHARS[rand.nextInt(MULTIPART_CHARS.length)]);
        }
		return "---------------------------" + buffer.toString();
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
			LOG.LogDebug(Module.PLATFORM, "cookies store found: " + cookieStore.getCookies().size() );
			// using hashmap to remove duplicated cookies keys
			Map<String,IOCookie> cookiesMap = new HashMap<String,IOCookie>();
			//Map<String,Cookie> cookiesStoreMap = new HashMap<String,Cookie>();

			// rollback
			//IOCookie[] cookies = new IOCookie[cookieStore.getCookies() .size()];
			
			for (int i = 0; i < cookieStore.getCookies().size(); i++) {
				Cookie cookie = cookieStore.getCookies().get(i);
				IOCookie ioCookie = new IOCookie();
				ioCookie.setName(cookie.getName());
				ioCookie.setValue(cookie.getValue());
				LOG.LogDebug(Module.PLATFORM, "cookies found: " + cookie.getName() + "=" + cookie.getValue() );
				
				cookiesMap.put(ioCookie.getName(), ioCookie);
				LOG.LogDebug(Module.PLATFORM, "cookies stored: " +cookiesMap.get(ioCookie.getName()).getName() + "=" + cookiesMap.get(ioCookie.getName()).getValue());
				//cookiesStoreMap.put(cookie.getName(), cookie);
				
				// rollback
				//cookies[i] = ioCookie;
				
			}
			
			// clearing cookie store
			/*
			LOG.LogDebug(Module.PLATFORM, "cookiestor. length: "+ cookieStore.getCookies().size());
			cookieStore.clear();
			LOG.LogDebug(Module.PLATFORM, "cookiestore cleared. length: "+  cookieStore.getCookies().size());
			Cookie[] newCookiesToStore = cookiesStoreMap.values().toArray(new Cookie[0]);
			for(int j=0; j < newCookiesToStore.length; j++) 
				cookieStore.addCookie(newCookiesToStore[j]);
			LOG.LogDebug(Module.PLATFORM, "cookiestore stored. length: "+  cookieStore.getCookies().size());
			*/
			
			
			IOCookie[] cookies = cookiesMap.values().toArray(new IOCookie[0]);
			LOG.LogDebug(Module.PLATFORM, "cookies processed (duplicated removed): " + cookies.length);
			
			// rollback
			//LOG.LogDebug(Module.PLATFORM, "cookies processed: " + cookies.length);
			
			response.getSession().setCookies(cookies);
		}

		/*************
		 * CACHE
		 ************
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
		*/
		
		/*************
		 * Headers HANDLING
		 *************/
		if(httpResponse.getAllHeaders()!=null) {
			List<IOHeader> headers = new ArrayList<IOHeader>();
			for (Header h : httpResponse.getAllHeaders()) {
				IOHeader objHeader = new IOHeader();
				objHeader.setName(h.getName());
				objHeader.setValue(h.getValue());
				headers.add(objHeader);
			}
			response.setHeaders((IOHeader[])headers.toArray(new IOHeader[httpResponse.getAllHeaders().length]));
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
    public void ClearCookieContainer() {
		LOG.LogDebug(Module.PLATFORM, "***** As per project demand... clearing cookie container");
		cookieStore.clear();
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
				LOG.LogDebug(Module.PLATFORM,
						"Unnandled Exception requesting service. Message:" + ex.getMessage());
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
				LOG.LogDebug(Module.PLATFORM,
						"Unnandled Exception requesting service. Message: "+ ex.getMessage());
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
				LOG.LogDebug(Module.PLATFORM, "vc - KeyManagementException - message: " + e.getMessage());
			} catch (UnrecoverableKeyException e) {
				LOG.LogDebug(Module.PLATFORM, "vc - UnrecoverableKeyException - message: " + e.getMessage());
			} catch (NoSuchAlgorithmException e) {
				LOG.LogDebug(Module.PLATFORM, "vc - NoSuchAlgorithmException - message: " + e.getMessage());
			} catch (CertificateException e) {
				LOG.LogDebug(Module.PLATFORM, "vc - CertificateException - message: " + e.getMessage());
			} catch (KeyStoreException e) {
				LOG.LogDebug(Module.PLATFORM, "vc - KeyStoreException - message: " + e.getMessage());
			} catch (IOException e) {
				LOG.LogDebug(Module.PLATFORM, "vc - IOException - message: " + e.getMessage());
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
						LOG.LogDebug(Module.PLATFORM, "A problem has been detected while accessing the device keystore. Message: " + e.getMessage());
					}
				}
			}
			socketFactory =  ValidatingSSLSocketFactory.GetInstance(trustStore, PUBLICKEYS, FINGERPRINTS);
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
		
		// [MOBPLAT-200] : allow gzip, deflate decompression modes
		httpSSLClient.addResponseInterceptor(new GzipHttpResponseInterceptor());
		
		LOG.LogDebug(Module.PLATFORM, "httpSSLClient stored for next HTTPS access");
		
	}
    
}