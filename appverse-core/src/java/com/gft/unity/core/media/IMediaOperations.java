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

public interface IMediaOperations {

    /**
     * Returns the metadata of the current media being played in the media
     * player.
     *
     * @return Metadata of current media being played, or <CODE>null</CODE> if
     * none.
     */
    public MediaMetadata GetCurrentMedia();

    /**
     * Returns the file "filePath" media metadata.
     *
     * @param filePath The media file path.
     * @return Metadata, or <CODE>null</CODE> if file not found.
     *
     */
    public MediaMetadata GetMetadata(String filePath);

    /**
     * Returns the media player current state.
     *
     * @return Media player state.
     */
    public MediaState GetState();

    /**
     * Starts playing the media file "filePath".
     *
     * @param filePath The media file path.
     * @return <CODE>true</CODE> on success, <CODE>false</CODE> otherwise.
     */
    public boolean Play(String filePath);

    /**
     * Starts playing the media stream "url".
     *
     * @param url Streaming url.
     * @return <CODE>true</CODE> on success, <CODE>false</CODE> otherwise.
     */
    public boolean PlayStream(String url);

    /**
     * Moves the current media playing position to "position".
     *
     * @param position Index position in msec.
     * @return Position after movement.
     */
    public long SeekPosition(long position);

    /**
     * Pauses the media player.
     *
     * @return <CODE>true</CODE> on success, <CODE>false</CODE> otherwise.
     */
    public boolean Pause();

    /**
     * Stops the media player.
     *
     * @return <CODE>true</CODE> on success, <CODE>false</CODE> otherwise.
     */
    public boolean Stop();
}
