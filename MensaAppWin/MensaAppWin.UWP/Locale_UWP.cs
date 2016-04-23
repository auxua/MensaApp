using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(MensaAppWin.UWP.Locale_UWP))]

namespace MensaAppWin.UWP
{
    public class Locale_UWP : MensaAppWin.ILocale
    {
        /// <remarks>
        /// Not sure if we can cache this info rather than querying every time
        /// </remarks>
        public string GetCurrent()
        {
            var culture = CultureInfo.CurrentCulture.Name;
            var lang = CultureInfo.CurrentUICulture.Name;

            //var lang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            //var culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            return lang;
        }
    }
}
