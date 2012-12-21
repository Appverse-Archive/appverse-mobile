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
package com.gft.unity.core.storage.filesystem;

public interface IFileSystem {

    /**
     * Copies the given file on "fromPath" to the "toPath".
     *
     * @param fromPath Relative path under configured application "resources"
     * directory.
     * @param toPath Relative path under configured application "documents"
     * directory. See GetDirectoryRoot().
     * @return <CODE>true</CODE> if file copied successfully, <CODE>false</CODE>
     * otherwise.
     */
    public boolean CopyFromResources(String fromPath, String toPath);

    /**
     * Copies the given remote file on "url" to the local path "toPath".
     *
     * @param url Remote URL to get the file.
     * @param toPath String relative path under configured application
     * "documents" directory. See GetDirectoryRoot().
     * @return <CODE>true</CODE> if file copied successfully, <CODE>false</CODE>
     * otherwise.
     */
    public boolean CopyFromRemote(String url, String toPath);

    /**
     * Creates a directory under the given base directory.
     *
     * @param directoryName The directory name.
     * @param baseDirectory The base Directory to create directory under it.
     * @return The directory created, or <CODE>null</CODE> if file cannot be
     * created.
     */
    public DirectoryData CreateDirectory(String directoryName,
            DirectoryData baseDirectory);

    /**
     * Creates a directory under the root directory.
     *
     * @param directoryName The directory name.
     * @return The directory created, or <CODE>null</CODE> if file cannot be
     * created.
     */
    public DirectoryData CreateDirectory(String directoryName);

    /**
     * Creates a file under the given directory.
     *
     * @param fileName The file name.
     * @param baseDirectory The base Directory to create file under it.
     * @return The file created, or <CODE>null</CODE> if file cannot be created.
     */
    public FileData CreateFile(String fileName, DirectoryData baseDirectory);

    /**
     * Creates a file under the root directory.
     *
     * @param fileName The file name.
     * @return The file created, or <CODE>null</CODE> if file cannot be created.
     */
    public FileData CreateFile(String fileName);

    /**
     * Deletes a directory (deleting first all its children).
     *
     * @param directory The directory to be deleted.
     * @return <CODE>true</CODE> if directory deleted successfully,
     * <CODE>false</CODE> otherwise.
     */
    public boolean DeleteDirectory(DirectoryData directory);

    /**
     * Deletes a file.
     *
     * @param file The file to be deleted.
     * @return <CODE>true</CODE> if file deleted successfully,
     * <CODE>false</CODE> otherwise.
     */
    public boolean DeleteFile(FileData file);

    /**
     * Checks if given directory exists.
     *
     * @param directory The directory to be checked.
     * @return <CODE>true</CODE> if directory exists, <CODE>false</CODE>
     * otherwise.
     */
    public boolean ExistsDirectory(DirectoryData directory);

    /**
     * Checks if given file exists.
     *
     * @param file The file to be checked.
     * @return <CODE>true</CODE> if file exists, <CODE>false</CODE> otherwise.
     *
     */
    public boolean ExistsFile(FileData file);

    /**
     * Get configured root directory.
     *
     * @return Root directory.
     */
    public DirectoryData GetDirectoryRoot();

    /**
     * List all directories under given directory.
     *
     * @param directory The directory to list files under it.
     * @return List of directories.
     */
    public DirectoryData[] ListDirectories(DirectoryData directory);

    /**
     * List all directories under root directory.
     *
     * @return List of directories.
     */
    public DirectoryData[] ListDirectories();

    /**
     * List all files under given directory.
     *
     * @param directory The directory to list files under it.
     * @return List of files.
     */
    public FileData[] ListFiles(DirectoryData directory);

    /**
     * Reads file on given path.
     *
     * @param file The file path, including file name.
     * @return Read file data, or <CODE>null</CODE> if file cannot be read.
     */
    public byte[] ReadFile(FileData file);

    /**
     * Writes contents to file on given path.
     *
     * @param file The file to add/append contents to.
     * @param contents Data to be written to file.
     * @param append <CODE>true</CODE> if data should be appended to previous
     * file data, <CODE>false</CODE> if data should override previous file data.
     * @return <CODE>true</CODE> if data written successfully,
     * <CODE>false</CODE> otherwise.
     */
    public boolean WriteFile(FileData file, byte[] contents, boolean append);

    /**
     * Stores the file under the given path with the given contents.
     *
     * @param directory Directory path. The directory is created if it does not
     * exist.
     * @param name File name.
     * @param data File data.
     * @return The file path.
     */
    public String StoreFile(String directory, String name, byte[] data);
}