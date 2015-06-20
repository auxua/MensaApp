using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MensaApp.pages
{
	public partial class APage : ContentPage
	{
		public APage ()
		{
			InitializeComponent ();

            //MensaAdapter.CatchMensaDataAsync();
            string test = Localization.Localize("TestText");
		}
	}
}
