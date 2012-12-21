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
using System.Globalization;

namespace Unity.Core.System
{
	public class Locale : IComparable<Locale>
	{
		public string Language { get; set; }

		public string Country { get; set; }

		public Locale ()
		{
		}
		
		public Locale (string localeDescriptor)
		{ 
			if (localeDescriptor != null) {
				string[] localeData = localeDescriptor.Split ('-');
				if (localeData.Length > 0) {
					this.Language = localeData [0];	
				}
				if (localeData.Length > 1) {
					this.Country = localeData [1];
				}
			}
		}

		/// <summary>
		/// DO NOT USE THIS METHOD, COUNTRY IS NOT BEING PROVIDED. 
		/// </summary>
		/// <param name="ci">
		/// A <see cref="CultureInfo"/>
		/// </param>
		public Locale (CultureInfo ci)
		{
			if (ci != null) {
				this.Language = ci.TwoLetterISOLanguageName;
				if (!ci.IsNeutralCulture) {

					try {
						RegionInfo ri = new RegionInfo (ci.LCID);
						// Bug:: "TwoLetterISORegionName" is always empty.
						this.Country = ri.TwoLetterISORegionName;
					} catch (Exception) {
						//throw;
					}
				}
			}
		}

		public override string ToString ()
		{
			string localeString = "";

			if (this.Language != null && this.Language.Length > 0) {
				localeString = localeString + this.Language;
			}

			if (this.Country != null && this.Country.Length > 0) {
				localeString = localeString + "-" + this.Country;
			}

			return localeString;
		}

        #region Miembros de IComparable<Locale>

		public int CompareTo (Locale other)
		{
			return this.ToString ().CompareTo (other.ToString ());
		}

        #endregion
	}
}
