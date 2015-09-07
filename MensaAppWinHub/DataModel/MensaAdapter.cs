﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Mensa;
using System.Threading;
using System.Net.Http;

namespace MensaAppWinHub.Data
{
    class MensaAdapter
    {
        
        public async static Task<bool> CatchMensaDataAsync()
        {
            // First, Load the Data from persistent storage
            //AppLoadStoreMenuDB sl = new AppLoadStoreMenuDB();
            //Mensa.MenuDB.Instance.LoadDB(sl);

            // Check the latest Day of the data
            if (!Mensa.MenuDB.Instance.isOutdated())
                return true;

            // The data is Outdated! Try getting the new data
            try
            {
                Dictionary<string, string> dict = await getSourcesAsync(Mensen);
                var res =  Mensa.MenuDB.Instance.ImportFromSources(dict);
                //Mensa.MenuDB.Instance.StoreDB(sl);
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
            { "Academica", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/academica-w.html")},
            { "Ahornstrasse", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/ahornstrasse-w.html")},  
            { "Bistro", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/templergraben-w.html")}, 
            { "Bayernallee", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/bayernallee-w.html")},
            { "Eupener Straße", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/eupenerstrasse-w.html")},
            { "Gastro Goethe", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/goethestrasse-w.html")},
            { "Mensa Vita", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/vita-w.html")},
            { "Forum Cafete", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/forum-w.html")},
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
                    /*if (!App.MensaActive(mens))
                        continue;*/
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
                //if (App.MensaActive(mens))
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
                /*if (!App.MensaActive(mens))
                    continue;*/
                newMensa = mens;
            }

            if (newMensa != null)
                return newMensa;

            newMensa = name;

            foreach (var mens in Mensen.Keys)
            {
                // add config-Check here
                //if (App.MensaActive(mens))
                    newMensa = mens;
            }

            // every Mensa is deactivated...
            return newMensa;
        }

        /// <summary>
        /// creates a dictionary of the web source text and the MensaName
        /// </summary>
        private async static Task<Dictionary<string, string>> getSourcesAsync(Dictionary<string, Uri> urls)
        {
            // Policy (input): urls.keys == MensaName, urls.Value == Uri of the plan
            // Policy (output): key==source, Value == MensaName

            Dictionary<string, string> dict = new Dictionary<string, string>();

            Mutex mutex = new Mutex();

            foreach (KeyValuePair<string, Uri> tuple in urls)
            {
                HttpClient client = new HttpClient();

                try
                {
                    string result = await client.GetStringAsync(tuple.Value);
                    mutex.WaitOne();
                    dict.Add(result, tuple.Key);
                    mutex.ReleaseMutex();
                }
                catch (Exception ex)
                {
                    dict.Add("", "");
                }
                
            }


            return dict;

        }

    }

}