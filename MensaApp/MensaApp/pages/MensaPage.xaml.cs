using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MensaApp.pages
{
	public partial class MensaPage : ContentPage
	{
		public MensaPage ()
		{
			InitializeComponent ();
            this.BindingContext = new ViewModels.MensaPageViewModel("Academica", new DateTime(2015, 06, 16));
            
		}

        void ToolBarTodayClicked(object sender, EventArgs e)
        {
            ViewModels.MensaPageViewModel vm = (ViewModels.MensaPageViewModel)this.BindingContext;
            // do sth.
        }
	}
}
