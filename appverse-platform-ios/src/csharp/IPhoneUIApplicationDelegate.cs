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
using Foundation;
using Unity.Platform.IPhone;
using Unity.Core.System;
using EventKit;
using AddressBook;

namespace Unity.Platform.IPhone
{
	public abstract partial class IPhoneUIApplicationDelegate: UIApplicationDelegate
	{

		/// <summary>
		/// The EKEventStore is intended to be long-lived. It's expensive to new it up
		/// and can be thought of as a database, so we create a single instance of it
		/// and reuse it throughout the app
		/// </summary>
		public EKEventStore EventStore
		{
			get { return eventStore; }
		}
		protected EKEventStore eventStore;

		public ABAddressBook AddressBook
		{
			get { return addressBook; }
		}
		protected ABAddressBook addressBook;

	
		public void ReloadAddressBook() {
			#if DEBUG
			log ("IPhoneUIApplicationDelegate creating address book instance");
			#endif
			if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {
				NSError nsError = new NSError();
				addressBook =ABAddressBook.Create(out nsError);
				#if DEBUG
				log ("IPhoneUIApplicationDelegate creating address book result: " +((nsError!=null)?nsError.Description:"no error"));
				#endif
			} else {
				addressBook = new ABAddressBook();
			}
		}

		public IPhoneUIApplicationDelegate () : base()
		{
			#if DEBUG
			log ("IPhoneUIApplicationDelegate constructor default");
			#endif

			IPhoneServiceLocator.CurrentDelegate = this;
			IPhoneServiceLocator.UIApplicationWeakDelegate = new UIApplicationWeakDelegate ();

			#if DEBUG
			log ("IPhoneUIApplicationDelegate creating event store instance");
			#endif
			eventStore = new EKEventStore ( );
			// creating address book instance
			ReloadAddressBook ();
			#if DEBUG
			log ("IPhoneUIApplicationDelegate constructor successfully ended");
			#endif
		}

		public IPhoneUIApplicationDelegate (IntPtr ptr) : base(ptr)
		{
			#if DEBUG
			log ("IPhoneUIApplicationDelegate constructor IntPtr");
			#endif
			IPhoneServiceLocator.CurrentDelegate = this;
			eventStore = new EKEventStore ( );
			if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {
				NSError nsError = new NSError();
				addressBook =ABAddressBook.Create(out nsError);
				#if DEBUG
				log ("IPhoneUIApplicationDelegate creating address book result: " +((nsError!=null)?nsError.Description:"no error"));
				#endif
			} else {
				addressBook = new ABAddressBook();
			}
		}
		/* deprecated
		public IPhoneUIApplicationDelegate (NSCoder coder) : base(coder)
		{
			#if DEBUG
			log ("AppDelegate constructor NSCoder");
			#endif
			IPhoneServiceLocator.CurrentDelegate = this;
			eventStore = new EKEventStore ( );
			if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {
				NSError nsError = new NSError();
				addressBook =ABAddressBook.Create(out nsError);
				#if DEBUG
				log ("IPhoneUIApplicationDelegate creating address book result: " +((nsError!=null)?nsError.Description:"no error"));
				#endif
			} else {
				addressBook = new ABAddressBook();
			}
		}
		*/

		public IPhoneUIApplicationDelegate (NSObjectFlag flag) : base(flag)
		{
			#if DEBUG
			log ("IPhoneUIApplicationDelegate constructor NSObjectFlag");
			#endif
			IPhoneServiceLocator.CurrentDelegate = this;
			eventStore = new EKEventStore ( );
			if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {
				NSError nsError = new NSError();
				addressBook =ABAddressBook.Create(out nsError);
				#if DEBUG
				log ("IPhoneUIApplicationDelegate creating address book result: " +((nsError!=null)?nsError.Description:"no error"));
				#endif
			} else {
				addressBook = new ABAddressBook();
			}
		}

		//not used :: public abstract UIWindow MainAppWindow ();

		public abstract UIViewController MainUIViewController ();

		public abstract bool ShowSplashScreen (UIInterfaceOrientation orientation);

		public abstract bool DismissSplashScreen ();

		//public abstract UIWebView MainUIWebView () ;
		public abstract void EvaluateJavascript (string jsStringToEvaluate);
		public abstract void LoadRequest (NSUrlRequest request);

		public abstract bool ShouldActivateManagedServices (); 

		public abstract void SetMainUIViewControllerAsTopController(bool topController);

		public abstract bool SecurityChecksPassed();

		public abstract int GetListeningPort ();
		
		#if DEBUG
		private void log (string message)
		{
			SystemLogger.Log (SystemLogger.Module.GUI, "AppDelegate: " + message);
			
		}
		#endif

	}
}

