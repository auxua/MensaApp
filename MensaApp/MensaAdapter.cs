using MensaApp.Resources.Strings;
using MensaPortable;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MensaApp
{
    public class MensaAdapter
    {

        private IDialogService dialogService;

        public MensaAdapter(IDialogService dialog) => dialogService = dialog;

        public async Task<bool> CatchMensaDataAsync()
        {

            AppLoadStoreMenuDB sl = new AppLoadStoreMenuDB();
#if DEBUG
            // Debugging? Reset the internal storage
            MenuDB.Instance.Reset(sl);
#endif
            // First, Load the Data from persistent storage
            MenuDB.Instance.LoadDB(sl);

            // Check the latest Day of the data
            if (!MenuDB.Instance.IsOutdated())
                return true;

            // The data is Outdated! Try getting the new data
            try
            {
                // Reset the whole Database to prevent too much Data on the Device
                MenuDB.Instance.Reset(sl);

                Dictionary<string, string> dict = await GetSourcesAsync(Mensen);

                var res = await Task.Run(() =>
                {
                    var r = MenuDB.Instance.ImportFromSources(dict);
                    MenuDB.Instance.StoreDB(sl);
                    return r;
                });

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
        public Dictionary<string, Uri> Mensen = new Dictionary<string, Uri>()
        {
            { "Academica", new Uri("https://www.studierendenwerk-aachen.de/speiseplaene/academica-w.html")},
            { "Ahornstrasse", new Uri("https://www.studierendenwerk-aachen.de/speiseplaene/ahornstrasse-w.html")},
            { "Bistro", new Uri("https://www.studierendenwerk-aachen.de/speiseplaene/templergraben-w.html")},
            { "Bayernallee", new Uri("https://www.studierendenwerk-aachen.de/speiseplaene/bayernallee-w.html")},
            { "Eupener Straße", new Uri("https://www.studierendenwerk-aachen.de/speiseplaene/eupenerstrasse-w.html")},
            { "Mensa Vita", new Uri("https://www.studierendenwerk-aachen.de/speiseplaene/vita-w.html")},
            { "Mensa Jülich", new Uri("https://www.studierendenwerk-aachen.de/speiseplaene/juelich-w.html")},
            { "KMSC", new Uri("https://www.studierendenwerk-aachen.de/speiseplaene/kmac-w.html")},
            { "Südpark", new Uri("https://www.studierendenwerk-aachen.de/speiseplaene/suedpark-w.html")},	
			/*{ "Forum Cafete", new Uri("https://www.studierendenwerk-aachen.de/speiseplaene/forum-w.html")},*/
        };

        public List<string> GetAllMensen()
            => Mensen.Keys.ToList();

        public List<string> GetActiveMensen()
        {
            var all = GetAllMensen();
            List<string> mens = new List<string>();
            foreach (var item in all)
                if (App.MensaActive(item))
                    mens.Add(item);
            return mens;
        }

        /// <summary>
        /// Gets the next Mensa (sorted by the dictionary) of the provided one.
        ///     Checks for enabled canteens
        /// </summary>
        public string getNextMensaName(string name)
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
        public string getPreviousMensaName(string name)
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

        public bool DownloadError = false;

        /*

        /// <summary>
        /// creates a dictionary of the web source text and the MensaName
        /// </summary>
        private async Task<Dictionary<string, string>> getSourcesAsync(Dictionary<string, Uri> urls)
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
                await Task.Delay(200);
                //Thread.Sleep(200);
            }

            string message = AppRessources.DownloadFail;

            if (DownloadError)
            {
                // Check for errors, inform user!
                await dialogService.ShowErrorAsync(message);
            }

            return dict;

        }

        */

        private readonly HttpClient _http = new();

        /// <summary>
        /// creates a dictionary of the web source text and the MensaName
        /// </summary>
        public async Task<Dictionary<string, string>> GetSourcesAsync(
            Dictionary<string, Uri> urls,
            CancellationToken ct = default)
        {
            DownloadError = false;

            // pro URL eine Task
            var tasks = urls.Select(async kvp =>
            {
                var mensaName = kvp.Key;
                var uri = kvp.Value;

                try
                {
                    // "nur" HTML als Text
                    var html = await _http.GetStringAsync(uri, ct);

                    // kompatibel zu deinem bisherigen Format: key = source, value = mensaName
                    return (ok: true, html: html, name: mensaName);
                }
                catch
                {
                    return (ok: false, html: "", name: mensaName);
                }
            }).ToArray();

            var results = await Task.WhenAll(tasks);

            var dict = new Dictionary<string, string>();
            foreach (var r in results)
            {
                if (!r.ok)
                {
                    DownloadError = true;
                    continue;
                }

                // Achtung: kann kollidieren, wenn 2 Mensen identisches HTML liefern
                // Dann lieber dict[r.name] = r.html; (empfohlen)
                if (!dict.ContainsKey(r.html))
                    dict.Add(r.html, r.name);
            }

            if (DownloadError)
                await dialogService.ShowErrorAsync(AppRessources.DownloadFail);

            return dict;
        }
    }

}
