﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PicturesSynchroniser.Common;
using PicturesSynchroniser.Models;
using PicturesSynchroniser.Views;
using TCD.Controls;
using TCD.Controls.Settings;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace PicturesSynchroniser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            SuspensionManager.KnownTypes.Add(typeof(ObservableCollection<PictureModel>));
            SuspensionManager.KnownTypes.Add(typeof(ObservableCollection<CameraPicturesModel>));
            SuspensionManager.KnownTypes.Add(typeof(ObservableCollection<CameraTimeSynchroniserModel>));
            SuspensionManager.KnownTypes.Add(typeof(ObservableCollection<SearchResultModel>));
            SuspensionManager.KnownTypes.Add(typeof(ObservableCollection<OptionModel>));
            SuspensionManager.KnownTypes.Add(typeof(List<StorageFolder>));
            SuspensionManager.KnownTypes.Add(typeof(CameraPicturesModel));
            SuspensionManager.KnownTypes.Add(typeof(CameraTimeSynchroniserModel));
            SuspensionManager.KnownTypes.Add(typeof(FileNameTemplateModel));
            SuspensionManager.KnownTypes.Add(typeof(SearchResultModel));
            SuspensionManager.KnownTypes.Add(typeof(OptionModel));
        }

        //internal static bool IsToastNotificated { get; set; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();

            // Init settings panel
            this.InitSettings();
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // event in OnWindowCreated to speed up searches once the application is already running

            // If the Window isn't already using Frame navigation, insert our own Frame
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            // If the app does not contain a top-level frame, it is possible that this 
            // is the initial launch of the app. Typically this method and OnLaunched 
            // in App.xaml.cs can call a common method.
            if (frame == null)
            {
                // Create a Frame to act as the navigation context and associate it with
                // a SuspensionManager key
                frame = new Frame();
                PicturesSynchroniser.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await PicturesSynchroniser.Common.SuspensionManager.RestoreAsync();
                    }
                    catch (PicturesSynchroniser.Common.SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }

            frame.Navigate(typeof(SearchResultsPage), args.QueryText);
            Window.Current.Content = frame;

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            try
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await SuspensionManager.SaveAsync();
                deferral.Complete();
            }
            catch (Exception ex)
            {
                throw new SuspensionManagerException(ex);
            }
        }

        // IMPORTANT you want to put this method in your App.cs and call it from your apps constructor!
        private void InitSettings()
        {
            // make settings entries
            // first make a pretty normal settings entry, using the SettingsPanel control
            // this control contains a button that opens a file picker..
            SettingsEntry optionsEntry = new SettingsEntry("Options", new SettingsPanelPage(), FlyoutDimension.Narrow);

            // set up the two entries with the settings contract wrapper
            SettingsContractWrapper wrapper = new SettingsContractWrapper(
                (Brush)App.Current.Resources["ApplicationForegroundThemeBrush"],//the foreground color of all flyouts
                (Brush)App.Current.Resources["ApplicationPageBackgroundThemeBrush"],//the background color of all flyouts
                (Brush)App.Current.Resources["ApplicationPageBackgroundThemeBrush"],//the theme brush of the app
                new BitmapImage(new Uri("ms-appx:/Assets/SmallLogo.png")),
                optionsEntry);
        }
    }
}