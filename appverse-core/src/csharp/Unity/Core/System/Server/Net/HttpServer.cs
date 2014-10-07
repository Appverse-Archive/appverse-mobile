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
using System.Collections;
using System.Net;
using System.Reflection;
using System.IO;

namespace Unity.Core.System.Server.Net
{
	public delegate bool ClientEvent (Server serv,ClientInfo new_client);

	public class HttpServer
	{
		private static HttpServer singletonServer;
		Server s;
		Hashtable hostmap = new Hashtable ();
		ArrayList handlers = new ArrayList ();
		Hashtable sessions = new Hashtable ();
		static Hashtable Responses = new Hashtable ();
		public static String CUSTOM_RESPONSE_HEADERS = "$custom_response_headers_replace_me$";
		int sessionTimeout = 600;

		public static HttpServer SingletonInstance {
			get {
				return HttpServer.singletonServer;
			}
		}

		public Hashtable Hostmap {
			get { return hostmap; }
		}

		public Server Server {
			get { return s; }
		}

		public ArrayList Handlers {
			get { return handlers; }
		}

		public int SessionTimeout {
			get { return sessionTimeout; }
			set {
				sessionTimeout = value;
				//CleanUpSessions ();
			}
		}

		public string GetFilename (HttpRequest req)
		{
			string folder = (string)hostmap [req.Host];
			if (folder == null) {
				folder = GetDefaultBasePath ();
			}
			
			if (req.Page == "/")
				return folder + "/index.html";
			else
				return folder + req.Page;
		}

		/// <summary>
		/// Get default path for the current executing assembly.
		/// </summary>
		/// <returns>Current execution path.</returns>
		public string GetDefaultBasePath ()
		{
			return Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="s"></param>
		public HttpServer (Server s)
		{
			this.s = s;
			s.Connect += new ClientEvent (ClientConnect);
			//handlers.Add (new FallbackHandler ());
			HttpServer.singletonServer = this;
		}

		public bool IsListening {
			get {
				bool listening = false;
				if (s != null) {
					listening = s.IsListening;
				}
				return listening;
			}
		}

		public bool Close ()
		{
			bool result = false;
			try {
				if (s != null) {
					s.Close ();
					//CleanUpSessions (true);
					s = null;
					result = true;
				} else {
					result = false;
				}
			} catch (Exception e) {
#if DEBUG
                SystemLogger.Log(SystemLogger.Module.CORE, "HttpServer: Close() failure: " + e.Source + " " + e.Message);
                result = false;
#endif
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		/*
		public Session RequestSession (HttpRequest req)
		{
			if (req.Session != null) {
				if (sessions[req.Session.ID] == req.Session)
					return req.Session;
			}
			req.Session = new Session (req.From);
			sessions[req.Session.ID] = req.Session;
			return req.Session;
		}
		*/
		/// <summary>
		/// 
		/// </summary>
		/*
		void CleanUpSessions (bool unconditionally)
		{
			ICollection keys = sessions.Keys;
			ArrayList toRemove = new ArrayList ();
			foreach (string k in keys) {
				Session s = (Session)sessions[k];
				int time = (int)((DateTime.Now - s.LastTouched).TotalSeconds);
				if ((time > sessionTimeout) || unconditionally) {
					toRemove.Add (k);
				}
			}
			foreach (object k in toRemove)
				sessions.Remove (k);
		}

		void CleanUpSessions ()
		{
			CleanUpSessions (false);
		}
		*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="ci"></param>
		/// <returns></returns>
		bool ClientConnect (Server s, ClientInfo ci)
		{
			ci.Delimiter = "\r\n\r\n";
			ci.Data = new ClientData (ci);
			ci.OnRead += new ConnectionRead (ClientRead);
			ci.OnReadBytes += new ConnectionReadBytes (ClientReadBytes);
			return true;
		}

		void ClientRead (ClientInfo ci, string text)
		{	
			// Read header, if in right state
			ClientData data = (ClientData)ci.Data;
			if (data.state != ClientState.Header)
				return;
			// already done; must be some text in content, which will be handled elsewhere
			text = text.Substring (data.headerskip);
			
			data.headerskip = 0;
			string[] lines = text.Replace ("\r\n", "\n").Split ('\n');
			data.req.HeaderText = text;
			// First line: METHOD /path/url HTTP/version
			string[] firstline = lines [0].Split (' ');
			if (firstline.Length != 3) {
				SendResponse (ci, data.req, new HttpResponse (400, "Incorrect first header line " + lines [0]), true);
				return;
			}
			if (firstline [2].Substring (0, 4) != "HTTP") {
				SendResponse (ci, data.req, new HttpResponse (400, "Unknown protocol " + firstline [2]), true);
				return;
			}
			data.req.Method = firstline [0];
			data.req.Url = firstline [1];
			data.req.HttpVersion = firstline [2].Substring (5);
			int p;
			for (int i = 1; i < lines.Length; i++) {
				p = lines [i].IndexOf (':');
				if (p > 0)
					data.req.Header [lines [i].Substring (0, p)] = lines [i].Substring (p + 2);
			}
			// If ? in URL, split out query information
			p = firstline [1].IndexOf ('?');
			if (p > 0) {
				data.req.Page = data.req.Url.Substring (0, p);
				data.req.QueryString = data.req.Url.Substring (p + 1);
			} else {
				data.req.Page = data.req.Url;
				data.req.QueryString = "";
			}
			
			if (data.req.Page.IndexOf ("..") >= 0) {
				SendResponse (ci, data.req, new HttpResponse (400, "Invalid path"), true);
				return;
			}
			
			data.req.Host = (string)data.req.Header ["Host"];
			if (null == data.req.Host) {
				SendResponse (ci, data.req, new HttpResponse (400, "No Host specified"), true);
				return;
			}
			
			if (null != data.req.Header ["Cookie"]) {
				string[] cookies = ((string)data.req.Header ["Cookie"]).Split (';');
				foreach (string cookie in cookies) {
					p = cookie.IndexOf ('=');
					if (p > 0) {
						data.req.Cookies [cookie.Substring (0, p).Trim ()] = cookie.Substring (p + 1);
					} else {
						data.req.Cookies [cookie.Trim ()] = "";
					}
				}
			}
			
			if (null == data.req.Header ["Content-Length"])
				data.req.ContentLength = 0;
			else
				data.req.ContentLength = Int32.Parse ((string)data.req.Header ["Content-Length"]);
			
			//if(data.req.ContentLength > 0){
			data.state = ClientState.PreContent;
			data.skip = text.Length + 4;
			//} else DoProcess(ci);
			
			//ClientReadBytes(ci, new byte[0], 0); // For content length 0 body
		}

		void SendResponse (ClientInfo ci, HttpRequest req, HttpResponse resp, bool close)
		{
			StringBuilder builder = new StringBuilder ();
			// Create Headers
			builder.Append ("HTTP/1.0 ");
			builder.Append (resp.ReturnCode);
			builder.Append (Responses [resp.ReturnCode]);
			builder.Append ("\r\nDate: ");
			builder.Append (DateTime.Now.ToString ("R"));
			builder.Append ("\r\nServer: UnityEmbedded/1.0 (iOS)");
			//builder.Append ("\r\nConnection: ");
			//builder.Append ((close ? "close" : "Keep-Alive"));
			
			if (resp.RawContent == null) {
				builder.Append ("\r\nContent-Length: ");
				builder.Append (resp.Content.Length);
			} else {
				builder.Append ("\r\nContent-Length: ");
				builder.Append (resp.RawContent.Length);
			}
			
			builder.Append ("\r\nContent-Encoding: utf-8");
			builder.Append ("\r\nContent-Type: ");

			builder.Append (resp.ContentType);

			// ADDING CUSTOM HEADERS(new feature)
			if (!CUSTOM_RESPONSE_HEADERS.StartsWith ("$"))
				builder.Append (CUSTOM_RESPONSE_HEADERS);
			
			if (req.Session != null) {
				builder.Append ("\r\nSet-Cookie: _sessid=");
				builder.Append (req.Session.ID);
				builder.Append ("; path=/");
			}
			
			foreach (DictionaryEntry de in resp.Header) {
				builder.Append ("\r\n");
				builder.Append (de.Key);
				builder.Append (": ");
				builder.Append (de.Value);
			}
			builder.Append ("\r\n\r\n");
			
			// Send Header
			ci.Send (builder.ToString ());
			
			// Send Body
			if (resp.RawContent != null) {
				ci.Send (resp.RawContent);
			} else {
				ci.Send (resp.Content);
			}
			
			// Close if not persistent connection
			if (close) {
				ci.Close ();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ci"></param>
		/// <param name="bytes"></param>
		/// <param name="len"></param>
		void ClientReadBytes (ClientInfo ci, byte[] bytes, int len)
		{
			//CleanUpSessions ();
			int ofs = 0;
			ClientData data = (ClientData)ci.Data;
			switch (data.state) {
			case ClientState.Content:
				break;
			case ClientState.PreContent:
				data.state = ClientState.Content;
				if ((data.skip - data.read) > len) {
					data.skip -= len;
					return;
				}
				ofs = data.skip - data.read;
				data.skip = 0;
				break;
			default:
				//case ClientState.Header: data.read += len - data.headerskip; return;
				data.read += len;
				return;
			}
			//data.req.Content += Encoding.Default.GetString (bytes, ofs, len - ofs);
			data.req.Content += Encoding.UTF8.GetString (bytes, ofs, len - ofs);
			data.req.BytesRead += len - ofs;
			data.headerskip += len - ofs;
			
			if (data.req.BytesRead >= data.req.ContentLength) {
				if (data.req.Method == "POST") {
					if (data.req.QueryString == "")
						data.req.QueryString = data.req.Content;
					else
						data.req.QueryString += "&" + data.req.Content;
				}
				ParseQuery (data.req);
				DoProcess (ci);
			}
		}

		void DoProcess (ClientInfo ci)
		{
			ClientData data = (ClientData)ci.Data;
			string sessid = (string)data.req.Cookies ["_sessid"];
			if (sessid != null)
				data.req.Session = (Session)sessions [sessid];
			bool closed = Process (ci, data.req);
			data.state = closed ? ClientState.Closed : ClientState.Header;
			data.read = 0;
			HttpRequest oldreq = data.req;
			data.req = new HttpRequest ();
			// Once processed, the connection will be used for a new request
			data.req.Session = oldreq.Session;
			// ... but session is persisted
			data.req.From = ((IPEndPoint)ci.Socket.RemoteEndPoint).Address;
		}

		void ParseQuery (HttpRequest req)
		{
			if (req.QueryString == "")
				return;
			string[] sections = req.QueryString.Split ('&');
			for (int i = 0; i < sections.Length; i++) {
				int p = sections [i].IndexOf ('=');
				if (p < 0)
					req.Query [sections [i]] = "";
				else
					req.Query [sections [i].Substring (0, p)] = sections [i].Substring (p + 1);
			}
		}

		//static long tTotal;

		protected virtual bool Process (ClientInfo ci, HttpRequest req)
		{
			//long tIn = DateTime.Now.Ticks;
			HttpResponse resp = new HttpResponse ();
			resp.Url = req.Url;
			for (int i = handlers.Count - 1; i >= 0; i--) {
				IHttpHandler handler = (IHttpHandler)handlers [i];
				if (handler.Process (this, req, resp)) {
					SendResponse (ci, req, resp, resp.ReturnCode != 200);
					//tTotal += DateTime.Now.Ticks - tIn;
					//Console.WriteLine ("Total: " + tTotal + " ticks");
					return resp.ReturnCode != 200;
				}
			}
			return true;
		}
	}
}
