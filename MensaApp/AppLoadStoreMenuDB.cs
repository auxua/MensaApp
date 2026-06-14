using System;
using System.Collections.Generic;
using System.IO;
using MensaPortable;
using Newtonsoft.Json;

namespace MensaApp
{
    class AppLoadStoreMenuDB : MenuDB.IStoreLoader
    {
        private const string MenuDbKey = "MenuDB";

#if WINDOWS
        private const string MenuDbFileName = "dishes.json";

        private static string MenuDbFilePath =>
            Path.Combine(FileSystem.AppDataDirectory, MenuDbFileName);
#endif

        public List<Dish> LoadDishes()
        {
#if WINDOWS
            if (!File.Exists(MenuDbFilePath))
                return new List<Dish>();

            var db = File.ReadAllText(MenuDbFilePath);

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
                TryDeleteMenuDbFile();
                return new List<Dish>();
            }
#else
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
#endif
        }

        public bool StoreDishes(List<Dish> dishes)
        {
            dishes ??= new List<Dish>();

            var json = JsonConvert.SerializeObject(dishes);

#if WINDOWS
            try
            {
                var directory = Path.GetDirectoryName(MenuDbFilePath);

                if (!string.IsNullOrWhiteSpace(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(MenuDbFilePath, json);

                if (Preferences.ContainsKey(MenuDbKey))
                    Preferences.Remove(MenuDbKey);

                return true;
            }
            catch
            {
                return false;
            }
#else
            Preferences.Set(MenuDbKey, json);
            return true;
#endif
        }

#if WINDOWS
        private static void TryDeleteMenuDbFile()
        {
            try
            {
                if (File.Exists(MenuDbFilePath))
                    File.Delete(MenuDbFilePath);
            }
            catch
            {
                // Ignore cleanup errors.
            }
        }
#endif
    }
}