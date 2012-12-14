/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  (“APL v2.0”).  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://www.appverse.mobi/licenses/apl_v2.0.pdf.

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
package com.gft.unity.core.i18n;

public interface II18N {

    /**
     * Provides the list of supported locales for the application.
     *
     * @return List of locales.
     */
    public Locale[] GetLocaleSupported();

    /**
     * Provides the list of supported locales for the application.
     *
     * @return List of locales.
     */
    public String[] GetLocaleSupportedDescriptors();

    /**
     * Returns literal "key" for the default locale.
     *
     * @param key The key to search for literal.
     * @return Resource literal.
     */
    public String GetResourceLiteral(String key);

    /**
     * Returns literal "key" for locale "locale".
     *
     * @param key The key to search for literal.
     * @param locale The locale.
     * @return Resource literal.
     */
    public String GetResourceLiteral(String key, Locale locale);

    /**
     * Returns literal "key" for locale "localeDescriptor".
     *
     * @param key The key to convert to a literal.
     * @param localeDescriptor String with the locale identifier.
     * @return Resource literal.
     */
    public String GetResourceLiteral(String key, String localeDescriptor);

    /**
     * Returns all literals for the default locale.
     *
     * @return All literals.
     */
    public ResourceLiteralDictionary GetResourceLiterals();

    /**
     * Returns all literals for locale "locale".
     *
     * @param locale The locale.
     * @return All literals.
     */
    public ResourceLiteralDictionary GetResourceLiterals(Locale locale);
}
