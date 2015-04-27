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
using System.Collections.Generic;
using System.IO;
#if WP8
using System.Threading.Tasks;
#endif
using Unity.Core.System;

namespace Unity.Core.Storage.FileSystem
{
    public class DirectoryData
    {
        public string FullName { get; set; }

        /// <summary>
        /// Parameterless constructor is needed when parsing jsonstring to object.
        /// </summary>
        public DirectoryData()
        {
        }

        public DirectoryData(string path)
        {
            this.FullName = path;
        }
#if !WP8
		public FileData[] GetFiles ()
		{
			List<FileData> list = new List<FileData> ();
			try {
				DirectoryInfo directory = new DirectoryInfo (this.FullName);
				FileInfo[] files = directory.GetFiles ();
				foreach (FileInfo file in files) {
					FileData fileData = new FileData (file.FullName, file.Length);
					list.Add (fileData);
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module .CORE, "Error getting files for directory [" + this.FullName + "]", e);
			}


			return list.ToArray ();

		}

		public DirectoryData[] GetDirectories ()
		{
			List<DirectoryData> list = new List<DirectoryData> ();
			try {
				DirectoryInfo directory = new DirectoryInfo (this.FullName);
				DirectoryInfo[] directories = directory.GetDirectories ();
				foreach (DirectoryInfo dir in directories) {
					DirectoryData dirData = new DirectoryData (dir.FullName);
					list.Add (dirData);
				}
			} catch (Exception e) {
				SystemLogger.Log (SystemLogger.Module .CORE, "Error getting files for directory [" + this.FullName + "]", e);
			}


			return list.ToArray ();
		}
#else
        public virtual Task<FileData[]> GetFiles()
        {
            return null;
        }

        public virtual Task<FileData[]> GetDirectories()
        {
            return null;
        }

#endif


    }
}
