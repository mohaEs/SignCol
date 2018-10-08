using DbModel.ViewModel.LanguageVM;
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
    /// Interaction logic for Language.xaml
    /// </summary>
    public partial class Language : Window// MahApps.Metro.Controls.MetroWindow
    {
        public Language()
        {
            InitializeComponent();
            LanguageData = new LanguageViewModel(new DbModel.ViewModel.LanguageVM.LanguageModel());
            DataContext = this;
        }
        public LanguageViewModel LanguageData { get; set; }
    }
}
