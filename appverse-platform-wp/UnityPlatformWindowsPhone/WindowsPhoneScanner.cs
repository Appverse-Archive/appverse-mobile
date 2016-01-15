/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Appverse.Core.Scanner;
using Newtonsoft.Json;
using Unity.Core.Media;
using Unity.Core.Media.Camera;
using UnityPlatformWindowsPhone.Internals;
using ZXing;
using ZXing.Common;

namespace UnityPlatformWindowsPhone
{
    public sealed class WindowsPhoneScanner : AbstractScanner, IAppverseService
    {
        public WindowsPhoneScanner()
        {
            MethodList = new List<MethodInvoker>(WindowsPhoneUtils.GetMethodInvokersList(this));
        }
        public override async Task DetectQRCode(bool autoHandleQR)
        {
            AppverseBridge.Instance.RuntimeHandler.ShowDetectQrView(autoHandleQR, new CameraOptions { UseCustomCameraOverlay = false, UseFrontCamera = false });
        }

        public override async Task DetectQRCodeFront(bool autoHandleQR)
        {
            AppverseBridge.Instance.RuntimeHandler.ShowDetectQrView(autoHandleQR, new CameraOptions { UseCustomCameraOverlay = false, UseFrontCamera = true });
        }

        public async override Task<QRType> HandleQRCode(MediaQRContent mediaQRContent)
        {
            if (mediaQRContent == null || String.IsNullOrWhiteSpace(mediaQRContent.Text)) return QRType.TEXT;
            var launchUri = Uri.IsWellFormedUriString(mediaQRContent.Text, UriKind.RelativeOrAbsolute)
                ? new Uri(mediaQRContent.Text)
                : null;
            await Launcher.LaunchUriAsync(launchUri);
            return mediaQRContent.QRType;
        }

        public override async Task GenerateQRCode(MediaQRContent content)
        {
            if (!AppverseBridge.Instance.RuntimeHandler.RuntimeDispatcher.HasThreadAccess)
            {
                await AppverseBridge.Instance.RuntimeHandler.RuntimeDispatcher.RunAsync(CoreDispatcherPriority.High, () => GenerateQRCode(content));
            }
            else
            {
                if (content == null) return;
                try
                {
                    var size = (content.Size == 0) ? 256 : content.Size;
                    var writer = new BarcodeWriter
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new EncodingOptions { Height = size, Width = size }
                    };

                    //content = encodeQRCodeContents(content);
                    var resultWriteableBitmap = writer.Write(content.Text);

                    var QRFile = await WindowsPhoneUtils.DocumentsFolder.CreateFileAsync(String.Concat("QR_", Guid.NewGuid().ToString().Replace("-", String.Empty), ".png"));
                    //Open File Stream to write content
                    using (var writeStream = await QRFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        byte[] pixels;
                        //Read pixels from generated image
                        using (var readStream = resultWriteableBitmap.PixelBuffer.AsStream())
                        {
                            pixels = new byte[readStream.Length];
                            await readStream.ReadAsync(pixels, 0, pixels.Length);
                        }
                        //Encode pixels in the stream
                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, writeStream);
                        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied,
                            (uint)resultWriteableBitmap.PixelWidth, (uint)resultWriteableBitmap.PixelHeight, 96, 96,
                            pixels);
                        await encoder.FlushAsync();
                    }
                    var mediaData = new MediaMetadata
                    {
                        ReferenceUrl = QRFile.Path.Replace(WindowsPhoneUtils.DocumentsFolder.Path, String.Empty),
                        Title = QRFile.DisplayName
                    };
                    WindowsPhoneUtils.InvokeCallback("Appverse.Scanner.onGeneratedQR", WindowsPhoneUtils.CALLBACKID, JsonConvert.SerializeObject(mediaData));
                }
                catch (Exception ex)
                {
                    WindowsPhoneUtils.Log(String.Concat("Error while generating QR: ", ex.Message));
                }
            }

        }

        #region PRIVATE_METHODS

        private MediaQRContent encodeQRCodeContents(MediaQRContent qrCode)
        {
            var data = qrCode.Text;

            switch (qrCode.QRType)
            {
                case QRType.EMAIL_ADDRESS:
                    qrCode.Text = "mailto:" + data;
                    break;
                case QRType.GEO:
                    var coord = qrCode.Coord;
                    if (coord != null)
                        qrCode.Text = "geo:" + coord.Latitude + "," + coord.Longitude;
                    break;
                case QRType.SMS:
                    qrCode.Text = "sms:" + data;
                    break;
                case QRType.TEL:
                    qrCode.Text = "tel:" + data;
                    break;
                case QRType.ADDRESSBOOK:
                    var sb = new StringBuilder("MECARD:");
                    var name = qrCode.Contact.Name;
                    if (string.IsNullOrEmpty(name))
                        sb.Append("N:" + name + ";");

                    var address = qrCode.Contact.Address;
                    if (string.IsNullOrEmpty(address))
                        sb.Append("ADR:" + address + ";");

                    var phone = qrCode.Contact.Phone;
                    if (string.IsNullOrEmpty(phone))
                        sb.Append("TEL:" + phone + ";");

                    var email = qrCode.Contact.Email;
                    if (string.IsNullOrEmpty(email))
                        sb.Append("EMAIL:" + email + ";");

                    var url = qrCode.Contact.Url;
                    if (string.IsNullOrEmpty(url))
                        sb.Append("URL:" + url + ";");

                    var note = qrCode.Contact.Note;
                    if (string.IsNullOrEmpty(note))
                        sb.Append("NOTE:" + note + ";");

                    qrCode.Text = sb.ToString();
                    break;
            }

            return qrCode;
        }
        #endregion

        public IReadOnlyList<MethodInvoker> MethodList { get; private set; }
    }
}
