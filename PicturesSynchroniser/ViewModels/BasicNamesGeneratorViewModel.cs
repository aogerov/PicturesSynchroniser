using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PicturesSynchroniser.BaseModels;
using PicturesSynchroniser.Behavior;
using PicturesSynchroniser.FileAccess;
using PicturesSynchroniser.Models;
using PicturesSynchroniser.Services;

namespace PicturesSynchroniser.ViewModels
{
    public class BasicNamesGeneratorViewModel : BaseViewModel
    {
        private ObservableCollection<PictureModel> allPictures;
        private CameraTimeSynchroniserModel cameraTimeSynchroniser;
        private FileNameTemplateModel fileNameTemplate;
        private int picturesWithInvalidMetaCount;

        public BasicNamesGeneratorViewModel()
        {
            this.allPictures = new ObservableCollection<PictureModel>();
            this.cameraTimeSynchroniser = new CameraTimeSynchroniserModel();
            this.fileNameTemplate = new FileNameTemplateModel();

            this.GeneratePicturesNames = new RelayCommand(this.GeneratePicturesNamesCommand);
            this.LoadPictures = new RelayCommand(this.LoadPicturesCommand);
        }

        public ICommand GeneratePicturesNames { get; private set; }

        public ICommand LoadPictures { get; private set; }

        public CameraTimeSynchroniserModel CameraTimeSynchroniser
        {
            get
            {
                return this.cameraTimeSynchroniser;
            }
            set
            {
                this.cameraTimeSynchroniser = value;
                this.OnPropertyChanged("CameraTimeSynchroniser");
            }
        }

        public FileNameTemplateModel FileNameTemplate
        {
            get
            {
                return this.fileNameTemplate;
            }
        }

        public override async void LoadState(object navParameter, Dictionary<string, object> viewModelState)
        {
            if (viewModelState != null)
            {
                this.allPictures = viewModelState["allPictures"] as ObservableCollection<PictureModel>;
                this.cameraTimeSynchroniser = viewModelState["cameraTimeSynchroniser"] as CameraTimeSynchroniserModel;
                this.fileNameTemplate = viewModelState["fileNameTemplate"] as FileNameTemplateModel;
                this.picturesWithInvalidMetaCount = (int)viewModelState["picturesWithInvalidMetaCount"];
            }
            else if (navParameter != null)
            {
                var navParameterAsList = navParameter as List<object>;

                var allPicturesAsNavParameter = navParameterAsList[0] as List<PictureModel>;
                this.picturesWithInvalidMetaCount = (int)navParameterAsList[1];

                var sortedByDatePictures = allPicturesAsNavParameter.OrderBy(p => p.StorageFile.DateCreated).ToList();
                if (picturesWithInvalidMetaCount > 0)
                {
                    var picturesWithInvalidMeta = new List<PictureModel>();
                    foreach (var picture in sortedByDatePictures)
                    {
                        var cameraInfo = await CameraMetaReader.GetCameraMeta(picture.StorageFile);
                        if (cameraInfo[0] == UnknownMeta.CameraManufacturer && cameraInfo[1] == UnknownMeta.CameraModel)
                        {
                            picturesWithInvalidMeta.Add(picture);
                        }
                        else
                        {
                            this.allPictures.Add(picture);
                        }
                    }

                    foreach (var picture in picturesWithInvalidMeta)
                    {
                        this.allPictures.Add(picture);
                    }
                }

                this.SetPicturesForTimeMerging();
            }

            this.AddShareContract();
            this.IsReadyToProceed = true;
            this.IsAllLoaded = true;
        }

        public override void SaveState(Dictionary<string, object> viewModelState)
        {
            viewModelState["allPictures"] = this.allPictures;
            viewModelState["cameraTimeSynchroniser"] = this.cameraTimeSynchroniser;
            viewModelState["fileNameTemplate"] = this.fileNameTemplate;
            viewModelState["picturesWithInvalidMetaCount"] = this.picturesWithInvalidMetaCount;

            this.RemoveShareContract();
        }

        private async void GeneratePicturesNamesCommand(object parameter)
        {
            this.fileNameTemplate.SaveFileNameTemplate(this.GetType().Name);
            var picturesWithNewDateTime = new List<PictureModel>();
            this.SetPicturesNewDateTime(picturesWithNewDateTime);

            var sortedPicturesByDateTime = picturesWithNewDateTime.OrderBy(p => p.NewDateTime).ToList();
            NameGenerator.SetPicturesNewFileName(sortedPicturesByDateTime, this.fileNameTemplate);
            NameGenerator.SetPicturesUserDefinedFileName(sortedPicturesByDateTime);

            var picturesDataForRename = new List<object>();

            if (picturesWithInvalidMetaCount > 0)
            {
                await this.SplitValidAndInvalidPictures(picturesDataForRename, sortedPicturesByDateTime);
            }
            else
            {
                picturesDataForRename.Add(sortedPicturesByDateTime);
            }

            this.NavigationService.Navigate(ViewType.NewNamesSetter, picturesDataForRename);
        }

        private async void LoadPicturesCommand(object parameter)
        {
            var selectedFiles = await FilePickerController.GetPictures();
            if (selectedFiles != null && selectedFiles.Count != 0)
            {
                this.NavigationService.Navigate(ViewType.Basic, selectedFiles);
            }
        }

        private async Task SplitValidAndInvalidPictures(
            List<object> picturesDataForRename, List<PictureModel> sortedPicturesByDateTime)
        {
            var validCameraPictures = new List<PictureModel>();
            var invalidCameraPictures = new CameraPicturesModel();
            invalidCameraPictures.CameraManufacturer = UnknownMeta.CameraManufacturer;
            invalidCameraPictures.CameraModel = UnknownMeta.CameraModel;

            foreach (var picture in sortedPicturesByDateTime)
            {
                var cameraInfo = await CameraMetaReader.GetCameraMeta(picture.StorageFile);
                if (cameraInfo[0] == UnknownMeta.CameraManufacturer && cameraInfo[1] == UnknownMeta.CameraModel)
                {
                    invalidCameraPictures.Add(picture);
                }
                else
                {
                    validCameraPictures.Add(picture);
                }
            }

            picturesDataForRename.Add(validCameraPictures);
            picturesDataForRename.Add(invalidCameraPictures);
        }

        private void SetPicturesForTimeMerging()
        {
            this.SetCameraTimeSynchroniserModel();
            this.SetFileNameKeyword();
            this.LoadFileNameTemplate();
        }

        private void SetCameraTimeSynchroniserModel()
        {
            var firstPicture = this.allPictures.First();

            this.CameraTimeSynchroniser = new CameraTimeSynchroniserModel
            {
                StorageFile = firstPicture.StorageFile,
                Year = firstPicture.Year,
                Month = firstPicture.Month,
                Day = firstPicture.Day,
                Hour = firstPicture.Hour,
                Minute = firstPicture.Minute,
                Second = firstPicture.Second
            };
        }

        private void SetFileNameKeyword()
        {
            var pathParts = this.allPictures.First().StorageFile.Path.Split('\\');
            if (pathParts.Length >= 2)
            {
                string keyword = NameGenerator.GetLongestKeyword(pathParts);
                if (!string.IsNullOrEmpty(keyword))
                {
                    var keywordLetters = keyword.ToLowerInvariant().ToCharArray();
                    keywordLetters[0] = Char.ToUpperInvariant(keywordLetters[0]);
                    this.fileNameTemplate.Keyword = new string(keywordLetters);
                }
            }
        }

        private void LoadFileNameTemplate()
        {
            string fileType = this.allPictures.First().StorageFile.FileType;
            string generatorName = this.GetType().Name;
            this.fileNameTemplate.LoadFileNameTemplate(generatorName, fileType);
        }

        private void SetPicturesNewDateTime(List<PictureModel> picturesWithNewDateTime)
        {
            foreach (var picture in this.allPictures)
            {
                NameGenerator.SetNewDateTime(picture, cameraTimeSynchroniser);
                picturesWithNewDateTime.Add(picture);
            }
        }
    }
}