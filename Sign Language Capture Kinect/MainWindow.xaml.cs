using Sign_Language_Capture_Kinnect.Pages;
using Sign_Language_Capture_Kinnect.Views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Data.Entity.Validation;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sign_Language_Capture_Kinnect.Views.Words;
using Sign_Language_Capture_Kinnect.Views.Membership;
using Sign_Language_Capture_Kinnect.Views.Language;
using DbModel.Context;
using DbModel.Services.Interfaces;
using StructureMap;
using DbModel.ViewModel.WordsVM;
using DbModel.Extensions;
using System.Windows.Input;
using DbModel.ViewModel.LanguageVM;

namespace Sign_Language_Capture_Kinnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window// MetroWindow
    {
        private ILanguages language;
        IUnitOfWork uow;
        public MainWindow()
        {
            //uow = ObjectFactory.GetInstance<IUnitOfWork>();
            //language = ObjectFactory.GetInstance<ILanguages>();

            //languageData = new LanguageViewModel(new LanguageModel(), uow);

            InitializeComponent();
            
            DataContext = this;

            //ImageBrush myBrush = new ImageBrush();
            //myBrush.ImageSource =
            //    new BitmapImage(new Uri("pack://siteoforigin:,,,/home_background.jpg", UriKind.Absolute));
            //this.Background = myBrush;
        }
        //public LanguageViewModel languageData { get; set; }

        private void RectSelectSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            new Settings().ShowDialog();
            // MessageBox.Show("sasda");
            //WindowSettings winSetting = new WindowSettings();
            //winSetting.Visibility = System.Windows.Visibility.Visible;
        }

        private void textBlockSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            new Settings().ShowDialog();
        }


        private void RectKinectCapture_MouseUp(object sender, MouseButtonEventArgs e)
        {
            new Kinnect().ShowDialog();
            //new AllWords_Kinnect().ShowDialog();
            //WindowCaptureKinect WindowCapture = new WindowCaptureKinect();
            //WindowCapture.Visibility = System.Windows.Visibility.Visible;
        }
        //private void RectLeapCapture_MouseUp(object sender, MouseButtonEventArgs e)
        //{
   //         new Leap().ShowDialog();
            //new AllWords_Leap().ShowDialog();
            //WindowCaptureKinect WindowCapture = new WindowCaptureKinect();
            //WindowCapture.Visibility = System.Windows.Visibility.Visible;
        //}

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }



        private void Word_Click(object sender, RoutedEventArgs e)
        {
            //new AllWords().ShowDialog();
        }
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            new Settings().ShowDialog();
        }


        private CustomDialog _customDialog;
        //private Dialogs.LoginDialog _loginwindow;
        private async void Window1_OnLoaded(object sender, RoutedEventArgs e)
        {
            //MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            //_customDialog = new CustomDialog();
            //var mySettings = new MetroDialogSettings()
            //{
            //    AffirmativeButtonText = "OK",
            //    AnimateShow = true,
            //    NegativeButtonText = "Go away!",
            //    FirstAuxiliaryButtonText = "Cancel",
            //};
            //_loginwindow = new Dialogs.LoginDialog();
            //_loginwindow.ButtonCancel.Click += ButtonCancelOnClick;
            //_loginwindow.ButtonLogin.Click += ButtonLoginOnClick;
            //_customDialog.Content = _loginwindow;
            //await this.ShowMetroDialogAsync(_customDialog);
        }
        private void ButtonLoginOnClick(object sender, RoutedEventArgs e)
        {
            //if (_loginwindow.TextBoxUserName.Text == "admin" && _loginwindow.PasswordBox1.Password == "admin")
            //{
            //    this.HideMetroDialogAsync(_customDialog);
            //}
            //else
            //{
            //    MessageBox.Show("Invallid Username or Password");
            //}
        }
        private void ButtonCancelOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
