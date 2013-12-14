using System;
using System.Threading.Tasks;
using PicturesSynchroniser.Models;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace PicturesSynchroniser.FileAccess
{
    internal class PictureMetaReader
    {
        internal static async Task<PictureModel> CreateNewPictureModel(StorageFile file)
        {
            var picture = new PictureModel();
            picture.StorageFile = file;
            picture.OriginalFileName = file.Name;
            picture.ThumbnailImage = await LoadPicture(file);

            var pictureProperties = await file.Properties.GetImagePropertiesAsync();
            DateTimeOffset dateTaken = pictureProperties.DateTaken;
            picture.Year = dateTaken.Year;
            picture.Month = dateTaken.Month;
            picture.Day = dateTaken.Day;
            picture.Hour = dateTaken.Hour;
            picture.Minute = dateTaken.Minute;
            picture.Second = dateTaken.Second;
            picture.Millisecond = dateTaken.Millisecond;

            return picture;
        }

        internal static async Task<BitmapImage> LoadPicture(StorageFile file)
        {
            var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.PicturesView);
            var picture = new BitmapImage();
            picture.SetSource(thumbnail);
            return picture;
        }
    }
}