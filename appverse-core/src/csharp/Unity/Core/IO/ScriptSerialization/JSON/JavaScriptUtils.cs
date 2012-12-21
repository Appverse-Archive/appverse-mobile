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
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

//using System.Web;
using System.Collections.Generic;

namespace Unity.Core.IO.ScriptSerialization.JSON
{
	internal static class JavaScriptUtils
	{
		public static void WriteEscapedJavaScriptString (string value, TextWriter writer)
		{
			WriteEscapedJavaScriptString (value, '"', true, writer);
		}

		public static void WriteEscapedJavaScriptString (string value, char delimiter, bool appendDelimiters, TextWriter writer)
		{
			// leading delimiter
			if (appendDelimiters)
				writer.Write (delimiter);

			if (!string.IsNullOrEmpty (value))
				for (int i = 0; i < value.Length; i++)
					WriteJavaScriptChar (value [i], delimiter, writer);

			// trailing delimiter
			if (appendDelimiters)
				writer.Write (delimiter);
		}

		public static void WriteEscapedJavaScriptChar (char value, char delimiter, bool appendDelimiters, TextWriter writer)
		{
			// leading delimiter
			if (appendDelimiters)
				writer.Write (delimiter);

			WriteJavaScriptChar (value, delimiter, writer);

			// trailing delimiter
			if (appendDelimiters)
				writer.Write (delimiter);
		}

		public static void WriteJavaScriptChar (char value, char delimiter, TextWriter writer)
		{
			switch (value) {
			case '\t':
				writer.Write (@"\t");
				break;
			case '\n':
				writer.Write (@"\n");
				break;
			case '\r':
				writer.Write (@"\r");
				break;
			case '\f':
				writer.Write (@"\f");
				break;
			case '\b':
				writer.Write (@"\b");
				break;
			case '<':
				writer.Write (@"\u003c");
				break;
			case '>':
				writer.Write (@"\u003e");
				break;
			case '"':
				// only escape if this charater is being used as the delimiter
				if (delimiter == '"')
					writer.Write (@"\""");
				else
					writer.Write (value);
				break;
			case '\'':
				writer.Write (@"\u0027");
				break;
			case '\\':
				writer.Write (@"\\");
				break;
			default:
				if (value > '\u001f')
					writer.Write (value);
				else {
					writer.Write ("\\u00");
					int intVal = (int)value;
					writer.Write ((char)('0' + (intVal >> 4)));
					intVal &= 0xf;
					writer.Write ((char)(intVal < 10 ? '0' + intVal : 'a' + (intVal - 10)));
				}
				break;
			}
		}
	}
}