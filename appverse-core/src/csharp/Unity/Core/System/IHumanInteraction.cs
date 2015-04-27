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
#if WP8
using System.Threading.Tasks;
#endif
namespace Unity.Core.System
{
    public interface IHumanInteraction
    {
#if !WP8
        /// <summary>
        /// List of configured Locales for the device.
        /// </summary>
        /// <returns>List of locales.</returns>
        Locale[] GetLocaleSupported();

        /// <summary>
        /// Current locale of the device.
        /// </summary>
        /// <returns>Locale info.</returns>
        Locale GetLocaleCurrent();

        /// <summary>
        /// Currently active input method.
        /// </summary>
        /// <returns>Input method.</returns>
        InputCapability GetInputMethodCurrent();

        /// <summary>
        /// Supported input methods.
        /// </summary>
        /// <returns>List of input methods supported by the device.</returns>
        InputCapability[] GetInputMethods();

        /// <summary>
        /// List of gestures supported natively by the device.
        /// </summary>
        /// <returns>List of gestures.</returns>
        InputGesture[] GetInputGestures();

        /// <summary>
        /// List of hardware buttons provided by the device.
        /// </summary>
        /// <returns>List of buttons.</returns>
        InputButton[] GetInputButtons();

        /// <summary>
        /// Copies a specified text to device clipboard.
        /// </summary>
        /// <returns>
        /// TRUE if the text was successfully copied to Clipboard, esle FALSE
        /// </returns>
        /// <param name='text'>
        /// Text to copy to the Clipboard
        /// </param>
        bool CopyToClipboard(string text);
#else
        /// <summary>
        /// List of configured Locales for the device.
        /// </summary>
        /// <returns>List of locales.</returns>
        Task<Locale[]> GetLocaleSupported();

        /// <summary>
        /// Current locale of the device.
        /// </summary>
        /// <returns>Locale info.</returns>
        Task<Locale> GetLocaleCurrent();

        /// <summary>
        /// Currently active input method.
        /// </summary>
        /// <returns>Input method.</returns>
        Task<InputCapability> GetInputMethodCurrent();

        /// <summary>
        /// Supported input methods.
        /// </summary>
        /// <returns>List of input methods supported by the device.</returns>
        Task<InputCapability[]> GetInputMethods();

        /// <summary>
        /// List of gestures supported natively by the device.
        /// </summary>
        /// <returns>List of gestures.</returns>
        Task<InputGesture[]> GetInputGestures();

        /// <summary>
        /// List of hardware buttons provided by the device.
        /// </summary>
        /// <returns>List of buttons.</returns>
        Task<InputButton[]> GetInputButtons();

        /// <summary>
        /// Copies a specified text to device clipboard.
        /// </summary>
        /// <returns>
        /// TRUE if the text was successfully copied to Clipboard, esle FALSE
        /// </returns>
        /// <param name='text'>
        /// Text to copy to the Clipboard
        /// </param>
        Task<bool> CopyToClipboard(string text);
#endif

    }//end IHumanInteraction

}//end namespace System