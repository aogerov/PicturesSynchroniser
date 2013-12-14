using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PicturesSynchroniser.FileAccess
{
    internal class FilePickerController
    {
        internal static async Task<IReadOnlyList<StorageFile>> GetPictures()
        {
            var openPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail,
                FileTypeFilter = { ".jpg", ".jpeg", ".png", ".bmp" }
            };

            var selectedFiles = await openPicker.PickMultipleFilesAsync();
            return selectedFiles;
        }

        internal static async void SavePicture(StorageFile file)
        {
            byte[] buffer;
            Stream stream = await file.OpenStreamForReadAsync();
            buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, (int)stream.Length);

            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedSaveFile = file,
                SuggestedFileName = file.Name
            };

            savePicker.FileTypeChoices.Add("JPEG-Image", new List<string>() { ".jpg" });
            savePicker.FileTypeChoices.Add("PNG-Image", new List<string>() { ".png" });

            var fileForSave = await savePicker.PickSaveFileAsync();
            if (fileForSave != null)
            {
                CachedFileManager.DeferUpdates(fileForSave);
                await FileIO.WriteBytesAsync(fileForSave, buffer);
                CachedFileManager.CompleteUpdatesAsync(fileForSave);
            }
        }
    }
}