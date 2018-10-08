using DbModel.Command;
using DbModel.Context;
using DbModel.Infrastructure;
using DbModel.Services;
using DbModel.Services.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MvvmValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using DbModel.DomainClasses.Entities;
using DbModel.DomainClasses.Enum;
using DbModel.Extensions;
using DbModel.ViewModel.LanguageVM;
using System.IO;
using System.Windows.Media;
using Microsoft.Win32;
using DbModel.ViewModel.OptionVM;
using System.Windows.Controls;
using OxyPlot;
using Caliburn.Micro;
using System.Windows.Forms.DataVisualization.Charting;

namespace DbModel.ViewModel.WordsVM
{
    public class rItem
    {
        public string Label { get; set; }
        public double Value1 { get; set; }
        public double Value2 { get; set; }
        public double Value3 { get; set; }
    }
    public class lItem
    {
        public string Label { get; set; }
        public double Value1 { get; set; }
        public double Value2 { get; set; }
        public double Value3 { get; set; }
    }
    public class WordVideoVM : ValidatableViewModelBase
    {
        IUnitOfWork _uow;
        public ObservableCollection<VideoModel> AllVideos { get; set; }
        public VideoModel VideoInfo { set; get; }

        public Collection<rItem> rItems { get; set; }
        public Collection<lItem> lItems { get; set; }
        public PlotModel Model1 { get; set; }

        private IOptionService option { set; get; }
        private IWords word { set; get; }
        private IVideo video { set; get; }
        private IUser user { set; get; }
        private AppConfig app { set; get; }

        public ListItems listitem;

        private MSChartVM chatmodel;

        private int? wordid;

        public int videotype = 1;
        private LeapKinnectType lk { get; set; }
        // 1 ==> kinnect
        // 2 ==> leap
        public WordVideoVM(VideoModel model)
        {

        }
        public WordVideoVM(VideoModel model, int videostyp, int? word_id, LeapKinnectType thisleaporkin, IUnitOfWork uw)
        {
            Contract.Requires(model != null);
            _uow = uw;
            videotype = videostyp;
            VideoInfo = model;
            video = new VideoService(_uow);
            word = new WordsService(_uow);
            option = new OptionService(_uow);
            user = new UserService(_uow);
            listitem = new ListItems();

            wordid = word_id;
            //AllLanguages = GetAllLanguages();
            //allWordTypes = listitem.GetWordType();
            app = option.GetAll();

            lk = thisleaporkin;
            //app.FileUrl = "c:";


            RefreshProducts();

            chatmodel = new MSChartVM();
            //chatmodel.BarChart();
            charttest = chatmodel.BarSeriesCollection;// PerVideoPartCount();


            if (model != null/* && UtilityClass.CheckIntHasValue(model.video_id)*/)
            {
                //wordType = model.Words.WordType;
                //elang = model.Words.Languages;
                //Name = model.Words.Name;
                eword = model.Words;
                
                if (!string.IsNullOrEmpty(model.KinnectFilePath) && videotype == 1)
                {
                    //euser = model.User;
                    //if (model.User_id.HasValue)
                    //    User_id = model.User_id.Value;

                    _attachPicture = new Uri(app.FileUrl + @"\" + model.KinnectFilePath);//UtilityClass.ByteToStream(app.FileUrl + model.FilePath);
                    selectedpic = model.KinnectFilePath;
                    prevpic = model.KinnectFilePath;
                }
                else if (!string.IsNullOrEmpty(model.LeapFilePath) && videotype != 1)
                {
                    //euser = model.User;
                    //if (model.User_id.HasValue)
                    //    User_id = model.User_id.Value;

                    _attachPicture = new Uri(app.FileUrl + @"\" + model.LeapFilePath);//UtilityClass.ByteToStream(app.FileUrl + model.FilePath);
                    selectedpic = model.LeapFilePath;
                    prevpic = model.LeapFilePath;
                }
            }


            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;


        }
        private VideoModel _gridselecteditem;
        public VideoModel GridSelectedItem
        {
            get { return _gridselecteditem; }
            set
            {
                _gridselecteditem = value;
                RaisePropertyChanged("GridSelectedItem");
            }
        }

        public RelayCommand _selectPictureCommand { get; private set; }
        public RelayCommand SelectPictureCommand
        {
            get
            {
                return _selectPictureCommand
                      ?? (_selectPictureCommand = new RelayCommand/*<object>*/(
                          OnSelectPictureCommand, CanMoveFirstCommand));
            }
        }
        //private byte[] selectedpic;
        private string mypath { set; get; }
        private string selectedpic, destinationFile, prevpic;
        private void OnSelectPictureCommand()
        {

            OpenFileDialog OpenFileDialog = new OpenFileDialog();
            //OpenFileDialog.Title = "Insert Video File";
            //OpenFileDialog.DefaultExt = "rtf";
            //OpenFileDialog.Filter = "Video Files (*.wmv, *.mpeg, *.avi)|*.wmv; *.mpeg; *.avi";

            //"Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            //OpenFileDialog.FilterIndex = 1;
            if (OpenFileDialog.ShowDialog() == true)
            {
                mypath = app.FileUrl + @"\" + Path.GetFileName(OpenFileDialog.FileName);// Path.Combine(app.FileUrl /*+ @"\"*/, Path.GetFileName(OpenFileDialog.FileName));

                //string destFile = mypath;
                //selectedpic = OpenFileDialog.FileName;
                destinationFile = mypath.Replace(@"\\", @"\")
                    .Substring(0, mypath.LastIndexOf(@"\") + 1) +
                    Guid.NewGuid().ToString().Replace("-", "") +
                    Path.GetExtension(OpenFileDialog.FileName);

                selectedpic = Path.GetFullPath(OpenFileDialog.FileName);
                //destinationFile = app.FileUrl + @"\" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(OpenFileDialog.FileName);
                //MessageBox.Show(selectedpic+"\n"+destinationFile);
                //File.Copy(selectedpic, destinationFile, true);

                //StreamReader reader = new StreamReader(selectedpic);
                //string fileContent = reader.ReadToEnd();

                //StreamWriter writer = new StreamWriter(destinationFile);
                //writer.Write(fileContent);

                AttachPicture = new Uri(Path.GetFullPath(OpenFileDialog.FileName));//UtilityClass.ByteToStream(Path.GetFullPath(OpenFileDialog.FileName));
            }
        }
        private bool CanMoveFirstCommand(/*object obj*/)
        {
            return true;
        }
        private Uri _attachPicture;
        public Uri AttachPicture
        {
            get
            {
                return _attachPicture;
            }
            set
            {
                _attachPicture = value;
                RaisePropertyChanged("AttachPicture");
            }
        }


        public RelayCommand _insertCommand { get; private set; }
        public RelayCommand InsertCommand
        {
            get
            {
                return _insertCommand
                      ?? (_insertCommand = new RelayCommand(
                          saveInfo(), canInsert()));
            }
        }
        private Func<bool> canInsert()
        {
            return () =>
            {
                if (video_id != 0)
                {
                    return false;
                }
                else
                    return true;
            };
        }
        private System.Action saveInfo()
        {
            return () =>
            {
                Validate();
                MvvmValidation.ValidationResult validationResult = Validator.GetResult();
                if (validationResult.IsValid) //!IsValid.GetValueOrDefault(true))
                {
                    try
                    {
                        if (string.IsNullOrEmpty(destinationFile))
                        {
                            MessageBox.Show("لطفا فیلدها را پر نمایید");
                            return;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(destinationFile))
                            {
                                if (!Directory.Exists(mypath))
                                {
                                    Directory.CreateDirectory(app.FileUrl);
                                    File.Copy(selectedpic, destinationFile, true);
                                }
                                else
                                {
                                    File.Copy(selectedpic, destinationFile, true);
                                }
                                if (videotype == 1)
                                {
                                    VideoInfo.KinnectFilePath = destinationFile.Remove(0, app.FileUrl.Length);
                                }
                                else
                                    VideoInfo.LeapFilePath = destinationFile.Remove(0, app.FileUrl.Length);
                            }
                            VideoInfo.User_id = User_id;
                            //VideoInfo.word_id = eword.word_id;
                            VideoInfo.LeapKinnectType = leapkinnecttype;

                            VideoInfo.word_id = eword.word_id;

                            video.Create(VideoInfo);
                            MessageBox.Show("Successful: Video is created.");
                            RefreshProducts();
                            Messenger.Default.Send(AllVideos, "InsertedService");
                            Validator.Reset();
                        }
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
            };
        }


        public RelayCommand _updateCommand { get; private set; }
        public RelayCommand UpdateCommand
        {
            get
            {
                return _updateCommand
                      ?? (_updateCommand = new RelayCommand(
                          UpdateInfo(), canUpdateInfo()));
            }
        }
        private Func<bool> canUpdateInfo()
        {
            return () =>
            {
                if (video_id == 0)
                {
                    // MessageBox.Show("Please First Select Record For Update");
                    return false;
                }
                else
                    return true;
            };
        }
        private System.Action UpdateInfo()
        {
            return () =>
            {
                Validate();
                MvvmValidation.ValidationResult validationResult = Validator.GetResult();
                if (validationResult.IsValid)
                {
                    try
                    {
                        VideoInfo.video_id = video_id;
                        if (!string.IsNullOrEmpty(destinationFile))
                        {
                            if (string.IsNullOrEmpty(prevpic))
                            {
                                if (!Directory.Exists(mypath))
                                {
                                    Directory.CreateDirectory(app.FileUrl/*FilesPath + @"\Fe" + FirstExam_id.Value.ToString()*/);
                                    File.Copy(selectedpic, destinationFile, true);
                                }
                                else
                                {
                                    File.Copy(selectedpic, destinationFile, true);
                                }
                                if (videotype == 1)
                                    VideoInfo.KinnectFilePath = destinationFile.Remove(0, app.FileUrl.Length);
                                else
                                    VideoInfo.LeapFilePath = destinationFile.Remove(0, app.FileUrl.Length);
                            }
                            else
                            {
                                string n = ((destinationFile.Contains(@"\")) ? destinationFile.Substring(
                                    destinationFile.LastIndexOf(@"\") + 1) : destinationFile);
                                if (n != prevpic)
                                {
                                    System.GC.Collect();
                                    System.GC.WaitForPendingFinalizers();

                                    File.Delete(app.FileUrl + prevpic);
                                    //documents.Delete(DocumentInfo.doc_id);
                                    File.Copy(selectedpic, destinationFile, true);
                                    if (videotype == 1)
                                        VideoInfo.KinnectFilePath = destinationFile.Remove(0, app.FileUrl.Length);
                                    else
                                        VideoInfo.LeapFilePath = destinationFile.Remove(0, app.FileUrl.Length);
                                }
                            }
                        }

                        if (euser == null)
                        {
                            MessageBox.Show("Please fill the fields.");
                            return;
                        }
                        else
                        {
                            //VideoInfo.User = euser;
                            VideoInfo.User_id = User_id;
                            VideoInfo.LeapKinnectType = leapkinnecttype;

                            VideoInfo.word_id = eword.word_id;

                            //VideoInfo.VideoType = VideoType;
                            video.UpdateVideo(VideoInfo);
                            MessageBox.Show("Successful: Video is updated.");
                            RefreshProducts();
                            Messenger.Default.Send(AllVideos, "InsertedService");
                            Validator.Reset();
                        }
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
            };
        }

        public RelayCommand _deleteCommand { get; private set; }
        public RelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand
                      ?? (_deleteCommand = new RelayCommand(
                          DeleteInfo(), canDeleteInfo()));
            }
        }
        private Func<bool> canDeleteInfo()
        {
            return () =>
            {
                if (video_id == 0)
                {
                    // MessageBox.Show("Please First Select Record For Update");
                    return false;
                }
                else
                    return true;
            };
        }
        private System.Action DeleteInfo()
        {
            return () =>
            {
                //Validate();
                //MvvmValidation.ValidationResult validationResult = Validator.GetResult();
                //if (validationResult.IsValid)
                //{
                try
                {
                    /*if (!string.IsNullOrEmpty(prevpic))
                    {
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();

                        File.Delete(app.FileUrl + prevpic);
                    }*/

                    //word.DeleteVideo(VideoInfo);
                    if(videotype==1)
                    {
                        if (!string.IsNullOrEmpty(KinnectFilePath))
                        {
                            System.GC.Collect();
                            System.GC.WaitForPendingFinalizers();

                            File.Delete(app.FileUrl + KinnectFilePath);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(LeapFilePath))
                        {
                            System.GC.Collect();
                            System.GC.WaitForPendingFinalizers();

                            File.Delete(app.FileUrl + LeapFilePath);
                        }
                    }
                    //video.DeleteKinnectVideo(VideoInfo, app);
                    //video.DeleteLeapVideo(VideoInfo, app);
                    video.Delete(VideoInfo.video_id);
                    MessageBox.Show("Successful: Video is removed.");
                    RefreshProducts();
                    Messenger.Default.Send(AllVideos, "InsertedService");
                    Validator.Reset();
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
                //}
            };
        }

      
        private BindableCollection<Series> _charttest;
        public BindableCollection<Series> charttest
        {
            get { return _charttest; }
            set { _charttest = value; }
        }










        private int start = 0, itemCount = 5;
        private string sortColumn = "video_id";
        private bool ascending = true;
        private int totalItems = 0;
        private NavigationReplayCommand firstCommand;
        private NavigationReplayCommand previousCommand;
        private NavigationReplayCommand nextCommand;
        private NavigationReplayCommand lastCommand;
        public int Start { get { return start + 1; } }
        public int End { get { return start + itemCount < totalItems ? start + itemCount : totalItems; } }
        public int TotalItems { get { return totalItems; } }
        public ICommand FirstCommand
        {
            get
            {
                if (firstCommand == null)
                {
                    firstCommand = new NavigationReplayCommand
                    (
                        param =>
                        {
                            start = 0;
                            RefreshProducts();
                        },
                        param =>
                        {
                            return start - itemCount >= 0 ? true : false;
                        }
                    );
                }
                return firstCommand;
            }
        }
        public ICommand PreviousCommand
        {
            get
            {
                if (previousCommand == null)
                {
                    previousCommand = new NavigationReplayCommand
                    (
                        param =>
                        {
                            start -= itemCount;
                            RefreshProducts();
                        },
                        param =>
                        {
                            return start - itemCount >= 0 ? true : false;
                        }
                    );
                }

                return previousCommand;
            }
        }
        public ICommand NextCommand
        {
            get
            {
                if (nextCommand == null)
                {
                    nextCommand = new NavigationReplayCommand
                    (
                        param =>
                        {
                            start += itemCount;
                            RefreshProducts();
                        },
                        param =>
                        {
                            return start + itemCount < totalItems ? true : false;
                        }
                    );
                }

                return nextCommand;
            }
        }
        public ICommand LastCommand
        {
            get
            {
                if (lastCommand == null)
                {
                    lastCommand = new NavigationReplayCommand
                    (
                        param =>
                        {
                            start = (totalItems / itemCount - 1) * itemCount;
                            start += totalItems % itemCount == 0 ? 0 : itemCount;
                            RefreshProducts();
                        },
                        param =>
                        {
                            return start + itemCount < totalItems ? true : false;
                        }
                    );
                }

                return lastCommand;
            }
        }
        public void Sort(string sortColumn, bool ascending)
        {
            this.sortColumn = sortColumn;
            this.ascending = ascending;

            RefreshProducts();
        }
        private void RefreshProducts()
        {
            AllVideos = video.GetWords(start, itemCount, sortColumn, ascending, out totalItems, wordid, lk);

            //NotifyPropertyChanged("Start");
            //NotifyPropertyChanged("End");
            //NotifyPropertyChanged("TotalItems");
        }















        public int WordCount1()
        {
            return word.WordCount(WordType.Numberslitle10);
        }
        public int WordCount2()
        {
            return word.WordCount(WordType.Numberslarger10);
        }
        public int WordCount3()
        {
            return word.WordCount(WordType.Letters);
        }
        public int WordCount4()
        {
            return word.WordCount(WordType.Words_By_Signs);
        }
        public int WordCount5()
        {
            return word.WordCount(WordType.Words_By_Letters);
        }
        public int WordCount6()
        {
            return word.WordCount(WordType.Sentences_By_Words);
        }
        public int WordCount7()
        {
            return word.WordCount(WordType.Sentences_By_Signs);
        }
        public int WordCount8()
        {
            return word.WordCount(WordType.Arbitrary_Sentences);
        }



        public int VideoCount1(LeapKinnectType mlk)
        {
            return word.VideoCount(WordType.Numberslitle10, mlk);
        }
        public int VideoCount2(LeapKinnectType mlk)
        {
            return word.VideoCount(WordType.Numberslarger10, mlk);
        }
        public int VideoCount3(LeapKinnectType mlk)
        {
            return word.VideoCount(WordType.Letters, mlk);
        }
        public int VideoCount4(LeapKinnectType mlk)
        {
            return word.VideoCount(WordType.Words_By_Signs, mlk);
        }
        public int VideoCount5(LeapKinnectType mlk)
        {
            return word.VideoCount(WordType.Words_By_Letters, mlk);
        }
        public int VideoCount6(LeapKinnectType mlk)
        {
            return word.VideoCount(WordType.Sentences_By_Words, mlk);
        }
        public int VideoCount7(LeapKinnectType mlk)
        {
            return word.VideoCount(WordType.Sentences_By_Signs, mlk);
        }
        public int VideoCount8(LeapKinnectType mlk)
        {
            return word.VideoCount(WordType.Arbitrary_Sentences, mlk);
        }


        public List<listcustomechart> PerVideoPartCount1(LeapKinnectType mlk)
        {
            return word.PerVideoCount(WordType.Numberslitle10, mlk);
        }
        public List<listcustomechart> PerVideoPartCount2(LeapKinnectType mlk)
        {
            return word.PerVideoCount(WordType.Numberslarger10, mlk);
        }
        public List<listcustomechart> PerVideoPartCount3(LeapKinnectType mlk)
        {
            return word.PerVideoCount(WordType.Letters, mlk);
        }
        public List<listcustomechart> PerVideoPartCount4(LeapKinnectType mlk)
        {
            return word.PerVideoCount(WordType.Words_By_Signs, mlk);
        }
        public List<listcustomechart> PerVideoPartCount5(LeapKinnectType mlk)
        {
            return word.PerVideoCount(WordType.Words_By_Letters, mlk);
        }
        public List<listcustomechart> PerVideoPartCount6(LeapKinnectType mlk)
        {
            return word.PerVideoCount(WordType.Sentences_By_Words, mlk);
        }
        public List<listcustomechart> PerVideoPartCount7(LeapKinnectType mlk)
        {
            return word.PerVideoCount(WordType.Sentences_By_Signs, mlk);
        }
        public List<listcustomechart> PerVideoPartCount8(LeapKinnectType mlk)
        {
            return word.PerVideoCount(WordType.Arbitrary_Sentences, mlk);
        }







        public LeapKinnectType leapkinnecttype
        {
            get { return VideoInfo.LeapKinnectType; }
            set
            {
                VideoInfo.LeapKinnectType = value;
                RaisePropertyChanged("leapkinnecttype");
                Validator.Validate(() => leapkinnecttype);
            }
        }

        public int video_id
        {
            get { return VideoInfo.video_id; }
            set
            {
                VideoInfo.video_id = value;
                RaisePropertyChanged("video_id");
                //Validator.Validate(() => Pid);
            }
        }
        private Words _eword;
        public Words eword
        {
            get
            {
                return _eword;
            }
            set
            {
                _eword = value;
                RaisePropertyChanged("eword");
            }
        }
        //public byte? VideoType
        //{
        //    get { return VideoInfo.VideoType; }
        //    set
        //    {
        //        VideoInfo.VideoType = value;
        //        RaisePropertyChanged("VideoType");
        //        //Validator.Validate(() => Pid);
        //    }
        //}
        /*public string Name
        {
            get { return VideoInfo.Words.Name; }
            set
            {
                VideoInfo.Words.Name = value;
                RaisePropertyChanged("Name");
                //Validator.Validate(() => Name);
            }
        }
        public WordType wordType
        {
            get { return VideoInfo.Words.WordType; }
            set
            {
                VideoInfo.Words.WordType = value;
                RaisePropertyChanged("wordType");
                //Validator.Validate(() => wordType);
            }
        }

        public int? Langid
        {
            get { return VideoInfo.Words.lang_id; }
            set
            {
                VideoInfo.Words.lang_id = value;
                RaisePropertyChanged("Langid");
                //Validator.Validate(() => Pid);
            }
        }
        private Languages _lang;
        public Languages elang
        {
            get
            {
                return _lang;
            }
            set
            {
                _lang = value;
                RaisePropertyChanged("elang");
            }
        }*/
        public string KinnectFilePath
        {
            get { return VideoInfo.KinnectFilePath; }
            set
            {
                VideoInfo.KinnectFilePath = destinationFile;
                RaisePropertyChanged("KinnectFilePath");
                //Validator.Validate(() => FilePath);
            }
        }
        public string LeapFilePath
        {
            get { return VideoInfo.LeapFilePath; }
            set
            {
                VideoInfo.LeapFilePath = destinationFile;
                RaisePropertyChanged("LeapFilePath");
                //Validator.Validate(() => FilePath);
            }
        }
        public int? User_id
        {
            get { return VideoInfo.User_id; }
            set
            {
                VideoInfo.User_id = value;
                RaisePropertyChanged("User_id");
                Validator.Validate(() => User_id);
            }
        }
        private User _user;
        public User euser
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                RaisePropertyChanged("euser");
            }
        }
        private bool? isValid;
        public bool? IsValid
        {
            get { return isValid; }
            private set
            {
                isValid = value;
                RaisePropertyChanged("IsValid");
            }
        }
        private string validationErrorsString;
        public string ValidationErrorsString
        {
            get { return validationErrorsString; }
            private set
            {
                validationErrorsString = value;
                RaisePropertyChanged("ValidationErrorsString");
            }
        }

        private void ConfigureValidationRules()
        {
            //Validator.AddRequiredRule(() => FilePath, "لطفا ویدئوی مربوطه را وارد کنید");

            Validator.AddRule(() => euser,
                                () =>
                                {
                                    if (euser == null)
                                    {
                                        return RuleResult.Invalid("لطفا فرد وارد کننده لغت را وارد کنید");
                                    }
                                    return RuleResult.Valid();
                                });
            Validator.AddRule(() => User_id,
                                () =>
                                {
                                    if (User_id == 0)
                                    {
                                        return RuleResult.Invalid("لطفا فرد وارد کننده لغت را وارد کنید");
                                    }
                                    return RuleResult.Valid();
                                });
        }

        private void Validate()
        {
            var uiThread = TaskScheduler.FromCurrentSynchronizationContext();

            Validator.ValidateAllAsync().ContinueWith(r =>
                OnValidateAllCompleted(r.Result), uiThread);

        }

        private void OnValidateAllCompleted(MvvmValidation.ValidationResult validationResult)
        {
            UpdateValidationSummary(validationResult);
        }

        private void OnValidationResultChanged(object sender, ValidationResultChangedEventArgs e)
        {
            if (!IsValid.GetValueOrDefault(true))
            {
                MvvmValidation.ValidationResult validationResult = Validator.GetResult();

                UpdateValidationSummary(validationResult);
            }
        }

        private void UpdateValidationSummary(MvvmValidation.ValidationResult validationResult)
        {
            IsValid = validationResult.IsValid;
            ValidationErrorsString = validationResult.ToString();
        }

    }
}