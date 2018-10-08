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
    public class Words_ViewModel : ValidatableViewModelBase
    {
        IUnitOfWork _uow;
        public ObservableCollection<WordsModel> AllWords { get; set; }
        public ObservableCollection<DropDownItems> AllLanguages { get; set; }
        public IList<DropDownItems> allWordTypes { get; set; }

        public Collection<rItem> rItems { get; set; }
        public Collection<lItem> lItems { get; set; }
        public PlotModel Model1 { get; set; }

        public WordsModel WordsInfo { set; get; }
        private IWords word { set; get; }
        private IOptionService option { set; get; }
        private ILanguages language { set; get; }
        private AppConfig app { set; get; }

        public ListItems listitem;

        private MSChartVM chatmodel;

        private WordType? _wt;
        public WordType? wt
        {
            get { return _wt; }
            set { _wt = value; }
        }
        //public WordType? wt { set; get; }

        private string _searchWordName;
        public string SearchWordName
        {
            get { return _searchWordName; }
            set { _searchWordName = value; }
        }

        public ObservableCollection<DropDownItems> GetAllLanguages()
        {
            ObservableCollection<LanguageModel> langu = language.GetAll();
            var baselines = new ObservableCollection<DropDownItems>();
            for(int i=0;i<langu.Count;i++)
            {
                baselines.Add(new DropDownItems() { ID = langu[i].lang_id, Value = langu[i].Name });
            }
            return baselines;
        }
        public Words_ViewModel(WordsModel model)
        {

        }
        private int _tt;
        public int TT
        {
            get { return _tt; }
            set { _tt = value; }
        }
        public Words_ViewModel(WordsModel model, int tt, IUnitOfWork uw)
        {
            _uow = uw;
            Contract.Requires(model != null);
            WordsInfo = model;
            word = new WordsService(_uow);
            option = new OptionService(_uow);
            language = new LanguageServise(_uow);
            listitem = new ListItems();

            chatmodel = new MSChartVM();
            //chatmodel.BarChart();
            charttest = chatmodel.BarSeriesCollection;
            TT = tt;
            AllLanguages = GetAllLanguages();
            allWordTypes = listitem.GetWordType();
            app = option.GetAll();
            
            RefreshProducts();

            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
        }
        public Words_ViewModel(WordsModel model, WordType? wtype, int tt, IUnitOfWork uw)
        {
            _uow = uw;
            Contract.Requires(model != null);
            WordsInfo = model;
            word = new WordsService(_uow);
            option = new OptionService(_uow);
            language = new LanguageServise(_uow);
            listitem = new ListItems();

            chatmodel = new MSChartVM();
            //chatmodel.BarChart();
            charttest = chatmodel.BarSeriesCollection;
            TT = tt;
            AllLanguages = GetAllLanguages();
            allWordTypes = listitem.GetWordType();
            app = option.GetAll();

            wt = wtype;

            RefreshProducts();

            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
        }

        private WordsModel _gridselecteditem;
        public WordsModel GridSelectedItem
        {
            get { return _gridselecteditem; }
            set
            {
                _gridselecteditem = value;
                RaisePropertyChanged("GridSelectedItem");
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
                if (word_id != 0)
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
                        if (string.IsNullOrEmpty(Name))
                        {
                            MessageBox.Show("Please fill the fields.");
                            return;
                        }
                        else
                        {
                            //if (!string.IsNullOrEmpty(destinationFile))
                            //{
                            //    if (!Directory.Exists(mypath))
                            //    {
                            //        Directory.CreateDirectory(app.FileUrl);
                            //        File.Copy(selectedpic, destinationFile, true);
                            //    }
                            //    else
                            //    {
                            //        File.Copy(selectedpic, destinationFile, true);
                            //    }
                            //    WordsInfo.FilePath = destinationFile.Remove(0, app.FileUrl.Length);
                            //}
                            WordsInfo.WordType = wordType;//UtilityClass.ParseEnum<WordType>(SelectedWordType.ID.Value.ToString());//wordType;
                    //        WordsInfo.User_id = User_id;
                            WordsInfo.Name = Name;
                            //WordsInfo.Languages = elang;
                            WordsInfo.lang_id = SelectedLanguage.ID.Value;

                            word.Create(WordsInfo);
                            MessageBox.Show("Successful: Item is created");
                            RefreshProducts();
                            Messenger.Default.Send(AllWords, "InsertedService");
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

        public RelayCommand<string> _searchCommand { get; private set; }
        public RelayCommand<string> SearchCommand
        {
            get
            {
                return _searchCommand
                      ?? (_searchCommand = new RelayCommand<string>(
                          searchInfo));
            }
        }

        private void searchInfo(string sres)
        {
            start = 0;
            itemCount = 5;
            totalItems = 0;
            //TotalItems = 0;
            Start = 0;
            //End = 0;

            firstCommand = null;
            previousCommand = null;
            nextCommand = null;
            lastCommand = null;

            RefreshProducts();
            //MessageBox.Show("T="+TotalItems.ToString()+" t="+totalItems.ToString()+
            //    " start="+Start+" end="+End);//AllWords.Count.ToString()+SearchWordName);
            //TotalItems = totalItems;
            //string s = "";
            //foreach (var item in AllWords)
            //{
            //    s += item.Name + " " + item.WordType + "\n";
            //}
            //MessageBox.Show(s);
            Messenger.Default.Send(AllWords, "MySearchNavigationService");

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
                if (word_id == 0)
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
                        if (string.IsNullOrEmpty(Name) && SelectedLanguage.ID.HasValue && SelectedWordType.ID.HasValue)
                        {
                            MessageBox.Show("Please fill the fields.");
                            return;
                        }
                        else
                        {
                            WordsInfo.word_id = word_id;

                            WordsInfo.Name = Name;
                            WordsInfo.lang_id = SelectedLanguage.ID.Value;
                            //WordsInfo.lang_id = SelectedLanguage.ID.Value;
                            WordsInfo.WordType = UtilityClass.IntToWordType(SelectedWordType.ID.Value);
                            //         WordsInfo.User_id = User_id;

                          
                            word.Update(WordsInfo);
                            MessageBox.Show("Successful: Item is Updated");
                            RefreshProducts();
                            Messenger.Default.Send(AllWords, "InsertedService");
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
                if (word_id == 0)
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

                    if (word.CheckIsVideoInWord(WordsInfo.word_id))
                    {
                        System.Windows.MessageBoxResult dialogResult = MessageBox.Show("This language has the recorded videos. \n All the videos woould be deleted. \n We will not delete it here.\n If you insist, delete it manully in the database file or create new database (in App_Data folder)."
                           , "Warning", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Question, System.Windows.MessageBoxResult.Cancel);
                        //if (dialogResult == System.Windows.MessageBoxResult.OK)
                        //{
                        //    //word.DeleteByLanguage(LanguageInfo.lang_id, app);
                        //    //language.Delete(LanguageInfo.lang_id);
                        //    //MessageBox.Show("Success language Remove");
                        //    //RefreshProducts();
                        //    //Messenger.Default.Send(AllLanguages, "InsertedService");
                        //}
                        //else if (dialogResult == System.Windows.MessageBoxResult.Cancel)
                        //{
                        //    //do something else
                        //}
                        Validator.Reset();
                    }
                    else
                    {
                        word.Delete(WordsInfo.word_id);
                        MessageBox.Show("Successful: Item is Removed");
                        RefreshProducts();
                        Messenger.Default.Send(AllWords, "InsertedService");
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
                //}
            };
        }











        private int start = 0, itemCount = 5;
        private string sortColumn = "word_id";
        private bool ascending = true;
        private int totalItems = 0;
        private NavigationReplayCommand firstCommand;
        private NavigationReplayCommand previousCommand;
        private NavigationReplayCommand nextCommand;
        private NavigationReplayCommand lastCommand;
        public int Start
        {
            get { return start + 1; }
            set
            {
                start = value;
                //RaisePropertyChanged("Start");
            }
        }
        public int End
        {
            get { return start + itemCount < totalItems ? start + itemCount : totalItems; }
            //set
            //{
            //    RaisePropertyChanged("End");
            //}
            //set { start = 0; itemCount = 0; totalItems = 0;}
        }
        public int TotalItems
        {
            get { return totalItems; }
            //set
            //{
            //    RaisePropertyChanged("TotalItems");
            //}
            //set { totalItems = value; }
        }
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
            if (TT == 2/* && !wt.HasValue*/)
                AllWords = word.GetWords(start, itemCount, sortColumn, ascending, out totalItems, SearchWordName);
            else if (TT == 1)
                AllWords = word.GetWords(start, itemCount, sortColumn, ascending, out totalItems, wt, SearchWordName);


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
            return word.VideoCount(WordType.Sentences_By_Words,mlk);
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





     
        private BindableCollection<Series> _charttest;
        public BindableCollection<Series> charttest
        {
            get { return _charttest; }
            set { _charttest = value; }
        }











        public int word_id
        {
            get { return WordsInfo.word_id; }
            set
            {
                WordsInfo.word_id = value;
                RaisePropertyChanged("word_id");
                //Validator.Validate(() => Pid);
            }
        }
        public string Name
        {
            get { return WordsInfo.Name; }
            set
            {
                WordsInfo.Name = value;
                RaisePropertyChanged("Name");
                Validator.Validate(() => Name);
            }
        }
        /*public string FilePath
        {
            get { return WordsInfo.FilePath; }
            set
            {
                WordsInfo.FilePath = destinationFile;
                RaisePropertyChanged("FilePath");
                //Validator.Validate(() => FilePath);
            }
        }*/
        private DropDownItems _selectedLanguage;// = new DropDownItems();
        public DropDownItems SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value; 
                RaisePropertyChanged("SelectedLanguage");
                Validator.Validate(() => SelectedLanguage);
            }
        }

        private DropDownItems _selectedWordType;// = new DropDownItems();
        public DropDownItems SelectedWordType
        {
            get { return _selectedWordType; }
            set
            {
                _selectedWordType = value;
                RaisePropertyChanged("SelectedWordType");
                Validator.Validate(() => SelectedWordType);
            }
        }
        public WordType wordType
        {
            get { return WordsInfo.WordType; }
            set
            {
                WordsInfo.WordType = value;
                RaisePropertyChanged("wordType");
                Validator.Validate(() => wordType);
            }
        }
        public int? videoCount
        {
            get { return WordsInfo.videoCount; }
            set
            {
                WordsInfo.videoCount = value;
                RaisePropertyChanged("videoCount");
                //Validator.Validate(() => Pid);
            }
        }
        public int Langid
        {
            get { return WordsInfo.Languages.lang_id; }
            set
            {
                WordsInfo.lang_id = value;
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
        }
        /*public int User_id
        {
            get { return WordsInfo.User_id; }
            set
            {
                WordsInfo.User_id = value;
                RaisePropertyChanged("User_id");
                //Validator.Validate(() => Pid);
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
        }*/
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
            Validator.AddRequiredRule(() => Name, "لطفا عبارت مورد نظر را وارد کنید");
            Validator.AddRequiredRule(() => wordType, "لطفا نوع عبارت را وارد کنید");


            Validator.AddRequiredRule(() => SelectedWordType, "لطفا نوع لغت را انتخاب کنید");
            Validator.AddRule(() => SelectedWordType,
                         () =>
                         {
                             if (SelectedWordType.ID == null || object.Equals(SelectedWordType.ID, string.Empty))
                             {
                                 return RuleResult.Invalid("نوع لغت نامعتبر است");
                             }
                             else
                             {
                                 wordType = UtilityClass.ParseEnum<WordType>(SelectedWordType.ID.Value.ToString());
                             }

                             return RuleResult.Valid();
                         });


            Validator.AddRequiredRule(() => SelectedLanguage, "لطفا زبان مورد نظر این لغت را انتخاب کنید");
            Validator.AddRule(() => SelectedLanguage,
                         () =>
                         {
                             if (SelectedLanguage.ID == null || object.Equals(SelectedLanguage.ID, string.Empty))
                             {
                                 return RuleResult.Invalid("زبان لغت نامعتبر است");
                             }
                             else
                             {
                                 elang = new Languages { lang_id = SelectedLanguage.ID.Value, Name = SelectedLanguage.Value };
                             }

                             return RuleResult.Valid();
                         });
            //Validator.AddRule(() => Langid,
            //                    () =>
            //                    {
            //                        if (Langid == 0)
            //                        {
            //                            return RuleResult.Invalid("لطفا زبان مورد نظر این لغت را انتخاب کنید");
            //                        }
            //                        return RuleResult.Valid();
            ////                    });
            //Validator.AddRule(() => euser,
            //                    () =>
            //                    {
            //                        if (euser == null)
            //                        {
            //                            return RuleResult.Invalid("لطفا فرد وارد کننده لغت را وارد کنید");
            //                        }
            //                        return RuleResult.Valid();
            //                    });
            //Validator.AddRule(() => User_id,
            //                    () =>
            //                    {
            //                        if (User_id == 0)
            //                        {
            //                            return RuleResult.Invalid("لطفا فرد وارد کننده لغت را وارد کنید");
            //                        }
            //                        return RuleResult.Valid();
            //                    });

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