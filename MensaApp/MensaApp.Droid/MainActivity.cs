using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace MensaApp.Droid
{
	[Activity (Label = "Mensa Aachen", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);

            // Init screen values
            var metrics = Resources.DisplayMetrics;
            DisplayWidth = ConvertPixelsToDp(metrics.WidthPixels);
            DisplayHeight = ConvertPixelsToDp(metrics.HeightPixels);

            LoadApplication (new MensaApp.App ());
		}

        public static int DisplayHeight { get; set; }
        public static int DisplayWidth { get; set; }
        
        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }

    }
}

