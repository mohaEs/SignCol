using DbModel.ViewModel.WordsVM;
using System;
using System.Collections.Generic;
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

namespace Sign_Language_Capture_Kinnect.Pages
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : Window// MahApps.Metro.Controls.MetroWindow
    {
        public Chart()
        {
            InitializeComponent();
            ChartData = new ChartVM();
            DataContext = this;
        }
        public ChartVM ChartData { get; set; }
    }
}
