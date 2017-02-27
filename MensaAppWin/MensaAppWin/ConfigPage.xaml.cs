using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

#if WINDOWS_UWP

using Windows.Gaming.Input;
using Windows.System;

#endif

namespace MensaAppWin
{
    public partial class ConfigPage : ContentPage, IGamePadSupport
    {
        private Cell focusCell = null;
        private enum TextCellSelect
        {
            APPINFO,
            REFRESH,
            NEXTDAY,
            NONE
        }

        private TextCellSelect textCellSelect = TextCellSelect.NONE;

        public ConfigPage(object vmo = null)
        {
            ViewModels.MensaPageViewModel vm = null;
            if (vmo.GetType() == typeof(ViewModels.MensaPageViewModel))
            {
                vm = vmo as ViewModels.MensaPageViewModel;
            }

            InitializeComponent();

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
                // Also allow tapped-event in here
                cell.Tapped += (s, e) => 
                {
                    this.focusCell = cell;
                    if (s == null) cell.On = !cell.On;
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
            SwitchCell VegieSwitch = new SwitchCell
            {
                Text = VegieOnly,
                On = App.getConfig("VegieOnly")
            };
            VegieSwitch.Tapped += (s, e) => 
            {
                this.focusCell = VegieSwitch;
                if (s == null) VegieSwitch.On = !VegieSwitch.On;
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
            DishesSwitch.Tapped += (s, e) => 
            {
                this.focusCell = DishesSwitch;
                if (s == null) DishesSwitch.On = !DishesSwitch.On;
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

            InfoCell.Tapped += InfoCell_Tapped;
            
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

            refreshCell.Tapped += RefreshCell_Tapped;
            

            sectionInfo.Add(refreshCell);
            sectionInfo.Add(InfoCell);
            sectionInfo.Add(versionCell);

            // Misc. Config
            TableSection sectionMisc = new TableSection("Misc.");

            SwitchCell autoDayCell = new SwitchCell();
            autoDayCell.Text = Localization.Localize("NextDayAuto");
            autoDayCell.On = App.getConfig("searchNextDay");
            autoDayCell.Tapped += (s, e) => 
            {
                this.focusCell = autoDayCell;
                if (s == null) autoDayCell.On = !autoDayCell.On;
            };
            autoDayCell.OnChanged += (s, e) =>
            {
                App.setConfig("searchNextDay", autoDayCell.On);
            };

            TextCell tc = new TextCell();
            tc.Text = "";
            tc.Detail = Localization.Localize("TapHere");

            tc.Tapped += Tc_Tapped;

            // sections itelf should not have tapped events
            versionCell.Tapped += Misc_Tapped;


            sectionMisc.Add(autoDayCell);
            sectionMisc.Add(tc);

            root.Add(sectionFilters);
            root.Add(sectionMensen);
            root.Add(sectionMisc);
            root.Add(sectionInfo);

            Label tmplabel = new Label();

            StackLayout stack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children = { new TableView(root), },
                Padding = new Thickness(10,0),
            };

            


            /*Content = new TableView
            {
                Root = root
            };*/

            Content = stack;

            //Content.Focus();
            tmplabel.Focus();
        }

        private void Tc_Tapped(object sender, EventArgs e)
        {
            if ((!App.isXbox) || (sender == null))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Info", Localization.Localize("NextDayAutoDescription"), "OK");
                });
            }
            else
            {
                this.focusCell = sender as TextCell;
                this.textCellSelect = TextCellSelect.NEXTDAY;
            }
        }

        private void RefreshCell_Tapped(object sender, EventArgs e)
        {
            if ((!App.isXbox) || (sender == null))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    // Ask user to confirm
                    var success = await DisplayAlert(Localization.Localize("Refresh"), Localization.Localize("RefreshAlert"), Localization.Localize("Yes"), Localization.Localize("No"));
                    if (!success) return;
                    // Reset MensaDB and re-create Mensapage - this will trigger data refresh automatically
                    MensaAdapter.DownloadError = false;
                    Mensa.MenuDB.Instance.Reset(new AppLoadStoreMenuDB());
                    App.Current.MainPage = new NavigationPage(new MensaPage());
                });
            }
            else
            {
                this.focusCell = sender as TextCell;
                this.textCellSelect = TextCellSelect.REFRESH;
            }
        }

        private void InfoCell_Tapped(object sender, EventArgs e)
        {
            if ((!App.isXbox) || (sender == null))
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Info", Localization.Localize("AboutText"), "OK") );
            else
            {
                this.focusCell = sender as TextCell;
                this.textCellSelect = TextCellSelect.APPINFO;
            }
        }

        /// <summary>
        /// This method should be called to reset the internal focus structures to prevent events for objects that are not having own tapped events
        /// </summary>
        private void Misc_Tapped(object sender, EventArgs e)
        {
            this.focusCell = null;
            this.textCellSelect = TextCellSelect.NONE;
        }

        public void ButtonTrigger(VirtualKey button)
        {
            // Back via B or menu buttons
            if ((button == VirtualKey.GamepadB) || (button == VirtualKey.GamepadMenu))
                this.OnBackButtonPressed();

            if (button == VirtualKey.GamepadA)
            {
                if (focusCell is SwitchCell)
                {
                    ((SwitchCell)focusCell).On = !((SwitchCell)focusCell).On;
                }
                else if (focusCell is TextCell)
                {
                    // TextCells - for now, we need to do the event manually
                    switch (this.textCellSelect)
                    {
                        case TextCellSelect.APPINFO:
                            InfoCell_Tapped(null, null);
                            break;

                        case TextCellSelect.NEXTDAY:
                            Tc_Tapped(null, null);
                            break;

                        case TextCellSelect.REFRESH:
                            RefreshCell_Tapped(null, null);
                            break;

                        case TextCellSelect.NONE:
                            break;
                    }
                }
            }
        }

    }
}
