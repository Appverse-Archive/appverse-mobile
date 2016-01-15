/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
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
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Core.Storage.FileSystem;
using System.IO;
using System.Net;
using Unity.Core.System;

namespace Unity.Platform.Windows
{
    public class WindowsFileSystem : AbstractFileSystem
    {

        public static string DEFAULT_ROOT_PATH = "C:\\";


        /// <summary>
        /// Get configured root directory.
        /// </summary>
        /// <returns>Root directory.</returns>
        public override DirectoryData GetDirectoryRoot()
        {
            return new DirectoryData(DEFAULT_ROOT_PATH);
        }

        public override DirectoryData GetDirectoryResources()
        {
            throw new NotImplementedException();
        }

        public override bool CopyFromRemote(string url, string toPath)
        {
            try
            {

                // delete file if already exists

                string path = Path.Combine(this.GetDirectoryRoot().FullName, toPath);

                if (File.Exists(path))
                {
                    this.DeleteFile(new FileData(path));
                }

                byte[] buffer = null;
                // Get remote file contents from remote location.
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                req.KeepAlive = true;

                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    // create an empty file
                    FileData toFile = this.CreateFile(toPath);

                    using (Stream stream = resp.GetResponseStream())
                    {
                        // Read to end of stream
                        MemoryStream memBuffer = new MemoryStream();
                        byte[] readBuffer = new byte[256];
                        int readLen = 0;
                        do
                        {
                            readLen = stream.Read(readBuffer, 0, readBuffer.Length);
                            memBuffer.Write(readBuffer, 0, readLen);
                        } while (readLen > 0);

                        buffer = memBuffer.ToArray();
                        memBuffer.Close();
                        memBuffer = null;
                    }
                    return this.WriteFile(toFile, buffer, false);
                }
            }
            catch (Exception ex)
            {
                SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error copying from [" + url + "] to file [" + toPath + "]", ex);
            }

            return false;
        }
    }
}
