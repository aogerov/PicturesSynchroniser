using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PicturesSynchroniser.BaseModels;
using PicturesSynchroniser.Behavior;
using PicturesSynchroniser.FileAccess;
using PicturesSynchroniser.Models;
using PicturesSynchroniser.Services;

namespace PicturesSynchroniser.ViewModels
{
    public class WizardNamesGeneratorViewModel : BaseViewModel
    {
        private ObservableCollection<CameraPicturesModel> validCameraPictures;
        private CameraPicturesModel invalidCameraPictures;

        private CameraTimeSynchroniserModel mainCamera;
        private ObservableCollection<CameraTimeSynchroniserModel> otherCameras;
        private FileNameTemplateModel fileNameTemplate;

        public WizardNamesGeneratorViewModel()
        {
            this.validCameraPictures = new ObservableCollection<CameraPicturesModel>();
            this.otherCameras = new ObservableCollection<CameraTimeSynchroniserModel>();
            this.fileNameTemplate = new FileNameTemplateModel();

            this.GeneratePicturesNames = new RelayCommand(this.GeneratePicturesNamesCommand);
            this.LoadPictures = new RelayCommand(this.LoadPicturesCommand);
        }

        public ICommand GeneratePicturesNames { get; private set; }

        public ICommand LoadPictures { get; private set; }

        public CameraTimeSynchroniserModel MainCamera
        {
            get
            {
                return this.mainCamera;
            }
            set
            {
                this.mainCamera = value;
                this.OnPropertyChanged("MainCamera");
            }
        }

        public IEnumerable<CameraTimeSynchroniserModel> OtherCameras
        {
            get
            {
                return this.otherCameras;
            }
        }

        public FileNameTemplateModel FileNameTemplate
        {
            get
            {
                return this.fileNameTemplate;
            }
        }

        public override void LoadState(object navParameter, Dictionary<string, object> viewModelState)
        {
            if (viewModelState != null)
            {
                this.validCameraPictures = viewModelState["validCameraPictures"] as ObservableCollection<CameraPicturesModel>;
                this.invalidCameraPictures = viewModelState["invalidCameraPictures"] as CameraPicturesModel;
                this.mainCamera = viewModelState["mainCamera"] as CameraTimeSynchroniserModel;
                this.otherCameras = viewModelState["otherCameras"] as ObservableCollection<CameraTimeSynchroniserModel>;
                this.fileNameTemplate = viewModelState["fileNameTemplate"] as FileNameTemplateModel;
            }
            else if (navParameter != null)
            {
                var navParameterAsList = navParameter as List<object>;

                var validCameraPicturesAsNav = navParameterAsList[0] as ObservableCollection<CameraPicturesModel>;
                var orderedValidCameraPictures =
                    validCameraPicturesAsNav.OrderBy(c => c.CameraManufacturer).ThenBy(c => c.CameraModel);

                foreach (var cameraPictures in orderedValidCameraPictures)
                {
                    this.validCameraPictures.Add(cameraPictures);
                }

                this.invalidCameraPictures = navParameterAsList[1] as CameraPicturesModel;

                this.SetPicturesForTimeMerging();
            }

            this.AddShareContract();
            this.IsReadyToProceed = true;
            this.IsAllLoaded = true;
        }

        public override void SaveState(Dictionary<string, object> viewModelState)
        {
            viewModelState["validCameraPictures"] = this.validCameraPictures;
            viewModelState["invalidCameraPictures"] = this.invalidCameraPictures;
            viewModelState["mainCamera"] = this.mainCamera;
            viewModelState["otherCameras"] = this.otherCameras;
            viewModelState["fileNameTemplate"] = this.fileNameTemplate;
            
            this.RemoveShareContract();
        }

        private void GeneratePicturesNamesCommand(object parameter)
        {
            this.fileNameTemplate.SaveFileNameTemplate(this.GetType().Name);
            var picturesWithNewDateTime = new List<PictureModel>();
            this.SetPicturesNewDateTime(picturesWithNewDateTime);

            var sortedPicturesByDateTime = picturesWithNewDateTime.OrderBy(p => p.NewDateTime).ToList();
            NameGenerator.SetPicturesNewFileName(sortedPicturesByDateTime, this.fileNameTemplate);
            NameGenerator.SetPicturesUserDefinedFileName(sortedPicturesByDateTime);
            NameGenerator.SetNewNameToInvalidCameraPictures(this.invalidCameraPictures.CameraPictures, sortedPicturesByDateTime);
            NameGenerator.SetPicturesUserDefinedFileName(this.invalidCameraPictures.CameraPictures);

            var picturesDataForRename = new List<object>();
            picturesDataForRename.Add(sortedPicturesByDateTime);
            picturesDataForRename.Add(this.invalidCameraPictures);

            this.NavigationService.Navigate(ViewType.NewNamesSetter, picturesDataForRename);
        }

        private async void LoadPicturesCommand(object parameter)
        {
            var selectedFiles = await FilePickerController.GetPictures();
            if (selectedFiles != null && selectedFiles.Count != 0)
            {
                this.NavigationService.Navigate(ViewType.Wizard, selectedFiles);
            }
        }

        private void SetPicturesForTimeMerging()
        {
            var pictureTimeSynchroniserModels = this.GetPictureTimeSynchroniserModels();
            this.GetMainCamera(pictureTimeSynchroniserModels);
            this.GetOtherCamerasWithTimeDifference(pictureTimeSynchroniserModels);
            this.SetFileNameKeyword();
            this.LoadFileNameTemplate();
        }

        private List<CameraTimeSynchroniserModel> GetPictureTimeSynchroniserModels()
        {
            var pictureTimeSynchroniserModels = new List<CameraTimeSynchroniserModel>();

            foreach (var cameraPictures in this.validCameraPictures)
            {
                var pictureTimeSynchroniserModel = this.ParseCameraTimeSynchroniserModel(cameraPictures);
                pictureTimeSynchroniserModels.Add(pictureTimeSynchroniserModel);
            }

            return pictureTimeSynchroniserModels;
        }

        private CameraTimeSynchroniserModel ParseCameraTimeSynchroniserModel(CameraPicturesModel cameraPictures)
        {
            var selectedPicture = cameraPictures.SelectedPicture;

            return new CameraTimeSynchroniserModel
            {
                StorageFile = selectedPicture.StorageFile,
                IsCameraSelected = cameraPictures.IsCameraSelected,
                CameraManufacturer = cameraPictures.CameraManufacturer,
                CameraModel = cameraPictures.CameraModel,
                PicturesCount = cameraPictures.PicturesCount,
                Year = selectedPicture.Year,
                Month = selectedPicture.Month,
                Day = selectedPicture.Day,
                Hour = selectedPicture.Hour,
                Minute = selectedPicture.Minute,
                Second = selectedPicture.Second
            };
        }

        private void GetMainCamera(List<CameraTimeSynchroniserModel> pictureTimeSynchroniserModels)
        {
            foreach (var pictureTimeSynchroniserModel in pictureTimeSynchroniserModels)
            {
                if (pictureTimeSynchroniserModel.IsCameraSelected)
                {
                    this.MainCamera = pictureTimeSynchroniserModel;
                    return;
                }
            }

            throw new ArgumentException("No main camera is selected");
        }

        private void GetOtherCamerasWithTimeDifference(List<CameraTimeSynchroniserModel> cameraTimeSynchroniserModels)
        {
            foreach (var otherCamera in cameraTimeSynchroniserModels)
            {
                if (!otherCamera.IsCameraSelected)
                {
                    otherCamera.YearDifference = otherCamera.Year - this.mainCamera.Year;
                    otherCamera.MonthDifference = otherCamera.Month - this.mainCamera.Month;
                    otherCamera.DayDifference = otherCamera.Day - this.mainCamera.Day;
                    otherCamera.HourDifference = otherCamera.Hour - this.mainCamera.Hour;
                    otherCamera.MinuteDifference = otherCamera.Minute - this.mainCamera.Minute;
                    otherCamera.SecondDifference = otherCamera.Second - this.mainCamera.Second;

                    this.otherCameras.Add(otherCamera);
                }
            }
        }

        private void SetFileNameKeyword()
        {
            var selectedPicturePath = this.mainCamera.StorageFile.Path;
            var pathParts = selectedPicturePath.Split('\\');
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
            string selectedPictureFileType = this.mainCamera.StorageFile.FileType;
            string generatorName = this.GetType().Name;
            this.fileNameTemplate.LoadFileNameTemplate(generatorName, selectedPictureFileType);
        }

        private void SetPicturesNewDateTime(List<PictureModel> picturesWithNewDateTime)
        {
            foreach (var cameraWithPictures in this.validCameraPictures)
            {
                var pictureTimeSynchroniser = this.GetTimeSynchroniser(cameraWithPictures);

                foreach (var picture in cameraWithPictures.CameraPictures)
                {
                    NameGenerator.SetNewDateTime(picture, pictureTimeSynchroniser);
                    picturesWithNewDateTime.Add(picture);
                }
            }
        }

        private CameraTimeSynchroniserModel GetTimeSynchroniser(CameraPicturesModel cameraWithPictures)
        {
            var pictureTimeSynchroniser = this.otherCameras.FirstOrDefault(
                x => x.CameraManufacturer == cameraWithPictures.CameraManufacturer &&
                     x.CameraModel == cameraWithPictures.CameraModel);

            if (pictureTimeSynchroniser == null)
            {
                if (this.mainCamera.CameraManufacturer != cameraWithPictures.CameraManufacturer ||
                    this.mainCamera.CameraModel != cameraWithPictures.CameraModel)
                {
                    throw new ArgumentException("Camera manufacturer or model missmatch");
                }

                pictureTimeSynchroniser = this.mainCamera;
            }

            return pictureTimeSynchroniser;
        }
    }
}