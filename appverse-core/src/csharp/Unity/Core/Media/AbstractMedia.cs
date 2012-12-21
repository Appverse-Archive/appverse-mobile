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
using System.Collections.Generic;
using System.Text;
using Unity.Core.Media.Audio;
using Unity.Core.Media.Video;
using Unity.Core.Media.Camera;

namespace Unity.Core.Media
{
	public abstract class AbstractMedia : IAudio, IVideo, ICamera
	{
		
		public AbstractMedia ()
		{
			this.State = MediaState.Stopped; // default state is stopped.	
		}
		
        #region Miembros de IMediaOperations

		public abstract MediaMetadata GetMetadata (string filePath);

		public abstract bool Play (string filePath);

		public abstract bool PlayStream (string url);

		public abstract long SeekPosition (long position);

		public abstract bool Stop ();

		public abstract bool Pause ();

		protected MediaState State { get; set; }

		public abstract MediaState GetState ();

		protected MediaMetadata CurrentMedia { get; set; }

		public abstract MediaMetadata GetCurrentMedia ();

        #endregion

        #region Miembros de ICamera

		public abstract MediaMetadata GetSnapshot ();

		public abstract MediaMetadata TakeSnapshot ();

        #endregion
		
        #region IAudio implementation
		public abstract bool StartAudioRecording (string outputFilePath);
        
		public abstract bool StopAudioRecording ();
        
        #endregion
		
        #region IVideo implementation
		public abstract bool StartVideoRecording (string outputFilePath);
        
		public abstract bool StopVideoRecording ();
        
        #endregion
		
		
	}
}
