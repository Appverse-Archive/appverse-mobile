using System;
using UnityPlatformWindowsPhone.Internals;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace UnityRuntimeWindowsPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuxBrowserPage : Page
    {
        private AuxBrowserInitObject _initObject;

        public AuxBrowserPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _initObject = e.Parameter as AuxBrowserInitObject;
            if (_initObject == null)
            {
                Frame.GoBack();
                return;
            }
            ((Storyboard)Resources["AuxGridFadeInStoryBoard"]).Begin();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _initObject = null;
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            base.OnNavigatedFrom(e);
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            BackAppBarButton_Click(sender, null);
            e.Handled = true;
        }

        private void BackAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (WebViewAux.CanGoBack) WebViewAux.GoBack();
            else
            {
                HomeAppBarButton1_Click(null, null);
            }
        }

        private void HomeAppBarButton1_Click(object sender, RoutedEventArgs e)
        {
            ((Storyboard)Resources["AuxGridFadeOutStoryBoard"]).Begin();
        }

        private async void WebViewAux_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
        }

        private async void WebViewAux_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
        }

        /// <summary>
        /// Callback executed when the AuxGridFadeOutStoryboards ends
        /// </summary>
        private async void AuxGridFadeOutStoryBoard_Completed(object sender, object e)
        {
            if (Dispatcher.HasThreadAccess)
            {
                await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
                AuxBrowserGrid.Opacity = BottomAppBar.Opacity = 0;
                BottomAppBar.IsEnabled = false;
                if (Frame.CanGoBack) Frame.GoBack();
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    AuxGridFadeOutStoryBoard_Completed(sender, e);
                });
            }
        }

        private async void AuxGridFadeInStoryBoard_OnCompleted(object sender, object e)
        {
            if (Dispatcher.HasThreadAccess)
            {
                AuxBrowserGrid.Opacity = BottomAppBar.Opacity = 1;
                BottomAppBar.IsEnabled = true;
                AuxBrowserTitleBar.Text = (!String.IsNullOrWhiteSpace(_initObject.TitleBar)) ? _initObject.TitleBar : "Title";
                WebViewAux.Navigate(new Uri(_initObject.URL));
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    AuxGridFadeInStoryBoard_OnCompleted(sender, e);
                });
            }
        }
    }
}