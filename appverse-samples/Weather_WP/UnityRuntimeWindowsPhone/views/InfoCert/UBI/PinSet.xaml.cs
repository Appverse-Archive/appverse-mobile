using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsPhoneSDKLibs.Src.manager.operations;
using WindowsPhoneSDKLibs.Src.interfaces;
using Windows.Phone.UI.Input;
using System.Text.RegularExpressions;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;

// Il modello di elemento per la pagina vuota è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkID=390556

namespace WindowsPhoneSDKLibs
{
    /// <summary>
    /// Pagina vuota che può essere utilizzata autonomamente oppure esplorata all'interno di un frame.
    /// </summary>
    public sealed partial class PinSet : Page
    {
        private static PinSetCallback pinSetCallback;
        private string pinSetPassword;
        private string pinSetConfirmPassword;
        
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

        public PinSet()
        {
            this.InitializeComponent();

            /*/#if INFOCERT            
            Background = new SolidColorBrush(Color.FromArgb(255, 0, 74, 140));
            this.Background = new SolidColorBrush(Color.FromArgb(255, 0, 74, 140));
            pinSet.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 74, 140));
            pinSetConfirm.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 74, 140));
            pinSet.Background = new SolidColorBrush(Colors.White);
            pinSetConfirm.Background = new SolidColorBrush(Colors.White);
            this.btnOK.Foreground = new SolidColorBrush(Colors.White);
            this.btnOK.Background = new SolidColorBrush(Color.FromArgb(255, 250, 132, 0));
            //#endif*/

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
            pinSet.Text = "";
            pinSetConfirm.Text = "";
        }

        public static void setPinSetCallback(PinSetCallback value)
        {
            pinSetCallback = value;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (pinSetPassword == null || pinSetPassword.Length < 1)
            {
                userAdvise.Text = "Il campo pin non può essere vuoto.";// "Pin field can not be empty.";
            }
            else if (pinSetPassword.Equals(pinSetConfirmPassword))
            {
                if (Frame.CanGoBack) Frame.GoBack();
                pinSetCallback.onPinSet(pinSetPassword);
            }
            else if (pinSetPassword.Length < 6 || pinSetConfirmPassword.Length < 6)
            {
                userAdvise.Text = "Lunghezza del pin non valida.";// "Pin length is not valid.";
            }
            else
            {
                userAdvise.Text = "I pin non corrispondono.";// "Pins don't match";
            }
        }

        private void pinSet_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            pinSetPassword = pinSet.Text;

            //replace text by *
            pinSet.Text = Regex.Replace(pinSetPassword, @".", "●");

            //take cursor to end of string
            pinSet.SelectionStart = pinSet.Text.Length;
        }

        private void pinSetConfirm_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            pinSetConfirmPassword = pinSetConfirm.Text;

            //replace text by *
            pinSetConfirm.Text = Regex.Replace(pinSetConfirmPassword, @".", "●");

            //take cursor to end of string
            pinSetConfirm.SelectionStart = pinSetConfirm.Text.Length;
        }
    }
}
