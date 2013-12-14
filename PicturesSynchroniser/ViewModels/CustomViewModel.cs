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
using Windows.Storage;
using Windows.UI.Xaml;

namespace PicturesSynchroniser.ViewModels
{
    public class CustomViewModel : BaseViewModel
    {
        private ObservableCollection<CameraPicturesModel> allCameraPictures;
        private double cameraPicturesGridHeight;

        public CustomViewModel()
        {
            this.allCameraPictures = new ObservableCollection<CameraPicturesModel>();
            this.SystemMessage = new MessageModel();
            this.RecalculateCameraPicturesGridHeight();

            this.LoadPictures = new RelayCommand(this.LoadPicturesCommand);
            this.SynchronisePictures = new RelayCommand(this.SynchronisePicturesCommand);
        }

        public MessageModel SystemMessage { get; set; }

        public ICommand LoadPictures { get; private set; }

        public ICommand SynchronisePictures { get; private set; }

        public IEnumerable<CameraPicturesModel> AllCameraPictures
        {
            get
            {
                return this.allCameraPictures;
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

        public override async void LoadState(object navParameter, Dictionary<string, object> viewModelState)
        {
            if (viewModelState != null)
            {
                this.allCameraPictures = viewModelState["allCameraPictures"] as ObservableCollection<CameraPicturesModel>;
                this.CameraPicturesGridHeight = (double)viewModelState["cameraPicturesGridHeight"];
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
            this.IsReadyToProceed = true;
            this.IsAllLoaded = true;
        }

        public override void SaveState(Dictionary<string, object> viewModelState)
        {
            viewModelState["allCameraPictures"] = this.allCameraPictures;
            viewModelState["cameraPicturesGridHeight"] = this.cameraPicturesGridHeight;
            
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
            this.NavigationService.Navigate(ViewType.CustomNamesGenerator, this.allCameraPictures);
        }

        private async Task ExecuteLoad(IReadOnlyList<StorageFile> selectedFiles)
        {
            this.allCameraPictures.Clear();
            int cameraCollectionsCount = 0;

            foreach (var file in selectedFiles)
            {
                if (!file.ContentType.Contains("image"))
                {
                    break;
                }

                var cameraInfo = await CameraMetaReader.GetCameraMeta(file);
                var cameraPictures = CameraMetaReader.GetCameraPicturesModel(this.allCameraPictures, cameraInfo);

                if (this.allCameraPictures.Count != cameraCollectionsCount)
                {
                    this.RecalculateCameraPicturesGridHeight();
                    cameraCollectionsCount++;
                }

                var picture = await PictureMetaReader.CreateNewPictureModel(file);
                cameraPictures.Add(picture);
            }

            this.SetReadResultMessage();
        }

        private void RecalculateCameraPicturesGridHeight()
        {
            double windowHeight = Window.Current.Bounds.Height - 240;

            if (this.allCameraPictures.Count() > 0)
            {
                this.CameraPicturesGridHeight = windowHeight / this.allCameraPictures.Count();
            }
            else
            {
                this.CameraPicturesGridHeight = windowHeight;
            }
        }

        private void SetReadResultMessage()
        {
            int validCameraPicturesCount = 0;
            if (!(this.allCameraPictures == null || this.allCameraPictures.Count == 0))
            {
                validCameraPicturesCount = this.allCameraPictures.Sum(c => c.PicturesCount);
            }

            int invalidCameraPicturesCount = 0;
            var cameraWithInvalidMeta = this.allCameraPictures.FirstOrDefault(a => !a.IsCameraWithValidMeta);
            if (cameraWithInvalidMeta != null)
            {
                invalidCameraPicturesCount = cameraWithInvalidMeta.PicturesCount;
            }

            string resultMessage = validCameraPicturesCount + " pictures succesfully read.";

            if (invalidCameraPicturesCount > 0)
            {
                resultMessage += "\r\nThe collection contains " + invalidCameraPicturesCount +
                                 " pictures with missing meta information. You can find them " +
                                 "in the \"" + UnknownMeta.CameraManufacturer + " - " +
                                 UnknownMeta.CameraModel + "\" section";
            }

            this.SystemMessage.Text = resultMessage;
            this.SystemMessage.IsAllReady = true;
            this.SystemMessage.SetTimeVisibility(9000);
        }
    }
}