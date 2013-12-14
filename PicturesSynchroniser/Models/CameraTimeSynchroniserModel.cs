using System;
using System.Linq;
using PicturesSynchroniser.Common;
using Windows.Storage;

namespace PicturesSynchroniser.Models
{
    public class CameraTimeSynchroniserModel : BindableBase
    {
        private StorageFile storageFile;
        private bool isCameraSelected;
        private string cameraManufacturer;
        private string cameraModel;
        private int picturesCount;
        private string picturesCountAsString;

        private int year;
        private int month;
        private int day;
        private int hour;
        private int minute;
        private int second;

        private int yearDifference;
        private int monthDifference;
        private int dayDifference;
        private int hourDifference;
        private int minuteDifference;
        private int secondDifference;

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

        public bool IsCameraSelected
        {
            get
            {
                return this.isCameraSelected;
            }
            set
            {
                this.isCameraSelected = value;
            }
        }

        public string CameraManufacturer
        {
            get
            {
                return this.cameraManufacturer;
            }
            set
            {
                this.SetProperty(ref this.cameraManufacturer, value);
            }
        }

        public string CameraModel
        {
            get
            {
                return this.cameraModel;
            }
            set
            {
                this.SetProperty(ref this.cameraModel, value);
            }
        }

        public int PicturesCount
        {
            get
            {
                return this.picturesCount;
            }
            set
            {
                if (value != null)
                {
                    this.picturesCount = value;
                    this.PicturesCountAsString = value.ToString();
                }
            }
        }

        public string PicturesCountAsString
        {
            get
            {
                return this.picturesCountAsString;
            }
            set
            {
                this.SetProperty(ref this.picturesCountAsString, value + " pictures");
            }
        }

        public string FullDate
        {
            get
            {
                return this.year + "." + this.month + "." + this.day + " " + this.hour + ":" + this.minute + ":" + this.second;
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
                this.SetProperty(ref this.year, value);
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
                this.SetProperty(ref this.month, value);
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
                this.SetProperty(ref this.day, value);
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
                this.SetProperty(ref this.hour, value);
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
                this.SetProperty(ref this.minute, value);
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
                this.SetProperty(ref this.second, value);
            }
        }

        public int YearDifference
        {
            get
            {
                return this.yearDifference;
            }
            set
            {
                this.SetProperty(ref this.yearDifference, value);
            }
        }

        public int MonthDifference
        {
            get
            {
                return this.monthDifference;
            }
            set
            {
                this.SetProperty(ref this.monthDifference, value);
            }
        }

        public int DayDifference
        {
            get
            {
                return this.dayDifference;
            }
            set
            {
                this.SetProperty(ref this.dayDifference, value);
            }
        }

        public int HourDifference
        {
            get
            {
                return this.hourDifference;
            }
            set
            {
                this.SetProperty(ref this.hourDifference, value);
            }
        }

        public int MinuteDifference
        {
            get
            {
                return this.minuteDifference;
            }
            set
            {
                this.SetProperty(ref this.minuteDifference, value);
            }
        }

        public int SecondDifference
        {
            get
            {
                return this.secondDifference;
            }
            set
            {
                this.SetProperty(ref this.secondDifference, value);
            }
        }
    }
}