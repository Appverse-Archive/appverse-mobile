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
package com.gft.unity.android.helpers;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.net.Socket;
import java.net.UnknownHostException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.KeyManagementException;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.NoSuchProviderException;
import java.security.Security;
import java.security.SignatureException;
import java.security.UnrecoverableKeyException;
import java.security.cert.CertPathBuilder;
import java.security.cert.CertPathBuilderException;
import java.security.cert.CertStore;
import java.security.cert.CertificateEncodingException;
import java.security.cert.CertificateException;
import java.security.cert.CertificateExpiredException;
import java.security.cert.CertificateNotYetValidException;
import java.security.cert.CollectionCertStoreParameters;
import java.security.cert.PKIXBuilderParameters;
import java.security.cert.PKIXCertPathBuilderResult;
import java.security.cert.TrustAnchor;
import java.security.cert.X509CertSelector;
import java.security.cert.X509Certificate;
import java.util.Arrays;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.TimeUnit;

import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

import org.apache.http.conn.ssl.SSLSocketFactory;
import org.spongycastle.asn1.ASN1Sequence;
import org.spongycastle.asn1.x509.SubjectPublicKeyInfo;
import org.spongycastle.jce.provider.X509CertParser;
import org.spongycastle.jce.provider.X509CertificateObject;
import org.spongycastle.x509.util.StreamParsingException;

import com.gft.unity.android.AndroidIO;
import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

/* NOT PROVIDED IN spongycastle version 1.52
import org.spongycastle.ocsp.BasicOCSPResp;
import org.spongycastle.ocsp.CertificateID;
import org.spongycastle.ocsp.OCSPException;
import org.spongycastle.ocsp.OCSPReq;
import org.spongycastle.ocsp.OCSPReqGenerator;
import org.spongycastle.ocsp.OCSPResp;
import org.spongycastle.ocsp.RevokedStatus;
import org.spongycastle.ocsp.SingleResp;
*/

public class ValidatingSSLSocketFactory extends SSLSocketFactory {
	
	public static final AndroidSystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();
	
    protected static SSLContext sslContext;
    protected static Map<Integer, Long> myCertificateList;
    protected static ValidatingSSLSocketFactory singletonFactory;
    private HashMap<String, String[]> publicKeys;
    private HashMap<String, String[]> fingerprints;
    private KeyStore truststore = null;
    public static ValidatingSSLSocketFactory GetInstance(KeyStore truststore, HashMap<String, String[]> pbks, HashMap<String, String[]> fps)
    {
    	if(singletonFactory==null){
    		try {
				singletonFactory = new ValidatingSSLSocketFactory(truststore, pbks, fps);
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

    private ValidatingSSLSocketFactory(final KeyStore _truststore, HashMap<String, String[]> pbks, HashMap<String, String[]> fps) throws NoSuchAlgorithmException, KeyManagementException, KeyStoreException, UnrecoverableKeyException {
        super(_truststore);
        truststore = _truststore;
        sslContext = SSLContext.getInstance("TLS");
        myCertificateList = new HashMap<Integer, Long>();
        publicKeys = pbks;
        fingerprints = fps;
        Security.addProvider(new org.spongycastle.jce.provider.BouncyCastleProvider());
        
        
    }

    @Override
    public Socket createSocket(Socket socket, String host, int port, boolean autoClose) throws IOException, UnknownHostException {
    	final String requestHostUri = host;
    	LOG.LogDebug(Module.PLATFORM, "ValidatingSSLSocketFactory - Create Socket for host requestHostUri: " + requestHostUri);
    	
    	TrustManager tm = new X509TrustManager() {
        	
        	private int DEFAULT_READWRITE_TIMEOUT = 15000; // 15 seconds timeout establishing connection
        	private int DEFAULT_RESPONSE_TIMEOUT = 100000; // 100 seconds timeout reading response
        	
        	 
        	private char[] HEX_CHARS = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};	        	
        	
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
        			LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Couldnt retrieve Accepted Issuers. " + ex.getMessage());
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
    				X509Certificate endCert = chain[0];
            		if(!certificateIsTheSame(endCert)){
            			if(certChainIsValid(endCert, chain)){
            				/* Checking only last certificate in the chain
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
	            			*/
            				
            				// ThHis magic is needed to verify self-signed
            				X509CertParser parser = new X509CertParser();
		            		ByteArrayInputStream bais = new ByteArrayInputStream(endCert.getEncoded());
		    	            parser.engineInit(bais);
		    	            bais.close();
		    	            
		            		X509CertificateObject cert = (X509CertificateObject)parser.engineRead();
            				if(certIsValidNow(cert)){
		            			if(!certIsSelfSigned(cert)){

		            				if(AndroidIO.ValidateFingerprints()){
		            					LOG.LogDebug(Module.PLATFORM, "Certificate Validation: VALIDATING FINGERPRINT");
			            				if(verifyFingerprint(endCert)){
			            					LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate Fingerprint is valid");
										} else {
											bErrorsFound = true;
											LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate Fingerprint not valid");
										}
		            				}else {
		            					LOG.LogDebug(Module.PLATFORM, "Certificate Validation: DO NOT VALIDATE FINGERPRINT");
		            				}
		            				
		            				if(AndroidIO.ValidatePublicKey()){
		            					LOG.LogDebug(Module.PLATFORM, "Certificate Validation: VALIDATING FINGERPRINT");
			            				if(verifyPublicKey(endCert)){
			            					LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate Public Key Fingerprint is valid");
										} else {
											bErrorsFound = true;
											LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate Public Key Fingerprint not valid");
										}
		            				}else {
		            					LOG.LogDebug(Module.PLATFORM, "Certificate Validation: DO NOT VALIDATE Public Key FINGERPRINT");
		            				}
		            				
		            			}else{ 
		            				bErrorsFound = true;
		            				LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate is Self Signed");
		            			}
			            	}else{ 
			            		bErrorsFound = true;
			            		LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate is expired");
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
            	catch (Exception e) {
            		bErrorsFound = true;
            		LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Unhandled error: " + e.getMessage());
            	}
            	if(bErrorsFound){ throw new CertificateException("Certificate Validation: Errors found in the Certificate Chain");}
            }
            
            /**
             * Checks the certificate fingerprint is the expected
             * @param cert
             * 				Certificate to check
             * @return
             * 				True if the certificate fingerprint was the expected one; false otherwise
             */
        	private boolean verifyFingerprint(X509Certificate endCert){
    			try {
    				LOG.LogDebug(Module.PLATFORM, "ValidatingSSLSocketFactory - verifyFingerprint requestHostUri: " + requestHostUri);
            		MessageDigest md;
					md = MessageDigest.getInstance("SHA1");
					md.update(endCert.getEncoded());
	                String thumbprint = dumpHex(md.digest());
	                
	                
	                
	                String[] fingerprint = null;
	                if(fingerprints.containsKey(requestHostUri)){
	                	fingerprint = fingerprints.get(requestHostUri);
	                	//LOG.LogDebug(Module.PLATFORM, "******** Certificate Validation: requestHostUri ["+ requestHostUri +"] fingerprint "+ Arrays.toString(FINGERPRINTS.get(requestHostUri)));
	                }
	                
	            			            		            	
	            	if(fingerprint != null){
	                	//LOG.LogDebug(Module.PLATFORM, "******** Certificate Validation: allowed fringerprint: " + Arrays.toString(fingerprint));
	                	//LOG.LogDebug(Module.PLATFORM, "******** Certificate Validation: tocheck fringerprint: ["+thumbprint+"]");
	                			                	
	                	if(!Arrays.asList(fingerprint).contains(thumbprint)){
	                	
	                		LOG.LogDebug(Module.PLATFORM, "Certificate Validation: WRONG CERTIFICATE FINGERPRINT");
	                		return false;
	                	}
	                }else{
	                	LOG.LogDebug(Module.PLATFORM, "WARNING Certificate Validation: NO FINGERPRINT FOUND (you should provide a valid fingerprint in your io-services-config.xml in order to validate HTTPS web certificates)");		                	
	                	return false;
	                }
	                return true;
    			}catch (NoSuchAlgorithmException e) {
    				LOG.LogDebug(Module.PLATFORM, "vf - NoSuchAlgorithmException - message: " + e.getMessage());
				} catch (CertificateEncodingException e) {
					LOG.LogDebug(Module.PLATFORM, "vf - CertificateEncodingException - message: " + e.getMessage());
				} catch (Exception e) {
					LOG.LogDebug(Module.PLATFORM, "vf - Unhandled Exception - message: " + e.getMessage());
					
				}
    			return false;
				
        	}
        	
        	/**
        	 * Get certificate fringerprint
        	 * @param data
        	 * @return fingerprint
        	 */
        	private String dumpHex(byte[] data) {
                final int n = data.length;
                final StringBuilder sb = new StringBuilder(n * 3 - 1);
                for (int i = 0; i < n; i++) {
                  if (i > 0) {
                    sb.append(' ');
                  }
                  sb.append(HEX_CHARS[(data[i] >> 4) & 0x0F]);
                  sb.append(HEX_CHARS[data[i] & 0x0F]);
                }
                return sb.toString();
        	}
        	
        	
        	
        	private boolean verifyPublicKey(X509Certificate endCert){
        		try {
    				LOG.LogDebug(Module.PLATFORM, "verifyPublickey");
            		
	                
					SubjectPublicKeyInfo subjectPublicKeyInfo = new SubjectPublicKeyInfo(
						    ASN1Sequence.getInstance(endCert.getPublicKey().getEncoded()));
					byte[] otherEncoded = subjectPublicKeyInfo.parsePublicKey().getEncoded();
					String thumbprint = dumpHex(otherEncoded).toUpperCase().replaceAll(" ", "");
											
	                   String[] fingerprint = null;		                		               
	                if(publicKeys.containsKey(requestHostUri)){
	                	fingerprint = publicKeys.get(requestHostUri);		                	
	                	//LOG.LogDebug(Module.PLATFORM, "******** Certificate Validation: requestHostUri ["+ requestHostUri +"] fingerprint "+ Arrays.toString(publicKeys.get(requestHostUri)));		                
	                }
	            			            		            	
	            	if(fingerprint != null){
	                	//LOG.LogDebug(Module.PLATFORM, "******** Certificate Validation: allowed fringerprint: " + Arrays.toString(fingerprint));
	                	//LOG.LogDebug(Module.PLATFORM, "******** Certificate Validation: tocheck fringerprint: ["+thumbprint+"]");
	                			                	
	                	if(!Arrays.asList(fingerprint).contains(thumbprint)){
	                	
	                		LOG.LogDebug(Module.PLATFORM, "Certificate Validation: WRONG CERTIFICATE PUBLIC KEY");
	                		return false;
	                	}
	                }else{
	                	LOG.LogDebug(Module.PLATFORM, "WARNING Certificate Validation: NO PUBLIC KEY FOUND (you should provide a valid fingerprint in your io-services-config.xml in order to validate HTTPS web certificates)");		                	
	                	return false;
	                }
	                return true;        			
				} catch (Exception e) {
					LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Unhandled error: " + e.getMessage());						
					
				}
    			return false;	
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
            /* OCSP support not provided in the new spongycastle version 1.52
             * 
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
            */
            
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
            		LOG.LogDebug(Module.PLATFORM, "Certificate Validation: checking if self-signed");
					cert.verify(cert.getPublicKey());
					LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Certificate is self-signed");
					for(X509Certificate trustedCert : this.getAcceptedIssuers()){
						//If trusted and cert have same name
						if(trustedCert.getSubjectX500Principal().getName().equals(cert.getIssuerX500Principal().getName())){
							LOG.LogDebug(Module.PLATFORM, "Certificate Validation: Trusted self-signed found.");
							return false;
						}
					}
					/*
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
					/*////////////////
					
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

        try {
			sslContext.init(null, new TrustManager[] { tm }, null);
		} catch (KeyManagementException e) {}
    	
    	
        return sslContext.getSocketFactory().createSocket(socket, host, port, autoClose);
    }
    
    @Override
    public Socket createSocket() throws IOException {
        return sslContext.getSocketFactory().createSocket();
    }
}
