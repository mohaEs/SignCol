using DbModel.ViewModel.OptionVM;
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
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Window// MahApps.Metro.Controls.MetroWindow
    {
        public Setting()
        {
            InitializeComponent();
            SettingData = new SettingViewModel(new AppConfig());
            DataContext = this;
        }
        public SettingViewModel SettingData { get; set; }
    }
}
