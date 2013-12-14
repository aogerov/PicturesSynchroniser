using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PicturesSynchroniser.Behavior;
using PicturesSynchroniser.Common;
using PicturesSynchroniser.FileAccess;
using PicturesSynchroniser.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

namespace PicturesSynchroniser.BaseModels
{
    public class BaseViewModel : BindableBase
    {
        private DataTransferManager dataTransferManager;
        private bool isAllLoaded;
        private bool isReadyToProceed;
        private string themeColor;

        public INavigationService NavigationService { get; set; }

        public ICommand NavigateToHome { get; private set; }

        public ICommand NavigateBack { get; private set; }

        public BaseViewModel()
        {
            this.ReadThemeColorFromStorage();
            this.NavigateToHome = new RelayCommand(this.NavigateToHomeCommand);
            this.NavigateBack = new RelayCommand(this.NavigateBackCommand);
        }

        public bool IsAllLoaded
        {
            get
            {
                return this.isAllLoaded;
            }
            set
            {
                isAllLoaded = value;
                this.OnPropertyChanged("IsAllLoaded");
            }
        }

        public bool IsReadyToProceed
        {
            get
            {
                return this.isReadyToProceed;
            }
            set
            {
                if (this.isReadyToProceed != value)
                {
                    this.isReadyToProceed = value;
                    this.OnPropertyChanged("IsReadyToProceed");
                }
            }
        }

        public string ThemeColor
        {
            get
            {
                return this.themeColor;
            }
            set
            {
                this.themeColor = value;
                this.OnPropertyChanged("ThemeColor");
            }
        }

        public virtual bool CanGoBack
        {
            get
            {
                if (this.NavigationService != null)
                {
                    return this.NavigationService.CanGoBack;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual bool CanGoForward
        {
            get
            {
                if (this.NavigationService != null)
                {
                    return this.NavigationService.CanGoForward;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual void LoadState(object navParameter, Dictionary<string, object> viewModelState)
        {
        }

        public virtual void SaveState(Dictionary<string, object> viewModelState)
        {
        }

        public void AddShareContract()
        {
            this.dataTransferManager = DataTransferManager.GetForCurrentView();
            this.dataTransferManager.DataRequested +=
                new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
        }

        public void RemoveShareContract()
        {
            this.dataTransferManager.DataRequested -=
                new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.ApplicationName = "Pictures Synchroniser";
            request.Data.Properties.Title = string.Format("Pictures Synchroniser");
            request.Data.Properties.Description = "The program can help you to synchronise your pictures.";
            string imageToShare = "ms-appx:///Assets/main_camera - original.png";
            string textAsHtml = HtmlFormatHelper.CreateHtmlFormat(imageToShare);
            request.Data.SetHtmlFormat(textAsHtml);
        }

        private void NavigateToHomeCommand(object parameter)
        {
            this.NavigationService.GoHome();
            //this.NavigationService.Navigate(ViewType.Main);
        }

        private void NavigateBackCommand(object parameter)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void ReadThemeColorFromStorage()
        {
            var localSettingsColor = StorageManager.Read("themeColor");            
            if (localSettingsColor != null)
            {
                this.themeColor = localSettingsColor;
            }
        }
    }
}