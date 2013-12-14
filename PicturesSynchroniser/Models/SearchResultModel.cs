using System;
using PicturesSynchroniser.Common;
using Windows.Storage;

namespace PicturesSynchroniser.Models
{
    public class SearchResultModel : BindableBase
    {
        private StorageFolder folder;

        public StorageFolder Folder
        {
            get
            {
                return this.folder;
            }
            set
            {
                this.folder = value;
                this.OnPropertyChanged("Folder");
            }
        }
    }
}