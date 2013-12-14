using System;
using System.Collections.Generic;
using System.Linq;
using PicturesSynchroniser.Common;
using PicturesSynchroniser.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace PicturesSynchroniser.BaseModels
{
    public class BaseView : LayoutAwarePage
    {
        public BaseView()
        {
            BindingOperations.SetBinding(this, DataContextChangedWatcherProperty, new Binding());
        }

        public static readonly DependencyProperty DataContextChangedWatcherProperty = DependencyProperty.Register(
            "DataContextChangedWatcher",
            typeof(object),
            typeof(BaseView),
            new PropertyMetadata(null, OnDataContextChanged));

        public object DataContextChangedWatcher
        {
            get
            {
                return GetValue(DataContextChangedWatcherProperty);
            }
            set
            {
                SetValue(DataContextChangedWatcherProperty, value);
            }
        }

        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            var viewModel = DataContext as BaseViewModel;
            if (viewModel != null)
            {
                viewModel.LoadState(navigationParameter, pageState);
            }
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            var viewModel = DataContext as BaseViewModel;
            if (viewModel != null)
            {
                viewModel.SaveState(pageState);
            }
        }

        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = ((BaseView)d).DataContext as BaseViewModel;
            if (viewModel != null)
            {
                viewModel.NavigationService = NavigationService.Current;
            }
        }
    }
}