using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using System.Globalization;

#if WINDOWS_UWP

using Windows.Gaming.Input;
using Windows.System;

#endif

namespace MensaAppWin
{
    public static class DateExtensions
    {
        public static string ToShortDateFormatString(this DateTime dt)
        {
            var culture = new CultureInfo(Localization.Locale());
            var day = culture.DateTimeFormat.GetDayName(dt.Date.DayOfWeek);
            var text = day + "\n" + dt.Date.Day + "." + dt.Date.Month + "." + dt.Date.Year;
            return text;
        }
    }

    public interface IGamePadSupport
    {
        void ButtonTrigger(VirtualKey button);
    }

    public class App : Application
	{
        public readonly static string Version = "1.7.2";

        public static bool isXbox = false;

        //public static void GamepadButton(string button)
        public static void GamepadButton(VirtualKey button)
        {
            if (App.Current.MainPage is NavigationPage)
            {

                var innerPage = ((NavigationPage)App.Current.MainPage).CurrentPage;
                if (innerPage is IGamePadSupport)
                {
                    ((IGamePadSupport)innerPage).ButtonTrigger(button);
                }

                /*if (innerPage is MensaPage)
                    ((MensaPage)innerPage).ButtonTrigger(button);
                else if (innerPage is ConfigPage)
                    ((ConfigPage)innerPage).ButtonTrigger(button);*/
            }
        }
        public App ()
		{
			// The root page of your application
            MainPage = new NavigationPage(new MensaPage());
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
