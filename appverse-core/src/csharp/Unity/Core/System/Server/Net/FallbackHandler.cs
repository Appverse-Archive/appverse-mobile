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

namespace Unity.Core.System.Server.Net
{
	public class FallbackHandler : IHttpHandler
	{
		public bool Process (HttpServer server, HttpRequest req, HttpResponse resp)
		{
			//server.RequestSession(req);
			StringBuilder sb = new StringBuilder ();
			sb.Append ("<h3>Session</h3>");
			sb.Append ("<p>ID: " + req.Session.ID + "<br>User: " + req.Session.User);
			sb.Append ("<h3>Header</h3>");
			sb.Append ("Method: " + req.Method + "; URL: '" + req.Url + "'; HTTP version " + req.HttpVersion + "<p>");
			foreach (DictionaryEntry ide in req.Header)
				sb.Append (" " + ide.Key + ": " + ide.Value + "<br>");
			sb.Append ("<h3>Cookies</h3>");
			foreach (DictionaryEntry ide in req.Cookies)
				sb.Append (" " + ide.Key + ": " + ide.Value + "<br>");
			sb.Append ("<h3>Query</h3>");
			foreach (DictionaryEntry ide in req.Query)
				sb.Append (" " + ide.Key + ": " + ide.Value + "<br>");
			sb.Append ("<h3>Content</h3>");
			sb.Append (req.Content);
			resp.Content = sb.ToString ();
			return true;
		}


	}
}
