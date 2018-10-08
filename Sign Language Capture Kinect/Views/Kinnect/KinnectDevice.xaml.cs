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
using System.IO;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Diagnostics;

using DbModel.Extensions;
using DbModel.ViewModel.WordsVM;
using DbModel.DomainClasses.Enum;
using DbModel.DomainClasses.Entities;
using DbModel.Context;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using StructureMap;
using DbModel.Services.Interfaces;
using DbModel.ViewModel.OptionVM;
using System.Data.Entity.Validation;
using DbModel.ViewModel;
using Sign_Language_Capture_Kinnect.Views.Words;

using Microsoft.VisualBasic.Devices;


namespace Sign_Language_Capture_Kinnect.Views.Kinnect
{
    /// <summary>
    /// Interaction logic for KinnectDevice.xaml
    /// </summary>
    public partial class KinnectDevice : Window
    {
        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        ProcessingsAndRendering _ProcessingInstance;

        string SavingPath;
        string StrComments;


        /// since (_sensor != null) is not relaible and does not work here, 
        /// we set our manual flag for kinect:
        bool isKinect = false;
        /// Current status text to display
        private string statusText = null;

        // measure the free memory of RAM and set the maximumFramesNumbers here



        private string userId;
        private User thisuser;
        private DbModel.DomainClasses.Entities.Words thisword;
        IUnitOfWork uow;
        private IOptionService ops;
        private IVideo videoservice;
        private AppConfig app;
        private ListItems listitem;
        private VideoModel VideoInfo;

        private int? ttt;

        public KinnectDevice(VideoModel video, User user, DbModel.DomainClasses.Entities.Words thword, int? ty)
        {
            uow = ObjectFactory.GetInstance<IUnitOfWork>();
            ops = ObjectFactory.GetInstance<IOptionService>();
            videoservice = ObjectFactory.GetInstance<IVideo>();
            app = ops.GetAll();
            VideoInfo = new VideoModel();

            registerMessenger();
            InitializeComponent();

            thisword = thword;

            ttt = ty;

            //SavingPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //listitem = new ListItems();
            string wtype = UtilityClass.IntToWordTypeString(thword.WordType);//listitem.GetWordType().FirstOrDefault(x => x.ID == UtilityClass.WordTypeToInt(thword.WordType)).Value;
            SavingPath = app.FileUrl + @"\" + thword.Languages.Name + "_" + wtype + "_" +
                thword.Name ;
            //textBoxPath.Text = SavingPath;
            textBoxComment.Text = SavingPath;
            //        textBoxComment.IsEnabled = false;
            
            

            _sensor = KinectSensor.GetDefault();
            if (_sensor != null)
            {

                _sensor.Open();

                _ProcessingInstance = new ProcessingsAndRendering();
                _ProcessingInstance.initializeCoordinateMapper(_sensor);
                _ProcessingInstance.InitializeColor(_sensor);
                _ProcessingInstance.InitializeIR(_sensor);
                _ProcessingInstance.InitializeDepth(_sensor);
                _ProcessingInstance.InitializeBodyIndex(_sensor); ;
                _ProcessingInstance.InitializeBody(_sensor);


                // set the status text

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body | FrameSourceTypes.BodyIndex);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
                //  textBlockLogs.Text = textBlockLogs.Text + "\n --- Kinect not Found :|";


            }

            DataContext = this;

        } /// end method mainwindow

        public WordVideoVM VideoData { get; set; }


        private void Choose_User_Click(object sender, RoutedEventArgs e)
        {
            new Views.Membership.SearchUser("Kinnect").ShowDialog();
            textBoxComment.Text = SavingPath;
        }
        private void registerMessenger()
        {
            //Get Search String From Search Form
            Messenger.Default.Register<User>(this, "SearchuserientService",
             (message) =>
             {
                 if (message != null)
                 {
                     Random rnd = new Random();
                     int rand = rnd.Next(1000, 99999);

                     User_id.Text = message.Name; /*"User ID = " + message.User_id.ToString() + ", Name = " +*/
                     userId = message.User_id.ToString();


                     VideoInfo.User = message;
                     VideoInfo.User_id = message.User_id;

                     //buttonStartCapturing.IsEnabled = true;
                     buttonInitializedCapturing.IsEnabled = true;

                     string lastpart = SavingPath.Substring(SavingPath.LastIndexOf(@"\") + 1);
                     string[] ui = lastpart.Split('_');
                     SavingPath = SavingPath.Substring(0, SavingPath.LastIndexOf(@"\") + 1) +
                                ui[0] + "_" + ui[1] + "_" + ui[2] + "_" + message.Name
                                + "_" + rand.ToString();

                 }
             });
            //Get Inserted
            Messenger.Default.Register<ObservableCollection<VideoModel>>(this, "InsertedService",
             (message) =>
             {
                 this.Close();
             });
        }
        private void SaveToDatabase()
        {
            try
            {

                VideoInfo.KinnectFilePath = SavingPath.Remove(0, app.FileUrl.Length);

                //VideoInfo.User_id = thisuser.User_id;
                VideoInfo.LeapKinnectType = LeapKinnectType.Kinnect;
                VideoInfo.word_id = thisword.word_id;

                videoservice.Create(VideoInfo);
                MessageBox.Show("Saving Successed.");
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var sdc in ex.EntityValidationErrors)
                {
                    foreach (var ssd in sdc.ValidationErrors)
                    {
                        sb.Append(ssd.ErrorMessage);
                    }
                }
                MessageBox.Show(sb.ToString());
            }
        }

        private void buttonInitializedCapturing_Click(object sender, RoutedEventArgs e)
        {
            _ProcessingInstance.InitializeCapturing();
            
            textBlockLogs.Text = " --- Initialization is done. \n" + textBlockLogs.Text;
            textBlockLogs.Text = " --- Your physical memory constrainet yields only \n" 
                + _ProcessingInstance.MaximumPossibleFrameNumners_AfterInitializedCaputing.ToString() 
                + "\nFrames for capturing. \n"
                + "Suppose 20 frames for each second. \n"
                + textBlockLogs.Text;
            buttonStartCapturing.IsEnabled = true;
        }

        private void buttonStartCapturing_Click(object sender, RoutedEventArgs e)
        {


            textBlockLogs.Text = " --- Capturing is started. \n" + textBlockLogs.Text;
            
            buttonStopCapturing.IsEnabled = true;
            StrComments = textBoxComment.Text;
            _ProcessingInstance.isCapturing = true;

            _ProcessingInstance.Counter_CapturingFrame = -1;
            //_ProcessingInstance.InitializeCapturing();
        }

        private void buttonSaveCapturing_Click(object sender, RoutedEventArgs e)
        {
            WindowMessageWriting wn = new WindowMessageWriting();
            wn.Show();
            System.Threading.Thread.Sleep(1000);

            CreateFolders(SavingPath);

            _ProcessingInstance.SaveTimes(_ProcessingInstance.stringCapturingTimes_Array, SavingPath);
            SaveComment(textBoxComment.Text, SavingPath);

            for (int i = 0; i < _ProcessingInstance.Counter_CapturingFrame-1; ++i)
            {
                /// Bitmaps
                /// 
                _ProcessingInstance.SaveImages_ArrayPixels(_ProcessingInstance.ColorPixels_Array[i], i, "color", SavingPath);
                System.Threading.Thread.Sleep(10);
                GC.Collect();
                _ProcessingInstance.SaveImages_ArrayPixels(_ProcessingInstance.InfraredPixels_Array[i], i, "infrared", SavingPath);
                _ProcessingInstance.SaveImages_ArrayPixels(_ProcessingInstance.DepthPixels_Array[i], i, "depth", SavingPath);
                _ProcessingInstance.SaveImages_ArrayPixels(_ProcessingInstance.BodyIndexPixels_Array[i], i, "bodyindex", SavingPath);
                _ProcessingInstance.SaveImages_ArrayPixels(_ProcessingInstance.ColorBodyIndexPixels_Array[i], i, "colorbody", SavingPath);

                /// Bodies
                _ProcessingInstance.SaveBodies(_ProcessingInstance.Bodies_Array[i], i, SavingPath);
            }

            _ProcessingInstance.ColorPixels_Array = null;
            _ProcessingInstance.InfraredPixels_Array = null;
            _ProcessingInstance.DepthPixels_Array = null;
            _ProcessingInstance.BodyIndexPixels_Array = null;
            _ProcessingInstance.ColorBodyIndexPixels_Array = null;
            _ProcessingInstance.Bodies_Array = null;
            _ProcessingInstance.stringCapturingTimes_Array = null;
            GC.Collect();

            //textBoxPath.IsEnabled = true;
            buttonStartCapturing.IsEnabled = false;
            buttonStopCapturing.IsEnabled = false;
            buttonDiscarding.IsEnabled = false;
            //buttonSavePath.IsEnabled = false;
            buttonSaveCapturing.IsEnabled = false;
            textBoxComment.IsEnabled = false;
            buttonInitializedCapturing.IsEnabled = false;
            buttonSavePath.IsEnabled = false;

            wn.Close();
            textBlockLogs.Text = "--- Session is saved. \n" + textBlockLogs.Text;



            SaveToDatabase();

        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            using (var colorFrame = reference.ColorFrameReference.AcquireFrame())
            using (var infraredFrame = reference.InfraredFrameReference.AcquireFrame())
            using (var depthFrame = reference.DepthFrameReference.AcquireFrame())
            using (var bodyIndexFrame = reference.BodyIndexFrameReference.AcquireFrame())
            using (var bodyFrame = reference.BodyFrameReference.AcquireFrame())
            {
                if (colorFrame == null || depthFrame == null || infraredFrame == null)
                {
                    return;
                }
                else
                {

                    if (_ProcessingInstance.isCapturing &&
                        _ProcessingInstance.Counter_CapturingFrame < _ProcessingInstance.MaximumPossibleFrameNumners_AfterInitializedCaputing)
                    {
                        _ProcessingInstance.Counter_CapturingFrame = _ProcessingInstance.Counter_CapturingFrame + 1;
                        _ProcessingInstance.stringCapturingTimes_Array[_ProcessingInstance.Counter_CapturingFrame] =
                            TimesToReadableString(colorFrame.RelativeTime);
                    }
                    else
                    { if(_ProcessingInstance.isCapturing && _ProcessingInstance.Counter_CapturingFrame == _ProcessingInstance.MaximumPossibleFrameNumners_AfterInitializedCaputing)
                        {
                            _ProcessingInstance.isCapturing = false;
                            buttonStopCapturing.IsEnabled = false;
                            buttonSaveCapturing.IsEnabled = true;
                            buttonDiscarding.IsEnabled = true;
                        }
                    }

                    /// Color
                    _ProcessingInstance.ProcessColor(colorFrame);
                    //ImageBoxRGB.Source = _ProcessingInstance.colorBitmap;
                    /// show both color frame and map coordinated skel on it:
                    ImageBoxRGB.Source = _ProcessingInstance.imageSourceBody_onColor;


                    /// Infrared
                    _ProcessingInstance.ProcessIR(infraredFrame);
                    ImageBoxIR.Source = _ProcessingInstance.infraredBitmap;

                    /// Depth
                    _ProcessingInstance.ProcessDepth(depthFrame);
                    ImageBoxDepth.Source = _ProcessingInstance.depthBitmap;

                    /// BodyIndex
                    if (bodyIndexFrame != null)
                    {
                        _ProcessingInstance.ProcessBodyIndex(bodyIndexFrame);
                        ImageBoxBodyIndex.Source = _ProcessingInstance.bodyIndexBitmap;
                    }

                    /// Body
                    if (bodyFrame != null)
                    {
                        _ProcessingInstance.ProcessBody(bodyFrame);
                        ImageBoxSkelBody.Source = _ProcessingInstance.imageSourceBody_onDepth;
                    }

                    if (colorFrame != null && depthFrame != null && bodyIndexFrame != null)
                    {
                        _ProcessingInstance.ProcessCoordinatingBodies(colorFrame, depthFrame, bodyIndexFrame);
                        ImageBoxRGBBodyMapped_onDepthSpace.Source = _ProcessingInstance.imageSourceColorBodies_onDepthSpace;

                    }

                }
            }
        }



        


        private void buttonStopCapturing_Click(object sender, RoutedEventArgs e)
        {
            buttonDiscarding.IsEnabled = true;
            buttonSaveCapturing.IsEnabled = true;
            _ProcessingInstance.isCapturing = false;
            buttonStartCapturing.IsEnabled = false;
            buttonStopCapturing.IsEnabled = false;
            textBlockLogs.Text = " --- Capturing stopped. \n" + textBlockLogs.Text;
        }


        private void buttonDiscarding_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("All Files of this session will be removed. Are you sure to delete?",
                "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                //do no stuff
            }
            else
            {
                //do yes stuff
                _ProcessingInstance.ColorPixels_Array = null;
                _ProcessingInstance.InfraredPixels_Array = null;
                _ProcessingInstance.DepthPixels_Array = null;
                _ProcessingInstance.BodyIndexPixels_Array = null;
                _ProcessingInstance.ColorBodyIndexPixels_Array = null;
                _ProcessingInstance.Bodies_Array = null;
                _ProcessingInstance.stringCapturingTimes_Array = null;
                GC.Collect();
                buttonSaveCapturing.IsEnabled = false;
                buttonStartCapturing.IsEnabled = true;

                //textBoxPath.IsEnabled = true;
                buttonStopCapturing.IsEnabled = false;
                buttonStartCapturing.IsEnabled = true;
                //buttonSavePath.IsEnabled = true;
                buttonSaveCapturing.IsEnabled = false;
                buttonDiscarding.IsEnabled = false;
                textBoxComment.IsEnabled = true;

                textBlockLogs.Text = " --- Session is deteleted. You can try again. \n" + textBlockLogs.Text;
            }
        }



        private void ClearFolder(string _Path)
        {

            System.IO.DirectoryInfo di = new DirectoryInfo(_Path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private void CreateFolders(string _Path)
        {
            Directory.CreateDirectory(_Path + "\\01 Times");
            Directory.CreateDirectory(_Path + "\\02 Color Frames");
            Directory.CreateDirectory(_Path + "\\03 Infrared Frames");
            Directory.CreateDirectory(_Path + "\\04 Depth Frames");
            Directory.CreateDirectory(_Path + "\\05 BodyIndex Frames");
            //Directory.CreateDirectory(_Path + "\\06 Body Skels Images");
            Directory.CreateDirectory(_Path + "\\06 Body Skels data");
            Directory.CreateDirectory(_Path + "\\07 Color Body Frames");
            //Directory.CreateDirectory(_Path + "\\08 Mapping Info");

        }




        private void textBoxComment_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            textBoxComment.Text = "";
            buttonStartCapturing.IsEnabled = true;
        }

        private void SaveComment(string txt, string _path)
        {
            string path = _path + "\\Comments" + ".txt";

            System.IO.File.WriteAllText(path, txt);

        }


        public static string TimesToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}{4}",
                span.Days >= 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
                span.Hours >= 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
                span.Minutes >= 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty,
                span.Seconds >= 0 ? string.Format("{0:0} second{1}, ", span.Seconds, span.Seconds == 1 ? String.Empty : "s") : string.Empty,
                span.Milliseconds >= 0 ? string.Format("{0:0} milisecond{1}", span.Milliseconds, span.Milliseconds == 1 ? String.Empty : "s") : string.Empty
                );

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;
        }



        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
  //          this.StatusText = this._sensor.IsAvailable ? Properties.Resources.RunningStatusText
  //                                                          : Properties.Resources.SensorNotAvailableStatusText;
        }

        

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this._reader != null)
            {
                // DepthFrameReader is IDisposable
                this._reader.Dispose();
                this._reader = null;
            }

            if (this._sensor != null)
            {
                this._sensor.Close();
                this._sensor = null;
            }

            registerMessenger();
            AllVideo_SpecialKinnect f = new AllVideo_SpecialKinnect(thisword.word_id, ttt.Value);
            f.Show();

        }


    } /// end class mainwindow
} /// end namespace