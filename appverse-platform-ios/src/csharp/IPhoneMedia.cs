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
using System.Runtime.InteropServices;
using System.Threading;
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using MonoTouch.MessageUI;
using MonoTouch.UIKit;
using Unity.Core.Media;
using Unity.Core.Notification;
using Unity.Core.System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Unity.Core.Storage.FileSystem;
using System.Drawing;

namespace Unity.Platform.IPhone
{

	public class IPhoneMedia : AbstractMedia
	{
		MPMoviePlayerController playerController = null; // Player controler instance

		UIPopoverController popover = null;

		public static string ASSETS_PATH = "assets";

		public override MediaMetadata GetCurrentMedia()
		{
			if(playerController != null && this.CurrentMedia != null) {
				this.CurrentMedia.DurationOffset = (long) playerController.CurrentPlaybackTime;
			}

			return this.CurrentMedia;
		}

		public override MediaState GetState()
		{
			return this.State;
		}



		public override MediaMetadata GetMetadata (string filePath)
		{
			string absolutePath = filePath;
			bool localPath = false;
			if(!absolutePath.StartsWith("http")) {
				// File path is relative path
				absolutePath = IPhoneUtils.GetInstance().GetFileFullPath(filePath);
				localPath = true;
			}
			NSUrl nsUrl = this.GetNSUrlFromPath(absolutePath, localPath);
			return this.GetMetadataFromUrl(nsUrl);
		}

		public override MediaMetadata GetSnapshot ()
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Getting picture from albums");

			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (ShowImagePickerView);
				thread.Start ();
			};

			//TODO change method signature to "void" return.
			return null;

		}
		public override MediaMetadata TakeSnapshot ()
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Getting picture from camera");
			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (ShowCameraView);
				thread.Start ();
			};

			//TODO change method signature to "void" return.
			return null;
		}

		[Export("ShowCameraView")]
		private void ShowCameraView ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {

				if(UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera)) {

					UIImagePickerController imagePickerController = new UIImagePickerController();
					imagePickerController.SourceType = UIImagePickerControllerSourceType.Camera;

					imagePickerController.FinishedPickingMedia += HandleCameraFinishedPickingMedia;
					imagePickerController.Canceled += HandleImagePickerControllerCanceled;

					IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().PresentModalViewController (imagePickerController, true);
					IPhoneServiceLocator.CurrentDelegate.SetMainUIViewControllerAsTopController(false);
				} else {
					INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
					if (notificationService != null) {
						notificationService.StartNotifyAlert ("Media Alert", "Camera is not available on this device.", "OK");
					}
				}
			});
		}

		[Export("ShowImagePickerView")]
		private void ShowImagePickerView ()
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				UIImagePickerController imagePickerController = new UIImagePickerController();
				imagePickerController.FinishedPickingImage += HandleImagePickerControllerFinishedPickingImage;
				imagePickerController.FinishedPickingMedia += HandleImagePickerControllerFinishedPickingMedia;
				imagePickerController.Canceled += HandleImagePickerControllerCanceled;


				if(IPhoneUtils.GetInstance().IsIPad()) {
					try {

						// in iPad, the UIImagePickerController should be presented inside a UIPopoverController, otherwise and exception is raised
						popover = new UIPopoverController(imagePickerController);
						UIView view = IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().View;
						//RectangleF frame = new RectangleF(new PointF(0,0),new SizeF(view.Frame.Size.Width, view.Frame.Size.Height));
						RectangleF frame = new RectangleF(new PointF(0,0),new SizeF(0,0));
						popover.PresentFromRect(frame, view, UIPopoverArrowDirection.Up, true); 

					}catch(Exception ex) {
						INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
						if (notificationService != null) {
							notificationService.StartNotifyAlert ("Media Alert", "Unable to reach Photo Library", "OK");
						}	
						if(popover != null && popover.PopoverVisible) {
							popover.Dismiss(true);
							popover.Dispose();
						}
					}

				} else {
					IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().PresentModalViewController (imagePickerController, true);
				}
			});
		}

		void HandleImagePickerControllerCanceled (object sender, EventArgs e)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Canceled picking image ");
				if(popover != null && popover.PopoverVisible) {
					popover.Dismiss(true);
					popover.Dispose();
				} else {
					IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().DismissModalViewControllerAnimated(true);
				}
			});
		}

		void HandleCameraFinishedPickingMedia (object sender, UIImagePickerMediaPickedEventArgs e)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().DismissModalViewControllerAnimated(true);
			});

			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Camera FinishedPickingMedia " + e.Info);

			MediaMetadata mediaData = new MediaMetadata();

			try {
				NSString mediaType = (NSString) e.Info.ValueForKey(UIImagePickerController.MediaType);
				UIImage image = (UIImage) e.Info.ValueForKey(UIImagePickerController.OriginalImage);

				if(image != null && mediaType !=null && mediaType == "public.image") { // "public.image" is the default UTI (uniform type) for images. 
					mediaData.Type = MediaType.Photo;
					mediaData.MimeType = MediaMetadata.GetMimeTypeFromExtension(".jpg");
					mediaData.Title = (image.GetHashCode() & 0x7FFFFFFF) + ".JPG";

					NSData imageData = image.AsJPEG();

					if(imageData !=null) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Getting image data raw data...");

						byte[] buffer = new byte[imageData.Length];
						Marshal.Copy(imageData.Bytes, buffer,0,buffer.Length);

						IFileSystem fileSystemService = (IFileSystem)IPhoneServiceLocator.GetInstance ().GetService ("file");
						SystemLogger.Log(SystemLogger.Module.CORE, "Storing media file on application filesystem...");

						mediaData.ReferenceUrl = fileSystemService.StoreFile(IPhoneMedia.ASSETS_PATH, mediaData.Title, buffer);
					}

					SystemLogger.Log(SystemLogger.Module.PLATFORM, mediaData.MimeType + ", "+ mediaData.ReferenceUrl + ", " + mediaData.Title);

				}
			} catch(Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error when extracting information from media file: " + ex.Message, ex);
			}

			IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Unity.Media.onFinishedPickingImage", mediaData);
		}

		void HandleImagePickerControllerFinishedPickingMedia (object sender, UIImagePickerMediaPickedEventArgs e)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				if(popover != null && popover.PopoverVisible) {
					popover.Dismiss(true);
					popover.Dispose();
				} else {
					IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().DismissModalViewControllerAnimated(true);
				}
			});

			SystemLogger.Log(SystemLogger.Module.PLATFORM, "FinishedPickingMedia " + e.Info);

			MediaMetadata mediaData = new MediaMetadata();
			mediaData.Type = MediaType.NotSupported;

			try {
				NSString mediaType = (NSString) e.Info.ValueForKey(UIImagePickerController.MediaType);
				UIImage image = (UIImage) e.Info.ValueForKey(UIImagePickerController.OriginalImage);
				object url = e.Info.ValueForKey(UIImagePickerController.ReferenceUrl);
				NSUrl nsReferenceUrl = new NSUrl(url.ToString());

				if(image != null && mediaType !=null && mediaType == "public.image") { // "public.image" is the default UTI (uniform type) for images. 
					mediaData.Type = MediaType.Photo;

					string fileExtension = Path.GetExtension(nsReferenceUrl.Path.ToLower());
					mediaData.MimeType = MediaMetadata.GetMimeTypeFromExtension(fileExtension);
					mediaData.Title = this.GetImageMediaTitle(nsReferenceUrl.AbsoluteString);


					NSData imageData = null;
					if(mediaData.MimeType == "image/png" || mediaData.MimeType == "image/gif" || mediaData.MimeType == "image/tiff") {
						imageData = image.AsPNG();
					} else if (mediaData.MimeType == "image/jpeg" || mediaData.MimeType == "image/jpg") {
						imageData = image.AsJPEG();
					}

					if(imageData !=null) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Getting image data raw data...");

						byte[] buffer = new byte[imageData.Length];
						Marshal.Copy(imageData.Bytes, buffer,0,buffer.Length);

						IFileSystem fileSystemService = (IFileSystem)IPhoneServiceLocator.GetInstance ().GetService ("file");
						SystemLogger.Log(SystemLogger.Module.CORE, "Storing media file on application filesystem...");

						mediaData.ReferenceUrl = fileSystemService.StoreFile(IPhoneMedia.ASSETS_PATH, mediaData.Title, buffer);
					}

					SystemLogger.Log(SystemLogger.Module.PLATFORM, mediaData.MimeType + ", "+ mediaData.ReferenceUrl + ", " + mediaData.Title);

				}
			} catch(Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error when extracting information from media file: " + ex.Message, ex);
			}

			IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Unity.Media.onFinishedPickingImage", mediaData);
		}

		private string GetImageMediaTitle(string assetUrl) {
			string title = null;
			// example: assets-library://asset/asset.GIF?id=C6F1206A-6DD1-48FA-8673-CB5D057E3ED6&ext=GIF

			if(assetUrl != null && assetUrl.IndexOf("id=") >= 0) {
				Uri uri = new Uri(assetUrl);
				string queryParams = uri.Query.Substring(1); // remove the "?" character
				//SystemLogger.Log(SystemLogger.Module.PLATFORM, queryParams);

				NameValueCollection nvc = new NameValueCollection();
				foreach ( string vp in Regex.Split( queryParams, "&" ) )
				{
					string[] singlePair = Regex.Split( vp, "=" );
					if ( singlePair.Length == 2 )
					{
						nvc.Add( singlePair[ 0 ], singlePair[ 1 ] ); 
						//SystemLogger.Log(SystemLogger.Module.PLATFORM, singlePair[ 0 ] + " / " + singlePair[ 1 ]);
					}    
				}

				string id = nvc.Get("id");
				string ext = nvc.Get("ext");

				if(ext == null) {
					return id;	
				}
				return id + "." + ext;
			}

			return title;
		}

		void HandleImagePickerControllerFinishedPickingImage (object sender, UIImagePickerImagePickedEventArgs e)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "FinishedPickingImage " + e.Image);
				IPhoneServiceLocator.CurrentDelegate.MainUIViewController().DismissModalViewControllerAnimated(true);
			});
		}

		public override bool Pause ()
		{
			bool paused = false;
			if(playerController != null && this.State == MediaState.Playing) {
				playerController.Pause();
				this.State = MediaState.Paused;
				paused = true;
			}

			return paused;

		}

		public override bool Play (string filePath)
		{
			// File path is relative path.
			string absolutePath = IPhoneUtils.GetInstance().GetFileFullPath(filePath);
			if(!File.Exists(absolutePath)) {
				// file path was not under application bundle path
				// try to find it under Documents folder
				if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
					var documents = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0].Path;
					absolutePath = Path.Combine(documents, filePath);
				} else {
					var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					absolutePath = Path.Combine(documents, filePath);
				};
				//absolutePath = Path.Combine(IPhoneFileSystem.DEFAULT_ROOT_PATH, filePath);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Media file does not exist on bundle path, checking under application documents: " + absolutePath);
			}

			NSUrl nsUrl = this.GetNSUrlFromPath(absolutePath, true);
			return this.PlayNSUrl(nsUrl);
		}


		[Export("ShowMediaPlayer")]
		private void ShowMediaPlayer (object url)
		{
			NSUrl nsUrl = (NSUrl) url;

			UIApplication.SharedApplication.InvokeOnMainThread (delegate {	
				AppverseMoviePlayerViewController vcMediaPlayer = new AppverseMoviePlayerViewController(nsUrl);
				this.playerController = vcMediaPlayer.MoviePlayer;
				this.CurrentMedia = GetMetadataFromUrl(nsUrl);
				this.State = MediaState.Playing;

				IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().PresentMoviePlayerViewController(vcMediaPlayer);
				IPhoneServiceLocator.CurrentDelegate.SetMainUIViewControllerAsTopController(false);
			});

		}

		public override bool PlayStream (string url)
		{
			NSUrl nsUrl = this.GetNSUrlFromPath(url, false);
			return this.PlayNSUrl(nsUrl);
		}

		/// <summary>
		/// Plaies the NS URL.
		/// </summary>
		/// <returns>
		/// <c>true</c>, if NS URL was played, <c>false</c> otherwise.
		/// </returns>
		/// <param name='nsUrl'>
		/// Ns URL.
		/// </param>
		private bool PlayNSUrl(NSUrl nsUrl) {
			if(nsUrl!=null) {
				if(playerController !=null && this.State==MediaState.Playing) {
					// if player is already playing, stop it.
					Stop();
				}

				// TODO check if we are paused on the same file or not, to re-start player data.
				if(playerController == null) {
					try {

						using (var pool = new NSAutoreleasePool ()) {
							var thread = new Thread (ShowMediaPlayer);
							thread.Start (nsUrl);
						};

					} catch (Exception) {
						SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error trying to get media file [" + nsUrl +"]");
					}
				}

				/*
				if(playerController != null) {
					// Start Playing.
					bool playing = playerController.Play();
					if(playing) {
						this.State = MediaState.Playing;	
					}
					return playing;
				}
				*/

				return true;

			} else {
				INotification notificationService = (INotification)IPhoneServiceLocator.GetInstance ().GetService ("notify");
				if (notificationService != null) {
					notificationService.StartNotifyAlert ("Media Alert", "Media file cannot be reproduced on on this device.", "OK");
				}
				return false;
			}
		}

		public override long SeekPosition (long position)
		{
			if(playerController != null) {
				playerController.CurrentPlaybackTime = position;
				return (long) playerController.CurrentPlaybackTime;
			}

			return -1;
		}

		public override bool Stop ()
		{
			if(playerController != null && this.State != MediaState.Stopped) {
				// Stop Playing.
				playerController.Stop();
				playerController = null;
				this.CurrentMedia = null;
				this.State = MediaState.Stopped;
			}

			return true;
		}

		public override bool StartAudioRecording (string outputFilePath)
		{
			throw new System.NotImplementedException();
		}


		public override bool StopAudioRecording ()
		{
			throw new System.NotImplementedException();
		}


		public override bool StartVideoRecording (string outputFilePath)
		{
			throw new System.NotImplementedException();
		}


		public override bool StopVideoRecording ()
		{
			throw new System.NotImplementedException();
		}


		#region Private Methods

		private NSUrl GetNSUrlFromPath(string path, bool localPath) {
			NSUrl nsUrl = null;
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "Getting nsurl from path: " + path);
			try {
				if(localPath) {
					// check resource from local file system
					//nsUrl = NSUrl.FromFilename(path);
					nsUrl = IPhoneUtils.GetInstance().GetNSUrlFromPath(path);
				} else {
					// check remote resource.
					// remote paths should be escaped using Uri format
					path = Uri.EscapeUriString(path);
					nsUrl = NSUrl.FromString(path);
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "nsUrl from remote string: " + nsUrl);
				}
			} catch (Exception) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error trying to get media file [" + path +"]");
			}

			return nsUrl;
		}

		private MediaMetadata GetMetadataFromUrl(NSUrl nsUrl) {
			MediaMetadata currentMedia = new MediaMetadata();
			try {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "getting metadata from nsUrl RelativeString: "+ nsUrl.RelativeString);

				// Getting mime type
				currentMedia.MimeType = MediaMetadata.GetMimeTypeFromExtension(nsUrl.RelativeString);
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "metadata mimetype: "+ currentMedia.MimeType);
				// Getting media type
				if(currentMedia.MimeType != null && currentMedia.MimeType.ToString().StartsWith("audio")) {
					currentMedia.Type = MediaType.Audio;	
				} else if (currentMedia.MimeType != null && currentMedia.MimeType.ToString().StartsWith("video")) {
					currentMedia.Type = MediaType.Video;	
				} else if (currentMedia.MimeType != null && currentMedia.MimeType.ToString().StartsWith("image")) {
					currentMedia.Type = MediaType.Photo;	
				} else {
					currentMedia.Type = MediaType.NotSupported;
				}

				AVUrlAsset urlAsset = AVUrlAsset.FromUrl(nsUrl, new NSDictionary());
				if(urlAsset != null) {
					currentMedia.Duration = urlAsset.Duration.Value;
					if(urlAsset.Duration.TimeScale != 0) { // TimeScale could be null in some devices
						currentMedia.Duration = urlAsset.Duration.Value / urlAsset.Duration.TimeScale;
					}
					AVMetadataItem[] metadataItems = urlAsset.CommonMetadata; // CommonMetadata could be null in some devices
					if(metadataItems != null) {
						foreach(AVMetadataItem metadataItem in metadataItems) {
							if(metadataItem.CommonKey == "title") {
								currentMedia.Title = metadataItem.StringValue.ToString();
								currentMedia.Album = metadataItem.Value.ToString();

							}
							if(metadataItem.CommonKey == "artist") {
								currentMedia.Artist = metadataItem.Value.ToString();
							}
							//TODO map category & handle
						}
					}
				}

			} catch (Exception ex) {
				SystemLogger.Log(SystemLogger.Module.PLATFORM, "Error getting metadata from [" + nsUrl +"]", ex);

				currentMedia = null;
			}

			return currentMedia;
		}

		#endregion

		#region Event Handling
		private void HandlePlayerFinishedPlaying (object sender, AVStatusEventArgs e)
		{
			this.Stop();
		}
		#endregion


	}

	public class AppverseMoviePlayerViewController : MPMoviePlayerViewController {

		public AppverseMoviePlayerViewController(NSUrl url) : base(url) {
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "AppverseMoviePlayerViewController init with url: " + url);
		}

		public override bool ShouldAutorotate ()
		{ 
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "AppverseMoviePlayerViewController ShouldAutorotate? " + true);
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			SystemLogger.Log(SystemLogger.Module.PLATFORM, "AppverseMoviePlayerViewController SupportedInterfaceOrientations: " + UIInterfaceOrientationMask.AllButUpsideDown);
			return UIInterfaceOrientationMask.AllButUpsideDown;
		}

	}
}
