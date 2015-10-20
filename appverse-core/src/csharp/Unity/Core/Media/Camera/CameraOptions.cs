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

namespace Unity.Core.Media.Camera
{
	public class CameraOptions
	{
		public CameraOptions() {
			ImageScaleFactor = 1;	 // default scale factor is 1
			UseFrontCamera = false;  // default is to use back camera
			UseCustomCameraOverlay = false;  // default is to use native camera overlay
			GuidelinesMargins = 10;  // default is 10 pixels margin

			GuidelinesColorHexadecimal = "#999999";
			ScanButtonColorHexadecimal = "#999999";
			ScanButtonPressedColorHexadecimal = "#666666";

			ScanButtonMarginBottom = 30; // default is 30 pixels margin-bottom

			ScanButtonWidth = 75;
			ScanButtonHeight = 35;

			ScanButtonIconWidth = 28; 
			ScanButtonIconHeight = 24;
			ScanButtonIconColorHexadecimal = "#ffffff";  // white icon by default

			DescriptionLabelText = "Place here your description";  // null to avoid description being shown
			DescriptionLabelColorHexadecimal = "#ffffff"; 
			DescriptionLabelMarginBottom = 60;
			DescriptionLabelMarginLeftRight = 25;
			DescriptionLabelHeight = 60;
			DescriptionLabelFontFamilyName = "Helvetica";  // each platform should define its own default font (among the ones available)
			DescriptionLabelFontSize = 20;

			CancelButtonText = "Cancel";
			CancelButtonColorHexadecimal = "#ffffff"; 
			CancelButtonWidth = 80;
			CancelButtonHeight = 35;
			CancelButtonFontFamilyName = "Helvetica";  // each platform should define its own default font (among the ones available)
			CancelButtonFontSize = 20;

		}

		public float ImageScaleFactor { get; set; }
		public bool UseFrontCamera { get; set; }
		public bool UseCustomCameraOverlay { get; set; }

		// guidelines customization
		public int GuidelinesMargins { get; set; }
		public string GuidelinesColorHexadecimal { get; set; }

		// scan button customization
		public string ScanButtonColorHexadecimal { get; set; }
		public string ScanButtonPressedColorHexadecimal { get; set; }
		public int ScanButtonWidth { get; set; }
		public int ScanButtonHeight { get; set; }
		public int ScanButtonMarginBottom { get; set;}
		public string ScanButtonIconColorHexadecimal { get; set; }
		public int ScanButtonIconWidth { get; set; }
		public int ScanButtonIconHeight { get; set; }

		// description label
		public string DescriptionLabelText { get; set; }
		public string DescriptionLabelColorHexadecimal { get; set; }
		public int DescriptionLabelMarginBottom { get; set;}
		public int DescriptionLabelMarginLeftRight { get; set;}
		public int DescriptionLabelHeight { get; set; }
		public string DescriptionLabelFontFamilyName  { get; set; }
		public int DescriptionLabelFontSize { get; set; }

		// cancel button
		public string CancelButtonText { get; set; }
		public string CancelButtonColorHexadecimal { get; set; }
		public int CancelButtonWidth { get; set; }
		public int CancelButtonHeight { get; set; }
		public string CancelButtonFontFamilyName  { get; set; }
		public int CancelButtonFontSize { get; set; }

	}

}