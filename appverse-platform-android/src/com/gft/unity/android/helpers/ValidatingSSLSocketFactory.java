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

import java.io.IOException;
import java.net.Socket;
import java.net.UnknownHostException;
import java.security.KeyManagementException;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.Security;
import java.security.UnrecoverableKeyException;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.Map;

import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

import org.apache.http.conn.ssl.SSLSocketFactory;

import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

public class ValidatingSSLSocketFactory extends SSLSocketFactory {
	
	public static final AndroidSystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();
	
    protected static SSLContext sslContext;
    protected static Map<Integer, Long> myCertificateList;
    protected static ValidatingSSLSocketFactory singletonFactory;
    public String requestHostUri = null;
    private HashMap<String, String[]> publicKeys;
    private HashMap<String, String[]> fingerprints;
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

    private ValidatingSSLSocketFactory(final KeyStore truststore, HashMap<String, String[]> pbks, HashMap<String, String[]> fps) throws NoSuchAlgorithmException, KeyManagementException, KeyStoreException, UnrecoverableKeyException {
        super(truststore);
        sslContext = SSLContext.getInstance("TLS");
        myCertificateList = new HashMap<Integer, Long>();
        publicKeys = pbks;
        fingerprints = fps;
        Security.addProvider(new org.spongycastle.jce.provider.BouncyCastleProvider());
        
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
            	
            }
            
            /**
             * Checks the certificate fingerprint is the expected
             * @param cert
             * 				Certificate to check
             * @return
             * 				True if the certificate fingerprint was the expected one; false otherwise
             */
        	private boolean verifyFingerprint(X509Certificate endCert){
    			
    			return true;
				
        	}
        	
        	/**
        	 * Get certificate fringerprint
        	 * @param data
        	 * @return fingerprint
        	 */
        	private String dumpHex(byte[] data) {
               
                return "";
        	}
        	
        	
        	
        	private boolean verifyPublicKey(X509Certificate endCert){
        		
    			return true;	
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
               
               	return true;
               
            }
            
            /**
             * Remove the certificates that have been accessed and visited after 10 minutes from memory
             */
            private void removeTimedOutCertificates(){
            	
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
            	
				return true;
			}
            
            /**
             * Checks that all the certificates in the chain are not revoked. The checks are made via OCSP 
             * @param chain
             * 				Certificate chain to check
             * @return
             * 				True if any certificate is Revoked, otherwise False
             */
            private boolean verifyCertificateOCSP(X509Certificate[] chain){
            	
            	return true;
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
            	
            	return true;
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
            	
            	return false;
            }
            
            
        };

        sslContext.init(null, new TrustManager[] { tm }, null);
    }

    @Override
    public Socket createSocket(Socket socket, String host, int port, boolean autoClose) throws IOException, UnknownHostException {
    	requestHostUri = host;
    	LOG.LogDebug(Module.PLATFORM, "ValidatingSSLSocketFactory - Create Socket for host requestHostUri: " + requestHostUri);
        return sslContext.getSocketFactory().createSocket(socket, host, port, autoClose);
    }
    
    @Override
    public Socket createSocket() throws IOException {
        return sslContext.getSocketFactory().createSocket();
    }
}
