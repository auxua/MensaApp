using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Mensa;
using System.Threading;

namespace MensaApp
{
    class MensaAdapter
    {
        
        public async static Task<bool> CatchMensaDataAsync()
        {
            // First, Load the Data from persistent storage
            AppLoadStoreMenuDB sl = new AppLoadStoreMenuDB();
            //Mensa.MenuDB.Instance.LoadDB(sl);

            // Check the latest Day of the data
            if (!Mensa.MenuDB.Instance.isOutdated())
                return true;

            // The data is Outdated! Try getting the new data
            try
            {
                Dictionary<string, string> dict = await getSourcesAsync(Mensen);
                return Mensa.MenuDB.Instance.ImportFromSources(dict);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public static Dictionary<string, Uri> Mensen = new Dictionary<string, Uri>()
        {
            { "Academica", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/academica-w.html")},
            { "Ahornstrasse", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/ahornstrasse-w.html")},  
            { "Bistro", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/templergraben-w.html")}, 
            { "Bayernallee", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/bayernallee-w.html")},
            { "Eupener Straße", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/eupenerstrasse-w.html")},
            { "Gastro Goethe", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/goethestrasse-w.html")},
            { "Mensa Vita", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/vita-w.html")},
            { "Forum Cafete", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/forum-w.html")},
        };

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

        private async static Task<Dictionary<string, string>> getSourcesAsync(Dictionary<string, Uri> urls)
        {
            // Policy (input): urls.keys == MensaName, urls.Value == Uri of the plan
            // Policy (output): key==source, Value == MensaName

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
                            string result = e.Result;
                            mutex.WaitOne();
                            dict.Add(result, tuple.Key);
                            mutex.ReleaseMutex();
                        }
                        catch (Exception)
                        {
                            dict.Add("","");
                        }
                    });
                client.DownloadStringAsync(tuple.Value);
            }

            // Wait for the actions to complete
			while (dict.Count != urls.Count)
			{
				await Task.Delay (200);
				//Thread.Sleep(200);
			}

            return dict;

        }

    }

}
