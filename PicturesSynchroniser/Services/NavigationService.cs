using System;
using System.Linq;
using PicturesSynchroniser.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PicturesSynchroniser.Services
{
    public class NavigationService : INavigationService
    {
        private static Frame Frame { get; set; }

        private static NavigationService instance;

        public static INavigationService Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new NavigationService();
                    Frame = (Frame)Window.Current.Content;
                }

                return instance;
            }
        }

        public bool CanGoBack
        {
            get
            {
                if (Frame != null)
                {
                    return Frame.CanGoBack;
                }

                return false;
            }
        }

        public bool CanGoForward
        {
            get
            {
                if (Frame != null)
                {
                    return Frame.CanGoForward;
                }

                return false;
            }
        }

        public void GoHome()
        {
            if (Frame != null)
            {
                while (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
            }
        }

        public void GoBack()
        {
            if (Frame != null && Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        public void GoForward()
        {
            if (Frame != null && Frame.CanGoForward)
            {
                Frame.GoForward();
            }
        }

        public void Navigate(ViewType pageName)
        {
            this.Navigate(pageName, null);
        }

        public void Navigate(ViewType pageName, object parameter)
        {
            var pageType = this.GetViewType(pageName);

            if (Frame != null && pageType != null)
            {
                Frame.Navigate(pageType, parameter);
            }
        }

        private Type GetViewType(ViewType view)
        {
            switch (view)
            {
                case ViewType.Main:
                    return typeof(MainPage);
                case ViewType.Wizard:
                    return typeof(WizardPage);
                case ViewType.Custom:
                    return typeof(CustomPage);
                case ViewType.Basic:
                    return typeof(BasicPage);
                case ViewType.WizardNamesGenerator:
                    return typeof(WizardNamesGeneratorPage);
                case ViewType.CustomNamesGenerator:
                    return typeof(CustomNamesGeneratorPage);
                case ViewType.BasicNamesGenerator:
                    return typeof(BasicNamesGeneratorPage);
                case ViewType.NewNamesSetter:
                    return typeof(NewNamesSetterPage);
                case ViewType.Result:
                    return typeof(ResultPage);
                default:
                    break;
            }

            return null;
        }
    }
}