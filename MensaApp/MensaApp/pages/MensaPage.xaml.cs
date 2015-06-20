﻿using System;
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
            var vm = new ViewModels.MensaPageViewModel("Ahornstrasse", new DateTime(2015, 06, 16));
            this.BindingContext = vm;
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
            Navigation.PushAsync(new pages.APage());
        }
	}
}