using Appverse.Core.Scanner;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Unity.Core.Media.Camera;
using UnityPlatformWindowsPhone;
using UnityPlatformWindowsPhone.Internals;
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
                
            }
            else
            {
                
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
                        
                    }
                    ShutterButton.Visibility = _qrDetectionModeEnabled ? Visibility.Collapsed : Visibility.Visible;
                    PhotoPreview.Width = Window.Current.Bounds.Width;
                    PhotoPreview.Height = Window.Current.Bounds.Height;
                    await StatusBar.GetForCurrentView().HideAsync();
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