using DbModel.Context;
using DbModel.DomainClasses.Entities;
using DbModel.Services.Interfaces;
using DbModel.ViewModel.UserVM;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Sign_Language_Capture_Kinnect.Views.Membership
{
    /// <summary>
    /// Interaction logic for SearchUser.xaml
    /// </summary>
    public partial class SearchUser : Window// MahApps.Metro.Controls.MetroWindow
    {
        private IUser user;
        IUnitOfWork uow;
        private string des;
        public SearchUser(string destination)
        {
            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            user = ObjectFactory.GetInstance<IUser>();

            userData = new User_ViewModel(new UserModel(), uow);
            registerMessenger();

            InitializeComponent();
            if (!string.IsNullOrEmpty(destination) && destination.Equals("Word"))
                des = "1";
            if (!string.IsNullOrEmpty(destination) && destination.Equals("Kinnect"))
                des = "2";

            DataContext = this;


        }
        public User_ViewModel userData { get; set; }
        private void registerMessenger()
        {
            //Send Selected Row For Edit
            Messenger.Default.Register<UserModel>(this, "MyNavigationService",// doNavigate);
                (fe) =>
                {
                    if (fe == null)
                        fe = new UserModel();
                    var addWindow = new NewMembership(fe);
                    addWindow.ShowDialog();
                });

            //Get Search String From Search Form
            Messenger.Default.Register<string>(this, "MySearchNavigationService",// getmsg);
             (message) =>
             {
                 if (!string.IsNullOrEmpty(message))
                 {
                     //  Name.Text = message;
                 }
             });

            //Get Datas After Insert
            Messenger.Default.Register<ObservableCollection<UserModel>>(this, "InsertedService",// getmsg);
             (message) =>
             {
                 userData.AllUser = message;
             });
        }
        private void Detail_Search_Click(object sender, RoutedEventArgs e)
        {
            if (des.Equals("1"))
            {
                Button b = (sender) as Button;
                if (b.CommandParameter != null)
                {
                    User ppp = user.GetUserEntityById(int.Parse(b.CommandParameter.ToString()));
                    Messenger.Default.Send(ppp, "SearchuserientService");
                    this.Close();
                }
            }
            else if (des.Equals("2"))
            {
                Button b = (sender) as Button;
                if (b.CommandParameter != null)
                {
                    User ppp = user.GetUserEntityById(int.Parse(b.CommandParameter.ToString()));
                    Messenger.Default.Send(ppp, "SearchuserientService");
                    this.Close();
                }
            }

        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            var new_win = new NewMembership(new UserModel());
            new_win.ShowDialog();
        }







        private DataGridColumn currentSortColumn;
        private ListSortDirection currentSortDirection;
        private void ProductsDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            // The current sorted column must be specified in XAML.
            if (dataGrid.HasItems)
            {
                var sortation = dataGrid.Columns.Where(c => c.SortDirection.HasValue).FirstOrDefault();//.Single();
                if (sortation != null)
                {
                    currentSortColumn = sortation;
                    currentSortDirection = currentSortColumn.SortDirection.Value;
                }
            }
        }
        private void ProductsDataGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (currentSortColumn != null)
            {
                currentSortColumn.SortDirection = currentSortDirection;
            }
        }
        private void ProductsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
            User_ViewModel mainViewModel = (User_ViewModel)DataContext;
            string sortField = String.Empty;
            switch (e.Column.SortMemberPath)
            {
                case ("pid"):
                    sortField = "pid";
                    break;
                case ("pdate"):
                    sortField = "pdate";
                    break;
            }

            ListSortDirection direction = (e.Column.SortDirection != ListSortDirection.Ascending) ?
                ListSortDirection.Ascending : ListSortDirection.Descending;

            bool sortAscending = direction == ListSortDirection.Ascending;

            mainViewModel.Sort(sortField, sortAscending);

            currentSortColumn.SortDirection = null;

            e.Column.SortDirection = direction;

            currentSortColumn = e.Column;
            currentSortDirection = direction;
        }

    }
}
