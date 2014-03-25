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
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using Unity.Core.System;

namespace Unity.Platform.IPhone
{
	public class IPhoneUIViewController : UIViewController
	{
		public UIView contentView {get;set;}
		
		public IPhoneUIViewController (string title, string backButtonText)
		{
			
			contentView = new UIView();
			contentView.Frame = new RectangleF(new PointF(0,0),new SizeF(this.View.Frame.Size.Width, this.View.Frame.Size.Height));
			contentView.BackgroundColor = UIColor.White;
			contentView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			
			UIBarButtonItem backButton = new UIBarButtonItem();
			backButton.Title=backButtonText;
			backButton.Style = UIBarButtonItemStyle.Bordered;
			backButton.Clicked += delegate(object sender, EventArgs e) {
				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					UIView[] subviews = this.View.Subviews;
					foreach(UIView subview in subviews) {
						if(subview.GetType() == typeof(UIWebView) ) {
							UIWebView webView = (UIWebView) subview;
							//clean webview by loading a blank page (prevent video players keep playing after view controller closes)
							webView.LoadHtmlString("<html></html>", new NSUrl("/")); 						
						}
					}
					IPhoneServiceLocator.CurrentDelegate.MainUIViewController ().DismissModalViewControllerAnimated(true);
				});
			};
			
			UINavigationItem navItem = new UINavigationItem(title);
			navItem.LeftBarButtonItem = backButton;
			
			UINavigationBar toolBar = new UINavigationBar();
			toolBar.Frame = new RectangleF(new PointF(0,0), new SizeF(this.View.Frame.Size.Width, this.GetNavigationBarHeight()));
			toolBar.PushNavigationItem(navItem, false);
			toolBar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			contentView.AddSubview(toolBar);     
			
			this.View = contentView;
			
			this.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
			this.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
			this.ModalInPopover = true;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="view">
		/// A <see cref="UIView"/>
		/// </param>
		public void AddInnerView(UIView view) {
			view.BackgroundColor = UIColor.White;
			float navigationBarHeight = this.GetNavigationBarHeight ();
			view.Frame = new RectangleF(new PointF(0,navigationBarHeight), new SizeF(this.View.Bounds.Width, this.View.Bounds.Height-navigationBarHeight));
			view.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			this.View.AddSubview(view);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);

		}

		/// <summary>
		/// Gets the height of the navigation bar to be applied.
		/// </summary>
		/// <returns>The navigation bar height.</returns>
		private float GetNavigationBarHeight() {
			float statusBarHeight = 0;
			float navigationBarHeight = 44;  // default navigation bar height

			try 
			{
				short os_major_version = Int16.Parse(UIDevice.CurrentDevice.SystemVersion .Split ('.') [0]);
				//this.log ("os_major_version: " + os_major_version);
				if(os_major_version>=7) {
					// when using landscape the width should be used, for portrait the height value
					statusBarHeight = Math.Min(UIApplication.SharedApplication.StatusBarFrame.Height, UIApplication.SharedApplication.StatusBarFrame.Width);
					//this.log ("statusBarHeight: " + statusBarHeight);
				} 
			} 
			catch (Exception ex) {
				this.log ("Exception detecting status bar height for current iOS SDK... Error message: " + ex.Message);
			}

			return navigationBarHeight + statusBarHeight;
		}

		private void log (string message)
		{
			#if DEBUG
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "IPhoneUIViewController : " + message);
			#endif

		}
	}
}

