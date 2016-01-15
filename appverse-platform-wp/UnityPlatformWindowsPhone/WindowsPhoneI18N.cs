/*
 Copyright (c) 2015 GFT Appverse, S.L., Sociedad Unipersonal.

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.Storage;
using Unity.Core.I18N;
using UnityPlatformWindowsPhone.Globalization;
using UnityPlatformWindowsPhone.Internals;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhoneI18N : AbstractI18N, IAppverseService
    {

        private const string AppConfigPath = @"Html\app\config";
        private const string DictTag = "dict";
        private const string I18NConfigFilePath = @"ms-appx:///Html\app\config\i18n-config.xml";
        private const string KeyTag = "key";
        private const string PlistExtension = ".plist";
        private GlobalizationConfig _i18NConfiguration;
        private readonly Dictionary<string, ResourceLiteralDictionary> _languageFilesDictionary = new Dictionary<string, ResourceLiteralDictionary>();
        private static readonly object LockObj = new object();

        public WindowsPhoneI18N()
        {
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
            var pendingTask = Task.Run(async () =>
            {
                try
                {
                    var i18NConfigFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(I18NConfigFilePath));
                    var serializer = new XmlSerializer(typeof(GlobalizationConfig));
                    var reader = XmlReader.Create(await i18NConfigFile.OpenStreamForReadAsync());
                    _i18NConfiguration = (GlobalizationConfig)serializer.Deserialize(reader);
                    await FillLanguagesDictionary();
                }
                catch (Exception ex)
                {
                    WindowsPhoneUtils.Log(ex.Message);
                }
            });
            pendingTask.Wait();
        }

        public override async Task<Locale[]> GetLocaleSupported()
        {
            return (_languageFilesDictionary != null && _languageFilesDictionary.Count > 0)
                ? _languageFilesDictionary.Keys.Select(key => new Locale(key)).ToArray()
                : null;
        }

        public override async Task<string[]> GetLocaleSupportedDescriptors()
        {
            return (_languageFilesDictionary != null && _languageFilesDictionary.Count > 0) ? _languageFilesDictionary.Keys.ToArray() : null;
        }

        public override async Task<string> GetResourceLiteral(string key)
        {
            return await GetResourceLiteral(key, _i18NConfiguration.DefaultLanguage.Language.ToLower());
        }

        public override async Task<string> GetResourceLiteral(string key, Locale locale)
        {
            if (String.IsNullOrWhiteSpace(key)) return String.Empty;
            key = key.ToUpper();
            var sLocaleName = (locale == null) ? _i18NConfiguration.DefaultLanguage.Language.ToLower() : locale.ToString().ToLower();
            sLocaleName = (sLocaleName.StartsWith("-")) ? sLocaleName.Remove(0, 1) : sLocaleName;
            sLocaleName = (sLocaleName.EndsWith("-")) ? sLocaleName.Remove(sLocaleName.Length - 1, 1) : sLocaleName;
            if (_languageFilesDictionary.ContainsKey(sLocaleName) && _languageFilesDictionary[sLocaleName].ContainsKey(key))
            {
                return _languageFilesDictionary[sLocaleName][key];
            }

            var sNewLocalName = (!String.IsNullOrWhiteSpace(locale.Language)) ? locale.Language.ToLower() : String.Empty;
            if (!sNewLocalName.Equals(sLocaleName) && !String.IsNullOrWhiteSpace(sNewLocalName))
            {
                if (_languageFilesDictionary.ContainsKey(sLocaleName) && _languageFilesDictionary[sLocaleName].ContainsKey(key))
                {
                    return _languageFilesDictionary[sLocaleName][key];
                }
            }
            sNewLocalName = (!String.IsNullOrWhiteSpace(locale.Country)) ? locale.Country.ToLower() : String.Empty;
            if (sNewLocalName.Equals(sLocaleName) || String.IsNullOrWhiteSpace(sNewLocalName)) return String.Empty;
            if (_languageFilesDictionary.ContainsKey(sLocaleName) && _languageFilesDictionary[sLocaleName].ContainsKey(key))
            {
                return _languageFilesDictionary[sLocaleName][key];
            }
            return String.Empty;
        }

        public override async Task<string> GetResourceLiteral(string key, string localeDescriptor)
        {
            return await GetResourceLiteral(key, new Locale(localeDescriptor));
        }

        public override async Task<ResourceLiteralDictionary> GetResourceLiterals()
        {
            return await GetResourceLiterals(_i18NConfiguration.DefaultLanguage.Language.ToLower());
        }

        public override async Task<ResourceLiteralDictionary> GetResourceLiterals(Locale locale)
        {
            var sLocaleName = (locale == null) ? _i18NConfiguration.DefaultLanguage.Language.ToLower() : locale.ToString().ToLower();
            sLocaleName = (sLocaleName.StartsWith("-")) ? sLocaleName.Remove(0, 1) : sLocaleName;
            sLocaleName = (sLocaleName.EndsWith("-")) ? sLocaleName.Remove(sLocaleName.Length - 1, 1) : sLocaleName;
            if (_languageFilesDictionary.ContainsKey(sLocaleName))
                return _languageFilesDictionary[sLocaleName];

            var sNewLocalName = (!String.IsNullOrWhiteSpace(locale.Language)) ? locale.Language.ToLower() : String.Empty;
            if (!sNewLocalName.Equals(sLocaleName) && !String.IsNullOrWhiteSpace(sNewLocalName))
            {
                if (_languageFilesDictionary.ContainsKey(sNewLocalName))
                {
                    return _languageFilesDictionary[sNewLocalName];
                }
            }
            sNewLocalName = (!String.IsNullOrWhiteSpace(locale.Country)) ? locale.Country.ToLower() : String.Empty;
            if (sNewLocalName.Equals(sLocaleName) || String.IsNullOrWhiteSpace(sNewLocalName)) return null;
            return _languageFilesDictionary.ContainsKey(sNewLocalName) ? _languageFilesDictionary[sNewLocalName] : null;
        }

        public override async Task<ResourceLiteralDictionary> GetResourceLiterals(string localeDescriptor)
        {
            return await GetResourceLiterals(new Locale(localeDescriptor));
        }

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }

        public string DefaultLocale
        {
            get
            {
                if (_i18NConfiguration.DefaultLanguage == null) return null;
                var sb = new StringBuilder();
                sb.Append(_i18NConfiguration.DefaultLanguage.Language);
                sb.Append(!String.IsNullOrWhiteSpace(_i18NConfiguration.DefaultLanguage.Country)
                    ? "-" + _i18NConfiguration.DefaultLanguage.Country
                    : "");
                return sb.ToString();
            }
        }

        #region PRIVATE_METHODS

        private async Task FillLanguagesDictionary()
        {

            var configFolder = await Package.Current.InstalledLocation.GetFolderAsync(AppConfigPath);
            var files = (await configFolder.GetFilesAsync()).Where(x => x.FileType.Equals(PlistExtension) && _i18NConfiguration.SupportedLanguages.Select(language => language.Language).ToList().Contains(x.DisplayName)).ToList();

            Parallel.ForEach(files, async languageFile =>
            {
                var languageFileContentDictionary = new ResourceLiteralDictionary();
                using (var fileStream = await languageFile.OpenStreamForReadAsync())
                {
                    var xDoc = XDocument.Load(fileStream);
                    if (xDoc.Root != null && xDoc.Root.HasElements)
                    {
                        var dict = xDoc.Root.Element(XName.Get(DictTag));
                        if (dict != null && dict.HasElements)
                        {
                            var elementList = dict.Descendants().ToList();
                            if (elementList.Count % 2 == 0)
                            {
                                for (var i = 0; i < elementList.Count; i += 2)
                                {
                                    var keyElement = elementList[i];
                                    var valueElement = elementList[i + 1];

                                    if (
                                        !keyElement.Name.LocalName.Equals(KeyTag,
                                            StringComparison.CurrentCultureIgnoreCase)) continue;
                                    if (!languageFileContentDictionary.ContainsKey(keyElement.Value.ToUpper()))
                                    {
                                        languageFileContentDictionary.Add(keyElement.Value.ToUpper(), valueElement.Value);
                                    }
                                }
                            }
                        }
                    }
                }
                lock (LockObj)
                {
                    _languageFilesDictionary.Add(languageFile.DisplayName.ToLower(), languageFileContentDictionary);
                }
            });
        }

        #endregion PRIVATE_METHODS
    }
}
