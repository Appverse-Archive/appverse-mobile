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
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Configuration;
using Unity.Core.IO.ScriptSerialization.JSON;

//using System.Web.Configuration;

namespace Unity.Core.IO.ScriptSerialization
{
	public class JavaScriptSerializer
	{
		internal const string SerializedTypeNameKey = "__type";
		
		internal abstract class LazyDictionary : IDictionary<string, object>
		{
			#region IDictionary<string,object> Members

			void IDictionary<string, object>.Add (string key, object value)
			{
				throw new NotSupportedException ();
			}

			bool IDictionary<string, object>.ContainsKey (string key)
			{
				throw new NotSupportedException ();
			}

			ICollection<string> IDictionary<string, object>.Keys {
				get { throw new NotSupportedException (); }
			}

			bool IDictionary<string, object>.Remove (string key)
			{
				throw new NotSupportedException ();
			}

			bool IDictionary<string, object>.TryGetValue (string key, out object value)
			{
				throw new NotSupportedException ();
			}

			ICollection<object> IDictionary<string, object>.Values {
				get { throw new NotSupportedException (); }
			}

			object IDictionary<string, object>.this [string key] {
				get {
					throw new NotSupportedException ();
				}
				set {
					throw new NotSupportedException ();
				}
			}

			#endregion

			#region ICollection<KeyValuePair<string,object>> Members

			void ICollection<KeyValuePair<string, object>>.Add (KeyValuePair<string, object> item)
			{
				throw new NotSupportedException ();
			}

			void ICollection<KeyValuePair<string, object>>.Clear ()
			{
				throw new NotSupportedException ();
			}

			bool ICollection<KeyValuePair<string, object>>.Contains (KeyValuePair<string, object> item)
			{
				throw new NotSupportedException ();
			}

			void ICollection<KeyValuePair<string, object>>.CopyTo (KeyValuePair<string, object>[] array, int arrayIndex)
			{
				throw new NotSupportedException ();
			}

			int ICollection<KeyValuePair<string, object>>.Count {
				get { throw new NotSupportedException (); }
			}

			bool ICollection<KeyValuePair<string, object>>.IsReadOnly {
				get { throw new NotSupportedException (); }
			}

			bool ICollection<KeyValuePair<string, object>>.Remove (KeyValuePair<string, object> item)
			{
				throw new NotSupportedException ();
			}

			#endregion

			#region IEnumerable<KeyValuePair<string,object>> Members

			IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator ()
			{
				return GetEnumerator ();
			}

			protected abstract IEnumerator<KeyValuePair<string, object>> GetEnumerator ();

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator ()
			{
				return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator ();
			}

			#endregion
		}

		List<IEnumerable<JavaScriptConverter>> _converterList;
		int _maxJsonLength;
		int _recursionLimit;
		JavaScriptTypeResolver _typeResolver;
		internal static readonly JavaScriptSerializer DefaultSerializer = new JavaScriptSerializer ();

		public JavaScriptSerializer () 
		//	: this(null)
		{
		}

		//public JavaScriptSerializer (JavaScriptTypeResolver resolver) 
		//{
		//    _typeResolver = resolver;

		//    ScriptingJsonSerializationSection section = (ScriptingJsonSerializationSection) ConfigurationManager.GetSection ("system.web.extensions/scripting/webServices/jsonSerialization");
		//    if (section == null) {
		//        _maxJsonLength = 102400;
		//        _recursionLimit = 100;
		//    }
		//    else {
		//        _maxJsonLength = section.MaxJsonLength;
		//        _recursionLimit = section.RecursionLimit;
		//    }
		//}

		public int MaxJsonLength {
			get {
				return _maxJsonLength;
			}
			set {
				_maxJsonLength = value;
			}
		}
		
		public int RecursionLimit {
			get {
				return _recursionLimit;
			}
			set {
				_recursionLimit = value;
			}
		}

		public T ConvertToType<T> (object obj)
		{
			if (obj == null)
				return default (T);

			return (T)ConvertToType (typeof(T), obj);
		}

		internal object ConvertToType (Type type, object obj)
		{
			if (obj == null)
				return null;

			if (obj is IDictionary<string, object>) {
				if (type == null) {
					obj = EvaluateDictionary ((IDictionary<string, object>)obj);
				} else {
					JavaScriptConverter converter = GetConverter (type);
					if (converter != null) {
						return converter.Deserialize (
                            EvaluateDictionary ((IDictionary<string, object>)obj),
                            type, this);
					}
				}

				return ConvertToObject ((IDictionary<string, object>)obj, type);
			}
			if (obj is IEnumerable<object>)
				return ConvertToList ((IEnumerable<object>)obj, type);

			if (type == null)
				return obj;

			Type sourceType = obj.GetType ();
			if (type.IsAssignableFrom (sourceType))
				return obj;

			if (type.IsEnum)
				return Enum.ToObject (type, obj);
			
			TypeConverter c = null;
			try {
				c = TypeDescriptor.GetConverter (type);
			
			} catch (Exception) {
				if (type.FullName == "System.Int32") {
					c = new Int32Converter ();
					// int
				} else if (type.FullName == "System.Int64") {
					c = new Int64Converter ();
					// long
				} else if (type.FullName == "System.Single") {
					c = new SingleConverter ();
					// float
				} else if (type.FullName == "System.Double") {
					c = new DoubleConverter ();
					// float
				} else if (type.FullName == "System.Boolean") {
					c = new BooleanConverter ();
					// bool
				} else if (type.FullName == "System.Byte") {
					c = new ByteConverter ();
				}
			}
			
			if (c.CanConvertFrom (sourceType)) {
				if (obj is string)
					return c.ConvertFromInvariantString ((string)obj);

				return c.ConvertFrom (obj);
			}

			/*
			 * Take care of the special case whereas in JSON an empty string ("") really means 
			 * an empty value 
			 * (see: https://bugzilla.novell.com/show_bug.cgi?id=328836)
			 */
			if ((type.IsGenericType) && (type.GetGenericTypeDefinition () == typeof(Nullable<>))) {
				string s = obj as String;
				if (String.IsNullOrEmpty (s))
					return null;
			}

			return Convert.ChangeType (obj, type);
		}

		public T Deserialize<T> (string input)
		{
			return ConvertToType<T> (DeserializeObjectInternal (input));
		}

		static object Evaluate (object value)
		{
			return Evaluate (value, false);
		}

		static object Evaluate (object value, bool convertListToArray)
		{
			if (value is IDictionary<string, object>)
				value = EvaluateDictionary ((IDictionary<string, object>)value, convertListToArray);
			else
			if (value is IEnumerable<object>)
				value = EvaluateList ((IEnumerable<object>)value, convertListToArray);
			return value;
		}

		static object EvaluateList (IEnumerable<object> e)
		{
			return EvaluateList (e, false);
		}

		static object EvaluateList (IEnumerable<object> e, bool convertListToArray)
		{
			ArrayList list = new ArrayList ();
			foreach (object value in e)
				list.Add (Evaluate (value, convertListToArray));

			return convertListToArray ? (object)list.ToArray () : list;
		}

		static IDictionary<string, object> EvaluateDictionary (IDictionary<string, object> dict)
		{
			return EvaluateDictionary (dict, false);
		}

		static IDictionary<string, object> EvaluateDictionary (IDictionary<string, object> dict, bool convertListToArray)
		{
			if (dict is Dictionary<string, object>)
				return dict;
			Dictionary<string, object> d = new Dictionary<string, object> (StringComparer.Ordinal);
			foreach (KeyValuePair<string, object> entry in dict) {
				d.Add (entry.Key, Evaluate (entry.Value, convertListToArray));
			}

			return d;
		}

		static readonly Type typeofObject = typeof(object);
		static readonly Type typeofGenList = typeof(List<>);

		object ConvertToList (IEnumerable<object> col, Type type)
		{
			Type elementType = null;
			if (type != null && type.HasElementType)
				elementType = type.GetElementType ();

			IList list;
			if (type == null || type.IsArray || typeofObject == type)
				list = new ArrayList ();
			else if (ReflectionUtils.IsInstantiatableType (type))
				// non-generic typed list
				list = (IList)Activator.CreateInstance (type, true);
			else if (ReflectionUtils.IsAssignable (type, typeofGenList)) {
				if (type.IsGenericType) {
					Type [] genArgs = type.GetGenericArguments ();
					elementType = genArgs [0];
					// generic list
					list = (IList)Activator.CreateInstance (typeofGenList.MakeGenericType (genArgs));
				} else
					list = new ArrayList ();
			} else
				throw new JsonSerializationException (string.Format ("Deserializing list type '{0}' not supported.", type.GetType ().Name));

			if (list.IsReadOnly) {
				EvaluateList (col);
				return list;
			}

			foreach (object value in col)
				list.Add (ConvertToType (elementType, value));

			if (type != null && type.IsArray)
				list = ((ArrayList)list).ToArray (elementType);

			return list;
		}

		object ConvertToObject (IDictionary<string, object> dict, Type type)
		{
			if (_typeResolver != null) {
				if (dict is JsonSerializer.DeserializerLazyDictionary) {
					JsonSerializer.DeserializerLazyDictionary lazyDict = (JsonSerializer.DeserializerLazyDictionary)dict;
					object first = lazyDict.PeekFirst ();
					if (first != null) {
						KeyValuePair<string, object> firstPair = (KeyValuePair<string, object>)first;
						if (firstPair.Key == SerializedTypeNameKey) {
							type = _typeResolver.ResolveType ((string)firstPair.Value);
						} else {
							dict = EvaluateDictionary (dict);
						}
					}
				}

				if (!(dict is JsonSerializer.DeserializerLazyDictionary) && dict.Keys.Contains (SerializedTypeNameKey)) {
					// already Evaluated
					type = _typeResolver.ResolveType ((string)dict [SerializedTypeNameKey]);
				}
			}

			object target = Activator.CreateInstance (type, true);

			foreach (KeyValuePair<string, object> entry in dict) {
				object value = entry.Value;
				if (target is IDictionary) {
					((IDictionary)target).Add (entry.Key, ConvertToType (ReflectionUtils.GetTypedDictionaryValueType (type), value));
					continue;
				}
				MemberInfo [] memberCollection = type.GetMember (entry.Key);
				if (memberCollection == null || memberCollection.Length == 0) {
					//must evaluate value
					Evaluate (value);
					continue;
				}

				MemberInfo member = memberCollection [0];

				if (!ReflectionUtils.CanSetMemberValue (member)) {
					//must evaluate value
					Evaluate (value);
					continue;
				}
				
				ReflectionUtils.SetMemberValue (member, target, ConvertToType (ReflectionUtils.GetMemberUnderlyingType (member), value));
			}

			return target;
		}

		public object DeserializeObject (string input)
		{
			object obj = Evaluate (DeserializeObjectInternal (new StringReader (input)), true);
			IDictionary dictObj = obj as IDictionary;
			if (dictObj != null && dictObj.Contains (SerializedTypeNameKey)) {
				if (_typeResolver == null) {
					throw new ArgumentNullException ("resolver", "Must have a type resolver to deserialize an object that has an '__type' member");
				}

				obj = ConvertToType (null, obj);
			}
			return obj; 
		}

		internal object DeserializeObjectInternal (string input)
		{
			return DeserializeObjectInternal (new StringReader (input));
		}

		internal object DeserializeObjectInternal (TextReader input)
		{
			JsonSerializer ser = new JsonSerializer (this, _typeResolver);
			ser.MaxJsonLength = MaxJsonLength;
			ser.RecursionLimit = RecursionLimit;
			return ser.Deserialize (input);
		}

		public void RegisterConverters (IEnumerable<JavaScriptConverter> converters)
		{
			if (converters == null)
				throw new ArgumentNullException ("converters");

			if (_converterList == null)
				_converterList = new List<IEnumerable<JavaScriptConverter>> ();
			_converterList.Add (converters);
		}

		internal JavaScriptConverter GetConverter (Type type)
		{
			if (_converterList != null)
				for (int i = 0; i < _converterList.Count; i++) {
					foreach (JavaScriptConverter converter in _converterList [i])
						foreach (Type supportedType in converter.SupportedTypes)
							if (supportedType == type)
								return converter;
				}

			return null;
		}

		public string Serialize (object obj)
		{
			StringBuilder b = new StringBuilder ();
			Serialize (obj, b);
			return b.ToString ();
		}

		public void Serialize (object obj, StringBuilder output)
		{
			Serialize (obj, new StringWriter (output));
		}

		internal void Serialize (object obj, TextWriter output)
		{
			JsonSerializer ser = new JsonSerializer (this, _typeResolver);
			ser.MaxJsonLength = MaxJsonLength;
			ser.RecursionLimit = RecursionLimit;
			ser.Serialize (output, obj);
		}
	}
}
