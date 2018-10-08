using DbModel.ViewModel.UserVM;
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
    /// Interaction logic for User_Management.xaml
    /// </summary>
    public partial class User_Management :  Window// MahApps.Metro.Controls.MetroWindow
    {
        public User_Management()
        {
            InitializeComponent();
            UserData = new User_ViewModel(new DbModel.ViewModel.UserVM.UserModel());
            DataContext = this;
        }
        public User_ViewModel UserData { get; set; }
    }
}
