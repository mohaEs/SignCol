using Caliburn.Micro;
using DbModel.Context;
using DbModel.DomainClasses.Enum;
using DbModel.Extensions;
using DbModel.Services;
using DbModel.Services.Interfaces;
using DbModel.ViewModel.WordsVM;
using GalaSoft.MvvmLight.Messaging;
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
    /// Interaction logic for AllWords_Kinnect.xaml
    /// </summary>
    public partial class AllWords_Kinnect : Window// MahApps.Metro.Controls.MetroWindow
    {
        //private int wordtyped = 0;
        private IWords word { set; get; }
        private IUser user { set; get; }
        private ILanguages language { set; get; }
        private IVideo video { set; get; }
        IUnitOfWork uow;
        public int? ttt;

        private WordType wt;

        private bool clickVideos = false;
        public AllWords_Kinnect(int? ty)
        {
            //wordtyped = ty; 
             uow = ObjectFactory.GetInstance<IUnitOfWork>();
            user = ObjectFactory.GetInstance<IUser>();
            language = ObjectFactory.GetInstance<ILanguages>();
            word = ObjectFactory.GetInstance<IWords>();
            video = ObjectFactory.GetInstance<IVideo>();

            registerMessenger();


            if (ty.HasValue)
            {
                wt = UtilityClass.IntToWordType(ty.Value);
                wordData = new Words_ViewModel(new WordsModel(), wt, 1, uow);
                wordData.wt = wt;

                ttt = 1;
            }
            else
            {
                wordData = new Words_ViewModel(new WordsModel(), 2, uow);
                ttt = 2;
            }
            //MessageBox.Show(wordData.TT.ToString());

            InitializeComponent();


            DataContext = this;
        }
        public Words_ViewModel wordData { get; set; }



        private void registerMessenger()
        {


            //Get Search String From Search Form
            Messenger.Default.Register<ObservableCollection<WordsModel>>(this, "MySearchNavigationService",// getmsg);
             (message) =>
             {
                 wordData.AllWords = message;
             });

            //Get Datas After Insert
            Messenger.Default.Register<ObservableCollection<WordsModel>>(this, "InsertedService",// getmsg);
             (message) =>
             {
                 wordData.AllWords = message;
                 
                 //this.Close();
             });
        }
        private void PART_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        // Detail Button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (sender) as Button;
            if (b.CommandParameter != null)
            {
                WordsModel fea = word.GetWordsById(int.Parse(b.CommandParameter.ToString()));
                //if (ttt.HasValue)
                //{
                    var addWindow = new NewWord(fea, fea.WordType, ttt.Value/*, 1/*UtilityClass.IntToWordType(wordtyped)*/);
                    //addWindow.ShowDialog();

                    addWindow.Show();


                //}
                //else
                //{
                //    var addWindow = new NewWord(fea, fea.WordType/*, 0/*UtilityClass.IntToWordType(wordtyped)*/);
                //    //addWindow.ShowDialog();

                //    addWindow.Show();

                //}
            }
        }
        // All Video For Special Word
        private void All_KinnectVideos_Click(object sender, RoutedEventArgs e)
        {
            Button b = (sender) as Button;
            if (b.CommandParameter != null)
            {
                VideoModel fea = video.GetVideoByWord(int.Parse(b.CommandParameter.ToString()), 1/*wordtyped*/);
                DbModel.DomainClasses.Entities.Words wor = word.GetWordsEntityById(int.Parse(b.CommandParameter.ToString()));
                if (fea == null)
                {
                    fea = new VideoModel();
                    fea.Words = wor;
                    var addWindow = new AllVideo_SpecialKinnect(/*fea,*/ wor.word_id, UtilityClass.WordTypeToInt(wor.WordType)/*wordtyped*/);
                    
                    addWindow.Show();
                   
                }
                else
                {
                    
                    var addWindow = new AllVideo_SpecialKinnect(/*fea,*/ wor.word_id, UtilityClass.WordTypeToInt(wor.WordType)/*wordtyped*/);
                    
                    addWindow.Show();
                }
            }
            clickVideos = true;
            this.Close();
            
        }
        private void Chart_Click(object sender, RoutedEventArgs e)
        {
            var searchform = new Chart();// SearchPatient(null);
            searchform.ShowDialog();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }







        private DataGridColumn currentSortColumn;
        private ListSortDirection currentSortDirection;
        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
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
        private void DataGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (currentSortColumn != null)
            {
                currentSortColumn.SortDirection = currentSortDirection;
            }
        }
        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
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

    }
}

/*  Collection<rItem> ch = new Collection<rItem>();
  List<string> datax = new List<string>();
  List<double> datay = new List<double>();
  if (ty.HasValue)
  {
      switch (ty.Value)
      {
          case 0:
              {
                  tyn.Content = "Number < 10";
                  List<listcustomechart> chlst = wordData.PerVideoPartCount1(LeapKinnectType.Kinnect);
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
                  List<listcustomechart> chlst = wordData.PerVideoPartCount2(LeapKinnectType.Kinnect);
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
                  List<listcustomechart> chlst = wordData.PerVideoPartCount3(LeapKinnectType.Kinnect);
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
                  List<listcustomechart> chlst = wordData.PerVideoPartCount4(LeapKinnectType.Kinnect);
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
                  List<listcustomechart> chlst = wordData.PerVideoPartCount5(LeapKinnectType.Kinnect);
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
                  List<listcustomechart> chlst = wordData.PerVideoPartCount6(LeapKinnectType.Kinnect);
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
                  List<listcustomechart> chlst = wordData.PerVideoPartCount7(LeapKinnectType.Kinnect);
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
                  List<listcustomechart> chlst = wordData.PerVideoPartCount8(LeapKinnectType.Kinnect);
                  foreach (var item in chlst)
                  {
                      //ch.Add(new rItem { Label = item.wordname, Value1 = item.count });
                      datax.Add(item.wordname);
                      datay.Add(item.count);
                  }
              }
              break;
      }
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
  wordData.charttest = SeriesCollection;*/
