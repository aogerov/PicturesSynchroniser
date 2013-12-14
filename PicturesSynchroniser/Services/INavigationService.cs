using System;
using System.Linq;

namespace PicturesSynchroniser.Services
{
    public interface INavigationService
    {
        void GoHome();

        void GoBack();

        void GoForward();

        void Navigate(ViewType pageName);

        void Navigate(ViewType pageName, object parameter);

        bool CanGoBack { get; }

        bool CanGoForward { get; }
    }
}