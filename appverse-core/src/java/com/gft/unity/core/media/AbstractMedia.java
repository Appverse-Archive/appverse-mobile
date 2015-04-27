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
package com.gft.unity.core.media;

import com.gft.unity.core.media.audio.IAudio;
import com.gft.unity.core.media.camera.ICamera;
import com.gft.unity.core.media.video.IVideo;

public abstract class AbstractMedia implements IAudio, ICamera, IVideo {

    public AbstractMedia() {
    }

    @Override
    public abstract MediaMetadata GetCurrentMedia();

    @Override
    public abstract MediaMetadata GetMetadata(String filePath);

    @Override
    public abstract MediaState GetState();

    @Override
    public abstract boolean Play(String filePath);

    @Override
    public abstract boolean PlayStream(String url);

    @Override
    public abstract long SeekPosition(long position);

    @Override
    public abstract boolean Pause();

    @Override
    public abstract boolean Stop();

    @Override
    public abstract boolean StartVideoRecording(String outputFilePath);

    @Override
    public abstract boolean StopVideoRecording();

    @Override
    public abstract MediaMetadata GetSnapshot();

    @Override
    public abstract MediaMetadata TakeSnapshot();
    
    @Override
    public abstract boolean StartAudioRecording(String outputFilePath);

    @Override
    public abstract boolean StopAudioRecording();
}
