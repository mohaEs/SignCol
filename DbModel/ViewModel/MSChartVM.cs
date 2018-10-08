using System;
using Caliburn.Micro;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;

namespace DbModel.ViewModel
{
    public class MSChartVM : PropertyChangedBase
    {
        public BindableCollection<Series> BarSeriesCollection { get; set; }
        //public BindableCollection<Series> LineSeriesCollection { get; set; }
        //public BindableCollection<Series> PieSeriesCollection { get; set; }
        //public BindableCollection<Series> PolarSeriesCollection { get; set; }

        public MSChartVM()
        {
            BarSeriesCollection = new BindableCollection<Series>();
            //LineSeriesCollection = new BindableCollection<Series>();
            //PieSeriesCollection = new BindableCollection<Series>();
            //PolarSeriesCollection = new BindableCollection<Series>();
        }

        public void BarChart(List<double> chartvaluse)
        {
            double[] data1 = chartvaluse.ToArray();//*/ new double[] { 32, 56, 35, 12, 35, 6, 23 };
            //double[] data2 = new double[] { 67, 24, 12, 8, 46, 14, 76 };

            BarSeriesCollection.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Column;
            ds["DrawingStyle"] = "Cylinder";
            ds.Points.DataBindY(data1);
            BarSeriesCollection.Add(ds);
            //return BarSeriesCollection;

            //ds = new Series();
            //ds.ChartType = SeriesChartType.Column;
            //ds["DrawingStyle"] = "Cylinder";
            //ds.Points.DataBindY(data2);
            //BarSeriesCollection.Add(ds);
        }

        /*public void LineChart()
        {
            LineSeriesCollection.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.BorderDashStyle = ChartDashStyle.Solid;
            ds.MarkerStyle = MarkerStyle.Diamond;
            ds.MarkerSize = 8;
            ds.BorderWidth = 2;
            ds.Name = "Sine";
            for (int i = 0; i < 70; i++)
            {
                double x = i / 5.0;
                double y = 1.1 * Math.Sin(x);
                ds.Points.AddXY(x, y);
            }
            LineSeriesCollection.Add(ds);

            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.BorderDashStyle = ChartDashStyle.Dash;
            ds.MarkerStyle = MarkerStyle.Circle;
            ds.MarkerSize = 8;
            ds.BorderWidth = 2;
            ds.Name = "Cosine";
            for (int i = 0; i < 70; i++)
            {
                double x = i / 5.0;
                double y = 1.1 * Math.Cos(x);
                ds.Points.AddXY(x, y);
            }
            LineSeriesCollection.Add(ds);
        }

        public void PieChart()
        {
            PieSeriesCollection.Clear();
            Random random = new Random();
            Series ds = new Series();
            for (int i = 0; i < 5; i++)
                ds.Points.AddY(random.Next(10, 50));
            ds.ChartType = SeriesChartType.Pie;
            ds["PointWidth"] = "0.5";
            ds.IsValueShownAsLabel = true;
            ds["BarLabelStyle"] = "Center";
            ds["DrawingStyle"] = "Cylinder";
            PieSeriesCollection.Add(ds);
        }

        public void PolarChart()
        {
            PolarSeriesCollection.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Polar;
            ds.BorderWidth = 2;
            for (int i = 0; i < 360; i++)
            {
                double x = 1.0 * i;
                double y = 0.001 + Math.Abs(Math.Sin(2.0 * x * Math.PI / 180.0) * Math.Cos(2.0 * x * Math.PI / 180.0));
                ds.Points.AddXY(x, y);
            }
            PolarSeriesCollection.Add(ds);

        }*/
    }
}
