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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Unity.Core.Media
{
	public class MediaMetadata
	{
		private static Hashtable mimetypes = new Hashtable ();
		
		static MediaMetadata ()
		{
			
			// audio/video mime types
			mimetypes [".aif"] = "audio/x-aiff";
			mimetypes [".aifc"] = "audio/x-aiff";
			mimetypes [".aiff"] = "audio/x-aiff";
			mimetypes [".au"] = "audio/basic";
			mimetypes [".avi"] = "video/x-msvideo";
			mimetypes [".dif"] = "video/x-dv";
			mimetypes [".dv"] = "video/x-dv";
			mimetypes [".kar"] = "audio/midi";	
			mimetypes [".m3u"] = "audio/x-mpegurl";
			mimetypes [".m4a"] = "audio/mp4a-latm";
			mimetypes [".m4b"] = "audio/mp4a-latm";
			mimetypes [".m4p"] = "audio/mp4a-latm";
			mimetypes [".m4u"] = "video/vnd.mpegurl";
			mimetypes [".m4v"] = "video/x-m4v";
			mimetypes [".mid"] = "audio/midi";
			mimetypes [".midi"] = "audio/midi";
			mimetypes [".mov"] = "video/quicktime";
			mimetypes [".movie"] = "video/x-sgi-movie";
			mimetypes [".mp2"] = "audio/mpeg";
			mimetypes [".mp3"] = "audio/mpeg";
			mimetypes [".mp4"] = "video/mp4";
			mimetypes [".mpe"] = "video/mpeg";
			mimetypes [".mpeg"] = "video/mpeg";
			mimetypes [".mpg"] = "video/mpeg";
			mimetypes [".mpga"] = "audio/mpeg";
			mimetypes [".mxu"] = "video/vnd.mpegur";
			mimetypes [".qt"] = "video/quicktime";
			mimetypes [".ra"] = "audio/x-pn-realaudio";
			mimetypes [".ram"] = "audio/x-pn-realaudio";
			mimetypes [".snd"] = "audio/basic";
			mimetypes [".wav"] = "audio/x-wav";
			
			// image mime types
			mimetypes [".bmp"] = "image/bmp";
			mimetypes [".cgm"] = "image/cgm";
			mimetypes [".djv"] = "image/vnd.djvu";
			mimetypes [".djvu"] = "image/vnd.djvu";
			mimetypes [".gif"] = "image/gif";
			mimetypes [".ico"] = "image/x-icon";
			mimetypes [".ief"] = "image/ief";
			mimetypes [".jp2"] = "image/jp2";
			mimetypes [".jpe"] = "image/jpeg";
			mimetypes [".jpeg"] = "image/jpeg";
			mimetypes [".jpg"] = "image/jpeg";
			mimetypes [".mac"] = "image/x-macpaint";
			mimetypes [".pbm"] = "image/x-portable-bitmap";
			mimetypes [".pct"] = "image/pict";
			mimetypes [".pgm"] = "image/x-portable-graymap";
			mimetypes [".pic"] = "image/pict";
			mimetypes [".pict"] = "image/pict";
			mimetypes [".png"] = "image/png";
			mimetypes [".pnm"] = "image/x-portable-anymap";
			mimetypes [".pnt"] = "image/x-macpaint";
			mimetypes [".pntg"] = "image/x-macpaint";
			mimetypes [".ppm"] = "image/x-portable-pixmap";
			mimetypes [".qti"] = "image/x-quicktime";
			mimetypes [".qtif"] = "image/x-quicktime";
			mimetypes [".ras"] = "image/x-cmu-raster";
			mimetypes [".rgb"] = "image/x-rgb";
			mimetypes [".svg"] = "image/svg+xml";
			mimetypes [".tif"] = "image/tiff";
			mimetypes [".tiff"] = "image/tiff";
			mimetypes [".wbmp"] = "image/vnd.wap.wbmp";
			mimetypes [".xbm"] = "image/x-xbitmap";
			mimetypes [".xpm"] = "image/x-xpixmap";
			mimetypes [".xwd"] = "image/x-xwindowdump";
		}
		
		/// <summary>
		/// Parameterless constructor is needed when parsing jsonstring to object.
		/// </summary>
		public MediaMetadata ()
		{
		}
		
		public MediaType Type { get; set; }

		public string MimeType { get; set; }

		public long Handle { get; set; } // memory pointer (where media file is).
		public string Artist { get; set; }

		public string Title { get; set; }

		public string Album { get; set; }

		public string Category { get; set; }

		public long DurationOffset { get; set; }

		public long Duration { get; set; }
		
		//public byte[] DataBytes { get; set; }
		//public string DataBase64Encoded { get; set; }
		public string ReferenceUrl { get; set; }
		
		public static string GetMimeTypeFromExtension (string path)
		{
			string mimeType = null;
			
			if (path != null) {
				string extension = Path.GetExtension (path);
				if (mimetypes.Contains (extension)) {
					mimeType = (string)mimetypes [extension];
				}
			}
			
			return mimeType;
		}
		
	}
}
