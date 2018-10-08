using DbModel.Context;
using DbModel.DomainClasses.Entities;
using DbModel.DomainClasses.Enum;
using DbModel.ViewModel.WordsVM;
using GalaSoft.MvvmLight.Messaging;
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
using System.Windows.Shapes;

namespace Sign_Language_Capture_Kinnect.Views.Words
{
    /// <summary>
    /// Interaction logic for NewVideo.xaml
    /// </summary>
    public partial class NewVideo_Kinect : Window
    {
        private string userId;
        IUnitOfWork uow;
        

        public NewVideo_Kinect(VideoModel video, User user, DbModel.DomainClasses.Entities.Words thword/*, int ty*/)
        {
            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            registerMessenger();
            InitializeComponent();
            if (video != null && user == null)
            {
                VideoData = new WordVideoVM(video,1, thword.word_id, LeapKinnectType.Kinnect, uow);
                VideoData.leapkinnecttype = LeapKinnectType.Kinnect;
                VideoData.eword = thword;

                if (VideoData.euser != null)
                {
                    User_id.Content = "userient ID = " + VideoData.euser.User_id.ToString() + ", Name = " + VideoData.euser.Name;
                    userId = VideoData.euser.User_id.ToString();

                }
            }
            if (video == null && user != null)
            {
                VideoModel f = new VideoModel();
                //f.userient = user;
                f.User_id = user.User_id;
                User_id.Content = "user ID = " + user.User_id.ToString() + ", Name = " + user.Name;
                userId = user.User_id.ToString();
                VideoData = new WordVideoVM(f, 1, thword.word_id, LeapKinnectType.Kinnect, uow);
                VideoData.leapkinnecttype = LeapKinnectType.Kinnect;
                VideoData.eword = thword;
                VideoData.User_id = user.User_id;
                VideoData.euser = user;
            }
            if (video != null && user != null)
            {
                video.User = user;
                video.User_id = user.User_id;
                User_id.Content = "user ID = " + user.User_id.ToString() + ", Name = " + user.Name;
                userId = user.User_id.ToString();
                VideoData = new WordVideoVM(video, 1, thword.word_id, LeapKinnectType.Kinnect, uow);
                VideoData.leapkinnecttype = LeapKinnectType.Kinnect;
                VideoData.eword = thword;
                VideoData.User_id = user.User_id;
                VideoData.euser = user;
            }
            else if (video == null && user == null)
            {
                VideoData = new WordVideoVM(new VideoModel(), 1, thword.word_id, LeapKinnectType.Kinnect, uow);
                VideoData.leapkinnecttype = LeapKinnectType.Kinnect;
                VideoData.eword = thword;
            }



            DataContext = this;

        }
        public WordVideoVM VideoData { get; set; }

        private void Choose_User_Click(object sender, RoutedEventArgs e)
        {
            new Views.Membership.SearchUser("Word").ShowDialog();
        }
        private void registerMessenger()
        {
            //Get Search String From Search Form
            Messenger.Default.Register<User>(this, "SearchuserientService",
             (message) =>
             {
                 if (message != null)
                 {
                     User_id.Content = "User ID = " + message.User_id.ToString() + ", Name = " + message.Name;
                     userId = message.User_id.ToString();
                     VideoData.euser = message;
                     VideoData.User_id = message.User_id;
                 }
             });
            //Get Inserted
            Messenger.Default.Register<ObservableCollection<VideoModel>>(this, "InsertedService",
             (message) =>
             {
                 this.Close();
             });
        }
    }
}
