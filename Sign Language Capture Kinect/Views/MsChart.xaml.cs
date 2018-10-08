using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Caliburn.Micro;
using System.Collections.Specialized;
using DbModel.Extensions;

namespace Sign_Language_Capture_Kinnect.Views
{
    /// <summary>
    /// Interaction logic for MsChart.xaml
    /// </summary>
    public partial class MsChart : UserControl
    {
        public MsChart()
        {
            InitializeComponent();
            SeriesCollection = new BindableCollection<Series>();

        }

        public static DependencyProperty XValueTypeProperty = DependencyProperty.Register("XValueType", typeof(string),
            typeof(MsChart), new FrameworkPropertyMetadata("Double", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string XValueType
        {
            get { return (string)GetValue(XValueTypeProperty); }
            set { SetValue(XValueTypeProperty, value); }
        }

        public static DependencyProperty XLabelProperty = DependencyProperty.Register("XLabel", typeof(string),
            typeof(MsChart), new FrameworkPropertyMetadata("Item Categories", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string XLabel
        {
            get { return (string)GetValue(XLabelProperty); }
            set { SetValue(XLabelProperty, value); }
        }

        public static DependencyProperty YLabelProperty = DependencyProperty.Register("YLabel", typeof(string),
            typeof(MsChart), new FrameworkPropertyMetadata("Videos Counts", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string YLabel
        {
            get { return (string)GetValue(YLabelProperty); }
            set { SetValue(YLabelProperty, value); }
        }

        public static DependencyProperty Y2LabelProperty = DependencyProperty.Register("Y2Label", typeof(string),
           typeof(MsChart), new FrameworkPropertyMetadata("Y2 Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Y2Label
        {
            get { return (string)GetValue(Y2LabelProperty); }
            set { SetValue(Y2LabelProperty, value); }
        }

        public static DependencyProperty IsArea3DProperty = DependencyProperty.Register("IsArea3D", typeof(bool),
        typeof(MsChart), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsArea3D
        {
            get { return (bool)GetValue(IsArea3DProperty); }
            set { SetValue(IsArea3DProperty, value); }
        }

        public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string),
            typeof(MsChart), new FrameworkPropertyMetadata("My Title", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static DependencyProperty ChartBackgroundProperty = DependencyProperty.Register("ChartBackground", typeof(ChartBackgroundColor),
           typeof(MsChart), new FrameworkPropertyMetadata(ChartBackgroundColor.Blue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ChartBackgroundColor ChartBackground
        {
            get { return (ChartBackgroundColor)GetValue(ChartBackgroundProperty); }
            set { SetValue(ChartBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SeriesCollectionProperty = DependencyProperty.Register("SeriesCollection",
                typeof(BindableCollection<Series>), typeof(MsChart), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSeriesChanged));

        public BindableCollection<Series> SeriesCollection
        {
            get { return (BindableCollection<Series>)GetValue(SeriesCollectionProperty); }
            set { SetValue(SeriesCollectionProperty, value); }
        }

        private static void OnSeriesChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var ms = sender as MsChart;
            var sc = e.NewValue as BindableCollection<Series>;
            if (sc != null)
                sc.CollectionChanged += ms.sc_CollectionChanged;
        }

        private void sc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SeriesCollection != null)
            {
                CheckCount = 0;
                if (SeriesCollection.Count > 0)
                    CheckCount = SeriesCollection.Count;
            }
        }


        private static DependencyProperty CheckCountProperty = DependencyProperty.Register("CheckCount", typeof(int),
            typeof(MsChart), new FrameworkPropertyMetadata(0, StartChart));

        private int CheckCount
        {
            get { return (int)GetValue(CheckCountProperty); }
            set { SetValue(CheckCountProperty, value); }
        }

        public static void StartChart(object sender, DependencyPropertyChangedEventArgs e)
        {
            var ms = sender as MsChart;
            ms.CheckCount = 1;
            //if (ms.CheckCount > 0)
            //{
            ms.myChart.Visible = true;
            ms.myChart.Series.Clear();
            ms.myChart.Titles.Clear();
            ms.myChart.Legends.Clear();
            ms.myChart.ChartAreas.Clear();

            MSChartHelper.MyChart(ms.myChart, ms.SeriesCollection, ms.Title, ms.XLabel, ms.YLabel, ms.ChartBackground, ms.Y2Label);
            if (ms.myChart.ChartAreas.Count > 0)
                ms.myChart.ChartAreas[0].Area3DStyle.Enable3D = ms.IsArea3D;

            ms.myChart.DataBind();
            //}
            //else
            //    ms.myChart.Visible = false;
        }

        public static DependencyProperty DataSourceProperty = DependencyProperty.Register("DataSource", typeof(object),
          typeof(MsChart), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSourceChanged));

        public object DataSource
        {
            get { return (object)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        private static void OnSourceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var ms = sender as MsChart;
            var sc = e.NewValue as object;
            if (sc != null)
            {
                ms.myChart.DataSource = ms.DataSource;
            }
        }

        public static DependencyProperty Chart1Property = DependencyProperty.Register("Chart1", typeof(Chart),
         typeof(MsChart), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnChartChanged));

        public Chart Chart1
        {
            get { return (Chart)GetValue(Chart1Property); }
            set { SetValue(Chart1Property, value); }
        }

        private static void OnChartChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var ms = sender as MsChart;
            var sc = e.NewValue as Chart;
            if (sc != null)
            {
                ms.myChart = ms.Chart1;
                ChartArea area = new ChartArea();
                MSChartHelper.ChartStyle(ms.Chart1, area, ChartBackgroundColor.Blue);
            }
        }

    }
}
