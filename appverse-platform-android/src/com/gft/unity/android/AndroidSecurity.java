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

import com.gft.unity.core.security.AbstractSecurity;

public class AndroidSecurity extends AbstractSecurity {

	@Override
	public boolean IsDeviceModified() {
		if (checkRootMethod1()){return true;}
        if (checkRootMethod2()){return true;}
        if (checkRootMethod3()){return true;}
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
	         return false;
	     }
	 }
	 
	 private static class ExecShell
	 {
		 public static enum SHELL_CMD {
			 
			 check_su_binary(new String[] {"/system/xbin/which","su"});
			 
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

			 try {
				 while ((line = in.readLine()) != null) {
					 fullResponse.add(line);
				 }
			 } catch (Exception e) {
				 //e.printStackTrace();
			 }
			 return fullResponse;		 
		 }
	 }
}