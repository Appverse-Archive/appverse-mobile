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
import java.io.FileDescriptor;
import java.io.FileInputStream;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.UUID;

import android.content.ContentResolver;
import android.content.Context;
import android.content.Intent;
import android.content.res.AssetFileDescriptor;
import android.content.res.AssetManager;
import android.database.Cursor;
import android.media.MediaPlayer;
import android.media.MediaPlayer.OnCompletionListener;
import android.media.MediaPlayer.OnErrorListener;
import android.media.MediaPlayer.OnPreparedListener;
import android.net.Uri;
import android.provider.MediaStore;
import android.webkit.MimeTypeMap;

import com.gft.unity.android.activity.AbstractActivityManagerListener;
import com.gft.unity.android.activity.AndroidActivityManager;
import com.gft.unity.android.activity.IActivityManager;
import com.gft.unity.core.media.AbstractMedia;
import com.gft.unity.core.media.MediaMetadata;
import com.gft.unity.core.media.MediaState;
import com.gft.unity.core.media.MediaType;
import com.gft.unity.core.storage.filesystem.IFileSystem;
import com.gft.unity.core.system.log.Logger;
import com.gft.unity.core.system.log.Logger.LogCategory;

public class AndroidMedia extends AbstractMedia {

	private static final String LOGGER_MODULE = "IMedia";
	private static final Logger LOGGER = Logger.getInstance(
			LogCategory.PLATFORM, LOGGER_MODULE);

	private MediaPlayer mp;
	private MediaState state;

	public AndroidMedia() {
		super();
		mp = null;
		state = MediaState.Stopped;
	}

	private OnPreparedListener onPrepareListener = new OnPreparedListener() {

		@Override
		public void onPrepared(MediaPlayer player) {
			mp.start();
			state = MediaState.Playing;
			LOGGER.logInfo("OnPreparedListener", "Playback started");
		}
	};

	private OnCompletionListener onCompleteListener = new OnCompletionListener() {

		@Override
		public void onCompletion(MediaPlayer player) {
			state = MediaState.Stopped;
			mp.release();
			mp = null;
			LOGGER.logInfo("OnCompletionListener", "Playback completed");
		}
	};

	private OnErrorListener onErrorListener = new OnErrorListener() {

		@Override
		public boolean onError(MediaPlayer player, int what, int extra) {
			state = MediaState.Error;
			LOGGER.logError("OnErrorListener", "Playback error [" + what + ","
					+ extra + "]");
			return true;
		}
	};

	@Override
	public MediaMetadata GetCurrentMedia() {
		// TODO implement IMediaOperations.GetCurrentMedia
		// MediaScanner or API Level 10+
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public MediaMetadata GetMetadata(String filePath) {
		// TODO implement IMediaOperations.GetMetadata
		// MediaScanner or API Level 10+
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public MediaState GetState() {
		MediaState result = null;

		LOGGER.logOperationBegin("GetState", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		result = state;

		LOGGER.logOperationEnd("GetState", result);

		return result;
	}

	@Override
	public boolean Play(String path) {
		boolean result = false;

		LOGGER.logOperationBegin("Play", new String[] { "path" },
				new Object[] { path });

		try {

			String mimeType = getMimeType(path);
			
			// copy file from asset directory to cache folder
			File file = copyAssetToInternalStorage(path);

			if (mimeType.startsWith("video/")) {

				// start activity
				Uri uri = Uri.fromFile(file);
				Intent intent = new Intent();
				intent.setAction(Intent.ACTION_VIEW);
				intent.setDataAndType(uri, "video/mp4");
				IActivityManager aam = (IActivityManager) AndroidServiceLocator
						.GetInstance()
						.GetService(
								AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
				result = aam.startActivity(intent);
			} else {
				/*AssetManager assets = AndroidServiceLocator.getContext()
						.getAssets();
				AssetFileDescriptor descriptor =  assets.openFd(path);
				
				//could not be used anymore the asset manager, because application assets could be 
				*/		
				FileInputStream fis = new FileInputStream(file);
				FileDescriptor descriptor = fis.getFD(); 
				state = MediaState.Stopped;

				createMediaPlayer(null);
				mp.setDataSource(descriptor); 
				
				/* could not be used with encrypted assets ,
				mp.setDataSource(descriptor.getFileDescriptor(),
					descriptor.getClass()StartOffset(), descriptor.getLength());
				descriptor.close();
				*/
				fis.close();
				mp.prepareAsync();
				result = true;
			}
		} catch (Exception ex) {
			LOGGER.logError("Play", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("Play", result);
		}

		return result;
	}

	private void createMediaPlayer(Uri uri) {
		if (uri == null) {
			mp = new MediaPlayer();
		} else {
			mp = MediaPlayer.create(AndroidServiceLocator.getContext(), uri);
		}
		mp.setOnCompletionListener(onCompleteListener);
		mp.setOnPreparedListener(onPrepareListener);
		mp.setOnErrorListener(onErrorListener);
	}

	private File copyAssetToInternalStorage(String path) throws Exception {

		Context context = AndroidServiceLocator.getContext();
		File mediaCacheFile = new File("temp."
				+ MimeTypeMap.getFileExtensionFromUrl(path));

		InputStream is = null;
		OutputStream os = null;

		try {
			is = new BufferedInputStream(AndroidUtils.getInstance().getAssetInputStream(context.getAssets(),path));
			os = new BufferedOutputStream(context.openFileOutput(
					mediaCacheFile.getPath(), Context.MODE_WORLD_READABLE));
			byte[] buffer = new byte[512];
			int length;
			while ((length = is.read(buffer)) > 0) {
				os.write(buffer, 0, length);
			}
		} finally {
			if (is != null) {
				is.close();
			}
			if (os != null) {
				os.close();
			}
		}

		return new File(context.getFilesDir(), mediaCacheFile.getPath());
	}

	@Override
	public boolean PlayStream(String url) {
		boolean result = false;

		LOGGER.logOperationBegin("PlayStream", new String[] { "url" },
				new Object[] { url });

		try {
			String mimeType = getMimeType(url);
			if (mimeType.startsWith("video/")) {

				// start activity
				Uri uri = Uri.parse(url);
				Intent intent = new Intent();
				intent.setAction(Intent.ACTION_VIEW);
				intent.setDataAndType(uri, "video/mp4");
				IActivityManager aam = (IActivityManager) AndroidServiceLocator
						.GetInstance()
						.GetService(
								AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
				if(aam.resolveActivity(intent)) {
					result = aam.startActivity(intent);
				} else {
					result = false;
				}
			} else {
				Uri uri = Uri.parse(url);
				state = MediaState.Stopped;
				createMediaPlayer(uri);
				mp.prepareAsync();
				mp.start();
				
				result = true;
			}
			
		} catch (Exception ex) {
			LOGGER.logError("PlayStream", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("PlayStream", result);
		}

		return result;
	}

	@Override
	public long SeekPosition(long position) {
		long result = -1;

		LOGGER.logOperationBegin("SeekPosition", new String[] { "position" },
				new Object[] { "paramValues" });

		try {
			if (mp != null) {
				mp.seekTo(Long.valueOf(position).intValue());
			}
			result = mp.getCurrentPosition();
		} catch (Exception ex) {
			LOGGER.logError("SeekPosition", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("SeekPosition", result);
		}

		return result;
	}

	@Override
	public boolean Pause() {
		boolean result = false;

		LOGGER.logOperationBegin("Pause", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		try {
			if ((mp != null) && mp.isPlaying()) {
				mp.pause();
				state = MediaState.Paused;
			}
			result = true;
		} catch (Exception ex) {
			LOGGER.logError("Pause", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("Pause", result);
		}

		return result;
	}

	@Override
	public boolean Stop() {
		boolean result = false;

		LOGGER.logOperationBegin("Stop", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		try {
			if ((mp != null) && mp.isPlaying()) {
				mp.stop();
				mp.release();
				mp = null;
			}
			state = MediaState.Stopped;
			result = true;
		} catch (Exception ex) {
			LOGGER.logError("Stop", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("Stop", result);
		}

		return result;
	}

	@Override
	public boolean StartAudioRecording(String outputFilePath) {
		// TODO implement IAudio.StartAudioRecording
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean StopAudioRecording() {
		// TODO implement IAudio.StopAudioRecording
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean StartVideoRecording(String outputFilePath) {
		// TODO implement IVideo.StartVideoRecording
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public boolean StopVideoRecording() {
		// TODO implement IVideo.StopVideoRecording
		throw new UnsupportedOperationException("Not supported yet.");
	}

	@Override
	public MediaMetadata GetSnapshot() {
		MediaMetadata result = null;

		LOGGER.logOperationBegin("GetSnapshot", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		try {

			AndroidActivityManager aam = (AndroidActivityManager) AndroidServiceLocator
					.GetInstance()
					.GetService(
							AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);

			Intent intent = new Intent(Intent.ACTION_PICK);
			intent.setType("image/*");

			aam.startActivityForResult(intent,
					AndroidActivityManager.GET_SNAPSHOT_RC,
					new GetSnapshotListener());
		} catch (Exception ex) {
			LOGGER.logError("GetSnapshot", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("GetSnapshot", result);
		}

		return result;
	}

	private class GetSnapshotListener extends AbstractActivityManagerListener {

		@Override
		public void onOk(int requestCode, Intent data) {

			LOGGER.logInfo("GetSnapshotListener.onOk", ((data!=null)?data.getDataString(): ""));
			
			try {

				Uri uri = data.getData();

				// retrieve image metadata
				MediaMetadata meta = retrieveMetadata(uri);
				if (meta == null) {
					meta = new MediaMetadata();
				}
				if (meta.getType() == null) {
					meta.setType(MediaType.Photo);
				}

				// copy image to internal storage
				copyImageToInternalStorage(uri, meta);

				imageSelectedJSCallback(meta);

			} catch (Exception ex) {
				LOGGER.logError("GetSnapshotListener.onOk", "Error", ex);
			}
		}
	}

	@Override
	public MediaMetadata TakeSnapshot() {
		MediaMetadata result = null;

		LOGGER.logOperationBegin("TakeSnapshot", Logger.EMPTY_PARAMS,
				Logger.EMPTY_VALUES);

		try {

			AndroidActivityManager aam = (AndroidActivityManager) AndroidServiceLocator
					.GetInstance()
					.GetService(
							AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);

			Context context = AndroidServiceLocator.getContext();

			Intent intent = new Intent(
					android.provider.MediaStore.ACTION_IMAGE_CAPTURE);
			File takeSnapshotPath = new File(context.getExternalCacheDir(),
					"IMG_" + UUID.randomUUID() + ".jpeg");
			takeSnapshotPath.getParentFile().mkdirs();
			intent.putExtra(MediaStore.EXTRA_OUTPUT,
					Uri.fromFile(takeSnapshotPath));

			aam.startActivityForResult(intent,
					AndroidActivityManager.TAKE_SNAPSHOT_RC,
					new TakeSnapshotListener(takeSnapshotPath));
		} catch (Exception ex) {
			LOGGER.logError("TakeSnapshot", "Error", ex);
		} finally {
			LOGGER.logOperationEnd("TakeSnapshot", result);
		}

		return result;
	}

	private class TakeSnapshotListener extends AbstractActivityManagerListener {

		private File targetPath;

		public TakeSnapshotListener(File targetPath) {
			this.targetPath = targetPath;
		}

		@Override
		public void onOk(int requestCode, Intent data) {

			LOGGER.logInfo("TakeSnapshotListener.onOk", ((data!=null)?data.getDataString(): ""));

			try {
				Uri uri = Uri.fromFile(targetPath);

				// retrieve image metadata
				MediaMetadata meta = new MediaMetadata();
				meta.setType(MediaType.Photo);
				meta.setMimeType("image/jpeg");
				meta.setTitle(UUID.randomUUID().toString());

				// copy image to internal storage
				copyImageToInternalStorage(uri, meta);

				imageSelectedJSCallback(meta);

			} catch (Exception ex) {
				LOGGER.logError("TakeSnapshotListener.onOk", "Error", ex);
			}
		}
	}

	private static MediaMetadata retrieveMetadata(Uri uri) {

		MediaMetadata meta = null;

		String[] projection = { MediaStore.MediaColumns.TITLE,
				MediaStore.MediaColumns.MIME_TYPE };
		ContentResolver cr = AndroidServiceLocator.getContext()
				.getContentResolver();
		Cursor cursor = cr.query(uri, projection, null, null, null);
		try {
			if (cursor != null && cursor.getCount() > 0) {
				meta = new MediaMetadata();
				cursor.moveToFirst();
				meta.setTitle(cursor.getString(0));
				String mimeType = cursor.getString(1);
				meta.setMimeType(mimeType);
				if (mimeType != null) {
					if (mimeType.startsWith("image/")) {
						meta.setType(MediaType.Photo);
					} else if (mimeType.startsWith("video/")) {
						meta.setType(MediaType.Video);
					} else if (mimeType.startsWith("audio/")) {
						meta.setType(MediaType.Audio);
					} else {
						meta.setType(MediaType.NotSupported);
					}
				}
			}
		} finally {
			if (cursor != null) {
				cursor.close();
				cursor = null;
			}
		}

		return meta;
	}

	private static void copyImageToInternalStorage(Uri uri, MediaMetadata meta) {

		BufferedInputStream bis = null;
		ByteArrayOutputStream baos = null;
		try {

			// read image data
			bis = new BufferedInputStream(AndroidServiceLocator.getContext()
					.getContentResolver().openInputStream(uri));
			baos = new ByteArrayOutputStream();
			byte[] buffer = new byte[1024];
			int length;
			while ((length = bis.read(buffer)) > 0) {
				baos.write(buffer, 0, length);
			}

			// store image in the application files folder
			IFileSystem fileService = (IFileSystem) AndroidServiceLocator
					.GetInstance().GetService(
							AndroidServiceLocator.SERVICE_TYPE_FILESYSTEM);
			String extension = getExtension(meta.getMimeType());
			String name = meta.getTitle();
			if (!name.endsWith(".jpg") && !name.endsWith(".jpeg")) {
				name += "." + extension;
			}
			String path = fileService.StoreFile(fileService.GetDirectoryRoot()
					.getFullName(), name, baos.toByteArray());
			// TODO StoreFile should return a relative path
			path = path.substring(path.lastIndexOf('/') + 1);
			meta.setReferenceUrl(path);
		} catch (Exception ex) {
			LOGGER.logError("CopyImageToInternalStorage", "Error", ex);
		} finally {
			if (bis != null) {
				try {
					bis.close();
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
	}

	private static void imageSelectedJSCallback(MediaMetadata meta) {

		IActivityManager am = (IActivityManager) AndroidServiceLocator
				.GetInstance().GetService(
						AndroidServiceLocator.SERVICE_ANDROID_ACTIVITY_MANAGER);
		am.executeJS("Unity.Media.onFinishedPickingImage", meta);
	}

	private static String getExtension(String mimeType) {
		return MimeTypeMap.getSingleton().getExtensionFromMimeType(mimeType);
	}

	private static String getMimeType(String url) {
		String type = null;
		String extension = MimeTypeMap.getFileExtensionFromUrl(url);
		if (extension != null) {
			MimeTypeMap mime = MimeTypeMap.getSingleton();
			type = mime.getMimeTypeFromExtension(extension);
		}
		return type;
	}
}
