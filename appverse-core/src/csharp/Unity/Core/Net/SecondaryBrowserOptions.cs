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
namespace Unity.Core.Net
{
	public class SecondaryBrowserOptions
	{
		private string _title, _url, _closeButtonText, _html = string.Empty;
		private string[] _browserFileExtensions;

		public SecondaryBrowserOptions(){
			CheckNullsAndSetDefaults ();
		}

		public SecondaryBrowserOptions (string title, string url, string closeButtonText, string html, string[] fileExtensions)
		{
			this._title = title;
			this._url = url;
			this._html = html;
			this._closeButtonText = closeButtonText;
			this._browserFileExtensions = fileExtensions;
		}


		public string Title {
			get {
				return _title;
			}
			set{ 
				this._title = value;
			}
		}

		public string Url {
			get {
				return _url;
			}
			set{ 
				this._url = value;
			}
		}

		public string CloseButtonText {
			get {
				return _closeButtonText;
			}
			set{ 
				this._closeButtonText = value;
			}
		}

		public string Html {
			get {
				return _html;
			}
			set{ 
				this._html = value;
			}
		}

		public string[] BrowserFileExtensions {
			get {
				return _browserFileExtensions;
			}
			set{ 
				this._browserFileExtensions = value;
			}
		}

		public string BrowserFileExtensionsAsString(){
			if (this._browserFileExtensions!= null && this._browserFileExtensions.Length > 0) {
				return string.Join (";", this._browserFileExtensions);
			}
			else {
				return string.Empty;
			}
		}

		public void CheckNullsAndSetDefaults(){
			_title=(string.IsNullOrEmpty(this._title))?"Title":this._title;
			_html = (string.IsNullOrEmpty (this._html)) ? "" : this._html;
			_url=(string.IsNullOrEmpty(this._url) && string.IsNullOrEmpty(this._html))?"http://www.google.com":this._url;
			_closeButtonText=(string.IsNullOrEmpty(this._closeButtonText))?"Close":this._closeButtonText;

			_browserFileExtensions = (_browserFileExtensions==null||_browserFileExtensions.Length==0)?null:this._browserFileExtensions;
		}
	}
}

