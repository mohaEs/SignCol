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
using System.Windows.Shapes;

namespace Sign_Language_Capture_Kinnect.Pages
{
    /// <summary>
    /// Interaction logic for Words.xaml
    /// </summary>
    public partial class Words : Window// MahApps.Metro.Controls.MetroWindow
    {
        public Words()
        {
            InitializeComponent();
            WordData = new Words_ViewModel(new DbModel.ViewModel.WordsVM.WordsModel());
            DataContext = this;
        }
        public Words_ViewModel WordData { get; set; }
    }
}
