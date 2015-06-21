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

            root.Add(sectionMensen);
            root.Add(sectionFilters);
            Content = new TableView
            {
                Root = root
            };
		}
	}
}
