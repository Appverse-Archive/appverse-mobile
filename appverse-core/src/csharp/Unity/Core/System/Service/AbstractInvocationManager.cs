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
using System.Reflection;
using Unity.Core.IO.ScriptSerialization;
using Unity.Core.Geo;

namespace Unity.Core.System.Service
{
    public abstract class AbstractInvocationManager : IInvocationManager
    {
#if !WP8
        protected static JavaScriptSerializer serialiser = new JavaScriptSerializer();

        static AbstractInvocationManager()
        {

            // Register Javascript Serialization Converters
            List<JavaScriptConverter> converters = new List<JavaScriptConverter>();
            converters.Add(new JavaScriptDateTimeConverter());
            converters.Add(new JSLocationCoordinateConverter());

            serialiser.RegisterConverters(converters);
        }

        public AbstractInvocationManager()
        {
        }

        #region IInvocationManager implementation

        public abstract byte[] InvokeService(object service, string methodName, string queryString);

        public abstract byte[] InvokeService(object service, string methodName, object[] invokeParams);

        public abstract string CacheControlHeader();

        #endregion

        public abstract object GetObject(Type type, object rawObject);

        protected object InvokeServiceMethod(Object service, string methodName, object[] invokeParams)
        {
            object result = null;

            if (service != null)
            {

                Type type = service.GetType();
                SystemLogger.Log(SystemLogger.Module.CORE, "### Service to Invoke: " + type);
                SystemLogger.Log(SystemLogger.Module.CORE, "### Invocation method: " + methodName);
                try
                {
                    // Get method invocation parameters, if any.
                    object[] methodParams = GetMethodParameters(type, methodName, invokeParams);
                    SystemLogger.Log(SystemLogger.Module.CORE, "### Invocation method params length: " + ((methodParams != null) ? methodParams.Length : 0));

                    // Invoke service.
                    result = type.InvokeMember(methodName, BindingFlags.InvokeMethod, Type.DefaultBinder, service, methodParams);
                }
                catch (Exception e)
                {
                    SystemLogger.Log(SystemLogger.Module.CORE, "Exception invoking method [" + methodName + "]", e);
                    // Throw exception up (to be catched at service handler level).
                    throw e;
                }
            }
            else
            {
                SystemLogger.Log(SystemLogger.Module.CORE, "No service object received to invoke.");
            }

            return result;
        }


        /// <summary>
        /// Returns an array of method invocation parameters,
        /// given a string array of parameters.
        /// Objects are instantiated as needed types.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="methodName"></param>
        /// <param name="invokeParams">array of invocation parameters.</param>
        /// <returns>Method invocation parameters as an object array.</returns>
        private object[] GetMethodParameters(Type serviceType, string methodName, object[] invokeParams)
        {
            object[] methodArguments = null;
            int index = 0;
            if (invokeParams != null && invokeParams.Length > 0)
            {

                MethodInfo[] methods = serviceType.GetMethods();
                MethodInfo mi = null;
                bool methodFound = false;

                for (int j = 0; j < methods.Length && !methodFound; j++)
                {
                    MethodInfo method = methods[j];
                    //SystemLogger.Log(SystemLogger.Module .CORE, "#Method info: " + method.Name + ", " + method.GetParameters().Length);
                    if (method.Name == methodName && method.GetParameters().Length == invokeParams.Length)
                    {
                        // matching method name and method arguments length
                        mi = method;
                        ParameterInfo[] paramsInfo = mi.GetParameters();
                        methodArguments = new object[(paramsInfo.Length)];
                        for (int i = 0; i < paramsInfo.Length; i++)
                        {
                            ParameterInfo pi = paramsInfo[i];
                            object parameter = invokeParams[i];

                            object paramObj = GetObject(pi.ParameterType, parameter);
                            if (paramObj == null || (paramObj.GetType() == pi.ParameterType))
                            {
                                // parameter type matches desired type.
                                methodArguments[index] = paramObj;
                                index++;
                                if (index == paramsInfo.Length)
                                {
                                    // all parameters matched
                                    methodFound = true;
                                }
                            }
                            else
                            {
                                // no matched type for an argument, try with next method.
                                index = 0;
                                methodArguments = null;
                                mi = null;
                                break;
                            }
                        }

                    }
                }
            }
            return methodArguments;
        }
#else
#endif
    }
}

