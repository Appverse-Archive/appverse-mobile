// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace UnityUI.iOS
{
	[Register ("UnityUI_iOSViewController_WKWebView")]
	partial class UnityUI_iOSViewController_WKWebView
	{
		[Outlet]
		public WebKit.WKWebView webView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (webView != null) {
				webView.Dispose ();
				webView = null;
			}
		}
	}
}
