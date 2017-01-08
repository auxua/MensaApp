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
        ViewModels.MensaPageViewModel vm;
        
        public MensaPage ()
		{
			InitializeComponent ();
#if __ANDROID__
            // This is a special Workaround for CM devices
            // CM brings different fonts and sizes. This workaround prevents the mensa names from breaking
            // It is not a good fix, but its working for now.

            Label mlabel = this.FindByName<Label>("MensaLabel");
            StackLayout mStack = this.FindByName<StackLayout>("MensaStack");
            //mlabel.FontSize = 20;

            var height = MensaApp.Droid.MainActivity.DisplayHeight;
            var width = MensaApp.Droid.MainActivity.DisplayWidth;

            var size = mlabel.FontSize;

            //Device.BeginInvokeOnMainThread(()=> this.DisplayAlert("screen", "sizes: " + height + "," + width, "OK"));

            //if (size > 26) mlabel.FontSize -= 4;

            mlabel.HorizontalOptions = LayoutOptions.Fill;
            mStack.Spacing = 4;
            mlabel.FontSize--;

#endif

            string mensa = MensaAdapter.getNextMensaName("Forum Cafete");
            vm = new ViewModels.MensaPageViewModel(mensa, DateTime.Now.Date);
            this.BindingContext = vm;

            //vm.GetNextDayCommand.Execute(null);
            /*
            // previous Day
            var prevGesture = new TapGestureRecognizer();
            prevGesture.Tapped += (s,e) => vm.GetPrevDayCommand.Execute(null);
            this.prevDayLabel.GestureRecognizers.Add(prevGesture);
            // next Day
            var nextGesture = new TapGestureRecognizer();
            nextGesture.Tapped += (s, e) => vm.GetNextDayCommand.Execute(null);
            this.nextDayLabel.GestureRecognizers.Add(nextGesture);
            // Previous Mensa
            var prevMensaGesture = new TapGestureRecognizer();
            prevMensaGesture.Tapped += (s, e) => vm.GetPrevMensaCommand.Execute(null);
            this.PrevMensaLabel.GestureRecognizers.Add(prevMensaGesture);
            // Next Mensa
            var nextMensaGesture = new TapGestureRecognizer();
            nextMensaGesture.Tapped += (s, e) => vm.GetNextMensaCommand.Execute(null);
            this.NextMensaLabel.GestureRecognizers.Add(nextMensaGesture);
             */
            this.PrevMensaButton.Clicked += (s, e) => vm.GetPrevMensaCommand.Execute(null);
            this.NextMensaButton.Clicked += (s, e) => vm.GetNextMensaCommand.Execute(null);
            this.NextDayButton.Clicked += (s, e) => vm.GetNextDayCommand.Execute(null);
            this.PrevDayButton.Clicked += (s, e) => vm.GetPrevDayCommand.Execute(null);

            
        }

        void ConfigClicked(object sender, EventArgs e)
        {
            /*ViewModels.MensaPageViewModel vm = (ViewModels.MensaPageViewModel)this.BindingContext;
            vm.GetNextDayCommand.Execute(null);*/
            // do sth.
            Navigation.PushAsync(new pages.ConfigPage(vm));
        }

	}
}
