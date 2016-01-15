/*
 Copyright (c) 2015 GFT Appverse, S.L., Sociedad Unipersonal.

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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web;

namespace UnityPlatformWindowsPhone.Internals
{
    public sealed class StreamLocalResolver : IUriToStreamResolver
    {
        private const string DocumentsRootFolderProtocol = "/DocumentsRootFolder/";
        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            if (uri == null) throw new Exception("No file to stream.");
            var path = uri.AbsolutePath;
            return GetContent(path).AsAsyncOperation();
        }

        private async Task<IInputStream> GetContent(string path)
        {
            try
            {
                StorageFile targetFile;
                Uri localUri;
                if (path.StartsWith(DocumentsRootFolderProtocol))
                {
                    path = path.Replace(DocumentsRootFolderProtocol, String.Empty);
                    localUri = new Uri("ms-appdata:///local/Documents" + (!path.StartsWith("/") ? String.Concat("/", path) : path));
                    targetFile = await StorageFile.GetFileFromApplicationUriAsync(localUri);
                    return await targetFile.OpenAsync(FileAccessMode.Read);
                }
                if (path.IndexOf('?') > 0)
                {
                    path = path.Substring(0, path.IndexOf('?'));
                }
                else if (path.IndexOf('#') > 0)
                {
                    path = path.Substring(0, path.IndexOf('#'));
                }
                else if (path.EndsWith("/"))
                {
                    path += "indexWP.html";
                }
                localUri = new Uri("ms-appx:///Html/WebResources/www" + path);
                targetFile = await StorageFile.GetFileFromApplicationUriAsync(localUri);

                return await targetFile.OpenAsync(FileAccessMode.Read).AsTask();
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
            }
            return new MemoryStream().AsInputStream();
        }
    }
}
