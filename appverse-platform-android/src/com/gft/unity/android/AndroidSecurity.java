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

import java.io.BufferedReader;
import java.io.File;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;

import android.content.Context;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Build;

import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.core.security.AbstractSecurity;
import com.gft.unity.core.security.KeyPair;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class AndroidSecurity extends AbstractSecurity {
	private static final String LOGGER_MODULE = "ISecurity";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);	
	
	private String SHARED_PACKAGE_NAME = null;
	private String PREFERENCES_FILE_NAME = "AppverseSettings"; // default value
	
	
	public AndroidSecurity() {
		super();
		
		SHARED_PACKAGE_NAME = this.checkProperty("Appverse_Shared_PackageName");
		PREFERENCES_FILE_NAME = this.checkProperty("Appverse_Shared_Preferences_Filename");
	}

	@Override
	public boolean IsDeviceModified() {
		if (checkRootMethod1()){return true;}
		if (checkRootMethod2()){return true;}

		// Tactical solution applied (16/01/2013) ::
		// ICS devices (levels 14 & 15) hang the application when the checkRootMethod3 is used. 
		// Application processes are blocked in that case and the address port could not be used anymore by any app till the device is fully restarted.
		if(Build.VERSION.SDK_INT < 14 || Build.VERSION.SDK_INT > 15) {
        	if (checkRootMethod3()){return true;}
		}
		
        return false;
	}
	
	 private boolean checkRootMethod1(){
	        String buildTags = android.os.Build.TAGS;

	        if (buildTags != null && buildTags.contains("test-keys")) {
	            return true;
	        }
	        return false;
	    }

	    
	 private boolean checkRootMethod2(){
		 try {
			 File file = new File("/system/app/Superuser.apk");
	         if (file.exists()) {
	        	 return true;
	         }
	     } catch (Exception e) { }
	     return false;
	 }

	 private boolean checkRootMethod3() {
		 if (new ExecShell().executeCommand(ExecShell.SHELL_CMD.check_su_binary) != null){
			 return true;
	     }else{
	    	 if (new ExecShell().executeCommand(ExecShell.SHELL_CMD.check_su_permission) != null){
	    		 return true;
	    	 }else return false;
	     }
	 }
	 
	 private static class ExecShell
	 {
		 public static enum SHELL_CMD {
			 check_su_binary(new String[] {"/system/xbin/which","su"}),
			 check_su_permission(new String[] {"su","-v"});
			 
			 String[] command;
		     SHELL_CMD(String[] command){
		    	 this.command = command;
		     }
		     
		     
		 }

		 public ArrayList<String> executeCommand(SHELL_CMD shellCmd){
			 String line = null;
			 ArrayList<String> fullResponse = new ArrayList<String>();
			 Process localProcess = null;
			 
			 try {
				 localProcess = Runtime.getRuntime().exec(shellCmd.command);
			 } catch (Exception e) {
				 return null;
				 //e.printStackTrace();
			 }

			 BufferedReader in = new BufferedReader(new InputStreamReader(localProcess.getInputStream()));
			 BufferedReader er = new BufferedReader(new InputStreamReader(localProcess.getErrorStream()));

			 try {
				 while ((line = in.readLine()) != null) {
					 fullResponse.add(line);
				 }
				 if(fullResponse.size()<=0){
					 while ((line = er.readLine()) != null) {
						 fullResponse.add(line);
					 }
				 }
				 in.close();
				 er.close();
			 } catch (Exception e) {
				 //e.printStackTrace();
			 }
			 return fullResponse;		 
		 }
	 }

	@Override
	public void GetStoredKeyValuePair(String keyname) {
		List<KeyPair> foundKeyPairs = new ArrayList<KeyPair>();
		try {
			SharedPreferences settings = GetOtherAppSharedPreferences();
			if(settings!=null){
				if(settings.contains(keyname)){
					foundKeyPairs.add(new KeyPair(keyname,settings.getString(keyname, null)));
				}
			}else LOGGER.logError("GetStoredKeyValuePair", "Storage Unit is null.");
				
		} catch (Exception e) {
			// TODO Auto-generated catch block
		}
		
		LOGGER.logInfo("GetStoredKeyValuePair", "Keys found in storage: " + foundKeyPairs.size());
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		am.executeJS("Unity.OnKeyValuePairsFound",new Object[]{ foundKeyPairs.toArray()});
	}
	
	@Override
	public void GetStoredKeyValuePairs(String[] keynames) {
		List<KeyPair> foundKeyPairs = new ArrayList<KeyPair>();
		try {
			SharedPreferences settings = GetOtherAppSharedPreferences();
			if(settings != null){
				for(String keyname: keynames){
					if(settings != null && settings.contains(keyname)){
						foundKeyPairs.add(new KeyPair(keyname,settings.getString(keyname, null)));
					}
				}
			}else{
				LOGGER.logError("GetStoredKeyValuePairs", "Storage Unit is null.");
			}
		} catch (Exception ex) {
			
		}
		LOGGER.logInfo("GetStoredKeyValuePairs", "Keys found in storage unit: " + foundKeyPairs.size());
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		am.executeJS("Unity.OnKeyValuePairsFound", new Object[]{foundKeyPairs.toArray()});
		
	}
	@Override
	public void RemoveStoredKeyValuePair(String keyname) {
		// TODO Auto-generated method stub
		List<String> successfullKeyPairs = new ArrayList<String>();
		List<String> failedKeyPairs = new ArrayList<String>(); 
		SharedPreferences settings = GetOtherAppSharedPreferences();
		if(settings!=null){
			if(settings.contains(keyname)){
				Editor ed = settings.edit();
				ed.remove(keyname);
				ed.commit();
				successfullKeyPairs.add(keyname);
			}else
				failedKeyPairs.add(keyname);
		}else{
			LOGGER.logError("RemoveStoredKeyValuePair", "Storage Unit is null.");
		}
		LOGGER.logInfo("RemoveStoredKeyValuePair", "Key removed from storage unit: " + successfullKeyPairs.size() + "; Keys Not removed from storage unit: " + failedKeyPairs.size());
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		am.executeJS("Unity.OnKeyValuePairsRemoveCompleted", new Object[]{successfullKeyPairs.toArray(), failedKeyPairs.toArray()});
		
	}
	@Override
	public void RemoveStoredKeyValuePairs(String[] keynames) {
		List<String> successfullKeyPairs = new ArrayList<String>();
		List<String> failedKeyPairs = new ArrayList<String>(); 
		SharedPreferences settings = GetOtherAppSharedPreferences();
		if(settings!=null){
			for(String keyname:keynames){
				if(settings.contains(keyname)){
					Editor ed = settings.edit();
					ed.remove(keyname);
					ed.commit();
					successfullKeyPairs.add(keyname);
				}else
					failedKeyPairs.add(keyname);
			}
		}else{
			LOGGER.logError("RemoveStoredKeyValuePairs", "Storage Unit is null.");
		}
		
		LOGGER.logInfo("RemoveStoredKeyValuePairs", "Keys removed from storage unit: " + successfullKeyPairs.size() + "; Keys Not removed from storage unit: " + failedKeyPairs.size());
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		am.executeJS("Unity.OnKeyValuePairsRemoveCompleted", new Object[]{successfullKeyPairs.toArray(), failedKeyPairs.toArray()});
		
	}
	@Override
	public void StoreKeyValuePair(KeyPair keypair) {
		List<String> successfullKeyPairs = new ArrayList<String>();
		List<String> failedKeyPairs = new ArrayList<String>();
		SharedPreferences settings = GetOtherAppSharedPreferences();
		if(settings!=null){
			try{
				Editor ed = settings.edit();
				ed.putString(keypair.getKey(), keypair.getValue());
				ed.commit();
				successfullKeyPairs.add(keypair.getKey());
			}catch(Exception ex){
				failedKeyPairs.add(keypair.getKey());
			}
		}else{
			LOGGER.logError("StoreKeyValuePair", "Storage Unit is null.");
		}
		
		LOGGER.logInfo("RemoveStoredKeyValuePairs", "Key stored in storage unit: " + successfullKeyPairs.size() + "; Keys Not stored in storage unit: " + failedKeyPairs.size());
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		am.executeJS("Unity.OnKeyValuePairsStoreCompleted", new Object[]{successfullKeyPairs.toArray(), failedKeyPairs.toArray()});
		
	}
	@Override
	public void StoreKeyValuePairs(KeyPair[] keypairs) {
		List<String> successfullKeyPairs = new ArrayList<String>();
		List<String> failedKeyPairs = new ArrayList<String>(); 
		SharedPreferences settings = GetOtherAppSharedPreferences();
		if(settings!=null){
			for(int i=0; i<keypairs.length; i++){
				try{
					String keyname = keypairs[i].getKey();
					String keyvalue = keypairs[i].getValue();
					Editor ed = settings.edit();
					ed.putString(keyname, keyvalue);
					ed.commit();
					successfullKeyPairs.add(keyname);
				}catch(Exception ex){ failedKeyPairs.add(keypairs[i].getKey());}
			}
		}else{
			LOGGER.logError("StoreKeyValuePairs", "Storage Unit is null.");
		}
		LOGGER.logInfo("StoredKeyValuePairs", "Key stored in storage unit: " + successfullKeyPairs.size() + "; Keys Not stored in storage unit: " + failedKeyPairs.size());
		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		am.executeJS("Unity.OnKeyValuePairsStoreCompleted", new Object[]{successfullKeyPairs.toArray(), failedKeyPairs.toArray()});
	}
	
	
	private SharedPreferences GetOtherAppSharedPreferences(){
		SharedPreferences settings = null; 
		try {
			Context otherAppCtx = AndroidServiceLocator.getContext().createPackageContext(SHARED_PACKAGE_NAME, Context.CONTEXT_IGNORE_SECURITY);
			settings = otherAppCtx.getSharedPreferences(PREFERENCES_FILE_NAME, Context.MODE_MULTI_PROCESS + Context.MODE_PRIVATE);
		} catch (NameNotFoundException e) {
			LOGGER.logError("Opening Storage Unit", "The storage unit could not be accessed.");
		} catch (Exception e) {
			LOGGER.logError("Opening Storage Unit", "The storage unit could not be accessed. Unhanlded error.");
		}
		return settings;
	}
	
	private String checkProperty(String propertyName) {
		try {
			int resourceIdentifier = AndroidServiceLocator.getContext().getResources().getIdentifier(propertyName, "string", 
					AndroidServiceLocator.getContext().getPackageName()); 
			String propertyValue = AndroidServiceLocator.getContext().getResources().getString(resourceIdentifier);
			LOGGER.logInfo("Checking Shared Package Name", propertyName + "? " + propertyValue);
			return propertyValue; 
				
		} catch (Exception ex) {
			LOGGER.logError("Checking Shared Package Name", "Exception getting value for " + propertyName + ": " + ex.getMessage());
			return null;
		}
	}
}