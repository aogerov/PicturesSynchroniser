using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace PicturesSynchroniser.Common
{
    public static class ToastNotificator
    {
        public static void RaiseBackupToastNotification()
        {
            bool isSilent = true;
            string imagePath = "ms-appx:///Assets/main_camera - original.png";
            string text = "Its hardly recommended, before synchronising any pictures with the program to backup them first!";

            string toastXmlString =
                "<toast>" +
                "<visual version='1'>" +
                "<binding template='toastImageAndText01'>" +
                "<image id='1' src='" + imagePath + "' alt='PicturesSynchroniser Logo'/>" +
                "<text id='1'>" + text + "</text>" +
                "</binding>" +
                "</visual>" +
                "<audio silent='" + isSilent.ToString().ToLower() + "'/>" +
                "</toast>";

            XmlDocument toastDom = new XmlDocument();
            toastDom.LoadXml(toastXmlString);
            ToastNotification toast = new ToastNotification(toastDom);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
