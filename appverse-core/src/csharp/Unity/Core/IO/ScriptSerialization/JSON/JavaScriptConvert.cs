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
 */using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Reflection;
using System.Data.SqlTypes;
using System.Xml;

namespace Unity.Core.IO.ScriptSerialization.JSON
{
	/// <summary>
	/// Provides methods for converting between common language runtime types and JavaScript types.
	/// </summary>
	static class JavaScriptConvert
	{
		/// <summary>
		/// Represents JavaScript's boolean value true as a string. This field is read-only.
		/// </summary>
		public static readonly string True;

		/// <summary>
		/// Represents JavaScript's boolean value false as a string. This field is read-only.
		/// </summary>
		public static readonly string False;

		/// <summary>
		/// Represents JavaScript's null as a string. This field is read-only.
		/// </summary>
		public static readonly string Null;

		/// <summary>
		/// Represents JavaScript's undefined as a string. This field is read-only.
		/// </summary>
		public static readonly string Undefined;
		internal static readonly long InitialJavaScriptDateTicks;
		internal static readonly DateTime MinimumJavaScriptDate;

		static JavaScriptConvert ()
		{
			True = "true";
			False = "false";
			Null = "null";
			Undefined = "undefined";

			InitialJavaScriptDateTicks = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
			MinimumJavaScriptDate = new DateTime (100, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		}

		/// <summary>
		/// Converts the <see cref="DateTime"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="DateTime"/>.</returns>
		public static string ToString (DateTime value)
		{
			long javaScriptTicks = ConvertDateTimeToJavaScriptTicks (value);

			return @"""\/Date(" + javaScriptTicks + @")\/""";
		}

		internal static long ConvertDateTimeToJavaScriptTicks (DateTime dateTime)
		{
			dateTime = dateTime.ToUniversalTime ();

			if (dateTime < MinimumJavaScriptDate)
				dateTime = MinimumJavaScriptDate;

			long javaScriptTicks = (dateTime.Ticks - InitialJavaScriptDateTicks) / (long)10000;

			return javaScriptTicks;
		}

		internal static DateTime ConvertJavaScriptTicksToDateTime (long javaScriptTicks)
		{
			return new DateTime ((javaScriptTicks * 10000) + InitialJavaScriptDateTicks, DateTimeKind.Utc);
		}

		/// <summary>
		/// Converts the <see cref="Boolean"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="Boolean"/>.</returns>
		public static string ToString (bool value)
		{
			return (value) ? True : False;
		}

		/// <summary>
		/// Converts the <see cref="Char"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="Char"/>.</returns>
		public static void WriteChar (char value, TextWriter writer)
		{
			if (value == '\0')
				writer.Write (Null);
			else
				JavaScriptUtils.WriteEscapedJavaScriptChar (value, '"', true, writer);
		}

		/// <summary>
		/// Converts the <see cref="Enum"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="Enum"/>.</returns>
		public static string ToString (Enum value)
		{
			return value.ToString ();
		}

		/// <summary>
		/// Converts the <see cref="Int32"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="Int32"/>.</returns>
		public static string ToString (int value)
		{
			return value.ToString (CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the <see cref="Int16"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="Int16"/>.</returns>
		public static string ToString (short value)
		{
			return value.ToString (CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the <see cref="UInt16"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="UInt16"/>.</returns>
		public static string ToString (ushort value)
		{
			return value.ToString (CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the <see cref="UInt32"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="UInt32"/>.</returns>
		public static string ToString (uint value)
		{
			return value.ToString (CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the <see cref="Int64"/>  to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="Int64"/>.</returns>
		public static string ToString (long value)
		{
			return value.ToString (CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the <see cref="UInt64"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="UInt64"/>.</returns>
		public static string ToString (ulong value)
		{
			return value.ToString (CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the <see cref="Single"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="Single"/>.</returns>
		public static string ToString (float value)
		{
			return value.ToString ("R", CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the <see cref="Double"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="Double"/>.</returns>
		public static string ToString (double value)
		{
			return value.ToString ("R", CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the <see cref="Byte"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="Byte"/>.</returns>
		public static string ToString (byte value)
		{
			return value.ToString (CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the <see cref="SByte"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="SByte"/>.</returns>
		public static string ToString (sbyte value)
		{
			return value.ToString (CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the <see cref="Decimal"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="SByte"/>.</returns>
		public static string ToString (decimal value)
		{
			return value.ToString (CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the <see cref="Guid"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="Guid"/>.</returns>
		//public static string ToString(Guid value)
		//{
		//    return '"' + value.ToString("D", CultureInfo.InvariantCulture) + '"';
		//}

		/// <summary>
		/// Converts the <see cref="String"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns>A Json string representation of the <see cref="String"/>.</returns>
		public static void WriteString (string value, TextWriter writer)
		{
			WriteString (value, '"', writer);
		}

		/// <summary>
		/// Converts the <see cref="String"/> to it's JavaScript string representation.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="delimter">The string delimiter character.</param>
		/// <returns>A Json string representation of the <see cref="String"/>.</returns>
		public static void WriteString (string value, char delimter, TextWriter writer)
		{
			JavaScriptUtils.WriteEscapedJavaScriptString (value, delimter, true, writer);
		}
	}
}