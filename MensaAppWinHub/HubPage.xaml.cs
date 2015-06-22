using MensaAppWinHub.Common;
using MensaAppWinHub.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Hub-Seite" ist unter http://go.microsoft.com/fwlink/?LinkID=321224 dokumentiert.

namespace MensaAppWinHub
{
    /// <summary>
    /// Eine Seite, auf der eine gruppierte Auflistung von Elementen angezeigt wird.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public List<String> list1 = new List<string>();

        /// <summary>
        /// NavigationHelper wird auf jeder Seite zur Unterstützung bei der Navigation verwendet und 
        /// Verwaltung der Prozesslebensdauer
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Dies kann in ein stark typisiertes Anzeigemodell geändert werden.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            list1.Add("Test1");
            list1.Add("Test2");
            list1.Add("This is a very long text that may extend the default length of this label but I want to know what will happen then. So let's see.");
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        /// <summary>
        /// Füllt die Seite mit Inhalt auf, der bei der Navigation übergeben wird.  Gespeicherte Zustände werden ebenfalls
        /// bereitgestellt, wenn eine Seite aus einer vorherigen Sitzung neu erstellt wird.
        /// </summary>
        /// <param name="sender">
        /// Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Ereignisdaten, die die Navigationsparameter bereitstellen, die an
        /// <see cref="Frame.Navigate(Type, Object)"/> als diese Seite ursprünglich angefordert wurde und
        /// ein Wörterbuch des Zustands, der von dieser Seite während einer früheren
        /// beibehalten wurde.  Der Zustand ist beim ersten Aufrufen einer Seite NULL.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (Mensa.MenuDB.Instance.isOutdated())
            {
                await MensaAdapter.CatchMensaDataAsync();
            }

            
            // TODO: Ein geeignetes Datenmodell für die problematische Domäne erstellen, um die Beispieldaten auszutauschen
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-4");
            this.DefaultViewModel["Section3Items"] = sampleDataGroup;

            //this.DefaultViewModel["Section1Items"] = this.list1;
            this.DefaultViewModel["Section1Items"] = new Mensa.MenuDB.QueryBuilder().ExecuteQuery();
            
            this.DefaultViewModel["Section1Enabled"] = false;

        }

        /// <summary>
        /// Wird aufgerufen, wenn auf einen HubSection-Header geklickt wird.
        /// </summary>
        /// <param name="sender">Der Hub, der den HubSection enthält, auf dessen Header geklickt wurde.</param>
        /// <param name="e">Ereignisdaten, die beschreiben, wie der Klick initiiert wurde.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Wird aufgerufen, wenn auf ein Element innerhalb eines Abschnitts geklickt wird.
        /// </summary>
        /// <param name="sender">Die GridView oder ListView
        /// die das angeklickte Element anzeigt.</param>
        /// <param name="e">Ereignisdaten, die das angeklickte Element beschreiben.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Zur entsprechenden Zielseite navigieren und die neue Seite konfigurieren,
            // indem die erforderlichen Informationen als Navigationsparameter übergeben werden
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        #region NavigationHelper-Registrierung

        /// Die in diesem Abschnitt bereitgestellten Methoden werden einfach verwendet, um
        /// damit NavigationHelper auf die Navigationsmethoden der Seite reagieren kann.
        /// 
        /// Platzieren Sie seitenspezifische Logik in Ereignishandlern für  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// und <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// Der Navigationsparameter ist in der LoadState-Methode verfügbar 
        /// zusätzlich zum Seitenzustand, der während einer früheren Sitzung beibehalten wurde.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
