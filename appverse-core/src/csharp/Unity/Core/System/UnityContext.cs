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

namespace Unity.Core.System
{
	public class UnityContext
	{
		/*
        private bool _Mac = false;
		
        private bool _Linux = false;
		*/

		private bool _Windows = false;
		private bool _iPod = false;
		private bool _iPad = false;
		private bool _iPhone = false;
		private bool _Android = false;
		private bool _Blackberry = false;
		private bool _TabletDevice = false; // hidden field

		/*
        public bool Mac
        {
            get
            {
                return _Mac;
            }
            set
            {
                _Mac = value;
            }
        }

        public bool Linux
        {
            get
            {
                return _Linux;
            }
            set
            {
                _Linux = value;
            }
        }
         */

		public bool Windows {
			get {
				return _Windows;
			}
			set {
				_Windows = value;
			}
		}

		public bool iPod {
			get {
				return _iPod;
			}
			set {
				_iPod = value;
			}
		}

		public bool iPad {
			get {
				return _iPad;
			}
			set {
				_iPad = value;
			}
		}

		public bool iPhone {
			get {
				return _iPhone;
			}
			set {
				_iPhone = value;
			}
		}

		public bool Android {
			get {
				return _Android;
			}
			set {
				_Android = value;
			}
		}

		public bool Blackberry {
			get {
				return _Blackberry;
			}
			set {
				_Blackberry = value;
			}
		}

		public bool TabletDevice {
			get {
				return _TabletDevice;
			}
			set {
				_TabletDevice = value;
			}
		}

		/*
        public bool Desktop
        {
            get
            {
                return (this.Mac || this.Windows || (this.Linux && !this.Android));
            }
        }
        */

		public bool Tablet {
			get {
				return (this.iPad || this.TabletDevice);
			}
		}

		public bool Phone {
			get {
				//return (!this.Desktop && !this.Tablet);
				return (!this.Tablet);
			}
		}

		public bool iOS {
			get {
				return (this.iPhone || this.iPad || this.iPod);
			}
		}

		
		
	}
}
