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
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.System.Threading;
using Windows.Web.Http;
using Unity.Core.Net;
using UnityPlatformWindowsPhone.Internals;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhoneNet : AbstractNet, IAppverseService
    {

        public int NetworkStatus { get; private set; }
        private ConnectionProfile _connection;
        private CancellationTokenSource _cancelToken = new CancellationTokenSource();
        private Task _signalTask;


        public WindowsPhoneNet()
        {
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
            NotifyConnectionChanged(false);
            NetworkInformation.NetworkStatusChanged += sender => CheckConnectionChanged();
        }

        private async void CheckConnectionChanged()
        {
            await NotifyConnectionChanged(true);
        }

        public async Task NotifyConnectionChanged(bool bExecuteJavascript)
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            if (connectionProfile == null)
            {
                //No Connection
                ConnectionProfile = null;
                NetworkStatus = (int)NetworkType.Unknown;
            }
            else
            {
                if (connectionProfile.IsWlanConnectionProfile)
                {
                    //Wifi - Do not check signal bars
                    var signalStr = connectionProfile.GetSignalBars();
                    NetworkStatus = (signalStr != null && signalStr > 1) ? (int)NetworkType.Wifi : (int)NetworkType.Unknown;
                }
                else if (connectionProfile.IsWwanConnectionProfile)
                {
                    //3G - Check signal bars
                    var signalStr = connectionProfile.GetSignalBars();
                    NetworkStatus = (signalStr != null && signalStr > 1) ? (int)NetworkType.Carrier_3G : (int)NetworkType.Unknown;
                }
                ConnectionProfile = connectionProfile;
            }
            //if (!bExecuteJavascript) return;
            try
            {
                if (!bExecuteJavascript) return;
                var jsonString = "try{if(Appverse&&Appverse.Net){Appverse.Net.NetworkStatus = " + NetworkStatus + ";}Appverse.Net.onConnectivityChange(Appverse.Net.NetworkStatus);}catch(e){}";
                await WindowsPhoneUtils.EvaluateJavascript(jsonString);
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log("Error ocurred while ConnectionChangeEvent: " + ex.Message);
            }
        }

        public override async Task<NetworkType[]> GetNetworkTypeSupported()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> IsNetworkReachable(string url)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(url) || NetworkStatus == (int)NetworkType.Unknown || !Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)) return false;
                var httpOptions = WindowsPhoneUtils.CreateHttpClientOptions(true);


                using (var client = new HttpClient(httpOptions))
                {
                    var req = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

                    var cts = new CancellationTokenSource(15000);

                    var httpWebResponse = await client.SendRequestAsync(req).AsTask(cts.Token);
                    if (httpWebResponse != null) httpWebResponse.EnsureSuccessStatusCode();
                }
                return true;
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(String.Concat("NetworkReachable Exception reaching: ", url, ".", ex.Message));
                return false;
            }
        }

        public override async Task<NetworkType> GetNetworkTypeReachable(string url)
        {
            throw new NotImplementedException();
        }

        public override async Task<NetworkType[]> GetNetworkTypeReachableList(string url)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> OpenBrowser(string title, string buttonText, string url)
        {
            try
            {
                AppverseBridge.Instance.RuntimeHandler.OpenBrowser(title, url);
                return true;
            }
            catch (Exception)
            {
            }
            return false;

        }

        public override async Task<bool> OpenBrowserWithOptions(SecondaryBrowserOptions browserOptions)
        {
            try
            {
                return await OpenBrowser(browserOptions.Title, String.Empty, browserOptions.Url);

            }
            catch (Exception)
            {
            }
            return false;
        }

        public override async Task<bool> ShowHtml(string title, string buttonText, string html)
        {
            try
            {
                await AppverseBridge.Instance.RuntimeHandler.LoadHtml(html);
                return true;
            }
            catch (Exception)
            {
            }
            return false;

        }

        public override async Task<bool> ShowHtmlWithOptions(SecondaryBrowserOptions browserOptions)
        {
            try
            {
                await ShowHtml(String.Empty, String.Empty, browserOptions.Html);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public override async Task<bool> DownloadFile(string url)
        {
            throw new NotImplementedException();
        }

        public override async Task<NetworkData> GetNetworkData()
        {
            throw new NotImplementedException();
        }

        private ConnectionProfile ConnectionProfile
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = value;
                _cancelToken.Cancel();
                _cancelToken = new CancellationTokenSource();
                _signalTask = Task.Run(() => CheckSignalStrenght(), _cancelToken.Token);
            }
        }

        private async Task CheckSignalStrenght()
        {
            //ConnectionProfile is only != null when in 3G, check signal only when in 3G
            while (ConnectionProfile != null)
            {
                await Task.Delay(3000);
                var signalStr = ConnectionProfile.GetSignalBars();
                var newStatus = (int)NetworkType.Unknown;
                if (ConnectionProfile.IsWlanConnectionProfile)
                {
                    newStatus = (signalStr != null && signalStr > 0) ? (int)NetworkType.Wifi : (int)NetworkType.Unknown;
                }
                else if (ConnectionProfile.IsWwanConnectionProfile)
                {
                    newStatus = (signalStr != null && signalStr > 0) ? (int)NetworkType.Carrier_3G : (int)NetworkType.Unknown;
                }
                if (NetworkStatus == newStatus) continue;
                NetworkStatus = newStatus;
                var jsonString = "try{if(Appverse&&Appverse.Net){Appverse.Net.NetworkStatus = " + NetworkStatus + ";}Appverse.Net.onConnectivityChange(Appverse.Net.NetworkStatus);}catch(e){}";
                await WindowsPhoneUtils.EvaluateJavascript(jsonString);
            }
        }

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }



    }
}
