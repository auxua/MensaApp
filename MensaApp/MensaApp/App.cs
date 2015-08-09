using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace MensaApp
{
	public class App : Application
	{
        public readonly static string Version = "1.0.1";
        
        public App ()
		{
			// The root page of your application
            MainPage = new NavigationPage(new pages.MensaPage());
            //MainPage = new pages.APage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

        
        #region Settings/Config

        public static bool MensaActive(string name)
        {
            if (Application.Current.Properties.ContainsKey(name))
            {
                string value = Application.Current.Properties[name] as string;
                return (value == "True");
            }
            Application.Current.Properties[name] = "True";
            return true;
        }

        public static void MensaActivate(string name)
        {
            Application.Current.Properties[name] = "True";
        }

        public static void MensaDeactivate(string name)
        {
            Application.Current.Properties[name] = "False";
        }

        public static bool getConfig(string name)
        {
            if (Application.Current.Properties.ContainsKey(name))
            {
                string value = Application.Current.Properties[name] as string;
                return (value == "True");
            }
            Application.Current.Properties[name] = "False";
            return false;
        }

        public static void setConfig(string name, bool value)
        {
            string v;
            if (value)
            {
                v = "True";
            }
            else
            {
                v = "False";
            }
            Application.Current.Properties[name] = v;
        }

        #endregion
	}
}
