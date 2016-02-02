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

import java.io.BufferedInputStream;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.math.BigInteger;
import java.net.URLDecoder;
import java.security.MessageDigest;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;
import java.util.zip.ZipInputStream;

import javax.crypto.Cipher;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

import android.content.pm.ApplicationInfo;
import android.content.res.AssetManager;
import android.util.Base64;

import com.gft.unity.android.AndroidSecurity;
import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;


public class AndroidUtils {

	private static final AndroidSystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();
	
	private String _RZ = "$ResourcesZipped$";
	private static int SALT_LENGTH = 8;
	
	//private ZipFile _zipFile = null;
	private String[] _zipEntriesNames = null;
	private Map<String, ZipEntry> _zipEntries = new HashMap<String, ZipEntry>();
	private Map<String, byte[]> _zipEntriesData = new HashMap<String, byte[]>();

	private static AndroidUtils singleton = null;
	
	public static AndroidUtils getInstance() {

		if (singleton == null) {
			singleton = new AndroidUtils();
			if(singleton.isResourcesZipped()) {
				singleton.loadZippedFile();
			}
		}
		
		return singleton;
	}
	
	public boolean isResourcesZipped() {
		return Boolean.parseBoolean(_RZ);
	}
	
	public static String toHex(byte[] bytes) {
	    BigInteger bi = new BigInteger(1, bytes);
	    return String.format("%0" + (bytes.length << 1) + "X", bi);
	}
	
	private InputStream DecryptInputStream(InputStream is) throws Exception {
		long startTime = System.currentTimeMillis();
		InputStream decrypted_is = null;
		
		byte[] aauep = getA_AU_EP ();
		
		// 0. Get binary data from input stream
		ByteArrayOutputStream baos = new ByteArrayOutputStream();
		byte[] buffer = new byte[32*1024];
		int count;
		while ((count = is.read(buffer)) != -1) {
			baos.write(buffer, 0, count);
		}
		is.close();
		byte[] inBytes = baos.toByteArray();
		//LOG.LogDebug(Module.PLATFORM, "# inBytes length: " + inBytes.length);
		baos.close();
		
		long stopTime1 = System.currentTimeMillis();
        long et1 = stopTime1 - startTime;
        LOG.LogDebug(Module.PLATFORM, "# decrypting input data: et1 "+ et1 + " milliseconds");
		
		// 1. Get the 8 byte salt
		byte[] salt =  new byte[SALT_LENGTH];
		System.arraycopy(inBytes, SALT_LENGTH, salt, 0, SALT_LENGTH);
		//LOG.LogDebug(Module.PLATFORM, "# using salt: " + AndroidUtils.toHex(salt));
		
		// 2. Remove salt from file data
		int saltedLength = SALT_LENGTH * 2;
		int aesDataLength = inBytes.length - saltedLength;
		byte[] aesData = new byte[aesDataLength];
		System.arraycopy(inBytes, saltedLength, aesData, 0, aesDataLength);
		//LOG.LogDebug(Module.PLATFORM, "# aesData length: " + aesData.length);
		
		// 3. Create Key and IV from password
	    MessageDigest md = MessageDigest.getInstance("MD5");
	   
	    int preKeyLength = aauep.length + salt.length;
		byte[] preKey = new byte[preKeyLength];
		System.arraycopy(aauep, 0, preKey, 0, aauep.length);
		System.arraycopy(salt, 0, preKey, aauep.length, salt.length);
		byte[] key = md.digest(preKey);
		aauep = null;
		//LOG.LogDebug(Module.PLATFORM, "# using key: " + AndroidUtils.toHex(key));
		
		int preIVLength = key.length + preKeyLength;
		byte[] preIV = new byte[preIVLength];
		System.arraycopy(key, 0, preIV, 0, key.length);
		System.arraycopy(preKey, 0, preIV, key.length, preKey.length);
		byte[] iv = md.digest(preIV);
		//LOG.LogDebug(Module.PLATFORM, "# using iv: " + AndroidUtils.toHex(iv));
		
		long stopTime5 = System.currentTimeMillis();
        
	    Cipher cipher = Cipher.getInstance("AES/CBC/NoPadding");
	    SecretKeySpec k = new SecretKeySpec(key, "AES");
	    cipher.init(Cipher.DECRYPT_MODE, k, new IvParameterSpec(iv));
	    
	    byte[] decryptedBytes = cipher.doFinal(aesData);
	    decrypted_is = new ByteArrayInputStream(decryptedBytes);
	    
	    long stopTime6 = System.currentTimeMillis();
        long et6= stopTime6 - stopTime5;
        LOG.LogDebug(Module.PLATFORM, "# decrypting input data uplasted "+ et6 + " milliseconds");
	   
	     
		LOG.LogDebug(Module.PLATFORM, "# outBytes length: " + decryptedBytes.length);
		return decrypted_is;
		
	}
	
	private byte[] getA_AU_EP () {
		 ZipFile zf = null;
		try{
			 ApplicationInfo ai = AndroidServiceLocator.getContext().getPackageManager().getApplicationInfo(AndroidServiceLocator.getContext().getPackageName(), 0);
		     zf = new ZipFile(ai.sourceDir);
		     ZipEntry ze = zf.getEntry(new String(getA_MWFF()));
		     
		     InputStream is =  null;
		     try {
		    	 is = zf.getInputStream(ze);
		    	 
		    	 ByteArrayOutputStream baos = new ByteArrayOutputStream();
					byte[] buffer = new byte[32*1024];
					int count;
					while ((count = is.read(buffer)) != -1) {
						baos.write(buffer, 0, count);
					}
					byte[] bytes = baos.toByteArray();
					
				    MessageDigest md = MessageDigest.getInstance("MD5");
				    md.update(bytes);
				    byte[] md5checksumBytes = md.digest();
				    
				    String md5checksum = "";

			        for (int i=0; i < md5checksumBytes.length; i++) {
			        	md5checksum += Integer.toString( ( md5checksumBytes[i] & 0xff ) + 0x100, 16).substring( 1 );  //convert to hex
			        }
			        
				    return md5checksum.getBytes("UTF8");
		    	 
		     } catch (Exception ex) {
				LOG.LogDebug(Module.PLATFORM, "# Exception: " + ex.getMessage());
		     } finally {
		    	 if (is!=null) is.close();
		     }
		     
	    }catch(Exception e){
		} finally {
			if(zf !=null) try{ zf.close(); }catch(Exception e){};
		}
		return new byte[0];
	}
	
	private void loadZippedFile() {
		
		long startTime = System.currentTimeMillis();
		List<String> entries = new ArrayList<String>();
		try {
			
			InputStream is = AndroidServiceLocator.getContext().getAssets().open(new String(getA_MWFZF ()));
			/*
			long stopTime1 = System.currentTimeMillis();
            long et1 = stopTime1 - startTime;
            LOG.LogDebug(Module.PLATFORM, "# Time elapsed loading zipped file: et1: "+ et1 + " milliseconds");
            */
            InputStream dis = this.DecryptInputStream(is);
            /*
            long stopTime2 = System.currentTimeMillis();
            long et2 = stopTime2 - stopTime1;
            LOG.LogDebug(Module.PLATFORM, "# Time elapsed loading zipped file: et2: "+ et2 + " milliseconds");
            */
			ZipInputStream zis = new ZipInputStream(new BufferedInputStream(dis));
			/*
			long stopTime3 = System.currentTimeMillis();
            long et3 = stopTime3 - stopTime2;
            LOG.LogDebug(Module.PLATFORM, "# Time elapsed loading zipped file: et3: "+ et3 + " milliseconds");
            */
			try {
				ZipEntry ze;
				while ((ze = zis.getNextEntry()) != null) {
					ByteArrayOutputStream baos = new ByteArrayOutputStream();
					byte[] buffer = new byte[32*1024];
					int count;
					while ((count = zis.read(buffer)) != -1) {
						baos.write(buffer, 0, count);
					}
					String filename = ze.getName();
					byte[] bytes = baos.toByteArray();
					
					entries.add(ze.getName());
					_zipEntries.put(ze.getName(), ze);
					_zipEntriesData.put(filename, bytes);
				}
			 } finally {
			     zis.close();
			 }
			/*
			long stopTime4 = System.currentTimeMillis();
            long et4 =  stopTime4 - stopTime3;
            LOG.LogDebug(Module.PLATFORM, "# Time elapsed loading zipped file: et4: "+ et4 + " milliseconds");
			*/
		} catch (Exception ex) {
			LOG.LogDebug(Module.PLATFORM, "# Exception loading resources as zipped file: " + ex.getMessage());
		}
		
		_zipEntriesNames = (String[]) entries.toArray(new String[0]);
		
		long stopTime = System.currentTimeMillis();
		
		//long elapsedTime = ((stopTime - startTime) / 1000); // in seconds
		long elapsedTime = stopTime - startTime; // milliseconds
		
		LOG.LogDebug(Module.PLATFORM, "# Time elapsed loading zipped file: "+ elapsedTime + " milliseconds");
	}

	/**
     * Getting zip entry from stored hashmap
     * @param entryName
     * @return
     */
    private ZipEntry getZipEntry(String entryName) {
		if(_zipEntries != null && _zipEntries.containsKey(entryName))  {
			return _zipEntries.get(entryName);
		} else {
			return null;
		}
	}
    
    /**
     * Getting inputstream from zip file entry given its name (from stored hahsmap)
     * @param entryName
     * @return
     */
    private InputStream getZipEntryInputStream(String entryName) {
		if(_zipEntriesData != null && _zipEntriesData.containsKey(entryName))  {
			byte[] data = _zipEntriesData.get(entryName);
			return new ByteArrayInputStream(data);
		} else {
			return null;
		}
	}
    
    public InputStream getAssetInputStream(AssetManager assetManager, String assetPath) throws IOException {
    	
    	if(this.isResourcesZipped()) {
    		
    		InputStream is = null;
			/*
			long startTime = System.currentTimeMillis();
			long et1 = 0;
			long et2 = 0;
			long uncompressedSize = 0;
			long compressedSize = 0;
			*/
			ZipEntry entry = this.getZipEntry(assetPath);
			if(entry != null) {
				//long stopTime1 = System.currentTimeMillis();
				//et1 = stopTime1 - startTime;
				
				//compressedSize = entry.getCompressedSize();
				//uncompressedSize = entry.getSize();
				
				is = this.getZipEntryInputStream(assetPath);
				//long stopTime2 = System.currentTimeMillis();
				//et2 = stopTime2 - stopTime1;
				
			} else {
				LOG.LogDebug(Module.PLATFORM, "# File not found: " + assetPath);
				return null;
			}
			
			//long elapsedTime = et1 + et2; // milliseconds
			//LOG.LogDebug(Module.PLATFORM, "CSV," + assetPath + ","+ uncompressedSize + "," + compressedSize+ ","+ et1 + "," + et2 + ","+ elapsedTime);
			return is;
			
		} else {
			//long startTime = System.currentTimeMillis();
			InputStream is = assetManager.open(assetPath);
			//long stopTime = System.currentTimeMillis();
            //long elapsedTime = stopTime - startTime; // milliseconds
            
            //LOG.LogDebug(Module.PLATFORM, "CSV not-zipped," + assetPath + ","+ is.available() + ","+ elapsedTime);
            //LOG.LogDebug(Module.PLATFORM, "# Resources are zipped, getting asset from zip file: " + assetPath + ", " +  elapsedTime + " milliseconds");
            
			return is;
		}
    }
    
    public static Map<String, String> getUrlParameters(String url, boolean includeContextPath, String contextPathKey)
            throws UnsupportedEncodingException {
        //Map<String, List<String>> params = new HashMap<String, List<String>>();
    	Map<String, String> params = new HashMap<String, String>();
    	if(url!=null){ // prevent errors if passing null url string
	        String[] urlParts = url.split("\\?");
	        if (urlParts.length > 1) {
	            String query = urlParts[1];
	            for (String param : query.split("&")) {
	                String pair[] = param.split("=");
	                String key = URLDecoder.decode(pair[0], "UTF-8");
	                String value = "";
	                if (pair.length > 1) {
	                    value = URLDecoder.decode(pair[1], "UTF-8");
	                }
	                //List<String> values = params.get(key);
	                //if (values == null) {
	                if (value == null) {
	                    //values = new ArrayList<String>();
	                	value = new String();
	                    //params.put(key, values);
	                }
	                //values.add(value);
	                
	                params.put(key, value);
	            }
	            
	            if(includeContextPath) {
	            	params.put((contextPathKey!=null?contextPathKey:"context_path"), urlParts[0]);
	            }
	        }
    	} else {
    		LOG.LogDebug(Module.PLATFORM, "AndroidUtils could  not parse a NULL url (method: getUrlParameters())");
    	}
        return params;
    }
    
	public static String checkStringsProperty(String propertyName) {
		try {
			int resourceIdentifier = AndroidServiceLocator.getContext().getResources().getIdentifier(propertyName, "string", 
					AndroidServiceLocator.getContext().getPackageName()); 
			String propertyValue = AndroidServiceLocator.getContext().getResources().getString(resourceIdentifier);
			LOG.LogDebug(Module.PLATFORM, "Checking Strings property: " + propertyName + "? " + propertyValue);
			return propertyValue; 
				
		} catch (Exception ex) {
			LOG.LogDebug(Module.PLATFORM, "Checking Strings property. Exception getting value for " + propertyName + ": " + ex.getMessage());
			return null;
		}
	}
	
	private char[] getA_MWFF () {
		return new char[] {0x63, 0x6c, 0x61, 0x73, 0x73, 0x65, 0x73, 0x2e, 0x64, 0x65, 0x78 } ;
	}
	
	private char[] getA_MWFZF () {
		return new char[] { 0x61, 0x70, 0x70, 0x2d, 0x65, 0x6e, 0x63, 0x72, 0x79, 0x70, 0x74, 0x65, 0x64, 0x2e, 0x7a, 0x69, 0x70}; 
	}
	
	private char[] getA_MWFZF2 () {
		return new char[] { 0x61, 0x73, 0x73, 0x65, 0x74, 0x73, 0x2f, 0x61, 0x70, 0x70, 0x2d, 0x65, 0x6e, 0x63, 0x72, 0x79, 0x70, 0x74, 0x65, 0x64, 0x2e, 0x7a, 0x69, 0x70}; 
	}
	
	private String md(){
		String md = AndroidServiceLocator.getContext().getPackageName()+"_"+AndroidSecurity.class.getName();
		return md;
	}
	
	/* Encryption Method */
    public String encrypt(String message) throws Exception 
    {     	
    	String cipherText = message;
    	try{      
    		
    		byte[] message1=Base64.encode(message.getBytes(),Base64.DEFAULT);
    		byte[] salt = md().getBytes();
    		salt = Arrays.copyOf(salt, 32);
    	    SecretKeySpec key = new SecretKeySpec(salt, "AES");
    	    Cipher c = Cipher.getInstance("AES");
    	    c.init(Cipher.ENCRYPT_MODE, key);
    	    byte[] encVal = c.doFinal(message1);
    	    cipherText=Base64.encodeToString(encVal, Base64.DEFAULT);
    	    
    	}catch(Exception e){
    		LOG.LogDebug(Module.PLATFORM, "encryption error: " + e.getMessage());	
    	}
        return cipherText; 
    } 
    
   /* Decryption Method */
    public String decrypt(String message) throws Exception { 
		
    	String decrypted = message;
		try{
			
		    Cipher c = Cipher.getInstance("AES");
		    byte[] salt = md().getBytes();
    		salt = Arrays.copyOf(salt, 32);
    	    SecretKeySpec key = new SecretKeySpec(salt, "AES");
		    c.init(Cipher.DECRYPT_MODE, key);
		    byte[] decordedValue = Base64.decode(message.getBytes(), Base64.DEFAULT);
		    byte[] decValue = c.doFinal(decordedValue);
		    String decryptedValue = new String(decValue);
		    decrypted=new String(Base64.decode(decryptedValue, Base64.DEFAULT));
		   
			
	       
	    }catch(Exception e){
	    	LOG.LogDebug(Module.PLATFORM, "decryption error: " + e.getMessage());	
		}
        return decrypted;
    }
	
}
