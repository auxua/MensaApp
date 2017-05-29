using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MensaApp.pages
{
	public partial class ConfigPage : ContentPage
	{
		public ConfigPage (object vmo = null)
		{
            ViewModels.MensaPageViewModel vm = null;
            if (vmo.GetType() == typeof(ViewModels.MensaPageViewModel))
            {
                vm = vmo as ViewModels.MensaPageViewModel;
            }
            
            InitializeComponent ();

            // get LocalizedStrings
            string MensenTitle = Localization.Localize("MensenTitle");
            string ConfigTitle = Localization.Localize("ConfigString");
            string FilterTitle = Localization.Localize("FilterTitle");
            string MainDishesOnly = Localization.Localize("MainDishesOnly");
            string VegieOnly = Localization.Localize("VegieOnly");
            string FurtherInfoTitle = Localization.Localize("FurtherInfoTitle");

            Title = ConfigTitle;

            TableRoot root = new TableRoot();
            TableSection sectionMensen = new TableSection(MensenTitle);
            foreach (var mens in MensaAdapter.Mensen)
            {
                SwitchCell cell = new SwitchCell
                {
                    Text = mens.Key,
                    On = App.MensaActive(mens.Key)
                };
                cell.OnChanged += (s, e) =>
                {
                    if (cell.On)
                    {
                        App.MensaActivate(mens.Key);
                    }
                    else
                    {
                        App.MensaDeactivate(mens.Key);
                    }
                };

                sectionMensen.Add(cell);
            }

            TableSection sectionFilters = new TableSection(FilterTitle);
            SwitchCell VegieSwitch = new SwitchCell {
                Text = VegieOnly,
                On = App.getConfig("VegieOnly")
            };
            VegieSwitch.OnChanged += (s, e) => 
            { 
                App.setConfig("VegieOnly", VegieSwitch.On);
                if (vm != null)
                    vm.NeedsUpdate = true;
            };

            SwitchCell DishesSwitch = new SwitchCell
            {
                Text = MainDishesOnly,
                On = App.getConfig("MainDishesOnly")
            };
            DishesSwitch.OnChanged += (s, e) =>
            {
                App.setConfig("MainDishesOnly", DishesSwitch.On);
                if (vm != null)
                    vm.NeedsUpdate = true;
            };

            sectionFilters.Add(VegieSwitch);
            sectionFilters.Add(DishesSwitch);


            TableSection sectionInfo = new TableSection(FurtherInfoTitle);

            
            TextCell InfoCell = new TextCell
            {
                Text = Localization.Localize("InfoCellText"),
                Detail = Localization.Localize("InfoCellDetail"),
            };
            InfoCell.Tapped += async (s, e) =>
                {
                    await DisplayAlert("Info", Localization.Localize("AboutText"), "OK");
                };

            TextCell versionCell = new TextCell
            {
                Text = "Version",
                Detail = App.Version
            };
            TextCell refreshCell = new TextCell
            {
                Text = Localization.Localize("Refresh"),
                Detail = Localization.Localize("RefreshDetail")
            };
            refreshCell.Tapped += (s, e) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    // Ask user to confirm
                    var success = await DisplayAlert(Localization.Localize("Refresh"), Localization.Localize("RefreshAlert"), Localization.Localize("Yes"), Localization.Localize("No"));
                    if (!success) return;
                    // Reset MensaDB and re-create Mensapage - this will trigger data refresh automatically
                    MensaAdapter.DownloadError = false;
                    MensaPortable.MenuDB.Instance.Reset(new AppLoadStoreMenuDB());
                    App.Current.MainPage = new NavigationPage(new pages.MensaPage());
                });
            };

            sectionInfo.Add(refreshCell);
            sectionInfo.Add(InfoCell);
            sectionInfo.Add(versionCell);

            // Misc. Config
            TableSection sectionMisc = new TableSection("Misc.");

            SwitchCell autoDayCell = new SwitchCell();
            autoDayCell.Text = Localization.Localize("NextDayAuto");
            autoDayCell.On = App.getConfig("searchNextDay");

            autoDayCell.OnChanged += (s, e) =>
            {
                App.setConfig("searchNextDay", autoDayCell.On);
            };

            TextCell tc = new TextCell();
            tc.Text = "";
            tc.Detail = Localization.Localize("TapHere");

            tc.Tapped += (s, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Info", Localization.Localize("NextDayAutoDescription"), "OK");
                });
            };

            sectionMisc.Add(autoDayCell);
            sectionMisc.Add(tc);

            root.Add(sectionFilters);
            root.Add(sectionMensen);
            root.Add(sectionMisc);
            root.Add(sectionInfo);

            

            /*StackLayout stack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children = { new TableView(root), }
            };*/


            Content = new TableView
            {
                Root = root
            };
		}
        
    }
}
