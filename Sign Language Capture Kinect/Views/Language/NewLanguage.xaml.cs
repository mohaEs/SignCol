using DbModel.Context;
using DbModel.ViewModel.LanguageVM;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sign_Language_Capture_Kinnect.Views.Language
{
    /// <summary>
    /// Interaction logic for NewLanguage.xaml
    /// </summary>
    public partial class NewLanguage : Window// MahApps.Metro.Controls.MetroWindow
    {
        IUnitOfWork uow;
        public NewLanguage(LanguageModel lang)
        {
            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            registerMessenger();
            InitializeComponent();
            if (lang != null)
            {
                languageData = new LanguageViewModel(lang, uow);
            }
            else
            {
                languageData = new LanguageViewModel(new LanguageModel(), uow);
            }
            DataContext = this;


        }
        public LanguageViewModel languageData { get; set; }
        private void registerMessenger()
        {
            //Get & Send Search String For PatientForm
      //      Messenger.Default.Register<ObservableCollection<LanguageModel>>(this, "InsertedService", doNavigate);


            //Get Datas After Insert
            Messenger.Default.Register<ObservableCollection<LanguageModel>>(this, "InsertedService",// getmsg);
             (message) =>
             {
                 languageData.AllLanguages = message;

                 //registerMessenger();
                 //new AllLanguages().Show();
                 this.Close();
             });
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            registerMessenger();
            AllLanguages f = new AllLanguages();
            f.Show();

        }
    }
}
