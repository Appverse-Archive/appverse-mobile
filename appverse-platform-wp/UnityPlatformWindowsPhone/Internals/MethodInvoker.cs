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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace UnityPlatformWindowsPhone.Internals
{
    public class MethodInvoker
    {

        internal IReadOnlyList<Type> ParameterTypeList { get; private set; }
        internal Delegate MethodDelegate { get; private set; }
        internal int ParameterCount { get; private set; }
        internal string MethodName { get; private set; }
        internal Type ReturnType { get; private set; }

        private MethodInvoker()
        {
        }

        internal static MethodInvoker Create(MethodInfo method, object target)
        {

            var args = new List<Type>(method.GetParameters().Select(p => p.ParameterType));
            Type delegateType;

            var returnMethodInvoker = new MethodInvoker
            {
                ParameterTypeList = args.ToList(),
                ParameterCount = args.Count,
                ReturnType = method.ReturnType,
                MethodName = method.Name
            };


            if (method.ReturnType == typeof(void))
            {
                delegateType = Expression.GetActionType(args.ToArray());
            }
            else
            {
                args.Add(method.ReturnType);
                delegateType = Expression.GetFuncType(args.ToArray());
            }

            returnMethodInvoker.MethodDelegate = method.CreateDelegate(delegateType, target);

            return returnMethodInvoker;

        }


    }
}
