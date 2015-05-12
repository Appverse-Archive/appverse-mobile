using System;

using UIKit;

namespace FidorLogin
{
	public partial class ViewController : UIViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		//private UITextField LoginText { get; set;}
		//private UITextField PasswordText { get; set;}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		partial void UIButton5_TouchUpInside (UIButton sender)
		{
			var title = "";
			var msg = "";
			if(this.LoginText.Text.Equals("GFT")&&this.PasswordText.Text.Equals("2015"))
			{
				title = "Success!";
				msg = "you are welcome";
			}else{
				title = "Fail!";
				msg = "YOU CANNOT PASS";
			}
			//Create Alert
			var okAlertController = UIAlertController.Create (title, msg, UIAlertControllerStyle.Alert);

			//Add Action
			okAlertController.AddAction (UIAlertAction.Create ("Ok", UIAlertActionStyle.Default, null));

			// Present Alert
			PresentViewController (okAlertController, true, null);
				
		}
	}
}

