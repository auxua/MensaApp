using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MensaApp.Resources.Strings;
using MensaPortable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace MensaApp.PageModels
{
    public partial class MensaToggleItem : ObservableObject
    {
        public MensaToggleItem(string name, bool isEnabled)
        {
            Name = name;
            _isEnabled = isEnabled;

        }

        public string Name { get; }

        [ObservableProperty]
        private bool _isEnabled;

        partial void OnIsEnabledChanged(bool oldValue, bool newValue)
        {
            ToggleCommand.Execute(this);
        }

        public ICommand ToggleCommand { get; set; }
    }

    public partial class SettingsPageModel : ObservableObject
    {
        private IDialogService dialog;
        private MensaAdapter mensaAdapter;

        public SettingsPageModel(IDialogService d, MensaAdapter mensa)
        {
            mensaAdapter = mensa;
            dialog = d;

            // Get Current Config
            var allmens = mensaAdapter.Mensen.Keys;
            foreach (var item in allmens)
            {
                bool act = App.MensaActive(item); 
                MensaToggleItem mti = new MensaToggleItem(item, act);
                mti.ToggleCommand = new Command(() =>
                {
                    if (mti.IsEnabled)
                    {
                        App.MensaActivate(mti.Name);
                    }
                    else
                    {
                        App.MensaDeactivate(mti.Name);
                    }
                });
                Mensen.Add(mti);
            }

        }

        partial void OnOnlyMainDishesChanged(bool value)
        {
            App.setConfig(App.Config_OnlyMainDishes, value);
        }

        [ObservableProperty]
        private bool onlyMainDishes;

        [ObservableProperty]
        private ObservableCollection<MensaToggleItem> mensen = new ObservableCollection<MensaToggleItem>();


        [RelayCommand]
        private async Task ReloadDataAsync()
        {
            bool resp = await dialog.ShowConfirmAsync(AppRessources.RefreshAlert, AppRessources.Refresh, AppRessources.Yes, AppRessources.No);
            if (!resp)
                return;

            MenuDB.Instance.Reset();
            dialog.ShowSnackInfoAsync(AppRessources.OutdatedLoading, 10);
            await mensaAdapter.CatchMensaDataAsync();

            // Hier: Daten neu laden (am besten mit IsBusy + UI-Feedback)
            // await _dataService.ReloadAsync();

            // Platzhalter
            await Task.CompletedTask;
        }

        [RelayCommand]
        private async Task OpenPrivacyAsync()
        {
            await Browser.Default.OpenAsync("https://apps.auxua.eu/privacy/PP_MensaAachen.htm", BrowserLaunchMode.External);
        }
    }
}
