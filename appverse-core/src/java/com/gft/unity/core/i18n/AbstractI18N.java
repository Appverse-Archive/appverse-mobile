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
package com.gft.unity.core.i18n;

import java.util.List;

public abstract class AbstractI18N implements II18N {

    protected static final String I18N_CONFIG_FILE = "app/config/i18n-config.xml";
    protected static final String APP_CONFIG_PATH = "app/config";
    protected static final String PLIST_EXTENSION = ".plist";
    protected static final String ENCODING = "UTF-8";
    protected static final String DEFAULT_LOCALE_TAG = "default";
    protected static final String SUPPORTED_LOCALE_TAG = "supportedLanguage";
    protected static final String SUPPORTED_LOCALES_TAG = "supportedLanguages";
    protected static final String LANGUAGE_ATTR = "language";
    protected static final String COUNTRY_ATTR = "country";
    private I18NConfiguration configuration;

    public AbstractI18N() {
        configuration = loadConfiguration();
    }
    
    public String getDefaultLocale() {
        if(configuration == null || configuration.getDefaultLocale() == null)
            return "undefined";
        return configuration.getDefaultLocale().toString();
    }

    @Override
    public Locale[] GetLocaleSupported() {

        List<Locale> locales = configuration.getSupportedLocales();
        return locales.toArray(new Locale[locales.size()]);
    }

    @Override
    public String[] GetLocaleSupportedDescriptors() {

        List<Locale> locales = configuration.getSupportedLocales();
        String[] descriptors = new String[locales.size()];
        int index = 0;
        for (Locale locale : locales) {
            descriptors[index] = locale.toString();
            index++;
        }
        return descriptors;
    }

    protected abstract I18NConfiguration loadConfiguration();

    @Override
    public String GetResourceLiteral(String key) {
        return GetResourceLiteral(key, configuration.getDefaultLocale());
    }

    @Override
    public String GetResourceLiteral(String key, Locale locale) {
        String literal;

        if (locale == null) {
            literal = GetResourceLiteral(key);
        } else {
            literal = GetResourceLiteral(key, locale.toString());
        }

        return literal;
    }

    @Override
    public abstract String GetResourceLiteral(String key,
            String localeDescriptor);

    @Override
    public ResourceLiteralDictionary GetResourceLiterals() {
        return GetResourceLiterals(configuration.getDefaultLocale());
    }

    @Override
    public ResourceLiteralDictionary GetResourceLiterals(Locale locale) {
        return GetResourceLiterals(locale.toString());
    }

    public abstract ResourceLiteralDictionary GetResourceLiterals(
            String localeDescriptor);
}
