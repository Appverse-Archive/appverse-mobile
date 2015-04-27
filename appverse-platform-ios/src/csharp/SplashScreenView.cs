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
using UIKit;
using System.Drawing;
using Unity.Core.System;
using System.Collections.Generic;
using CoreGraphics;

namespace Unity.Platform.IPhone
{
	public class SplashScreenView : UIView
	{
		private UIImageView splashview = null;
		
		private static Dictionary<float,string> availableSplashScreens = new Dictionary<float,string>();

		private static float IPHONE5_HEIGHT_RESOLUTION = 568;
		
		static SplashScreenView() {
		
			// portrait
			availableSplashScreens.Add (320, 	 							@"Default");					// iPhone 3-3GS  / iPhone 4-4S (@2x)
			availableSplashScreens.Add (320*IPHONE5_HEIGHT_RESOLUTION, 		@"Default-568h");				// iPhone 5
			availableSplashScreens.Add (375, 	 							@"Default-667h");  			// iPhone 6
			availableSplashScreens.Add (414, 	 							@"Default-736h");  			// iPhone 6 Plus
			availableSplashScreens.Add (768, 	 							@"Default-Portrait");  			// iPad / iPad Retina (@2x)

			// landscape
			availableSplashScreens.Add (1024,	 							@"Default-Landscape");			// iPad / iPad Retina (@2x)
			availableSplashScreens.Add (736, 	 							@"Default-414h");  			// iPhone 6 Plus
		}
		
		public SplashScreenView (UIInterfaceOrientation orientation)
		{
			splashview = new UIImageView();
			this.SetSplashViewImage(orientation);
			splashview.AutoresizingMask = UIViewAutoresizing.All;// UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;
			splashview.ContentMode = UIViewContentMode.Top;
			this.AutosizesSubviews = false;
			this.AutoresizingMask = UIViewAutoresizing.None;
			this.AddSubview(splashview);
		}
		
		public void SetSplashViewForOrientation(UIInterfaceOrientation orientation) {
			
			this.log("Show SplashScreen for current orientation: " + orientation);
			
			
		    this.SetSplashViewImage(orientation);
			
			//this.LayoutSubviews();
		}
		
		private void SetSplashViewImage(UIInterfaceOrientation orientation) {
			UIScreen screen = UIScreen.MainScreen;
			nfloat statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Height;
			log("bounds width " + screen.Bounds.Width);
			log("bounds height " + screen.Bounds.Height);
			log("status bar height " + statusBarHeight);
			nfloat screenScale = screen.Scale;
			log("scale " + screenScale);

			string splashScreenImage = "";

			short os_major_version = Int16.Parse(UIDevice.CurrentDevice.SystemVersion .Split ('.') [0]);

			// Fix [MOBPLAT-158] :: avoid blank space at the bottom when holding splashscreen on view till view loads in iOS 7 devices
			// Fix [MOBPLAT-158] MAPS 15/11/2013 :: avoid splashscreen position re-allocation in iPad versions < ios7  + apps compiled with iOS7.
			// On those iPad devices the app splashscreen should be placed -20px when the app is compiled with ios7 and run in a < ios7 
			//if(IPhoneUtils.GetInstance().IsIPad() || os_major_version>=7) {
			if(os_major_version>=7) {
				statusBarHeight = 0;
			}
			
			if (orientation == UIInterfaceOrientation.LandscapeRight || orientation == UIInterfaceOrientation.LandscapeLeft)  
		    {   
				if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
					nfloat boundsHeight = screen.Bounds.Width;
					if (availableSplashScreens.ContainsKey ((float)boundsHeight))
						splashScreenImage = availableSplashScreens [(float)boundsHeight];
					splashview.Frame = new CGRect (0, -statusBarHeight, screen.Bounds.Width, screen.Bounds.Height);
					log ("loading splashscreen on landscape [key=" + boundsHeight + "]");
				} else {
					nfloat boundsHeight = screen.Bounds.Height;
					if (availableSplashScreens.ContainsKey ((float)boundsHeight))
						splashScreenImage = availableSplashScreens [(float)boundsHeight];
					splashview.Frame = new CGRect (0, -statusBarHeight, screen.Bounds.Height, screen.Bounds.Width);
					log ("loading splashscreen on landscape [key=" + boundsHeight + "]");
				}

			} 
			else if (orientation == UIInterfaceOrientation.Portrait || orientation == UIInterfaceOrientation.PortraitUpsideDown) 
		    {
				nfloat boundsWidth = screen.Bounds.Width;
				if(screen.Bounds.Height == IPHONE5_HEIGHT_RESOLUTION) {
					boundsWidth = screen.Bounds.Width * IPHONE5_HEIGHT_RESOLUTION;
				}
				if(availableSplashScreens.ContainsKey((float)boundsWidth))
					splashScreenImage = availableSplashScreens[(float)boundsWidth];
				splashview.Frame = new CGRect(0, -statusBarHeight, screen.Bounds.Width, screen.Bounds.Height);
				log("loading splashscreen on portrait [key=" + boundsWidth + "]");
			}
			log("loading splashscreen image " + splashScreenImage);
			splashview.Image = UIImage.FromBundle(splashScreenImage);
		}
		
		
		private void log (string message)
		{
#if DEBUG
			SystemLogger.Log (SystemLogger.Module.GUI, "SplashScreenViewController : " + message);
#endif
			
		}
	}
}

