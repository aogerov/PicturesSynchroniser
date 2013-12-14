using System;

namespace PicturesSynchroniser.Models
{
    public static class UnknownMeta
    {
        private static string cameraManufacturer = "Unknown manufacturer";
        private static string cameraModel = "Unknown camera model";

        public static string CameraManufacturer
        {
            get
            {
                return cameraManufacturer;
            }
        }

        public static string CameraModel
        {
            get
            {
                return cameraModel;
            }
        }
    }
}
