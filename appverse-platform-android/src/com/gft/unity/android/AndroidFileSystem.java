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

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URL;
import java.net.HttpURLConnection;
import java.util.ArrayList;
import java.util.List;

import android.content.Context;

import com.gft.unity.core.storage.filesystem.AbstractFileSystem;
import com.gft.unity.core.storage.filesystem.DirectoryData;
import com.gft.unity.core.storage.filesystem.FileData;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

// TODO check for relative paths starting with ../ or ./../
// TODO verify baseDirectory exists
public class AndroidFileSystem extends AbstractFileSystem {

	private static final String LOGGER_MODULE = "IFileSystem";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);

	private static final String RESOURCES_PATH = "WebResources/";

	private static final int BUFFER_SIZE = 4096;

	private String rootDirectory;

	public AndroidFileSystem() {

		Context context = AndroidServiceLocator.getContext();
		rootDirectory = context.getFilesDir().getAbsolutePath();
	}

	@Override
	public boolean CopyFromResources(String fromPath, String toPath) {
		boolean result = false;

		LOGGER.logOperationBegin("CopyFromResources", new String[] {
				"fromPath", "toPath" }, new Object[] { fromPath, toPath });

		BufferedInputStream bis = null;
		try {
			// open source resource file
			if (!fromPath.startsWith(RESOURCES_PATH)) {
				fromPath = RESOURCES_PATH + fromPath;
			}
						
			bis = new BufferedInputStream(AndroidServiceLocator.getContext()
					.getAssets().open(fromPath), BUFFER_SIZE);
			
			// write to internal storage file
			write(bis, toPath);

			result = true;
		} catch (Exception ex) {
			LOGGER.logError("CopyFromResources", "Error", ex);
		} finally {
			closeStream(bis);
			LOGGER.logOperationEnd("CopyFromResources", result);
		}

		return result;
	}

	@Override
	public boolean CopyFromRemote(String url, String toPath) {
		boolean result = false;

		LOGGER.logOperationBegin("CopyFromRemote", new String[] { "url",
				"toPath" }, new Object[] { url, toPath });

		BufferedInputStream bis = null;
		HttpURLConnection connection = null;
		try {
			// open source URL
			URL mUrl = new URL(url);
			connection = (HttpURLConnection) mUrl.openConnection();
			//bis = new BufferedInputStream(mUrl.openStream(), BUFFER_SIZE);
			bis = new BufferedInputStream(connection.getInputStream(), BUFFER_SIZE);
			
			// used HttpURLConnection to follow redirects (redirects from HTTP to HTTPS or viceversa are not followed by default)
			while(!mUrl.getProtocol().equals(connection.getURL().getProtocol())) {
				closeStream(bis);
				if(connection!=null) {
					connection.disconnect();
				}
				mUrl = new URL(url.replace(mUrl.getProtocol(), connection.getURL().getProtocol()));
				connection = (HttpURLConnection) mUrl.openConnection();
				bis = new BufferedInputStream(connection.getInputStream(), BUFFER_SIZE);
			}
			
			
			// write to internal storage file
			write(bis, toPath);

			result = true;
		} catch (Exception ex) {
			LOGGER.logError("CopyFromRemote", "Error", ex);
		} finally {
			closeStream(bis);
			if(connection!=null) {
				connection.disconnect();
			}
			LOGGER.logOperationEnd("CopyFromRemote", result);
		}

		return result;
	}

	/**
	 * Writes data from input stream "bis" to internal storage file "toPath". If
	 * the file "toPath" exists, the data is overwritten. Missing parent
	 * sub-directories are created.
	 * 
	 * @param bis
	 *            Data to write
	 * @param toPath
	 *            Internal storage file path
	 * @throws IOException
	 */
	private void write(BufferedInputStream bis, String toPath)
			throws IOException {

		BufferedOutputStream bos = null;
		try {
			// open target internal storage file
			File file = new File(normalizePath(toPath));
			//TODO review
			if(checkSecurePath(file)){
				if (file.exists()) {
					file.delete();
				}
				file.getParentFile().mkdirs();
				file.createNewFile();
				bos = new BufferedOutputStream(new FileOutputStream(file),
						BUFFER_SIZE);

				// read data from the source URL and write it to the target internal
				// storage file
				int length = 0;
				byte[] buffer = new byte[1024];
				while ((length = bis.read(buffer)) != -1) {
					bos.write(buffer, 0, length);
				}
			}			
		} finally {
			closeStream(bos);
		}
	}

	@Override
	public DirectoryData CreateDirectory(String directoryName,
			DirectoryData baseDirectory) {
		DirectoryData result = null;

		LOGGER.logOperationBegin("CreateDirectory", new String[] {
				"directoryName", "baseDirectory" }, new Object[] {
				directoryName, baseDirectory });

		try {

			String directory = null;
			if (baseDirectory != null) {
				directory = baseDirectory.getFullName();
			}
			if (directory == null) {
				directory = rootDirectory;
			}
			
			File temp = new File(directory, directoryName);			
			String path = temp.getAbsolutePath();
			path = normalizePath(path);

			File f = new File(path);
			//TODO review
			if(!checkSecurePath(f)){
				return null;
			}
			if (!f.exists()) {
				f.mkdirs();
				result = new DirectoryData(f.getAbsolutePath());
			}
		} catch (Exception ex) {
			LOGGER.logError("CreateDirectory", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("CreateDirectory", result);
		}

		return result;
	}

	@Override
	public DirectoryData CreateDirectory(String directoryName) {
		return CreateDirectory(directoryName, null);
	}
	
	
	
	@Override
	public FileData CreateFile(String fileName, DirectoryData baseDirectory) {
		FileData result = null;

		LOGGER.logOperationBegin("CreateFile", new String[] { "fileName",
				"baseDirectory" }, new Object[] { fileName, baseDirectory });

		try {

			String directory = null;
			if (baseDirectory != null) {
				directory = baseDirectory.getFullName();
			}
			if (directory == null) {
				directory = rootDirectory;
			}
			
			File temp = new File(directory, fileName);			
			String path = temp.getAbsolutePath();
			
			path = normalizePath(path);

			File f = new File(path);
			//TODO review
			if(!checkSecurePath(f)){
				return null;
			}
			if (!f.exists()) {
				f.createNewFile();
				result = new FileData(path, f.length());
			}
		} catch (Exception ex) {
			LOGGER.logError("CreateFile", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("CreateFile", result);
		}

		return result;
	}

	@Override
	public FileData CreateFile(String fileName) {
		return CreateFile(fileName, null);
	}

	@Override
	public boolean DeleteDirectory(DirectoryData directory) {
		boolean result = false;

		LOGGER.logOperationBegin("DeleteDirectory",
				new String[] { "directory" }, new Object[] { directory });

		try {

			String path = directory.getFullName();
			
			if (path != null) {
				path = normalizePath(path);
				File f = new File(path);
				//TODO review
				if(!checkSecurePath(f)) {					
					return false;				
				}
				if (f.exists() && f.isDirectory()) {
					result = deleteR(f);
				}
			}
		} catch (Exception ex) {
			LOGGER.logError("DeleteDirectory", "Error", ex);
		}

		LOGGER.logOperationEnd("DeleteDirectory", result);

		return result;
	}

	private boolean deleteR(File f) {
		//TODO review
		if (f.isDirectory()) {
			File[] children = f.listFiles();
			for (File child : children) {
				deleteR(child);
			}
		}

		return f.delete();
	}

	@Override
	public boolean DeleteFile(FileData file) {
		boolean result = false;

		LOGGER.logOperationBegin("DeleteFile", new String[] { "file" },
				new Object[] { file });

		try {
			String path = file.getFullName();
			if (path != null) {
				path = normalizePath(path);
				File f = new File(path);
				//TODO review
				if(!checkSecurePath(f)){
					return false;
				}
				if (f.exists() && f.isFile()) {
					result = f.delete();
				}
			}
		} catch (Exception ex) {
			LOGGER.logError("DeleteFile", "Error", ex);
		}

		LOGGER.logOperationEnd("DeleteFile", result);

		return result;
	}

	@Override
	public boolean ExistsDirectory(DirectoryData directory) {
		boolean result = false;

		LOGGER.logOperationBegin("ExistsDirectory",
				new String[] { "directory" }, new Object[] { directory });

		try {
			String path = directory.getFullName();
			if (path != null) {
				path = normalizePath(path);
				File f = new File(path);
				//TODO review
				if(!checkSecurePath(f)){
					return false;
				}
				result = f.exists() && f.isDirectory();
			}
		} catch (Exception ex) {
			LOGGER.logError("ExistsDirectory", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("ExistsDirectory", result);
		}

		return result;
	}

	@Override
	public boolean ExistsFile(FileData file) {
		boolean result = false;

		LOGGER.logOperationBegin("ExistsFile", new String[] { "file" },
				new Object[] { file });

		try {
			String path = file.getFullName();
			if (path != null) {
				path = normalizePath(path);
				File f = new File(path);
				//TODO review
				if(!checkSecurePath(f)){
					return false;
				}
				result = f.exists() && f.isFile();
			}
		} catch (Exception ex) {
			LOGGER.logError("ExistsFile", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("ExistsFile", result);
		}

		return result;
	}

	@Override
	public DirectoryData GetDirectoryRoot() {
		DirectoryData result = null;

		LOGGER.logOperationBegin("GetDirectoryRoot", new String[] {},
				new Object[] {});

		try {
			result = new DirectoryData(rootDirectory);
		} catch (Exception ex) {
			LOGGER.logError("GetDirectoryRoot", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("GetDirectoryRoot", result);
		}

		return result;
	}

	@Override
	public DirectoryData[] ListDirectories(DirectoryData baseDirectory) {
		DirectoryData[] result = null;

		LOGGER.logOperationBegin("ListDirectories",
				new String[] { "baseDirectory" },
				new Object[] { baseDirectory });

		try {

			String directory = null;
			if (baseDirectory != null) {
				directory = baseDirectory.getFullName();
			}
			if (directory == null) {
				directory = rootDirectory;
			}

			List<DirectoryData> list = new ArrayList<DirectoryData>();
			String path = normalizePath(directory);
			File base = new File(path);
			//TODO review
			if(!checkSecurePath(base)){
				return null;
			}
			if (base.isDirectory()) {
				File[] directories = base.listFiles();
				if (directories != null) {
					for (int i = 0; i < directories.length; i++) {
						File f = directories[i];
						if (f.isDirectory()) {
							list.add(new DirectoryData(f.getAbsolutePath()));
						}
					}
				}
			}
			result = list.toArray(new DirectoryData[list.size()]);
		} catch (Exception ex) {
			LOGGER.logError("ListDirectories", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("ListDirectories", result);
		}

		return result;
	}

	@Override
	public DirectoryData[] ListDirectories() {
		return ListDirectories(null);
	}

	@Override
	public FileData[] ListFiles(DirectoryData baseDirectory) {
		FileData[] result = null;

		LOGGER.logOperationBegin("ListFiles", new String[] { "directory" },
				new Object[] { baseDirectory });

		try {

			String directory = null;
			if (baseDirectory != null) {
				directory = baseDirectory.getFullName();
			}
			if (directory == null) {
				directory = rootDirectory;
			}

			List<FileData> list = new ArrayList<FileData>();
			String path = normalizePath(directory);
			File base = new File(path);
			//TODO review
			if(!checkSecurePath(base)){
				return null;
			}
			if (base.isDirectory()) {
				File[] directories = base.listFiles();
				if (directories != null) {
					for (int i = 0; i < directories.length; i++) {
						File f = directories[i];
						if (f.isFile()) {
							list.add(new FileData(f.getAbsolutePath(), f
									.length()));
						}
					}
				}
			}
			result = list.toArray(new FileData[list.size()]);
		} catch (Exception ex) {
			LOGGER.logError("ListFiles", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("ListFiles", result);
		}

		return result;
	}

	@Override
	public byte[] ReadFile(FileData file) {
		byte[] result = null;

		LOGGER.logOperationBegin("ReadFile", new String[] { "file" },
				new Object[] { file });

		BufferedInputStream bis = null;
		ByteArrayOutputStream baos = null;
		try {

			String path = file.getFullName();
			if (path == null) {
				return null;
			}
			path = normalizePath(path);
			File f = new File(path);
			//TODO review
			if(!checkSecurePath(f)){
				return null;
			}
			bis = new BufferedInputStream(new FileInputStream(f), BUFFER_SIZE);
			baos = new ByteArrayOutputStream((int) f.length());

			int length = 0;
			byte[] buffer = new byte[1024];
			while ((length = bis.read(buffer)) != -1) {
				baos.write(buffer, 0, length);
			}

			result = baos.toByteArray();
		} catch (Exception ex) {
			LOGGER.logError("ReadFile", "Error", ex);
		} finally {
			closeStream(bis);
			closeStream(baos);
			LOGGER.logOperationEnd("ReadFile", result);
		}

		return result;
	}

	@Override
	public boolean WriteFile(FileData file, byte[] contents, boolean append) {
		boolean result = false;

		LOGGER.logOperationBegin("WriteFile", new String[] { "file",
				"contents", "append" }, new Object[] { file, contents, append });

		BufferedOutputStream bos = null;
		try {
			String path = file.getFullName();
			if (path == null) {
				return false;
			}
			path = normalizePath(path);
			File f = new File(path);
			//TODO review
			if(!checkSecurePath(f)){
				return false;
			}
			
			bos = new BufferedOutputStream(new FileOutputStream(f, append),
					BUFFER_SIZE);
			bos.write(contents);

			result = true;
		} catch (Exception ex) {
			LOGGER.logError("WriteFile", "Error", ex);
		} finally {
			closeStream(bos);
			LOGGER.logOperationEnd("WriteFile", result);
		}

		return result;
	}

	@Override
	public String StoreFile(String directory, String name, byte[] data) {
		String result = null;

		LOGGER.logOperationBegin("StoreFile", new String[] { "directory",
				"name", "data" }, new Object[] { directory, name, data });

		BufferedOutputStream bos = null;
		try {

			result = new File(directory, name).getAbsolutePath();
			result = normalizePath(result);
			File f = new File(result);
			
			File dir = new File(normalizePath(directory)); // new File(directory)
			
			//TODO review
			if(!checkSecurePath(f) || !checkSecurePath(dir)){
				return "";
			}
			
			dir.mkdirs();

			bos = new BufferedOutputStream(new FileOutputStream(f, false),
					BUFFER_SIZE);
			bos.write(data);

		} catch (Exception ex) {
			result = null;
			LOGGER.logError("StoreFile", "Error", ex);
		} finally {
			closeStream(bos);
			LOGGER.logOperationEnd("StoreFile", result);
		}

		return result;
	}

	private static void closeStream(InputStream is) {

		try {
			if (is != null) {
				is.close();
			}
		} catch (Exception ex) {
			LOGGER.logWarning("CloseStream", "Error closing stream", ex);
		}
	}

	private static void closeStream(OutputStream os) {

		try {
			if (os != null) {
				os.close();
			}
		} catch (Exception ex) {
			LOGGER.logWarning("CloseStream", "Error closing stream", ex);
		}
	}
	
	private String normalizePath(String path) throws IOException {

		if (!path.startsWith(rootDirectory)) {
			path = new File(rootDirectory, path).getCanonicalPath();
		}

		return path;
	}
	
	private boolean checkSecurePath(File file){
		try {
			String path = file.getCanonicalPath();
			if (!path.startsWith(rootDirectory)) {
				LOGGER.logError("CreateFile", "Error: Try to access forbidden path: "+path);
				return false;
			}
		} catch (IOException e) {
			LOGGER.logError("CreateFile", "Error: Try to access forbidden path. Exception!");			
			e.printStackTrace();
			return false;
		}

		return true;
	}
}
