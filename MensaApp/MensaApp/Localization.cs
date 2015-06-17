using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;
using Xamarin.Forms;
using System.Reflection;

namespace MensaApp
{
    public class Localization
    {
        /// <remarks>
        /// Maybe we can cache this info rather than querying every time
        /// </remarks>
        public static string Locale()
        {
            return DependencyService.Get<ILocale>().GetCurrent();
        }

        public static string Localize(string key, string comment)
        {

            // FOR DEBUGGING
            var assembly = typeof(Localization).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
                System.Diagnostics.Debug.WriteLine("found resource: " + res);

            var netLanguage = Locale();

            //ResourceManager temp = new ResourceManager("MensaApp.MensaAppResources", typeof(Localization).GetTypeInfo().Assembly);
            ResourceManager temp = new ResourceManager("MensaApp."+ProjectInfix()+".MensaAppResources", typeof(Localization).GetTypeInfo().Assembly);

            string result = temp.GetString(key, new CultureInfo(netLanguage));

            return result;
        }

        public static string ProjectInfix()
        {
            if (Device.OS == TargetPlatform.Android)
                return "Droid";
            if (Device.OS == TargetPlatform.iOS)
                return "iOS";
            return "WinPhone";
        }
    }
}
