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
using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace Unity.Core.IO.ScriptSerialization.JSON
{
	internal static class ReflectionUtils
	{
		public static bool IsInstantiatableType (Type t)
		{
			if (t == null)
				throw new ArgumentNullException ("t");

			if (t.IsAbstract || t.IsInterface || t.IsArray)
				return false;

			if (!HasDefaultConstructor (t))
				return false;

			return true;
		}

		public static bool HasDefaultConstructor (Type t)
		{
			if (t == null)
				throw new ArgumentNullException ("t");

			return (t.GetConstructor (BindingFlags.Instance, null, Type.EmptyTypes, null) != null);
		}

		public static bool IsAssignable (Type to, Type from)
		{
			if (to == null)
				throw new ArgumentNullException ("to");

			if (to.IsAssignableFrom (from))
				return true;

			if (to.IsGenericType && from.IsGenericTypeDefinition)
				return to.IsAssignableFrom (from.MakeGenericType (to.GetGenericArguments ()));

			return false;
		}

		public static bool IsSubClass (Type type, Type check)
		{
			if (type == null || check == null)
				return false;

			if (type == check)
				return true;

			if (check.IsInterface) {
				foreach (Type t in type.GetInterfaces()) {
					if (IsSubClass (t, check))
						return true;
				}
			}
			if (type.IsGenericType && !type.IsGenericTypeDefinition) {
				if (IsSubClass (type.GetGenericTypeDefinition (), check))
					return true;
			}
			return IsSubClass (type.BaseType, check);
		}

		/// <summary>
		/// Gets the type of the typed list's items.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The type of the typed list's items.</returns>
		public static Type GetTypedListItemType (Type type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");

			if (type.IsArray)
				return type.GetElementType ();
			else if (type.IsGenericType && typeof(List<>).IsAssignableFrom (type.GetGenericTypeDefinition ()))
				return type.GetGenericArguments () [0];
			else
				throw new Exception ("Bad type");
		}

		public static Type GetTypedDictionaryValueType (Type type)
		{
			if (type == null)
				throw new ArgumentNullException ("type");

			Type genDictType = GetGenericDictionary (type);

			if (genDictType != null)
				return genDictType.GetGenericArguments () [1];
			else if (typeof(IDictionary).IsAssignableFrom (type))
				return null;
			else
				throw new Exception ("Bad type");
		}

		static readonly Type GenericDictionaryType = typeof(IDictionary<,>);

		public static Type GetGenericDictionary (Type type)
		{
			if (type.IsGenericType && GenericDictionaryType.IsAssignableFrom (type.GetGenericTypeDefinition ()))
				return type;

			Type[] ifaces = type.GetInterfaces ();
			if (ifaces != null)
				for (int i = 0; i < ifaces.Length; i++) {
					Type current = GetGenericDictionary (ifaces [i]);
					if (current != null)
						return current;
				}

			return null;
		}

		public static Type GetMemberUnderlyingType (MemberInfo member)
		{
			switch (member.MemberType) {
			case MemberTypes.Field:
				return ((FieldInfo)member).FieldType;
			case MemberTypes.Property:
				return ((PropertyInfo)member).PropertyType;
			case MemberTypes.Event:
				return ((EventInfo)member).EventHandlerType;
			default:
				throw new ArgumentException ("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member");
			}
		}

		/// <summary>
		/// Determines whether the member is an indexed property.
		/// </summary>
		/// <param name="member">The member.</param>
		/// <returns>
		/// 	<c>true</c> if the member is an indexed property; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsIndexedProperty (MemberInfo member)
		{
			if (member == null)
				throw new ArgumentNullException ("member");

			PropertyInfo propertyInfo = member as PropertyInfo;

			if (propertyInfo != null)
				return IsIndexedProperty (propertyInfo);
			else
				return false;
		}

		/// <summary>
		/// Determines whether the property is an indexed property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>
		/// 	<c>true</c> if the property is an indexed property; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsIndexedProperty (PropertyInfo property)
		{
			if (property == null)
				throw new ArgumentNullException ("property");

			return (property.GetIndexParameters ().Length > 0);
		}

		/// <summary>
		/// Gets the member's value on the object.
		/// </summary>
		/// <param name="member">The member.</param>
		/// <param name="target">The target object.</param>
		/// <returns>The member's value on the object.</returns>
		public static object GetMemberValue (MemberInfo member, object target)
		{
			switch (member.MemberType) {
			case MemberTypes.Field:
				return ((FieldInfo)member).GetValue (target);
			case MemberTypes.Property:
				try {
					return ((PropertyInfo)member).GetValue (target, null);
				} catch (TargetParameterCountException e) {
					throw new ArgumentException ("MemberInfo has index parameters", "member", e);
				}
			default:
				throw new ArgumentException ("MemberInfo is not of type FieldInfo or PropertyInfo", "member");
			}
		}

		/// <summary>
		/// Sets the member's value on the target object.
		/// </summary>
		/// <param name="member">The member.</param>
		/// <param name="target">The target.</param>
		/// <param name="value">The value.</param>
		public static void SetMemberValue (MemberInfo member, object target, object value)
		{
			switch (member.MemberType) {
			case MemberTypes.Field:
				((FieldInfo)member).SetValue (target, value);
				break;
			case MemberTypes.Property:
				((PropertyInfo)member).SetValue (target, value, null);
				break;
			default:
				throw new ArgumentException ("MemberInfo must be if type FieldInfo or PropertyInfo", "member");
			}
		}

		/// <summary>
		/// Determines whether the specified MemberInfo can be read.
		/// </summary>
		/// <param name="member">The MemberInfo to determine whether can be read.</param>
		/// <returns>
		/// 	<c>true</c> if the specified MemberInfo can be read; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanReadMemberValue (MemberInfo member)
		{
			switch (member.MemberType) {
			case MemberTypes.Field:
				return true;
			case MemberTypes.Property:
				return ((PropertyInfo)member).CanRead;
			default:
				return false;
			}
		}

		/// <summary>
		/// Determines whether the specified MemberInfo can be set.
		/// </summary>
		/// <param name="member">The MemberInfo to determine whether can be set.</param>
		/// <returns>
		/// 	<c>true</c> if the specified MemberInfo can be set; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanSetMemberValue (MemberInfo member)
		{
			switch (member.MemberType) {
			case MemberTypes.Field:
				return true;
			case MemberTypes.Property:
				return ((PropertyInfo)member).CanWrite;
			default:
				return false;
			}
		}

		public static IEnumerable<MemberInfo> GetFieldsAndProperties (Type type, BindingFlags bindingAttr)
		{

			MemberInfo [] members = type.GetFields (bindingAttr);
			for (int i = 0; i < members.Length; i++)
				yield return members [i];
			members = type.GetProperties (bindingAttr);
			for (int i = 0; i < members.Length; i++)
				yield return members [i];
		}
	}
}
