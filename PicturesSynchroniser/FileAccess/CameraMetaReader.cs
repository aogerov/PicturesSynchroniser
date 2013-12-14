using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PicturesSynchroniser.Models;
using Windows.Storage;

namespace PicturesSynchroniser.FileAccess
{
    internal class CameraMetaReader
    {
        internal static async Task<List<string>> GetCameraMeta(StorageFile file)
        {
            var pictureProperties = await file.Properties.GetImagePropertiesAsync();
            string cameraManufacturer = pictureProperties.CameraManufacturer;
            if (string.IsNullOrEmpty(cameraManufacturer))
            {
                cameraManufacturer = UnknownMeta.CameraManufacturer;
            }

            string cameraModel = pictureProperties.CameraModel;
            if (string.IsNullOrEmpty(cameraModel))
            {
                cameraModel = UnknownMeta.CameraModel;
            }

            var cameraInfo = new List<string>();
            cameraInfo.Add(cameraManufacturer);
            cameraInfo.Add(cameraModel);
            return cameraInfo;
        }

        internal static CameraPicturesModel GetCameraPicturesModelExtended(
            ObservableCollection<CameraPicturesModel> validCameraPictures,
            CameraPicturesModel invalidCameraPictures, List<string> cameraInfo)
        {
            if (cameraInfo[0] == UnknownMeta.CameraManufacturer && cameraInfo[1] == UnknownMeta.CameraModel)
            {
                if (invalidCameraPictures == null)
                {
                    invalidCameraPictures = new CameraPicturesModel();
                    SetCameraMeta(invalidCameraPictures, cameraInfo);
                }

                return invalidCameraPictures;
            }
            else
            {
                var cameraPictures = GetCameraPicturesModel(validCameraPictures, cameraInfo);
                return cameraPictures;
            }
        }

        internal static CameraPicturesModel GetCameraPicturesModelMixed(
            ObservableCollection<CameraPicturesModel> cameraPicturesCollection, List<string> cameraInfo)
        {
            if (cameraInfo[0] == UnknownMeta.CameraManufacturer && cameraInfo[1] == UnknownMeta.CameraModel)
            {
                var invalidCameraPictures = cameraPicturesCollection.FirstOrDefault(c => c.IsCameraWithValidMeta == false);
                if (invalidCameraPictures == null)
                {
                    invalidCameraPictures = new CameraPicturesModel();
                    SetCameraMeta(invalidCameraPictures, cameraInfo);
                }

                return invalidCameraPictures;
            }
            else
            {
                var cameraPictures = cameraPicturesCollection.FirstOrDefault(c => c.IsCameraWithValidMeta == true);
                if (cameraPictures == null)
                {
                    cameraPictures = new CameraPicturesModel();
                    SetCameraMeta(cameraPictures, cameraInfo);
                }

                return cameraPictures;
            }
        }

        internal static CameraPicturesModel GetCameraPicturesModel(
            ObservableCollection<CameraPicturesModel> cameraPicturesCollection, List<string> cameraInfo)
        {
            var cameraPictures = cameraPicturesCollection.FirstOrDefault(
                c => c.CameraManufacturer == cameraInfo[0] && c.CameraModel == cameraInfo[1]);

            if (cameraPictures == null)
            {
                cameraPictures = new CameraPicturesModel();
                SetCameraMeta(cameraPictures, cameraInfo);
                cameraPicturesCollection.Add(cameraPictures);
            }

            return cameraPictures;
        }

        private static void SetCameraMeta(CameraPicturesModel cameraPictures, List<string> cameraInfo)
        {
            cameraPictures.CameraManufacturer = cameraInfo[0];
            cameraPictures.CameraModel = cameraInfo[1];

            if (cameraInfo[0] == UnknownMeta.CameraManufacturer && cameraInfo[1] == UnknownMeta.CameraModel)
            {
                cameraPictures.IsCameraWithValidMeta = false;
            }
            else
            {
                cameraPictures.IsCameraWithValidMeta = true;
            }
        }
    }
}