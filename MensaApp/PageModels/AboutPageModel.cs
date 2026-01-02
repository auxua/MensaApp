using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MensaApp.Services;

namespace MensaApp.PageModels
{
    public partial class AboutPageModel : ObservableObject
    {
        [ObservableProperty]
        private string version;

        public Command OpenCodeCommand { get; }

        public AboutPageModel()
        {
            version = App.Version;
            OpenCodeCommand = new Command(() =>
            {
                Browser.Default.OpenAsync("https://github.com/auxua/MensaApp");
            });
                
        }


    }
}