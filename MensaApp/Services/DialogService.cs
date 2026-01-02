using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.ApplicationModel;

namespace MensaApp.Services
{

    public interface IDialogService
    {
        Task ShowErrorAsync(string message, string title = "Error");
        Task ShowMessageAsync(string message, string title = "Info");
        Task<bool> ShowConfirmAsync(string message, string title, string accept, string cancel);
        Task ShowSnackInfoAsync(string message, int seconds=3);
        Task ShowSnackInfoAsync(string message, Action act, string actionText, int seconds = 3);
        Task<string> ShowSelectionAsync(string title, string[] options);
    }


    public class DialogService : IDialogService
    {
        public Task ShowErrorAsync(string message, string title = "Error")
            => MainThread.InvokeOnMainThreadAsync(() =>
                Shell.Current.DisplayAlertAsync(title, message, "OK"));

        public Task ShowMessageAsync(string message, string title = "Info")
            => MainThread.InvokeOnMainThreadAsync(() =>
                Shell.Current.DisplayAlertAsync(title, message, "OK"));

        public Task<bool> ShowConfirmAsync(string message, string title, string accept, string cancel)
            => MainThread.InvokeOnMainThreadAsync(() =>
                Shell.Current.DisplayAlertAsync(title, message, accept, cancel));

        public Task ShowSnackInfoAsync(string message, int seconds = 3)
            => Snackbar.Make(message, duration: TimeSpan.FromSeconds(seconds)).Show();
#if WINDOWS

        public Task ShowSnackInfoAsync(string message, Action act, string actionText, int seconds = 3)
            { return null; }
#else
        public Task ShowSnackInfoAsync(string message, Action act, string actionText, int seconds = 3)
            => Snackbar.Make(message, duration: TimeSpan.FromSeconds(seconds),action: act, actionButtonText: actionText).Show();
#endif
        public Task<string> ShowSelectionAsync(string title, string[] options)
            => MainThread.InvokeOnMainThreadAsync(() =>
                Shell.Current.DisplayActionSheetAsync(title, "Cancel", null, options));
    }
}
