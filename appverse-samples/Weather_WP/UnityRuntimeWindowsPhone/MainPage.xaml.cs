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
 
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Unity.Core.Media.Camera;
using Unity.Core.Notification;
using UnityPlatformWindowsPhone;
using UnityPlatformWindowsPhone.Internals;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace UnityRuntimeWindowsPhone
{
    public sealed partial class MainPage : IRTBridge
    {
        private bool _bIsKeyboardPresent;
        private CoreApplicationView _coreviweView;
        private readonly AppverseBridge _appverseBridge;
        private Uri _homeUri;
        public static MainPage Current { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            Current = this;
            Webview = WebViewControl;
            _appverseBridge = AppverseBridge.Instance;
            _appverseBridge.RuntimeHandler = this;
            ApplicationView.GetForCurrentView().IsScreenCaptureEnabled = !WindowsPhoneUtils.AppverseDisableThumbnails;
            NavigationCacheMode = NavigationCacheMode.Required;
            InputPane.GetForCurrentView().Showing += MainPage_KeyboardShowing;
            InputPane.GetForCurrentView().Hiding += MainPage_KeyboardHiding;
            extendedSplashImage.SetValue(HeightProperty, Window.Current.Bounds.Height);
            extendedSplashImage.SetValue(WidthProperty, Window.Current.Bounds.Width);
            SplashGrid.Visibility = WindowsPhoneUtils.AppverseShowSplashScreen ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Dismisses Appverse SplashScreen
        /// </summary>
        /// <returns></returns>
        public async Task DismissSplashScreen()
        {
            if (Dispatcher.HasThreadAccess)
            {
                if (SplashGrid.Opacity != 1) return;
                ((Storyboard)Resources["HideSplashStoryBoard"]).Begin();
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => DismissSplashScreen());
            }
        }

        /// <summary>
        /// Shows Appverse SplashScreen
        /// </summary>
        /// <returns></returns>
        public async Task ShowSplashScreen()
        {
            if (Dispatcher.HasThreadAccess)
            {
                if (SplashGrid.Opacity != 0) return;
                ((Storyboard)Resources["ShowSplashStoryBoard"]).Begin();
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => ShowSplashScreen());
            }
        }

        /// <summary>
        /// Shows the CameraView in QR Detection mode
        /// </summary>
        /// <param name="autoHandleQrCode"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ShowDetectQrView(bool autoHandleQrCode, CameraOptions options)
        {
            NavigateToCameraViewPage(new CameraViewInitObject { CameraOptions = options, AutoHandleQRCode = autoHandleQrCode, QRDetectionMode = true });
        }

        /// <summary>
        /// Launches the CameraView in photo mode
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task ShowCameraView(CameraOptions options)
        {
            NavigateToCameraViewPage(new CameraViewInitObject { CameraOptions = null, AutoHandleQRCode = false, QRDetectionMode = false });
        }

        private void NavigateToCameraViewPage(CameraViewInitObject parameter)
        {

            if (Dispatcher.HasThreadAccess)
            {
                try
                {
                    Frame.Navigate(typeof(CameraViewPage), parameter);
                }
                catch (Exception ex)
                {
                    WindowsPhoneUtils.Log(String.Concat(ex.Message, " ---- ", ex.StackTrace));
                }
            }
            else
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.High, () => NavigateToCameraViewPage(parameter));
            }

        }

        /// <summary>
        /// Method Called when the app is activated after a PhotoPicker has been launched
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void GetSnapshotOnActivated(CoreApplicationView sender, IActivatedEventArgs args)
        {
            try
            {
                _coreviweView.Activated -= GetSnapshotOnActivated;
                var arguments = args as FileOpenPickerContinuationEventArgs;
                if (arguments == null) return;
                if (arguments.Files.Count == 0)
                {
                    WindowsPhoneUtils.InvokeCallback("Appverse.Media.onFinishedPickingImage", "callbackid", JsonConvert.SerializeObject(null));
                }
                else
                {
                    var file = await arguments.Files[0].CopyAsync(WindowsPhoneUtils.DocumentsFolder, arguments.Files[0].Name, NameCollisionOption.ReplaceExisting);
                    var returnMediaMetadata = await AppverseBridge.Instance.PlatformHandler.GetMediaMetadata(file);
                    WindowsPhoneUtils.InvokeCallback("Appverse.Media.onFinishedPickingImage", "callbackid", JsonConvert.SerializeObject(returnMediaMetadata));
                }
            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(ex.Message);
            }
        }

        /// <summary>
        /// Handles the back button.
        /// </summary>
        private async void HandleBackButton()
        {
            if (WebViewControl.Dispatcher.HasThreadAccess)
            {
                WebViewControl.InvokeScriptAsync("eval", new[] { "Appverse.backButtonListener();" });
            }
            else
            {
                await WebViewControl.Dispatcher.RunAsync(CoreDispatcherPriority.High, () => WebViewControl.InvokeScriptAsync("eval", new[] { "Appverse.backButtonListener();" }));
            }
        }

        /// <summary>
        /// Opens a secondary browser.
        /// </summary>
        /// <param name="sTitleBar">The browser's title.</param>
        /// <param name="sUrl">The URL to load.</param>
        /// <returns></returns>
        public async Task OpenBrowser(string sTitleBar, string sUrl)
        {
            if (String.IsNullOrWhiteSpace(sUrl) || !Uri.IsWellFormedUriString(sUrl, UriKind.RelativeOrAbsolute)) return;
            NavigateToAuxBrowserPage(sTitleBar, sUrl);
        }

        /// <summary>
        /// Gets the Primary webview.
        /// </summary>
        /// <value>
        /// The Primary WebView.
        /// </value>
        public WebView Webview { get; private set; }

        public async Task LoadHtml(string sHtml)
        {
            await WebViewControl.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                Webview.NavigateToString(sHtml);
            });
        }

        /// <summary>
        /// Invoked when this page is being navigated away.
        /// </summary>
        /// <param name="e">Event data that describes how this page is navigating.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= MainPage_BackPressed;
            Window.Current.VisibilityChanged -= CurrentOnVisibilityChanged;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                Frame.BackStack.Clear();
                HardwareButtons.BackPressed += MainPage_BackPressed;
                Window.Current.VisibilityChanged += CurrentOnVisibilityChanged;
                if (e.NavigationMode != NavigationMode.Back)
                    Task.Run(async () =>
                    {
                        await Task.Delay(200);
                        _appverseBridge.IndexFileResetEvent.WaitOne();
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        {
                            if (_homeUri == null)
                            {
                                _homeUri = WebViewControl.BuildLocalStreamUri("MyApp", "/indexWP.html");
                                WebViewControl.NavigateToLocalStreamUri(_homeUri, new StreamLocalResolver());
                            }
                            else
                            {
                                var oTransferObject = e.Parameter as MainPageTransferObject;
                                if (oTransferObject != null) Task.Run(() => { WindowsPhoneUtils.InvokeCallback(oTransferObject.CallbackName, oTransferObject.CallbackID, oTransferObject.JSONContent); });
                            }
                        });
                    });
                Window.Current.Activate();

            }
            catch (Exception ex)
            {
                WindowsPhoneUtils.Log(String.Concat(ex.Message, " ---- ", ex.StackTrace));
            }
        }

        /// <summary>
        /// Method called when the app is sent to background/foreground
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="visibilityChangedEventArgs">The <see cref="VisibilityChangedEventArgs"/> instance containing the event data.</param>
        private async void CurrentOnVisibilityChanged(object sender, VisibilityChangedEventArgs visibilityChangedEventArgs)
        {
            if (Webview.Source == null) return;

            if (!visibilityChangedEventArgs.Visible)
            {
                //if (WindowsPhoneUtils.AppverseDisableThumbnails) await ShowSplashScreen();
            }
            else
            {
                //DismissSplashScreen();
                AppverseBridge.Instance.PlatformHandler.ApplicationVisiblityChanged(sender, visibilityChangedEventArgs);
            }
        }

        /// <summary>
        /// Callback executed when the Main Grid fade out animation is completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainGridFadeOutStoryBoard_Completed(object sender, object e)
        {
        }

        /// <summary>
        /// Overrides the back button press to navigate in the WebView's back stack instead of the application's.
        /// </summary>
        private void MainPage_BackPressed(object sender, BackPressedEventArgs e)
        {
            HandleBackButton();
            e.Handled = true;
        }

        private async void MainPage_KeyboardHiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            if (!args.EnsuredFocusedElementInView) args.EnsuredFocusedElementInView = true;
            if (!_bIsKeyboardPresent) return;
            _bIsKeyboardPresent = false;
            await WebViewControl.InvokeScriptAsync("eval", new[] { "Appverse.OnWPKeyboardHide();" });
        }

        private async void MainPage_KeyboardShowing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            if (!args.EnsuredFocusedElementInView) args.EnsuredFocusedElementInView = true;
            if (_bIsKeyboardPresent) return;
            _bIsKeyboardPresent = true;
            await WebViewControl.InvokeScriptAsync("eval", new[] { "Appverse.OnWPKeyboardShow();" });
        }
        

        private void NavigateToAuxBrowserPage(string sTitleBar, string sUrl)
        {
            if (Dispatcher.HasThreadAccess)
            {
                try
                {
                    Frame.Navigate(typeof(AuxBrowserPage), new AuxBrowserInitObject { TitleBar = sTitleBar, URL = sUrl });
                }
                catch (Exception ex)
                {
                    WindowsPhoneUtils.Log(String.Concat(ex.Message, " ---- ", ex.StackTrace));
                }
            }
            else
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.High, () => NavigateToAuxBrowserPage(sTitleBar, sUrl));
            }
        }

        private async void WebViewControl_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            WindowsPhoneUtils.Log("Launched event: NAVIGATIONCOMPLETED");
            if (args.IsSuccess) return;
            WindowsPhoneUtils.Log(String.Concat("Navigation - 404 - " + args.Uri.AbsoluteUri));
        }

        private void WebViewControl_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            WindowsPhoneUtils.Log("Launched event: NAVIGATIONSTARTING");
            WindowsPhoneUtils.Log(String.Concat("Navigation - REQ - ", args.Uri.AbsoluteUri));
        }

        /// <summary>
        /// Handles the ScriptNotify event of the WebViewControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyEventArgs"/> instance containing the event data.</param>
        private async void WebViewControl_ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (!e.CallingUri.Scheme.Equals("ms-local-stream") || !e.CallingUri.LocalPath.EndsWith(@".html")) return;
            var sCommunicationMessage = e.Value;
            Task.Run(() => { _appverseBridge.PlatformHandler.HandleRequest(sCommunicationMessage); });
        }

        /// <summary>
        /// Callback called when the Appverse SplashScreen is dismissed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HideSplashStoryBoard_OnCompleted(object sender, object e)
        {
        }

        /// <summary>
        /// Notifies the javascript layer when the application has been launched using a notification
        /// </summary>
        /// <param name="ev">The LaunchActivatedEventArgs received on application start</param>
        /// <returns></returns>
        public async Task ProcessPushNotification(LaunchActivatedEventArgs ev)
        {
            NotificationData notificationData = new NotificationData
            {
                AlertMessage =
                    String.Concat(ev.Arguments),
                CustomDataJsonString = ev.Arguments,
                AppWasRunning = ev.PreviousExecutionState == ApplicationExecutionState.Running
            };
            await WindowsPhoneUtils.InvokeCallback("Appverse.PushNotifications.OnRemoteNotificationReceived", "callbackid", JsonConvert.SerializeObject(notificationData));
        }

        /// <summary>
        /// Shows the PhotoPicker native object
        /// </summary>
        /// <returns></returns>
        public async Task ShowPhotoPicker()
        {
            if (Dispatcher.HasThreadAccess)
            {
                var photoPicker = new FileOpenPicker
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                    CommitButtonText = "OK"
                };
                photoPicker.FileTypeFilter.Add(".jpg");
                photoPicker.FileTypeFilter.Add(".jpeg");
                photoPicker.FileTypeFilter.Add(".png");
                photoPicker.FileTypeFilter.Add(".gif");
                photoPicker.FileTypeFilter.Add(".tiff");
                try
                {
                    _coreviweView = CoreApplication.GetCurrentView();
                    _coreviweView.Activated += GetSnapshotOnActivated;
                    photoPicker.PickSingleFileAndContinue();
                }
                catch (Exception ex)
                {
                    WindowsPhoneUtils.Log(String.Concat(ex.Message, " ---- ", ex.StackTrace));
                }
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => ShowPhotoPicker());
            }
        }

        public CoreDispatcher RuntimeDispatcher
        {
            get { return Dispatcher; }
        }

        

       


    }
}