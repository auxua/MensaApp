using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MensaAppWin;
using Windows.UI.Xaml.Media.Imaging;

namespace MensaAppWin.UWP
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
            this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                
                Xamarin.Forms.Forms.Init(e);
                Xamarin.Forms.DependencyService.Register< MensaAppWin.UWP.Locale_UWP> (); // add this

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();

            // Gamepad
            var str = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            //MensaAppWin.App.isXbox = true;
            if (str.ToLower().Contains("xbox"))
            {
                MensaAppWin.App.isXbox = true;
                // Only activte Gamepad for XBOX
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            }

            // Only for Debugging!
            //MensaAppWin.App.isXbox = true;
            //Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;


        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        //private async void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            //if (args.Handled) return;

            MensaAppWin.App.GamepadButton(args.VirtualKey);

            //if (args.VirtualKey == Windows.System.VirtualKey.GamepadLeftTrigger)
            /*{
                var dlg = new MessageDialog("Button: "+args.VirtualKey.ToString());
                dlg.Commands.Add(new UICommand("Yes", null, "YES"));
                dlg.Commands.Add(new UICommand("No", null, "NO"));
                

                var op = await dlg.ShowAsync();
                if ((string)op.Id == "YES")
                {
                    //Do something
                }
            }*/   
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /*private async void SaveImageSource_Click(object sender, RoutedEventArgs e)
        {                        
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(RenderedGrid, width, height);
            RenderedImage.Source = renderTargetBitmap;
        }*/
    }
}
