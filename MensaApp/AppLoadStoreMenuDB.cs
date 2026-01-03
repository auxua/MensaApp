using System;
using System.Collections.Generic;
using System.Text;
using MensaPortable;
using Newtonsoft.Json;

namespace MensaApp
{
    class AppLoadStoreMenuDB : MenuDB.IStoreLoader
    {
        private const string MenuDbKey = "MenuDB";

        public List<Dish> LoadDishes()
        {
            if (!Preferences.ContainsKey(MenuDbKey))
                return new List<Dish>();

            var db = Preferences.Get(MenuDbKey, string.Empty);
            if (string.IsNullOrWhiteSpace(db))
                return new List<Dish>();

            try
            {
                return JsonConvert.DeserializeObject<List<Dish>>(db)
                       ?? new List<Dish>();
            }
            catch
            {
                // Broken DB --> Drop broken data
                Preferences.Remove(MenuDbKey); 
                return new List<Dish>();
            }
        }

        public bool StoreDishes(List<Dish> dishes)
        {
            dishes ??= new List<Dish>(); // Null handling
            Preferences.Set(MenuDbKey, JsonConvert.SerializeObject(dishes));
            return true;
        }
    }
}
