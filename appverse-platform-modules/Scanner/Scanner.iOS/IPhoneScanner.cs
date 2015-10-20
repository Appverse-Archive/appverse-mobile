/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
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
using Appverse.Core.Scanner;

using ZXing.Mobile;
using ZXing.Client.Result;
using UIKit;
using Unity.Core.IO.ScriptSerialization;
using WebKit;
using Foundation;
using System.Threading;
using MessageUI;
using ZXing.QrCode;
using ZXing;
using ZXing.Common;
using Unity.Core.Media; 
using Unity.Core.Storage.FileSystem;
using System.IO;
using System.Text;


namespace Appverse.Platform.IPhone
{
	public class IPhoneScanner : AbstractScanner
	{

		public static string DEFAULT_ROOT_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		public IPhoneScanner ()
		{
		}

		#region implemented abstract members of AbstractScanner

		public override void GenerateQRCode(MediaQRContent content)
		{
			SystemLogger.Log (SystemLogger.Module.PLATFORM, "1");
			try{
				MediaMetadata mediaData = new MediaMetadata();
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "2");

				int size = content.Size;
				if (size == 0) {
					size = 256;
				}
				var writer = new ZXing.BarcodeWriter
				{ 
					Format = BarcodeFormat.QR_CODE, 
					Options = new EncodingOptions { Height = size, Width = size } 
				};
				//var img = writer.Write(content.Text);
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "3");

				var uuid = Guid.NewGuid ();
				string s = uuid.ToString ();
				String filename = "QR_" + s;
				NSError err;
				DirectoryData dest = new DirectoryData(DEFAULT_ROOT_PATH);
				string path = Path.Combine(dest.FullName, filename+".png");
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "4");
				content = encodeQRCodeContents(content);
				using(UIImage img = writer.Write(content.Text)) {
					
					using (var data = img.AsPNG ()) {
						data.Save (path, true, out err);
					}
				}
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "5");

				mediaData.ReferenceUrl = filename+".png";
				mediaData.Title = filename;

				SystemLogger.Log (SystemLogger.Module.PLATFORM, "6");

				UIApplication.SharedApplication.InvokeOnMainThread (delegate {
					UIViewController viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
					FireUnityJavascriptEvent(viewController, "Appverse.Scanner.onGeneratedQR", mediaData);

				});
			}catch(Exception ex)
			{
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "GenerateQRCode - exception: " + ex.Message);
			}
		}

		private MediaQRContent encodeQRCodeContents(MediaQRContent qrCode)
		{
			var data = qrCode.Text;

			switch (qrCode.QRType) 
			{
			case QRType.EMAIL_ADDRESS:
				qrCode.Text = "mailto:" + data;
				break;
			case QRType.GEO:
				var coord = qrCode.Coord;
				if(coord != null)
					qrCode.Text = "geo:" + coord.Latitude +","+ coord.Longitude;
				break;
			case QRType.SMS:
				qrCode.Text = "sms:" + data;
				break;
			case QRType.TEL:
				qrCode.Text = "tel:" + data;
				break;
			case QRType.ADDRESSBOOK:
				StringBuilder sb = new StringBuilder ("MECARD:");
				var name = qrCode.Contact.Name;
				if (string.IsNullOrEmpty (name) == true)
					sb.Append ("N:" + name + ";");

				var address = qrCode.Contact.Address;
				if (string.IsNullOrEmpty (address) == true)
					sb.Append ("ADR:" + address + ";");

				var phone = qrCode.Contact.Phone;
				if (string.IsNullOrEmpty (phone) == true)
					sb.Append ("TEL:" + phone + ";");

				var email = qrCode.Contact.Email;
				if (string.IsNullOrEmpty (email) == true)
					sb.Append ("EMAIL:" + email + ";");

				var url = qrCode.Contact.Url;
				if (string.IsNullOrEmpty (url) == true)
					sb.Append ("URL:" + url + ";");

				var note = qrCode.Contact.Note;
				if (string.IsNullOrEmpty (note) == true)
					sb.Append ("NOTE:" + note + ";");
				
				qrCode.Text = sb.ToString();
				break;
			default:
				break;
			}

			return qrCode;

		}

		public override void DetectQRCode (bool autoHandleQR)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {

				UIViewController viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;

				MobileBarcodeScanner scanner = new MobileBarcodeScanner(viewController);

		
				scanner.Scan().ContinueWith(t => {   
					MediaQRContent resultQRContent = null;
					if (t.Result != null){
						resultQRContent = new MediaQRContent(t.Result.Text, ZxingToBarcode(t.Result.BarcodeFormat), getQRTypeFromCode(t.Result));
						//SystemLogger.Log(SystemLogger.Module.PLATFORM, "QR CODE returnValue: " + resultQRContent);
					}

					UIApplication.SharedApplication.InvokeOnMainThread (delegate {
						FireUnityJavascriptEvent(viewController, "Appverse.Scanner.onQRCodeDetected", resultQRContent);
						if(autoHandleQR) HandleQRCode(resultQRContent);
					});

				});
				// TODO: check how to do this
				//IPhoneServiceLocator.CurrentDelegate.SetMainUIViewControllerAsTopController(false);
			});

		}

		public override void DetectQRCodeFront (bool autoHandleQR)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {

				UIViewController viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;

				MobileBarcodeScanner scanner = new MobileBarcodeScanner(viewController);

				var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
				options.UseFrontCameraIfAvailable = true;
				//TODO OR NOT TODO FLASH
				scanner.Scan(options).ContinueWith(t => {   
					MediaQRContent resultQRContent = null;
					if (t.Result != null){
						resultQRContent = new MediaQRContent(t.Result.Text, ZxingToBarcode(t.Result.BarcodeFormat), getQRTypeFromCode(t.Result));
						//SystemLogger.Log(SystemLogger.Module.PLATFORM, "QR CODE returnValue: " + resultQRContent);
					}

					UIApplication.SharedApplication.InvokeOnMainThread (delegate {
						FireUnityJavascriptEvent(viewController, "Appverse.Scanner.onQRCodeDetected", resultQRContent);
						if(autoHandleQR) HandleQRCode(resultQRContent);
					});

				});
				// TODO: check how to do this
				//IPhoneServiceLocator.CurrentDelegate.SetMainUIViewControllerAsTopController(false);
			});

		}

		private BarCodeType ZxingToBarcode (ZXing.BarcodeFormat format){
			foreach(BarCodeType type in Enum.GetValues(typeof(BarCodeType))){
				if(format.ToString().Equals(type.ToString())) return type;
			}
			return BarCodeType.DEFAULT;

		}

		private ZXing.BarcodeFormat BarcodeToZxing (BarCodeType format){
			foreach(ZXing.BarcodeFormat type in Enum.GetValues(typeof(ZXing.BarcodeFormat))){
				if(format.ToString().Equals(type.ToString())) return type;
			}

			return ZXing.BarcodeFormat.QR_CODE;
		}

		private QRType getQRTypeFromCode (ZXing.Result readQRCode){
			ParsedResult parsed = ResultParser.parseResult(readQRCode);
			switch(parsed.Type){
			case ParsedResultType.ADDRESSBOOK:
				return QRType.ADDRESSBOOK;
			case ParsedResultType.CALENDAR:
				return QRType.CALENDAR;
			case ParsedResultType.EMAIL_ADDRESS:
				return QRType.EMAIL_ADDRESS;
			case ParsedResultType.GEO:
				return QRType.GEO;
			case ParsedResultType.ISBN:
				return QRType.ISBN;
			case ParsedResultType.PRODUCT:
				return QRType.PRODUCT;
			case ParsedResultType.SMS:
				return QRType.SMS;
			case ParsedResultType.TEL:
				return QRType.TEL;
			case ParsedResultType.URI:
				return QRType.URI;
			case ParsedResultType.WIFI:
				return QRType.WIFI;
			case ParsedResultType.TEXT:
			default:
				return QRType.TEXT;
			}
		}


		public override QRType HandleQRCode (MediaQRContent mediaQRContent)
		{
			if (mediaQRContent != null) {
				try {
					NSUrl param = new NSUrl (mediaQRContent.Text);

					UIApplication.SharedApplication.InvokeOnMainThread (delegate {
						// UIApplication.SharedApplication.OpenUrl method must be called from UI main thread [AMOB-14]
						switch (mediaQRContent.QRType) {
						case QRType.EMAIL_ADDRESS:
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "HandleQRCode - EMAIL_ADDRESS type");
							if ((UIApplication.SharedApplication.CanOpenUrl (param)) && (MFMailComposeViewController.CanSendMail)) {
								UIApplication.SharedApplication.OpenUrl (param);
							} else {
								StartNotifyAlert ("Mail Alert", "Sending of mail messages is not enabled or supported on this device.", "OK");
							}
							break;
						case QRType.TEL:

								SystemLogger.Log (SystemLogger.Module.PLATFORM, "HandleQRCode - TEL type");
								if (UIApplication.SharedApplication.CanOpenUrl (param)) {
									UIApplication.SharedApplication.OpenUrl (param);
								} else {
									StartNotifyAlert ("Phone Alert", "Establishing voice calls is not enabled or supported on this device.", "OK");
								}
							
							break;
						case QRType.URI:
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "HandleQRCode - URI type");
							if (UIApplication.SharedApplication.CanOpenUrl (param)) {
								UIApplication.SharedApplication.OpenUrl (param);
							} else {
								StartNotifyAlert ("Browser Alert", "The requested URL could not be automatically opened.", "OK");
							}
							break;
						default:
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "HandleQRCode - not maanged type");
							StartNotifyAlert ("QR Alert", "The QR Code " + mediaQRContent.QRType.ToString() + " cannot be processed automatically.", "OK");
							break;
						}

					});
				} catch (Exception ex) {
					SystemLogger.Log (SystemLogger.Module.PLATFORM, "HandleQRCode - exception: " + ex.Message);
					StartNotifyAlert ("QR Alert", "The QR Code cannot be handled due to an unhandled exception (see log).", "OK");
				}
				return mediaQRContent.QRType;
			}

			return QRType.TEXT;
		}

		private bool StartNotifyAlert (string title, string message, string buttonText)
		{
			using (var pool = new NSAutoreleasePool ()) {
				var thread = new Thread (ShowAlert);
				string[] alertData = new string[] { title, message, buttonText };
				thread.Start (alertData);
			}
			return true;
		}

		[Export("ShowAlert")]
		private void ShowAlert (object alertData)
		{
			UIApplication.SharedApplication.InvokeOnMainThread (delegate { 
				string[] alertParams = (string[]) alertData;
				UIAlertView alert = new UIAlertView(alertParams[0],alertParams[1],null,alertParams[2],null);
				alert.Show();
			});
		}


		private void FireUnityJavascriptEvent (UIViewController viewController, string method, object data)
		{
			JavaScriptSerializer Serialiser = new JavaScriptSerializer (); 
			string dataJSONString = "null";
			if (data != null) {
				dataJSONString = Serialiser.Serialize (data);
				if (data is String) {
					dataJSONString = "'" + (data as String) + "'";
				}
			}
			string jsCallbackFunction = "if("+method+"){"+method+"("+dataJSONString+");}";
			//only for testing //SystemLogger.Log(SystemLogger.Module.PLATFORM, "NotifyJavascript (single object): " + method + ", dataJSONString: " + dataJSONString);

			bool webViewFound = false;
			if (viewController != null && viewController.View != null) {

				UIView[] subViews = viewController.View.Subviews;

				foreach(UIView subView in subViews) {
					if (subView is UIWebView) {
						webViewFound = true;

						// evaluate javascript as a UIWebView
						(subView as UIWebView).EvaluateJavascript (jsCallbackFunction);

					} else if (subView is WKWebView) {
						webViewFound = true;

						// evaluate javascript as a WKWebView
						(subView as WKWebView).EvaluateJavaScript (new NSString(jsCallbackFunction), delegate (NSObject result, NSError error) {
							SystemLogger.Log (SystemLogger.Module.PLATFORM, "NotifyJavascript COMPLETED (" + method + ")");
						});
					}
				}
			} 

			if (webViewFound) {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "NotifyJavascript EVALUATED (" + method + ")");
			} else {
				SystemLogger.Log (SystemLogger.Module.PLATFORM, "It was not possible to find a WebView to evaluate the javascript method");
			}

		}

		#endregion

	}
}

