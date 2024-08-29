using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gestures;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using Google.Android.Material.AppBar;

using sRadio.DataModel;

namespace sRadio
{
    public class SettingsFragment : AndroidX.Fragment.App.Fragment
    {
      
        string [] radioDirectoryOptions = new string[] { "SHOUTcast", "Directory 2", "Directory 3" };
        
        int clickCount = 0;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.settings_fragment, container, false);
        }


        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            view.Clickable = true;
            var ma = (MainActivity)Activity;
            var buttonBack = (ImageButton)view.FindViewById(Resource.Id.settings_back_button);
            var stationDirectory = (LinearLayout)view.FindViewById(Resource.Id.station_directory_layout);
            var stationDirectoryText = (TextView)view.FindViewById(Resource.Id.settings_station_directory_text);
            var help = (LinearLayout)view.FindViewById(Resource.Id.settings_help_layout);
            var about = (LinearLayout)view.FindViewById(Resource.Id.settings_about_layout);

            stationDirectory.Click += async (sender, e) =>
            {
                
                
                var dlg = new MediaUtils();
                var result = await dlg.DisplayListDialog(view.Context, "Select a radio directory", radioDirectoryOptions);

                switch(result)
                {
                    case 0:
                        {
                            stationDirectoryText.Text = radioDirectoryOptions[0];
                            ma.SetRadioDirectoryFromFragement(0);
                            break;
                        }
                    case 1:
                        {
                            stationDirectoryText.Text = radioDirectoryOptions[1];
                            ma.SetRadioDirectoryFromFragement(1);
                            break;
                        }
                    case 2:
                        {
                            stationDirectoryText.Text = radioDirectoryOptions[2];
                            ma.SetRadioDirectoryFromFragement(2);
                            break;
                        }

                }
               

            };

            help.Click += (sender, e) =>
            {
                /* Create the Intent */
                var intent = new Intent(Intent.ActionSendto);
                intent.SetData(Android.Net.Uri.Parse("mailto:")); // only email apps should handle this
                intent.PutExtra(Intent.ExtraEmail, new String[] { "sradio_app@hotmail.com" });
                intent.PutExtra(Intent.ExtraSubject, "App feedback");
                StartActivity(intent);
            };
            
            buttonBack.Click += (sender, e) =>
            {
                CloseFragement();
            };

            about.Click += (sender, e) =>
            {
                if (clickCount == 15)
                {
                    Toast.MakeText(Context, "Settings Restored", ToastLength.Long).Show();
                    ma.RestoreDiegosStations();
                    clickCount++;
                }
                else { clickCount++; }
            };

            RestoreSettings(view);

        }


        public void CloseFragement()
        {
            var ma = (MainActivity)this.Activity;
            var imm = (InputMethodManager)ma.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(this.View.WindowToken, 0);

           
            Activity.SupportFragmentManager.BeginTransaction()
            .SetCustomAnimations(Resource.Animation.enter_from_right, Resource.Animation.exit_to_right, Resource.Animation.enter_from_right, Resource.Animation.abc_slide_out_bottom)
            .Remove(this)
            .Commit();
        }
    
        public void RestoreSettings(View view)
        {
            var ma = (MainActivity)Activity;
            var stationDirectoryText = (TextView)view.FindViewById(Resource.Id.settings_station_directory_text);

            if (MainActivity.radioDirectory == 0)
                stationDirectoryText.Text = radioDirectoryOptions[0];
            if (MainActivity.radioDirectory == 1)
                stationDirectoryText.Text = radioDirectoryOptions[1];
            if (MainActivity.radioDirectory == 2)
                stationDirectoryText.Text = radioDirectoryOptions[2];

        }

    }
}