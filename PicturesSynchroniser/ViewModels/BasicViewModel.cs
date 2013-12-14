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
    public class BasicViewModel : BaseViewModel
    {
        private ObservableCollection<PictureModel> allPictures;
        private int picturesWithInvalidMeta;
        private double cameraPicturesGridHeight;

        public BasicViewModel()
        {
            this.allPictures = new ObservableCollection<PictureModel>();
            this.SystemMessage = new MessageModel();
            this.RecalculateCameraPicturesGridHeight();

            this.LoadPictures = new RelayCommand(this.LoadPicturesCommand);
            this.SynchronisePictures = new RelayCommand(this.SynchronisePicturesCommand);
        }

        public MessageModel SystemMessage { get; set; }

        public ICommand LoadPictures { get; private set; }

        public ICommand SynchronisePictures { get; private set; }

        public IEnumerable<PictureModel> AllPictures
        {
            get
            {
                return this.allPictures;
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
                this.allPictures = viewModelState["allPictures"] as ObservableCollection<PictureModel>;
                this.picturesWithInvalidMeta = (int)viewModelState["picturesWithInvalidMeta"];
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
            viewModelState["allPictures"] = this.allPictures;
            viewModelState["picturesWithInvalidMeta"] = this.picturesWithInvalidMeta;
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
            var navParameter = new List<object>();
            navParameter.Add(allPictures.ToList());
            navParameter.Add(this.picturesWithInvalidMeta);
            this.NavigationService.Navigate(ViewType.BasicNamesGenerator, navParameter);
        }

        private async Task ExecuteLoad(IReadOnlyList<StorageFile> selectedFiles)
        {
            this.allPictures.Clear();

            foreach (var file in selectedFiles)
            {
                if (!file.ContentType.Contains("image"))
                {
                    break;
                }

                var cameraInfo = await CameraMetaReader.GetCameraMeta(file);
                if (cameraInfo[0] == UnknownMeta.CameraManufacturer && cameraInfo[1] == UnknownMeta.CameraModel)
                {
                    this.picturesWithInvalidMeta++;
                }

                var picture = await PictureMetaReader.CreateNewPictureModel(file);
                this.allPictures.Add(picture);
            }

            this.SetReadResultMessage();
        }

        private void RecalculateCameraPicturesGridHeight()
        {
            double windowHeight = Window.Current.Bounds.Height - 200;

            if (this.allPictures.Count() > 0)
            {
                this.CameraPicturesGridHeight = windowHeight / this.allPictures.Count();
            }
            else
            {
                this.CameraPicturesGridHeight = windowHeight;
            }
        }

        private void SetReadResultMessage()
        {
            int validCameraPicturesCount = 0;
            if (!(this.allPictures == null || this.allPictures.Count == 0))
            {
                validCameraPicturesCount = this.allPictures.Count;
            }

            string resultMessage = validCameraPicturesCount + " pictures succesfully read.";
            this.SystemMessage.IsAllReady = true;

            if (this.picturesWithInvalidMeta > 0)
            {
                resultMessage += "\r\nThe collection contains " + this.picturesWithInvalidMeta +
                                 " pictures with missing meta information. You will have to set their names manually.";
            }

            this.SystemMessage.Text = resultMessage;
            this.SystemMessage.SetTimeVisibility(9000);
        }
    }
}