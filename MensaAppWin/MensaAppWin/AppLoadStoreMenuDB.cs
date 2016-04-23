using System;
using System.Collections.Generic;
using System.Text;
using Mensa;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MensaAppWin
{
    class AppLoadStoreMenuDB : MenuDB.StoreLoader
    {
        public List<DataTypes.Dish> LoadDishes()
        {
            if (Application.Current.Properties.ContainsKey("MenuDB"))
            {
                return JsonConvert.DeserializeObject<List<DataTypes.Dish>>(Application.Current.Properties["MenuDB"] as string);
            }
            return new List<DataTypes.Dish>();
        }

        public bool StoreDishes(List<DataTypes.Dish> dishes)
        {
            Application.Current.Properties["MenuDB"] = JsonConvert.SerializeObject(dishes);
            Application.Current.SavePropertiesAsync();
            return true;
        }
    }
}
