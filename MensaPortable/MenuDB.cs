﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MensaPortable.DataTypes;

namespace MensaPortable
{
    public static class DishesExtension
    {
        const string prefix = "<span class=\"menue-nutr\">+ ";

        public static void ExtractNutritionInfo(this Dish dish)
        {
            // Check for Nutrition info
            if (!dish.Name.StartsWith(prefix)) return;

            dish.NutritionInfo = new Nutrition();
            var splits = dish.Name.Split(new string[] { "<br />", "<br/>" , "<div>" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var inner in splits)
            {
                if (dish.NutritionInfo.Caloric == null && inner.StartsWith("<div class=\"nutr-info\">"))
                {
                    dish.NutritionInfo.Caloric = inner.Replace("<div class=\"nutr-info\">", "").Replace("Brennwert = ", "");
                }
                else if (dish.NutritionInfo.Caloric == null && inner.StartsWith("Brennwert"))
                    dish.NutritionInfo.Caloric = inner.Replace("Brennwert = ", "").Replace("</div>","");
                else if (dish.NutritionInfo.Fat == null && inner.StartsWith("Fett"))
                    dish.NutritionInfo.Fat = inner.Replace("Fett = ", "").Replace("</div>", "");
                else if (dish.NutritionInfo.Carbohydrates == null && inner.StartsWith("Kohlenhydrate"))
                    dish.NutritionInfo.Carbohydrates = inner.Replace("Kohlenhydrate = ", "").Replace("</div>", "");
                else if (dish.NutritionInfo.Proteins == null && inner.StartsWith("Eiweiß"))
                    dish.NutritionInfo.Proteins = inner.Replace("Eiweiß = ", "").Replace("</div>", "");
            }
            // Cleanup name
            dish.Name = splits.First().Replace(prefix, "").Replace("<div class=\"nutr-info\">", "");

            // Cleanup different trailing texts
            if (String.IsNullOrWhiteSpace(dish.NutritionInfo.Proteins)) return;
            var pos = dish.NutritionInfo.Proteins.IndexOf("g");
            if (dish.NutritionInfo.Proteins.Length <= pos+1) return;
            dish.NutritionInfo.Proteins = dish.NutritionInfo.Proteins.Remove(pos+1);
            
        }
    }

    public class MenuDB
    {

        #region Singleton implementation
        
        private static MenuDB instance;

        private MenuDB()
        {
            this.Dishes = new List<DataTypes.Dish>();
        }

        public static MenuDB Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MenuDB();
                }
                return instance;
            }
        }

        #endregion

        #region Data Fields

        internal List<DataTypes.Dish> Dishes;

        #endregion

        #region QueryBuilder

        /// <summary>
        /// To query the DB, these QueryBuilder-Objects should be used to create the query
        /// </summary>
        public class QueryBuilder
        {
            private string Mensa = null;
            private DateTime date = default(DateTime);
            private string Kind = null;
            private bool SideDishes = true;
            private bool MainDishes = true;
            //private bool observed = false;

            public QueryBuilder ByMensa(string m)
            {
                this.Mensa = m;
                return this;
            }

            public QueryBuilder ByDate(DateTime d)
            {
                this.date = d;
                return this;
            }

            public QueryBuilder ByKind(string k)
            {
                this.Kind = k;
                return this;
            }

            public QueryBuilder NoSideDishes()
            {
                this.SideDishes = false;
                return this;
            }

            public QueryBuilder NoMainDishes()
            {
                this.MainDishes = false;
                return this;
            }

            /*public QueryBuilder AsObservable()
            {
                this.observed = true;
                return this;
            }*/


            public IList<DataTypes.Dish> ExecuteQuery()
            {
                // Get all items matching the Query
                List<DataTypes.Dish> list = MenuDB.Instance.Dishes.FindAll((x) =>
                    {
                        // If Query-Date is set and the item is not matching
                        if ((this.date != default(DateTime)) && (!this.date.Equals(x.Date)))
                            return false;

                        if ((this.Kind != null) && (!(this.Kind == x.Kind)) && (!(x.isSideDish())))
                            return false;

                        if ((!this.MainDishes) && (!x.isSideDish()))
                            return false;

                        if ((!this.SideDishes) && (x.isSideDish()))
                            return false;

                        if ((this.Mensa != null) && (x.Mensa != this.Mensa))
                            return false;

                        return true;
                    });
                return list;
            }
        }

        #endregion     

        #region Importer

        public bool ImportFromSources(Dictionary<string,string> sourcesFromMensa)
        {
            foreach(KeyValuePair<string,string> s in sourcesFromMensa)
            {
                if (!ImportFromSource(s.Key, s.Value))
                    return false;
            }
            return true;
        }

        public bool ImportFromSource(string source, string MensaName)
        {
            try
            {
                /*
                *   Remark: Complete Regex matches computation takes a bit time,
                *   if there are no matches, it takes much time. 
                *   => Quick Fixes have better performance (for now)
                */

                // Quick Fix: If there are 10 "geschlossen", there is no data
                /*string haystack = source;
                string needle = "geschlossen";
                int needleCount = (haystack.Length - source.Replace(needle, "").Length) / needle.Length;
                if (needleCount > 9)
                    return true;*/

                // End of April 2016: STW changed Syntax, now every dish is listed with "geschlossen" - optimization invalid

                /*
                // Quick Fix: If there are 10 "Mensa geschlossen", there is no data
                string haystack = source;
                string needle = "Mensa geschlossen";
                int needleCount = (haystack.Length - source.Replace(needle, "").Length) / needle.Length;
                if (needleCount > 9)
                    return true;

                // Quick Fix 2: If there are 10 "Geschlossen wegen Umbauarbeiten", there is no data
                haystack = source;
                needle = "Geschlossen";
                needleCount = (haystack.Length - source.Replace(needle, "").Length) / needle.Length;
                if (needleCount > 9)
                    return true;*/

                // Remove newlines
                source = source.Replace("\n", " ");
                
                //string regex = "<h3 class=\"default-headline\">\\s*<a.*?>(.*?)<\\/a>";
                //string days = "<h3.*?>\\s*?<a.*?>\\s*?(.*?\\n.*?)\\s*?<\\/a>";
                string days = "<h3.*?>\\s*?<a.*?>\\s*?(.*?)\\s*?<\\/a>\\s*?<\\/h3>";
                //string regex = "<div.*?>\\s*?(<table.*?>.*?<\\/table>).*?(<table.*?>.*?<\\/table>).*?<\\/div>";
            
                /*
                 * 
                 * Groups:
                 *  1: Date
                 *  2: table of dishes
                 *  3: table of side dishes
                 * 
                 * */
                string regex = days+"\\s*?<div.*?>\\s*?(<table.*?>.*?<\\/table>).*?(<table.*?>.*?<\\/table>).*?<\\/div>";
                //Match match = Regex.Match(source, regex);

                MatchCollection matches = Regex.Matches(source, regex);

                foreach (Match m in matches)
                {
                    DateTime date = getDateFromDayString(m.Groups[1].Value);
                    this.Dishes.AddRange(getDishesFromTable(m.Groups[2].Value,MensaName,date));
                    this.Dishes.AddRange(getSideDishesFromTable(m.Groups[3].Value,date,MensaName));
                    //Console.WriteLine(m.Groups[1].Value + "||" + m.Groups[2].Value + "||" + m.Groups[3].Value);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private List<DataTypes.Dish> getSideDishesFromTable(string p,DateTime date, string MensaName)
        {
            List<DataTypes.Dish> list = new List<DataTypes.Dish>();
            /*
             * 
             * Groups:
             *  1: Dish kind
             *  2: Name of Dish
             * 
             * */

            // Regex for SPAN-based dishes with "oder"
            //string regex = "<tr.*?>.*?<td.*?>.*?<span.*?>(.*?)<\\/span>.*?<span.*?>(.*?)<\\/span>.*?<\\/td>.*?<\\/tr>";

            // Regex for SPAN-based dishes without "oder"
            //string regex = "<tr.*?>.*?<td.*?>.*?<span.*?>(.*?)<\\/span>.*?<span.*?>(.*?<span class=\"or\">oder<\\/span>.*?)<\\/span>.*?<\\/td>.*?<\\/tr>";
            //string regex = "<tr.*?>.*?<td.*?>.*?<span.*?>(.*?)<\\/span>.*?<span.*?>(.*?<span class=\"or\">oder<\\/span>.*?)<\\/span>\\s*<\\/td>.*?<\\/tr>";
            // New Regex - adapted to new STW page, also Side dishes may not have a kind name
            string regex = "<tr>.*?<span.*?>(.*?)<\\/span><span.*?>(.*?)<\\/span><\\/td>";
            

            //string regex = "<tr.*?>.*?<td.*?>(.*?)<\\/td>.*?<td.*?>(.*?)<\\/td>.*?<td><\\/td>.*?<\\/tr>";
            MatchCollection matches = Regex.Matches(p, regex);

            foreach (Match m in matches)
            {
                //Console.WriteLine(m.Groups[1].Value.Trim() + ": " + m.Groups[2].Value.Trim());
                DataTypes.Dish dish = new DataTypes.Dish(m.Groups[2].Value.Trim(), m.Groups[1].Value.Trim(), date, MensaName);
                dish.ExtractNutritionInfo();
                list.Add(dish);
            }
            return list;
        }

        private DateTime getDateFromDayString(string p)
        {
            var split = p.Split(',');
            var innersplit = split[1].Split('.');
            DateTime date = new DateTime(int.Parse(innersplit[2]), int.Parse(innersplit[1]), int.Parse(innersplit[0]));
            return date;
        }

        private List<DataTypes.Dish> getDishesFromTable(string p, string MensaName, DateTime date)
        {
            List<DataTypes.Dish> list = new List<DataTypes.Dish>();
            /*
             * 
             * Groups:
             *  1: Dish kind
             *  2: Name of Dish
             *  3: Nutrition Information
             *  4: Price
             * 
             * */
            //string regex = "<tr.*?>.*?<td.*?>(.*?)<\\/td>.*?<td.*?>(.*?)<\\/td>.*?<td.*?>(.*?)<\\/td>.*?<\\/tr>";
            //string regex = "<tr.*?>.*?<td.*?>.?*<span.*?>(.*?)<\\span>.*?<span.*?>(.*?)<\\/span>.*?<span.*?>(.*?)<\\/span>.*?<\\/td>.*?<\\/tr>";
            // Version 1.6.3 regex
            //string regex = "<tr.*?>.*?<td.*?>.*?<span.*?>(.*?)<\\/span>.*?<span.*?>(.*?)<\\/span>.*?<span.*?>(.*?)<\\/span>.*?<\\/td>.*?<\\/tr>";
            // New regex (new STW site, including Nutrition info)
            string regex = "<tr.*?>.*?<td.*?>.*?<span.*?>(.*?)<\\/span>.*?<span.*?><span.*?>(.*?)<\\/div>.*?<span.*?>(.*?)<\\/span>.*?<\\/td>.*?<\\/tr>";
            MatchCollection matches = Regex.Matches(p, regex);
            foreach (Match m in matches)
            {
                //Console.WriteLine(m.Groups[1].Value.Trim() + ": " + m.Groups[2].Value.Trim() + " for " + m.Groups[3].Value.Trim());
                //DataTypes.Dish dish = new DataTypes.Dish(m.Groups[2].Value.Trim(), m.Groups[1].Value.Trim(), date, MensaName, m.Groups[3].Value.Trim());
                DataTypes.Dish dish = new DataTypes.Dish(
                    m.Groups[2].Value.Trim(),
                    m.Groups[1].Value.Trim(),
                    date, 
                    MensaName, 
                    m.Groups[3].Value.Trim()
                    );
                dish.ExtractNutritionInfo();
                list.Add(dish);
            }
            return list;
        }

        #endregion

        #region Store/Load interface

        public interface StoreLoader
        {
            List<DataTypes.Dish> LoadDishes();

            bool StoreDishes(List<DataTypes.Dish> dishes);
        }

        public bool StoreDB(StoreLoader sl)
        {
            return sl.StoreDishes(this.Dishes);
        }

        public bool LoadDB(StoreLoader sl)
        {
            List<DataTypes.Dish> dishes = sl.LoadDishes();
            if (dishes == null)
                return false;

            Dishes.AddRange(dishes);
            return true;
        }

        #endregion

        public void Reset(StoreLoader sl)
        {
            this.Dishes.Clear();
            this.StoreDB(sl);
        }
        
        public void Reset()
        {
            this.Dishes.Clear();
        }

        public DateTime getNextAvailableDay(DateTime dt)
        {
            // In case of empty set (e.g. holidays), return old value;
            if (this.Dishes.Count == 0) return dt;
            DateTime now = dt.AddMonths(1);
            
            List<DataTypes.Dish> list = MenuDB.Instance.Dishes.FindAll((x) =>
            {
                
                // If Query-Date is set and the item is not matching
                if (DateTime.Compare(dt,x.Date) <0)
                {
                    if (DateTime.Compare(now,x.Date) > 0)
                    {
                        now = x.Date;
                    }
                    return true;
                }
                return false;
            });

            if (now.Equals(dt.AddMonths(1)))
            {
                now = dt;
            }
            return now;
        }

        public DateTime getPreviousAvailableDay(DateTime dt)
        {
            //DateTime dt_back = dt;
            if (this.Dishes.Count == 0) return dt;
            dt = dt.Date;
            DateTime now = dt.Date.AddMonths(-2);
            //now = now.AddDays(-1);

            List<DataTypes.Dish> list = MenuDB.Instance.Dishes.FindAll((x) =>
            {

                // If Query-Date is set and the item is not matching
                if (DateTime.Compare(dt, x.Date) > 0)
                {
                    if (DateTime.Compare(now, x.Date) < 0)
                    {
                        now = x.Date;
                    }
                    return true;
                }
                return false;
            });

            /*if (DateTime.Compare(now, dt) == 0)
                return dt_back;*/

            if (DateTime.Compare(now, dt.Date.AddMonths(-1))<0)
                return dt;

            return now;
        }
        
        public bool isOutdated()
        {
            if (MenuDB.Instance.Dishes.Count == 0)
                return true;
            return !MenuDB.Instance.Dishes.Any((x) =>
                {
                    return (DateTime.Compare(x.Date, DateTime.Now.Date) >= 0);
                });
            
        }
    }
}
