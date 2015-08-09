using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using Mensa;

namespace MensaConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Uri uri = new Uri("http://www.studentenwerk-aachen.de/speiseplaene/academica-w.html");
            /*Uri uri = new Uri("http://www.studentenwerk-aachen.de/de/Gastronomie/cafeteria-forum-cafete-wochenplan.html");

            HttpClient http = new HttpClient();
            var response = http.GetByteArrayAsync(uri).Result;
            String source = Encoding.UTF8.GetString(response);*/
            //String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);

            //Mensa.MenuDB.Instance.ImportFromSource(source, "Mensa Academica");

            Mensa.MenuDB.Instance.ImportFromSources(getSourcesAsync(Mensen).Result);

            //source = source.Replace("\n", " ");

            ////string regex = "<h3 class=\"default-headline\">\\s*<a.*?>(.*?)<\\/a>";
            ////string days = "<h3.*?>\\s*?<a.*?>\\s*?(.*?\\n.*?)\\s*?<\\/a>";
            //string days = "<h3.*?>\\s*?<a.*?>\\s*?(.*?)\\s*?<\\/a>\\s*?<\\/h3>";
            ////string regex = "<div.*?>\\s*?(<table.*?>.*?<\\/table>).*?(<table.*?>.*?<\\/table>).*?<\\/div>";
            
            ///*
            // * 
            // * Groups:
            // *  1: Date
            // *  2: table of dishes
            // *  3: table of side dishes
            // * 
            // * */
            //string regex = days+"\\s*?<div.*?>\\s*?(<table.*?>.*?<\\/table>).*?(<table.*?>.*?<\\/table>).*?<\\/div>";
            ////Match match = Regex.Match(source, regex);

            //MatchCollection matches = Regex.Matches(source, regex);

            //foreach (Match m in matches)
            //{
            //    Console.WriteLine("=== Found ===");
            //    getDateFromDayString(m.Groups[1].Value);
            //    getDishesFromTable(m.Groups[2].Value);
            //    getSideDishesFromTable(m.Groups[3].Value);
            //    //Console.WriteLine(m.Groups[1].Value + "||" + m.Groups[2].Value + "||" + m.Groups[3].Value);
            //}



            Console.ReadLine();
        }

        /*static Dictionary<string, Uri> Mensen = new Dictionary<string, Uri>()
        {
            //{ "Academica", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/academica-w.html")},
            //{ "Ahornstrasse", new Uri("http://www.studentenwerk-aachen.de/speiseplaene/ahornstrasse-w.html")},  
            { "Forum Cafete", new Uri("http://www.studentenwerk-aachen.de/de/Gastronomie/cafeteria-forum-cafete-wochenplan.html")},  
        };*/

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

        public async static Task<Dictionary<string,string>> getSourcesAsync(Dictionary<string,Uri> urls)
        {
            // Policy (input): urls.keys == MensaName, urls.Value == Uri of the plan
            // Policy (output): key==source, Value == MensaName

            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (KeyValuePair<string,Uri> tuple in urls)
            {
                HttpClient client = new HttpClient();
                byte[] response = await client.GetByteArrayAsync(tuple.Value);
                String source = Encoding.UTF8.GetString(response);
                dict.Add(source,tuple.Key);
            }
            return dict;
            
        }

        private static void getSideDishesFromTable(string p)
        {
            /*
             * 
             * Groups:
             *  1: Dish kind
             *  2: Name of Dish
             * 
             * */
            string regex = "<tr.*?>.*?<td.*?>(.*?)<\\/td>.*?<td.*?>(.*?)<\\/td>.*?<td><\\/td>.*?<\\/tr>";
            MatchCollection matches = Regex.Matches(p, regex);
            foreach (Match m in matches)
            {
                Console.WriteLine(m.Groups[1].Value.Trim() + ": " + m.Groups[2].Value.Trim());
            }
        }

        private static void getDateFromDayString(string p)
        {
            var split = p.Split(',');
            var innersplit = split[1].Split('.');
            DateTime date = new DateTime(int.Parse(innersplit[2]), int.Parse(innersplit[1]), int.Parse(innersplit[0]));
        }

        private static void getDishesFromTable(string p)
        {
            /*
             * 
             * Groups:
             *  1: Dish kind
             *  2: Name of Dish
             *  3: Price
             * 
             * */
            string regex = "<tr.*?>.*?<td.*?>(.*?)<\\/td>.*?<td.*?>(.*?)<\\/td>.*?<td.*?>(.*?)<\\/td>.*?<\\/tr>";
            MatchCollection matches = Regex.Matches(p, regex);
            foreach (Match m in matches)
            {
                Console.WriteLine(m.Groups[1].Value.Trim() + ": " + m.Groups[2].Value.Trim() + " for " + m.Groups[3].Value.Trim());
            }
        }
    }
}
