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
using Unity.Core.System.Service;

namespace Unity.Platform.Windows
{
    public class WindowsServiceLocator : AbstractServiceLocator
    {
        static WindowsServiceLocator()
        {
            typedServices["net"]        = new WindowsNet();
            typedServices["system"]     = new WindowsSystem();
            typedServices["file"]       = new WindowsFileSystem();
            typedServices["db"]         = new WindowsDatabase();
            typedServices["io"]         = new WindowsIO();
            typedServices["notify"]     = new WindowsNotification();
			typedServices["geo"]     	= new WindowsGeo();
			typedServices["message"]    = new WindowsMessaging();
			typedServices["phone"]    	= new WindowsTelephony();
			typedServices["media"]    	= new WindowsMedia();
			typedServices["i18n"]    	= new WindowsI18N();
			typedServices["log"]    	= new WindowsLog();
            typedServices["analytics"]  = new WindowsAnalytics();
            typedServices["security"]   = new WindowsSecurity();
            typedServices["webtrekk"]   = new WindowsWebtrekk();
            typedServices["loader"]     = new WindowsAppLoader();
        }

        /// <summary>
        /// Private Constructor.
        /// Used to force usage of static GetInstance() method.
        /// </summary>
        private WindowsServiceLocator() : base() { }

        /// <summary>
        /// Hides the AbstractServiceLocator class static method by using the keyword new.
        /// </summary>
        /// <returns>Singleton IServiceLocator.</returns>
        public new static IServiceLocator GetInstance()
        {
            if (singletonServiceLocator == null)
            {
                singletonServiceLocator = new WindowsServiceLocator();
            }
            return singletonServiceLocator;
        }

    }
}
