using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PicturesSynchroniser.BaseModels;
using PicturesSynchroniser.Behavior;
using PicturesSynchroniser.Events;
using PicturesSynchroniser.FileAccess;
using PicturesSynchroniser.Models;
using PicturesSynchroniser.Services;
using Windows.Storage;
using Windows.UI.Xaml;

namespace PicturesSynchroniser.ViewModels
{
    public class WizardViewModel : BaseViewModel
    {
        private ObservableCollection<CameraPicturesModel> validCameraPictures;
        private CameraPicturesModel invalidCameraPictures;
        private double cameraPicturesGridHeight;

        public WizardViewModel()
        {
            this.validCameraPictures = new ObservableCollection<CameraPicturesModel>();
            this.SystemMessage = new MessageModel();
            this.CheckIfAllIsSelected();
            this.RecalculateCameraPicturesGridHeight();

            this.LoadPictures = new RelayCommand(this.LoadPicturesCommand);
            this.SynchronisePictures = new RelayCommand(this.SynchronisePicturesCommand);
        }

        public MessageModel SystemMessage { get; set; }

        public ICommand LoadPictures { get; private set; }

        public ICommand SynchronisePictures { get; private set; }

        public IEnumerable<CameraPicturesModel> ValidCameraPictures
        {
            get
            {
                return this.validCameraPictures;
            }
        }

        public double CameraPicturesGridHeight
        {
            get
            {
                return this.cameraPicturesGridHeight;
            }
            set
            {
                this.cameraPicturesGridHeight = value;
                this.OnPropertyChanged("CameraPicturesGridHeight");
            }
        }

        public int SelectCamera
        {
            get
            {
                return -1;
            }
            set
            {
                int index = value;

                if (index == -1)
                {
                    foreach (var cameraPictures in this.validCameraPictures)
                    {
                        cameraPictures.IsCameraSelected = false;
                    }

                    this.CheckIfAllIsSelected();
                    return;
                }

                if (this.validCameraPictures[index].IsCameraSelected)
                {
                    this.validCameraPictures[index].IsCameraSelected = false;
                    this.CheckIfAllIsSelected();
                    return;
                }

                foreach (var cameraPictures in this.validCameraPictures)
                {
                    cameraPictures.IsCameraSelected = false;
                }

                this.validCameraPictures[index].IsCameraSelected = true;
                this.CheckIfAllIsSelected();
            }
        }

        public override async void LoadState(object navParameter, Dictionary<string, object> viewModelState)
        {
            if (viewModelState != null)
            {
                this.validCameraPictures = viewModelState["validCameraPictures"] as ObservableCollection<CameraPicturesModel>;
                this.invalidCameraPictures = viewModelState["invalidCameraPictures"] as CameraPicturesModel;
                this.CameraPicturesGridHeight = (double)viewModelState["cameraPicturesGridHeight"];
                this.IsReadyToProceed = (bool)viewModelState["isReadyToProceed"];
            }
            else if (navParameter != null)
            {
                var selectedFiles = navParameter as IReadOnlyList<StorageFile>;
                if (selectedFiles.Count != 0)
                {
                    await this.ExecuteLoad(selectedFiles);
                }
                else
                {
                    this.NavigationService.GoBack();
                }
            }

            this.AddShareContract();
            this.IsAllLoaded = true;
        }

        public override void SaveState(Dictionary<string, object> viewModelState)
        {
            viewModelState["validCameraPictures"] = this.validCameraPictures;
            viewModelState["invalidCameraPictures"] = this.invalidCameraPictures;
            viewModelState["cameraPicturesGridHeight"] = this.cameraPicturesGridHeight;
            viewModelState["isReadyToProceed"] = this.IsReadyToProceed;
            
            this.RemoveShareContract();
        }

        private async void LoadPicturesCommand(object parameter)
        {
            var selectedFiles = await FilePickerController.GetPictures();
            if (selectedFiles.Count != 0)
            {
                this.ExecuteLoad(selectedFiles);
            }
        }

        private void SynchronisePicturesCommand(object parameter)
        {
            bool isAllReady = this.ValidatePicturesSelection();
            this.SystemMessage.IsAllReady = isAllReady;
            this.SystemMessage.SetTimeVisibility(10000);

            if (isAllReady)
            {
                var navParameter = new List<object>();
                navParameter.Add(this.validCameraPictures);
                navParameter.Add(this.invalidCameraPictures);

                this.NavigationService.Navigate(ViewType.WizardNamesGenerator, navParameter);
            }
        }

        private async Task ExecuteLoad(IReadOnlyList<StorageFile> selectedFiles)
        {
            this.validCameraPictures.Clear();
            this.invalidCameraPictures = new CameraPicturesModel();
            int cameraCollectionsCount = 0;

            foreach (var file in selectedFiles)
            {
                if (!file.ContentType.Contains("image"))
                {
                    break;
                }

                var cameraInfo = await CameraMetaReader.GetCameraMeta(file);
                var cameraPictures = CameraMetaReader.GetCameraPicturesModelExtended(
                    this.validCameraPictures, this.invalidCameraPictures, cameraInfo);
                
                if (this.validCameraPictures.Count != cameraCollectionsCount)
                {
                    this.RecalculateCameraPicturesGridHeight();
                    cameraPictures.CameraSelectionStateChanged += CameraSelectionStateChangedEvent;
                    cameraCollectionsCount++;
                }

                var picture = await PictureMetaReader.CreateNewPictureModel(file);
                cameraPictures.Add(picture);
            }

            this.SetReadResultMessage();
        }

        private void CameraSelectionStateChangedEvent(object sender, CameraSelectionStateChangedArgs e)
        {
            this.CheckIfAllIsSelected();
        }

        private void RecalculateCameraPicturesGridHeight()
        {
            double windowHeight = Window.Current.Bounds.Height - 230;

            if (this.validCameraPictures.Count() > 0)
            {
                this.CameraPicturesGridHeight = windowHeight / this.validCameraPictures.Count();
            }
            else
            {
                this.CameraPicturesGridHeight = windowHeight;
            }
        }

        private void CheckIfAllIsSelected()
        {
            bool isCameraSelected = this.CheckIfCameraIsSelected();
            bool allValidPicturesAreSelected = this.CheckIfAllValidPicturesAreSelected();

            if (isCameraSelected && allValidPicturesAreSelected)
            {
                this.IsReadyToProceed = true;
            }
            else
            {
                this.IsReadyToProceed = false;
            }
        }

        private void SetReadResultMessage()
        {
            int validCameraPicturesCount = 0;
            if (!(this.validCameraPictures == null || this.validCameraPictures.Count == 0))
            {
                validCameraPicturesCount = this.validCameraPictures.Sum(c => c.PicturesCount);
            }

            int invalidCameraPicturesCount = 0;
            if (this.invalidCameraPictures != null)
            {
                invalidCameraPicturesCount = this.invalidCameraPictures.PicturesCount;
            }

            string resultMessage = (validCameraPicturesCount + invalidCameraPicturesCount) +
                                   " pictures succesfully read.";

            if (invalidCameraPicturesCount > 0)
            {
                resultMessage += "\r\nThe collection contains " + invalidCameraPicturesCount +
                                 " pictures with missing meta information, therefore are not shown below.";
            }

            this.SystemMessage.Text = resultMessage;
            this.SystemMessage.IsAllReady = true;
            this.SystemMessage.SetTimeVisibility(9000);
        }

        private bool ValidatePicturesSelection()
        {
            string noExistingPictures = "Please select pictures for synchronising.";
            string invalidPicturesAndCameraSelection = "Pease select your main camera and a picture for each one of the cameras.\r\n" +
                                                       "For easyer synchronising, the selected pictures has to be made as close as possible in time.";

            string invalidCamersSelection = "Pease select your main camera before processing to the synchronisation";
            string invalidPicturesSelection = "Pease select a picture for each one of the cameras.\r\n" +
                                              "For easyer synchronising, the selected pictures has to be made as close as possible in time.";

            if (this.validCameraPictures == null || this.validCameraPictures.Count == 0)
            {
                this.SystemMessage.Text = noExistingPictures;
                return false;
            }

            bool isCameraSelected = this.CheckIfCameraIsSelected();
            bool allValidPicturesAreSelected = this.CheckIfAllValidPicturesAreSelected();

            if (isCameraSelected == false && allValidPicturesAreSelected == false)
            {
                this.SystemMessage.Text = invalidPicturesAndCameraSelection;
                return false;
            }

            if (!isCameraSelected)
            {
                this.SystemMessage.Text = invalidCamersSelection;
                return false;
            }

            if (!allValidPicturesAreSelected)
            {
                this.SystemMessage.Text = invalidPicturesSelection;
                return false;
            }

            return true;
        }

        private bool CheckIfCameraIsSelected()
        {
            int selectedCamerasCount = this.validCameraPictures.Where(c => c.IsCameraSelected).Count();
            if (selectedCamerasCount == 1)
            {
                return true;
            }

            return false;
        }

        private bool CheckIfAllValidPicturesAreSelected()
        {
            int selectedPicturesCount = this.validCameraPictures.Where(c => c.SelectedPicture != null).Count();
            if (selectedPicturesCount == this.validCameraPictures.Count)
            {
                return true;
            }

            return false;
        }
    }
}