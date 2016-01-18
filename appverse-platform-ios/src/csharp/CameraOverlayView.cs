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

using Unity.Core.Media.Camera;
using UIKit;
using CoreGraphics;
using Unity.Core.Log;
using Unity.Core.System;
using System.IO;
using System.Security.Cryptography;
using Foundation;

namespace Unity.Platform.IPhone
{

	public class CameraOverlayView : UIView
	{

		// Transform values for full screen support
		public static nfloat CAMERA_ASPECT_RATIO = 1.333333f;	// 4:3 is the aspect ratio for taking photos

		public UIImagePickerController imagePickerController;
		public CameraOptions overlaySettings = new CameraOptions(); // default values

		public CameraOverlayView (UIImagePickerController controller, CameraOptions _settings, CGRect frame) : base (frame)
		{
			log ("Intializing Camera Overlay from frame...");
			if(_settings != null)
				overlaySettings = _settings;

			// Clear the background of the overlay:
			this.Opaque = false;  
			this.BackgroundColor = UIColor.Clear;  // transparent

			UIImage overlayGraphic;
			string OverlayImage = _settings.Overlay;

			log ("Overlay Image: "+OverlayImage);
			if (OverlayImage != null && File.Exists("./"+OverlayImage+".png")){
					log ("Overlay Image: "+OverlayImage + " found!");
					overlayGraphic = UIImage.FromBundle (OverlayImage+".png");

			} else {
				log ("Overlay Image not found");
				// Load the image to show in the overlay:
				overlayGraphic = UIImage.FromBundle (@"overlaygraphic.png");
			}
			overlayGraphic = overlayGraphic.ImageWithRenderingMode (UIImageRenderingMode.AlwaysTemplate);  // convert image to template

			UIImageView overlayGraphicView = new UIImageView (overlayGraphic);
			overlayGraphicView.Frame = new CGRect(
				overlaySettings.GuidelinesMargins, 
				overlaySettings.GuidelinesMargins, 
				frame.Width - overlaySettings.GuidelinesMargins * 2 , 
				frame.Height - overlaySettings.GuidelinesMargins * 2);

			log ("guidelines tint color: " + overlaySettings.GuidelinesColorHexadecimal);
			UIColor color = GetUIColorfromHex (overlaySettings.GuidelinesColorHexadecimal);
			if (color != null)
				overlayGraphicView.TintColor = color;

			this.AddSubview (overlayGraphicView);

			ScanButton scanButton = new ScanButton (
				new CGRect ((frame.Width / 2) - (overlaySettings.ScanButtonWidth /2), frame.Height - overlaySettings.ScanButtonHeight - overlaySettings.ScanButtonMarginBottom , 
					overlaySettings.ScanButtonWidth, overlaySettings.ScanButtonHeight), overlaySettings);
			scanButton.TouchUpInside += delegate(object sender, EventArgs e) {

				log("Scan Button TouchUpInside... " );

				controller.TakePicture();
			};

			this.AddSubview (scanButton);

			if (overlaySettings.DescriptionLabelText != null) {
				UILabel label = new UILabel (new CGRect (overlaySettings.DescriptionLabelMarginLeftRight, 
									frame.Height - overlaySettings.DescriptionLabelHeight - overlaySettings.DescriptionLabelMarginBottom, 
					frame.Width - overlaySettings.DescriptionLabelMarginLeftRight * 2, overlaySettings.DescriptionLabelHeight));  // applying "DescriptionLabelMarginLeftRight" margins to width and x position
				label.Text = overlaySettings.DescriptionLabelText;

				color = GetUIColorfromHex (overlaySettings.DescriptionLabelColorHexadecimal);
				if(color != null)
					label.TextColor = color; 
				label.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
				label.TextAlignment = UITextAlignment.Center; // centered aligned
				label.LineBreakMode = UILineBreakMode.WordWrap; // wrap text by words
				label.Lines = 2;

				if (overlaySettings.DescriptionLabelFontFamilyName != null) {
					UIFont labelFont = UIFont.FromName (overlaySettings.DescriptionLabelFontFamilyName, overlaySettings.DescriptionLabelFontSize);
					if (labelFont != null) {
						label.Font = labelFont;
					} else {
						log ("Font family [" + overlaySettings.DescriptionLabelFontFamilyName + "] for 'DescriptionLabelFontFamilyName' not found");
					}
				}

				this.AddSubview (label);
			}

			UILabel cancelLabel = new UILabel (new CGRect ((frame.Width / 4) - (overlaySettings.CancelButtonWidth /2), 
				frame.Height - overlaySettings.CancelButtonHeight - overlaySettings.ScanButtonMarginBottom, 
				overlaySettings.CancelButtonWidth, overlaySettings.CancelButtonHeight));
			cancelLabel.Text = overlaySettings.CancelButtonText;
			color = GetUIColorfromHex (overlaySettings.CancelButtonColorHexadecimal);
			if(color != null)
				cancelLabel.TextColor = color;
			cancelLabel.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
			cancelLabel.TextAlignment = UITextAlignment.Center; // centered aligned

			// list of available ios fonts: https://developer.xamarin.com/recipes/ios/standard_controls/fonts/enumerate_fonts/
			if (overlaySettings.CancelButtonFontFamilyName != null) {
				UIFont cancelLabelFont = UIFont.FromName (overlaySettings.CancelButtonFontFamilyName, overlaySettings.CancelButtonFontSize);
				if (cancelLabelFont != null) {
					cancelLabel.Font = cancelLabelFont;
				} else {
					log ("Font family [" + overlaySettings.CancelButtonFontFamilyName + "] for 'CancelButtonFontFamilyName' not found");
				}
			}

			UITapGestureRecognizer cancelLabelTap = new UITapGestureRecognizer(() => {
				log("Cancel Button TouchesEnded... " );
				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					SystemLogger.Log(SystemLogger.Module.PLATFORM, "Canceled picking image ");
					IPhoneUtils.GetInstance().FireUnityJavascriptEvent("Appverse.Media.onFinishedPickingImage", null);
					IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().DismissModalViewController(true);
				});
			});

			cancelLabel.UserInteractionEnabled = true;
			cancelLabel.AddGestureRecognizer(cancelLabelTap);

			this.AddSubview (cancelLabel);

		}

		public UIColor GetUIColorfromRGB (int red, int green, int blue) {
			return UIColor.FromRGBA (red, green, blue, 255);
		}

		public static UIColor GetUIColorfromHex (string hexValue) {
			if (hexValue == null)
				return null;
			float alpha = 1.0f; // opacity hardcoded
			

			var colorString = hexValue.Replace ("#", "");
			
			if (alpha > 1.0f) {
				alpha = 1.0f;
			} else if (alpha < 0.0f) {
				alpha = 0.0f;
			}

			float red, green, blue;

			switch (colorString.Length) 
			{
			case 3 : // #RGB
				{
					red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
					green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
					blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
					return UIColor.FromRGBA(red, green, blue, alpha);
				}
			case 6 : // #RRGGBB
				{
					red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
					green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
					blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
					return UIColor.FromRGBA(red, green, blue, alpha);
				}   

			default :
				throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));

			}
		}

		protected void log (string message)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "CameraOverlayView : " + message);

		}
	}

	public class ScanButton : UIButton {

		public ScanButton (CGRect frame, CameraOptions options) : base (frame)
		{
			UIImage buttonImage  = UIImage.FromBundle(@"scanbutton");
			buttonImage = buttonImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);  // convert image to template

			this.SetImage (buttonImage, UIControlState.Normal);
			this.SetImage (buttonImage, UIControlState.Highlighted);

			UIImage buttonIconImage  = UIImage.FromBundle(@"scaniconbutton");
			buttonIconImage = buttonIconImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);  // convert image to template

			UIImageView buttonIconImageView = new UIImageView (buttonIconImage);

			if(options != null) {
				buttonIconImageView.Frame =  new CGRect(
					(frame.Width / 2) - (options.ScanButtonIconWidth /2), 
					(frame.Height / 2) - (options.ScanButtonIconHeight /2), 
					options.ScanButtonIconWidth , 
					options.ScanButtonIconHeight);  // we will place the icon in the middle of the button image

				SystemLogger.Log (SystemLogger.Module.PLATFORM, "ScanButton : tint color to be used " + options.ScanButtonColorHexadecimal);
				UIColor color = CameraOverlayView.GetUIColorfromHex (options.ScanButtonColorHexadecimal);
				if (color != null)
					this.TintColor = color;
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "ScanButton : tint color to be used for icon " + options.ScanButtonIconColorHexadecimal);
				color = CameraOverlayView.GetUIColorfromHex (options.ScanButtonIconColorHexadecimal);
				if (color != null)
					buttonIconImageView.TintColor = color;
			}

			this.TouchUpInside += delegate {

				if(options != null) {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "ScanButton : TouchUpInside... changing color to:  " + options.ScanButtonPressedColorHexadecimal);
					UIColor color = CameraOverlayView.GetUIColorfromHex (options.ScanButtonPressedColorHexadecimal);
					if(color != null)
						this.TintColor = color;
				}
			};

			this.AddSubview (buttonIconImageView);  // adding inside image as a new subview

		}


	}


	
}
