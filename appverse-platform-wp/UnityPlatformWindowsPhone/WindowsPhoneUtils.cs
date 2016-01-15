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


using Appverse.Core.Scanner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage;
using Windows.UI.Core;
using Windows.Web.Http.Filters;
using UnityPlatformWindowsPhone.Internals;
using ZXing;
using ZXing.Client.Result;
#if DEBUG
using System.Diagnostics;
using Windows.Storage.Streams;
using Windows.ApplicationModel.Email;
#endif

namespace UnityPlatformWindowsPhone
{
    public static class WindowsPhoneUtils
    {
#if DEBUG
        private static StorageFile logFile;
#endif
        //CERTIFICATE VALIDATION VARIABLE HERE
        private const bool _Validate_Certificates = true;
        //BUILD VARIABLES -- Initialized with default values
        public const bool AppverseShowSplashScreen = true;
        public const bool AppverseDisableThumbnails = true;
        //
        private static readonly Uri DummyIndexFileUri = new Uri("ms-appx:///Html/indexWP.html");
        private static readonly Uri RealIndexFileUri = new Uri("ms-appx:///Html/WebResources/www/index.html");
        public static readonly StorageFolder DocumentsFolder;
        internal const string CALLBACKID = "callbackid";


        static WindowsPhoneUtils()
        {
            DocumentsFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync("Documents", CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
#if DEBUG
            Task.Run(async () =>
            {
                logFile =
                    await
                        ApplicationData.Current.TemporaryFolder.CreateFileAsync("Debug_Log.txt",
                            CreationCollisionOption.ReplaceExisting).AsTask();
                await FileIO.AppendLinesAsync(logFile, new[] { "STARTING FILE" });
            });
#endif
        }


        internal static IEnumerable<MethodInvoker> GetMethodInvokersList(object target)
        {
            return
                target.GetType()
                    .GetTypeInfo()
                    .DeclaredMethods.Where((method) => method.IsPublic && !method.IsStatic)
                    .Select(method => MethodInvoker.Create(method, target))
                    .ToList();
        }

        public static IDictionary<string, string> GetQueryParams(String queryText)
        {
            var querySegments = queryText.Split('&');
            var returnDict = new Dictionary<string, string>();
            var lastKey = String.Empty;
            foreach (var segment in querySegments)
            {
                if (segment.Contains('='))
                {
                    var key = segment.Split('=')[0];
                    var value = segment.Replace(key + "=", String.Empty);
                    returnDict.Add(key, value);
                    lastKey = key;
                }
                else
                {
                    returnDict[lastKey] = returnDict[lastKey] + '&' + segment;
                }
            }
            return returnDict;

        }

        public static async Task InvokeCallback(string callbackFunction, string callbackId, string jsonResultString)
        {

            if (AppverseBridge.Instance.RuntimeHandler.Webview.Dispatcher.HasThreadAccess)
            {
                await FireJsCallback(callbackFunction, callbackId, jsonResultString);
            }
            else
            {
                AppverseBridge.Instance.RuntimeHandler.Webview.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                {
                    await InvokeCallback(callbackFunction, callbackId, jsonResultString);
                });
            }

        }

        private static async Task FireJsCallback(string callbackFunction, string callbackId, string jsonResultString)
        {
            try
            {
                var jsCallbackFunction = "try{if(" + callbackFunction + "){" + callbackFunction + "(" + jsonResultString +
                                         ", '" + callbackId +
                                         "');}}catch(e) {console.log('error executing javascript callback " + callbackFunction + ": ' + e)}";
                AppverseBridge.Instance.RuntimeHandler.Webview.InvokeScriptAsync("eval", new[] { "console.log(" + jsonResultString + ");" });
                var result = AppverseBridge.Instance.RuntimeHandler.Webview.InvokeScriptAsync("eval", new[] { jsCallbackFunction });
                result.Completed = (operation, status) =>
                {
                    Log(String.Concat("Callback Completed: ", callbackFunction, "  Status:", operation.Status, " ErrCode: ", operation.ErrorCode, " Results: ", operation.GetResults()));
                    if (operation.Status == AsyncStatus.Completed)
                    {
                        // Nothing.
                    }
                };
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        public static async Task EvaluateJavascript(string sJavascript)
        {
            try
            {
                if (AppverseBridge.Instance.RuntimeHandler.Webview.Dispatcher.HasThreadAccess)
                {
                    await FireJavascript(sJavascript);
                }
                else
                {
                    await AppverseBridge.Instance.RuntimeHandler.Webview.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                    {
                        await EvaluateJavascript(sJavascript);
                    });
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private static async Task FireJavascript(string sJavascript)
        {
            try
            {
                if (AppverseBridge.Instance.RuntimeHandler == null ||
                    AppverseBridge.Instance.RuntimeHandler.Webview == null ||
                    AppverseBridge.Instance.RuntimeHandler.Webview.Source == null) return;
                await AppverseBridge.Instance.RuntimeHandler.Webview.InvokeScriptAsync("eval", new[] { sJavascript });
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        public static async Task<bool> WriteAppverseContextInFile(string sAppverseContext)
        {
            try
            {
                const string sHeadTag = "</style>";
                var sfRealIndexFile = await StorageFile.GetFileFromApplicationUriAsync(RealIndexFileUri);
                var sfOutputDirectory = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(sfRealIndexFile.Path));
                var p1 = await StorageFile.GetFileFromApplicationUriAsync(DummyIndexFileUri);
                var outputFileTask = p1.CopyAsync(sfOutputDirectory, p1.Name, NameCollisionOption.ReplaceExisting);

                string sOriginalContent;
                using (var indexStream = await sfRealIndexFile.OpenStreamForReadAsync())
                using (var reader = new StreamReader(indexStream))
                {
                    sOriginalContent = await reader.ReadToEndAsync();

                }
                sOriginalContent = sOriginalContent.Replace(sHeadTag,
                    String.Concat(sHeadTag, Environment.NewLine, sAppverseContext, Environment.NewLine));
                using (var indexModifiedStream = await (await outputFileTask).OpenStreamForWriteAsync())
                {
                    indexModifiedStream.SetLength(0);
                    await indexModifiedStream.FlushAsync();
                    using (var writer = new StreamWriter(indexModifiedStream))
                    {
                        await writer.WriteAsync(sOriginalContent);
                        await writer.FlushAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log(String.Concat("Error trying to write AppverseContext in file. ", ex.Message));
            }
            return false;
        }

        public static QRType GetQRTypeFromCode(Result readQRCode)
        {
            var parsed = ResultParser.parseResult(readQRCode);
            switch (parsed.Type)
            {
                case ParsedResultType.ADDRESSBOOK:
                    return QRType.ADDRESSBOOK;
                case ParsedResultType.CALENDAR:
                    return QRType.CALENDAR;
                case ParsedResultType.EMAIL_ADDRESS:
                    return QRType.EMAIL_ADDRESS;
                case ParsedResultType.GEO:
                    return QRType.GEO;
                case ParsedResultType.ISBN:
                    return QRType.ISBN;
                case ParsedResultType.PRODUCT:
                    return QRType.PRODUCT;
                case ParsedResultType.SMS:
                    return QRType.SMS;
                case ParsedResultType.TEL:
                    return QRType.TEL;
                case ParsedResultType.URI:
                    return QRType.URI;
                case ParsedResultType.WIFI:
                    return QRType.WIFI;
                default:
                    return QRType.TEXT;
            }
        }

        public static BarCodeType ZxingToBarcode(BarcodeFormat format)
        {
            return Enum.GetValues(typeof(BarCodeType))
                    .Cast<BarCodeType>().First(type => format.ToString().Equals(type.ToString()));
        }

        public static BarcodeFormat BarcodeToZxing(BarCodeType format)
        {
            return Enum.GetValues(typeof(BarcodeFormat))
                    .Cast<BarcodeFormat>().First(code => code.ToString().Equals(format.ToString()));
        }

        public static HttpBaseProtocolFilter CreateHttpClientOptions(bool bAllowAutoRedirect)
        {
            // CacheControl to avoid writing/reading from cache and AutomaticDecompression = true (gzip,deflate)
            var clientBaseConfig = new HttpBaseProtocolFilter()
            {
                AllowAutoRedirect = bAllowAutoRedirect,
                CacheControl =
                {
                    WriteBehavior = HttpCacheWriteBehavior.NoCache,
                    ReadBehavior = HttpCacheReadBehavior.MostRecent
                },
                AutomaticDecompression = true
            };
            if (_Validate_Certificates)
                return clientBaseConfig;

            /* Not Ignorable errors: 
                 * BasicConstraintsError
                 * InvalidCertificateAuthorityPolicy;
                 * InvalidSignature
                 * OtherErrors
                 * Revoked
                 * UnknownCriticalExtension
                */
            clientBaseConfig.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
            clientBaseConfig.IgnorableServerCertificateErrors.Add(ChainValidationResult.IncompleteChain);
            clientBaseConfig.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
            clientBaseConfig.IgnorableServerCertificateErrors.Add(ChainValidationResult.RevocationFailure);
            clientBaseConfig.IgnorableServerCertificateErrors.Add(ChainValidationResult.RevocationInformationMissing);
            clientBaseConfig.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
            clientBaseConfig.IgnorableServerCertificateErrors.Add(ChainValidationResult.WrongUsage);
            return clientBaseConfig;
        }

        public static async Task<IList<DeviceInformation>> EnumerateDevices(DeviceClass deviceClass)
        {
            return (await DeviceInformation.FindAllAsync(deviceClass)).ToList();
        }

        public static async Task Log(string sMessage)
        {
#if DEBUG
            Debug.WriteLine(sMessage);
            await FileIO.AppendLinesAsync(logFile, new[] { DateTime.Now + " " + sMessage });
#endif
        }

#if DEBUG
        public static async Task SendLogFileToDeveloper()
        {
            try
            {
                var mail = new EmailMessage
                {
                    Body = "File Attached",
                    Subject = "Debug File from " + DateTime.UtcNow + " (UTC)",
                    To = { new EmailRecipient("ddbc@gft.com", "David Barranco") },
                    Attachments = { new EmailAttachment(logFile.Name, RandomAccessStreamReference.CreateFromFile(logFile)) }
                };
                if (AppverseBridge.Instance.RuntimeHandler.Webview.Dispatcher.HasThreadAccess) await EmailManager.ShowComposeNewEmailAsync(mail);
                else await AppverseBridge.Instance.RuntimeHandler.Webview.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () => await EmailManager.ShowComposeNewEmailAsync(mail));
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }
#endif

        public static async Task<IStorageFile> GetFileFromReferenceURL(string sReferenceUrl)
        {
            return await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/Documents/" + sReferenceUrl));
        }
    }
}