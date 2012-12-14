/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://www.appverse.mobi/licenses/apl_v2.0.pdf.

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

import java.io.BufferedOutputStream;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.math.BigInteger;
import java.net.HttpURLConnection;
import java.net.Socket;
import java.net.URI;
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
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.Vector;
import java.util.concurrent.TimeUnit;

import javax.net.ssl.HostnameVerifier;
import javax.net.ssl.HttpsURLConnection;
import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

import org.apache.http.HttpHost;
import org.apache.http.HttpResponse;
import org.apache.http.HttpVersion;
import org.apache.http.client.CookieStore;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.conn.params.ConnRoutePNames;
import org.apache.http.conn.scheme.PlainSocketFactory;
import org.apache.http.conn.scheme.Scheme;
import org.apache.http.conn.scheme.SchemeRegistry;
import org.apache.http.conn.ssl.SSLSocketFactory;
import org.apache.http.conn.ssl.X509HostnameVerifier;
import org.apache.http.cookie.Cookie;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.impl.conn.tsccm.ThreadSafeClientConnManager;
import org.apache.http.params.HttpProtocolParams;
import org.apache.http.protocol.HTTP;
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
import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;
// [MOBPLAT-63] - required for custom sslsocketfactory
// [MOBPLAT-63] - end

// TODO review implementation
public class AndroidIO extends AbstractIO {

	private static final SystemLogger LOG = SystemLogger.getInstance();
	private static String _VALIDATECERTIFICATES = "$ValidateCertificates$";
	
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
	
	private static DefaultHttpClient httpClient = new DefaultHttpClient();
	private static final CookieStore cookieStore = httpClient.getCookieStore();

	public AndroidIO() {
		loadServicesConfig();
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
			LOG.Log(Module.PLATFORM, "LoadConfig error ["
					+ SERVICES_CONFIG_FILE + "]: " + ex.getMessage());
		}
		servicesConfig.setServices(servicesList
				.toArray(new IOService[servicesList.size()]));
	}

	@Override
	// TODO review IOResponse.InvokeService implementation
	public IOResponse InvokeService(IORequest request, IOService service) {

		IOServiceEndpoint endpoint = service.getEndpoint();
		IOResponse response = new IOResponse();
		response.setSession(new IOSessionContext());

		if (service != null) {
			LOG.Log(Module.PLATFORM, "Request content: " + request.getContent());

			if (endpoint == null) {
				LOG.Log(Module.PLATFORM,
						"No endpoint configured for this service name: "
								+ service.getName());
				return response;
			}

			String requestUriString = endpoint.getHost() + ":"
					+ endpoint.getPort() + endpoint.getPath();
			if (endpoint.getPort() == 0) {
				requestUriString = endpoint.getHost() + endpoint.getPath();
			}

			LOG.Log(Module.PLATFORM, "Requesting service: " + requestUriString);
			
			Thread timeoutThread = null;
			
			try {

				if (requestUriString.startsWith(HTTPS_SCHEME)) {
					HostnameVerifier hostnameVerifier = org.apache.http.conn.ssl.SSLSocketFactory.ALLOW_ALL_HOSTNAME_VERIFIER;
					// Set verifier
					HttpsURLConnection
							.setDefaultHostnameVerifier(hostnameVerifier);
					SchemeRegistry registry = new SchemeRegistry();
					
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
					
					/******************************** 
					 * USING CUSTOM SSLSocketFactory - accept all certificates
					 * See more details on jira ticket [MOBPLAT-63]
					 ********************************
					*/ 
					SSLSocketFactory socketFactory;
					if(!Validatecertificates())
					{
						KeyStore trustStore = KeyStore.getInstance(KeyStore.getDefaultType());
						trustStore.load(null, null);
						socketFactory =  new MySSLSocketFactory(trustStore);
					}else{
				        /*
				        /******************************** 
						 * USING VALIDATING SSLSocketFactory - Validating all certificates
						 * See more details on jira ticket [MOBPLAT-63]
						 ********************************
						 */
						KeyStore trustStore;
						if(Build.VERSION.SDK_INT>=14){
							trustStore = KeyStore.getInstance("AndroidCAStore");
							trustStore.load(null,null);					
						}else{
							trustStore = KeyStore.getInstance(KeyStore.getDefaultType());
							String filename = "/system/etc/security/cacerts.bks".replace('/', File.separatorChar);
							FileInputStream is = new FileInputStream(filename);
							trustStore.load(is,"changeit".toCharArray());
					        is.close();
						}
				        socketFactory =  ValidatingSSLSocketFactory.GetInstance(trustStore);
				        socketFactory.setHostnameVerifier((X509HostnameVerifier) hostnameVerifier);
					}
					LOG.Log(Module.PLATFORM, "Certificate Validation Enabled = " + this.Validatecertificates());
					registry.register(new Scheme("https", socketFactory, 443));
					registry.register(new Scheme("http", PlainSocketFactory.getSocketFactory(), 80));
					ThreadSafeClientConnManager mgr = new ThreadSafeClientConnManager(
							httpClient.getParams(), registry);
					httpClient = new DefaultHttpClient(mgr,
							new DefaultHttpClient().getParams());
				} else {
					httpClient = new DefaultHttpClient();
				}
				// preserving the cookies between requests
				httpClient.setCookieStore(cookieStore);
				
				httpClient.getParams().setParameter("http.protocol.version", HttpVersion.HTTP_1_0);  // not chunked requests
				httpClient.getParams().setIntParameter("http.connection.timeout", DEFAULT_READWRITE_TIMEOUT);
				httpClient.getParams().setIntParameter("http.socket.timeout", DEFAULT_RESPONSE_TIMEOUT);
			
				HttpUriRequest httpRequest = null;

				if (endpoint.getProxyUrl() != null
						&& !endpoint.getProxyUrl().equals("")
						&& !endpoint.getProxyUrl().equals("null")) {
					URI proxyUrl = new URI(endpoint.getProxyUrl());
					HttpHost proxyHost = new HttpHost(proxyUrl.getHost(),
							proxyUrl.getPort(), proxyUrl.getScheme());
					httpClient.getParams().setParameter(
							ConnRoutePNames.DEFAULT_PROXY, proxyHost);
				}

				if (service.getRequestMethod() != RequestMethod.POST) {
					// add request content to the URI string when GET method.
					if (request.getContent() != null) {
						requestUriString += request.getContent();
					}
					httpRequest = new HttpGet(requestUriString);
				} else {
					httpRequest = new HttpPost(requestUriString);
					if (request.getContent() != null
							&& request.getContent().length() > 0) {
						((HttpPost) httpRequest).setEntity(new StringEntity(
								request.getContent(), HTTP.UTF_8));
					}
				}

				if (request.getContentType() != null) {
					httpRequest.setHeader("Content-Type",
							request.getContentType());
				} else {
					httpRequest.setHeader("Content-Type",
							contentTypes.get(service.getType()).toString());
				}
				if (request.getHeaders() != null
						&& request.getHeaders().length > 0) {
					for (IOHeader header : request.getHeaders()) {
						httpRequest.setHeader(header.getName(),
								header.getValue());
					}
				}

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

				httpRequest.setHeader("Accept",
						contentTypes.get(service.getType()).toString());
				// httpRequest.setHeader("content-length",
				// String.valueOf(request.getContentLength()));
				httpRequest.setHeader("keep-alive", String.valueOf(false));
				// TODO: set conn timeout
				LOG.Log(Module.PLATFORM, "Downloading service content");

				IOperatingSystem system = (IOperatingSystem) AndroidServiceLocator
						.GetInstance().GetService(
								AndroidServiceLocator.SERVICE_TYPE_SYSTEM);
				HttpProtocolParams.setUserAgent(httpClient.getParams(),
						system.GetOSUserAgent());

				// Throw a new Thread to check absolute timeout
				timeoutThread = new Thread(new CheckTimeoutThread(httpRequest));
				timeoutThread.start();
				
				long start = System.currentTimeMillis();
				HttpResponse httpResponse = httpClient.execute(httpRequest);
				LOG.Log(Module.PLATFORM,
						"Content downloaded in "
								+ (System.currentTimeMillis() - start) + "ms");

				byte[] resultBinary = null;
				String responseMimeTypeOverride = null;
				InputStream responseStream = httpResponse.getEntity()
						.getContent();
				int length = (int) httpResponse.getEntity().getContentLength();

				if (length > Integer.MAX_VALUE) {
					// TODO:responseStream is too large
				}

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

				if (length > 0) {
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

				} else {
					// Read to end of stream, if content length is not provided.
					ByteArrayOutputStream outBuffer = new ByteArrayOutputStream();

					byte[] readBuffer = new byte[256];
					int readLen = 0;
					try {
						while ((readLen = responseStream.read(readBuffer, 0,
								readBuffer.length)) > 0) {
							outBuffer.write(readBuffer, 0, readLen);
						}
					} finally {
						resultBinary = outBuffer.toByteArray();
						outBuffer.close();
						outBuffer = null;
					}
				}
				
				// preserve cache-control header from remote server, if any
				String cacheControlHeader = (httpResponse.getLastHeader("Cache-Control")!=null? httpResponse.getLastHeader("Cache-Control").getValue() : null);
				if (cacheControlHeader != null && !cacheControlHeader.isEmpty()) {
					LOG.Log(Module.PLATFORM, "Found Cache-Control header on response: " + cacheControlHeader + ", using it on internal response...");
					
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

				if (ServiceType.OCTET_BINARY.equals(service.getType())) {
					if (responseMimeTypeOverride != null
							&& !responseMimeTypeOverride.equals(contentTypes
									.get(service.getType()))) {
						response.setContentType(responseMimeTypeOverride);
					} else {
						response.setContentType(contentTypes.get(
								service.getType()).toString());
					}
					response.setContentBinary(resultBinary); // Assign binary
					// content here
				} else {
					response.setContentType(contentTypes.get(service.getType())
							.toString());
					response.setContent(new String(resultBinary));
				}
				
			} catch (Exception ex) {
				LOG.Log(Module.PLATFORM,
						"Unnandled Exception requesting service: "
								+ requestUriString + ".", ex);
				response.setContentType(contentTypes.get(ServiceType.REST_JSON)
						.toString());
				response.setContent("Unhandled Exception Requesting Service: "
						+ requestUriString + ". Message: " + ex.getMessage());
			} finally {
				// abort any previous timeout checking thread
				if(timeoutThread!=null && timeoutThread.isAlive()) {
					timeoutThread.interrupt();
				}
			}
		}

		LOG.Log(Module.PLATFORM, "invoke service finished");
		return response;
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
				
				LOG.Log(Module.PLATFORM, "*** INVOKE SERVICE TIMEOUT *** Absolute timeout checking completed.");
				if(httpRequest != null) {
					LOG.Log(Module.PLATFORM, "*** INVOKE SERVICE TIMEOUT *** Aborting request...");
					httpRequest.abort();
				}
				
			} catch (InterruptedException e) {
				LOG.Log(Module.PLATFORM, "*** INVOKE SERVICE TIMEOUT *** Absolute timeout checking interrupted.");
			}
			
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
					LOG.Log(Module.PLATFORM, "Certificate Validation: Accepting all certificates");
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
	                return null;
	            }
	            public void checkClientTrusted(X509Certificate[] chain, String authType) throws CertificateException {
	            }

	            public void checkServerTrusted(X509Certificate[] chain, String authType) throws CertificateException {
	            	boolean bErrorsFound = false;
	            	try {
	    				LOG.Log(Module.PLATFORM, "Certificate Validation: Starting Certificate Validation process");
	            		//check the certificate is in memory
	            		if(!certificateIsTheSame(chain[0])){
		            		X509CertParser parser = new X509CertParser();
		            		ByteArrayInputStream bais = new ByteArrayInputStream(chain[0].getEncoded());
		    	            parser.engineInit(bais);
		    	            bais.close();
		            		X509CertificateObject cert = (X509CertificateObject)parser.engineRead();
		            		if(certIsValidNow(cert)){
		            			if(!certIsSelfSigned(cert)){
		            				if(certChainIsValid(cert, chain)){
		            					//all checks went OK. Add certificate to memory with current date and time
		            					myCertificateList.put(Integer.valueOf(chain[0].hashCode()), Long.valueOf(System.currentTimeMillis()));
		            					LOG.Log(Module.PLATFORM, "Certificate Validation: Certificate is Valid");
		            				}
		            			}
			            	}
	            		}else{LOG.Log(Module.PLATFORM, "Certificate Validation: Trusted Certificate");}
					} catch (StreamParsingException e) {
						bErrorsFound = true;
					} catch (IOException e) {
						bErrorsFound = true;
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
	            private boolean certChainIsValid(X509CertificateObject cert, X509Certificate[] chain){
	            	if(!verifyCertificateOCSP(chain)){
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
					        LOG.Log(Module.PLATFORM, "Certificate Validation: Certificate Path is valid");
		            		return true;
	            		}catch (CertPathBuilderException e) {
							LOG.Log(Module.PLATFORM, "Certificate Validation: Errors found in the Certificate Path ");
						} catch (NoSuchAlgorithmException e) {
							LOG.Log(Module.PLATFORM, "Certificate Validation: Errors found in the Certificate Path ");
						}catch (InvalidAlgorithmParameterException e) {
							LOG.Log(Module.PLATFORM, "Certificate Validation: Errors found in the Certificate Path ");
						}
	            	}else{return false;}
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
						LOG.Log(Module.PLATFORM, "Certificate Validation: Errors found in the Certificate OCSP Validation ");
						bCertificateIsRevoked = true;
					} catch (OCSPException e) {
						LOG.Log(Module.PLATFORM, "Certificate Validation: Errors found in the Certificate OCSP Validation");
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
						LOG.Log(Module.PLATFORM, "Certificate Validation: Certificate is not expired");
						return true;
					} catch (CertificateExpiredException e) {
						LOG.Log(Module.PLATFORM, "Certificate Validation: Certificate is expired");
					} catch (CertificateNotYetValidException e) {
						LOG.Log(Module.PLATFORM, "Certificate Validation: Certificate is not yet valid");
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
						LOG.Log(Module.PLATFORM, "Certificate Validation: Certificate is self-signed");
						return true;
					} catch (InvalidKeyException e) {
					} catch (CertificateException e) {
					} catch (NoSuchAlgorithmException e) {
					} catch (NoSuchProviderException e) {
					} catch (SignatureException e) {}
	            	LOG.Log(Module.PLATFORM, "Certificate Validation: Certificate is not self signed");
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
			                        LOG.Log(Module.PLATFORM, "Certificate " + cert.getSubjectX500Principal().getName() + " is revoked.");
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
	                	LOG.Log(Module.PLATFORM, "Downloading CRL from : " + crlURL);
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
}