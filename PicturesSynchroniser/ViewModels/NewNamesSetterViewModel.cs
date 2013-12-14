using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using PicturesSynchroniser.BaseModels;
using PicturesSynchroniser.Behavior;
using PicturesSynchroniser.Events;
using PicturesSynchroniser.Models;
using PicturesSynchroniser.Services;

namespace PicturesSynchroniser.ViewModels
{
    public class NewNamesSetterViewModel : BaseViewModel
    {
        private ObservableCollection<PictureModel> picturesReadyForRename;
        private int picturesWithChangedNewName;
        private int picturesWithInavalidMetaCount;

        private PictureModel selectedPictureForEdit;

        public NewNamesSetterViewModel()
        {
            this.SystemMessage = new MessageModel();
            this.picturesReadyForRename = new ObservableCollection<PictureModel>();
            this.RenamePictures = new RelayCommand(RenamePicturesCommand);
        }

        public MessageModel SystemMessage { get; set; }

        public ICommand RenamePictures { get; private set; }

        public IEnumerable<PictureModel> PicturesReadyForRename
        {
            get
            {
                return this.picturesReadyForRename;
            }
            set
            {
                if (value != null)
                {
                    this.picturesReadyForRename.Clear();
                    foreach (var pictures in value)
                    {
                        this.picturesReadyForRename.Add(pictures);
                    }
                }
            }
        }

        public PictureModel SelectedPictureForEdit
        {
            get
            {
                return this.selectedPictureForEdit;
            }
            set
            {
                if (value != null)
                {
                    foreach (var picture in this.picturesReadyForRename)
                    {
                        picture.IsNewFileNameVisible = true;
                        picture.IsUserDefinedFileNameFieldVisible = false;
                    }

                    this.selectedPictureForEdit = value;
                    this.selectedPictureForEdit.IsNewFileNameVisible = false;
                    this.selectedPictureForEdit.IsUserDefinedFileNameFieldVisible = true;
                }
            }
        }

        public override void LoadState(object navParameter, Dictionary<string, object> viewModelState)
        {
            if (viewModelState != null)
            {
                this.picturesReadyForRename = viewModelState["picturesReadyForRename"] as ObservableCollection<PictureModel>;
                this.picturesWithChangedNewName = (int)viewModelState["picturesWithChangedNewName"];
                this.picturesWithInavalidMetaCount = (int)viewModelState["picturesWithInavalidMetaCount"];
                this.selectedPictureForEdit = viewModelState["selectedPictureForEdit"] as PictureModel;
            }
            else if (navParameter != null)
            {
                var picturesDataForRename = navParameter as List<object>;
                if (picturesDataForRename != null)
                {
                    this.picturesReadyForRename.Clear();

                    var picturesWithNewName = picturesDataForRename[0] as List<PictureModel>;
                    this.AddPicturesWithNewName(picturesWithNewName);

                    if (picturesDataForRename.Count > 1)
                    {
                        var invalidCameraPictures = picturesDataForRename[1] as CameraPicturesModel;
                        this.AddInvalidCameraPictures(invalidCameraPictures);
                    }

                    this.SetSystemMessage();
                }
            }

            this.AddShareContract();
            this.IsReadyToProceed = true;
            this.IsAllLoaded = true;
        }

        public override void SaveState(Dictionary<string, object> viewModelState)
        {
            viewModelState["picturesReadyForRename"] = this.picturesReadyForRename;
            viewModelState["picturesWithChangedNewName"] = this.picturesWithChangedNewName;
            viewModelState["picturesWithInavalidMetaCount"] = this.picturesWithInavalidMetaCount;
            viewModelState["selectedPictureForEdit"] = this.selectedPictureForEdit;
            viewModelState["isReadyToProceed"] = this.IsReadyToProceed;
            viewModelState["isAllLoaded"] = this.IsAllLoaded;
            
            this.RemoveShareContract();
        }

        private async void RenamePicturesCommand(object paramete)
        {
            bool allNewNamesAreUnique = this.CheckIfAllNewNamesAreUnique();
            if (!allNewNamesAreUnique)
            {
                this.SetRenameImpossibleMessageNamesNotUnique();
                return;
            }

            var fileNamesCollisions = this.GetFileNameCollisions();
            if (fileNamesCollisions.Count() != 0)
            {
                this.SetRenameImpossibleMessageCollisions(fileNamesCollisions);
                return;
            }

            try
            {
                int renamedPicturesCount = 0;

                foreach (var picture in this.picturesReadyForRename)
                {
                    if (picture.NewFileName != picture.OriginalFileName)
                    {
                        await picture.StorageFile.RenameAsync(picture.NewFileName);
                        renamedPicturesCount++;
                    }
                }

                this.RenameSuccess(renamedPicturesCount);
            }
            catch (Exception ex)
            {
                this.RenameFailure(ex.Message);
            }
        }

        private void AddPicturesWithNewName(List<PictureModel> picturesWithNewName)
        {
            if (picturesWithNewName != null)
            {
                foreach (var picture in picturesWithNewName)
                {
                    this.AddPicture(picture);
                }

                this.picturesWithChangedNewName =
                    picturesWithNewName.Where(p => p.IsNamedAsUserDefined == false).Count();
            }
        }

        private void AddInvalidCameraPictures(CameraPicturesModel invalidCameraPictures)
        {
            if (invalidCameraPictures != null)
            {
                foreach (var picture in invalidCameraPictures.CameraPictures)
                {
                    this.AddPicture(picture);
                }

                this.picturesWithInavalidMetaCount = invalidCameraPictures.PicturesCount;
            }
        }

        private void AddPicture(PictureModel picture)
        {
            picture.PictureNameChangedByUser += this.PictureNameChangedByUserEvent;
            this.picturesReadyForRename.Add(picture);
        }

        private void PictureNameChangedByUserEvent(object sender, PictureNameChangedByUserArgs e)
        {
            var picture = sender as PictureModel;
            if (string.IsNullOrEmpty(picture.UserDefinedFileName))
            {
                this.RollbackUserDefinedFileName(picture);
                return;
            }

            string userDefinedFullName = picture.UserDefinedFileName + picture.StorageFile.FileType.ToLower();
            var pictureNameAlreadyExists = this.picturesReadyForRename.FirstOrDefault(
                p => p.NewFileName.ToLower() == userDefinedFullName.ToLower());

            if (pictureNameAlreadyExists == null)
            {
                picture.NewFileName = userDefinedFullName;
                picture.IsNamedAsUserDefined = true;
                this.SetSystemMessage();
            }
            else
            {
                this.RollbackUserDefinedFileName(picture);
            }
        }

        private void RollbackUserDefinedFileName(PictureModel picture)
        {
            if (picture.NewFileName.Contains("."))
            {
                picture.UserDefinedFileName = picture.NewFileName.Substring(
                    0, picture.NewFileName.LastIndexOf('.'));
            }
            else
            {
                picture.UserDefinedFileName = picture.NewFileName;
            }
        }

        private bool CheckIfAllNewNamesAreUnique()
        {
            int uniqueCount = this.picturesReadyForRename.Select(p => p.NewFileName).Distinct().Count();
            if (uniqueCount == this.picturesReadyForRename.Count)
            {
                return true;
            }

            return false;
        }

        private IEnumerable<string> GetFileNameCollisions()
        {
            var originalFileNames = this.picturesReadyForRename.Select(p => p.OriginalFileName).ToList();
            var newFileNames = this.picturesReadyForRename.Select(p => p.NewFileName).ToList();

            var possibleCollisions = originalFileNames.Intersect(newFileNames);
            var realCollisions = new List<string>();
            foreach (var collisionFileName in possibleCollisions)
            {
                var collision = this.picturesReadyForRename.FirstOrDefault(p => p.NewFileName == collisionFileName);
                if (collision.NewFileName != collision.OriginalFileName)
                {
                    realCollisions.Add(collision.NewFileName);
                }
            }

            return realCollisions;
        }

        private void RenameSuccess(int renamedPicturesCount)
        {
            var successResultModel = new ResultModel
            {
                IsSuccessful = true,
                RenamedPicturesCount = renamedPicturesCount,
                PicturesForRollback = this.picturesReadyForRename
            };

            this.NavigationService.Navigate(ViewType.Result, successResultModel);
        }

        private void RenameFailure(string errorMessage)
        {
            var errorResultModel = new ResultModel
            {
                IsSuccessful = false,
                RenamedPicturesCount = this.picturesReadyForRename.Count,
                ErrorMessage = errorMessage,
                PicturesForRollback = this.picturesReadyForRename
            };

            this.NavigationService.Navigate(ViewType.Result, errorResultModel);
        }

        private void SetSystemMessage()
        {
            var incorrectNamedPicturesCount = this.picturesReadyForRename.Where(
                p => p.IsNamedAsUserDefined == false).Count();

            if (incorrectNamedPicturesCount == 0)
            {
                this.IsReadyToProceed = true;
                this.SystemMessage.IsAllReady = true;
                this.SystemMessage.Text = "Please check your file names carefully before renaming! You can change any file name by clicking over it.";
                this.SystemMessage.SetTimeVisibility(9000);
            }
            else
            {
                this.IsReadyToProceed = false;
                this.SystemMessage.IsAllReady = false;
                this.SystemMessage.Text = "Please check your file names carefully before renaming! You can change any file name by clicking over it." +
                                          "\r\nCurrently there are " + incorrectNamedPicturesCount + " pictures with different names and/or " +
                                          "with the original names marked in orange color.";

                this.SystemMessage.SetTimeVisibility(15000);
            }
        }

        private void SetRenameImpossibleMessageNamesNotUnique()
        {
            string systemMessage = "Please check your file names. One or more of them are same. Change those file names before proceed.";
            this.SystemMessage.Text = systemMessage;
            this.SystemMessage.IsAllReady = false;
            this.SystemMessage.SetTimeVisibility(10000);
        }

        private void SetRenameImpossibleMessageCollisions(IEnumerable<string> fileNamesCollisions)
        {
            string systemMessage = "The system found " + fileNamesCollisions.Count() +
                                   " naming collisions with your original file names. Change the following file names before continue:\r\n";

            var collisionFileNames = new StringBuilder();
            foreach (var fileName in fileNamesCollisions)
            {
                collisionFileNames.Append(fileName + "  ");
            }

            collisionFileNames.Length = collisionFileNames.Length - 2;
            systemMessage += collisionFileNames.ToString();

            this.SystemMessage.Text = systemMessage;
            this.SystemMessage.IsAllReady = false;
            this.SystemMessage.SetTimeVisibility(10000);
        }
    }
}