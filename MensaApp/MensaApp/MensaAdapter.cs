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
            Mensa.MenuDB.Instance.LoadDB(sl);

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
        
        static Dictionary<string, Uri> Mensen = new Dictionary<string, Uri>()
        {
            { "Academica", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/academica-w.html")},
            { "Ahornstrasse", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/ahornstrasse-w.html")},  
        };

        private async static Task<Dictionary<string, string>> getSourcesAsync(Dictionary<string, Uri> urls)
        {
            // Policy (input): urls.keys == MensaName, urls.Value == Uri of the plan
            // Policy (output): key==source, Value == MensaName

            Dictionary<string, string> dict = new Dictionary<string, string>();

            Mutex mutex = new Mutex();

            foreach (KeyValuePair<string, Uri> tuple in urls)
            {
                WebClient client = new WebClient();
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
                await Task.Delay(200);

            return dict;

        }

    }
}
