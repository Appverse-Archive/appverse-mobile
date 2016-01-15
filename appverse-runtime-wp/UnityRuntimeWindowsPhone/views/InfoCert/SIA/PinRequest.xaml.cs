using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsPhoneSDKLibs.Src.interfaces;
using WindowsPhoneSDKLibs.Src.manager.operations;

// Il modello di elemento per la pagina vuota è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkId=391641

namespace WindowsPhoneSDKLibs
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class PinRequest : Page
    {
        private static PinRequestCallback pinRequestCallback;
        private string pinRequestPassword;

        #region ViewModelProperty: Background
        private Brush _background;
        public Brush Background
        {
            get
            {
                return _background;
            }

            set
            {
                _background = value;
            }
        }
        #endregion


        public PinRequest()
        {
            this.InitializeComponent();

            //#if INFOCERT
			/*
            this.Background = pinRequest.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 74, 140));
            pinRequest.Background = this.btnOK.Foreground = new SolidColorBrush(Colors.White);
            this.btnOK.Background = new SolidColorBrush(Color.FromArgb(255, 250, 132, 0));
			*/
            //#end if

            this.NavigationCacheMode = NavigationCacheMode.Required;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Dispatcher.HasThreadAccess)
            {
                if (Frame.CanGoBack) Frame.GoBack();
                e.Handled = true;
            }
            else Dispatcher.RunAsync(CoreDispatcherPriority.High, () => { HardwareButtons_BackPressed(sender, e); });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        /// <summary>
        /// Richiamato quando la pagina sta per essere visualizzata in un Frame.
        /// </summary>
        /// <param name="e">Dati dell'evento in cui vengono descritte le modalità con cui la pagina è stata raggiunta.
        /// Questo parametro viene in genere utilizzato per configurare la pagina.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: preparare la pagina da visualizzare qui.
            pinRequest.Text = "";

            // TODO: se l'applicazione contiene più pagine, assicurarsi che si stia
            // gestendo il pulsante Indietro dell'hardware effettuando la registrazione per
            // l'evento Windows.Phone.UI.Input.HardwareButtons.BackPressed.
            // Se si utilizza l'elemento NavigationHelper fornito da alcuni modelli,
            // questo evento viene gestito automaticamente.
        }

        public static void setPinRequestCallback(PinRequestCallback value)
        {
            pinRequestCallback = value;
        }

        public string getPinRequestText()
        {
            return pinRequest.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (pinRequestPassword == null || pinRequestPassword.Length < 6)
            {
                userAdvise.Text = "Pin length is not valid.";
            }

            if (pinRequestCallback != null)
            {
                if (Frame.CanGoBack) Frame.GoBack();
                pinRequestCallback.onPinRequest(pinRequestPassword);
            }
        }

        private void pinRequest_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            pinRequestPassword = pinRequest.Text;

            //replace text by *
            pinRequest.Text = Regex.Replace(pinRequestPassword, @".", "●");

            //take cursor to end of string
            pinRequest.SelectionStart = pinRequest.Text.Length;
        }
    }
}
