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
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Core.Media;
using Unity.Core.Media.Camera;

namespace Unity.Platform.Windows
{
    public class WindowsMedia : AbstractMedia
    {
        public override MediaMetadata GetMetadata(string filePath)
        {
            throw new NotImplementedException();
        }

        public override bool Play(string filePath)
        {
            throw new NotImplementedException();
        }

        public override bool PlayStream(string url)
        {
            throw new NotImplementedException();
        }

        public override long SeekPosition(long position)
        {
            throw new NotImplementedException();
        }

        public override bool Stop()
        {
            throw new NotImplementedException();
        }

        public override bool Pause()
        {
            throw new NotImplementedException();
        }

        public override MediaMetadata GetSnapshot()
        {
            throw new NotImplementedException();
        }

        public override MediaMetadata TakeSnapshot()
        {
            throw new NotImplementedException();
        }

        public override void TakeSnapshotWithOptions(CameraOptions options)
        {
            throw new NotImplementedException();
        }

        public override MediaMetadata GetCurrentMedia()
        {
            throw new NotImplementedException();
        }

        public override MediaState GetState()
        {
            throw new NotImplementedException();
        }

		public override bool StartAudioRecording (string outputFilePath)
		{
			throw new NotImplementedException();
		}


		public override bool StopAudioRecording ()
		{
			throw new NotImplementedException();
		}


		public override bool StartVideoRecording (string outputFilePath)
		{
			throw new NotImplementedException();
		}


		public override bool StopVideoRecording ()
		{
			throw new NotImplementedException();
		}

        /*
        public override void DetectQRCode(bool autoHandleQR)
        {
            throw new NotImplementedException();
        }

        public override QRType HandleQRCode(MediaQRContent mediaQRContent)
        {
            throw new NotImplementedException();
        }*/
    }
}
