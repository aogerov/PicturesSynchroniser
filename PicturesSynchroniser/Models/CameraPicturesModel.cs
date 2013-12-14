using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PicturesSynchroniser.Common;
using PicturesSynchroniser.Events;

namespace PicturesSynchroniser.Models
{
    public class CameraPicturesModel : BindableBase
    {
        private string cameraManufacturer;
        private string cameraModel;
        private int picturesCount;
        private string picturesCountAsString;
        private bool isCameraSelected;
        private bool isCameraWithValidMeta;
        private PictureModel selectedPicture;
        private ObservableCollection<PictureModel> cameraPictures;

        public CameraPicturesModel()
        {
            this.cameraPictures = new ObservableCollection<PictureModel>();
        }

        public event EventHandler<CameraSelectionStateChangedArgs> CameraSelectionStateChanged;

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

        public bool IsCameraWithValidMeta
        {
            get
            {
                return this.isCameraWithValidMeta;
            }
            set
            {
                this.SetProperty(ref this.isCameraWithValidMeta, value);
            }
        }

        public PictureModel SelectedPicture
        {
            get
            {
                return this.selectedPicture;
            }
            set
            {
                this.selectedPicture = value;
                this.CameraSelectionStateChanged(this, new CameraSelectionStateChangedArgs());
            }
        }

        public IEnumerable<PictureModel> CameraPictures
        {
            get
            {
                if (this.cameraPictures == null)
                {
                    this.cameraPictures = new ObservableCollection<PictureModel>();
                }

                return this.cameraPictures;
            }
        }

        public void Add(PictureModel pictureModel)
        {
            this.cameraPictures.Add(pictureModel);
            this.PicturesCount = this.cameraPictures.Count;
        }

        public void AddRange(IEnumerable<PictureModel> pictureModels)
        {
            foreach (var pictureModel in pictureModels)
            {
                this.cameraPictures.Add(pictureModel);
            }

            this.PicturesCount = this.cameraPictures.Count;
        }

        public void Clear()
        {
            this.cameraPictures.Clear();
            this.PicturesCount = this.cameraPictures.Count;
        }
    }
}