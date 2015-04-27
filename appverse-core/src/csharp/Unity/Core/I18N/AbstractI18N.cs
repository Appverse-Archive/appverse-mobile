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
using System.IO;
#if WP8
using System.Threading.Tasks;
#endif
using System.Xml;
using Unity.Core.I18N;
using Unity.Core.System;

namespace Unity.Core.I18N
{
    public abstract class AbstractI18N : II18N
    {
        private string i18nConfigFile = "app/config/i18n-config.xml";
        private const string SUPPORTED_LANGUAGE_TAG = "supportedLanguage";
        private const string ID_TAG = "id";
        private const string LANGUAGE_ATTRIBUTE_NAME = "language";
        private const string COUNTRY_ATTRIBUTE_NAME = "country";
        public const string PLIST_PATTERN = "*.plist";
        private const string APP_CONFIG_DIR = "app/config";
        private const string DEFAULT_TAG = "default";
        private const string PLIST_EXTENSION = ".plist";
        protected string[] localeSupported = null;
        private string defaultLanguageCountry = null;
#if !WP8
        public virtual string I18NConfigFile
        {
            get { return this.i18nConfigFile; }
            set { this.i18nConfigFile = value; }
        }

        public string DefaultLocale
        {
            get { return this.defaultLanguageCountry; }
        }

        public AbstractI18N()
        {
            getLocaleConfig();
        }
        protected virtual bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        protected virtual string[] GetDirectoryFiles(string directoryPath, string filePattern)
        {
            if (Directory.Exists(directoryPath))
            {
                return Directory.GetFiles(directoryPath, filePattern);
            }
            return new string[0];
        }

        private string[] getLocaleSuportedPlist()
        {
            List<string> idsLocale = new List<string>();
            string pathI18NConfigFileDir = Path.GetDirectoryName(I18NConfigFile);

            foreach (string item in this.GetDirectoryFiles(pathI18NConfigFileDir, PLIST_PATTERN))
            {
                string localeId = Path.GetFileNameWithoutExtension(item);
                idsLocale.Add(localeId);
            }

            return idsLocale.ToArray();
        }

        protected virtual XmlTextReader getXmlTextReader(string textFilePath)
        {
            return new XmlTextReader(textFilePath);
        }

        private string[] getLocaleSupportedXml()
        {
            List<string> idsLocale = new List<string>();
            if (this.FileExists(I18NConfigFile))
            {
                try
                {
                    XmlTextReader reader = this.getXmlTextReader(I18NConfigFile);
                    try
                    {
                        while (reader.Read())
                        {
                            XmlNodeType nodeType = reader.NodeType;
                            if ((nodeType == XmlNodeType.Element) && (reader.Name == SUPPORTED_LANGUAGE_TAG))
                            {
                                string languageAndCountryId = readSingleAttribute(reader, ID_TAG);
                                // we need the real language and country selected by deafult, if provided
                                string defaultLanguage = readSingleAttribute(reader, LANGUAGE_ATTRIBUTE_NAME);
                                string defaultCountry = readSingleAttribute(reader, COUNTRY_ATTRIBUTE_NAME);

                                reader.Read(); // continue with next node

                                if (defaultLanguage != null)
                                {
                                    languageAndCountryId = (new Locale(defaultLanguage)).ToString();
                                    if (defaultCountry != null)
                                    {
                                        languageAndCountryId = (new Locale(defaultLanguage + "-" + defaultCountry)).ToString();
                                    }
                                }
                                idsLocale.Add(languageAndCountryId);
                            }
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (UriFormatException e)
                {
#if DEBUG
                    SystemLogger.Log(SystemLogger.Module.CORE, "# loadSettings. Getting Content From XmlReader on file path: " + I18NConfigFile + ":" + e.Message);
#endif
                }
            }
            return idsLocale.ToArray();
        }

        public string readSingleAttribute(XmlTextReader reader, string attributeId)
        {
            return reader.GetAttribute(attributeId);
        }

        public string readAttribute(XmlTextReader reader, string attributeId)
        {
            string language = reader.GetAttribute(attributeId);
            reader.Read();
            return language;
        }

        private string[] evaluateLocaleSupported(string[] localeSupportedXml, string[] localeSupportedPlist)
        {
            List<string> listLocaleSupported = new List<string>();
            foreach (string itemInXml in localeSupportedXml)
            {
                foreach (string itemInPlist in localeSupportedPlist)
                {
                    if (itemInPlist == itemInXml)
                    {
                        listLocaleSupported.Add(itemInXml);
                    }
                }
            }
            return listLocaleSupported.ToArray();
        }

        private string evalDefaultLanguageCountry()
        {
            bool isFound = false;
            string languageAndCountryId = string.Empty;
            if (this.FileExists(I18NConfigFile))
            {
                try
                {
                    XmlTextReader reader = this.getXmlTextReader(I18NConfigFile);
                    try
                    {
                        while ((reader.Read() && !isFound))
                        {
                            XmlNodeType nodeType = reader.NodeType;
                            if ((nodeType == XmlNodeType.Element) && (reader.Name == DEFAULT_TAG))
                            {
                                languageAndCountryId = readSingleAttribute(reader, ID_TAG);
                                // we need the real language and country selected by deafult, if provided
                                string defaultLanguage = readSingleAttribute(reader, LANGUAGE_ATTRIBUTE_NAME);
                                string defaultCountry = readSingleAttribute(reader, COUNTRY_ATTRIBUTE_NAME);

                                reader.Read(); // continue with next node

                                if (defaultLanguage != null)
                                {
                                    languageAndCountryId = (new Locale(defaultLanguage)).ToString();
                                    if (defaultCountry != null)
                                    {
                                        languageAndCountryId = (new Locale(defaultLanguage + "-" + defaultCountry)).ToString();
                                    }
                                }
                                isFound = true;
                            }
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                catch (UriFormatException e)
                {
#if DEBUG
                    SystemLogger.Log(SystemLogger.Module.CORE, "# loadSettings. Getting Content From XmlReader on file path: " + I18NConfigFile + ":" + e.Message);
#endif
                }
            }
            return languageAndCountryId;
        }

        protected virtual string readFromPlistFile(string file, string key)
        {
            string result = string.Empty;
            if (this.FileExists(file))
            {
                XmlTextReader reader = this.getXmlTextReader(file);
                try
                {
                    reader.XmlResolver = null;
                    XmlDocument XmlDoc = new XmlDocument();
                    XmlDoc.Load(reader);
                    XmlNodeList keys = XmlDoc.SelectNodes("plist/dict/key");
                    XmlNodeList values = XmlDoc.SelectSingleNode("plist/dict").ChildNodes;
                    for (int value = 0; value <= keys.Count - 1; value++)
                    {
                        if (keys[value].InnerText == key)
                        {
                            result = values[value * 2 + 1].InnerText;
                        }
                    }
                    reader.Close();
                }
                catch (Exception)
                {
                    result = string.Empty;
                    reader.Close();
                }
                result = (result == string.Empty) ? String.Format("&lt;{0}&gt;", key) : result;
            }
            else
            {
                // if file does not exists, means that requested locale is not supported by application
                // try then to get default locale string
                return GetResourceLiteral(key);
            }

            return result;
        }

        protected virtual ResourceLiteralDictionary readAllFromPlistFile(string file)
        {
            ResourceLiteralDictionary result = null;
            if (this.FileExists(file))
            {
                result = new ResourceLiteralDictionary();
                try
                {
                    using (XmlTextReader reader = this.getXmlTextReader(file))
                    {
                        reader.XmlResolver = null;
                        XmlDocument XmlDoc = new XmlDocument();
                        XmlDoc.Load(reader);
                        XmlNodeList keys = XmlDoc.SelectNodes("plist/dict/key");
                        XmlNodeList values = XmlDoc.SelectSingleNode("plist/dict").ChildNodes;
                        for (int value = 0; value <= keys.Count - 1; value++)
                        {
                            if (result.ContainsKey(keys[value].InnerText))
                            {
                                result[keys[value].InnerText] = values[value * 2 + 1].InnerText;
                            }
                            else
                            {
                                result.Add(keys[value].InnerText, values[value * 2 + 1].InnerText);
                            }

                        }
                    }
                }
                catch (Exception)
                {
                    result = null;
                }
            }
            else
            {
                // if file does not exists, means that requested locale is not supported by application
                // try then to get default locale string
                return GetResourceLiterals();
            }
            return result;
        }

        public void getLocaleConfig()
        {
            try
            {
                string[] localeSupportedXml = getLocaleSupportedXml();
                string[] localeSupportedPlist = getLocaleSuportedPlist();
                this.localeSupported = evaluateLocaleSupported(localeSupportedXml, localeSupportedPlist);
                this.defaultLanguageCountry = evalDefaultLanguageCountry();

                SystemLogger.Log(SystemLogger.Module.CORE,
                                 "# supported locales (config): " + localeSupportedXml.Length +
                                 ", supported plist files: " + localeSupportedPlist.Length +
                                 ", localeSupported: " + this.localeSupported.Length +
                                 ", defaultLanguageCountry: " + this.defaultLanguageCountry);
            }
            catch (Exception ex)
            {
                SystemLogger.Log(SystemLogger.Module.CORE, "Exception when loading i18n configuration: " + ex.Message, ex);
            }
        }
#else
#endif


        #region Miembros de II18N
#if !WP8
        /// <summary>
        /// List of supported locales for the application.
        /// </summary>
        /// <returns>List of locales.
        /// A <see cref="Locale[]"/>
        /// </returns>
        public Locale[] GetLocaleSupported()
        {
            List<Locale> supportedLocales = new List<Locale>();
            if (this.localeSupported != null)
            {
                foreach (string item in this.localeSupported)
                {
                    Locale loc = new Locale(item);
                    supportedLocales.Add(loc);
                }
            }

            return supportedLocales.ToArray();
        }

        /// <summary>
        /// List of supported locales for the application.
        /// </summary>
        /// <returns>List of locale descriptors.
        /// A <see cref="System.String[]"/>
        /// </returns>
        public string[] GetLocaleSupportedDescriptors()
        {
            List<string> supportedLocaleDescriptors = new List<string>();
            Locale[] supportedLocales = GetLocaleSupported();
            if (supportedLocales != null)
            {
                foreach (Locale locale in supportedLocales)
                {
                    supportedLocaleDescriptors.Add(locale.ToString());
                }
            }

            return supportedLocaleDescriptors.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public string GetResourceLiteral(string key)
        {
            return GetResourceLiteral(key, this.defaultLanguageCountry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="locale">
        /// A <see cref="Locale"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public string GetResourceLiteral(string key, Locale locale)
        {
            if (locale == null)
            {
                return GetResourceLiteral(key);
            }
            else
            {
                return GetResourceLiteral(key, locale.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="localeDescriptor">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public string GetResourceLiteral(string key, string localeDescriptor)
        {
            string pathI18NConfigFile = Path.GetDirectoryName(I18NConfigFile);
            string plistFullPathFileName = pathI18NConfigFile + "/" + localeDescriptor + PLIST_EXTENSION;
            // Get file specified by full localeDescriptor
            if (!this.FileExists(plistFullPathFileName))
            {
                // Get file specified by language only if Country is not specificaly defined.
                Locale locale = new Locale(localeDescriptor);
                string language = locale.Language;
                plistFullPathFileName = pathI18NConfigFile + "/" + language + PLIST_EXTENSION;
            }

            return readFromPlistFile(plistFullPathFileName, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// A <see cref="Unity.Core.I18N.ResourceLiteralDictionary"/>
        /// </returns>
        public ResourceLiteralDictionary GetResourceLiterals()
        {
            return GetResourceLiterals(this.defaultLanguageCountry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locale">
        /// A <see cref="Locale"/>
        /// </param>
        /// <returns>
        /// A <see cref="Unity.Core.I18N.ResourceLiteralDictionary"/>
        /// </returns>
        public ResourceLiteralDictionary GetResourceLiterals(Locale locale)
        {
            if (locale == null)
            {
                return GetResourceLiterals();
            }
            else
            {
                return GetResourceLiterals(locale.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localeDescriptor">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="Unity.Core.I18N.ResourceLiteralDictionary"/>
        /// </returns>
        public ResourceLiteralDictionary GetResourceLiterals(string localeDescriptor)
        {
            string pathI18NConfigFile = Path.GetDirectoryName(I18NConfigFile);
            string plistFullPathFileName = pathI18NConfigFile + "/" + localeDescriptor + PLIST_EXTENSION;
            if (!this.FileExists(plistFullPathFileName))
            {
                // Get file specified by language only if Country is not specificaly defined.
                Locale locale = new Locale(localeDescriptor);
                string language = locale.Language;
                plistFullPathFileName = pathI18NConfigFile + "/" + language + PLIST_EXTENSION;
            }
            return readAllFromPlistFile(plistFullPathFileName);
        }
#else
        public abstract Task<Locale[]> GetLocaleSupported();
        public abstract Task<string[]> GetLocaleSupportedDescriptors();
        public abstract Task<string> GetResourceLiteral(string key);
        public abstract Task<string> GetResourceLiteral(string key, Locale locale);
        public abstract Task<string> GetResourceLiteral(string key, string localeDescriptor);
        public abstract Task<ResourceLiteralDictionary> GetResourceLiterals();
        public abstract Task<ResourceLiteralDictionary> GetResourceLiterals(Locale locale);
        public abstract Task<ResourceLiteralDictionary> GetResourceLiterals(string localeDescriptor);
#endif

        #endregion


    }
}
