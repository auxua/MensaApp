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

            sectionInfo.Add(InfoCell);
            sectionInfo.Add(versionCell);

            root.Add(sectionFilters);
            root.Add(sectionMensen);
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
