using Appverse.Core.Scanner;
using Lumia.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Unity.Core.Media.Camera;
using UnityPlatformWindowsPhone;
using UnityPlatformWindowsPhone.Internals;
using VideoEffects;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ZXing;
using ZXing.Common;
using Panel = Windows.Devices.Enumeration.Panel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UnityRuntimeWindowsPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CameraViewPage : Page
    {
        private IList<DeviceInformation> _deviceList;
        private MediaCapture _cameraCapture;
        private MediaCaptureInitializationSettings _captureInitSettings;
        private ScannerAutoFocus _scannerAutoFocus;
        private CameraOptions _cameraOptions;
        private bool _qrDetectionModeEnabled, _qrAutoHandleQrCode, _bInitializingCamera;
        private BarcodeReader _barcodeReader;
        private MainPageTransferObject _oReturnObject;

        public CameraViewPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            Window.Current.VisibilityChanged -= Current_VisibilityChanged;
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var initOptions = e.Parameter as CameraViewInitObject;
            if (initOptions == null)
            {
                Frame.GoBack();
                return;
            }
            _cameraOptions = initOptions.CameraOptions;
            _qrDetectionModeEnabled = initOptions.QRDetectionMode;
            _qrAutoHandleQrCode = initOptions.AutoHandleQRCode;
            ((Storyboard)Resources["CameraGridFadeInStoryBoard"]).Begin();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            Window.Current.VisibilityChanged += Current_VisibilityChanged;
        }

        void Current_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (!e.Visible)
            {
                DisposeCaptureAsync();
            }
            else
            {
                PrepareCameraView();
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            CancelCameraButton_OnClick(null, null);
        }

        private async void CameraGridFadeOutStoryBoard_OnCompleted(object sender, object e)
        {
            if (Dispatcher.HasThreadAccess)
            {
                PhotoPreview.Source = null;
                CameraGrid.Visibility = Visibility.Collapsed;
                CancelCameraButton.IsEnabled = false;
                ShutterButton.IsEnabled = false;
                //Return to mainPage passing parameter
                if (_oReturnObject == null)
                {
                    if (Frame.CanGoBack) Frame.GoBack();
                }
                else
                {
                    Frame.Navigate(typeof(MainPage), _oReturnObject);
                }
                _oReturnObject = null;
                GC.Collect();
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    CameraGridFadeOutStoryBoard_OnCompleted(sender, e);
                });
            }
        }

        private async void CameraShutter_Click(object sender, RoutedEventArgs e)
        {
            if (_cameraCapture == null) return;
            try
            {
                using (var memoryStream = new InMemoryRandomAccessStream())
                {
                    var jpegFile = await WindowsPhoneUtils.DocumentsFolder.CreateFileAsync(
                               Guid.NewGuid().ToString().Replace("-", String.Empty) + ".jpg", CreationCollisionOption.ReplaceExisting);

                    if (_cameraCapture.VideoDeviceController.FocusControl.Supported)
                        await _cameraCapture.VideoDeviceController.FocusControl.FocusAsync();
                    await _cameraCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), memoryStream);

                    var dec = await BitmapDecoder.CreateAsync(BitmapDecoder.JpegDecoderId, memoryStream);
                    using (var fileStream = await jpegFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var enc = await BitmapEncoder.CreateForTranscodingAsync(fileStream, dec);

                        if (_cameraOptions != null && _cameraOptions.ImageScaleFactor != 1)
                        {
                            enc.BitmapTransform.ScaledWidth = (uint)(dec.PixelWidth * _cameraOptions.ImageScaleFactor);
                            enc.BitmapTransform.ScaledHeight = (uint)(dec.PixelHeight * _cameraOptions.ImageScaleFactor);
                        }

                        //roate the image
                        enc.BitmapTransform.Rotation = BitmapRotation.Clockwise90Degrees;

                        //write changes to the image stream
                        await enc.FlushAsync();
                    }
                    var returnMediaMetadata = await AppverseBridge.Instance.PlatformHandler.GetMediaMetadata(jpegFile);
                    _oReturnObject = new MainPageTransferObject { CallbackName = "Appverse.Media.onFinishedPickingImage", JSONContent = JsonConvert.SerializeObject(returnMediaMetadata) };
                }
                HideCameraView();
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
            }
        }

        private async Task HideCameraView()
        {
            if (Dispatcher.HasThreadAccess)
            {
                try
                {
                    if (_cameraCapture != null)
                    {
                        await DisposeCaptureAsync();
                    }
                }
                catch (ObjectDisposedException ex)
                {
                }
                _cameraCapture = null;
                await StatusBar.GetForCurrentView().ShowAsync();
                ((Storyboard)Resources["CameraGridFadeOutStoryBoard"]).Begin();
            }
            else
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.High, async () => await HideCameraView());
            }
        }

        private async Task InitCaptureSettings(Panel deviceLocation)
        {
            try
            {
                _deviceList = _deviceList ?? await WindowsPhoneUtils.EnumerateDevices(DeviceClass.VideoCapture);
                var deviceSelected = _deviceList.FirstOrDefault(camera => camera.IsEnabled && camera.EnclosureLocation != null && camera.EnclosureLocation.Panel == deviceLocation);
                deviceSelected = deviceSelected ?? (_deviceList.Count > 0 ? _deviceList[0] : null);

                if (deviceSelected == null) return;
                _captureInitSettings = new MediaCaptureInitializationSettings()
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                    PhotoCaptureSource = PhotoCaptureSource.VideoPreview,
                    VideoDeviceId = deviceSelected.Id
                };
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log("No camera suitable found");
            }
        }

        private async Task InitiateCameraCaptureObject(Panel deviceLocation)
        {
            try
            {
                if (_bInitializingCamera || _cameraCapture != null) return;
                _bInitializingCamera = true;
                await InitCaptureSettings(deviceLocation);
                _cameraCapture = new MediaCapture();
                await _cameraCapture.InitializeAsync(_captureInitSettings);
                //Enable QR Detector
                if (_qrDetectionModeEnabled)
                {
                    var formats = _cameraCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo);
                    var format = (VideoEncodingProperties)formats.OrderBy((item) =>
                    {
                        var props = (VideoEncodingProperties)item;
                        return Math.Abs(props.Height - ActualWidth) + Math.Abs(props.Width - ActualHeight);
                    }).First();
                    await _cameraCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, format);
                    var definition = new LumiaAnalyzerDefinition(ColorMode.Yuv420Sp, format.Width >= format.Height ? format.Width : format.Height, AnalyzeImage);
                    await _cameraCapture.AddEffectAsync(MediaStreamType.VideoPreview, definition.ActivatableClassId, definition.Properties);
                    _barcodeReader = _barcodeReader ?? new BarcodeReader
                    {
                        Options = new DecodingOptions
                        {
                            PossibleFormats = new[] { BarcodeFormat.QR_CODE, BarcodeFormat.CODE_128 },
                            TryHarder = true
                        }
                    };
                }

                PhotoPreview.Source = _cameraCapture;
                await _cameraCapture.StartPreviewAsync();
                _cameraCapture.Failed += CameraCaptureOnFailed;
                _scannerAutoFocus = await ScannerAutoFocus.StartAsync(_cameraCapture.VideoDeviceController.FocusControl);
                _cameraCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
            }
            _bInitializingCamera = false;
        }

        private void CameraCaptureOnFailed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CancelCameraButton_OnClick(null, null);
            });
        }

        private async Task DisposeCaptureAsync()
        {
            _cameraCapture.Failed -= CameraCaptureOnFailed;
            await _cameraCapture.ClearEffectsAsync(MediaStreamType.VideoPreview);
            PhotoPreview.Source = null;
            _barcodeReader = null;

            if (_scannerAutoFocus != null)
            {
                _scannerAutoFocus.Dispose();
                _scannerAutoFocus = null;
            }

            MediaCapture capture;
            lock (this)
            {
                capture = _cameraCapture;
                _cameraCapture = null;
            }

            if (capture != null)
            {
                capture.Failed -= CameraCaptureOnFailed;
                capture.Dispose();
            }
        }

        private void AnalyzeImage(Bitmap bitmap, TimeSpan time)
        {
            if (!_qrDetectionModeEnabled) return;
            var result = _barcodeReader.Decode(bitmap.Buffers[0].Buffer.ToArray(),
                (int)bitmap.Buffers[0].Pitch,
                (int)bitmap.Dimensions.Height,
                RGBLuminanceSource.BitmapFormat.Gray8);
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (result == null)
                {
                    if (_scannerAutoFocus != null) _scannerAutoFocus.BarcodeFound = false;
                }
                else
                {
                    if (_scannerAutoFocus != null)
                    {
                        _cameraCapture.Failed -= CameraCaptureOnFailed;
                        _cameraCapture.ClearEffectsAsync(MediaStreamType.VideoPreview);
                        _scannerAutoFocus.BarcodeFound = true;
                    }
                    var returnQR = new MediaQRContent(result.Text,
                                                WindowsPhoneUtils.ZxingToBarcode(result.BarcodeFormat),
                                                WindowsPhoneUtils.GetQRTypeFromCode(result));
                    _oReturnObject = new MainPageTransferObject { CallbackName = "Appverse.Scanner.onQRCodeDetected", JSONContent = JsonConvert.SerializeObject(returnQR) };
                    if (_qrAutoHandleQrCode)
                    {
                        var launchUri = Uri.IsWellFormedUriString(returnQR.Text, UriKind.RelativeOrAbsolute)
                            ? new Uri(returnQR.Text)
                            : null;
                        var ignore = Launcher.LaunchUriAsync(launchUri).AsTask().Result;
                    }
                    _cameraCapture.ClearEffectsAsync(MediaStreamType.VideoPreview);
                    HideCameraView();
                }
            });
        }

        private async Task EnableCameraCustomOverlay(CameraOptions options)
        {
            await StatusBar.GetForCurrentView().HideAsync();
            if (options == null || !options.UseCustomCameraOverlay) return;
            _cameraOptions = options;
            var GuideMarksBrush = FromHexColorToBrush(options.GuidelinesColorHexadecimal);
            var screenWidth = Window.Current.Bounds.Width;
            var screenHeight = Window.Current.Bounds.Height;
            var WPBlackMargin = 25;

            /*ULGuideMark.Stroke = URGuideMark.Stroke = BLGuideMark.Stroke = BRGuideMark.Stroke = GuideMarksBrush;
           //UpperLeft Mark
           ULGuideMark.Points = new PointCollection() { new Point(WPBlackMargin, (screenHeight / 10) + WPBlackMargin), new Point(WPBlackMargin, WPBlackMargin), new Point((screenWidth / 10) + WPBlackMargin, WPBlackMargin) };
           //UpperRight Mark
           URGuideMark.Points = new PointCollection() { new Point((screenWidth - screenWidth / 10) - WPBlackMargin, WPBlackMargin), new Point(screenWidth - WPBlackMargin, WPBlackMargin), new Point(screenWidth - WPBlackMargin, (screenHeight / 10) + WPBlackMargin) };
           //BottomLeft Mark
           BLGuideMark.Points = new PointCollection() { new Point(WPBlackMargin, (screenHeight - WPBlackMargin - (screenHeight / 10))), new Point(WPBlackMargin, screenHeight - WPBlackMargin), new Point((screenWidth / 10) + WPBlackMargin, screenHeight - WPBlackMargin) };
           //BottomRight Mark
           BRGuideMark.Points = new PointCollection() { new Point((screenWidth - WPBlackMargin - (screenWidth / 10)), screenHeight - WPBlackMargin), new Point(screenWidth - WPBlackMargin, screenHeight - WPBlackMargin), new Point(screenWidth - WPBlackMargin, (screenHeight - WPBlackMargin - (screenHeight / 10))) };
           */
            if (options.Overlay != null)
            {
                BitmapImage imageBitmap;
                try
                {
                    imageBitmap = new BitmapImage(new Uri("ms-appx:///Assets/" + options.Overlay + ".png"));
                }
                catch (Exception ex)
                {
                    imageBitmap = new BitmapImage(new Uri("ms-appx:///Assets/background.png"));
                }

                CameraPreviewOverlay.Source = imageBitmap;
            }

            CameraButtonsGrid.Margin = new Thickness(0, 0, 0, options.ScanButtonMarginBottom);

            // CameraDescription Label
            CameraDescriptionTextBlock.Text = options.DescriptionLabelText;
            CameraDescriptionTextBlock.Foreground = FromHexColorToBrush(options.DescriptionLabelColorHexadecimal);
            CameraDescriptionTextBlock.Height = options.DescriptionLabelHeight;
            CameraDescriptionTextBlock.FontFamily = new FontFamily(options.DescriptionLabelFontFamilyName);
            CameraDescriptionTextBlock.FontSize = options.DescriptionLabelFontSize;
            CameraDescriptionTextBlock.Margin = new Thickness(options.DescriptionLabelMarginLeftRight, 0, options.DescriptionLabelMarginLeftRight, options.DescriptionLabelMarginBottom);

            // CameraDescription Label Shadows...
            CameraDescriptionTextBlockShadow1.Text =
                CameraDescriptionTextBlockShadow1.Text =
                    CameraDescriptionTextBlockShadow1.Text = CameraDescriptionTextBlockShadow1.Text = CameraDescriptionTextBlock.Text;
            CameraDescriptionTextBlockShadow1.Height =
                CameraDescriptionTextBlockShadow1.Height =
                    CameraDescriptionTextBlockShadow1.Height = CameraDescriptionTextBlockShadow1.Height = CameraDescriptionTextBlock.Height;
            CameraDescriptionTextBlockShadow1.FontFamily =
                CameraDescriptionTextBlockShadow1.FontFamily =
                    CameraDescriptionTextBlockShadow1.FontFamily = CameraDescriptionTextBlockShadow1.FontFamily = CameraDescriptionTextBlock.FontFamily;
            CameraDescriptionTextBlockShadow1.FontSize =
                CameraDescriptionTextBlockShadow1.FontSize =
                    CameraDescriptionTextBlockShadow1.FontSize = CameraDescriptionTextBlockShadow1.FontSize = CameraDescriptionTextBlock.FontSize;
            CameraDescriptionTextBlockShadow1.Margin = new Thickness(options.DescriptionLabelMarginLeftRight + 3, 0 + 3, options.DescriptionLabelMarginLeftRight - 3, options.DescriptionLabelMarginBottom - 3);
            CameraDescriptionTextBlockShadow2.Margin = new Thickness(options.DescriptionLabelMarginLeftRight + 4, 0 + 4, options.DescriptionLabelMarginLeftRight - 4, options.DescriptionLabelMarginBottom - 4);
            CameraDescriptionTextBlockShadow3.Margin = new Thickness(options.DescriptionLabelMarginLeftRight + 5, 0 + 5, options.DescriptionLabelMarginLeftRight - 5, options.DescriptionLabelMarginBottom - 5);
            CameraDescriptionTextBlockShadow4.Margin = new Thickness(options.DescriptionLabelMarginLeftRight + 6, 0 + 6, options.DescriptionLabelMarginLeftRight - 6, options.DescriptionLabelMarginBottom - 6);
            CameraDescriptionTextBlockShadow1.Visibility =
                CameraDescriptionTextBlockShadow2.Visibility =
                    CameraDescriptionTextBlockShadow3.Visibility =
                        CameraDescriptionTextBlockShadow4.Visibility = Visibility.Visible;
            //Shutter Button
            ShutterButton.Height = options.ScanButtonHeight;
            ShutterButton.Width = options.ScanButtonWidth;
            ShutterButton.IsEnabled = true;

            //CancelButton
            CancelCameraButton.Content = options.CancelButtonText;
            CancelCameraButton.Width = options.CancelButtonWidth;
            CancelCameraButton.Height = options.CancelButtonHeight;
            CancelCameraButton.FontSize = options.CancelButtonFontSize;
            CancelCameraButton.Foreground = FromHexColorToBrush(options.CancelButtonColorHexadecimal);
            CancelCameraButton.FontFamily = new FontFamily(options.CancelButtonFontFamilyName);
            CancelCameraButton.IsEnabled = true;

            CameraButtonsGrid.Visibility = Visibility.Visible;
        }

        private SolidColorBrush FromHexColorToBrush(string hexColor)
        {
            hexColor = hexColor.Replace("#", string.Empty);

            var r = (byte)(Convert.ToUInt32(hexColor.Substring(0, 2), 16));
            var g = (byte)(Convert.ToUInt32(hexColor.Substring(2, 2), 16));
            var b = (byte)(Convert.ToUInt32(hexColor.Substring(4, 2), 16));
            return new SolidColorBrush(Color.FromArgb(255, r, g, b));
        }

        private async Task DisableCameraCustomOverlay()
        {
            if (Dispatcher.HasThreadAccess)
            {
                CameraButtonsGrid.Visibility = Visibility.Visible;
                ShutterButton.IsEnabled = CancelCameraButton.IsEnabled = true;
                ShutterButton.Visibility = Visibility.Visible;
                //ULGuideMark.Visibility = URGuideMark.Visibility = BLGuideMark.Visibility = BRGuideMark.Visibility = Visibility.Collapsed;
                CameraDescriptionTextBlock.Visibility =
                    CameraDescriptionTextBlockShadow1.Visibility =
                        CameraDescriptionTextBlockShadow2.Visibility =
                            CameraDescriptionTextBlockShadow3.Visibility =
                                CameraDescriptionTextBlockShadow4.Visibility = Visibility.Collapsed;
            }
            else await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () => { await DisableCameraCustomOverlay(); });
        }

        private async void CancelCameraButton_OnClick(object sender, RoutedEventArgs e)
        {
            _oReturnObject = new MainPageTransferObject
            {
                CallbackName = "Appverse.Media.onFinishedPickingImage",
                JSONContent = JsonConvert.SerializeObject(_oReturnObject)
            };
            await HideCameraView();
        }

        private async void CameraGridFadeInStoryBoard_Completed(object sender, object e)
        {
            await PrepareCameraView();
        }

        private async Task PrepareCameraView()
        {
            if (Dispatcher.HasThreadAccess)
            {
                try
                {
                    var deviceCameraPanel = (_cameraOptions == null) ? Panel.Back : _cameraOptions.UseFrontCamera ? Panel.Front : Panel.Back;
                    if (_cameraOptions == null || !_cameraOptions.UseCustomCameraOverlay)
                    {
                        await DisableCameraCustomOverlay();
                    }
                    else
                    {
                        await EnableCameraCustomOverlay(_cameraOptions);
                    }
                    ShutterButton.Visibility = _qrDetectionModeEnabled ? Visibility.Collapsed : Visibility.Visible;
                    PhotoPreview.Width = Window.Current.Bounds.Width;
                    PhotoPreview.Height = Window.Current.Bounds.Height;
                    await StatusBar.GetForCurrentView().HideAsync();
                    await InitiateCameraCaptureObject(deviceCameraPanel);
                }
                catch (Exception ex)
                {
                    WindowsPhoneUtils.Log(ex.Message);
                }
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => { PrepareCameraView(); });
            }
        }
    }
}