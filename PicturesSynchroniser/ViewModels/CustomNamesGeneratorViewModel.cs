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
    public class CustomNamesGeneratorViewModel : BaseViewModel
    {
        private ObservableCollection<CameraPicturesModel> allCameraPictures;
        private ObservableCollection<CameraTimeSynchroniserModel> cameraTimeModels;
        private FileNameTemplateModel fileNameTemplate;

        public CustomNamesGeneratorViewModel()
        {
            this.allCameraPictures = new ObservableCollection<CameraPicturesModel>();
            this.cameraTimeModels = new ObservableCollection<CameraTimeSynchroniserModel>();
            this.fileNameTemplate = new FileNameTemplateModel();

            this.GeneratePicturesNames = new RelayCommand(this.GeneratePicturesNamesCommand);
            this.LoadPictures = new RelayCommand(this.LoadPicturesCommand);
        }

        public ICommand GeneratePicturesNames { get; private set; }

        public ICommand LoadPictures { get; private set; }

        public IEnumerable<CameraTimeSynchroniserModel> CameraTimeModels
        {
            get
            {
                return this.cameraTimeModels;
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
                this.allCameraPictures = viewModelState["allCameraPictures"] as ObservableCollection<CameraPicturesModel>;
                this.cameraTimeModels = viewModelState["cameraTimeModels"] as ObservableCollection<CameraTimeSynchroniserModel>;
                this.fileNameTemplate = viewModelState["fileNameTemplate"] as FileNameTemplateModel;
            }
            else if (navParameter != null)
            {
                var allCameraPicturesAsNav = navParameter as ObservableCollection<CameraPicturesModel>;

                for (int i = 0; i < allCameraPicturesAsNav.Count; i++)
                {
                    var sortedByDatePictures = allCameraPicturesAsNav[i].CameraPictures.OrderBy(p => p.StorageFile.DateCreated).ToList();
                    allCameraPicturesAsNav[i].Clear();
                    allCameraPicturesAsNav[i].AddRange(sortedByDatePictures);
                }

                this.allCameraPictures.Clear();
                var sortedAllCameraPictures = allCameraPicturesAsNav.OrderBy(c => c.CameraManufacturer).ThenBy(c => c.CameraModel);
                foreach (var cameraPictures in sortedAllCameraPictures)
                {
                    this.allCameraPictures.Add(cameraPictures);
                }

                this.SetPicturesForTimeMerging();
            }

            this.AddShareContract();
            this.IsReadyToProceed = true;
            this.IsAllLoaded = true;
        }

        public override void SaveState(Dictionary<string, object> viewModelState)
        {
            viewModelState["allCameraPictures"] = this.allCameraPictures;
            viewModelState["cameraTimeModels"] = this.cameraTimeModels;
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

            var picturesDataForRename = new List<object>();
            picturesDataForRename.Add(sortedPicturesByDateTime);

            this.NavigationService.Navigate(ViewType.NewNamesSetter, picturesDataForRename);
        }

        private async void LoadPicturesCommand(object parameter)
        {
            var selectedFiles = await FilePickerController.GetPictures();
            if (selectedFiles != null && selectedFiles.Count != 0)
            {
                this.NavigationService.Navigate(ViewType.Custom, selectedFiles);
            }
        }

        private void SetPicturesForTimeMerging()
        {
            var cameraTimeSynchroniserModels = this.GetCameraTimeSynchroniserModels();
            this.GetOtherCamerasWithTimeDifference(cameraTimeSynchroniserModels);
            this.SetFileNameKeyword();
            this.LoadFileNameTemplate();
        }

        private List<CameraTimeSynchroniserModel> GetCameraTimeSynchroniserModels()
        {
            var cameraTimeSynchroniserModels = new List<CameraTimeSynchroniserModel>();

            foreach (var cameraPictures in this.allCameraPictures)
            {
                var pictureTimeSynchroniserModel = this.ParseCameraTimeSynchroniserModel(cameraPictures);
                cameraTimeSynchroniserModels.Add(pictureTimeSynchroniserModel);
            }

            return cameraTimeSynchroniserModels;
        }

        private CameraTimeSynchroniserModel ParseCameraTimeSynchroniserModel(CameraPicturesModel cameraPictures)
        {
            var firstPicture = cameraPictures.CameraPictures.First();

            return new CameraTimeSynchroniserModel
            {
                StorageFile = firstPicture.StorageFile,
                IsCameraSelected = cameraPictures.IsCameraSelected,
                CameraManufacturer = cameraPictures.CameraManufacturer,
                CameraModel = cameraPictures.CameraModel,
                PicturesCount = cameraPictures.PicturesCount,
                Year = firstPicture.Year,
                Month = firstPicture.Month,
                Day = firstPicture.Day,
                Hour = firstPicture.Hour,
                Minute = firstPicture.Minute,
                Second = firstPicture.Second
            };
        }

        private void GetOtherCamerasWithTimeDifference(List<CameraTimeSynchroniserModel> cameraTimeSynchroniserModels)
        {
            foreach (var otherCamera in cameraTimeSynchroniserModels)
            {
                if (!otherCamera.IsCameraSelected)
                {
                    otherCamera.YearDifference = 0;
                    otherCamera.MonthDifference = 0;
                    otherCamera.DayDifference = 0;
                    otherCamera.HourDifference = 0;
                    otherCamera.MinuteDifference = 0;
                    otherCamera.SecondDifference = 0;

                    this.cameraTimeModels.Add(otherCamera);
                }
            }
        }

        private void SetFileNameKeyword()
        {
            var pathInBiggestCollection = this.cameraTimeModels.FirstOrDefault(
                x => x.PicturesCount == this.cameraTimeModels.Max(c => c.PicturesCount)).StorageFile.Path;

            var pathParts = pathInBiggestCollection.Split('\\');
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
            var fileTypeInBiggestCollection = this.cameraTimeModels.FirstOrDefault(
                x => x.PicturesCount == this.cameraTimeModels.Max(c => c.PicturesCount)).StorageFile.FileType;
            
            string generatorName = this.GetType().Name;
            this.fileNameTemplate.LoadFileNameTemplate(generatorName, fileTypeInBiggestCollection);
        }

        private void SetPicturesNewDateTime(List<PictureModel> picturesWithNewDateTime)
        {
            foreach (var cameraWithPictures in this.allCameraPictures)
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
            var pictureTimeSynchroniser = this.cameraTimeModels.FirstOrDefault(
                x => x.CameraManufacturer == cameraWithPictures.CameraManufacturer &&
                     x.CameraModel == cameraWithPictures.CameraModel);

            if (pictureTimeSynchroniser == null)
            {
                throw new ArgumentException("Camera manufacturer or model missmatch");
            }

            return pictureTimeSynchroniser;
        }
    }
}