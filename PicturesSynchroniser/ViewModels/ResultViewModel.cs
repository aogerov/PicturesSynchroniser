using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using PicturesSynchroniser.BaseModels;
using PicturesSynchroniser.Behavior;
using PicturesSynchroniser.Models;

namespace PicturesSynchroniser.ViewModels
{
    public class ResultViewModel : BaseViewModel
    {
        private ObservableCollection<PictureModel> picturesForRollback;

        private bool renameIsSuccessful;
        private string successResultText;
        private bool renameFailed;
        private string failOptionsText;

        public ResultViewModel()
        {
            this.RollbackPictures = new RelayCommand(this.RollbackPicturesCommand);
        }

        public ICommand RollbackPictures { get; private set; }

        public bool RenameIsSuccessful
        {
            get
            {
                return this.renameIsSuccessful;
            }
            set
            {
                this.renameIsSuccessful = value;
                this.OnPropertyChanged("RenameIsSuccessful");
            }
        }

        public string SuccessResultText
        {
            get
            {
                return this.successResultText;
            }
            set
            {
                this.successResultText = value;
                this.OnPropertyChanged("SuccessResultText");
            }
        }

        public bool RenameFailed
        {
            get
            {
                return this.renameFailed;
            }
            set
            {
                this.renameFailed = value;
                this.OnPropertyChanged("RenameFailed");
            }
        }

        public string FailOptionsText
        {
            get
            {
                return this.failOptionsText;
            }
            set
            {
                this.failOptionsText = value;
                this.OnPropertyChanged("FailOptionsText");
            }
        }

        public override void LoadState(object navParameter, Dictionary<string, object> viewModelState)
        {
            if (viewModelState != null)
            {
                this.picturesForRollback = viewModelState["picturesForRollback"] as ObservableCollection<PictureModel>;
                this.RenameIsSuccessful = (bool)viewModelState["renameIsSuccessful"];
                this.SuccessResultText = (string)viewModelState["successResultText"];
                this.RenameFailed = (bool)viewModelState["renameFailed"];
                this.FailOptionsText = (string)viewModelState["failOptionsText"];
            }
            else if (navParameter != null)
            {
                var resultModel = navParameter as ResultModel;
                if (resultModel.IsSuccessful)
                {
                    this.SetSuccessOutput(resultModel);
                }
                else
                {
                    this.SetFailureOutput(resultModel);
                }
            }

            this.AddShareContract();
            this.IsReadyToProceed = true;
            this.IsAllLoaded = true;
        }

        public override void SaveState(Dictionary<string, object> viewModelState)
        {
            viewModelState["picturesForRollback"] = this.picturesForRollback;
            viewModelState["renameIsSuccessful"] = this.renameIsSuccessful;
            viewModelState["successResultText"] = this.successResultText;
            viewModelState["renameFailed"] = this.renameFailed;
            viewModelState["failOptionsText"] = this.failOptionsText;
            
            this.RemoveShareContract();
        }

        private async void RollbackPicturesCommand(object parameter)
        {
            int rollbackFailCount = 0;
            var errorMessages = new StringBuilder();

            foreach (var picture in this.picturesForRollback)
            {
                try
                {
                    if (picture.OriginalFileName != picture.NewFileName)
                    {
                        await picture.StorageFile.RenameAsync(picture.OriginalFileName);
                    }
                }
                catch (Exception ex)
                {
                    rollbackFailCount++;
                    errorMessages.Append("Error " + rollbackFailCount + ": " + ex.Message);
                }
            }

            if (rollbackFailCount == 0)
            {
                this.SuccessResultText = picturesForRollback.Count() + " pictures rollbacked successfully";
            }
            else
            {
                this.SuccessResultText = rollbackFailCount + " pictures failed on rollback. Check your file names manually.";
            }

            this.RenameIsSuccessful = true;
            this.RenameFailed = false;
        }

        private void SetSuccessOutput(ResultModel resultModel)
        {
            this.SuccessResultText = resultModel.RenamedPicturesCount + " pictures renamed successfully. ";
            if (resultModel.RenamedPicturesCount != resultModel.PicturesForRollback.Count())
            {
                this.SuccessResultText += (resultModel.PicturesForRollback.Count() - resultModel.RenamedPicturesCount) +
                                          " pictures maintain with their original file name.";
            }

            this.RenameIsSuccessful = true;
            this.RenameFailed = false;
        }

        private void SetFailureOutput(ResultModel resultModel)
        {
            this.picturesForRollback = resultModel.PicturesForRollback;
            this.FailOptionsText = "One or more pictures failed on rename.";
            this.FailOptionsText += "\r\nSystem message: " + resultModel.ErrorMessage;
            this.FailOptionsText += "\r\nDo you want to rollback your original file names?";
            this.RenameFailed = true;
            this.RenameIsSuccessful = false;
        }
    }
}