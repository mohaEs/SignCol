using DbModel.Context;
using DbModel.ViewModel.UserVM;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sign_Language_Capture_Kinnect.Views.Membership
{
    /// <summary>
    /// Interaction logic for NewMembership.xaml
    /// </summary>
    public partial class NewMembership : Window// MahApps.Metro.Controls.MetroWindow
    {
        IUnitOfWork uow;
        public NewMembership(UserModel user)
        {
            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            registerMessenger();
            InitializeComponent();
            if (user != null)
            {
                userData = new User_ViewModel(user, uow);
            }
            else
            {
                userData = new User_ViewModel(new UserModel(), uow);
            }
            DataContext = this;


        }
        public User_ViewModel userData { get; set; }
        private void registerMessenger()
        {
            //Get & Send Search String For PatientForm
            Messenger.Default.Register<ObservableCollection<UserModel>>(this, "InsertedService", doNavigate);
        }

        private void doNavigate(ObservableCollection<UserModel> alldata)
        {
            userData.AllUser = alldata;
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            registerMessenger();
            AllMemberships f = new AllMemberships();
            f.Show();

            //this.Close();
        }
    }
}
