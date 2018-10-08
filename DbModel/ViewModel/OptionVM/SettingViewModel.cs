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
using Microsoft.Win32;
using System.Windows.Forms;

namespace DbModel.ViewModel.OptionVM
{
    public class SettingViewModel : ValidatableViewModelBase
    {
        IUnitOfWork _uow;
        public AppConfig OptionInfo { set; get; }
        public AppConfig AllOptions { set; get; }
        private IOptionService ops { set; get; }

        public SettingViewModel(AppConfig model)
        {

        }
        public SettingViewModel(AppConfig model, IUnitOfWork uw)
        {
            Contract.Requires(model != null);
            _uow = uw;
            OptionInfo = model;
            ops = new OptionService(_uow);
            AllOptions = ops.GetAll();

            FileUrl = AllOptions.FileUrl;
            //RefreshProducts();

            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
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
                //if (option_Id == 0)
                //{
                //    // MessageBox.Show("Please First Select Record For Update");
                //    return false;
                //}
                //else
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
                        //OptionInfo.option_Id = option_Id;
                        OptionInfo.FileUrl = FileUrl;

                        ops.Update(OptionInfo);
                        _uow.SaveChanges();
                        System.Windows.Forms.MessageBox.Show("Successful: Setting is updated.");
                        RefreshProducts();
                        Messenger.Default.Send(AllOptions, "InsertedService");


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
                        System.Windows.Forms.MessageBox.Show(sb.ToString());
                    }
                }
            };
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

            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Choose Video Directory";
                //dlg.SelectedPath = Text;
                dlg.ShowNewFolderButton = true;
                DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    FileUrl = dlg.SelectedPath;
                    AttachPicture = new Uri(dlg.SelectedPath);//UtilityClass.ByteToStream(Path.GetFullPath(OpenFileDialog.FileName));

                }
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



        private void RefreshProducts()
        {
            AllOptions = ops.GetAll();

            //NotifyPropertyChanged("Start");
            //NotifyPropertyChanged("End");
            //NotifyPropertyChanged("TotalItems");
        }
        //public int option_Id
        //{
        //    get { return OptionInfo.option_Id; }
        //    set
        //    {
        //        OptionInfo.option_Id = value;
        //        RaisePropertyChanged("option_Id");
        //        //Validator.Validate(() => Pid);
        //    }
        //}
        public string FileUrl
        {
            get { return OptionInfo.FileUrl; }
            set
            {
                OptionInfo.FileUrl = value;
                RaisePropertyChanged("FileUrl");
                Validator.Validate(() => FileUrl);
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
            Validator.AddRequiredRule(() => FileUrl, "لطفا مسیر فایل ها را وارد کنید");
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
