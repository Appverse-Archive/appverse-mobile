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
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.math.BigInteger;
import java.security.MessageDigest;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;

import javax.crypto.Cipher;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

import android.content.res.AssetManager;

import com.gft.unity.core.system.SystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;


public class AndroidUtils {

	private static final SystemLogger LOG = SystemLogger.getInstance();
	
	private static String _ResourcesZipped = "$ResourcesZipped$";
	private static String APP_RESOURCES_ZIP = "app-encrypted.zip";
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
		return Boolean.parseBoolean(AndroidUtils._ResourcesZipped);
	}
	
	public static String toHex(byte[] bytes) {
	    BigInteger bi = new BigInteger(1, bytes);
	    return String.format("%0" + (bytes.length << 1) + "X", bi);
	}
	
	private InputStream DecryptInputStream(InputStream is) throws Exception {
		long startTime = System.currentTimeMillis();
		InputStream decrypted_is = null;
		
		// 0. Get binary data from input stream
		ByteArrayOutputStream baos = new ByteArrayOutputStream();
		byte[] buffer = new byte[32*1024];
		int count;
		while ((count = is.read(buffer)) != -1) {
			baos.write(buffer, 0, count);
		}
		is.close();
		byte[] inBytes = baos.toByteArray();
		//LOG.Log(Module.PLATFORM, "# inBytes length: " + inBytes.length);
		baos.close();
		
		long stopTime1 = System.currentTimeMillis();
        long et1 = stopTime1 - startTime;
        LOG.Log(Module.PLATFORM, "# decrypting input data: et1 "+ et1 + " milliseconds");
		
		// 1. Get the 8 byte salt
		byte[] salt =  new byte[SALT_LENGTH];
		System.arraycopy(inBytes, SALT_LENGTH, salt, 0, SALT_LENGTH);
		//LOG.Log(Module.PLATFORM, "# using salt: " + AndroidUtils.toHex(salt));
		
		// 2. Remove salt from file data
		int saltedLength = SALT_LENGTH * 2;
		int aesDataLength = inBytes.length - saltedLength;
		byte[] aesData = new byte[aesDataLength];
		System.arraycopy(inBytes, saltedLength, aesData, 0, aesDataLength);
		//LOG.Log(Module.PLATFORM, "# aesData length: " + aesData.length);
		
		// 3. Create Key and IV from password
	    byte[] password = "hashB".getBytes("UTF8");
	    MessageDigest md = MessageDigest.getInstance("MD5");
	    
	    int preKeyLength = password.length + salt.length;
		byte[] preKey = new byte[preKeyLength];
		System.arraycopy(password, 0, preKey, 0, password.length);
		System.arraycopy(salt, 0, preKey, password.length, salt.length);
		byte[] key = md.digest(preKey);
		//LOG.Log(Module.PLATFORM, "# using key: " + AndroidUtils.toHex(key));
		
		int preIVLength = key.length + preKeyLength;
		byte[] preIV = new byte[preIVLength];
		System.arraycopy(key, 0, preIV, 0, key.length);
		System.arraycopy(preKey, 0, preIV, key.length, preKey.length);
		byte[] iv = md.digest(preIV);
		//LOG.Log(Module.PLATFORM, "# using iv: " + AndroidUtils.toHex(iv));
		
		long stopTime5 = System.currentTimeMillis();
        
	    Cipher cipher = Cipher.getInstance("AES/CBC/NoPadding");
	    SecretKeySpec k = new SecretKeySpec(key, "AES");
	    cipher.init(Cipher.DECRYPT_MODE, k, new IvParameterSpec(iv));
	    
	    byte[] decryptedBytes = cipher.doFinal(aesData);
	    decrypted_is = new ByteArrayInputStream(decryptedBytes);
	    
	    long stopTime6 = System.currentTimeMillis();
        long et6= stopTime6 - stopTime5;
        LOG.Log(Module.PLATFORM, "# decrypting input data uplasted "+ et6 + " milliseconds");
	   
	     
		LOG.Log(Module.PLATFORM, "# outBytes length: " + decryptedBytes.length);
		return decrypted_is;
		
	}
	
	private void loadZippedFile() {
		
		long startTime = System.currentTimeMillis();
		List<String> entries = new ArrayList<String>();
		try {
			
			InputStream is = AndroidServiceLocator.getContext().getAssets().open(APP_RESOURCES_ZIP);
			/*
			long stopTime1 = System.currentTimeMillis();
            long et1 = stopTime1 - startTime;
            LOG.Log(Module.PLATFORM, "# Time elapsed loading zipped file: et1: "+ et1 + " milliseconds");
            */
            InputStream dis = this.DecryptInputStream(is);
            /*
            long stopTime2 = System.currentTimeMillis();
            long et2 = stopTime2 - stopTime1;
            LOG.Log(Module.PLATFORM, "# Time elapsed loading zipped file: et2: "+ et2 + " milliseconds");
            */
			ZipInputStream zis = new ZipInputStream(new BufferedInputStream(dis));
			/*
			long stopTime3 = System.currentTimeMillis();
            long et3 = stopTime3 - stopTime2;
            LOG.Log(Module.PLATFORM, "# Time elapsed loading zipped file: et3: "+ et3 + " milliseconds");
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
            LOG.Log(Module.PLATFORM, "# Time elapsed loading zipped file: et4: "+ et4 + " milliseconds");
			*/
		} catch (Exception ex) {
			LOG.Log(Module.PLATFORM, "# Exception loading resources as zipped file: " + ex.getMessage(), ex);
		}
		
		_zipEntriesNames = (String[]) entries.toArray(new String[0]);
		
		long stopTime = System.currentTimeMillis();
		
		//long elapsedTime = ((stopTime - startTime) / 1000); // in seconds
		long elapsedTime = stopTime - startTime; // milliseconds
		
		LOG.Log(Module.PLATFORM, "# Time elapsed loading zipped file: "+ elapsedTime + " milliseconds");
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
				LOG.Log(Module.PLATFORM, "# File not found: " + assetPath);
				return null;
			}
			
			//long elapsedTime = et1 + et2; // milliseconds
			//LOG.Log(Module.PLATFORM, "CSV," + assetPath + ","+ uncompressedSize + "," + compressedSize+ ","+ et1 + "," + et2 + ","+ elapsedTime);
			return is;
			
		} else {
			//long startTime = System.currentTimeMillis();
			InputStream is = assetManager.open(assetPath);
			//long stopTime = System.currentTimeMillis();
            //long elapsedTime = stopTime - startTime; // milliseconds
            
            //LOG.Log(Module.PLATFORM, "CSV not-zipped," + assetPath + ","+ is.available() + ","+ elapsedTime);
            //LOG.Log(Module.PLATFORM, "# Resources are zipped, getting asset from zip file: " + assetPath + ", " +  elapsedTime + " milliseconds");
            
			return is;
		}
    }
	
}
