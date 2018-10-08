using DbModel.Context;
using DbModel.Services.Interfaces;
using DbModel.ViewModel.OptionVM;
using DbModel.ViewModel.WordsVM;
using GalaSoft.MvvmLight.Messaging;
using Sign_Language_Capture_Kinnect.Views.Language;
using Sign_Language_Capture_Kinnect.Views.Membership;
using Sign_Language_Capture_Kinnect.Views.Words;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sign_Language_Capture_Kinnect.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window// MahApps.Metro.Controls.MetroWindow
    {
        IUnitOfWork uow;
        private IOptionService ops;
        public Settings()
        {
            AppConfig con = new AppConfig();
            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            ops = ObjectFactory.GetInstance<IOptionService>();


            AppData = new SettingViewModel(new AppConfig()/*con*/, uow);
            //registerMessenger();
            InitializeComponent();

            DataContext = this;


        }
        public SettingViewModel AppData { get; set; }
        private void registerMessenger()
        {
            //Get Datas After Insert
            Messenger.Default.Register<AppConfig>(this, "InsertedService",// getmsg);
             (message) =>
             {
                 AppData.AllOptions = message;
             });

        }
        private void Language_Click(object sender, RoutedEventArgs e)
        {
            new AllLanguages().ShowDialog();
        }
        private void User_Click(object sender, RoutedEventArgs e)
        {
            new AllMemberships().ShowDialog();
        }

        private void Wordkinnect_Click(object sender, RoutedEventArgs e)
        {
            new AllWords_Kinnect(null).ShowDialog();
        }
        private void NewWord_Click(object sender, RoutedEventArgs e)
        {
            var new_win = new NewWord(null/*new WordsModel()*/, 
                null, 2/*DbModel.DomainClasses.Enum.WordType.Arbitrary_Sentences*/);
            new_win.ShowDialog();
        }
        private void Chart_Click(object sender, RoutedEventArgs e)
        {
            var searchform = new Chart();// SearchPatient(null);
            searchform.ShowDialog();
        }
    }
}
