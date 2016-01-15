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
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Unity.Core.Media;
using Unity.Core.Media.Camera;
using UnityPlatformWindowsPhone.Internals;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhoneMedia : AbstractMedia, IAppverseService
    {
        public WindowsPhoneMedia()
        {
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
        }

        internal async Task<MediaMetadata> GetMetadata(StorageFile targetFile)
        {
            if (targetFile == null) return null;
            var fileMetaData = new MediaMetadata();
            try
            {
                fileMetaData.ReferenceUrl = targetFile.Path.StartsWith(WindowsPhoneUtils.DocumentsFolder.Path) ? targetFile.Path.Replace(WindowsPhoneUtils.DocumentsFolder.Path, String.Empty) : String.Empty;
                fileMetaData.Size = (long)(await targetFile.GetBasicPropertiesAsync()).Size;
                fileMetaData.MimeType = targetFile.ContentType;
                fileMetaData.Type = MediaType.NotSupported;
                if (targetFile.ContentType.Contains("audio/"))
                {
                    var musicProperties = await targetFile.Properties.GetMusicPropertiesAsync();
                    fileMetaData.Album = musicProperties.Album;
                    fileMetaData.Artist = musicProperties.Artist;
                    fileMetaData.Category = musicProperties.Genre.Aggregate(new StringBuilder(), (sb, a) => sb.Append(String.Join(",", a)), sb => sb.ToString());
                    fileMetaData.Duration = (long)musicProperties.Duration.TotalSeconds;
                    fileMetaData.Title = musicProperties.Title;
                    fileMetaData.Type = MediaType.Audio;
                }
                else if (targetFile.ContentType.Contains("video/"))
                {
                    var videoProperties = await targetFile.Properties.GetVideoPropertiesAsync();
                    fileMetaData.Duration = (long)videoProperties.Duration.TotalSeconds;
                    fileMetaData.Artist = videoProperties.Directors.Aggregate(new StringBuilder(), (sb, a) => sb.Append(String.Join(",", a)), sb => sb.ToString());
                    fileMetaData.Title = videoProperties.Title;
                    fileMetaData.Category = videoProperties.Keywords.Aggregate(new StringBuilder(), (sb, a) => sb.Append(String.Join(",", a)), sb => sb.ToString());
                    fileMetaData.Type = MediaType.Video;

                }
                else if (targetFile.ContentType.Contains("image/"))
                {
                    var imgProperties = await targetFile.Properties.GetImagePropertiesAsync();
                    fileMetaData.Title = imgProperties.Title;
                    fileMetaData.Category = imgProperties.Keywords.Aggregate(new StringBuilder(), (sb, a) => sb.Append(String.Join(",", a)), sb => sb.ToString());
                    fileMetaData.Type = MediaType.Photo;
                }
                return fileMetaData;
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
                return null;
            }
        }

        public override async Task<MediaMetadata> GetMetadata(string filePath)
        {

            return await GetMetadata(await StorageFile.GetFileFromPathAsync(Path.Combine(WindowsPhoneUtils.DocumentsFolder.Path, filePath.Replace('/', '\\'))));

        }

        public override async Task<bool> Play(string filePath)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> PlayStream(string url)
        {
            throw new NotImplementedException();
        }

        public override async Task<long> SeekPosition(long position)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> Stop()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> Pause()
        {
            throw new NotImplementedException();
        }

        public override async Task<MediaState> GetState()
        {
            throw new NotImplementedException();
        }

        public override async Task<MediaMetadata> GetCurrentMedia()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> StartAudioRecording(string outputFilePath)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> StopAudioRecording()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> StartVideoRecording(string outputFilePath)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> StopVideoRecording()
        {
            throw new NotImplementedException();
        }

        public override async Task<MediaMetadata> GetSnapshot()
        {
            AppverseBridge.Instance.RuntimeHandler.ShowPhotoPicker();
            return null;
        }

        public override async Task<MediaMetadata> TakeSnapshot()
        {
            AppverseBridge.Instance.RuntimeHandler.ShowCameraView(null);
            return null;
        }

        public override async Task TakeSnapshotWithOptions(CameraOptions options)
        {
            AppverseBridge.Instance.RuntimeHandler.ShowCameraView(options);
        }

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }
    }
}
