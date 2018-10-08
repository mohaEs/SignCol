using DbModel.Context;
using DbModel.Services.Interfaces;
using DbModel.ViewModel.WordsVM;
using DbModel.ViewModel.UserVM;
using Sign_Language_Capture_Kinnect.Views.Membership;

using Caliburn.Micro;
using System.Windows.Forms.DataVisualization.Charting;
using DbModel.Extensions;
using DbModel.ViewModel;
using DbModel.DomainClasses.Enum;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sign_Language_Capture_Kinnect.Views.Words
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : Window//MahApps.Metro.Controls.MetroWindow
    {
        IUnitOfWork uow;
        private IWords word { set; get; }
        public Collection<Item> Items { get; set; }
        public PlotModel Model1 { get; set; }
        public Words_ViewModel wordData { get; set; }

        public Chart()
        {
            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            word = ObjectFactory.GetInstance<IWords>();
            InitializeComponent();


            /// for item categoris -----------
            ChartData = new ChartVM(uow);
            
            // Create some data
            this.Items = new Collection<Item>
                            {
                                new Item {Label = "Cat1", Value1 = ChartData.wt1()},
                                new Item {Label = "Cat2", Value1 = ChartData.wt2()},
                                new Item {Label = "Cat3", Value1 = ChartData.wt3()},
                                new Item {Label = "Cat4", Value1 = ChartData.wt4()},
                                new Item {Label = "Cat5", Value1 = ChartData.wt5()},
                                new Item {Label = "Cat6", Value1 = ChartData.wt6()},
                                new Item {Label = "Cat7", Value1 = ChartData.wt7()},
                                new Item {Label = "Cat8", Value1 = ChartData.wt8()}

                            };


            // Create the plot model
            var tmp = new PlotModel { LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter, LegendOrientation = LegendOrientation.Vertical };

            // Add the axes, note that MinimumPadding and AbsoluteMinimum should be set on the value axis.
            tmp.Axes.Add(new CategoryAxis { Position = AxisPosition.Left, ItemsSource = this.Items, LabelField = "Label" });
            tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, MinimumPadding = 0, AbsoluteMinimum = 0 });

            // Add the series, note that the BarSeries are using the same ItemsSource as the CategoryAxis.
            //tmp.Series.Add(new BarSeries { Title = "2009", ItemsSource = this.Items, ValueField = "Value1" });
            tmp.Series.Add(new ColumnSeries { ItemsSource = this.Items, ValueField = "Value1" });

            this.Model1 = tmp;
            this.DataContext = this;

            /// end for item captories  -----------
            /// for video capturing  -----------

            wordData = new Words_ViewModel(new WordsModel(),2, uow);
            string[] datax = new string[] { "Cat1", "Cat2", "Cat3", "Cat4", "Cat5", "Cat6", "Cat7", "Cat8" };
            double[] datay = new double[] { wordData.VideoCount1(LeapKinnectType.Kinnect), wordData.VideoCount2(LeapKinnectType.Kinnect),
                wordData.VideoCount3(LeapKinnectType.Kinnect), wordData.VideoCount4(LeapKinnectType.Kinnect),
                wordData.VideoCount5(LeapKinnectType.Kinnect), wordData.VideoCount6(LeapKinnectType.Kinnect),
                wordData.VideoCount7(LeapKinnectType.Kinnect), wordData.VideoCount8(LeapKinnectType.Kinnect) };

            BindableCollection<System.Windows.Forms.DataVisualization.Charting.Series> SeriesCollection = new BindableCollection<System.Windows.Forms.DataVisualization.Charting.Series>();
            System.Windows.Forms.DataVisualization.Charting.Series ds = new System.Windows.Forms.DataVisualization.Charting.Series();
            ds.ChartType = SeriesChartType.Column;
            ds["DrawingStyle"] = "Cylinder";
            //ds.Points.DataBindY(data1);
            ds.Points.DataBindXY(datax, datay);
            SeriesCollection.Add(ds);
            MsChart chm = new MsChart();
            chart1.SeriesCollection = SeriesCollection;// nn.BarSeriesCollection;
            chart1.Title = "Capturing Status by Kinect";
            MsChart.StartChart(chart1, new DependencyPropertyChangedEventArgs());

            ///end for video capturing  -----------
        }

        //public Words_ViewModel wordData { get; set; }
        public ChartVM ChartData { get; set; }
    }
    public class Item
    {
        public string Label { get; set; }
        public double Value1 { get; set; }

    }
}
