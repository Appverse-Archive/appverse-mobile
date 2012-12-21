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

namespace Unity.Core.IO.ScriptSerialization.JSON
{
	/// <summary>
	/// Specifies the type of Json token.
	/// </summary>
	enum JsonToken
	{
		/// <summary>
		/// This is returned by the <see cref="JsonReader"/> if a <see cref="JsonReader.Read"/> method has not been called. 
		/// </summary>
		None,
		/// <summary>
		/// An object start token.
		/// </summary>
		StartObject,
		/// <summary>
		/// An array start token.
		/// </summary>
		StartArray,
		/// <summary>
		/// An object property name.
		/// </summary>
		PropertyName,
		/// <summary>
		/// A comment.
		/// </summary>
		Comment,
		/// <summary>
		/// An interger.
		/// </summary>
		Integer,
		/// <summary>
		/// A float.
		/// </summary>
		Float,
		/// <summary>
		/// A string.
		/// </summary>
		String,
		/// <summary>
		/// A boolean.
		/// </summary>
		Boolean,
		/// <summary>
		/// A null token.
		/// </summary>
		Null,
		/// <summary>
		/// An undefined token.
		/// </summary>
		Undefined,
		/// <summary>
		/// An object end token.
		/// </summary>
		EndObject,
		/// <summary>
		/// An array end token.
		/// </summary>
		EndArray,
		/// <summary>
		/// A JavaScript object constructor.
		/// </summary>
		Constructor,
		/// <summary>
		/// A Date.
		/// </summary>
		Date
	}
}
