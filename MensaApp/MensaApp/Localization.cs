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

        public static string Localize(string key)
        {
#if DEBUG
            // FOR DEBUGGING
            var assembly = typeof(Localization).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
                System.Diagnostics.Debug.WriteLine("found resource: " + res);
#endif
            var netLanguage = Locale();

            ResourceManager temp;

			/*
			 * Workaround!
			 * 
			 * On Visual Studio Build, use the upper line,
			 * using Xamarin on iOS user the second line (with ResourceFiles.)
			 * 
			 * This is a Bug in Xamarin and will be fixed later... hopefully
			 */ 

            //temp = new ResourceManager("MensaApp." + ProjectInfix() + ".MensaAppResources", typeof(Localization).GetTypeInfo().Assembly);

			temp = new ResourceManager("MensaApp." + ProjectInfix() + ".ResourceFiles.MensaAppResources", typeof(Localization).GetTypeInfo().Assembly);

            //ResourceManager temp = new ResourceManager("MensaApp.MensaAppResources", typeof(Localization).GetTypeInfo().Assembly);
            /*if (Device.OS != TargetPlatform.iOS)
            {
                temp = new ResourceManager("MensaApp." + ProjectInfix() + ".MensaAppResources", typeof(Localization).GetTypeInfo().Assembly);
            } 
            else
            {
                temp = new ResourceManager("MensaApp." + ProjectInfix() + ".ResourceFiles.MensaAppResources", typeof(Localization).GetTypeInfo().Assembly);
            }*/
			//ResourceManager temp = new ResourceManager("MensaApp."+ProjectInfix()+".MensaAppResources", typeof(Localization).GetTypeInfo().Assembly);

            var cult = new CultureInfo(netLanguage);

            string result = temp.GetString(key, cult);

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
