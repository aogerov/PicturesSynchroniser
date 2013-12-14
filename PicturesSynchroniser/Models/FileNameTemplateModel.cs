using System;
using System.Linq;
using Newtonsoft.Json;
using PicturesSynchroniser.Common;
using PicturesSynchroniser.FileAccess;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace PicturesSynchroniser.Models
{
    public class FileNameTemplateModel : BindableBase
    {
        private string keyword;
        private string fileNameExample;
        private string fileType;

        private string dayIncluded;
        private string dateIncluded;
        private string timeIncluded;
        private string enumerationIncluded;

        private SolidColorBrush keywordColor;
        private SolidColorBrush dayColor;
        private SolidColorBrush dateColor;
        private SolidColorBrush timeColor;
        private SolidColorBrush enumerationColor;
        private FileNameTemplateStrings fileNameStrings;

        public FileNameTemplateModel()
        {
            this.fileNameStrings = new FileNameTemplateStrings();
        }

        public string Keyword
        {
            get
            {
                return this.keyword;
            }
            set
            {
                this.SetProperty(ref this.keyword, value);
                this.ChangeFileNameExample();
                this.ChangeKeywordColor();
            }
        }

        public string DayIncluded
        {
            get
            {
                return this.dayIncluded;
            }
            set
            {
                this.SetProperty(ref this.dayIncluded, value);
                this.ChangeFileNameExample();
                this.ChangeDayColor();
            }
        }

        public string DateIncluded
        {
            get
            {
                return this.dateIncluded;
            }
            set
            {
                this.SetProperty(ref this.dateIncluded, value);
                this.ChangeFileNameExample();
                this.ChangeDateColor();
            }
        }

        public string TimeIncluded
        {
            get
            {
                return this.timeIncluded;
            }
            set
            {
                this.SetProperty(ref this.timeIncluded, value);
                this.ChangeFileNameExample();
                this.ChangeTimeColor();
            }
        }

        public string EnumerationIncluded
        {
            get
            {
                return this.enumerationIncluded;
            }
            set
            {
                this.SetProperty(ref this.enumerationIncluded, value);
                this.ChangeFileNameExample();
                this.ChangeEnumerationColor();
            }
        }

        public SolidColorBrush KeywordColor
        {
            get
            {
                return this.keywordColor;
            }
            set
            {
                this.SetProperty(ref this.keywordColor, value);
            }
        }

        public SolidColorBrush DayColor
        {
            get
            {
                return this.dayColor;
            }
            set
            {
                this.SetProperty(ref this.dayColor, value);
            }
        }

        public SolidColorBrush DateColor
        {
            get
            {
                return this.dateColor;
            }
            set
            {
                this.SetProperty(ref this.dateColor, value);
            }
        }

        public SolidColorBrush TimeColor
        {
            get
            {
                return this.timeColor;
            }
            set
            {
                this.SetProperty(ref this.timeColor, value);
            }
        }

        public SolidColorBrush EnumerationColor
        {
            get
            {
                return this.enumerationColor;
            }
            set
            {
                this.SetProperty(ref this.enumerationColor, value);
            }
        }

        public FileNameTemplateStrings FileNameStrings
        {
            get
            {
                return this.fileNameStrings;
            }
        }

        public string FileType
        {
            get
            {
                return this.fileType;
            }
            set
            {
                this.fileType = value;
            }
        }

        public string FileNameExample
        {
            get
            {
                return this.fileNameExample;
            }
            set
            {
                this.SetProperty(ref this.fileNameExample, value);
            }
        }

        public void LoadFileNameTemplate(string generatorName, string fileType)
        {
            string storageData = StorageManager.Read(generatorName);
            if (storageData != null)
            {
                var template = JsonConvert.DeserializeObject<StorageFileNameTemplateModel>(storageData);

                this.DayIncluded = template.DayIncluded;
                this.DateIncluded = template.DateIncluded;
                this.TimeIncluded = template.TimeIncluded;
                this.EnumerationIncluded = template.EnumerationIncluded;
            }
            else
            {
                this.DayIncluded = FileNameTemplateStrings.DayYes;
                this.DateIncluded = FileNameTemplateStrings.DateNotIncluded;
                this.TimeIncluded = FileNameTemplateStrings.TimeHourMinuteSecond;
                this.EnumerationIncluded = FileNameTemplateStrings.EnumerationNo;
            }

            this.FileType = fileType;
        }

        public void SaveFileNameTemplate(string generatorName)
        {
            var template = new StorageFileNameTemplateModel
            {
                DayIncluded = this.DayIncluded,
                DateIncluded = this.DateIncluded,
                TimeIncluded = this.TimeIncluded,
                EnumerationIncluded = this.EnumerationIncluded
            };

            var serialisedTemplate = JsonConvert.SerializeObject(template);
            StorageManager.Write(generatorName, serialisedTemplate);
        }

        private void ChangeFileNameExample()
        {
            string newFileNameExample = this.keyword;

            if (this.dayIncluded == FileNameTemplateStrings.DayYes)
            {
                newFileNameExample += " Day 1";
            }

            if (this.dateIncluded == FileNameTemplateStrings.DateYearMonthDay)
            {
                newFileNameExample += " 2013.09.27";
            }
            else if (this.dateIncluded == FileNameTemplateStrings.DateMonthDay)
            {
                newFileNameExample += " 09.27";
            }

            if (this.timeIncluded == FileNameTemplateStrings.TimeHourMinuteSecondMillisecond)
            {
                newFileNameExample += " 18.34.55.749";
            }
            else if (this.timeIncluded == FileNameTemplateStrings.TimeHourMinuteSecond)
            {
                newFileNameExample += " 18.34.55";
            }
            else if (this.timeIncluded == FileNameTemplateStrings.TimeHourMinute)
            {
                newFileNameExample += " 18.34";
            }

            if (this.enumerationIncluded == FileNameTemplateStrings.EnumerationYes)
            {
                newFileNameExample += " 001";
            }

            newFileNameExample += this.fileType;
            this.FileNameExample = newFileNameExample;
        }

        private void ChangeKeywordColor()
        {
            if (string.IsNullOrEmpty(keyword))
            {
                this.KeywordColor = new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                this.KeywordColor = new SolidColorBrush(Colors.Orange);
            }
        }

        private void ChangeDayColor()
        {
            if (this.dayIncluded == FileNameTemplateStrings.DayNo)
            {
                this.DayColor = new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                this.DayColor = new SolidColorBrush(Colors.Orange);
            }
        }

        private void ChangeDateColor()
        {
            if (this.dateIncluded == FileNameTemplateStrings.DateNotIncluded)
            {
                this.DateColor = new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                this.DateColor = new SolidColorBrush(Colors.Orange);
            }
        }

        private void ChangeTimeColor()
        {
            if (this.timeIncluded == FileNameTemplateStrings.TimeNotIncluded)
            {
                this.TimeColor = new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                this.TimeColor = new SolidColorBrush(Colors.Orange);
            }
        }

        private void ChangeEnumerationColor()
        {
            if (this.enumerationIncluded == FileNameTemplateStrings.EnumerationNo)
            {
                this.EnumerationColor = new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                this.EnumerationColor = new SolidColorBrush(Colors.Orange);
            }
        }
    }
}