using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using Caliburn.Micro;


namespace DbModel.Extensions
{
    public static class MSChartHelper
    {
        public static void MyChart(Chart chart1, BindableCollection<Series> chartSeries, string chartTitle, string xLabel, string yLabel, ChartBackgroundColor backgroundColor, params string[] y2Label)
        {
            if (chart1.ChartAreas.Count < 1)
            {
                ChartArea area = new ChartArea();
                ChartStyle(chart1, area, backgroundColor);
            }

            if (chartTitle != "")
                chart1.Titles.Add(chartTitle);
            chart1.ChartAreas[0].AxisX.Title = xLabel;
            chart1.ChartAreas[0].AxisY.Title = yLabel;
            if (y2Label.Length > 0)
                chart1.ChartAreas[0].AxisY2.Title = y2Label[0];

            foreach (var ds in chartSeries)
                chart1.Series.Add(ds);

            if (chartSeries.Count > 1)
            {
                Legend legend = new Legend();
                legend.Font = new System.Drawing.Font("Trebuchet MS", 7.0F, FontStyle.Regular);
                legend.BackColor = Color.Transparent;
                legend.AutoFitMinFontSize = 5;
                legend.LegendStyle = LegendStyle.Column;

                legend.IsDockedInsideChartArea = true;
                legend.Docking = Docking.Left;
                legend.InsideChartArea = chart1.ChartAreas[0].Name;
                chart1.Legends.Add(legend);
            }
        }


        public static void ChartStyle(Chart chart1, ChartArea area, ChartBackgroundColor backgroundColor)
        {
            int r1 = 211;
            int g1 = 223;
            int b1 = 240;
            int r2 = 26;
            int g2 = 59;
            int b2 = 105;
            int r3 = 165;
            int g3 = 191;
            int b3 = 228;

            switch (backgroundColor)
            {
                case ChartBackgroundColor.Blue:
                    chart1.BackColor = Color.FromArgb(r1, g1, b1);
                    chart1.BorderlineColor = Color.FromArgb(r2, g2, b2);
                    area.BackColor = Color.FromArgb(64, r3, g3, b3);
                    break;
                case ChartBackgroundColor.Green:
                    chart1.BackColor = Color.FromArgb(g1, b1, r1);
                    chart1.BorderlineColor = Color.FromArgb(g2, b2, r2);
                    area.BackColor = Color.FromArgb(64, g3, b3, r3);
                    break;
                case ChartBackgroundColor.Red:
                    chart1.BackColor = Color.FromArgb(b1, r1, g1);
                    chart1.BorderlineColor = Color.FromArgb(b2, r2, g2);
                    area.BackColor = Color.FromArgb(64, b3, r3, g3);
                    break;
                case ChartBackgroundColor.White:
                    chart1.BackColor = Color.White;
                    chart1.BorderlineColor = Color.White;
                    area.BackColor = Color.White;
                    break;
            }

            if (backgroundColor != ChartBackgroundColor.White)
            {
                chart1.BackSecondaryColor = Color.White;
                chart1.BackGradientStyle = GradientStyle.TopBottom;
                chart1.BorderlineDashStyle = ChartDashStyle.Solid;
                chart1.BorderlineWidth = 2;
                chart1.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

                area.Area3DStyle.IsClustered = true;
                area.Area3DStyle.Perspective = 10;
                area.Area3DStyle.IsRightAngleAxes = false;
                area.Area3DStyle.WallWidth = 0;
                area.Area3DStyle.Inclination = 15;
                area.Area3DStyle.Rotation = 10;
            }

            area.AxisX.IsLabelAutoFit = false;
            area.AxisX.LabelStyle.Font = new Font("Trebuchet MS", 7.25F, FontStyle.Regular);
            //area.AxisX.LabelStyle.IsEndLabelVisible = false;
            area.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            area.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.IsStartedFromZero = false;
            area.AxisX.RoundAxisValues();

            area.AxisY.IsLabelAutoFit = false;
            area.AxisY.LabelStyle.Font = new Font("Trebuchet MS", 7.25F, System.Drawing.FontStyle.Regular);
            area.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisY.IsStartedFromZero = false;

            area.AxisY2.IsLabelAutoFit = false;
            area.AxisY2.LabelStyle.Font = new Font("Trebuchet MS", 7.25F, System.Drawing.FontStyle.Regular);
            area.AxisY2.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisY2.MajorGrid.LineColor = Color.FromArgb(15, 15, 15, 15);
            area.AxisY2.IsStartedFromZero = false;


            area.BackSecondaryColor = System.Drawing.Color.White;
            area.BackGradientStyle = GradientStyle.TopBottom;
            area.BorderColor = Color.FromArgb(64, 64, 64, 64);
            area.BorderDashStyle = ChartDashStyle.Solid;
            area.Position.Auto = false;
            area.Position.Height = 82F;
            area.Position.Width = 88F;
            area.Position.X = 3F;
            area.Position.Y = 10F;
            area.ShadowColor = Color.Transparent;

            chart1.ChartAreas.Add(area);
            chart1.Invalidate();
        }


        public static List<System.Drawing.Color> GetColors()
        {
            List<Color> my_colors = new List<Color>();
            my_colors.Add(Color.DarkBlue);
            my_colors.Add(Color.DarkRed);
            my_colors.Add(Color.DarkGreen);
            my_colors.Add(Color.Black);
            my_colors.Add(Color.DarkCyan);
            my_colors.Add(Color.DarkViolet);
            my_colors.Add(Color.DarkOrange);
            my_colors.Add(Color.Maroon);
            my_colors.Add(Color.SaddleBrown);
            my_colors.Add(Color.DarkOliveGreen);

            return my_colors;
        }
    }

    public enum ChartBackgroundColor
    {
        Blue = 0,
        Green = 1,
        Red = 2,
        White = 3,
    }
}
