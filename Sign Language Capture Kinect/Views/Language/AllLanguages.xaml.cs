using DbModel.Context;
using DbModel.Services.Interfaces;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StructureMap;
using DbModel.ViewModel.LanguageVM;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Sign_Language_Capture_Kinnect.Views.Language
{
    /// <summary>
    /// Interaction logic for AllLanguages.xaml
    /// </summary>
    public partial class AllLanguages : Window// MahApps.Metro.Controls.MetroWindow
    {
        private ILanguages language;
        IUnitOfWork uow;
        public AllLanguages()
        {
            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            language = ObjectFactory.GetInstance<ILanguages>();
            
            languageData = new LanguageViewModel(new LanguageModel(), uow);
            registerMessenger();

            InitializeComponent();

            DataContext = this;


        }
        public LanguageViewModel languageData { get; set; }
        private void registerMessenger()
        {
            //Send Selected Row For Edit
            Messenger.Default.Register<LanguageModel>(this, "MyNavigationService",// doNavigate);
                (fe) =>
                {
                    if (fe == null)
                        fe = new LanguageModel();
                    var addWindow = new NewLanguage(fe);
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
            Messenger.Default.Register<ObservableCollection<LanguageModel>>(this, "InsertedService",// getmsg);
             (message) =>
             {
                 //languageData.RefreshProducts();
                 languageData.AllLanguages = message;

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
                LanguageModel ppp = language.GetLanguagesById(int.Parse(b.CommandParameter.ToString()));
                var addWindow = new NewLanguage(ppp);
                //addWindow.ShowDialog();

                addWindow.Show();
                this.Close();

 
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            var new_win = new NewLanguage(new LanguageModel());
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
            LanguageViewModel mainViewModel = (LanguageViewModel)DataContext;
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
