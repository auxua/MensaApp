using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(MensaApp.WinPhone.Locale_WinPhone))]

namespace MensaApp.WinPhone
{
    public class Locale_WinPhone : MensaApp.ILocale
    {
        /// <remarks>
        /// Not sure if we can cache this info rather than querying every time
        /// </remarks>
        public string GetCurrent()
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            return lang;
        }
    }
}
