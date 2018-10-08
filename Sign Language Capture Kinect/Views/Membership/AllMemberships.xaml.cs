using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using System.Linq;
using DbModel.Context;
using DbModel.Services.Interfaces;
using DbModel.ViewModel.UserVM;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sign_Language_Capture_Kinnect.Views.Membership
{
    /// <summary>
    /// Interaction logic for AllMemberships.xaml
    /// </summary>
    public partial class AllMemberships : Window// MahApps.Metro.Controls.MetroWindow
    {
        private IUser user;
        IUnitOfWork uow;
        public AllMemberships()
        {
            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            user = ObjectFactory.GetInstance<IUser>();

            userData = new User_ViewModel(new UserModel(), uow);
            registerMessenger();

            InitializeComponent();

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
        private void PART_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Button b = (sender) as Button;
            if (b.CommandParameter != null)
            {
                UserModel ppp = user.GetUserById(int.Parse(b.CommandParameter.ToString()));
                var addWindow = new NewMembership(ppp);
                //addWindow.ShowDialog();

                addWindow.Show();
                this.Close();
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {


            var new_win = new NewMembership(new UserModel());
            new_win.Show();

            this.Close();
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            //var searchform = new SearchPatient(null);
            //searchform.ShowDialog();
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
