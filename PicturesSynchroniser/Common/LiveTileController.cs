using System;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace PicturesSynchroniser.Common
{
    public static class LiveTileController
    {
        public static void SendLiveTileUpdate()
        {
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideImageAndText01);

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes.First().InnerText = "Pictures Synchronizer";

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
            ((XmlElement)tileImageAttributes.First()).SetAttribute("src", "ms-appx:///Assets/310x150.png");
            ((XmlElement)tileImageAttributes.First()).SetAttribute("alt", "Pictures Synchronizer");

            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideImageAndText01);
            XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
            squareTileTextAttributes.First().AppendChild(squareTileXml.CreateTextNode("Pictures Synchronizer"));
            IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(tileXml);
            tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(5);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }
    }
}
