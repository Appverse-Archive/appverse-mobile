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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Newtonsoft.Json;
using Unity.Core.Media;
using Unity.Core.System.Service;
using UnityPlatformWindowsPhone.Internals;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhoneServiceLocator : AbstractServiceLocator, IAppverseServiceHandler
    {
        private string _appverseContext = String.Empty;
        private const string InternalServerUrl = "https://appverse/service/";

        private readonly ReadOnlyDictionary<string, IAppverseService> _appverseServices;

        private static WindowsPhoneServiceLocator _singletonServiceLocator;

        private WindowsPhoneServiceLocator()
        {
            _appverseServices =
                new ReadOnlyDictionary<string, IAppverseService>(new Dictionary<string, IAppverseService>
                {
                    {"net", new WindowsPhoneNet()},
                    {"system", new WindowsPhoneSystem()},
                    {"pim", new WindowsPhonePim()},
                    {"file", new WindowsPhoneFileSystem()},
                    {"io", new WindowsPhoneIO()},
                    {"geo", new WindowsPhoneGeo()},
                    {"message", new WindowsPhoneMessaging()},
                    {"phone", new WindowsPhoneTelephony()},
                    {"media", new WindowsPhoneMedia()},
                    {"i18n", new WindowsPhoneI18N()},
                    {"log", new WindowsPhoneLog()},
                    {"scanner", new WindowsPhoneScanner()},
                    {"push", new WindowsPhonePushNotification()},
                    //{"notify", new WindowsPhoneNotification},
                    //{"db", new WindowsPhoneDatabase()},
                    //{"analytics", new WindowsPhoneAnalytics()},
                    //{"webtrekk", new WindowsPhoneWebtrekk()},
                    //{"loader", new WindowsPhoneAppLoader()},
                    {"security", new WindowsPhoneSecurity()}

                });
            Task.Run(() => { InitAppverseContext(); });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Singleton IServiceLocator.</returns>
        public static WindowsPhoneServiceLocator GetInstance()
        {
            return _singletonServiceLocator ?? (_singletonServiceLocator = new WindowsPhoneServiceLocator());
        }

        public override object GetService(string name)
        {
            return _appverseServices.ContainsKey(name) ? _appverseServices[name] : null;
        }

        public async Task<MediaMetadata> GetMediaMetadata(StorageFile targetFile)
        {
            return await (_appverseServices["media"] as WindowsPhoneMedia).GetMetadata(targetFile);
        }

        public async Task HandleRequest(string jsonContent)
        {
            try
            {
                var requestObject = JsonConvert.DeserializeAnonymousType(jsonContent, new { uri = "", query = "" });
                if (!String.IsNullOrWhiteSpace(requestObject.query))
                {
                    if (!requestObject.uri.StartsWith(InternalServerUrl)) return;
                    var serviceMethod = requestObject.uri.Replace(InternalServerUrl, String.Empty).Split('/');
                    if (serviceMethod.Length != 2) return;
                    var queryObject = AppverseQueryObject.Create(requestObject.query);
                    if (!_appverseServices.ContainsKey(serviceMethod[0]))
                    {
                        WindowsPhoneUtils.InvokeCallback(queryObject.Callback, queryObject.CallbackID, JsonConvert.SerializeObject(null));
                        return;
                    }
                    var matchingMethodsList =
                        _appverseServices[serviceMethod[0]].MethodList.Where(
                            method =>
                                method.MethodName.Equals(serviceMethod[1]) &&
                                method.ParameterCount == queryObject.ParamsCount).ToList();

                    if (matchingMethodsList.Count == 0)
                    {
                        WindowsPhoneUtils.InvokeCallback(queryObject.Callback, queryObject.CallbackID, JsonConvert.SerializeObject(null));
                        return;
                    }

                    var jsonObjectProperties = (queryObject.QueryJsonObject != null) ? queryObject.QueryJsonObject.Properties().ToList() : null;

                    foreach (var method in matchingMethodsList)
                    {
                        try
                        {
                            dynamic methodTask;
                            if (queryObject.ParamsCount > 0)
                            {
                                var invokeParamsList = new List<object>();
                                for (var i = 0; i < queryObject.ParamsCount; i++)
                                {
                                    if (method.ParameterTypeList[i].GetTypeInfo().IsPrimitive ||
                                        method.ParameterTypeList[i] == typeof(string))
                                    {
                                        var p = Convert.ChangeType(jsonObjectProperties[i].Value,
                                            method.ParameterTypeList[i]);
                                        invokeParamsList.Add(p);

                                    }
                                    else
                                    {
                                        invokeParamsList.Add(
                                            JsonConvert.DeserializeObject(jsonObjectProperties[i].Value.ToString(),
                                                method.ParameterTypeList[i]));
                                    }
                                }
                                methodTask = method.MethodDelegate.DynamicInvoke(invokeParamsList.ToArray());
                            }
                            else
                            {
                                methodTask = method.MethodDelegate.DynamicInvoke();
                            }
                            if (queryObject.Callback == String.Empty || methodTask == null || method.ReturnType == typeof(Task)) break;
                            dynamic methodReturnObject = await methodTask;
                            WindowsPhoneUtils.InvokeCallback(queryObject.Callback, queryObject.CallbackID, JsonConvert.SerializeObject(methodReturnObject));
                            break;
                        }
                        catch (Exception ex)
                        {
                            WindowsPhoneUtils.Log("SERVICE LOCATOR HANDLER ERROR: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log("WP SERVICE LOCATOR SERVICE HANDLER ERROR: " + ex.Message);
            }

        }

        public async Task ApplicationVisiblityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (!e.Visible)
            {
                WindowsPhoneUtils.EvaluateJavascript("Appverse.backgroundApplicationListener();");
                return;
            }
            var netService = _appverseServices["net"] as WindowsPhoneNet;
            netService.NotifyConnectionChanged(true);
            WindowsPhoneUtils.EvaluateJavascript("Appverse.foregroundApplicationListener();");
        }

        private async Task InitAppverseContext()
        {
            try
            {
                var systemService = _appverseServices["system"] as WindowsPhoneSystem;
                var i18NService = _appverseServices["i18n"] as WindowsPhoneI18N;
                var ioService = _appverseServices["io"] as WindowsPhoneIO;
                var netService = _appverseServices["net"] as WindowsPhoneNet;

                var sb = new StringBuilder();
                sb.Append("<script type=\"text/javascript\"> ");
                //1- GetUnityContext
                var unityContext = await systemService.GetUnityContext();

                var jsonString = "_AppverseContext = " + JsonConvert.SerializeObject(unityContext);
                sb.Append(jsonString);

                //2- OSInfo
                var osInfo = await systemService.GetOSInfo();
                jsonString = "_OSInfo = " + JsonConvert.SerializeObject(osInfo);
                sb.Append(";" + jsonString);

                //3- HWinfo
                var hwInfo = await systemService.GetOSHardwareInfo();
                jsonString = "_HwInfo = " + JsonConvert.SerializeObject(hwInfo);
                sb.Append(";" + jsonString);

                //4- Current Device Locale
                var currentLocale = await systemService.GetLocaleCurrent();
                jsonString = "_CurrentDeviceLocale = " + JsonConvert.SerializeObject(currentLocale);
                sb.Append(";" + jsonString);

                //5- I18N
                var _supportedLocale = await i18NService.GetLocaleSupported();
                jsonString = "_i18n = {};  _i18n['default'] = '" + i18NService.DefaultLocale + "'; ";
                var localeLiterals = "";
                foreach (var supportedLocale in _supportedLocale)
                {
                    var literals = await i18NService.GetResourceLiterals(supportedLocale);
                    var literalsJsonString = JsonConvert.SerializeObject(literals);
                    localeLiterals = localeLiterals + " _i18n['" + supportedLocale + "'] = " + literalsJsonString + "; ";

                }
                jsonString = jsonString + localeLiterals;
                sb.Append(";" + jsonString);

                //6- Configured IO services endpoints
                jsonString = "_IOServices = {}; ";
                foreach (var service in await ioService.GetServices())
                {
                    var serviceJson = JsonConvert.SerializeObject(service);
                    jsonString = jsonString + " _IOServices['" + service.Name + "-" +
                                 JsonConvert.SerializeObject(service.Type) + "'] = " + serviceJson + "; ";
                }
                sb.Append(jsonString);

                //7- Network
                jsonString = "_NetworkStatus = " + netService.NetworkStatus + ";";
                sb.Append(jsonString);
                sb.Append("</script>");
                _appverseContext = sb.ToString();
                Debug.WriteLine(_appverseContext);
                await WindowsPhoneUtils.WriteAppverseContextInFile(_appverseContext);
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log("CONTEXT INIT ERROR: " + ex.Message);
            }
            finally
            {
                AppverseBridge.Instance.IndexFileResetEvent.Set();
            }
        }
    }
}