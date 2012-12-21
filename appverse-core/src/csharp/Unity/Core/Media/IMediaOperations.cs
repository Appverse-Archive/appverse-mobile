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

namespace Unity.Core.Media
{
	public interface IMediaOperations
	{
		/// <summary>
		/// Get Media metadata.
		/// </summary>
		/// <param name="filePath">The media file path.</param>
		/// <returns>Media metadata.</returns>
		MediaMetadata GetMetadata (string filePath);

		/// <summary>
		/// Start playing media.
		/// </summary>
		/// <param name="filePath">The media file path.</param>
		bool Play (string filePath);

		/// <summary>
		/// Start playing media stream.
		/// </summary>
		/// <param name="url">Streaming url</param>
		bool PlayStream (string url);

		/// <summary>
		/// Move player to the given position in the media.
		/// </summary>
		/// <param name="position">Index Position.</param>
		long SeekPosition (long position);

		/// <summary>
		/// Stop playing media file.
		/// </summary>
		bool Stop ();

		/// <summary>
		/// Pause playing media file.
		/// </summary>
		bool Pause ();

		/// <summary>
		/// Player current state.
		/// </summary>
		//MediaState State { get; }
		MediaState GetState ();

		/// <summary>
		/// Current media on player.
		/// </summary>
		//MediaMetadata CurrentMedia { get; }

		MediaMetadata GetCurrentMedia ();

	}
}
