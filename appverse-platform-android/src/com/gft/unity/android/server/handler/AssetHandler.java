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
package com.gft.unity.android.server.handler;

import java.io.BufferedInputStream;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.URLDecoder;

import android.content.res.AssetManager;

import com.gft.unity.android.AndroidServiceLocator;
import com.gft.unity.android.AndroidUtils;
import com.gft.unity.core.storage.filesystem.FileData;
import com.gft.unity.core.storage.filesystem.IFileSystem;
import com.gft.unity.core.system.server.net.HttpRequest;
import com.gft.unity.core.system.server.net.HttpResponse;
import com.gft.unity.core.system.server.net.Request;
import com.gft.unity.core.system.server.net.Response;
import com.gft.unity.core.system.server.net.Server;

public class AssetHandler extends AndroidHandler {

	private static final String ASSET_PATH = "/WebResources/";
	private static final String DOCUMENT_PATH = "/documents/";

	private AssetManager assetManager;
	private IFileSystem fs;

	@Override
	public boolean initialize(String handlerName, Server server) {
		super.initialize(handlerName, server);

		if (assetManager == null) {
			assetManager = (AssetManager) getService(AndroidServiceLocator.SERVICE_ANDROID_ASSET_MANAGER);
		}

		if (fs == null) {
			fs = (IFileSystem) getService(AndroidServiceLocator.SERVICE_TYPE_FILESYSTEM);
		}

		Log("Initialized.");

		return true;
	}

	@Override
	public boolean handle(Request aRequest, Response aResponse)
			throws IOException {

		if (aRequest instanceof HttpRequest) {
			HttpRequest request = (HttpRequest) aRequest;
			HttpResponse response = (HttpResponse) aResponse;
			if ("GET".equalsIgnoreCase(request.getMethod())) {
				if (!request.getUrl().startsWith("/service")) {
					return handleRequest(request, response);
				}
			}
		} else {
			// JUST FOR LOCAL TESTING, DO NOT UNCOMMENT FOR PLATFORM RELEASE
			// LogDebug("Expecting HttpRequest, received " + aRequest);
			LogDebug("Not valid HttpRequest received");
		}

		return false;
	}

	private boolean handleRequest(HttpRequest request, HttpResponse response)
			throws IOException {

		byte[] data = null;
		String requestPath = request.getUrl();

		// get mime type
		String type = getMimeType(requestPath);
		if (type == null) {
			LogDebug("Mimetype for asset: " + requestPath
					+ " could not be determined.");
			LogDebug("Request for asset: " + requestPath + " was refused.");
			return false;
		}

		// handle documents
		if (requestPath.startsWith(DOCUMENT_PATH)) {
			data = handleDocument(request);
		}

		// handle other assets
		if (data == null) {
			data = handleAsset(request);
		}

		if (data == null) {
			LogDebug("Request for asset/document: " + requestPath
					+ ". Asset not found.");
			return false;
		}

		int bisLength = data.length;
		ByteArrayInputStream bais = new ByteArrayInputStream(data);
		response.setMimeType(type);
		response.sendResponse(bais, bisLength);

		return true;
	}

	private byte[] handleDocument(HttpRequest request) {

		// normalize the document path
		String documentPath = normalizeDocumentPath(request);
		request.putProperty("file-path", documentPath);

		// read document/file
		FileData fd = new FileData();
		fd.setFullName(documentPath);
		return fs.ReadFile(fd);
	}

	private String normalizeDocumentPath(HttpRequest request) {

		String documentPath = URLDecoder.decode(request.getUrl());
		return documentPath.substring(DOCUMENT_PATH.length());
	}

	private byte[] handleAsset(HttpRequest request) throws IOException {

		String assetPath = normalizeAssetPath(request);
		LogDebug("Handling asset in path: " + assetPath);
		
		request.putProperty("file-path", assetPath);

		InputStream is = null;
		BufferedInputStream bis = null;
		ByteArrayOutputStream baos = null;
		try {
			is = AndroidUtils.getInstance().getAssetInputStream(assetManager, assetPath);
			bis = new BufferedInputStream(is, 2048);
			baos = new ByteArrayOutputStream();
			byte[] buf = new byte[512];
			int length = 0;
			while ((length = bis.read(buf)) != -1) {
				baos.write(buf, 0, length);
			}
		} catch (IOException ex) {
			Log("Error loading asset", ex);
			return null;
		} finally {
			if (bis != null) {
				try {
					bis.close();
				} catch (Exception ex) {
				}
			}
			if (is != null) {
				try {
					is.close();
				} catch (Exception ex) {
				}
			}
			if (baos != null) {
				try {
					baos.close();
				} catch (Exception ex) {
				}
			}
		}

		return baos.toByteArray();
	}

	private String normalizeAssetPath(HttpRequest request) {

		String assetPath = URLDecoder.decode(request.getUrl());
		if (!assetPath.startsWith(ASSET_PATH)) {
			assetPath = ASSET_PATH + request.getUrl();
		}

		// remove leading "/"
		assetPath = assetPath.substring(1);

		return assetPath;
	}
}
