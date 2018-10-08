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
    /// Interaction logic for Video.xaml
    /// </summary>
    public partial class Video : Window// 
    {
        public Video()
        {
            InitializeComponent();
            VideoData = new WordVideoVM(new VideoModel());
            DataContext = this;
        }
        public WordVideoVM VideoData { get; set; }
    }
}
