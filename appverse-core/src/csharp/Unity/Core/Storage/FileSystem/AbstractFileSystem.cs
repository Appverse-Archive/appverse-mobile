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
#if WP8
using System.Threading.Tasks;
#endif
using Unity.Core.System;

namespace Unity.Core.Storage.FileSystem
{
    public abstract class AbstractFileSystem : IFileSystem
    {
#if !WP8
        #region Miembros de IFileSystem

        /// <summary>
        /// Creates a file under the given directory.
        /// </summary>
        /// <param name="baseDirectory">The base Directory to create file under it.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>The file created, or null if file cannot be created.</returns>
        public FileData CreateFile(string fileName, DirectoryData baseDirectory)
        {
            FileData file = null;
            string path = Path.Combine(baseDirectory.FullName, fileName);
            //TODO Review
            if (!CheckSecurePath(path))
                return null;
            string rootPath = GetDirectoryRoot().FullName;

            if (baseDirectory != null)
            {
                if (!path.StartsWith(rootPath))
                {
                    path = Path.Combine(rootPath, path);  // concats root path to the given base path.
                }
            }
            else
            {
                path = rootPath; // creates file under root path.
            }

            if (!File.Exists(path))
            {
                try
                {
                    FileStream fs = File.Create(path);
                    file = new FileData(path, fs.Length);
                    fs.Close();
                }
                catch (Exception e)
                {
                    SystemLogger.Log(SystemLogger.Module.CORE, "Error creating file [" + path + "]", e);
                }
            }

            return file;
        }

        /// <summary>
        /// Creates a file under the root directory.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The file created, or null if file cannot be created.</returns>
        public FileData CreateFile(string fileName)
        {
            return CreateFile(fileName, GetDirectoryRoot());
        }

        /// <summary>
        /// Creates a directory under the given base directory.
        /// </summary>
        /// <param name="baseDirectory">>The base Directory to create directory under it.</param>
        /// <param name="directoryName">The directory name. If null, directory is created under root path.</param>
        /// <returns>The directory created, or null if file cannot be created.</returns>
        public DirectoryData CreateDirectory(string directoryName, DirectoryData baseDirectory)
        {
            DirectoryData directory = null;
            string path = Path.Combine(baseDirectory.FullName, directoryName);
            //TODO Review
            if (!CheckSecurePath(path))
                return null;
            string rootPath = GetDirectoryRoot().FullName;
            if (baseDirectory != null)
            {
                if (!path.StartsWith(rootPath))
                {
                    path = Path.Combine(rootPath, path);  // concats root path to the given base path.
                }
            }
            else
            {
                path = rootPath; // creates directory under root path.
            }
            if (!ExistsDirectory(new DirectoryData(path)))
            {  // creates directory if it does not already exist.
                try
                {
                    directory = new DirectoryData(Directory.CreateDirectory(path).FullName);
                }
                catch (Exception e)
                {
                    SystemLogger.Log(SystemLogger.Module.CORE, "Error creating directory [" + path + "]", e);
                }
            }

            return directory;
        }

        /// <summary>
        /// Creates a directory under the root directory.
        /// </summary>
        /// <param name="directoryName">The directory name.</param>
        /// <returns>The directory created, or null if file cannot be created.</returns>
        public DirectoryData CreateDirectory(string directoryName)
        {
            return CreateDirectory(directoryName, GetDirectoryRoot());
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="file">The file to be deleted.</param>
        /// <returns>True on successful deletion.</returns>
        public bool DeleteFile(FileData file)
        {
            bool deleted = false;
            if (file != null)
            {
                string roothPath = GetDirectoryRoot().FullName;
                try
                {
                    if (!file.FullName.StartsWith(roothPath))
                    {
                        file.FullName = Path.Combine(roothPath, file.FullName);
                    }
                    //TODO Review
                    if (!CheckSecurePath(file.FullName))
                        return false;
                    File.Delete(file.FullName);
                    deleted = true;
                }
                catch (Exception e)
                {
                    SystemLogger.Log(SystemLogger.Module.CORE, "Error deleting file [" + file.FullName + "]", e);
                }
            }

            return deleted;
        }

        /// <summary>
        /// Deletes a directory (deleting first all its children).
        /// </summary>
        /// <param name="directory">The directory to be deleted.</param>
        /// <returns>True on successful deletion.</returns>
        public bool DeleteDirectory(DirectoryData directory)
        {
            bool deleted = false;
            if (directory != null)
            {
                string roothPath = GetDirectoryRoot().FullName;
                try
                {
                    if (!directory.FullName.StartsWith(roothPath))
                    {
                        directory.FullName = Path.Combine(roothPath, directory.FullName);
                    }
                    //TODO Review
                    if (!CheckSecurePath(directory.FullName))
                        return false;
                    Directory.Delete(directory.FullName, true); // recursively delete contents
                    deleted = true;
                }
                catch (Exception e)
                {
                    SystemLogger.Log(SystemLogger.Module.CORE, "Error deleting directory [" + directory.FullName + "]", e);
                }
            }

            return deleted;
        }

        /// <summary>
        /// List all files under given directory.
        /// </summary>
        /// <param name="directory">The directory to list files under it.</param>
        /// <returns>List of files.</returns>
        public FileData[] ListFiles(DirectoryData directory)
        {
            if (directory == null)
            {
                return GetDirectoryRoot().GetFiles();
            }

            string roothPath = GetDirectoryRoot().FullName;
            if (!directory.FullName.StartsWith(roothPath))
            {
                directory.FullName = Path.Combine(roothPath, directory.FullName);
            }
            //TODO Review
            if (!CheckSecurePath(directory.FullName))
                return null;
            return directory.GetFiles();
        }

        /// <summary>
        /// List all directories under given directory.
        /// </summary>
        /// <param name="directory">The directry to list files under it.</param>
        /// <returns>List of directories.</returns>
        public DirectoryData[] ListDirectories(DirectoryData directory)
        {
            if (directory == null)
            {
                return ListDirectories();
            }

            string roothPath = GetDirectoryRoot().FullName;
            if (!directory.FullName.StartsWith(roothPath))
            {
                directory.FullName = Path.Combine(roothPath, directory.FullName);
            }
            //TODO Review
            if (!CheckSecurePath(directory.FullName))
                return null;
            return directory.GetDirectories();
        }

        /// <summary>
        /// List all directories under root directory.
        /// </summary>
        /// <returns>List of directories.</returns>
        public DirectoryData[] ListDirectories()
        {
            return GetDirectoryRoot().GetDirectories();
        }

        public abstract DirectoryData GetDirectoryRoot();

        public abstract DirectoryData GetDirectoryResources();

        /// <summary>
        /// Reads file on given path.
        /// </summary>
        /// <param name="path">The file path, including file name.</param>
        /// <returns>Readed bytes.</returns>
        public byte[] ReadFile(FileData file)
        {
            //TODO Review
            if (!CheckSecurePath(file.FullName))
                return null;
            byte[] fileBytes = null;

            if (ExistsFile(file))
            {
                try
                {
                    FileInfo fi = new FileInfo(file.FullName);
                    file.Length = fi.Length;
                    FileStream fs = fi.OpenRead();
                    fileBytes = new byte[(int)file.Length];

                    fs.Read(fileBytes, 0, (int)file.Length);

                    fs.Close();
                    fs = null;
                    fi = null;
                }
                catch (Exception e)
                {
                    SystemLogger.Log(SystemLogger.Module.CORE, "Error reading file [" + file.FullName + "]", e);
                }
            }

            return fileBytes;
        }

        /// <summary>
        /// Writes contents to file on given path.
        /// </summary>
        /// <param name="file">The file to add/append contents to.</param>
        /// <param name="contents">Data to be written to file.</param>
        /// <param name="append">True if data should be appended to previous file data.</param>
        public bool WriteFile(FileData file, byte[] contents, bool append)
        {
            //TODO Review
            if (!CheckSecurePath(file.FullName))
                return false;
            bool success = false;
            if (ExistsFile(file))
            {
                if (!append)
                {
                    // Delete file contents.
                    using (StreamWriter sw = new StreamWriter(file.FullName))
                    {
                        sw.Write("");
                        sw.Close();
                    }
                }

                try
                {
                    FileInfo fi = new FileInfo(file.FullName);
                    file.Length = fi.Length;
                    FileStream fs = fi.OpenWrite();

                    if (append)
                    {   // move to final position.
                        fs.Position = file.Length;
                    }
                    fs.Write(contents, 0, contents.Length);

                    fs.Close();
                    fs = null;
                    fi = null;
                    success = true;
                }
                catch (Exception e)
                {
                    SystemLogger.Log(SystemLogger.Module.CORE, "Error writing to file [" + file.FullName + "]", e);
                }
            }
            return success;
        }

        /// <summary>
        /// Checks if given file exists.
        /// </summary>
        /// <param name="file">The file to be checked.</param>
        /// <returns>True if file exists.</returns>
        public bool ExistsFile(FileData file)
        {
            if (file == null)
            {
                return false;
            }
            string roothPath = GetDirectoryRoot().FullName;
            if (!file.FullName.StartsWith(roothPath))
            {
                file.FullName = Path.Combine(roothPath, file.FullName);
            }
            //TODO Review
            if (!CheckSecurePath(file.FullName))
                return false;
            return File.Exists(file.FullName);
        }

        /// <summary>
        /// Checks if given directory exists.
        /// </summary>
        /// <param name="directory">The directory to be checked.</param>
        /// <returns>True if directory exists.</returns>
        public bool ExistsDirectory(DirectoryData directory)
        {
            if (directory == null)
            {
                return false;
            }
            string roothPath = GetDirectoryRoot().FullName;
            if (!directory.FullName.StartsWith(roothPath))
            {
                directory.FullName = Path.Combine(roothPath, directory.FullName);
            }
            //TODO Review
            if (!CheckSecurePath(directory.FullName))
                return false;
            return Directory.Exists(directory.FullName);
        }

        /// <summary>
        /// Copies the given file on "fromPath" to the "toPath". 
        /// </summary>
        /// <param name="fromPath">Relative path under configured application "resources" directory.
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="toPath">Relative path under configured application "documents" directory. See GetDirectoryRoot().
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public bool CopyFromResources(string fromPath, string toPath)
        {
            try
            {
                DirectoryData resourcesDir = GetDirectoryResources();
                string fromFilePath = Path.Combine(resourcesDir.FullName, fromPath);
                //TODO Review
                if (!CheckSecurePath(fromFilePath))
                    return false;
                FileData sourceFile = new FileData(fromFilePath);
                //TODO Review
                if (!CheckSecurePath(sourceFile.FullName))
                    return false;
                if (ExistsFile(sourceFile))
                {
                    DirectoryData rootDir = GetDirectoryRoot();
                    string toFilePath = Path.Combine(rootDir.FullName, toPath);

                    try
                    {
                        File.Copy(fromFilePath, toFilePath);
                    }
                    catch (Exception ex)
                    {
                        SystemLogger.Log(SystemLogger.Module.CORE, "Error copying from file [" + fromFilePath + "] to file [" + toFilePath + "]", ex);
                        return false;
                    }
                    return true;
                }
                else
                {
                    SystemLogger.Log(SystemLogger.Module.CORE, "Error copying from file [" + fromFilePath + "]. File does not exists.");
                }
            }
            catch (Exception)
            {
                SystemLogger.Log(SystemLogger.Module.CORE, "Error copying from file [" + fromPath + "]. Unhandled exception.");
            }

            return false;

        }

        public abstract bool CopyFromRemote(string url, string toPath);


        #endregion


        /// <summary>
        /// Stores the file, under the given directory, with the given contents.
        /// </summary>
        /// <returns>
        /// The file. The file contents are overwritten if it already exists.
        /// </returns>
        /// <param name='directoryPath'>
        /// Directory path. The directory is created if it does not exist.
        /// </param>
        /// <param name='fileName'>
        /// File name.
        /// </param>
        /// <param name='fileData'>
        /// File data.
        /// </param>
        public string StoreFile(string directoryPath, string fileName, byte[] fileData)
        {

            // Create directory if it does not exists
            DirectoryData assetsDirectory = this.CreateDirectory(directoryPath);
            if (assetsDirectory == null)
            {
                // Directory already exists
                assetsDirectory = new DirectoryData(Path.Combine(this.GetDirectoryRoot().FullName, directoryPath));
            }
            //TODO Review
            if (!CheckSecurePath(assetsDirectory.FullName))
                return null;

            SystemLogger.Log(SystemLogger.Module.CORE, "Directory created: " + assetsDirectory.FullName);

            // Create file on the directory
            FileData storedFile = this.CreateFile(fileName, assetsDirectory);
            if (storedFile == null)
            {
                // File already created (would be overwritten with the WriteFile call)
                storedFile = new FileData(Path.Combine(assetsDirectory.FullName, fileName), 0);
            }
            //TODO Review
            if (!CheckSecurePath(storedFile.FullName))
                return null;
            // Write contents to file
            bool success = this.WriteFile(storedFile, fileData, false);
            if (success)
            {
                SystemLogger.Log(SystemLogger.Module.CORE, "File sucessfully stored locally: " + storedFile.FullName);
            }
            else
            {
                SystemLogger.Log(SystemLogger.Module.CORE, "File could not be stored locally to path: " + storedFile.FullName);
            }

            return Path.Combine(directoryPath, fileName);

        }

        public bool CheckSecurePath(string path)
        {

            string rootPath = Path.GetFullPath(this.GetDirectoryRoot().FullName);
            string filePath = Path.GetFullPath(path);
            //SystemLogger.Log (SystemLogger.Module.CORE, "CheckSecurePath [RootPath: "+rootPath+"] is in [Filepath: "+ filePath+"]");
            return filePath.StartsWith(rootPath);
        }
#else
        public abstract Task<FileData> CreateFile(string fileName, DirectoryData baseDirectory);
        public abstract Task<FileData> CreateFile(string fileName);
        public abstract Task<DirectoryData> CreateDirectory(string directoryName, DirectoryData baseDirectory);
        public abstract Task<DirectoryData> CreateDirectory(string directoryName);
        public abstract Task<bool> DeleteFile(FileData file);
        public abstract Task<bool> DeleteDirectory(DirectoryData directory);
        public abstract Task<FileData[]> ListFiles(DirectoryData directory);
        public abstract Task<DirectoryData[]> ListDirectories(DirectoryData directory);
        public abstract Task<DirectoryData[]> ListDirectories();
        public abstract Task<DirectoryData> GetDirectoryRoot();
        public abstract Task<byte[]> ReadFile(FileData file);
        public abstract Task<bool> WriteFile(FileData file, byte[] contents, bool append);
        public abstract Task<bool> ExistsFile(FileData file);
        public abstract Task<bool> ExistsDirectory(DirectoryData directory);
        public abstract Task<bool> CopyFromResources(string fromPath, string toPath);
        public abstract Task<bool> CopyFromRemote(string url, string toPath);
        public abstract Task<string> StoreFile(string directoryPath, string fileName, byte[] fileData);
#endif
    }
}
