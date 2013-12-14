using System;
using PicturesSynchroniser.Common;
using PicturesSynchroniser.Events;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace PicturesSynchroniser.Models
{
    public class PictureModel : BindableBase
    {
        private StorageFile storageFile;
        private BitmapImage thumbnailImage;
        private DateTime newDateTime;
        private string originalFileName;
        private string newFileName;
        private string userDefinedFileName;

        private bool isNewFileNameVisible;
        private bool isUserDefinedFileNameFieldVisible;
        private bool isNamedAsUserDefined;

        private int year;
        private int month;
        private int day;
        private int hour;
        private int minute;
        private int second;
        private int millisecond;

        public PictureModel()
        {
            this.isNewFileNameVisible = true;
            this.isNamedAsUserDefined = true;
        }

        public event EventHandler<PictureNameChangedByUserArgs> PictureNameChangedByUser;

        public StorageFile StorageFile
        {
            get
            {
                return this.storageFile;
            }
            set
            {
                this.storageFile = value;
            }
        }

        public BitmapImage ThumbnailImage
        {
            get
            {
                return this.thumbnailImage;
            }
            set
            {
                if (value != null)
                {
                    this.SetProperty(ref this.thumbnailImage, value);
                }
            }
        }

        public DateTime NewDateTime
        {
            get
            {
                return this.newDateTime;
            }
            set
            {
                this.newDateTime = value;
            }
        }

        public string OriginalFileName
        {
            get
            {
                return this.originalFileName;
            }
            set
            {
                this.originalFileName = value;
            }
        }

        public string NewFileName
        {
            get
            {
                if (this.newFileName == null)
                {
                    this.newFileName = string.Empty;
                }

                return this.newFileName;
            }
            set
            {
                this.SetProperty(ref this.newFileName, value);
            }
        }

        public string UserDefinedFileName
        {
            get
            {
                if (this.userDefinedFileName == null)
                {
                    this.userDefinedFileName = string.Empty;
                }

                return this.userDefinedFileName;
            }
            set
            {
                if (this.userDefinedFileName != value)
                {
                    this.SetProperty(ref this.userDefinedFileName, value);

                    if (this.PictureNameChangedByUser != null)
                    {
                        this.PictureNameChangedByUser(this, new PictureNameChangedByUserArgs());
                    }
                }
            }
        }

        public bool IsNewFileNameVisible
        {
            get
            {
                return this.isNewFileNameVisible;
            }
            set
            {
                this.SetProperty(ref this.isNewFileNameVisible, value);
            }
        }

        public bool IsUserDefinedFileNameFieldVisible
        {
            get
            {
                return this.isUserDefinedFileNameFieldVisible;
            }
            set
            {
                this.SetProperty(ref this.isUserDefinedFileNameFieldVisible, value);
            }
        }

        public bool IsNamedAsUserDefined
        {
            get
            {
                return this.isNamedAsUserDefined;
            }
            set
            {
                this.SetProperty(ref this.isNamedAsUserDefined, value);
            }
        }

        public int Year
        {
            get
            {
                return this.year;
            }
            set
            {
                this.year = value;
            }
        }

        public int Month
        {
            get
            {
                return this.month;
            }
            set
            {
                this.month = value;
            }
        }

        public int Day
        {
            get
            {
                return this.day;
            }
            set
            {
                this.day = value;
            }
        }

        public int Hour
        {
            get
            {
                return this.hour;
            }
            set
            {
                this.hour = value;
            }
        }

        public int Minute
        {
            get
            {
                return this.minute;
            }
            set
            {
                this.minute = value;
            }
        }

        public int Second
        {
            get
            {
                return this.second;
            }
            set
            {
                this.second = value;
            }
        }

        public int Millisecond
        {
            get
            {
                return this.millisecond;
            }
            set
            {
                this.millisecond = value;
            }
        }
    }
}