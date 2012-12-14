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
namespace Unity.Core.Storage.FileSystem
{
	public interface IFileSystem
	{

		/// <summary>
		/// Creates a file under the given directory.
		/// </summary>
		/// <param name="baseDirectory">The base Directory to create file under it.</param>
		/// <param name="fileName">The file name.</param>
		/// <returns>The file created, or null if file cannot be created.</returns>
		FileData CreateFile (string fileName, DirectoryData baseDirectory);
        
		/// <summary>
		/// Creates a file under the root directory.
		/// </summary>
		/// <param name="fileName">The file name.</param>
		/// <returns>The file created, or null if file cannot be created.</returns>
		FileData CreateFile (string fileName);

		/// <summary>
		/// Creates a directory under the given base directory.
		/// </summary>
		/// <param name="baseDirectory">>The base Directory to create directory under it.</param>
		/// <param name="directoryName">The directory name.</param>
		/// <returns>The directory created, or null if file cannot be created.</returns>
		DirectoryData CreateDirectory (string directoryName, DirectoryData baseDirectory);

		/// <summary>
		/// Creates a directory under the root directory.
		/// </summary>
		/// <param name="directoryName">The directory name.</param>
		/// <returns>The directory created, or null if file cannot be created.</returns>
		DirectoryData CreateDirectory (string directoryName);

		/// <summary>
		/// Deletes a file.
		/// </summary>
		/// <param name="file">The file to be deleted.</param>
		/// <returns>True on successful deletion.</returns>
		bool DeleteFile (FileData file);

		/// <summary>
		/// Deletes a directory (deleting first all its children).
		/// </summary>
		/// <param name="directory">The directory to be deleted.</param>
		/// <returns>True on successful deletion.</returns>
		bool DeleteDirectory (DirectoryData directory);

		/// <summary>
		/// List all files under given directory.
		/// </summary>
		/// <param name="directory">The directry to list files under it.</param>
		/// <returns>List of files.</returns>
		FileData[] ListFiles (DirectoryData directory);

		/// <summary>
		/// List all directories under given directory.
		/// </summary>
		/// <param name="directory">The directry to list files under it.</param>
		/// <returns>List of directories.</returns>
		DirectoryData[] ListDirectories (DirectoryData directory);

		/// <summary>
		/// List all directories under root directory.
		/// </summary>
		/// <returns>List of directories.</returns>
		DirectoryData[] ListDirectories ();

		/// <summary>
		/// Get configured root directory.
		/// </summary>
		/// <returns>Root directory.</returns>
		DirectoryData GetDirectoryRoot ();

		/// <summary>
		/// Reads file on given path.
		/// </summary>
		/// <param name="path">The file path, including file name.</param>
		/// <returns>Readed bytes.</returns>
		byte[] ReadFile (FileData file);

		/// <summary>
		/// Writes contents to file on given path.
		/// </summary>
		/// <param name="file">The file to add/append contents to.</param>
		/// <param name="contents">Data to be written to file.</param>
		/// <param name="append">True if data should be appended to previous file data.</param>
		bool WriteFile (FileData file, byte[] contents, bool append);

		/// <summary>
		/// Checks if given file exists.
		/// </summary>
		/// <param name="file">The file to be checked.</param>
		/// <returns>True if file exists.</returns>
		bool ExistsFile (FileData file);

		/// <summary>
		/// Checks if given directory exists.
		/// </summary>
		/// <param name="directory">The directory to be checked.</param>
		/// <returns>True if directory exists.</returns>
		bool ExistsDirectory (DirectoryData directory);
		
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
		bool CopyFromResources (string fromPath, string toPath);
		
		/// <summary>
		/// Copies the given remote file on "url" to the local path "toPath". 
		/// </summary>
		/// <param name="url">Remote url to get the file.
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="toPath">Relative path under configured application "documents" directory. See GetDirectoryRoot().
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		bool CopyFromRemote (string url, string toPath);
		
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
		string StoreFile (string directoryPath, string fileName, byte[] fileData);


	}//end IFileSystem

}//end namespace FileSystem