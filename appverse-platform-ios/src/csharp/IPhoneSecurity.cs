/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
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
using System;
using System.IO;
using System.Collections.Specialized;
using Unity.Core.Security;


namespace Unity.Platform.IPhone
{
	public class IPhoneSecurity:AbstractSecurity
	{
		/// <summary>
		/// Paths that usually are found in modified devices
		/// </summary>

		private StringCollection scPathsToCheck = null;
		private StringCollection scPermissionPaths = null;

		public IPhoneSecurity () : base()
		{
			scPathsToCheck = new StringCollection();
			scPathsToCheck.Add("/private/var/stash");
			scPathsToCheck.Add("/Applications/Cydia.app/");
			scPermissionPaths = new StringCollection();
			scPermissionPaths.Add("/System/");
			scPermissionPaths.Add("/private/");
		}


		/// <summary>
		/// Checks if the device has been modified
		/// </summary>
		/// <returns>
		/// TRUE if the device is modified, otherwise FALSE
		/// </returns>
		public override bool IsDeviceModified ()
		{

			bool bDeviceIsModified = false;
			try{
			foreach (String sPath in scPathsToCheck) {
				bDeviceIsModified = CheckPath (sPath);
				if (bDeviceIsModified) return true;
			}

			foreach (string sPath in scPermissionPaths) {
				bDeviceIsModified = CheckPathAccess(sPath);
				if(bDeviceIsModified) return true;
			}
			}catch(Exception ex){
				//Nothing to show in order to avoid showing information about the security measures
			}
			return bDeviceIsModified;
		}

		/// <summary>
		/// Checks the existence of the specified paths.
		/// </summary>
		/// <returns>
		/// TRUE if any specified path exists, otherwise FALSE
		/// </returns>
		/// <param name='scPaths'>
		/// String with the path to be checked
		/// </param>
		private bool CheckPath (string sPath)
		{
			return Directory.Exists(sPath);
		}


		/// <summary>
		/// Checks if we have full access over a directory
		/// </summary>
		/// <returns>
		/// TRUE if we have full access over any of the specified directories
		/// </returns>
		/// <param name='scPaths'>
		/// Directory paths to be checked
		/// </param>
		private bool CheckPathAccess (string sPath)
		{
			//If the folder is READONLY an exception will be thrown, otherwise we have access over the folder.
			try{
				if(CheckPath(sPath)){
					DirectoryInfo dirInfo = new DirectoryInfo(sPath);
					dirInfo.GetAccessControl(System.Security.AccessControl.AccessControlSections.All);
				}else{return false;}
			}catch(UnauthorizedAccessException ex){
				return false;
			}
			return true;
		}
	}
}

