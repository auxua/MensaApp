using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
//using Mensa;
using MensaPortable;
using System.Threading;

namespace MensaApp
{
    class MensaAdapter
    {
        
        public async static Task<bool> CatchMensaDataAsync()
        {

            AppLoadStoreMenuDB sl = new AppLoadStoreMenuDB();
#if DEBUG
            // Debugging? Reset the internal storage
            MensaPortable.MenuDB.Instance.Reset(sl);
#endif
            // First, Load the Data from persistent storage
            MensaPortable.MenuDB.Instance.LoadDB(sl);

            // Check the latest Day of the data
            if (!MensaPortable.MenuDB.Instance.isOutdated())
                return true;

            // The data is Outdated! Try getting the new data
            try
            {
                // Reset the whole Database to prevent too much Data on the Device
                MensaPortable.MenuDB.Instance.Reset(sl);

                Dictionary<string, string> dict = await getSourcesAsync(Mensen);
                var res =  MensaPortable.MenuDB.Instance.ImportFromSources(dict);
                MensaPortable.MenuDB.Instance.StoreDB(sl);
                return res;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        /// <summary>
        /// All known canteens and the corrsponding URL
        /// </summary>
        public static Dictionary<string, Uri> Mensen = new Dictionary<string, Uri>()
        {
			{ "Academica", new Uri("http://www.studierendenwerk-aachen.de/speiseplaene/academica-w.html")},
			{ "Ahornstrasse", new Uri("http://www.studierendenwerk-aachen.de/speiseplaene/ahornstrasse-w.html")},  
			{ "Bistro", new Uri("http://www.studierendenwerk-aachen.de/speiseplaene/templergraben-w.html")}, 
			{ "Bayernallee", new Uri("http://www.studierendenwerk-aachen.de/speiseplaene/bayernallee-w.html")},
			{ "Eupener Straße", new Uri("http://www.studierendenwerk-aachen.de/speiseplaene/eupenerstrasse-w.html")},
            { "Mensa Vita", new Uri("http://www.studierendenwerk-aachen.de/speiseplaene/vita-w.html")},
            { "Mensa Jülich", new Uri("https://www.studierendenwerk-aachen.de/de/Gastronomie/mensa-juelich-wochenplan.html")},
            
            //{ "Gastro Goethe", new Uri("http://www.studierendenwerk-aachen.de/speiseplaene/goethestrasse-w.html")},	
			/*{ "Forum Cafete", new Uri("http://www.studierendenwerk-aachen.de/speiseplaene/forum-w.html")},*/
        };

        /// <summary>
        /// Gets the next Mensa (sorted by the dictionary) of the provided one.
        ///     Checks for enabled canteens
        /// </summary>
        public static string getNextMensaName(string name)
        {
            // Idea: iterate thhrough the dict and get the first item, that is after the name-mensa
            bool flag = false;
            string newMensa = null;
            foreach (var mens in Mensen.Keys)
            {
                if (flag)
                {
                    // add config-cehck here
                    if (!App.MensaActive(mens))
                        continue;
                    newMensa = mens;
                    break;
                }
                if (mens == name)
                    flag = true;
            }

            if (newMensa != null)
                return newMensa;

            foreach (var mens in Mensen.Keys)
            {
                // add config-Check here
                if (App.MensaActive(mens))
                    return mens;
            }

            // every Mensa is deactivated...
            return name;
        }

        /// <summary>
        /// Gets the previous Mensa (sorted by the dictionary) of the provided one.
        ///     Checks for enabled canteens
        /// </summary>
        public static string getPreviousMensaName(string name)
        {
            // Idea: iterate thhrough the dict and get the first item, that is before the name-mensa
            string newMensa = null;
            foreach (var mens in Mensen.Keys)
            {
                if (mens == name)
                    break;

                // Add config-check here
                if (!App.MensaActive(mens))
                    continue;
                newMensa = mens;
            }

            if (newMensa != null)
                return newMensa;

            newMensa = name;

            foreach (var mens in Mensen.Keys)
            {
                // add config-Check here
                if (App.MensaActive(mens))
                    newMensa = mens;
            }

            // every Mensa is deactivated...
            return newMensa;
        }

        public static bool DownloadError = false;

        /// <summary>
        /// creates a dictionary of the web source text and the MensaName
        /// </summary>
        private async static Task<Dictionary<string, string>> getSourcesAsync(Dictionary<string, Uri> urls)
        {
            // Policy (input): urls.keys == MensaName, urls.Value == Uri of the plan
            // Policy (output): key==source, Value == MensaName
            DownloadError = false;
            int downloads = 0;
            Dictionary<string, string> dict = new Dictionary<string, string>();

            Mutex mutex = new Mutex();

            foreach (KeyValuePair<string, Uri> tuple in urls)
            {
                WebClient client = new WebClient();
				client.Encoding = Encoding.UTF8;
                //client.DownloadStringCompleted += client_DownloadStringCompleted;
                client.DownloadStringCompleted += ((object sender, DownloadStringCompletedEventArgs e) =>
                    {
                        try 
                        {
                            // Error occured?
                            if (e.Error != null)
                            {
                                DownloadError = true;
                            }
                            else
                            {
                                // No error -> Add to dict.
                                string result = e.Result;
                                mutex.WaitOne();
                                dict.Add(result, tuple.Key);
                                mutex.ReleaseMutex();
                            }
                            
                        }
                        catch (Exception)
                        {
                            // Error while getting data. Set flag!
                            DownloadError = true;
                        }
                        finally
                        {
                            // In every case, count this for a finished task
                            mutex.WaitOne();
                            downloads++;
                            mutex.ReleaseMutex();
                        }
                    });
                client.DownloadStringAsync(tuple.Value);
            }

            // Wait for the actions to complete
			while (downloads != urls.Count)
			{
				await Task.Delay (200);
				//Thread.Sleep(200);
			}

            string message = Localization.Localize("DownloadFail");

            if (DownloadError)
            {
                // Check for errors, inform user!
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    App.Current.MainPage.DisplayAlert("Error", message, "OK");
                });
            }

            return dict;

        }

    }

}
