using System;

namespace PicturesSynchroniser.FileAccess
{
    public static class StorageManager
    {
        public static string Read(string localName)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string localSettingsValue = localSettings.Values[localName] as string;
            return localSettingsValue;
        }

        public static void Write(string localName, string value)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[localName] = value;
        }
    }
}
