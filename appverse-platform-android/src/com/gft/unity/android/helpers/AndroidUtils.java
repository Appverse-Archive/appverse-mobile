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
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.util.HashMap;
import java.util.Map;

import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

import android.content.res.AssetManager;


public class AndroidUtils {

	private static final AndroidSystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();
	
	
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
		LOG.LogDebug(Module.PLATFORM, "isResourcesZipped called");
		return false;
	}
	
	public static String toHex(byte[] bytes) {	    
		LOG.LogDebug(Module.PLATFORM, "toHex called");
	    return "";
	}
	
	
	
	private void loadZippedFile() {
		LOG.LogDebug(Module.PLATFORM, "loadZippedFile called");
	}

	
    
    public InputStream getAssetInputStream(AssetManager assetManager, String assetPath) throws IOException {
    	LOG.LogDebug(Module.PLATFORM, "getAssetInputStream called");
    	
		
		InputStream is = assetManager.open(assetPath);
		
		return is;
	
    }
    
    public static Map<String, String> getUrlParameters(String url, boolean includeContextPath, String contextPathKey)
            throws UnsupportedEncodingException {
    	LOG.LogDebug(Module.PLATFORM, "getUrlParameters called");
    	
    	
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
	                if (value == null) {
	                	value = new String();
	                }
	                
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
	
	
	
	
	/* Encryption Method */
    public String encrypt(String message) throws Exception 
    {     	
    	String cipherText = message;
    	
        return cipherText; 
    } 
    
   /* Decryption Method */
    public String decrypt(String message) throws Exception { 
		
    	String decrypted = message;
		
        return decrypted;
    }
	
}
