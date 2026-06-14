using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace MensaApp
{
    public static class MauiProgram
    {

        private static void RegisterGlobalExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                CrashLogger.Log(
                    "AppDomain.CurrentDomain.UnhandledException",
                    e.ExceptionObject as Exception,
                    $"IsTerminating: {e.IsTerminating}");
            };

            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                CrashLogger.Log(
                    "TaskScheduler.UnobservedTaskException",
                    e.Exception);

                e.SetObserved();
            };

#if WINDOWS
        Microsoft.UI.Xaml.Application.Current.UnhandledException += (_, e) =>
        {
            CrashLogger.Log(
                "Microsoft.UI.Xaml.Application.Current.UnhandledException",
                e.Exception,
                $"Message: {e.Message}");

            // Nur zum Diagnostizieren eventuell auf true setzen.
            // In Release normalerweise false lassen, sonst verschluckst du echte Crashes.
            // e.Handled = true;
        };
#endif
        }

        public static MauiApp CreateMauiApp()
        {

            //RegisterGlobalExceptionHandlers();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit(options =>
                {
                    options.SetShouldEnableSnackbarOnWindows(false);
                })
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
                    //#if WINDOWS
                    //    				Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
                    //    				{
                    //    					handler.PlatformView.SingleSelectionFollowsFocus = false;
                    //    				});

                    //    				Microsoft.Maui.Handlers.ContentViewHandler.Mapper.AppendToMapping(nameof(Pages.Controls.CategoryChart), (handler, view) =>
                    //    				{
                    //    					if (view is Pages.Controls.CategoryChart && handler.PlatformView is Microsoft.Maui.Platform.ContentPanel contentPanel)
                    //    					{
                    //    						contentPanel.IsTabStop = true;
                    //    					}
                    //    				});
                    //#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                    //fonts.AddFont("FluentSystemIcons-Regular.ttf", "FluentUI");
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<AboutPageModel>();
            builder.Services.AddSingleton<SettingsPageModel>();

            // Custom Services
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<MensaAdapter>();

            return builder.Build();
        }
    }
}
