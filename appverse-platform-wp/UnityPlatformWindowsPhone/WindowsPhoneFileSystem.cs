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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Unity.Core.Storage.FileSystem;
using UnityPlatformWindowsPhone.Internals;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhoneFileSystem : AbstractFileSystem, IAppverseService
    {
        private readonly StorageFolder _LocalDocumentsFolder;

        public WindowsPhoneFileSystem()
        {
            _LocalDocumentsFolder = WindowsPhoneUtils.DocumentsFolder;
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
        }

        public override async Task<FileData> CreateFile(string fileName, DirectoryData baseDirectory)
        {
            var folder = await CreateFolderTree(baseDirectory);
            var createdFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            return new FileData(createdFile.Path.Replace(_LocalDocumentsFolder.Path, String.Empty));
        }

        public override async Task<FileData> CreateFile(string fileName)
        {
            return await CreateFile(fileName, null);
        }

        public override async Task<DirectoryData> CreateDirectory(string directoryName, DirectoryData baseDirectory)
        {
            var folder = await CreateFolderTree(baseDirectory);
            var createdFolder = await folder.CreateFolderAsync(directoryName, CreationCollisionOption.OpenIfExists);
            return new DirectoryData(createdFolder.Path.Replace(_LocalDocumentsFolder.Path, String.Empty));
        }

        public override async Task<DirectoryData> CreateDirectory(string directoryName)
        {
            return await CreateDirectory(directoryName, null);
        }

        public override async Task<bool> DeleteFile(FileData file)
        {
            var path = GetFilePath(file);
            var fileToDelete = await GetStorageItem(path, false);
            if (fileToDelete == null) return false;
            await fileToDelete.DeleteAsync(StorageDeleteOption.PermanentDelete);
            return true;
        }

        public override async Task<bool> DeleteDirectory(DirectoryData directory)
        {
            var folderToDelete = await GetStorageItem(GetDirectoryPath(directory), true);
            if (folderToDelete == null || folderToDelete.Path.Equals(_LocalDocumentsFolder.Path) || folderToDelete.Path.Equals(ApplicationData.Current.LocalFolder.Path)) return false;
            await folderToDelete.DeleteAsync(StorageDeleteOption.PermanentDelete);
            return true;
        }

        public override async Task<FileData[]> ListFiles(DirectoryData directory)
        {
            var selectedFolder = await GetStorageItem(GetDirectoryPath(directory), true) as StorageFolder;
            return selectedFolder == null ? null : (await selectedFolder.GetFilesAsync()).Select(file => new FileData(file.Path.Remove(0, _LocalDocumentsFolder.Path.Length))).ToArray();
        }

        public override async Task<DirectoryData[]> ListDirectories(DirectoryData directory)
        {
            var selectedFolder = await GetStorageItem(GetDirectoryPath(directory), true) as StorageFolder;
            return selectedFolder == null ? null : (await selectedFolder.GetFoldersAsync()).Select(folder => new DirectoryData(folder.Path.Remove(0, _LocalDocumentsFolder.Path.Length))).ToArray();
        }

        public override async Task<DirectoryData[]> ListDirectories()
        {
            return await ListDirectories(null);
        }

        public override async Task<DirectoryData> GetDirectoryRoot()
        {
            return new DirectoryData(_LocalDocumentsFolder.Path);
        }

        public override async Task<byte[]> ReadFile(FileData file)
        {
            var fileToRead = await GetStorageItem(GetFilePath(file), false) as StorageFile;
            if (fileToRead == null) return null;
            var buffer = await FileIO.ReadBufferAsync(fileToRead);
            return buffer.ToArray();
        }

        public override async Task<bool> WriteFile(FileData file, byte[] contents, bool append)
        {
            try
            {
                var folder = await CreateFolderTree(new DirectoryData(file.FullName));
                StorageFile targetFile;
                if (append)
                {
                    targetFile = await folder.CreateFileAsync(Path.GetFileName(file.FullName), CreationCollisionOption.OpenIfExists);
                    await FileIO.AppendTextAsync(targetFile, Encoding.UTF8.GetString(contents, 0, contents.Length));
                }
                else
                {
                    targetFile =
                        await folder.CreateFileAsync(Path.GetFileName(file.FullName), CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteBytesAsync(targetFile, contents);
                }
                return true;
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
                return false;
            }
        }

        public override async Task<bool> ExistsFile(FileData file)
        {
            var targetFile = await GetStorageItem(GetFilePath(file), false);
            return targetFile != null;
        }

        public override async Task<bool> ExistsDirectory(DirectoryData directory)
        {
            var targetDirectory = await GetStorageItem(GetDirectoryPath(directory), true);
            return targetDirectory != null;
        }

        public override async Task<bool> CopyFromResources(string fromPath, string toPath)
        {
            var fromFile = await GetStorageItem(GetFilePath(new FileData(fromPath)), false) as StorageFile;
            if (fromFile == null) return false;
            var folder = await CreateFolderTree(new DirectoryData(toPath));
            if (Path.HasExtension(toPath))
            {
                await fromFile.CopyAsync(folder, Path.GetFileName(toPath), NameCollisionOption.ReplaceExisting);
            }
            else
            {
                await fromFile.CopyAsync(folder, fromFile.Name, NameCollisionOption.ReplaceExisting);
            }
            return true;
        }

        public override async Task<bool> CopyFromRemote(string url, string toPath)
        {
            try
            {
                Uri downloadUri;
                if (!Uri.TryCreate(url, UriKind.Absolute, out downloadUri)) return false;
                var folder = await CreateFolderTree(new DirectoryData(toPath));
                var createdFile = await folder.CreateFileAsync(Path.GetFileName(toPath), CreationCollisionOption.ReplaceExisting);
                var downloader = new BackgroundDownloader();
                var downloadOperation = downloader.CreateDownload(downloadUri, createdFile);
                await downloadOperation.StartAsync();
                var responseInfo = downloadOperation.GetResponseInformation();
                return responseInfo.StatusCode == 200;
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
                return false;
            }
        }

        public override async Task<string> StoreFile(string directoryPath, string fileName, byte[] fileData)
        {
            var folder = await CreateFolderTree(new DirectoryData(GetDirectoryPath(new DirectoryData(directoryPath))));
            var fileToStore = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(fileToStore, fileData);
            return fileToStore.Path.Replace(_LocalDocumentsFolder.Path, String.Empty);
        }

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }

        #region Private_Methods


        private string GetItemPath(string path)
        {
            path = path.Replace('/', '\\');
            return !path.StartsWith(_LocalDocumentsFolder.Path) ? Path.Combine(_LocalDocumentsFolder.Path, path) : path;
        }

        private string GetDirectoryPath(DirectoryData folderObject)
        {
            return folderObject == null ? _LocalDocumentsFolder.Path : GetItemPath(folderObject.FullName);
        }

        private string GetFilePath(FileData fileObject)
        {
            if (fileObject == null || String.IsNullOrWhiteSpace(fileObject.FullName)) return String.Empty;
            return GetItemPath(fileObject.FullName);
        }

        private async Task<IStorageItem> GetStorageItem(string path, bool bIsDirectory)
        {
            try
            {
                return bIsDirectory
                    ? (await StorageFolder.GetFolderFromPathAsync(path)) as IStorageItem
                    : await StorageFile.GetFileFromPathAsync(path);
            }
            catch (FileNotFoundException ex)
            {
                WindowsPhoneUtils.Log("File not found at path: " + path);
            }
            catch (UnauthorizedAccessException ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
            }
            catch (Exception ex) { WindowsPhoneUtils.Log(ex.Message); }
            return null;
        }
        private async Task<StorageFolder> CreateFolderTree(DirectoryData baseDirectory)
        {
            if (baseDirectory == null || String.IsNullOrWhiteSpace(baseDirectory.FullName)) return _LocalDocumentsFolder;
            var returnFolder = _LocalDocumentsFolder;
            if (Path.HasExtension(baseDirectory.FullName))
                baseDirectory.FullName = Path.GetDirectoryName(baseDirectory.FullName);
            foreach (var directoryName in baseDirectory.FullName.Split(new[] { @"/", @"\" },
                StringSplitOptions.RemoveEmptyEntries))
            {
                returnFolder = await returnFolder.CreateFolderAsync(directoryName, CreationCollisionOption.OpenIfExists);
            }
            return returnFolder;
        }

        #endregion
    }
}
