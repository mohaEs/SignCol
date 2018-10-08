using DbModel.Command;
using DbModel.Context;
using DbModel.Infrastructure;
using DbModel.Services;
using DbModel.Services.Interfaces;
using DbModel.ViewModel.OptionVM;
using DbModel.ViewModel.WordsVM;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MvvmValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace DbModel.ViewModel.UserVM
{
    public class User_ViewModel : ValidatableViewModelBase
    {
        IUnitOfWork _uow;
        public ObservableCollection<UserModel> AllUser { get; set; }

        public UserModel UserInfo { set; get; }
        private IUser user { set; get; }
        private IWords word { set; get; }
        private IVideo video { set; get; }
        private IOptionService option { set; get; }
        private AppConfig app { set; get; }

        public ListItems listitem;
        public User_ViewModel(UserModel model)
        {

        }
        public User_ViewModel(UserModel model, IUnitOfWork uw)
        {
            Contract.Requires(model != null);
            _uow = uw;
            UserInfo = model;
            user = new UserService(_uow);
            word = new WordsService(_uow);
            video = new VideoService(_uow);
            option = new OptionService(_uow);
            listitem = new ListItems();
            app = option.GetAll();

            RefreshProducts();

            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
        }

        private UserModel _gridselecteditem;
        public UserModel GridSelectedItem
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
                if (User_id != 0)
                {
                    return false;
                }
                else
                    return true;
            };
        }
        private Action saveInfo()
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
                            MessageBox.Show("لطفا فیلدها را پر نمایید");
                            return;
                        }
                        else
                        {
                            UserInfo.Age = Age;
                            UserInfo.Name = Name;
                            UserInfo.Phone = Phone;

                            user.Create(UserInfo);
                            MessageBox.Show("Successful: Performer is created.");
                            RefreshProducts();
                            Messenger.Default.Send(AllUser, "InsertedService");
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
                if (User_id == 0)
                {
                    // MessageBox.Show("Please First Select Record For Update");
                    return false;
                }
                else
                    return true;
            };
        }
        private Action UpdateInfo()
        {
            return () =>
            {
                Validate();
                MvvmValidation.ValidationResult validationResult = Validator.GetResult();
                if (validationResult.IsValid)
                {
                    try
                    {
                        UserInfo.User_id = User_id;
                        UserInfo.Age = Age;
                        UserInfo.Name = Name;
                        UserInfo.Phone = Phone;                    

                        user.Update(UserInfo);
                        MessageBox.Show("Successful: Performer is updated.");
                        RefreshProducts();
                        Messenger.Default.Send(AllUser, "InsertedService");
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
                if (User_id == 0)
                {
                    // MessageBox.Show("Please First Select Record For Update");
                    return false;
                }
                else
                    return true;
            };
        }
        private Action DeleteInfo()
        {
            return () =>
            {
                //Validate();
                //MvvmValidation.ValidationResult validationResult = Validator.GetResult();
                //if (validationResult.IsValid)
                //{
                try
                {
                    if (user.CheckUserHaveVideo(UserInfo.User_id))
                    {
                        System.Windows.MessageBoxResult dialogResult = MessageBox.Show("This performer has recorded vidoes. \n Are you sure you want to remove it? \n All the videos will be deleted too."
                           , "Warning", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question, System.Windows.MessageBoxResult.Cancel);
                        if (dialogResult == System.Windows.MessageBoxResult.Yes)
                        {
                            //word.DeleteByUser(UserInfo.User_id, app);
                            var uid = UserInfo.User_id;
                            ObservableCollection<VideoModel> all = video.GetAllUserVideos(uid);
                            foreach (var item in all)
                            {
                                if (!string.IsNullOrEmpty(item.KinnectFilePath))
                                {
                                    System.GC.Collect();
                                    System.GC.WaitForPendingFinalizers();
                                    string pa = app.FileUrl + item.KinnectFilePath;
                                    if(File.Exists(pa))
                                        File.Delete(pa);
                                }
                            }
                            video.DeleteByUser(uid);

                     //       user.Delete(uid);
                            MessageBox.Show("Successful: Performer is removed.");
                            RefreshProducts();
                            Messenger.Default.Send(AllUser, "InsertedService");
                        }
                        else if (dialogResult == System.Windows.MessageBoxResult.No)
                        {
                            //do something else
                        }
                        Validator.Reset();
                    }
                    else
                    {
                        var uid = UserInfo.User_id;
                        video.DeleteByUser(uid);

                        //       user.Delete(uid);
                        MessageBox.Show("Successful: Perfomer is Removed.");
                        RefreshProducts();
                        Messenger.Default.Send(AllUser, "InsertedService");
                        //user.Delete(UserInfo.User_id);
                        //MessageBox.Show("Success User Remove");
                        //RefreshProducts();
                        //Messenger.Default.Send(AllUser, "InsertedService");
                        //Validator.Reset();
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
        private string sortColumn = "User_id";
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
            AllUser = user.GetUsers(start, itemCount, sortColumn, ascending, out totalItems);

            //NotifyPropertyChanged("Start");
            //NotifyPropertyChanged("End");
            //NotifyPropertyChanged("TotalItems");
        }
















        public int User_id
        {
            get { return UserInfo.User_id; }
            set
            {
                UserInfo.User_id = value;
                RaisePropertyChanged("User_id");
                //Validator.Validate(() => Pid);
            }
        }
        public string Name
        {
            get { return UserInfo.Name; }
            set
            {
                UserInfo.Name = value;
                RaisePropertyChanged("Name");
                Validator.Validate(() => Name);
            }
        }
        public string Age
        {
            get { return UserInfo.Age; }
            set
            {
                UserInfo.Age = value;
                RaisePropertyChanged("Age");
                Validator.Validate(() => Age);
            }
        }
        public string Phone
        {
            get { return UserInfo.Phone; }
            set
            {
                UserInfo.Phone = value;
                RaisePropertyChanged("Phone");
                Validator.Validate(() => Phone);
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
        private int r;
        private void ConfigureValidationRules()
        {
            Validator.AddRequiredRule(() => Name, "Please add the name.");
            Validator.AddRequiredRule(() => Phone, "Please add phone number,");
            Validator.AddRequiredRule(() => Age, "Please add the age.");
            Validator.AddRule(() => Age,
                         () =>
                         {
                             if (Age == null && int.TryParse(Age,out r))
                             {
                                 return RuleResult.Invalid("Please set age by numbers.");
                             }
                             else
                             {
                                 Age = Age;
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