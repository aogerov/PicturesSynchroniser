using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PicturesSynchroniser.BaseModels;
using PicturesSynchroniser.Behavior;
using PicturesSynchroniser.FileAccess;
using PicturesSynchroniser.Models;

namespace PicturesSynchroniser.ViewModels
{
    public class SettingsPanelViewModel : BaseViewModel
    {
        private bool isOptionsItemActive;
        private OptionModel selectedItem;
        private ObservableCollection<OptionModel> options;

        private string currentColor;
        private List<string> colors;

        public SettingsPanelViewModel()
        {
            var colors = new List<string> 
            {
                "LightCoral", "Crimson", "LightPink", "PaleVioletRed",
                "LightSalmon", "Orange", "LightYellow", "PaleGoldenrod",
                "Khaki", "Lavender", "MediumOrchid", "MediumSlateBlue",
                "GreenYellow", "YellowGreen", "LightGreen", "MediumAquamarine",
                "CadetBlue", "SkyBlue", "RoyalBlue", "NavajoWhite", "Wheat",
                "MediumBlue", "MistyRose", "DarkGray", "Purple", "Green", "Black"
            };

            this.colors = colors.OrderBy(x => x).ToList();
            this.colors.Insert(0, "Default");
            this.currentColor = this.colors[2];

            this.ChangeColor = new RelayCommand(ChangeColorCommand);
        }

        public ICommand ChangeColor { get; private set; }

        public IEnumerable<OptionModel> Options
        {
            get
            {
                if (this.options == null)
                {
                    this.options = new ObservableCollection<OptionModel>
                    {
                        new OptionModel { Type = "Background" }
                    };
                }

                return this.options;
            }
        }

        public OptionModel SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                this.selectedItem = value;
                this.IsOptionsItemActive = !this.IsOptionsItemActive;
            }
        }

        public bool IsOptionsItemActive
        {
            get
            {
                return this.isOptionsItemActive;
            }
            set
            {
                this.isOptionsItemActive = value;
                this.OnPropertyChanged("IsOptionsItemActive");
            }
        }

        public IEnumerable<string> Colors
        {
            get
            {
                return this.colors;
            }
        }

        public string CurrentColor
        {
            get
            {
                if (this.currentColor == "Default")
                {
                    return "White";
                }

                return this.currentColor;
            }

            set
            {
                if (value == "Default")
                {
                    this.currentColor = null;
                }
                else
                {
                    this.currentColor = value;
                }

                this.OnPropertyChanged("CurrentColor");
            }
        }

        private void ChangeColorCommand(object parameter)
        {
            StorageManager.Write("themeColor", currentColor);
            this.ThemeColor = currentColor;
        }
    }
}