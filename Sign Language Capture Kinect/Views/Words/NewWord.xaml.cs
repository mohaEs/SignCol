using DbModel.Context;
using DbModel.ViewModel.WordsVM;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DbModel.DomainClasses.Entities;
using DbModel.DomainClasses.Enum;
using DbModel.ViewModel;
using System.Linq;
using DbModel.Extensions;
using System.Windows.Controls;

namespace Sign_Language_Capture_Kinnect.Views.Words
{
    /// <summary>
    /// Interaction logic for NewWord.xaml
    /// </summary>
    public partial class NewWord : Window//MahApps.Metro.Controls.MetroWindow
    {
        private string userId;
        IUnitOfWork uow;
        private WordType? wrt;
        //private int tty;

        public NewWord(WordsModel word, WordType? wt/*, User user*/, int ty)
        {
            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            registerMessenger();
            InitializeComponent();

            wrt = wt;
            //tty = ty;

            if (word != null)
            {
                ListItems listitem = new ListItems();
                if(wt.HasValue)
                    wordData = new Words_ViewModel(word, wt, ty, uow);
                else
                    wordData = new Words_ViewModel(word, ty, uow);

                DropDownItems wtl = listitem.GetWordType().FirstOrDefault(x => x.ID == UtilityClass.WordTypeToInt(wordData.wordType)/*(int)model.WordType*/);
                WordType.SelectedIndex = wtl.ID.Value;

                ObservableCollection<DropDownItems> lanlist = wordData.GetAllLanguages();
                DropDownItems ltl = lanlist.FirstOrDefault(x => x.ID == wordData.Langid);
                Language.ItemsSource = lanlist;
                for(int i=0;i<lanlist.Count;i++)
                {
                    if (lanlist[i].ID == ltl.ID)
                    {
                        Language.SelectedIndex = i;
                        wordData.SelectedLanguage = ltl;
                    }
                }

            }
            else
            {
                wordData = new Words_ViewModel(new WordsModel(), wt, 1, uow);
   
            }

            DataContext = this;

            
        }
        public Words_ViewModel wordData { get; set; }

        private void registerMessenger()
        {
            //Get & Send Search String For PatientForm
            Messenger.Default.Register<ObservableCollection<WordsModel>>(this, "InsertedService", doNavigate);
        }

        private void doNavigate(ObservableCollection<WordsModel> alldata)
        {
            this.Close();
        }

        /*private void registerMessenger()
        {
            //Get Search String From Search Form
            Messenger.Default.Register<User>(this, "SearchuserientService",
             (message) =>
             {
                 if (message != null)
                 {
                     //User_id.Content = "User ID = " + message.User_id.ToString() + ", Name = " + message.Name;
                     //userId = message.User_id.ToString();
                     //wordData.euser = message;
                     //wordData.User_id = message.User_id;
                 }
             });
            //Get Inserted
            Messenger.Default.Register<ObservableCollection<WordsModel>>(this, "InsertedService",
             (message) =>
             {
                 wordData.AllWords = message;
                 this.Close();
             });
        }*/
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //registerMessenger();
        }
    }
}
