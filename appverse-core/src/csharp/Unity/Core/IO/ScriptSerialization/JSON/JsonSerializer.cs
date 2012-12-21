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
using System.IO;
using System.Collections;
using System.Reflection;
using System.ComponentModel;

namespace Unity.Core.IO.ScriptSerialization.JSON
{
	/// <summary>
	/// Specifies reference loop handling options for the <see cref="JsonWriter"/>.
	/// </summary>
	enum ReferenceLoopHandling
	{
		/// <summary>
		/// Throw a <see cref="JsonSerializationException"/> when a loop is encountered.
		/// </summary>
		Error = 0,
		/// <summary>
		/// Ignore loop references and do not serialize.
		/// </summary>
		Ignore = 1,
		/// <summary>
		/// Serialize loop references.
		/// </summary>
		Serialize = 2
	}

	/// <summary>
	/// Serializes and deserializes objects into and from the Json format.
	/// The <see cref="JsonSerializer"/> enables you to control how objects are encoded into Json.
	/// </summary>
	sealed class JsonSerializer
	{
		sealed internal class DeserializerLazyDictionary : JavaScriptSerializer.LazyDictionary
		{
			readonly JsonReader _reader;
			readonly JsonSerializer _serializer;
			IEnumerator<KeyValuePair<string, object>> _innerEnum;
			object _firstValue;

			public DeserializerLazyDictionary (JsonReader reader, JsonSerializer serializer)
			{
				_reader = reader;
				_serializer = serializer;
			}

			public object PeekFirst ()
			{
				if (_innerEnum != null)
					throw new InvalidOperationException ("first already taken");

				_innerEnum = _serializer.PopulateObject (_reader);

				if (_innerEnum.MoveNext ())
					_firstValue = _innerEnum.Current;

				return _firstValue;
			}

			protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator ()
			{
				if (_innerEnum == null)
					_innerEnum = _serializer.PopulateObject (_reader);

				if (_firstValue != null)
					yield return (KeyValuePair<string, object>)_firstValue;

				while (_innerEnum.MoveNext ())
					yield return _innerEnum.Current;
			}
		}

		sealed class SerializerLazyDictionary : JavaScriptSerializer.LazyDictionary
		{
			readonly object _source;

			public SerializerLazyDictionary (object source)
			{
				_source = source;
			}

			protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator ()
			{
				foreach (MemberInfo member in ReflectionUtils.GetFieldsAndProperties (_source.GetType (), BindingFlags.Public | BindingFlags.Instance)) {
					if (ReflectionUtils.CanReadMemberValue (member) && !member.IsDefined (typeof(ScriptIgnoreAttribute), true))
					if (!ReflectionUtils.IsIndexedProperty (member))
						yield return new KeyValuePair<string, object> (member.Name, ReflectionUtils.GetMemberValue (member, _source));
				}
			}
		}

		sealed class GenericDictionaryLazyDictionary : JavaScriptSerializer.LazyDictionary
		{
			readonly object _source;
			readonly PropertyInfo _piKeys;
			readonly PropertyInfo _piValues;

			public GenericDictionaryLazyDictionary (object source, Type dictType)
			{
				_source = source;
				_piKeys = dictType.GetProperty ("Keys");
				_piValues = dictType.GetProperty ("Values");
			}

			protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator ()
			{
				
				IEnumerable eKeys = (IEnumerable)_piKeys.GetValue (_source, null);
				IEnumerator eValues = ((IEnumerable)_piValues.GetValue (_source, null)).GetEnumerator ();
				foreach (object key in eKeys) {
					string keyString = key == null ? null : key.ToString ();
					if (!eValues.MoveNext ())
						throw new IndexOutOfRangeException (keyString);


					yield return new KeyValuePair<string, object> (keyString, eValues.Current);
				}

				if (eValues.MoveNext ())
					throw new IndexOutOfRangeException (eValues.Current != null ? eValues.Current.ToString () : String.Empty);
			}
		}

		private int _maxJsonLength;
		private int _recursionLimit;
		private int _currentRecursionCounter;
		private ReferenceLoopHandling _referenceLoopHandling;
		readonly JavaScriptSerializer _context;
		readonly JavaScriptTypeResolver _typeResolver;

		public int MaxJsonLength {
			get { return _maxJsonLength; }
			set { _maxJsonLength = value; }
		}

		public int RecursionLimit {
			get { return _recursionLimit; }
			set { _recursionLimit = value; }
		}

		/// <summary>
		/// Get or set how reference loops (e.g. a class referencing itself) is handled.
		/// </summary>
		public ReferenceLoopHandling ReferenceLoopHandling {
			get { return _referenceLoopHandling; }
			set {
				if (value < ReferenceLoopHandling.Error || value > ReferenceLoopHandling.Serialize) {
					throw new ArgumentOutOfRangeException ("value");
				}
				_referenceLoopHandling = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonSerializer"/> class.
		/// </summary>
		public JsonSerializer (JavaScriptSerializer context, JavaScriptTypeResolver resolver)
		{
			_context = context;
			_typeResolver = resolver;
			_referenceLoopHandling = ReferenceLoopHandling.Error;
		}

		#region Deserialize
		public object Deserialize (TextReader reader)
		{
			return Deserialize (new JsonReader (reader, MaxJsonLength, RecursionLimit));
		}

		/// <summary>
		/// Deserializes the Json structure contained by the specified <see cref="JsonReader"/>
		/// into an instance of the specified type.
		/// </summary>
		/// <param name="reader">The type of object to create.</param>
		/// <param name="objectType">The <see cref="Type"/> of object being deserialized.</param>
		/// <returns>The instance of <paramref name="objectType"/> being deserialized.</returns>
		object Deserialize (JsonReader reader)
		{
			if (!reader.Read ())
				return null;

			return GetObject (reader);
		}

		private object GetObject (JsonReader reader/*, Type objectType*/)
		{
			if (RecursionLimit > 0 && reader.CurrentRecursionLevel >= RecursionLimit) {
				throw new ArgumentException ("RecursionLimit exceeded.");
			}

			object value;

			switch (reader.TokenType) {
			// populate a typed object or generic dictionary/array
			// depending upon whether an objectType was supplied
			case JsonToken.StartObject:
				//value = PopulateObject(reader/*, objectType*/);
				value = new DeserializerLazyDictionary (reader, this);
				break;
			case JsonToken.StartArray:
				value = PopulateList (reader/*, objectType*/);
				break;
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Date:
				//value = EnsureType(reader.Value, objectType);
				value = reader.Value;
				break;
			case JsonToken.Constructor:
				value = reader.Value.ToString ();
				break;
			case JsonToken.Null:
			case JsonToken.Undefined:
				value = null;
				break;
			default:
				throw new JsonSerializationException ("Unexpected token whil deserializing object: " + reader.TokenType);
			}

			return value;
		}

		private IEnumerable<object> PopulateList (JsonReader reader/*, Type objectType*/)
		{

			while (reader.Read()) {
				switch (reader.TokenType) {
				case JsonToken.EndArray:
					yield break;
				case JsonToken.Comment:
					break;
				default:
					yield return GetObject (reader/*, elementType*/);

					break;
				}
			}

			throw new JsonSerializationException ("Unexpected end when deserializing array.");
		}

		private IEnumerator<KeyValuePair<string, object>> PopulateObject (JsonReader reader/*, Type objectType*/)
		{
			reader.IncrementRecursionLevel ();
			while (reader.Read ()) {
				switch (reader.TokenType) {
				case JsonToken.PropertyName:
					string memberName = reader.Value.ToString ();

					if (!reader.Read ())
						throw new JsonSerializationException (string.Format ("Unexpected end when setting {0}'s value.", memberName));
					yield return new KeyValuePair<string, object> (memberName, GetObject (reader));
					break;
				case JsonToken.EndObject:
					reader.DecrementRecursionLevel ();
					yield break;
				default:
					throw new JsonSerializationException ("Unexpected token when deserializing object: " + reader.TokenType);
				}
			}

			throw new JsonSerializationException ("Unexpected end when deserializing object.");
		}
		#endregion

		#region Serialize
		/// <summary>
		/// Serializes the specified <see cref="Object"/> and writes the Json structure
		/// to a <c>Stream</c> using the specified <see cref="TextWriter"/>. 
		/// </summary>
		/// <param name="textWriter">The <see cref="TextWriter"/> used to write the Json structure.</param>
		/// <param name="value">The <see cref="Object"/> to serialize.</param>
		public void Serialize (TextWriter textWriter, object value)
		{
			Serialize (new JsonWriter (textWriter, MaxJsonLength), value);
		}

		/// <summary>
		/// Serializes the specified <see cref="Object"/> and writes the Json structure
		/// to a <c>Stream</c> using the specified <see cref="JsonWriter"/>. 
		/// </summary>
		/// <param name="jsonWriter">The <see cref="JsonWriter"/> used to write the Json structure.</param>
		/// <param name="value">The <see cref="Object"/> to serialize.</param>
		void Serialize (JsonWriter jsonWriter, object value)
		{
			SerializeValue (jsonWriter, value);
		}

		private void SerializeValue (JsonWriter writer, object value)
		{
			//JsonConverter converter;
			_currentRecursionCounter++;
			if (RecursionLimit > 0 && _currentRecursionCounter > RecursionLimit) {
				throw new ArgumentException ("RecursionLimit exceeded.");
			}

			if (value == null) {
				writer.WriteNull ();
			} else {
				JavaScriptConverter jsconverter = _context.GetConverter (value.GetType ());
				if (jsconverter != null) {
					value = jsconverter.Serialize (value, _context);
					if (value == null) {
						writer.WriteNull ();
						return;
					}
				}

				Type valueType = value.GetType ();
				switch (Type.GetTypeCode (valueType)) {
				case TypeCode.String:
					writer.WriteValue ((string)value);
					break;
				case TypeCode.Char:
					writer.WriteValue ((char)value);
					break;
				case TypeCode.Boolean:
					writer.WriteValue ((bool)value);
					break;
				case TypeCode.SByte:
					writer.WriteValue ((sbyte)value);
					break;
				case TypeCode.Int16:
					writer.WriteValue ((short)value);
					break;
				case TypeCode.UInt16:
					writer.WriteValue ((ushort)value);
					break;
				case TypeCode.Int32:
					writer.WriteValue ((int)value);
					break;
				case TypeCode.Byte:
					writer.WriteValue ((byte)value);
					break;
				case TypeCode.UInt32:
					writer.WriteValue ((uint)value);
					break;
				case TypeCode.Int64:
					writer.WriteValue ((long)value);
					break;
				case TypeCode.UInt64:
					writer.WriteValue ((ulong)value);
					break;
				case TypeCode.Single:
					writer.WriteValue ((float)value);
					break;
				case TypeCode.Double:
					writer.WriteValue ((double)value);
					break;
				case TypeCode.DateTime:
					writer.WriteValue ((DateTime)value);
					break;
				case TypeCode.Decimal:
					writer.WriteValue ((decimal)value);
					break;
				default:
					

					ThrowOnReferenceLoop (writer, value);
					writer.SerializeStack.Push (value);
					try {
						Type genDictType;
						if (value is IDictionary)
							SerializeDictionary (writer, (IDictionary)value);
						else if (value is IDictionary<string, object>)
							SerializeDictionary (writer, (IDictionary<string, object>)value, null);
						else if ((genDictType = ReflectionUtils.GetGenericDictionary (valueType)) != null)
							SerializeDictionary (writer, new GenericDictionaryLazyDictionary (value, genDictType), null);
						else if (value is IEnumerable) {
							SerializeEnumerable (writer, (IEnumerable)value);
						} else {
							SerializeCustomObject (writer, value, valueType);
						}
					} finally {

						object x = writer.SerializeStack.Pop ();
						if (x != value)
							throw new InvalidOperationException ("Serialization stack is corrupted");
					}

					break;
				}
			}

			_currentRecursionCounter--;
		}

		private void ThrowOnReferenceLoop (JsonWriter writer, object value)
		{
			switch (_referenceLoopHandling) {
			case ReferenceLoopHandling.Error:
				if (writer.SerializeStack.Contains (value))
					throw new JsonSerializationException ("Self referencing loop");
				break;
			case ReferenceLoopHandling.Ignore:
				// return from method
				return;
			case ReferenceLoopHandling.Serialize:
				// continue
				break;
			default:
				throw new InvalidOperationException (string.Format ("Unexpected ReferenceLoopHandling value: '{0}'", _referenceLoopHandling));
			}
		}

		private void SerializeEnumerable (JsonWriter writer, IEnumerable values)
		{
			writer.WriteStartArray ();

			foreach (object value in values)
				SerializeValue (writer, value);

			writer.WriteEndArray ();
		}

		private void SerializeDictionary (JsonWriter writer, IDictionary values)
		{
			writer.WriteStartObject ();

			foreach (DictionaryEntry entry in values)
				SerializePair (writer, entry.Key.ToString (), entry.Value);

			writer.WriteEndObject ();
		}

		private void SerializeDictionary (JsonWriter writer, IDictionary<string, object> values, string typeID)
		{
			writer.WriteStartObject ();

			if (typeID != null) {
				SerializePair (writer, JavaScriptSerializer.SerializedTypeNameKey, typeID);
			}

			foreach (KeyValuePair<string, object> entry in values)
				SerializePair (writer, entry.Key, entry.Value);

			writer.WriteEndObject ();
		}

		private void SerializeCustomObject (JsonWriter writer, object value, Type valueType)
		{
			if (value is Uri) {
				Uri uri = value as Uri;
				writer.WriteValue (uri.GetComponents (UriComponents.AbsoluteUri, UriFormat.UriEscaped));
				return;
			}
			if (valueType == typeof(Guid)) {
				writer.WriteValue (((Guid)value).ToString ());
				return;
			}

			string typeID = null;
			if (_typeResolver != null) {
				typeID = _typeResolver.ResolveTypeId (valueType);
			}

			SerializeDictionary (writer, new SerializerLazyDictionary (value), typeID);
		}

		private void SerializePair (JsonWriter writer, string key, object value)
		{
			writer.WritePropertyName (key);
			SerializeValue (writer, value);
		}

		#endregion
	}
}
