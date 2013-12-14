using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PicturesSynchroniser.BaseModels;
using PicturesSynchroniser.Models;
using PicturesSynchroniser.Services;
using Windows.Storage;

namespace PicturesSynchroniser.ViewModels
{
    public class SearchResultsViewModel : BaseViewModel
    {
        private List<StorageFolder> pictureFolders;
        private string queryText;
        private SearchResultModel selectedFolder;
        private ObservableCollection<SearchResultModel> results;

        public SearchResultsViewModel()
        {
            this.queryText = string.Empty;
            this.results = new ObservableCollection<SearchResultModel>();
        }

        public string QueryText
        {
            get
            {
                return this.queryText;
            }
            set
            {
                this.queryText = value;
                this.OnPropertyChanged("QueryText");
            }
        }

        public SearchResultModel SelectedFolder
        {
            get
            {
                return this.selectedFolder;
            }
            set
            {
                this.selectedFolder = value;
                this.NavigateToWizard();
            }
        }

        public IEnumerable<SearchResultModel> Results
        {
            get
            {
                return this.results;
            }
            set
            {
                this.results.Clear();

                foreach (var item in value)
                {
                    this.results.Add(item);
                }
            }
        }

        public override async void LoadState(Object navParameter, Dictionary<String, Object> viewModelState)
        {
            if (viewModelState != null)
            {
                this.pictureFolders = viewModelState["pictureFolders"] as List<StorageFolder>;
                this.QueryText = (string)viewModelState["queryText"];
                this.selectedFolder = viewModelState["selectedFolder"] as SearchResultModel;
                this.results = viewModelState["results"] as ObservableCollection<SearchResultModel>;
            }
            else if (navParameter != null)
            {
                string queryTextAsNavParameter = navParameter as String;
                this.QueryText = queryTextAsNavParameter;
                await this.LoadPictureFolderNames();
            }

            this.AddShareContract();
            this.IsReadyToProceed = true;
            this.IsAllLoaded = true;
        }

        public override void SaveState(Dictionary<string, object> viewModelState)
        {
            viewModelState["pictureFolders"] = this.pictureFolders;
            viewModelState["queryText"] = this.queryText;
            viewModelState["selectedFolder"] = this.selectedFolder;
            viewModelState["results"] = this.results;
            
            this.RemoveShareContract();
        }

        private async Task LoadPictureFolderNames()
        {
            if (this.pictureFolders == null)
            {
                StorageFolder picturesMainFolder = Windows.Storage.KnownFolders.PicturesLibrary;
                var subFolders = await picturesMainFolder.GetFoldersAsync();
                this.pictureFolders = subFolders.ToList();
            }

            foreach (var folder in pictureFolders)
            {
                if (folder.Name.ToLower().Contains(this.QueryText.ToLower()))
                {
                    var newFolder = new SearchResultModel
                    {
                        Folder = folder
                    };

                    this.results.Add(newFolder);
                }
            }
        }

        private async void NavigateToWizard()
        {
            var selectedFiles = await selectedFolder.Folder.GetFilesAsync();
            if (selectedFiles != null && selectedFiles.Count != 0)
            {
                this.NavigationService.Navigate(ViewType.Wizard, selectedFiles);
            }
        }
    }
}