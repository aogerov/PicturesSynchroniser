using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PicturesSynchroniser.BaseModels;
using PicturesSynchroniser.Behavior;
using PicturesSynchroniser.Common;
using PicturesSynchroniser.FileAccess;
using PicturesSynchroniser.Services;
using Windows.Foundation;
using Windows.Media.Capture;

namespace PicturesSynchroniser.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private bool isToastNotificated;

        public MainViewModel()
        {
            LiveTileController.SendLiveTileUpdate();
            this.TakePicture = new RelayCommand(this.TakePictureCommand);

            this.NavigateToBasic = new RelayCommand(this.NavigateToBasicCommand);
            this.NavigateToCustom = new RelayCommand(this.NavigateToCustomCommand);
            this.NavigateToWizard = new RelayCommand(this.NavigateToWizardCommand);
        }
        
        public ICommand NavigateToBasic { get; private set; }

        public ICommand NavigateToCustom { get; private set; }

        public ICommand NavigateToWizard { get; private set; }

        public ICommand TakePicture { get; private set; }

        public ViewType BasicType
        {
            get
            {
                return ViewType.Basic;
            }
        }

        public ViewType CustomType
        {
            get
            {
                return ViewType.Custom;
            }
        }

        public ViewType WizardType
        {
            get
            {
                return ViewType.Wizard;
            }
        }

        private void NavigateToBasicCommand(object parameter)
        {
            this.Navigate(ViewType.Basic);
        }

        private void NavigateToCustomCommand(object parameter)
        {
            this.Navigate(ViewType.Custom);
        }
        
        private void NavigateToWizardCommand(object parameter)
        {
            this.Navigate(ViewType.Wizard);
        }

        public override void LoadState(object navParameter, Dictionary<string, object> viewModelState)
        {
            if (viewModelState != null && viewModelState.ContainsKey("toast"))
            {
                this.isToastNotificated = (bool)viewModelState["toast"];
            }

            if (!this.isToastNotificated) // && !App.IsToastNotificated
            {
                ToastNotificator.RaiseBackupToastNotification();
                isToastNotificated = true;
                //App.IsToastNotificated = true;
            }

            this.AddShareContract();
        }

        public override void SaveState(Dictionary<string, object> viewModelState)
        {
            viewModelState["toast"] = isToastNotificated;

            this.RemoveShareContract();
        }

        private async void TakePictureCommand(object parameter)
        {
            var cameraCapture = new CameraCaptureUI();
            cameraCapture.PhotoSettings.CroppedAspectRatio = new Size(4, 3);

            var file = await cameraCapture.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (file != null)
            {
                FilePickerController.SavePicture(file);
            }
        }

        private async void Navigate(ViewType view)
        {
            var selectedFiles = await FilePickerController.GetPictures();
            if (selectedFiles != null && selectedFiles.Count != 0)
            {
                this.NavigationService.Navigate(view, selectedFiles);
            }
        }
    }
}