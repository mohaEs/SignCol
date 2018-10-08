using Caliburn.Micro;
using DbModel.Context;
using DbModel.DomainClasses.Enum;
using DbModel.Services;
using DbModel.Services.Interfaces;
using DbModel.ViewModel.WordsVM;
using GalaSoft.MvvmLight.Messaging;
using Sign_Language_Capture_Kinnect.Views.Kinnect;
//using OxyPlot;
//using OxyPlot.Axes;
//using OxyPlot.Series;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sign_Language_Capture_Kinnect.Views.Words
{
    /// <summary>
    /// Interaction logic for AllVideo_SpecialKinnect.xaml
    /// </summary>
    public partial class AllVideo_SpecialKinnect : Window
    {
        private int wordtyped = 0;
        private int word_id = 0;
        private IWords word { set; get; }
        private IUser user { set; get; }
        private ILanguages language { set; get; }
        private IVideo video { set; get; }
        IUnitOfWork uow;

        private int? ttt;

        private bool buttonRecord = false;
        public AllVideo_SpecialKinnect(/*VideoModel vm, */int wordid, int ty)
        {
            word_id = wordid;
            ttt = ty;

            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            user = ObjectFactory.GetInstance<IUser>();
            language = ObjectFactory.GetInstance<ILanguages>();
            word = ObjectFactory.GetInstance<IWords>();
            video = ObjectFactory.GetInstance<IVideo>();

            videoData = new WordVideoVM(new VideoModel(), 2, wordid, LeapKinnectType.Kinnect, uow);
            registerMessenger();

            InitializeComponent();


            Collection<rItem> ch = new Collection<rItem>();
            List<string> datax = new List<string>();
            List<double> datay = new List<double>();
            switch (ty)
            {
                case 0:
                    {
                        tyn.Content = "Number < 10";
                        List<listcustomechart> chlst = videoData.PerVideoPartCount1(LeapKinnectType.Kinnect);
                        foreach (var item in chlst)
                        {
                            //ch.Add(new rItem { Label = item.wordname, Value1 = item.count });
                            datax.Add(item.wordname);
                            datay.Add(item.count);
                        }
                    }
                    break;
                case 1:
                    {
                        tyn.Content = "Number > 10";
                        List<listcustomechart> chlst = videoData.PerVideoPartCount2(LeapKinnectType.Kinnect);
                        foreach (var item in chlst)
                        {
                            //ch.Add(new rItem { Label = item.wordname, Value1 = item.count });
                            datax.Add(item.wordname);
                            datay.Add(item.count);
                        }
                    }
                    break;
                case 2:
                    {
                        tyn.Content = "Letter";
                        List<listcustomechart> chlst = videoData.PerVideoPartCount3(LeapKinnectType.Kinnect);
                        foreach (var item in chlst)
                        {
                            //ch.Add(new rItem { Label = item.wordname, Value1 = item.count });
                            datax.Add(item.wordname);
                            datay.Add(item.count);
                        }
                    }
                    break;
                case 3:
                    {
                        tyn.Content = "Word by Sign";
                        List<listcustomechart> chlst = videoData.PerVideoPartCount4(LeapKinnectType.Kinnect);
                        foreach (var item in chlst)
                        {
                            //ch.Add(new rItem { Label = item.wordname, Value1 = item.count });
                            datax.Add(item.wordname);
                            datay.Add(item.count);
                        }
                    }
                    break;
                case 4:
                    {
                        tyn.Content = "Word by letters";
                        List<listcustomechart> chlst = videoData.PerVideoPartCount5(LeapKinnectType.Kinnect);
                        foreach (var item in chlst)
                        {
                            //ch.Add(new rItem { Label = item.wordname, Value1 = item.count });
                            datax.Add(item.wordname);
                            datay.Add(item.count);
                        }
                    }
                    break;
                case 5:
                    {
                        tyn.Content = "Sentence by Words";
                        List<listcustomechart> chlst = videoData.PerVideoPartCount6(LeapKinnectType.Kinnect);
                        foreach (var item in chlst)
                        {
                            //ch.Add(new rItem { Label = item.wordname, Value1 = item.count });
                            datax.Add(item.wordname);
                            datay.Add(item.count);
                        }
                    }
                    break;
                case 6:
                    {
                        tyn.Content = "Sentence by Signs";
                        List<listcustomechart> chlst = videoData.PerVideoPartCount7(LeapKinnectType.Kinnect);
                        foreach (var item in chlst)
                        {
                            //ch.Add(new rItem { Label = item.wordname, Value1 = item.count });
                            datax.Add(item.wordname);
                            datay.Add(item.count);
                        }
                    }
                    break;
                case 7:
                    {
                        tyn.Content = "Arbitrary Sentence";
                        List<listcustomechart> chlst = videoData.PerVideoPartCount8(LeapKinnectType.Kinnect);
                        foreach (var item in chlst)
                        {
                            //ch.Add(new rItem { Label = item.wordname, Value1 = item.count });
                            datax.Add(item.wordname);
                            datay.Add(item.count);
                        }
                    }
                    break;
            }
            
            BindableCollection<Series> SeriesCollection = new BindableCollection<Series>();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Column;
            ds["DrawingStyle"] = "Cylinder";
            //ds.Points.DataBindY(data1);
            ds.Points.DataBindXY(datax.ToArray(), datay.ToArray());
            //ds.Points.DataBindXY(datax2, datay2);
            SeriesCollection.Add(ds);
            MsChart chm = new MsChart();
            videoData.charttest = SeriesCollection;


            DataContext = this;
        }
        public WordVideoVM videoData { get; set; }
        private void registerMessenger()
        {
            //Send Selected Row For Edit
            Messenger.Default.Register<VideoModel>(this, "MyNavigationService",// doNavigate);
                (fe) =>
                {
                    if (fe == null)
                        fe = new VideoModel();
                    var addWindow = new NewVideo_Kinect(fe, null, null/*, 2*/);
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
            Messenger.Default.Register<ObservableCollection<VideoModel>>(this, "InsertedService",// getmsg);
             (message) =>
             {
                 videoData.AllVideos = message;
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
                VideoModel fea = video.GetVideoById(int.Parse(b.CommandParameter.ToString()));
                DbModel.DomainClasses.Entities.Words wor = word.GetWordsEntityById(fea.word_id.Value);

                var addWindow = new KinnectDevice(fea, fea.User, wor, ttt);
                addWindow.Show();
                this.Close();
            }
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            VideoModel fea = video.GetVideoByWord(word_id, 1);
            DbModel.DomainClasses.Entities.Words wor = word.GetWordsEntityById(word_id);
            var addWindow = new KinnectDevice(new VideoModel(), null, wor, ttt);
            addWindow.Show();
            buttonRecord = true;
            this.Close();

        }

        private void Chart_Click(object sender, RoutedEventArgs e)
        {
            var searchform = new Chart();// SearchPatient(null);
            searchform.ShowDialog();
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
            Words_ViewModel mainViewModel = (Words_ViewModel)DataContext;
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            registerMessenger();
            //AllVideo_SpecialKinnect f = new AllVideo_SpecialKinnect(thisword.word_id, ttt.Value);
            if (buttonRecord)
                { }
                else
            {
                new AllWords_Kinnect(ttt).Show();
                //this.Close();
            }
            //f.Show();
        }
    }
}