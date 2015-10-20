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

using System;
using System.IO;

using System.Drawing;
using UIKit;
using CoreGraphics;

namespace Unity.Platform.IPhone
{

	public static class ImageResizer
	{
		static ImageResizer()
		{
		}


		public static byte[] ResizeImage(byte[] imageData, float width, float height)
		{
			// Load the bitmap
			UIImage originalImage = ImageFromByteArray(imageData);
			//
			var Hoehe = originalImage.Size.Height;
			var Breite = originalImage.Size.Width;
			//
			nfloat ZielHoehe = 0;
			nfloat ZielBreite = 0;
			//

			if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
			{
				ZielHoehe = height;
				nfloat teiler = Hoehe / height;
				ZielBreite = Breite / teiler;
			}
			else // Breite (61 for Avatar) ist Master
			{
				ZielBreite = width;
				nfloat teiler = Breite / width;
				ZielHoehe = Hoehe / teiler;
			}
			//
			width = (float)ZielBreite;
			height = (float)ZielHoehe;
			//
			UIGraphics.BeginImageContext(new SizeF(width, height));
			originalImage.Draw(new RectangleF(0, 0, width, height));
			var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			//
			var bytesImagen = resizedImage.AsJPEG().ToArray();
			resizedImage.Dispose();
			return bytesImagen;
		}


		public static UIKit.UIImage ImageFromByteArray(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			//
			UIKit.UIImage image;
			try
			{
				image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
			}
			catch (Exception e)
			{
				Console.WriteLine("Image load failed: " + e.Message);
				return null;
			}
			return image;
		}
	}

	
}
