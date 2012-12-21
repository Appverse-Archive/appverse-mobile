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
namespace Unity.Core.I18N
{
	public interface II18N
	{

		/// <summary>
		/// List of supported locales for the application.
		/// </summary>
		/// <returns>List of locales.</returns>
		Locale[] GetLocaleSupported ();

		/// <summary>
		/// List of supported locales for the application.
		/// </summary>
		/// <returns>List of locales.</returns>
		string[] GetLocaleSupportedDescriptors ();


		/// <summary>
		/// Get literal for default locale.
		/// </summary>
		/// <param name="key">The key to search for literal.</param>
		/// <returns>Resource literal</returns>
		string GetResourceLiteral (string key);

		/// <summary>
		/// Get literal for given locale.
		/// </summary>
		/// <param name="key">The key to search for literal.</param>
		/// <param name="locale">The locale.</param>
		/// <returns>Resource literal</returns>
		string GetResourceLiteral (string key, Locale locale);

		/// <summary>
		/// Get literal for a given locale.
		/// </summary>
		/// <param name="key">The key to convert to a literal.</param>
		/// <param name="localeDescriptor">String with the locale identifier.</param>
		/// <returns>Resource literal</returns>
		string GetResourceLiteral (string key, string localeDescriptor);
		
		/// <summary>
		/// Get all literals for default locale.
		/// </summary>
		/// <returns>IDictionary containing all the resource literals.</returns>
		ResourceLiteralDictionary GetResourceLiterals ();
		
		/// <summary>
		/// Get all literals for a given locale.
		/// </summary>
		/// <returns>IDictionary containing all the resource literals.</returns>
		/// <param name="locale">The locale.</param>
		ResourceLiteralDictionary GetResourceLiterals (Locale locale);
		
		/// <summary>
		/// Get all literals for a given locale.
		/// </summary>
		/// <returns>IDictionary containing all the resource literals.</returns>
		/// <param name='"localeDescriptor">String with the locale identifier.</param>
		ResourceLiteralDictionary GetResourceLiterals (string localeDescriptor);


	}//end II18N

}//end namespace I18N