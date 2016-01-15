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
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Unity.Core.Net;

namespace Unity.Platform.Windows
{
    public class WindowsNet : AbstractNet
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="ni"></param>
        /// <returns></returns>
        private List<NetworkType> AddToNetworkTypeList(List<NetworkType> list, NetworkInterface ni) 
        {
            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
            {
                // wireless 
                if (!list.Contains(NetworkType.Wifi)) // Add 'wifi' type if it is not already added. 
                {
                    list.Add(NetworkType.Wifi);
                }
            }
            else if (ni.NetworkInterfaceType != NetworkInterfaceType.Unknown
                        && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                // cable
                if (!list.Contains(NetworkType.Cable)) // Add 'cable' type if it is not already added. 
                {
                    list.Add(NetworkType.Cable);
                }
            }

            return list;
        }


        /// <summary>
        /// List of supported network types (cable, wifi, etc.) on the current device.
        /// </summary>
        /// <returns>Network types.</returns>
        public override NetworkType[] GetNetworkTypeSupported()
        {

            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            Console.WriteLine("Interface information for {0}.{1}     ",
            computerProperties.HostName, computerProperties.DomainName);

            List<NetworkType> supportedTypesList = new List<NetworkType>();

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics == null || nics.Length < 1)
            {
#if DEBUG
                Console.WriteLine("No network interfaces found.");
#endif
                return null;
            }
#if DEBUG
            Console.WriteLine("Number of interfaces .................... : {0}", nics.Length);
#endif
            foreach (NetworkInterface adapter in nics)
            {
#if DEBUG
                Console.WriteLine();
                Console.WriteLine(adapter.Description);
                Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '='));
                Console.WriteLine("  ID....................................... : {0}", adapter.Id);
                Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
                Console.WriteLine("  Physical Address ........................ : {0}",
                           adapter.GetPhysicalAddress().ToString());
                Console.WriteLine("  Operational status ...................... : {0}",
                    adapter.OperationalStatus);
                Console.WriteLine("  Speed ................................... : {0}", adapter.Speed / 1000000 + "Mbps");
                string versions = "";

                // Create a display string for the supported IP versions.
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    versions = "IPv4";
                }
                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    if (versions.Length > 0)
                    {
                        versions += " ";
                    }
                    versions += "IPv6";
                }
                Console.WriteLine("  IP version .............................. : {0}", versions);

                Console.WriteLine("  Receive Only ............................ : {0}",
                    adapter.IsReceiveOnly);
                Console.WriteLine("  Multicast ............................... : {0}",
                    adapter.SupportsMulticast);

                Console.WriteLine();
#endif
                // Add to list of supported network types.
                supportedTypesList = AddToNetworkTypeList(supportedTypesList, adapter);
            }

            /*
             * ANOTHER APPROACH , using Win 32 classes
             * 
            string query = "SELECT * FROM Win32_NetworkAdapter";
            ManagementObjectSearcher moSearch = new ManagementObjectSearcher(query);
            ManagementObjectCollection moCollection = moSearch.Get();// Every record in this collection is a network interface
            foreach (ManagementObject mo in moCollection)
            {
                Console.WriteLine("# " + mo["Name"] + ", type:" + mo["AdapterType"] + ", Availability: " + mo["Availability"] + ", Index:" + mo["Index"]);
            }
            */

            return supportedTypesList.ToArray();
        }

        /// <summary>
        /// List of network types available to reach the given url.
        /// </summary>
        /// <param name="url">The url to check reachability.</param>
        /// <returns>List of network types, ordered by preference.</returns>
        public override NetworkType[] GetNetworkTypeReachableList(string url)
        {
            NetworkType[] reachableNetworks = new List<NetworkType>().ToArray();

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // A network connection is considered to be available if any network interface 
                // is marked "up" and is not a loopback or tunnel interface.

                // if a network is available, ping to check reachability.
                try
                {
                    //Ping could be a good solution in those networks where it was allowed (however proxy/firewall could deny "ping"'s)
                    //Ping ping = new Ping();
                    string domainName = extractDomainNameFromURL(url);
                    //IWebProxy systemProxy = WebRequest.DefaultWebProxy; //It is not used at present but useful to know it;
                    try
                    {
                        IPHostEntry hostInfo = Dns.GetHostEntry(domainName);
                        IPAddress[] address = hostInfo.AddressList;
                        String[] alias = hostInfo.Aliases;
                        //PingReply reply = ping.Send(url, DEFAULT_TIMEOUT);
                        //if (reply.Status == IPStatus.Success)
                        //{
#if DEBUG
                        //Console.WriteLine("Address: {0}", reply.Address.ToString());
                        //Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                        //Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                        //Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                        //Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                        Console.WriteLine("GetNetworkTypeReachableList : Hostname: " + hostInfo.HostName);
                        foreach (IPAddress item in address)
                        {
                            Console.WriteLine("GetNetworkTypeReachableList : Address:" + item);
                        }
                        foreach (String item in alias)
                        {
                            Console.WriteLine("GetNetworkTypeReachableList : Alias:" + item);
                        }
#endif

                        // ******* ASSUMPTION: the given "GetAllNetworkInterfaces" list order is assumed to be a "preference" order.
                        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                        if (nics != null || nics.Length < 1)
                        {
                            List<NetworkType> list = new List<NetworkType>();
                            foreach (NetworkInterface adapter in nics)
                            {
                                if (adapter.OperationalStatus == OperationalStatus.Up)
                                {
                                    // ONLY "UP" status interfaces are added to the list.
                                    // Add to list of supported network types.
                                    list = AddToNetworkTypeList(list, adapter);
                                }
                            }
                            reachableNetworks = list.ToArray();
                        }
                        else
                        {
#if DEBUG
                            Console.WriteLine("No network interfaces found.");
#endif
                        }
                        //}
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Console.WriteLine("Could not resolve: " + domainName + ":" + e.Message);
#endif
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                Console.WriteLine("Exception when doing ping against url [" + url + "]: " + e.Message);
#endif
                }
            }
            return reachableNetworks;
        }

        public override bool OpenBrowser(string title, string buttonText, string url)
        {
            throw new NotImplementedException();
        }

        public override bool ShowHtml(string title, string buttonText, string html)
        {
            throw new NotImplementedException();
        }


        public override bool OpenBrowserWithOptions(SecondaryBrowserOptions browserOptions)
        {
            throw new NotImplementedException();
        }

        public override bool ShowHtmlWithOptions(SecondaryBrowserOptions browserOptions)
        {
            throw new NotImplementedException();
        }

        public string extractDomainNameFromURL(string url)
        {
            if (!url.Contains("://"))
            {
                url = "http://" + url;
            }

            return new Uri(url).Host;
        }

        public override bool DownloadFile(string url)
        {
            try
            {
                // open url in default web browser
                System.Diagnostics.Process.Start(Uri.EscapeUriString(url));

                return true;
            }
            catch (Exception ex)
            {
                
#if DEBUG
                Console.WriteLine("Exception on WindowsNet#DownloadFile method: " + ex.Message);
#endif
            }
            return false;
        }

        public override NetworkData GetNetworkData()
        {
            throw new NotImplementedException();
        }
    }
}
