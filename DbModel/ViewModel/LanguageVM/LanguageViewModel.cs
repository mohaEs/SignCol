using DbModel.Command;
using DbModel.Context;
using DbModel.Infrastructure;
using DbModel.Services;
using DbModel.Services.Interfaces;
using DbModel.ViewModel.OptionVM;
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

namespace DbModel.ViewModel.LanguageVM
{
    public class LanguageViewModel : ValidatableViewModelBase
    {
        IUnitOfWork _uow;
        public ObservableCollection<LanguageModel> AllLanguages { get; set; }

        public LanguageModel LanguageInfo { set; get; }
        private ILanguages language { set; get; }
        private IWords word { set; get; }
        private IOptionService option { set; get; }
        private AppConfig app { set; get; }

        public ListItems listitem;
        public LanguageViewModel(LanguageModel model)
        {

        }
        public LanguageViewModel(LanguageModel model, IUnitOfWork uw)
        {
            Contract.Requires(model != null);
            _uow = uw;
            LanguageInfo = model;
            language = new LanguageServise(_uow);
            word = new WordsService(_uow);
            option = new OptionService(_uow);
            listitem = new ListItems();
            app = option.GetAll();

            RefreshProducts();

            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
        }

        private LanguageModel _gridselecteditem;
        public LanguageModel GridSelectedItem
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
                if (lang_id != 0)
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
                        LanguageInfo.Name = Name;

                        language.Create(LanguageInfo);
                        MessageBox.Show("Successful: language is created.");
                        RefreshProducts();
                        Messenger.Default.Send(AllLanguages, "InsertedService");
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
                if (lang_id == 0)
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
                        LanguageInfo.lang_id = lang_id;
                        LanguageInfo.Name = Name;

                        language.Update(LanguageInfo);
                        MessageBox.Show("Successful: language is updated");
                        RefreshProducts();
                        Messenger.Default.Send(AllLanguages, "InsertedService");
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
                if (lang_id == 0)
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
                    if (language.CheckIsLanguageInWord(LanguageInfo.lang_id))
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
                        language.Delete(LanguageInfo.lang_id);
                        MessageBox.Show("Successful: language is Removed");
                        RefreshProducts();
                        Messenger.Default.Send(AllLanguages, "InsertedService");
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
                //    }
            };
        }








        private int start = 0, itemCount = 5;
        private string sortColumn = "lang_id";
        private bool ascending = true;
        public int totalItems = 0;
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
        public void RefreshProducts()
        {
            AllLanguages = language.GetLanguages(start, itemCount, sortColumn, ascending, out totalItems);

            //NotifyPropertyChanged("Start");
            //NotifyPropertyChanged("End");
            //NotifyPropertyChanged("TotalItems");
        }
















        public int lang_id
        {
            get { return LanguageInfo.lang_id; }
            set
            {
                LanguageInfo.lang_id = value;
                RaisePropertyChanged("lang_id");
                //Validator.Validate(() => Pid);
            }
        }
        public string Name
        {
            get { return LanguageInfo.Name; }
            set
            {
                LanguageInfo.Name = value;
                RaisePropertyChanged("Name");
                Validator.Validate(() => Name);
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
            Validator.AddRequiredRule(() => Name, "لطفا نام زبان مورد نظر را وارد کنید");
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
