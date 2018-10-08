using DbModel.Context;
using DbModel.Services.Interfaces;
using DbModel.ViewModel.WordsVM;
using DbModel.ViewModel.UserVM;
using Sign_Language_Capture_Kinnect.Views.Membership;

using GalaSoft.MvvmLight.Messaging;
using OxyPlot;
using OxyPlot.Axes;
//using OxyPlot.Series;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using Caliburn.Micro;
using System.Windows.Forms.DataVisualization.Charting;
using DbModel.Extensions;
using DbModel.ViewModel;
using DbModel.DomainClasses.Enum;

namespace Sign_Language_Capture_Kinnect.Views.Words
{
    /// <summary>
    /// Interaction logic for Kinnect.xaml
    /// </summary>

    public partial class Kinnect : Window
    {
        private IWords word { set; get; }
        private IUser user { set; get; }
        private ILanguages language { set; get; }
        private IVideo video { set; get; }
        IUnitOfWork uow;
        public Kinnect()
        {
            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            user = ObjectFactory.GetInstance<IUser>();
            language = ObjectFactory.GetInstance<ILanguages>();
            word = ObjectFactory.GetInstance<IWords>();
            video = ObjectFactory.GetInstance<IVideo>();

            wordData = new Words_ViewModel(new WordsModel(), 2, uow);
            registerMessenger();

            InitializeComponent();

            //ImageBrush myBrush = new ImageBrush();
            //myBrush.ImageSource =
            //    new BitmapImage(new Uri("pack://siteoforigin:,,,/back3.jpg", UriKind.Absolute));
            //this.Background = myBrush;

            //wordData.charttest = wordData.PerVideoPartCount();
            //MessageBox.Show(wordData.charttest[0].ToString());

            string[] datax = new string[] { "Cat1", "Cat2", "Cat3", "Cat4", "Cat5", "Cat6", "Cat7", "Cat8" };
            //double[] datay = new double[] { wordData.WordCount1(), wordData.WordCount2()
            //    , wordData.WordCount3(),wordData.WordCount4(),
            //    wordData.WordCount5(), wordData.WordCount6(),
            //    wordData.WordCount7(), wordData.WordCount8() };
            double[] datay = new double[] { wordData.VideoCount1(LeapKinnectType.Kinnect), wordData.VideoCount2(LeapKinnectType.Kinnect),
                wordData.VideoCount3(LeapKinnectType.Kinnect), wordData.VideoCount4(LeapKinnectType.Kinnect),
                wordData.VideoCount5(LeapKinnectType.Kinnect), wordData.VideoCount6(LeapKinnectType.Kinnect),
                wordData.VideoCount7(LeapKinnectType.Kinnect), wordData.VideoCount8(LeapKinnectType.Kinnect) };
            //double[] datay = new double[] { 32, 56, 35, 12, 35, 6, 23, 56 };
            BindableCollection<Series> SeriesCollection = new BindableCollection<Series>();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Column;
            ds["DrawingStyle"] = "Cylinder";
            //ds.Points.DataBindY(data1);
            ds.Points.DataBindXY(datax, datay);
            SeriesCollection.Add(ds);
            MsChart chm = new MsChart();
            wordData.charttest = SeriesCollection;
            chart1.SeriesCollection = SeriesCollection;// nn.BarSeriesCollection;
            chart1.Title = "Capturing Status by Kinect";           
            MsChart.StartChart(chart1, new DependencyPropertyChangedEventArgs());



            DataContext = this;
        }
        public Words_ViewModel wordData { get; set; }
        private void registerMessenger()
        {

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
            Messenger.Default.Register<ObservableCollection<WordsModel>>(this, "InsertedService",// getmsg);
             (message) =>
             {
                 wordData.AllWords = message;
             });
        }
        private void PART_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void Add_KinnectVideo_Click(object sender, RoutedEventArgs e)
        {
            /*Button b = (sender) as Button;
            if (b.CommandParameter != null)
            {
                VideoModel fea = video.GetVideoByWord(int.Parse(b.CommandParameter.ToString()), 1);
                //WordsModel wor = word.GetWordsById(int.Parse(b.CommandParameter.ToString()));
                DbModel.DomainClasses.Entities.Words wor = word.GetWordsEntityById(int.Parse(b.CommandParameter.ToString()));
                if (fea == null)
                {
                    fea = new VideoModel();
                    fea.Words = wor;
                    var addWindow = new NewVideo_Kinect(fea, null);
                    addWindow.ShowDialog();
                }
                else
                {
                    fea.Words = wor;
                    var addWindow = new NewVideo_Kinect(fea, fea.User);
                    addWindow.ShowDialog();
                }
            }*/
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {
            // var new_win = new NewWord(new WordsModel()/*, null*/);
            // new_win.ShowDialog();

            var new_win = new NewMembership(new UserModel());
            new_win.ShowDialog();
        }
        private void Chart_Click(object sender, RoutedEventArgs e)
        {
            var searchform = new Chart();// SearchPatient(null);
            searchform.ShowDialog();
        }



        private void number_big10_MouseUp(object sender, MouseButtonEventArgs e)
        {
            new AllWords_Kinnect(0).Show();
            this.Close();
        }
        private void number_little10_MouseUp(object sender, MouseButtonEventArgs e)
        {
            new AllWords_Kinnect(1).Show();
            this.Close();
        }
        private void Horoof_MouseUp(object sender, MouseButtonEventArgs e)
        {
            new AllWords_Kinnect(2).Show();
            this.Close();
        }
        private void words_by_eshare(object sender, MouseButtonEventArgs e)
        {
            new AllWords_Kinnect(3).Show();
            this.Close();
        }
        private void words_by_harf(object sender, MouseButtonEventArgs e)
        {
            new AllWords_Kinnect(4).Show();
            this.Close();
        }
        private void Sentences_by_Letters(object sender, MouseButtonEventArgs e)
        {
            new AllWords_Kinnect(5).Show();
            this.Close();
        }
        private void Sentences_by_Signs(object sender, MouseButtonEventArgs e)
        {
            new AllWords_Kinnect(6).Show();
            this.Close();
        }
        private void Custome_statement(object sender, MouseButtonEventArgs e)
        {
            new AllWords_Kinnect(7).Show();
            this.Close();
        }




    }
}
